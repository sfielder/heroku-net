using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class CreateForumEntryRequest
    {
        public short Type { get; set; }
        public System.Guid JobGUID { get; set; }
        public string Message { get; set; }
        public System.Guid Recipient { get; set; }
    }
}