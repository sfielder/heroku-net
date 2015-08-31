using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    public class CustomerViewController : BaseController
    {
        #region Constructor
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IOrganizationSubscriptionRepository _IOrganizationSubscriptionRepository;
        private readonly IUserSubscriptionRepository _IUserSubscriptionRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPeopleRepository _IPeopleRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        public CustomerViewController()
        {
            this._IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
            this._IOrganizationSubscriptionRepository = new OrganizationSubscriptionRepository(new WorkersInMotionDB());
            this._IUserSubscriptionRepository = new UserSubscriptionRepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
        }
        public CustomerViewController(WorkersInMotionDB context)
        {
            this._IOrganizationRepository = new OrganizationRepository(context);
            this._IUserRepository = new UserRepository(context);
            this._IOrganizationSubscriptionRepository = new OrganizationSubscriptionRepository(context);
            this._IUserSubscriptionRepository = new UserSubscriptionRepository(context);
            this._IPlaceRepository = new PlaceRepository(context);
            this._IPeopleRepository = new PeopleRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
        }

        #endregion

        public void pageContactCountList(int selectedValue)
        {
            if (selectedValue == 0)
                selectedValue = 5;
            ViewBag.pageContactCountValue = selectedValue;
            var countList = new List<SelectListItem>
            {
                         new SelectListItem { Text = "5", Value = "5"},
                         new SelectListItem { Text = "15", Value = "15"},
                         new SelectListItem { Text = "20", Value = "20"}
           };
            ViewBag.Contact_pCountList = new SelectList(countList, "Value", "Text", selectedValue);
        }
        public void pageStoreCountList(int selectedValue)
        {
            if (selectedValue == 0)
                selectedValue = 5;
            ViewBag.pageStoreCountValue = selectedValue;
            var countList = new List<SelectListItem>
            {
                         new SelectListItem { Text = "5", Value = "5"},
                         new SelectListItem { Text = "15", Value = "15"},
                         new SelectListItem { Text = "20", Value = "20"}
           };
            ViewBag.Store_pCountList = new SelectList(countList, "Value", "Text", selectedValue);
        }

        //
        // GET: /CustomerView/
        public ActionResult Index(string id = "", string customerid = "", string regionguid = "", string selection = "", string Contact_RowCount = "", string Store_RowCount = "", int page = 1, string Contact_Search = "", string Store_Search = "")
        {
            Logger.Debug("Inside CustomerView Controller- Index");
            try
            {
                ViewBag.CustomerID = customerid;
                ViewBag.RegionGUID = regionguid;
                int contact_TotalPage = 0;
                int contact_TotalRecord = 0;
                int contact_pCount = 0;

                int store_TotalPage = 0;
                int store_TotalRecord = 0;
                int store_pCount = 0;

                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(Contact_RowCount))
                    {
                        int.TryParse(Contact_RowCount, out contact_pCount);
                        pageContactCountList(contact_pCount);

                    }
                    else
                    {
                        pageContactCountList(contact_pCount);
                    }
                    if (!string.IsNullOrEmpty(Store_RowCount))
                    {
                        int.TryParse(Store_RowCount, out store_pCount);
                        pageStoreCountList(store_pCount);

                    }
                    else
                    {
                        pageStoreCountList(store_pCount);
                    }


                    customerview pcustomerview = new customerview();
                    pcustomerview.PeopleViewModel = new PeopleViewModel();
                    pcustomerview.MarketViewModel = new MarketViewModel();
                    pcustomerview.PlaceModel = new PlaceModel();


                    if (!string.IsNullOrEmpty(customerid))
                    {
                        ViewBag.CustomerID = customerid;
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<div class='actions'>");
                        sb.Append("<div class='btn-group'>");
                        if (!string.IsNullOrEmpty(regionguid))
                        {
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> " + _IRegionRepository.GetRegionNameByRegionGUID(new Guid(regionguid)) + " <i class='icon-angle-down'></i></a>");
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(selection) && selection == "All")
                            {
                                sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> All <i class='icon-angle-down'></i></a>");
                            }
                            else
                            {
                                sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Select Region <i class='icon-angle-down'></i></a>");
                            }
                        }
                        sb.Append("<ul id='ulworkgroup' style='height:100px;overflow-y:scroll' class='dropdown-menu pull-right'>");
                        if (string.IsNullOrEmpty(selection) || selection != "All")
                        {
                            //sb.Append("<li><a href=" + Url.Action("Index", "CustomerView", new { id = "Stores", customerid = customerid, selection = "All" }) + ">All</a></li>");
                            sb.Append("<li><a onclick=\"RedirectAction('Stores','');\">All</a></li>");
                        }
                        List<Region> RegionList = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                        foreach (Region item in RegionList)
                        {
                            //sb.Append("<li><a href=" + Url.Action("Index", "CustomerView", new { id = "Stores", customerid = customerid, regionguid = item.RegionGUID.ToString() }) + " data-groupguid=" + item.RegionGUID + ">" + item.Name + "</a></li>");
                            sb.Append("<li><a onclick=\"RedirectAction('Stores','" + item.RegionGUID + "','" + customerid + "');\" data-groupguid=" + item.RegionGUID + ">" + item.Name + "</a></li>");
                        }
                        sb.Append("</ul>");
                        sb.Append("</div>");
                        sb.Append("</div>");
                        ViewBag.RegionList = sb.ToString();

                        Session["CustomerGUID"] = customerid;
                        if (!string.IsNullOrEmpty(id))
                        {
                            TempData["TabName"] = id;
                        }
                        else
                        {
                            TempData["TabName"] = "Details";
                        }
                        #region Contact Details

                        var peopleList = new PeopleViewModel();
                        peopleList.PeopleList = new List<PeopleModel>();
                        var appPeople = new List<Person>();

                        appPeople = _IPeopleRepository.GetPeopleByPlaceGUID(new Guid(customerid)).ToList();
                        ViewBag.PlaceGUID = customerid;

                        if (appPeople != null && appPeople.Count > 0)
                        {
                            ViewBag.Contact_Search = Contact_Search;
                            if (!string.IsNullOrEmpty(Contact_Search))
                            {
                                Contact_Search = Contact_Search.ToLower();
                                appPeople = appPeople.Where(
                                    p => (!String.IsNullOrEmpty(p.FirstName) && p.FirstName.ToLower().StartsWith(Contact_Search))
                                || (!String.IsNullOrEmpty(p.BusinessPhone) && p.BusinessPhone.ToLower().StartsWith(Contact_Search))
                                || (!String.IsNullOrEmpty(p.MobilePhone) && p.MobilePhone.ToLower().StartsWith(Contact_Search))
                                || (!String.IsNullOrEmpty(p.Emails) && p.Emails.ToLower().StartsWith(Contact_Search))).ToList();
                            }



                            contact_TotalRecord = appPeople.ToList().Count;
                            contact_TotalPage = (contact_TotalRecord / (int)ViewBag.pageContactCountValue) + ((contact_TotalRecord % (int)ViewBag.pageContactCountValue) > 0 ? 1 : 0);

                            ViewBag.Contact_TotalRows = contact_TotalRecord;
                            appPeople = appPeople.OrderBy(a => a.OrganizationGUID).Skip(((page - 1) * (int)ViewBag.pageContactCountValue)).Take((int)ViewBag.pageContactCountValue).ToList();

                            foreach (var people in appPeople.ToList())
                            {
                                peopleList.PeopleList.Add(new PeopleModel
                                {
                                    PeopleGUID = people.PeopleGUID.ToString(),
                                    PlaceGUID = people.PlaceGUID.ToString(),
                                    CompanyName = people.CompanyName,
                                    BusinessPhone = people.BusinessPhone,
                                    MarketGUID = people.MarketGUID.ToString(),
                                    FirstName = people.FirstName,
                                    LastName = people.LastName,
                                    UserGUID = people.UserGUID.ToString(),
                                    OrganizationGUID = people.OrganizationGUID.ToString(),
                                    MobilePhone = people.MobilePhone,
                                    HomePhone = people.HomePhone,
                                    Emails = people.Emails,
                                    AddressLine1 = people.AddressLine1,
                                    AddressLine2 = people.AddressLine2,
                                    City = people.City,
                                    State = people.State,
                                    Country = people.Country,
                                    ZipCode = people.ZipCode,
                                    StoreID = getStoreID(people.PeopleGUID.ToString())

                                });
                            }
                        }

                        var viewModel = new PeopleViewModel();
                        // viewModel.Place = placeList.PlaceList.AsEnumerable();
                        viewModel.People = peopleList.PeopleList.AsEnumerable();
                        pcustomerview.PeopleViewModel = viewModel;
                        #endregion

                        #region Customer Stop Details

                        var marketList = new MarketViewModel();
                        marketList.MarketList = new List<MarketModel>();
                        var appMarket = new List<Market>();
                        if (!string.IsNullOrEmpty(regionguid))
                        {
                            TempData["TabName"] = "Stores";
                            appMarket = _IMarketRepository.GetMarketByOwnerandRegionGUID(new Guid(regionguid), new Guid(customerid), 1).ToList();
                        }
                        else
                        {
                            appMarket = _IMarketRepository.GetMarketByOwnerGUID(new Guid(customerid)).ToList();
                        }

                        if (appMarket != null && appMarket.Count > 0)
                        {
                            ViewBag.Store_Search = Store_Search;
                            if (!string.IsNullOrEmpty(Store_Search))
                            {
                                Store_Search = Store_Search.ToLower();
                                appMarket = appMarket.Where(
                                    p => (!String.IsNullOrEmpty(p.MarketName) && p.MarketName.ToLower().StartsWith(Store_Search))
                                || (!String.IsNullOrEmpty(p.MarketID) && p.MarketID.ToLower().StartsWith(Store_Search))
                                || (!String.IsNullOrEmpty(_IUserProfileRepository.GetUserProfileByUserID(new Guid(p.UserGUID.ToString()), new Guid(p.OrganizationGUID.ToString())).FirstName) && (_IUserProfileRepository.GetUserProfileByUserID(new Guid(p.UserGUID.ToString()), new Guid(p.OrganizationGUID.ToString())).FirstName).ToLower().Contains(Store_Search))
                                || (!String.IsNullOrEmpty(p.MarketPhone) && p.MarketPhone.ToLower().StartsWith(Store_Search))
                                || (!String.IsNullOrEmpty(p.Emails) && p.Emails.ToLower().StartsWith(Store_Search))).ToList();
                            }

                            store_TotalRecord = appMarket.ToList().Count;
                            store_TotalPage = (store_TotalRecord / (int)ViewBag.pageStoreCountValue) + ((store_TotalRecord % (int)ViewBag.pageStoreCountValue) > 0 ? 1 : 0);

                            ViewBag.Store_TotalRows = store_TotalRecord;
                            appMarket = appMarket.OrderBy(a => a.OrganizationGUID).Skip(((page - 1) * (int)ViewBag.pageStoreCountValue)).Take((int)ViewBag.pageStoreCountValue).ToList();

                            foreach (var market in appMarket.ToList())
                            {
                                MarketModel MarketModel = new MarketModel();

                                MarketModel.MarketID = market.MarketID;
                                MarketModel.MarketGUID = market.MarketGUID.ToString();
                                MarketModel.UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString();
                                MarketModel.OrganizationGUID = market.OrganizationGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString();
                                MarketModel.OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString();
                                MarketModel.MarketName = market.MarketName;
                                MarketModel.MarketPhone = market.MarketPhone;
                                MarketModel.PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString();
                                MarketModel.FirstName = market.FirstName;

                                if (!string.IsNullOrEmpty(market.RMUserID))
                                {
                                    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(market.RMUserID, Session["OrganizationGUID"].ToString());
                                    if (_globalUser != null)
                                    {
                                        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, new Guid(MarketModel.OrganizationGUID));
                                        if (_userprofile != null)
                                        {
                                            MarketModel.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                        }
                                        else
                                        {
                                            MarketModel.RMName = "";
                                        }
                                    }

                                }
                                MarketModel.LastName = market.LastName;
                                MarketModel.MobilePhone = market.MobilePhone;
                                MarketModel.HomePhone = market.HomePhone;
                                MarketModel.Emails = market.Emails;
                                MarketModel.AddressLine1 = market.AddressLine1;
                                MarketModel.AddressLine2 = market.AddressLine2;
                                MarketModel.City = market.City;
                                MarketModel.State = market.State;
                                MarketModel.Country = market.Country;
                                MarketModel.ZipCode = market.ZipCode;
                                MarketModel.RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString();
                                MarketModel.TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString();
                                MarketModel.RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "";
                                MarketModel.TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "";
                                marketList.MarketList.Add(MarketModel);
                            }
                        }

                        var mviewModel = new MarketViewModel();
                        //  mviewModel.Place = placeList.PlaceList.AsEnumerable();
                        mviewModel.Market = marketList.MarketList.AsEnumerable();
                        pcustomerview.MarketViewModel = mviewModel;
                        #endregion

                        #region Customer Edit
                        PlaceModel lplace = new PlaceModel();
                        lplace.PlaceGUID = customerid;
                        Place Place = _IPlaceRepository.GetPlaceByID(new Guid(lplace.PlaceGUID));
                        if (Place != null)
                        {
                            lplace.PlaceGUID = Place.PlaceGUID.ToString();
                            lplace.PlaceID = Place.PlaceID;
                            lplace.UserGUID = Place.UserGUID.ToString();
                            lplace.OrganizationGUID = Place.OrganizationGUID != null ? Place.OrganizationGUID.ToString() : Guid.Empty.ToString();
                            lplace.PlaceName = Place.PlaceName;
                            lplace.FirstName = Place.FirstName;
                            lplace.LastName = Place.LastName;
                            lplace.MobilePhone = Place.MobilePhone;
                            lplace.PlacePhone = Place.PlacePhone;
                            lplace.HomePhone = Place.HomePhone;
                            lplace.Emails = Place.Emails;
                            lplace.AddressLine1 = Place.AddressLine1;
                            lplace.AddressLine2 = Place.AddressLine2;
                            lplace.City = Place.City;
                            lplace.State = Place.State;
                            lplace.Country = Place.Country;
                            lplace.ZipCode = Place.ZipCode;
                            Session["PlaceName"] = Place.PlaceName;
                            pcustomerview.PlaceModel = lplace;
                            ViewBag.ClientID = lplace.PlaceGUID.ToString();
                            //UserProfile _userProfile = _IUserProfileRepository.GetUserProfileByUserID(Place.UserGUID);
                            //if (_userProfile != null)
                            //{
                            //    pcustomerview.ManagerName = _userProfile.FirstName + " " + _userProfile.LastName;
                            //}
                            //else
                            //{
                            //    pcustomerview.ManagerName = "";
                            //}

                        }
                        #endregion

                        if (!string.IsNullOrEmpty(Contact_RowCount))
                            ViewBag.pageContactCountValue = int.Parse(Contact_RowCount);
                        else
                            ViewBag.pageContactCountValue = 5;
                        if (!string.IsNullOrEmpty(Store_RowCount))
                            ViewBag.pageStoreCountValue = int.Parse(Store_RowCount);
                        else
                            ViewBag.pageStoreCountValue = 5;
                        bool contact = false;
                        bool store = false;
                        if (null != Request && System.Text.RegularExpressions.Regex.IsMatch(Request.Url.ToString(), string.Format(@"\b{0}\b", "Contacts")))
                            contact = true;
                        if (null != Request && System.Text.RegularExpressions.Regex.IsMatch(Request.Url.ToString(), string.Format(@"\b{0}\b", "Stores")))
                            store = true;

                        if (contact)
                            TempData["TabName"] = "Contacts";
                        else if (store)
                            TempData["TabName"] = "Stores";
                    }
                    return View(pcustomerview);
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
        public ActionResult EditCustomer(customerview pcustomerview)
        {
            Logger.Debug("Inside Place Controller- Edit Http Post");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        if (pcustomerview.PlaceModel != null)
                        {
                            Place Place = new Place();
                            Place.PlaceGUID = new Guid(pcustomerview.PlaceModel.PlaceGUID);
                            Place.UserGUID = new Guid(pcustomerview.PlaceModel.UserGUID);
                            Place.PlaceID = pcustomerview.PlaceModel.PlaceID;
                            Place.OrganizationGUID = !string.IsNullOrEmpty(pcustomerview.PlaceModel.OrganizationGUID) ? new Guid(pcustomerview.PlaceModel.OrganizationGUID) : Guid.Empty;
                            Place.PlaceName = pcustomerview.PlaceModel.PlaceName;
                            Place.FirstName = pcustomerview.PlaceModel.FirstName;
                            Place.LastName = pcustomerview.PlaceModel.LastName;
                            Place.MobilePhone = pcustomerview.PlaceModel.MobilePhone;
                            Place.PlacePhone = pcustomerview.PlaceModel.PlacePhone;
                            Place.HomePhone = pcustomerview.PlaceModel.HomePhone;
                            Place.Emails = pcustomerview.PlaceModel.Emails;
                            Place.AddressLine1 = pcustomerview.PlaceModel.AddressLine1;
                            Place.AddressLine2 = pcustomerview.PlaceModel.AddressLine2;
                            Place.City = pcustomerview.PlaceModel.City;
                            Place.State = pcustomerview.PlaceModel.State;
                            Place.Country = pcustomerview.PlaceModel.Country;
                            Place.ZipCode = pcustomerview.PlaceModel.ZipCode;
                            Place.UpdatedDate = DateTime.UtcNow;
                            LatLong latLong = new LatLong();
                            latLong = GetLatLngCode(Place.AddressLine1, Place.AddressLine2, Place.City, Place.State, Place.Country, Place.ZipCode);
                            Place.TimeZone = getTimeZone(latLong.Latitude, latLong.Longitude).ToString();

                            int placeInsertResult = _IPlaceRepository.UpdatePlace(Place);
                            // int placeInsertResult = _IPlaceRepository.Save();
                            if (placeInsertResult > 0)
                            {
                                //return RedirectToAction("Index", "CustomerView", new { id = "Details", customerid = Place.PlaceGUID.ToString() });
                                return RedirectToAction("Index", "Place");
                            }
                            else
                            {
                                TempData["TabName"] = "Details";
                                pcustomerview.PeopleViewModel = ContactDetails(pcustomerview.PlaceModel.PlaceGUID);
                                pcustomerview.MarketViewModel = CustomerStopDetails(pcustomerview.PlaceModel.PlaceGUID);
                            }
                        }
                    }
                    else
                    {
                        TempData["TabName"] = "Details";
                        pcustomerview.PeopleViewModel = ContactDetails(pcustomerview.PlaceModel.PlaceGUID);
                        pcustomerview.MarketViewModel = CustomerStopDetails(pcustomerview.PlaceModel.PlaceGUID);

                    }
                    return View("Index", pcustomerview);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(pcustomerview);
            }
        }
        private PeopleViewModel ContactDetails(string PlaceGUID)
        {
            try
            {
                var peopleList = new PeopleViewModel();
                peopleList.PeopleList = new List<PeopleModel>();
                var appPeople = new List<Person>();

                appPeople = _IPeopleRepository.GetPeopleByPlaceGUID(new Guid(PlaceGUID)).ToList();
                ViewBag.PlaceGUID = PlaceGUID;


                foreach (var people in appPeople.ToList())
                {
                    peopleList.PeopleList.Add(new PeopleModel
                    {
                        PeopleGUID = people.PeopleGUID.ToString(),
                        PlaceGUID = people.PlaceGUID.ToString(),
                        CompanyName = people.CompanyName,
                        BusinessPhone = people.BusinessPhone,
                        MarketGUID = people.MarketGUID.ToString(),
                        FirstName = people.FirstName,
                        LastName = people.LastName,
                        UserGUID = people.UserGUID.ToString(),
                        OrganizationGUID = people.OrganizationGUID.ToString(),
                        MobilePhone = people.MobilePhone,
                        HomePhone = people.HomePhone,
                        Emails = people.Emails,
                        AddressLine1 = people.AddressLine1,
                        AddressLine2 = people.AddressLine2,
                        City = people.City,
                        State = people.State,
                        Country = people.Country,
                        ZipCode = people.ZipCode,
                        StoreID = getStoreID(people.PeopleGUID.ToString())

                    });
                }

                var viewModel = new PeopleViewModel();
                // viewModel.Place = placeList.PlaceList.AsEnumerable();
                viewModel.People = peopleList.PeopleList.AsEnumerable();
                return viewModel;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        private MarketViewModel CustomerStopDetails(string PlaceGUID)
        {
            try
            {

                var marketList = new MarketViewModel();
                marketList.MarketList = new List<MarketModel>();
                var appMarket = new List<Market>();

                appMarket = _IMarketRepository.GetMarketByOwnerGUID(new Guid(PlaceGUID)).ToList();


                foreach (var market in appMarket.ToList())
                {
                    marketList.MarketList.Add(new MarketModel
                    {
                        MarketID = market.MarketID,
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

                var mviewModel = new MarketViewModel();
                //  mviewModel.Place = placeList.PlaceList.AsEnumerable();
                mviewModel.Market = marketList.MarketList.AsEnumerable();
                return mviewModel;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        private string getStoreID(string PeopleGUID)
        {
            string StoreID = "";
            if (!string.IsNullOrEmpty(PeopleGUID))
            {
                Market _market = _IMarketRepository.GetMarketByPrimaryContactGUID(new Guid(PeopleGUID), 1);
                if (_market != null)
                {
                    StoreID = _market.MarketID;
                }
                else
                {
                    StoreID = "";
                }
            }
            return StoreID;
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

        [HttpPost]
        public ActionResult ImportCSV(HttpPostedFileBase fileInput, string customerguid)
        {
            Logger.Debug("Inside Place Controller- Create Http Post");
            Session["OutputBufferURL"] = null;
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (fileInput != null && !string.IsNullOrEmpty(fileInput.FileName.Trim()))
                    {
                        if (fileInput.FileName.Remove(0, fileInput.FileName.LastIndexOf(".") + 1) != "csv")
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Invalid excel file for Provider upload');</script>";
                        }
                        else
                        {
                            DataTable lStoreTable = new DataTable();
                            DataTable NotInsert = new DataTable();
                            lStoreTable = getCsvToDataset(fileInput.InputStream);

                            if (null == lStoreTable || (null == lStoreTable && null == lStoreTable.DataSet))
                            {
                                TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Invalid csv file for Store upload');</script>";
                            }
                            else
                            {
                                NotInsert = lStoreTable.Clone();
                                NotInsert.TableName = "NotInsert";
                                //bool IsTransactionCompleted = false;
                                //TimeSpan span = new TimeSpan(0, 0, 10, 30);

                                //using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                                //{
                                foreach (DataRow dr in lStoreTable.Rows)
                                {
                                    //Parallel.Invoke(() =>
                                    //{
                                    //});
                                    if (InsertOrUpdateStore(dr) == 0)
                                    {
                                        DataRow dNew = NotInsert.NewRow();
                                        dNew = dr;
                                        NotInsert.ImportRow(dr);
                                    }

                                }
                                //    transactionScope.Complete();
                                //    IsTransactionCompleted = true;
                                //}
                                // if (& NotInsert != null && NotInsert.Rows.Count > 0)
                                //if (IsTransactionCompleted == true && NotInsert != null && NotInsert.Rows.Count > 0)
                                if (NotInsert != null && NotInsert.Rows.Count > 0)
                                {
                                    byte[] outputBuffer = null;

                                    using (MemoryStream tempStream = new MemoryStream())
                                    {
                                        using (StreamWriter writer = new StreamWriter(tempStream))
                                        {
                                            ExportToCSV.WriteDataTable(NotInsert, writer, true);
                                        }

                                        outputBuffer = tempStream.ToArray();
                                    }
                                    Session["OutputBuffer"] = outputBuffer;
                                    // return File(outputBuffer, "text/csv", "export.csv");
                                }
                                else
                                {
                                    Session["OutputBuffer"] = null;
                                }
                                TempData["TabName"] = "Stores";

                                return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = customerguid });

                            }

                        }
                    }
                    else
                    {
                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Please attach a file for Store Upload');</script>";
                        return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = customerguid });
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
            return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = customerguid });
        }

        public ActionResult ImportStore(string customerguid)
        {
            try
            {
                Session["OutputBuffer"] = null;
                string lTempData = String.Format(ConfigurationManager.AppSettings.Get("StoreURL"));
                string result = GetRequest(new Uri(lTempData), 1000000);
                DataTable lStoreTable = new DataTable();

                string[] tableData = result.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var col = from cl in tableData[0].Split(",".ToCharArray())
                          select new DataColumn(cl);
                lStoreTable.Columns.AddRange(col.ToArray());

                (from st in tableData.Skip(1)
                 select lStoreTable.Rows.Add(st.Split(",".ToCharArray()))).ToList();

                if (null == lStoreTable || null == lStoreTable.Rows)
                {
                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Invalid csv file for Store upload');</script>";
                }
                else
                {
                    DataTable NotInsert = new DataTable();
                    NotInsert = lStoreTable.Clone();
                    NotInsert.TableName = "NotInsert";

                    foreach (DataRow dr in lStoreTable.Rows)
                    {

                        if (InsertOrUpdateStore(dr) == 0)
                        {
                            DataRow dNew = NotInsert.NewRow();
                            dNew = dr;
                            NotInsert.ImportRow(dr);
                        }

                    }

                    if (NotInsert != null && NotInsert.Rows.Count > 0)
                    {
                        byte[] outputBuffer = null;

                        using (MemoryStream tempStream = new MemoryStream())
                        {
                            using (StreamWriter writer = new StreamWriter(tempStream))
                            {
                                ExportToCSV.WriteDataTable(NotInsert, writer, true);
                            }

                            outputBuffer = tempStream.ToArray();
                        }
                        Session["OutputBufferURL"] = outputBuffer;
                    }
                    else
                    {
                        Session["OutputBufferURL"] = null;
                    }
                    TempData["TabName"] = "Stores";

                    return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = customerguid });

                }
                return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = customerguid });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Session["OutputBuffer"] = null;
                Session["OutputBufferURL"] = null;
                TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Unable to Insert/Update Stores');</script>";
                return RedirectToAction("Index", "CustomerView", new { id = "Stores", customerid = customerguid });
            }
        }

        public class WebClientWithTimeout : WebClient
        {
            private readonly int timeoutMilliseconds;
            public WebClientWithTimeout(int timeoutMilliseconds)
            {
                this.timeoutMilliseconds = timeoutMilliseconds;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var result = base.GetWebRequest(address);
                result.Timeout = timeoutMilliseconds;
                return result;
            }
        }
        public string GetRequest(Uri uri, int timeoutMilliseconds)
        {
            using (var client = new WebClientWithTimeout(timeoutMilliseconds))
            {
                return client.DownloadString(uri);
            }
        }


        private int InsertOrUpdateStore(DataRow dr)
        {
            int result = 0;
            try
            {
                Place _place = _IPlaceRepository.GetPlaceByID(dr["parentid"].ToString(), new Guid(Session["OrganizationGUID"].ToString()));
                Territory _territory = _ITerritoryRepository.GetTerritoryByTerritoryID(dr["marketid"].ToString(), new Guid(Session["OrganizationGUID"].ToString()));
                Market _market = _IMarketRepository.GetMarketByCustomerID(new Guid(Session["OrganizationGUID"].ToString()), dr["parentid"].ToString(), dr["storenum"].ToString());
                //Market _market = _IMarketRepository.GetMarketByCustomerID(new Guid(Session["OrganizationGUID"].ToString()), dr["parentid"].ToString(), dr["storenum"].ToString().PadLeft(4, '0'));

                Regex objphonePattern = new Regex(@"(1?)(-| ?)(\()?([0-9]{3})(\)|-| |\)-|\) )?([0-9]{3})(-| )?([0-9]{3,14}|[0-9]{3,14})");
                Regex objEmailPattern = new Regex(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?");

                //if (_place != null && _territory != null && _market == null && objphonePattern.IsMatch(dr["phone"].ToString()) && objEmailPattern.IsMatch(dr["email"].ToString()))
                //var result = input.ToString().PadLeft(length, '0');leading zero
                if (_place != null)
                {
                    Market Market = new Market();

                    Market.MarketID = dr["storenum"].ToString();
                    //Market.MarketID = Market.MarketID.PadLeft(4, '0');
                    Market.IsDefault = true;
                    if (!string.IsNullOrEmpty(Session["UserGUID"].ToString()) && Session["UserGUID"].ToString() != Guid.Empty.ToString())
                    {
                        Market.UserGUID = new Guid(Session["UserGUID"].ToString());
                    }
                    else
                    {
                        Market.UserGUID = null;
                    }
                    Market.EntityType = 1;
                    if (!string.IsNullOrEmpty(Session["OrganizationGUID"].ToString()) && Session["OrganizationGUID"].ToString() != Guid.Empty.ToString())
                    {
                        Market.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    }
                    else
                    {
                        Market.OrganizationGUID = null;
                    }
                    if (_place.PlaceGUID != Guid.Empty)
                    {
                        Market.OwnerGUID = _place.PlaceGUID;
                    }
                    else
                    {
                        Market.OwnerGUID = null;
                    }
                    Market.MarketName = dr["name"].ToString();

                    List<Person> _people = _IPeopleRepository.GetPeopleByPlaceGUID(_place.PlaceGUID).ToList();
                    if (_people.Count > 0)
                    {
                        _people = _people.OrderByDescending(x => x.CreatedDate).ToList();
                        if (_people[0].PeopleGUID != Guid.Empty)
                        {
                            Market.PrimaryContactGUID = _people[0].PeopleGUID;
                        }
                        else
                        {
                            Market.PrimaryContactGUID = null;
                        }
                    }
                    else
                    {
                        Market.PrimaryContactGUID = null;
                    }
                    if (_territory != null && _territory.RegionGUID != null && _territory.RegionGUID != Guid.Empty)
                    {
                        Market.RegionGUID = _territory.RegionGUID;
                    }
                    else
                    {
                        Market.RegionGUID = null;
                    }
                    if (_territory != null && _territory.TerritoryGUID != Guid.Empty)
                    {
                        Market.TerritoryGUID = _territory.TerritoryGUID;
                    }
                    else
                    {
                        Market.TerritoryGUID = null;
                    }
                    Market.ParentID = dr["parentid"].ToString();
                    Market.TeritoryID = dr["marketid"].ToString();
                    Market.RMUserID = dr["regionalmanager_userid"].ToString();
                    Market.FMUserID = dr["fieldmanager_userid"].ToString();
                    Market.RegionName = dr["region"].ToString();
                    if (!string.IsNullOrEmpty(dr["regionalmanager_name"].ToString()))
                    {
                        string[] names = dr["regionalmanager_name"].ToString().Split(' ');
                        if (names.Length > 1)
                        {
                            Market.FirstName = names[0].ToString();
                            Market.LastName = names[1].ToString();
                        }
                        else
                        {
                            Market.FirstName = "";
                            Market.LastName = "";
                        }
                    }
                    else
                    {
                        Market.FirstName = "";
                        Market.LastName = "";
                    }
                    Market.MobilePhone = "";
                    Market.MarketPhone = dr["phone"].ToString();
                    Market.HomePhone = dr["phone"].ToString();
                    Market.Emails = dr["email"].ToString();
                    Market.AddressLine1 = dr["addr1"].ToString();
                    Market.AddressLine2 = dr["addr2"].ToString();
                    Market.City = dr["city"].ToString();
                    Market.State = dr["state"].ToString();
                    Market.Country = dr["country"].ToString();
                    Market.ZipCode = dr["postalcode"].ToString();
                    Market.IsDeleted = false;

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
                    bool UserCreated = false;
                    if (null == lObjPOSResp || !lObjPOSResp.store.apistatus.Equals("OK"))
                    {
                        //If this returns null, return not found error to the mobile
                    }
                    else
                    {
                        Market.StoreJSON = new JavaScriptSerializer().Serialize(lObjPOSResp);
                        Market.StoreJSON = Convert.ToBase64String(Encoding.UTF8.GetBytes(Market.StoreJSON));

                    }

                    if (_market == null)
                    {
                        Market.MarketGUID = Guid.NewGuid();
                        Market.CreateDate = DateTime.UtcNow;
                        result = _IMarketRepository.InsertMarket(Market);
                        // result = _IMarketRepository.Save();
                    }
                    else
                    {
                        Market.MarketGUID = _market.MarketGUID;
                        Market.CreateDate = _market.CreateDate;
                        result = _IMarketRepository.UpdateMarket(Market);
                        // result = _IMarketRepository.Save();
                    }
                    //no need to create user now, when userid is not available in the global user table
                    //if (result > 0)
                    //{
                    //    UserCreated = CreateUserByStoreJson(lObjPOSResp, Market.RegionGUID, Market.TerritoryGUID);
                    //}
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.InnerException.ToString());
                return result;
            }
        }
        public bool CreateUserByStoreJson(S_POSStoreResponse lObjPOSResp, Guid? RegionGUID, Guid? TerritoryGUID)
        {
            Logger.Debug("Inside CustomerView Controller- CreateUserByStoreJson");
            try
            {

                bool result = false;
                int userCount = 0, resultCount = 0;
                if (Session["OrganizationGUID"] != null)
                {
                    if (lObjPOSResp.store.fieldmanager != null)
                    {
                        GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(lObjPOSResp.store.fieldmanager.userid, Session["OrganizationGUID"].ToString());
                        if (_globalUser == null)
                        {
                            userCount = userCount + 1;
                            AspUser user = new AspUser();
                            user.UserName = lObjPOSResp.store.fieldmanager.name;
                            user.PasswordHash = "Password";
                            user.ConfirmPassword = "Password";
                            user.SecurityStamp = "";
                            user.Discriminator = "";
                            string[] names = lObjPOSResp.store.fieldmanager.name.ToString().Split(' ');
                            if (names.Length > 1)
                            {
                                user.FirstName = names[0].ToString();
                                user.LastName = names[1].ToString();
                            }
                            else
                            {
                                user.FirstName = "";
                                user.LastName = "";
                            }

                            user.EmailID = lObjPOSResp.store.fieldmanager.email;
                            user.UserID = lObjPOSResp.store.fieldmanager.userid;
                            user.BusinessPhone = lObjPOSResp.store.fieldmanager.phone;
                            user.HomePhone = lObjPOSResp.store.fieldmanager.phone;
                            user.MobilePhone = lObjPOSResp.store.fieldmanager.phone;
                            user.RegionGUID = RegionGUID != null ? RegionGUID.ToString() : "";
                            user.TerritoryGUID = TerritoryGUID != null ? TerritoryGUID.ToString() : "";
                            user.OrganizationGUID = Session["OrganizationGUID"] != null ? Session["OrganizationGUID"].ToString() : "";
                            AspNetRole Role = _IUserRepository.GetRolebyUserType("ENT_U");
                            if (Role != null)
                            {
                                user.RoleGUID = Role.Id;
                            }
                            user.AddressLine1 = lObjPOSResp.store.addr1;
                            user.AddressLine2 = lObjPOSResp.store.addr2;
                            user.City = lObjPOSResp.store.addr2;
                            user.State = lObjPOSResp.store.state;
                            user.Country = lObjPOSResp.store.country;
                            user.ZipCode = lObjPOSResp.store.postalcode;
                            user.IsActive = true;
                            if (UserCreate(user) > 0)
                            {
                                resultCount = resultCount + 1;
                            }
                        }
                    }
                    if (lObjPOSResp.store.regionalmanager != null)
                    {

                        GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(lObjPOSResp.store.regionalmanager.userid, Session["OrganizationGUID"].ToString());
                        if (_globalUser == null)
                        {
                            userCount = userCount + 1;
                            AspUser user = new AspUser();
                            user.UserName = lObjPOSResp.store.regionalmanager.name;
                            user.PasswordHash = "Password";
                            user.ConfirmPassword = "Password";
                            user.SecurityStamp = "";
                            user.Discriminator = "";
                            string[] names = lObjPOSResp.store.regionalmanager.name.ToString().Split(' ');
                            if (names.Length > 1)
                            {
                                user.FirstName = names[0].ToString();
                                user.LastName = names[1].ToString();
                            }
                            else
                            {
                                user.FirstName = "";
                                user.LastName = "";
                            }

                            user.EmailID = lObjPOSResp.store.regionalmanager.email;
                            user.UserID = lObjPOSResp.store.regionalmanager.userid;
                            user.BusinessPhone = lObjPOSResp.store.regionalmanager.phone;
                            user.HomePhone = lObjPOSResp.store.regionalmanager.phone;
                            user.MobilePhone = lObjPOSResp.store.regionalmanager.phone;
                            user.RegionGUID = RegionGUID != null ? RegionGUID.ToString() : "";
                            user.TerritoryGUID = TerritoryGUID != null ? TerritoryGUID.ToString() : "";
                            user.OrganizationGUID = Session["OrganizationGUID"] != null ? Session["OrganizationGUID"].ToString() : "";
                            AspNetRole Role = _IUserRepository.GetRolebyUserType("ENT_U_RM");
                            if (Role != null)
                            {
                                user.RoleGUID = Role.Id;
                            }
                            user.AddressLine1 = lObjPOSResp.store.addr1;
                            user.AddressLine2 = lObjPOSResp.store.addr2;
                            user.City = lObjPOSResp.store.addr2;
                            user.State = lObjPOSResp.store.state;
                            user.Country = lObjPOSResp.store.country;
                            user.ZipCode = lObjPOSResp.store.postalcode;
                            user.IsActive = true;
                            if (UserCreate(user) > 0)
                            {
                                resultCount = resultCount + 1;
                            }
                        }
                    }
                    if (lObjPOSResp.store.managers != null && lObjPOSResp.store.managers.Count > 0)
                    {
                        foreach (S_POSResponseManagers manager in lObjPOSResp.store.managers)
                        {

                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(manager.userid, Session["OrganizationGUID"].ToString());
                            if (_globalUser == null)
                            {
                                userCount = userCount + 1;
                                AspUser user = new AspUser();
                                user.UserName = manager.name;
                                user.PasswordHash = "Password";
                                user.ConfirmPassword = "Password";
                                user.SecurityStamp = "";
                                user.Discriminator = "";
                                string[] names = manager.name.ToString().Split(' ');
                                if (names.Length > 1)
                                {
                                    user.FirstName = names[0].ToString();
                                    user.LastName = names[1].ToString();
                                }
                                else
                                {
                                    user.FirstName = "";
                                    user.LastName = "";
                                }

                                user.EmailID = manager.email;
                                user.UserID = manager.userid;
                                user.BusinessPhone = manager.phone;
                                user.HomePhone = manager.phone;
                                user.MobilePhone = manager.phone;
                                user.RegionGUID = RegionGUID != null ? RegionGUID.ToString() : "";
                                user.TerritoryGUID = TerritoryGUID != null ? TerritoryGUID.ToString() : "";
                                user.OrganizationGUID = Session["OrganizationGUID"] != null ? Session["OrganizationGUID"].ToString() : "";
                                AspNetRole Role = _IUserRepository.GetRolebyUserType("ENT_U");
                                if (Role != null)
                                {
                                    user.RoleGUID = Role.Id;
                                }
                                user.AddressLine1 = lObjPOSResp.store.addr1;
                                user.AddressLine2 = lObjPOSResp.store.addr2;
                                user.City = lObjPOSResp.store.addr2;
                                user.State = lObjPOSResp.store.state;
                                user.Country = lObjPOSResp.store.country;
                                user.ZipCode = lObjPOSResp.store.postalcode;
                                user.IsActive = true;
                                if (UserCreate(user) > 0)
                                {
                                    resultCount = resultCount + 1;
                                }
                            }
                        }
                    }
                    if (resultCount == userCount)
                        result = true;
                    else
                        result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }
        }
        public int UserCreate(AspUser user)
        {
            Logger.Debug("Inside CustomerView Controller- UserCreate");
            int result = 0;
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    OrganizationSubscription orgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(Session["OrganizationGUID"].ToString()));
                    //if (orgSubscription.SubscriptionPurchased > orgSubscription.SubscriptionConsumed)
                    GlobalUser aspuser = _IUserRepository.GlobalUserLogin(user.UserName, Session["OrganizationGUID"].ToString());
                    if (aspuser == null)
                    {
                        LatLong latLong = new LatLong();
                        latLong = GetLatLngCode(user.AddressLine1, user.AddressLine2, user.City, user.State, user.Country, user.ZipCode);
                        GlobalUser globalUser = new GlobalUser();
                        globalUser.UserGUID = Guid.NewGuid();
                        globalUser.USERID = user.UserID;
                        globalUser.Role_Id = user.RoleGUID;
                        globalUser.UserName = user.UserName;
                        globalUser.Password = _IUserRepository.EncodeTo64(user.PasswordHash);
                        globalUser.IsActive = true;
                        globalUser.IsDelete = false;
                        globalUser.Latitude = latLong.Latitude;
                        globalUser.Longitude = latLong.Longitude;
                        globalUser.CreateDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            globalUser.CreateBy = new Guid(Session["UserGUID"].ToString());
                        globalUser.LastModifiedDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            globalUser.LastModifiedBy = new Guid(Session["UserGUID"].ToString());

                        OrganizationUsersMap organizationUserMap = new OrganizationUsersMap();
                        organizationUserMap.OrganizationUserMapGUID = Guid.NewGuid();
                        organizationUserMap.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        organizationUserMap.UserGUID = globalUser.UserGUID;
                        organizationUserMap.IsContractor = false;
                        organizationUserMap.IsActive = true;
                        organizationUserMap.Status = 0;

                        if (!string.IsNullOrEmpty(user.RegionGUID) && user.RegionGUID != Guid.Empty.ToString())
                        {
                            organizationUserMap.RegionGUID = new Guid(user.RegionGUID);
                        }
                        else
                        {
                            organizationUserMap.RegionGUID = null;
                        }
                        if (!string.IsNullOrEmpty(user.TerritoryGUID) && user.TerritoryGUID != Guid.Empty.ToString())
                        {
                            organizationUserMap.TerritoryGUID = new Guid(user.TerritoryGUID);
                        }
                        else
                        {
                            organizationUserMap.TerritoryGUID = null;
                        }


                        organizationUserMap.UserType = "ENT_A";
                        organizationUserMap.CreateDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            organizationUserMap.CreateBy = new Guid(Session["UserGUID"].ToString());
                        organizationUserMap.LastModifiedDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            organizationUserMap.LastModifiedBy = new Guid(Session["UserGUID"].ToString());


                        UserProfile userprofile = new UserProfile();
                        userprofile.ProfileGUID = Guid.NewGuid();
                        userprofile.UserGUID = globalUser.UserGUID;
                        userprofile.CompanyName = _IOrganizationRepository.GetOrganizationByID(new Guid(Session["OrganizationGUID"].ToString())).OrganizationFullName;
                        userprofile.FirstName = user.FirstName;
                        userprofile.LastName = user.LastName;
                        userprofile.MobilePhone = user.MobilePhone;
                        userprofile.BusinessPhone = user.BusinessPhone;
                        userprofile.HomePhone = user.HomePhone;
                        userprofile.EmailID = user.EmailID;
                        userprofile.AddressLine1 = user.AddressLine1;
                        userprofile.AddressLine2 = user.AddressLine2;
                        userprofile.City = user.City;
                        userprofile.State = user.State;
                        userprofile.Country = user.Country;
                        userprofile.Latitude = latLong.Latitude;
                        userprofile.Longitude = latLong.Longitude;
                        userprofile.ZipCode = user.ZipCode;
                        userprofile.IsDeleted = false;
                        userprofile.PicFileURL = user.ImageURL;
                        userprofile.LastModifiedDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            userprofile.LastModifiedBy = new Guid(Session["UserGUID"].ToString());

                        AspNetUser aspnetuser = new AspNetUser();
                        aspnetuser.Id = globalUser.UserGUID.ToString();
                        aspnetuser.UserName = user.UserName;
                        aspnetuser.FirstName = user.FirstName;
                        aspnetuser.LastName = user.LastName;
                        aspnetuser.PasswordHash = _IUserRepository.EncodeTo64(user.PasswordHash);
                        aspnetuser.PhoneNumber = user.MobilePhone;
                        aspnetuser.EmailID = user.EmailID;
                        aspnetuser.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        aspnetuser.SecurityStamp = "";
                        aspnetuser.Discriminator = "";


                        UserSubscription userSubscription = new UserSubscription();
                        if (orgSubscription != null)
                        {

                            userSubscription.UserSubscriptionGUID = Guid.NewGuid();
                            userSubscription.UserGUID = globalUser.UserGUID;
                            userSubscription.OrganizationSubscriptionGUID = orgSubscription.OrganizationSubscriptionGUID;
                            userSubscription.IsActive = true;
                            userSubscription.CreatedDate = DateTime.UtcNow;
                        }

                        int userresult = _IUserRepository.InsertUser(aspnetuser);
                        //int userresult = _IUserRepository.Save();
                        if (userresult > 0)
                        {
                            int guresult = _IGlobalUserRepository.InsertGlobalUser(globalUser);
                            //int guresult = _IGlobalUserRepository.Save();
                            if (guresult > 0)
                            {
                                int usrresult = _IUserProfileRepository.InsertUserProfile(userprofile);
                                // int usrresult = _IUserProfileRepository.Save();
                                if (usrresult > 0)
                                {
                                    int uSubscriptionResult = _IUserSubscriptionRepository.InsertUserSubscription(userSubscription);
                                    //int uSubscriptionResult = _IUserSubscriptionRepository.Save();
                                    if (uSubscriptionResult > 0)
                                    {
                                        int orgusermap = _IOrganizationRepository.InsertOrganizationUserMap(organizationUserMap);
                                        //int orgusermap = _IOrganizationRepository.Save();
                                        if (orgusermap > 0)
                                        {
                                            orgSubscription.SubscriptionConsumed = orgSubscription.SubscriptionConsumed + 1;
                                            result = _IOrganizationSubscriptionRepository.UpdateOrganizationSubscriptionCount(orgSubscription);
                                        }
                                        else
                                        {
                                            _IUserSubscriptionRepository.DeleteUserSubscription(userSubscription.UserSubscriptionGUID);
                                            //_IUserSubscriptionRepository.Save();
                                            _IUserProfileRepository.DeleteUserProfile(userprofile.ProfileGUID);
                                            //_IUserProfileRepository.Save();
                                            _IGlobalUserRepository.DeleteGlobalUser(globalUser.UserGUID);
                                            //_IGlobalUserRepository.Save();
                                            _IUserRepository.DeleteUser(aspnetuser.Id);
                                            // _IUserRepository.Save();
                                        }
                                    }
                                    else
                                    {
                                        _IUserProfileRepository.DeleteUserProfile(userprofile.ProfileGUID);
                                        // _IUserProfileRepository.Save();
                                        _IGlobalUserRepository.DeleteGlobalUser(globalUser.UserGUID);
                                        //_IGlobalUserRepository.Save();
                                        _IUserRepository.DeleteUser(aspnetuser.Id);
                                        // _IUserRepository.Save();
                                    }

                                }
                                else
                                {
                                    _IGlobalUserRepository.DeleteGlobalUser(globalUser.UserGUID);
                                    //_IGlobalUserRepository.Save();
                                    _IUserRepository.DeleteUser(aspnetuser.Id);
                                    //_IUserRepository.Save();
                                }
                            }
                            else
                            {
                                _IUserRepository.DeleteUser(aspnetuser.Id);
                                // _IUserRepository.Save();
                            }
                        }
                        else
                        {
                            _IUserRepository.DeleteUser(aspnetuser.Id);
                            //_IUserRepository.Save();
                        }
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return 0;
            }
        }
        public ActionResult NotInsert()
        {
            try
            {
                if (Session["OutputBuffer"] != null)
                {
                    byte[] outputBuffer = (byte[])Session["OutputBuffer"];
                    //Session["OutputBuffer"] = null;
                    return File(outputBuffer, "text/csv", "export.csv");

                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");
            }
        }
        public ActionResult NotInsertFromURL()
        {
            try
            {
                if (Session["OutputBufferURL"] != null)
                {
                    byte[] outputBuffer = (byte[])Session["OutputBufferURL"];
                    //Session["OutputBuffer"] = null;
                    return File(outputBuffer, "text/csv", "export.csv");

                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");
            }
        }
        private static DataTable getCsvToDataset(Stream pInputStream)
        {
            string strLine = null;
            string[] strArray = null;
            char[] charArray = new char[] { ',' };
            DataSet ds = new DataSet();
            DataTable dt = ds.Tables.Add("Store");
            StreamReader sr = new StreamReader(pInputStream);

            try
            {
                // Process the header row to get the column names
                strLine = sr.ReadLine();
                Regex regx = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                strArray = regx.Split(strLine);

                for (int p = 0; p < strArray.Length; p++)
                {

                    strArray[p] = strArray[p].Trim(' ', '"');
                    dt.Columns.Add(strArray[p].Trim());
                }
                //strArray = strLine.Split(charArray);
                //for (int x = 0; x <= strArray.GetUpperBound(0); x++)
                //{
                //    dt.Columns.Add(strArray[x].Trim());
                //}

                // Process every other row to get a record for the table (a new doctor)
                strLine = sr.ReadLine();
                while (strLine != null)
                {
                    //   Regex regx = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    strArray = regx.Split(strLine);
                    DataRow dr = dt.NewRow();
                    for (int p = 0; p < strArray.Length; p++)
                    {

                        strArray[p] = strArray[p].Trim(' ', '"');
                        if (dr[p] != null)
                            dr[p] = strArray[p].Trim();
                    }
                    //strArray = strLine.Split(charArray);
                    //DataRow dr = dt.NewRow();
                    //for (int i = 0; i <= strArray.GetUpperBound(0); i++)
                    //{
                    //    if (dr[i] != null)
                    //        dr[i] = strArray[i].Trim();
                    //}

                    //DataRow dr = dt.NewRow();
                    //for (int i = 0; i <= strArray.GetUpperBound(0); i++)
                    //{
                    //    if (dr[i] != null)
                    //        dr[i] = strArray[i].Trim();
                    //}

                    dt.Rows.Add(dr);
                    strLine = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                sr.Close();
            }
            return ds.Tables[0];
        }

        public string ConvertToXML(DataTable dt)
        {
            System.IO.MemoryStream mstr = new System.IO.MemoryStream();
            dt.WriteXml(mstr, true);
            mstr.Seek(0, System.IO.SeekOrigin.Begin);
            System.IO.StreamReader sr = new System.IO.StreamReader(mstr);
            string xmlString;
            xmlString = sr.ReadToEnd();
            return (xmlString);
        }
    }
}