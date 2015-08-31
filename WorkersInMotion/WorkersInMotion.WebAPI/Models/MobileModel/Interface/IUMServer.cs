using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.WebAPI.Models.MobileModel.Interface
{
    public interface IUMServer
    {
        bool ValidateUser(string SessionGUID);
        string convertdate(DateTime date);
        System.Guid GetUserGUID(string SessionGUID);
        LoginResponse Login(LoginRequest pLoginRequest);
        CreateAccountResponse CreateAccount(LoginRequest pLoginRequest);
        int ForgotPassword(ForgotPasswordRequest pForgotPasswordRequest);
        GetRouteUserResponse GetRouteUsers(GetRouteUserRequest GetRouteUserRequest);
        int PostHeartBeat(HeartBeatRequest HeartBeatRequest, Guid UserGUID);
        LocationResponse GetUserLocation(Guid WorkerID);
        MobilePlace ConvertPlaceforMobile(Place Place);
        MobilePeople ConvertPeopleForMobile(Person Person);
        MobileMarket ConvertMarketforMobile(Market _market);
        TerritoryRegion GetTerritoryRegion(Guid UserGUID);
        DownloadUsers DownloadUsers(string SessionID, Nullable<System.Guid> UserGUID = null);

        UMResponse GetUsers(string SessionID);
        UMResponseOrganization GetOrganization(string SessionID);
        Guid GetOrganizationGUIDBySessionID(string SessionID);
        List<MCustomers> GetCustomers(Guid pOrganizatonGUID);
    }
}
