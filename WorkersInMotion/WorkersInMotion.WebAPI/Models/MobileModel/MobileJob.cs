using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MobileJob
    {
        public Nullable<System.Guid> JobGUID { get; set; }
        public string JobName { get; set; }
        public string JobReferenceNo { get; set; }
        public string CustomerCompanyName { get; set; }
        public string CustomerContactName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerLogoURL { get; set; }
        public string Cost { get; set; }
        public short CostType { get; set; }
        public string PreferedStartTime { get; set; }
        public string PreferedEndTime { get; set; }
        public string ScheduledStartTime { get; set; }
        public string ScheduledEndTime { get; set; }
        public string ActualStartTime { get; set; }
        public string ActualEndTime { get; set; }
        public double EstimatedDuration { get; set; }
        public double ActualDuration { get; set; }
        public int StatusCode { get; set; }
        public int SubStatusCode { get; set; }
        public List<Note> Notes { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public string CreateDate { get; set; }
        public Nullable<bool> Urgent { get; set; }
        public Nullable<System.Guid> AssignedUserGUID { get; set; }
        public string TermsURL { get; set; }
        public int PictureRequired { get; set; }
        public int SignOffRequired { get; set; }
        //  public string JobClass { get; set; }
        public short JobClass { get; set; }
        public string PONumber { get; set; }
        public bool LocationSpecific { get; set; }
        public Nullable<short> LocationType { get; set; }
        public Nullable<System.Guid> CustomerStopGUID { get; set; }
        public Nullable<System.Guid> ServicePointGUID { get; set; }
        public Nullable<System.Guid> RegionGUID { get; set; }
        public Nullable<System.Guid> TerritoryGUID { get; set; }
        //Added to return the Base64 encoded JSON received from the POS server
        public string POSJson { get; set; }
        //public Nullable<System.Guid> CustomerGUID { get; set; }
        //public Nullable<System.Guid> OrganizationGUID { get; set; }

    }
}