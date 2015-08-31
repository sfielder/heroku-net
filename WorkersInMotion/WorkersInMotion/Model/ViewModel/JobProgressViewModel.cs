using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class JobProgressViewModel
    {
        public System.Guid JobProgressGUID { get; set; }
        public Nullable<System.Guid> JobGUID { get; set; }
        public Nullable<int> JobStatus { get; set; }
        public Nullable<int> JobSubStatus { get; set; }
        public string StatusNote { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<bool> LocationMismatch { get; set; }
        public Nullable<double> Duration { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<System.Guid> LastModifiedBy { get; set; }


        public string Status { get; set; }
        public string DurationInHourFormat { get; set; }
    }
}