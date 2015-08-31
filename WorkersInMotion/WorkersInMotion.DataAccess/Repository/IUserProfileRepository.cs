using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;



namespace WorkersInMotion.DataAccess.Repository
{
    public interface IUserProfileRepository : IDisposable
    {
        IEnumerable<UserProfile> GetUserProfiles();
        UserProfile GetUserProfileByID(Guid ProfileGUID);
        IEnumerable<UserProfile> GetUserProfilesbyOrganizationGUID(Guid OrganizationGUID);
        IEnumerable<UserProfile> GetUserProfilesbyOrganizationGUID(Guid OrganizationGUID, string UserType);
        //IEnumerable<UserProfile> GetUserProfileByGroupGUID(Guid GroupGUID);
        UserProfile GetUserProfileByUserID(Guid UserGUID, Guid OrganizationGUID);
        string GetUserIDFromEmail(string emailID);
        int InsertUserProfile(UserProfile UserProfile);
        int DeleteUserProfile(Guid ProfileGUID);
        int DeleteUserProfileByUserGUID(Guid UserGUID);
        int UpdateUserProfile(UserProfile UserProfile);
        //int Save();
    }
}