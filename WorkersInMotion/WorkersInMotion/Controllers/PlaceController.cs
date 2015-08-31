using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.DataAccess.Repository;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Text;
using PagedList;
using System.Xml;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Model.ViewModel;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class PlaceController : BaseController
    {
        #region Constructor
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPeopleRepository _IPeopleRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        public PlaceController()
        {
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
        }
        public PlaceController(WorkersInMotionDB context)
        {
            this._IPlaceRepository = new PlaceRepository(context);
            this._IPeopleRepository = new PeopleRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
        }

        #endregion
        //
        // GET: /Place/
        public ActionResult Index()
        {
            Logger.Debug("Inside Place Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    var placeList = new PlaceViewModel();
                    placeList.PlaceList = new List<PlaceModel>();
                    var appPlace = new List<Place>();

                    appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                    foreach (var place in appPlace.ToList())
                    {
                        placeList.PlaceList.Add(new PlaceModel
                        {
                            PlaceGUID = place.PlaceGUID.ToString(),
                            PlaceID = place.PlaceID,
                            PlaceName = place.PlaceName,
                            PlacePhone = place.PlacePhone,
                            FirstName = place.FirstName,
                            LastName = place.LastName,
                            UserGUID = place.UserGUID.ToString(),
                            OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                            MobilePhone = place.MobilePhone,
                            HomePhone = place.HomePhone,
                            Emails = place.Emails,
                            TimeZone = place.TimeZone,
                            AddressLine1 = place.AddressLine1,
                            AddressLine2 = place.AddressLine2,
                            City = place.City,
                            State = place.State,
                            Country = place.Country,
                            ZipCode = place.ZipCode

                        });
                    }
                    var viewModel = new PlaceViewModel();
                    viewModel.Place = placeList.PlaceList.AsEnumerable();
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");

            }
        }
        public ActionResult Create()
        {

            if (Session["OrganizationGUID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SessionTimeOut", "User");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlaceModel place)
        {
            Logger.Debug("Inside Place Controller- Create Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        Place _place = _IPlaceRepository.GetPlaceByID(place.PlaceID, new Guid(Session["OrganizationGUID"].ToString()));
                        if (_place == null)
                        {
                            Place Place = new Place();
                            Place.PlaceGUID = Guid.NewGuid();
                            Place.UserGUID = new Guid(Session["UserGUID"].ToString());
                            Place.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                            Place.PlaceID = place.PlaceID;
                            Place.PlaceName = place.PlaceName;
                            Place.FirstName = place.FirstName;
                            Place.LastName = place.LastName;
                            Place.MobilePhone = place.MobilePhone;
                            Place.PlacePhone = place.PlacePhone;
                            Place.HomePhone = place.HomePhone;
                            Place.Emails = place.Emails;
                            Place.AddressLine1 = place.AddressLine1;
                            Place.AddressLine2 = place.AddressLine2;
                            Place.City = place.City;
                            Place.State = place.State;
                            Place.Country = place.Country;
                            Place.ZipCode = place.ZipCode;
                            Place.CategoryID = 0;
                            Place.IsDeleted = false;
                            Place.CreateDate = DateTime.UtcNow;
                            Place.UpdatedDate = DateTime.UtcNow;
                            LatLong latLong = new LatLong();
                            latLong = GetLatLngCode(Place.AddressLine1, Place.AddressLine2, Place.City, Place.State, Place.Country, Place.ZipCode);
                            Place.TimeZone = getTimeZone(latLong.Latitude, latLong.Longitude).ToString();

                            Person People = new Person();
                            People.PeopleGUID = Guid.NewGuid();
                            People.UserGUID = Place.UserGUID;
                            People.OrganizationGUID = Place.OrganizationGUID;
                            People.IsPrimaryContact = true;
                            People.PlaceGUID = Place.PlaceGUID;
                            People.FirstName = place.FirstName;
                            People.LastName = place.LastName;
                            People.MobilePhone = place.MobilePhone;
                            People.CompanyName = place.PlaceName;
                            People.BusinessPhone = place.PlacePhone;
                            People.HomePhone = place.HomePhone;
                            People.Emails = place.Emails;
                            People.AddressLine1 = place.AddressLine1;
                            People.AddressLine2 = place.AddressLine2;
                            People.City = place.City;
                            People.State = place.State;
                            People.Country = place.Country;
                            People.ZipCode = place.ZipCode;
                            People.CategoryID = 0;
                            People.IsDeleted = false;
                            People.CreatedDate = DateTime.UtcNow;
                            People.UpdatedDate = DateTime.UtcNow;

                            //Market Market = new Market();
                            //Market.MarketGUID = Guid.NewGuid();
                            //Market.IsDefault = true;
                            //Market.UserGUID = Place.UserGUID;
                            //Market.EntityType = 1;
                            //Market.OrganizationGUID = Place.OrganizationGUID;
                            //Market.OwnerGUID = Place.PlaceGUID;
                            //Market.MarketName = Place.PlaceName;
                            //Market.PrimaryContactGUID = People.PeopleGUID;
                            //Market.FirstName = place.FirstName;
                            //Market.LastName = place.LastName;
                            //Market.MobilePhone = place.MobilePhone;
                            //Market.MarketPhone = place.PlacePhone;
                            //Market.HomePhone = place.HomePhone;
                            //Market.Emails = place.Emails;
                            //Market.AddressLine1 = place.AddressLine1;
                            //Market.AddressLine2 = place.AddressLine2;
                            //Market.City = place.City;
                            //Market.State = place.State;
                            //Market.Country = place.Country;
                            //Market.ZipCode = place.ZipCode;
                            //Market.IsDeleted = false;
                            //Market.CreateDate = DateTime.UtcNow;
                            //Market.UpdatedDate = DateTime.UtcNow;

                            int placeInsertResult = _IPlaceRepository.InsertPlace(Place);
                            //int placeInsertResult = _IPlaceRepository.Save();
                            if (placeInsertResult > 0)
                            {
                                int peopleInsertResult = _IPeopleRepository.InsertPeople(People);
                                // int peopleInsertResult = _IPeopleRepository.Save();
                                if (peopleInsertResult > 0)
                                {
                                    //_IMarketRepository.InsertMarket(Market);
                                    //int marketInsertResult = _IMarketRepository.Save();
                                    //if (marketInsertResult > 0)
                                    //{
                                    return RedirectToAction("Index");
                                    //}
                                    //else
                                    //{
                                    //    _IMarketRepository.DeleteMarket(Market.MarketGUID);
                                    //    _IMarketRepository.Save();
                                    //    _IPeopleRepository.DeletePeople(People.PeopleGUID);
                                    //    _IPeopleRepository.Save();
                                    //    _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                                    //    _IPlaceRepository.Save();
                                    //}
                                }
                                else
                                {
                                    _IPeopleRepository.DeletePeople(People.PeopleGUID);
                                    // _IPeopleRepository.Save();
                                    _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                                    //_IPlaceRepository.Save();
                                }
                            }
                            else
                            {
                                _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                                // _IPlaceRepository.Save();
                            }
                        }
                        else
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Client ID already configured');</script>";
                            return View(place);
                        }
                    }
                    else
                    {
                        return View(place);
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(place);
            }
        }
        public ActionResult Edit(string id = "")
        {
            Logger.Debug("Inside Place Controller- Edit");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    PlaceModel place = new PlaceModel();
                    place.PlaceGUID = id;
                    Place Place = _IPlaceRepository.GetPlaceByID(new Guid(place.PlaceGUID));
                    if (Place != null)
                    {
                        place.PlaceGUID = Place.PlaceGUID.ToString();
                        place.PlaceID = Place.PlaceID;
                        place.UserGUID = Place.UserGUID.ToString();
                        place.OrganizationGUID = Place.OrganizationGUID != null ? Place.OrganizationGUID.ToString() : Guid.Empty.ToString();
                        place.PlaceName = Place.PlaceName;
                        place.FirstName = Place.FirstName;
                        place.LastName = Place.LastName;
                        place.MobilePhone = Place.MobilePhone;
                        place.PlacePhone = Place.PlacePhone;
                        place.HomePhone = Place.HomePhone;
                        place.Emails = Place.Emails;
                        place.AddressLine1 = Place.AddressLine1;
                        place.AddressLine2 = Place.AddressLine2;
                        place.City = Place.City;
                        place.State = Place.State;
                        place.Country = Place.Country;
                        place.ZipCode = Place.ZipCode;
                    }
                    return View(place);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View();
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PlaceModel place)
        {
            Logger.Debug("Inside Place Controller- Edit Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        Place Place = new Place();
                        Place.PlaceGUID = new Guid(place.PlaceGUID);
                        Place.PlaceID = place.PlaceID;
                        Place.UserGUID = new Guid(place.UserGUID);
                        if (!string.IsNullOrEmpty(place.OrganizationGUID) && place.OrganizationGUID != Guid.Empty.ToString())
                        {
                            Place.OrganizationGUID = new Guid(place.OrganizationGUID);
                        }
                        else
                        {
                            Place.OrganizationGUID = null;
                        }
                        Place.PlaceName = place.PlaceName;
                        Place.FirstName = place.FirstName;
                        Place.LastName = place.LastName;
                        Place.MobilePhone = place.MobilePhone;
                        Place.PlacePhone = place.PlacePhone;
                        Place.HomePhone = place.HomePhone;
                        Place.Emails = place.Emails;
                        Place.AddressLine1 = place.AddressLine1;
                        Place.AddressLine2 = place.AddressLine2;
                        Place.City = place.City;
                        Place.State = place.State;
                        Place.Country = place.Country;
                        Place.ZipCode = place.ZipCode;
                        Place.UpdatedDate = DateTime.UtcNow;
                        LatLong latLong = new LatLong();
                        latLong = GetLatLngCode(Place.AddressLine1, Place.AddressLine2, Place.City, Place.State, Place.Country, Place.ZipCode);
                        Place.TimeZone = getTimeZone(latLong.Latitude, latLong.Longitude).ToString();

                        int placeInsertResult = _IPlaceRepository.UpdatePlace(Place);
                        //int placeInsertResult = _IPlaceRepository.Save();
                        if (placeInsertResult > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return View(place);
                        }
                    }
                    return View(place);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(place);
            }
        }

        public ActionResult Delete(string id = "")
        {
            Logger.Debug("Inside Place Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    PlaceModel place = new PlaceModel();
                    place.PlaceGUID = id;
                    _IPlaceRepository.DeletePlace(new Guid(place.PlaceGUID));
                    //_IPlaceRepository.Save();
                    _IPeopleRepository.DeletePeopleByPlaceGUID(new Guid(place.PlaceGUID));
                    _IMarketRepository.DeleteMarketByOwnerGUID(new Guid(place.PlaceGUID));

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Index");
            }
        }
        public LatLong GetLatLngCode(string address1, string address2, string city, string state, string country, string zipcode)
        {
            string address = address1 + " ," + address2 + " ," + city + " ," + state + " ," + country + " ," + zipcode;
            address = address.Substring(0, address.LastIndexOf(","));

            string urlAddress = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + HttpUtility.UrlEncode(address) + "&sensor=false";
            string[] returnValue = new string[2];
            string tzone = string.Empty;
            Logger.Debug("Inside Organization Controller- GetLatLngCode");
            try
            {
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(urlAddress);
                XmlNodeList objXmlNodeList = objXmlDocument.SelectNodes("/GeocodeResponse/result/geometry/location");
                foreach (XmlNode objXmlNode in objXmlNodeList)
                {
                    // GET LONGITUDE
                    returnValue[0] = objXmlNode.ChildNodes.Item(0).InnerText;

                    // GET LATITUDE
                    returnValue[1] = objXmlNode.ChildNodes.Item(1).InnerText;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                // Process an error action here if needed 
            }
            return new LatLong(Convert.ToDouble(returnValue[0]), Convert.ToDouble(returnValue[1]));

        }
        public double getTimeZone(double lat, double lon)
        {
            double tzone = 0;
            Logger.Debug("Inside Organization Controller- GetTimeZone");
            try
            {
                string _timeZone = "https://maps.googleapis.com/maps/api/timezone/xml?location=" + lat + "," + lon + "&timestamp=" + DateTime.UtcNow.TimeOfDay.Ticks + "&sensor=true";
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(_timeZone);
                XmlNodeList objXmlNodeList = objXmlDocument.SelectNodes("/TimeZoneResponse");
                foreach (XmlNode objXmlNode in objXmlNodeList)
                {
                    if (objXmlNode.ChildNodes.Count >= 4)
                        tzone = Convert.ToDouble(objXmlNode.ChildNodes.Item(1).InnerText) / 60;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return tzone;
        }
    }
}