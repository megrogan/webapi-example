using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http.Dispatcher;
using Castle.Windsor;
using TravelRepublic.IoC;

namespace TravelRepublic.Flights.Web
{
    public class Global : System.Web.HttpApplication
    {
        private WebApiSetup webApiSetup;

        protected void Application_Start(object sender, EventArgs e)
        {
            this.webApiSetup = new WebApiSetup(GlobalConfiguration.Configuration, "bin");
        }

        public override void Dispose()
        {
            if (this.webApiSetup != null)
            {
                this.webApiSetup.Dispose();
            }

            base.Dispose();
        }
    }
}