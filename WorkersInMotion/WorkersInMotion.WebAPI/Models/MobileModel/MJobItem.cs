using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MJobItem
    {
        public System.Guid JobItemGUID { get; set; }
        public Nullable<System.Guid> JobIndexGUID { get; set; }
        public Nullable<System.Guid> JobPageGUID { get; set; }
        public Nullable<System.Guid> ItemLogicalID { get; set; }
        public Nullable<System.Guid> ItemValueGUID { get; set; }
        public Nullable<System.DateTime> ItemCaptureTime { get; set; }
        public string ItemValue { get; set; }
        public string ContentType { get; set; }
        public byte[] FileContent { get; set; }
        public Nullable<double> GPSLatitude { get; set; }
        public Nullable<double> GPSLongitude { get; set; }
        public Nullable<double> GPSAltitude { get; set; }
        public Nullable<System.Guid> CreateUserGUID { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
    }
}