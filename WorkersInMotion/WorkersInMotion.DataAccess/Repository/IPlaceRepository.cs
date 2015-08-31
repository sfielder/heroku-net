using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IPlaceRepository : IDisposable
    {
        IEnumerable<Place> GetPlaceByUserGUID(Guid UserGUID);
        Place GetPlaceByID(Guid PlaceGUID);
        Place GetPlaceByID(string PlaceID);
        Place GetPlaceByID(string PlaceID, Guid OrganizationGUID);

        IEnumerable<Place> GetPlaceByOrganizationGUID(Guid OrganizationGUID);
        int InsertPlace(Place place);
        int DeletePlace(Guid placeguid);
        int DeletePlaceByOrganizationGUID(Guid OrganizationGUID);
        int UpdatePlace(Place place);
        //int Save();
    }
}