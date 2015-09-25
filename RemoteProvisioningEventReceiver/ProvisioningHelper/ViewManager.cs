using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace $safeprojectname$.ProvisioningHelper
{
    public class ViewManager:BaseManager
    {

        /// <summary>
        /// constructor with no parameter, this will keep the default Page definiton folder as defined int the variable region
        /// </summary>
        public ViewManager()
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
            XmlNodeList views = doc.SelectNodes("Views/View");
            if (add)
            {
                foreach (XmlNode view in views)
                {
                    //get the destination list name from ListName attribute
                    List dest = context.Web.Lists.GetByTitle(view.Attributes[Constants.ViewAttributeNames.ListName].Value);
                    ViewCreationInformation info = new ViewCreationInformation();
                    info.SetAsDefaultView = Boolean.Parse(view.Attributes[Constants.ViewAttributeNames.DefaultView].Value);
                    info.Title = view.Attributes[Constants.ViewAttributeNames.Title].Value;
                    info.ViewTypeKind = (ViewType)int.Parse(view.Attributes[Constants.ViewAttributeNames.ViewType].Value);
                    info.ViewFields = view.Attributes[Constants.ViewAttributeNames.ViewFields].Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    View newView = dest.Views.Add(info);
                    context.ExecuteQuery();
                    newView.Toolbar = view.Attributes[Constants.ViewAttributeNames.Toolbar].Value;
                    newView.JSLink = view.Attributes[Constants.ViewAttributeNames.JSLink].Value;
                    newView.Update();
                    context.ExecuteQuery();
                }
            }
            else
            {
                foreach (XmlNode view in views)
                {
                    //get the destination list name from ListName attribute
                    List dest = context.Web.Lists.GetByTitle(view.Attributes[Constants.ViewAttributeNames.ListName].Value);
                    ViewCollection listViews = dest.Views;
                    context.Load(listViews);
                    context.ExecuteQuery();
                    foreach (View viewItem in listViews)
                    {
                        if (viewItem.Title == view.Attributes[Constants.ViewAttributeNames.Toolbar].Value)
                            viewItem.DeleteObject();
                    }
                }
                context.ExecuteQuery();
            }
        }
    }
}