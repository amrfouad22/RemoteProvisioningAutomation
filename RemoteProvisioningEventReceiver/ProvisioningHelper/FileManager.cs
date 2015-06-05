using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Hosting;

namespace  $safeprojectname$.ProvisioningHelper
{
    public class FileManager
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="clientContext"></param>
       /// <param name="dest"></param>
       /// <param name="path"></param>
       /// <param name="filter"></param>
       /// <param name="recursive"></param>
        private void CopyFolder(ClientContext clientContext, Folder dest, string path,string filter,bool recursive,bool checkin , bool publish)
        {
            string[] files = Directory.GetFiles(path, filter, SearchOption.TopDirectoryOnly);

            foreach (string f in files)
            {
                //upload the file to the sharepoint folder
                FileCreationInformation fci = new FileCreationInformation();
                string name = f.Substring(f.LastIndexOf("\\") + 1);
                fci.Url = name;
                fci.Content = System.IO.File.ReadAllBytes(f);
                fci.Overwrite = true;
                Microsoft.SharePoint.Client.File fileToUpload = dest.Files.Add(fci);
                //if it's the page layout
                if (name.EndsWith(".aspx"))
                {
                    fileToUpload.ListItemAllFields["Title"] = name.Replace(".aspx", string.Empty);
                    fileToUpload.ListItemAllFields["UIVersion"] = "15";
                    fileToUpload.ListItemAllFields["ContentTypeId"] = "0x01010007FF3E057FA8AB4AA42FCB67B453FFC100E214EEE741181F4E9F7ACC43278EE81100B432574477BA904292DFD58D26CE0E2";
                    fileToUpload.ListItemAllFields["PublishingAssociatedContentType"] = ";#Article Page;#0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB064584E219954237AF3900242457EFB8B24247815D688C526CD44D;#";
                    fileToUpload.ListItemAllFields.Update();                  
                }
                else if (name.EndsWith(".master"))//master page assuming only single custom master page exists
                {
                    //get the name of the master page 
                    string masterPageFileName=name.Substring(0, name.LastIndexOf("."));
                    fileToUpload.ListItemAllFields["Title"] = masterPageFileName;
                    fileToUpload.ListItemAllFields["UIVersion"] = "15";
                    fileToUpload.ListItemAllFields["ContentTypeId"] = "0x01010500BF544AFE46ACEF42B8DA22C9CE89526E";
                    fileToUpload.ListItemAllFields.Update();                    
                }
                clientContext.Load(fileToUpload);
                clientContext.ExecuteQuery();
                if (checkin)
                {
                    fileToUpload.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                    fileToUpload.Publish(string.Empty);
                }

            }
            if (recursive)
            {
                foreach (string d in Directory.GetDirectories(path))
                {
                    //get the folder name
                    string name = d.Substring(d.LastIndexOf("\\") + 1);
                    Folder newdest = dest.Folders.Add(name);
                    clientContext.ExecuteQuery();
                    CopyFolder(clientContext, newdest, d, filter, recursive,checkin,publish);
                }
            }
        }
        /// <summary>
        /// this function process all the xml files under the Files folder and copy the files to the host web accordingly based on the FileCopy Definition
        /// </summary>
        public  void ProcessFileCopy(ClientContext clientContext, bool add)
        {
            System.Diagnostics.Trace.WriteLine("Start Processing File Copy definitions....." );

            System.Diagnostics.Trace.WriteLine("Processing files @" + HostingEnvironment.MapPath("~/" + Constants.FILE_COPY_DEF_FOLDER));
            foreach (string copyDef in Directory.GetFiles(HostingEnvironment.MapPath("~/" + Constants.FILE_COPY_DEF_FOLDER)))
            {
                ProcessSingleFileCopy(clientContext,add,copyDef);
            }
        }

        public void ProcessSingleFileCopy(ClientContext clientContext, bool add, string copyDef)
        {
            System.Diagnostics.Trace.WriteLine("processing copy of file:" + copyDef); 
                //read the xml FileCopy Definition
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(System.IO.File.ReadAllText(copyDef));
                System.Diagnostics.Trace.WriteLine(doc.ChildNodes.Count);
                XmlNodeList fileCopyList = doc.SelectNodes("/FileCopies/FileCopy");
                foreach (XmlNode fileCopy in fileCopyList)
                {
                    //load the SharePoint.Client.Folder instance using the relative Folder Url defined in the Xml definition.
                    bool bList = fileCopy.Attributes[Constants.FileCopyAttributeNames.TaregtFolder] == null;
                    Folder dest;
                    if (bList)
                    {
                        System.Diagnostics.Trace.WriteLine("Getting list @" + fileCopy.Attributes[Constants.FileCopyAttributeNames.TaregtList].Value);
                        List list = clientContext.Web.Lists.GetByTitle(fileCopy.Attributes[Constants.FileCopyAttributeNames.TaregtList].Value);
                        clientContext.Load(list, l => l.RootFolder);
                        clientContext.ExecuteQuery();
                        dest = list.RootFolder;
                    }
                    else 
                        dest = clientContext.Web.Folders.GetByUrl(fileCopy.Attributes[Constants.FileCopyAttributeNames.TaregtFolder].Value);
                    clientContext.Load(dest,d=>d.ServerRelativeUrl);                    
                    clientContext.ExecuteQuery();
                    Folder target=dest.Folders.Add(fileCopy.Attributes[Constants.FileCopyAttributeNames.LocalFolder].Value);
                    List parentList = clientContext.Web.GetList(dest.ServerRelativeUrl);
                    clientContext.Load(target, t=>t.ServerRelativeUrl);
                    clientContext.Load(parentList);
                    clientContext.ExecuteQuery();
                    if (add)
                    {
                        CopyFolder(clientContext,target , HostingEnvironment.ApplicationPhysicalPath + "/" + fileCopy.Attributes[Constants.FileCopyAttributeNames.LocalFolder].Value, fileCopy.Attributes[Constants.FileCopyAttributeNames.Filter].Value, fileCopy.Attributes[Constants.FileCopyAttributeNames.Recursive].Value == "TRUE",parentList.EnableMinorVersions,parentList.EnableVersioning);
                    }
                    else 
                    {
                        System.Diagnostics.Trace.WriteLine("Deleting Folder:"+target.ServerRelativeUrl);
                        target.DeleteObject();
                        clientContext.ExecuteQuery();
                    }
                }
        }

    }
}