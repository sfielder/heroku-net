using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Text;
using PagedList;
using System.Collections;
using System.Data;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.Model;
using System.Xml;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Model.ViewModel;

namespace WorkersInMotion.Controllers
{

    public class UserController : BaseController
    {
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

        public UserController()
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
        public UserController(WorkersInMotionDB context)
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
        public void pageCountList(int selectedValue)
        {
            if (selectedValue == 0)
                selectedValue = 5;
            ViewBag.pageCountValue = selectedValue;
            var countList = new List<SelectListItem>
            {
                         new SelectListItem { Text = "5", Value = "5"},
                         new SelectListItem { Text = "15", Value = "15"},
                         new SelectListItem { Text = "20", Value = "20"}
           };
            ViewBag.pCountList = new SelectList(countList, "Value", "Text", selectedValue);
        }
        #endregion
        public void DropdownValues()
        {
            //var GroupDetails = _IGroupRepository.GetGroupByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
            //{
            //    Value = r.GroupGUID.ToString(),
            //    Text = r.Name
            //});
            //ViewBag.GroupDetails = new SelectList(GroupDetails, "Value", "Text");

            var RegionDetails = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
            {
                Value = r.RegionGUID.ToString(),
                Text = r.Name
            });
            ViewBag.RegionDetails = new SelectList(RegionDetails, "Value", "Text");

            // string Tregion = RegionDetails.Count() > 0 ? RegionDetails.First().Value : "";

            var TerritoryDetails = _ITerritoryRepository.GetTerritoryByOrganizationGUID(Guid.Empty).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
            {
                Value = r.TerritoryGUID.ToString(),
                Text = r.Name
            });
            ViewBag.TerritoryDetails = new SelectList(TerritoryDetails, "Value", "Text");

