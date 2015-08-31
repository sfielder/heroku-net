using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;
using WorkersInMotion.Log;

namespace WorkersInMotion.WebAPI.Models.MobileModel.Service
{
    public class EMServer : IEMServer
    {
        #region Variables Declaration
        readonly ILogService _ILogservice;
        protected ILogService Logger
        {
            get
            {
                return _ILogservice;
            }
        }
        #endregion
        public EMServer()
        {
            _ILogservice = new Log4NetService(GetType());
        }
        public string convertdate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") == "0001-01-01T00:00:00Z" ? "" : date.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
        public bool ValidateUser(string SessionGUID)
        {
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            return (_IUserRepository.ValidateUser(SessionGUID));
        }

        public System.Guid GetUserGUID(string SessionGUID)
        {
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            return (_IUserRepository.GetUserGUID(SessionGUID));
        }

        public Guid GetOrganizationGUID(string SessionGUID)
        {
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            return (_IUserRepository.GetOrganizationGUID(SessionGUID));
        }

        public EMCustomers GetCustomers(Guid OrganizationGUID)
        {
            IPlaceRepository _IPlaceRepository;
            _IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());

            EMCustomers lresponse = new EMCustomers();
            lresponse.Customers = new List<MobilePlace>();

            List<Place> _Customers = _IPlaceRepository.GetPlaceByOrganizationGUID(OrganizationGUID).ToList();
            foreach (Place item in _Customers)
            {
                lresponse.Customers.Add(ConvertPlaceforMobile(item));
            }
            return lresponse;
        }

        public EMContacts GetContacts(Guid OrganizationGUID)
        {
            IPeopleRepository _IPeopleRepository;
            _IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());

            EMContacts lresponse = new EMContacts();
            lresponse.Contacts = new List<MobilePeople>();

