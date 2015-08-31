using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class RegionModel
    {
        [ScaffoldColumn(false)]
        public string RegionGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationGUID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Region ID")]
        public string RegionID { get; set; }
    }
    public class RegionViewModel
    {
        public IList<RegionModel> Region { get; set; }
    }
}