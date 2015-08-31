using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class Forum
    {
        public System.Guid JobGUID { get; set; }
        public IList<Message> Messages { get; set; }

    }
    public class Message
    {
        public string MessgageID { get; set; }
        public string PosterID { get; set; }
        public string PosterName { get; set; }
        public string PostDate { get; set; }
        public int PosterRole { get; set; }
        public short MessageType { get; set; }
    }
}