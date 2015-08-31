using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.WebAPI.Models.MobileModel.POSModels;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class CreateJobForPORequest
    {
        public string JobClass { get; set; }
        public string PONumber { get; set; }
        public string JobName { get; set; }
        public poClass POSJson { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string SessionGUID { get; set; }
    }
}