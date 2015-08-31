using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.DataAccess.Repository
{
    public class JobSchemaRepository : IJobSchemaRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public JobSchemaRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }

        public IEnumerable<JobForm> GetJobSchema(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobForms
            //            where p.OrganizationGUID == OrganizationGUID && p.IsDeleted == false
            //            select p).ToList().OrderBy(x => x.FriendlyName);
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<JobForm>("select * from  JobForms where OrganizationGUID=@pOrganizationGUID order by FriendlyName");
        }

        public IEnumerable<JobForm> GetJobSchemabyGroupCode(string Skill)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobForms
            //            where p.Skill == Skill && p.IsDeleted == false
            //            select p).ToList().OrderBy(x => x.FriendlyName);
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pSkill", SqlDbType.NVarChar, -1);
            Param[0].Value = Skill;
            Param[1] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[1].Value = false;
            return context.Database.SqlQuery<JobForm>("select * from  JobForms where Skill=@pSkill and IsDeleted=@pIsDeleted order by FriendlyName");

        }
        public JobForm GetJobSchemabyJobFormID(Guid pjobFormID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobForms
            //            where p.JobFormGUID == pjobFormID && p.IsDeleted == false
            //            select p).FirstOrDefault();
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pjobFormID", SqlDbType.UniqueIdentifier);
            Param[0].Value = pjobFormID;
            Param[1] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[1].Value = false;
            return context.Database.SqlQuery<JobForm>("select * from  JobForms where JobFormGUID=@pjobFormID and IsDeleted=@pIsDeleted").FirstOrDefault();

        }
        public Guid GetJobFormIDfromJobForm(string jobform)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobForms
            //            where p.JobForm1 == jobform && p.IsDeleted == false
            //            select p).FirstOrDefault().JobFormGUID;
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pjobform", SqlDbType.Text);
            Param[0].Value = jobform;
            Param[1] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[1].Value = false;
            JobForm jobforms = context.Database.SqlQuery<JobForm>("select * from  JobForms where JobForm=@pjobform and IsDeleted=@pIsDeleted").FirstOrDefault();
            if (jobforms != null)
                return jobforms.JobFormGUID;
            else
                return Guid.Empty;
        }
        public JobForm JobSchemaDetails(Guid JobFormGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.JobForms
            //            where p.JobFormGUID == JobFormGUID && p.IsDeleted == false
            //            select p).SingleOrDefault();
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pJobFormGUID", SqlDbType.Text);
            Param[0].Value = JobFormGUID;
            Param[1] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[1].Value = false;
            return context.Database.SqlQuery<JobForm>("select * from  JobForms where JobFormGUID=@pJobFormGUID and IsDeleted=@pIsDeleted").FirstOrDefault();
        }
        //public IEnumerable<JobPageSchema> GetJobPageSchema(Guid JobLogicalID)
        //{
        //    using (var dataContext = new WorkersInMotionJobDB())
        //    {
        //        return (from p in dataContext.JobPageSchema
        //                where p.JobLogicalID == JobLogicalID
        //                select p).OrderBy(p => p.CreateDate).ToList();
        //    }
        //}

        public IEnumerable<Job> GetjobByJobFormClass(Int16 jobclass, Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Jobs
            //            where p.JobClass == jobclass && p.OrganizationGUID == OrganizationGUID && p.IsDeleted == false
            //            select p).ToList().OrderBy(x => x.JobName);
            //}
            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pjobclass", SqlDbType.SmallInt);
            Param[0].Value = jobclass;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationGUID;
            Param[2] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[2].Value = false;
            return context.Database.SqlQuery<Job>("select * from  Jobs where JobClass=@pjobclass and pOrganizationGUID=@pOrganizationGUID  and IsDeleted=@pIsDeleted").ToList();
        }
        //public IEnumerable<JobItemSchema> GetJobItemSchema(Guid JobLogicalID)
        //{
        //    using (var dataContext = new WorkersInMotionJobDB())
        //    {
        //        return (from p in dataContext.JobItemSchema
        //                where p.JobLogicalID == JobLogicalID
        //                select p).OrderBy(p => p.Createdate).ToList();
        //    }
        //}
        public int SetDeleteFlag(Guid JobFormGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var qry = from p in dataContext.JobForms where p.JobFormGUID == JobFormGUID select p;
            //    var item = qry.Single();
            //    item.IsDeleted = true;
            //    return dataContext.SaveChanges();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobFormGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobFormGUID;
            return context.Database.ExecuteSqlCommand("update JobForms set IsDeleted=true where JobFormGUID =@pJobFormGUID", Param);
        }
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
        //public int Save()
        //{
        //    return context.SaveChanges();
        //}
        //public void InsertJobItemSchema(JobItemSchema JobItemSchema)
        //{
        //    context.JobItemSchema.Add(JobItemSchema);
        //}

        //public void InsertJobPageSchema(JobPageSchema JobPageSchema)
        //{
        //    context.JobPageSchema.Add(JobPageSchema);
        //}

        public int InsertJobSchema(JobForm JobForm)
        {
            //context.JobForms.Add(JobForm);

            SqlParameter[] Param = new SqlParameter[11];
            Param[0] = new SqlParameter("@pJobFormGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobForm.JobFormGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)JobForm.OrganizationGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pSkill", SqlDbType.NVarChar, -1);
            Param[2].Value = (object)JobForm.Skill ?? DBNull.Value;
            Param[3] = new SqlParameter("@pJobClass", SqlDbType.SmallInt);
            Param[3].Value = (object)JobForm.JobClass ?? DBNull.Value;
            Param[4] = new SqlParameter("@pLocationType", SqlDbType.SmallInt);
            Param[4].Value = (object)JobForm.LocationType ?? DBNull.Value;
            Param[5] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[5].Value = (object)JobForm.IsActive ?? DBNull.Value;
            Param[6] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[6].Value = (object)JobForm.IsDeleted ?? DBNull.Value;
            Param[7] = new SqlParameter("@pFriendlyName", SqlDbType.NVarChar, 50);
            Param[7].Value = (object)JobForm.FriendlyName ?? DBNull.Value;
            Param[8] = new SqlParameter("@pJobForm1", SqlDbType.Text);
            Param[8].Value = (object)JobForm.JobForm1 ?? DBNull.Value;
            Param[9] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[9].Value = (object)JobForm.LastModifiedDate ?? DBNull.Value;
            Param[10] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[10].Value = (object)JobForm.LastModifiedBy ?? DBNull.Value;


            return context.Database.ExecuteSqlCommand("insert into JobForms(JobFormGUID,OrganizationGUID,Skill,JobClass,LocationType,IsActive,IsDeleted,FriendlyName,"
                            + "JobForm1,LastModifiedDate,LastModifiedBy)values(@pJobFormGUID,@pOrganizationGUID,@pSkill,@pJobClass,@pLocationType,@pIsActive,@pIsDeleted,@pFriendlyName,"
                            + "@pJobForm1,@pLastModifiedDate,@pLastModifiedBy)", Param);
        }
        //public void DeleteJobPageByJobLogicalGUID(Guid LogicalGUID)
        //{
        //    using (var dataContext = new WorkersInMotionJobDB())
        //    {
        //        var JobPageList = (from p in dataContext.JobPageSchema
        //                           where p.JobLogicalID == LogicalGUID
        //                           select p).ToList();
        //        foreach (var item in JobPageList)
        //        {
        //            dataContext.JobPageSchema.Remove(item);
        //            dataContext.SaveChanges();
        //        }
        //    }
        //}

        //public void DeleteJobPageItemByJobLogicalGUID(Guid LogicalGUID)
        //{
        //    using (var dataContext = new WorkersInMotionJobContext())
        //    {
        //        var JobItemList = (from p in dataContext.JobItemSchema
        //                           where p.JobLogicalID == LogicalGUID
        //                           select p).ToList();
        //        foreach (var item in JobItemList)
        //        {
        //            dataContext.JobItemSchema.Remove(item);
        //            dataContext.SaveChanges();
        //        }
        //    }
        //}

        //public void DeleteJobPageItemByJobPageGUID(Guid pageGUID)
        //{
        //    using (var dataContext = new WorkersInMotionJobContext())
        //    {
        //        var JobItemList = (from p in dataContext.JobItemSchema
        //                           where p.PageLogicalID == pageGUID
        //                           select p).ToList();
        //        foreach (var item in JobItemList)
        //        {
        //            dataContext.JobItemSchema.Remove(item);
        //            dataContext.SaveChanges();
        //        }
        //    }
        //}
        //public void DeleteJobPageByID(Guid PageLogicalGUID)
        //{
        //    JobPageSchema JobPageSchema = context.JobPageSchema.Find(PageLogicalGUID);
        //    if (JobPageSchema != null)
        //        context.JobPageSchema.Remove(JobPageSchema);
        //}

        public int DeleteJobSchema(Guid JobFormGUID)
        {
            //JobForm JobSchema = context.JobForms.Find(JobFormGUID);
            //if (JobSchema != null)
            //    context.JobForms.Remove(JobSchema);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pJobFormGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = JobFormGUID;
            return context.Database.ExecuteSqlCommand("delete from JobForms where JobFormGUID =@pJobFormGUID", Param);
        }


    }
}