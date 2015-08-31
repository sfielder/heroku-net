using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.DataAccess.Repository
{
    public interface IUserRepository : IDisposable
    {
        bool ValidateUser(string SessionGUID);
        System.Guid GetUserGUID(string SessionGUID);
        Guid GetOrganizationGUID(string SessionGUID);
        OrganizationUsersMap GetUserByID(Guid UserGUID);
        IEnumerable<AspNetUser> GetUsers();
        GlobalUser UserLogin(string userName, string password);
        List<p_GetUsers_Result> DownloadUsers(string SessionID, Nullable<System.Guid> UserGUID = null);
        MasterLogin UserLoginServer(string userName, string password);
        UserHeartBeat GetUserLocation(Guid WorkerID);
        int InsertUserHeartBeat(UserHeartBeat heartbeat);
        int DeleteUserDevices(Guid userDeviceID);
        //MasterLogin GetMasterLoginByUserGUID(Guid UserGUID);
        List<MasterLogin> GetMasterLogin(MasterLogin pMasterLogin);
        MasterLogin GetMasterLoginByLoginID(Guid LoginID);
        //UserDevice GetUserDeviceByLoginAndDeviceID(Guid LoginGUID, string DeviceGUID);
        List<UserDevice> GetUserDevice(UserDevice pUserDevice);
        //string UpdateMasterLoginEntries(Guid LoginGUID, Guid UserGUID);
        string UpdateMasterLogin(MasterLogin pMasterLogin);
        int UpdateMasterLogins(MasterLogin masterLogin);
        //GlobalUser GlobalUserLogin(string userName, string password);
        GlobalUser GlobalUserLogin(string userName, string OrganizationGUID);
        int InsertMasterLogin(MasterLogin masterlogin);
        int InsertUserDevice(UserDevice UserDevice);
        AspNetRole GetRole(string RoleID);
        AspNetRole GetRolebyUserType(string UserType);
        AspNetUser GetUserByID(string userID);
        IEnumerable<dynamic> GetJobCount(Guid OrganizationGUID);
        int InsertUser(AspNetUser user);
        int DeleteUser(string userID);
        int DeleteByOrganizationGUID(Guid OrganizationGUID);
        int UpdateUser(AspNetUser user);
        //int Save();
        string EncodeTo64(string toEncode);
        string DecodeFrom64(string encodedData);
        string GetLocalDateTime(DateTime? Convertdate, string timezoneid);
        DateTime? GetLocalDateTime_DateReturnType(DateTime? Convertdate, string ClientTimeZoneoffset);

        int CheckUserName(string pUserName);
    }
}