            var RoleDetails = _IGlobalUserRepository.GetRoles().ToList().Where(r => r.UserType != "WIM_A" && r.UserType != "ENT_A" && r.UserType != "IND_C").OrderBy(r => r.Name).Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            });
            ViewBag.RoleDetails = new SelectList(RoleDetails, "Value", "Text");

        }
        // GET: /User/
        [SessionExpireFilter]
        public ActionResult Index(string id = "")
        {
            Logger.Debug("Inside User Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    var userList = new AspNetUserViewModel();
                    userList.Users = new List<AspUser>();
                    var appUser = new List<UserProfile>();

                    if (string.IsNullOrEmpty(id))
                    {
                        appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).OrderBy(sort => sort.FirstName).ToList();
                    }
                    else
                    {
                        appUser = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(id)).OrderBy(sort => sort.FirstName).ToList();
                    }
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
                            //  Groupname = _IGroupRepository.GetGroupNameByGroupGUID(new Guid(_globalUser.GroupGUID.ToString()));
                            userType = _IGlobalUserRepository.GetUserTypeByRoleID(_globalUser.Role_Id);
                        }
                        string UserTypeName = _IGlobalUserRepository.GetUserRoleName(user.UserGUID);

                        if (userSubscription != null)
                            userList.Users.Add(new AspUser { UserTypeName = UserTypeName, RegionName = Regionname, TerritoryName = Territoryname, GroupName = Groupname, OrganizationSubscriptionGUID = userSubscription.OrganizationSubscriptionGUID.ToString(), UserType = userType, IsActive = userSubscription.IsActive, SubscriptionGUID = userSubscription.UserSubscriptionGUID.ToString(), FirstName = user.FirstName, LastName = user.LastName, Id = user.UserGUID.ToString(), EmailID = user.EmailID, MobilePhone = user.MobilePhone, City = user.City, State = user.State, Country = user.Country });
                    }
                    userList.Users = userList.Users.OrderBy(sort => sort.FirstName).ToList();

                    DropdownValues();
                    OrganizationSubscription orgSubscription = new OrganizationSubscription();
                    if (Session["UserType"] != null && Session["UserType"].ToString() != "WIM_A")
                    {
                        if (string.IsNullOrEmpty(id))
                        {
                            orgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(Session["OrganizationGUID"].ToString()));
                        }
                        else
                        {
                            orgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(id));
                        }
                        if (orgSubscription != null)
                        {
                            ViewBag.EnableCreateUserButton = "true";
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
                    return View(userList.Users.AsEnumerable());
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

        public ActionResult Login(string redirectUrl = "", string SessionExpire = "")
        {
            Session.Abandon();
            Session.Clear();
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                ViewBag.URL = redirectUrl;
            }
            if (!string.IsNullOrEmpty(SessionExpire))
            {
                TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','  Session Expired! Login Again... ');</script>";
            }
            return View();
        }
        public ActionResult SessionTimeOut()
        {
            Session.Abandon();
            Session.Clear();
            return View();
        }

        [SessionExpireFilter]
        public ActionResult DashBoard(string pDay = "", string selection = "", string pDayUser = "", string selectionUser = "", string RowCount = "", int page = 1, string search = "")
        {
            Logger.Debug("Inside User Controller- Index");
            try
            {
                int totalPage = 0;
                int totalRecord = 0;
                int pCount = 0;

                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(RowCount))
                    {
                        int.TryParse(RowCount, out pCount);
                        pageCountList(pCount);

                    }
                    else
                    {
                        pageCountList(pCount);
                    }

                    Job lJob = new Job();
                    lJob.AssignedUserGUID = new Guid(Session["UserGUID"].ToString());
                    lJob.IsDeleted = false;
                    lJob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    ViewBag.Flag = "false";

                    #region Dropdown

                    if (Session["UserType"] != null && !string.IsNullOrEmpty(Session["UserType"].ToString()) && Session["UserType"].ToString() == "ENT_A")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<div class='actions'>");
                        sb.Append("<div class='btn-group'>");
                        if (!string.IsNullOrEmpty(pDay))
                        {
                            pDay = _IUserRepository.DecodeFrom64(pDay);
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>" + pDay + " <i class='icon-angle-down'></i></a>");
                        }
                        else
                        {

                            //if (!string.IsNullOrEmpty(selection) && selection == "Current Period")
                            //{
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Current Period <i class='icon-angle-down'></i></a>");
                            //}
                            //else
                            //{
                            //    sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Select <i class='icon-angle-down'></i></a>");
                            //}

                        }
                        sb.Append("<ul id='ulworkgroup' class='dropdown-menu pull-right'>");


                        //if (!string.IsNullOrEmpty(selection) || selection != "Current Period")
                        //{
                        //    sb.Append("<li><a href=" + Url.Action("DashBoard", "User", new { selection = "Current Period" }) + ">Current Period</a></li>");
                        //}

                        if (!string.IsNullOrEmpty(pDay))
                        {
                            if (pDay != "Current Period")
                            {
                                sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Current Period") }) + " data-groupguid='>Current Period'>Current Period</a></li>");
                            }
                            if (pDay != "Period 2")
                            {
                                sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 2") }) + " data-groupguid='Period 2'>Period 2</a></li>");
                            }
                            if (pDay != "Period 3")
                            {
                                sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 3") }) + " data-groupguid='Period 3'>Period 3</a></li>");
                            }
                            if (pDay != "Period 4")
                            {
                                sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 4") }) + " data-groupguid='Period 4'>Period 4</a></li>");
                            }
                            if (pDay != "Period 5")
                            {
                                sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 5") }) + " data-groupguid='Period 5'>Period 5</a></li>");
                            }
                            if (pDay != "Period 6")
                            {
                                sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 6") }) + " data-groupguid='Period 6'>Period 6</a></li>");
                            }
                        }
                        else
                        {
                            //sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Current Period") }) + " data-groupguid='Current Period'>Current Period</a></li>");
                            sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 2") }) + " data-groupguid='Period 2'>Period 2</a></li>");
                            sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 3") }) + " data-groupguid='Period 3'>Period 3</a></li>");
                            sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 4") }) + " data-groupguid='Period 4'>Period 4</a></li>");
                            sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 5") }) + " data-groupguid='Period 5'>Period 5</a></li>");
                            sb.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDay = _IUserRepository.EncodeTo64("Period 6") }) + " data-groupguid='Period 6'>Period 6</a></li>");

                        }
                        sb.Append("</ul>");
                        sb.Append("</div>");
                        sb.Append("</div>");

                        ViewBag.FilterList = sb.ToString();
                    }
                    else
                    {
                        ViewBag.FilterList = "";
                    }
                    #endregion

                    #region Dropdown User Activity
                    StringBuilder sbUser = new StringBuilder();
                    sbUser.Append("<div class='actions'>");
                    sbUser.Append("<div class='btn-group'>");
                    if (!string.IsNullOrEmpty(pDayUser))
                    {
                        pDayUser = _IUserRepository.DecodeFrom64(pDayUser);
                        sbUser.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>" + pDayUser + " <i class='icon-angle-down'></i></a>");
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(selectionUser) && selectionUser == "All")
                        {
                            sbUser.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> All <i class='icon-angle-down'></i></a>");
                        }
                        else
                        {
                            sbUser.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Select <i class='icon-angle-down'></i></a>");
                        }

                    }
                    sbUser.Append("<ul id='ulworkgroup' class='dropdown-menu pull-right'>");


                    if (string.IsNullOrEmpty(selectionUser) || selectionUser != "All")
                    {
                        sbUser.Append("<li><a href=" + Url.Action("Dashboard", "User", new { selectionUser = "All" }) + ">All</a></li>");
                    }

                    if (!string.IsNullOrEmpty(pDayUser))
                    {
                        if (pDayUser != "Today")
                        {
                            sbUser.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDayUser = _IUserRepository.EncodeTo64("Today") }) + " data-groupguid='Today'>Today</a></li>");
                        }
                        if (pDayUser != "7 Days")
                        {
                            sbUser.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDayUser = _IUserRepository.EncodeTo64("7 Days") }) + " data-groupguid='7 Days'>7 Days</a></li>");
                        }
                        if (pDayUser != "30 Days")
                        {
                            sbUser.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDayUser = _IUserRepository.EncodeTo64("30 Days") }) + " data-groupguid=30 Days'>30 Days</a></li>");
                        }
                    }
                    else
                    {
                        sbUser.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDayUser = _IUserRepository.EncodeTo64("Today") }) + " data-groupguid='Today'>Today</a></li>");
                        sbUser.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDayUser = _IUserRepository.EncodeTo64("7 Days") }) + " data-groupguid='7 Days'>7 Days</a></li>");
                        sbUser.Append("<li><a href=" + Url.Action("Dashboard", "User", new { pDayUser = _IUserRepository.EncodeTo64("30 Days") }) + " data-groupguid=30 Days'>30 Days</a></li>");
                    }
                    sbUser.Append("</ul>");
                    sbUser.Append("</div>");
                    sbUser.Append("</div>");

                    ViewBag.UserActivityDropdown = sbUser.ToString();
                    #endregion




                    #region Pie Chart
                    Market _Market = new Market();
                    _Market.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    if (!string.IsNullOrEmpty(pDay))
                    {
                        switch (pDay)
                        {

                            case "Current Period":
                                _Market.LastStoreVisitedDate = DateTime.UtcNow.Date.AddDays(-45);
                                ViewBag.LastModifiedDate = DateTime.UtcNow.Date.AddDays(-45).ToString("dd-MM-yyyy");
                                ViewBag.FromDate = DateTime.UtcNow.Date.AddDays(-45).ToString("dd-MM-yyyy");
                                ViewBag.ToDate = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                                break;

                            case "Period 2":
                                _Market.LastStoreVisitedDate = DateTime.UtcNow.Date.AddDays(-52);
                                ViewBag.LastModifiedDate = DateTime.UtcNow.Date.AddDays(-52).ToString("dd-MM-yyyy");
                                ViewBag.FromDate = DateTime.UtcNow.Date.AddDays(-52).ToString("dd-MM-yyyy");
                                ViewBag.ToDate = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                                break;
                            case "Period 3":
                                _Market.LastStoreVisitedDate = DateTime.UtcNow.Date.AddDays(-59);
                                ViewBag.LastModifiedDate = DateTime.UtcNow.Date.AddDays(-59).ToString("dd-MM-yyyy");
                                ViewBag.FromDate = DateTime.UtcNow.Date.AddDays(-59).ToString("dd-MM-yyyy");
                                ViewBag.ToDate = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                                break;
                            case "Period 4":
                                _Market.LastStoreVisitedDate = DateTime.UtcNow.Date.AddDays(-66);
                                ViewBag.LastModifiedDate = DateTime.UtcNow.Date.AddDays(-66).ToString("dd-MM-yyyy");
                                ViewBag.FromDate = DateTime.UtcNow.Date.AddDays(-66).ToString("dd-MM-yyyy");
                                ViewBag.ToDate = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                                break;
                            case "Period 5":
                                _Market.LastStoreVisitedDate = DateTime.UtcNow.Date.AddDays(-73);
                                ViewBag.LastModifiedDate = DateTime.UtcNow.Date.AddDays(-73).ToString("dd-MM-yyyy");
                                ViewBag.FromDate = DateTime.UtcNow.Date.AddDays(-73).ToString("dd-MM-yyyy");
                                ViewBag.ToDate = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                                break;
                            case "Period 6":
                                _Market.LastStoreVisitedDate = DateTime.UtcNow.Date.AddDays(-80);
                                ViewBag.LastModifiedDate = DateTime.UtcNow.Date.AddDays(-80).ToString("dd-MM-yyyy");
                                ViewBag.FromDate = DateTime.UtcNow.Date.AddDays(-80).ToString("dd-MM-yyyy");
                                ViewBag.ToDate = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                                break;
                            default:
                                _Market.LastStoreVisitedDate = DateTime.UtcNow.Date.AddDays(-45);
                                ViewBag.LastModifiedDate = DateTime.UtcNow.Date.AddDays(-45).ToString("dd-MM-yyyy");
                                ViewBag.FromDate = DateTime.UtcNow.Date.AddDays(-45).ToString("dd-MM-yyyy");
                                ViewBag.ToDate = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                                break;

                        }
                    }
                    else
                    {
                        _Market.LastStoreVisitedDate = DateTime.UtcNow.Date.AddDays(-45);
                        ViewBag.LastModifiedDate = DateTime.UtcNow.Date.AddDays(-45).ToString("dd-MM-yyyy");
                        ViewBag.FromDate = DateTime.UtcNow.Date.AddDays(-45).ToString("dd-MM-yyyy");
                        ViewBag.ToDate = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                    }
                    int jobCount = 0;
                    // List<Job> pjobVistList = pjobList.Where(x => x.LastModifiedDate >= DateTime.UtcNow.AddDays(-45)).ToList();
                    string FieldManagerName = string.Empty;
                    List<Market> pAllStoreList = new List<Market>();
                    List<Market> pStoreNonVistList = new List<Market>();


                    //FOr Regional Manager
                    if (Session["UserType"] != null && !string.IsNullOrEmpty(Session["UserType"].ToString()) && Session["UserType"].ToString() == "ENT_U_RM" && Session["UserGUID"] != null)
                    {
                        OrganizationUsersMap OrgUserMap = _IOrganizationRepository.GetOrganizationUserMapByUserGUID(new Guid(Session["UserGUID"].ToString()), new Guid(Session["OrganizationGUID"].ToString()));
                        if (OrgUserMap != null)
                        {
                            _Market.RegionGUID = OrgUserMap.RegionGUID;
                        }
                    }

                    //check the current user is Field Manager or not
                    if (Session["UserType"] != null && !string.IsNullOrEmpty(Session["UserType"].ToString()) && Session["UserType"].ToString() == "ENT_U" && Session["UserGUID"] != null)
                    {
                        GlobalUser globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(Session["UserGUID"].ToString()));
                        if (globalUser != null)
                        {
                            //FieldManagerName = globalUser.USERID;
                            _Market.FMUserID = globalUser.USERID;
                        }
                    }
                    pStoreNonVistList = _IMarketRepository.GetStoreNonVisit(_Market);
                    pAllStoreList = _IMarketRepository.GetAllStores(_Market);

                    JobStatusPercentageList pJobStatusPercentageList = new JobStatusPercentageList();
                    pJobStatusPercentageList.data = new List<JobStatusPercentage>();
                    if (pAllStoreList != null && pAllStoreList.Count > 0 && pStoreNonVistList != null && pStoreNonVistList.Count > 0)
                    {
                        jobCount = pAllStoreList.Count;
                        JobStatusPercentage rJobStatusPercentage = new JobStatusPercentage();
                        rJobStatusPercentage.label = "Visit";
                        rJobStatusPercentage.data = (int)Math.Round((decimal)((pAllStoreList.Count - pStoreNonVistList.Count) * 100) / jobCount);
                        pJobStatusPercentageList.data.Add(rJobStatusPercentage);

                        JobStatusPercentage pJobStatusPercentage = new JobStatusPercentage();
                        pJobStatusPercentage.label = "Non Visit";
                        pJobStatusPercentage.data = (int)Math.Round((decimal)(pStoreNonVistList.Count * 100) / jobCount);
                        //Convert.ToInt32((pStoreNonVistList.Count * 100) / jobCount);
                        pJobStatusPercentageList.data.Add(pJobStatusPercentage);
                    }

                    if (pJobStatusPercentageList != null && pJobStatusPercentageList.data != null && pJobStatusPercentageList.data.Count > 0)
                    {
                        ViewBag.VisitPercentage = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(pJobStatusPercentageList.data);
                        ViewBag.Series = pJobStatusPercentageList.data.Count;
                        ViewBag.Flag = "true";
                    }
                    #endregion
                    //display UserActivity Graph
                    #region UserActivity Graph
                    Job uJob = new Job();
                    uJob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    if (Session["UserType"] != null && !string.IsNullOrEmpty(Session["UserType"].ToString()) && Session["UserType"].ToString() == "ENT_U" && Session["UserGUID"] != null)
                    {
                        GlobalUser globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(Session["UserGUID"].ToString()));
                        if (globalUser != null)
                        {
                            //User Activity
                            uJob.AssignedUserGUID = globalUser.UserGUID;
                        }
                    }
                    //FOr Regional Manager
                    if (Session["UserType"] != null && !string.IsNullOrEmpty(Session["UserType"].ToString()) && Session["UserType"].ToString() == "ENT_U_RM" && Session["UserGUID"] != null)
                    {
                        OrganizationUsersMap OrgUserMap = _IOrganizationRepository.GetOrganizationUserMapByUserGUID(new Guid(Session["UserGUID"].ToString()), new Guid(Session["OrganizationGUID"].ToString()));
                        if (OrgUserMap != null)
                        {
                            uJob.RegionGUID = OrgUserMap.RegionGUID;
                        }
                    }

                    if (!string.IsNullOrEmpty(pDayUser))
                    {
                        if (pDayUser == "Today")
                        {
                            uJob.ActualStartTime = DateTime.UtcNow;
                            uJob.ActualEndTime = DateTime.UtcNow;
                        }
                        if (pDayUser == "7 Days")
                        {
                            uJob.ActualStartTime = DateTime.UtcNow.AddDays(-6);
                            uJob.ActualEndTime = DateTime.UtcNow;
                        }
                        if (pDayUser == "30 Days")
                        {
                            uJob.ActualStartTime = DateTime.UtcNow.AddDays(-30);
                            uJob.ActualEndTime = DateTime.UtcNow;
                        }
                    }
                    ViewBag.Activity = "false";
                    var userActivityJobList = _IJobRepository.Job_UserActivityGraph(uJob) != null ? _IJobRepository.Job_UserActivityGraph(uJob).ToList() : null;
                    if (userActivityJobList != null && userActivityJobList.Count > 0)
                    {
                        //  int i = 0;
                        int arrayCount = 0;
                        int[,] myAL;
                        DateTime pdate = DateTime.Now;
                        if (pDayUser == "Today")
                        {
                            arrayCount = 3;
                            myAL = new int[3, 2];
                        }
                        else if (pDayUser == "7 Days")
                        {
                            arrayCount = 9;
                            myAL = new int[9, 2];
                            // pdate = DateTime.Now.AddDays(-7);
                            //pdate = userActivityJobList[0].datevalue;
                            pdate = userActivityJobList[0].GetType().GetProperties()[0].GetValue(userActivityJobList[0], null);
                        }
                        else if (pDayUser == "30 Days")
                        {
                            arrayCount = 32;
                            myAL = new int[32, 2];
                            //pdate = DateTime.Now.AddDays(-30);
                            //pdate = userActivityJobList[0].datevalue;
                            pdate = userActivityJobList[0].GetType().GetProperties()[0].GetValue(userActivityJobList[0], null);
                        }
                        else
                        {
                            //arrayCount = (int)(userActivityJobList[userActivityJobList.Count - 1].datevalue - userActivityJobList[0].datevalue).TotalDays;
                            arrayCount = (int)((userActivityJobList[userActivityJobList.Count - 1].GetType().GetProperties()[0].GetValue(userActivityJobList[userActivityJobList.Count - 1], null)) - (userActivityJobList[0].GetType().GetProperties()[0].GetValue(userActivityJobList[0], null))).TotalDays;
                            myAL = new int[arrayCount, 2];
                            //pdate = userActivityJobList[0].datevalue;
                            pdate = userActivityJobList[0].GetType().GetProperties()[0].GetValue(userActivityJobList[0], null);
                        }

                        //foreach (var item in userActivityJobList)
                        for (int i = 0; i < arrayCount; i++)
                        {
                            myAL[i, 0] = i;
                            int binded = 0;
                            for (int j = 0; j < userActivityJobList.Count; j++)
                            {
                                if (pdate.Date == userActivityJobList[j].GetType().GetProperties()[0].GetValue(userActivityJobList[j], null))
                                {
                                    myAL[i, 1] = userActivityJobList[j].GetType().GetProperties()[1].GetValue(userActivityJobList[j], null);
                                    binded = 1;
                                }
                            }
                            if (binded == 0)
                            {
                                myAL[i, 1] = 0;
                            }
                            pdate = pdate.AddDays(1);
                            ViewBag.Activity = "true";
                        }
                        ViewBag.UserActivityList = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(myAL);
                    }

                    #endregion

                    //ScriptManager.RegisterClientScriptBlock(this, this.Page.GetType(), "function", "", true);

                    #region store non visit

                    List<MarketModel> Marketmodel = new List<MarketModel>();
                    if (pStoreNonVistList != null && pStoreNonVistList.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(search))
                        {
                            search = search.ToLower();
                            pStoreNonVistList = pStoreNonVistList.Where(x => (!String.IsNullOrEmpty(x.RegionName) && x.RegionName.ToLower().Contains(search))
                                    || (!String.IsNullOrEmpty(x.MarketID) && x.MarketID.ToLower().Contains(search))
                                    || (!String.IsNullOrEmpty(x.MarketName) && x.MarketName.ToLower().Contains(search))
                                    ).ToList();
                        }

                        totalRecord = pStoreNonVistList.ToList().Count;
                        totalPage = (totalRecord / (int)ViewBag.pageCountValue) + ((totalRecord % (int)ViewBag.pageCountValue) > 0 ? 1 : 0);

                        ViewBag.TotalRows = totalRecord;
                        pStoreNonVistList = pStoreNonVistList.OrderBy(a => a.OrganizationGUID).Skip(((page - 1) * (int)ViewBag.pageCountValue)).Take((int)ViewBag.pageCountValue).ToList();

                        foreach (Market market in pStoreNonVistList)
                        {
                            Marketmodel.Add(ConvertToStoreNonVisit(market));
                        }
                    }
                    
                    return View(Marketmodel);
                    #endregion
                }


                else
                {
                    //  TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Login", "User");

            }
        }

        private MarketModel ConvertToStoreNonVisit(Market market)
        {
            try
            {
                MarketModel _market = new MarketModel();
                _market.MarketID = market.MarketID;
                _market.MarketGUID = market.MarketGUID.ToString();
                _market.UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString();
                _market.OrganizationGUID = market.OrganizationGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString();
                // _market.OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString();
                _market.MarketName = market.MarketName;
                // _market.MarketPhone = market.MarketPhone;
                // _market.PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString();
                //if (!string.IsNullOrEmpty(market.RMUserID))
                //{
                //    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(market.RMUserID, _market.OrganizationGUID);
                //    if (_globalUser != null)
                //    {
                //        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, new Guid(_market.OrganizationGUID));
                //        if (_userprofile != null)
                //        {
                //            _market.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                //        }
                //        else
                //        {
                //            _market.RMName = "";
                //        }
                //    }

                //}
                //else
                //{
                //    _market.RMName = "";
                //}
                //if (!string.IsNullOrEmpty(market.FMUserID))
                //{
                //    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(market.FMUserID, _market.OrganizationGUID);
                //    if (_globalUser != null)
                //    {
                //        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, new Guid(_market.OrganizationGUID));
                //        if (_userprofile != null)
                //        {
                //            _market.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                //        }
                //        else
                //        {
                //            _market.FMName = "";
                //        }
                //    }

                //}
                //else
                //{
                //    _market.FMName = "";
                //}
                //_market.FirstName = market.FirstName;
                //_market.LastName = market.LastName;
                //_market.MobilePhone = market.MobilePhone;
                //_market.HomePhone = market.HomePhone;
                //_market.Emails = market.Emails;
                //_market.AddressLine1 = market.AddressLine1;
                //_market.AddressLine2 = market.AddressLine2;
                //_market.City = market.City;
                //_market.State = market.State;
                //_market.Country = market.Country;
                //_market.ZipCode = market.ZipCode;
                //_market.RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString();
                //_market.TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString();
                _market.RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "";
                //_market.TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "";
                //_market.LastStoreVisitedDate = market.LastStoreVisitedDate != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(market.LastStoreVisitedDate, Session["TimeZoneID"].ToString()) : market.LastStoreVisitedDate.ToString() : "";

                return _market;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }


        }


        public int CalculatePercentage(float pTop, float pBelow)
        {
            float lDevide = pTop / pBelow;
            return (int)Math.Round(lDevide * 100f);
        }
        public enum FrequencyType
        {
            None = 0,
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
            Quarterly = 4,
            Annually = 5,
        }
        public DateTime[] GetRange(FrequencyType frequency, DateTime dateToCheck)
        {
            DateTime[] result = new DateTime[2];
            DateTime dateRangeBegin = dateToCheck;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0); //One day 
            DateTime dateRangeEnd = DateTime.Today.Add(duration);

            switch (frequency)
            {
                case FrequencyType.Daily:
                    dateRangeBegin = dateToCheck;
                    dateRangeEnd = dateRangeBegin;
                    break;

                case FrequencyType.Weekly:
                    //dateRangeBegin = dateToCheck.AddDays(-(int)dateToCheck.DayOfWeek);
                    //dateRangeEnd = dateToCheck.AddDays(6 - (int)dateToCheck.DayOfWeek);


                    dateRangeBegin = dateToCheck.AddDays(-6);
                    dateRangeEnd = dateToCheck.AddDays(0);
                    break;

                case FrequencyType.Monthly:
                    duration = new TimeSpan(DateTime.DaysInMonth(dateToCheck.Year, dateToCheck.Month) - 1, 0, 0, 0);
                    dateRangeBegin = dateToCheck.AddDays((-1) * dateToCheck.Day + 1);
                    dateRangeEnd = dateRangeBegin.Add(duration);
                    break;

                case FrequencyType.Quarterly:
                    int currentQuater = (dateToCheck.Date.Month - 1) / 3 + 1;
                    int daysInLastMonthOfQuarter = DateTime.DaysInMonth(dateToCheck.Year, 3 * currentQuater);
                    dateRangeBegin = new DateTime(dateToCheck.Year, 3 * currentQuater - 2, 1);
                    dateRangeEnd = new DateTime(dateToCheck.Year, 3 * currentQuater, daysInLastMonthOfQuarter);
                    break;

                case FrequencyType.Annually:
                    dateRangeBegin = new DateTime(dateToCheck.Year, 1, 1);
                    dateRangeEnd = new DateTime(dateToCheck.Year, 12, 31);
                    break;
            }
            result[0] = dateRangeBegin.Date;
            result[1] = dateRangeEnd.Date;
            return result;
        }

        public List<DateTime> GetDates(DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = new List<DateTime>();
            dates.Add(startDate);
            while ((startDate = startDate.AddDays(1)) < endDate)
            {
                dates.Add(startDate);
            }
            dates.Add(endDate);
            return dates;
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            Logger.Debug("Inside User Controller- Login HttpPost");
            string UpdatedSessionID = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    //MasterLogin aspuser = _IUserRepository.UserLoginServer(login.UserName, login.Password);
                    var aspuser = _IUserRepository.UserLogin(login.UserName, _IUserRepository.EncodeTo64(login.Password));
                    if (aspuser != null)
                    {

                        List<MasterLogin> masterlogins = new List<MasterLogin>();
                        MasterLogin lMasterLogin = new MasterLogin();
                        lMasterLogin.UserGUID = aspuser.UserGUID;
                        // Logger.Debug("Updatating MasterLogin Record" + aspuser.UserGUID);
                        lMasterLogin.LoginType = (short)eLoginType.WebLogin;
                        masterlogins = _IUserRepository.GetMasterLogin(lMasterLogin);

                        //var GeoloactionofCurrentIP = GeoIp.GetMy();
                        //double latitude = 0, longitude = 0, timezone = 0;
                        //if (GeoloactionofCurrentIP.KeyValue != null)
                        //{
                        //    latitude = Convert.ToDouble(GeoloactionofCurrentIP.KeyValue["Latitude"]);
                        //    longitude = Convert.ToDouble(GeoloactionofCurrentIP.KeyValue["Longitude"]);
                        //    timezone = getTimeZone(latitude, longitude);
                        //}

                        if (masterlogins != null && masterlogins.Count > 0)
                        {
                            #region masterlogins record available
                            lMasterLogin = masterlogins[0]; // Alok need to be fixed
                            // Update the Master Login
                            lMasterLogin.ExpiryTime = DateTime.UtcNow.AddYears(10);
                            Logger.Debug("Updatating MasterLogin Record");
                            UpdatedSessionID = _IUserRepository.UpdateMasterLogin(lMasterLogin);
                            if (!string.IsNullOrEmpty(UpdatedSessionID))
                            {
                                #region UpdatedSessionID is not null
                                Logger.Debug("Updated Session ID: " + UpdatedSessionID);
                                #endregion
                            }
                            else
                            {
                                Logger.Error("Unable to get the Session");
                            }
                            #endregion
                        }
                        else
                        {
                            #region masterlogins record not available
                            //Logger.Debug("Creating MasterLogin Record");
                            lMasterLogin.ExpiryTime = DateTime.UtcNow.AddYears(10);
                            if (CreateMasterLogin(lMasterLogin, ref UpdatedSessionID) > 0)
                            {
                                // UpdatedSessionID = lMasterLogin.SessionID;
                                Logger.Debug("New Session ID: " + UpdatedSessionID);
                            }
                            #endregion
                        }

                        HttpCookie currentLoggedInUser = Request.Cookies["LID"];
                        string PreviousLoginUserID = string.Empty;
                        if (currentLoggedInUser != null)
                        {
                            //Logger.Debug("currentLoggedInUser is not null");
                            PreviousLoginUserID = _IUserRepository.DecodeFrom64(currentLoggedInUser.Value);
                        }
                        else
                        {
                            //Logger.Debug("currentLoggedInUser is  null");
                            currentLoggedInUser = new HttpCookie("LID");
                        }
                        currentLoggedInUser.Value = _IUserRepository.EncodeTo64(aspuser.UserGUID.ToString());
                        // Logger.Debug("currentLoggedInUser value" + currentLoggedInUser.Value);
                        Response.Cookies.Add(currentLoggedInUser);

                        Session["UserGUID"] = aspuser.UserGUID.ToString();
                        Session["OrganizationGUID"] = _IOrganizationRepository.GetOrganizationIDByUserGUID(aspuser.UserGUID);
                        if (Session["OrganizationGUID"] != null && !string.IsNullOrEmpty(Session["OrganizationGUID"].ToString()))
                            Session["OrganizationName"] = _IOrganizationRepository.GetOrganizationByID(new Guid(Session["OrganizationGUID"].ToString())).OrganizationFullName;
                        Session["UserName"] = _IGlobalUserRepository.GetGlobalUserByID(aspuser.UserGUID).UserName;
                        Session["SessionID"] = UpdatedSessionID;
                        Session["UserType"] = _IGlobalUserRepository.GetUserType(aspuser.UserGUID);
                        // Logger.Debug("End of Session Assignment");
                        if (!string.IsNullOrEmpty(login.RedirectURL))
                        {
                            //return RedirectToAction(login.RedirectURL);
                            if (PreviousLoginUserID == aspuser.UserGUID.ToString())
                            {
                                login.RedirectURL = _IUserRepository.DecodeFrom64(login.RedirectURL);
                                Response.Redirect(login.RedirectURL);
                                return null;
                            }
                            else
                            {
                                return RedirectToAction("../User/Dashboard");
                            }
                        }
                        else
                        {
                            if (Session["UserType"].ToString() == "WIM_A")
                            {
                                return RedirectToAction("../User/Dashboard");
                            }
                            else
                            {
                                OrganizationSubscription OrgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(Session["OrganizationGUID"].ToString()));
                                if (OrgSubscription != null && OrgSubscription.IsActive == true)
                                {
                                    if (DateTime.Compare(Convert.ToDateTime(OrgSubscription.ExpiryDate), DateTime.UtcNow) >= 0)
                                    {
                                        UserSubscription userSubscription = _IUserSubscriptionRepository.GetUserSubscriptionByUserID(new Guid(Session["UserGUID"].ToString()));
                                        if (userSubscription != null && userSubscription.IsActive)
                                        {
                                            return RedirectToAction("Dashboard");
                                        }
                                        else
                                        {
                                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion - Subscription','User is not Subscribed');</script>";
                                            return RedirectToAction("Login");
                                        }
                                    }
                                    else
                                    {
                                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion - Subscription','Organization Subscription is Expired');</script>";
                                        return RedirectToAction("Login");
                                    }
                                }
                                else
                                {
                                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion - Subscription','Organization is not Subscribed');</script>";
                                    return RedirectToAction("Login");
                                }
                            }
                        }

                    }
                    else
                    {
                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion - Login error','Enter valid User Name and Password');</script>";
                        return RedirectToAction("Login");
                    }

                }
                return View(login);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(login);
            }
        }
        private int CreateMasterLogin(MasterLogin pMasterLogin, ref string UpdatedSessionID)
        {
            int lResult;
            IUserRepository _IUserRepository = new UserRepository(new WorkersInMotionDB());
            MasterLogin lMasterLogin = new MasterLogin();
            lMasterLogin.LoginGUID = Guid.NewGuid();
            lMasterLogin.LoginType = pMasterLogin.LoginType;
            lMasterLogin.UserGUID = pMasterLogin.UserGUID;
            lMasterLogin.IsActive = true;
            lMasterLogin.SessionID = Guid.NewGuid().ToString();
            UpdatedSessionID = lMasterLogin.SessionID.ToString();
            lMasterLogin.ExpiryTime = pMasterLogin.ExpiryTime;
            lMasterLogin.SessionTimeOut = 60;
            lMasterLogin.IsLoggedIn = true;
            lMasterLogin.Phone = "";
            lMasterLogin.CreateDate = DateTime.UtcNow;
            lMasterLogin.CreateBy = pMasterLogin.UserGUID;
            lMasterLogin.LastModifiedDate = DateTime.UtcNow;
            lMasterLogin.LastModifiedBy = pMasterLogin.UserGUID;
            //_IUserRepository.InsertMasterLogin(lMasterLogin);
            if (_IUserRepository.InsertMasterLogin(lMasterLogin) > 0)
            {
                lResult = 1;
            }
            else
            {
                lResult = 0;

            }
            return lResult;
        }

        [SessionExpireFilter]
        public ActionResult UserName()
        {
            if (Session["UserGUID"] != null)
            {
                return Content("Welcome " + _IGlobalUserRepository.GetGlobalUserByID(new Guid(Session["UserGUID"].ToString())).UserName);
            }
            else
                return Content("");
        }

        [SessionExpireFilter]
        public ActionResult UserDetails()
        {
            if (Session["UserGUID"] != null)
                return Content(_IGlobalUserRepository.GetGlobalUserByID(new Guid(Session["UserGUID"].ToString())).UserName);
            else
                return Content("");
        }
        [SessionExpireFilter]
        public ActionResult PendingJobList()
        {
            StringBuilder sb = new StringBuilder();
            Job lJob = new Job();
            Logger.Debug("Inside User Controller- Pending Job List");
            try
            {
                if (Session["UserType"] != null && Session["UserType"].ToString() != "WIM_A" && Session["UserType"].ToString() != "ENT_A")
                {
                    lJob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    lJob.IsDeleted = false;
                    int jscount = _IJobRepository.GetOpenJobs(lJob).ToList().Count;
                    sb.Append("<li class='dropdown' id='header_notification_bar'>");
                    sb.Append("<a href='#' class='dropdown-toggle' data-toggle='dropdown' data-hover='dropdown' data-close-others='true'>");
                    sb.Append("<i class='icon-warning-sign'></i>");
                    sb.Append("<span class='badge'>" + jscount + "</span>");
                    sb.Append("</a>");
                    sb.Append("<ul class='dropdown-menu extended notification'>");
                    sb.Append("<li>");
                    sb.Append("<p id='pjobstatus'>You have " + jscount + "  pending in queue");
                    sb.Append("</p>");
                    sb.Append("</li>");

                    sb.Append("<li>");
                    // sb.Append("<div class='slimScrollDiv' style='position: relative; overflow: hidden; width: auto; height: 100px;'>");
                    sb.Append("<ul class='dropdown-menu-list scroller' style='height: 100px; overflow: hidden; width: auto;'>");

                    List<Job> ListJob = _IJobRepository.GetOpenJobs(lJob).ToList();
                    if (ListJob != null && ListJob.Count > 0)
                    {
                        foreach (Job job in ListJob)
                        {
                            sb.Append("<li><a href='#'><span class='label label-sm label-icon label-info'><i class='icon-bell'></i></span>" + job.JobName + "  | <span class='time'>" + _IJobRepository.GetCustomerName(new Guid(job.CustomerGUID.ToString())) + "</span></a></li>");
                        }
                    }


                    sb.Append("</ul>");

                    sb.Append("</li>");
                    sb.Append("<li class='external'>");
                    if (Session["UserType"] != null && Session["UserType"].ToString() != "ENT_U")
                    {
                        sb.Append("<a href='/Job/Index'>See Pending Job Report<i class='m-icon-swapright'></i></a>");
                    }
                    else
                    {
                        sb.Append("<a href='#'>See Pending Job Report<i class='m-icon-swapright'></i></a>");
                    }
                    sb.Append("</li>");
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.Length = 0;
                sb.Append("");
                Logger.Error(ex.Message);
            }
            return Content(sb.ToString());
        }
        [SessionExpireFilter]
        public ActionResult JobStatusList()
        {
            StringBuilder sb = new StringBuilder();
            Logger.Debug("Inside User Controller- Job Status List");
            try
            {
                if (Session["UserType"] != null && Session["UserType"].ToString() != "WIM_A" && Session["UserType"].ToString() != "ENT_A")
                {
                    int jscount = _IJobRepository.GetjobStatusByRegionAndTerritory(new Guid(Session["UserGUID"].ToString())).ToList().Count;
                    sb.Append("<li class='dropdown' id='header_task_bar'>");
                    sb.Append("<a href='#' class='dropdown-toggle' data-toggle='dropdown' data-hover='dropdown' data-close-others='true'>");
                    sb.Append("<i class='icon-tasks'></i>");
                    sb.Append("<span class='badge'>" + jscount + "</span>");
                    sb.Append("</a>");
                    sb.Append("<ul class='dropdown-menu extended tasks'>");
                    sb.Append("<li>");
                    sb.Append("<p id='pjobstatus'>You have " + jscount + "  job status in queue");
                    sb.Append("</p>");
                    sb.Append("</li>");

                    sb.Append("<li>");

                    sb.Append("<ul class='dropdown-menu-list scroller' style='height: 100px; overflow: hidden; width: auto;'>");

                    List<Job> ListJob = _IJobRepository.GetjobStatusByRegionAndTerritory(new Guid(Session["UserGUID"].ToString())).ToList();
                    if (ListJob != null && ListJob.Count > 0)
                    {
                        foreach (Job job in ListJob)
                        {
                            if (job.StatusCode == 1)
                                sb.Append("<li><a href='#'><span class='task'><span class='desc'>" + job.JobName + " (Open)</span><span class='percent'>40%</span></span><span class='progress'><span style='width: 40%;' class='progress-bar progress-bar-success' aria-valuenow='40' aria-valuemin='0' aria-valuemax='100'> <span class='sr-only'>40% Complete</span></span></span></a></li>");
                            else if (job.StatusCode == 2)
                                sb.Append("<li><a href='#'><span class='task'><span class='desc'>" + job.JobName + " (Assigned)</span><span class='percent'>40%</span></span><span class='progress'><span style='width: 40%;' class='progress-bar progress-bar-success' aria-valuenow='40' aria-valuemin='0' aria-valuemax='100'> <span class='sr-only'>40% Complete</span></span></span></a></li>");
                            else
                                sb.Append("<li><a href='#'><span class='task'><span class='desc'>" + job.JobName + " (Job Start)</span><span class='percent'>65%</span></span><span class='progress progress-striped'><span style='width: 65%;' class='progress-bar progress-bar-danger' aria-valuenow='65' aria-valuemin='0' aria-valuemax='100'> <span class='sr-only'>65% Complete</span></span></span></a></li>");
                        }
                    }
                    sb.Append("</ul>");

                    sb.Append("</li>");
                    sb.Append("<li class='external'>");
                    sb.Append("<a href='/JobStatus/Index'>See Job Status Report<i class='m-icon-swapright'></i>");
                    sb.Append("</a>");
                    sb.Append("</li>");
                    sb.Append("</ul>");
                    sb.Append("</li>");
                }
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.Length = 0;
                sb.Append("");
                Logger.Error(ex.Message);
            }
            return Content(sb.ToString());
        }

        [SessionExpireFilter]
        public ActionResult JobDetail()
        {

            StringBuilder sb = new StringBuilder();
            Logger.Debug("Inside User Controller- JobDetail");
            try
            {
                if (Session["UserType"] != null && Session["UserType"].ToString() != "WIM_A" && Session["UserType"].ToString() != "ENT_A")
                {
                    sb.Append("<li id='lifirst'>");
                    if (Session["UserType"].ToString() != "ENT_U")
                    {
                        sb.Append("<a href='/Job/Index'>");
                    }
                    else
                    {
                        sb.Append("<a href='#'>");
                    }
                    sb.Append("<i class='icon-envelope'>");
                    //  sb.Append("</i>&nbsp;Pending Jobs<span id='spanpendingjobs' class='badge badge-danger'>" + _IJobRepository.GetjobByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList().Count + "</span>");

                    sb.Append("</i>&nbsp;Pending Jobs<span id='spanpendingjobs' class='badge badge-danger'>0</span>");
                    sb.Append("</a>");
                    sb.Append("</li>");
                    sb.Append("<li id='lisecond'>");
                    sb.Append("<a href='/JobStatus/Index'>");
                    sb.Append("<i class='icon-tasks'>");
                    //sb.Append("</i>&nbsp;Job Status<span id='spanjobstatus' class='badge badge-success'>" + _IJobRepository.GetjobStatusByRegionAndTerritory(new Guid(Session["UserGUID"].ToString())).ToList().Count + "</span>");
                    sb.Append("</i>&nbsp;Job Status<span id='spanjobstatus' class='badge badge-success'>0</span>");
                    sb.Append("</a>");
                    sb.Append("</li>");
                    sb.Append("<li id='lithrid'>");
                    sb.Append("<a href='#'>");
                    sb.Append("<i class='icon-calendar'>");
                    sb.Append("</i>&nbsp;Work Calendar");
                    sb.Append("</a>");
                    sb.Append("</li>");
                    sb.Append("<li class='divider'></li>");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return Content(sb.ToString());
        }
        [SessionExpireFilter]
        public ActionResult Menu()
        {
            StringBuilder sb = new StringBuilder();
            Job lJob = new Job();
            if (Session["UserType"] != null && Session["UserType"].ToString() == "WIM_A")
            {
                sb.Append("<ul class='page-sidebar-menu'>");
                sb.Append("<li>");
                sb.Append("<div class='sidebar-toggler hidden-phone'></div>");
                sb.Append("</li>");
                sb.Append("<li></br></li>");

                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-user'></i><span class='title'>Softtrends</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/User/Dashboard'>Dashboard</a></li>");
                sb.Append("<li class=''><a href='/User/Edit/" + Session["UserGUID"].ToString() + "?Account=True'>Account</a></li>");
                sb.Append("<li><a href='/Organization/Index'>Organizations</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                sb.Append("</ul>");


            }
            else if (Session["UserType"] != null && (Session["UserType"].ToString() == "ENT_A"))
            {
                sb.Append("<ul class='page-sidebar-menu'>");
                sb.Append("<li>");
                sb.Append("<div class='sidebar-toggler hidden-phone'></div>");
                sb.Append("</li>");
                sb.Append("<li></br></li>");

                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-user'></i><span class='title' style='text-transform:capitalize'>" + Session["OrganizationName"].ToString() + "</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/User/Dashboard'>Dashboard</a></li>");
                sb.Append("<li class=''><a href='/User/Edit/" + Session["UserGUID"].ToString() + "?Account=true'>My Account</a></li>");
                sb.Append("<li class=''><a href='/MyCompany/Index'>My Company</a></li>");
                //sb.Append("<li class=''><a href='/User/Index'>Users</a></li>");
                //sb.Append("<li><a href='/OrganizationSubscription/Index/" + Session["OrganizationGUID"].ToString() + "'>Manage Subscription</a></li>");
                //sb.Append("<li class=''><a href='/User/Dashboard'>Settings</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-group'></i><span class='title'>Client Information</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/Place/Index'>Clients</a></li>");
                sb.Append("<li class=''><a href='/PO/Index'>Client POs</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                //sb.Append("<li class=''>");
                //sb.Append("<a href=''><i class='icon-cogs'></i><span class='title'>Visit Tracking</span><span class='arrow'></span></a>");
                //sb.Append("<ul class='sub-menu' style='display: none;'>");
                //sb.Append("<li class=''><a href='/JobStatus/Index'>Visit List</a></li>");
                ////sb.Append("<li class=''><a href='/Job/Create'>Create Job</a></li>");
                //sb.Append("</ul>");
                //sb.Append("</li>");
                //sb.Append("<li class=''>");
                //sb.Append("<a href=''><i class='icon-cogs'></i><span class='title'>Settings</span><span class='arrow'></span></a>");
                //sb.Append("<ul class='sub-menu' style='display: none;'>");
                //sb.Append("<li class=''><a href='/Territory/Index'><span class='badge badge-roundless badge-info'>Add/Manage</span> Territory & Region</a></li>");
                //sb.Append("<li class=''><a href='/Group/Index'><span class='badge badge-roundless badge-info'>Add/Manage</span> Worker Groups</a></li>");
                //sb.Append("<li class=''><a href='/ServicePoint/Index'><span class='badge badge-info'>" + _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 0).Count() + "</span>Service Points</a></li>");
                //sb.Append("<li class=''><a href='/JobSchema/Index'>Visit Forms</a></li>");
                //sb.Append("</ul>");
                //sb.Append("</li>");

                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-cogs'></i><span class='title'>Reports</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/StoreVisit/Index'>Store Visit Reports</a></li>");
                sb.Append("<li class=''><a href='/SiteVisit/Index'>Site Visit Reports</a></li>");
                sb.Append("<li class=''><a href='/UserActivities/Index'>User Activities</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                sb.Append("</ul>");

            }
            else if (Session["UserType"] != null && (Session["UserType"].ToString() == "ENT_U_RM" || Session["UserType"].ToString() == "ENT_U_TM" || Session["UserType"].ToString() == "ENT_OM"))
            {

                sb.Append("<ul class='page-sidebar-menu'>");
                sb.Append("<li>");
                sb.Append("<div class='sidebar-toggler hidden-phone'></div>");
                sb.Append("</li>");
                sb.Append("<li></br></li>");

                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-user'></i><span class='title'>User</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/User/Dashboard'>Dashboard</a></li>");
                sb.Append("<li class=''><a href='/User/Edit/" + Session["UserGUID"].ToString() + "?Account=true'>My Account</a></li>");
                sb.Append("<li class=''><a href='/MyCompany/Index'>My Company</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-group'></i><span class='title'>Client Information</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/Place/Index'>Clients</a></li>");
                sb.Append("<li class=''><a href='/PO/Index'>Client POs</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                //sb.Append("<li class=''>");
                //sb.Append("<a href=''><i class='icon-cogs'></i><span class='title'>Settings</span><span class='arrow'></span></a>");
                //sb.Append("<ul class='sub-menu' style='display: none;'>");
                //sb.Append("<li class=''><a href='/ServicePoint/Index'><span class='badge badge-info'>" + _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 0).Count() + "</span>Service Points</a></li>");
                //sb.Append("</ul>");
                //sb.Append("</li>");
                //sb.Append("<li class=''>");
                //sb.Append("<a href=''><i class='icon-time'></i><span class='title'>Visit Tracking</span><span class='arrow'></span></a>");
                //sb.Append("<ul class='sub-menu' style='display: none;'>");
                lJob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                lJob.IsDeleted = false;
                //sb.Append("<li class=''><a href='/JobStatus/Index'>Visit List</a></li>");
                //// sb.Append("<li class=''><a href='/Job/Create'>Create Job</a></li>");
                //sb.Append("</ul>");
                //sb.Append("</li>");
                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-cogs'></i><span class='title'>Reports</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/StoreVisit/Index'>Store Visit Reports</a></li>");
                sb.Append("<li class=''><a href='/SiteVisit/Index'>Site Visit Reports</a></li>");
                sb.Append("<li class=''><a href='/UserActivities/Index'>User Activities</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                sb.Append("</ul>");
            }
            else if (Session["UserType"] != null && Session["UserType"].ToString() == "ENT_U")
            {
                sb.Append("<ul class='page-sidebar-menu'>");
                sb.Append("<li>");
                sb.Append("<div class='sidebar-toggler hidden-phone'></div>");
                sb.Append("</li>");
                sb.Append("<li></br></li>");

                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-user'></i><span class='title'>User</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/User/Dashboard'>Dashboard</a></li>");
                sb.Append("<li class=''><a href='/User/Edit/" + Session["UserGUID"].ToString() + "?Account=true'>Account</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-group'></i><span class='title'>Client Information</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/Place/Index'>Clients</a></li>");
                sb.Append("<li class=''><a href='/PO/Index'>Client POs</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                //sb.Append("<li class=''>");
                //sb.Append("<a href=''><i class='icon-cogs'></i><span class='title'>Settings</span><span class='arrow'></span></a>");
                //sb.Append("<ul class='sub-menu' style='display: none;'>");
                //sb.Append("<li class=''><a href='/ServicePoint/Index'><span class='badge badge-info'>" + _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 0).Count() + "</span>Service Points</a></li>");
                //sb.Append("</ul>");
                //sb.Append("</li>");
                //sb.Append("<li class=''>");
                //sb.Append("<a href=''><i class='icon-time'></i><span class='title'>Visit Tracking</span><span class='arrow'></span></a>");
                //sb.Append("<ul class='sub-menu' style='display: none;'>");
                //sb.Append("<li class=''><a href='/JobStatus/Index'>Visit List</a></li>");
                //// sb.Append("<li class=''><a href='/Job/Create'>Create Job</a></li>");
                //sb.Append("</ul>");
                //sb.Append("</li>");

                sb.Append("<li class=''>");
                sb.Append("<a href=''><i class='icon-cogs'></i><span class='title'>Reports</span><span class='arrow'></span></a>");
                sb.Append("<ul class='sub-menu' style='display: none;'>");
                sb.Append("<li class=''><a href='/StoreVisit/Index'>Store Visit Reports</a></li>");
                sb.Append("<li class=''><a href='/SiteVisit/Index'>Site Visit Reports</a></li>");
                sb.Append("<li class=''><a href='/UserActivities/Index'>User Activities</a></li>");
                sb.Append("</ul>");
                sb.Append("</li>");
                sb.Append("</ul>");
            }
            return Content(sb.ToString());
        }
        [SessionExpireFilter]
        public ActionResult Create()
        {
            if (Session["OrganizationGUID"] != null)
            {
                DropdownValues();
                ViewBag.OrganizationName = _IOrganizationRepository.GetOrganizationByID(new Guid(Session["OrganizationGUID"].ToString())).OrganizationFullName;
                return View();
            }
            else
            {
                return RedirectToAction("../User/Login");
            }
        }
        [SessionExpireFilter]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AspUser user, string GroupGUID, string RegionGUID, string TerritoryGUID, string RoleGUID)
        {
            Logger.Debug("Inside User Controller- Create HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    ViewBag.OrganizationName = _IOrganizationRepository.GetOrganizationByID(new Guid(Session["OrganizationGUID"].ToString())).OrganizationFullName;
                    if (ModelState.IsValid)
                    {
                        OrganizationSubscription orgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(Session["OrganizationGUID"].ToString()));
                        // if (orgSubscription.SubscriptionPurchased > orgSubscription.SubscriptionConsumed)
                        {
                            GlobalUser aspuser = _IUserRepository.GlobalUserLogin(user.UserName, Session["OrganizationGUID"].ToString());
                            GlobalUser aUser = _IGlobalUserRepository.GetGlobalUserByUserID(user.UserID, Session["OrganizationGUID"].ToString());
                            if (aspuser == null && aUser == null)
                            {
                                LatLong latLong = new LatLong();
                                latLong = GetLatLngCode(user.AddressLine1, user.AddressLine2, user.City, user.State, user.Country, user.ZipCode);
                                GlobalUser globalUser = new GlobalUser();
                                globalUser.UserGUID = Guid.NewGuid();
                                globalUser.USERID = user.UserID;
                                globalUser.Role_Id = RoleGUID;
                                globalUser.UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName.Trim() : "";
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

                                if (!string.IsNullOrEmpty(RegionGUID) && RegionGUID != Guid.Empty.ToString())
                                {
                                    organizationUserMap.RegionGUID = new Guid(RegionGUID);
                                }
                                else
                                {
                                    organizationUserMap.RegionGUID = null;
                                }
                                if (!string.IsNullOrEmpty(TerritoryGUID) && TerritoryGUID != Guid.Empty.ToString())
                                {
                                    organizationUserMap.TerritoryGUID = new Guid(TerritoryGUID);
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
                                userprofile.FirstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName.Trim() : "";
                                userprofile.LastName = !string.IsNullOrEmpty(user.LastName) ? user.LastName.Trim() : "";
                                userprofile.MobilePhone = user.MobilePhone;
                                userprofile.BusinessPhone = user.BusinessPhone;
                                userprofile.HomePhone = user.HomePhone;
                                userprofile.EmailID = !string.IsNullOrEmpty(user.EmailID) ? user.EmailID.Trim() : "";
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
                                aspnetuser.UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName.Trim() : "";
                                aspnetuser.FirstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName.Trim() : "";
                                aspnetuser.LastName = !string.IsNullOrEmpty(user.LastName) ? user.LastName.Trim() : "";
                                aspnetuser.PasswordHash = _IUserRepository.EncodeTo64(user.PasswordHash);
                                aspnetuser.PhoneNumber = user.MobilePhone;
                                aspnetuser.EmailID = !string.IsNullOrEmpty(user.EmailID) ? user.EmailID.Trim() : "";
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
                                        //int usrresult = _IUserProfileRepository.Save();
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
                                                    _IOrganizationSubscriptionRepository.UpdateOrganizationSubscription(orgSubscription);
                                                    //_IOrganizationSubscriptionRepository.Save();
                                                    return RedirectToAction("Index", "MyCompany", new { id = "Users" });
                                                }
                                                else
                                                {
                                                    _IUserSubscriptionRepository.DeleteUserSubscription(userSubscription.UserSubscriptionGUID);
                                                    //_IUserSubscriptionRepository.Save();
                                                    _IUserProfileRepository.DeleteUserProfile(userprofile.ProfileGUID);
                                                    // _IUserProfileRepository.Save();
                                                    _IGlobalUserRepository.DeleteGlobalUser(globalUser.UserGUID);
                                                    //_IGlobalUserRepository.Save();
                                                    _IUserRepository.DeleteUser(aspnetuser.Id);
                                                    //_IUserRepository.Save();
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
                                    // _IUserRepository.Save();
                                }
                            }
                            else if (aspuser != null)
                            {
                                //UserName already exists for this Organization
                                TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','User Aleady Exists');</script>";
                            }
                            else if (aUser != null)
                            {
                                //UserID already exists for this Organization
                                TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','UserID is Aleady Exists');</script>";
                            }
                        }
                    }
                    else
                    {
                        var TerritoryDetails = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(RegionGUID)).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
                        {
                            Value = r.TerritoryGUID.ToString(),
                            Text = r.Name
                        });
                        ViewBag.TerritoryDetails = new SelectList(TerritoryDetails, "Value", "Text");

                    }

                    return View(user);
                }
                else
                {
                    //TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    //return RedirectToAction("../User/Login");
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(user);
            }
        }
        [SessionExpireFilter]
        public ActionResult Edit(string id = "", string Account = "")
        {

            Logger.Debug("Inside User Controller- Edit");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();

                    AspUser user = new AspUser();
                    user.Id = id;
                    AspNetUser aspuser = _IUserRepository.GetUserByID(user.Id);
                    GlobalUser globaluser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(id));
                    UserProfile userprofile = _IUserProfileRepository.GetUserProfileByUserID(new Guid(id), new Guid(Session["OrganizationGUID"].ToString()));
                    OrganizationUsersMap organizationuserMap = _IOrganizationRepository.GetOrganizationUserMapByUserGUID(new Guid(id));
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        user.Id = globaluser.UserGUID.ToString();
                        user.UserID = globaluser.USERID;
                        user.RegionGUID = organizationuserMap.RegionGUID != null ? organizationuserMap.RegionGUID.ToString() : Guid.Empty.ToString();
                        user.TerritoryGUID = organizationuserMap.TerritoryGUID != null ? organizationuserMap.TerritoryGUID.ToString() : Guid.Empty.ToString();
                        // user.GroupGUID = globaluser.GroupGUID.ToString();
                        user.UserName = globaluser.UserName;
                        ViewBag.UserName = globaluser.UserName;
                        user.CompanyName = userprofile.CompanyName;
                        if (string.IsNullOrEmpty(Account))
                            ViewBag.BreadCrumbUserName = globaluser.UserName;

                        user.PasswordHash = _IUserRepository.DecodeFrom64(globaluser.Password);
                        user.ConfirmPassword = _IUserRepository.DecodeFrom64(globaluser.Password);
                        user.OrganizationGUID = organizationuserMap.OrganizationGUID.ToString();
                        user.OrganizationUserMapGUID = organizationuserMap.OrganizationUserMapGUID.ToString();
                        user.RoleGUID = globaluser.Role_Id;

                        string UserType = _IGlobalUserRepository.GetUserTypeByRoleID(user.RoleGUID);

                        if (UserType == "ENT_A")
                        {
                            var RoleDetails = _IGlobalUserRepository.GetRoles().ToList().Where(r => r.UserType != "WIM_A" && r.UserType != "IND_C").OrderBy(r => r.Name).Select(r => new SelectListItem
                            {
                                Value = r.Id.ToString(),
                                Text = r.Name
                            });
                            ViewBag.RoleDetails = new SelectList(RoleDetails, "Value", "Text");
                        }
                        else if (UserType == "WIM_A")
                        {
                            var RoleDetails = _IGlobalUserRepository.GetRoles().ToList().Where(r => r.UserType != "IND_C").OrderBy(r => r.Name).Select(r => new SelectListItem
                            {
                                Value = r.Id.ToString(),
                                Text = r.Name
                            });
                            ViewBag.RoleDetails = new SelectList(RoleDetails, "Value", "Text");
                        }

                        user.ProfileGUID = userprofile.ProfileGUID.ToString();
                        user.FirstName = userprofile.FirstName;
                        user.LastName = userprofile.LastName;
                        user.MobilePhone = userprofile.MobilePhone;
                        user.AddressLine1 = userprofile.AddressLine1;
                        user.AddressLine2 = userprofile.AddressLine2;
                        user.City = userprofile.City;
                        user.State = userprofile.State;
                        user.Country = userprofile.Country;
                        user.EmailID = userprofile.EmailID;
                        user.BusinessPhone = userprofile.BusinessPhone;
                        user.HomePhone = userprofile.HomePhone;
                        user.UserGUID = userprofile.UserGUID.ToString();
                        user.ZipCode = userprofile.ZipCode;

                        user.SecurityStamp = aspuser.SecurityStamp;
                        user.Discriminator = aspuser.Discriminator;

                        var TerritoryDetails = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(user.RegionGUID)).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
                        {
                            Value = r.TerritoryGUID.ToString(),
                            Text = r.Name
                        });
                        ViewBag.TerritoryDetails = new SelectList(TerritoryDetails, "Value", "Text");
                    }

                    return View(user);
                }
                else
                {
                    //TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    //return RedirectToAction("../User/Login");
                    return RedirectToAction("Login", "User");
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
        public ActionResult Edit(AspUser user, string GroupGUID, string RegionGUID, string TerritoryGUID, string RoleGUID)
        {
            Logger.Debug("Inside User Controller- Edit HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    ViewBag.UserName = user.UserName;
                    string oldpassword = _IUserRepository.DecodeFrom64(_IGlobalUserRepository.GetPassword(new Guid(user.UserGUID)));
                    if (ModelState.IsValid)
                    {
                        LatLong latLong = new LatLong();
                        latLong = GetLatLngCode(user.AddressLine1, user.AddressLine2, user.City, user.State, user.Country, user.ZipCode);
                        GlobalUser globalUser = new GlobalUser();
                        globalUser.UserGUID = new Guid(user.UserGUID);
                        globalUser.USERID = !string.IsNullOrEmpty(user.UserID) ? user.UserID.Trim() : "";
                        globalUser.Role_Id = RoleGUID;
                        globalUser.UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName.Trim() : "";
                        globalUser.Password = _IUserRepository.EncodeTo64(user.PasswordHash);
                        globalUser.IsActive = true;
                        globalUser.IsDelete = false;
                        globalUser.Latitude = latLong.Latitude;
                        globalUser.Longitude = latLong.Longitude;
                        globalUser.LastModifiedDate = DateTime.UtcNow;
                        if (Session["UserGUID"] != null)
                            globalUser.LastModifiedBy = new Guid(Session["UserGUID"].ToString());




                        if (_IGlobalUserRepository.GetRole(globalUser.Role_Id).UserType == "ENT_A")
                        {
                            var RoleDetails = _IGlobalUserRepository.GetRoles().ToList().Where(r => r.UserType != "WIM_A" && r.UserType != "IND_C").OrderBy(r => r.Name).Select(r => new SelectListItem
                            {
                                Value = r.Id.ToString(),
                                Text = r.Name
                            });
                            ViewBag.RoleDetails = new SelectList(RoleDetails, "Value", "Text");
                        }
                        else if (_IGlobalUserRepository.GetRole(globalUser.Role_Id).UserType == "WIM_A")
                        {
                            var RoleDetails = _IGlobalUserRepository.GetRoles().ToList().Where(r => r.UserType != "IND_C").OrderBy(r => r.Name).Select(r => new SelectListItem
                            {
                                Value = r.Id.ToString(),
                                Text = r.Name
                            });
                            ViewBag.RoleDetails = new SelectList(RoleDetails, "Value", "Text");
                        }

                        UserProfile userprofile = new UserProfile();
                        userprofile.ProfileGUID = new Guid(user.ProfileGUID);
                        userprofile.UserGUID = new Guid(user.UserGUID);
                        userprofile.CompanyName = user.CompanyName;
                        userprofile.FirstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName.Trim() : "";
                        userprofile.LastName = !string.IsNullOrEmpty(user.LastName) ? user.LastName.Trim() : "";
                        userprofile.MobilePhone = user.MobilePhone;
                        userprofile.BusinessPhone = user.BusinessPhone;
                        userprofile.HomePhone = user.HomePhone;
                        userprofile.EmailID = !string.IsNullOrEmpty(user.EmailID) ? user.EmailID.Trim() : "";
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
                        aspnetuser.Id = user.UserGUID;
                        aspnetuser.UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName.Trim() : "";
                        aspnetuser.FirstName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName.Trim() : "";
                        aspnetuser.LastName = !string.IsNullOrEmpty(user.LastName) ? user.LastName.Trim() : "";
                        aspnetuser.PasswordHash = _IUserRepository.EncodeTo64(user.PasswordHash);
                        aspnetuser.PhoneNumber = user.MobilePhone;
                        aspnetuser.EmailID = !string.IsNullOrEmpty(user.EmailID) ? user.EmailID.Trim() : "";
                        aspnetuser.OrganizationGUID = new Guid(user.OrganizationGUID);
                        aspnetuser.SecurityStamp = "";
                        aspnetuser.Discriminator = "";

                        OrganizationUsersMap organizationUserMap = new OrganizationUsersMap();
                        organizationUserMap.OrganizationUserMapGUID = new Guid(user.OrganizationUserMapGUID);
                        organizationUserMap.OrganizationGUID = new Guid(user.OrganizationGUID);
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



                        int userresult = _IUserRepository.UpdateUser(aspnetuser);
                        //int userresult = _IUserRepository.Save();
                        if (userresult > 0)
                        {
                            int guresult = _IGlobalUserRepository.UpdateGlobalUser(globalUser);
                            //int guresult = _IGlobalUserRepository.Save();
                            if (guresult > 0)
                            {
                                int usrresult = _IUserProfileRepository.UpdateUserProfile(userprofile);
                                //int usrresult = _IUserProfileRepository.Save();
                                if (usrresult > 0)
                                {

                                    if (_IOrganizationRepository.UpdateOrganizationUserMap(organizationUserMap) > 0)
                                    {
                                        if (oldpassword != user.PasswordHash)
                                        {
                                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Password Changed Successfully');</script>";
                                            if (user.UserGUID == Session["UserGUID"].ToString())
                                                return RedirectToAction("../User/Login");
                                            else
                                                return RedirectToAction("Index", "MyCompany", new { id = "Users" });
                                        }
                                        else if (Session["UserType"] != null && Session["UserType"].ToString() == "SFT_A")
                                        {
                                            return RedirectToAction("Index", "Organization");
                                        }
                                        else if (Session["UserType"] != null && Session["UserType"].ToString() == "ENT_A")
                                        {
                                            return RedirectToAction("Index", "MyCompany", new { id = "Users" });
                                        }
                                        else
                                        {
                                            return RedirectToAction("Dashboard", "User");
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else
                    {

                        if (Session["UserType"] != null && Session["UserType"].ToString() == "ENT_A")
                        {
                            var RoleDetails = _IGlobalUserRepository.GetRoles().ToList().Where(r => r.UserType != "WIM_A" && r.UserType != "IND_C").OrderBy(r => r.Name).Select(r => new SelectListItem
                            {
                                Value = r.Id.ToString(),
                                Text = r.Name
                            });
                            ViewBag.RoleDetails = new SelectList(RoleDetails, "Value", "Text");
                        }
                        else if (Session["UserType"] != null && Session["UserType"].ToString() == "WIM_A")
                        {
                            var RoleDetails = _IGlobalUserRepository.GetRoles().ToList().Where(r => r.UserType != "IND_C").OrderBy(r => r.Name).Select(r => new SelectListItem
                            {
                                Value = r.Id.ToString(),
                                Text = r.Name
                            });
                            ViewBag.RoleDetails = new SelectList(RoleDetails, "Value", "Text");
                        }
                        var TerritoryDetails = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(RegionGUID)).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
                        {
                            Value = r.TerritoryGUID.ToString(),
                            Text = r.Name
                        });
                        ViewBag.TerritoryDetails = new SelectList(TerritoryDetails, "Value", "Text");
                    }
                    return View(user);

                }
                else
                {
                    //TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    //return RedirectToAction("../User/Login");
                    return RedirectToAction("SessionTimeOut", "User");
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(user);
            }
        }
        [SessionExpireFilter]
        public ActionResult Delete(string id = "")
        {
            Logger.Debug("Inside User Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    AspUser user = new AspUser();
                    user.Id = id;
                    _IMarketRepository.DeleteMarketByUserGUID(new Guid(id));

                    _IOrganizationRepository.DeleteOrganizationUserMapByUserGUID(new Guid(id));
                    _IUserSubscriptionRepository.DeleteUserSubscriptionByUserGUID(new Guid(id));
                    _IGlobalUserRepository.DeleteGlobalUser(new Guid(id));
                    //_IGlobalUserRepository.Save();
                    _IUserProfileRepository.DeleteUserProfile(new Guid(id));
                    // _IUserProfileRepository.Save();
                    _IUserRepository.DeleteUser(user.Id);
                    //_IUserRepository.Save();



                    OrganizationSubscription orgSubscription = _IOrganizationSubscriptionRepository.GetOrganizationSubscriptionByOrgID(new Guid(Session["OrganizationGUID"].ToString()));
                    orgSubscription.SubscriptionConsumed = orgSubscription.SubscriptionConsumed - 1;
                    _IOrganizationSubscriptionRepository.UpdateOrganizationSubscription(orgSubscription);
                    //_IOrganizationSubscriptionRepository.Save();
                    return RedirectToAction("Index", "MyCompany", new { id = "Users" });
                }
                else
                {
                    //TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    //return RedirectToAction("../User/Login");
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Index");
            }
        }
        public JsonResult Region(string regionguid)
        {
            Logger.Debug("Inside User Controller- Region");
            JsonResult result = new JsonResult();
            try
            {
                var TerritoryDetails = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(regionguid)).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
                {
                    Value = r.TerritoryGUID.ToString(),
                    Text = r.Name
                });

                result.Data = new SelectList(TerritoryDetails, "Value", "Text");
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return result;
            }
        }
        [SessionExpireFilter]
        public ActionResult Subscribe(string id = "")
        {
            Logger.Debug("Inside User Controller- Subscribe");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    UserSubscription userSubscription = _IUserSubscriptionRepository.GetUserSubscriptionBySubscriptionID(new Guid(id));
                    userSubscription.IsActive = true;
                    _IUserSubscriptionRepository.UpdateUserSubscription(userSubscription);
                    // _IUserSubscriptionRepository.Save();
                    return RedirectToAction("Index", "MyCompany", new { id = "Users" });
                }
                else
                {
                    //TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    //return RedirectToAction("../User/Login");
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Index");
            }
        }
        [SessionExpireFilter]
        public ActionResult UnSubscribe(string id = "")
        {
            Logger.Debug("Inside User Controller- Subscribe");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    UserSubscription userSubscription = _IUserSubscriptionRepository.GetUserSubscriptionBySubscriptionID(new Guid(id));
                    userSubscription.IsActive = false;
                    _IUserSubscriptionRepository.UpdateUserSubscription(userSubscription);
                    // _IUserSubscriptionRepository.Save();

                    return RedirectToAction("Index", "MyCompany", new { id = "Users" });
                }
                else
                {
                    //TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    //return RedirectToAction("../User/Login");
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("../User/Login");
        }
        public ActionResult TrailSubscription()
        {

            return View();
        }
        public ActionResult Agreement()
        {

            return View();
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

        public JsonResult GetTimeZoneID(string TimeZoneID)
        {
            Logger.Debug("Inside User Controller- GetTimeZoneID");
            JsonResult result = new JsonResult();
            result.Data = "";
            try
            {
                if (!string.IsNullOrEmpty(TimeZoneID))
                {
                    Session["TimeZoneID"] = TimeZoneID;
                    result.Data = "Success";
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return result;
            }
        }
    }
}