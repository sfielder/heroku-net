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
    public class TerritoryRepository : ITerritoryRepository, IDisposable
    {

        private WorkersInMotionDB context;

        public TerritoryRepository(WorkersInMotionDB context)
        {
            this.context = context;
        }
        public IEnumerable<Territory> GetTerritory()
        {
            //var Territory = context.Territories.ToList();
            //return context.Territories.ToList().OrderBy(x => x.Name);

            return context.Database.SqlQuery<Territory>("Select * from  Territories order by Name");
        }

        public IEnumerable<Territory> GetTerritoryByRegionGUID(Guid RegionGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Territories
            //            where p.RegionGUID == RegionGUID
            //            select p).ToList().OrderBy(x => x.Name);
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;

            return context.Database.SqlQuery<Territory>("Select * from Territories where RegionGUID=@pRegionGUID order by Name", Param);


        }
        public Territory GetTerritoryByTerritoryID(string TerritoryID, Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Territories
            //            where p.TerritoryID == TerritoryID && p.OrganizationGUID == OrganizationGUID
            //            select p).FirstOrDefault();
            //}


            SqlParameter[] Param = new SqlParameter[2];
            Param[0] = new SqlParameter("@pTerritoryID", SqlDbType.NVarChar, 50);
            Param[0].Value = TerritoryID;
            Param[1] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = OrganizationGUID;

            return context.Database.SqlQuery<Territory>("Select * from Territories where TerritoryID=@pTerritoryID and OrganizationGUID=@pOrganizationGUID", Param).FirstOrDefault();
        }


        public string GetTerritoryNameByTerritoryGUID(Guid TerritoryGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Territories
            //            where p.TerritoryGUID == TerritoryGUID
            //            select p.Name).SingleOrDefault();
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = TerritoryGUID;

            Territory uTerritory = context.Database.SqlQuery<Territory>("Select * from Territories where TerritoryGUID=@pTerritoryGUID", Param).FirstOrDefault();
            if (uTerritory != null)
                return uTerritory.Name;
            else
                return string.Empty;
        }


        public IEnumerable<Territory> GetTerritoryByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    return (from p in dataContext.Territories
            //            where p.OrganizationGUID == OrganizationGUID
            //            select p).ToList().OrderBy(x => x.Name);

            //}


            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;

            return context.Database.SqlQuery<Territory>("Select * from Territories where OrganizationGUID=@pOrganizationGUID order by Name", Param);
        }

        public Territory GetTerritoryByID(Guid TerritoryGUID)
        {
            //return context.Territories.Find(TerritoryGUID);
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = TerritoryGUID;
            return context.Database.SqlQuery<Territory>("Select * from Territories where TerritoryGUID=@pTerritoryGUID", Param).FirstOrDefault();
        }

        public int InsertTerritory(Territory Territory)
        {
            //context.Territories.Add(Territory);

            SqlParameter[] Param = new SqlParameter[7];
            Param[0] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Territory.TerritoryGUID;
            Param[1] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)Territory.RegionGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pName", SqlDbType.NVarChar, 50);
            Param[2].Value = (object)Territory.Name ?? DBNull.Value;
            Param[3] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)Territory.OrganizationGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIsDefault", SqlDbType.Bit);
            Param[4].Value = (object)Territory.IsDefault ?? DBNull.Value;
            Param[5] = new SqlParameter("@pDescription", SqlDbType.NVarChar, 256);
            Param[5].Value = (object)Territory.Description ?? DBNull.Value;
            Param[6] = new SqlParameter("@pTerritoryID", SqlDbType.NVarChar, 256);
            Param[6].Value = (object)Territory.TerritoryID ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("insert into Territories(TerritoryGUID,RegionGUID,Name,OrganizationGUID,IsDefault,Description,TerritoryID)"
            + "values(@pTerritoryGUID,@pRegionGUID,@pName,@pOrganizationGUID,@pIsDefault,@pDescription,@pTerritoryID)", Param);
        }

        public int DeleteTerritory(Guid TerritoryGUID)
        {
            //Territory Territory = context.Territories.Find(TerritoryGUID);
            //if (Territory != null)
            //    context.Territories.Remove(Territory);


            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = TerritoryGUID;
            return context.Database.ExecuteSqlCommand("delete from Territories where TerritoryGUID=@pTerritoryGUID", Param);
        }

        public int DeleteTerritoryByRegionGUID(Guid RegionGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Territorylist = (from p in dataContext.Territories
            //                         where p.RegionGUID == RegionGUID
            //                         select p).ToList();
            //    foreach (var item in Territorylist)
            //    {
            //        dataContext.Territories.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}

            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = RegionGUID;
            return context.Database.ExecuteSqlCommand("delete from Territories where RegionGUID=@pRegionGUID", Param);
        }


        public int DeleteTerritoryByOrganizationGUID(Guid OrganizationGUID)
        {
            //using (var dataContext = new WorkersInMotionDB())
            //{
            //    var Territorylist = (from p in dataContext.Territories
            //                         where p.OrganizationGUID == OrganizationGUID
            //                         select p).ToList();
            //    foreach (var item in Territorylist)
            //    {
            //        dataContext.Territories.Remove(item);
            //        dataContext.SaveChanges();
            //    }
            //}
            SqlParameter[] Param = new SqlParameter[1];
            Param[0] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = OrganizationGUID;
            return context.Database.ExecuteSqlCommand("delete from Territories where OrganizationGUID=@pOrganizationGUID", Param);
        }

        public int UpdateTerritory(Territory Territory)
        {
            // context.Entry(Territory).State = EntityState.Modified;
            SqlParameter[] Param = new SqlParameter[7];
            Param[0] = new SqlParameter("@pTerritoryGUID", SqlDbType.UniqueIdentifier);
            Param[0].Value = Territory.TerritoryGUID;
            Param[1] = new SqlParameter("@pRegionGUID", SqlDbType.UniqueIdentifier);
            Param[1].Value = (object)Territory.RegionGUID ?? DBNull.Value;
            Param[2] = new SqlParameter("@pName", SqlDbType.NVarChar, 50);
            Param[2].Value = (object)Territory.Name ?? DBNull.Value;
            Param[3] = new SqlParameter("@pOrganizationGUID", SqlDbType.UniqueIdentifier);
            Param[3].Value = (object)Territory.OrganizationGUID ?? DBNull.Value;
            Param[4] = new SqlParameter("@pIsDefault", SqlDbType.Bit);
            Param[4].Value = (object)Territory.IsDefault ?? DBNull.Value;
            Param[5] = new SqlParameter("@pDescription", SqlDbType.NVarChar, 256);
            Param[5].Value = (object)Territory.Description ?? DBNull.Value;
            Param[6] = new SqlParameter("@pTerritoryID", SqlDbType.NVarChar, 256);
            Param[6].Value = (object)Territory.TerritoryID ?? DBNull.Value;

            return context.Database.ExecuteSqlCommand("update Territories set RegionGUID=@pRegionGUID,Name=@pName,"
            + "OrganizationGUID=@pOrganizationGUID,IsDefault=@pIsDefault,Description=@pDescription,TerritoryID=@pTerritoryID where TerritoryGUID=@pTerritoryGUID", Param);
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