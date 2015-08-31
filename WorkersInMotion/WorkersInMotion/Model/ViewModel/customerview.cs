using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class customerview
    {
        public PeopleViewModel PeopleViewModel { get; set; }
        public MarketViewModel MarketViewModel { get; set; }
        public PlaceModel PlaceModel { get; set; }

        public string ManagerName { get; set; }
    }
}