using System.Linq;
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

            HttpContext.Current.Response.Redirect("/account/login?returnUrl=" + HttpContext.Current.Request.Path);
            return;
        }
    }


    public class AuthorizePowerUserAttribute : AuthorizeAttribute
    {
        public string Message { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                using (var context = new OpenTrackerDbContext())
                {
                    var retrieveTempUser = (from u in context.users
                                            where Account.Class >= (decimal)AccountValidation.Class.PowerUser
                                            && u.id == Account.UserId
                                            select u).Take(1).FirstOrDefault();
                    if (retrieveTempUser != null)
                        return;
                }
            }

            HttpContext.Current.Response.Redirect("/account/login?returnUrl=" + HttpContext.Current.Request.Path);
            return;
        }
    }

    public class AuthorizeUploaderAttribute : AuthorizeAttribute
    {
        public string Message { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                using (var context = new OpenTrackerDbContext())
                {
                    var retrieveTempUser = (from u in context.users
                                            where Account.Class >= (decimal) AccountValidation.Class.Uploader
                                            && u.id == Account.UserId
                                            select u).Take(1).FirstOrDefault();
                    if (retrieveTempUser != null)
                        return;
                }
            }
            
            HttpContext.Current.Response.Redirect("/account/login?returnUrl=" + HttpContext.Current.Request.Path);
            return;
        }
    }
}
