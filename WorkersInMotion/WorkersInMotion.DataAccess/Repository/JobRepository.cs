using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Model.ViewModel;



namespace WorkersInMotion.DataAccess.Repository
{
    public class JobRepository : IJobRepository, IDisposable
    {
        private WorkersInMotionDB context;
        private WorkersInMotionDB contextWIM;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPORepository _IPORepository;

        public JobRepository(WorkersInMotionDB context)
        {
            this.context = context;
            this.contextWIM = new WorkersInMotionDB();
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IPORepository = new PORepository(new WorkersInMotionDB());
        }
        //public IEnumerable<Job> GetJobByUserGUIDForServer(Guid UserGUID)
        //{
        //    using (var dataContext = new WorkersInMotionJobDB())
        //    {
        //        return (from p in dataContext.Jobs
        //                where p.AssignedUserGUID == UserGUID
        //                select p).ToList().OrderBy(x => x.ScheduledStartTime);
        //    }
        //}
        public SqlParameter[] SetParametersForStoreCreate(Job job)
        {
            SqlParameter[] Param = new SqlParameter[28];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = job.JobGUID;// (object)globaluser.Role_Id ?? DBNull.Value;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = job.OrganizationGUID;
            Param[2] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)job.RegionGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)job.TerritoryGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pLocationType", SqlDbType.SmallInt);
            Param[4].Value = (object)job.LocationType ?? DBNull.Value;
            Param[5] = new SqlParameter("@pCustomerGUID", SqlDbType.UniqueIdentifier);
            Param[5].Value = (object)job.CustomerGUID ?? DBNull.Value;
            Param[6] = new SqlParameter("@pCustomerStopGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)job.CustomerStopGUID ?? DBNull.Value;

            Param[7] = new SqlParameter("@pServiceAddress", SqlDbType.NVarChar, -1);
            Param[7].Value = (object)job.ServiceAddress ?? DBNull.Value;
            Param[8] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[8].Value = (object)job.Latitude ?? DBNull.Value;
            Param[9] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[9].Value = (object)job.Longitude ?? DBNull.Value;
            Param[10] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
            Param[10].Value = (object)job.AssignedUserGUID ?? DBNull.Value;
            Param[11] = new SqlParameter("@pManagerUserGUID", SqlDbType.UniqueIdentifier);
            Param[11].Value = (object)job.ManagerUserGUID ?? DBNull.Value;
            Param[12] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[12].Value = (object)job.IsActive ?? DBNull.Value;
            Param[13] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[13].Value = (object)job.IsDeleted ?? DBNull.Value;
            Param[14] = new SqlParameter("@pIsUrgent", SqlDbType.Bit);
            Param[14].Value = (object)job.IsUrgent ?? DBNull.Value;
            Param[15] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[15].Value = (object)job.StatusCode ?? DBNull.Value;
            Param[16] = new SqlParameter("@pSubStatusCode", SqlDbType.Int);
            Param[16].Value = (object)job.SubStatusCode ?? DBNull.Value;
            Param[17] = new SqlParameter("@pIsSecheduled", SqlDbType.Bit);
            Param[17].Value = (object)job.IsSecheduled ?? DBNull.Value;
            Param[18] = new SqlParameter("@pJobName", SqlDbType.NVarChar, -1);
            Param[18].Value = (object)job.JobName ?? DBNull.Value;
            Param[19] = new SqlParameter("@pPreferedStartTime", SqlDbType.DateTime);
            Param[19].Value = (object)job.PreferedStartTime ?? DBNull.Value;
            Param[20] = new SqlParameter("@pPreferedEndTime", SqlDbType.DateTime);
            Param[20].Value = (object)job.PreferedEndTime ?? DBNull.Value;
            Param[21] = new SqlParameter("@pScheduledStartTime", SqlDbType.DateTime);
            Param[21].Value = job.ScheduledStartTime.ToString("MM-dd-yyyy") == "01-01-0001" ? DateTime.UtcNow.AddYears(-10) : job.ScheduledStartTime;
            Param[22] = new SqlParameter("@pActualStartTime", SqlDbType.DateTime);
            Param[22].Value = (object)job.ActualStartTime ?? DBNull.Value;




            Param[23] = new SqlParameter("@pJobClass", SqlDbType.SmallInt);
            Param[23].Value = (object)job.JobClass ?? DBNull.Value;



            Param[24] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[24].Value = (object)job.CreateDate ?? DBNull.Value;
            Param[25] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[25].Value = (object)job.CreateBy ?? DBNull.Value;
            Param[26] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[26].Value = (object)job.LastModifiedDate ?? DBNull.Value;
            Param[27] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[27].Value = (object)job.LastModifiedBy ?? DBNull.Value;


            return Param;
        }
        public SqlParameter[] SetParametersForPOCreate(Job job)
        {
            SqlParameter[] Param = new SqlParameter[29];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = job.JobGUID;// (object)globaluser.Role_Id ?? DBNull.Value;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = job.OrganizationGUID;
            Param[2] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)job.RegionGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)job.TerritoryGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pLocationType", SqlDbType.SmallInt);
            Param[4].Value = (object)job.LocationType ?? DBNull.Value;
            Param[5] = new SqlParameter("@pCustomerGUID", SqlDbType.UniqueIdentifier);
            Param[5].Value = (object)job.CustomerGUID ?? DBNull.Value;
            Param[6] = new SqlParameter("@pCustomerStopGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)job.CustomerStopGUID ?? DBNull.Value;

            Param[7] = new SqlParameter("@pServiceAddress", SqlDbType.NVarChar, -1);
            Param[7].Value = (object)job.ServiceAddress ?? DBNull.Value;
            Param[8] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[8].Value = (object)job.Latitude ?? DBNull.Value;
            Param[9] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[9].Value = (object)job.Longitude ?? DBNull.Value;
            Param[10] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
            Param[10].Value = (object)job.AssignedUserGUID ?? DBNull.Value;
            Param[11] = new SqlParameter("@pManagerUserGUID", SqlDbType.UniqueIdentifier);
            Param[11].Value = (object)job.ManagerUserGUID ?? DBNull.Value;
            Param[12] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[12].Value = (object)job.IsActive ?? DBNull.Value;
            Param[13] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[13].Value = (object)job.IsDeleted ?? DBNull.Value;
            Param[14] = new SqlParameter("@pIsUrgent", SqlDbType.Bit);
            Param[14].Value = (object)job.IsUrgent ?? DBNull.Value;
            Param[15] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[15].Value = (object)job.StatusCode ?? DBNull.Value;
            Param[16] = new SqlParameter("@pSubStatusCode", SqlDbType.Int);
            Param[16].Value = (object)job.SubStatusCode ?? DBNull.Value;
            Param[17] = new SqlParameter("@pIsSecheduled", SqlDbType.Bit);
            Param[17].Value = (object)job.IsSecheduled ?? DBNull.Value;
            Param[18] = new SqlParameter("@pJobName", SqlDbType.NVarChar, -1);
            Param[18].Value = (object)job.JobName ?? DBNull.Value;
            Param[19] = new SqlParameter("@pPreferedStartTime", SqlDbType.DateTime);
            Param[19].Value = (object)job.PreferedStartTime ?? DBNull.Value;
            Param[20] = new SqlParameter("@pPreferedEndTime", SqlDbType.DateTime);
            Param[20].Value = (object)job.PreferedEndTime ?? DBNull.Value;
            Param[21] = new SqlParameter("@pScheduledStartTime", SqlDbType.DateTime);
            Param[21].Value = job.ScheduledStartTime.ToString("MM-dd-yyyy") == "01-01-0001" ? DateTime.UtcNow.AddYears(-10) : job.ScheduledStartTime;
            Param[22] = new SqlParameter("@pActualStartTime", SqlDbType.DateTime);
            Param[22].Value = (object)job.ActualStartTime ?? DBNull.Value;




            Param[23] = new SqlParameter("@pJobClass", SqlDbType.SmallInt);
            Param[23].Value = (object)job.JobClass ?? DBNull.Value;



            Param[24] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[24].Value = (object)job.CreateDate ?? DBNull.Value;
            Param[25] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[25].Value = (object)job.CreateBy ?? DBNull.Value;
            Param[26] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[26].Value = (object)job.LastModifiedDate ?? DBNull.Value;
            Param[27] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[27].Value = (object)job.LastModifiedBy ?? DBNull.Value;
            Param[28] = new SqlParameter("@pPONumber", SqlDbType.NVarChar, 50);
            Param[28].Value = (object)job.PONumber ?? DBNull.Value;

            return Param;
        }
        public SqlParameter[] SetParametersforUpdate(Job job)
        {
            SqlParameter[] Param = new SqlParameter[13];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = job.JobGUID;// (object)globaluser.Role_Id ?? DBNull.Value;
            Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[1].Value = (object)job.StatusCode ?? DBNull.Value;
            Param[2] = new SqlParameter("@pSubStatusCode", SqlDbType.Int);
            Param[2].Value = (object)job.SubStatusCode ?? DBNull.Value;
            Param[3] = new SqlParameter("@pActualStartTime", SqlDbType.DateTime);
            Param[3].Value = (object)job.ActualStartTime ?? DBNull.Value;
            Param[4] = new SqlParameter("@pActualEndTime", SqlDbType.DateTime);
            Param[4].Value = (object)job.ActualEndTime ?? DBNull.Value;
            Param[5] = new SqlParameter("@pPreferedEndTime", SqlDbType.DateTime);
            Param[5].Value = (object)job.PreferedEndTime ?? DBNull.Value;
            Param[6] = new SqlParameter("@pScheduledEndTime", SqlDbType.DateTime);
            Param[6].Value = (object)job.ScheduledEndTime ?? DBNull.Value;
            Param[7] = new SqlParameter("@pEstimatedDuration", SqlDbType.Float);
            Param[7].Value = (object)job.EstimatedDuration ?? DBNull.Value;
            Param[8] = new SqlParameter("@pQuotedDuration", SqlDbType.Float);
            Param[8].Value = (object)job.QuotedDuration ?? DBNull.Value;
            Param[9] = new SqlParameter("@pActualDuration", SqlDbType.Float);
            Param[9].Value = (object)job.ActualDuration ?? DBNull.Value;
            Param[10] = new SqlParameter("@pJobForm", SqlDbType.Text);
            Param[10].Value = (object)job.JobForm ?? DBNull.Value;
            Param[11] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[11].Value = (object)job.LastModifiedDate ?? DBNull.Value;
            Param[12] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)job.LastModifiedBy ?? DBNull.Value;

            return Param;
        }
        public SqlParameter[] SetParameters(Job job)
        {
            SqlParameter[] Param = new SqlParameter[46];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = job.JobGUID;// (object)globaluser.Role_Id ?? DBNull.Value;
            Param[1] = new SqlParameter("@pJobReferenceNo", SqlDbType.NVarChar, -1);
            Param[1].Value = (object)job.JobReferenceNo ?? DBNull.Value;
            Param[2] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = job.OrganizationGUID;
            Param[3] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)job.RegionGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[4].Value = (object)job.TerritoryGUID ?? DBNull.Value;
            Param[5] = new SqlParameter("@pLocationType", SqlDbType.SmallInt);
            Param[5].Value = (object)job.LocationType ?? DBNull.Value;
            Param[6] = new SqlParameter("@pCustomerGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)job.CustomerGUID ?? DBNull.Value;
            Param[7] = new SqlParameter("@pCustomerStopGUID", SqlDbType.UniqueIdentifier);
            Param[7].Value = (object)job.CustomerStopGUID ?? DBNull.Value;
            Param[8] = new SqlParameter("@pServicePointGUID", SqlDbType.UniqueIdentifier);
            Param[8].Value = (object)job.ServicePointGUID ?? DBNull.Value;
            Param[9] = new SqlParameter("@pServiceAddress", SqlDbType.NVarChar, -1);
            Param[9].Value = (object)job.ServiceAddress ?? DBNull.Value;
            Param[10] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[10].Value = (object)job.Latitude ?? DBNull.Value;
            Param[11] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[11].Value = (object)job.Longitude ?? DBNull.Value;
            Param[12] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)job.AssignedUserGUID ?? DBNull.Value;
            Param[13] = new SqlParameter("@pManagerUserGUID", SqlDbType.UniqueIdentifier);
            Param[13].Value = (object)job.ManagerUserGUID ?? DBNull.Value;
            Param[14] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[14].Value = (object)job.IsActive ?? DBNull.Value;
            Param[15] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[15].Value = (object)job.IsDeleted ?? DBNull.Value;
            Param[16] = new SqlParameter("@pIsUrgent", SqlDbType.Bit);
            Param[16].Value = (object)job.IsUrgent ?? DBNull.Value;
            Param[17] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[17].Value = (object)job.StatusCode ?? DBNull.Value;
            Param[18] = new SqlParameter("@pSubStatusCode", SqlDbType.Int);
            Param[18].Value = (object)job.SubStatusCode ?? DBNull.Value;
            Param[19] = new SqlParameter("@pIsSecheduled", SqlDbType.Bit);
            Param[19].Value = (object)job.IsSecheduled ?? DBNull.Value;
            Param[20] = new SqlParameter("@pJobName", SqlDbType.NVarChar, -1);
            Param[20].Value = (object)job.JobName ?? DBNull.Value;
            Param[21] = new SqlParameter("@pPreferedStartTime", SqlDbType.DateTime);
            Param[21].Value = (object)job.PreferedStartTime ?? DBNull.Value;
            Param[22] = new SqlParameter("@pPreferedEndTime", SqlDbType.DateTime);
            Param[22].Value = (object)job.PreferedEndTime ?? DBNull.Value;
            Param[23] = new SqlParameter("@pScheduledStartTime", SqlDbType.DateTime);
            Param[23].Value = job.ScheduledStartTime.ToString("MM-dd-yyyy") == "01-01-0001" ? DateTime.UtcNow.AddYears(-10) : job.ScheduledStartTime;
            Param[24] = new SqlParameter("@pScheduledEndTime", SqlDbType.DateTime);
            Param[24].Value = (object)job.ScheduledEndTime ?? DBNull.Value;
            Param[25] = new SqlParameter("@pActualStartTime", SqlDbType.DateTime);
            Param[25].Value = (object)job.ActualStartTime ?? DBNull.Value;
            Param[26] = new SqlParameter("@pActualEndTime", SqlDbType.DateTime);
            Param[26].Value = (object)job.ActualEndTime ?? DBNull.Value;
            Param[27] = new SqlParameter("@pEstimatedDuration", SqlDbType.Float);
            Param[27].Value = (object)job.EstimatedDuration ?? DBNull.Value;
            Param[28] = new SqlParameter("@pQuotedDuration", SqlDbType.Float);
            Param[28].Value = (object)job.QuotedDuration ?? DBNull.Value;
            Param[29] = new SqlParameter("@pActualDuration", SqlDbType.Float);
            Param[29].Value = (object)job.ActualDuration ?? DBNull.Value;
            Param[30] = new SqlParameter("@pCostType", SqlDbType.SmallInt);
            Param[30].Value = (object)job.CostType ?? DBNull.Value;
            Param[31] = new SqlParameter("@pQuotedCost", SqlDbType.Float);
            Param[31].Value = (object)job.QuotedCost ?? DBNull.Value;
            Param[32] = new SqlParameter("@pActualCost", SqlDbType.Float);
            Param[32].Value = (object)job.ActualCost ?? DBNull.Value;
            Param[33] = new SqlParameter("@pJobForm", SqlDbType.Text);
            Param[33].Value = (object)job.JobForm ?? DBNull.Value;
            Param[34] = new SqlParameter("@pJobClass", SqlDbType.SmallInt);
            Param[34].Value = (object)job.JobClass ?? DBNull.Value;
            Param[35] = new SqlParameter("@pSignOffRequired", SqlDbType.SmallInt);
            Param[35].Value = (object)job.SignOffRequired ?? DBNull.Value;
            Param[36] = new SqlParameter("@pSignoffName", SqlDbType.NVarChar, -1);
            Param[36].Value = (object)job.SignoffName ?? DBNull.Value;
            Param[37] = new SqlParameter("@pPictureRequired", SqlDbType.SmallInt);
            Param[37].Value = (object)job.PictureRequired ?? DBNull.Value;
            Param[38] = new SqlParameter("@pPictureDescription", SqlDbType.NVarChar, -1);
            Param[38].Value = (object)job.PictureDescription ?? DBNull.Value;
            Param[39] = new SqlParameter("@pLocationSpecific", SqlDbType.Bit);
            Param[39].Value = (object)job.LocationSpecific ?? DBNull.Value;
            Param[40] = new SqlParameter("@pPONumber", SqlDbType.NVarChar, 50);
            Param[40].Value = (object)job.PONumber ?? DBNull.Value;
            Param[41] = new SqlParameter("@pTermsURL", SqlDbType.NVarChar, -1);
            Param[41].Value = (object)job.TermsURL ?? DBNull.Value;
            Param[42] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[42].Value = (object)job.CreateDate ?? DBNull.Value;
            Param[43] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[43].Value = (object)job.CreateBy ?? DBNull.Value;
            Param[44] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[44].Value = (object)job.LastModifiedDate ?? DBNull.Value;
            Param[45] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[45].Value = (object)job.LastModifiedBy ?? DBNull.Value;

            return Param;
        }
        public IEnumerable<dynamic> Job_UserActivityGraph(Job pJob)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    //List<Job> joblist = (from p in dataContext.Jobs
                //    //                     where
                //    //                     (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
                //    //                     && (pJob.CustomerGUID == null || pJob.CustomerGUID == Guid.Empty || p.CustomerGUID == pJob.CustomerGUID)
                //    //                     && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
                //    //                     && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
                //    //                     && (pJob.AssignedUserGUID == null || pJob.AssignedUserGUID == Guid.Empty || p.AssignedUserGUID == pJob.AssignedUserGUID)
                //    //                         //&& ((p.PreferedStartTime == null || (p.PreferedStartTime != null && Convert.ToDateTime(p.PreferedStartTime.Value.Date).Date >= pJob.PreferedStartTime.Value.Date)) && (p.PreferedEndTime == null || (p.PreferedEndTime != null && Convert.ToDateTime(p.PreferedEndTime.Value.Date).Date <= pJob.PreferedEndTime.Value.Date)))
                //    //                     && p.IsDeleted == false && p.StatusCode != 1
                //    //                     && p.JobName == "Store Visit"
                //    //                     select p).OrderBy(x => x.ScheduledStartTime).ToList();
                //    SqlParameter[] Param = new SqlParameter[46];
                //    Param = SetParameters(pJob);
                //    List<Job> joblist = context.Database.SqlQuery<Job>("select * from Jobs where"
                //                    + "(OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "AND (CustomerGUID=@pCustomerGUID OR @pCustomerGUID is NULL or @pCustomerGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "and IsDeleted=0 and StatusCode!=1 and JobName='Store Visit' Order by ScheduledStartTime", Param).ToList();

                //    if (joblist != null && joblist.Count > 0)
                //    {
                //        if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
                //            joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
                //        var groups = joblist.GroupBy(item => new { Convert.ToDateTime(item.CreateDate.Value.Date).Date }).Select(item => new
                //        {
                //            datevalue = item.Key.Date,
                //            count = item.Count()
                //        }).ToList();

                //        return groups;
                //    }
                //    else
                //        return null;
                //    //var joblist = (from p in dataContext.Jobs
                //    //               where (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
                //    //                && (pJob.AssignedUserGUID == null || pJob.AssignedUserGUID == Guid.Empty || p.AssignedUserGUID == pJob.AssignedUserGUID)
                //    //                && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
                //    //               && p.IsDeleted == false
                //    //               && p.StatusCode != 1
                //    //               select p).OrderBy(x => x.CreateDate).ToList();
                //    //if (joblist != null && joblist.Count > 0)
                //    //{
                //    //    if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
                //    //        joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
                //    //}

                //}
                //SqlParameter[] Param = new SqlParameter[46];
                //Param = SetParameters(pJob);



                SqlParameter[] Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = pJob.OrganizationGUID;
                Param[1] = new SqlParameter("@pCustomerGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = (object)pJob.CustomerGUID ?? DBNull.Value;
                Param[2] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[2].Value = (object)pJob.RegionGUID ?? DBNull.Value;
                Param[3] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
                Param[3].Value = (object)pJob.TerritoryGUID ?? DBNull.Value;




                List<Job> joblist = context.Database.SqlQuery<Job>("select * from Jobs where"
                                + "(OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (CustomerGUID=@pCustomerGUID OR @pCustomerGUID is NULL or @pCustomerGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " and IsDeleted=0 and StatusCode!=1 and JobName='Store Visit' Order by ScheduledStartTime", Param).ToList();

                if (joblist != null && joblist.Count > 0)
                {
                    if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
                        joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
                    var groups = joblist.GroupBy(item => new { Convert.ToDateTime(item.CreateDate.Value.Date).Date }).Select(item => new
                    {
                        datevalue = item.Key.Date,
                        count = item.Count()
                    }).ToList();

                    return groups;
                }
                else
                    return null;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        public List<Job> GetJobs(Job pJob)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    //List<Job> joblist = (from p in dataContext.Jobs
                //    //                     where
                //    //                     (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
                //    //                     && (pJob.AssignedUserGUID == null || pJob.AssignedUserGUID == Guid.Empty || p.AssignedUserGUID == pJob.AssignedUserGUID)
                //    //                     && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
                //    //                     && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
                //    //                     && (pJob.PONumber == null || pJob.PONumber == "" || p.PONumber == pJob.PONumber)
                //    //                     && (pJob.JobGUID == Guid.Empty || p.JobGUID == pJob.JobGUID)
                //    //                     && p.IsDeleted == false
                //    //                     select p).OrderBy(x => x.ScheduledStartTime).ToList();

                //    SqlParameter[] Param = new SqlParameter[46];
                //    Param = SetParameters(pJob);
                //    List<Job> joblist = context.Database.SqlQuery<Job>("select * from Jobs where"
                //                    + "(OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "AND (AssignedUserGUID=@pAssignedUserGUID OR @pAssignedUserGUID is NULL or @pAssignedUserGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "AND (JobGUID=@pJobGUID or @pJobGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                //                    + "AND (PONumber=@pPONumber OR @pPONumber is NULL or @pPONumber=''"
                //                    + "and IsDeleted=0"
                //                    + "Order by ScheduledStartTime", Param).ToList();


                //    if (joblist != null && joblist.Count > 0)
                //    {
                //        if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
                //            joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
                //        return joblist;
                //    }
                //    return null;

                //}
                //SqlParameter[] Param = new SqlParameter[46];
                //Param = SetParameters(pJob);


                SqlParameter[] Param = new SqlParameter[6];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = pJob.OrganizationGUID;
                Param[1] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = pJob.JobGUID;
                Param[2] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[2].Value = (object)pJob.RegionGUID ?? DBNull.Value;
                Param[3] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
                Param[3].Value = (object)pJob.TerritoryGUID ?? DBNull.Value;
                Param[4] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                Param[4].Value = (object)pJob.AssignedUserGUID ?? DBNull.Value;
                Param[5] = new SqlParameter("@pPONumber", SqlDbType.NVarChar, 50);
                Param[5].Value = (object)pJob.PONumber ?? DBNull.Value;




                List<Job> joblist = context.Database.SqlQuery<Job>("select * from Jobs where"
                                + "(OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (AssignedUserGUID=@pAssignedUserGUID OR @pAssignedUserGUID is NULL or @pAssignedUserGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (JobGUID=@pJobGUID or @pJobGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (PONumber=@pPONumber OR @pPONumber is NULL or @pPONumber='')"
                                + " and IsDeleted=0 "
                                + " Order by ScheduledStartTime", Param).ToList();


                if (joblist != null && joblist.Count > 0)
                {
                    if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
                        joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
                    return joblist;
                }
                return null;

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public List<Job> GetOpenJobs(Job pJob)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.Jobs
                //            where
                //            (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
                //                // && (pJob.AssignedUserGUID == null || pJob.AssignedUserGUID == Guid.Empty || p.AssignedUserGUID == pJob.AssignedUserGUID)
                //            && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
                //            && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
                //            && p.StatusCode == 1 && p.IsDeleted == false
                //            select p).OrderBy(x => x.ScheduledStartTime).ToList();

                //}
                //SqlParameter[] Param = new SqlParameter[46];
                //Param = SetParameters(pJob);


                SqlParameter[] Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = pJob.OrganizationGUID;
                Param[1] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = (object)pJob.RegionGUID ?? DBNull.Value;
                Param[2] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
                Param[2].Value = (object)pJob.TerritoryGUID ?? DBNull.Value;


                return context.Database.SqlQuery<Job>("select * from Jobs where"
                                + "(OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                + " and IsDeleted=0 and StatusCode=1"
                                + " Order by ScheduledStartTime", Param).ToList();

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public List<Job> GetJobStatus(Job pJob)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.Jobs
                //            where
                //            (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
                //            && (pJob.AssignedUserGUID == null || pJob.AssignedUserGUID == Guid.Empty || p.AssignedUserGUID == pJob.AssignedUserGUID)
                //            && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
                //            && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
                //            && (pJob.JobGUID == Guid.Empty || p.JobGUID == pJob.TerritoryGUID)
                //            && p.StatusCode >= 2 && p.IsDeleted == false
                //            select p).OrderBy(x => x.ScheduledStartTime).ToList();

                //}
                //SqlParameter[] Param = new SqlParameter[46];
                //Param = SetParameters(pJob);


                SqlParameter[] Param = new SqlParameter[5];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = pJob.OrganizationGUID;
                Param[1] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = pJob.JobGUID;
                Param[2] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[2].Value = (object)pJob.RegionGUID ?? DBNull.Value;
                Param[3] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
                Param[3].Value = (object)pJob.TerritoryGUID ?? DBNull.Value;
                Param[4] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                Param[4].Value = (object)pJob.AssignedUserGUID ?? DBNull.Value;


                return context.Database.SqlQuery<Job>("select * from Jobs where"
                                    + " (OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (AssignedUserGUID=@pAssignedUserGUID OR @pAssignedUserGUID is NULL or @pAssignedUserGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (JobGUID=@pJobGUID or @pJobGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " and IsDeleted=0 and StatusCode >=2"
                                    + " Order by ScheduledStartTime", Param).ToList();


            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public List<Job> GetJobByUserGUID(Guid UserGUID)
        {
            try
            {
                //((OrganizationGUID==Guid.Empty ||  p.OrganizationGUID == OrganizationGUID ) )

                //SqlParameter[] sqlParam = new SqlParameter[2];
                //sqlParam[0] = new SqlParameter("@UserGUID", SqlDbType.UniqueIdentifier);
                //sqlParam[0].Value = UserGUID;
                //sqlParam[1] = new SqlParameter("@pErrorCode", SqlDbType.NVarChar, 200);
                //sqlParam[1].Value = "";
                //sqlParam[1].Direction = ParameterDirection.Output;
                //_jmResponse.JobList = context.Database.SqlQuery<p_GetJobs_Result>("dbo.p_getjobs  @UserGUID,@pErrorCode=@pErrorCode output", sqlParam).ToList();
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.Jobs
                //            where p.AssignedUserGUID == UserGUID
                //            select p).OrderBy(x => x.ScheduledStartTime).ToList();

                //}
                SqlParameter[] Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = UserGUID;
                return context.Database.SqlQuery<Job>("select * from Jobs where UserGUID=@pUserGUID", Param).ToList();

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public Job GetJobByCustomerStopGUID(Guid CustomerStopGUID)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.Jobs
                //            where p.CustomerStopGUID == CustomerStopGUID
                //            select p).FirstOrDefault();
                //}
                SqlParameter[] Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@pCustomerStopGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = CustomerStopGUID;
                return context.Database.SqlQuery<Job>("select * from Jobs where CustomerStopGUID=@pCustomerStopGUID", Param).FirstOrDefault();

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        public List<Job> GetStoreVisitJobs(Job pJob)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    List<Job> joblist = (from p in dataContext.Jobs
                //                         where
                //                         (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
                //                         && (pJob.CustomerGUID == null || pJob.CustomerGUID == Guid.Empty || p.CustomerGUID == pJob.CustomerGUID)
                //                         && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
                //                         && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
                //                         && (pJob.AssignedUserGUID == null || pJob.AssignedUserGUID == Guid.Empty || p.AssignedUserGUID == pJob.AssignedUserGUID)
                //                             //&& ((p.PreferedStartTime == null || (p.PreferedStartTime != null && Convert.ToDateTime(p.PreferedStartTime.Value.Date).Date >= pJob.PreferedStartTime.Value.Date)) && (p.PreferedEndTime == null || (p.PreferedEndTime != null && Convert.ToDateTime(p.PreferedEndTime.Value.Date).Date <= pJob.PreferedEndTime.Value.Date)))
                //                         && p.IsDeleted == false && p.StatusCode != 1
                //                         && p.JobName == pJob.JobName
                //                         select p).OrderBy(x => x.ScheduledStartTime).ToList();

                //    if (joblist != null && joblist.Count > 0)
                //    {
                //        if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
                //            joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
                //        return joblist;
                //    }
                //    return null;


                //}
                //SqlParameter[] Param = new SqlParameter[46];
                //Param = SetParameters(pJob);




                SqlParameter[] Param = new SqlParameter[6];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = pJob.OrganizationGUID;
                Param[1] = new SqlParameter("@pCustomerGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = (object)pJob.CustomerGUID ?? DBNull.Value;
                Param[2] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[2].Value = (object)pJob.RegionGUID ?? DBNull.Value;
                Param[3] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
                Param[3].Value = (object)pJob.TerritoryGUID ?? DBNull.Value;
                Param[4] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                Param[4].Value = (object)pJob.AssignedUserGUID ?? DBNull.Value;
                Param[5] = new SqlParameter("@pJobName", SqlDbType.NVarChar, -1);
                Param[5].Value = (object)pJob.JobName ?? DBNull.Value;



                List<Job> joblist = context.Database.SqlQuery<Job>("select * from Jobs where"
                                    + " (OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (CustomerGUID=@pCustomerGUID OR @pCustomerGUID is NULL or @pCustomerGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (AssignedUserGUID=@pAssignedUserGUID OR @pAssignedUserGUID is NULL or @pAssignedUserGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " and IsDeleted=0 and StatusCode !=1 and JobName=@pJobName"
                                    + " Order by ScheduledStartTime", Param).ToList();
                if (joblist != null && joblist.Count > 0)
                {
                    if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
                        joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
                    return joblist;
                }
                return null;


            }
            catch (Exception exception)
            {
                throw exception;
            }


        }

        //public List<Job> GetStoreVisitJobs(Job pJob, string FieldManagerID)
        //{
        //    try
        //    {
        //        List<Job> ljob = new List<Job>();
        //        //using (var dataContext = new WorkersInMotionDB())
        //        //{
        //        //    List<Market> MarketList = (from p in dataContext.Markets
        //        //                               where
        //        //                               (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
        //        //                               && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
        //        //                               && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
        //        //                               && (p.FMUserID == FieldManagerID)
        //        //                                   //&& ((p.CreateDate == null || (p.CreateDate != null && Convert.ToDateTime(p.CreateDate.Value.Date).Date >= pjob.ActualStartTime.Value.Date)) && (p.CreateDate == null || (p.CreateDate != null && Convert.ToDateTime(p.CreateDate.Value.Date).Date <= pjob.ActualEndTime.Value.Date)))
        //        //                               && p.IsDeleted == null || p.IsDeleted == false
        //        //                               select p).OrderBy(x => x.MarketName).ToList();

        //        //    if (MarketList != null && MarketList.Count > 0)
        //        //    {
        //        //        foreach (Market market in MarketList)
        //        //        {
        //        //            List<Job> joblist = (from p in dataContext.Jobs
        //        //                                 where
        //        //                                 (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
        //        //                                 && (pJob.CustomerGUID == null || pJob.CustomerGUID == Guid.Empty || p.CustomerGUID == pJob.CustomerGUID)
        //        //                                 && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
        //        //                                 && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
        //        //                                 && (p.CustomerStopGUID == market.MarketGUID)
        //        //                                     //&& ((p.PreferedStartTime == null || (p.PreferedStartTime != null && Convert.ToDateTime(p.PreferedStartTime.Value.Date).Date >= pJob.PreferedStartTime.Value.Date)) && (p.PreferedEndTime == null || (p.PreferedEndTime != null && Convert.ToDateTime(p.PreferedEndTime.Value.Date).Date <= pJob.PreferedEndTime.Value.Date)))
        //        //                                 && p.IsDeleted == false && p.StatusCode != 1
        //        //                                 && p.JobName == pJob.JobName
        //        //                                 select p).OrderBy(x => x.ScheduledStartTime).ToList();


        //        //            if (joblist != null && joblist.Count > 0)
        //        //            {
        //        //                foreach (Job jobitem in joblist)
        //        //                {
        //        //                    ljob.Add(jobitem);
        //        //                }
        //        //            }
        //        //        }
        //        //    }
        //        //    if (ljob != null && ljob.Count > 0)
        //        //    {
        //        //        if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
        //        //            ljob = ljob.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
        //        //        return ljob;
        //        //    }
        //        //    return null;


        //        //}




        //        SqlParameter[] Param = new SqlParameter[46];
        //        Param = SetParameters(pJob);
        //        List<Job> joblist = context.Database.SqlQuery<Job>("select * from Jobs where"
        //                            + "(OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
        //                            + "AND (CustomerGUID=@pCustomerGUID OR @pCustomerGUID is NULL or @pCustomerGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
        //                            + "AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
        //                            + "AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
        //                            + "and IsDeleted=0 and StatusCode !=1 and JobName=@pJobName"
        //                            + "and CustomerStopGUID in(select MarketGUID from Markets where"
        //                    + "(OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
        //                    + "AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
        //                    + "AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
        //                    + "AND (AssignedUserGUID=@pAssignedUserGUID OR @pAssignedUserGUID is NULL or @pAssignedUserGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
        //                    + "AND (IsDeleted=0 OR @pIsDeleted is NULL)"
        //                    + "AND FMUserID='" + FieldManagerID + "')"
        //                            + " Order by ScheduledStartTime", Param).ToList();


        //        if (joblist != null && joblist.Count > 0)
        //        {
        //            if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
        //                joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
        //            return joblist;
        //        }


        //        return null;


        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }


        //}
        //public List<Market> GetStoreVisitJobs(Guid OrganizationGUID, Nullable<Guid> CustomerGUID)
        //{
        //    try
        //    {
        //        using (var dataContext = new WorkersInMotionDB())
        //        {
        //            //DateTime date = DateTime.ParseExact(DateTime.UtcNow.AddDays(-45).ToShortDateString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //            DateTime date = DateTime.UtcNow.AddDays(-45);
        //            if (CustomerGUID == null && CustomerGUID != Guid.Empty)
        //            {
        //                List<Market> MarketList = (from p in dataContext.Markets
        //                                           where
        //                                           (OrganizationGUID == Guid.Empty || p.OrganizationGUID == OrganizationGUID)
        //                                           && p.IsDeleted == null || p.IsDeleted == false
        //                                           select p).OrderBy(x => x.MarketName).ToList();

        //                if (MarketList != null && MarketList.Count > 0)
        //                {
        //                    MarketList = MarketList.Where(p => p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date > date.Date).ToList();
        //                    return MarketList;
        //                }
        //                return null;
        //            }
        //            else
        //            {
        //                List<Market> MarketList = (from p in dataContext.Markets
        //                                           where
        //                                           (OrganizationGUID == Guid.Empty || p.OrganizationGUID == OrganizationGUID)
        //                                           && p.OwnerGUID == CustomerGUID
        //                                           && p.IsDeleted == null || p.IsDeleted == false
        //                                           select p).OrderBy(x => x.MarketName).ToList();

        //                if (MarketList != null && MarketList.Count > 0)
        //                {
        //                    MarketList = MarketList.Where(p => p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date > date.Date).ToList();
        //                    return MarketList;
        //                }
        //                return null;
        //            }
        //        }


        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}


        public List<Job> GetSiteVisitJobs(Job pJob)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    List<Job> joblist = (from p in dataContext.Jobs
                //                         where
                //                         (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
                //                         && (pJob.CustomerGUID == null || pJob.CustomerGUID == Guid.Empty || p.CustomerGUID == pJob.CustomerGUID)
                //                         && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
                //                         && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
                //                         && (pJob.AssignedUserGUID == null || pJob.AssignedUserGUID == Guid.Empty || p.AssignedUserGUID == pJob.AssignedUserGUID)
                //                             //&& ((p.PreferedStartTime == null || (p.PreferedStartTime != null && Convert.ToDateTime(p.PreferedStartTime.Value.Date).Date >= pJob.PreferedStartTime.Value.Date)) && (p.PreferedEndTime == null || (p.PreferedEndTime != null && Convert.ToDateTime(p.PreferedEndTime.Value.Date).Date <= pJob.PreferedEndTime.Value.Date)))
                //                         && p.IsDeleted == false && p.StatusCode != 1
                //                         && p.JobName != pJob.JobName
                //                         select p).OrderBy(x => x.ScheduledStartTime).ToList();
                //    if (joblist != null && joblist.Count > 0)
                //    {
                //        if (pJob.PreferedStartTime != null && pJob.PreferedEndTime != null)
                //            joblist = joblist.Where(p => (p.PreferedStartTime == null || (p.PreferedStartTime != null && Convert.ToDateTime(p.PreferedStartTime.Value.Date).Date >= pJob.PreferedStartTime.Value.Date)) && (p.PreferedEndTime == null || (p.PreferedEndTime != null && Convert.ToDateTime(p.PreferedEndTime.Value.Date).Date <= pJob.PreferedEndTime.Value.Date))).ToList();
                //        return joblist;
                //    }
                //    return null;

                //}
                //SqlParameter[] Param = new SqlParameter[46];
                //Param = SetParameters(pJob);


                SqlParameter[] Param = new SqlParameter[6];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = pJob.OrganizationGUID;
                Param[1] = new SqlParameter("@pCustomerGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = (object)pJob.CustomerGUID ?? DBNull.Value;
                Param[2] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[2].Value = (object)pJob.RegionGUID ?? DBNull.Value;
                Param[3] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
                Param[3].Value = (object)pJob.TerritoryGUID ?? DBNull.Value;
                Param[4] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                Param[4].Value = (object)pJob.AssignedUserGUID ?? DBNull.Value;
                Param[5] = new SqlParameter("@pJobName", SqlDbType.NVarChar, -1);
                Param[5].Value = (object)pJob.JobName ?? DBNull.Value;




                List<Job> joblist = context.Database.SqlQuery<Job>("select * from Jobs where"
                                   + " (OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                   + " AND (CustomerGUID=@pCustomerGUID OR @pCustomerGUID is NULL or @pCustomerGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                   + " AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                   + " AND (TerritoryGUID=@pTerritoryGUID OR @pTerritoryGUID is NULL or @pTerritoryGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                   + " AND (AssignedUserGUID=@pAssignedUserGUID OR @pAssignedUserGUID is NULL or @pAssignedUserGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                   + " and IsDeleted=0 and StatusCode !=1 and JobName != @pJobName"
                                   + " Order by ScheduledStartTime", Param).ToList();
                if (joblist != null && joblist.Count > 0)
                {
                    if (pJob.PreferedStartTime != null && pJob.PreferedEndTime != null)
                        joblist = joblist.Where(p => (p.PreferedStartTime == null || (p.PreferedStartTime != null && Convert.ToDateTime(p.PreferedStartTime.Value.Date).Date >= pJob.PreferedStartTime.Value.Date)) && (p.PreferedEndTime == null || (p.PreferedEndTime != null && Convert.ToDateTime(p.PreferedEndTime.Value.Date).Date <= pJob.PreferedEndTime.Value.Date))).ToList();
                    return joblist;
                }
                return null;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        //public List<Market> GetStoreNonVisitJobsForDashboard(Job pJob)
        //{
        //    try
        //    {
        //        using (var dataContext = new WorkersInMotionDB())
        //        {

        //            //DateTime date = DateTime.ParseExact(DateTime.UtcNow.AddDays(-45).ToShortDateString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //            //DateTime date = DateTime.UtcNow.AddDays(-45);
        //            DateTime date = pJob.LastModifiedDate != null ? Convert.ToDateTime(pJob.LastModifiedDate).ToUniversalTime() : DateTime.UtcNow.AddDays(-45);

        //            List<Job> joblist = (from p in dataContext.Jobs
        //                                 where
        //                                 (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
        //                                 && (pJob.CustomerGUID == null || pJob.CustomerGUID == Guid.Empty || p.CustomerGUID == pJob.CustomerGUID)
        //                                 && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
        //                                 && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
        //                                 && (pJob.AssignedUserGUID == null || pJob.AssignedUserGUID == Guid.Empty || p.AssignedUserGUID == pJob.AssignedUserGUID)
        //                                     //&& ((p.PreferedStartTime == null || (p.PreferedStartTime != null && Convert.ToDateTime(p.PreferedStartTime.Value.Date).Date >= pJob.PreferedStartTime.Value.Date)) && (p.PreferedEndTime == null || (p.PreferedEndTime != null && Convert.ToDateTime(p.PreferedEndTime.Value.Date).Date <= pJob.PreferedEndTime.Value.Date)))
        //                                 && p.IsDeleted == false
        //                                 && p.JobName == pJob.JobName
        //                                 select p).OrderBy(x => x.ScheduledStartTime).ToList();
        //            if (joblist != null && joblist.Count > 0)
        //            {
        //                if (pJob.PreferedStartTime != null && pJob.PreferedEndTime != null)
        //                    joblist = joblist.Where(p => (p.PreferedStartTime == null || (p.PreferedStartTime != null && Convert.ToDateTime(p.PreferedStartTime.Value.Date).Date >= pJob.PreferedStartTime.Value.Date)) && (p.PreferedEndTime == null || (p.PreferedEndTime != null && Convert.ToDateTime(p.PreferedEndTime.Value.Date).Date <= pJob.PreferedEndTime.Value.Date))).ToList();
        //                else if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
        //                    joblist = joblist.Where(p => (p.ActualStartTime == null || (p.ActualStartTime != null && Convert.ToDateTime(p.ActualStartTime.Value.Date).Date >= pJob.ActualStartTime.Value.Date)) && (p.ActualEndTime == null || (p.ActualEndTime != null && Convert.ToDateTime(p.ActualEndTime.Value.Date).Date <= pJob.ActualEndTime.Value.Date))).ToList();
        //            }
        //            List<Market> MarketList = new List<Market>();
        //            if (joblist != null && joblist.Count > 0)
        //            {
        //                foreach (Job _job in joblist)
        //                {
        //                    MarketList.Add((from p in dataContext.Markets
        //                                    where p.MarketGUID == _job.CustomerStopGUID
        //                                    && (p.IsDeleted == null || p.IsDeleted == false)
        //                                    select p).FirstOrDefault());
        //                }

        //            }

        //            if (MarketList != null && MarketList.Count > 0)
        //            {
        //                //MarketList = MarketList.GroupBy(i => i.MarketID).Select(group => group.First()).ToList();
        //                //if (MarketList != null && MarketList.Count > 0)
        //                {
        //                    if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
        //                    {
        //                        MarketList = MarketList.Where(p => (p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= pJob.ActualStartTime.Value.Date))).ToList();
        //                        return MarketList;
        //                    }
        //                    else if (pJob.PreferedStartTime != null && pJob.PreferedEndTime != null)
        //                    {
        //                        MarketList = MarketList.Where(p => (p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= pJob.PreferedStartTime.Value.Date))).ToList();
        //                        return MarketList;
        //                    }
        //                }
        //            }
        //            return null;


        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}

        //public List<Market> GetStoreNonVisitJobs(Job pjob, Nullable<Guid> CustomerGUID)
        //{
        //    try
        //    {
        //        using (var dataContext = new WorkersInMotionDB())
        //        {

        //            //DateTime date = DateTime.ParseExact(DateTime.UtcNow.AddDays(-45).ToShortDateString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //            //DateTime date = DateTime.UtcNow.AddDays(-45);
        //            DateTime date = pjob.LastModifiedDate != null ? Convert.ToDateTime(pjob.LastModifiedDate).ToUniversalTime() : DateTime.UtcNow.AddDays(-45);
        //            if (CustomerGUID == null && CustomerGUID != Guid.Empty)
        //            {
        //                List<Market> MarketList = (from p in dataContext.Markets
        //                                           where
        //                                           (pjob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pjob.OrganizationGUID)
        //                                               //&& ((p.CreateDate == null || (p.CreateDate != null && Convert.ToDateTime(p.CreateDate.Value.Date).Date >= pjob.ActualStartTime.Value.Date)) && (p.CreateDate == null || (p.CreateDate != null && Convert.ToDateTime(p.CreateDate.Value.Date).Date <= pjob.ActualEndTime.Value.Date)))
        //                                           && p.IsDeleted == null || p.IsDeleted == false
        //                                           select p).OrderBy(x => x.MarketName).ToList();

        //                if (MarketList != null && MarketList.Count > 0)
        //                {
        //                    if (pjob.ActualStartTime != null && pjob.ActualEndTime != null)
        //                        MarketList = MarketList.Where(p => (p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= pjob.ActualStartTime.Value.Date))).ToList();
        //                    else
        //                        MarketList = MarketList.Where(p => p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= date.Date)).ToList();
        //                    return MarketList;
        //                }
        //                return null;
        //            }
        //            else
        //            {
        //                List<Market> MarketList = (from p in dataContext.Markets
        //                                           where
        //                                            (pjob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pjob.OrganizationGUID)
        //                                           && p.OwnerGUID == CustomerGUID
        //                                           && p.IsDeleted == null || p.IsDeleted == false
        //                                           select p).OrderBy(x => x.MarketName).ToList();

        //                if (MarketList != null && MarketList.Count > 0)
        //                {

        //                    if (pjob.ActualStartTime != null && pjob.ActualEndTime != null)
        //                        MarketList = MarketList.Where(p => (p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= pjob.ActualStartTime.Value.Date))).ToList();
        //                    else
        //                        MarketList = MarketList.Where(p => p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= date.Date)).ToList();
        //                    return MarketList;
        //                }
        //                return null;
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}
        //public List<Job> GetStoreNonVisitJobs()
        //{
        //    try
        //    {
        //        List<Job> ljob = new List<Job>();
        //        List<Market> Markets = new List<Market>();
        //        using (var dataContext = new WorkersInMotionDB())
        //        {
        //            Markets = (from p in dataContext.Markets
        //                       where
        //                       (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
        //                       && (pJob.CustomerStopGUID == null || pJob.CustomerStopGUID == Guid.Empty || p.MarketGUID == pJob.CustomerStopGUID)
        //                       && p.IsDeleted == false && p.EntityType == 1
        //                       select p).OrderBy(x => x.MarketName).ToList();
        //        }

        //        using (var dataContext = new WorkersInMotionJobDB())
        //        {
        //            DateTime date = DateTime.UtcNow.AddDays(-45);
        //            foreach (Market item in Markets)
        //            {
        //                Job pjob = (from p in dataContext.Jobs
        //                            where
        //                            (item.OrganizationGUID == Guid.Empty || p.OrganizationGUID == item.OrganizationGUID)
        //                            && (item.MarketGUID == null || item.MarketGUID == Guid.Empty || p.CustomerStopGUID == item.MarketGUID)
        //                            && ((pJob.PreferedStartTime == null || p.PreferedStartTime >= pJob.PreferedStartTime) && (pJob.PreferedEndTime == null || p.PreferedEndTime <= pJob.PreferedEndTime))
        //                            && (p.LastModifiedDate <= date)
        //                            && p.StatusCode == 1
        //                            && p.IsDeleted == false
        //                            && p.JobName != pJob.JobName
        //                            select p).FirstOrDefault();
        //                if (pjob != null)
        //                {
        //                    ljob.Add(pjob);
        //                }
        //            }

        //        }
        //        return ljob;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}

        //public List<Job> GetStoreNonVisitJobs(Job pJob)
        //{
        //    try
        //    {
        //        List<Job> ljob = new List<Job>();
        //        DateTime date = pJob.LastModifiedDate != null ? Convert.ToDateTime(pJob.LastModifiedDate).ToUniversalTime() : DateTime.UtcNow.AddDays(-45);
        //        using (var dataContext = new WorkersInMotionDB())
        //        {
        //            List<Market> MarketList = (from p in dataContext.Markets
        //                                       where
        //                                       (pJob.OrganizationGUID == null || pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
        //                                       && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
        //                                       && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
        //                                       && p.IsDeleted == false
        //                                       select p).OrderBy(x => x.MarketName).ToList();

        //            if (MarketList != null && MarketList.Count > 0)
        //            {
        //                if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
        //                    MarketList = MarketList.Where(p => (p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= pJob.ActualStartTime.Value.Date))).ToList();
        //                else
        //                    MarketList = MarketList.Where(p => p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= date.Date)).ToList();
        //            }
        //            foreach (Market item in MarketList)
        //            {
        //                List<Job> pjobList = (from p in dataContext.Jobs
        //                                      where
        //                                      (item.OrganizationGUID == Guid.Empty || p.OrganizationGUID == item.OrganizationGUID)
        //                                      && (item.MarketGUID == null || item.MarketGUID == Guid.Empty || p.CustomerStopGUID == item.MarketGUID)
        //                                      && ((pJob.PreferedStartTime == null || p.PreferedStartTime >= pJob.PreferedStartTime) && (pJob.PreferedEndTime == null || p.PreferedEndTime <= pJob.PreferedEndTime))
        //                                          // && (p.LastModifiedDate <= date)
        //                                      && p.StatusCode == 1
        //                                      && p.IsDeleted == false
        //                                      && p.JobName == pJob.JobName
        //                                      select p).ToList();
        //                if (pjobList != null && pjobList.Count > 0)
        //                {
        //                    foreach (Job jobitem in pjobList)
        //                    {
        //                        ljob.Add(jobitem);
        //                    }
        //                }
        //            }

        //        }
        //        return ljob;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}


        //public List<Job> GetStoreNonVisitJobs(Job pJob, string FieldManagerID)
        //{
        //    try
        //    {
        //        List<Job> ljob = new List<Job>();
        //        DateTime date = pJob.LastModifiedDate != null ? Convert.ToDateTime(pJob.LastModifiedDate).ToUniversalTime() : DateTime.UtcNow.AddDays(-45);
        //        using (var dataContext = new WorkersInMotionDB())
        //        {
        //            List<Market> MarketList = (from p in dataContext.Markets
        //                                       where
        //                                       (pJob.OrganizationGUID == null || pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
        //                                       && (pJob.RegionGUID == null || pJob.RegionGUID == Guid.Empty || p.RegionGUID == pJob.RegionGUID)
        //                                       && (pJob.TerritoryGUID == null || pJob.TerritoryGUID == Guid.Empty || p.TerritoryGUID == pJob.TerritoryGUID)
        //                                       && (p.FMUserID == FieldManagerID)
        //                                       //&& p.IsDeleted == null || p.IsDeleted == false
        //                                       select p).OrderBy(x => x.MarketName).ToList();

        //            if (MarketList != null && MarketList.Count > 0)
        //            {
        //                if (pJob.ActualStartTime != null && pJob.ActualEndTime != null)
        //                    MarketList = MarketList.Where(p => (p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= pJob.ActualStartTime.Value.Date))).ToList();
        //                else
        //                    MarketList = MarketList.Where(p => p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= date.Date)).ToList();
        //            }
        //            foreach (Market item in MarketList)
        //            {
        //                List<Job> pjobList = (from p in dataContext.Jobs
        //                                      where
        //                                      (item.OrganizationGUID == Guid.Empty || p.OrganizationGUID == item.OrganizationGUID)
        //                                      && (item.MarketGUID == null || item.MarketGUID == Guid.Empty || p.CustomerStopGUID == item.MarketGUID)
        //                                      && ((pJob.PreferedStartTime == null || p.PreferedStartTime >= pJob.PreferedStartTime) && (pJob.PreferedEndTime == null || p.PreferedEndTime <= pJob.PreferedEndTime))
        //                                          // && (p.LastModifiedDate <= date)
        //                                      && p.StatusCode == 1
        //                                      && p.IsDeleted == false
        //                                      && p.JobName == pJob.JobName
        //                                      select p).ToList();
        //                if (pjobList != null && pjobList.Count > 0)
        //                {
        //                    foreach (Job jobitem in pjobList)
        //                    {
        //                        ljob.Add(jobitem);
        //                    }
        //                }
        //            }

        //        }
        //        return ljob;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}

        //public List<Job> GetSiteNonVisitJobs(Job pJob)
        //{
        //    try
        //    {
        //        List<Job> ljob = new List<Job>();
        //        List<Market> Markets = new List<Market>();
        //        using (var dataContext = new WorkersInMotionDB())
        //        {
        //            Markets = (from p in dataContext.Markets
        //                       where
        //                       (pJob.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pJob.OrganizationGUID)
        //                       && (pJob.CustomerStopGUID == null || pJob.CustomerStopGUID == Guid.Empty || p.MarketGUID == pJob.CustomerStopGUID)
        //                       && p.IsDeleted == false && p.EntityType == 1
        //                       select p).OrderBy(x => x.MarketName).ToList();

        //            DateTime date = DateTime.UtcNow.AddDays(-45);
        //            foreach (Market item in Markets)
        //            {
        //                Job pjob = (from p in dataContext.Jobs
        //                            where
        //                            (item.OrganizationGUID == Guid.Empty || p.OrganizationGUID == item.OrganizationGUID)
        //                            && (item.MarketGUID == null || item.MarketGUID == Guid.Empty || p.CustomerStopGUID == item.MarketGUID)
        //                            && ((pJob.PreferedStartTime == null || p.PreferedStartTime >= pJob.PreferedStartTime) && (pJob.PreferedEndTime == null || p.PreferedEndTime <= pJob.PreferedEndTime))
        //                            && (p.LastModifiedDate <= date)
        //                            && p.StatusCode == 1
        //                            && p.IsDeleted == false
        //                            && p.JobName != pJob.JobName
        //                            select p).FirstOrDefault();
        //                if (pjob != null)
        //                {
        //                    ljob.Add(pjob);
        //                }
        //            }

        //        }
        //        return ljob;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}



        public List<JobNote> GetJobNotesfromJobGUID(Guid JobGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobNotes
            //            where p.JobGUID == JobGUID
            //            select p).ToList();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobGUID;
            return context.Database.SqlQuery<JobNote>("Select *  from JobNotes where JobGUID=@pJobGUID", Param).ToList();
        }


        public List<Job> GetOpenJobByUserGUID(Guid UserGUID)
        {
            try
            {

                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.Jobs
                //            where p.AssignedUserGUID == UserGUID && p.StatusCode == 1
                //            select p).ToList();

                //}

                SqlParameter[] Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = UserGUID;
                Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
                Param[1].Value = 1;
                return context.Database.SqlQuery<Job>("Select *  from Jobs where UserGUID=@pUserGUID and StatusCode=@pStatusCode", Param).ToList();


            }
            catch (Exception exception)
            {
                return null;
            }

        }

        public int UpdateJobStatus(Job pJob)
        {
            int lCurrentJobStatus = 0;
            int lJobSaveState = 0;
            try
            {
                int result = 0;

                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var qry = from p in dataContext.Jobs where p.JobGUID == pJob.JobGUID select p;
                //    var lJob = qry.Single();
                //    lCurrentJobStatus = lJob.StatusCode != null ? (int)lJob.StatusCode : 0;

                //    //Update Job record
                //    lJob.StatusCode = pJob.StatusCode;
                //    lJob.SubStatusCode = pJob.SubStatusCode;
                //    if (pJob.StatusCode == 3)
                //    {
                //        lJob.ActualStartTime = pJob.ActualStartTime;
                //    }
                //    if (pJob.StatusCode >= 4)
                //    {
                //        lJob.ActualEndTime = pJob.ActualEndTime;
                //    }
                //    lJob.Latitude = pJob.Latitude;
                //    lJob.Longitude = pJob.Longitude;
                //    lJob.LastModifiedDate = DateTime.UtcNow;
                //    lJob.LastModifiedBy = pJob.LastModifiedBy;

                //    switch (lCurrentJobStatus)
                //    {
                //        case 1:
                //            // Job Is Open Now
                //            if (pJob.StatusCode >= 1)
                //            {
                //                lJob.AssignedUserGUID = null;
                //                lJobSaveState = dataContext.SaveChanges();
                //            }
                //            else
                //            {
                //                lJobSaveState = 0;
                //                result = -1;
                //            }
                //            break;
                //        case 2:
                //            // Job is assigned now
                //            if (pJob.StatusCode > 1)
                //            {
                //                lJob.AssignedUserGUID = pJob.AssignedUserGUID;
                //                lJobSaveState = dataContext.SaveChanges();
                //            }
                //            else
                //            {
                //                lJobSaveState = 0;
                //                result = -2;
                //            }
                //            break;
                //        case 3:// Job is in progress now                            
                //            if (pJob.StatusCode > 2)
                //            {

                //                lJob.AssignedUserGUID = pJob.AssignedUserGUID;
                //                lJobSaveState = dataContext.SaveChanges();
                //            }
                //            else
                //            {
                //                lJobSaveState = 0;
                //                result = -3;
                //            }
                //            break;
                //        case 4: // Job is Abandon
                //            if (pJob.StatusCode > 3)
                //            {
                //                lJob.AssignedUserGUID = pJob.AssignedUserGUID;
                //                lJobSaveState = dataContext.SaveChanges();
                //            }
                //            else
                //            {
                //                lJobSaveState = 0;
                //                result = -4;
                //            }
                //            break;
                //        case 5: // Job is Suspended
                //            if (pJob.StatusCode > 4)
                //            {
                //                lJobSaveState = dataContext.SaveChanges();
                //            }
                //            else
                //            {
                //                lJobSaveState = 0;
                //                result = -5;
                //            }
                //            break;
                //        case 6: // Job is Complete
                //            if (pJob.StatusCode > 5)
                //            {
                //                lJobSaveState = dataContext.SaveChanges();
                //            }
                //            else
                //            {
                //                lJobSaveState = 0;
                //                result = -6;
                //            }
                //            break;

                //    }
                //}

                Job lJob = GetJobByID(pJob.JobGUID);

                lCurrentJobStatus = lJob.StatusCode != null ? (int)lJob.StatusCode : 0;

                //Update Job record
                lJob.StatusCode = pJob.StatusCode;
                lJob.SubStatusCode = pJob.SubStatusCode;
                if (pJob.StatusCode == 3)
                {
                    lJob.ActualStartTime = pJob.ActualStartTime;
                }
                if (pJob.StatusCode >= 4)
                {
                    lJob.ActualEndTime = pJob.ActualEndTime;
                }
                lJob.Latitude = pJob.Latitude;
                lJob.Longitude = pJob.Longitude;
                lJob.LastModifiedDate = DateTime.UtcNow;
                lJob.LastModifiedBy = pJob.LastModifiedBy;


                SqlParameter[] Param = new SqlParameter[10];
                Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = lJob.JobGUID;
                Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
                Param[1].Value = lJob.StatusCode;
                Param[2] = new SqlParameter("@pSubStatusCode", SqlDbType.Int);
                Param[2].Value = lJob.SubStatusCode;
                Param[3] = new SqlParameter("@pActualStartTime", SqlDbType.DateTime);
                Param[3].Value = lJob.ActualStartTime;
                Param[4] = new SqlParameter("@pActualEndTime", SqlDbType.DateTime);
                Param[4].Value = lJob.ActualEndTime;
                Param[5] = new SqlParameter("@pLatitude", SqlDbType.Float);
                Param[5].Value = lJob.Latitude;
                Param[6] = new SqlParameter("@pLongitude", SqlDbType.Float);
                Param[6].Value = lJob.Longitude;
                Param[7] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                Param[7].Value = lJob.LastModifiedDate;
                Param[8] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
                Param[8].Value = lJob.LastModifiedBy;
                switch (lCurrentJobStatus)
                {
                    case 1:
                        // Job Is Open Now
                        if (pJob.StatusCode >= 1)
                        {
                            lJob.AssignedUserGUID = null;
                            Param[9] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                            Param[9].Value = lJob.AssignedUserGUID;

                            // lJobSaveState = dataContext.SaveChanges();
                            lJobSaveState = context.Database.ExecuteSqlCommand("update Jobs set StatusCode=@pStatusCode,"
                                + "SubStatusCode=@pSubStatusCode,ActualStartTime=@pActualStartTime,ActualEndTime=@pActualEndTime,Latitude=@pLatitude,Longitude=@pLongitude,"
                                + "LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,AssignedUserGUID=@pAssignedUserGUID where JobGUID=@pJobGUID", Param);
                        }
                        else
                        {
                            lJobSaveState = 0;
                            result = -1;
                        }
                        break;
                    case 2:
                        // Job is assigned now
                        if (pJob.StatusCode > 1)
                        {
                            lJob.AssignedUserGUID = pJob.AssignedUserGUID;
                            Param[9] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                            Param[9].Value = lJob.AssignedUserGUID;
                            //lJobSaveState = dataContext.SaveChanges();
                            lJobSaveState = context.Database.ExecuteSqlCommand("update Jobs set StatusCode=@pStatusCode,"
                                + "SubStatusCode=@pSubStatusCode,ActualStartTime=@pActualStartTime,ActualEndTime=@pActualEndTime,Latitude=@pLatitude,Longitude=@pLongitude,"
                                + "LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,AssignedUserGUID=@pAssignedUserGUID where JobGUID=@pJobGUID", Param);
                        }
                        else
                        {
                            lJobSaveState = 0;
                            result = -2;
                        }
                        break;
                    case 3:// Job is in progress now                            
                        if (pJob.StatusCode > 2)
                        {

                            lJob.AssignedUserGUID = pJob.AssignedUserGUID;
                            Param[9] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                            Param[9].Value = lJob.AssignedUserGUID;
                            //lJobSaveState = dataContext.SaveChanges();
                            lJobSaveState = context.Database.ExecuteSqlCommand("update Jobs set StatusCode=@pStatusCode,"
                                + "SubStatusCode=@pSubStatusCode,ActualStartTime=@pActualStartTime,ActualEndTime=@pActualEndTime,Latitude=@pLatitude,Longitude=@pLongitude,"
                                + "LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,AssignedUserGUID=@pAssignedUserGUID where JobGUID=@pJobGUID", Param);
                        }
                        else
                        {
                            lJobSaveState = 0;
                            result = -3;
                        }
                        break;
                    case 4: // Job is Abandon
                        if (pJob.StatusCode > 3)
                        {
                            lJob.AssignedUserGUID = pJob.AssignedUserGUID;
                            Param[9] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                            Param[9].Value = lJob.AssignedUserGUID;
                            //lJobSaveState = dataContext.SaveChanges();
                            lJobSaveState = context.Database.ExecuteSqlCommand("update Jobs set StatusCode=@pStatusCode,"
                                + "SubStatusCode=@pSubStatusCode,ActualStartTime=@pActualStartTime,ActualEndTime=@pActualEndTime,Latitude=@pLatitude,Longitude=@pLongitude,"
                                + "LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,AssignedUserGUID=@pAssignedUserGUID where JobGUID=@pJobGUID", Param);
                        }
                        else
                        {
                            lJobSaveState = 0;
                            result = -4;
                        }
                        break;
                    case 5: // Job is Suspended
                        if (pJob.StatusCode > 4)
                        {
                            Param[9] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                            Param[9].Value = lJob.AssignedUserGUID;
                            //lJobSaveState = dataContext.SaveChanges();
                            lJobSaveState = context.Database.ExecuteSqlCommand("update Jobs set StatusCode=@pStatusCode,"
                                + "SubStatusCode=@pSubStatusCode,ActualStartTime=@pActualStartTime,ActualEndTime=@pActualEndTime,Latitude=@pLatitude,Longitude=@pLongitude,"
                                + "LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,AssignedUserGUID=@pAssignedUserGUID where JobGUID=@pJobGUID", Param);
                        }
                        else
                        {
                            lJobSaveState = 0;
                            result = -5;
                        }
                        break;
                    case 6: // Job is Complete
                        if (pJob.StatusCode > 5)
                        {
                            Param[9] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                            Param[9].Value = lJob.AssignedUserGUID;
                            //lJobSaveState = dataContext.SaveChanges();
                            lJobSaveState = context.Database.ExecuteSqlCommand("update Jobs set StatusCode=@pStatusCode,"
                                + "SubStatusCode=@pSubStatusCode,ActualStartTime=@pActualStartTime,ActualEndTime=@pActualEndTime,Latitude=@pLatitude,Longitude=@pLongitude,"
                                + "LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,AssignedUserGUID=@pAssignedUserGUID where JobGUID=@pJobGUID", Param);
                        }
                        else
                        {
                            lJobSaveState = 0;
                            result = -6;
                        }
                        break;

                }

                return result;
            }
            catch (Exception exception)
            {
                return 0;
            }
        }

        public int InsertJobProgress(JobProgress pJobProgress)
        {
            int result = 0; ;
            try
            {
                JobProgress _jobProgress = new JobProgress();
                _jobProgress.JobProgressGUID = Guid.NewGuid();
                _jobProgress.JobGUID = pJobProgress.JobGUID;
                _jobProgress.JobStatus = pJobProgress.JobStatus;
                _jobProgress.JobSubStatus = pJobProgress.JobSubStatus;
                _jobProgress.StartTime = pJobProgress.StartTime;
                _jobProgress.Duration = pJobProgress.Duration;
                _jobProgress.Latitude = pJobProgress.Latitude;
                _jobProgress.Longitude = pJobProgress.Longitude;
                _jobProgress.LastModifiedDate = DateTime.UtcNow;
                _jobProgress.LastModifiedBy = pJobProgress.LastModifiedBy;
                if (pJobProgress.LocationMismatch != null)
                {
                    _jobProgress.LocationMismatch = pJobProgress.LocationMismatch;
                }
                //context.JobProgresses.Add(_jobProgress);
                //result = Save();

                SqlParameter[] Param = new SqlParameter[10];
                Param[0] = new SqlParameter("@pJobProgressGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = _jobProgress.JobProgressGUID;
                Param[1] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = (object)_jobProgress.JobGUID ?? DBNull.Value;
                Param[2] = new SqlParameter("@pJobStatus", SqlDbType.Int);
                Param[2].Value = (object)_jobProgress.JobStatus ?? DBNull.Value;
                Param[3] = new SqlParameter("@pJobSubStatus", SqlDbType.Int);
                Param[3].Value = (object)_jobProgress.JobSubStatus ?? DBNull.Value;
                Param[4] = new SqlParameter("@pStatusNote", SqlDbType.NVarChar, -1);
                Param[4].Value = (object)_jobProgress.StatusNote ?? DBNull.Value;
                Param[5] = new SqlParameter("@pStartTime", SqlDbType.DateTime);
                Param[5].Value = (object)_jobProgress.StartTime ?? DBNull.Value;
                //Param[6] = new SqlParameter("@pLocationMismatch", SqlDbType.Bit);
                //Param[6].Value = (object)_jobProgress.LocationMismatch ?? DBNull.Value;
                //Heroku
                //Param[7] = new SqlParameter("@pDuration", SqlDbType.Float);
                //Param[7].Value = (object)_jobProgress.Duration ?? DBNull.Value;
                Param[6] = new SqlParameter("@pLatitude", SqlDbType.Float);
                Param[6].Value = (object)_jobProgress.Latitude ?? DBNull.Value;
                Param[7] = new SqlParameter("@pLongitude", SqlDbType.Float);
                Param[7].Value = (object)_jobProgress.Longitude ?? DBNull.Value;
                Param[8] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                Param[8].Value = (object)_jobProgress.LastModifiedDate ?? DBNull.Value;
                Param[9] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
                Param[9].Value = (object)_jobProgress.LastModifiedBy ?? DBNull.Value;

                result = context.Database.ExecuteSqlCommand("insert into JobProgress(JobProgressGUID,JobGUID,JobStatus,JobSubStatus,StatusNote,"
                    + "StartTime,Latitude,Longitude,LastModifiedDate,LastModifiedBy)values(@pJobProgressGUID,@pJobGUID,@pJobStatus,@pJobSubStatus,@pStatusNote,"
                    + "@pStartTime,@pLatitude,@pLongitude,@pLastModifiedDate,@pLastModifiedBy)", Param);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public int InsertJobProgressWithDuration(JobProgress pJobProgress)
        {
            int result = 0; ;
            try
            {
                JobProgress _jobProgress = new JobProgress();
                _jobProgress.JobProgressGUID = Guid.NewGuid();
                _jobProgress.JobGUID = pJobProgress.JobGUID;
                _jobProgress.JobStatus = pJobProgress.JobStatus;
                _jobProgress.JobSubStatus = pJobProgress.JobSubStatus;
                _jobProgress.StartTime = pJobProgress.StartTime;
                _jobProgress.Duration = pJobProgress.Duration;
                _jobProgress.Latitude = pJobProgress.Latitude;
                _jobProgress.Longitude = pJobProgress.Longitude;
                _jobProgress.LastModifiedDate = DateTime.UtcNow;
                _jobProgress.LastModifiedBy = pJobProgress.LastModifiedBy;
                if (pJobProgress.LocationMismatch != null)
                {
                    _jobProgress.LocationMismatch = pJobProgress.LocationMismatch;
                }
                //context.JobProgresses.Add(_jobProgress);
                //result = Save();

                SqlParameter[] Param = new SqlParameter[11];
                Param[0] = new SqlParameter("@pJobProgressGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = _jobProgress.JobProgressGUID;
                Param[1] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = (object)_jobProgress.JobGUID ?? DBNull.Value;
                Param[2] = new SqlParameter("@pJobStatus", SqlDbType.Int);
                Param[2].Value = (object)_jobProgress.JobStatus ?? DBNull.Value;
                Param[3] = new SqlParameter("@pJobSubStatus", SqlDbType.Int);
                Param[3].Value = (object)_jobProgress.JobSubStatus ?? DBNull.Value;
                Param[4] = new SqlParameter("@pStatusNote", SqlDbType.NVarChar, -1);
                Param[4].Value = (object)_jobProgress.StatusNote ?? DBNull.Value;
                Param[5] = new SqlParameter("@pStartTime", SqlDbType.DateTime);
                Param[5].Value = (object)_jobProgress.StartTime ?? DBNull.Value;
                //Param[6] = new SqlParameter("@pLocationMismatch", SqlDbType.Bit);
                //Param[6].Value = (object)_jobProgress.LocationMismatch ?? DBNull.Value;
                Param[6] = new SqlParameter("@pDuration", SqlDbType.Float);
                Param[6].Value = (object)_jobProgress.Duration ?? DBNull.Value;
                Param[7] = new SqlParameter("@pLatitude", SqlDbType.Float);
                Param[7].Value = (object)_jobProgress.Latitude ?? DBNull.Value;
                Param[8] = new SqlParameter("@pLongitude", SqlDbType.Float);
                Param[8].Value = (object)_jobProgress.Longitude ?? DBNull.Value;
                Param[9] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                Param[9].Value = (object)_jobProgress.LastModifiedDate ?? DBNull.Value;
                Param[10] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
                Param[10].Value = (object)_jobProgress.LastModifiedBy ?? DBNull.Value;

                result = context.Database.ExecuteSqlCommand("insert into JobProgress(JobProgressGUID,JobGUID,JobStatus,JobSubStatus,StatusNote,"
                    + "StartTime,Duration,Latitude,Longitude,LastModifiedDate,LastModifiedBy)values(@pJobProgressGUID,@pJobGUID,@pJobStatus,@pJobSubStatus,@pStatusNote,"
                    + "@pStartTime,@pDuration,@pLatitude,@pLongitude,@pLastModifiedDate,@pLastModifiedBy)", Param);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public int UpdateJobProgress(JobProgress pJobProgress)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var qry = from p in dataContext.JobProgresses where p.JobGUID == pJobProgress.JobGUID && p.JobStatus == 1 select p;
                //    var item = qry.Single();
                //    item.StartTime = pJobProgress.StartTime;
                //    item.LastModifiedBy = pJobProgress.LastModifiedBy;
                //    return dataContext.SaveChanges();
                //}
                JobProgress _jobProgress = GetJobProgressMismatch(new Guid(pJobProgress.JobGUID.ToString()), 1);
                _jobProgress.StartTime = pJobProgress.StartTime;
                _jobProgress.LastModifiedBy = pJobProgress.LastModifiedBy;
                _jobProgress.LastModifiedDate = DateTime.UtcNow;
                SqlParameter[] Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@pJobProgressGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = _jobProgress.JobProgressGUID;
                Param[1] = new SqlParameter("@pStartTime", SqlDbType.DateTime);
                Param[1].Value = (object)_jobProgress.StartTime ?? DBNull.Value;
                Param[2] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                Param[2].Value = (object)_jobProgress.LastModifiedDate ?? DBNull.Value;
                Param[3] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
                Param[3].Value = (object)_jobProgress.LastModifiedBy ?? DBNull.Value;

                return context.Database.ExecuteSqlCommand("Update JobProgress set StartTime=@pStartTime,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy where JobProgressGUID=@pJobProgressGUID", Param);


            }
            catch (Exception exception)
            {
                return 0;
            }
        }

        public JobProgress GetJobProgressMismatch(Guid JobGUID, int Status)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobProgresses
            //            where p.JobGUID == JobGUID && p.JobStatus == Status
            //            select p).FirstOrDefault();
            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobGUID;
            Param[1] = new SqlParameter("@pJobStatus", SqlDbType.Int);
            Param[1].Value = Status;
            return context.Database.SqlQuery<JobProgress>("select * from JobProgress where JobGUID=@pJobGUID and JobStatus=@pJobStatus", Param).FirstOrDefault();

        }
        public List<JobProgress> GetJobProgress(Guid JobGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobProgresses
            //            where p.JobGUID == JobGUID
            //            select p).OrderByDescending(one => one.StartTime).ToList();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobGUID;
            return context.Database.SqlQuery<JobProgress>("select * from JobProgress where JobGUID=@pJobGUID order by StartTime desc", Param).ToList();

        }
        public List<JobCostType> getJobCostTypes()
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobCostTypes
            //            select p).ToList();
            //}
            return context.Database.SqlQuery<JobCostType>("select * from JobCostTypes").ToList();

        }

        public List<JobStatusOrganization> getJobStatusOrganization(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobStatusOrganizations
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).ToList();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<JobStatusOrganization>("select * from JobStatusOrganizations where OrganizationGUID=@pOrganizationGUID", Param).ToList();

        }
        public List<JobSubStatusOrganization> getJobSubStatusOrganization(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobSubStatusOrganizations
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).ToList();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<JobSubStatusOrganization>("select * from JobSubStatusOrganizations where OrganizationGUID=@pOrganizationGUID", Param).ToList();
        }

        public List<OptionList> getOptionList(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.OptionLists
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).ToList();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<OptionList>("select * from OptionLists where OrganizationGUID=@pOrganizationGUID", Param).ToList();
        }


        public int InsertJobFormData(JobFormData JobFormData)
        {
            // context.JobFormDatas.Add(JobFormData);
            SqlParameter[] Param = new SqlParameter[6];
            Param[0] = new SqlParameter("@pJobFormDataGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobFormData.JobFormDataGUID;
            Param[1] = new SqlParameter("@pJobFormGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)JobFormData.JobFormDataGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)JobFormData.JobGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pControlsID", SqlDbType.NVarChar, -1);
            Param[3].Value = (object)JobFormData.ControlsID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pVal", SqlDbType.NChar, 10);
            Param[4].Value = (object)JobFormData.Val ?? DBNull.Value;
            Param[5] = new SqlParameter("@pValID", SqlDbType.NChar, 10);
            Param[5].Value = (object)JobFormData.ValID ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into JobFormData(JobFormDataGUID,JobFormGUID,JobGUID,ControlsID,Val,ValID)values"
                            + "(@pJobFormDataGUID,@pJobFormGUID,@pJobGUID,@pControlsID,@pVal,@pValID)", Param);

        }

        public POs GetPOs(POs PO)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.POs
                //            where
                //            (PO.POGUID == Guid.Empty || p.POGUID == PO.POGUID)
                //            && (string.IsNullOrEmpty(PO.PONumber) || p.PONumber == PO.PONumber)
                //            select p).FirstOrDefault();

                //}
                SqlParameter[] Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@pPOGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = PO.POGUID;
                Param[1] = new SqlParameter("@pPONumber", SqlDbType.NVarChar, 50);
                Param[1].Value = (object)PO.PONumber ?? DBNull.Value;

                return context.Database.SqlQuery<POs>("select * from POs where"
                                    + "(POGUID=@pPOGUID or @pPOGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (PONumber=@pPONumber OR @pPONumber is NULL or @pPONumber='')", Param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public POs GetPOsForClientStoreOrganization(POs PO)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.POs
                //            where
                //            (PO.PONumber == p.PONumber)
                //            && (p.OrganizationGUID == PO.OrganizationGUID)
                //            && (p.MarketID == PO.MarketID)
                //            && (p.PlaceID == PO.PlaceID)
                //            select p).FirstOrDefault();

                //}

                SqlParameter[] Param = new SqlParameter[4];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = (object)PO.OrganizationGUID ?? DBNull.Value;
                Param[1] = new SqlParameter("@pPONumber", SqlDbType.NVarChar, 50);
                Param[1].Value = (object)PO.PONumber ?? DBNull.Value;
                Param[2] = new SqlParameter("@pMarketID", SqlDbType.NVarChar, 50);
                Param[2].Value = (object)PO.MarketID ?? DBNull.Value;
                Param[3] = new SqlParameter("@pPlaceID", SqlDbType.NVarChar, 50);
                Param[3].Value = (object)PO.PlaceID ?? DBNull.Value;

                return context.Database.SqlQuery<POs>("select * from POs where OrganizationGUID=@pOrganizationGUID and"
                            + " PONumber=@pPONumber and MarketID=@pMarketID and PlaceID=@pPlaceID", Param).FirstOrDefault();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<POs> GetPOList(string SessionID)
        {
            try
            {
                //Guid OrganizationGUID = Guid.Empty;
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    string UserGUID = (from p in dataContext.MasterLogins
                //                       where p.SessionID == SessionID
                //                       select p).SingleOrDefault().UserGUID.ToString();
                //    if (!string.IsNullOrEmpty(UserGUID))
                //    {
                //        OrganizationGUID = (from p in dataContext.OrganizationUsersMaps
                //                            where p.UserGUID == new Guid(UserGUID)
                //                            select p).SingleOrDefault().OrganizationGUID;
                //    }

                //    return (from p in dataContext.POs
                //            where p.OrganizationGUID == OrganizationGUID
                //            select p).ToList();

                //}

                SqlParameter[] Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@pSessionID", SqlDbType.NVarChar, -1);
                Param[0].Value = SessionID;
                return context.Database.SqlQuery<POs>("select * from POs where OrganizationGUID in (select OrganizationGUID from OrganizationUsersMap where UserGUID in (Select UserGUID from MasterLogins where SessionID=@pSessionID))", Param).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int InsertPO(POs PO)
        {
            //context.POs.Add(PO);
            return _IPORepository.CreatePO(PO);
        }

        public Job CreateJobbyStoreID(Job jobRequest)
        {
            Job lresponse = new Job();
            try
            {
                Job job = new Job();
                job.JobGUID = Guid.NewGuid();
                //  job.JobFormGUID = jobRequest.JobFormGUID;
                job.IsDeleted = false;
                job.OrganizationGUID = jobRequest.OrganizationGUID;
                job.RegionGUID = jobRequest.RegionGUID;
                job.TerritoryGUID = jobRequest.TerritoryGUID;
                job.LocationType = jobRequest.LocationType;
                job.CustomerGUID = jobRequest.CustomerGUID;
                job.StatusCode = 1;
                job.SubStatusCode = 0;
                job.CustomerStopGUID = jobRequest.CustomerStopGUID;
                job.JobClass = jobRequest.JobClass;
                job.JobForm = jobRequest.JobForm;

                LatLong latLong = new LatLong();
                job.ServiceAddress = jobRequest.ServiceAddress;
                job.Latitude = jobRequest.Latitude;
                job.Longitude = jobRequest.Longitude;
                job.JobName = jobRequest.JobName;
                job.IsSecheduled = jobRequest.IsSecheduled;
                job.ManagerUserGUID = jobRequest.ManagerUserGUID;
                job.AssignedUserGUID = jobRequest.AssignedUserGUID;

                //job.EstimatedDutation = _job.JobModel.EstimatedDuration;
                //job.ScheduledStartTime = Convert.ToDateTime(_job.JobModel.PreferredStartTime);
                job.PreferedStartTime = jobRequest.PreferedStartTime;
                job.PreferedEndTime = jobRequest.PreferedEndTime;
                job.ActualStartTime = jobRequest.ActualStartTime;

                job.PONumber = jobRequest.PONumber;
                job.ScheduledStartTime = DateTime.UtcNow;
                job.CreateDate = DateTime.UtcNow;
                job.CreateBy = jobRequest.CreateBy;
                job.LastModifiedDate = DateTime.UtcNow;
                job.LastModifiedBy = jobRequest.LastModifiedBy;
                job.IsDeleted = false;
                job.IsActive = true;
                int result = InsertJobbyStoreID(job);
                //int result = Save();
                if (result > 0)
                {
                    lresponse = GetJobByID(job.JobGUID);
                }
                return lresponse;
            }
            catch (Exception ex)
            {
                return lresponse;
            }
        }
        public Job CreateJobbyPO(Job jobRequest)
        {
            Job lresponse = new Job();
            try
            {
                Job job = new Job();
                job.JobGUID = Guid.NewGuid();
                //  job.JobFormGUID = jobRequest.JobFormGUID;
                job.IsDeleted = false;
                job.OrganizationGUID = jobRequest.OrganizationGUID;
                job.RegionGUID = jobRequest.RegionGUID;
                job.TerritoryGUID = jobRequest.TerritoryGUID;
                job.LocationType = jobRequest.LocationType;
                job.CustomerGUID = jobRequest.CustomerGUID;
                job.StatusCode = 1;
                job.SubStatusCode = 0;
                job.CustomerStopGUID = jobRequest.CustomerStopGUID;
                job.JobClass = jobRequest.JobClass;
                job.JobForm = jobRequest.JobForm;

                LatLong latLong = new LatLong();
                job.ServiceAddress = jobRequest.ServiceAddress;
                job.Latitude = jobRequest.Latitude;
                job.Longitude = jobRequest.Longitude;
                job.JobName = jobRequest.JobName;
                job.IsSecheduled = jobRequest.IsSecheduled;
                job.ManagerUserGUID = jobRequest.ManagerUserGUID;
                job.AssignedUserGUID = jobRequest.AssignedUserGUID;

                //job.EstimatedDutation = _job.JobModel.EstimatedDuration;
                //job.ScheduledStartTime = Convert.ToDateTime(_job.JobModel.PreferredStartTime);
                job.PreferedStartTime = jobRequest.PreferedStartTime;
                job.PreferedEndTime = jobRequest.PreferedEndTime;
                job.ActualStartTime = jobRequest.ActualStartTime;

                job.PONumber = jobRequest.PONumber;
                job.ScheduledStartTime = DateTime.UtcNow;
                job.CreateDate = DateTime.UtcNow;
                job.CreateBy = jobRequest.CreateBy;
                job.LastModifiedDate = DateTime.UtcNow;
                job.LastModifiedBy = jobRequest.LastModifiedBy;
                job.IsDeleted = false;
                job.IsActive = true;
                // lretString.Append(" [1.1.2: Input string" + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(job) + "]");
                int result = InsertJobbyPO(job);
                //int result = Save();
                if (result > 0)
                {
                    lresponse = GetJobByID(job.JobGUID);
                }
                return lresponse;
            }
            catch (Exception ex)
            {
                return lresponse;
            }
        }
        public Job CreateJob(Job jobRequest)
        {
            Job lresponse = new Job();
            try
            {
                Job job = new Job();
                job.JobGUID = Guid.NewGuid();
                //  job.JobFormGUID = jobRequest.JobFormGUID;
                job.IsDeleted = false;
                job.OrganizationGUID = jobRequest.OrganizationGUID;
                job.RegionGUID = jobRequest.RegionGUID;
                job.TerritoryGUID = jobRequest.TerritoryGUID;
                job.LocationType = jobRequest.LocationType;
                job.CustomerGUID = jobRequest.CustomerGUID;
                job.StatusCode = 1;
                job.SubStatusCode = 0;
                job.CustomerStopGUID = jobRequest.CustomerStopGUID;
                job.JobClass = jobRequest.JobClass;
                job.JobForm = jobRequest.JobForm;

                LatLong latLong = new LatLong();
                job.ServiceAddress = jobRequest.ServiceAddress;
                job.Latitude = jobRequest.Latitude;
                job.Longitude = jobRequest.Longitude;
                job.JobName = jobRequest.JobName;
                job.IsSecheduled = jobRequest.IsSecheduled;
                job.ManagerUserGUID = jobRequest.ManagerUserGUID;
                job.AssignedUserGUID = jobRequest.AssignedUserGUID;

                //job.EstimatedDutation = _job.JobModel.EstimatedDuration;
                //job.ScheduledStartTime = Convert.ToDateTime(_job.JobModel.PreferredStartTime);
                job.PreferedStartTime = jobRequest.PreferedStartTime;
                job.PreferedEndTime = jobRequest.PreferedEndTime;
                job.ActualStartTime = jobRequest.ActualStartTime;

                job.PONumber = jobRequest.PONumber;
                job.ScheduledStartTime = DateTime.UtcNow;
                job.CreateDate = DateTime.UtcNow;
                job.CreateBy = jobRequest.CreateBy;
                job.LastModifiedDate = DateTime.UtcNow;
                job.LastModifiedBy = jobRequest.LastModifiedBy;
                job.IsDeleted = false;
                job.IsActive = true;
                int result = InsertJob(job);
                //int result = Save();
                if (result > 0)
                {
                    lresponse = GetJobByID(job.JobGUID);
                }
                return lresponse;
            }
            catch (Exception ex)
            {
                return lresponse;
            }
        }

        public int InsertJobNotes(JobNote jobNote)
        {
            //context.JobNotes.Add(jobNote);

            SqlParameter[] Param = new SqlParameter[9];
            Param[0] = new SqlParameter("@pJobNoteGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = jobNote.JobNoteGUID;
            Param[1] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)jobNote.JobGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pDeletable", SqlDbType.Bit);
            Param[2].Value = (object)jobNote.Deletable ?? DBNull.Value;
            Param[3] = new SqlParameter("@pNoteType", SqlDbType.SmallInt);
            Param[3].Value = (object)jobNote.NoteType ?? DBNull.Value;
            Param[4] = new SqlParameter("@pNoteText", SqlDbType.NVarChar, -1);
            Param[4].Value = (object)jobNote.NoteText ?? DBNull.Value;
            Param[5] = new SqlParameter("@pFileName", SqlDbType.NVarChar, -1);
            Param[5].Value = (object)jobNote.FileName ?? DBNull.Value;
            Param[6] = new SqlParameter("@pFileURL", SqlDbType.NVarChar, -1);
            Param[6].Value = (object)jobNote.FileURL ?? DBNull.Value;
            Param[7] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[7].Value = (object)jobNote.CreateDate ?? DBNull.Value;
            Param[8] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[8].Value = (object)jobNote.CreateBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into JobNotes(JobNoteGUID,JobGUID,Deletable,NoteType,NoteText,FileName,"
                     + "FileURL,CreateDate,CreateBy)values(@pJobNoteGUID,@pJobGUID,@pDeletable,@pNoteType,@pNoteText,@pFileName,"
                     + "@pFileURL,@pCreateDate,@pCreateBy)", Param);
        }

        public int UploadJobs(Job UploadJobRequest)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var qry = from p in dataContext.Jobs where p.JobGUID == UploadJobRequest.JobGUID select p;
                //    var item = qry.Single();
                //    item.StatusCode = UploadJobRequest.StatusCode;
                //    item.SubStatusCode = UploadJobRequest.SubStatusCode;
                //    item.ActualStartTime = UploadJobRequest.ActualStartTime;
                //    item.ActualEndTime = UploadJobRequest.ActualEndTime;
                //    item.PreferedEndTime = UploadJobRequest.PreferedEndTime;
                //    item.EstimatedDuration = UploadJobRequest.EstimatedDuration;
                //    item.ActualDuration = UploadJobRequest.ActualDuration;
                //    item.QuotedDuration = UploadJobRequest.QuotedDuration;
                //    item.ScheduledEndTime = UploadJobRequest.ScheduledEndTime;
                //    item.LastModifiedDate = DateTime.UtcNow;
                //    item.LastModifiedBy = UploadJobRequest.LastModifiedBy;
                //    item.JobForm = UploadJobRequest.JobForm;
                //    return dataContext.SaveChanges();
                //}

                Job job = GetJobByID(UploadJobRequest.JobGUID);
                job.StatusCode = UploadJobRequest.StatusCode;
                job.SubStatusCode = UploadJobRequest.SubStatusCode;
                job.ActualStartTime = UploadJobRequest.ActualStartTime;
                job.ActualEndTime = UploadJobRequest.ActualEndTime;
                job.PreferedEndTime = UploadJobRequest.PreferedEndTime;
                job.EstimatedDuration = UploadJobRequest.EstimatedDuration;
                job.ActualDuration = UploadJobRequest.ActualDuration;
                job.QuotedDuration = UploadJobRequest.QuotedDuration;
                job.ScheduledEndTime = UploadJobRequest.ScheduledEndTime;
                job.LastModifiedDate = DateTime.UtcNow;
                job.LastModifiedBy = UploadJobRequest.LastModifiedBy;
                job.JobForm = UploadJobRequest.JobForm;
                SqlParameter[] Param = new SqlParameter[13];
                Param = SetParametersforUpdate(job);

                return context.Database.ExecuteSqlCommand("update Jobs set StatusCode=@pStatusCode,SubStatusCode=@pSubStatusCode,ActualStartTime=@pActualStartTime,"
                + "ActualEndTime=@pActualEndTime,PreferedEndTime=@pPreferedEndTime,EstimatedDuration=@pEstimatedDuration,ActualDuration=@pActualDuration,"
                + "QuotedDuration=@pQuotedDuration,ScheduledEndTime=@pScheduledEndTime,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,"
                + "JobForm=@pJobForm where JobGUID=@pJobGUID", Param);



            }
            catch (Exception exception)
            {
                return 0;
            }
        }


        public dynamic GetRouteUsers(SqlParameter[] UserAvailability, SqlParameter[] UserLocation)
        {
            try
            {
                var result1 = contextWIM.Database.SqlQuery<dynamic>("p_GetUsersLocationForADate @pOrganizationGUID,@pDate,@pStartTime, @pEndTime, @pDuration,@pRegionGUID,@pTerritoryGUID,@pLatitude,@pLongitude,@pSearchWithinZoneArea,@pSearch,@pErrorCode", UserLocation).ToList();
                var result = contextWIM.Database.SqlQuery<dynamic>("p_GetUsersAvailability @pSkill,@pDate,@pStartTime, @pEndTime, @pDuration,@pRegionGUID,@pTerritoryGUID,@pLatitude,@pLongitude,@pSearchWithinZoneArea,@pSearch,@pErrorCode", UserAvailability).ToList();
                return result;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public List<p_GetJobStatistics_Result> GetJobStatus(SqlParameter[] sqlParam)
        {

            try
            {

                List<p_GetJobStatistics_Result> result = context.Database.SqlQuery<p_GetJobStatistics_Result>("dbo.p_GetJobStatistics @UserGUID,@pStatsForManager,@pErrorCode=@pErrorCode output", sqlParam).ToList();
                return result;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public int AssignJob(Job _Job)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var qry = from p in dataContext.Jobs where p.JobGUID == _Job.JobGUID select p;
                //    var item = qry.Single();
                //    if (item.StatusCode < 2 && item.AssignedUserGUID == null)
                //    {

                //        item.AssignedUserGUID = _Job.AssignedUserGUID;
                //        item.LastModifiedDate = DateTime.UtcNow;
                //        item.LastModifiedBy = _Job.LastModifiedBy;
                //        item.StatusCode = 2;
                //        return dataContext.SaveChanges();
                //    }
                //    else
                //    {
                //        if (item.AssignedUserGUID != _Job.AssignedUserGUID)
                //        {
                //            //Job is assigned to another user.
                //            return -1;
                //        }
                //        else
                //        {
                //            // Job is already in progress or completed
                //            return -2;
                //        }
                //    }

                //}
                Job job = GetJobByID(_Job.JobGUID);
                if (job.StatusCode < 2 && job.AssignedUserGUID == null)
                {

                    job.AssignedUserGUID = _Job.AssignedUserGUID;
                    job.LastModifiedDate = DateTime.UtcNow;
                    job.LastModifiedBy = _Job.LastModifiedBy;
                    job.StatusCode = 2;


                    SqlParameter[] Param = new SqlParameter[5];
                    Param[0] = new SqlParameter("@pAssignedUserGUID", SqlDbType.UniqueIdentifier);
                    Param[0].Value = job.AssignedUserGUID;
                    Param[1] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                    Param[1].Value = job.LastModifiedDate;
                    Param[2] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
                    Param[2].Value = job.LastModifiedBy;
                    Param[3] = new SqlParameter("@pStatusCode", SqlDbType.Int);
                    Param[3].Value = job.StatusCode;
                    Param[4] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
                    Param[4].Value = job.JobGUID;
                    return context.Database.ExecuteSqlCommand("update Jobs set AssignedUserGUID=@pAssignedUserGUID,LastModifiedDate=@pLastModifiedDate,"
                    + "LastModifiedBy=@pLastModifiedBy,StatusCode=@pStatusCode where JobGUID=@pJobGUID", Param);
                }
                else
                {
                    if (job.AssignedUserGUID != _Job.AssignedUserGUID)
                    {
                        //Job is assigned to another user.
                        return -1;
                    }
                    else
                    {
                        // Job is already in progress or completed
                        return -2;
                    }
                }

            }
            catch (Exception exception)
            {
                return 0;
            }
        }


        public int InsertJobAssigned(JobAssigned _JobAssigned)
        {
            //context.JobAssigneds.Add(_JobAssigned);
            SqlParameter[] Param = new SqlParameter[9];
            Param[0] = new SqlParameter("@pJobAssignGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = _JobAssigned.JobAssignGUID;
            Param[1] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)_JobAssigned.JobGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)_JobAssigned.UserGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pStartTime", SqlDbType.DateTime);
            Param[3].Value = (object)_JobAssigned.StartTime ?? DBNull.Value;
            Param[4] = new SqlParameter("@pEndTime", SqlDbType.DateTime);
            Param[4].Value = (object)_JobAssigned.EndTime ?? DBNull.Value;
            Param[5] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[5].Value = (object)_JobAssigned.Latitude ?? DBNull.Value;
            Param[6] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[6].Value = (object)_JobAssigned.Longitude ?? DBNull.Value;
            Param[7] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[7].Value = (object)_JobAssigned.LastModifiedDate ?? DBNull.Value;
            Param[8] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[8].Value = (object)_JobAssigned.LastModifiedBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into JobAssigned(JobAssignGUID,JobGUID,UserGUID,StartTime,EndTime,Latitude,Longitude,LastModifiedDate,LastModifiedBy)"
            + "values(@pJobAssignGUID,@pJobGUID,@pUserGUID,@pStartTime,@pEndTime,@pLatitude,@pLongitude,@pLastModifiedDate,@pLastModifiedBy)", Param);
        }

        public JobForum GetJobForumbyJobGUID(Guid JobGUID)
        {
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobGUID;
            return context.Database.SqlQuery<JobForum>("select *  from JobForums where JobGUID=@pJobGUID", Param).FirstOrDefault();
        }

        public int UpdateForumStatus(JobForum JobForumRequest)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var qry = from p in dataContext.JobForums where p.JobGUID == JobForumRequest.JobGUID select p;
                //    var item = qry.FirstOrDefault();
                //    item.ForumStatus = JobForumRequest.ForumStatus;
                //    item.LastModifiedDate = DateTime.UtcNow;
                //    item.LastModifiedBy = JobForumRequest.LastModifiedBy;
                //    return dataContext.SaveChanges();
                //}
                if (JobForumRequest.JobGUID != null)
                {
                    JobForum forum = GetJobForumbyJobGUID(new Guid(JobForumRequest.JobGUID.ToString()));
                    forum.ForumStatus = JobForumRequest.ForumStatus;
                    forum.LastModifiedDate = DateTime.UtcNow;
                    forum.LastModifiedBy = JobForumRequest.LastModifiedBy;
                    SqlParameter[] Param = new SqlParameter[4];
                    Param[0] = new SqlParameter("@pForumStatus", SqlDbType.SmallInt);
                    Param[0].Value = forum.ForumStatus;
                    Param[1] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                    Param[1].Value = forum.LastModifiedDate;
                    Param[2] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
                    Param[2].Value = forum.LastModifiedBy;
                    Param[3] = new SqlParameter("@pJobForumGUID", SqlDbType.UniqueIdentifier);
                    Param[3].Value = forum.JobForumGUID;
                    return context.Database.ExecuteSqlCommand("update JobForums set ForumStatus=@pForumStatus,LastModifiedDate=@pLastModifiedDate,"
                    + "LastModifiedBy=@pLastModifiedBy where JobForumGUID=@pJobForumGUID", Param);
                }
                else
                    return 0;


            }
            catch (Exception exception)
            {
                throw exception;
                //return 0;
            }
        }
        public int UpdateForumEntries(JobForum UpdateForumEntryRequest)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var qry = from p in dataContext.JobForums where p.JobGUID == UpdateForumEntryRequest.JobGUID select p;
                //    var item = qry.Single();
                //    item.ForumStatus = UpdateForumEntryRequest.ForumStatus;
                //    item.UserGUID = UpdateForumEntryRequest.UserGUID;
                //    item.ForumText = UpdateForumEntryRequest.ForumText;
                //    item.LastModifiedDate = DateTime.UtcNow;
                //    item.LastModifiedBy = UpdateForumEntryRequest.LastModifiedBy;
                //    return dataContext.SaveChanges();
                //}
                if (UpdateForumEntryRequest.JobGUID != null)
                {
                    JobForum forum = GetJobForumbyJobGUID(new Guid(UpdateForumEntryRequest.JobGUID.ToString()));
                    forum.ForumStatus = UpdateForumEntryRequest.ForumStatus;
                    forum.UserGUID = UpdateForumEntryRequest.UserGUID;
                    forum.ForumText = UpdateForumEntryRequest.ForumText;
                    forum.LastModifiedDate = DateTime.UtcNow;
                    forum.LastModifiedBy = UpdateForumEntryRequest.LastModifiedBy;

                    SqlParameter[] Param = new SqlParameter[5];
                    Param[0] = new SqlParameter("@pJobForumGUID", SqlDbType.UniqueIdentifier);
                    Param[0].Value = forum.JobForumGUID;
                    Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
                    Param[1].Value = (object)forum.UserGUID ?? DBNull.Value;
                    Param[2] = new SqlParameter("@pForumText", SqlDbType.NVarChar, -1);
                    Param[2].Value = (object)forum.ForumText ?? DBNull.Value;
                    Param[3] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                    Param[3].Value = (object)forum.LastModifiedDate ?? DBNull.Value;
                    Param[4] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
                    Param[4].Value = (object)forum.LastModifiedBy ?? DBNull.Value;

                    return context.Database.ExecuteSqlCommand("update JobForums set UserGUID=@pUserGUID,ForumText=@pForumText,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy"
                        + " where JobForumGUID=@pJobForumGUID", Param);
                }
                else
                    return 0;


            }
            catch (Exception exception)
            {
                return 0;
            }
        }

        public int InsertForumEntries(JobForum lJobForum)
        {
            //context.JobForums.Add(lJobForum);
            SqlParameter[] Param = new SqlParameter[12];
            Param[0] = new SqlParameter("@pJobForumGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = lJobForum.JobForumGUID;
            Param[1] = new SqlParameter("@pJobOfferGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)lJobForum.JobOfferGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)lJobForum.JobGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)lJobForum.UserGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pParentJobForumGUID", SqlDbType.UniqueIdentifier);
            Param[4].Value = (object)lJobForum.ParentJobForumGUID ?? DBNull.Value;
            Param[5] = new SqlParameter("@pForumText", SqlDbType.NVarChar, -1);
            Param[5].Value = (object)lJobForum.ForumText ?? DBNull.Value;
            Param[6] = new SqlParameter("@pIsRead", SqlDbType.Bit);
            Param[6].Value = (object)lJobForum.IsRead ?? DBNull.Value;
            Param[7] = new SqlParameter("@pForumStatus", SqlDbType.SmallInt);
            Param[7].Value = (object)lJobForum.ForumStatus ?? DBNull.Value;
            Param[8] = new SqlParameter("@pStartTimePreference", SqlDbType.DateTime);
            Param[8].Value = (object)lJobForum.StartTimePreference ?? DBNull.Value;
            Param[9] = new SqlParameter("@pCounterCostOffer", SqlDbType.Float);
            Param[9].Value = (object)lJobForum.CounterCostOffer ?? DBNull.Value;
            Param[10] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[10].Value = (object)lJobForum.LastModifiedDate ?? DBNull.Value;
            Param[11] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[11].Value = (object)lJobForum.LastModifiedBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into JobForums(JobForumGUID,JobOfferGUID,JobGUID,UserGUID,ParentJobForumGUID,ForumText,"
                + "IsRead,ForumStatus,StartTimePreference,CounterCostOffer,LastModifiedDate,LastModifiedBy)values(@pJobForumGUID,@pJobOfferGUID,@pJobGUID,@pUserGUID,@pParentJobForumGUID,@pForumText,"
                + "@pIsRead,@pForumStatus,@pStartTimePreference,@pCounterCostOffer,@pLastModifiedDate,@pLastModifiedBy)", Param);
        }

        public List<JobForum> GetForumEntries(Guid JobGUID)
        {

            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobForums
            //            where p.JobGUID == JobGUID
            //            select p).ToList();

            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobGUID;
            return context.Database.SqlQuery<JobForum>("select * from JobForums where JobGUID=@pJobGUID", Param).ToList();


        }

        public IEnumerable<Job> GetjobByUserandJobID(Guid UserGUID, Guid JobIndexGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where (p.AssignedUserGUID == UserGUID || p.JobGUID == JobIndexGUID) && p.IsDeleted == false
            //            select p).ToList().OrderByDescending(x => x.StatusCode);
            //}
            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            Param[1] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = JobIndexGUID;
            Param[2] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[2].Value = false;
            return context.Database.SqlQuery<Job>("select * from Jobs where IsDeleted=@pIsDeleted and (JobGUID=@pJobGUID or AssignedUserGUID=@pUserGUID)", Param);
        }


        public Job GetJobByID(Guid JobGUID)
        {
            //return context.Jobs.Find(JobGUID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobGUID;
            return context.Database.SqlQuery<Job>("Select * from Jobs where JobGUID=@pJobGUID", Param).FirstOrDefault();
        }

        /// <summary>
        /// Old 
        /// </summary>
        /// <param name="OrganizationGUID"></param>
        /// <returns></returns>
        public IEnumerable<Job> GetjobByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.OrganizationGUID == OrganizationGUID && p.StatusCode == 1
            //            select p).ToList().OrderBy(x => x.ScheduledStartTime);
            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[1].Value = 1;
            return context.Database.SqlQuery<Job>("Select * from Jobs where OrganizationGUID=@pOrganizationGUID and StatusCode=@pStatusCode", Param);
        }

        public IEnumerable<Job> GetjobByGroupGUID(Guid GroupGUID)
        {
            //using (var dataContext = new WorkersInMotionJobContext())
            //{
            //    return (from p in dataContext.Job
            //            where p.GroupCode == GroupGUID && p.Status == 2 && p.UserGUID == null
            //            select p).ToList().OrderBy(x => x.ServiceStartTime);
            //}

            return null;
        }

        public IEnumerable<Job> GetjobByTerritoryGUID(Guid TerritoryGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.TerritoryGUID == TerritoryGUID && p.StatusCode == 1
            //            select p).ToList().OrderBy(x => x.ScheduledStartTime);
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = TerritoryGUID;
            Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[1].Value = 1;
            return context.Database.SqlQuery<Job>("Select * from Jobs where TerritoryGUID=@pTerritoryGUID and StatusCode=@pStatusCode", Param);
        }

        public IEnumerable<Job> GetjobByOrganizationGUIDBetweenDate(Guid OrganizationGUID, DateTime fromdate, DateTime todate)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.OrganizationGUID == OrganizationGUID && p.StatusCode == 1 && p.PreferedEndTime <= todate && p.PreferedStartTime >= fromdate
            //            select p).ToList().OrderBy(x => x.ScheduledStartTime);
            //}
            SqlParameter[] Param = new SqlParameter[4];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            Param[1] = new SqlParameter("@pfromdate", SqlDbType.DateTime);
            Param[1].Value = fromdate;
            Param[2] = new SqlParameter("@ptodate", SqlDbType.DateTime);
            Param[2].Value = todate;
            Param[3] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[3].Value = 1;
            return context.Database.SqlQuery<Job>("Select * from Jobs where OrganizationGUID=@pOrganizationGUID and StatusCode=@pStatusCode and PreferedEndTime <=@ptodate and PreferedStartTime >=@pfromdate order by ScheduledStartTime", Param);
        }

        public IEnumerable<Job> GetjobByRegionandTerritory(Guid UserGUID)
        {
            //Guid RegionGUID, TerritoryGUID;
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    RegionGUID = new Guid((from p in dataContext.OrganizationUsersMaps
            //                           where p.UserGUID == UserGUID
            //                           select p).FirstOrDefault().RegionGUID.ToString());
            //    TerritoryGUID = new Guid((from p in dataContext.OrganizationUsersMaps
            //                              where p.UserGUID == UserGUID
            //                              select p).FirstOrDefault().TerritoryGUID.ToString());

            //    return (from p in dataContext.Jobs
            //            where p.RegionGUID == RegionGUID && p.TerritoryGUID == TerritoryGUID && p.StatusCode == 1 && p.AssignedUserGUID == null
            //            select p).ToList().OrderBy(x => x.ActualStartTime);
            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[1].Value = 1;
            return context.Database.SqlQuery<Job>("Select * from Jobs where RegionGUID in(select RegionGUID from OrganizationUsersMap where UserGUID=@pUserGUID) "
                + " and TerritoryGUID in(select TerritoryGUID from OrganizationUsersMap where UserGUID=@pUserGUID) and StatusCode=@pStatusCode and AssignedUserGUID is null", Param);
        }
        public IEnumerable<Job> GetjobStatusByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.OrganizationGUID == OrganizationGUID && p.StatusCode >= 2 && p.IsDeleted == false
            //            select p).ToList().OrderByDescending(x => x.ScheduledStartTime);
            //}

            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[1].Value = 2;
            Param[2] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[2].Value = false;
            return context.Database.SqlQuery<Job>("select * from Jobs where OrganizationGUID=@pOrganizationGUID and StatusCode >=@pStatusCode and IsDeleted=@pIsDeleted order by ScheduledStartTime desc", Param);

        }

        public IEnumerable<Job> GetjobStatusByUserGUID(Guid UserGUID, Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.OrganizationGUID == OrganizationGUID && p.AssignedUserGUID == UserGUID && p.StatusCode >= 2 && p.IsDeleted == false
            //            select p).ToList().OrderByDescending(x => x.ActualStartTime);
            //}
            SqlParameter[] Param = new SqlParameter[4];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = UserGUID;
            Param[2] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[2].Value = 2;
            Param[3] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[3].Value = false;
            return context.Database.SqlQuery<Job>("select * from Jobs where OrganizationGUID=@pOrganizationGUID and AssignedUserGUID=@pUserGUID and StatusCode>=@pStatusCode and IsDeleted=@pIsDeleted order by ActualStartTime desc", Param);

        }

        public IEnumerable<Job> GetjobStatusByUserGUIDBetweenDate(Guid UserGUID, DateTime fromdate, DateTime todate)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.AssignedUserGUID == UserGUID && p.StatusCode >= 2 && p.IsDeleted == false && p.PreferedEndTime <= todate && p.PreferedStartTime >= fromdate
            //            select p).ToList().OrderByDescending(x => x.ActualStartTime);
            //}

            SqlParameter[] Param = new SqlParameter[5];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[1].Value = 2;
            Param[2] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[2].Value = false;
            Param[3] = new SqlParameter("@pfromdate", SqlDbType.DateTime);
            Param[3].Value = fromdate;
            Param[4] = new SqlParameter("@ptodate", SqlDbType.DateTime);
            Param[4].Value = todate;
            return context.Database.SqlQuery<Job>("select * from Jobs where AssignedUserGUID=@pUserGUID and StatusCode >=@pStatusCode and IsDeleted=@pIsDeleted"
                + " and PreferedEndTime <= @ptodate and  PreferedStartTime >=@pfromdate order by ActualStartTime desc", Param);

        }

        public IEnumerable<Job> GetjobStatusByJobLogicalID(Guid JobLogicalID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.JobGUID == JobLogicalID && p.IsDeleted == false
            //            select p).ToList().OrderByDescending(x => x.ActualStartTime);
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pJobLogicalID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobLogicalID;
            Param[1] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[1].Value = false;
            return context.Database.SqlQuery<Job>("Select * from Jobs where JobGUID=@pJobLogicalID and IsDeleted=@pIsDeleted order by ActualStartTime", Param);
        }
        public IEnumerable<Job> GetjobStatusByRegionAndTerritory(Guid UserGUID)
        {
            //Guid RegionGUID, TerritoryGUID;
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    RegionGUID = new Guid((from p in dataContext.OrganizationUsersMaps
            //                           where p.UserGUID == UserGUID
            //                           select p).FirstOrDefault().RegionGUID.ToString());
            //    TerritoryGUID = new Guid((from p in dataContext.OrganizationUsersMaps
            //                              where p.UserGUID == UserGUID
            //                              select p).FirstOrDefault().TerritoryGUID.ToString());

            //    return (from p in dataContext.Jobs
            //            where p.RegionGUID == RegionGUID && p.TerritoryGUID == TerritoryGUID && p.StatusCode >= 2 && p.AssignedUserGUID != null && p.IsDeleted == false
            //            select p).ToList().OrderByDescending(x => x.ActualStartTime);
            //}

            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[1].Value = 2;
            Param[2] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[2].Value = false;
            return context.Database.SqlQuery<Job>("Select * from Jobs where RegionGUID in(select RegionGUID from OrganizationUsersMap where UserGUID=@pUserGUID) "
                + " and TerritoryGUID in(select TerritoryGUID from OrganizationUsersMap where UserGUID=@pUserGUID) and StatusCode >=@pStatusCode and IsDeleted=@pIsDeleted and AssignedUserGUID is not null order by ActualStartTime desc", Param);

        }

        public IEnumerable<Job> GetjobStatusByRegionAndTerritoryBetweenDate(Guid UserGUID, DateTime fromdate, DateTime todate)
        {
            //Guid RegionGUID, TerritoryGUID;
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    RegionGUID = new Guid((from p in dataContext.OrganizationUsersMaps
            //                           where p.UserGUID == UserGUID
            //                           select p).FirstOrDefault().RegionGUID.ToString());
            //    TerritoryGUID = new Guid((from p in dataContext.OrganizationUsersMaps
            //                              where p.UserGUID == UserGUID
            //                              select p).FirstOrDefault().TerritoryGUID.ToString());

            //    return (from p in dataContext.Jobs
            //            where p.RegionGUID == RegionGUID && p.TerritoryGUID == TerritoryGUID && p.StatusCode >= 2 && p.AssignedUserGUID != null && p.IsDeleted == false && p.PreferedEndTime <= todate && p.PreferedStartTime >= fromdate
            //            select p).ToList().OrderByDescending(x => x.ActualStartTime);
            //}

            SqlParameter[] Param = new SqlParameter[5];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            Param[1] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[1].Value = 2;
            Param[2] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[2].Value = false;
            Param[3] = new SqlParameter("@pfromdate", SqlDbType.DateTime);
            Param[3].Value = false;
            Param[4] = new SqlParameter("@ptodate", SqlDbType.DateTime);
            Param[4].Value = false;
            return context.Database.SqlQuery<Job>("Select * from Jobs where RegionGUID in(select RegionGUID from OrganizationUsersMap where UserGUID=@pUserGUID) "
                + " and TerritoryGUID in(select TerritoryGUID from OrganizationUsersMap where UserGUID=@pUserGUID) and StatusCode >=@pStatusCode and IsDeleted=@pIsDeleted and AssignedUserGUID is not null "
                + " PreferedEndTime <= @ptodate and  p.PreferedStartTime >= @pfromdate order by ActualStartTime desc", Param);

        }

        public IEnumerable<Job> GetJobByRegionandTerritory(Guid RegionGUID, Guid TerritoryGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.RegionGUID == RegionGUID && p.TerritoryGUID == TerritoryGUID && p.StatusCode >= 2 && p.AssignedUserGUID != null && p.IsDeleted == false
            //            select p).ToList().OrderBy(x => x.ActualStartTime);
            //}

            SqlParameter[] Param = new SqlParameter[4];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            Param[1] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = TerritoryGUID;
            Param[2] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[2].Value = 2;
            Param[3] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[3].Value = false;

            return context.Database.SqlQuery<Job>("Select * from Jobs where RegionGUID=@pRegionGUID"
                + " and TerritoryGUID=@pTerritoryGUID and StatusCode >=@pStatusCode and IsDeleted=@pIsDeleted and AssignedUserGUID is not null "
                + " order by ActualStartTime", Param);

        }

        public IEnumerable<Job> GetJobByLastDownloadtTime(string lastdownloadtime)
        {
            DateTime lTime = Convert.ToDateTime(lastdownloadtime);
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.PreferedStartTime >= lTime && p.PreferedStartTime <= DateTime.UtcNow && p.StatusCode >= 2 && p.AssignedUserGUID != null && p.IsDeleted == false
            //            select p).ToList().OrderBy(x => x.ActualStartTime);
            //}

            SqlParameter[] Param = new SqlParameter[4];
            Param[0] = new SqlParameter("@plTimeFrom", SqlDbType.DateTime);
            Param[0].Value = lTime;
            Param[1] = new SqlParameter("@plTimeTo", SqlDbType.DateTime);
            Param[1].Value = DateTime.UtcNow;
            Param[2] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[2].Value = 2;
            Param[3] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[3].Value = false;

            return context.Database.SqlQuery<Job>("Select * from Jobs where PreferedStartTime >= @plTimeFrom && p.PreferedStartTime <= @plTimeTo "
               + " and StatusCode >=@pStatusCode and IsDeleted=@pIsDeleted and AssignedUserGUID is not null "
               + " order by ActualStartTime", Param);
        }
        public IEnumerable<Job> GetJobByGroupGUIDforClient(Guid GroupGUID)
        {
            //using (var dataContext = new WorkersInMotionJobContext())
            //{
            //    return (from p in dataContext.Job
            //            where p.GroupCode == GroupGUID && p.Status >= 2 && p.UserGUID != null && p.IsDeleted == false
            //            select p).ToList().OrderBy(x => x.ServiceStartTime);
            //}
            return null;
        }

        public int InsertJob(Job job)
        {
            try
            {
                //context.Jobs.Add(job);
                SqlParameter[] Param = new SqlParameter[46];
                Param = SetParameters(job);

                return context.Database.ExecuteSqlCommand("insert into Jobs(JobGUID,JobReferenceNo,OrganizationGUID,RegionGUID,TerritoryGUID,LocationType,CustomerGUID,"
                    + "CustomerStopGUID,ServicePointGUID,ServiceAddress,Latitude,Longitude,AssignedUserGUID,ManagerUserGUID,IsActive,IsDeleted,IsUrgent,StatusCode,"
                    + "SubStatusCode,IsSecheduled,JobName,PreferedStartTime,PreferedEndTime,ScheduledStartTime,ScheduledEndTime,ActualStartTime,ActualEndTime,"
                    + "EstimatedDuration,QuotedDuration,ActualDuration,CostType,QuotedCost,ActualCost,JobForm,JobClass,SignOffRequired,SignoffName,PictureRequired,"
                    + "PictureDescription,LocationSpecific,PONumber,TermsURL,CreateDate,CreateBy,LastModifiedDate,LastModifiedBy)"
                    + "values(@pJobGUID,@pJobReferenceNo,@pOrganizationGUID,@pRegionGUID,@pTerritoryGUID,@pLocationType,@pCustomerGUID,"
                    + "@pCustomerStopGUID,@pServicePointGUID,@pServiceAddress,@pLatitude,@pLongitude,@pAssignedUserGUID,@pManagerUserGUID,@pIsActive,@pIsDeleted,@pIsUrgent,@pStatusCode,"
                    + "@pSubStatusCode,@pIsSecheduled,@pJobName,@pPreferedStartTime,@pPreferedEndTime,@pScheduledStartTime,@pScheduledEndTime,@pActualStartTime,@pActualEndTime,"
                    + "@pEstimatedDuration,@pQuotedDuration,@pActualDuration,@pCostType,@pQuotedCost,@pActualCost,@pJobForm,@pJobClass,@pSignOffRequired,@pSignoffName,@pPictureRequired,"
                    + "@pPictureDescription,@pLocationSpecific,@pPONumber,@pTermsURL,@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy)", Param);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }


        public int InsertJobbyStoreID(Job job)
        {
            try
            {
                //context.Jobs.Add(job);
                SqlParameter[] Param = new SqlParameter[28];
                Param = SetParametersForStoreCreate(job);

                return context.Database.ExecuteSqlCommand("insert into Jobs(JobGUID,OrganizationGUID,RegionGUID,TerritoryGUID,LocationType,CustomerGUID,"
                    + "CustomerStopGUID,ServiceAddress,Latitude,Longitude,AssignedUserGUID,ManagerUserGUID,IsActive,IsDeleted,IsUrgent,StatusCode,"
                    + "SubStatusCode,IsSecheduled,JobName,PreferedStartTime,PreferedEndTime,ScheduledStartTime,ActualStartTime,"
                    + "JobClass,"
                    + "CreateDate,CreateBy,LastModifiedDate,LastModifiedBy)"
                    + "values(@pJobGUID,@pOrganizationGUID,@pRegionGUID,@pTerritoryGUID,@pLocationType,@pCustomerGUID,"
                    + "@pCustomerStopGUID,@pServiceAddress,@pLatitude,@pLongitude,@pAssignedUserGUID,@pManagerUserGUID,@pIsActive,@pIsDeleted,@pIsUrgent,@pStatusCode,"
                    + "@pSubStatusCode,@pIsSecheduled,@pJobName,@pPreferedStartTime,@pPreferedEndTime,@pScheduledStartTime,@pActualStartTime,"
                    + "@pJobClass,"
                    + "@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy)", Param);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public int InsertJobbyPO(Job job)
        {
            try
            {
                //context.Jobs.Add(job);
                SqlParameter[] Param = new SqlParameter[29];
                Param = SetParametersForPOCreate(job);

                return context.Database.ExecuteSqlCommand("insert into Jobs(JobGUID,OrganizationGUID,RegionGUID,TerritoryGUID,LocationType,CustomerGUID,"
                    + "CustomerStopGUID,ServiceAddress,Latitude,Longitude,AssignedUserGUID,ManagerUserGUID,IsActive,IsDeleted,IsUrgent,StatusCode,"
                    + "SubStatusCode,IsSecheduled,JobName,PreferedStartTime,PreferedEndTime,ScheduledStartTime,ActualStartTime,"
                    + "JobClass,"
                    + "CreateDate,CreateBy,LastModifiedDate,LastModifiedBy,PONumber)"
                    + "values(@pJobGUID,@pOrganizationGUID,@pRegionGUID,@pTerritoryGUID,@pLocationType,@pCustomerGUID,"
                    + "@pCustomerStopGUID,@pServiceAddress,@pLatitude,@pLongitude,@pAssignedUserGUID,@pManagerUserGUID,@pIsActive,@pIsDeleted,@pIsUrgent,@pStatusCode,"
                    + "@pSubStatusCode,@pIsSecheduled,@pJobName,@pPreferedStartTime,@pPreferedEndTime,@pScheduledStartTime,@pActualStartTime,"
                    + "@pJobClass,"
                    + "@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy,@pPONumber)", Param);
            }
            catch (Exception ex)
            {
                //lretString.Append("InsertJobbyPO Error : " + ex.Message + "");
                return 0;
            }

        }

        public int DeleteJob(Guid JobGUID)
        {
            //Job Job = context.Jobs.Find(JobGUID);
            //if (Job != null)
            //{
            //    using (var dataContext = new WorkersInMotionDB())
            //    {
            //        List<JobProgress> jobProgress = (from p in dataContext.JobProgresses
            //                                         where p.JobGUID == Job.JobGUID
            //                                         select p).ToList();
            //        if (jobProgress != null)
            //        {
            //            foreach (JobProgress item in jobProgress)
            //            {
            //                dataContext.JobProgresses.Remove(item);
            //                dataContext.SaveChanges();
            //            }
            //        }
            //    }
            //    context.Jobs.Remove(Job);
            //    context.SaveChanges();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobGUID;
            //int result = context.Database.ExecuteSqlCommand("", Param);
            return context.Database.ExecuteSqlCommand("delete from JobProgress where JobGUID=@pJobGUID delete from Jobs where JobGUID=@pJobGUID", Param);

            //using (var dataContext = new WorkersInMotionJobDB())
            //{
            //    Job job = (from p in dataContext.Jobs
            //               where p.JobGUID == JobGUID
            //               select p).FirstOrDefault();

            //    if (job != null)
            //    {
            //        dataContext.Jobs.Remove(job);
            //        dataContext.SaveChanges();
            //    }
            //}
        }

        //public void DeleteJobByJobLogicalID(Guid JobLogicalID)
        //{
        //    //using (var dataContext = new WorkersInMotionJobDB())
        //    //{
        //    //    var joblist = (from p in dataContext.Jobs
        //    //                   where p.JobFormGUID == JobLogicalID
        //    //                   select p).ToList();

        //    //    foreach (var item in joblist)
        //    //    {
        //    //        dataContext.Jobs.Remove(item);
        //    //        dataContext.SaveChanges();
        //    //    }
        //    //}

        //}

        public void SetDeleteFlag(Guid JobIndexGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var qry = from p in dataContext.Jobs where p.JobGUID == JobIndexGUID select p;
            //    var item = qry.Single();
            //    item.IsDeleted = true;
            //    dataContext.SaveChanges();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobIndexGUID;
            context.Database.ExecuteSqlCommand("update jobs set IsDeleted=true where JobGUID=@pJobGUID", Param);



        }

        public string GetStatusName(int status)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobStatus
            //            where p.StatusCode == status
            //            select p.Status).SingleOrDefault();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pstatus", SqlDbType.Int);
            Param[0].Value = status;
            JobStatu jobStatus = context.Database.SqlQuery<JobStatu>("select * from JobStatus where StatusCode=@pstatus", Param).FirstOrDefault();
            if (jobStatus != null)
                return jobStatus.Status;
            else
                return string.Empty;
        }

        public int GetStatusID(string statusname)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobStatus
            //            where p.Status == statusname
            //            select p.StatusCode).SingleOrDefault();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pstatusname", SqlDbType.NVarChar, 50);
            Param[0].Value = statusname;
            JobStatu jobStatus = context.Database.SqlQuery<JobStatu>("select * from JobStatus where Status=@pstatusname", Param).FirstOrDefault();
            if (jobStatus != null)
                return jobStatus.StatusCode;
            else
                return 0;
        }
        public string GetCustomerName(Guid CustomerGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Places
            //            where p.PlaceGUID == CustomerGUID
            //            select p.PlaceName).SingleOrDefault();


            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pCustomerGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = CustomerGUID;
            Place place = context.Database.SqlQuery<Place>("select * from Places where PlaceGUID=@pCustomerGUID", Param).FirstOrDefault();
            if (place != null)
                return place.PlaceName;
            else
                return string.Empty; ;
        }
        public string GetGroupName(Guid GroupGUID)
        {
            //using (var dataContext = new WorkersInMotionContext())
            //{
            //    return (from p in dataContext.Group
            //            where p.GroupGUID == GroupGUID
            //            select p.Name).SingleOrDefault();


            //}
            return "";
        }

        public string GetCustomerPointName(Guid StopsGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.MarketGUID == StopsGUID
            //            select p.MarketName).SingleOrDefault();


            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pStopsGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = StopsGUID;
            Market market = context.Database.SqlQuery<Market>("select * from Markets where MarketGUID=@pStopsGUID", Param).FirstOrDefault();
            if (market != null)
                return market.MarketName;
            else
                return string.Empty; ;
        }
        public int DeleteJobByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var joblist = (from p in dataContext.Jobs
            //                   where p.OrganizationGUID == OrganizationGUID
            //                   select p).ToList();

            //    foreach (var item in joblist)
            //    {
            //        dataContext.Jobs.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from Jobs where OrganizationGUID=@pOrganizationGUID", Param);
        }
        //public void AssignUser(Guid UserGUID, Guid JobIndexGUID)
        //{
        //    using (var dataContext = new WorkersInMotionJobDB())
        //    {
        //        var qry = from p in dataContext.Jobs where p.JobGUID == JobIndexGUID select p;
        //        var item = qry.Single();
        //        item.AssignedUserGUID = UserGUID;
        //        item.ActialStartTime = DateTime.UtcNow;
        //        item.ActualEndTime = DateTime.UtcNow.AddDays(1);
        //        item.PreferedEndTime = DateTime.UtcNow;
        //        item.PreferedStartTime = DateTime.UtcNow;
        //        item.StatusCode = 2;
        //        dataContext.SaveChanges();
        //    }
        //}

        public int UpdateJobFromClient(Job job)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var qry = from p in dataContext.Jobs where p.JobGUID == job.JobGUID select p;
            //    var item = qry.Single();
            //    item.StatusCode = job.StatusCode;
            //    item.ActualStartTime = job.ActualStartTime;
            //    return dataContext.SaveChanges();
            //}

            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pStatusCode", SqlDbType.Int);
            Param[0].Value = job.StatusCode;
            Param[1] = new SqlParameter("@pActualStartTime", SqlDbType.DateTime);
            Param[1].Value = job.ActualStartTime;
            Param[2] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = job.JobGUID;
            return context.Database.ExecuteSqlCommand("update Jobs set StatusCode=@pStatusCode,ActualStartTime=@pActualStartTime where JobGUID=@pJobGUID", Param);


        }
        public int UpdateJob(Job job)
        {
            // context.Entry(job).State = EntityState.Modified;

            SqlParameter[] Param = new SqlParameter[46];
            Param = SetParameters(job);

            return context.Database.ExecuteSqlCommand("update Job set JobReferenceNo=@pJobReferenceNo,OrganizationGUID=@pOrganizationGUID,"
                + "RegionGUID=@pRegionGUID,TerritoryGUID=@pTerritoryGUID,LocationType=@pLocationType,CustomerGUID=@pCustomerGUID,"
                + "CustomerStopGUID=@pCustomerStopGUID,ServicePointGUID=@pServicePointGUID,ServiceAddress=@pServiceAddress,Latitude=@pLatitude,Longitude=@pLongitude,"
                + "AssignedUserGUID=@pAssignedUserGUID,ManagerUserGUID=@pManagerUserGUID,IsActive=@pIsActive,IsDeleted=@pIsDeleted,IsUrgent=@pIsUrgent,StatusCode=@pStatusCode,"
                + "SubStatusCode=@pSubStatusCode,IsSecheduled=@pIsSecheduled,JobName=@pJobName,PreferedStartTime=@pPreferedStartTime,PreferedEndTime=@pPreferedEndTime,"
                + "ScheduledStartTime=@pScheduledStartTime,ScheduledEndTime=@pScheduledEndTime,ActualStartTime=@pActualStartTime`,ActualEndTime=@pActualEndTime,"
                + "EstimatedDuration=@pEstimatedDuration,QuotedDuration=@pQuotedDuration,ActualDuration=@pActualDuration,CostType=@pCostType,"
                + "QuotedCost=@pQuotedCost,ActualCost=@pActualCost,JobForm=@pJobForm,JobClass=@pJobClass,SignOffRequired=@pSignOffRequired,SignoffName=@pSignoffName,PictureRequired=@pPictureRequired,"
                + "PictureDescription=@pPictureDescription,LocationSpecific=@pLocationSpecific,PONumber=@pPONumber,TermsURL=@pTermsURL,CreateDate=@pCreateDate,"
                + "CreateBy=@pCreateBy,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy where JobGUID=@pJobGUID", Param);

        }

        //public int Save()
        //{
        //    return context.SaveChanges();
        //}

        private bool disposed = false;


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public string GetOrganizationName(Guid pJobGUID)
        {
            //string lOrganizationName = string.Empty;
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var qry = from p in dataContext.Jobs where p.JobGUID == pJobGUID select p;
            //    var item = qry.Single();
            //    if (item != null && item.OrganizationGUID != Guid.Empty)
            //    {
            //        using (var dataContextOrganization = new WorkersInMotionDB())
            //        {
            //            var qryOrganization = from o in dataContextOrganization.Organizations where o.OrganizationGUID == item.OrganizationGUID select o;
            //            var itemOrganization = qryOrganization.Single();
            //            if (itemOrganization != null && !string.IsNullOrEmpty(itemOrganization.OrganizationName))
            //            {
            //                lOrganizationName = itemOrganization.OrganizationName;
            //            }
            //        }
            //    }

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = pJobGUID;
            Organization orgnization = context.Database.SqlQuery<Organization>("select * from Organizations where OrganizationGUID in (select OrganizationGUID from Jobs where JobGUID=@pJobGUID)", Param).FirstOrDefault();
            if (orgnization != null)
                return orgnization.OrganizationName;
            else
                return string.Empty;

        }


        public Job GetJobByPONumber(string pPONumber)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.PONumber == pPONumber
            //            select p).OrderByDescending(x => x.PreferedStartTime).FirstOrDefault();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pPONumber", SqlDbType.NVarChar, 50);
            Param[0].Value = pPONumber;
            return context.Database.SqlQuery<Job>("select * from Jobs where PONumber=@pPONumber", Param).FirstOrDefault();

        }

        ////
        //public String secondToHrMin2(double pSecond)
        //{
        //    String lRetValue;
        //    int lHours = (int)(pSecond / 3600);
        //    long lRemSeconds = ((int)(pSecond % 3600));
        //    int lMinutes = ((int)lRemSeconds) / 60;
        //    int lSeconds = ((int)(lRemSeconds % 60));

        //    if (0 < lHours)
        //    {
        //        lRetValue = String.format(" %d:%d:%d ", lHours, lMinutes, lSeconds);
        //    }
        //    else if (0 < lMinutes)
        //    {
        //        lRetValue = String.format(" 00:%d:%d", lMinutes, lSeconds);
        //    }
        //    else
        //    {
        //        lRetValue = String.format(" 00:00:%d", lSeconds);
        //    }
        //    return lRetValue;
        //}


        public Guid? GetOrganizationGUID(Guid pUserGUID)
        {
            Guid? lOrganizationGUID = null;
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var qry = from p in dataContext.OrganizationUsersMaps where p.UserGUID == pUserGUID select p;
            //    var item = qry.Single();
            //    if (item != null && item.OrganizationGUID != Guid.Empty)
            //    {
            //        lOrganizationGUID = item.OrganizationGUID;
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = pUserGUID;
            OrganizationUsersMap orgUserMap = context.Database.SqlQuery<OrganizationUsersMap>("select * from OrganizationUsersMap where UserGUID=@pUserGUID", Param).FirstOrDefault();
            if (orgUserMap != null)
                lOrganizationGUID = orgUserMap.OrganizationGUID;
            return lOrganizationGUID;
        }
    }
}