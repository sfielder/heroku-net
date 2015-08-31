using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class TerritoryModel
    {
        [ScaffoldColumn(false)]
        public string TerritoryGUID { get; set; }
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
        [Display(Name = "Market ID")]
        public string TerritoryID { get; set; }
    }
    public class TerritoryViewModel
    {
        public IList<TerritoryModel> Territory { get; set; }
    }
}