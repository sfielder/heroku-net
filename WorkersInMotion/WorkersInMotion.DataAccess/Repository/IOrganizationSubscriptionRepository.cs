using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IOrganizationSubscriptionRepository : IDisposable
    {
        IEnumerable<OrganizationSubscription> GetOrganizationSubscription();
        OrganizationSubscription GetOrganizationSubscriptionByOrgID(Guid OrganizationGUID);
        OrganizationSubscription GetOrganizationSubscriptionBySubscriptionID(Guid OrganizationSubscriptionGUID);
        int InsertOrganizationSubscription(OrganizationSubscription OrganizationSubscription);
        int DeleteOrganizationSubscription(Guid OrganizationSubscriptionGUID);
        int DeleteOrganizationSubscriptionByOrganizationGUID(Guid OrganizationGUID);
        int UpdateOrganizationSubscription(OrganizationSubscription OrganizationSubscription);
        int UpdateOrganizationSubscriptionCount(OrganizationSubscription OrganizationSubscription);
        //int Save();
    }
}