using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IGlobalUserRepository : IDisposable
    {
        IEnumerable<GlobalUser> GetGlobalUser();
        IEnumerable<AspNetRole> GetRoles();
        GlobalUser GetGlobalUserByID(Guid UserGUID);
        //GlobalUser GetGlobalUserByUserID(string UserID);
        GlobalUser GetGlobalUserByUserID(string UserID, string OrganizationGUID);
        IEnumerable<GlobalUser> GetGlobalUserByOrganizationGUID(Guid OrganizationGUID);

        IEnumerable<GlobalUser> GetGlobalUserByRegionandTerritory(Guid RegionGUID, Guid TerritoryGUID);
        string GetOrganizationAdminRoleID();
        int InsertGlobalUser(GlobalUser globaluser);
        GlobalUser GetPasswordFromUserGUID(Guid UserGUID);
        string GetPassword(Guid UserID);
        string GetUserTypeByRoleID(string RoleGUID);
        int DeleteGlobalUser(Guid UserGUID);
        //  void DeleteGlobalUserByOrganizationGUID(Guid OrganizationGUID);
        int UpdateGlobalUser(GlobalUser globaluser);
        string GetUserType(Guid UserID);
        string GetUserRoleName(Guid UserID);
        AspNetRole GetRole(string RoleID);
        string GetRoleID(string UserType);
        //int Save();
        //GetUsers GetClientUsers(string SessionID);
        //GetUsers GetUserFromClient(Guid UserGUID, Guid OrganizationGUID);
        //int UpdateGroupGUID(Guid UserGUID, Guid GroupGUID);
        string GetUserID(string SessionID);
        string GetOrganizationID(string SessionID);
        //GlobalUser DeviceLogin(GlobalUser plGlobalUser);
    }
}