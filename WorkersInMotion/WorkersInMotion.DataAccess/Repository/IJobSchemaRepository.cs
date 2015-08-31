using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.DataAccess.Repository
{
    public interface IJobSchemaRepository : IDisposable
    {
        IEnumerable<JobForm> GetJobSchema(Guid OrganizationGUID);
        IEnumerable<JobForm> GetJobSchemabyGroupCode(string Skill);
        int InsertJobSchema(JobForm JobForm);
        JobForm JobSchemaDetails(Guid JobFormGUID);
        IEnumerable<Job> GetjobByJobFormClass(Int16 jobclass, Guid OrganizationGUID);
        int SetDeleteFlag(Guid JobGUID);
        int DeleteJobSchema(Guid LogicalGUID);
        JobForm GetJobSchemabyJobFormID(Guid pjobFormID);
        Guid GetJobFormIDfromJobForm(string jobform);
        //void InsertJobItemSchema(JobItemSchema JobItemSchema);
        //void InsertJobPageSchema(JobPageSchema JobPageSchema);

        //IEnumerable<JobItemSchema> GetJobItemSchema(Guid JobLogicalID);
        //IEnumerable<JobPageSchema> GetJobPageSchema(Guid JobLogicalID);

        // void DeleteJobPageByJobFormGUID(Guid JobFormGUID);
        //void DeleteJobPageItemByJobLogicalGUID(Guid LogicalGUID);
        //void DeleteJobPageItemByJobPageGUID(Guid pageGUID);
        //void DeleteJobPageByID(Guid PageLogicalGUID);

        //int Save();
    }
}