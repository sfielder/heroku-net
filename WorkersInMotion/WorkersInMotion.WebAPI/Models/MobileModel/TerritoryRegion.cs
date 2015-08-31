using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class TerritoryRegion
    {
        public List<MobileTerritory> Territories { get; set; }
        public List<MobileRegion> Regions { get; set; }
    }
    public class MobileRegion
    {
        public Nullable<System.Guid> RegionGUID { get; set; }
        public Nullable<System.Guid> TerritoryGUID { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public string Description { get; set; }
    }
    public class MobileTerritory
    {
        public Nullable<System.Guid> TerritoryGUID { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public string Description { get; set; }
    }
}