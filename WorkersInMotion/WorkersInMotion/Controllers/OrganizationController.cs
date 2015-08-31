using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.DataAccess.Repository;
using System.Xml;
using PagedList;
using System.Text;
using System.Configuration;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Model.ViewModel;
namespace WorkersInMotion.Controllers
{
    public class OrganizationController : BaseController
    {
        #region Constructor
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IOrganizationSubscriptionRepository _IOrganizationSubscriptionRepository;
        private readonly IUserSubscriptionRepository _IUserSubscriptionRepository;
        private readonly IMarketRepository _IMarketRepository;

        private string FILE_PATH = null;
        string m_cCCEmailId = null;
        string m_cFromEmailId = null;
        string m_cSMTPHost = null;
        string m_cServerURL = null;
        string m_cSMTPUserName = null;
        private string m_cPortNo = null;
        int m_cDeleteOldTrackingInfo = 10;

        public OrganizationController()
        {
            this._IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            // this._IGroupRepository = new GroupRepository(new WorkersInMotionDB());
            this._IOrganizationSubscriptionRepository = new OrganizationSubscriptionRepository(new WorkersInMotionDB());
            this._IUserSubscriptionRepository = new UserSubscriptionRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
        }
        public OrganizationController(WorkersInMotionDB context)
        {
            this._IOrganizationRepository = new OrganizationRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
            this._IUserRepository = new UserRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            //  this._IGroupRepository = new GroupRepository(context);
            this._IOrganizationSubscriptionRepository = new OrganizationSubscriptionRepository(context);
            this._IUserSubscriptionRepository = new UserSubscriptionRepository(context);
            this._IMarketRepository = new MarketRepository(context);
        }

        #endregion
        //
        // GET: /Organization/
        [SessionExpireFilter]
        public ActionResult Index()
        {
            Logger.Debug("Inside Organization Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    var userList = new OrganizationViewModel();
                    userList.Organization = new List<OrganizationView>();
                    var appOrganization = _IOrganizationRepository.GetOrganization().ToList();
                    foreach (var organization in appOrganization.ToList())
                    {
                        userList.Organization.Add(new OrganizationView
                        {
                            OrganizationFullName = organization.OrganizationFullName,
                            OrganizationName = organization.OrganizationFullName.Trim(),
                            OrganizationGUID = organization.OrganizationGUID,
                            Website = organization.Website,
                            Phone = organization.Phone,
                            TimeZone = Convert.ToDouble(organization.TimeZone),
                            AddressLine1 = organization.AddressLine1,
                            AddressLine2 = organization.AddressLine2,
                            ImageURL = organization.ImageURL,
                            City = organization.City,
                            State = organization.State,
                            Country = organization.Country,
                            ZipCode = organization.ZipCode,
                            EmailID = organization.EmailID,
                            IsActive = organization.IsActive,
                            IsDeleted = organization.IsDeleted,
                            CreatedDate = organization.CreateDate,
                        });
                    }
                    return View(userList);
                }
                else
                {
                    //TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("../User/Login");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");

            }
        }

        //
        // GET: /Organization/Details/5
        [SessionExpireFilter]
        public ActionResult Details(int id)
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

