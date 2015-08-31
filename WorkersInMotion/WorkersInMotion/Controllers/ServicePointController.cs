using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Model.ViewModel;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class ServicePointController : BaseController
    {
        #region Constructor
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPeopleRepository _IPeopleRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        public ServicePointController()
        {
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
        }
        public ServicePointController(WorkersInMotionDB context)
        {
            this._IPlaceRepository = new PlaceRepository(context);
            this._IPeopleRepository = new PeopleRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IRegionRepository = new RegionRepository(context);
        }

        #endregion
        public void DropdownValues()
        {
            var RegionDetails = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList().Select(r => new SelectListItem
            {
                Value = r.RegionGUID.ToString(),
                Text = r.Name
            });
            ViewBag.RegionDetails = new SelectList(RegionDetails, "Value", "Text");

            var TerritoryDetails = _ITerritoryRepository.GetTerritoryByOrganizationGUID(Guid.Empty).ToList().Select(r => new SelectListItem
            {
                Value = r.TerritoryGUID.ToString(),
                Text = r.Name
            });
            ViewBag.TerritoryDetails = new SelectList(TerritoryDetails, "Value", "Text");
        }
        //
        // GET: /ServicePoint/
        public ActionResult Index(string id = "", string territoryguid = "")
        {
            Logger.Debug("Inside People Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    var regionList = new RegionViewModel();
                    regionList.Region = new List<RegionModel>();

                    var appRegion = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                    regionList.Region.Add(new RegionModel { Name = "All Regions", RegionGUID = "", Description = "", OrganizationGUID = "" });
                    foreach (var region in appRegion.ToList())
                    {
                        regionList.Region.Add(new RegionModel { Name = region.Name, RegionGUID = region.RegionGUID.ToString(), Description = region.Description, OrganizationGUID = region.OrganizationGUID != null ? region.OrganizationGUID.ToString() : Guid.Empty.ToString() });
                    }

                    var territoryList = new TerritoryViewModel();
                    territoryList.Territory = new List<TerritoryModel>();
                    var appTerritory = new List<Territory>();
                    if (string.IsNullOrEmpty(id) || id == Guid.Empty.ToString())
                    {
                        appTerritory = _ITerritoryRepository.GetTerritoryByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    }
                    else
                    {
                        appTerritory = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(id)).ToList();
                    }
                    territoryList.Territory.Add(new TerritoryModel { Name = "All Territories", TerritoryGUID = "", RegionGUID = "", Description = "", OrganizationGUID = "" });
                    foreach (var territory in appTerritory.ToList())
                    {
                        territoryList.Territory.Add(new TerritoryModel { Name = territory.Name, RegionGUID = territory.RegionGUID != null ? territory.RegionGUID.ToString() : Guid.Empty.ToString(), TerritoryGUID = territory.TerritoryGUID.ToString(), Description = territory.Description, OrganizationGUID = territory.OrganizationGUID != null ? territory.OrganizationGUID.ToString() : Guid.Empty.ToString() });
                    }

                    var marketList = new ServicePointViewModel();
                    marketList.MarketList = new List<ServicePointModel>();
                    var appMarket = new List<Market>();
                    if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(territoryguid))
                    {
                        // if (Session["UserType"].ToString() == "ENT_A")
                        {
                            appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 0).ToList();
                        }
                    }
                    else if (!string.IsNullOrEmpty(id) && string.IsNullOrEmpty(territoryguid))
                    {
                        appMarket = _IMarketRepository.GetMarketByRegionGUID(new Guid(id), 0).ToList();
                        ViewBag.TerritoryGUID = id;
                    }
                    else if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(territoryguid))
                    {
                        appMarket = _IMarketRepository.GetMarketByRegionGUIDandTerritoryGUID(new Guid(territoryguid), new Guid(id), 0).ToList();
                        ViewBag.RegionGUID = territoryguid;
                        ViewBag.TerritoryGUID = id;
                    }
                    foreach (var market in appMarket.ToList())
                    {
                        marketList.MarketList.Add(new ServicePointModel
                        {
                            MarketGUID = market.MarketGUID.ToString(),
                            UserGUID = market.UserGUID.ToString(),
                            OrganizationGUID = market.OrganizationGUID.ToString(),
                            OwnerGUID = market.OwnerGUID.ToString(),
                            MarketName = market.MarketName,
                            MarketPhone = market.MarketPhone,
                            PrimaryContactGUID = market.PrimaryContactGUID.ToString(),
                            FirstName = market.FirstName,
                            LastName = market.LastName,
                            MobilePhone = market.MobilePhone,
                            HomePhone = market.HomePhone,
                            Emails = market.Emails,
                            AddressLine1 = market.AddressLine1,
                            AddressLine2 = market.AddressLine2,
                            City = market.City,
                            State = market.State,
                            Country = market.Country,
                            ZipCode = market.ZipCode,
                            RegionGUID = market.RegionGUID.ToString(),
                            TerritoryGUID = market.TerritoryGUID.ToString(),
                            RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "",
                            TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "",
                        });
                    }

                    var viewModel = new ServicePointViewModel();
                    viewModel.RegionModel = regionList.Region.AsEnumerable();
                    viewModel.TerritoryModel = territoryList.Territory.AsEnumerable();
                    viewModel.MarketModel = marketList.MarketList.AsEnumerable();

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
                DropdownValues();
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
        public ActionResult Create(ServicePointModel market, string RegionGUID, string TerritoryGUID)
        {
            Logger.Debug("Inside ServicePoint Controller- Create Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    if (ModelState.IsValid)
                    {
                        Market Market = new Market();
                        Market.MarketGUID = Guid.NewGuid();
                        Market.IsDefault = true;
                        Market.UserGUID = new Guid(Session["UserGUID"].ToString());
                        Market.EntityType = 0;
                        Market.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        Market.OwnerGUID = new Guid(Session["OrganizationGUID"].ToString());
                        Market.MarketName = market.MarketName;
                        Market.PrimaryContactGUID = new Guid(Session["UserGUID"].ToString());
                        if (!string.IsNullOrEmpty(RegionGUID) && RegionGUID != Guid.Empty.ToString())
                        {
                            Market.RegionGUID = new Guid(RegionGUID);
                        }
                        else
                        {
                            Market.Region = null;
                        }

                        if (!string.IsNullOrEmpty(TerritoryGUID) && TerritoryGUID != Guid.Empty.ToString())
                        {
                            Market.TerritoryGUID = new Guid(TerritoryGUID);
                        }
                        else
                        {
                            Market.TerritoryGUID = null;
                        }

                        Market.FirstName = market.FirstName;
                        Market.LastName = market.LastName;
                        Market.MobilePhone = market.MobilePhone;
                        Market.MarketPhone = market.MarketPhone;
                        Market.HomePhone = market.HomePhone;
                        Market.Emails = market.Emails;
                        Market.AddressLine1 = market.AddressLine1;
                        Market.AddressLine2 = market.AddressLine2;
                        Market.City = market.City;
                        Market.State = market.State;
                        Market.Country = market.Country;
                        Market.ZipCode = market.ZipCode;
                        Market.IsDeleted = false;
                        Market.CreateDate = DateTime.UtcNow;
                        Market.UpdatedDate = DateTime.UtcNow;

                        LatLong latLong = new LatLong();
                        latLong = GetLatLngCode(Market.AddressLine1, Market.AddressLine2, Market.City, Market.State, Market.Country, Market.ZipCode);
                        Market.TimeZone = getTimeZone(latLong.Latitude, latLong.Longitude).ToString();
                        Market.Latitude = latLong.Latitude;
                        Market.Longitude = latLong.Longitude;
                        int marketInsertResult = _IMarketRepository.InsertMarket(Market);
                        // int marketInsertResult = _IMarketRepository.Save();
                        if (marketInsertResult > 0)
                        {
                            return RedirectToAction("Index");
                        }

                    }
                    return View(market);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(market);
            }
        }

        public ActionResult Edit(string id = "")
        {

            Logger.Debug("Inside ServicePoint Controller- Create Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    Market EditMarket = _IMarketRepository.GetMarketByID(new Guid(id));
                    ServicePointModel Market = new ServicePointModel();
                    if (EditMarket != null)
                    {
                        Market.MarketGUID = EditMarket.MarketGUID.ToString();
                        Market.IsDefault = Convert.ToBoolean(EditMarket.IsDefault);
                        Market.UserGUID = EditMarket.UserGUID.ToString();
                        Market.EntityType = Convert.ToInt32(EditMarket.EntityType);
                        Market.OrganizationGUID = EditMarket.OrganizationGUID != null ? EditMarket.OrganizationGUID.ToString() : Guid.Empty.ToString();
                        Market.OwnerGUID = EditMarket.OwnerGUID != null ? EditMarket.OwnerGUID.ToString() : Guid.Empty.ToString();
                        Market.MarketName = EditMarket.MarketName;
                        Market.PrimaryContactGUID = EditMarket.PrimaryContactGUID != null ? EditMarket.PrimaryContactGUID.ToString() : Guid.Empty.ToString();
                        Market.RegionGUID = EditMarket.RegionGUID != null ? EditMarket.RegionGUID.ToString() : Guid.Empty.ToString();
                        Market.TerritoryGUID = EditMarket.TerritoryGUID != null ? EditMarket.TerritoryGUID.ToString() : Guid.Empty.ToString();
                        Market.FirstName = EditMarket.FirstName;
                        Market.LastName = EditMarket.LastName;
                        Market.MobilePhone = EditMarket.MobilePhone;
                        Market.MarketPhone = EditMarket.MarketPhone;
                        Market.HomePhone = EditMarket.HomePhone;
                        Market.Emails = EditMarket.Emails;
                        Market.AddressLine1 = EditMarket.AddressLine1;
                        Market.AddressLine2 = EditMarket.AddressLine2;
                        Market.City = EditMarket.City;
                        Market.State = EditMarket.State;
                        Market.Country = EditMarket.Country;
                        Market.ZipCode = EditMarket.ZipCode;
                        Market.CreateDate = Convert.ToDateTime(EditMarket.CreateDate);
                        if (!string.IsNullOrEmpty(Market.RegionGUID))
                        {
                            var TerritoryDetails = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(Market.RegionGUID)).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
                            {
                                Value = r.TerritoryGUID.ToString(),
                                Text = r.Name
                            });
                            ViewBag.TerritoryDetails = new SelectList(TerritoryDetails, "Value", "Text");
                        }
                        return View(Market);
                    }
                    else
                    {
                        return View();
                    }

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
        public ActionResult Edit(ServicePointModel market)
        {
            Logger.Debug("Inside ServicePoint Controller- Create Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    if (ModelState.IsValid)
                    {
                        Market Market = new Market();
                        Market.MarketGUID = new Guid(market.MarketGUID);
                        Market.IsDefault = true;
                        Market.UserGUID = new Guid(market.UserGUID);
                        Market.EntityType = 0;
                        if (!string.IsNullOrEmpty(market.OrganizationGUID) && market.OrganizationGUID != Guid.Empty.ToString())
                        {
                            Market.OrganizationGUID = new Guid(market.OrganizationGUID);
                        }
                        else
                        {
                            Market.OrganizationGUID = null;
                        }
                        if (!string.IsNullOrEmpty(market.UserGUID) && market.UserGUID != Guid.Empty.ToString())
                        {
                            Market.OwnerGUID = new Guid(market.UserGUID);
                        }
                        else
                        {
                            Market.OwnerGUID = null;
                        }

                        Market.MarketName = market.MarketName;
                        if (!string.IsNullOrEmpty(market.UserGUID) && market.UserGUID != Guid.Empty.ToString())
                        {
                            Market.PrimaryContactGUID = new Guid(market.UserGUID);
                        }
                        else
                        {
                            Market.PrimaryContactGUID = null;
                        }

                        if (!string.IsNullOrEmpty(market.RegionGUID) && market.RegionGUID != Guid.Empty.ToString())
                        {
                            Market.RegionGUID = new Guid(market.RegionGUID);
                        }
                        else
                        {
                            Market.RegionGUID = null;
                        }

                        if (!string.IsNullOrEmpty(market.TerritoryGUID) && market.TerritoryGUID != Guid.Empty.ToString())
                        {
                            Market.TerritoryGUID = new Guid(market.TerritoryGUID);
                        }
                        else
                        {
                            Market.TerritoryGUID = null;
                        }

                        Market.FirstName = market.FirstName;
                        Market.LastName = market.LastName;
                        Market.MobilePhone = market.MobilePhone;
                        Market.MarketPhone = market.MarketPhone;
                        Market.HomePhone = market.HomePhone;
                        Market.Emails = market.Emails;
                        Market.AddressLine1 = market.AddressLine1;
                        Market.AddressLine2 = market.AddressLine2;
                        Market.City = market.City;
                        Market.State = market.State;
                        Market.Country = market.Country;
                        Market.ZipCode = market.ZipCode;
                        Market.IsDeleted = false;
                        Market.CreateDate = market.CreateDate;
                        Market.UpdatedDate = DateTime.UtcNow;

                        LatLong latLong = new LatLong();
                        latLong = GetLatLngCode(Market.AddressLine1, Market.AddressLine2, Market.City, Market.State, Market.Country, Market.ZipCode);
                        Market.TimeZone = getTimeZone(latLong.Latitude, latLong.Longitude).ToString();
                        Market.Latitude = latLong.Latitude;
                        Market.Longitude = latLong.Longitude;
                        int marketUpdateResult = _IMarketRepository.UpdateMarket(Market);
                        //int marketUpdateResult = _IMarketRepository.Save();
                        if (marketUpdateResult > 0)
                        {
                            return RedirectToAction("Index");
                        }

                    }
                    return View(market);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(market);
            }
        }
        public ActionResult Delete(string id = "")
        {
            Logger.Debug("Inside Market Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    ServicePointModel market = new ServicePointModel();
                    market.MarketGUID = id;
                    _IMarketRepository.DeleteMarket(new Guid(market.MarketGUID));
                    //_IMarketRepository.Save();

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