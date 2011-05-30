using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OpenTracker.Core.Common.Validation;

namespace OpenTracker.Models.Account
{
    public class RegisterModel
    {

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }


        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "This is not a valid email address.")]
        [Display(Name = "Email address")]
        [Email(ErrorMessage = "This is not a valid email address.")]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        public string SecurityCode { get; set; }



    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }



    }
}