            List<Person> _Contacts = _IPeopleRepository.GetPeopleByOrganizationGUID(OrganizationGUID).ToList();
            foreach (Person item in _Contacts)
            {
                lresponse.Contacts.Add(ConvertPeopleForMobile(item));
            }
            return lresponse;
        }

        public EMMarkets GetCustomerStops(Guid OrganizationGUID)
        {
            IMarketRepository _IMarketRepository;
            _IMarketRepository = new MarketRepository(new WorkersInMotionDB());

            EMMarkets lresponse = new EMMarkets();
            lresponse.Markets = new List<MobileMarket>();

            List<Market> _Markets = _IMarketRepository.GetMarketByOrganizationGUID(OrganizationGUID, 1).ToList();
            foreach (Market item in _Markets)
            {
                lresponse.Markets.Add(ConvertMarketforMobile(item));
            }
            return lresponse;
        }

        public EMMarkets GetServicePoints(Guid OrganizationGUID)
        {
            IMarketRepository _IMarketRepository;
            _IMarketRepository = new MarketRepository(new WorkersInMotionDB());

            EMMarkets lresponse = new EMMarkets();
            lresponse.Markets = new List<MobileMarket>();

            List<Market> _Markets = _IMarketRepository.GetMarketByOrganizationGUID(OrganizationGUID, 0).ToList();
            foreach (Market item in _Markets)
            {
                lresponse.Markets.Add(ConvertMarketforMobile(item));
            }
            return lresponse;
        }
        public MobilePlace ConvertPlaceforMobile(Place Place)
        {
            if (Place != null)
            {
                MobilePlace _place = new MobilePlace();
                _place.PlaceGUID = Place.PlaceGUID;
                _place.PlaceID = Place.PlaceID;
                _place.UserGUID = Place.UserGUID;
                _place.OrganizationGUID = Place.OrganizationGUID;
                _place.PlaceName = Place.PlaceName;
                _place.FirstName = Place.FirstName;
                _place.LastName = Place.LastName;
                _place.MobilePhone = Place.MobilePhone;
                _place.PlacePhone = Place.PlacePhone;
                _place.HomePhone = Place.HomePhone;
                _place.Emails = Place.Emails;
                _place.TimeZone = Place.TimeZone;
                _place.AddressLine1 = Place.AddressLine1;
                _place.AddressLine2 = Place.AddressLine2;
                _place.City = Place.City;
                _place.State = Place.State;
                _place.Country = Place.Country;
                _place.ZipCode = Place.ZipCode;
                _place.CategoryID = Place.CategoryID;
                _place.IsDeleted = Place.IsDeleted;
                _place.ImageURL = Place.ImageURL;
                _place.CreateDate = convertdate(Convert.ToDateTime(Place.CreateDate));// Place.CreateDate;
                _place.UpdatedDate = convertdate(Convert.ToDateTime(Place.UpdatedDate));// Place.UpdatedDate;
                return _place;
            }
            else
            {
                return null;
            }
        }

        public MobilePeople ConvertPeopleForMobile(Person Person)
        {
            if (Person != null)
            {
                MobilePeople _person = new MobilePeople();
                _person.PeopleGUID = Person.PeopleGUID;
                _person.RecordStatus = Person.RecordStatus;
                _person.UserGUID = Person.UserGUID;
                _person.OrganizationGUID = Person.OrganizationGUID;
                _person.IsPrimaryContact = Person.IsPrimaryContact;
                _person.PlaceGUID = Person.PlaceGUID;
                _person.MarketGUID = Person.MarketGUID;
                _person.FirstName = Person.FirstName;
                _person.LastName = Person.LastName;
                _person.CompanyName = Person.CompanyName;
                _person.MobilePhone = Person.MobilePhone;
                _person.BusinessPhone = Person.BusinessPhone;
                _person.HomePhone = Person.HomePhone;
                _person.Emails = Person.Emails;
                _person.AddressLine1 = Person.AddressLine1;
                _person.AddressLine2 = Person.AddressLine2;
                _person.City = Person.City;
                _person.State = Person.State;
                _person.Country = Person.Country;
                _person.ZipCode = Person.ZipCode;
                _person.CategoryID = Person.CategoryID;
                _person.IsDeleted = Person.IsDeleted;
                _person.ImageURL = Person.ImageURL;
                _person.CreatedDate = convertdate(Convert.ToDateTime(Person.CreatedDate));// Person.CreatedDate;
                _person.UpdatedDate = convertdate(Convert.ToDateTime(Person.UpdatedDate));// Person.UpdatedDate;

                return _person;
            }
            else
            {
                return null;
            }

        }
        public MobileMarket ConvertMarketforMobile(Market _market)
        {
            if (_market != null)
            {
                MobileMarket _MobileMarket = new MobileMarket();
                // Market _market = context.Markets.Find(MarketGUID);
                _MobileMarket.MarketGUID = _market.MarketGUID;
                _MobileMarket.RecordStatus = _market.RecordStatus;
                _MobileMarket.IsDefault = _market.IsDefault;
                _MobileMarket.Version = _market.Version;
                _MobileMarket.UserGUID = _market.UserGUID;
                _MobileMarket.EntityType = _market.EntityType;
                _MobileMarket.OrganizationGUID = _market.OrganizationGUID;
                _MobileMarket.OwnerGUID = _market.OwnerGUID;
                _MobileMarket.MarketName = _market.MarketName;
                _MobileMarket.RegionGUID = _market.RegionGUID;
                _MobileMarket.TerritoryGUID = _market.TerritoryGUID;
                _MobileMarket.PrimaryContactGUID = _market.PrimaryContactGUID;
                _MobileMarket.FirstName = _market.FirstName;
                _MobileMarket.LastName = _market.LastName;
                _MobileMarket.MobilePhone = _market.MobilePhone;
                _MobileMarket.MarketPhone = _market.MarketPhone;
                _MobileMarket.HomePhone = _market.HomePhone;
                _MobileMarket.Emails = _market.Emails;
                _MobileMarket.TimeZone = _market.TimeZone;
                _MobileMarket.AddressLine1 = _market.AddressLine1;
                _MobileMarket.AddressLine2 = _market.AddressLine2;
                _MobileMarket.City = _market.City;
                _MobileMarket.State = _market.State;
                _MobileMarket.Country = _market.Country;
                _MobileMarket.ZipCode = _market.ZipCode;
                _MobileMarket.Latitude = _market.Latitude;
                _MobileMarket.Longitude = _market.Longitude;
                _MobileMarket.ImageURL = _market.ImageURL;
                _MobileMarket.CreateDate = convertdate(Convert.ToDateTime(_market.CreateDate));// _market.CreateDate;
                _MobileMarket.UpdatedDate = convertdate(Convert.ToDateTime(_market.UpdatedDate));// _market.UpdatedDate;
                _MobileMarket.IsDeleted = _market.IsDeleted;

                return _MobileMarket;
            }
            else
            {
                return null;
            }
        }

        //public EMResponse GetContactsbyCustomerGUID(string CustomerGUID)
        //{
        //    IPeopleRepository _IPeopleRepository;
        //    _IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
        //    IGlobalUserRepository _IGlobalUserRepository;
        //    _IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
        //    EMResponse _emResponse = new EMResponse();
        //    _emResponse.Contacts = new List<MContacts>();
        //    if (!string.IsNullOrEmpty(CustomerGUID))
        //    {

        //        List<Person> Contacts = _IPeopleRepository.GetPeopleByPlaceGUID(new Guid(CustomerGUID)).ToList();
        //        if (Contacts != null && Contacts.Count > 0)
        //        {
        //            foreach (Person item in Contacts)
        //            {
        //                MContacts _mcontacts = new MContacts();
        //                _mcontacts.PeopleGUID = item.PeopleGUID;
        //                _mcontacts.RecordStatus = item.RecordStatus;
        //                _mcontacts.UserGUID = item.UserGUID;
        //                _mcontacts.OrganizationGUID = item.OrganizationGUID;
        //                _mcontacts.IsPrimaryContact = item.IsPrimaryContact;
        //                _mcontacts.PlaceGUID = item.PlaceGUID;
        //                _mcontacts.MarketGUID = item.MarketGUID;
        //                _mcontacts.FirstName = item.FirstName;
        //                _mcontacts.LastName = item.LastName;
        //                _mcontacts.CompanyName = item.CompanyName;
        //                _mcontacts.MobilePhone = item.MobilePhone;
        //                _mcontacts.BusinessPhone = item.BusinessPhone;
        //                _mcontacts.HomePhone = item.HomePhone;
        //                _mcontacts.Emails = item.Emails;
        //                _mcontacts.AddressLine1 = item.AddressLine1;
        //                _mcontacts.AddressLine2 = item.AddressLine2;
        //                _mcontacts.City = item.City;
        //                _mcontacts.State = item.State;
        //                _mcontacts.Country = item.Country;
        //                _mcontacts.ZipCode = item.ZipCode;
        //                _mcontacts.CategoryID = item.CategoryID;
        //                _mcontacts.IsDeleted = item.IsDeleted;
        //                _mcontacts.CreatedDate = item.CreatedDate;
        //                _mcontacts.UpdatedDate = item.UpdatedDate;
        //                _mcontacts.PicFilename = item.ImageURL;

        //                _emResponse.Contacts.Add(_mcontacts);
        //            }
        //        }
        //    }
        //    return _emResponse;
        //}

        //public EMResponse GetContactsbySessionID(string SessionID)
        //{
        //    IPeopleRepository _IPeopleRepository;
        //    _IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
        //    //IRegionRepository _IRegionRepository;
        //    //_IRegionRepository = new RegionRepository(new WorkersInMotionDB());
        //    //ITerritoryRepository _ITerritoryRepository;
        //    //_ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
        //    IOrganizationRepository _IOrganizationRepository;
        //    _IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
        //    IGlobalUserRepository _IGlobalUserRepository;
        //    _IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
        //    EMResponse _emResponse = new EMResponse();
        //    _emResponse.Contacts = new List<MContacts>();
        //    if (!string.IsNullOrEmpty(SessionID))
        //    {
        //        // string AreaGUID = _IRegionRepository.GetRegionGUID(SessionID);
        //        // string ZoneGUID = _ITerritoryRepository.GetTerritoryGUID(SessionID);
        //        string OrganizationGUID = _IOrganizationRepository.GetOrganizationID(SessionID);
        //        if (!string.IsNullOrEmpty(OrganizationGUID))
        //        {
        //            List<Person> Contacts = _IPeopleRepository.GetPeopleByOrganizationGUID(new Guid(OrganizationGUID)).ToList();
        //            if (Contacts != null && Contacts.Count > 0)
        //            {
        //                foreach (Person item in Contacts)
        //                {
        //                    MContacts _mcontacts = new MContacts();
        //                    _mcontacts.PeopleGUID = item.PeopleGUID;
        //                    _mcontacts.RecordStatus = item.RecordStatus;
        //                    _mcontacts.UserGUID = item.UserGUID;
        //                    _mcontacts.OrganizationGUID = item.OrganizationGUID;
        //                    _mcontacts.IsPrimaryContact = item.IsPrimaryContact;
        //                    _mcontacts.PlaceGUID = item.PlaceGUID;
        //                    _mcontacts.MarketGUID = item.MarketGUID;
        //                    _mcontacts.FirstName = item.FirstName;
        //                    _mcontacts.LastName = item.LastName;
        //                    _mcontacts.CompanyName = item.CompanyName;
        //                    _mcontacts.MobilePhone = item.MobilePhone;
        //                    _mcontacts.BusinessPhone = item.BusinessPhone;
        //                    _mcontacts.HomePhone = item.HomePhone;
        //                    _mcontacts.Emails = item.Emails;
        //                    _mcontacts.AddressLine1 = item.AddressLine1;
        //                    _mcontacts.AddressLine2 = item.AddressLine2;
        //                    _mcontacts.City = item.City;
        //                    _mcontacts.State = item.State;
        //                    _mcontacts.Country = item.Country;
        //                    _mcontacts.ZipCode = item.ZipCode;
        //                    _mcontacts.CategoryID = item.CategoryID;
        //                    _mcontacts.IsDeleted = item.IsDeleted;
        //                    _mcontacts.CreatedDate = item.CreatedDate;
        //                    _mcontacts.UpdatedDate = item.UpdatedDate;
        //                    _mcontacts.PicFilename = item.ImageURL;

        //                    _emResponse.Contacts.Add(_mcontacts);
        //                }
        //            }
        //        }
        //    }
        //    return _emResponse;
        //}


        //public EMResponse GetCustomerStopbyCustomerGUID(string CustomerGUID)
        //{
        //    IMarketRepository _IMarketRepository;
        //    _IMarketRepository = new MarketRepository(new WorkersInMotionDB());
        //    IGlobalUserRepository _IGlobalUserRepository;
        //    _IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
        //    EMResponse _emResponse = new EMResponse();
        //    _emResponse.Markets = new List<MMarkets>();
        //    if (!string.IsNullOrEmpty(CustomerGUID))
        //    {

        //        List<Market> Markets = _IMarketRepository.GetMarketByOwnerGUID(new Guid(CustomerGUID)).ToList();
        //        if (Markets != null && Markets.Count > 0)
        //        {
        //            foreach (Market item in Markets)
        //            {
        //                MMarkets _mmarket = new MMarkets();
        //                _mmarket.MarketGUID = item.MarketGUID;
        //                _mmarket.RecordStatus = item.RecordStatus;
        //                _mmarket.IsDefault = item.IsDefault;
        //                _mmarket.Version = item.Version;
        //                _mmarket.UserGUID = item.UserGUID;
        //                _mmarket.EntityType = item.EntityType;
        //                _mmarket.OrganizationGUID = item.OrganizationGUID;
        //                _mmarket.OwnerGUID = item.OwnerGUID;
        //                _mmarket.MarketName = item.MarketName;
        //                _mmarket.RegionGUID = item.RegionGUID;
        //                _mmarket.TerritoryGUID = item.TerritoryGUID;
        //                _mmarket.PrimaryContactGUID = item.PrimaryContactGUID;
        //                _mmarket.FirstName = item.FirstName;
        //                _mmarket.LastName = item.LastName;
        //                _mmarket.MobilePhone = item.MobilePhone;
        //                _mmarket.MarketPhone = item.MarketPhone;
        //                _mmarket.HomePhone = item.HomePhone;
        //                _mmarket.Emails = item.Emails;
        //                _mmarket.TimeZone = item.TimeZone;
        //                _mmarket.AddressLine1 = item.AddressLine1;
        //                _mmarket.AddressLine2 = item.AddressLine2;
        //                _mmarket.City = item.City;
        //                _mmarket.State = item.State;
        //                _mmarket.Country = item.Country;
        //                _mmarket.ZipCode = item.ZipCode;
        //                _mmarket.CreateDate = Convert.ToDateTime(item.CreateDate.ToString());
        //                _mmarket.UpdatedDate = Convert.ToDateTime(item.UpdatedDate.ToString());
        //                _mmarket.IsDeleted = item.IsDeleted;

        //                _emResponse.Markets.Add(_mmarket);
        //            }
        //        }
        //    }
        //    return _emResponse;
        //}

        //public EMResponse GetCustomerStopbySessionID(string SessionID)
        //{
        //    IMarketRepository _IMarketRepository;
        //    _IMarketRepository = new MarketRepository(new WorkersInMotionDB());
        //    IRegionRepository _IRegionRepository;
        //    _IRegionRepository = new RegionRepository(new WorkersInMotionDB());
        //    ITerritoryRepository _ITerritoryRepository;
        //    _ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
        //    IOrganizationRepository _IOrganizationRepository;
        //    _IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
        //    IGlobalUserRepository _IGlobalUserRepository;
        //    _IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
        //    EMResponse _emResponse = new EMResponse();
        //    _emResponse.Markets = new List<MMarkets>();
        //    if (!string.IsNullOrEmpty(SessionID))
        //    {
        //        string AreaGUID = _IRegionRepository.GetRegionGUID(SessionID);
        //        string ZoneGUID = _ITerritoryRepository.GetTerritoryGUID(SessionID);
        //        string OrganizationGUID = _IOrganizationRepository.GetOrganizationID(SessionID);
        //        if (!string.IsNullOrEmpty(OrganizationGUID) && !string.IsNullOrEmpty(AreaGUID) && !string.IsNullOrEmpty(ZoneGUID))
        //        {
        //            List<Market> Markets = _IMarketRepository.GetAllMarketByRegionGUIDandTerritoryGUID(new Guid(AreaGUID), new Guid(ZoneGUID)).ToList();
        //            if (Markets != null && Markets.Count > 0)
        //            {
        //                foreach (Market item in Markets)
        //                {
        //                    MMarkets _mmarket = new MMarkets();
        //                    _mmarket.MarketGUID = item.MarketGUID;
        //                    _mmarket.RecordStatus = item.RecordStatus;
        //                    _mmarket.IsDefault = item.IsDefault;
        //                    _mmarket.Version = item.Version;
        //                    _mmarket.UserGUID = item.UserGUID;
        //                    _mmarket.EntityType = item.EntityType;
        //                    _mmarket.OrganizationGUID = item.OrganizationGUID;
        //                    _mmarket.OwnerGUID = item.OwnerGUID;
        //                    _mmarket.MarketName = item.MarketName;
        //                    _mmarket.RegionGUID = item.RegionGUID;
        //                    _mmarket.TerritoryGUID = item.TerritoryGUID;
        //                    _mmarket.PrimaryContactGUID = item.PrimaryContactGUID;
        //                    _mmarket.FirstName = item.FirstName;
        //                    _mmarket.LastName = item.LastName;
        //                    _mmarket.MobilePhone = item.MobilePhone;
        //                    _mmarket.MarketPhone = item.MarketPhone;
        //                    _mmarket.HomePhone = item.HomePhone;
        //                    _mmarket.Emails = item.Emails;
        //                    _mmarket.TimeZone = item.TimeZone;
        //                    _mmarket.AddressLine1 = item.AddressLine1;
        //                    _mmarket.AddressLine2 = item.AddressLine2;
        //                    _mmarket.City = item.City;
        //                    _mmarket.State = item.State;
        //                    _mmarket.Country = item.Country;
        //                    _mmarket.ZipCode = item.ZipCode;
        //                    _mmarket.CreateDate = Convert.ToDateTime(item.CreateDate.ToString());
        //                    _mmarket.UpdatedDate = Convert.ToDateTime(item.UpdatedDate.ToString());
        //                    _mmarket.IsDeleted = item.IsDeleted;

        //                    _emResponse.Markets.Add(_mmarket);
        //                }
        //            }
        //        }
        //    }
        //    return _emResponse;
        //}

    }
}
