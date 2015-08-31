using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.WebAPI.Models.MobileModel.Interface
{
    public interface IEMServer
    {
        bool ValidateUser(string SessionGUID);
        string convertdate(DateTime date);
        System.Guid GetUserGUID(string SessionGUID);
        Guid GetOrganizationGUID(string SessionGUID);
        EMCustomers GetCustomers(Guid OrganizationGUID);
        EMContacts GetContacts(Guid OrganizationGUID);
        EMMarkets GetCustomerStops(Guid OrganizationGUID);
        EMMarkets GetServicePoints(Guid OrganizationGUID);
        //EMContacts GetCustomers(string SessionID);
        //EMMarkets GetCustomerStop(string SessionID);
        //EMMarkets GetServicePoints(string SessionID);

    }
}
