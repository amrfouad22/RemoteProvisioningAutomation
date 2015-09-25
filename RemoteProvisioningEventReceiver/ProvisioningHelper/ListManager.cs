using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace $safeprojectname$.ProvisioningHelper
{
    public class ListManager:BaseManager 
    {
        /// <summary>
        /// constructor with no parameter, this will keep the default list definiton folder as defined int the variable region
        /// </summary>
        public ListManager()
        {
        }
       
        public override void Process(ClientContext context,bool add, string def)
        {
            if (add)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(System.IO.File.ReadAllText(def));
                XmlNode list = doc.SelectSingleNode("/List");
                ListCreationInformation info = new ListCreationInformation();
                info.Title = list.Attributes["Title"].Value;
                info.Url = list.Attributes["Url"].Value;
                info.TemplateType = int.Parse(list.Attributes["BaseType"].Value);
                System.Diagnostics.Trace.TraceInformation("Creating List Name:" + info.Title);
                List l = context.Web.Lists.Add(info);
                context.Load(l);
                context.ExecuteQuery();
                XmlNodeList fields = doc.SelectNodes("/List/Fields/Field");
                foreach (XmlNode field in fields)
                {
                    string xml = field.OuterXml;
                    //change the list name to list Id
                    if (field.Attributes["Type"].Value.Contains("Lookup"))
                    {
                        List parent = context.Site.RootWeb.Lists.GetByTitle(field.Attributes["List"].Value);
                        context.Load(parent, p => p.Id);
                        context.ExecuteQuery();
                        xml = xml.Replace("List=\"" + field.Attributes["List"].Value + "\"", "List=\"{" + parent.Id + "}\"");
                        xml = xml.Replace("~sitecollection", "{" + context.Web.Id.ToString() + "}");
                    }
                    Field f = l.Fields.AddFieldAsXml(xml, true, AddFieldOptions.AddFieldToDefaultView);
                    context.Load(f);
                }
                //ensure list creation before adding items
                context.ExecuteQuery();
                //adding the list items if exists
                XmlNodeList items = doc.SelectNodes("/List/Items/Item");
                foreach (XmlNode item in items)
                {
                    ListItem i = l.AddItem(new ListItemCreationInformation());
                    foreach (XmlAttribute att in item.Attributes)
                    {
                        if (att.Value.Contains("Lookup:"))
                        {
                            i[att.Name] = new FieldLookupValue() { LookupId = int.Parse(att.Value.Replace("Lookup:", "")) };
                        }
                        else
                        {
                            i[att.Name] = att.Value;
                        }
                    }
                    i.Update();
                }
                context.ExecuteQuery();
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(System.IO.File.ReadAllText(def));
                XmlNode list = doc.SelectSingleNode("/List");
                List l = context.Web.Lists.GetByTitle(list.Attributes["Title"].Value);
                l.DeleteObject();
                context.ExecuteQuery();

            }
        }
    }
}