using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class JobPageItemModel
    {
        public string ItemLogicalID { get; set; }
        public string JobLogicalID { get; set; }
        public string PageLogicalID { get; set; }
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
    public class JobPageItemViewModel
    {
        public IList<JobPageItemModel> JobPageItemModel { get; set; }
    }
}