using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel.POSModels
{
    public class POSStoreResponse
    {
        public POSStore store { get; set; }
    }

    public class POSStore
    {
        public string apistatus { get; set; }
        public string accountid { get; set; }
        public string storenum { get; set; }
        public string name { get; set; }
        public string parentid { get; set; }
        public string parentname { get; set; }
        public string status { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string addr3 { get; set; }
        public string addr4 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalcode { get; set; }
        public string country { get; set; }
        public string marketid { get; set; }
        public string market { get; set; }
        public string region { get; set; }
        public POSResponseRegionalManager regionalmanager { get; set; }
        public POSResponseFieldManager fieldmanager { get; set; }
    }
}