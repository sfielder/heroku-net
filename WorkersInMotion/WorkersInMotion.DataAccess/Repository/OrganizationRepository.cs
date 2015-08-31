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
    public class OrganizationRepository : IOrganizationRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public OrganizationRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }
        public IEnumerable<Organization> GetOrganization()
        {
            //var Organization = context.Organizations.ToList();
            //return context.Organizations.ToList().OrderBy(x => x.OrganizationFullName);

            return context.Database.SqlQuery<Organization>("select * from  Organizations order by OrganizationFullName");
        }

        public Organization GetOrganizationByID(Guid OrganizationGUID)
        {
            //return context.Organizations.Find(OrganizationGUID);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<Organization>("select * from  Organizations where OrganizationGUID=@pOrganizationGUID", Param).FirstOrDefault();
        }
        public Guid GetOrganizationIDByUserGUID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.OrganizationUsersMaps
            //            where p.UserGUID == UserGUID
            //            select p).FirstOrDefault().OrganizationGUID;
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.SqlQuery<Guid>("select OrganizationGUID from  OrganizationUsersMap where UserGUID=@pUserGUID", Param).FirstOrDefault();
        }

        public OrganizationUsersMap GetOrganizationUserMapByUserGUID(Guid UserGUID)
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

        public OrganizationUsersMap GetOrganizationUserMapByUserGUID(Guid UserGUID, Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.OrganizationUsersMaps
            //            where p.UserGUID == UserGUID && p.OrganizationGUID == OrganizationGUID
            //            select p).FirstOrDefault();
            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationGUID;
            return context.Database.SqlQuery<OrganizationUsersMap>("select * from  OrganizationUsersMap where UserGUID=@pUserGUID and OrganizationGUID=@pOrganizationGUID", Param).FirstOrDefault();
        }


        public List<OrganizationUsersMap> GetOrganizationUserMapByOrgGUID(Guid OrganizationGUID)
        {
            //    using (var dataContext = new WorkersInMotionDB())
            //    {
            //        return (from p in dataContext.OrganizationUsersMaps
            //                where p.OrganizationGUID == OrganizationGUID
            //                select p).ToList();
            //    }

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<OrganizationUsersMap>("select * from  OrganizationUsersMap where OrganizationGUID=@pOrganizationGUID", Param).ToList();
        }


        public Organization GetOrganizationByName(string OrganizationName)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Organizations
            //            where p.OrganizationName == OrganizationName || p.OrganizationFullName == OrganizationName
            //            select p).FirstOrDefault();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationName", SqlDbType.NVarChar, -1);
            Param[0].Value = OrganizationName;
            return context.Database.SqlQuery<Organization>("select * from  OrganizationUsersMap where OrganizationName=@pOrganizationName", Param).FirstOrDefault();
        }

        public int InsertOrganization(Organization organization)
        {
            //context.Organizations.Add(organization);

            SqlParameter[] Param = new SqlParameter[23];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = organization.OrganizationGUID;
            Param[1] = new SqlParameter("@pOrganizationName", SqlDbType.NVarChar, -1);
            Param[1].Value = organization.OrganizationName;
            Param[2] = new SqlParameter("@pOrganizationFullName", SqlDbType.NVarChar, -1);
            Param[2].Value = organization.OrganizationFullName;
            Param[3] = new SqlParameter("@pWebsite", SqlDbType.NVarChar, -1);
            Param[3].Value = (object)organization.Website ?? DBNull.Value;
            Param[4] = new SqlParameter("@pPhone", SqlDbType.NVarChar, -1);
            Param[4].Value = (object)organization.Phone ?? DBNull.Value;
            Param[5] = new SqlParameter("@pTimeZone", SqlDbType.Float);
            Param[5].Value = (object)organization.TimeZone ?? DBNull.Value;
            Param[6] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[6].Value = (object)organization.Latitude ?? DBNull.Value;
            Param[7] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[7].Value = (object)organization.Longitude ?? DBNull.Value;
            Param[8] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, -1);
            Param[8].Value = (object)organization.AddressLine1 ?? DBNull.Value;
            Param[9] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, -1);
            Param[9].Value = (object)organization.AddressLine2 ?? DBNull.Value;
            Param[10] = new SqlParameter("@pCity", SqlDbType.NVarChar, -1);
            Param[10].Value = organization.City;
            Param[11] = new SqlParameter("@pState", SqlDbType.NVarChar, -1);
            Param[11].Value = organization.State;
            Param[12] = new SqlParameter("@pCountry", SqlDbType.NVarChar, -1);
            Param[12].Value = organization.Country;
            Param[13] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, -1);
            Param[13].Value = organization.ZipCode;
            Param[14] = new SqlParameter("@pEmailID", SqlDbType.NVarChar, -1);
            Param[14].Value = organization.EmailID;
            Param[15] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[15].Value = (object)organization.IsActive ?? DBNull.Value;
            Param[16] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[16].Value = (object)organization.IsDeleted ?? DBNull.Value;
            Param[17] = new SqlParameter("@pAllowContractors", SqlDbType.Bit);
            Param[17].Value = (object)organization.AllowContractors ?? DBNull.Value;
            Param[18] = new SqlParameter("@pImageURL", SqlDbType.NVarChar, -1);
            Param[18].Value = (object)organization.ImageURL ?? DBNull.Value;
            Param[19] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[19].Value = (object)organization.CreateDate ?? DBNull.Value;
            Param[20] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[20].Value = (object)organization.CreateBy ?? DBNull.Value;
            Param[21] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[21].Value = (object)organization.LastModifiedDate ?? DBNull.Value;
            Param[22] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[22].Value = (object)organization.LastModifiedBy ?? DBNull.Value;


            return context.Database.ExecuteSqlCommand("insert into Organizations(OrganizationGUID,OrganizationName,OrganizationFullName,Website,Phone,TimeZone,Latitude,Longitude,"
                                    + "AddressLine1,AddressLine2,City,State,Country,ZipCode,EmailID,IsActive,IsDeleted,AllowContractors,ImageURL,CreateDate,CreateBy,LastModifiedDate,LastModifiedBy)"
                                    + "values(@pOrganizationGUID,@pOrganizationName,@pOrganizationFullName,@pWebsite,@pPhone,@pTimeZone,@pLatitude,@pLongitude,"
                                    + "@pAddressLine1,@pAddressLine2,@pCity,@pState,@pCountry,@pZipCode,@pEmailID,@pIsActive,@pIsDeleted,@pAllowContractors,"
                                    + "@pImageURL,@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy)", Param);
        }

        public int InsertOrganizationUserMap(OrganizationUsersMap OrganizationUsersMap)
        {
            //    context.OrganizationUsersMaps.Add(OrganizationUsersMap);
            SqlParameter[] Param = new SqlParameter[15];
            Param[0] = new SqlParameter("@pOrganizationUserMapGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationUsersMap.OrganizationUserMapGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationUsersMap.OrganizationGUID;
            Param[2] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = OrganizationUsersMap.UserGUID;
            Param[3] = new SqlParameter("@pIsContractor", SqlDbType.Bit);
            Param[3].Value = OrganizationUsersMap.IsContractor;
            Param[4] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[4].Value = (object)OrganizationUsersMap.IsActive ?? DBNull.Value;
            Param[5] = new SqlParameter("@pStatus", SqlDbType.SmallInt);
            Param[5].Value = OrganizationUsersMap.Status;
            Param[6] = new SqlParameter("@pHourlyRate", SqlDbType.Float);
            Param[6].Value = (object)OrganizationUsersMap.HourlyRate ?? DBNull.Value;
            Param[7] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[7].Value = (object)OrganizationUsersMap.RegionGUID ?? DBNull.Value;
            Param[8] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[8].Value = (object)OrganizationUsersMap.TerritoryGUID ?? DBNull.Value;
            Param[9] = new SqlParameter("@pUserType", SqlDbType.NVarChar, 10);
            Param[9].Value = (object)OrganizationUsersMap.UserType ?? DBNull.Value;
            Param[10] = new SqlParameter("@pUserSubTypeCode", SqlDbType.NVarChar, 10);
            Param[10].Value = (object)OrganizationUsersMap.UserSubTypeCode ?? DBNull.Value;
            Param[11] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[11].Value = (object)OrganizationUsersMap.CreateDate ?? DBNull.Value;
            Param[12] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)OrganizationUsersMap.CreateBy ?? DBNull.Value;
            Param[13] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[13].Value = (object)OrganizationUsersMap.LastModifiedDate ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[14].Value = (object)OrganizationUsersMap.LastModifiedBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into OrganizationUsersMap(OrganizationUserMapGUID,OrganizationGUID,UserGUID,IsContractor,IsActive,Status,HourlyRate,RegionGUID,"
                                + "TerritoryGUID,UserType,UserSubTypeCode,CreateDate,CreateBy,LastModifiedDate,LastModifiedBy)values(@pOrganizationUserMapGUID,@pOrganizationGUID,@pUserGUID,@pIsContractor,@pIsActive,@pStatus,@pHourlyRate,@pRegionGUID,"
                                + "@pTerritoryGUID,@pUserType,@pUserSubTypeCode,@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy)", Param);
        }

        public int DeleteOrganizationUserMap(Guid OrganizationUserMapGUID)
        {
            //OrganizationUsersMap OrganizationUserMap = context.OrganizationUsersMaps.Find(OrganizationUserMapGUID);
            //if (OrganizationUserMap != null)
            //    context.OrganizationUsersMaps.Remove(OrganizationUserMap);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationUserMapGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationUserMapGUID;
            return context.Database.ExecuteSqlCommand("delete from OrganizationUsersMap where OrganizationUserMapGUID=@pOrganizationUserMapGUID", Param);
        }
        public int DeleteOrganizationUserMapByOrganizationGUID(Guid OrganizationGUID)
        {
            //List<OrganizationUsersMap> OrganizationUserMapList = new List<OrganizationUsersMap>();
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    OrganizationUserMapList = (from p in dataContext.OrganizationUsersMaps
            //                               where p.OrganizationGUID == OrganizationGUID
            //                               select p).ToList();
            //    if (OrganizationUserMapList != null)
            //    {
            //        foreach (OrganizationUsersMap item in OrganizationUserMapList)
            //        {
            //            dataContext.OrganizationUsersMaps.Remove(item);
            //            //dataContext.OrganizationUsersMaps.Attach(item);
            //            //dataContext.Entry(item).State = EntityState.Deleted;
            //            dataContext.SaveChanges();
            //        }
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from OrganizationUsersMap where OrganizationGUID=@pOrganizationGUID", Param);
        }

        public int DeleteOrganizationUserMapByUserGUID(Guid UserGUID)
        {
            //OrganizationUsersMap OrganizationUserMap = new OrganizationUsersMap();
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    OrganizationUserMap = (from p in dataContext.OrganizationUsersMaps
            //                           where p.UserGUID == UserGUID
            //                           select p).FirstOrDefault();
            //    if (OrganizationUserMap != null)
            //    {
            //        dataContext.OrganizationUsersMaps.Remove(OrganizationUserMap);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.ExecuteSqlCommand("delete from OrganizationUsersMap where UserGUID=@pUserGUID", Param);
        }
        public int DeleteOrganization(Guid OrganizationGUID)
        {
            //Organization Organization = context.Organizations.Find(OrganizationGUID);
            //if (Organization != null)
            //    context.Organizations.Remove(Organization);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from Organizations where OrganizationGUID=@pOrganizationGUID", Param);
        }

        public int UpdateOrganization(Organization organization)
        {
            //context.Entry(organization).State = EntityState.Modified;

            SqlParameter[] Param = new SqlParameter[23];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = organization.OrganizationGUID;
            Param[1] = new SqlParameter("@pOrganizationName", SqlDbType.NVarChar, -1);
            Param[1].Value = organization.OrganizationName;
            Param[2] = new SqlParameter("@pOrganizationFullName", SqlDbType.NVarChar, -1);
            Param[2].Value = organization.OrganizationFullName;
            Param[3] = new SqlParameter("@pWebsite", SqlDbType.NVarChar, -1);
            Param[3].Value = (object)organization.Website ?? DBNull.Value;
            Param[4] = new SqlParameter("@pPhone", SqlDbType.NVarChar, -1);
            Param[4].Value = (object)organization.Phone ?? DBNull.Value;
            Param[5] = new SqlParameter("@pTimeZone", SqlDbType.Float);
            Param[5].Value = (object)organization.TimeZone ?? DBNull.Value;
            Param[6] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[6].Value = (object)organization.Latitude ?? DBNull.Value;
            Param[7] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[7].Value = (object)organization.Longitude ?? DBNull.Value;
            Param[8] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, -1);
            Param[8].Value = (object)organization.AddressLine1 ?? DBNull.Value;
            Param[9] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, -1);
            Param[9].Value = (object)organization.AddressLine2 ?? DBNull.Value;
            Param[10] = new SqlParameter("@pCity", SqlDbType.NVarChar, -1);
            Param[10].Value = organization.City;
            Param[11] = new SqlParameter("@pState", SqlDbType.NVarChar, -1);
            Param[11].Value = organization.State;
            Param[12] = new SqlParameter("@pCountry", SqlDbType.NVarChar, -1);
            Param[12].Value = organization.Country;
            Param[13] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, -1);
            Param[13].Value = organization.ZipCode;
            Param[14] = new SqlParameter("@pEmailID", SqlDbType.NVarChar, -1);
            Param[14].Value = organization.EmailID;
            Param[15] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[15].Value = (object)organization.IsActive ?? DBNull.Value;
            Param[16] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[16].Value = (object)organization.IsDeleted ?? DBNull.Value;
            Param[17] = new SqlParameter("@pAllowContractors", SqlDbType.Bit);
            Param[17].Value = (object)organization.AllowContractors ?? DBNull.Value;
            Param[18] = new SqlParameter("@pImageURL", SqlDbType.NVarChar, -1);
            Param[18].Value = (object)organization.ImageURL ?? DBNull.Value;
            Param[19] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[19].Value = (object)organization.CreateDate ?? DBNull.Value;
            Param[20] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[20].Value = (object)organization.CreateBy ?? DBNull.Value;
            Param[21] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[21].Value = (object)organization.LastModifiedDate ?? DBNull.Value;
            Param[22] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[22].Value = (object)organization.LastModifiedBy ?? DBNull.Value;


            return context.Database.ExecuteSqlCommand("update Organizations set OrganizationName=@pOrganizationName,"
                                + "OrganizationFullName=@pOrganizationFullName,Website=@pWebsite,Phone=@pPhone,TimeZone=@pTimeZone,Latitude=@pLatitude,Longitude=@pLongitude,"
                                + "AddressLine1=@pAddressLine1,AddressLine2=@pAddressLine2,City=@pCity,State=@pState,Country=@pCountry,"
                                + "ZipCode=@pZipCode,EmailID=@pEmailID,IsActive=@pIsActive,IsDeleted=@pIsDeleted,AllowContractors=@pAllowContractors,"
                                + "ImageURL=@pImageURL,CreateDate=@pCreateDate,CreateBy=@pCreateBy,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy where OrganizationGUID=@pOrganizationGUID", Param);

        }
        public int UpdateOrganizationUserMap(OrganizationUsersMap OrganizationUsersMap)
        {
            //context.Entry(organizationUserMap).State = EntityState.Modified;
            SqlParameter[] Param = new SqlParameter[15];
            Param[0] = new SqlParameter("@pOrganizationUserMapGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationUsersMap.OrganizationUserMapGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationUsersMap.OrganizationGUID;
            Param[2] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = OrganizationUsersMap.UserGUID;
            Param[3] = new SqlParameter("@pIsContractor", SqlDbType.Bit);
            Param[3].Value = OrganizationUsersMap.IsContractor;
            Param[4] = new SqlParameter("@pIsActive", SqlDbType.Bit);
            Param[4].Value = (object)OrganizationUsersMap.IsActive ?? DBNull.Value;
            Param[5] = new SqlParameter("@pStatus", SqlDbType.SmallInt);
            Param[5].Value = OrganizationUsersMap.Status;
            Param[6] = new SqlParameter("@pHourlyRate", SqlDbType.Float);
            Param[6].Value = (object)OrganizationUsersMap.HourlyRate ?? DBNull.Value;
            Param[7] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[7].Value = (object)OrganizationUsersMap.RegionGUID ?? DBNull.Value;
            Param[8] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[8].Value = (object)OrganizationUsersMap.TerritoryGUID ?? DBNull.Value;
            Param[9] = new SqlParameter("@pUserType", SqlDbType.NVarChar, 10);
            Param[9].Value = (object)OrganizationUsersMap.UserType ?? DBNull.Value;
            Param[10] = new SqlParameter("@pUserSubTypeCode", SqlDbType.NVarChar, 10);
            Param[10].Value = (object)OrganizationUsersMap.UserSubTypeCode ?? DBNull.Value;
            Param[11] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[11].Value = (object)OrganizationUsersMap.CreateDate ?? DBNull.Value;
            Param[12] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)OrganizationUsersMap.CreateBy ?? DBNull.Value;
            Param[13] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[13].Value = (object)OrganizationUsersMap.LastModifiedDate ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[14].Value = (object)OrganizationUsersMap.LastModifiedBy ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("Update OrganizationUsersMap set OrganizationGUID=@pOrganizationGUID,"
                                + "UserGUID=@pUserGUID,IsContractor=@pIsContractor,IsActive=@pIsActive,Status=@pStatus,HourlyRate=@pHourlyRate,RegionGUID=@pRegionGUID,"
                                + "TerritoryGUID=@pTerritoryGUID,UserType=@pUserType,UserSubTypeCode=@pUserSubTypeCode,"
                                + "CreateDate=@pCreateDate,CreateBy=@pCreateBy,LastModifiedDate=@pLastModifiedDate,LastModifiedBy=@pLastModifiedBy where OrganizationUserMapGUID=@pOrganizationUserMapGUID", Param);

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