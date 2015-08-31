using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class UMResponse
    {
        public IList<MGlobalUser> GlobalUser { get; set; }
        public IList<MUserProfile> UserProfile { get; set; }
        //public ResponseStatus Status { get; set; }

    }
    //public class GetUsers
    //{
    //    public IList<MGlobalUser> GlobalUser { get; set; }
    //    public IList<MUserProfile> UserProfile { get; set; }
    //}
    public class UMResponseOrganization
    {
        public MOrganization Organization { get; set; }
    }
    public class UMResponseCustomer
    {
        public List<MCustomers> Customers { get; set; }
    }
}