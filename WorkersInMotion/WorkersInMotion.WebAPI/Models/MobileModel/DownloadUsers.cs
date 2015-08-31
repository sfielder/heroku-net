using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class DownloadUsers
    {
        public List<p_GetUsers_Result> UserRecords { get; set; }
    }
}