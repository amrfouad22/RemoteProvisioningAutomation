using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace $safeprojectname$.ProvisioningHelper
{
    public class ThemeManager:BaseManager
    {
        public ThemeManager()
        {

        }
        public override void Process(Microsoft.SharePoint.Client.ClientContext context, bool add, string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(path));
            XmlNodeList themes = doc.SelectNodes("Themes/Theme");
            //upload the spcolor file
            string themeName = string.Empty;
            string fontName = string.Empty;
            if (add)
            {
                //load the server relative url 
                var web = context.Web;
                context.Load(web, w=>w.ServerRelativeUrl);
                context.ExecuteQuery();
                foreach (XmlNode theme in themes)
                {
                    //upload the spcolor file
                    uploadThemeFile(context, "Theme Gallery", "15", theme.Attributes[Constants.ThemeAttributeNames.SPColorFile].Value, add,out themeName);
                    //upload the font file
                    uploadThemeFile(context, "Theme Gallery", "15", theme.Attributes[Constants.ThemeAttributeNames.FontFile].Value, add,out fontName);
                    //add the composed look item
                    List list = context.Web.Lists.GetByTitle("Composed Looks");
                    ListItem composedlook=list.AddItem(new ListItemCreationInformation());
                    composedlook["Title"] = theme.Attributes[Constants.ThemeAttributeNames.Title].Value;
                    composedlook["Name"] = theme.Attributes[Constants.ThemeAttributeNames.Name].Value;
                    composedlook["DisplayOrder"] = theme.Attributes[Constants.ThemeAttributeNames.DisplayOrder].Value;
                    composedlook["MasterPageUrl"] = web.ServerRelativeUrl + theme.Attributes[Constants.ThemeAttributeNames.MasterPageUrl].Value;
                    composedlook["ThemeUrl"] = web.ServerRelativeUrl+"/_catalogs/theme/15/"+themeName;
                    composedlook["ImageUrl"] = theme.Attributes[Constants.ThemeAttributeNames.ImageUrl].Value;
                    composedlook["FontSchemeUrl"] = fontName == string.Empty ? string.Empty : web.ServerRelativeUrl + "/_catalogs/theme/15/" + fontName;
                    composedlook.Update();
                    context.ExecuteQuery();
                }
            }
            else
            {
                foreach (XmlNode theme in themes)
                {
                  
                    uploadThemeFile(context, "Theme Gallery", "15", theme.Attributes[Constants.ThemeAttributeNames.SPColorFile].Value, add,out themeName);
                    uploadThemeFile(context, "Theme Gallery", "15", theme.Attributes[Constants.ThemeAttributeNames.FontFile].Value, add,out fontName);
                    //delete the composed look item
                    List list = context.Web.Lists.GetByTitle("Composed Looks");
                    ListItemCollection items = list.GetItems(new CamlQuery { ViewXml = string.Format("<View><Query><Where><Eq><FieldRef Name='Name'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", theme.Attributes[Constants.ThemeAttributeNames.Name].Value) });
                    context.ExecuteQuery();
                    items[0].DeleteObject();
                    context.ExecuteQuery();
                }
            }

        }

        private void uploadThemeFile(ClientContext context,string listName, string folderUrl, string filePath,bool add,out string name)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                name = string.Empty;
                return;
            }
            string path = HostingEnvironment.MapPath(filePath);
            List list = context.Web.Lists.GetByTitle(listName);
            Folder dest = list.RootFolder.Folders.GetByUrl(folderUrl);
            if (add)
            {
                FileCreationInformation fci = new FileCreationInformation();
                name = path.Substring(path.LastIndexOf("\\") + 1);
                fci.Url = name;
                fci.Content = System.IO.File.ReadAllBytes(path);
                fci.Overwrite = true;
                Microsoft.SharePoint.Client.File fileToUpload = dest.Files.Add(fci);
                context.Load(fileToUpload);
                context.ExecuteQuery();
                //fileToUpload.CheckIn(string.Empty, CheckinType.MajorCheckIn);
                //fileToUpload.Publish(string.Empty);
            }
            else
            {
                //delete the file
                name = path.Substring(path.LastIndexOf("\\") + 1);
                dest.Files.GetByUrl(name).DeleteObject();
                context.ExecuteQuery();
            }
        }

      
    }
}