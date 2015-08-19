using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using  $safeprojectname$.ProvisioningHelper;
using System.Web.Hosting;

namespace  $safeprojectname$.ProvisioningHelper
{
    public class SequenceProcessor
    {
        public void Process(ClientContext context,bool add,string path)
        {
            XmlDocument seq = new XmlDocument();
            seq.Load(HostingEnvironment.MapPath("~/"+path));
            foreach (XmlNode item in seq.SelectNodes("/Sequence/Item"))
            {
                switch((Constants.RemoteProvisioningType)Enum.Parse(typeof(Constants.RemoteProvisioningType),item.Attributes[Constants.SequenceItemAttributes.Type].Value))
                {
                    case Constants.RemoteProvisioningType.List:
                        //if it's a list hand it to the list Processor
                        ListManager list = new ListManager();
                        if (add)
                            list.CreateList(context, HostingEnvironment.MapPath(item.Attributes[Constants.SequenceItemAttributes.Path].Value));
                        else
                            list.RemoveList(context, HostingEnvironment.MapPath(item.Attributes[Constants.SequenceItemAttributes.Path].Value));
                        break;
                    case Constants.RemoteProvisioningType.File:
                        FileManager file = new FileManager();
                        file.ProcessSingleFileCopy(context, add, HostingEnvironment.MapPath(item.Attributes[Constants.SequenceItemAttributes.Path].Value));
                        break;
                    case Constants.RemoteProvisioningType.CustomAction:
                        CustomActionsManager actions = new CustomActionsManager();
                        actions.ProcessSingleAction(context, add, HostingEnvironment.MapPath(item.Attributes[Constants.SequenceItemAttributes.Path].Value));
                        break;
                    case Constants.RemoteProvisioningType.Page:
                        PageManager page = new PageManager();
                        page.ProcessPageCreation(context, add, HostingEnvironment.MapPath(item.Attributes[Constants.SequenceItemAttributes.Path].Value));
                        break;
                    case Constants.RemoteProvisioningType.View:
                        ViewManager view = new ViewManager();
                        view.ProcessListView(context, add, HostingEnvironment.MapPath(item.Attributes[Constants.SequenceItemAttributes.Path].Value));
                        break;
                    default:
                        throw new Exception("Not Supported Remote Provisioning Type");
                }
            }

        }

      
    }
}