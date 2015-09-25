using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml;

namespace $safeprojectname$.ProvisioningHelper
{
    public class CustomActionsManager:BaseManager
    {
        public override void Process(ClientContext context, bool add, string customActionDef)
        {
            context.Load(context.Web, w => w.ServerRelativeUrl, w => w.Url);
            context.ExecuteQuery();
            System.Diagnostics.Trace.WriteLine("processing custom action definition:" + customActionDef);
            //read the xml FileCopy Definition
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(customActionDef));
            XmlNodeList customActions = doc.SelectNodes("/CustomAction");
            foreach (XmlNode customAction in customActions)
            {
                //read the action definition
                string jsLink = customAction.Attributes[Constants.CustomActionAttributes.ScriptSrc] == null ? string.Empty : customAction.Attributes[Constants.CustomActionAttributes.ScriptSrc].Value.Replace("~siteCollection", context.Web.Url);
                System.Diagnostics.Trace.WriteLine("Script link value:" + jsLink);
                string location = customAction.Attributes[Constants.CustomActionAttributes.Location] == null ? string.Empty : customAction.Attributes[Constants.CustomActionAttributes.Location].Value;
                string name = customAction.Attributes[Constants.CustomActionAttributes.Name] == null ? string.Empty : customAction.Attributes[Constants.CustomActionAttributes.Name].Value;
                string description = customAction.Attributes[Constants.CustomActionAttributes.Description] == null ? string.Empty : customAction.Attributes[Constants.CustomActionAttributes.Description].Value;
                int sequence = customAction.Attributes[Constants.CustomActionAttributes.Sequence] == null ? 100 : int.Parse(customAction.Attributes[Constants.CustomActionAttributes.Sequence].Value);
                var actions = context.Web.UserCustomActions;
                context.Load(actions);
                context.ExecuteQuery();
                if (add)
                {
                    StringBuilder scripts = new StringBuilder(@"var headID = document.getElementsByTagName('head')[0]; var");
                    scripts.AppendFormat(@" newScript = document.createElement('script');newScript.type = 'text/javascript';newScript.src = '{0}';headID.appendChild(newScript);", jsLink);
                    string scriptBlock = scripts.ToString();
                    foreach (var action in actions.ToArray())
                    {
                        if (action.Name == name && action.Location == location)
                        {
                            action.DeleteObject();
                            context.ExecuteQuery();
                        }
                    }
                    var newAction = actions.Add();
                    newAction.Name = name;
                    newAction.Description = description;
                    newAction.Location = location;
                    newAction.ScriptBlock = scriptBlock;
                    newAction.Update();
                    context.Load(context.Web, s => s.UserCustomActions);
                    context.ExecuteQuery();
                }
                else //remove the user custom actions
                {
                    foreach (var action in actions.ToArray())
                    {
                        System.Diagnostics.Trace.WriteLine("Comparing action :" + action.Name);
                        if (action.Name == customAction.Attributes[Constants.CustomActionAttributes.Name].Value && action.Location == customAction.Attributes[Constants.CustomActionAttributes.Location].Value)
                        {
                            System.Diagnostics.Trace.WriteLine("Deleting action..." + action.Name);
                            action.DeleteObject();
                            context.ExecuteQuery();
                        }
                    }

                }
            }
        }
    }
}