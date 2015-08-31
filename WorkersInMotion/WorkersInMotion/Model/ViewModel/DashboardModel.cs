using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class DashboardModel
    {

    }
    public class JobStatusPercentageList
    {
        public List<JobStatusPercentage> data { get; set; }
    }
    public class JobStatusPercentage
    {
        public string label { get; set; }
        public float data { get; set; }
    }
}