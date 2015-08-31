using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IUserSubscriptionRepository : IDisposable
    {
        IEnumerable<UserSubscription> GetUserSubscription();
        UserSubscription GetUserSubscriptionByUserID(Guid UserGUID);
        UserSubscription GetUserSubscriptionBySubscriptionID(Guid UserSubscriptionGUID);
        int InsertUserSubscription(UserSubscription UserSubscription);
        int DeleteUserSubscription(Guid UserSubscriptionGUID);
        int DeleteUserSubscriptionByUserGUID(Guid UserGUID);
        int DeleteUserSubscriptionByOrgSubID(Guid OrgSubscriptionGUID);
        int UpdateUserSubscription(UserSubscription UserSubscription);
        //int Save();
    }
}