using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class UploadJobRequest
    {
        public System.Guid JobGUID { get; set; }
        public DateTime StartTIme { get; set; }
        public int ElapsedTime { get; set; }
        public int Status { get; set; }
        public int Substatus { get; set; }
        public List<UploadJobNotes> Notes { get; set; }
        public JobFormDataRequest Form { get; set; }
    }

    public class UploadJobNotes
    {
        public short Type { get; set; }
        public string NoteText { get; set; }
        public string ContentName { get; set; }
    }
    public class UploadJobRequestNew : MobileJob
    {
        //public string ActualEndTime { get; set; }
        //public string ActualStartTime { get; set; }
        //public string CreateDate { get; set; }
        //public string EndTime { get; set; }
        //public double EstimatedDuration { get; set; }
        //public Nullable<System.Guid> JobGUID { get; set; }
        //public string JobName { get; set; }
        //public Nullable<double> Latitude { get; set; }
        //public bool LocationSpecific { get; set; }
        //public Nullable<double> Longitude { get; set; }
        //public string PONumber { get; set; }
        //public int PictureRequired { get; set; }
        //public string PreferedStartTime { get; set; }
        //public string PreferedEndTime { get; set; }
        //public string ScheduledStartTime { get; set; }
        //public string ScheduledEndTime { get; set; }
        //public int SignOffRequired { get; set; }
        //public string StartTime { get; set; }
        //public int StatusCode { get; set; }
        //public int SubStatusCode { get; set; }
        //public Nullable<bool> Urgent { get; set; }
        //public double ActualDuration { get; set; }
        //public string Cost { get; set; }
        //public short CostType { get; set; }
        //public int ElapsedTime { get; set; }
        //public int Status { get; set; }
        //public int FormClassID { get; set; }

        public JobFormDataRequest Form { get; set; }
        public string PONumber { get; set; }
        public string StoreID { get; set; }
        public string SessionGUID { get; set; }
    }
}