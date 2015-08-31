using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MobilePeople
    {
        public System.Guid PeopleGUID { get; set; }
        public Nullable<int> RecordStatus { get; set; }
        public System.Guid UserGUID { get; set; }
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public Nullable<bool> IsPrimaryContact { get; set; }
        public Nullable<System.Guid> PlaceGUID { get; set; }
        public Nullable<System.Guid> MarketGUID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string HomePhone { get; set; }
        public string Emails { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string ImageURL { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

    }
}