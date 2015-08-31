using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class CreateJobForCustomerStopRequest
    {
        public string CustomerStopID { get; set; }
        public string CustomerID { get; set; }
        public string JobClass { get; set; }
        public string JobName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string SessionGUID { get; set; }
    }
}