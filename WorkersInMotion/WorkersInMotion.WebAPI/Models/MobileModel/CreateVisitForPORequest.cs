using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class CreateVisitForPORequest:POs
    {
        public string JobName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}