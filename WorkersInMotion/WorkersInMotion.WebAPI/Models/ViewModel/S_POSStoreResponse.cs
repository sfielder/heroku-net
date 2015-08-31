using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.Model.ViewModel
{
    public class S_POSStoreResponse
    {
        public S_POSStore store { get; set; }
    }
    public class S_POSStoreResponseForPO
    {
        public S_POSStore store { get; set; }
        public S_POSResponseFieldManager fieldmanager { get; set; }
    }
    public class S_POSStore
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
        public S_POSResponseRegionalManager regionalmanager { get; set; }
        public S_POSResponseFieldManager fieldmanager { get; set; }
        public List<S_POSResponseManagers> managers { get; set; }
    }

    public class S_POSResponseRegionalManager
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class S_POSResponseFieldManager
    {
        public string userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
    //04-02-2015 Prabhu
    public class S_POSResponseManagers
    {
        public string managerid { get; set; }
        public string userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string job_category { get; set; }
    }
}