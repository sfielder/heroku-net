using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class CreateAccountResponse
    {
        public string UserGUID { get; set; }
        public int Role { get; set; }
    }
}