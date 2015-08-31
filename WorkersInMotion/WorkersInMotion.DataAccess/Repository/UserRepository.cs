using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;


using System.Data;
using System.Data.SqlClient;

using System.Text;
using WorkersInMotion.Log;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.ObjectModel;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.DataAccess.Repository
{
    public class UserRepository : IUserRepository, IDisposable
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
        private WorkersInMotionDB context;

        //public UserRepository()
        //{
        //    WorkersInMotionContext context = new WorkersInMotionContext();
        //    this.context = context;
        //}
        public UserRepository(WorkersInMotionDB context)
        {
            this.context = context;
            _ILogservice = new Log4NetService(GetType());

        }

        public IEnumerable<AspNetUser> GetUsers()
        {
            //var appUsers = context.AspNetUsers.ToList();
            //return context.AspNetUsers.ToList().OrderBy(x => x.UserName);

            return context.Database.SqlQuery<AspNetUser>("select * from AspNetUsers order by UserName").ToList();
        }

        public AspNetRole GetRole(string RoleID)
        {
            //return context.AspNetRoles.Find(RoleID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRoleID", SqlDbType.NVarChar, 128);
            Param[0].Value = RoleID;
            return context.Database.SqlQuery<AspNetRole>("select * from  AspNetRoles where Id=@pRoleID", Param).FirstOrDefault();
        }
        public AspNetRole GetRolebyUserType(string UserType)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.AspNetRoles
            //            where p.UserType == UserType
            //            select p).FirstOrDefault();

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserType", SqlDbType.NVarChar, 10);
            Param[0].Value = UserType;
            return context.Database.SqlQuery<AspNetRole>("select * from  AspNetRoles where UserType=@pUserType", Param).FirstOrDefault();
        }
        public AspNetUser GetUserByID(string userID)
        {
            //return context.AspNetUsers.Find(userID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@puserID", SqlDbType.NVarChar, 128);
            Param[0].Value = userID;
            return context.Database.SqlQuery<AspNetUser>("select * from  AspNetUsers where Id=@puserID", Param).FirstOrDefault();
        }

        public int InsertUser(AspNetUser user)
        {
            //context.AspNetUsers.Add(user);
            SqlParameter[] Param = new SqlParameter[10];
            Param[0] = new SqlParameter("@pId", SqlDbType.NVarChar, 128);
            Param[0].Value = user.Id;
            Param[1] = new SqlParameter("@pUserName", SqlDbType.NVarChar, -1);
            Param[1].Value = (object)user.UserName ?? DBNull.Value;
            Param[2] = new SqlParameter("@pPasswordHash", SqlDbType.NVarChar, -1);
            Param[2].Value = (object)user.PasswordHash ?? DBNull.Value;
            Param[3] = new SqlParameter("@pSecurityStamp", SqlDbType.NVarChar, -1);
            Param[3].Value = (object)user.SecurityStamp ?? DBNull.Value;
            Param[4] = new SqlParameter("@pDiscriminator", SqlDbType.NVarChar, 128);
            Param[4].Value = user.Discriminator;
            Param[5] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, 128);
            Param[5].Value = (object)user.FirstName ?? DBNull.Value;
            Param[6] = new SqlParameter("@pLastName", SqlDbType.NVarChar, 128);
            Param[6].Value = (object)user.LastName ?? DBNull.Value;
            Param[7] = new SqlParameter("@pEmailID", SqlDbType.NVarChar, 128);
            Param[7].Value = (object)user.EmailID ?? DBNull.Value;
            Param[8] = new SqlParameter("@pPhoneNumber", SqlDbType.NVarChar, 128);
            Param[8].Value = (object)user.PhoneNumber ?? DBNull.Value;
            Param[9] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[9].Value = (object)user.OrganizationGUID ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into AspNetUsers(Id,UserName,PasswordHash,SecurityStamp,Discriminator,FirstName,LastName,EmailID,PhoneNumber,OrganizationGUID)values(@pId,@pUserName,@pPasswordHash,@pSecurityStamp,@pDiscriminator,@pFirstName,@pLastName,@pEmailID,@pPhoneNumber,@pOrganizationGUID)", Param);
        }

        public int DeleteUser(string userID)
        {
            //AspNetUser user = context.AspNetUsers.Find(userID);
            //if (user != null)
            //    context.AspNetUsers.Remove(user);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@puserID", SqlDbType.NVarChar, 128);
            Param[0].Value = userID;
            return context.Database.ExecuteSqlCommand("delete from  AspNetUsers where Id=@puserID", Param);
        }

        public int DeleteByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Userlist = (from p in dataContext.AspNetUsers
            //                    where p.OrganizationGUID == OrganizationGUID
            //                    select p).ToList();
            //    foreach (var item in Userlist)
            //    {
            //        dataContext.AspNetUsers.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from  AspNetUsers where OrganizationGUID=@pOrganizationGUID", Param);
        }

        public int UpdateUser(AspNetUser user)
        {
            //context.Entry(user).State = EntityState.Modified;
            SqlParameter[] Param = new SqlParameter[10];
            Param[0] = new SqlParameter("@pId", SqlDbType.NVarChar, 128);
            Param[0].Value = user.Id;
            Param[1] = new SqlParameter("@pUserName", SqlDbType.NVarChar, -1);
            Param[1].Value = (object)user.UserName ?? DBNull.Value;
            Param[2] = new SqlParameter("@pPasswordHash", SqlDbType.NVarChar, -1);
            Param[2].Value = (object)user.PasswordHash ?? DBNull.Value;
            Param[3] = new SqlParameter("@pSecurityStamp", SqlDbType.NVarChar, -1);
            Param[3].Value = (object)user.SecurityStamp ?? DBNull.Value;
            Param[4] = new SqlParameter("@pDiscriminator", SqlDbType.NVarChar, 128);
            Param[4].Value = user.Discriminator;
            Param[5] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, 128);
            Param[5].Value = (object)user.FirstName ?? DBNull.Value;
            Param[6] = new SqlParameter("@pLastName", SqlDbType.NVarChar, 128);
            Param[6].Value = (object)user.LastName ?? DBNull.Value;
            Param[7] = new SqlParameter("@pEmailID", SqlDbType.NVarChar, 128);
            Param[7].Value = (object)user.EmailID ?? DBNull.Value;
            Param[8] = new SqlParameter("@pPhoneNumber", SqlDbType.NVarChar, 128);
            Param[8].Value = (object)user.PhoneNumber ?? DBNull.Value;
            Param[9] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[9].Value = (object)user.OrganizationGUID ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("update AspNetUsers set UserName=@pUserName,PasswordHash=@pPasswordHash,SecurityStamp=@pSecurityStamp,Discriminator=@pDiscriminator,FirstName=@pFirstName,LastName=@pLastName,EmailID=@pEmailID,PhoneNumber=@pPhoneNumber,OrganizationGUID=@pOrganizationGUID where Id=@pId)", Param);

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

        public int InsertMasterLogin(MasterLogin masterlogin)
        {
            //context.MasterLogins.Add(masterlogin);
            SqlParameter[] Param = new SqlParameter[13];
            Param[0] = new SqlParameter("@pLoginGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = masterlogin.LoginGUID;
            Param[1] = new SqlParameter("@pLoginType", SqlDbType.SmallInt);
            Param[1].Value = (object)masterlogin.LoginType ?? DBNull.Value;
            Param[2] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = masterlogin.UserGUID;
            Param[3] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[3].Value = masterlogin.IsActive;
            Param[4] = new SqlParameter("@pSessionID", SqlDbType.NVarChar, -1);
            Param[4].Value = (object)masterlogin.SessionID ?? DBNull.Value;
            Param[5] = new SqlParameter("@pExpiryTime", SqlDbType.DateTime);
            Param[5].Value = masterlogin.ExpiryTime;
            Param[6] = new SqlParameter("@pSessionTimeOut", SqlDbType.Int);
            Param[6].Value = (object)masterlogin.SessionTimeOut ?? DBNull.Value;
            Param[7] = new SqlParameter("@pIsLoggedIn", SqlDbType.Bit);
            Param[7].Value = (object)masterlogin.IsLoggedIn ?? DBNull.Value;
            Param[8] = new SqlParameter("@pPhone", SqlDbType.NVarChar, -1);
            Param[8].Value = (object)masterlogin.Phone ?? DBNull.Value;
            Param[9] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[9].Value = (object)masterlogin.CreateDate ?? DBNull.Value;
            Param[10] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[10].Value = (object)masterlogin.CreateBy ?? DBNull.Value;
            Param[11] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[11].Value = (object)masterlogin.LastModifiedDate ?? DBNull.Value;
            Param[12] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)masterlogin.LastModifiedBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into MasterLogins(LoginGUID,LoginType,UserGUID,IsActive,SessionID,ExpiryTime,SessionTimeOut,IsLoggedIn,Phone,CreateDate,CreateBy,LastModifiedDate,LastModifiedBy)"
                + "values(@pLoginGUID,@pLoginType,@pUserGUID,@pIsActive,@pSessionID,@pExpiryTime,@pSessionTimeOut,@pIsLoggedIn,@pPhone,@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy)", Param);
        }

        public int InsertUserDevice(UserDevice UserDevice)
        {
            //context.UserDevices.Add(UserDevice);
            SqlParameter[] Param = new SqlParameter[15];
            Param[0] = new SqlParameter("@pUserDevicesGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserDevice.UserDevicesGUID;
            Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)UserDevice.UserGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pLoginGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)UserDevice.LoginGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pDeviceID", SqlDbType.NVarChar, -1);
            Param[3].Value = (object)UserDevice.DeviceID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIPAddress", SqlDbType.NVarChar, -1);
            Param[4].Value = (object)UserDevice.IPAddress ?? DBNull.Value;
            Param[5] = new SqlParameter("@pDeviceInfo", SqlDbType.NVarChar, -1);
            Param[5].Value = (object)UserDevice.DeviceInfo ?? DBNull.Value;
            Param[6] = new SqlParameter("@pDeviceType", SqlDbType.SmallInt);
            Param[6].Value = (object)UserDevice.DeviceType ?? DBNull.Value;
            Param[7] = new SqlParameter("@pPUSHID", SqlDbType.NVarChar, -1);
            Param[7].Value = (object)UserDevice.PUSHID ?? DBNull.Value;
            Param[8] = new SqlParameter("@pPhone", SqlDbType.NVarChar, -1);
            Param[8].Value = (object)UserDevice.Phone ?? DBNull.Value;
            Param[9] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[9].Value = (object)UserDevice.IsActive ?? DBNull.Value;
            Param[10] = new SqlParameter("@pTimeZone", SqlDbType.Float);
            Param[10].Value = (object)UserDevice.TimeZone ?? DBNull.Value;
            Param[11] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[11].Value = (object)UserDevice.CreateDate ?? DBNull.Value;
            Param[12] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)UserDevice.CreateBy ?? DBNull.Value;
            Param[13] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[13].Value = (object)UserDevice.LastModifiedDate ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[14].Value = (object)UserDevice.LastModifiedBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into UserDevices(UserDevicesGUID,UserGUID,LoginGUID,DeviceID,IPAddress,DeviceInfo,DeviceType,PUSHID,Phone,"
                + "IsActive,TimeZone,CreateDate,CreateBy,LastModifiedDate,LastModifiedBy)"
                + "values(@pUserDevicesGUID,@pUserGUID,@pLoginGUID,@pDeviceID,@pIPAddress,@pDeviceInfo,@pDeviceType,@pPUSHID,@pPhone,"
                + "@pIsActive,@pTimeZone,@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy)", Param);
        }
        public bool ValidateUser(string SessionGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var MasterLogin = (from p in dataContext.MasterLogins
            //                       where p.SessionID == SessionGUID && p.ExpiryTime > DateTime.UtcNow
            //                       select p).FirstOrDefault();
            //    if (MasterLogin != null)
            //    {
            //        if (MasterLogin.LoginType == 1)
            //        {
            //            MasterLogin.ExpiryTime = MasterLogin.ExpiryTime.AddYears(10);
            //        }
            //        else
            //        {
            //            MasterLogin.ExpiryTime = MasterLogin.ExpiryTime.AddMinutes(30);
            //        }
            //        MasterLogin.LastModifiedDate = DateTime.UtcNow;
            //        if (dataContext.SaveChanges() > 0)
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //        return false;
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pSessionGUID", SqlDbType.NVarChar, -1);
            Param[0].Value = SessionGUID;
            var MasterLogin = context.Database.SqlQuery<MasterLogin>("select * from  MasterLogins where SessionID=@pSessionGUID", Param).FirstOrDefault();
            if (MasterLogin != null)
            {
                if (MasterLogin.LoginType == 1)
                {
                    MasterLogin.ExpiryTime = DateTime.UtcNow.AddYears(10);
                }
                else
                {
                    MasterLogin.ExpiryTime = MasterLogin.ExpiryTime.AddMinutes(30);
                }
                MasterLogin.LastModifiedDate = DateTime.UtcNow;

                SqlParameter[] ParamNew = new SqlParameter[3];
                ParamNew[0] = new SqlParameter("@pExpiryTime", SqlDbType.DateTime);
                ParamNew[0].Value = MasterLogin.ExpiryTime;
                ParamNew[1] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                ParamNew[1].Value = MasterLogin.LastModifiedDate;
                ParamNew[2] = new SqlParameter("@pLoginGUID", SqlDbType.UniqueIdentifier);
                ParamNew[2].Value = MasterLogin.LoginGUID;
                int result = context.Database.ExecuteSqlCommand("Update MasterLogins set ExpiryTime=@pExpiryTime,LastModifiedDate=@pLastModifiedDate where LoginGUID=@pLoginGUID", ParamNew);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        public Guid GetUserGUID(string SessionGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.MasterLogins
            //            where p.SessionID == SessionGUID
            //            select p).FirstOrDefault().UserGUID;


            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pSessionGUID", SqlDbType.NVarChar, -1);
            Param[0].Value = SessionGUID;
            MasterLogin masterLogin = context.Database.SqlQuery<MasterLogin>("select * from  MasterLogins where SessionID=@pSessionGUID", Param).FirstOrDefault();
            if (masterLogin != null)
                return masterLogin.UserGUID;
            else
                return Guid.Empty;
        }

        public OrganizationUsersMap GetUserByID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.OrganizationUsersMaps
            //            where p.UserGUID == UserGUID
            //            select p).FirstOrDefault();


            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.SqlQuery<OrganizationUsersMap>("select * from  OrganizationUsersMap where UserGUID=@pUserGUID", Param).FirstOrDefault();
        }
        public Guid GetOrganizationGUID(string SessionGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    string UserGUID = (from p in dataContext.MasterLogins
            //                       where p.SessionID == SessionGUID
            //                       select p).FirstOrDefault().UserGUID.ToString();

            //    if (!string.IsNullOrEmpty(UserGUID))
            //    {
            //        return (from p in dataContext.OrganizationUsersMaps
            //                where p.UserGUID == new Guid(UserGUID)
            //                select p).FirstOrDefault().OrganizationGUID;
            //    }
            //    else
            //    {
            //        return Guid.Empty;
            //    }

            //}
            Guid UserGUID = GetUserGUID(SessionGUID);
            if (UserGUID != Guid.Empty)
            {
                OrganizationUsersMap orgUserMap = GetUserByID(UserGUID);
                if (orgUserMap != null)
                    return orgUserMap.OrganizationGUID;
                else
                    return Guid.Empty;
            }
            else
            {
                return Guid.Empty;
            }
        }
        //public GlobalUser GlobalUserLogin(string userName, string password)
        //{
        //    using (var dataContext = new WorkersInMotionDB())
        //    {
        //        return (from p in dataContext.GlobalUsers
        //                where p.UserName == userName && p.Password == password
        //                select p).FirstOrDefault();
        //    }
        //}
        public GlobalUser GlobalUserLogin(string userName, string OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    GlobalUser globalUser = (from p in dataContext.GlobalUsers
            //                             where p.UserName == userName && p.IsActive == true && p.IsDelete == false
            //                             select p).FirstOrDefault();
            //    if (globalUser != null)
            //    {
            //        OrganizationUsersMap userMap = (from p in dataContext.OrganizationUsersMaps
            //                                        where p.UserGUID == globalUser.UserGUID && p.OrganizationGUID == new Guid(OrganizationGUID)
            //                                        select p).FirstOrDefault();
            //        if (userMap != null)
            //            return globalUser;
            //        else
            //            return null;
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}

            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pUserName", SqlDbType.NVarChar, -1);
            Param[0].Value = userName;
            Param[1] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[1].Value = true;
            Param[2] = new SqlParameter("@pIsDelete", SqlDbType.Bit);
            Param[2].Value = false;
            GlobalUser globalUser = context.Database.SqlQuery<GlobalUser>("select * from  GlobalUsers where UserName=@pUserName and IsActive=@pIsActive and IsDelete=@pIsDelete", Param).FirstOrDefault();
            if (globalUser != null)
            {
                SqlParameter[] ParamOrg = new SqlParameter[2];
                ParamOrg[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
                ParamOrg[0].Value = globalUser.UserGUID;
                ParamOrg[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                ParamOrg[1].Value = new Guid(OrganizationGUID);
                OrganizationUsersMap userMap = context.Database.SqlQuery<OrganizationUsersMap>("select * from  OrganizationUsersMap where UserGUID=@pUserGUID and OrganizationGUID=@pOrganizationGUID", ParamOrg).FirstOrDefault();
                if (userMap != null)
                    return globalUser;
                else
                    return null;
            }
            else
                return null;
        }
        public IEnumerable<dynamic> GetJobCount(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var joblist = (from p in dataContext.Jobs
            //                   where p.OrganizationGUID == OrganizationGUID && (p.StatusCode == 1 || p.StatusCode == 6)
            //                   select p);

            //    var groups = joblist.GroupBy(item => new { item.CreateDate.Value.Month, item.StatusCode }).Select(item => new
            //    {
            //        month = item.Key.Month,
            //        status = item.Key.StatusCode,
            //        count = item.Count()
            //    }).ToList();

            //    return groups;
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;

            var joblist = context.Database.SqlQuery<Job>("select * from  Jobs where OrganizationGUID=@pOrganizationGUID and (StatusCode=1 or StatusCode=6)", Param).ToList();
            var groups = joblist.GroupBy(item => new { item.CreateDate.Value.Month, item.StatusCode }).Select(item => new
            {
                month = item.Key.Month,
                status = item.Key.StatusCode,
                count = item.Count()
            }).ToList();

            return groups;


        }
        public GlobalUser UserLogin(string userName, string password)
        {

            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.GlobalUsers
            //            where p.UserName == userName && p.Password == password
            //            select p).FirstOrDefault();

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pUserName", SqlDbType.NVarChar, -1);
            Param[0].Value = userName;
            Param[1] = new SqlParameter("@pPassword", SqlDbType.NVarChar, -1);
            Param[1].Value = password;
            return context.Database.SqlQuery<GlobalUser>("select * from  GlobalUsers where UserName=@pUserName and Password=@pPassword", Param).FirstOrDefault();


        }
        public MasterLogin GetMasterLoginByLoginID(Guid LoginID)
        {
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pLoginGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = LoginID;

            return context.Database.SqlQuery<MasterLogin>("select * from  MasterLogins where LoginGUID=@pLoginGUID", Param).FirstOrDefault();

        }

        public MasterLogin UserLoginServer(string userName, string password)
        {
            Logger.Debug("Inside UserLogin");
            MasterLogin masterlogin = new MasterLogin();
            try
            {
                //  MasterLogin masterlogin = new MasterLogin();
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var aspuser = (from p in dataContext.GlobalUsers
                //                   where p.UserName == userName && p.Password == password
                //                   select p).FirstOrDefault();
                //    if (aspuser != null)
                //    {
                //        Logger.Debug("Inside Role");
                //        AspNetRole asprole = GetRole(aspuser.Role_Id);

                //        masterlogin = (from p in dataContext.MasterLogins
                //                       where p.UserGUID == aspuser.UserGUID
                //                       select p).FirstOrDefault();
                //        if (masterlogin != null)
                //        {
                //            Logger.Debug("Inside MasterLogin");

                //            var qry = from p in dataContext.MasterLogins where p.LoginGUID == masterlogin.LoginGUID select p;
                //            var item = qry.Single();
                //            item.SessionID = Guid.NewGuid().ToString();
                //            item.LastModifiedDate = DateTime.UtcNow;
                //            item.LastModifiedBy = aspuser.UserGUID;
                //            if (dataContext.SaveChanges() > 0)
                //            {
                //                return masterlogin;
                //            }
                //            else
                //            {
                //                return null;
                //            }
                //        }
                //        else
                //        {
                //            Logger.Debug("Inside MasterLogin Create");
                //            masterlogin = new MasterLogin();
                //            masterlogin.LoginGUID = Guid.NewGuid();
                //            masterlogin.UserGUID = aspuser.UserGUID;
                //            masterlogin.IsActive = true;
                //            masterlogin.SessionID = Guid.NewGuid().ToString();
                //            masterlogin.ExpiryTime = DateTime.UtcNow;
                //            masterlogin.SessionTimeOut = 60;
                //            masterlogin.IsLoggedIn = true;
                //            masterlogin.Phone = "";
                //            masterlogin.CreateDate = DateTime.UtcNow;
                //            masterlogin.CreateBy = aspuser.UserGUID;
                //            masterlogin.LastModifiedDate = DateTime.UtcNow;
                //            masterlogin.LastModifiedBy = aspuser.UserGUID;
                //            int result = InsertMasterLogin(masterlogin);
                //            //int result = Save();
                //            if (result > 0)
                //            {
                //                return masterlogin;
                //            }
                //            else
                //            {
                //                return null;
                //            }

                //        }
                //    }
                //    else
                //    {
                //        Logger.Debug("AspUser is null");
                //        return null;
                //    }
                //}

                using (var dataContext = new WorkersInMotionDB())
                {

                    var aspuser = UserLogin(userName, password);
                    if (aspuser != null)
                    {
                        Logger.Debug("Inside Role");
                        AspNetRole asprole = GetRole(aspuser.Role_Id);

                        SqlParameter[] Param = new SqlParameter[1];
                        Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
                        Param[0].Value = aspuser.UserGUID;

                        masterlogin = context.Database.SqlQuery<MasterLogin>("select * from  MasterLogins where UserGUID=@pUserGUID", Param).FirstOrDefault();

                        if (masterlogin != null)
                        {
                            Logger.Debug("Inside MasterLogin");

                            //var qry = from p in dataContext.MasterLogins where p.LoginGUID == masterlogin.LoginGUID select p;
                            //var item = qry.Single();
                            //item.SessionID = Guid.NewGuid().ToString();
                            //item.LastModifiedDate = DateTime.UtcNow;
                            //item.LastModifiedBy = aspuser.UserGUID;

                            masterlogin.SessionID = Guid.NewGuid().ToString();
                            masterlogin.LastModifiedDate = DateTime.UtcNow;
                            if (UpdateMasterLogins(masterlogin) > 0)
                            {
                                return masterlogin;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            Logger.Debug("Inside MasterLogin Create");
                            masterlogin = new MasterLogin();
                            masterlogin.LoginGUID = Guid.NewGuid();
                            masterlogin.UserGUID = aspuser.UserGUID;
                            masterlogin.IsActive = true;
                            masterlogin.SessionID = Guid.NewGuid().ToString();
                            masterlogin.ExpiryTime = DateTime.UtcNow;
                            masterlogin.SessionTimeOut = 60;
                            masterlogin.IsLoggedIn = true;
                            masterlogin.Phone = "";
                            masterlogin.CreateDate = DateTime.UtcNow;
                            masterlogin.CreateBy = aspuser.UserGUID;
                            masterlogin.LastModifiedDate = DateTime.UtcNow;
                            masterlogin.LastModifiedBy = aspuser.UserGUID;
                            int result = InsertMasterLogin(masterlogin);
                            //int result = Save();
                            if (result > 0)
                            {
                                return masterlogin;
                            }
                            else
                            {
                                return null;
                            }

                        }
                    }
                    else
                    {
                        Logger.Debug("AspUser is null");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        public int UpdateMasterLogins(MasterLogin masterLogin)
        {
            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pLoginGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = masterLogin.LoginGUID;
            Param[1] = new SqlParameter("@pSessionID", SqlDbType.NVarChar, -1);
            Param[1].Value = masterLogin.SessionID;
            Param[2] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[2].Value = masterLogin.LastModifiedDate;

            return context.Database.ExecuteSqlCommand("update MasterLogins set SessionID=@pSessionID,LastModifiedDate=@pLastModifiedDate where LoginGUID=@pLoginGUID", Param);

        }
        //public MasterLogin GetMasterLoginByUserGUID(Guid UserGUID)
        //{
        //    using (var dataContext = new WorkersInMotionDB())
        //    {
        //        return (from p in dataContext.MasterLogins
        //                where p.UserGUID == UserGUID
        //                select p).FirstOrDefault();

        //    }
        //}
        public List<MasterLogin> GetMasterLogin(MasterLogin pMasterLogin)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.MasterLogins
            //            where p.UserGUID == pMasterLogin.UserGUID
            //            && (pMasterLogin.LoginType == null || p.LoginType == pMasterLogin.LoginType)
            //            select p).ToList();

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = pMasterLogin.UserGUID;
            Param[1] = new SqlParameter("@pLoginType", SqlDbType.SmallInt);
            Param[1].Value = pMasterLogin.LoginType;

            return context.Database.SqlQuery<MasterLogin>("select * from  MasterLogins where UserGUID=@pUserGUID and (LoginType is null or LoginType=@pLoginType)", Param).ToList();

        }
        //public UserDevice GetUserDeviceByLoginAndDeviceID(Guid LoginGUID, string DeviceGUID)
        //{
        //    using (var dataContext = new WorkersInMotionDB())
        //    {
        //        return (from p in dataContext.UserDevices
        //                where p.LoginGUID == LoginGUID && p.DeviceID == DeviceGUID
        //                select p).FirstOrDefault();

        //    }
        //}
        public List<UserDevice> GetUserDevice(UserDevice pUserDevice)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.UserDevices
            //            where p.LoginGUID == pUserDevice.LoginGUID && p.DeviceID == pUserDevice.DeviceID
            //            select p).ToList();

            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pLoginGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = pUserDevice.LoginGUID;
            Param[1] = new SqlParameter("@pDeviceID", SqlDbType.NVarChar, -1);
            Param[1].Value = (object)pUserDevice.DeviceID ?? DBNull.Value;

            return context.Database.SqlQuery<UserDevice>("select * from  UserDevices where LoginGUID=@pLoginGUID and DeviceID=@pDeviceID", Param).ToList();
        }
        //public string UpdateMasterLoginEntries(Guid LoginGUID, Guid UserGUID)
        //{
        //    using (var dataContext = new WorkersInMotionDB())
        //    {
        //        var qry = from p in dataContext.MasterLogins where p.LoginGUID == LoginGUID select p;
        //        var item = qry.Single();
        //        item.SessionID = Guid.NewGuid().ToString();
        //        item.LastModifiedDate = DateTime.UtcNow;
        //        item.LastModifiedBy = UserGUID;
        //        if (dataContext.SaveChanges() > 0)
        //        {
        //            return item.SessionID;
        //        }
        //        else
        //        {
        //            return "";
        //        }

        //    }
        //}

        public int DeleteUserDevices(Guid userDeviceID)
        {
            //UserDevice userDevice = context.UserDevices.Find(userDeviceID);
            //if (userDevice != null)
            //    context.UserDevices.Remove(userDevice);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserDeviceID", SqlDbType.UniqueIdentifier);
            Param[0].Value = userDeviceID;

            return context.Database.ExecuteSqlCommand("delete from UserDevices where UserDevicesGUID=@pUserDeviceID", Param);
        }

        public int InsertUserHeartBeat(UserHeartBeat heartbeat)
        {
            //context.UserHeartBeats.Add(heartbeat);

            SqlParameter[] Param = new SqlParameter[7];
            Param[0] = new SqlParameter("@pHeartBeatGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = heartbeat.HeartBeatGUID;
            Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = heartbeat.UserGUID;
            Param[2] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[2].Value = (object)heartbeat.Latitude ?? DBNull.Value;
            Param[3] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[3].Value = (object)heartbeat.Longitude ?? DBNull.Value;
            Param[4] = new SqlParameter("@pHeartBeatTime", SqlDbType.DateTime);
            Param[4].Value = (object)heartbeat.HeartBeatTime ?? DBNull.Value;
            Param[5] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[5].Value = (object)heartbeat.CreateDate ?? DBNull.Value;
            Param[6] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)heartbeat.CreateBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into  UserHeartBeats(HeartBeatGUID,UserGUID,Latitude,Longitude,HeartBeatTime,CreateDate,CreateBy)"
              + "values(@pHeartBeatGUID,@pUserGUID,@pLatitude,@pLongitude,@pHeartBeatTime,@pCreateDate,@pCreateBy)", Param);

        }
        public UserHeartBeat GetUserLocation(Guid WorkerID)
        {
            try
            {

                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.UserHeartBeats
                //            where p.UserGUID == WorkerID
                //            select p).OrderByDescending(x => x.HeartBeatTime).FirstOrDefault();

                //}
                SqlParameter[] Param = new SqlParameter[1];
                Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = WorkerID;

                return context.Database.SqlQuery<UserHeartBeat>("select * from  UserHeartBeats where UserGUID=@pUserGUID", Param).FirstOrDefault();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public void UpdateMasterLogin(MasterLogin masterlogin)
        //{
        //    context.Entry(masterlogin).State = EntityState.Modified;
        //}
        public string UpdateMasterLogin(MasterLogin pMasterLogin)
        {
            try
            {

                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var qry = from p in dataContext.MasterLogins where p.LoginGUID == pMasterLogin.LoginGUID select p;
                //    var item = qry.Single();
                //    item.SessionID = Guid.NewGuid().ToString();
                //    item.LastModifiedDate = DateTime.UtcNow;
                //    item.LastModifiedBy = pMasterLogin.UserGUID;
                //    item.ExpiryTime = pMasterLogin.ExpiryTime;
                //    if (dataContext.SaveChanges() > 0)
                //    {
                //        return item.SessionID;
                //    }
                //    else
                //    {
                //        return string.Empty;
                //    }

                //}
                MasterLogin mLogin = GetMasterLoginByLoginID(pMasterLogin.LoginGUID);
                if (mLogin != null)
                {
                    mLogin.SessionID = Guid.NewGuid().ToString();
                    mLogin.LastModifiedDate = DateTime.UtcNow;
                    SqlParameter[] Param = new SqlParameter[5];
                    Param[0] = new SqlParameter("@pLoginGUID", SqlDbType.UniqueIdentifier);
                    Param[0].Value = mLogin.LoginGUID;
                    Param[1] = new SqlParameter("@pSessionID", SqlDbType.NVarChar, -1);
                    Param[1].Value = mLogin.SessionID;
                    Param[2] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
                    Param[2].Value = mLogin.LastModifiedDate;
                    Param[3] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
                    Param[3].Value = pMasterLogin.UserGUID;
                    Param[4] = new SqlParameter("@pExpiryTime", SqlDbType.DateTime);
                    Param[4].Value = pMasterLogin.ExpiryTime;

                    int result = context.Database.ExecuteSqlCommand("update MasterLogins set SessionID=@pSessionID,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,ExpiryTime=@pExpiryTime where LoginGUID=@pLoginGUID", Param);
                    if (result > 0)
                    {
                        return mLogin.SessionID;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<p_GetUsers_Result> DownloadUsers(string SessionID, Nullable<System.Guid> UserGUID = null)
        {
            List<p_GetUsers_Result> lresponse = new List<p_GetUsers_Result>();
            if (UserGUID == null)
            {
                SqlParameter[] sqlParam = new SqlParameter[2];
                sqlParam[0] = new SqlParameter("@pSessionID", SqlDbType.NVarChar);
                sqlParam[0].Value = SessionID;
                sqlParam[1] = new SqlParameter("@pErrorCode", SqlDbType.NVarChar, 200);
                sqlParam[1].Value = "";
                sqlParam[1].Direction = ParameterDirection.Output;
                lresponse = context.Database.SqlQuery<p_GetUsers_Result>("exec dbo.p_GetUsers  @pSessionID,@pErrorCode=@pErrorCode output", sqlParam).ToList();
            }
            else
            {
                SqlParameter[] sqlParam = new SqlParameter[3];
                sqlParam[0] = new SqlParameter("@pSessionID", SqlDbType.NVarChar);
                sqlParam[0].Value = SessionID;
                sqlParam[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
                sqlParam[1].Value = UserGUID;
                sqlParam[2] = new SqlParameter("@pErrorCode", SqlDbType.NVarChar, 200);
                sqlParam[2].Value = "";
                sqlParam[2].Direction = ParameterDirection.Output;
                lresponse = context.Database.SqlQuery<p_GetUsers_Result>("exec dbo.p_GetUsers  @pSessionID,@pUserGUID,@pErrorCode=@pErrorCode output", sqlParam).ToList();
            }
            return lresponse;
        }
        public int CheckUserName(string pUserName)
        {
            int lRetVal = -1;
            // 1: User Already Exists
            // 0: User not found
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    lRetVal = (from p in dataContext.GlobalUsers
            //               where p.UserName == pUserName && p.IsActive == true && p.IsDelete == false
            //               select p).Count();
            //}
            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pUserName", SqlDbType.NVarChar, -1);
            Param[0].Value = pUserName;
            Param[1] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[1].Value = true;
            Param[2] = new SqlParameter("@pIsDelete", SqlDbType.Bit);
            Param[2].Value = false;

            lRetVal = context.Database.SqlQuery<GlobalUser>("select * from  GlobalUsers where UserName=@pUserName and IsActive=@pIsActive and IsDelete=@pIsDelete", Param).Count();

            return lRetVal;
        }
        /// </summary>
        /// <param name="toEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes

                  = System.Text.Encoding.Unicode.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }

        /// <summary>
        /// The method to Decode your Base64 strings.
        /// </summary>
        /// <param name="encodedData">The String containing the characters to decode.</param>
        /// <returns>A String containing the results of decoding the specified sequence of bytes.</returns>
        public string DecodeFrom64(string encodedData)
        {

            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);

            string returnValue = System.Text.Encoding.Unicode.GetString(encodedDataAsBytes);

            return returnValue;

        }

        public DateTime? GetLocalDateTime_DateReturnType(DateTime? Convertdate, string ClientTimeZoneoffset)
        {
            try
            {
                if (Convertdate != null && ClientTimeZoneoffset != null)
                {
                    string Temp = ClientTimeZoneoffset.Trim();
                    if (!Temp.Contains("+") && !Temp.Contains("-"))
                    {
                        Temp = Temp.Insert(0, "+");
                    }
                    //Retrieve all system time zones available into a collection
                    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                    DateTime startTime = DateTime.Parse(Convertdate.ToString());
                    DateTime _now = DateTime.Parse(Convertdate.ToString());
                    foreach (TimeZoneInfo timeZoneInfo in timeZones)
                    {
                        if (timeZoneInfo.ToString().Contains(Temp))
                        {
                            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo.Id);
                            _now = TimeZoneInfo.ConvertTime(startTime, TimeZoneInfo.Utc, tst);
                            break;
                        }
                    }
                    return _now;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string GetLocalDateTime(DateTime? Convertdate, string ClientTimeZoneoffset)
        {
            try
            {
                if (Convertdate != null && ClientTimeZoneoffset != null)
                {
                    string Temp = ClientTimeZoneoffset.Trim();
                    if (!Temp.Contains("+") && !Temp.Contains("-"))
                    {
                        Temp = Temp.Insert(0, "+");
                    }
                    //Retrieve all system time zones available into a collection
                    ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                    DateTime startTime = DateTime.Parse(Convertdate.ToString());
                    DateTime _now = DateTime.Parse(Convertdate.ToString());
                    foreach (TimeZoneInfo timeZoneInfo in timeZones)
                    {
                        if (timeZoneInfo.ToString().Contains(Temp))
                        {
                            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo.Id);
                            _now = TimeZoneInfo.ConvertTime(startTime, TimeZoneInfo.Utc, tst);
                            break;
                        }
                    }
                    return _now.ToString();
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                return "";
            }

            ////string nzTimeZoneKey = "India Standard Time";
            //string result = string.Empty;
            //try
            //{

            //    if (!string.IsNullOrEmpty(timezoneid) && Convertdate != null)
            //    {
            //        DateTime _date = Convert.ToDateTime(Convertdate);
            //        string nzTimeZoneKey = timezoneid;
            //        TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            //        result = TimeZoneInfo.ConvertTimeFromUtc(_date, nzTimeZone).ToString();
            //    }
            //    else
            //    {
            //        result = "";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result = "";
            //}
            //return result;
        }
    }



    public class GeoIp
    {
        static public GeoIpData GetMy()
        {
            string url = "http://freegeoip.net/xml/";
            WebClient wc = new WebClient();
            wc.Proxy = null;
            MemoryStream ms = new MemoryStream(wc.DownloadData(url));
            XmlTextReader rdr = new XmlTextReader(url);
            XmlDocument doc = new XmlDocument();
            ms.Position = 0;
            doc.Load(ms);
            ms.Dispose();
            GeoIpData retval = new GeoIpData();
            foreach (XmlElement el in doc.ChildNodes[1].ChildNodes)
            {
                retval.KeyValue.Add(el.Name, el.InnerText);
            }
            return retval;
        }
    }
    public class GeoIpData
    {
        public GeoIpData()
        {
            KeyValue = new Dictionary<string, string>();
        }
        public Dictionary<string, string> KeyValue;
    }



}