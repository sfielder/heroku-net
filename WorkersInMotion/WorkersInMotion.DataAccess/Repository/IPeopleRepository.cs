using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.DataAccess.Repository
{
    public interface IPeopleRepository : IDisposable
    {
        IEnumerable<Person> GetPeopleByUserGUID(Guid UserGUID);
        Person GetPeopleByID(Guid PeopleGUID);
        IEnumerable<Person> GetPeopleByOrganizationGUID(Guid OrganizationGUID);
        string GetPeopleNameByPeopleGUID(Guid PeopleGUID);

        IEnumerable<Person> GetPeopleByPlaceGUID(Guid PlaceGUID);
        int InsertPeople(Person Person);
        int DeletePeople(Guid Peopleguid);
        int DeletePeopleByOrganizationGUID(Guid OrganizationGUID);
        int DeletePeopleByPlaceGUID(Guid PlaceGUID);
        int UpdatePeople(Person Person);
        //int Save();
    }
}