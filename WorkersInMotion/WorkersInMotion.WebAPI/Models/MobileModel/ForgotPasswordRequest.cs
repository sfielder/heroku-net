using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class ForgotPasswordRequest
    {
        public string Cred { get; set; }
        public string PushID { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
    }
}