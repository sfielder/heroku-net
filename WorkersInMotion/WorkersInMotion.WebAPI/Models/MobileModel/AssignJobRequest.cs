using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class AssignJobRequest
    {
        public System.Guid WorkerID { get; set; }
        public System.Guid JobGUID { get; set; }
    }
}