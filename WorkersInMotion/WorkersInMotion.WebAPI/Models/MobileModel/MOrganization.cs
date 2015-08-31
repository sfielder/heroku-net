using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MOrganization
    {
        public System.Guid OrganizationGUID { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationFullName { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public string TimeZone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string EmailID { get; set; }
        public string ApplicationURL { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}