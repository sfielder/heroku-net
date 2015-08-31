using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class OrganizationView
    {
        [ScaffoldColumn(false)]
        public System.Guid OrganizationGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationName { get; set; }
        [Required]
        [Display(Name = "Organization Full Name")]
        public string OrganizationFullName { get; set; }
        [Display(Name = "Website")]
        public string Website { get; set; }
        [Display(Name = "Phone")]
        [RegularExpression(@"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{3,14}|[0-9]{3,14})", ErrorMessage = "Invalid Phone Number.")]
        public string Phone { get; set; }
        [ScaffoldColumn(false)]
        public double TimeZone { get; set; }
        [Display(Name = "Address Line1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line2")]
        public string AddressLine2 { get; set; }
        [Required]
        [Display(Name = "City")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please enter characters only.")]
        public string City { get; set; }
        [Required]
        [Display(Name = "State")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please enter characters only.")]
        public string State { get; set; }
        [Required]
        [Display(Name = "Country")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please enter characters only.")]
        public string Country { get; set; }
        [Required]
        [Display(Name = "Zip Code")]
        [RegularExpression(@"^([^<>]){1,10}$", ErrorMessage = "Invalid Zip Code.")]
        public string ZipCode { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Invalid Email ID.")]
        [Display(Name = "Email ID")]
        public string EmailID { get; set; }

        public string ImageURL { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<bool> IsActive { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<bool> IsDeleted { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
       
        [Display(Name = "User ID")]
        public string UserID { get; set; }
        [Required(ErrorMessage = "You cannot use a blank password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "You cannot use a blank password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The new password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }
        [ScaffoldColumn(false)]
        public string UserType { get; set; }

    }
    public class OrganizationViewModel
    {
        public IList<OrganizationView> Organization { get; set; }
    }
}