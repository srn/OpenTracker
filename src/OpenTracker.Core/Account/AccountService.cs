using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using OpenTracker.Core.Common;

namespace OpenTracker.Core.Account
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountService
    {
        /// <summary>
        /// 
        /// </summary>
        OpenTrackerDbContext context = new OpenTrackerDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passhash"></param>
        /// <returns></returns>
        public int ValidateUser(string username, string passhash)
        {
            using (context)
            {
                var retrieveTempUser = (from t in context.users
                                        where t.username == username && t.activated == 1
                                        select t).Take(1).FirstOrDefault();
                if (retrieveTempUser == null)
                    return 0;
                if (BCrypt.CheckPassword(passhash, retrieveTempUser.passhash))
                    return (int) retrieveTempUser.id;
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public AccountCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email))
                throw new ArgumentException("Value cannot be null or empty.", "email");

            var checkUsernameAlreadyExist = (from _accounts in context.users
                                             where _accounts.username == userName
                                             select _accounts).Take(1).FirstOrDefault();

            if (checkUsernameAlreadyExist != null)
                return AccountCreateStatus.DuplicateUserName;

            var activateSecret = AccountValidation.MD5(string.Format(password));
            // creates the new user
            var newUser = new users
                              {
                                  username = userName,
                                  passhash = password,
                                  email = email,
                                  passkey = AccountValidation.MD5(password),
                                  activatesecret = activateSecret
                              };
            context.AddTousers(newUser);
            context.SaveChanges();

            var client = new SmtpClient("smtp.gmail.com", 587)
                             {
                                 Credentials = new NetworkCredential("myopentracker@gmail.com", "lol123123"),
                                 EnableSsl = true
                             };
            using (var msg = new MailMessage())
            {
                var BASE_URL = TrackerSettings.BASE_URL
                    .Replace("http://", string.Empty)
                    .Replace("https://", string.Empty);
                msg.From = new MailAddress("no-reply@open-tracker.org");
                msg.Subject = string.Format("{0} user registration confirmation‏", BASE_URL);

                var bewlder = new StringBuilder();
                bewlder.AppendFormat(
                    @"
You have requested a new user account on {0} and you have
specified this address ({1}) as user contact.
 
If you did not do this, please ignore this email. The person who entered your
email address had the IP address {2}. Please do not reply.
 
To confirm your user registration, you have to follow this link:
 
http://{0}/account/activate/{3}/
 
After you do this, you will be able to use your new account. If you fail to
do this, you account will be deleted within a few days. We urge you to read
the RULES and FAQ before you start using {0}.
",
                    BASE_URL,
                    email,
                    HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],
                    activateSecret
                );
                msg.Body = bewlder.ToString();

                msg.To.Add(new MailAddress(email));
                client.Send(msg);


                return AccountCreateStatus.Success;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum AccountCreateStatus
        {
            Success,
            DuplicateUserName,
            DuplicateEmail,
            InvalidPassword,
            InvalidEmail,
            InvalidAnswer,
            InvalidQuestion,
            InvalidUserName,
            ServerError,
            UserRejected,
        };
    }


    public class AccountInformation
    {
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Class { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Uploaded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Downloaded { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public AccountInformation()
        {
            var id = ((FormsIdentity)HttpContext.Current.User.Identity).Ticket;
            this.UserId = Convert.ToInt32(id.UserData.Split(';')[1]);

            using (var context = new OpenTrackerDbContext())
            {
                var retrieveUser = (from u in context.users
                                        where u.id == UserId
                                        select new
                                        {
                                            Class = u.@class,
                                            Uploaded = u.uploaded,
                                            Downloaded = u.downloaded
                                        }).Take(1).FirstOrDefault();
                if (retrieveUser == null)
                {
                    HttpContext.Current.Response.Redirect("/");
                }
                else
                {
                    Class = (int) retrieveUser.Class;
                    Uploaded = (long) retrieveUser.Uploaded;
                    Downloaded = (long) retrieveUser.Downloaded;
                }
            }
        }

        


    }
}
