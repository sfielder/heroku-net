using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MGlobalUser
    {
        public System.Guid UserGUID { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public string Role_Id { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ApplicationURL { get; set; }
        public int ReportingPlaceType { get; set; }
        public Guid ReportPlaceGUID { get; set; }
        public bool IsExempt { get; set; }
        public Guid RegionGUID { get; set; }
        public Guid TerritoryGUID { get; set; }
        public bool IsDelete { get; set; }
        public Guid GroupGUID { get; set; }
    }
}