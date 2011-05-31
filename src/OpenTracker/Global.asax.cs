using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace OpenTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("elmah.axd");

            routes.MapRoute(
                "errors", // Route name
                "errors", // URL with parameters
                new { controller = "Browse", action = "Errors" } // Parameter defaults
            );


            // 
            // EXAMPLE: /announce
            // EXAMPLE: /announce/252af0c913275t7d4711654b2293895f
            //
            routes.MapRoute(
                "Announce", // Route name
                "announce/{passkey}", // URL with parameters
                new { controller = "Announce", action = "Announce", passkey = UrlParameter.Optional } // Parameter defaults
            );


            routes.MapRoute(
                "DownloadTorrent", // Route name
                "download/{torrentid}", // URL with parameters
                new { controller = "Tracker", action = "DownloadTorrent", torrentid = @"\d" } // Parameter defaults
            );


            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }


        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == string.Empty)
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            // retrieve roles from UserData
            var roles = authTicket.UserData.Split(';');
            if (Context.User != null)
                Context.User = new GenericPrincipal(Context.User.Identity, roles);
        }

    }


}