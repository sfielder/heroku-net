using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkersInMotion.Log;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;
using System.Web.Script.Serialization;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Repository;

namespace WorkersInMotion.WebAPI.Models.MobileModel.Service
{
    public class UMServer : IUMServer
    {
        #region Variables Declaration
        readonly ILogService _ILogservice;
        protected ILogService Logger
        {
            get
            {
                return _ILogservice;
            }
        }
        #endregion
        private string FILE_PATH = null;
        string m_cCCEmailId = null;
        string m_cFromEmailId = null;
        string m_cSMTPHost = null;
        string m_cServerURL = null;
        string m_cSMTPUserName = null;
        private string m_cPortNo = null;
        int m_cDeleteOldTrackingInfo = 10;

        public UMServer()
        {
            _ILogservice = new Log4NetService(GetType());
        }
        public string convertdate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") == "0001-01-01T00:00:00Z" ? "" : date.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
        public bool ValidateUser(string SessionGUID)
        {
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            return (_IUserRepository.ValidateUser(SessionGUID));

        }
        public System.Guid GetUserGUID(string SessionGUID)
        {
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            return (_IUserRepository.GetUserGUID(SessionGUID));
        }

        public LoginResponse Login(LoginRequest pLoginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            string UpdatedSessionID = string.Empty;
            Logger.Debug("Inside UserLogin");
            LoginResponse lResponse = new LoginResponse();
            try
            {
                MasterLogin masterlogin = new MasterLogin();
                var aspuser = _IUserRepository.UserLogin(pLoginRequest.UserName, _IUserRepository.EncodeTo64(pLoginRequest.Password));
                if (aspuser != null)
                {
                    Logger.Debug("Inside Role");
                    AspNetRole asprole = _IUserRepository.GetRole(aspuser.Role_Id);
                    switch (asprole.UserType)
                    {
                        case "WIM_A":
                        case "ENT_A":
                        case "ENT_OM":
                        case "ENT_U_RM":
                        case "ENT_U_TM":
                            lResponse.Role = 1;
                            break;
                        case "ENT_U":
                            lResponse.Role = 2;
                            break;
                        case "IND_C":
                            lResponse.Role = 3;
                            break;
                        default:
                            break;
                    }

                    UserDevice userDevice = new UserDevice();
                    List<MasterLogin> masterlogins = new List<MasterLogin>();
                    MasterLogin lMasterLogin = new MasterLogin();
                    lMasterLogin.UserGUID = aspuser.UserGUID;
                    lMasterLogin.LoginType = (short)pLoginRequest.LoginType;
                    masterlogins = _IUserRepository.GetMasterLogin(lMasterLogin);
                    if (masterlogins != null && masterlogins.Count > 0)
                    {
                        #region masterlogins record available
                        masterlogin = masterlogins[0]; // Alok need to be fixed
                        // Update the Master Login
                        masterlogin.ExpiryTime = DateTime.UtcNow.AddYears(10);
                        Logger.Debug("Updating MasterLogin Record");
                        UpdatedSessionID = _IUserRepository.UpdateMasterLogin(masterlogin);
                        if (!string.IsNullOrEmpty(UpdatedSessionID))
                        {
                            #region UpdatedSessionID is not null

                            Logger.Debug("Updated Session ID: " + UpdatedSessionID);
                            lResponse.SessionID = UpdatedSessionID;
                            lResponse.UserGUID = aspuser.UserGUID.ToString();

                            Logger.Debug("Inside MasterLogin");
                            userDevice.LoginGUID = masterlogin.LoginGUID;
                            userDevice.DeviceID = pLoginRequest.DeviceInfo.deviceid;
                            List<UserDevice> lUserDevices = _IUserRepository.GetUserDevice(userDevice);
                            if (lUserDevices != null && lUserDevices.Count > 0)
                            {
                                // Delete the user device record
                                userDevice = lUserDevices[0]; // Need to modify Alok
                                int deviceresult = _IUserRepository.DeleteUserDevices(userDevice.UserDevicesGUID);
                                //int deviceresult = _IUserRepository.Save();
                                if (deviceresult <= 0)
                                {
                                    lResponse = null;
                                    return lResponse;
                                }
                            }
                            // Insert the User Device info
                            if (CreateUserDevice(masterlogin, pLoginRequest) > 0)
                            {
                                if (!string.IsNullOrEmpty(lResponse.SessionID) && !string.IsNullOrEmpty(lResponse.UserGUID))
                                {
                                    DownloadUsers lDownloadUsers = DownloadUsers(lResponse.SessionID, new Guid(lResponse.UserGUID));
                                    if (lDownloadUsers != null && lDownloadUsers.UserRecords.Count > 0)
                                        lResponse.UserRecord = lDownloadUsers.UserRecords[0];
                                    else
                                        lResponse.UserRecord = null;
                                }
                                Logger.Debug("UserDevice record created for updated Session ID: " + UpdatedSessionID);

                            }
                            #endregion
                        }
                        else
                        {
                            #region UpdatedSessionID is NULL
                            Logger.Error("Unable to generate Session ID");
                            lResponse = null;
                            return lResponse;
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region masterlogins record not available
                        Logger.Debug("Creating MasterLogin Record");
                        lMasterLogin.ExpiryTime = DateTime.UtcNow.AddYears(10);
                        if (CreateMasterLogin(lMasterLogin) > 0)
                        {
                            Logger.Debug("New Session ID: " + lMasterLogin.SessionID);
                            lResponse.SessionID = lMasterLogin.SessionID;
                            lResponse.UserGUID = lMasterLogin.UserGUID.ToString();
                            Logger.Debug("Inside UserDevice create");

                            if (CreateUserDevice(masterlogin, pLoginRequest) > 0)
                            {
                                if (!string.IsNullOrEmpty(lResponse.SessionID) && !string.IsNullOrEmpty(lResponse.UserGUID))
                                {
                                    DownloadUsers lDownloadUsers = DownloadUsers(lResponse.SessionID, new Guid(lResponse.UserGUID));
                                    if (lDownloadUsers != null && lDownloadUsers.UserRecords.Count > 0)
                                        lResponse.UserRecord = lDownloadUsers.UserRecords[0];
                                    else
                                        lResponse.UserRecord = null;
                                }
                                Logger.Debug("UserDevice record created for new Session ID: " + lMasterLogin.SessionID);
                            }
                            else
                            {
                                Logger.Error("Unable to craete UserDevice record for new Session ID: " + lMasterLogin.SessionID);
                                lResponse = null;
                            }
                        }
                        else
                        {
                            Logger.Error("Unable to craete MasterLogin record");
                            lResponse = null;
                        }
                        #endregion
                    }
                }
                else
                {
                    Logger.Error("Unable to find user record in  AspUser");
                    lResponse = null;
                }
                return lResponse;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                lResponse = null;
                //lResponse.SessionID = new WorkersInMotionDB().Database.Connection.ConnectionString + " Exception: " + ex.Message;
                return lResponse;
            }

        }
        private int CreateMasterLogin(MasterLogin pMasterLogin)
        {
            int lResult;
            IUserRepository _IUserRepository = new UserRepository(new WorkersInMotionDB());
            MasterLogin lMasterLogin = new MasterLogin();
            lMasterLogin.LoginGUID = Guid.NewGuid();
            lMasterLogin.LoginType = pMasterLogin.LoginType;
            lMasterLogin.UserGUID = pMasterLogin.UserGUID;
            lMasterLogin.IsActive = true;
            lMasterLogin.SessionID = Guid.NewGuid().ToString();
            lMasterLogin.ExpiryTime = pMasterLogin.ExpiryTime;
            lMasterLogin.SessionTimeOut = 60;
            lMasterLogin.IsLoggedIn = true;
            lMasterLogin.Phone = "";
            lMasterLogin.CreateDate = DateTime.UtcNow;
            lMasterLogin.CreateBy = pMasterLogin.UserGUID;
            lMasterLogin.LastModifiedDate = DateTime.UtcNow;
            lMasterLogin.LastModifiedBy = pMasterLogin.UserGUID;
            //_IUserRepository.InsertMasterLogin(lMasterLogin);
            if (_IUserRepository.InsertMasterLogin(lMasterLogin) > 0)
            {
                pMasterLogin.SessionID = lMasterLogin.SessionID;
                lResult = 1;
            }
            else
            {
                lResult = 0;

            }
            return lResult;
        }
        private int CreateMasterLogin(MasterLogin pMasterLogin, LoginRequest pLoginRequest)
        {
            int lResult;
            IUserRepository _IUserRepository = new UserRepository(new WorkersInMotionDB());
            UserDevice lUserDevice = new UserDevice();
            lUserDevice.UserDevicesGUID = Guid.NewGuid();
            lUserDevice.LoginGUID = pMasterLogin.LoginGUID;
            lUserDevice.UserGUID = pMasterLogin.UserGUID;
            lUserDevice.IPAddress = pLoginRequest.DeviceInfo.deviceipaddress;
            lUserDevice.DeviceID = pLoginRequest.DeviceInfo.deviceid;
            lUserDevice.DeviceInfo = new JavaScriptSerializer().Serialize(pLoginRequest.DeviceInfo);
            lUserDevice.DeviceType = pLoginRequest.DeviceInfo.devicetype;
            lUserDevice.PUSHID = pLoginRequest.PushID;
            lUserDevice.Phone = pMasterLogin.Phone;
            lUserDevice.IsActive = true;
            lUserDevice.TimeZone = pLoginRequest.DeviceInfo.TimeZone;
            lUserDevice.CreateDate = DateTime.UtcNow;
            lUserDevice.CreateBy = pMasterLogin.UserGUID;
            lUserDevice.LastModifiedDate = DateTime.UtcNow;
            lUserDevice.LastModifiedBy = pMasterLogin.UserGUID;
            //_IUserRepository.InsertUserDevice(lUserDevice);
            if (_IUserRepository.InsertUserDevice(lUserDevice) > 0)
            {
                lResult = 1;
            }
            else
            {
                lResult = 0;

            }
            return lResult;
        }
        private int CreateUserDevice(MasterLogin pMasterLogin, LoginRequest pLoginRequest)
        {
            int lResult;
            IUserRepository _IUserRepository = new UserRepository(new WorkersInMotionDB());
            UserDevice lUserDevice = new UserDevice();
            lUserDevice.UserDevicesGUID = Guid.NewGuid();
            lUserDevice.LoginGUID = pMasterLogin.LoginGUID;
            lUserDevice.UserGUID = pMasterLogin.UserGUID;
            lUserDevice.IPAddress = pLoginRequest.DeviceInfo.deviceipaddress;
            lUserDevice.DeviceID = pLoginRequest.DeviceInfo.deviceid;
            lUserDevice.DeviceInfo = new JavaScriptSerializer().Serialize(pLoginRequest.DeviceInfo);
            lUserDevice.DeviceType = pLoginRequest.DeviceInfo.devicetype;
            lUserDevice.PUSHID = pLoginRequest.PushID;
            lUserDevice.Phone = pMasterLogin.Phone;
            lUserDevice.IsActive = true;
            lUserDevice.TimeZone = pLoginRequest.DeviceInfo.TimeZone;
            lUserDevice.CreateDate = DateTime.UtcNow;
            lUserDevice.CreateBy = pMasterLogin.UserGUID;
            lUserDevice.LastModifiedDate = DateTime.UtcNow;
            lUserDevice.LastModifiedBy = pMasterLogin.UserGUID;
            // _IUserRepository.InsertUserDevice(lUserDevice);
            if (_IUserRepository.InsertUserDevice(lUserDevice) > 0)
            {
                lResult = 1;
            }
            else
            {
                lResult = 0;

            }
            return lResult;
        }
        public CreateAccountResponse CreateAccount(LoginRequest pLoginRequest)
        {
            CreateAccountResponse CreateAccountResponse = new CreateAccountResponse();
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            IGlobalUserRepository _IGlobalUserRepository;
            _IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            IUserProfileRepository _IUserProfileRepository;
            _IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            int lRetVal = _IUserRepository.CheckUserName(pLoginRequest.UserName);
            if (lRetVal == 0)
            {
                GlobalUser _globalUser = new GlobalUser();
                _globalUser.UserGUID = Guid.NewGuid();
                _globalUser.UserName = pLoginRequest.UserName;
                _globalUser.Password = pLoginRequest.Password;
                _globalUser.Role_Id = _IGlobalUserRepository.GetRoleID("IND_C");
                _globalUser.IsActive = true;
                _globalUser.IsDelete = false;
                _globalUser.CreateDate = DateTime.UtcNow;
                _globalUser.CreateBy = _globalUser.UserGUID;
                _globalUser.LastModifiedDate = DateTime.UtcNow;
                _globalUser.LastModifiedBy = _globalUser.UserGUID;
                int result = _IGlobalUserRepository.InsertGlobalUser(_globalUser);
                //int result = _IGlobalUserRepository.Save();
                if (result > 0)
                {
                    UserProfile _userProfile = new UserProfile();
                    _userProfile.ProfileGUID = Guid.NewGuid();
                    _userProfile.UserGUID = _globalUser.UserGUID;
                    _userProfile.FirstName = _globalUser.UserName;
                    _userProfile.LastModifiedDate = DateTime.UtcNow;
                    _userProfile.LastModifiedBy = _globalUser.UserGUID;
                    int resprofileInsert = _IUserProfileRepository.InsertUserProfile(_userProfile);
                    //int resprofileInsert = _IUserProfileRepository.Save();
                    if (resprofileInsert > 0)
                    {
                        CreateAccountResponse.Role = 2;
                        CreateAccountResponse.UserGUID = _userProfile.UserGUID.ToString();
                    }
                    else
                    {
                        _IGlobalUserRepository.DeleteGlobalUser(_globalUser.UserGUID);
                        //_IGlobalUserRepository.Save();
                        CreateAccountResponse = null;
                    }
                }
                else
                {
                    CreateAccountResponse = null;
                }
                return CreateAccountResponse;
            }
            else
            {
                return null;
            }
        }

        public int ForgotPassword(ForgotPasswordRequest pForgotPasswordRequest)
        {
            IUserProfileRepository _IUserProfileRepository;
            _IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            IGlobalUserRepository _IGlobalUserRepository;
            _IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            string UserGUID = _IUserProfileRepository.GetUserIDFromEmail(pForgotPasswordRequest.Cred);
            GlobalUser globalUser = _IGlobalUserRepository.GetPasswordFromUserGUID(new Guid(UserGUID));
            if (globalUser != null)
            {
                EmailManager();
                //string url = AppDomain.CurrentDomain.BaseDirectory;
                //TextReader textreader = new StreamReader(url + "EmailTemplate.html");
                //string content = textreader.ReadToEnd();
                //content = content.Replace("$UserName$", globalUser.UserName);
                //content = content.Replace("$Password$", globalUser.Password);

                StringBuilder sbMailBody = new StringBuilder();
                sbMailBody.Append("<html>");
                sbMailBody.Append("<head></head>");
                sbMailBody.Append("<body>");
                sbMailBody.Append("<table cellspacing=\"2\" cellpadding=\"2\" border=\"0\" width=\"100%\">");
                sbMailBody.Append("<tr>");
                sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                sbMailBody.Append("Dear " + globalUser.UserName + ",");
                sbMailBody.Append("</td>");
                sbMailBody.Append("</tr>");
                sbMailBody.Append("<tr>");
                sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                sbMailBody.Append("</td>");
                sbMailBody.Append("</tr>");
                sbMailBody.Append("<tr>");
                sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                sbMailBody.Append("<b>User Name :</b>" + globalUser.UserName + "");
                sbMailBody.Append("</td>");
                sbMailBody.Append("</tr>");

                sbMailBody.Append("<tr>");
                sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                sbMailBody.Append("<b>Password :</b>" + globalUser.Password + "");
                sbMailBody.Append("</td>");
                sbMailBody.Append("</tr>");
                sbMailBody.Append("<tr>");
                sbMailBody.Append("<tr>");
                sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                sbMailBody.Append("</td>");
                sbMailBody.Append("</tr>");
                sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                sbMailBody.Append("on WorkersInMotion Website");
                sbMailBody.Append("</td>");
                sbMailBody.Append("</tr>");

                sbMailBody.Append("</table>");
                sbMailBody.Append("</body>");

                sbMailBody.Append("</html>");

                sftMail lMail = new sftMail(pForgotPasswordRequest.Cred, m_cSMTPUserName);
                lMail.FromDisplayName = string.Empty;
                lMail.FromAddress = m_cSMTPUserName;
                lMail.ToDisplayName = string.Empty;
                lMail.IsMailBodyHTML = true;
                lMail.MailSubject = "Password Recovery";
                lMail.MailBody = sbMailBody.ToString();
                lMail.SmtpHost = m_cSMTPHost;
                if (!string.IsNullOrEmpty(m_cPortNo))
                {
                    lMail.PortNo = Convert.ToInt32(m_cPortNo);
                }
                else
                {
                    lMail.PortNo = 25;
                }
                if (lMail.SendMail())
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 401;
            }

        }

        public void EmailManager()
        {
            Logger.Debug("Inside EmailManager");
            try
            {
                FILE_PATH = Environment.CurrentDirectory;
                m_cCCEmailId = ConfigurationManager.AppSettings.Get("ccEmailId");
                m_cFromEmailId = ConfigurationManager.AppSettings.Get("fromEmailId");
                m_cSMTPHost = ConfigurationManager.AppSettings.Get("smtpAddress");
                m_cPortNo = ConfigurationManager.AppSettings["MailPort"];
                m_cSMTPUserName = ConfigurationManager.AppSettings["SMTPUserName"];
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        public GetRouteUserResponse GetRouteUsers(GetRouteUserRequest GetRouteUserRequest)
        {
            GetRouteUserResponse lresponse = new GetRouteUserResponse();
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            //lresponse = _IJobRepository.GetRouteUsers(GetRouteUserRequest);
            //return lresponse;
            try
            {
                GetRouteUserResponse lResponse = new GetRouteUserResponse();

                SqlParameter[] Param = new SqlParameter[12];
                Param[11] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[11].Value = DBNull.Value;
                Param[0] = new SqlParameter("@pDate", SqlDbType.Date);
                Param[0].Value = DateTime.UtcNow;
                Param[1] = new SqlParameter("@pStartTime", SqlDbType.Time);
                Param[1].Value = DateTime.UtcNow.TimeOfDay;
                Param[2] = new SqlParameter("@pEndTime", SqlDbType.Time);
                Param[2].Value = DateTime.UtcNow.TimeOfDay;
                Param[3] = new SqlParameter("@pDuration", SqlDbType.Float);
                Param[3].Value = 230;
                Param[4] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[4].Value = DBNull.Value;
                Param[5] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
                Param[5].Value = DBNull.Value;
                Param[6] = new SqlParameter("@pLatitude", SqlDbType.Float);
                Param[6].Value = 0;
                Param[7] = new SqlParameter("@pLongitude", SqlDbType.Float);
                Param[7].Value = 0;
                Param[8] = new SqlParameter("@pSearchWithinZoneArea", SqlDbType.Bit);
                Param[8].Value = 0;
                Param[9] = new SqlParameter("@pSearch", SqlDbType.Bit);
                Param[9].Value = 0;
                Param[10] = new SqlParameter("@pErrorCode", SqlDbType.NVarChar, 200);
                Param[10].Value = "";
                Param[10].Direction = ParameterDirection.Output;

                // var result1 = contextWIM.Database.SqlQuery<dynamic>("p_GetUsersLocationForADate @pOrganizationGUID,@pDate,@pStartTime, @pEndTime, @pDuration,@pRegionGUID,@pTerritoryGUID,@pLatitude,@pLongitude,@pSearchWithinZoneArea,@pSearch,@pErrorCode", Param).ToList();

                SqlParameter[] sqlParam = new SqlParameter[12];
                sqlParam[0] = new SqlParameter("@pSkill", SqlDbType.NVarChar);
                sqlParam[0].Value = "";
                sqlParam[1] = new SqlParameter("@pDate", SqlDbType.Date);
                sqlParam[1].Value = DateTime.UtcNow;
                sqlParam[2] = new SqlParameter("@pStartTime", SqlDbType.Time);
                sqlParam[2].Value = DateTime.UtcNow.TimeOfDay;
                sqlParam[3] = new SqlParameter("@pEndTime", SqlDbType.Time);
                sqlParam[3].Value = DateTime.UtcNow.TimeOfDay;
                sqlParam[4] = new SqlParameter("@pDuration", SqlDbType.Float);
                sqlParam[4].Value = 230;
                sqlParam[5] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                sqlParam[5].Value = DBNull.Value;
                sqlParam[6] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
                sqlParam[6].Value = DBNull.Value;
                sqlParam[7] = new SqlParameter("@pLatitude", SqlDbType.Float);
                sqlParam[7].Value = 0;
                sqlParam[8] = new SqlParameter("@pLongitude", SqlDbType.Float);
                sqlParam[8].Value = 0;
                sqlParam[9] = new SqlParameter("@pSearchWithinZoneArea", SqlDbType.Bit);
                sqlParam[9].Value = 0;
                sqlParam[10] = new SqlParameter("@pSearch", SqlDbType.Bit);
                sqlParam[10].Value = 0;
                sqlParam[11] = new SqlParameter("@pErrorCode", SqlDbType.NVarChar, 200);
                sqlParam[11].Value = "";
                sqlParam[11].Direction = ParameterDirection.Output;

                // var result = contextWIM.Database.SqlQuery<dynamic>("p_GetUsersAvailability @pSkill,@pDate,@pStartTime, @pEndTime, @pDuration,@pRegionGUID,@pTerritoryGUID,@pLatitude,@pLongitude,@pSearchWithinZoneArea,@pSearch,@pErrorCode", sqlParam).ToList();
                lresponse = _IJobRepository.GetRouteUsers(sqlParam, Param);
                return lResponse;
            }
            catch (Exception exception)
            {
                return null;
            }
        }


        public int PostHeartBeat(HeartBeatRequest HeartBeatRequest, Guid UserGUID)
        {
            try
            {
                int result = 0;
                IUserRepository _IUserRepository;
                _IUserRepository = new UserRepository(new WorkersInMotionDB());
                UserHeartBeat _userHeartBeat = new UserHeartBeat();
                _userHeartBeat.HeartBeatGUID = Guid.NewGuid();
                _userHeartBeat.UserGUID = UserGUID;
                _userHeartBeat.Latitude = HeartBeatRequest.latitude;
                _userHeartBeat.Longitude = HeartBeatRequest.longitude;
                _userHeartBeat.HeartBeatTime = HeartBeatRequest.time;
                _userHeartBeat.CreateDate = DateTime.UtcNow;
                _userHeartBeat.CreateBy = UserGUID;
                result = _IUserRepository.InsertUserHeartBeat(_userHeartBeat);
                //result = _IUserRepository.Save();
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public LocationResponse GetUserLocation(Guid WorkerID)
        {
            LocationResponse lresponse = new LocationResponse();
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            UserHeartBeat UserHeartBeatList = _IUserRepository.GetUserLocation(WorkerID);

            if (UserHeartBeatList != null)
            {
                lresponse.Latitude = Convert.ToDouble(UserHeartBeatList.Latitude);
                lresponse.Longitude = Convert.ToDouble(UserHeartBeatList.Longitude);
                lresponse.Time = convertdate(Convert.ToDateTime(UserHeartBeatList.HeartBeatTime));// Convert.ToDateTime(item.HeartBeatTime);
                lresponse.UserGUID = UserHeartBeatList.UserGUID;

            }
            return lresponse;
        }
        public MobilePlace ConvertPlaceforMobile(Place Place)
        {
            if (Place != null)
            {
                MobilePlace _place = new MobilePlace();
                _place.PlaceGUID = Place.PlaceGUID;
                _place.PlaceID = Place.PlaceID;
                _place.UserGUID = Place.UserGUID;
                _place.OrganizationGUID = Place.OrganizationGUID;
                _place.PlaceName = Place.PlaceName;
                _place.FirstName = Place.FirstName;
                _place.LastName = Place.LastName;
                _place.MobilePhone = Place.MobilePhone;
                _place.PlacePhone = Place.PlacePhone;
                _place.HomePhone = Place.HomePhone;
                _place.Emails = Place.Emails;
                _place.TimeZone = Place.TimeZone;
                _place.AddressLine1 = Place.AddressLine1;
                _place.AddressLine2 = Place.AddressLine2;
                _place.City = Place.City;
                _place.State = Place.State;
                _place.Country = Place.Country;
                _place.ZipCode = Place.ZipCode;
                _place.CategoryID = Place.CategoryID;
                _place.IsDeleted = Place.IsDeleted;
                _place.ImageURL = Place.ImageURL;
                _place.CreateDate = convertdate(Convert.ToDateTime(Place.CreateDate)); //Place.CreateDate;
                _place.UpdatedDate = convertdate(Convert.ToDateTime(Place.UpdatedDate));// Place.UpdatedDate;
                return _place;
            }
            else
            {
                return null;
            }
        }

        public MobilePeople ConvertPeopleForMobile(Person Person)
        {
            if (Person != null)
            {
                MobilePeople _person = new MobilePeople();
                _person.PeopleGUID = Person.PeopleGUID;
                _person.RecordStatus = Person.RecordStatus;
                _person.UserGUID = Person.UserGUID;
                _person.OrganizationGUID = Person.OrganizationGUID;
                _person.IsPrimaryContact = Person.IsPrimaryContact;
                _person.PlaceGUID = Person.PlaceGUID;
                _person.MarketGUID = Person.MarketGUID;
                _person.FirstName = Person.FirstName;
                _person.LastName = Person.LastName;
                _person.CompanyName = Person.CompanyName;
                _person.MobilePhone = Person.MobilePhone;
                _person.BusinessPhone = Person.BusinessPhone;
                _person.HomePhone = Person.HomePhone;
                _person.Emails = Person.Emails;
                _person.AddressLine1 = Person.AddressLine1;
                _person.AddressLine2 = Person.AddressLine2;
                _person.City = Person.City;
                _person.State = Person.State;
                _person.Country = Person.Country;
                _person.ZipCode = Person.ZipCode;
                _person.CategoryID = Person.CategoryID;
                _person.IsDeleted = Person.IsDeleted;
                _person.ImageURL = Person.ImageURL;
                _person.CreatedDate = convertdate(Convert.ToDateTime(Person.CreatedDate));// Person.CreatedDate;
                _person.UpdatedDate = convertdate(Convert.ToDateTime(Person.UpdatedDate));// Person.UpdatedDate;

                return _person;
            }
            else
            {
                return null;
            }

        }

        public MobileMarket ConvertMarketforMobile(Market _market)
        {
            if (_market != null)
            {
                MobileMarket _MobileMarket = new MobileMarket();
                // Market _market = context.Markets.Find(MarketGUID);
                _MobileMarket.MarketGUID = _market.MarketGUID;
                _MobileMarket.RecordStatus = _market.RecordStatus;
                _MobileMarket.IsDefault = _market.IsDefault;
                _MobileMarket.Version = _market.Version;
                _MobileMarket.UserGUID = _market.UserGUID;
                _MobileMarket.EntityType = _market.EntityType;
                _MobileMarket.OrganizationGUID = _market.OrganizationGUID;
                _MobileMarket.OwnerGUID = _market.OwnerGUID;
                _MobileMarket.MarketName = _market.MarketName;
                _MobileMarket.RegionGUID = _market.RegionGUID;
                _MobileMarket.TerritoryGUID = _market.TerritoryGUID;
                _MobileMarket.PrimaryContactGUID = _market.PrimaryContactGUID;
                _MobileMarket.FirstName = _market.FirstName;
                _MobileMarket.LastName = _market.LastName;
                _MobileMarket.MobilePhone = _market.MobilePhone;
                _MobileMarket.MarketPhone = _market.MarketPhone;
                _MobileMarket.HomePhone = _market.HomePhone;
                _MobileMarket.Emails = _market.Emails;
                _MobileMarket.TimeZone = _market.TimeZone;
                _MobileMarket.AddressLine1 = _market.AddressLine1;
                _MobileMarket.AddressLine2 = _market.AddressLine2;
                _MobileMarket.City = _market.City;
                _MobileMarket.State = _market.State;
                _MobileMarket.Country = _market.Country;
                _MobileMarket.ZipCode = _market.ZipCode;
                _MobileMarket.Latitude = _market.Latitude;
                _MobileMarket.Longitude = _market.Longitude;
                _MobileMarket.ImageURL = _market.ImageURL;
                _MobileMarket.CreateDate = convertdate(Convert.ToDateTime(_market.CreateDate));// _market.CreateDate;
                _MobileMarket.UpdatedDate = convertdate(Convert.ToDateTime(_market.UpdatedDate));// _market.UpdatedDate;
                _MobileMarket.IsDeleted = _market.IsDeleted;

                return _MobileMarket;
            }
            else
            {
                return null;
            }
        }
        public UMResponse GetUsers(string SessionID)
        {
            //IGlobalUserRepository _IGlobalUserRepository;
            //_IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            //IUserProfileRepository _IUserProfileRepository;
            //_IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            UMResponse _umResponse = new UMResponse();
            //_umResponse.GlobalUser = new List<MGlobalUser>();
            //_umResponse.UserProfile = new List<MUserProfile>();
            //GetUsers _getUsers = new GetUsers();
            //if (!string.IsNullOrEmpty(SessionID))
            //{
            //    _getUsers = _IGlobalUserRepository.GetClientUsers(SessionID);
            //}
            //if (_getUsers.GlobalUser.Count > 0)
            //{
            //    foreach (GlobalUser item in _getUsers.GlobalUser)
            //    {
            //        //MGlobalUser _mglobalUser = new MGlobalUser();
            //        //_mglobalUser.UserGUID = item.UserGUID;
            //        //_mglobalUser.UserType = item.UserType;
            //        //_mglobalUser.UserName = item.UserName;
            //        //_mglobalUser.Password = item.Password;
            //        //_mglobalUser.OrganizationGUID = item.OrganizationGUID;
            //        //_mglobalUser.Role_Id = item.Role_Id;
            //        //_mglobalUser.IsActive = item.IsActive;
            //        //_mglobalUser.CreatedDate = item.CreatedDate;
            //        //_mglobalUser.ApplicationURL = item.ApplicationURL;
            //        //_mglobalUser.ReportingPlaceType = item.ReportingPlaceType != null ? Convert.ToInt32(item.ReportingPlaceType) : 0;
            //        //_mglobalUser.ReportPlaceGUID = item.ReportPlaceGUID != null ? new Guid(item.ReportPlaceGUID.ToString()) : Guid.Empty;
            //        //_mglobalUser.IsExempt = item.IsExempt;
            //        //_mglobalUser.RegionGUID = new Guid(item.RegionGUID.ToString());
            //        //_mglobalUser.TerritoryGUID = new Guid(item.TerritoryGUID.ToString());
            //        //_mglobalUser.GroupGUID = new Guid(item.GroupGUID.ToString());
            //        //_mglobalUser.IsDelete = item.IsDelete != null ? Convert.ToBoolean(item.IsDelete) : false;
            //        // _umResponse.GlobalUser.Add(_mglobalUser);
            //    }

            //}
            //if (_getUsers.GlobalUser.Count > 0)
            //{
            //    foreach (UserProfile item in _getUsers.UserProfile)
            //    {
            //        //MUserProfile _muserprofile = new MUserProfile();
            //        //_muserprofile.UserGUID = item.UserGUID;
            //        //_muserprofile.ProfileGUID = item.ProfileGUID;
            //        //_muserprofile.OrganizationGUID = item.OrganizationGUID;
            //        //_muserprofile.FirstName = item.FirstName;
            //        //_muserprofile.LastName = item.LastName;
            //        //_muserprofile.CompanyName = item.CompanyName;
            //        //_muserprofile.MobilePhone = item.MobilePhone;
            //        //_muserprofile.BusinessPhone = item.BusinessPhone;
            //        //_muserprofile.HomePhone = item.HomePhone;
            //        //_muserprofile.Email = item.Email;
            //        //_muserprofile.AddressLine1 = item.AddressLine1;
            //        //_muserprofile.AddressLine2 = item.AddressLine2;
            //        //_muserprofile.City = item.City;
            //        //_muserprofile.State = item.State;
            //        //_muserprofile.Country = item.Country;
            //        //_muserprofile.ZipCode = item.ZipCode;
            //        //_muserprofile.IsDeleted = item.IsDeleted;
            //        //_muserprofile.CreatedDate = item.CreatedDate;
            //        //_muserprofile.PicFilename = item.PicFilename;
            //        //_umResponse.UserProfile.Add(_muserprofile);
            //    }

            //}

            return _umResponse;
        }
        public Guid GetOrganizationGUIDBySessionID(string SessionID)
        {
            IUserRepository lUserRepository = new UserRepository(new WorkersInMotionDB());
            return lUserRepository.GetOrganizationGUID(SessionID);
        }
        public UMResponseOrganization GetOrganization(string SessionID)
        {
            IOrganizationRepository _IOrganizationRepository;
            _IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            UMResponseOrganization _umResponse = new UMResponseOrganization();
            //_umResponse.Organization = new MOrganization();
            //if (!string.IsNullOrEmpty(SessionID))
            //{
            //    string OrganizationGUID = _IOrganizationRepository.GetOrganizationID(SessionID);
            //    if (!string.IsNullOrEmpty(OrganizationGUID))
            //    {
            //        Organization org = _IOrganizationRepository.GetOrganizationByID(new Guid(OrganizationGUID));
            //        if (org != null)
            //        {
            //            _umResponse.Organization.OrganizationGUID = org.OrganizationGUID;
            //            _umResponse.Organization.OrganizationName = org.OrganizationName;
            //            _umResponse.Organization.OrganizationFullName = org.OrganizationFullName;
            //            _umResponse.Organization.Website = org.Website;
            //            _umResponse.Organization.Phone = org.Phone;
            //            //  _umResponse.Organization.TimeZone = org.TimeZone;
            //            _umResponse.Organization.AddressLine1 = org.AddressLine1;
            //            _umResponse.Organization.AddressLine2 = org.AddressLine2;
            //            _umResponse.Organization.City = org.City;
            //            _umResponse.Organization.State = org.State;
            //            _umResponse.Organization.Country = org.Country;
            //            _umResponse.Organization.ZipCode = org.ZipCode;
            //            _umResponse.Organization.EmailID = org.EmailID;
            //            // _umResponse.Organization.ApplicationURL = org.ApplicationURL;
            //            _umResponse.Organization.IsActive = org.IsActive;
            //            _umResponse.Organization.IsDeleted = org.IsDeleted;
            //            //_umResponse.Organization.CreatedDate = org.CreatedDate;

            //        }
            //    }
            //}
            return _umResponse;
        }

        public TerritoryRegion GetTerritoryRegion(Guid UserGUID)
        {
            TerritoryRegion lTerritoryRegion = new TerritoryRegion();
            ITerritoryRepository _ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            IRegionRepository _IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            IOrganizationRepository _IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            Guid OrganizationGUID = _IOrganizationRepository.GetOrganizationIDByUserGUID(UserGUID);


            List<Territory> TerritoryList = _ITerritoryRepository.GetTerritoryByOrganizationGUID(OrganizationGUID).ToList();
            if (TerritoryList != null && TerritoryList.Count > 0)
            {
                lTerritoryRegion.Territories = new List<MobileTerritory>();
                foreach (Territory item in TerritoryList)
                {
                    MobileTerritory lterritory = new MobileTerritory();
                    lterritory = convertTerritoryToMobileTerritory(item);
                    lTerritoryRegion.Territories.Add(lterritory);
                }
            }
            List<Region> RegionList = _IRegionRepository.GetRegionByOrganizationGUID(OrganizationGUID).ToList();
            if (RegionList != null && RegionList.Count > 0)
            {
                lTerritoryRegion.Regions = new List<MobileRegion>();
                foreach (Region item in RegionList)
                {
                    MobileRegion lregion = convertRegionToMobileRegion(item);
                    lTerritoryRegion.Regions.Add(lregion);
                }
            }
            return lTerritoryRegion;
        }

        private MobileRegion convertRegionToMobileRegion(Region item)
        {
            MobileRegion lregion = new MobileRegion();
            lregion.RegionGUID = item.RegionGUID;
            lregion.OrganizationGUID = item.OrganizationGUID;
            lregion.Name = item.Name;
            lregion.IsDefault = item.IsDefault;
            lregion.Description = item.Description;
            return lregion;
        }

        private MobileTerritory convertTerritoryToMobileTerritory(Territory item)
        {
            MobileTerritory lterritory = new MobileTerritory();
            lterritory.TerritoryGUID = item.TerritoryGUID;
            lterritory.OrganizationGUID = item.OrganizationGUID;
            lterritory.Name = item.Name;
            lterritory.IsDefault = item.IsDefault;
            lterritory.Description = item.Description;
            return lterritory;
        }

        public DownloadUsers DownloadUsers(string SessionID, Nullable<System.Guid> UserGUID = null)
        {
            DownloadUsers lresponse = new DownloadUsers();
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());

            lresponse.UserRecords = _IUserRepository.DownloadUsers(SessionID, UserGUID);
            return lresponse;
        }

        //public DownloadUsers GetOrganization(string SessionID)
        //{
        //    IOrganizationRepository _IOrganizationRepository;
        //    _IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
        //    DownloadUsers lDownloadUsers = new DownloadUsers();

        //    SqlParameter[] sqlParam = new SqlParameter[2];
        //    sqlParam[0] = new SqlParameter("@pSessionID", SqlDbType.NVarChar);
        //    sqlParam[0].Value = SessionID;
        //    sqlParam[1] = new SqlParameter("@pErrorCode", SqlDbType.NVarChar, 200);
        //    sqlParam[1].Value = "";
        //    sqlParam[1].Direction = ParameterDirection.Output;
        //    _jmResponse.JobList = context.Database.SqlQuery<p_GetJobs_Result>("dbo.p_getjobs  @pSessionID,@pErrorCode=@pErrorCode output", sqlParam).ToList();
        //    return lDownloadUsers;
        //}



        public List<MCustomers> GetCustomers(Guid lOrganizationGUID)
        {
            DownloadUsers lresponse = new DownloadUsers();
            IPlaceRepository _IPlaceRepository;
            _IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            IEnumerable<Place> lPlaces = _IPlaceRepository.GetPlaceByOrganizationGUID(lOrganizationGUID);
            return ConvertToMobileCustomer(lPlaces); ;
        }

        private List<MCustomers> ConvertToMobileCustomer(IEnumerable<Place> lPlaces)
        {
            List<MCustomers> lCustomers = new List<MCustomers>();
            foreach (Place lPlace in lPlaces)
            {
                MCustomers lMCustomers = new MCustomers();
                lMCustomers.PlaceGUID = lPlace.PlaceGUID;
                lMCustomers.PlaceID = lPlace.PlaceID;
                lMCustomers.PlaceName = lPlace.PlaceName;
                lCustomers.Add(lMCustomers);
            }
            return lCustomers;
        }
    }
}
