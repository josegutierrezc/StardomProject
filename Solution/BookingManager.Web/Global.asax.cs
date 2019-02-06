using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BookingManager.Web.Helpers;

namespace BookingManager.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (Context.User != null)
            {
                string[] privileges = ApplicationHelper.Instance.GetTagValueFromIdentity(Context.User.Identity, ApplicationHelper.UserPrivileges).Split(';');
                GenericPrincipal userPrincipal = new GenericPrincipal(Context.User.Identity, privileges);
                Context.User = userPrincipal;
            }
        }
    }
}
