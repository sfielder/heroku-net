using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class OrganizationEditView
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
        public string TimeZone { get; set; }
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
        [ScaffoldColumn(false)]
        public Nullable<System.Guid> CreateBy { get; set; }

    }
}