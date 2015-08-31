using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class HeartBeatRequest
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public DateTime time { get; set; }
    }
}