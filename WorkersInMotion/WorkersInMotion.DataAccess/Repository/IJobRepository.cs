using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IJobRepository : IDisposable
    {

        List<Job> GetJobs(Job pJob);
        List<Job> GetOpenJobs(Job pJob);
        List<Job> GetJobStatus(Job pJob);
        //List<Job> GetJobByUserGUID(Guid UserGUID);
        //List<Job> GetOpenJobByUserGUID(Guid UserGUID);
        int UpdateJobStatus(Job _job);
        int UploadJobs(Job UploadJobRequest);
        dynamic GetRouteUsers(SqlParameter[] UserAvailability, SqlParameter[] UserLocation);
        List<p_GetJobStatistics_Result> GetJobStatus(SqlParameter[] sqlParam);
        int AssignJob(Job _Job);
        JobForum GetJobForumbyJobGUID(Guid JobGUID);
        int UpdateForumStatus(JobForum JobForumRequest);
        int UpdateForumEntries(JobForum UpdateForumEntryRequest);
        int InsertForumEntries(JobForum lJobForum);
        List<JobForum> GetForumEntries(Guid JobGUID);
        List<JobNote> GetJobNotesfromJobGUID(Guid JobGUID);
        List<JobCostType> getJobCostTypes();
        List<JobStatusOrganization> getJobStatusOrganization(Guid OrganizationGUID);
        List<JobSubStatusOrganization> getJobSubStatusOrganization(Guid OrganizationGUID);
        List<OptionList> getOptionList(Guid OrganizationGUID);
        int InsertJobAssigned(JobAssigned _JobAssigned);
        JobProgress GetJobProgressMismatch(Guid JobGUID, int Status);
        List<JobProgress> GetJobProgress(Guid JobGUID);
        List<Job> GetStoreVisitJobs(Job pJob);
        //List<Job> GetStoreVisitJobs(Job pJob, string FieldManagerID);
        // List<Market> GetStoreVisitJobs(Guid OrganizationGUID, Nullable<Guid> CustomerGUID);
        // List<Market> GetStoreNonVisitJobs(Job pjob, Nullable<Guid> CustomerGUID);
        // List<Market> GetStoreNonVisitJobsForDashboard(Job pJob);
        List<Job> GetSiteVisitJobs(Job pJob);
        //List<Job> GetSiteNonVisitJobs(Job pJob);
        // List<Job> GetStoreNonVisitJobs(Job pJob);
        // List<Job> GetStoreNonVisitJobs(Job pJob, string FieldManagerID);
        Job GetJobByCustomerStopGUID(Guid CustomerStopGUID);
        IEnumerable<dynamic> Job_UserActivityGraph(Job pJob);

        int InsertJobFormData(JobFormData JobFormData);
        int InsertJobNotes(JobNote jobNote);
        POs GetPOs(POs PO);
        POs GetPOsForClientStoreOrganization(POs PO);
        List<POs> GetPOList(string SessionID);
        int InsertPO(POs PO);
        // IEnumerable<Job> GetJobByUserGUIDForServer(Guid UserGUID);
        Job CreateJob(Job jobRequest);
        Job CreateJobbyStoreID(Job jobRequest);
        Job CreateJobbyPO(Job jobRequest);
        Job GetJobByID(Guid JobGUID);
        // IEnumerable<Job> GetjobByOrganizationGUID(Guid OrganizationGUID);
        IEnumerable<Job> GetjobByOrganizationGUIDBetweenDate(Guid OrganizationGUID, DateTime fromdate, DateTime todate);
        IEnumerable<Job> GetjobByUserandJobID(Guid UserGUID, Guid JobIndexGUID);
        // IEnumerable<Job> GetjobStatusByOrganizationGUID(Guid OrganizationGUID);
        //  IEnumerable<Job> GetJobByRegionandTerritory(Guid RegionGUID, Guid TerritoryGUID);
        IEnumerable<Job> GetJobByLastDownloadtTime(string lastdownloadtime);
        IEnumerable<Job> GetJobByGroupGUIDforClient(Guid GroupGUID);
        IEnumerable<Job> GetjobStatusByUserGUIDBetweenDate(Guid UserGUID, DateTime fromdate, DateTime todate);
        IEnumerable<Job> GetjobStatusByRegionAndTerritoryBetweenDate(Guid UserGUID, DateTime fromdate, DateTime todate);
        IEnumerable<Job> GetjobStatusByRegionAndTerritory(Guid UserGUID);
        IEnumerable<Job> GetjobByRegionandTerritory(Guid UserGUID);
        //  IEnumerable<Job> GetjobByTerritoryGUID(Guid TerritoryGUID);
        IEnumerable<Job> GetjobByGroupGUID(Guid GroupGUID);
        int UpdateJobFromClient(Job job);
        //  IEnumerable<Job> GetjobStatusByJobLogicalID(Guid JobLogicalID);
        //   IEnumerable<Job> GetjobStatusByUserGUID(Guid UserGUID, Guid OrganizationGUID);
        int InsertJobbyStoreID(Job job);
        int InsertJobbyPO(Job job);
        int InsertJob(Job job);
        //  int DeleteJobByJobLogicalID(Guid JobLogicalID);
        int DeleteJob(Guid JobIndexGUID);
        void SetDeleteFlag(Guid JobIndexGUID);
        int DeleteJobByOrganizationGUID(Guid OrganizationGUID);
        int UpdateJob(Job job);
        string GetCustomerPointName(Guid StopsGUID);
        string GetStatusName(int status);
        int GetStatusID(string statusname);
        string GetGroupName(Guid GroupGUID);
        string GetCustomerName(Guid CustomerGUID);
        // void AssignUser(Guid UserGUID, Guid JobIndexGUID);
        int InsertJobProgress(JobProgress pJobProgress);
        int InsertJobProgressWithDuration(JobProgress pJobProgress);
        int UpdateJobProgress(JobProgress pJobProgress);
        //int Save();
        string GetOrganizationName(Guid pJobGUID);
        Guid? GetOrganizationGUID(Guid pUserGUID);
        Job GetJobByPONumber(string pPONumber);


    }
}