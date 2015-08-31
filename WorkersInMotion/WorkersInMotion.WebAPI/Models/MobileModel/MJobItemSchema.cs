using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MJobItemSchema
    {
        public Guid ItemLogicalID { get; set; }
        public Guid JobLogicalID { get; set; }
        public Guid PageLogicalID { get; set; }
        public int SortOrder { get; set; }
        public string ItemName { get; set; }
        public string ItemControlType { get; set; }
        public DateTime ItemCaptureTime { get; set; }
        public string ItemValueType { get; set; }
        public string ItemValue { get; set; }
        public bool IsRequired { get; set; }
        public int CanView { get; set; }
        public int CanEdit { get; set; }
        public int CanRepeat { get; set; }
        public DateTime Createdate { get; set; }
        public int ItemOrder { get; set; }
    }
}