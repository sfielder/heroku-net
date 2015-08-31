using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class EMCustomers
    {
        public IList<MobilePlace> Customers { get; set; }

    }
    public class EMContacts
    {
        public IList<MobilePeople> Contacts { get; set; }

    }
    public class EMMarkets
    {
        public IList<MobileMarket> Markets { get; set; }
    }
}