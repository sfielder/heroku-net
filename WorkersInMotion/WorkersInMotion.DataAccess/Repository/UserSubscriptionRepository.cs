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
    public class UserSubscriptionRepository : IUserSubscriptionRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public UserSubscriptionRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }
        public IEnumerable<UserSubscription> GetUserSubscription()
        {
            //var UserSubscription = context.UserSubscriptions.ToList();
            //return context.UserSubscriptions.ToList();
            return context.Database.SqlQuery<UserSubscription>("select * from UserSubscriptions").ToList();
        }

        public UserSubscription GetUserSubscriptionByUserID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.UserSubscriptions
            //            where p.UserGUID == UserGUID
            //            select p).FirstOrDefault();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.SqlQuery<UserSubscription>("select * from UserSubscriptions where UserGUID=@pUserGUID", Param).FirstOrDefault();

        }

        public UserSubscription GetUserSubscriptionBySubscriptionID(Guid UserSubscriptionGUID)
        {
            //return context.UserSubscriptions.Find(UserSubscriptionGUID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserSubscriptionGUID;
            return context.Database.SqlQuery<UserSubscription>("select * from UserSubscriptions where UserSubscriptionGUID=@pUserSubscriptionGUID", Param).FirstOrDefault();
        }

        public int InsertUserSubscription(UserSubscription UserSubscription)
        {
            // context.UserSubscriptions.Add(UserSubscription);

            SqlParameter[] Param = new SqlParameter[5];
            Param[0] = new SqlParameter("@pUserSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserSubscription.UserSubscriptionGUID;
            Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = UserSubscription.UserGUID;
            Param[2] = new SqlParameter("@pOrganizationSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)UserSubscription.OrganizationSubscriptionGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[3].Value = (object)UserSubscription.IsActive ?? DBNull.Value;
            Param[4] = new SqlParameter("@pCreatedDate", SqlDbType.DateTime);
            Param[4].Value = (object)UserSubscription.CreatedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into UserSubscriptions(UserSubscriptionGUID,UserGUID,OrganizationSubscriptionGUID,IsActive,CreatedDate) values(@pUserSubscriptionGUID,@pUserGUID,@pOrganizationSubscriptionGUID,@pIsActive,@pCreatedDate)", Param);
        }

        public int DeleteUserSubscription(Guid UserSubscriptionGUID)
        {
            //UserSubscription UserSubscription = context.UserSubscriptions.Find(UserSubscriptionGUID);
            //if (UserSubscription != null)
            //    context.UserSubscriptions.Remove(UserSubscription);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserSubscriptionGUID;
            return context.Database.ExecuteSqlCommand("delete from UserSubscriptions where UserSubscriptionGUID=@pUserSubscriptionGUID", Param);
        }
        public int DeleteUserSubscriptionByOrgSubID(Guid OrgSubscriptionGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var UserSubscriptionlist = (from p in dataContext.UserSubscriptions
            //                                where p.OrganizationSubscriptionGUID == OrgSubscriptionGUID
            //                                select p).ToList();
            //    foreach (var item in UserSubscriptionlist)
            //    {
            //        dataContext.UserSubscriptions.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrgSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrgSubscriptionGUID;
            return context.Database.ExecuteSqlCommand("delete from UserSubscriptions where OrganizationSubscriptionGUID=@pOrgSubscriptionGUID", Param);
        }


        public int DeleteUserSubscriptionByUserGUID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var UserSubscriptionlist = (from p in dataContext.UserSubscriptions
            //                                where p.UserGUID == UserGUID
            //                                select p).ToList();
            //    foreach (var item in UserSubscriptionlist)
            //    {
            //        dataContext.UserSubscriptions.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.ExecuteSqlCommand("delete from UserSubscriptions where UserGUID=@pUserGUID", Param);
        }

        public int UpdateUserSubscription(UserSubscription UserSubscription)
        {
            // context.Entry(UserSubscription).State = EntityState.Modified;

            SqlParameter[] Param = new SqlParameter[5];
            Param[0] = new SqlParameter("@pUserSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserSubscription.UserSubscriptionGUID;
            Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = UserSubscription.UserGUID;
            Param[2] = new SqlParameter("@pOrganizationSubscriptionGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)UserSubscription.OrganizationSubscriptionGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[3].Value = (object)UserSubscription.IsActive ?? DBNull.Value;
            Param[4] = new SqlParameter("@pCreatedDate", SqlDbType.DateTime);
            Param[4].Value = (object)UserSubscription.CreatedDate ?? DBNull.Value;
            return context.Database.ExecuteSqlCommand("Update UserSubscriptions set UserGUID=@pUserGUID,OrganizationSubscriptionGUID=@pOrganizationSubscriptionGUID,IsActive=@pIsActive,CreatedDate=@pCreatedDate where UserSubscriptionGUID=@pUserSubscriptionGUID", Param);
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