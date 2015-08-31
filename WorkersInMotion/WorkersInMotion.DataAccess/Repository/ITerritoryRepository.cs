using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface ITerritoryRepository : IDisposable
    {

        IEnumerable<Territory> GetTerritory();
        IEnumerable<Territory> GetTerritoryByRegionGUID(Guid RegionGUID);
        IEnumerable<Territory> GetTerritoryByOrganizationGUID(Guid OrganizationGUID);
        string GetTerritoryNameByTerritoryGUID(Guid TerritoryGUID);
        Territory GetTerritoryByID(Guid TerritoryGUID);
        int InsertTerritory(Territory Territory);
        int DeleteTerritory(Guid TerritoryGUID);
        int DeleteTerritoryByRegionGUID(Guid RegionGUID);
        int DeleteTerritoryByOrganizationGUID(Guid OrganizationGUID);
        int UpdateTerritory(Territory Territory);
        Territory GetTerritoryByTerritoryID(string TerritoryID, Guid OrganizationGUID);
        //int Save();


    }
}