        //
        // GET: /Organization/Create
        public ActionResult Create()
        {
            //if (Session["OrganizationGUID"] != null)
            return View();

        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrganizationView organization)
        {
            Logger.Debug("Inside Organization Controller- Create HttpPost");
            try
            {
                if (ModelState.IsValid)
                {
                    if (_IOrganizationRepository.GetOrganizationByID(organization.OrganizationGUID) == null)
                    {
                        Organization Organization = new Organization();
                        Organization.OrganizationFullName = organization.OrganizationFullName;
                        Organization.OrganizationName = organization.OrganizationFullName.Trim();
                        Organization.OrganizationGUID = Guid.NewGuid();
                        Organization.Website = organization.Website;
                        Organization.Phone = organization.Phone;
                        Organization.AddressLine1 = organization.AddressLine1;
                        Organization.AddressLine2 = organization.AddressLine2;
                        Organization.City = organization.City;
                        Organization.Country = organization.Country;
                        Organization.ZipCode = organization.ZipCode;
                        LatLong latLong = new LatLong();
                        latLong = GetLatLngCode(Organization.AddressLine1, Organization.AddressLine2, Organization.City, Organization.State, Organization.Country, Organization.ZipCode);
                        Organization.TimeZone = Convert.ToDouble(getTimeZone(latLong.Latitude, latLong.Longitude));
                        Organization.Latitude = latLong.Latitude;
                        Organization.Longitude = latLong.Longitude;
                        Organization.EmailID = organization.EmailID;
                        Organization.State = organization.State;
                        Organization.IsActive = false;
                        Organization.IsDeleted = false;
                        Organization.AllowContractors = true;
                        Organization.ImageURL = organization.ImageURL;
                        Organization.CreateDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            Organization.CreateBy = new Guid(Session["UserGUID"].ToString());
                        Organization.LastModifiedDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            Organization.LastModifiedBy = new Guid(Session["UserGUID"].ToString());
                        //Group Group = new Group();
                        //Group.GroupGUID = Guid.NewGuid();
                        //Group.Name = "Default";
                        //Group.Description = "Default";
                        //Group.OrganizationGUID = Organization.OrganizationGUID;
                        //Group.IsDefault = false;

                        Region Region = new Region();
                        Region.RegionGUID = Guid.NewGuid();
                        Region.Name = "Default";
                        Region.Description = "Default";
                        if (Organization.OrganizationGUID != Guid.Empty)
                        {
                            Region.OrganizationGUID = Organization.OrganizationGUID;
                        }
                        else
                        {
                            Region.OrganizationGUID = null;
                        }
                        Region.IsDefault = false;

                        Territory Territory = new Territory();
                        Territory.TerritoryGUID = Guid.NewGuid();
                        if (Territory.RegionGUID != Guid.Empty)
                        {
                            Territory.RegionGUID = Territory.RegionGUID;
                        }
                        else
                        {
                            Territory.RegionGUID = null;
                        }
                        Territory.Name = "Default";
                        Territory.Description = "Default";
                        if (Organization.OrganizationGUID != Guid.Empty)
                        {
                            Territory.OrganizationGUID = Organization.OrganizationGUID;
                        }
                        else
                        {
                            Territory.OrganizationGUID = null;
                        }
                        Territory.IsDefault = false;




                        GlobalUser globalUser = new GlobalUser();
                        globalUser.UserGUID = Guid.NewGuid();
                        globalUser.USERID = organization.UserID;
                        globalUser.UserName = organization.UserName;
                        globalUser.Password = _IUserRepository.EncodeTo64(organization.Password);
                        globalUser.IsActive = true;
                        globalUser.IsDelete = false;
                        globalUser.Latitude = latLong.Latitude;
                        globalUser.Longitude = latLong.Longitude;
                        globalUser.Role_Id = _IGlobalUserRepository.GetOrganizationAdminRoleID();
                        globalUser.CreateDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            globalUser.CreateBy = new Guid(Session["UserGUID"].ToString());
                        globalUser.LastModifiedDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            globalUser.LastModifiedBy = new Guid(Session["UserGUID"].ToString());

                        UserProfile userprofile = new UserProfile();
                        userprofile.ProfileGUID = Guid.NewGuid();
                        userprofile.UserGUID = globalUser.UserGUID;
                        userprofile.CompanyName = Organization.OrganizationFullName;
                        userprofile.FirstName = organization.FirstName;
                        userprofile.LastName = organization.LastName;
                        userprofile.MobilePhone = "";
                        userprofile.BusinessPhone = organization.Phone;
                        userprofile.HomePhone = "";
                        userprofile.EmailID = organization.EmailID;
                        userprofile.AddressLine1 = organization.AddressLine1;
                        userprofile.AddressLine2 = organization.AddressLine2;
                        userprofile.City = organization.City;
                        userprofile.State = organization.State;
                        userprofile.Country = organization.Country;
                        userprofile.Latitude = latLong.Latitude;
                        userprofile.Longitude = latLong.Longitude;
                        userprofile.ZipCode = organization.ZipCode;
                        userprofile.IsDeleted = false;
                        userprofile.PicFileURL = Organization.ImageURL;
                        userprofile.LastModifiedDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            userprofile.LastModifiedBy = new Guid(Session["UserGUID"].ToString());


                        userprofile.CompanyName = Organization.OrganizationName;

                        AspNetUser aspnetuser = new AspNetUser();
                        aspnetuser.Id = globalUser.UserGUID.ToString();
                        aspnetuser.UserName = organization.UserName;
                        aspnetuser.FirstName = organization.FirstName;
                        aspnetuser.LastName = organization.LastName;
                        aspnetuser.PasswordHash = _IUserRepository.EncodeTo64(organization.Password);
                        aspnetuser.PhoneNumber = organization.Phone;
                        aspnetuser.EmailID = organization.EmailID;
                        if (Organization.OrganizationGUID != Guid.Empty)
                        {
                            aspnetuser.OrganizationGUID = Organization.OrganizationGUID;
                        }
                        else
                        {
                            aspnetuser.OrganizationGUID = null;
                        }
                        aspnetuser.SecurityStamp = "";
                        aspnetuser.Discriminator = "";

                        OrganizationSubscription organizationSubscription = new OrganizationSubscription();
                        organizationSubscription.OrganizationSubscriptionGUID = Guid.NewGuid();
                        organizationSubscription.OrganizationGUID = Organization.OrganizationGUID;
                        organizationSubscription.IsActive = true;
                        organizationSubscription.Version = 1;
                        organizationSubscription.SubscriptionPurchased = 100;
                        organizationSubscription.SubscriptionConsumed = 1;
                        organizationSubscription.StartDate = DateTime.UtcNow;
                        organizationSubscription.ExpiryDate = DateTime.UtcNow.AddDays(30);
                        organizationSubscription.CreatedDate = DateTime.UtcNow;


                        UserSubscription userSubscription = new UserSubscription();
                        userSubscription.UserSubscriptionGUID = Guid.NewGuid();
                        userSubscription.UserGUID = globalUser.UserGUID;
                        if (organizationSubscription.OrganizationSubscriptionGUID != Guid.Empty)
                        {
                            userSubscription.OrganizationSubscriptionGUID = organizationSubscription.OrganizationSubscriptionGUID;
                        }
                        else
                        {
                            userSubscription.OrganizationSubscriptionGUID = null;
                        }

                        userSubscription.IsActive = true;
                        userSubscription.CreatedDate = DateTime.UtcNow;

                        Market Market = new Market();
                        Market.MarketGUID = Guid.NewGuid();
                        Market.IsDefault = true;
                        if (globalUser.UserGUID != Guid.Empty)
                        {
                            Market.UserGUID = globalUser.UserGUID;
                        }
                        else
                        {
                            Market.UserGUID = null;
                        }
                        Market.EntityType = 0;
                        if (Organization.OrganizationGUID != Guid.Empty)
                        {
                            Market.OrganizationGUID = Organization.OrganizationGUID;
                        }
                        else
                        {
                            Market.OrganizationGUID = null;
                        }
                        if (Organization.OrganizationGUID != Guid.Empty)
                        {
                            Market.OwnerGUID = Organization.OrganizationGUID;
                        }
                        else
                        {
                            Market.OwnerGUID = null;
                        }
                        Market.MarketName = Organization.OrganizationFullName;
                        if (Region.RegionGUID != Guid.Empty)
                        {
                            Market.RegionGUID = Region.RegionGUID;
                        }
                        else
                        {
                            Market.RegionGUID = null;
                        }
                        if (Territory.TerritoryGUID != Guid.Empty)
                        {
                            Market.TerritoryGUID = Territory.TerritoryGUID;
                        }
                        else
                        {
                            Market.TerritoryGUID = null;
                        }
                        if (globalUser.UserGUID != Guid.Empty)
                        {
                            Market.PrimaryContactGUID = globalUser.UserGUID;
                        }
                        else
                        {
                            Market.PrimaryContactGUID = null;
                        }
                        Market.FirstName = organization.FirstName;
                        Market.LastName = organization.LastName;
                        Market.MobilePhone = "";
                        Market.MarketPhone = organization.Phone;
                        Market.HomePhone = "";
                        Market.Emails = organization.EmailID;
                        Market.TimeZone = Organization.TimeZone.ToString();
                        Market.AddressLine1 = organization.AddressLine1;
                        Market.AddressLine2 = organization.AddressLine2;
                        Market.City = organization.City;
                        Market.State = organization.State;
                        Market.Country = organization.Country;
                        Market.ZipCode = organization.ZipCode;
                        Market.Latitude = latLong.Latitude;
                        Market.Longitude = latLong.Longitude;
                        Market.ImageURL = organization.ImageURL;
                        Market.IsDeleted = false;
                        Market.CreateDate = DateTime.UtcNow;
                        Market.UpdatedDate = DateTime.UtcNow;

                        OrganizationUsersMap organizationUserMap = new OrganizationUsersMap();
                        organizationUserMap.OrganizationUserMapGUID = Guid.NewGuid();
                        organizationUserMap.OrganizationGUID = Organization.OrganizationGUID;
                        organizationUserMap.UserGUID = globalUser.UserGUID;
                        organizationUserMap.IsContractor = false;
                        organizationUserMap.IsActive = true;
                        organizationUserMap.Status = 0;
                        if (Region.RegionGUID != Guid.Empty)
                        {
                            organizationUserMap.RegionGUID = Region.RegionGUID;
                        }
                        else
                        {
                            organizationUserMap.RegionGUID = null;
                        }
                        if (Territory.TerritoryGUID != Guid.Empty)
                        {
                            organizationUserMap.TerritoryGUID = Territory.TerritoryGUID;
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



                        int result = _IOrganizationRepository.InsertOrganization(Organization);
                        // int result = _IOrganizationRepository.Save();
                        if (result > 0)
                        {
                            _IOrganizationSubscriptionRepository.InsertOrganizationSubscription(organizationSubscription);
                            //_IOrganizationSubscriptionRepository.Save();
                            //_IGroupRepository.InsertGroup(Group);
                            //_IGroupRepository.Save();
                            _IRegionRepository.InsertRegion(Region);
                            //_IRegionRepository.Save();
                            _ITerritoryRepository.InsertTerritory(Territory);
                            // _ITerritoryRepository.Save();

                            int userresult = _IUserRepository.InsertUser(aspnetuser);
                            // int userresult = _IUserRepository.Save();
                            if (userresult > 0)
                            {
                                int guresult = _IGlobalUserRepository.InsertGlobalUser(globalUser);
                                //int guresult = _IGlobalUserRepository.Save();

                                if (guresult > 0)
                                {
                                    int OrgUserMap = _IOrganizationRepository.InsertOrganizationUserMap(organizationUserMap);
                                    //int OrgUserMap = _IOrganizationRepository.Save();
                                    if (OrgUserMap > 0)
                                    {
                                        int usrresult = _IUserProfileRepository.InsertUserProfile(userprofile);
                                        //int usrresult = _IUserProfileRepository.Save();
                                        if (usrresult > 0)
                                        {
                                            int usubresult = _IUserSubscriptionRepository.InsertUserSubscription(userSubscription);
                                            //int usubresult = _IUserSubscriptionRepository.Save();
                                            if (usubresult > 0)
                                            {
                                                int marketresult = _IMarketRepository.InsertMarket(Market);
                                                //int marketresult = _IMarketRepository.Save();
                                                if (marketresult > 0)
                                                {
                                                    if (Session["UserType"] != null && Session["UserType"].ToString() == "WIM_A")
                                                    {
                                                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Organization Created Successfully');</script>";
                                                        return RedirectToAction("Index", "Organization");
                                                    }
                                                    else
                                                    {
                                                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Organization Created Successfully');</script>";
                                                        return RedirectToAction("../User/Login");
                                                    }
                                                }
                                                else
                                                {
                                                    _IUserSubscriptionRepository.DeleteUserSubscription(userSubscription.UserSubscriptionGUID);
                                                    //_IUserSubscriptionRepository.Save();
                                                    //_IGroupRepository.DeleteGroup(Group.GroupGUID);
                                                    //_IGroupRepository.Save();
                                                    _ITerritoryRepository.DeleteTerritoryByRegionGUID(Region.RegionGUID);
                                                    //_ITerritoryRepository.Save();
                                                    _IRegionRepository.DeleteRegion(Region.RegionGUID);
                                                    //_IRegionRepository.Save();


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
                                                //_IGroupRepository.DeleteGroup(Group.GroupGUID);
                                                //_IGroupRepository.Save();
                                                _ITerritoryRepository.DeleteTerritoryByRegionGUID(Region.RegionGUID);
                                                //_ITerritoryRepository.Save();
                                                _IRegionRepository.DeleteRegion(Region.RegionGUID);
                                                // _IRegionRepository.Save();


                                                _IUserSubscriptionRepository.DeleteUserSubscription(userSubscription.UserSubscriptionGUID);
                                                //_IUserSubscriptionRepository.Save();
                                                _IUserProfileRepository.DeleteUserProfile(userprofile.ProfileGUID);
                                                //_IUserProfileRepository.Save();
                                                _IGlobalUserRepository.DeleteGlobalUser(globalUser.UserGUID);
                                                //_IGlobalUserRepository.Save();
                                                _IUserRepository.DeleteUser(aspnetuser.Id);
                                                //_IUserRepository.Save();

                                                _IOrganizationSubscriptionRepository.DeleteOrganizationSubscription(organizationSubscription.OrganizationSubscriptionGUID);
                                                //_IOrganizationSubscriptionRepository.Save();
                                                //_IUserSubscriptionRepository.Save();
                                                _IOrganizationRepository.DeleteOrganization(Organization.OrganizationGUID);
                                                //_IOrganizationRepository.Save();
                                            }
                                        }
                                        else
                                        {
                                            _IOrganizationRepository.DeleteOrganizationUserMap(organizationUserMap.OrganizationUserMapGUID);
                                            //_IOrganizationRepository.Save();
                                            _ITerritoryRepository.DeleteTerritoryByRegionGUID(Region.RegionGUID);
                                            //_ITerritoryRepository.Save();
                                            _IRegionRepository.DeleteRegion(Region.RegionGUID);
                                            //_IRegionRepository.Save();


                                            _IUserSubscriptionRepository.DeleteUserSubscription(userSubscription.UserSubscriptionGUID);
                                            //_IUserSubscriptionRepository.Save();
                                            _IUserProfileRepository.DeleteUserProfile(userprofile.ProfileGUID);
                                            //_IUserProfileRepository.Save();
                                            _IGlobalUserRepository.DeleteGlobalUser(globalUser.UserGUID);
                                            //_IGlobalUserRepository.Save();
                                            _IUserRepository.DeleteUser(aspnetuser.Id);
                                            //_IUserRepository.Save();

                                            _IOrganizationSubscriptionRepository.DeleteOrganizationSubscription(organizationSubscription.OrganizationSubscriptionGUID);
                                            //_IOrganizationSubscriptionRepository.Save();
                                            //_IUserSubscriptionRepository.Save();
                                            _IOrganizationRepository.DeleteOrganization(Organization.OrganizationGUID);
                                            //_IOrganizationRepository.Save();
                                        }
                                    }
                                    else
                                    {
                                        //_IGroupRepository.DeleteGroup(Group.GroupGUID);
                                        //_IGroupRepository.Save();
                                        _ITerritoryRepository.DeleteTerritoryByRegionGUID(Region.RegionGUID);
                                        //_ITerritoryRepository.Save();
                                        _IRegionRepository.DeleteRegion(Region.RegionGUID);
                                        // _IRegionRepository.Save();


                                        _IUserSubscriptionRepository.DeleteUserSubscription(userSubscription.UserSubscriptionGUID);
                                        //_IUserSubscriptionRepository.Save();
                                        _IUserProfileRepository.DeleteUserProfile(userprofile.ProfileGUID);
                                        // _IUserProfileRepository.Save();
                                        _IGlobalUserRepository.DeleteGlobalUser(globalUser.UserGUID);
                                        //_IGlobalUserRepository.Save();
                                        _IUserRepository.DeleteUser(aspnetuser.Id);
                                        //_IUserRepository.Save();

                                        _IOrganizationSubscriptionRepository.DeleteOrganizationSubscription(organizationSubscription.OrganizationSubscriptionGUID);
                                        //_IOrganizationSubscriptionRepository.Save();
                                        //_IUserSubscriptionRepository.Save();
                                        _IOrganizationRepository.DeleteOrganization(Organization.OrganizationGUID);
                                        //_IOrganizationRepository.Save();
                                    }
                                }
                                else
                                {
                                    _IGlobalUserRepository.DeleteGlobalUser(globalUser.UserGUID);
                                    //_IGlobalUserRepository.Save();
                                    _IUserRepository.DeleteUser(aspnetuser.Id);
                                    // _IUserRepository.Save();
                                    _IOrganizationRepository.DeleteOrganization(Organization.OrganizationGUID);
                                    //_IOrganizationRepository.Save();
                                }
                            }
                            else
                            {
                                _IUserRepository.DeleteUser(aspnetuser.Id);
                                // _IUserRepository.Save();
                                _IOrganizationSubscriptionRepository.DeleteOrganizationSubscription(organizationSubscription.OrganizationSubscriptionGUID);
                                //_IOrganizationSubscriptionRepository.Save();
                                _IOrganizationRepository.DeleteOrganization(Organization.OrganizationGUID);
                                // _IOrganizationRepository.Save();
                            }
                        }
                        else
                        {
                            _IOrganizationRepository.DeleteOrganization(Organization.OrganizationGUID);
                            //_IOrganizationRepository.Save();
                        }
                    }
                    else
                    {
                        //UserName already exists
                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Organization Name is Aleady Exists');</script>";
                    }
                }
                return View(organization);

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(organization);
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
        [SessionExpireFilter]
        public ActionResult Edit(string id = "")
        {
            Logger.Debug("Inside User Controller- Edit");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    OrganizationEditView organization = new OrganizationEditView();
                    organization.OrganizationGUID = new Guid(id);
                    Organization Organization = _IOrganizationRepository.GetOrganizationByID(organization.OrganizationGUID);
                    if (organization == null)
                    {
                        return HttpNotFound();
                    }
                    else
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

                    }
                    return View(organization);
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

        //
        // POST: /Default1/Edit/5
        [SessionExpireFilter]
        [HttpPost]
        public ActionResult Edit(OrganizationEditView organization)
        {
            Logger.Debug("Inside Organization Controller- Edit HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
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

                        _IOrganizationRepository.UpdateOrganization(Organization);
                        //_IOrganizationRepository.Save();
                        return RedirectToAction("Index");
                    }
                    // else
                    {
                        return View(organization);
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
                return View(organization);
            }
        }

        [SessionExpireFilter]
        public ActionResult Delete(string id = "")
        {
            Logger.Debug("Inside Organization Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    OrganizationView organization = new OrganizationView();
                    organization.OrganizationGUID = new Guid(id);
                    OrganizationSubscription orgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(organization.OrganizationGUID);
                    if (orgSubscription != null)
                    {
                        _IUserSubscriptionRepository.DeleteUserSubscriptionByOrgSubID(orgSubscription.OrganizationSubscriptionGUID);

                        _IOrganizationSubscriptionRepository.DeleteOrganizationSubscription(orgSubscription.OrganizationSubscriptionGUID);
                        // _IOrganizationSubscriptionRepository.Save();
                    }

                    _IMarketRepository.DeleteMarketByOrganizationGUID(organization.OrganizationGUID);

                    List<OrganizationUsersMap> OrganizationuserList = _IOrganizationRepository.GetOrganizationUserMapByOrgGUID(new Guid(id));

                    _IOrganizationRepository.DeleteOrganizationUserMapByOrganizationGUID(organization.OrganizationGUID);

                    foreach (OrganizationUsersMap item in OrganizationuserList)
                    {

                        if (_IGlobalUserRepository.DeleteGlobalUser(item.UserGUID) > 0)
                        {
                            _IUserProfileRepository.DeleteUserProfileByUserGUID(item.UserGUID);
                            //_IUserProfileRepository.Save();
                        }
                    }

                    // _IUserProfileRepository.DeleteUserProfileByOrganizationGUID(organization.OrganizationGUID);
                    // _IGroupRepository.DeleteGroupByOrganizationGUID(organization.OrganizationGUID);
                    _ITerritoryRepository.DeleteTerritoryByOrganizationGUID(organization.OrganizationGUID);
                    _IRegionRepository.DeleteRegionByOrganizationGUID(organization.OrganizationGUID);
                    _IUserRepository.DeleteByOrganizationGUID(organization.OrganizationGUID);
                    int result = _IOrganizationRepository.DeleteOrganization(organization.OrganizationGUID);
                    //int result = _IOrganizationRepository.Save();
                    _IUserRepository.DeleteByOrganizationGUID(organization.OrganizationGUID);

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
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(string EmailID = "")
        {
            Logger.Debug("Inside Organization Controller- Forget Password");
            try
            {
                if (!string.IsNullOrEmpty(EmailID))
                {

                    string UserGUID = _IUserProfileRepository.GetUserIDFromEmail(EmailID);
                    GlobalUser globalUser = _IGlobalUserRepository.GetPasswordFromUserGUID(new Guid(UserGUID));
                    if (globalUser != null)
                    {
                        EmailManager();
                        StringBuilder sbMailBody = new StringBuilder();
                        sbMailBody.Append("<html>");
                        sbMailBody.Append("<head></head>");
                        sbMailBody.Append("<body>");
                        sbMailBody.Append("<table cellspacing=\"2\" cellpadding=\"2\" border=\"0\" width=\"100%\">");
                        sbMailBody.Append("<tr>");
                        sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                        sbMailBody.Append("Dear " + globalUser.UserName + ",");
                        sbMailBody.Append("</td>");
                        sbMailBody.Append("</tr>");
                        sbMailBody.Append("<tr>");
                        sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                        sbMailBody.Append("</td>");
                        sbMailBody.Append("</tr>");
                        sbMailBody.Append("<tr>");
                        sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                        sbMailBody.Append("<b>User Name :</b>" + globalUser.UserName + "");
                        sbMailBody.Append("</td>");
                        sbMailBody.Append("</tr>");

                        sbMailBody.Append("<tr>");
                        sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                        sbMailBody.Append("<b>Password :</b>" + globalUser.Password + "");
                        sbMailBody.Append("</td>");
                        sbMailBody.Append("</tr>");
                        sbMailBody.Append("<tr>");
                        sbMailBody.Append("<tr>");
                        sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                        sbMailBody.Append("</td>");
                        sbMailBody.Append("</tr>");
                        sbMailBody.Append("<td align=\"left\" width=\"300px\">");
                        sbMailBody.Append("on WorkersInMotion Website");
                        sbMailBody.Append("</td>");
                        sbMailBody.Append("</tr>");

                        sbMailBody.Append("</table>");
                        sbMailBody.Append("</body>");

                        sbMailBody.Append("</html>");

                        sftMail lMail = new sftMail(EmailID, m_cSMTPUserName);
                        //sftMail lMail = new sftMail("arunk@softtrends.com", m_cFromEmailId);
                        //lMail.CCAddress = m_cCCEmailId;
                        lMail.FromDisplayName = "Workers-In-Motion";
                        lMail.FromAddress = m_cSMTPUserName;
                        lMail.ToDisplayName = string.Empty;
                        lMail.IsMailBodyHTML = true;
                        lMail.MailSubject = "Password Recovery";
                        lMail.MailBody = sbMailBody.ToString();
                        lMail.SmtpHost = m_cSMTPHost;
                        if (!string.IsNullOrEmpty(m_cPortNo))
                        {
                            lMail.PortNo = Convert.ToInt32(m_cPortNo);
                        }
                        else
                        {
                            lMail.PortNo = 25;
                        }
                        if (lMail.SendMail())
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Password has been sent successfully to the entered email id');</script>";
                            return RedirectToAction("../User/Login");
                        }
                        else
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Password not send successfully');</script>";
                            return RedirectToAction("Login", "User");
                        }
                    }
                    else
                    {
                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Email ID is not registered');</script>";
                        return RedirectToAction("Login", "User");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Password not send successfully');</script>";
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");
            }
        }
        public void EmailManager()
        {
            Logger.Debug("Inside EmailManager");
            try
            {
                FILE_PATH = Environment.CurrentDirectory;
                m_cCCEmailId = ConfigurationManager.AppSettings.Get("ccEmailId");
                m_cFromEmailId = ConfigurationManager.AppSettings.Get("fromEmailId");
                m_cSMTPHost = ConfigurationManager.AppSettings.Get("smtpAddress");
                m_cPortNo = ConfigurationManager.AppSettings["MailPort"];
                m_cSMTPUserName = ConfigurationManager.AppSettings["SMTPUserName"];
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
    }
}