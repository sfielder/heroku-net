using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.DataAccess.Repository
{
    public class PORepository : IPORepository
    {
        private WorkersInMotionDB context;

        public PORepository(WorkersInMotionDB context)
        {
            this.context = context;
        }
        public PORepository()
        {
            WorkersInMotionDB context = new WorkersInMotionDB();
            this.context = context;
        }
        public IEnumerable<POs> GetPOList(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{

            //    return (from p in dataContext.POs
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).ToList();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;

            return context.Database.SqlQuery<POs>("Select * from POs where OrganizationGUID=@pOrganizationGUID", Param);

        }
        public IEnumerable<POs> GetPOListByPlaceID(Guid OrganizationGUID, string PlaceID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{

            //    return (from p in dataContext.POs
            //            where p.OrganizationGUID == OrganizationGUID && p.PlaceID == PlaceID
            //            select p).ToList();
            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            Param[1] = new SqlParameter("@pPlaceID", SqlDbType.NVarChar, 50);
            Param[1].Value = PlaceID;
            return context.Database.SqlQuery<POs>("Select * from POs where  OrganizationGUID=@pOrganizationGUID and PlaceID=@pPlaceID", Param);
        }

        public POs GetPObyPoNumber(string PONumber)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{

            //    return (from p in dataContext.POs
            //            where p.PONumber == PONumber
            //            select p).OrderByDescending(x => x.CreateDate).FirstOrDefault();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pPONumber", SqlDbType.NVarChar, 50);
            Param[0].Value = PONumber;

            return context.Database.SqlQuery<POs>("Select * from POs where PONumber=@pPONumber order by CreateDate desc", Param).FirstOrDefault();
        }
        public int CreatePO(POs NewPO)
        {
            return InsertPO(NewPO);
            //return Save();
        }
        private int InsertPO(POs NewPO)
        {
            //context.POs.Add(NewPO);

            SqlParameter[] Param = new SqlParameter[25];
            Param[0] = new SqlParameter("@pPOGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = NewPO.POGUID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)NewPO.OrganizationGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)NewPO.RegionGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)NewPO.TerritoryGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pPONumber", SqlDbType.NVarChar, 50);
            Param[4].Value = (object)NewPO.PONumber ?? DBNull.Value;
            Param[5] = new SqlParameter("@pStatus", SqlDbType.SmallInt);
            Param[5].Value = (object)NewPO.Status ?? DBNull.Value;
           
            Param[6] = new SqlParameter("@pPlaceID", SqlDbType.NVarChar, 50);
            Param[6].Value = (object)NewPO.PlaceID ?? DBNull.Value;
            //Heroku
            //Param[8] = new SqlParameter("@pLocationType", SqlDbType.SmallInt);
            //Param[8].Value = (object)NewPO.LocationType ?? DBNull.Value;
            Param[7] = new SqlParameter("@pMarketID", SqlDbType.NVarChar, 50);
            Param[7].Value = (object)NewPO.MarketID ?? DBNull.Value;
            Param[8] = new SqlParameter("@pEndCustomerAddress", SqlDbType.NVarChar, -1);
            Param[8].Value = (object)NewPO.EndCustomerAddress ?? DBNull.Value;
            Param[9] = new SqlParameter("@pEndCustomerName", SqlDbType.NVarChar, -1);
            Param[9].Value = (object)NewPO.EndCustomerName ?? DBNull.Value;
            Param[10] = new SqlParameter("@pEndCustomerPhone", SqlDbType.NVarChar, -1);
            Param[10].Value = (object)NewPO.EndCustomerPhone ?? DBNull.Value;
           
           
            Param[11] = new SqlParameter("@pCreateDate", SqlDbType.DateTime);
            Param[11].Value = (object)NewPO.CreateDate ?? DBNull.Value;
            Param[12] = new SqlParameter("@pCreateBy", SqlDbType.UniqueIdentifier);
            Param[12].Value = (object)NewPO.CreateBy ?? DBNull.Value;
            Param[13] = new SqlParameter("@pLastModifiedDate", SqlDbType.DateTime);
            Param[13].Value = (object)NewPO.LastModifiedDate ?? DBNull.Value;
            Param[14] = new SqlParameter("@pLastModifiedBy", SqlDbType.UniqueIdentifier);
            Param[14].Value = (object)NewPO.LastModifiedBy ?? DBNull.Value;
           
            Param[15] = new SqlParameter("@pTerritoryID", SqlDbType.NVarChar, 50);
            Param[15].Value = (object)NewPO.TerritoryID ?? DBNull.Value;
          
            Param[16] = new SqlParameter("@pRMUserID", SqlDbType.NVarChar, 50);
            Param[16].Value = (object)NewPO.RMUserID ?? DBNull.Value;
            Param[17] = new SqlParameter("@pFMUserID", SqlDbType.NVarChar, 50);
            Param[17].Value = (object)NewPO.FMUserID ?? DBNull.Value;
            Param[18] = new SqlParameter("@pPOJobType", SqlDbType.NVarChar, 50);
            Param[18].Value = (object)NewPO.POJobType ?? DBNull.Value;
            Param[19] = new SqlParameter("@pPOJobCode", SqlDbType.NVarChar, 50);
            Param[19].Value = (object)NewPO.POJobCode ?? DBNull.Value;
            Param[20] = new SqlParameter("@pPOJson", SqlDbType.NVarChar, -1);
            Param[20].Value = (object)NewPO.POJson ?? DBNull.Value;
            Param[21] = new SqlParameter("@pInstallerName", SqlDbType.NVarChar, 50);
            Param[21].Value = (object)NewPO.InstallerName ?? DBNull.Value;
            Param[22] = new SqlParameter("@pPOCustomerName", SqlDbType.NVarChar, 50);
            Param[22].Value = (object)NewPO.POCustomerName ?? DBNull.Value;
            Param[23] = new SqlParameter("@pPOCustomerPhone", SqlDbType.NVarChar, 50);
            Param[23].Value = (object)NewPO.POCustomerPhone ?? DBNull.Value;
            Param[24] = new SqlParameter("@pPOCustomerMobile", SqlDbType.NVarChar, 50);
            Param[24].Value = (object)NewPO.POCustomerMobile ?? DBNull.Value;


            return context.Database.ExecuteSqlCommand("insert into POs(POGUID,OrganizationGUID,RegionGUID,TerritoryGUID,PONumber,Status,PlaceID"
                + " ,MarketID,EndCustomerAddress,EndCustomerName,EndCustomerPhone"
                + ",CreateDate,CreateBy,LastModifiedDate,LastModifiedBy,TerritoryID,RMUserID,FMUserID,POJobType"
                + ",POJobCode,POJson,InstallerName,POCustomerName,POCustomerPhone,POCustomerMobile)"
                + "values(@pPOGUID,@pOrganizationGUID,@pRegionGUID,@pTerritoryGUID,@pPONumber"
                + ",@pStatus,@pPlaceID,@pMarketID,@pEndCustomerAddress,@pEndCustomerName,@pEndCustomerPhone"
                + ",@pCreateDate,@pCreateBy,@pLastModifiedDate,@pLastModifiedBy,@pTerritoryID"
                + ",@pRMUserID,@pFMUserID,@pPOJobType,@pPOJobCode,@pPOJson,@pInstallerName,@pPOCustomerName,@pPOCustomerPhone,@pPOCustomerMobile)", Param);
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