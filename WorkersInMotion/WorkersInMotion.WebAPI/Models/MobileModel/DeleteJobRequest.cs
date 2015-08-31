using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class DeleteJobRequest
    {
        public System.Guid JobGUID { get; set; }
        internal string OrganizationName { get; set; }
        public string SessionGUID { get; set; }
    }
   
}