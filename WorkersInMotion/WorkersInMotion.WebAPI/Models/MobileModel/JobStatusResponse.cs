using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class JobStatusResponse
    {
        public List<p_GetJobStatistics_Result> JobStatistics { get; set; }
        //public Today today { get; set; }
        //public Week week { get; set; }
        //public Month month { get; set; }
    }
    public class Today
    {
        public string OpenCount { get; set; }
        public string InProgressCount { get; set; }
        public string CompleteCount { get; set; }
        public string HrsWorked { get; set; }
        public string DuePercentage { get; set; }
        public string Billing { get; set; }
    }
    public class Week
    {
        public string OpenCount { get; set; }
        public string InProgressCount { get; set; }
        public string CompleteCount { get; set; }
        public string HrsWorked { get; set; }
        public string DuePercentage { get; set; }
        public string Billing { get; set; }
    }
    public class Month
    {
        public string OpenCount { get; set; }
        public string InProgressCount { get; set; }
        public string CompleteCount { get; set; }
        public string HrsWorked { get; set; }
        public string DuePercentage { get; set; }
        public string Billing { get; set; }
    }
}