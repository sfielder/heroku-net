using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class ErrorResponse
    {
        public HttpStatusCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}