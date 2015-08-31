using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class JobRequest
    {
        public string UserGUID { get; set; }
        public string LastDownloadTime { get; set; }
        public string RegionGUID { get; set; }
        public string TerritoryGUID { get; set; }
        public string GroupGUID { get; set; }
    }
}