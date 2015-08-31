using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You cannot use a blank password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string EmailID { get; set; }

        public string RedirectURL { get; set; }
    }
    public class loginmodel
    {
        public LoginViewModel LoginViewModel { get; set; }
        public ForgetPasswordModel ForgetPasswordModel { get; set; }
    }
}