using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RemoteProvisioningProjectExtension
{[Export(typeof(ISharePointProjectExtension))]
    internal class ProjectExtension : ISharePointProjectExtension
    {
        public void Initialize(ISharePointProjectService projectService)
        {
            projectService.ProjectMenuItemsRequested += projectService_ProjectMenuItemsRequested;

        }

        void projectService_ProjectMenuItemsRequested(object sender, SharePointProjectMenuItemsRequestedEventArgs e)
        {

            IMenuItem showSPIInfoMenuItem = e.ActionMenuItems.Add("Enable Remote Provisioning");
            showSPIInfoMenuItem.Click += showSPIInfoMenuItem_Click;
        }

        void showSPIInfoMenuItem_Click(object sender, MenuItemEventArgs e)
        {
            //here check if the web app is generated or not
            ISharePointProject project = (ISharePointProject)e.Owner;
            //only if there is no previous link added(fresh app)
            if (project.AppSettings.InstalledEventEndpoint == null && project.AppSettings.WebProjectPath == null)
            {
                string webProjectName = project.Name + "Web";
                DTE dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as DTE;
                Solution2 solution = (Solution2)dte.Solution;
                string tmp = solution.GetProjectTemplate("RemoteProvisioningEventReceiver.zip", "CSharp");
                //string tmp = solution.GetProjectTemplate("RemoteProvisioning.zip", "CSharp");
                string solutionFolder = solution.FullName.Substring(0, solution.FullName.LastIndexOf("\\"));
                Project web = solution.AddFromTemplate(tmp, solutionFolder + "\\" + webProjectName, webProjectName, false);
                //beacuse web always return with null value !!!
                bool success = false;
                foreach (Project item in solution.Projects)
                {
                    if (item.FullName == solutionFolder + "\\" + webProjectName + "\\" + webProjectName + ".csproj")
                    {
                        success = true;
                        web = item;
                        break;
                    }
                }
                if (success)
                {
                    project.AppSettings.LinkToWebProject(web.FullName, false, false);
                    project.AppSettings.InstalledEventEndpoint = "~remoteAppUrl/Services/AppEventReceiver.svc";
                    project.AppSettings.UninstallingEventEndpoint = "~remoteAppUrl/Services/AppEventReceiver.svc";
                    //add app permission and remote webapplication node to xml
                    XmlDocument appManifest = new XmlDocument();
                    appManifest.Load(project.AppSettings.AppManifest.FullPath);
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(appManifest.NameTable);
                    nsmgr.AddNamespace("appManifest", "http://schemas.microsoft.com/sharepoint/2012/app/manifest");
                    XmlNode appPrinciple = appManifest.ChildNodes[2].ChildNodes[1];
                    appPrinciple.InnerXml = " <RemoteWebApplication ClientId=\"*\"/>";
                    XmlNode app = appManifest.ChildNodes[2];
                    app.InnerXml+=@"<AppPermissionRequests><AppPermissionRequest Scope=""http://sharepoint/content/sitecollection"" Right=""FullControl"" /></AppPermissionRequests>";
                    appManifest.Save(project.AppSettings.AppManifest.FullPath);
                }
            }
        }



    }
}
