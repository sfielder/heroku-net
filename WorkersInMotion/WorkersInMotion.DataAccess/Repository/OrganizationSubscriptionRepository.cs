using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;



namespace WorkersInMotion.DataAccess.Repository
{
    public class OrganizationSubscriptionRepository : IOrganizationSubscriptionRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public OrganizationSubscriptionRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }

        public IEnumerable<OrganizationSubscription> GetOrganizationSubscription()
        {
            //var OrganizationSubscription = context.OrganizationSubscriptions.ToList();
            //return context.OrganizationSubscriptions.ToList();

            return context.Database.SqlQuery<OrganizationSubscription>("select * from  OrganizationSubscriptions");
        }

        public OrganizationSubscription GetOrganizationSubscriptionByOrgID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.OrganizationSubscriptions
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).FirstOrDefault();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;

            return context.Database.SqlQuery<OrganizationSubscription>("Select * from OrganizationSubscriptions where OrganizationGUID=@pOrganizationGUID", Param).FirstOrDefault();
        }

        public OrganizationSubscription GetOrganizationSubscriptionBySubscriptionID(Guid OrganizationSubscriptionGUID)
        {
            //return context.OrganizationSubscriptions.Find(OrganizationSubscriptionGUID);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationSubscriptionGUID;

            return context.Database.SqlQuery<OrganizationSubscription>("Select * from OrganizationSubscriptions where OrganizationSubscriptionGUID=@pOrganizationSubscriptionGUID", Param).FirstOrDefault();
        }


        public int InsertOrganizationSubscription(OrganizationSubscription OrganizationSubscription)
        {
            //context.OrganizationSubscriptions.Add(OrganizationSubscription);
            SqlParameter[] Param = new SqlParameter[9];
            Param[0] = new SqlParameter("@pOrganizationSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationSubscription.OrganizationSubscriptionGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationSubscription.OrganizationGUID;
            Param[2] = new SqlParameter("@pVersion", SqlDbType.Int);
            Param[2].Value = OrganizationSubscription.Version;
            Param[3] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[3].Value = OrganizationSubscription.IsActive;
            Param[4] = new SqlParameter("@pSubscriptionPurchased", SqlDbType.Int);
            Param[4].Value = OrganizationSubscription.SubscriptionPurchased;
            Param[5] = new SqlParameter("@pSubscriptionConsumed", SqlDbType.Int);
            Param[5].Value = OrganizationSubscription.SubscriptionConsumed;
            Param[6] = new SqlParameter("@pStartDate", SqlDbType.DateTime);
            Param[6].Value = (object)OrganizationSubscription.StartDate ?? DBNull.Value;
            Param[7] = new SqlParameter("@pExpiryDate", SqlDbType.DateTime);
            Param[7].Value = (object)OrganizationSubscription.ExpiryDate ?? DBNull.Value;
            Param[8] = new SqlParameter("@pCreatedDate", SqlDbType.DateTime);
            Param[8].Value = (object)OrganizationSubscription.CreatedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into OrganizationSubscriptions(OrganizationSubscriptionGUID,OrganizationGUID,Version,IsActive,SubscriptionPurchased,"
                + "SubscriptionConsumed,StartDate,ExpiryDate,CreatedDate)values(@pOrganizationSubscriptionGUID,@pOrganizationGUID,@pVersion,@pIsActive,@pSubscriptionPurchased,"
                + "@pSubscriptionConsumed,@pStartDate,@pExpiryDate,@pCreatedDate)", Param);

        }

        public int DeleteOrganizationSubscription(Guid OrganizationSubscriptionGUID)
        {
            //OrganizationSubscription OrganizationSubscription = context.OrganizationSubscriptions.Find(OrganizationSubscriptionGUID);
            //if (OrganizationSubscription != null)
            //    context.OrganizationSubscriptions.Remove(OrganizationSubscription);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationSubscriptionGUID;
            return context.Database.ExecuteSqlCommand("delete from OrganizationSubscriptions where OrganizationSubscriptionGUID=@pOrganizationSubscriptionGUID", Param);
        }

        public int DeleteOrganizationSubscriptionByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var OrganizationSubscriptionlist = (from p in dataContext.OrganizationSubscriptions
            //                                        where p.OrganizationGUID == OrganizationGUID
            //                                        select p).ToList();
            //    foreach (var item in OrganizationSubscriptionlist)
            //    {
            //        dataContext.OrganizationSubscriptions.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from OrganizationSubscriptions where OrganizationGUID=@pOrganizationGUID", Param);
        }

        public int UpdateOrganizationSubscription(OrganizationSubscription OrganizationSubscription)
        {
            // context.Entry(OrganizationSubscription).State = EntityState.Modified;

            SqlParameter[] Param = new SqlParameter[9];
            Param[0] = new SqlParameter("@pOrganizationSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationSubscription.OrganizationSubscriptionGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationSubscription.OrganizationGUID;
            Param[2] = new SqlParameter("@pVersion", SqlDbType.Int);
            Param[2].Value = OrganizationSubscription.Version;
            Param[3] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[3].Value = OrganizationSubscription.IsActive;
            Param[4] = new SqlParameter("@pSubscriptionPurchased", SqlDbType.Int);
            Param[4].Value = OrganizationSubscription.SubscriptionPurchased;
            Param[5] = new SqlParameter("@pSubscriptionConsumed", SqlDbType.Int);
            Param[5].Value = OrganizationSubscription.SubscriptionConsumed;
            Param[6] = new SqlParameter("@pStartDate", SqlDbType.DateTime);
            Param[6].Value = (object)OrganizationSubscription.StartDate ?? DBNull.Value;
            Param[7] = new SqlParameter("@pExpiryDate", SqlDbType.DateTime);
            Param[7].Value = (object)OrganizationSubscription.ExpiryDate ?? DBNull.Value;
            Param[8] = new SqlParameter("@pCreatedDate", SqlDbType.DateTime);
            Param[8].Value = (object)OrganizationSubscription.CreatedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("update OrganizationSubscriptions set OrganizationGUID=@pOrganizationGUID,Version=@pVersion,IsActive=@pIsActive,SubscriptionPurchased=@pSubscriptionPurchased,"
                + "SubscriptionConsumed=@pSubscriptionConsumed,StartDate=@pStartDate,ExpiryDate=@pExpiryDate,CreatedDate=@pCreatedDate where OrganizationSubscriptionGUID=@pOrganizationSubscriptionGUID", Param);

        }
        public int UpdateOrganizationSubscriptionCount(OrganizationSubscription OrganizationSubscription)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var qry = from p in dataContext.OrganizationSubscriptions where p.OrganizationSubscriptionGUID == OrganizationSubscription.OrganizationSubscriptionGUID select p;
            //    var item = qry.Single();
            //    item.SubscriptionConsumed = item.SubscriptionConsumed + 1;
            //    return dataContext.SaveChanges();
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pSubscriptionConsumed", SqlDbType.Int);
            Param[0].Value = OrganizationSubscription.SubscriptionConsumed + 1;
            Param[1] = new SqlParameter("@pOrganizationSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationSubscription.OrganizationSubscriptionGUID;

            return context.Database.ExecuteSqlCommand("Update OrganizationSubscriptions set SubscriptionConsumed=@pSubscriptionConsumed where OrganizationSubscriptionGUID=@pOrganizationSubscriptionGUID", Param);
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

    }
}