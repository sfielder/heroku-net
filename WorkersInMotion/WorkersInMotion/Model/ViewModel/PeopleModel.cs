﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class PeopleModel
    {
        [ScaffoldColumn(false)]
        public string PeopleGUID { get; set; }
        [ScaffoldColumn(false)]
        public int RecordStatus { get; set; }
        [ScaffoldColumn(false)]
        public string UserGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationGUID { get; set; }
        [ScaffoldColumn(false)]
        public bool IsPrimaryContact { get; set; }
        [ScaffoldColumn(false)]
        public string PlaceGUID { get; set; }
        [ScaffoldColumn(false)]
        public string MarketGUID { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Client")]
        public string CompanyName { get; set; }
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"(?:\+\s*\d{2}[\s-]*)?(?:\d[-\s]*){10}", ErrorMessage = "Invalid Mobile Number.")]
        public string MobilePhone { get; set; }
        [Required]
        [Display(Name = "Business Phone")]
        [RegularExpression(@"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{3,14}|[0-9]{3,14})", ErrorMessage = "Invalid Phone Number.")]
        public string BusinessPhone { get; set; }
        [Display(Name = "Home Phone")]
        [RegularExpression(@"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{3,14}|[0-9]{3,14})", ErrorMessage = "Invalid Phone Number.")]
        public string HomePhone { get; set; }
        [Required]
        [Display(Name = "Email ID")]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Invalid Email ID.")]
        public string Emails { get; set; }
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
        public string PicFilename { get; set; }

        //Customer View-COntacts
        public string StoreID { get; set; }
    }
    public class PeopleViewModel
    {
        public IList<PeopleModel> PeopleList { get; set; }
        public IEnumerable<PeopleModel> People { get; set; }
        public IEnumerable<PlaceModel> Place { get; set; }
    }
    public class PeopleViewForCreate
    {
        public PeopleModel PeopleModel { get; set; }
        public PlaceViewModel PlaceViewModel { get; set; }
    }
}