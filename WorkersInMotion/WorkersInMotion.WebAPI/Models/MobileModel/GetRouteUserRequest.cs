using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class GetRouteUserRequest
    {
        public System.Guid JobGUID { get; set; }
        public int flag { get; set; }
        public int Request { get; set; }
    }
}