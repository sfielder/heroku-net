using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class POView
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
        public string EmailID { get; set; }
        public string InstallerName { get; set; }
        public string POCustomerName { get; set; }
        public string POCustomerPhone { get; set; }
        public string PoCustomerMobile { get; set; }
        public string JobName { get; set; }
        public string RegionName { get; set; }
        public string TerritoryName { get; set; }
        public string StoreName { get; set; }
        public string PlaceName { get; set; }
    }
    public class POViewLists
    {
        public List<POView> POViewList { get; set; }
    }
}