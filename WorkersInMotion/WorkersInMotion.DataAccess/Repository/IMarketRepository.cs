using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IMarketRepository : IDisposable
    {
        IEnumerable<Market> GetMarketByUserGUID(Guid UserGUID, int entitytype);
        Market GetMarketByID(Guid MarketGUID);
        Market GetMarketByMarketID(Guid OrganizationGUID, string MarketID);
        Market GetMarketByPrimaryContactGUID(Guid PrimaryContactGUID, int entitytype);
        IEnumerable<Market> GetMarketByOrganizationGUID(Guid OrganizationGUID, int entitytype);
        IEnumerable<Market> GetMarketByRegionGUID(Guid RegionGUID, int entitytype);
        IEnumerable<Market> GetMarketByTerritoryGUID(Guid TerritoryGUID, int entitytype);
        IEnumerable<Market> GetMarketByRegionGUIDandTerritoryGUID(Guid RegionGUID, Guid TerritoryGUID, int entitytype);
        IEnumerable<Market> GetMarketByOwnerandRegionGUID(Guid RegionGUID, Guid OwnerGUID, int entityType);
        IEnumerable<Market> GetAllMarketByRegionGUIDandTerritoryGUID(Guid RegionGUID, Guid TerritoryGUID);
        IEnumerable<Market> GetMarketByOwnerGUID(Guid OwnerGUID);
        int InsertMarket(Market Market);
        int DeleteMarket(Guid Marketguid);
        int DeleteMarketByOrganizationGUID(Guid OrganizationGUID);
        int DeleteMarketByUserGUID(Guid UserGUID);
        int DeleteMarketByOwnerGUID(Guid OwnerGUID);
        int DeleteMarketByRegionGUID(Guid RegionGUID);
        int DeleteMarketByTerritoryGUID(Guid TerritoryGUID);
        int UpdateMarket(Market Market);
        int UpdateStoreVisitedDate(Guid MarketGUID);
        //int Save();

        List<Market> GetStoreNonVisit(Market pMarket);//Pending
        List<Market> GetAllStores(Market pMarket);//Pending

        Market GetMarketByCustomerID(Guid OrganizationGUID, string pCustomerID, string pCustomerStopID);
    }
}