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
    public class MarketRepository : IMarketRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public MarketRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }

        public IEnumerable<Market> GetMarketByUserGUID(Guid UserGUID, int entitytype)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.UserGUID == UserGUID && p.EntityType == entitytype
            //            select p).ToList().OrderBy(x => x.MarketName);

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            Param[1] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[1].Value = entitytype;

            return context.Database.SqlQuery<Market>("Select * from Markets where UserGUID=@pUserGUID and EntityType=@pEntityType order by MarketName", Param);

        }
        public Market GetMarketByPrimaryContactGUID(Guid PrimaryContactGUID, int entitytype)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.PrimaryContactGUID == PrimaryContactGUID && p.EntityType == entitytype
            //            select p).FirstOrDefault();

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pPrimaryContactGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = PrimaryContactGUID;
            Param[1] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[1].Value = entitytype;

            return context.Database.SqlQuery<Market>("Select * from Markets where PrimaryContactGUID=@pPrimaryContactGUID and EntityType=@pEntityType", Param).FirstOrDefault();

        }
        public Market GetMarketByID(Guid MarketGUID)
        {
            //return context.Markets.Find(MarketGUID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pMarketGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = MarketGUID;


            return context.Database.SqlQuery<Market>("Select * from Markets where MarketGUID=@pMarketGUID", Param).FirstOrDefault();
        }

        public Market GetMarketByMarketID(Guid OrganizationGUID, string MarketID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.OrganizationGUID == OrganizationGUID && p.MarketID == MarketID
            //            select p).OrderByDescending(x => x.CreateDate).FirstOrDefault();

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            Param[1] = new SqlParameter("@pMarketID", SqlDbType.NVarChar, 50);
            Param[1].Value = MarketID;

            return context.Database.SqlQuery<Market>("Select * from Markets where OrganizationGUID=@pOrganizationGUID and MarketID=@pMarketID", Param).FirstOrDefault();

        }
        public Market GetMarketByCustomerID(Guid OrganizationGUID, string pCustomerID, string pMarketID)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    var query = (from p in dataContext.Places
                //                 where p.OrganizationGUID == OrganizationGUID
                //                 && p.PlaceID == pCustomerID
                //                 select p).FirstOrDefault();
                //    if (query != null && query.PlaceGUID != Guid.Empty)
                //    {
                //        return (from p in dataContext.Markets
                //                where p.OrganizationGUID == OrganizationGUID
                //                && p.OwnerGUID == query.PlaceGUID
                //                && p.MarketID == pMarketID
                //                select p).FirstOrDefault();
                //    }
                //    else
                //    {
                //        return null;
                //    }

                //}
                SqlParameter[] Param = new SqlParameter[3];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = OrganizationGUID;
                Param[1] = new SqlParameter("@pPlaceID", SqlDbType.NVarChar, 50);
                Param[1].Value = pCustomerID;
                Param[2] = new SqlParameter("@pMarketID", SqlDbType.NVarChar, 50);
                Param[2].Value = pMarketID;

                return context.Database.SqlQuery<Market>("Select * from Markets where OrganizationGUID=@pOrganizationGUID and MarketID=@pMarketID and OwnerGUID in(Select top(1)PlaceGUID from Places where PlaceID=@pPlaceID and OrganizationGUID=@pOrganizationGUID)", Param).FirstOrDefault();


            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<Market> GetMarketByOwnerGUID(Guid OwnerGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.OwnerGUID == OwnerGUID
            //            select p).ToList().OrderBy(x => x.MarketName);

            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOwnerGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OwnerGUID;


            return context.Database.SqlQuery<Market>("Select * from Markets where OwnerGUID=@pOwnerGUID order by MarketName", Param);

        }
        public IEnumerable<Market> GetMarketByRegionGUID(Guid RegionGUID, int entitytype)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.RegionGUID == RegionGUID && p.EntityType == entitytype
            //            select p).ToList().OrderBy(x => x.MarketName);

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            Param[1] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[1].Value = entitytype;


            return context.Database.SqlQuery<Market>("Select * from Markets where RegionGUID=@pRegionGUID and EntityType=@pEntityType order by MarketName order by MarketName", Param);

        }

        public IEnumerable<Market> GetMarketByTerritoryGUID(Guid TerritoryGUID, int entitytype)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.TerritoryGUID == TerritoryGUID && p.EntityType == entitytype
            //            select p).ToList().OrderBy(x => x.MarketName);

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = TerritoryGUID;
            Param[1] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[1].Value = entitytype;


            return context.Database.SqlQuery<Market>("Select * from Markets where TerritoryGUID=@pTerritoryGUID and EntityType=@pEntityType order by MarketName", Param);

        }
        public IEnumerable<Market> GetMarketByRegionGUIDandTerritoryGUID(Guid RegionGUID, Guid TerritoryGUID, int entitytype)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.RegionGUID == RegionGUID && p.TerritoryGUID == TerritoryGUID && p.EntityType == entitytype
            //            select p).ToList().OrderBy(x => x.MarketName);

            //}
            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            Param[1] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = TerritoryGUID;
            Param[2] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[2].Value = entitytype;
            return context.Database.SqlQuery<Market>("Select * from Markets where RegionGUID=@pRegionGUID and TerritoryGUID=@pTerritoryGUID and EntityType=@pEntityType order by MarketName", Param);

        }

        public IEnumerable<Market> GetAllMarketByRegionGUIDandTerritoryGUID(Guid RegionGUID, Guid TerritoryGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.RegionGUID == RegionGUID && p.TerritoryGUID == TerritoryGUID
            //            select p).ToList().OrderBy(x => x.MarketName);

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            Param[1] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = TerritoryGUID;

            return context.Database.SqlQuery<Market>("Select * from Markets where RegionGUID=@pRegionGUID and TerritoryGUID=@pTerritoryGUID order by MarketName", Param);

        }

        public IEnumerable<Market> GetMarketByOwnerandRegionGUID(Guid RegionGUID, Guid OwnerGUID, int entityType)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.RegionGUID == RegionGUID && p.OwnerGUID == OwnerGUID && p.EntityType == entityType
            //            select p).ToList().OrderBy(x => x.MarketName);

            //}
            SqlParameter[] Param = new SqlParameter[3];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            Param[1] = new SqlParameter("@pOwnerGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OwnerGUID;
            Param[2] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[2].Value = entityType;
            return context.Database.SqlQuery<Market>("Select * from Markets where RegionGUID=@pRegionGUID and OwnerGUID=@pOwnerGUID and EntityType=@pEntityType order by MarketName", Param);

        }
        public IEnumerable<Market> GetMarketByOrganizationGUID(Guid OrganizationGUID, int entitytype)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Markets
            //            where p.OrganizationGUID == OrganizationGUID && p.EntityType == entitytype
            //            select p).ToList().OrderBy(x => x.MarketName);

            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            Param[1] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[1].Value = entitytype;
            return context.Database.SqlQuery<Market>("Select * from Markets where pOrganizationGUID=@pOrganizationGUID and EntityType=@pEntityType order by MarketName", Param);

        }
        public int CreateMarket(Market NewMarket)
        {
            //context.Markets.Add(NewMarket);
            //return Save();
            return InsertMarket(NewMarket);
        }
        public int InsertMarket(Market Market)
        {
            // context.Markets.Add(Market);
            SqlParameter[] Param = new SqlParameter[39];
            Param[0] = new SqlParameter("@pMarketGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Market.MarketGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)Market.OrganizationGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pMarketID", SqlDbType.NVarChar, 50);
            Param[2].Value = (object)Market.MarketID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pRecordStatus", SqlDbType.Int);
            Param[3].Value = (object)Market.RecordStatus ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIsDefault", SqlDbType.Bit);
            Param[4].Value = (object)Market.IsDefault ?? DBNull.Value;
            Param[5] = new SqlParameter("@pVersion", SqlDbType.Bit);
            Param[5].Value = (object)Market.Version ?? DBNull.Value;
            Param[6] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)Market.UserGUID ?? DBNull.Value;
            Param[7] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[7].Value = (object)Market.EntityType ?? DBNull.Value;
            Param[8] = new SqlParameter("@pOwnerGUID", SqlDbType.UniqueIdentifier);
            Param[8].Value = (object)Market.OwnerGUID ?? DBNull.Value;
            Param[9] = new SqlParameter("@pMarketName", SqlDbType.NVarChar, 128);
            Param[9].Value = (object)Market.MarketName ?? DBNull.Value;
            Param[10] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[10].Value = (object)Market.RegionGUID ?? DBNull.Value;
            Param[11] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[11].Value = (object)Market.TerritoryGUID ?? DBNull.Value;
            Param[12] = new SqlParameter("@pPrimaryContactGUID", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)Market.PrimaryContactGUID ?? DBNull.Value;
            Param[13] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, 50);
            Param[13].Value = (object)Market.FirstName ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLastName", SqlDbType.NVarChar, 50);
            Param[14].Value = (object)Market.LastName ?? DBNull.Value;
            Param[15] = new SqlParameter("@pMobilePhone", SqlDbType.NVarChar, 20);
            Param[15].Value = (object)Market.MobilePhone ?? DBNull.Value;
            Param[16] = new SqlParameter("@pMarketPhone", SqlDbType.NVarChar, 20);
            Param[16].Value = (object)Market.MarketPhone ?? DBNull.Value;
            Param[17] = new SqlParameter("@pHomePhone", SqlDbType.NVarChar, 20);
            Param[17].Value = (object)Market.HomePhone ?? DBNull.Value;
            Param[18] = new SqlParameter("@pEmails", SqlDbType.NVarChar, -1);
            Param[18].Value = (object)Market.Emails ?? DBNull.Value;
            Param[19] = new SqlParameter("@pTimeZone", SqlDbType.NVarChar, 50);
            Param[19].Value = (object)Market.TimeZone ?? DBNull.Value;
            Param[20] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, 256);
            Param[20].Value = (object)Market.AddressLine1 ?? DBNull.Value;
            Param[21] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, 256);
            Param[21].Value = (object)Market.AddressLine2 ?? DBNull.Value;
            Param[22] = new SqlParameter("@pCity", SqlDbType.NVarChar, 128);
            Param[22].Value = (object)Market.City ?? DBNull.Value;
            Param[23] = new SqlParameter("@pState", SqlDbType.NVarChar, 128);
            Param[23].Value = (object)Market.State ?? DBNull.Value;
            Param[24] = new SqlParameter("@pCountry", SqlDbType.NVarChar, 128);
            Param[24].Value = (object)Market.Country ?? DBNull.Value;
            Param[25] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, 20);
            Param[25].Value = (object)Market.ZipCode ?? DBNull.Value;
            Param[26] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[26].Value = (object)Market.Latitude ?? DBNull.Value;
            Param[27] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[27].Value = (object)Market.Longitude ?? DBNull.Value;
            Param[28] = new SqlParameter("@pImageURL", SqlDbType.NVarChar, -1);
            Param[28].Value = (object)Market.ImageURL ?? DBNull.Value;
            Param[29] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[29].Value = (object)Market.CreateDate ?? DBNull.Value;
            Param[30] = new SqlParameter("@pUpdatedDate", SqlDbType.DateTime);
            Param[30].Value = (object)Market.UpdatedDate ?? DBNull.Value;
            Param[31] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[31].Value = (object)Market.IsDeleted ?? DBNull.Value;
            Param[32] = new SqlParameter("@pParentID", SqlDbType.NVarChar, 50);
            Param[32].Value = (object)Market.ParentID ?? DBNull.Value;
            Param[33] = new SqlParameter("@pTeritoryID", SqlDbType.NVarChar, 50);
            Param[33].Value = (object)Market.TeritoryID ?? DBNull.Value;
            Param[34] = new SqlParameter("@pRegionName", SqlDbType.NVarChar, 50);
            Param[34].Value = (object)Market.RegionName ?? DBNull.Value;
            Param[35] = new SqlParameter("@pRMUserID", SqlDbType.NVarChar, 50);
            Param[35].Value = (object)Market.RMUserID ?? DBNull.Value;
            Param[36] = new SqlParameter("@pFMUserID", SqlDbType.NVarChar, 50);
            Param[36].Value = (object)Market.FMUserID ?? DBNull.Value;
            Param[37] = new SqlParameter("@pStoreJSON", SqlDbType.NVarChar, -1);
            Param[37].Value = (object)Market.StoreJSON ?? DBNull.Value;
            Param[38] = new SqlParameter("@pLastStoreVisitedDate", SqlDbType.DateTime);
            Param[38].Value = (object)Market.LastStoreVisitedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into Markets(MarketGUID,OrganizationGUID,MarketID,RecordStatus,IsDefault,Version,UserGUID,"
            + "EntityType,OwnerGUID,MarketName,RegionGUID,TerritoryGUID,PrimaryContactGUID,FirstName,LastName,MobilePhone,MarketPhone,HomePhone,Emails,TimeZone,"
            + "AddressLine1,AddressLine2,City,State,Country,ZipCode,Latitude,Longitude,ImageURL,CreateDate,UpdatedDate,IsDeleted,ParentID,TeritoryID,RegionName,RMUserID,FMUserID,StoreJSON,LastStoreVisitedDate)"
            + "values(@pMarketGUID,@pOrganizationGUID,@pMarketID,@pRecordStatus,@pIsDefault,@pVersion,@pUserGUID,"
            + "@pEntityType,@pOwnerGUID,@pMarketName,@pRegionGUID,@pTerritoryGUID,@pPrimaryContactGUID,@pFirstName,@pLastName,@pMobilePhone,@pMarketPhone,@pHomePhone,@pEmails,@pTimeZone,"
            + "@pAddressLine1,@pAddressLine2,@pCity,@pState,@pCountry,@pZipCode,@pLatitude,@pLongitude,@pImageURL,@pCreateDate,@pUpdatedDate,@pIsDeleted,@pParentID,@pTeritoryID,@pRegionName,@pRMUserID,@pFMUserID,@pStoreJSON,@pLastStoreVisitedDate)", Param);

        }

        public int DeleteMarket(Guid Marketguid)
        {
            //Market Market = context.Markets.Find(Marketguid);
            //if (Market != null)
            //    context.Markets.Remove(Market);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pMarketGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Marketguid;

            return context.Database.ExecuteSqlCommand("delete from Markets where MarketGUID=@pMarketGUID", Param);

        }

        public int DeleteMarketByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Marketlist = (from p in dataContext.Markets
            //                      where p.OrganizationGUID == OrganizationGUID
            //                      select p).ToList();
            //    foreach (var item in Marketlist)
            //    {
            //        dataContext.Markets.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;

            return context.Database.ExecuteSqlCommand("delete from Markets where OrganizationGUID=@pOrganizationGUID", Param);
        }
        public int DeleteMarketByOwnerGUID(Guid OwnerGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Marketlist = (from p in dataContext.Markets
            //                      where p.OwnerGUID == OwnerGUID && p.EntityType == 1
            //                      select p).ToList();
            //    foreach (var item in Marketlist)
            //    {
            //        dataContext.Markets.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pOwnerGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OwnerGUID;
            Param[1] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[1].Value = 1;

            return context.Database.ExecuteSqlCommand("delete from Markets where OwnerGUID=@pOwnerGUID and EntityType=@pEntityType", Param);
        }

        public int DeleteMarketByUserGUID(Guid UserGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Marketlist = (from p in dataContext.Markets
            //                      where p.UserGUID == UserGUID && p.EntityType == 1
            //                      select p).ToList();
            //    foreach (var item in Marketlist)
            //    {
            //        dataContext.Markets.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = UserGUID;
            Param[1] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[1].Value = 1;

            return context.Database.ExecuteSqlCommand("delete from Markets where UserGUID=@pUserGUID and EntityType=@pEntityType", Param);
        }

        public int UpdateStoreVisitedDate(Guid MarketGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var qry = from p in dataContext.Markets where p.MarketGUID == MarketGUID select p;
            //    var item = qry.Single();
            //    item.LastStoreVisitedDate = DateTime.UtcNow;
            //    dataContext.SaveChanges();
            //}
            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pMarketGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = MarketGUID;
            Param[1] = new SqlParameter("@pLastStoreVisitedDate", SqlDbType.DateTime);
            Param[1].Value = DateTime.UtcNow;
            return context.Database.ExecuteSqlCommand("update Markets set  LastStoreVisitedDate=@pLastStoreVisitedDate where MarketGUID=@pMarketGUID", Param);

        }
        public int DeleteMarketByTerritoryGUID(Guid TerritoryGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Marketlist = (from p in dataContext.Markets
            //                      where p.TerritoryGUID == TerritoryGUID
            //                      select p).ToList();
            //    foreach (var item in Marketlist)
            //    {
            //        dataContext.Markets.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = TerritoryGUID;

            return context.Database.ExecuteSqlCommand("delete from Markets where TerritoryGUID=@pTerritoryGUID", Param);

        }
        public int DeleteMarketByRegionGUID(Guid RegionGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Marketlist = (from p in dataContext.Markets
            //                      where p.RegionGUID == RegionGUID
            //                      select p).ToList();
            //    foreach (var item in Marketlist)
            //    {
            //        dataContext.Markets.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;

            return context.Database.ExecuteSqlCommand("delete from Markets where RegionGUID=@pRegionGUID", Param);
        }
        public int UpdateMarket(Market Market)
        {
            // context.Entry(Market).State = EntityState.Modified;

            SqlParameter[] Param = new SqlParameter[39];
            Param[0] = new SqlParameter("@pMarketGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Market.MarketGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)Market.OrganizationGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pMarketID", SqlDbType.NVarChar, 50);
            Param[2].Value = (object)Market.MarketID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pRecordStatus", SqlDbType.Int);
            Param[3].Value = (object)Market.RecordStatus ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIsDefault", SqlDbType.Bit);
            Param[4].Value = (object)Market.IsDefault ?? DBNull.Value;
            Param[5] = new SqlParameter("@pVersion", SqlDbType.Bit);
            Param[5].Value = (object)Market.Version ?? DBNull.Value;
            Param[6] = new SqlParameter("@pUserGUID", SqlDbType.UniqueIdentifier);
            Param[6].Value = (object)Market.UserGUID ?? DBNull.Value;
            Param[7] = new SqlParameter("@pEntityType", SqlDbType.Int);
            Param[7].Value = (object)Market.EntityType ?? DBNull.Value;
            Param[8] = new SqlParameter("@pOwnerGUID", SqlDbType.UniqueIdentifier);
            Param[8].Value = (object)Market.OwnerGUID ?? DBNull.Value;
            Param[9] = new SqlParameter("@pMarketName", SqlDbType.NVarChar, 128);
            Param[9].Value = (object)Market.MarketName ?? DBNull.Value;
            Param[10] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[10].Value = (object)Market.RegionGUID ?? DBNull.Value;
            Param[11] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[11].Value = (object)Market.TerritoryGUID ?? DBNull.Value;
            Param[12] = new SqlParameter("@pPrimaryContactGUID", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)Market.PrimaryContactGUID ?? DBNull.Value;
            Param[13] = new SqlParameter("@pFirstName", SqlDbType.NVarChar, 50);
            Param[13].Value = (object)Market.FirstName ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLastName", SqlDbType.NVarChar, 50);
            Param[14].Value = (object)Market.LastName ?? DBNull.Value;
            Param[15] = new SqlParameter("@pMobilePhone", SqlDbType.NVarChar, 20);
            Param[15].Value = (object)Market.MobilePhone ?? DBNull.Value;
            Param[16] = new SqlParameter("@pMarketPhone", SqlDbType.NVarChar, 20);
            Param[16].Value = (object)Market.MarketPhone ?? DBNull.Value;
            Param[17] = new SqlParameter("@pHomePhone", SqlDbType.NVarChar, 20);
            Param[17].Value = (object)Market.HomePhone ?? DBNull.Value;
            Param[18] = new SqlParameter("@pEmails", SqlDbType.NVarChar, -1);
            Param[18].Value = (object)Market.Emails ?? DBNull.Value;
            Param[19] = new SqlParameter("@pTimeZone", SqlDbType.NVarChar, 50);
            Param[19].Value = (object)Market.TimeZone ?? DBNull.Value;
            Param[20] = new SqlParameter("@pAddressLine1", SqlDbType.NVarChar, 256);
            Param[20].Value = (object)Market.AddressLine1 ?? DBNull.Value;
            Param[21] = new SqlParameter("@pAddressLine2", SqlDbType.NVarChar, 256);
            Param[21].Value = (object)Market.AddressLine2 ?? DBNull.Value;
            Param[22] = new SqlParameter("@pCity", SqlDbType.NVarChar, 128);
            Param[22].Value = (object)Market.City ?? DBNull.Value;
            Param[23] = new SqlParameter("@pState", SqlDbType.NVarChar, 128);
            Param[23].Value = (object)Market.State ?? DBNull.Value;
            Param[24] = new SqlParameter("@pCountry", SqlDbType.NVarChar, 128);
            Param[24].Value = (object)Market.Country ?? DBNull.Value;
            Param[25] = new SqlParameter("@pZipCode", SqlDbType.NVarChar, 20);
            Param[25].Value = (object)Market.ZipCode ?? DBNull.Value;
            Param[26] = new SqlParameter("@pLatitude", SqlDbType.Float);
            Param[26].Value = (object)Market.Latitude ?? DBNull.Value;
            Param[27] = new SqlParameter("@pLongitude", SqlDbType.Float);
            Param[27].Value = (object)Market.Longitude ?? DBNull.Value;
            Param[28] = new SqlParameter("@pImageURL", SqlDbType.NVarChar, -1);
            Param[28].Value = (object)Market.ImageURL ?? DBNull.Value;
            Param[29] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[29].Value = (object)Market.CreateDate ?? DBNull.Value;
            Param[30] = new SqlParameter("@pUpdatedDate", SqlDbType.DateTime);
            Param[30].Value = (object)Market.UpdatedDate ?? DBNull.Value;
            Param[31] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
            Param[31].Value = (object)Market.IsDeleted ?? DBNull.Value;
            Param[32] = new SqlParameter("@pParentID", SqlDbType.NVarChar, 50);
            Param[32].Value = (object)Market.ParentID ?? DBNull.Value;
            Param[33] = new SqlParameter("@pTeritoryID", SqlDbType.NVarChar, 50);
            Param[33].Value = (object)Market.TeritoryID ?? DBNull.Value;
            Param[34] = new SqlParameter("@pRegionName", SqlDbType.NVarChar, 50);
            Param[34].Value = (object)Market.RegionName ?? DBNull.Value;
            Param[35] = new SqlParameter("@pRMUserID", SqlDbType.NVarChar, 50);
            Param[35].Value = (object)Market.RMUserID ?? DBNull.Value;
            Param[36] = new SqlParameter("@pFMUserID", SqlDbType.NVarChar, 50);
            Param[36].Value = (object)Market.FMUserID ?? DBNull.Value;
            Param[37] = new SqlParameter("@pStoreJSON", SqlDbType.NVarChar, -1);
            Param[37].Value = (object)Market.StoreJSON ?? DBNull.Value;
            Param[38] = new SqlParameter("@pLastStoreVisitedDate", SqlDbType.DateTime);
            Param[38].Value = (object)Market.LastStoreVisitedDate ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("Update Markets set OrganizationGUID=@pOrganizationGUID,"
            + "MarketID=@pMarketID,RecordStatus=@pRecordStatus,IsDefault=@pIsDefault,Version=@pVersion,UserGUID=@pUserGUID,"
            + "EntityType=@pEntityType,OwnerGUID=@pOwnerGUID,MarketName=@pMarketName,RegionGUID=@pRegionGUID,TerritoryGUID=@pTerritoryGUID,"
            + "PrimaryContactGUID=@pPrimaryContactGUID,FirstName=@pFirstName,LastName=@pLastName,MobilePhone=@pMobilePhone,MarketPhone=@pMarketPhone,"
            + "HomePhone=@pHomePhone,Emails=@pEmails,TimeZone=@pTimeZone,"
            + "AddressLine1=@pAddressLine1,AddressLine2=@pAddressLine2,City=@pCity,State=@pState,Country=@pCountry,"
            + "ZipCode=@pZipCode,Latitude=@pLatitude,Longitude=@pLongitude,ImageURL=@pImageURL,CreateDate=@pCreateDate,"
            + "UpdatedDate=@pUpdatedDate,IsDeleted=@pIsDeleted,ParentID=@pParentID,TeritoryID=@pTeritoryID,RegionName=@pRegionName,"
            + "RMUserID=@pRMUserID,FMUserID=@pFMUserID,StoreJSON=@pStoreJSON,LastStoreVisitedDate=@pLastStoreVisitedDate where MarketGUID=@pMarketGUID", Param);

        }
        public List<Market> GetStoreNonVisit(Market pMarket)
        {
            try
            {
                DateTime date = pMarket.LastStoreVisitedDate != null ? Convert.ToDateTime(pMarket.LastStoreVisitedDate).ToUniversalTime() : DateTime.UtcNow.AddDays(-45);
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    DateTime date = pMarket.LastStoreVisitedDate != null ? Convert.ToDateTime(pMarket.LastStoreVisitedDate).ToUniversalTime() : DateTime.UtcNow.AddDays(-45);
                //    List<Market> MarketList = (from p in dataContext.Markets
                //                               where
                //                               (pMarket.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pMarket.OrganizationGUID)
                //                               && (pMarket.RegionGUID == null || pMarket.RegionGUID == Guid.Empty || p.RegionGUID == pMarket.RegionGUID)
                //                               && (pMarket.OwnerGUID == null || pMarket.OwnerGUID == Guid.Empty || p.OwnerGUID == pMarket.OwnerGUID)
                //                               && (pMarket.FMUserID == null || pMarket.FMUserID == string.Empty || p.FMUserID == pMarket.FMUserID)
                //                               && (p.IsDeleted == null || p.IsDeleted == false)
                //                               select p).OrderBy(x => x.MarketName).ToList();

                //    if (MarketList != null && MarketList.Count > 0)
                //    {
                //        MarketList = MarketList.Where(p => p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= date.Date)).ToList();
                //        return MarketList;
                //    }
                //    return null;

                //}
                SqlParameter[] Param = new SqlParameter[5];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = (object)pMarket.OrganizationGUID ?? DBNull.Value;
                Param[1] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = (object)pMarket.RegionGUID ?? DBNull.Value;
                Param[2] = new SqlParameter("@pOwnerGUID", SqlDbType.UniqueIdentifier);
                Param[2].Value = (object)pMarket.OwnerGUID ?? DBNull.Value;
                Param[3] = new SqlParameter("@pFMUserID", SqlDbType.NVarChar, 50);
                Param[3].Value = (object)pMarket.FMUserID ?? DBNull.Value;
                Param[4] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
                Param[4].Value = (object)pMarket.IsDeleted ?? DBNull.Value;

                List<Market> MarketList = context.Database.SqlQuery<Market>("select * from Markets where"
                                    + " (OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (OwnerGUID=@pOwnerGUID OR @pOwnerGUID is NULL or @pOwnerGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (FMUserID=@pFMUserID OR @pFMUserID is NULL or @pFMUserID='')"
                                    + " AND (IsDeleted=@pIsDeleted OR @pIsDeleted is NULL)"
                                    + " Order by MarketName", Param).ToList();

                if (MarketList != null && MarketList.Count > 0)
                {
                    MarketList = MarketList.Where(p => p.LastStoreVisitedDate == null || (p.LastStoreVisitedDate != null && Convert.ToDateTime(p.LastStoreVisitedDate.Value.Date).Date <= date.Date)).ToList();
                    return MarketList;
                }
                return null;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public List<Market> GetAllStores(Market pMarket)
        {
            try
            {
                //using (var dataContext = new WorkersInMotionDB())
                //{
                //    return (from p in dataContext.Markets
                //            where
                //            (pMarket.OrganizationGUID == Guid.Empty || p.OrganizationGUID == pMarket.OrganizationGUID)
                //            && (pMarket.RegionGUID == null || pMarket.RegionGUID == Guid.Empty || p.RegionGUID == pMarket.RegionGUID)
                //            && (pMarket.OwnerGUID == null || pMarket.OwnerGUID == Guid.Empty || p.OwnerGUID == pMarket.OwnerGUID)
                //            && (pMarket.FMUserID == null || pMarket.FMUserID == string.Empty || p.FMUserID == pMarket.FMUserID)
                //            && (p.IsDeleted == null || p.IsDeleted == false)
                //            select p).OrderBy(x => x.MarketName).ToList();


                //}
                SqlParameter[] Param = new SqlParameter[5];
                Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
                Param[0].Value = (object)pMarket.OrganizationGUID ?? DBNull.Value;
                Param[1] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
                Param[1].Value = (object)pMarket.RegionGUID ?? DBNull.Value;
                Param[2] = new SqlParameter("@pOwnerGUID", SqlDbType.UniqueIdentifier);
                Param[2].Value = (object)pMarket.OwnerGUID ?? DBNull.Value;
                Param[3] = new SqlParameter("@pFMUserID", SqlDbType.NVarChar, 50);
                Param[3].Value = (object)pMarket.FMUserID ?? DBNull.Value;
                Param[4] = new SqlParameter("@pIsDeleted", SqlDbType.Bit);
                Param[4].Value = (object)pMarket.IsDeleted ?? DBNull.Value;

                return context.Database.SqlQuery<Market>("select * from Markets where"
                                    + " (OrganizationGUID=@pOrganizationGUID OR @pOrganizationGUID is NULL or @pOrganizationGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (RegionGUID=@pRegionGUID OR @pRegionGUID is NULL or @pRegionGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (OwnerGUID=@pOwnerGUID OR @pOwnerGUID is NULL or @pOwnerGUID=(cast(cast(0 as binary) as uniqueidentifier)))"
                                    + " AND (FMUserID=@pFMUserID OR @pFMUserID is NULL or @pFMUserID='')"
                                    + " AND (IsDeleted=@pIsDeleted OR @pIsDeleted is NULL)"
                                    + " Order by MarketName", Param).ToList();
            }
            catch (Exception exception)
            {
                throw exception;
            }
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