using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace $safeprojectname$.ProvisioningHelper
{
    public abstract class BaseManager
    {
        public virtual void Process(ClientContext context, bool add, string path)
        {

        }
    }
}