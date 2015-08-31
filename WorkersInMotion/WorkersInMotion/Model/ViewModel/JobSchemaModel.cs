using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class JobSchemaModel
    {
        [ScaffoldColumn(false)]
        public string JobLogicalID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationGUID { get; set; }
        //[Display(Name = "Group Code")]
        //[Required]
        //public string GroupCode { get; set; }
        //[Display(Name = "Estimated Duration (In Hours)")]
        //[Required]
        //public int EstimatedDuration { get; set; }
        public string LastModifiedBy { get; set; }
        [Required]
        [Display(Name = "Job Class Name")]
        public string JobSchemaName { get; set; }
        [Required]
        [Display(Name = "Job Form ID")]
        //[RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter integer only.")]
        // [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter Integer Value Only")]
        public Nullable<short> JobClass { get; set; }
        [ScaffoldColumn(false)]
        public DateTime LastModifiedDate { get; set; }
        [ScaffoldColumn(false)]
        public bool IsDeleted { get; set; }

        public string LastModifiedDateTime { get; set; }
    }
    public class JobSchemaViewModel
    {
        public IList<JobSchemaModel> JobSchemaModel { get; set; }
    }

    public class JobSchemaEditView
    {
        public JobSchemaModel JobSchemaModel { get; set; }
        public IList<JobPageSchemaModel> JobPageSchemaModel { get; set; }
        public IList<JobPageItemModel> JobPageItemModel { get; set; }
    }
}