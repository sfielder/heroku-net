using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class mycompany
    {
        public AspNetUserViewModel AspNetUserViewModel { get; set; }
        public RegionViewModel RegionViewModel { get; set; }
        public TerritoryViewModel TerritoryViewModel { get; set; }
        public OrganizationEditView OrganizationEditView { get; set; }
    }
}