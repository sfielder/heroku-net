using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MJobSchema
    {
        public Guid JobLogicalID { get; set; }
        public Guid OrganizationGUID { get; set; }
        public Guid GroupCode { get; set; }
        public int EstimatedDuration { get; set; }
        public Guid CreatedByUserGUID { get; set; }
        public string JobSchemaName { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}