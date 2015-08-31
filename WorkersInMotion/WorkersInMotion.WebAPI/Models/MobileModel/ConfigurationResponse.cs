using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class ConfigurationResponse
    {
        public List<CostTypes> CostTypes { get; set; }
        public List<Statuses> Statuses { get; set; }
        public List<SubStatuses> SubStatuses { get; set; }
        public List<OptionLists> Lists { get; set; }
    }

    public class CostTypes
    {
        public int CostType { get; set; }
        public string CostName { get; set; }
        public string Rate { get; set; }
        public string Description { get; set; }
        public string CurrencySymbol { get; set; }
    }
    public class Statuses
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
    }
    public class SubStatuses
    {
        public int StatusCode { get; set; }
        public int SubStatusCode { get; set; }
        public string Status { get; set; }
    }

    public class OptionLists
    {
        public string ListGUID { get; set; }
        public short ListType { get; set; }
        public string ListURL { get; set; }
        public string ListValues { get; set; }
    }
}