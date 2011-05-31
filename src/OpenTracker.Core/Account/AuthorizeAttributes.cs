using System.Web;
using System.Web.Mvc;

namespace OpenTracker.Core.Account
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public string Message { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return;

            HttpContext.Current.Response.Redirect("/Account/Login?returnUrl=" + HttpContext.Current.Request.Path);
            return;
        }
    }


    public class AuthorizePowerUserAttribute : AuthorizeAttribute
    {
        public string Message { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return;

            /*
            var result = new ViewResult
                             {
                                 ViewName = "/Views/Account/Login.cshtml" // this can be a property
                                 // MasterName = "_Layout.cshtml" // this can also be a property
                             };
            result.ViewBag.Message = this.Message;
            filterContext.Result = result;
            */
            HttpContext.Current.Response.Redirect("/Account/Login");
            return;
        }
    }


}
