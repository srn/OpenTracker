using System;
using System.Linq;
using System.Security.Principal;

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
        public Boolean ValidateUser(string username, string passhash)
        {
            using (context)
            {
                var retrieveTempUser = (from t in context.users
                                        where t.username == username
                                        select t).Take(1).FirstOrDefault();
                return retrieveTempUser != null && BCrypt.CheckPassword(passhash, retrieveTempUser.passhash);
            }

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
            else
            {
                // creates the new user
                var newUser = new users
                {
                    username = userName,
                    passhash = password,
                    email = email,
                    passkey = AccountValidation.MD5(password)
                };
                context.AddTousers(newUser);
                context.SaveChanges();

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
}
