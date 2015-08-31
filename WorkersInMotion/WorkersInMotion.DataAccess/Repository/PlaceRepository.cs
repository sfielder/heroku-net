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
    public class PlaceRepository : IPlaceRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public PlaceRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }

        public IEnumerable<Place> GetPlaceByUserGUID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Places
            //            where p.UserGUID == UserGUID
            //            select p).ToList().OrderBy(x => x.PlaceName);

            //}


            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            return context.Database.SqlQuery<Place>("select * from  Places where UserGUID=@pUserGUID order by PlaceName", Param);
        }


        public Place GetPlaceByID(Guid PlaceGUID)
        {
            // return context.Places.Find(PlaceGUID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = PlaceGUID;

            return context.Database.SqlQuery<Place>("Select * from Places where PlaceGUID=@pPlaceGUID", Param).FirstOrDefault();
        }
        public Place GetPlaceByID(string PlaceID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Places
            //            where p.PlaceID == PlaceID
            //            select p).FirstOrDefault();

            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pPlaceID", SqlDbType.NVarChar, 50);
            Param[0].Value = PlaceID;

            return context.Database.SqlQuery<Place>("Select * from Places where PlaceID=@pPlaceID", Param).FirstOrDefault();
        }
        public Place GetPlaceByID(string PlaceID, Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Places
            //            where p.PlaceID == PlaceID && p.OrganizationGUID == OrganizationGUID
            //            select p).FirstOrDefault();

            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pPlaceID", SqlDbType.NVarChar, 50);
            Param[0].Value = PlaceID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationGUID;
            return context.Database.SqlQuery<Place>("Select * from Places where PlaceID=@pPlaceID and OrganizationGUID=@pOrganizationGUID", Param).FirstOrDefault();
        }
        public IEnumerable<Place> GetPlaceByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Places
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).ToList().OrderBy(x => x.PlaceName);

            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<Place>("select * from  Places where OrganizationGUID=@pOrganizationGUID order by PlaceName", Param);

        }

        public int InsertPlace(Place place)
        {
            //context.Places.Add(place);
            SqlParameter[] Param = new SqlParameter[23];
            Param[0] = new SqlParameter("@pPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = place.PlaceGUID;
            Param[1] = new SqlParameter("@pPlaceID", SqlDbType.NVarChar, 50);
            Param[1].Value = (object)place.PlaceID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = place.UserGUID;
            Param[3] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)place.OrganizationGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pPlaceName", SqlDbType.NVarChar, 50);
            Param[4].Value = (object)place.PlaceName ?? DBNull.Value;
            Param[5] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, 50);
            Param[5].Value = (object)place.FirstName ?? DBNull.Value;
            Param[6] = new SqlParameter("@pLastName", SqlDbType.NVarChar, 50);
            Param[6].Value = (object)place.LastName ?? DBNull.Value;
            Param[7] = new SqlParameter("@pMobilePhone", SqlDbType.NVarChar, 20);
            Param[7].Value = (object)place.MobilePhone ?? DBNull.Value;
            Param[8] = new SqlParameter("@pPlacePhone", SqlDbType.NVarChar, 20);
            Param[8].Value = (object)place.PlacePhone ?? DBNull.Value;
            Param[9] = new SqlParameter("@pHomePhone", SqlDbType.NVarChar, 20);
            Param[9].Value = (object)place.HomePhone ?? DBNull.Value;
            Param[10] = new SqlParameter("@pEmails", SqlDbType.NVarChar, -1);
            Param[10].Value = (object)place.Emails ?? DBNull.Value;
            Param[11] = new SqlParameter("@pTimeZone", SqlDbType.NVarChar, 20);
            Param[11].Value = (object)place.TimeZone ?? DBNull.Value;
            Param[12] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, 256);
            Param[12].Value = (object)place.AddressLine1 ?? DBNull.Value;
            Param[13] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, 256);
            Param[13].Value = (object)place.AddressLine2 ?? DBNull.Value;
            Param[14] = new SqlParameter("@pCity", SqlDbType.NVarChar, 128);
            Param[14].Value = (object)place.City ?? DBNull.Value;
            Param[15] = new SqlParameter("@pState", SqlDbType.NVarChar, 128);
            Param[15].Value = (object)place.State ?? DBNull.Value;
            Param[16] = new SqlParameter("@pCountry", SqlDbType.NVarChar, 128);
            Param[16].Value = (object)place.Country ?? DBNull.Value;
            Param[17] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, 20);
            Param[17].Value = (object)place.ZipCode ?? DBNull.Value;
            Param[18] = new SqlParameter("@pCategoryID", SqlDbType.Int);
            Param[18].Value = (object)place.CategoryID ?? DBNull.Value;
            Param[19] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[19].Value = (object)place.IsDeleted ?? DBNull.Value;
            Param[20] = new SqlParameter("@pImageURL", SqlDbType.NVarChar, -1);
            Param[20].Value = (object)place.ImageURL ?? DBNull.Value;
            Param[21] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[21].Value = (object)place.CreateDate ?? DBNull.Value;
            Param[22] = new SqlParameter("@pUpdatedDate", SqlDbType.DateTime);
            Param[22].Value = (object)place.UpdatedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into Places(PlaceGUID,PlaceID,UserGUID,OrganizationGUID,PlaceName,FirstName,LastName,MobilePhone,"
                    + "PlacePhone,HomePhone,Emails,TimeZone,AddressLine1,AddressLine2,City,State,Country,ZipCode,CategoryID,IsDeleted,ImageURL,CreateDate,UpdatedDate)values"
                    + "(@pPlaceGUID,@pPlaceID,@pUserGUID,@pOrganizationGUID,@pPlaceName,@pFirstName,@pLastName,@pMobilePhone,@pPlacePhone,@pHomePhone,@pEmails,@pTimeZone,"
                    + " @pAddressLine1,@pAddressLine2,@pCity,@pState,@pCountry,@pZipCode,@pCategoryID,@pIsDeleted,@pImageURL,@pCreateDate,@pUpdatedDate)", Param);
        }

        public int DeletePlace(Guid placeguid)
        {
            //Place Place = context.Places.Find(placeguid);
            //if (Place != null)
            //    context.Places.Remove(Place);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pplaceguid", SqlDbType.UniqueIdentifier);
            Param[0].Value = placeguid;
            return context.Database.ExecuteSqlCommand("delete from Places where PlaceGUID=@pplaceguid", Param);
        }

        public int DeletePlaceByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Placelist = (from p in dataContext.Places
            //                     where p.OrganizationGUID == OrganizationGUID
            //                     select p).ToList();
            //    foreach (var item in Placelist)
            //    {
            //        dataContext.Places.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from Places where OrganizationGUID=@pOrganizationGUID", Param);

        }

        public int UpdatePlace(Place place)
        {
            //context.Entry(place).State = EntityState.Modified;
            SqlParameter[] Param = new SqlParameter[23];
            Param[0] = new SqlParameter("@pPlaceGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = place.PlaceGUID;
            Param[1] = new SqlParameter("@pPlaceID", SqlDbType.NVarChar, 50);
            Param[1].Value = (object)place.PlaceID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = place.UserGUID;
            Param[3] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)place.OrganizationGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pPlaceName", SqlDbType.NVarChar, 50);
            Param[4].Value = (object)place.PlaceName ?? DBNull.Value;
            Param[5] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, 50);
            Param[5].Value = (object)place.FirstName ?? DBNull.Value;
            Param[6] = new SqlParameter("@pLastName", SqlDbType.NVarChar, 50);
            Param[6].Value = (object)place.LastName ?? DBNull.Value;
            Param[7] = new SqlParameter("@pMobilePhone", SqlDbType.NVarChar, 20);
            Param[7].Value = (object)place.MobilePhone ?? DBNull.Value;
            Param[8] = new SqlParameter("@pPlacePhone", SqlDbType.NVarChar, 20);
            Param[8].Value = (object)place.PlacePhone ?? DBNull.Value;
            Param[9] = new SqlParameter("@pHomePhone", SqlDbType.NVarChar, 20);
            Param[9].Value = (object)place.HomePhone ?? DBNull.Value;
            Param[10] = new SqlParameter("@pEmails", SqlDbType.NVarChar, -1);
            Param[10].Value = (object)place.Emails ?? DBNull.Value;
            Param[11] = new SqlParameter("@pTimeZone", SqlDbType.NVarChar, 20);
            Param[11].Value = (object)place.TimeZone ?? DBNull.Value;
            Param[12] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, 256);
            Param[12].Value = (object)place.AddressLine1 ?? DBNull.Value;
            Param[13] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, 256);
            Param[13].Value = (object)place.AddressLine2 ?? DBNull.Value;
            Param[14] = new SqlParameter("@pCity", SqlDbType.NVarChar, 128);
            Param[14].Value = (object)place.City ?? DBNull.Value;
            Param[15] = new SqlParameter("@pState", SqlDbType.NVarChar, 128);
            Param[15].Value = (object)place.State ?? DBNull.Value;
            Param[16] = new SqlParameter("@pCountry", SqlDbType.NVarChar, 128);
            Param[16].Value = (object)place.Country ?? DBNull.Value;
            Param[17] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, 20);
            Param[17].Value = (object)place.ZipCode ?? DBNull.Value;
            Param[18] = new SqlParameter("@pCategoryID", SqlDbType.Int);
            Param[18].Value = (object)place.CategoryID ?? DBNull.Value;
            Param[19] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[19].Value = (object)place.IsDeleted ?? DBNull.Value;
            Param[20] = new SqlParameter("@pImageURL", SqlDbType.NVarChar, -1);
            Param[20].Value = (object)place.ImageURL ?? DBNull.Value;
            Param[21] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[21].Value = (object)place.CreateDate ?? DBNull.Value;
            Param[22] = new SqlParameter("@pUpdatedDate", SqlDbType.DateTime);
            Param[22].Value = (object)place.UpdatedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("update Places set PlaceID=@pPlaceID,UserGUID=@pUserGUID,OrganizationGUID=@pOrganizationGUID,"
                     + "PlaceName=@pPlaceName,FirstName=@pFirstName,LastName=@pLastName,MobilePhone=@pMobilePhone,"
                     + "PlacePhone=@pPlacePhone,HomePhone=@pHomePhone,Emails=@pEmails,TimeZone=@pTimeZone,"
                     + "AddressLine1=@pAddressLine1,AddressLine2=@pAddressLine2,City=@pCity,State=@pState,Country=@pCountry,ZipCode=@pZipCode,"
                     + "CategoryID=@pCategoryID,IsDeleted=@pIsDeleted,ImageURL=@pImageURL,CreateDate=@pCreateDate,UpdatedDate=@pUpdatedDate where PlaceGUID=@pPlaceGUID", Param);
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