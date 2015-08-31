using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class GroupModel
    {
        [ScaffoldColumn(false)]
        public string GroupGUID { get; set; }
        [ScaffoldColumn(false)]
        public string OrganizationGUID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
    public class GroupViewModel
    {
        public IList<GroupModel> Group { get; set; }
    }
}