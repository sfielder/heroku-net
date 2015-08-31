using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class JobFormDataRequest
    {
        public string FormID { get; set; }
        public List<JobFormDataValue> Values { get; set; }
        public string JobGUID { get; set; }
    }
    public class JobFormDataValue
    {
        public string Value { get; set; }
        public string ControlID { get; set; }
        public string ControlLabel { get; set; }
        public string parentID { get; set; }
        public string controlParentLabel { get; set; }
        public int ValueID { get; set; }

    }
}