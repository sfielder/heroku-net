using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class UpdateJobStatusRequest
    {
        public System.Guid JobGUID { get; set; }
        public int Status { get; set; }
        public int Substatus { get; set; }
        public DateTime StartTIme { get; set; }
        public int ElapsedTime { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool LocationMismatch { get; set; }
    }
}