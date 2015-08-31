using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class JobStatusRequest
    {
        public System.Guid WorkerID { get; set; }
        public bool IsAll { get; set; }
    }
}