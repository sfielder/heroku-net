using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class MarketModel
    {
        [ScaffoldColumn(false)]
        public string MarketGUID { get; set; }
        [ScaffoldColumn(false)]
        public int RecordStatus { get; set; }
        [ScaffoldColumn(false)]
        public bool IsDefault { get; set; }
        [ScaffoldColumn(false)]
        public int Version { get; set; }
        [ScaffoldColumn(false)]
        public string UserGUID { get; set; }
        [ScaffoldColumn(false)]
        public int EntityType { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OwnerGUID { get; set; }
        [Display(Name = "Store Name")]
        [Required]
        public string MarketName { get; set; }

        [Display(Name = "Store ID")]
        [Required]
        public string MarketID { get; set; }

        public string RegionName { get; set; }

        public string TerritoryName { get; set; }
        [Display(Name = "Region Name")]
        [Required]
        public string RegionGUID { get; set; }
        [Display(Name = "Market Name")]
        [Required]
        public string TerritoryGUID { get; set; }

        public string TerritoryID { get; set; }
        [ScaffoldColumn(false)]
        public string PrimaryContactGUID { get; set; }

        public string RMUserGUID { get; set; }
        public string FMUserGUID { get; set; }

        [Required]
        [Display(Name = "Regional Manager")]
        public string RMName { get; set; }
        [Required]
        [Display(Name = "Field Manager")]
        public string FMName { get; set; }

        //[Required]
        //[Display(Name = "Contact Name")]
        //public string ContactName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [RegularExpression(@"(?:\+\s*\d{2}[\s-]*)?(?:\d[-\s]*){10}", ErrorMessage = "Invalid Mobile Number.")]
        [Display(Name = "Mobile Number")]
        public string MobilePhone { get; set; }
        [Required]
        [Display(Name = "Store Phone")]
        [RegularExpression(@"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{3,14}|[0-9]{3,14})", ErrorMessage = "Invalid Phone Number.")]
        public string MarketPhone { get; set; }
        [Display(Name = "Home Phone")]
        [RegularExpression(@"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{3,14}|[0-9]{3,14})", ErrorMessage = "Invalid Phone Number.")]
        public string HomePhone { get; set; }
        [Required]
        [Display(Name = "Email ID")]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Invalid Email ID.")]
        public string Emails { get; set; }
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
        [ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }

        public string LastStoreVisitedDate { get; set; }

    }
    public class MarketViewModel
    {
        public IList<MarketModel> MarketList { get; set; }
        public IEnumerable<MarketModel> Market { get; set; }
        public IEnumerable<PlaceModel> Place { get; set; }

    }
    public class MarketViewForCreate
    {
        public MarketModel MarketModel { get; set; }
        public PeopleViewModel PeopleViewModel { get; set; }


        //For New Implementation
        public IEnumerable<AspUser> RMUser { get; set; }
        public IEnumerable<AspUser> FMUser { get; set; }
    }

    public class ContactValues
    {
        public Guid UserGUID { get; set; }
        public Guid OrganizationGUID { get; set; }
        public Guid OwnerGUID { get; set; }
        public Guid PrimaryContactGUID { get; set; }
        public string ContactName { get; set; }
    }

}