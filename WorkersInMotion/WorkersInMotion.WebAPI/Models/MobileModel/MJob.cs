using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class MJob
    {
        public Guid JobIndexGUID { get; set; }
        public Guid JobLogicalID { get; set; }
        public int JobID { get; set; }
        public string JobReferenceNo { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.Guid> UserGUID { get; set; }
        public Guid OrganizationGUID { get; set; }
        public string JobName { get; set; }
        public int CustType { get; set; }
        public Guid CustGUID { get; set; }
        public string JobNote { get; set; }
        public int SortOrder { get; set; }
        public Nullable<System.DateTime> ServiceStartTime { get; set; }
        public Nullable<System.DateTime> ServiceEndTime { get; set; }
        public Nullable<System.DateTime> ActualStartTime { get; set; }
        public Nullable<System.DateTime> ActualEndTime { get; set; }
        public int Status { get; set; }
        public bool IsScheduled { get; set; }
        public double GPSLatitude { get; set; }
        public double GPSLongitude { get; set; }
        public double GPSAltitude { get; set; }
        public Guid CreateUserGUID { get; set; }
        public DateTime CreateDate { get; set; }
        public int EstimatedDuration { get; set; }
        public DateTime PreferredStartTime { get; set; }
        public DateTime PreferredEndTime { get; set; }
        public Guid RegionCode { get; set; }
        public Guid TerritoryCode { get; set; }
        public Guid DeptCode { get; set; }
        public Guid GroupCode { get; set; }
        public Guid ServicePointGUID { get; set; }
        public Guid StopsGUID { get; set; }
        public string ServiceAddress { get; set; }
        public string Instruction { get; set; }
        public string SignoffName { get; set; }
        public string SignoffSignature { get; set; }
        public byte[] SignoffSignatureContent { get; set; }
        public string WorkImage { get; set; }
        public byte[] WorkImageContent { get; set; }
    }
}