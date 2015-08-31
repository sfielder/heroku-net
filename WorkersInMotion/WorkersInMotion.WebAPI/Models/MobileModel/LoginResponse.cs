using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{

    public class LoginResponse
    {
        public string SessionID { get; set; }
        public string UserGUID { get; set; }
        public int Role { get; set; }
        public p_GetUsers_Result UserRecord { get; set; }

    }

    //public class LoginReturn
    //{
    //    public LoginResponse LoginResponse { get; set; }
    //}
}