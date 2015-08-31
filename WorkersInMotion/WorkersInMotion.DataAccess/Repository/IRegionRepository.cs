using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IRegionRepository : IDisposable
    {
        IEnumerable<Region> GetRegions();
        Region GetRegionByID(Guid RegionGUID);
        Region GetRegionByRegionID(string RegionID);
        Region GetRegionByRegionID(string RegionID, Guid OrganizationGUID);
        IEnumerable<Region> GetRegionByOrganizationGUID(Guid OrganizationGUID);
        string GetRegionNameByRegionGUID(Guid RegionGUID);
        int InsertRegion(Region Region);
        int DeleteRegion(Guid RegionGUID);
        int DeleteRegionByOrganizationGUID(Guid OrganizationGUID);
        int UpdateRegion(Region Region);
        //int Save();
    }
}