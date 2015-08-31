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
    public class UserProfileRepository : IUserProfileRepository, IDisposable
    {
        private WorkersInMotionDB context;
        private readonly IUserRepository _IUserRepository;
        public UserProfileRepository(WorkersInMotionDB context)
        {
            this.context = context;
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
        }
        public IEnumerable<UserProfile> GetUserProfiles()
        {
            //var UserProfile = context.UserProfiles.ToList();
            //return context.UserProfiles.ToList().OrderBy(x => x.FirstName);



            return context.Database.SqlQuery<UserProfile>("select * from  UserProfiles order by FirstName");

        }
        public IEnumerable<UserProfile> GetUserProfilesbyOrganizationGUID(Guid OrganizationGUID)
        {
            List<UserProfile> userProfileList = new List<UserProfile>();
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    List<OrganizationUsersMap> OrgUserList = (from p in dataContext.OrganizationUsersMaps
            //                                              where p.OrganizationGUID == OrganizationGUID
            //                                              select p).ToList();

            //    foreach (OrganizationUsersMap item in OrgUserList)
            //    {
            //        UserProfile _uProfile = (from p in dataContext.UserProfiles
            //                                 where p.UserGUID == item.UserGUID
            //                                 select p).FirstOrDefault();
            //        userProfileList.Add(_uProfile);
            //    }
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;

            userProfileList = context.Database.SqlQuery<UserProfile>("Select * from UserProfiles where UserGUID in (select UserGUID from  OrganizationUsersMap where OrganizationGUID=@pOrganizationGUID)", Param).ToList();

            return userProfileList;
        }
        public IEnumerable<UserProfile> GetUserProfilesbyOrganizationGUID(Guid OrganizationGUID, string UserType)
        {
            List<UserProfile> userProfileList = new List<UserProfile>();
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    string RoleID = (from p in dataContext.AspNetRoles
            //                     where p.UserType == UserType
            //                     select p.Id).FirstOrDefault();

            //    if (!string.IsNullOrEmpty(RoleID))
            //    {
            //        List<OrganizationUsersMap> OrgUserList = (from p in dataContext.OrganizationUsersMaps
            //                                                  where p.OrganizationGUID == OrganizationGUID
            //                                                  select p).ToList();


            //        foreach (OrganizationUsersMap item in OrgUserList)
            //        {
            //            GlobalUser _globalUSer = (from p in dataContext.GlobalUsers
            //                                      where p.UserGUID == item.UserGUID && p.Role_Id == RoleID
            //                                      select p).FirstOrDefault();
            //            if (_globalUSer != null)
            //            {
            //                UserProfile _uProfile = (from p in dataContext.UserProfiles
            //                                         where p.UserGUID == item.UserGUID
            //                                         select p).FirstOrDefault();
            //                userProfileList.Add(_uProfile);
            //            }
            //        }
            //    }
            //}
            AspNetRole Role = _IUserRepository.GetRolebyUserType(UserType);
            if (Role != null)
            {
                SqlParameter[] Param = new SqlParameter[2];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = OrganizationGUID;
                Param[1] = new SqlParameter("@pRoleID", SqlDbType.NVarChar, 128);
                Param[1].Value = Role.Id;

                userProfileList = context.Database.SqlQuery<UserProfile>("Select * from UserProfiles where UserGUID in (Select UserGUID from GlobalUsers where Role_Id=@pRoleID and UserGUID in (select UserGUID from  OrganizationUsersMap where OrganizationGUID=@pOrganizationGUID))", Param).ToList();

            }

            return userProfileList;
        }
        //public IEnumerable<UserProfile> GetUserProfileByGroupGUID(Guid GroupGUID)
        //{
        //    IList<UserProfile> userProfileList = new List<UserProfile>();

        //    using (var dataContext = new WorkersInMotionDB())
        //    {
        //        IEnumerable<GlobalUser> GlobalUserList = (from p in dataContext.GlobalUser
        //                                                  where p.GroupGUID == GroupGUID
        //                                                  select p).ToList().OrderBy(r => r.UserName);
        //        foreach (var item in GlobalUserList)
        //        {
        //            UserProfile userProfile = (from p in dataContext.UserProfile
        //                                       where p.UserGUID == item.UserGUID
        //                                       select p).FirstOrDefault();
        //            userProfileList.Add(userProfile);
        //        }
        //        return userProfileList.OrderBy(r => r.FirstName);
        //    }
        ////}
        public UserProfile GetUserProfileByID(Guid ProfileGUID)
        {
            // return context.UserProfiles.Find(ProfileGUID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pProfileGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = ProfileGUID;

            return context.Database.SqlQuery<UserProfile>("Select * from UserProfiles where ProfileGUID=@pProfileGUID", Param).FirstOrDefault();

        }

        public UserProfile GetUserProfileByUserID(Guid UserGUID, Guid OrganizationGUID)
        {
            //OrganizationUsersMap _OrganizationUsermap = new OrganizationUsersMap();
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    _OrganizationUsermap = (from p in dataContext.OrganizationUsersMaps
            //                            where p.UserGUID == UserGUID && p.OrganizationGUID == OrganizationGUID
            //                            select p).FirstOrDefault();
            //    if (_OrganizationUsermap != null)
            //    {
            //        return (from p in dataContext.UserProfiles
            //                where p.UserGUID == _OrganizationUsermap.UserGUID
            //                select p).FirstOrDefault();
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = UserGUID;

            return context.Database.SqlQuery<UserProfile>("Select * from UserProfiles where UserGUID in (select UserGUID from  OrganizationUsersMap where OrganizationGUID=@pOrganizationGUID and UserGUID=@pUserGUID)", Param).FirstOrDefault();

        }

        public string GetUserIDFromEmail(string emailID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.UserProfiles
            //            where p.EmailID == emailID
            //            select p.UserGUID).FirstOrDefault().ToString();
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pEmailID", SqlDbType.NVarChar, -1);
            Param[0].Value = emailID;


            UserProfile uProfile = context.Database.SqlQuery<UserProfile>("Select * from UserProfiles where EmailID=@pEmailID", Param).FirstOrDefault();
            if (uProfile != null)
                return uProfile.EmailID;
            else
                return string.Empty;


        }

        public int InsertUserProfile(UserProfile userprofile)
        {
            //context.UserProfiles.Add(userprofile);
            SqlParameter[] Param = new SqlParameter[21];
            Param[0] = new SqlParameter("@pProfileGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = userprofile.ProfileGUID;
            Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = userprofile.UserGUID;
            Param[2] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, -1);
            Param[2].Value = (object)userprofile.FirstName ?? DBNull.Value;
            Param[3] = new SqlParameter("@pLastName", SqlDbType.NVarChar, -1);
            Param[3].Value = (object)userprofile.LastName ?? DBNull.Value;
            Param[4] = new SqlParameter("@pCompanyName", SqlDbType.NVarChar, -1);
            Param[4].Value = (object)userprofile.CompanyName ?? DBNull.Value;
            Param[5] = new SqlParameter("@pMobilePhone", SqlDbType.NVarChar, -1);
            Param[5].Value = (object)userprofile.MobilePhone ?? DBNull.Value;
            Param[6] = new SqlParameter("@pBusinessPhone", SqlDbType.NVarChar, -1);
            Param[6].Value = (object)userprofile.BusinessPhone ?? DBNull.Value;
            Param[7] = new SqlParameter("@pHomePhone", SqlDbType.NVarChar, -1);
            Param[7].Value = (object)userprofile.HomePhone ?? DBNull.Value;
            Param[8] = new SqlParameter("@pEmailID", SqlDbType.NVarChar, -1);
            Param[8].Value = (object)userprofile.EmailID ?? DBNull.Value;
            Param[9] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, -1);
            Param[9].Value = (object)userprofile.AddressLine1 ?? DBNull.Value;
            Param[10] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, -1);
            Param[10].Value = (object)userprofile.AddressLine2 ?? DBNull.Value;
            Param[11] = new SqlParameter("@pCity", SqlDbType.NVarChar, -1);
            Param[11].Value = (object)userprofile.City ?? DBNull.Value;
            Param[12] = new SqlParameter("@pState", SqlDbType.NVarChar, -1);
            Param[12].Value = (object)userprofile.State ?? DBNull.Value;
            Param[13] = new SqlParameter("@pCountry", SqlDbType.NVarChar, -1);
            Param[13].Value = (object)userprofile.Country ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[14].Value = (object)userprofile.Latitude ?? DBNull.Value;
            Param[15] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[15].Value = (object)userprofile.Longitude ?? DBNull.Value;
            Param[16] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, -1);
            Param[16].Value = (object)userprofile.ZipCode ?? DBNull.Value;
            Param[17] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[17].Value = (object)userprofile.IsDeleted ?? DBNull.Value;
            Param[18] = new SqlParameter("@pPicFileURL", SqlDbType.NVarChar, -1);
            Param[18].Value = (object)userprofile.PicFileURL ?? DBNull.Value;
            Param[19] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[19].Value = (object)userprofile.LastModifiedDate ?? DBNull.Value;
            Param[20] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[20].Value = (object)userprofile.LastModifiedBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into UserProfiles(ProfileGUID,UserGUID,FirstName,LastName,CompanyName,MobilePhone,BusinessPhone,"
            + "HomePhone,EmailID,AddressLine1,AddressLine2,City,State,Country,Latitude,Longitude,ZipCode,IsDeleted,PicFileURL,LastModifiedDate,LastModifiedBy)"
            + "values(@pProfileGUID,@pUserGUID,@pFirstName,@pLastName,@pCompanyName,@pMobilePhone,@pBusinessPhone,"
            + "@pHomePhone,@pEmailID,@pAddressLine1,@pAddressLine2,@pCity,@pState,@pCountry,@pLatitude,@pLongitude,@pZipCode,@pIsDeleted,@pPicFileURL,@pLastModifiedDate,@pLastModifiedBy)", Param);

        }

        public int DeleteUserProfile(Guid ProfileGUID)
        {
            //UserProfile userprofile = context.UserProfiles.Find(ProfileGUID);
            //if (userprofile != null)
            //    context.UserProfiles.Remove(userprofile);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pProfileGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = ProfileGUID;


            return context.Database.ExecuteSqlCommand("delete from UserProfiles where ProfileGUID=@pProfileGUID", Param);
        }

        public int DeleteUserProfileByUserGUID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Userlist = (from p in dataContext.UserProfiles
            //                    where p.UserGUID == UserGUID
            //                    select p).FirstOrDefault();
            //    if (Userlist != null)
            //    {
            //        dataContext.UserProfiles.Remove(Userlist);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;


            return context.Database.ExecuteSqlCommand("delete from UserProfiles where UserGUID=@pUserGUID", Param);
        }

        public int UpdateUserProfile(UserProfile userprofile)
        {
            // context.Entry(userprofile).State = EntityState.Modified;
            SqlParameter[] Param = new SqlParameter[21];
            Param[0] = new SqlParameter("@pProfileGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = userprofile.ProfileGUID;
            Param[1] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = userprofile.UserGUID;
            Param[2] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, -1);
            Param[2].Value = (object)userprofile.FirstName ?? DBNull.Value;
            Param[3] = new SqlParameter("@pLastName", SqlDbType.NVarChar, -1);
            Param[3].Value = (object)userprofile.LastName ?? DBNull.Value;
            Param[4] = new SqlParameter("@pCompanyName", SqlDbType.NVarChar, -1);
            Param[4].Value = (object)userprofile.CompanyName ?? DBNull.Value;
            Param[5] = new SqlParameter("@pMobilePhone", SqlDbType.NVarChar, -1);
            Param[5].Value = (object)userprofile.MobilePhone ?? DBNull.Value;
            Param[6] = new SqlParameter("@pBusinessPhone", SqlDbType.NVarChar, -1);
            Param[6].Value = (object)userprofile.BusinessPhone ?? DBNull.Value;
            Param[7] = new SqlParameter("@pHomePhone", SqlDbType.NVarChar, -1);
            Param[7].Value = (object)userprofile.HomePhone ?? DBNull.Value;
            Param[8] = new SqlParameter("@pEmailID", SqlDbType.NVarChar, -1);
            Param[8].Value = (object)userprofile.EmailID ?? DBNull.Value;
            Param[9] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, -1);
            Param[9].Value = (object)userprofile.AddressLine1 ?? DBNull.Value;
            Param[10] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, -1);
            Param[10].Value = (object)userprofile.AddressLine2 ?? DBNull.Value;
            Param[11] = new SqlParameter("@pCity", SqlDbType.NVarChar, -1);
            Param[11].Value = (object)userprofile.City ?? DBNull.Value;
            Param[12] = new SqlParameter("@pState", SqlDbType.NVarChar, -1);
            Param[12].Value = (object)userprofile.State ?? DBNull.Value;
            Param[13] = new SqlParameter("@pCountry", SqlDbType.NVarChar, -1);
            Param[13].Value = (object)userprofile.Country ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[14].Value = (object)userprofile.Latitude ?? DBNull.Value;
            Param[15] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[15].Value = (object)userprofile.Longitude ?? DBNull.Value;
            Param[16] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, -1);
            Param[16].Value = (object)userprofile.ZipCode ?? DBNull.Value;
            Param[17] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[17].Value = (object)userprofile.IsDeleted ?? DBNull.Value;
            Param[18] = new SqlParameter("@pPicFileURL", SqlDbType.NVarChar, -1);
            Param[18].Value = (object)userprofile.PicFileURL ?? DBNull.Value;
            Param[19] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[19].Value = (object)userprofile.LastModifiedDate ?? DBNull.Value;
            Param[20] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[20].Value = (object)userprofile.LastModifiedBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("update UserProfiles set ProfileGUID=@pProfileGUID,UserGUID=@pUserGUID,FirstName=@pFirstName,LastName=@pLastName,CompanyName=@pCompanyName,MobilePhone=@pMobilePhone,BusinessPhone=@pBusinessPhone,"
            + "HomePhone=@pHomePhone,EmailID=@pEmailID,AddressLine1=@pAddressLine1,AddressLine2=@pAddressLine2,City=@pCity,State=@pState,Country=@pCountry,Latitude=@pLatitude,Longitude=@pLongitude,ZipCode=@pZipCode,IsDeleted=@pIsDeleted,PicFileURL=@pPicFileURL,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy", Param);

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


    }
}