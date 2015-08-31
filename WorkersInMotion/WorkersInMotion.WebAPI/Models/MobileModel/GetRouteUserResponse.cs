using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class GetRouteUserResponse
    {
        public IList<Workers> LWorkers { get; set; }
    }

    public class Workers
    {
        public string WorkerID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Distance { get; set; }
        public string ImageURL { get; set; }
        public string AvailStart { get; set; }
        public string AvailEnd { get; set; }
        public string PreJobDet { get; set; }
        public int Type { get; set; }
        public string Rate { get; set; }
    }
}