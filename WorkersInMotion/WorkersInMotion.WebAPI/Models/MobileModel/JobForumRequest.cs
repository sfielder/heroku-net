using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class JobForumRequest
    {
        public System.Guid JobGUID { get; set; }
        public short Acceptance { get; set; }
    }
}