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
    public class GlobalUserRepository : IGlobalUserRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public GlobalUserRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }
        public GlobalUserRepository()
        {
            WorkersInMotionDB context = new WorkersInMotionDB();
            this.context = context;
        }
        public IEnumerable<GlobalUser> GetGlobalUser()
        {
            //var GlobalUser = context.GlobalUsers.ToList();
            //return context.GlobalUsers.ToList().OrderBy(r => r.UserName);

            return context.Database.SqlQuery<GlobalUser>("select * from GlobalUsers order by UserName");
        }
        public IEnumerable<GlobalUser> GetGlobalUserByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    List<GlobalUser> globalUserList = new List<GlobalUser>();
            //    List<OrganizationUsersMap> OrgUserList = (from p in dataContext.OrganizationUsersMaps
            //                                              where p.OrganizationGUID == OrganizationGUID
            //                                              select p).ToList();
            //    if (OrgUserList != null)
            //    {
            //        foreach (OrganizationUsersMap item in OrgUserList)
            //        {
            //            GlobalUser _gUser = (from p in dataContext.GlobalUsers
            //                                 where p.UserGUID == item.UserGUID
            //                                 select p).FirstOrDefault();
            //            if (_gUser != null)
            //            {
            //                globalUserList.Add(_gUser);
            //            }
            //        }
            //    }
            //    return globalUserList.OrderBy(x => x.UserName);
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;


            return context.Database.SqlQuery<GlobalUser>("Select * from GlobalUsers where UserGUID in(Select UserGUID from OrganizationUsersMap where OrganizationGUID=@pOrganizationGUID) order by UserName", Param);


        }

        public IEnumerable<GlobalUser> GetGlobalUserByRegionandTerritory(Guid RegionGUID, Guid TerritoryGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    List<GlobalUser> globalUserList = new List<GlobalUser>();
            //    List<OrganizationUsersMap> OrgUserList = (from p in dataContext.OrganizationUsersMaps
            //                                              where p.RegionGUID == RegionGUID && p.TerritoryGUID == TerritoryGUID
            //                                              select p).ToList();
            //    if (OrgUserList != null)
            //    {
            //        foreach (OrganizationUsersMap item in OrgUserList)
            //        {
            //            GlobalUser _gUser = (from p in dataContext.GlobalUsers
            //                                 where p.UserGUID == item.UserGUID
            //                                 select p).FirstOrDefault();
            //            if (_gUser != null)
            //            {
            //                globalUserList.Add(_gUser);
            //            }
            //        }
            //    }
            //    return globalUserList.OrderBy(x => x.UserName);
            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            Param[1] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = TerritoryGUID;


            return context.Database.SqlQuery<GlobalUser>("Select * from GlobalUsers where UserGUID in(Select UserGUID from OrganizationUsersMap where RegionGUID=@pRegionGUID and TerritoryGUID=@pTerritoryGUID) order by UserName", Param);


        }


        public IEnumerable<AspNetRole> GetRoles()
        {
            //var AspNetRole = context.AspNetRoles.ToList();
            //return context.AspNetRoles.ToList().OrderBy(r => r.Name);
            return context.Database.SqlQuery<AspNetRole>("Select * from AspNetRoles order by Name");
        }

        public string GetUserType(Guid UserID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    string RoleID = (from p in dataContext.GlobalUsers
            //                     where p.UserGUID == UserID
            //                         select p.Role_Id).FirstOrDefault();

            //    string userType = (from p in dataContext.AspNetRoles
            //                       where p.Id == RoleID
            //                       select p.UserType).FirstOrDefault();
            //    return userType;

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserID;

            AspNetRole role = context.Database.SqlQuery<AspNetRole>("Select * from AspNetRoles where Id in(Select Role_Id from GlobalUsers where UserGUID=@pUserGUID)", Param).FirstOrDefault();
            if (role != null)
                return role.UserType;
            else
                return string.Empty;


        }

        public string GetPassword(Guid UserID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.GlobalUsers
            //            where p.UserGUID == UserID
            //            select p.Password).FirstOrDefault();


            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserID;

            GlobalUser user = context.Database.SqlQuery<GlobalUser>("Select * from GlobalUsers where UserGUID=@pUserGUID", Param).FirstOrDefault();
            if (user != null)
                return user.Password;
            else
                return string.Empty;


        }
        public string GetUserRoleName(Guid UserID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    string RoleID = (from p in dataContext.GlobalUsers
            //                     where p.UserGUID == UserID
            //                     select p.Role_Id).FirstOrDefault();

            //    string RoleName = (from p in dataContext.AspNetRoles
            //                       where p.Id == RoleID
            //                       select p.Name).FirstOrDefault();
            //    return RoleName;

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserID;

            AspNetRole role = context.Database.SqlQuery<AspNetRole>("Select * from AspNetRoles where Id in(Select Role_Id from GlobalUsers where UserGUID=@pUserGUID)", Param).FirstOrDefault();
            if (role != null)
                return role.Name;
            else
                return string.Empty;

        }
        public string GetUserTypeByRoleID(string RoleGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    string userType = (from p in dataContext.AspNetRoles
            //                       where p.Id == RoleGUID
            //                       select p.UserType).FirstOrDefault();
            //    return userType;

            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRoleGUID", SqlDbType.NVarChar, 128);
            Param[0].Value = RoleGUID;

            AspNetRole role = context.Database.SqlQuery<AspNetRole>("Select * from AspNetRoles where Id=@pRoleGUID", Param).FirstOrDefault();
            if (role != null)
                return role.UserType;
            else
                return string.Empty;

        }
        public string GetOrganizationAdminRoleID()
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.AspNetRoles
            //            where p.Name == "Organization Administrator"
            //            select p.Id).SingleOrDefault();

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pName", SqlDbType.NVarChar, -1);
            Param[0].Value = "Organization Administrator";
            AspNetRole role = context.Database.SqlQuery<AspNetRole>("Select * from AspNetRoles where Name=@pName", Param).FirstOrDefault();
            if (role != null)
                return role.Id;
            else
                return string.Empty;
        }

        public GlobalUser GetPasswordFromUserGUID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.GlobalUsers
            //            where p.UserGUID == UserGUID
            //            select p).SingleOrDefault();

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.SqlQuery<GlobalUser>("Select * from GlobalUsers where UserGUID=@pUserGUID", Param).FirstOrDefault();
        }
        //public GlobalUser GetGlobalUserByUserID(string UserID)
        //{
        //    using (var dataContext = new WorkersInMotionDB())
        //    {
        //        return (from p in dataContext.GlobalUsers
        //                where p.USERID == UserID
        //                select p).FirstOrDefault();

        //    }
        //}

        public GlobalUser GetGlobalUserByUserID(string UserID, string OrganizationGUID)
        {
            //GlobalUser User = null;
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    List<OrganizationUsersMap> Organization_Users = (from p in dataContext.OrganizationUsersMaps
            //                                                     where p.OrganizationGUID == new Guid(OrganizationGUID)
            //                                                     select p).ToList();

            //    if (Organization_Users != null)
            //    {
            //        foreach (OrganizationUsersMap item in Organization_Users)
            //        {
            //            GlobalUser _gUser = (from p in dataContext.GlobalUsers
            //                                 where p.UserGUID == item.UserGUID
            //                                 select p).FirstOrDefault();
            //            if (_gUser != null && !string.IsNullOrEmpty(_gUser.USERID) && _gUser.USERID.ToUpper().Trim() == UserID.ToUpper().Trim())
            //            {
            //                User = _gUser;
            //                break;
            //            }
            //        }
            //    }
            //    return User;

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pUSERID", SqlDbType.NVarChar, 50);
            Param[0].Value = UserID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = new Guid(OrganizationGUID);
            return context.Database.SqlQuery<GlobalUser>("Select * from GlobalUsers where UserGUID in(Select UserGUID from GlobalUsers where UserID=@pUSERID and UserGUID in(Select UserGUID from OrganizationUsersMap where OrganizationGUID=@pOrganizationGUID))", Param).FirstOrDefault();
        }
        public GlobalUser GetGlobalUserByID(Guid UserGUID)
        {
            // return context.GlobalUsers.Find(UserGUID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.SqlQuery<GlobalUser>("Select * from GlobalUsers where UserGUID=@pUserGUID", Param).FirstOrDefault();
        }

        public int InsertGlobalUser(GlobalUser globaluser)
        {
            // context.GlobalUsers.Add(globaluser);

            SqlParameter[] Param = new SqlParameter[16];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = globaluser.UserGUID;
            Param[1] = new SqlParameter("@pUserName", SqlDbType.NVarChar, -1);
            Param[1].Value = globaluser.UserName;
            Param[2] = new SqlParameter("@pPassword", SqlDbType.NVarChar, -1);
            Param[2].Value = globaluser.Password;
            Param[3] = new SqlParameter("@pRole_Id", SqlDbType.NVarChar, 128);
            Param[3].Value = (object)globaluser.Role_Id ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[4].Value = (object)globaluser.IsActive ?? DBNull.Value;
            Param[5] = new SqlParameter("@pReportingPlaceType", SqlDbType.Int);
            Param[5].Value = (object)globaluser.ReportingPlaceType ?? DBNull.Value;
            Param[6] = new SqlParameter("@pReportPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)globaluser.ReportPlaceGUID ?? DBNull.Value;
            Param[7] = new SqlParameter("@pIsExempt", SqlDbType.Bit);
            Param[7].Value = (object)globaluser.IsExempt ?? DBNull.Value;
            Param[8] = new SqlParameter("@pIsDelete", SqlDbType.Bit);
            Param[8].Value = (object)globaluser.IsDelete ?? DBNull.Value;
            Param[9] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[9].Value = (object)globaluser.Latitude ?? DBNull.Value;
            Param[10] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[10].Value = (object)globaluser.Longitude ?? DBNull.Value;
            Param[11] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[11].Value = (object)globaluser.CreateDate ?? DBNull.Value;
            Param[12] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)globaluser.CreateBy ?? DBNull.Value;
            Param[13] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[13].Value = (object)globaluser.LastModifiedDate ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[14].Value = (object)globaluser.LastModifiedBy ?? DBNull.Value;
            Param[15] = new SqlParameter("@pUSERID", SqlDbType.NVarChar, 50);
            Param[15].Value = (object)globaluser.USERID ?? DBNull.Value;


            return context.Database.ExecuteSqlCommand("Insert into GlobalUsers(UserGUID,UserName,Password,Role_Id,IsActive,ReportingPlaceType,ReportPlaceGUID,"
                    + "IsExempt,IsDelete,Latitude,Longitude,CreateDate,CreateBy,LastModifiedDate,LastModifiedBy,USERID)"
                    + "values(@pUserGUID,@pUserName,@pPassword,@pRole_Id,@pIsActive,@pReportingPlaceType,@pReportPlaceGUID,"
                    + "@pIsExempt,@pIsDelete,@pLatitude,@pLongitude,@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy,@pUSERID)", Param);

        }



        public int DeleteGlobalUser(Guid UserGUID)
        {
            //GlobalUser globaluser = context.GlobalUsers.Find(UserGUID);
            //if (globaluser != null)
            //    context.GlobalUsers.Remove(globaluser);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.ExecuteSqlCommand("delete from GlobalUsers where UserGUID=@pUserGUID", Param);
        }

        //public void DeleteGlobalUserByOrganizationGUID(Guid OrganizationGUID)
        //{
        //    List<OrganizationUsersMap> OrganizationUserMapList = new List<OrganizationUsersMap>();
        //    using (var dataContext = new WorkersInMotionDB())
        //    {
        //        OrganizationUserMapList = (from p in dataContext.OrganizationUsersMaps
        //                                   where p.OrganizationGUID == OrganizationGUID
        //                                   select p).ToList();
        //        if (OrganizationUserMapList != null)
        //        {
        //            foreach (OrganizationUsersMap item in OrganizationUserMapList)
        //            {
        //                var GlobalUser = (from p in dataContext.GlobalUsers
        //                                  where p.UserGUID == item.UserGUID
        //                                  select p).FirstOrDefault();
        //                if (GlobalUser != null)
        //                {
        //                    dataContext.GlobalUsers.Remove(GlobalUser);
        //                    dataContext.SaveChanges();
        //                }
        //            }
        //        }
        //    }
        //}

        public int UpdateGlobalUser(GlobalUser globaluser)
        {
            //context.Entry(globaluser).State = EntityState.Modified;

            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    //    var qry = from p in dataContext.MasterLogin where p.UserGUID == globaluser.UserGUID select p;
            //    //    if (qry != null && qry.Count() > 0)
            //    //    {
            //    //        var item = qry.Single();
            //    //        item.UserType = GetRole(globaluser.Role_Id).UserType;
            //    //        item.RegionGUID = globaluser.RegionGUID;
            //    //        item.TerritoryGUID = globaluser.TerritoryGUID;
            //    //        item.GroupGUID = globaluser.GroupGUID;
            //    //        dataContext.SaveChanges();
            //    //    }
            //}

            SqlParameter[] Param = new SqlParameter[16];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = globaluser.UserGUID;
            Param[1] = new SqlParameter("@pUserName", SqlDbType.NVarChar, -1);
            Param[1].Value = globaluser.UserName;
            Param[2] = new SqlParameter("@pPassword", SqlDbType.NVarChar, -1);
            Param[2].Value = globaluser.Password;
            Param[3] = new SqlParameter("@pRole_Id", SqlDbType.NVarChar, 128);
            Param[3].Value = (object)globaluser.Role_Id ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[4].Value = (object)globaluser.IsActive ?? DBNull.Value;
            Param[5] = new SqlParameter("@pReportingPlaceType", SqlDbType.Int);
            Param[5].Value = (object)globaluser.ReportingPlaceType ?? DBNull.Value;
            Param[6] = new SqlParameter("@pReportPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)globaluser.ReportPlaceGUID ?? DBNull.Value;
            Param[7] = new SqlParameter("@pIsExempt", SqlDbType.Bit);
            Param[7].Value = (object)globaluser.IsExempt ?? DBNull.Value;
            Param[8] = new SqlParameter("@pIsDelete", SqlDbType.Bit);
            Param[8].Value = (object)globaluser.IsDelete ?? DBNull.Value;
            Param[9] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[9].Value = (object)globaluser.Latitude ?? DBNull.Value;
            Param[10] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[10].Value = (object)globaluser.Longitude ?? DBNull.Value;
            Param[11] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[11].Value = (object)globaluser.CreateDate ?? DBNull.Value;
            Param[12] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)globaluser.CreateBy ?? DBNull.Value;
            Param[13] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[13].Value = (object)globaluser.LastModifiedDate ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[14].Value = (object)globaluser.LastModifiedBy ?? DBNull.Value;
            Param[15] = new SqlParameter("@pUSERID", SqlDbType.NVarChar, 50);
            Param[15].Value = (object)globaluser.USERID ?? DBNull.Value;


            return context.Database.ExecuteSqlCommand("Update GlobalUsers set UserName=@pUserName,Password=@pPassword,Role_Id=@pRole_Id,IsActive=@pIsActive,ReportingPlaceType=@pReportingPlaceType,ReportPlaceGUID=@pReportPlaceGUID,"
                    + "IsExempt=@pIsExempt,IsDelete=@pIsDelete,Latitude=@pLatitude,Longitude=@pLongitude,CreateDate=@pCreateDate,CreateBy=@pCreateBy,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy,USERID=@pUSERID where UserGUID=@pUserGUID", Param);

        }

        //public int UpdateGroupGUID(Guid UserGUID, Guid GroupGUID)
        //{
        //    int result = 0;
        //    using (var dataContext = new WorkersInMotionDB())
        //    {
        //        var qry = from p in dataContext.GlobalUsers where p.UserGUID == UserGUID select p;
        //        if (qry != null && qry.Count() > 0)
        //        {
        //            //var item = qry.Single();
        //            //item.GroupGUID = GroupGUID;
        //            //result = dataContext.SaveChanges();
        //        }
        //    }
        //    return result;
        //}

        public string GetRoleID(string UserType)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.AspNetRoles
            //            where p.UserType == UserType
            //            select p).SingleOrDefault().Id;

            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserType", SqlDbType.NVarChar, 10);
            Param[0].Value = UserType;
            AspNetRole role = context.Database.SqlQuery<AspNetRole>("Select * from AspNetRoles where UserType=@pUserType", Param).FirstOrDefault();
            if (role != null)
                return role.Id;
            else
                return string.Empty;
        }
        public AspNetRole GetRole(string RoleID)
        {
            //return context.AspNetRoles.Find(RoleID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pId", SqlDbType.NVarChar, 128);
            Param[0].Value = RoleID;
            return context.Database.SqlQuery<AspNetRole>("Select * from AspNetRoles where Id=@pId", Param).FirstOrDefault();
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
        //public GlobalUser DeviceLogin(GlobalUser plGlobalUser)
        //{
        //    GlobalUser lGlobalUser = new GlobalUser();
        //    return lGlobalUser;
        //}

        //public GetUsers GetUserFromClient(Guid UserGUID, Guid OrganizationGUID)
        //{
        //    string UserType = GetUserType(UserGUID);
        //    GetUsers _getUsers = new GetUsers();
        //    List<GlobalUser> globalUser = new List<GlobalUser>();
        //    List<UserProfile> userProfile = new List<UserProfile>();
        //    if (!string.IsNullOrEmpty(UserType))
        //    {
        //        //if (UserType == "ENT_A")
        //        //{
        //        //    using (var dataContext = new WorkersInMotionContext())
        //        //    {

        //        //        globalUser = (from p in dataContext.GlobalUser
        //        //                      where p.OrganizationGUID == OrganizationGUID
        //        //                      select p).ToList();
        //        //        foreach (GlobalUser item in globalUser)
        //        //        {
        //        //            userProfile.Add((from p in dataContext.UserProfile
        //        //                             where p.UserGUID == item.UserGUID
        //        //                             select p).SingleOrDefault());
        //        //        }
        //        //    }
        //        //}
        //        //else if (UserType == "ENT_U_RM")
        //        //{
        //        //    using (var dataContext = new WorkersInMotionContext())
        //        //    {

        //        //        globalUser = (from p in dataContext.GlobalUser
        //        //                      where p.OrganizationGUID == OrganizationGUID && (p.UserType == "ENT_U_RM" || p.UserType == "ENT_U_TM" || p.UserType == "ENT_U")
        //        //                      select p).ToList();
        //        //        foreach (GlobalUser item in globalUser)
        //        //        {
        //        //            userProfile.Add((from p in dataContext.UserProfile
        //        //                             where p.UserGUID == item.UserGUID
        //        //                             select p).SingleOrDefault());
        //        //        }
        //        //    }
        //        //}
        //        //else if (UserType == "ENT_U_TM")
        //        //{
        //        //    using (var dataContext = new WorkersInMotionContext())
        //        //    {

        //        //        globalUser = (from p in dataContext.GlobalUser
        //        //                      where p.OrganizationGUID == OrganizationGUID && (p.UserType == "ENT_U_TM" || p.UserType == "ENT_U")
        //        //                      select p).ToList();
        //        //        foreach (GlobalUser item in globalUser)
        //        //        {
        //        //            userProfile.Add((from p in dataContext.UserProfile
        //        //                             where p.UserGUID == item.UserGUID
        //        //                             select p).SingleOrDefault());
        //        //        }
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    using (var dataContext = new WorkersInMotionContext())
        //        //    {

        //        //        globalUser = (from p in dataContext.GlobalUser
        //        //                      where p.UserGUID == UserGUID
        //        //                      select p).ToList();
        //        //        foreach (GlobalUser item in globalUser)
        //        //        {
        //        //            userProfile.Add((from p in dataContext.UserProfile
        //        //                             where p.UserGUID == item.UserGUID
        //        //                             select p).SingleOrDefault());
        //        //        }

        //        //    }
        //        //}
        //    }
        //    _getUsers.GlobalUser = globalUser;
        //    _getUsers.UserProfile = userProfile;
        //    return _getUsers;
        //}

        //public GetUsers GetClientUsers(string SessionID)
        //{
        //    GetUsers _getUsers = new GetUsers();
        //    using (var dataContext = new WorkersInMotionDB())
        //    {

        //        //Guid OrganizationGUID = new Guid((from p in dataContext.MasterLogin
        //        //                                  where p.SessionID == SessionID
        //        //                                  select p).SingleOrDefault().OrganizationGUID.ToString());
        //        //Guid UserGUID = new Guid((from p in dataContext.MasterLogin
        //        //                          where p.SessionID == SessionID
        //        //                          select p).SingleOrDefault().UserGUID.ToString());
        //        //_getUsers = GetUserFromClient(UserGUID, OrganizationGUID);
        //    }
        //    return _getUsers;
        //}

        public string GetUserID(string SessionID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.MasterLogins
            //            where p.SessionID == SessionID
            //            select p).SingleOrDefault().UserGUID.ToString();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pSessionID", SqlDbType.NVarChar, -1);
            Param[0].Value = SessionID;
            MasterLogin mLogin = context.Database.SqlQuery<MasterLogin>("Select * from MasterLogins where SessionID=@pSessionID", Param).FirstOrDefault();
            if (mLogin != null)
                return mLogin.UserGUID.ToString();
            else
                return string.Empty;

        }

        public string GetOrganizationID(string SessionID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    string UserGUID = (from p in dataContext.MasterLogins
            //                       where p.SessionID == SessionID
            //                       select p).SingleOrDefault().UserGUID.ToString();
            //    if (!string.IsNullOrEmpty(UserGUID))
            //    {
            //        return (from p in dataContext.OrganizationUsersMaps
            //                where p.UserGUID == new Guid(UserGUID)
            //                select p).SingleOrDefault().OrganizationGUID.ToString();
            //    }
            //    else
            //    {
            //        return "";
            //    }
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pSessionID", SqlDbType.NVarChar, -1);
            Param[0].Value = SessionID;
            OrganizationUsersMap mOrganizationUsersMap = context.Database.SqlQuery<OrganizationUsersMap>("Select * from OrganizationUsersMap where UserGUID in (Select UserGUID from MasterLogins where SessionID=@pSessionID)", Param).FirstOrDefault();
            if (mOrganizationUsersMap != null)
                return mOrganizationUsersMap.OrganizationGUID.ToString();
            else
                return string.Empty;

        }
    }


    public class GetUsers
    {
        public IList<GlobalUser> GlobalUser { get; set; }
        public IList<UserProfile> UserProfile { get; set; }
    }
}