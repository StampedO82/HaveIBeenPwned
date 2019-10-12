using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using EmailWebApi.Configuration;
using GrainInterfaces;
using Orleans;
using Orleans.Configuration;

namespace EmailWebApi
{
    public class Global : System.Web.HttpApplication
    { 
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(EmailWebAPIConfig.Register);
            var t = EmailWebAPIConfig.ConnectToSiloAsync();
            if (t.Result != "Connected")
                throw new Exception(t.Result);

        }

        protected void Session_Start(object sender, EventArgs e)
        {
           
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //this.
            //Response.Redirect("~/Views/Shared/_Layout.cshtml");
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}