using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkersInMotion.WebAPI.Models.MobileModel
{
    public class JMResponse
    {
        //  public List<p_GetJobs_Result> JobList { get; set; }
        public List<MobileJob> Jobs { get; set; }
    }

    public class Contact
    {
        public string Company { get; set; }
        public string Person { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public System.Guid JobID { get; set; }
    }
    public class Note
    {
        public short NoteType { get; set; }
        public bool Deletable { get; set; }
        public System.Guid JobGUID { get; set; }
        public string NoteText { get; set; }
        public string CreatedByName { get; set; }
        public string JobNoteGUID { get; set; }
        public string CreatedDate { get; set; }
        public string FileURL { get; set; }
    }
    public class jobschema
    {
        public MJobSchema MJobSchema { get; set; }
        public IList<MJobPageSchema> MJobPageSchemaList { get; set; }
        public IList<MJobItemSchema> MJobItemSchemaList { get; set; }
    }
}