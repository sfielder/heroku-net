using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class BreadCrumb
    {
        public List<breadcrumstruct> breadcrumbs { get; set; }
    }
    public class breadcrumstruct
    {
        public string name { get; set; }
        public string action { get; set; }
        public string controller { get; set; }
        public int flag { get; set; }
    }
}