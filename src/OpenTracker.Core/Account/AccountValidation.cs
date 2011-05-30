using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenTracker.Core.Account
{
    public static class AccountValidation
    {
        /// <summary>
        /// 
        /// </summary>
        public enum @Class
        {
            User = 1,
            PowerUser = 2,
            Uploader = 4,
            VIP = 8,
            Moderator = 16,
            Administrator = 32,
            Owner = 64,
            Developer = 128
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        public static string MD5(string originalPassword)
        {
            var md5 = new MD5CryptoServiceProvider();
            var originalBytes = Encoding.Default.GetBytes(originalPassword);
            var encodedBytes = md5.ComputeHash(originalBytes);

            return BitConverter.ToString(encodedBytes).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createStatus"></param>
        /// <returns></returns>
        public static string ErrorCodeToString(AccountService.AccountCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case AccountService.AccountCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case AccountService.AccountCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case AccountService.AccountCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case AccountService.AccountCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case AccountService.AccountCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case AccountService.AccountCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case AccountService.AccountCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case AccountService.AccountCreateStatus.ServerError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case AccountService.AccountCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

    }


}
