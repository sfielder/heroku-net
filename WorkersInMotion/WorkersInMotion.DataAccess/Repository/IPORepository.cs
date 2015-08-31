using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.DataAccess.Repository
{
    public interface IPORepository : IDisposable
    {
        IEnumerable<POs> GetPOList(Guid OrganizationGUID);
        IEnumerable<POs> GetPOListByPlaceID(Guid OrganizationGUID, string PlaceID);
        POs GetPObyPoNumber(string PONumber);
        //int Save();
        int CreatePO(POs NewPO);
    }
}