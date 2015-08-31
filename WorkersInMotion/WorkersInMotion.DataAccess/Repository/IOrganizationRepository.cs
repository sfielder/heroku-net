using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IOrganizationRepository : IDisposable
    {
        IEnumerable<Organization> GetOrganization();
        Organization GetOrganizationByID(Guid OrganizationGUID);
        Guid GetOrganizationIDByUserGUID(Guid UserGUID);
        List<OrganizationUsersMap> GetOrganizationUserMapByOrgGUID(Guid OrganizationGUID);
        OrganizationUsersMap GetOrganizationUserMapByUserGUID(Guid UserGUID);
        OrganizationUsersMap GetOrganizationUserMapByUserGUID(Guid UserGUID, Guid OrganizationGUID);
        int InsertOrganization(Organization organization);
        Organization GetOrganizationByName(string OrganizationName);
        int DeleteOrganization(Guid OrganizationGUID);
        int UpdateOrganization(Organization organization);
        int UpdateOrganizationUserMap(OrganizationUsersMap organizationUserMap);
        int InsertOrganizationUserMap(OrganizationUsersMap OrganizationUsersMap);
        int DeleteOrganizationUserMap(Guid OrganizationUserMapGUID);
        int DeleteOrganizationUserMapByUserGUID(Guid UserGUID);
        int DeleteOrganizationUserMapByOrganizationGUID(Guid OrganizationGUID);
        //int Save();
    }
}