using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class JobModel
    {
        [Required]
        [Display(Name = "Visit Name")]
        public string JobName { get; set; }

        public Nullable<System.Guid> JobIndexGUID { get; set; }
        [Display(Name = "Visit Reference ID")]
        public string JobReferenceNo { get; set; }

        [Required]
        [Display(Name = "Visit Schema")]
        public string JobClass { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Visit Type")]
        public string IsScheduled { get; set; }

        [Required]
        [Display(Name = "Store")]
        public string CustomerName { get; set; }


        [ScaffoldColumn(false)]
        public Guid StopsGUID { get; set; }

        [ScaffoldColumn(false)]
        public Guid CustGUID { get; set; }

        [ScaffoldColumn(false)]
        public Guid RegionCode { get; set; }
        [ScaffoldColumn(false)]
        public Guid TerritoryCode { get; set; }

        [ScaffoldColumn(false)]
        public string CustomerPointName { get; set; }

        [ScaffoldColumn(false)]
        public string GroupName { get; set; }
        [Display(Name = "Check-In")]
        public string ActualStartTime { get; set; }
        [Display(Name = "Check-Out")]
        public string ActualEndTime { get; set; }
        [Display(Name = "Duration (hh:mm)")]
        public Nullable<double> EstimatedDuration { get; set; }
        [Display(Name = "Instruction To Worker")]
        public string Instruction { get; set; }
        public string PreferredStartTime { get; set; }
        public string PreferredEndTime { get; set; }
        public string Duration { get; set; }

        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> CreateDate { get; set; }


        public int Status { get; set; }



    }
    public class AssignModel
    {
        [Required]
        public string JobName { get; set; }

        public Nullable<System.Guid> JobIndexGUID { get; set; }

        [ScaffoldColumn(false)]
        public string ServiceStartTime { get; set; }

        [ScaffoldColumn(false)]
        public Guid UserGUID { get; set; }

    }

    public class AssignJobViewModel
    {
        public IList<AssignModel> AssignJobModelList { get; set; }
    }
    public class JobViewModel
    {
        public IList<JobModel> JobModelList { get; set; }
        public JobModel JobModel { get; set; }
        public IList<MarketModel> Market { get; set; }
        public IList<PlaceModel> Place { get; set; }

        public List<JobProgressViewModel> JobProgressList { get; set; }
    }

    public class AssignJobModel
    {
        public JobModel JobModel { get; set; }
        public IList<JobModel> JobModelList { get; set; }
        public IList<GlobalUserModel> GlobalUsers { get; set; }
    }

    public class JobStatusModel
    {

        public string JobName { get; set; }
        public string JobIndexGUID { get; set; }
        public string JobLogicalID { get; set; }
        public string UserGUID { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }

        public string ActualStartTime { get; set; }
        public string ActualEndTime { get; set; }
        public string PreferredStartTime { get; set; }
        public string EstimatedStartTime { get; set; }
        public string PreferredEndTime { get; set; }
        public string GroupName { get; set; }
        public double EstimatedDuration { get; set; }
        public string CustomerName { get; set; }
        public string StoreID { get; set; }
        public string PONumber { get; set; }

        public string SiteAddress { get; set; }
        public string Email { get; set; }
        public string AssociateName { get; set; }
        public string AssociateContactNumber { get; set; }
        public int statuscode { get; set; }
        public bool locationmismatch { get; set; }

        public string RegionGUID { get; set; }
        public string TerritoryGUID { get; set; }
        public string FieldManager { get; set; }
        public string RegionalManager { get; set; }
        public string LastModifiedDate { get; set; }




    }
    public class JobStatusViewModel
    {
        public IList<JobStatusModel> JobStatusModel { get; set; }

        public IList<GlobalUserModel> GlobalUsers { get; set; }

        public JobModel JobModel { get; set; }
    }
}