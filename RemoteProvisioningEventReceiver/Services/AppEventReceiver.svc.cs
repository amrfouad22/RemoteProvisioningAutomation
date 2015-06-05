using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using $safeprojectname$.ProvisioningHelper;

namespace $safeprojectname$.Services
{
     public class AppEventReceiver : IRemoteEventService
    {
        #region events
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();
            System.Diagnostics.Trace.TraceInformation("Remote Event Reciever Starts here....");
            System.Diagnostics.Trace.TraceInformation(properties.EventType.ToString());
            try
            {
                switch (properties.EventType)
                {
                    case SPRemoteEventType.AppInstalled:
                        AppInstall(properties);
                        break;
                    case SPRemoteEventType.AppUninstalling:
                        System.Diagnostics.Trace.TraceInformation("uninstall app starts here.....");
                        AppUninstall(properties);
                        break;
                }
                return result;

            }
            catch (Exception ex)
            {
                //do nothing
                System.Diagnostics.Trace.TraceError(ex.Message);
                System.Diagnostics.Trace.TraceError(ex.StackTrace);
                result.ErrorMessage = ex.Message;
                result.Status = properties.EventType.Equals(SPRemoteEventType.AppInstalled) ? SPRemoteEventServiceStatus.CancelWithError : SPRemoteEventServiceStatus.Continue;
                return result;
            }
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            // This method is not used by app events
        }

        #endregion

        #region install methods
        /// <summary>
        /// App installation
        /// </summary>
        /// <param name="properties"></param>
        private void AppInstall(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    SequenceProcessor processor = new SequenceProcessor();
                    processor.Process(clientContext,true,"ProvisioningSequence.xml");
                }
            }
        }

        #endregion

        #region uninstall functions
        private void AppUninstall(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    SequenceProcessor processor = new SequenceProcessor();
                    processor.Process(clientContext, false, "ProvisioningSequence.xml");
                }
            }
        }
        #endregion
    }
}
