using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class GlobalUserModel
    {
        [ScaffoldColumn(false)]
        public System.Guid UserGUID { get; set; }
        [ScaffoldColumn(false)]
        public string UserType { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<int> UserSubTypeCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [ScaffoldColumn(false)]
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public string Role_Id { get; set; }
        public string ApplicationURL { get; set; }
        public Nullable<System.Guid> ReportPlaceGUID { get; set; }
        public Nullable<System.Guid> RegionGUID { get; set; }
        public Nullable<System.Guid> TerritoryGUID { get; set; }
        public Nullable<System.Guid> GroupGUID { get; set; }
    }
    public class GlobalUserViewModel
    {
        public IList<GlobalUserModel> GlobalUsers { get; set; }
    }
}