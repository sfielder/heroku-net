using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel.POSModels
{
    public class POSPOResponse
    {
        public poClass po { get; set; }
        public apiClass api { get; set; }
    }
    public class apiClass
    {
        public string name { get; set; }
        public string vers { get; set; }
        public string status { get; set; }
    }
    public class poClass
    {
        //public string apistatus { get; set; }
        public string ticketid { get; set; }
        public string ticketnumber { get; set; }
        public string wostatus { get; set; }
        public string scheduleddate { get; set; }
        public string postalcode { get; set; }
        public string zipcode { get; set; }
        public string timezone { get; set; }
        public string dst { get; set; }
        public POSResponseStore store { get; set; }
        public POSResponseJob job { get; set; }
        public POSResponseInstaller installer { get; set; }
        public POSResponseCrew crew { get; set; }
        public POSResponseCustomer customer { get; set; }
        public POSResponseRegionalManager regionalmanager { get; set; }
        public POSResponseFieldManager fieldmanager { get; set; }
    }

    public class POSResponseStore
    {
        public string accountid { get; set; }
        public string storenum { get; set; }
        public string name { get; set; }
        public string parentid { get; set; }
        public string parentname { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalcode { get; set; }
        public string country { get; set; }
        public string marketid { get; set; }
        public string market { get; set; }
        public string region { get; set; }
    }

    public class POSResponseJob
    {
        public string type { get; set; }
        public string desc { get; set; }
        public string code { get; set; }
    }

    public class POSResponseInstaller
    {
        public string accountid { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }

    public class POSResponseCrew
    {
        public string contactid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string badge { get; set; }
        public string workphone { get; set; }
    }

    public class POSResponseCustomer
    {
        public string contactid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalcode { get; set; }
        public string homephone { get; set; }
        public string mobilephone { get; set; }
        public string yearhomebuilt { get; set; }
    }

    public class POSResponseRegionalManager
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class POSResponseFieldManager
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
}