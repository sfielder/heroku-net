using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MJobPage
    {
        public System.Guid JobPageGUID { get; set; }
        public Nullable<System.Guid> JobIndexGUID { get; set; }
        public Nullable<System.Guid> PageLogicalID { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public string PageName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string PageDescription { get; set; }
        public string PageAttributes { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }


        public IList<MJobItem> JobItemList { get; set; }
    }
}