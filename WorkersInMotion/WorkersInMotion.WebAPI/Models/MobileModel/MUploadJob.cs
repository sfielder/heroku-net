using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MUploadJob
    {
        public Guid JobIndexGUID { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> ActualStartTime { get; set; }

        public IList<MJobPage> JobPageList { get; set; }
    }
}