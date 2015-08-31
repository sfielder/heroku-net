using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class SiteVisitReports
    {
        public List<SiteVisit> SiteVisitList { get; set; }
    }
    public class SiteVisit
    {
        public string OrganizationGUID { get; set; }
        public string MarketID { get; set; }
        public string RMName { get; set; }
        public string FMName { get; set; }
        public Guid RegionGUID { get; set; }
        public string RegionName { get; set; }
        public Guid TerritoryGUID { get; set; }
        public string TerritoryName { get; set; }
        public Guid CustomerStopGUID { get; set; }
        public string CustomerStopName { get; set; }
        public Guid CustomerGUID { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string JobName { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public bool LocationMismatch { get; set; }
        public Guid JobGUID { get; set; }
        public string PONumber { get; set; }
        public string ActualStartTime { get; set; }
        public string ActualEndTime { get; set; }
    }

}