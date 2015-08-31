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
    public class RegionRepository : IRegionRepository, IDisposable
    {
        private WorkersInMotionDB context;

        public RegionRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }

        public IEnumerable<Region> GetRegions()
        {
            //var Region = context.Regions.ToList();
            //return context.Regions.ToList().OrderBy(x => x.Name);

            return context.Database.SqlQuery<Region>("select * from  Regions order by Name");
        }

        public Region GetRegionByID(Guid RegionGUID)
        {
            //return context.Regions.Find(RegionGUID);

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;

            return context.Database.SqlQuery<Region>("Select * from Regions where RegionGUID=@pRegionGUID", Param).FirstOrDefault();
        }

        public Region GetRegionByRegionID(string RegionID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Regions
            //            where p.REGIONID == RegionID
            //            select p).FirstOrDefault();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRegionID", SqlDbType.NVarChar, 50);
            Param[0].Value = RegionID;

            return context.Database.SqlQuery<Region>("Select * from Regions where RegionID=@pRegionID", Param).FirstOrDefault();
        }
        public Region GetRegionByRegionID(string RegionID, Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Regions
            //            where p.REGIONID == RegionID && p.OrganizationGUID == OrganizationGUID
            //            select p).FirstOrDefault();
            //}

            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pRegionID", SqlDbType.NVarChar, 50);
            Param[0].Value = RegionID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationGUID;
            return context.Database.SqlQuery<Region>("Select * from Regions where RegionID=@pRegionID and  OrganizationGUID=@pOrganizationGUID", Param).FirstOrDefault();
        }

        public IEnumerable<Region> GetRegionByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Regions
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).ToList().OrderBy(x => x.Name);
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.SqlQuery<Region>("select * from  Regions where OrganizationGUID=@pOrganizationGUID order by Name", Param);
        }
        public string GetRegionNameByRegionGUID(Guid RegionGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Regions
            //            where p.RegionGUID == RegionGUID
            //            select p.Name).SingleOrDefault();
            //}


            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            Region uRegion = context.Database.SqlQuery<Region>("Select * from Regions where RegionGUID=@pRegionGUID", Param).SingleOrDefault();
            if (uRegion != null)
                return uRegion.Name;
            else
                return string.Empty;
        }




        public int InsertRegion(Region Region)
        {
            //context.Regions.Add(Region);

            SqlParameter[] Param = new SqlParameter[6];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Region.RegionGUID;
            Param[1] = new SqlParameter("@pName", SqlDbType.NVarChar, 50);
            Param[1].Value = (object)Region.Name ?? DBNull.Value;
            Param[2] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)Region.OrganizationGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pIsDefault", SqlDbType.Bit);
            Param[3].Value = (object)Region.IsDefault ?? DBNull.Value;
            Param[4] = new SqlParameter("@pDescription", SqlDbType.NVarChar, 256);
            Param[4].Value = (object)Region.Description ?? DBNull.Value;
            Param[5] = new SqlParameter("@pREGIONID", SqlDbType.NVarChar, 50);
            Param[5].Value = (object)Region.REGIONID ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into Regions(RegionGUID,Name,OrganizationGUID,IsDefault,Description,REGIONID)"
            + "values(@pRegionGUID,@pName,@pOrganizationGUID,@pIsDefault,@pDescription,@pREGIONID)", Param);
        }

        public int DeleteRegion(Guid RegionGUID)
        {
            //Region region = context.Regions.Find(RegionGUID);
            //if (region != null)
            //    context.Regions.Remove(region);


            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            return context.Database.ExecuteSqlCommand("delete from Regions where RegionGUID=@pRegionGUID", Param);
        }

        public int DeleteRegionByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var RegionList = (from p in dataContext.Regions
            //                      where p.OrganizationGUID == OrganizationGUID
            //                      select p).ToList();
            //    foreach (var item in RegionList)
            //    {
            //        dataContext.Regions.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from Regions where OrganizationGUID=@pOrganizationGUID", Param);
        }

        public int UpdateRegion(Region Region)
        {
            // context.Entry(Region).State = EntityState.Modified;
            SqlParameter[] Param = new SqlParameter[6];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Region.RegionGUID;
            Param[1] = new SqlParameter("@pName", SqlDbType.NVarChar, 50);
            Param[1].Value = (object)Region.Name ?? DBNull.Value;
            Param[2] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[2].Value = (object)Region.OrganizationGUID ?? DBNull.Value;
            Param[3] = new SqlParameter("@pIsDefault", SqlDbType.Bit);
            Param[3].Value = (object)Region.IsDefault ?? DBNull.Value;
            Param[4] = new SqlParameter("@pDescription", SqlDbType.NVarChar, 256);
            Param[4].Value = (object)Region.Description ?? DBNull.Value;
            Param[5] = new SqlParameter("@pREGIONID", SqlDbType.NVarChar, 50);
            Param[5].Value = (object)Region.REGIONID ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("update Regions set Name=@pName,OrganizationGUID=@pOrganizationGUID,"
                    + "IsDefault=@pIsDefault,Description=@pDescription,REGIONID=@pREGIONID where RegionGUID=@pRegionGUID", Param);
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