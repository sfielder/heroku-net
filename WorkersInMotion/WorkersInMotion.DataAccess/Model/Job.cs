//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkersInMotion.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Job
    {
        public Job()
        {
            this.JobAssigneds = new HashSet<JobAssigned>();
            this.JobForums = new HashSet<JobForum>();
            this.JobNotes = new HashSet<JobNote>();
            this.JobOffers = new HashSet<JobOffer>();
            this.JobProgresses = new HashSet<JobProgress>();
        }
    
        public System.Guid JobGUID { get; set; }
        public string JobReferenceNo { get; set; }
        public System.Guid OrganizationGUID { get; set; }
        public Nullable<System.Guid> RegionGUID { get; set; }
        public Nullable<System.Guid> TerritoryGUID { get; set; }
        public Nullable<short> LocationType { get; set; }
        public Nullable<System.Guid> CustomerGUID { get; set; }
        public Nullable<System.Guid> CustomerStopGUID { get; set; }
        public Nullable<System.Guid> ServicePointGUID { get; set; }
        public string ServiceAddress { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<System.Guid> AssignedUserGUID { get; set; }
        public Nullable<System.Guid> ManagerUserGUID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsUrgent { get; set; }
        public Nullable<int> StatusCode { get; set; }
        public Nullable<int> SubStatusCode { get; set; }
        public Nullable<bool> IsSecheduled { get; set; }
        public string JobName { get; set; }
        public Nullable<System.DateTime> PreferedStartTime { get; set; }
        public Nullable<System.DateTime> PreferedEndTime { get; set; }
        public System.DateTime ScheduledStartTime { get; set; }
        public Nullable<System.DateTime> ScheduledEndTime { get; set; }
        public Nullable<System.DateTime> ActualStartTime { get; set; }
        public Nullable<System.DateTime> ActualEndTime { get; set; }
        public Nullable<double> EstimatedDuration { get; set; }
        public Nullable<double> QuotedDuration { get; set; }
        public Nullable<double> ActualDuration { get; set; }
        public Nullable<short> CostType { get; set; }
        public Nullable<double> QuotedCost { get; set; }
        public Nullable<double> ActualCost { get; set; }
        public string JobForm { get; set; }
        public Nullable<short> JobClass { get; set; }
        public Nullable<short> SignOffRequired { get; set; }
        public string SignoffName { get; set; }
        public Nullable<short> PictureRequired { get; set; }
        public string PictureDescription { get; set; }
        public Nullable<bool> LocationSpecific { get; set; }
        public string PONumber { get; set; }
        public string TermsURL { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<System.Guid> LastModifiedBy { get; set; }
    
        public virtual ICollection<JobAssigned> JobAssigneds { get; set; }
        public virtual JobCostType JobCostType { get; set; }
        public virtual ICollection<JobForum> JobForums { get; set; }
        public virtual ICollection<JobNote> JobNotes { get; set; }
        public virtual ICollection<JobOffer> JobOffers { get; set; }
        public virtual ICollection<JobProgress> JobProgresses { get; set; }
    }
}
