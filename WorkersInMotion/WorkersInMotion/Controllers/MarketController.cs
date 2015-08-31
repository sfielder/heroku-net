using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Xml;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.Utility;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Model.ViewModel;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class MarketController : BaseController
    {
        #region Constructor
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPeopleRepository _IPeopleRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        public MarketController()
        {
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
        }
        public MarketController(WorkersInMotionDB context)
        {
            this._IPlaceRepository = new PlaceRepository(context);
            this._IPeopleRepository = new PeopleRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IUserRepository = new UserRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
        }

        #endregion
        //
        // GET: /Market/
        public void DropdownValues(string RegionGUID = "", string TerritoryGUID = "")
        {
            var RegionDetails = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList().Select(r => new SelectListItem
            {
                Value = r.RegionGUID.ToString(),
                Text = r.Name,
            });
            ViewBag.RegionDetails = new SelectList(RegionDetails, "Value", "Text", RegionGUID);

            var TerritoryDetails = _ITerritoryRepository.GetTerritoryByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList().Select(r => new SelectListItem
            {
                Value = r.TerritoryGUID.ToString(),
                Text = r.Name
            });
            ViewBag.TerritoryDetails = new SelectList(TerritoryDetails, "Value", "Text", TerritoryGUID);
        }
        public ActionResult Index(string id = "")
        {
            Logger.Debug("Inside People Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    var placeList = new PlaceViewModel();
                    placeList.PlaceList = new List<PlaceModel>();
                    var appPlace = new List<Place>();
                    DropdownValues();
                    appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    placeList.PlaceList.Add(new PlaceModel
                    {
                        PlaceGUID = "",
                        PlaceName = "All",
                        UserGUID = "",
                        OrganizationGUID = "",
                    });
                    foreach (var place in appPlace.ToList())
                    {
                        placeList.PlaceList.Add(new PlaceModel
                        {
                            PlaceGUID = place.PlaceGUID.ToString(),
                            PlaceID = place.PlaceID,
                            PlaceName = place.PlaceName,
                            UserGUID = place.UserGUID.ToString(),
                            OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                        });
                    }

                    var marketList = new MarketViewModel();
                    marketList.MarketList = new List<MarketModel>();
                    var appMarket = new List<Market>();
                    if (string.IsNullOrEmpty(id))
                    {
                        // if (Session["UserType"].ToString() == "ENT_A")
                        {
                            appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 1).ToList();
                        }
                        //else
                        //{
                        //    appMarket = _IMarketRepository.GetMarketByUserGUID(new Guid(Session["UserGUID"].ToString()), 1).ToList();
                        //}
                    }
                    else
                    {
                        appMarket = _IMarketRepository.GetMarketByOwnerGUID(new Guid(id)).ToList();
                        ViewBag.PlaceGUID = id;
                    }
                    foreach (var market in appMarket.ToList())
                    {
                        marketList.MarketList.Add(new MarketModel
                        {
                            MarketGUID = market.MarketGUID.ToString(),
                            UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString(),
                            OrganizationGUID = market.OrganizationGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                            OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString(),
                            MarketName = market.MarketName,
                            MarketPhone = market.MarketPhone,
                            PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString(),
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
                            RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString(),
                            TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString(),
                            RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "",
                            TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "",
                        });
                    }

                    var viewModel = new MarketViewModel();
                    viewModel.Place = placeList.PlaceList.AsEnumerable();
                    viewModel.Market = marketList.MarketList.AsEnumerable();

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


        public ActionResult Create(string id = "", string customerid = "")
        {
            Logger.Debug("Inside Market Controller- Create");
            try
            {

                if (Session["OrganizationGUID"] != null)
                {
                    Session["CustomerName"] = null;
                    Session["CustomerGUID"] = customerid;
                    TempData["TabName"] = "Stores";
                    DropdownValues();

                    List<AspUser> RMUserList = new List<AspUser>();
                    var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U_RM").OrderBy(sort => sort.FirstName).ToList();
                    //if (string.IsNullOrEmpty(id))
                    {
                        RMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                        foreach (var user in appUser.ToList())
                        {
                            RMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                        }
                    }
                    List<AspUser> FMUserList = new List<AspUser>();
                    var appUserFM = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U").OrderBy(sort => sort.FirstName).ToList();
                    //if (string.IsNullOrEmpty(id))
                    {
                        FMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                        foreach (var user in appUserFM.ToList())
                        {
                            FMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                        }
                    }
                    var MarketViewForCreate = new MarketViewForCreate();
                    MarketViewForCreate.RMUser = RMUserList.AsEnumerable();
                    MarketViewForCreate.FMUser = FMUserList.AsEnumerable();

                    return View(MarketViewForCreate);
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MarketViewForCreate marketcreate)
        {
            Logger.Debug("Inside Place Controller- Create Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    TempData["TabName"] = "Stores";
                    DropdownValues();
                    if (ModelState.IsValid)
                    {
                        MarketModel market = new MarketModel();
                        market = marketcreate.MarketModel;
                        Market Market = new Market();
                        Market.MarketGUID = Guid.NewGuid();
                        Market.MarketID = market.MarketID;
                        Market.IsDefault = true;
                        if (!string.IsNullOrEmpty(market.UserGUID) && market.UserGUID != Guid.Empty.ToString())
                        {
                            Market.UserGUID = new Guid(market.UserGUID);
                        }
                        else
                        {
                            Market.UserGUID = new Guid(Session["UserGUID"].ToString());
                        }
                        Market.EntityType = 1;
                        if (!string.IsNullOrEmpty(market.OrganizationGUID) && market.OrganizationGUID != Guid.Empty.ToString())
                        {
                            Market.OrganizationGUID = new Guid(market.OrganizationGUID);
                        }
                        else
                        {
                            Market.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        }
                        if (Session["CustomerGUID"] != null && !string.IsNullOrEmpty(Session["CustomerGUID"].ToString()) && Session["CustomerGUID"].ToString() != Guid.Empty.ToString())
                        {
                            Market.OwnerGUID = new Guid(Session["CustomerGUID"].ToString());
                            Place _place = _IPlaceRepository.GetPlaceByID(new Guid(Market.OwnerGUID.ToString()));
                            if (_place != null)
                                Market.ParentID = _place.PlaceID;
                            List<Person> _people = _IPeopleRepository.GetPeopleByPlaceGUID(new Guid(Session["CustomerGUID"].ToString())).ToList();
                            if (_people != null && _people.Count > 0)
                            {
                                Person _person = _people.OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                                if (_person.PeopleGUID != Guid.Empty)
                                {
                                    Market.PrimaryContactGUID = _person.PeopleGUID;
                                }
                                else
                                {
                                    Market.PrimaryContactGUID = null;
                                }
                            }

                        }
                        else
                        {
                            Market.OwnerGUID = null;
                            Market.PrimaryContactGUID = null;
                        }
                        Market.MarketName = market.MarketName;


                        if (!string.IsNullOrEmpty(market.RegionGUID) && market.RegionGUID != Guid.Empty.ToString())
                        {
                            Market.RegionGUID = new Guid(market.RegionGUID);
                            Region _region = _IRegionRepository.GetRegionByID(new Guid(market.RegionGUID));
                            if (_region != null)
                            {
                                Market.RegionName = _region.Name;
                            }
                        }
                        else
                        {
                            Market.RegionGUID = null;
                        }
                        if (!string.IsNullOrEmpty(market.TerritoryGUID) && market.TerritoryGUID != Guid.Empty.ToString())
                        {
                            Market.TerritoryGUID = new Guid(market.TerritoryGUID);
                            Territory _territory = _ITerritoryRepository.GetTerritoryByID(new Guid(market.TerritoryGUID));
                            if (_territory != null)
                            {
                                Market.TeritoryID = _territory.TerritoryID;
                            }
                        }
                        else
                        {
                            Market.TerritoryGUID = null;
                        }

                        if (!string.IsNullOrEmpty(market.RMUserGUID) && market.RMUserGUID != Guid.Empty.ToString())
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(market.RMUserGUID));
                            if (_globalUser != null)
                            {
                                Market.RMUserID = _globalUser.USERID;
                            }
                        }
                        if (!string.IsNullOrEmpty(market.FMUserGUID) && market.FMUserGUID != Guid.Empty.ToString())
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(market.RMUserGUID));
                            if (_globalUser != null)
                            {
                                Market.FMUserID = _globalUser.USERID;
                            }
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

                        //As per disscussion with kousik
                        var lWebClient = new WebClient();
                        string lTempData = String.Format(ConfigurationManager.AppSettings.Get("ClientStoreURL"), Market.MarketID);
                        lTempData = lWebClient.DownloadString(lTempData);
                        S_POSStoreResponse lObjPOSResp = new JavaScriptSerializer().Deserialize<S_POSStoreResponse>(lTempData);
                        if (null == lObjPOSResp || !lObjPOSResp.store.apistatus.Equals("OK"))
                        {
                            //If this returns null, return not found error to the mobile
                        }
                        else
                        {
                            Market.StoreJSON = new JavaScriptSerializer().Serialize(lObjPOSResp);
                            Market.StoreJSON = Convert.ToBase64String(Encoding.UTF8.GetBytes(Market.StoreJSON));
                        }


                        int marketInsertResult = _IMarketRepository.InsertMarket(Market);
                        // int marketInsertResult = _IMarketRepository.Save();
                        if (marketInsertResult > 0)
                        {
                            return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = Market.OwnerGUID.ToString() });
                        }
                        else
                        {
                            List<AspUser> RMUserList = new List<AspUser>();
                            var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U_RM").OrderBy(sort => sort.FirstName).ToList();
                            //if (string.IsNullOrEmpty(id))
                            {
                                RMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                                foreach (var user in appUser.ToList())
                                {
                                    RMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                                }
                            }
                            List<AspUser> FMUserList = new List<AspUser>();
                            var appUserFM = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U").OrderBy(sort => sort.FirstName).ToList();
                            //if (string.IsNullOrEmpty(id))
                            {
                                FMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                                foreach (var user in appUserFM.ToList())
                                {
                                    FMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                                }
                            }
                            marketcreate.RMUser = RMUserList.AsEnumerable();
                            marketcreate.FMUser = FMUserList.AsEnumerable();
                            return View(marketcreate);
                        }

                    }
                    else
                    {
                        List<AspUser> RMUserList = new List<AspUser>();
                        var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U_RM").OrderBy(sort => sort.FirstName).ToList();
                        //if (string.IsNullOrEmpty(id))
                        {
                            RMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                            foreach (var user in appUser.ToList())
                            {
                                RMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                            }
                        }
                        List<AspUser> FMUserList = new List<AspUser>();
                        var appUserFM = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U").OrderBy(sort => sort.FirstName).ToList();
                        //if (string.IsNullOrEmpty(id))
                        {
                            FMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                            foreach (var user in appUserFM.ToList())
                            {
                                FMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                            }
                        }
                        marketcreate.RMUser = RMUserList.AsEnumerable();
                        marketcreate.FMUser = FMUserList.AsEnumerable();
                        return View(marketcreate);
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
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult Edit(string id = "", string marketguid = "")
        {
            Logger.Debug("Inside Market Controller- Create");
            try
            {

                if (Session["OrganizationGUID"] != null)
                {
                    Session["CustomerName"] = null;

                    TempData["TabName"] = "Stores";
                    Market EditMarket = _IMarketRepository.GetMarketByID(new Guid(marketguid));
                    MarketModel Market = new MarketModel();
                    if (EditMarket != null)
                    {
                        ViewBag.MarketName = EditMarket.MarketName;
                        Market.MarketGUID = EditMarket.MarketGUID.ToString();
                        Market.MarketID = !string.IsNullOrEmpty(EditMarket.MarketID) ? EditMarket.MarketID.ToString() : "";
                        Market.IsDefault = Convert.ToBoolean(EditMarket.IsDefault);
                        Market.UserGUID = EditMarket.UserGUID != null ? EditMarket.UserGUID.ToString() : Guid.Empty.ToString();
                        Market.EntityType = Convert.ToInt32(EditMarket.EntityType);
                        Market.OrganizationGUID = EditMarket.OrganizationGUID != null ? EditMarket.OrganizationGUID.ToString() : Guid.Empty.ToString();
                        Market.OwnerGUID = EditMarket.OwnerGUID != null ? EditMarket.OwnerGUID.ToString() : Guid.Empty.ToString();
                        Session["CustomerGUID"] = Market.OwnerGUID;
                        //   Market.ContactName = EditMarket.PrimaryContactGUID != null ? _IPeopleRepository.GetPeopleNameByPeopleGUID(new Guid(EditMarket.PrimaryContactGUID.ToString())) : "";
                        Market.MarketName = EditMarket.MarketName;
                        Market.PrimaryContactGUID = EditMarket.PrimaryContactGUID != null ? EditMarket.PrimaryContactGUID.ToString() : Guid.Empty.ToString();
                        Market.RegionGUID = EditMarket.RegionGUID != null ? EditMarket.RegionGUID.ToString() : Guid.Empty.ToString();
                        Market.TerritoryGUID = EditMarket.TerritoryGUID != null ? EditMarket.TerritoryGUID.ToString() : Guid.Empty.ToString();
                        Market.RegionName = EditMarket.RegionName;
                        Market.TerritoryID = EditMarket.TeritoryID;

                        if (!string.IsNullOrEmpty(EditMarket.RMUserID))
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(EditMarket.RMUserID, Session["OrganizationGUID"].ToString());
                            if (_globalUser != null)
                            {
                                Market.RMUserGUID = _globalUser.UserGUID.ToString();
                                UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, new Guid(Market.OrganizationGUID));
                                if (_userprofile != null)
                                {
                                    Market.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                }
                            }
                            else
                            {
                                Market.RMUserGUID = Guid.Empty.ToString();

                            }

                        }
                        if (!string.IsNullOrEmpty(EditMarket.FMUserID))
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(EditMarket.FMUserID, Session["OrganizationGUID"].ToString());
                            if (_globalUser != null)
                            {
                                Market.FMUserGUID = _globalUser.UserGUID.ToString();
                                UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, new Guid(Market.OrganizationGUID));
                                if (_userprofile != null)
                                {
                                    Market.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                }
                            }
                            else
                            {
                                Market.FMUserGUID = Guid.Empty.ToString();
                            }

                        }
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


                    }


                    DropdownValues();
                    if (Market != null && !string.IsNullOrEmpty(Market.RegionGUID))
                    {
                        var TerritoryDetails = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(Market.RegionGUID)).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
                        {
                            Value = r.TerritoryGUID.ToString(),
                            Text = r.Name
                        });
                        ViewBag.TerritoryDetails = new SelectList(TerritoryDetails, "Value", "Text");

                    }

                    List<AspUser> RMUserList = new List<AspUser>();
                    var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U_RM").OrderBy(sort => sort.FirstName).ToList();
                    //if (string.IsNullOrEmpty(id))
                    {
                        RMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                        foreach (var user in appUser.ToList())
                        {
                            RMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                        }
                    }
                    List<AspUser> FMUserList = new List<AspUser>();
                    var appUserFM = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U").OrderBy(sort => sort.FirstName).ToList();
                    //if (string.IsNullOrEmpty(id))
                    {
                        FMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                        foreach (var user in appUserFM.ToList())
                        {
                            FMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                        }
                    }
                    var MarketViewForCreate = new MarketViewForCreate();
                    MarketViewForCreate.RMUser = RMUserList.AsEnumerable();
                    MarketViewForCreate.FMUser = FMUserList.AsEnumerable();
                    MarketViewForCreate.MarketModel = Market;
                    return View(MarketViewForCreate);
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MarketViewForCreate marketcreate)
        {
            Logger.Debug("Inside Place Controller- Edit Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    TempData["TabName"] = "Stores";
                    ViewBag.MarketName = !string.IsNullOrEmpty(marketcreate.MarketModel.MarketName) ? marketcreate.MarketModel.MarketName.ToString() : _IMarketRepository.GetMarketByID(new Guid(marketcreate.MarketModel.MarketGUID)).MarketName;
                    DropdownValues();
                    if (ModelState.IsValid)
                    {
                        MarketModel market = new MarketModel();
                        market = marketcreate.MarketModel;
                        Market Market = new Market();
                        Market.MarketGUID = new Guid(market.MarketGUID);
                        Market.MarketID = market.MarketID;
                        Market.IsDefault = true;
                        if (!string.IsNullOrEmpty(market.UserGUID) && market.UserGUID != Guid.Empty.ToString())
                        {
                            Market.UserGUID = new Guid(market.UserGUID);
                        }
                        else
                        {
                            Market.UserGUID = null;
                        }
                        Market.EntityType = market.EntityType;
                        if (!string.IsNullOrEmpty(market.OrganizationGUID) && market.OrganizationGUID != Guid.Empty.ToString())
                        {
                            Market.OrganizationGUID = new Guid(market.OrganizationGUID);
                        }
                        else
                        {
                            Market.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        }
                        if (!string.IsNullOrEmpty(market.OwnerGUID) && market.OwnerGUID != Guid.Empty.ToString())
                        {
                            Market.OwnerGUID = new Guid(market.OwnerGUID);
                        }
                        else
                        {
                            Market.OwnerGUID = null;
                        }
                        Market.MarketName = market.MarketName;
                        if (!string.IsNullOrEmpty(market.PrimaryContactGUID) && market.PrimaryContactGUID != Guid.Empty.ToString())
                        {
                            Market.PrimaryContactGUID = new Guid(market.PrimaryContactGUID);
                        }
                        else
                        {
                            Market.PrimaryContactGUID = null;
                        }
                        if (!string.IsNullOrEmpty(market.RegionGUID) && market.RegionGUID != Guid.Empty.ToString())
                        {
                            Market.RegionGUID = new Guid(market.RegionGUID);
                            Region _region = _IRegionRepository.GetRegionByID(new Guid(market.RegionGUID));
                            if (_region != null)
                            {
                                Market.RegionName = _region.Name;
                            }
                        }
                        else
                        {
                            Market.RegionGUID = null;
                        }
                        if (!string.IsNullOrEmpty(market.TerritoryGUID) && market.TerritoryGUID != Guid.Empty.ToString())
                        {
                            Market.TerritoryGUID = new Guid(market.TerritoryGUID);
                            Territory _territory = _ITerritoryRepository.GetTerritoryByID(new Guid(market.TerritoryGUID));
                            if (_territory != null)
                            {
                                Market.TeritoryID = _territory.TerritoryID;
                            }
                        }
                        else
                        {
                            Market.TerritoryGUID = null;
                        }
                        if (!string.IsNullOrEmpty(market.RMUserGUID) && market.RMUserGUID != Guid.Empty.ToString())
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(market.RMUserGUID));
                            if (_globalUser != null)
                            {
                                Market.RMUserID = _globalUser.USERID;
                            }
                        }
                        if (!string.IsNullOrEmpty(market.FMUserGUID) && market.FMUserGUID != Guid.Empty.ToString())
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(market.FMUserGUID));
                            if (_globalUser != null)
                            {
                                Market.FMUserID = _globalUser.USERID;
                            }
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
                        Market.CreateDate = Convert.ToDateTime(market.CreateDate);
                        Market.UpdatedDate = DateTime.UtcNow;
                        Market.IsDeleted = false;

                        LatLong latLong = new LatLong();
                        latLong = GetLatLngCode(Market.AddressLine1, Market.AddressLine2, Market.City, Market.State, Market.Country, Market.ZipCode);
                        Market.TimeZone = getTimeZone(latLong.Latitude, latLong.Longitude).ToString();
                        Market.Latitude = latLong.Latitude;
                        Market.Longitude = latLong.Longitude;


                        //As per disscussion with kousik
                        var lWebClient = new WebClient();
                        string lTempData = String.Format(ConfigurationManager.AppSettings.Get("ClientStoreURL"), Market.MarketID);
                        lTempData = lWebClient.DownloadString(lTempData);
                        S_POSStoreResponse lObjPOSResp = new JavaScriptSerializer().Deserialize<S_POSStoreResponse>(lTempData);
                        if (null == lObjPOSResp || !lObjPOSResp.store.apistatus.Equals("OK"))
                        {
                            //If this returns null, return not found error to the mobile
                        }
                        else
                        {
                            Market.StoreJSON = new JavaScriptSerializer().Serialize(lObjPOSResp);
                            Market.StoreJSON = Convert.ToBase64String(Encoding.UTF8.GetBytes(Market.StoreJSON));
                        }


                        int marketUpdateResult = _IMarketRepository.UpdateMarket(Market);
                        //int marketUpdateResult = _IMarketRepository.Save();
                        if (marketUpdateResult > 0)
                        {
                            return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = Market.OwnerGUID.ToString() });
                        }
                        else
                        {
                            List<AspUser> RMUserList = new List<AspUser>();
                            var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U_RM").OrderBy(sort => sort.FirstName).ToList();
                            //if (string.IsNullOrEmpty(id))
                            {
                                RMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                                foreach (var user in appUser.ToList())
                                {
                                    RMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                                }
                            }
                            List<AspUser> FMUserList = new List<AspUser>();
                            var appUserFM = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U").OrderBy(sort => sort.FirstName).ToList();
                            //if (string.IsNullOrEmpty(id))
                            {
                                FMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                                foreach (var user in appUserFM.ToList())
                                {
                                    FMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                                }
                            }

                            marketcreate.RMUser = RMUserList.AsEnumerable();
                            marketcreate.FMUser = FMUserList.AsEnumerable();

                            return View(marketcreate);
                        }

                    }
                    else
                    {
                        List<AspUser> RMUserList = new List<AspUser>();
                        var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U_RM").OrderBy(sort => sort.FirstName).ToList();
                        //if (string.IsNullOrEmpty(id))
                        {
                            RMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                            foreach (var user in appUser.ToList())
                            {
                                RMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                            }
                        }
                        List<AspUser> FMUserList = new List<AspUser>();
                        var appUserFM = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), "ENT_U").OrderBy(sort => sort.FirstName).ToList();
                        //if (string.IsNullOrEmpty(id))
                        {
                            FMUserList.Add(new AspUser { FirstName = "None", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                            foreach (var user in appUserFM.ToList())
                            {
                                FMUserList.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                            }
                        }
                        marketcreate.RMUser = RMUserList.AsEnumerable();
                        marketcreate.FMUser = FMUserList.AsEnumerable();
                        return View(marketcreate);
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
                return RedirectToAction("Login", "User");
            }
        }


        public ActionResult Delete(string id = "", string customerguid = "")
        {
            Logger.Debug("Inside Market Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    MarketModel market = new MarketModel();
                    market.MarketGUID = id;
                    _IMarketRepository.DeleteMarket(new Guid(market.MarketGUID));
                    // _IMarketRepository.Save();

                    return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = customerguid });
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

        public JsonResult SaveItem(string _marketModel)
        {
            //ViewBag.PeopleModelItem = _peopleModel;
            Logger.Debug("Inside People Controller- SaveItem");
            JsonResult result = new JsonResult();
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var people = js.Deserialize<dynamic>(_marketModel);
                MarketModel Market = new MarketModel();
                Market.MarketName = people["MarketName"];
                Market.RegionGUID = people["RegionGUID"];
                Market.TerritoryGUID = people["TerritoryGUID"];
                Market.FirstName = people["FirstName"];
                Market.LastName = people["LastName"];
                Market.MobilePhone = people["MobilePhone"];
                Market.MarketPhone = people["MarketPhone"];
                Market.HomePhone = people["HomePhone"];
                Market.Emails = people["Email"];
                Market.AddressLine1 = people["Address1"];
                Market.AddressLine2 = people["Address2"];
                Market.City = people["City"];
                Market.State = people["State"];
                Market.Country = people["Country"];
                Market.ZipCode = people["ZipCode"];

                TempData["MarketModel"] = Market;
                result.Data = "success";
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                result.Data = "failure";
                return result;
            }
            //  people["ID"] 

        }


        public ActionResult Contact(string id = "")
        {
            Logger.Debug("Inside People Controller- Create");
            try
            {

                if (Session["OrganizationGUID"] != null)
                {
                    var userList = new AspNetUserViewModel();
                    userList.Users = new List<AspUser>();
                    var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();
                    //if (string.IsNullOrEmpty(id))
                    {
                        userList.Users.Add(new AspUser { FirstName = "All", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                        foreach (var user in appUser.ToList())
                        {
                            userList.Users.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                        }
                    }

                    var placeList = new PlaceViewModel();
                    placeList.PlaceList = new List<PlaceModel>();
                    var appPlace = new List<Place>();
                    if (string.IsNullOrEmpty(id) || Guid.Empty == new Guid(id))
                    {
                        appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                        if (!string.IsNullOrEmpty(id) && Guid.Empty == new Guid(id))
                            ViewBag.UserID = Guid.Empty.ToString();
                    }
                    else
                    {
                        appPlace = _IPlaceRepository.GetPlaceByUserGUID(new Guid(id)).ToList();
                        ViewBag.UserID = id;
                    }
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
                    var placeviewModel = new PlaceViewModel();
                    placeviewModel.Place = placeList.PlaceList.AsEnumerable();
                    placeviewModel.User = userList.Users.AsEnumerable();

                    var PeopleViewForCreate = new PeopleViewForCreate();
                    PeopleViewForCreate.PlaceViewModel = placeviewModel;
                    if (TempData["PeopleModel"] != null)
                        PeopleViewForCreate.PeopleModel = (PeopleModel)TempData["PeopleModel"];
                    //if (string.IsNullOrEmpty(id))
                    return View(PeopleViewForCreate);
                    //else
                    //    return PartialView("../Place/_customer", PeopleViewForCreate);
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(PeopleViewForCreate peoplecreate)
        {
            Logger.Debug("Inside Place Controller- Create Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        PeopleModel people = new PeopleModel();
                        people = peoplecreate.PeopleModel;
                        Person People = new Person();
                        People.PeopleGUID = Guid.NewGuid();

                        People.UserGUID = new Guid(people.UserGUID.ToString());
                        if (!string.IsNullOrEmpty(people.OrganizationGUID.ToString()) && people.OrganizationGUID != Guid.Empty.ToString())
                        {
                            People.OrganizationGUID = new Guid(people.OrganizationGUID.ToString());
                        }
                        else
                        {
                            People.OrganizationGUID = null;
                        }
                        People.IsPrimaryContact = true;
                        if (!string.IsNullOrEmpty(people.PlaceGUID.ToString()) && people.PlaceGUID != Guid.Empty.ToString())
                        {
                            People.PlaceGUID = new Guid(people.PlaceGUID.ToString());
                        }
                        else
                        {
                            People.PlaceGUID = null;
                        }
                        People.FirstName = people.FirstName;
                        People.LastName = people.LastName;
                        People.MobilePhone = people.MobilePhone;
                        People.CompanyName = people.CompanyName;
                        People.BusinessPhone = people.BusinessPhone;
                        People.HomePhone = people.HomePhone;
                        People.Emails = people.Emails;
                        People.AddressLine1 = people.AddressLine1;
                        People.AddressLine2 = people.AddressLine2;
                        People.City = people.City;
                        People.State = people.State;
                        People.Country = people.Country;
                        People.ZipCode = people.ZipCode;
                        People.CategoryID = 0;
                        People.IsDeleted = false;
                        People.CreatedDate = DateTime.UtcNow;
                        People.UpdatedDate = DateTime.UtcNow;


                        int peopleInsertResult = _IPeopleRepository.InsertPeople(People);
                        //int peopleInsertResult = _IPeopleRepository.Save();
                        if (peopleInsertResult > 0)
                        {
                            ContactValues contactValues = new ContactValues();
                            contactValues.UserGUID = People.UserGUID;
                            contactValues.OrganizationGUID = People.OrganizationGUID != null ? new Guid(People.OrganizationGUID.ToString()) : Guid.Empty;
                            contactValues.OwnerGUID = People.PlaceGUID != null ? new Guid(People.PlaceGUID.ToString()) : Guid.Empty;
                            contactValues.PrimaryContactGUID = People.PeopleGUID;
                            contactValues.ContactName = People.FirstName;
                            TempData["ContactDetails"] = contactValues;
                            return RedirectToAction("Create");
                        }
                        else
                        {
                            return View(peoplecreate);
                        }

                    }
                    else
                    {
                        var userList = new AspNetUserViewModel();
                        userList.Users = new List<AspUser>();
                        var appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();
                        //if (string.IsNullOrEmpty(id))
                        {
                            userList.Users.Add(new AspUser { FirstName = "All", LastName = "", UserGUID = Guid.Empty.ToString(), OrganizationGUID = "" });
                            foreach (var user in appUser.ToList())
                            {
                                userList.Users.Add(new AspUser { FirstName = user.FirstName, LastName = user.LastName, UserGUID = user.UserGUID.ToString() });
                            }
                        }

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
                        var placeviewModel = new PlaceViewModel();
                        placeviewModel.Place = placeList.PlaceList.AsEnumerable();
                        placeviewModel.User = userList.Users.AsEnumerable();

                        var PeopleViewForCreate = new PeopleViewForCreate();
                        PeopleViewForCreate.PlaceViewModel = placeviewModel;
                        return View(PeopleViewForCreate);
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
                return View(peoplecreate);
            }
        }

        public JsonResult SaveItemContact(string _peopleModel)
        {
            //ViewBag.PeopleModelItem = _peopleModel;
            Logger.Debug("Inside People Controller- SaveItem");
            JsonResult result = new JsonResult();
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var people = js.Deserialize<dynamic>(_peopleModel);
                PeopleModel People = new PeopleModel();
                People.FirstName = people["FirstName"];
                People.LastName = people["LastName"];
                People.MobilePhone = people["MobilePhone"];
                People.BusinessPhone = people["BusinessPhone"];
                People.HomePhone = people["HomePhone"];
                People.Emails = people["Email"];
                People.AddressLine1 = people["Address1"];
                People.AddressLine2 = people["Address2"];
                People.City = people["City"];
                People.State = people["State"];
                People.Country = people["Country"];
                People.ZipCode = people["ZipCode"];

                TempData["PeopleModel"] = People;
                result.Data = "success";
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                result.Data = "failure";
                return result;
            }
            //  return View("Create",Create(people["ID"], );
            //  people["ID"] 

        }



    }
}