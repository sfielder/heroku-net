using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class JobPageSchemaModel
    {
        [ScaffoldColumn(false)]
        public string PageLogicalID { get; set; }
        [ScaffoldColumn(false)]
        public string JobLogicalID { get; set; }
        public string PageSchemaName { get; set; }
        [ScaffoldColumn(false)]
        public int CanRepeat { get; set; }
        [ScaffoldColumn(false)]
        public DateTime CreateDate { get; set; }
    }
    public class JobPageSchemaViewModel
    {
        public IList<JobPageSchemaModel> JobPageSchemaModel { get; set; }
    }

    public class JobFormModel
    {
        public List<JobPageSchemaModel> JobPageList { get; set; }
        public List<JobPageItemModel> JobItemList { get; set; }
    }
}