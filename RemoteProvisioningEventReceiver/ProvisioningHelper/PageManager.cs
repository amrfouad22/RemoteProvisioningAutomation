using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace $safeprojectname$.ProvisioningHelper
{
    public class PageManager:BaseManager
    {

        /// <summary>
        /// constructor with no parameter, this will keep the default Page definiton folder as defined int the variable region
        /// </summary>
        public PageManager()
        {
        }
        /// <summary>
        /// Creates Pages in the given web and Library using the passed client context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="web"></param>

        /// <summary>
        /// creates a pages using a single definition file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="def"></param>
        public override void Process(ClientContext context, bool add, string def)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(def));
            XmlNodeList pages = doc.SelectNodes("Pages/Page");
            if (add)
            {
                foreach (XmlNode page in pages)
                {
                    //get the destination list name from ListName attribute
                    List dest = context.Web.Lists.GetByTitle(page.Attributes[Constants.PageAttributeNames.ListName].Value);
                    FileCreationInformation info = new FileCreationInformation();
                    info.Content = System.IO.File.ReadAllBytes(HostingEnvironment.MapPath(page.Attributes[Constants.PageAttributeNames.ContentPath].Value));
                    info.Url = page.Attributes[Constants.PageAttributeNames.Title].Value;
                    File added = dest.RootFolder.Files.Add(info);
                    added.ListItemAllFields["ContentTypeId"] = page.Attributes[Constants.PageAttributeNames.ContentTypeId].Value;
                    added.ListItemAllFields["Title"] = page.Attributes[Constants.PageAttributeNames.Title].Value;
                }
            }
            else
            {
                foreach (XmlNode page in pages)
                {
                    //get the destination list name from ListName attribute
                    List dest = context.Web.Lists.GetByTitle(page.Attributes[Constants.PageAttributeNames.ListName].Value);
                    File file = dest.RootFolder.Files.GetByUrl(page.Attributes[Constants.PageAttributeNames.Title].Value);
                    file.DeleteObject();
                }
            }
            context.ExecuteQuery();
        }
    }
}