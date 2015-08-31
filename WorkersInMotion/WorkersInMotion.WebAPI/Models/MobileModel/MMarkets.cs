using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MMarkets
    {
        public System.Guid MarketGUID { get; set; }
        public Nullable<int> RecordStatus { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public Nullable<int> Version { get; set; }
        public Nullable<System.Guid> UserGUID { get; set; }
        public Nullable<int> EntityType { get; set; }
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public Nullable<System.Guid> OwnerGUID { get; set; }
        public string MarketName { get; set; }
        public Nullable<System.Guid> RegionGUID { get; set; }
        public Nullable<System.Guid> TerritoryGUID { get; set; }
        public Nullable<System.Guid> PrimaryContactGUID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string MarketPhone { get; set; }
        public string HomePhone { get; set; }
        public string Emails { get; set; }
        public string TimeZone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}