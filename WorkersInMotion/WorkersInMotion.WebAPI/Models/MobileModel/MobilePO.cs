using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MobilePO
    {
        public System.Guid POGUID { get; set; }
        public Nullable<System.Guid> OrganizationGUID { get; set; }
        public Nullable<System.Guid> RegionGUID { get; set; }
        public Nullable<System.Guid> TerritoryGUID { get; set; }
        public string PONumber { get; set; }
        public Nullable<short> Status { get; set; }
        public string Description { get; set; }
        public string PlaceID { get; set; }
        public Nullable<short> LocationType { get; set; }
        public string MarketID { get; set; }
        public string EndCustomerAddress { get; set; }
        public string EndCustomerName { get; set; }
        public string EndCustomerPhone { get; set; }
        public string WorkerName { get; set; }
        public Nullable<bool> CustomBooleanValue { get; set; }
        public Nullable<short> JobClass { get; set; }
        public string OrderDate { get; set; }
        public string PreferredDateTime { get; set; }
        public Nullable<double> EstimatedCost { get; set; }
        public string CreateDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public string LastModifiedDate { get; set; }
        public Nullable<System.Guid> LastModifiedBy { get; set; }
    }

    public class MobilePoList
    {
        public List<MobilePO> POList { get; set; }
    }
}