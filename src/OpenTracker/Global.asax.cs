using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using OpenTracker.Controllers;

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
                "Activate User", // Route name
                "account/activate/{hash}", // URL with parameters
                new { controller = "Account", action = "Activate", hash = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Login => Register post messages", // Route name
                "account/login/{message}", // URL with parameters
                new { controller = "Account", action = "Login", message = UrlParameter.Optional } // Parameter defaults
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
            var role = authTicket.UserData.Split(';');
            if (Context.User != null)
                Context.User = new GenericPrincipal(Context.User.Identity, new[] { role[0] });
        }


        protected void Application_Error()
        {
#if (!DEBUG)
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "General";
            routeData.Values["exception"] = exception;
            Response.StatusCode = 500;
            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();
                switch (Response.StatusCode)
                {
                    case 403:
                        routeData.Values["action"] = "Http403";
                        break;
                    case 404:
                        routeData.Values["action"] = "Http404";
                        break;
                }
            }

            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;
            IController errorsController = new ErrorController();
            var wrapper = new HttpContextWrapper(Context);
            var rc = new RequestContext(wrapper, routeData);
            errorsController.Execute(rc);
#endif
        }


    }


}