using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using $safeprojectname$.ProvisioningHelper;
using System.Web.Hosting;
using System.Xml.XPath;

namespace $safeprojectname$.ProvisioningHelper
{
    public class SequenceProcessor
    {
        public void Process(ClientContext context, bool add, string path)
        {
            XmlDocument seq = new XmlDocument();
            seq.Load(HostingEnvironment.MapPath("~/" + path));
            XPathNavigator nav = seq.CreateNavigator();
            XPathExpression exp = nav.Compile(@"/Sequence/Item");
            exp.AddSort("@Order", add ? XmlSortOrder.Ascending : XmlSortOrder.Descending, XmlCaseOrder.None, string.Empty, XmlDataType.Number);
            //XmlNodeList items = seq.SelectNodes("/Sequence/Item");
            foreach (XPathNavigator navItem in nav.Select(exp))
            {
                XmlNode item = (XmlNode)navItem.UnderlyingObject;
                BaseManager itemManager;
                switch ((Constants.RemoteProvisioningType)Enum.Parse(typeof(Constants.RemoteProvisioningType), item.Attributes[Constants.SequenceItemAttributes.Type].Value))
                {
                    case Constants.RemoteProvisioningType.List:
                        itemManager = new ListManager();
                        break;
                    case Constants.RemoteProvisioningType.File:
                        itemManager = new FileManager();
                        break;
                    case Constants.RemoteProvisioningType.CustomAction:
                        itemManager = new CustomActionsManager();
                      break;
                    case Constants.RemoteProvisioningType.Page:
                        itemManager= new PageManager();
                        break;
                    case Constants.RemoteProvisioningType.View:
                        itemManager = new ViewManager();
                        break;
                    case Constants.RemoteProvisioningType.Theme:
                        itemManager = new ThemeManager();
                        break;
                    default:
                        throw new Exception("Not Supported Remote Provisioning Type");
                }
                //process the item according to the type
                itemManager.Process(context, add, HostingEnvironment.MapPath(item.Attributes[Constants.SequenceItemAttributes.Path].Value));
            }

        }


    }
}