using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MJobPageSchema
    {
        public Guid PageLogicalID { get; set; }
        public Guid JobLogicalID { get; set; }
        public string PageSchemaName { get; set; }
        public int CanRepeat { get; set; }
        public DateTime CreateDate { get; set; }
    }
}