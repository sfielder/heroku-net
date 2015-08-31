using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class LocationResponse
    {
        public System.Guid UserGUID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Time { get; set; }
    }
}
