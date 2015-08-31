using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public enum eLoginType
    {
        WebLogin = 0,
        DeviceLogin = 1,
        APITest = 2
    }
    public class AspUser
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "User ID")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "You cannot use a blank password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; }
        [Required(ErrorMessage = "You cannot use a blank password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("PasswordHash", ErrorMessage = "The new password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }
        public string SecurityStamp { get; set; }
        public string Discriminator { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email ID")]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Invalid Email ID.")]
        public string EmailID { get; set; }
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"(?:\+\s*\d{2}[\s-]*)?(?:\d[-\s]*){10}", ErrorMessage = "Invalid Mobile Number.")]
        public string MobilePhone { get; set; }
        [Display(Name = "Business Phone")]
        [RegularExpression(@"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{3,14}|[0-9]{3,14})", ErrorMessage = "Invalid Phone Number.")]
        public string BusinessPhone { get; set; }
        [Display(Name = "Home Phone")]
        [RegularExpression(@"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{3,14}|[0-9]{3,14})", ErrorMessage = "Invalid Phone Number.")]
        public string HomePhone { get; set; }
        [Display(Name = "User Type")]
        [Required]
        public string RoleGUID { get; set; }
        //[Display(Name = "Worker Group")]
        //[Required]
        //public string GroupGUID { get; set; }
        [Display(Name = "Region")]
        [Required]
        public string RegionGUID { get; set; }
        [Display(Name = "Market")]
        [Required]
        public string TerritoryGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationGUID { get; set; }
        [ScaffoldColumn(false)]
        public string ProfileGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationUserMapGUID { get; set; }
        [ScaffoldColumn(false)]
        public string UserGUID { get; set; }
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
        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        [ScaffoldColumn(false)]
        public string SubscriptionGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationSubscriptionGUID { get; set; }

        public string ImageURL { get; set; }



        [ScaffoldColumn(false)]
        public string UserType { get; set; }

        public string RegionName { get; set; }
        public string TerritoryName { get; set; }
        public string GroupName { get; set; }
        public string UserTypeName { get; set; }
        public string CompanyName { get; set; }
        //[ScaffoldColumn(false)]
        //public int SubscriptionPurchased { get; set; }
        //[ScaffoldColumn(false)]
        //public int SubscriptionConsumed { get; set; }
    }

    public class AspNetUserViewModel
    {
        public IList<AspUser> Users { get; set; }
    }
}