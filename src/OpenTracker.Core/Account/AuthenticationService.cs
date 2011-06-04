using System;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace OpenTracker.Core.Account
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie, int userid);
        void SignOut();
    }

    /// <summary>
    /// 
    /// </summary>
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="createPersistentCookie"></param>
        /// <param name="userId"></param>
        public void SignIn(string userName, bool createPersistentCookie, int userId)
        {
            if (String.IsNullOrEmpty(userName)) 
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (userId <= 0)
                throw new ArgumentException("Value cannot be null or empty.", "userId");

            var userData = string.Format(" ;{0}", userId);
            var authTicket = new FormsAuthenticationTicket(
                1,                              // version
                userName,                       // user name
                DateTime.Now,                   // created
                DateTime.Now.AddMinutes(15),    // expires
                createPersistentCookie,         // persistent?
                userData                        // can be used to store roles etc.
            );

            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
        

    }


}
