using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class MyCompanyController : BaseController
    {
        //
        // GET: /MyCompany/
        #region Constructor
        private readonly IJobRepository _IJobRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IOrganizationSubscriptionRepository _IOrganizationSubscriptionRepository;
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IUserSubscriptionRepository _IUserSubscriptionRepository;
        private readonly IMarketRepository _IMarketRepository;

        public MyCompanyController()
        {
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            //  this._IGroupRepository = new GroupRepository(new WorkersInMotionContext());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            this._IOrganizationSubscriptionRepository = new OrganizationSubscriptionRepository(new WorkersInMotionDB());
            this._IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            this._IUserSubscriptionRepository = new UserSubscriptionRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
        }
        public MyCompanyController(WorkersInMotionDB context)
        {
            this._IUserProfileRepository = new UserProfileRepository(context);
            //  this._IGroupRepository = new GroupRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IUserRepository = new UserRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
            this._IOrganizationSubscriptionRepository = new OrganizationSubscriptionRepository(context);
            this._IOrganizationRepository = new OrganizationRepository(context);
            this._IUserSubscriptionRepository = new UserSubscriptionRepository(context);
            this._IMarketRepository = new MarketRepository(context);
        }
        //public UserController(WorkersInMotionJobContext context)
        //{
        // //   this._IJobRepository = new JobRepository(context);
        //}

        #endregion
        public ActionResult Index(string id = "", string regionguid = "", string selection = "")
        {
            Logger.Debug("Inside User Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        TempData["TabName"] = id;
                    }
                    else
                    {
                        TempData["TabName"] = "Details";
                    }
                    mycompany pmycompany = new mycompany();
                    pmycompany.OrganizationEditView = new OrganizationEditView();
                    pmycompany.AspNetUserViewModel = new AspNetUserViewModel();
                    pmycompany.TerritoryViewModel = new TerritoryViewModel();
                    pmycompany.RegionViewModel = new RegionViewModel();


                    //switch (id)
                    //{
                    //    case "Users":
                    //        ViewBag.TabName = "Users";
                    var userList = new AspNetUserViewModel();
                    userList.Users = new List<AspUser>();
                    var appUser = new List<UserProfile>();

                    appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();

                    foreach (var user in appUser.ToList())
                    {
                        UserSubscription userSubscription = _IUserSubscriptionRepository.GetUserSubscriptionByUserID(user.UserGUID);
                        GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByID(user.UserGUID);
                        string Regionname = string.Empty;
                        string Territoryname = string.Empty;
                        string Groupname = string.Empty;
                        string userType = string.Empty;
                        if (_globalUser != null)
                        {
                            OrganizationUsersMap _orgUserMap = _IOrganizationRepository.GetOrganizationUserMapByUserGUID(user.UserGUID);

                            if (_orgUserMap != null && _orgUserMap.RegionGUID != null)
                            {
                                Regionname = _IRegionRepository.GetRegionNameByRegionGUID(new Guid(_orgUserMap.RegionGUID.ToString()));
                            }
                            else
                            {
                                Regionname = "";
                            }
                            if (_orgUserMap != null && _orgUserMap.TerritoryGUID != null)
                            {
                                Territoryname = _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(_orgUserMap.TerritoryGUID.ToString()));
                            }
                            else
                            {
                                Territoryname = "";
                            }


                            // Groupname = _IGroupRepository.GetGroupNameByGroupGUID(new Guid(_globalUser.GroupGUID.ToString()));
                            userType = _IGlobalUserRepository.GetUserTypeByRoleID(_globalUser.Role_Id);
                        }
                        string UserTypeName = _IGlobalUserRepository.GetUserRoleName(user.UserGUID);

                        if (userSubscription != null)
                            userList.Users.Add(new AspUser { UserTypeName = UserTypeName, RegionName = Regionname, TerritoryName = Territoryname, GroupName = Groupname, OrganizationSubscriptionGUID = userSubscription.OrganizationSubscriptionGUID.ToString(), UserType = userType, IsActive = userSubscription.IsActive, SubscriptionGUID = userSubscription.UserSubscriptionGUID.ToString(), FirstName = user.FirstName, LastName = user.LastName, Id = user.UserGUID.ToString(), EmailID = user.EmailID, MobilePhone = user.MobilePhone, City = user.City, State = user.State, Country = user.Country });
                    }

                    OrganizationSubscription orgSubscription = new OrganizationSubscription();
                    if (Session["UserType"] != null && Session["UserType"].ToString() != "WIM_A")
                    {
                        orgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(Session["OrganizationGUID"].ToString()));
                        if (orgSubscription != null)
                        {
                            ViewBag.EnableCreateUserButton = "true";
                        }
                        else
                        {
                            ViewBag.EnableCreateUserButton = "false";
                        }
                        //if (orgSubscription != null && orgSubscription.SubscriptionPurchased > orgSubscription.SubscriptionConsumed)
                        //{
                        //    ViewBag.EnableCreateUserButton = "true";
                        //}
                        //else
                        //{
                        //    ViewBag.EnableCreateUserButton = "false";
                        //}
                    }
                    else
                    {
                        ViewBag.EnableCreateUserButton = "false";
                    }
                    pmycompany.AspNetUserViewModel = userList;
                    //    break;
                    //case "Region":
                    //    ViewBag.TabName = "Regions";
                    var territoryList = new TerritoryViewModel();
                    territoryList.Territory = new List<TerritoryModel>();
                    List<Territory> appTerritory = new List<Territory>();
                    if (!string.IsNullOrEmpty(regionguid) && regionguid != Guid.Empty.ToString())
                    {
                        appTerritory = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(regionguid)).ToList();
                        TempData["TabName"] = "Markets";
                        ViewBag.AddTerritory = "true";
                        ViewBag.Id = regionguid;
                    }
                    else
                    {
                        appTerritory = _ITerritoryRepository.GetTerritoryByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    }
                    foreach (var territory in appTerritory.ToList())
                    {
                        territoryList.Territory.Add(new TerritoryModel { Name = territory.Name, TerritoryGUID = territory.TerritoryGUID.ToString(), RegionGUID = territory.RegionGUID != null ? territory.RegionGUID.ToString() : Guid.Empty.ToString(), Description = territory.Description, OrganizationGUID = territory.OrganizationGUID != null ? territory.OrganizationGUID.ToString() : Guid.Empty.ToString() });
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class='actions'>");
                    sb.Append("<div class='btn-group'>");
                    if (!string.IsNullOrEmpty(regionguid) && regionguid != Guid.Empty.ToString())
                    {
                        sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> " + _IRegionRepository.GetRegionNameByRegionGUID(new Guid(regionguid)) + " <i class='icon-angle-down'></i></a>");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(selection) && selection == "All")
                        {
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>All<i class='icon-angle-down'></i></a>");
                        }
                        else
                        {
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Select Region <i class='icon-angle-down'></i></a>");
                        }
                    }
                    sb.Append("<ul id='ulworkgroup' style='height:100px;overflow-y:scroll' class='dropdown-menu pull-right'>");
                    if (string.IsNullOrEmpty(selection) || selection != "All")
                    {
                        sb.Append("<li><a href=" + Url.Action("Index", "MyCompany", new { id = "Markets", selection = "All" }) + ">All</a></li>");
                    }
                    List<Region> RegList = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    foreach (Region item in RegList)
                    {
                        sb.Append("<li><a href=" + Url.Action("Index", "MyCompany", new { regionguid = item.RegionGUID.ToString() }) + " data-groupguid=" + item.RegionGUID + ">" + item.Name + "</a></li>");
                    }
                    sb.Append("</ul>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    ViewBag.RegionList = sb.ToString();
                    pmycompany.TerritoryViewModel = territoryList;
                    //    break;
                    //case "Territory":
                    //    ViewBag.TabName = "Territories";
                    var regionList = new RegionViewModel();
                    regionList.Region = new List<RegionModel>();
                    var appRegion = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    foreach (var region in appRegion.ToList())
                    {
                        regionList.Region.Add(new RegionModel { Name = region.Name, RegionGUID = region.RegionGUID.ToString(), Description = region.Description, OrganizationGUID = region.OrganizationGUID != null ? region.OrganizationGUID.ToString() : Guid.Empty.ToString() });
                    }
                    pmycompany.RegionViewModel = regionList;
                    //    break;
                    //case "Organization":
                    //    ViewBag.TabName = "Details";
                    OrganizationEditView organization = new OrganizationEditView();
                    organization.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    Organization Organization = _IOrganizationRepository.GetOrganizationByID(organization.OrganizationGUID);
                    if (organization != null)
                    {
                        organization.OrganizationFullName = Organization.OrganizationFullName;
                        organization.OrganizationName = Organization.OrganizationFullName.Trim();
                        organization.OrganizationGUID = Organization.OrganizationGUID;
                        organization.Website = Organization.Website;
                        organization.Phone = Organization.Phone;
                        organization.TimeZone = Organization.TimeZone.ToString();
                        organization.AddressLine1 = Organization.AddressLine1;
                        organization.AddressLine2 = Organization.AddressLine2;
                        organization.ImageURL = Organization.ImageURL;
                        organization.City = Organization.City;
                        organization.Country = Organization.Country;
                        organization.State = Organization.State;
                        organization.ZipCode = Organization.ZipCode;
                        organization.EmailID = Organization.EmailID;
                        organization.IsActive = Organization.IsActive;
                        organization.IsDeleted = Organization.IsDeleted;
                        organization.CreatedDate = Organization.CreateDate;
                        organization.CreateBy = Organization.CreateBy;
                        pmycompany.OrganizationEditView = organization;
                    }

                    //    break;
                    //default:
                    //    ViewBag.TabName = "Details";
                    //    OrganizationEditView porganization = new OrganizationEditView();
                    //    porganization.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    //    Organization pOrganization = _IOrganizationRepository.GetOrganizationByID(porganization.OrganizationGUID);
                    //    if (porganization != null)
                    //    {
                    //        porganization.OrganizationFullName = pOrganization.OrganizationFullName;
                    //        porganization.OrganizationName = pOrganization.OrganizationFullName.Trim();
                    //        porganization.OrganizationGUID = pOrganization.OrganizationGUID;
                    //        porganization.Website = pOrganization.Website;
                    //        porganization.Phone = pOrganization.Phone;
                    //        porganization.TimeZone = pOrganization.TimeZone.ToString();
                    //        porganization.AddressLine1 = pOrganization.AddressLine1;
                    //        porganization.AddressLine2 = pOrganization.AddressLine2;
                    //        porganization.ImageURL = pOrganization.ImageURL;
                    //        porganization.City = pOrganization.City;
                    //        porganization.Country = pOrganization.Country;
                    //        porganization.State = pOrganization.State;
                    //        porganization.ZipCode = pOrganization.ZipCode;
                    //        porganization.EmailID = pOrganization.EmailID;
                    //        porganization.IsActive = pOrganization.IsActive;
                    //        porganization.IsDeleted = pOrganization.IsDeleted;
                    //        porganization.CreatedDate = pOrganization.CreateDate;
                    //        porganization.CreateBy = pOrganization.CreateBy;
                    //        pmycompany.OrganizationEditView.Add(porganization);
                    //    }
                    //    break;
                    // }



                    return View(pmycompany);
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
        public ActionResult EditOrg(mycompany mycompany)
        {
            Logger.Debug("Inside Organization Controller- Edit HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        if (mycompany.OrganizationEditView != null)
                        {
                            OrganizationEditView organization = mycompany.OrganizationEditView;

                            Organization Organization = new Organization();
                            Organization.OrganizationFullName = organization.OrganizationFullName;
                            Organization.OrganizationName = organization.OrganizationFullName.Trim();
                            Organization.OrganizationGUID = organization.OrganizationGUID;
                            Organization.Website = organization.Website;
                            Organization.Phone = organization.Phone;
                            // Organization.TimeZone = organization.TimeZone;
                            Organization.AddressLine1 = organization.AddressLine1;
                            Organization.AddressLine2 = organization.AddressLine2;
                            Organization.ImageURL = organization.ImageURL;
                            Organization.City = organization.City;
                            Organization.Country = organization.Country;
                            Organization.State = organization.State;
                            Organization.ZipCode = organization.ZipCode;
                            Organization.EmailID = organization.EmailID;
                            Organization.IsActive = organization.IsActive;
                            Organization.IsDeleted = organization.IsDeleted;
                            Organization.AllowContractors = true;
                            Organization.CreateDate = organization.CreatedDate;
                            Organization.CreateBy = organization.CreateBy;
                            Organization.LastModifiedDate = DateTime.UtcNow;
                            if (Session["UserGUID"] != null)
                                Organization.LastModifiedBy = new Guid(Session["UserGUID"].ToString());


                            LatLong latLong = new LatLong();
                            latLong = GetLatLngCode(Organization.AddressLine1, Organization.AddressLine2, Organization.City, Organization.State, Organization.Country, Organization.ZipCode);
                            Organization.TimeZone = Convert.ToDouble(getTimeZone(latLong.Latitude, latLong.Longitude));


                            if (_IOrganizationRepository.UpdateOrganization(Organization) > 0)
                            {
                                return RedirectToAction("Index", "MyCompany");
                            }
                            else
                            {
                                TempData["TabName"] = "Details";
                                mycompany.AspNetUserViewModel = UserDetails();

                                mycompany.RegionViewModel = RegionDetails();

                                mycompany.TerritoryViewModel = TerritoryDetails();
                                return View("Index", mycompany);
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "MyCompany");
                        }

                    }
                    else
                    {
                        TempData["TabName"] = "Details";
                        mycompany.AspNetUserViewModel = UserDetails();

                        mycompany.RegionViewModel = RegionDetails();

                        mycompany.TerritoryViewModel = TerritoryDetails();
                        return View("Index", mycompany);
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
                return View(mycompany);
            }
        }

        private AspNetUserViewModel UserDetails()
        {
            #region UserDetails
            try
            {
                var userList = new AspNetUserViewModel();
                userList.Users = new List<AspUser>();
                var appUser = new List<UserProfile>();

                appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();

                foreach (var user in appUser.ToList())
                {
                    UserSubscription userSubscription = _IUserSubscriptionRepository.GetUserSubscriptionByUserID(user.UserGUID);
                    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByID(user.UserGUID);
                    string Regionname = string.Empty;
                    string Territoryname = string.Empty;
                    string Groupname = string.Empty;
                    string userType = string.Empty;
                    if (_globalUser != null)
                    {
                        OrganizationUsersMap _orgUserMap = _IOrganizationRepository.GetOrganizationUserMapByUserGUID(user.UserGUID);

                        if (_orgUserMap != null && _orgUserMap.RegionGUID != null)
                        {
                            Regionname = _IRegionRepository.GetRegionNameByRegionGUID(new Guid(_orgUserMap.RegionGUID.ToString()));
                        }
                        else
                        {
                            Regionname = "";
                        }
                        if (_orgUserMap != null && _orgUserMap.TerritoryGUID != null)
                        {
                            Territoryname = _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(_orgUserMap.TerritoryGUID.ToString()));
                        }
                        else
                        {
                            Territoryname = "";
                        }

                        //Regionname = _IRegionRepository.GetRegionNameByRegionGUID(new Guid(_IOrganizationRepository.GetOrganizationUserMapByUserGUID(user.UserGUID).RegionGUID.ToString()));
                        //Territoryname = _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(_IOrganizationRepository.GetOrganizationUserMapByUserGUID(user.UserGUID).TerritoryGUID.ToString()));
                        userType = _IGlobalUserRepository.GetUserTypeByRoleID(_globalUser.Role_Id);
                    }
                    string UserTypeName = _IGlobalUserRepository.GetUserRoleName(user.UserGUID);

                    if (userSubscription != null)
                        userList.Users.Add(new AspUser { UserTypeName = UserTypeName, RegionName = Regionname, TerritoryName = Territoryname, GroupName = Groupname, OrganizationSubscriptionGUID = userSubscription.OrganizationSubscriptionGUID.ToString(), UserType = userType, IsActive = userSubscription.IsActive, SubscriptionGUID = userSubscription.UserSubscriptionGUID.ToString(), FirstName = user.FirstName, LastName = user.LastName, Id = user.UserGUID.ToString(), EmailID = user.EmailID, MobilePhone = user.MobilePhone, City = user.City, State = user.State, Country = user.Country });
                }

                OrganizationSubscription orgSubscription = new OrganizationSubscription();
                if (Session["UserType"] != null && Session["UserType"].ToString() != "WIM_A")
                {
                    orgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(Session["OrganizationGUID"].ToString()));
                    if (orgSubscription != null)
                    {
                        ViewBag.EnableCreateUserButton = "true";
                    }
                    else
                    {
                        ViewBag.EnableCreateUserButton = "false";
                    }
                    //if (orgSubscription != null && orgSubscription.SubscriptionPurchased > orgSubscription.SubscriptionConsumed)
                    //{
                    //    ViewBag.EnableCreateUserButton = "true";
                    //}
                    //else
                    //{
                    //    ViewBag.EnableCreateUserButton = "false";
                    //}
                }
                else
                {
                    ViewBag.EnableCreateUserButton = "false";
                }
                return userList;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
            #endregion
        }

        private TerritoryViewModel TerritoryDetails()
        {
            #region TerritoryDetails
            try
            {
                var territoryList = new TerritoryViewModel();
                territoryList.Territory = new List<TerritoryModel>();
                List<Territory> appTerritory = new List<Territory>();

                appTerritory = _ITerritoryRepository.GetTerritoryByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                foreach (var territory in appTerritory.ToList())
                {
                    territoryList.Territory.Add(new TerritoryModel { Name = territory.Name, TerritoryGUID = territory.TerritoryGUID.ToString(), RegionGUID = territory.RegionGUID != null ? territory.RegionGUID.ToString() : Guid.Empty.ToString(), Description = territory.Description, OrganizationGUID = territory.OrganizationGUID != null ? territory.OrganizationGUID.ToString() : Guid.Empty.ToString() });
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class='actions'>");
                sb.Append("<div class='btn-group'>");

                sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Select Territory <i class='icon-angle-down'></i></a>");
                sb.Append("<ul id='ulworkgroup' class='dropdown-menu pull-right'>");
                sb.Append("<li><a href=" + Url.Action("Index", "MyCompany", new { id = "Markets" }) + ">All</a></li>");
                List<Region> RegionList = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                foreach (Region item in RegionList)
                {
                    sb.Append("<li><a href=" + Url.Action("Index", "MyCompany", new { regionguid = item.RegionGUID.ToString() }) + " data-groupguid=" + item.RegionGUID + ">" + item.Name + "</a></li>");
                }
                sb.Append("</ul>");
                sb.Append("</div>");
                sb.Append("</div>");
                ViewBag.RegionList = sb.ToString();
                return territoryList;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
            #endregion
        }

        private RegionViewModel RegionDetails()
        {
            #region RegionDetails
            try
            {
                var regionList = new RegionViewModel();
                regionList.Region = new List<RegionModel>();
                var appRegion = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                foreach (var region in appRegion.ToList())
                {
                    regionList.Region.Add(new RegionModel { Name = region.Name, RegionGUID = region.RegionGUID.ToString(), Description = region.Description, OrganizationGUID = region.OrganizationGUID != null ? region.OrganizationGUID.ToString() : Guid.Empty.ToString() });
                }
                return regionList;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
            #endregion
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