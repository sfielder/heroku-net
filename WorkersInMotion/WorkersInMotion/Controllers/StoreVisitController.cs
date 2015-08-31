using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.tool.xml;
using RazorPDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.Utility;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class StoreVisitController : BaseController
    {

        #region Constructor
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPeopleRepository _IPeopleRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IJobRepository _IJobRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        private readonly IPORepository _IPORepository;
        public StoreVisitController()
        {
            this._IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            this._IPORepository = new PORepository(new WorkersInMotionDB());
        }
        public StoreVisitController(WorkersInMotionDB context)
        {
            this._IOrganizationRepository = new OrganizationRepository(context);
            this._IPlaceRepository = new PlaceRepository(context);
            this._IPeopleRepository = new PeopleRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IJobRepository = new JobRepository(context);
            this._IUserRepository = new UserRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
            this._IPORepository = new PORepository(context);
        }

        #endregion

        public void pageVisitCountList(int selectedValue)
        {
            if (selectedValue == 0)
                selectedValue = 5;
            ViewBag.pageVisitCountValue = selectedValue;
            var countList = new List<SelectListItem>
            {
                         new SelectListItem { Text = "5", Value = "5"},
                         new SelectListItem { Text = "15", Value = "15"},
                         new SelectListItem { Text = "20", Value = "20"}
           };
            ViewBag.Visit_pCountList = new SelectList(countList, "Value", "Text", selectedValue);
        }
        public void pageNonVisitCountList(int selectedValue)
        {
            if (selectedValue == 0)
                selectedValue = 5;
            ViewBag.pageNonCountValue = selectedValue;
            var countList = new List<SelectListItem>
            {
                         new SelectListItem { Text = "5", Value = "5"},
                         new SelectListItem { Text = "15", Value = "15"},
                         new SelectListItem { Text = "20", Value = "20"}
           };
            ViewBag.NonVisit_pCountList = new SelectList(countList, "Value", "Text", selectedValue);
        }

        //
        // GET: /StoreVist/
        public ActionResult Index(string id = "", string customerid = "", string FromDate = "", string ToDate = "", string Date = "", string selection = "", string LastModifiedDate = "", string Visit_RowCount = "", string NonVisit_RowCount = "", int page = 1, string Visit_Search = "", string NonVisit_Search = "", string hdnrecordsize="")
        {
            Logger.Debug("Inside CustomerView Controller- Index");
            try
            {
                ViewBag.CustomerID = customerid;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.Date = Date;
                int visit_TotalPage = 0;
                int visit_TotalRecord = 0;
                int visit_pCount = 0;

                int nonvisit_TotalPage = 0;
                int nonvisit_TotalRecord = 0;
                int nonvisit_pCount = 0;
                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(Visit_RowCount))
                    {
                        int.TryParse(Visit_RowCount, out visit_pCount);
                        pageVisitCountList(visit_pCount);
                    }
                    else
                    {
                        pageVisitCountList(visit_pCount);
                    }
                    if (!string.IsNullOrEmpty(NonVisit_RowCount))
                    {
                        int.TryParse(NonVisit_RowCount, out nonvisit_pCount);
                        pageNonVisitCountList(nonvisit_pCount);
                    }
                    else
                    {
                        pageNonVisitCountList(nonvisit_pCount);
                    }

                    if (!string.IsNullOrEmpty(Date))
                    {
                        ViewBag.DateValue = HttpUtility.HtmlDecode(Date);
                        Session["DateValue"] = Date;
                    }
                    else if (!string.IsNullOrEmpty(LastModifiedDate))
                    {
                        Logger.Debug("Inside Last Modfied dislay");
                        if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                        {
                            DateTime pFrom = new DateTime(), pTo = new DateTime();
                            if (ParseExact(FromDate, "dd-MM-yyyy", out pFrom))
                            {
                                ParseExact(ToDate, "dd-MM-yyyy", out pTo);
                                Logger.Debug("Inside");
                                string datevalue = pFrom.ToString("MMMM d, yyyy") + " - " + pTo.ToString("MMMM d, yyyy");
                                ViewBag.DateValue = datevalue;
                                Session["DateValue"] = datevalue;
                            }
                            else
                            {
                                Logger.Debug("OutSide");
                                string datevalue = DateTime.Now.AddDays(-29).ToString("MMMM d, yyyy") + " - " + DateTime.Now.ToString("MMMM d, yyyy");
                                ViewBag.DateValue = datevalue;
                                Session["DateValue"] = datevalue;
                            }
                            Logger.Debug("From Parst : " + pFrom);
                            Logger.Debug("To Parse : " + pTo);
                        }
                        else
                        {
                            Logger.Debug("OutSide1");
                            string datevalue = DateTime.Now.AddDays(-29).ToString("MMMM d, yyyy") + " - " + DateTime.Now.ToString("MMMM d, yyyy");
                            ViewBag.DateValue = datevalue;
                            Session["DateValue"] = datevalue;
                        }
                    }
                    else
                    {
                        Logger.Error("OutSide2");
                        string datevalue = DateTime.Now.AddDays(-29).ToString("MMMM d, yyyy") + " - " + DateTime.Now.ToString("MMMM d, yyyy");
                        ViewBag.DateValue = datevalue;
                        Session["DateValue"] = datevalue;
                    }
                    StoreVisitReports pStoreVisitReports = new StoreVisitReports();
                    pStoreVisitReports.StoreVisitList = new List<StoreVisit>();
                    pStoreVisitReports.StoreNonVisitList = new List<MarketModel>();

                    Session["CustomerGUID"] = customerid;
                    if (!string.IsNullOrEmpty(id))
                    {
                        TempData["TabName"] = id;
                    }
                    else
                    {
                        TempData["TabName"] = "Visits";
                    }

                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb1 = new StringBuilder();
                    sb.Append("<div class='actions'>");
                    sb.Append("<div class='btn-group'>");
                    sb1.Append("<div class='actions'>");
                    sb1.Append("<div class='btn-group'>");
                    if (!string.IsNullOrEmpty(customerid))
                    {
                        Place pplace = _IPlaceRepository.GetPlaceByID(new Guid(customerid));
                        if (pplace != null)
                        {
                            Session["ClientCompanyName"] = pplace.PlaceName;
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>" + pplace.PlaceName + " <i class='icon-angle-down'></i></a>");
                            sb1.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>" + pplace.PlaceName + " <i class='icon-angle-down'></i></a>");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(selection) && selection == "All")
                        {
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>All <i class='icon-angle-down'></i></a>");
                            sb1.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>All <i class='icon-angle-down'></i></a>");
                        }
                        else
                        {
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>Select Client <i class='icon-angle-down'></i></a>");
                            sb1.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>Select Client <i class='icon-angle-down'></i></a>");
                        }
                    }
                    sb.Append("<ul id='ulworkgroup' style='height:100px;overflow-y:scroll' class='dropdown-menu pull-right'>");
                    sb1.Append("<ul id='ulworkgroup' style='height:100px;overflow-y:scroll' class='dropdown-menu pull-right'>");
                    if (string.IsNullOrEmpty(selection) || selection != "All")
                    {
                        //sb.Append("<li><a href=" + Url.Action("Index", new { id = "Visits", selection = "All" }) + ">All</a></li>");
                        //sb1.Append("<li><a href=" + Url.Action("Index", new { id = "Non-Visits", selection = "All" }) + ">All</a></li>");
                        sb.Append("<li><a onclick=\"RedirectAction('Visits','');\">All</a></li>");
                        sb1.Append("<li><a onclick=\"RedirectAction('NonVisits','');\">All</a></li>");
                    }

                    List<Place> PlaceList = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    foreach (Place item in PlaceList)
                    {
                        //sb.Append("<li><a href=" + Url.Action("Index", new { id = "Visits", customerid = item.PlaceGUID.ToString() }) + " data-groupguid=" + item.PlaceGUID + ">" + item.PlaceName + "</a></li>");
                        //sb1.Append("<li><a href=" + Url.Action("Index", new { id = "Non-Visits", customerid = item.PlaceGUID.ToString() }) + " data-groupguid=" + item.PlaceGUID + ">" + item.PlaceName + "</a></li>");

                        sb.Append("<li><a onclick=\"RedirectAction('Visits','" + item.PlaceGUID.ToString() + "');\" data-groupguid=" + item.PlaceGUID + ">" + item.PlaceName + "</a></li>");
                        sb1.Append("<li><a onclick=\"RedirectAction('NonVisits','" + item.PlaceGUID.ToString() + "');\" data-groupguid=" + item.PlaceGUID + ">" + item.PlaceName + "</a></li>");
                    }
                    sb.Append("</ul>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb1.Append("</ul>");
                    sb1.Append("</div>");
                    sb1.Append("</div>");
                    ViewBag.CustomerListVisit = sb.ToString();
                    ViewBag.CustomerListNonVisit = sb1.ToString();

                    Market _market = new Market();
                    Job ljob = new Job();
                    ljob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    _market.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                    if (!string.IsNullOrEmpty(customerid))
                    {
                        _market.OwnerGUID = new Guid(customerid);
                        ljob.CustomerGUID = new Guid(customerid);
                    }

                    DateTime From, To, Datecontent, LModifiedDate;

                    //LastModifiedDate = _IUserRepository.DecodeFrom64(LastModifiedDate);
                    if (!string.IsNullOrEmpty(LastModifiedDate) && ParseExact(LastModifiedDate, "dd-MM-yyyy", out LModifiedDate))
                    {
                        ljob.LastModifiedDate = LModifiedDate;
                        _market.LastStoreVisitedDate = LModifiedDate;
                    }
                    else if (!string.IsNullOrEmpty(FromDate) && ParseExact(FromDate, "dd-MM-yyyy", out From))
                        _market.LastStoreVisitedDate = From;
                    else
                        _market.LastStoreVisitedDate = DateTime.Now.AddDays(-29);

                    if (!string.IsNullOrEmpty(FromDate) && ParseExact(FromDate, "dd-MM-yyyy", out From))
                    {
                        ljob.ActualStartTime = From;
                    }
                    else
                    {
                        Logger.Error("From Else");
                        ljob.ActualStartTime = DateTime.Now.AddDays(-29);
                    }
                    if (!string.IsNullOrEmpty(ToDate) && ParseExact(ToDate, "dd-MM-yyyy", out To))
                    {
                        ljob.ActualEndTime = To;
                    }
                    else
                    {
                        Logger.Error("To Else");
                        ljob.ActualEndTime = DateTime.Now;
                    }
                    ljob.JobName = "Store Visit";
                    Logger.Debug("Job Object" + Newtonsoft.Json.JsonConvert.SerializeObject(ljob));

                    //FOr Regional Manager
                    if (Session["UserType"] != null && !string.IsNullOrEmpty(Session["UserType"].ToString()) && Session["UserType"].ToString() == "ENT_U_RM" && Session["UserGUID"] != null)
                    {
                        OrganizationUsersMap OrgUserMap = _IOrganizationRepository.GetOrganizationUserMapByUserGUID(new Guid(Session["UserGUID"].ToString()), ljob.OrganizationGUID);
                        if (OrgUserMap != null)
                        {
                            ljob.RegionGUID = OrgUserMap.RegionGUID;
                            _market.RegionGUID = OrgUserMap.RegionGUID;
                        }
                    }

                    //check the current user is Field Manager or not
                    if (Session["UserType"] != null && !string.IsNullOrEmpty(Session["UserType"].ToString()) && Session["UserType"].ToString() == "ENT_U" && Session["UserGUID"] != null)
                    {
                        GlobalUser globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(Session["UserGUID"].ToString()));
                        if (globalUser != null)
                        {
                            ljob.AssignedUserGUID = globalUser.UserGUID;
                            _market.FMUserID = globalUser.USERID;
                        }
                    }

                    List<Job> lstorevisit = _IJobRepository.GetStoreVisitJobs(ljob);
                    List<Market> lstorenonvisit = new List<Market>();
                    lstorenonvisit = _IMarketRepository.GetStoreNonVisit(_market);
                    if (lstorevisit != null && lstorevisit.Count > 0)
                    {
                        ViewBag.Visit_Search = Visit_Search;
                        if (!string.IsNullOrEmpty(Visit_Search))
                        {
                            Visit_Search = Visit_Search.ToLower();
                            lstorevisit = lstorevisit.Where(
                                p => (!String.IsNullOrEmpty(_IRegionRepository.GetRegionNameByRegionGUID(new Guid(p.RegionGUID.ToString()))) && _IRegionRepository.GetRegionNameByRegionGUID(new Guid(p.RegionGUID.ToString())).ToLower().Contains(Visit_Search))
                            || (!String.IsNullOrEmpty(p.CustomerStopGUID.ToString()) && _IMarketRepository.GetMarketByID(new Guid(p.CustomerStopGUID.ToString())).MarketID.ToLower().Contains(Visit_Search))
                            || (!String.IsNullOrEmpty(p.PONumber) && _IMarketRepository.GetMarketByCustomerID(p.OrganizationGUID, _IPORepository.GetPObyPoNumber(p.PONumber).PlaceID, _IPORepository.GetPObyPoNumber(p.PONumber).MarketID).MarketID.ToLower().Contains(Visit_Search))
                            || (!String.IsNullOrEmpty(p.CustomerStopGUID.ToString()) && _IMarketRepository.GetMarketByID(new Guid(p.CustomerStopGUID.ToString())).MarketName.ToLower().Contains(Visit_Search))
                            || (!String.IsNullOrEmpty(p.PONumber) && _IMarketRepository.GetMarketByCustomerID(p.OrganizationGUID, _IPORepository.GetPObyPoNumber(p.PONumber).PlaceID, _IPORepository.GetPObyPoNumber(p.PONumber).MarketID).MarketName.ToLower().Contains(Visit_Search))
                            || (!String.IsNullOrEmpty(_IJobRepository.GetStatusName((int)p.StatusCode)) && _IJobRepository.GetStatusName((int)p.StatusCode).ToLower().Contains(Visit_Search))
                            || (!String.IsNullOrEmpty(_IUserProfileRepository.GetUserProfileByUserID(_IGlobalUserRepository.GetGlobalUserByID(new Guid(p.ManagerUserGUID.ToString())).UserGUID, p.OrganizationGUID).ToString()) && _IUserProfileRepository.GetUserProfileByUserID(_IGlobalUserRepository.GetGlobalUserByID(new Guid(p.ManagerUserGUID.ToString())).UserGUID, p.OrganizationGUID).ToString().ToLower().Contains(Visit_Search))).ToList();
                        }

                        visit_TotalRecord = lstorevisit.ToList().Count;
                        visit_TotalPage = (visit_TotalRecord / (int)ViewBag.pageVisitCountValue) + ((visit_TotalRecord % (int)ViewBag.pageVisitCountValue) > 0 ? 1 : 0);

                        ViewBag.Visit_TotalRows = visit_TotalRecord;
                        lstorevisit = lstorevisit.OrderBy(a => a.OrganizationGUID).Skip(((page - 1) * (int)ViewBag.pageVisitCountValue)).Take((int)ViewBag.pageVisitCountValue).ToList();

                        foreach (Job job in lstorevisit)
                        {
                            StoreVisit storevisit = ConvertToStoreVisit(job);
                            if (storevisit != null)
                            {
                                pStoreVisitReports.StoreVisitList.Add(storevisit);
                            }
                        }
                    }

                    // Logger.Debug("Store Non Visit Count : " + lstorenonvisit.Count);
                    if (lstorenonvisit != null && lstorenonvisit.Count > 0)
                    {
                        ViewBag.NonVisit_Search = NonVisit_Search;
                        if (!string.IsNullOrEmpty(NonVisit_Search))
                        {
                            NonVisit_Search = NonVisit_Search.ToLower();
                            lstorenonvisit = lstorenonvisit.Where(
                                p => (!String.IsNullOrEmpty(p.RegionName) && p.RegionName.ToLower().Contains(NonVisit_Search))
                            || (!String.IsNullOrEmpty(p.MarketID) && p.MarketID.ToLower().Contains(NonVisit_Search))
                            || (!String.IsNullOrEmpty(p.MarketName) && p.MarketName.ToLower().Contains(NonVisit_Search))).ToList();
                        }

                        nonvisit_TotalRecord = lstorenonvisit.ToList().Count;
                        nonvisit_TotalPage = (nonvisit_TotalRecord / (int)ViewBag.pageNonCountValue) + ((nonvisit_TotalRecord % (int)ViewBag.pageNonCountValue) > 0 ? 1 : 0);

                        ViewBag.NonVisit_TotalRows = nonvisit_TotalRecord;
                        lstorenonvisit = lstorenonvisit.OrderBy(a => a.OrganizationGUID).Skip(((page - 1) * (int)ViewBag.pageNonCountValue)).Take((int)ViewBag.pageNonCountValue).ToList();

                        foreach (Market market in lstorenonvisit)
                        {
                            MarketModel _marketModel = ConvertToStoreNonVisit(market);
                            if (_marketModel != null)
                            {
                                pStoreVisitReports.StoreNonVisitList.Add(_marketModel);
                            }
                        }
                    }
                    Session["StoreVisit"] = pStoreVisitReports.StoreVisitList;
                    Session["StoreNonVisit"] = pStoreVisitReports.StoreNonVisitList;

                    if (!string.IsNullOrEmpty(Visit_RowCount))
                        ViewBag.pageVisitCountValue = int.Parse(Visit_RowCount);
                    else
                        ViewBag.pageVisitCountValue = 5;
                    if (!string.IsNullOrEmpty(NonVisit_RowCount))
                        ViewBag.pageNonCountValue = int.Parse(NonVisit_RowCount);
                    else
                        ViewBag.pageNonCountValue = 5;
                    
                    bool visit = false;
                    bool nonvisit = false;
                    if (null != Request && System.Text.RegularExpressions.Regex.IsMatch(Request.Url.ToString().Replace("-", ""), string.Format(@"\b{0}\b", "Visits")))
                        visit = true;
                    if (null != Request && System.Text.RegularExpressions.Regex.IsMatch(Request.Url.ToString().Replace("-", ""), string.Format(@"\b{0}\b", "NonVisits")))
                        nonvisit = true;

                    if (visit)
                        TempData["TabName"] = "Visits";
                    else if(nonvisit)
                        TempData["TabName"] = "Non-Visits";

                    return View(pStoreVisitReports);
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

        private StoreVisit ConvertToStoreVisit(Job job)
        {
            try
            {
                DateTime Datecontent, Datecontent1, Datecontent2;
                StoreVisit _store = new StoreVisit();
                _store.JobGUID = job.JobGUID;
                _store.JobName = job.JobName;
                _store.PONumber = job.PONumber;
                if (job.RegionGUID != null && !string.IsNullOrEmpty(job.RegionGUID.ToString()))
                {
                    _store.RegionGUID = new Guid(job.RegionGUID.ToString());
                    _store.RegionName = _IRegionRepository.GetRegionNameByRegionGUID(_store.RegionGUID);
                }
                else
                {
                    _store.RegionName = "";
                }
                if (job.TerritoryGUID != null && !string.IsNullOrEmpty(job.TerritoryGUID.ToString()))
                {
                    _store.TerritoryGUID = new Guid(job.TerritoryGUID.ToString());
                    _store.TerritoryName = _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(_store.TerritoryGUID);
                }
                else
                {
                    _store.TerritoryName = "";
                }
                _store.StatusCode = job.StatusCode != null ? Convert.ToInt32(job.StatusCode) : 0;

                _store.Status = _IJobRepository.GetStatusName(_store.StatusCode);
                if (DateTime.TryParse(job.ActualStartTime.ToString(), out Datecontent1))
                {
                    if (Session["TimeZoneID"] != null)
                    {
                        _store.ActualStartTime = _IUserRepository.GetLocalDateTime(Datecontent1, Session["TimeZoneID"].ToString());
                        _store.ActualStartTime = Convert.ToDateTime(_store.ActualStartTime).ToString("MM/dd/yy hh:mm tt");
                    }
                    else
                    {
                        _store.ActualStartTime = Datecontent1.ToString("MM/dd/yy hh:mm tt");
                    }
                }
                if (DateTime.TryParse(job.ActualEndTime.ToString(), out Datecontent2))
                {
                    if (Session["TimeZoneID"] != null)
                    {
                        _store.ActualEndTime = _IUserRepository.GetLocalDateTime(Datecontent2, Session["TimeZoneID"].ToString());
                        _store.ActualEndTime = Convert.ToDateTime(_store.ActualEndTime).ToString("MM/dd/yy hh:mm tt");
                    }
                    else
                    {
                        _store.ActualEndTime = Datecontent2.ToString("MM/dd/yy hh:mm tt");
                    }
                }
                if (DateTime.TryParse(job.LastModifiedDate.ToString(), out Datecontent))
                {
                    if (Session["TimeZoneID"] != null)
                    {
                        _store.Date = _IUserRepository.GetLocalDateTime(Datecontent, Session["TimeZoneID"].ToString());
                        _store.Date = Convert.ToDateTime(_store.Date).ToString("MM/dd/yy hh:mm tt");
                    }
                    else
                    {
                        _store.Date = Datecontent.ToString("MM/dd/yy hh:mm tt");
                    }
                }
                //Prabhu--While creating Job using device they are not sending location mismatch flag,so this code will not work
                //JobProgress ljobprogress = _IJobRepository.GetJobProgressMismatch(job.JobGUID, _store.StatusCode);
                //if (ljobprogress != null)
                //    _store.LocationMismatch = ljobprogress.LocationMismatch != null ? Convert.ToBoolean(ljobprogress.LocationMismatch) : false;
                //else
                //    _store.LocationMismatch = false;

                _store.CustomerStopGUID = job.CustomerStopGUID != null ? new Guid(job.CustomerStopGUID.ToString()) : Guid.Empty;

                _store.LocationMismatch = LocationMismatch(job.JobGUID, _store.CustomerStopGUID);

                if (_store.CustomerStopGUID != Guid.Empty)
                {
                    Market _Market = _IMarketRepository.GetMarketByID(_store.CustomerStopGUID);
                    if (_Market != null)
                    {
                        _store.CustomerFirstName = _Market.FirstName;
                        _store.CustomerLastName = _Market.LastName;
                        _store.CustomerStopName = _Market.MarketName;
                        _store.MarketID = _Market.MarketID;
                        if (!string.IsNullOrEmpty(_Market.RMUserID))
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.RMUserID, Session["OrganizationGUID"].ToString());
                            if (_globalUser != null)
                            {
                                UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                if (_userprofile != null)
                                {
                                    _store.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                }
                                else
                                {
                                    _store.RMName = "";
                                }
                            }

                        }
                        else
                        {
                            _store.RMName = "";
                        }
                        //As i discussed with samant sir,he told to get FiledManager Name from GlobalUser table by using ManagerUserGUID in JobTable
                        //if (!string.IsNullOrEmpty(_Market.FMUserID))
                        //{
                        //    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.FMUserID);
                        //    if (_globalUser != null)
                        //    {
                        //        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                        //        if (_userprofile != null)
                        //        {
                        //            _store.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                        //        }
                        //        else
                        //        {
                        //            _store.FMName = "";
                        //        }
                        //    }

                        //}
                        //else
                        //{
                        //    _store.FMName = "";
                        //}
                    }
                    else
                    {
                        _store.CustomerStopName = "";
                        _store.MarketID = "";
                        _store.RMName = "";
                        //_store.FMName = "";
                        _store.CustomerFirstName = "";
                        _store.CustomerLastName = "";
                    }

                }
                else if (!string.IsNullOrEmpty(job.PONumber))
                {
                    POs _po = _IPORepository.GetPObyPoNumber(job.PONumber);
                    if (_po != null)
                    {
                        Market _Market = _IMarketRepository.GetMarketByCustomerID(job.OrganizationGUID, _po.PlaceID, _po.MarketID);
                        if (_Market != null)
                        {

                            _store.CustomerFirstName = _Market.FirstName;
                            _store.CustomerLastName = _Market.LastName;
                            _store.CustomerStopName = _Market.MarketName;
                            _store.MarketID = _Market.MarketID;
                            if (!string.IsNullOrEmpty(_Market.RMUserID))
                            {
                                GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.RMUserID, Session["OrganizationGUID"].ToString());
                                if (_globalUser != null)
                                {
                                    UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                    if (_userprofile != null)
                                    {
                                        _store.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                    }
                                    else
                                    {
                                        _store.RMName = "";
                                    }
                                }

                            }
                            else
                            {
                                _store.RMName = "";
                            }
                            //As i discussed with samant sir,he told to get FiledManager Name from GlobalUser table by using ManagerUserGUID in JobTable
                            //if (!string.IsNullOrEmpty(_Market.FMUserID))
                            //{
                            //    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.FMUserID);
                            //    if (_globalUser != null)
                            //    {
                            //        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                            //        if (_userprofile != null)
                            //        {
                            //            _store.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                            //        }
                            //        else
                            //        {
                            //            _store.FMName = "";
                            //        }
                            //    }

                            //}
                            //else
                            //{
                            //    _store.FMName = "";
                            //}
                        }
                        else
                        {
                            _store.CustomerStopName = "";
                            _store.MarketID = "";
                            _store.RMName = "";
                            //_store.FMName = "";
                            _store.CustomerFirstName = "";
                            _store.CustomerLastName = "";
                        }

                    }
                    else
                    {
                        _store.CustomerStopName = "";
                        _store.MarketID = "";
                        _store.RMName = "";
                        //_store.FMName = "";
                        _store.CustomerFirstName = "";
                        _store.CustomerLastName = "";
                    }
                }
                else
                {
                    _store.CustomerStopName = "";
                    _store.MarketID = "";
                    _store.RMName = "";
                    //_store.FMName = "";
                    _store.CustomerFirstName = "";
                    _store.CustomerLastName = "";
                }
                if (job.ManagerUserGUID != null && job.ManagerUserGUID != Guid.Empty)
                {
                    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(job.ManagerUserGUID.ToString()));
                    if (_globalUser != null)
                    {
                        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                        if (_userprofile != null)
                        {
                            _store.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                        }
                        else
                        {
                            _store.FMName = "";
                        }
                    }
                }

                return _store;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);

                return null;
            }
        }

        private bool LocationMismatch(Guid JobGUID, Guid CustomerStopGUID)
        {
            bool result = false;
            List<JobProgress> jobProgressList = new List<JobProgress>();
            jobProgressList = _IJobRepository.GetJobProgress(JobGUID);
            Market pMarket = _IMarketRepository.GetMarketByID(CustomerStopGUID);
            if (pMarket != null && jobProgressList != null && jobProgressList.Count >= 2)
            {
                foreach (JobProgress jobProgress in jobProgressList)
                {
                    if (pMarket.Latitude != null && jobProgress.Latitude != null && pMarket.Longitude != null && jobProgress.Longitude != null)
                    {
                        if (pMarket.Latitude == jobProgress.Latitude && pMarket.Longitude == jobProgress.Longitude)
                        {
                            result = false;
                        }
                        else
                        {
                            result = true;
                            break;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
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
                _market.OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString();
                _market.MarketName = market.MarketName;
                _market.MarketPhone = market.MarketPhone;
                _market.PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString();
                if (!string.IsNullOrEmpty(market.RMUserID))
                {
                    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(market.RMUserID, _market.OrganizationGUID);
                    if (_globalUser != null)
                    {
                        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, new Guid(_market.OrganizationGUID));
                        if (_userprofile != null)
                        {
                            _market.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                        }
                        else
                        {
                            _market.RMName = "";
                        }
                    }

                }
                else
                {
                    _market.RMName = "";
                }
                if (!string.IsNullOrEmpty(market.FMUserID))
                {
                    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(market.FMUserID, Session["OrganizationGUID"].ToString());
                    if (_globalUser != null)
                    {
                        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, new Guid(_market.OrganizationGUID));
                        if (_userprofile != null)
                        {
                            _market.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                        }
                        else
                        {
                            _market.FMName = "";
                        }
                    }

                }
                else
                {
                    _market.FMName = "";
                }
                _market.FirstName = market.FirstName;
                _market.LastName = market.LastName;
                _market.MobilePhone = market.MobilePhone;
                _market.HomePhone = market.HomePhone;
                _market.Emails = market.Emails;
                _market.AddressLine1 = market.AddressLine1;
                _market.AddressLine2 = market.AddressLine2;
                _market.City = market.City;
                _market.State = market.State;
                _market.Country = market.Country;
                _market.ZipCode = market.ZipCode;
                _market.RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString();
                _market.TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString();
                _market.RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "";
                _market.TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "";
                _market.LastStoreVisitedDate = market.LastStoreVisitedDate != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(market.LastStoreVisitedDate, Session["TimeZoneID"].ToString()) : market.LastStoreVisitedDate.ToString() : "";

                return _market;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }


        }

        public ActionResult JobForm(string id = "", string type = "")
        {
            Logger.Debug("Inside Job Controller- View Job");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    ViewBag.Type = type;
                    Job _job = new Job();
                    JobFormNew pJobFormView = new JobFormNew();
                    if (!string.IsNullOrEmpty(type) && type == "Visit")
                    {
                        _job = _IJobRepository.GetJobByID(new Guid(id));
                        //pJobFormView.JobGUID = id;
                        if (_job != null)
                        {
                            ViewBag.JobName = _job.JobName;
                        }
                    }
                    else
                    {
                        Market _market = _IMarketRepository.GetMarketByID(new Guid(id));
                        if (_market != null)
                        {
                            _job = _IJobRepository.GetJobByCustomerStopGUID(new Guid(id));
                            if (_job != null)
                            {
                                ViewBag.JobName = _job.JobName;
                            }
                        }
                    }

                    if (_job != null && !string.IsNullOrEmpty(_job.JobForm))
                    {
                        pJobFormView = JobFormJsonConvert(_job.JobForm, "ImageURL", _job.JobGUID.ToString());
                    }
                    if (pJobFormView != null && pJobFormView.FormValues != null && pJobFormView.FormValues.Count > 0)
                    {
                        JobFormHeading JobFormHeading = GetJobFormDetails(_job);
                        if (JobFormHeading != null)
                        {
                            pJobFormView.JobFormHeading = JobFormHeading;
                        }
                        else
                        {
                            JobFormHeading = null;
                        }
                        pJobFormView.JobFormHeading = JobFormHeading;
                        pJobFormView.FormValues.OrderBy(x => x.ControlID);
                    }

                    return View(pJobFormView);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                if (Session["UserType"] != null && Session["UserType"].ToString() != "ENT_A")
                    return RedirectToAction("../JobStatus/Index");
                else
                    return RedirectToAction("Index");
            }
        }

        private JobFormNew JobFormJsonConvert(string jobfomJson, string urlname, string jobguid)
        {
            try
            {
                JobFormNew pJobFormView = (!string.IsNullOrEmpty(jobfomJson)) ? new JavaScriptSerializer().Deserialize<JobFormNew>(jobfomJson) : null;

                if (pJobFormView != null)
                {

                    if (pJobFormView.Values != null)
                    {
                        pJobFormView.FormValues = new List<JobFormValueDetails>();
                        //for (int i = 0; i < pJobFormView.Values.Count; i++)
                        foreach (JobFormValues pFormValues in pJobFormView.Values)
                        {
                            //JobFormValues pFormValues = pJobFormView.Values[i];
                            JobFormValueDetails pFormDetails = new JobFormValueDetails();
                            string[] Controls = pFormValues.ControlID.Split('_');
                            if (Controls.Length > 2)
                            {
                                int controlid, controltype;
                                pFormDetails.FormID = Controls[0];
                                if (int.TryParse(Controls[1], out controlid))
                                {
                                    pFormDetails.ControlID = controlid;
                                }
                                else
                                {
                                    pFormDetails.ControlID = 0;
                                }
                                if (int.TryParse(Controls[2], out controltype))
                                {
                                    pFormDetails.ControlType = (ControlType)controltype;
                                }
                                else
                                {
                                    pFormDetails.ControlType = 0;
                                }

                            }
                            int parentid;
                            pFormDetails.Value = pFormValues.Value;
                            pFormDetails.ControlLabel = pFormValues.ControlLabel;
                            if (int.TryParse(pFormValues.parentID, out parentid))
                            {
                                pFormDetails.parentID = parentid;
                            }
                            else
                            {
                                pFormDetails.parentID = 0;
                            }
                            pFormDetails.controlParentLabel = pFormValues.controlParentLabel;
                            pFormDetails.ValueID = pFormValues.ValueID;
                            pFormDetails.currentValueID = pFormValues.currentValueID;

                            pFormDetails.ImagePath = System.Configuration.ConfigurationManager.AppSettings.Get(urlname).ToString() + Session["OrganizationGUID"].ToString() + "/Jobs/" + pJobFormView.JobGUID;
                            pFormDetails.OrganizationGUID = Session["OrganizationGUID"].ToString();
                            pFormDetails.JobGUID = pJobFormView.JobGUID;
                            pJobFormView.FormValues.Add(pFormDetails);
                        }
                        pJobFormView.JobGUID = jobguid;
                    }
                }
                if (pJobFormView != null && pJobFormView.FormValues != null && pJobFormView.FormValues.Count > 0)
                {
                    pJobFormView.FormValues.OrderBy(x => x.ControlID);
                }
                if (!string.IsNullOrEmpty(jobguid))
                {
                    Job pjob = _IJobRepository.GetJobByID(new Guid(jobguid));
                    if (pjob != null)
                    {
                        List<JobProgress> jobProgressList = new List<JobProgress>();
                        jobProgressList = _IJobRepository.GetJobProgress(pjob.JobGUID);
                        List<string> coordinate = new List<string>();
                        int i = 0;

                        Market pMarket = pjob.CustomerStopGUID != null ? _IMarketRepository.GetMarketByID(new Guid(pjob.CustomerStopGUID.ToString())) : null;
                        if (pMarket != null)
                        {

                            pJobFormView.CoordinateList = new List<CoOrdinates>();

                            if (pMarket.Latitude != null && pMarket.Longitude != null)
                            {
                                CoOrdinates pCoOrdinates = new CoOrdinates();
                                pCoOrdinates.Latitude = Convert.ToDouble(pMarket.Latitude);
                                pCoOrdinates.Longitude = Convert.ToDouble(pMarket.Longitude);
                                pCoOrdinates.Address = pMarket.AddressLine1 + "<br><br/>" + pMarket.AddressLine2;
                                pCoOrdinates.City = pMarket.City;
                                pCoOrdinates.State = pMarket.State;
                                pCoOrdinates.Country = pMarket.Country;
                                pCoOrdinates.JobName = pjob.JobName;
                                pCoOrdinates.StoreName = pMarket.MarketName.ToString();
                                pCoOrdinates.Count = i;
                                i++;
                                pJobFormView.CoordinateList.Add(pCoOrdinates);

                                coordinate.Add(pMarket.Latitude.ToString() + "~" + pMarket.Longitude.ToString() + "~store~" + pCoOrdinates.StoreName.ToString());
                            }

                        }


                        if (jobProgressList != null && jobProgressList.Count > 0)
                        {
                            jobProgressList = jobProgressList.OrderBy(x => x.JobStatus).ToList();
                            if (pMarket == null)
                            {
                                pJobFormView.CoordinateList = new List<CoOrdinates>();
                            }
                            foreach (JobProgress pJobProgress in jobProgressList)
                            {
                                if (pJobProgress.Latitude != null && pJobProgress.Longitude != null)
                                {
                                    CoOrdinates pCoOrdinates = new CoOrdinates();
                                    pCoOrdinates.Latitude = Convert.ToDouble(pJobProgress.Latitude);
                                    pCoOrdinates.Longitude = Convert.ToDouble(pJobProgress.Longitude);
                                    pCoOrdinates.JobName = pjob.JobName;
                                    pCoOrdinates.Count = i;
                                    if (pJobProgress.JobStatus != null && Convert.ToInt32(pJobProgress.JobStatus) == 1)
                                    {

                                        pCoOrdinates.StartTime = pJobProgress.StartTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(pJobProgress.StartTime, Session["TimeZoneID"].ToString()) : pJobProgress.StartTime.ToString() : "";
                                        pCoOrdinates.StartTime = !string.IsNullOrEmpty(pCoOrdinates.StartTime) ? _IUserRepository.GetLocalDateTime(pJobProgress.StartTime, Session["TimeZoneID"].ToString()) : "";
                                        pCoOrdinates.StartTime = !string.IsNullOrEmpty(pCoOrdinates.StartTime) ? Convert.ToDateTime(pCoOrdinates.StartTime).ToString("MM/dd/yy hh:mm tt") : "";

                                        coordinate.Add(pCoOrdinates.Latitude.ToString() + "~" + pCoOrdinates.Longitude.ToString() + "~start~" + pCoOrdinates.JobName.ToString() + "~" + pCoOrdinates.StartTime.ToString());
                                    }
                                    else
                                    {
                                        pCoOrdinates.EndTime = pJobProgress.StartTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(pJobProgress.StartTime, Session["TimeZoneID"].ToString()) : pJobProgress.StartTime.ToString() : "";
                                        pCoOrdinates.EndTime = !string.IsNullOrEmpty(pCoOrdinates.EndTime) ? _IUserRepository.GetLocalDateTime(pJobProgress.StartTime, Session["TimeZoneID"].ToString()) : "";
                                        pCoOrdinates.EndTime = !string.IsNullOrEmpty(pCoOrdinates.EndTime) ? Convert.ToDateTime(pCoOrdinates.EndTime).ToString("MM/dd/yy hh:mm tt") : "";

                                        coordinate.Add(pCoOrdinates.Latitude.ToString() + "~" + pCoOrdinates.Longitude.ToString() + "~stop~" + pCoOrdinates.JobName.ToString() + "~" + pCoOrdinates.EndTime.ToString());
                                    }
                                    pJobFormView.CoordinateList.Add(pCoOrdinates);

                                }
                                i++;
                            }



                        }


                        //for displaying google static map in pdf
                        //first need to display job end coordinates
                        //second need to display job start coordinates
                        //Third need to display store coordinates
                        if (pJobFormView.CoordinateList != null && pJobFormView.CoordinateList.Count > 0)
                        {
                            pJobFormView.CoordinateList = pJobFormView.CoordinateList.OrderByDescending(x => x.Count).ToList();
                        }
                        ViewBag.CoOrdinates = coordinate;
                    }
                }
                return pJobFormView;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        public ActionResult GeneratePDFByID(string id = "", string type = "")
        {
            Logger.Debug("Inside Store Visit Controller- GeneratePDF");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    StringBuilder pVisit = new StringBuilder();
                    if (!string.IsNullOrEmpty(type) && type == "StoreVisit")
                    {
                        Job _job = _IJobRepository.GetJobByID(new Guid(id));
                        if (_job != null)
                        {
                            StoreVisit _storeVisit = ConvertToStoreVisit(_job);
                            if (_storeVisit != null)
                            {
                                Response.ContentType = "application/pdf";

                                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                                //pVisit.Append("<div>");

                                //pVisit.Append("<table style='clear:both;width:100%;border-spacing: 0px;border-color:black;color: black;border-collapse: collapse;font-family: verdana;font-size: 10px;border:1px solid black;' border='1' cellpadding='8'>");
                                //pVisit.Append("<thead>");

                                Session["VisitType"] = "Store Visit Report";
                                string datevalue = Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(DateTime.UtcNow, Session["TimeZoneID"].ToString()) : DateTime.UtcNow.ToString();
                                Response.AddHeader("content-disposition", "attachment;filename=" + Session["OrganizationName"].ToString() + "_StoreVisit_" + datevalue + ".pdf");
                                //Response.AddHeader("content-disposition", "attachment;filename=Visits.pdf");
                                //pVisit.Append("<tr>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Region</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Store ID</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Store Name</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Visit Date</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Status</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Field Manager</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Details</td>");
                                //pVisit.Append("</tr>");
                                //pVisit.Append("</thead>");

                                //pVisit.Append("<tbody>");
                                //pVisit.Append("<tr>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _storeVisit.RegionName + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _storeVisit.MarketID + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _storeVisit.CustomerStopName + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _storeVisit.Date + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _storeVisit.Status + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _storeVisit.FMName + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'><a href='#" + _storeVisit.JobGUID.ToString() + "'>Detail</a></td>");
                                ////pVisit.Append("<a href='#'>Detail</a>");
                                //pVisit.Append("</tr>");
                                //pVisit.Append("</tbody>");
                                //pVisit.Append("</table>");
                                //pVisit.Append("</div>");
                                if (_job != null)
                                {
                                    ViewBag.JobName = _job.JobName;
                                    JobFormNew pJobFormView = new JobFormNew();
                                    if (!string.IsNullOrEmpty(_job.JobForm))
                                    {
                                        pJobFormView = JobFormJsonConvert(_job.JobForm, "PDFImageURL", _job.JobGUID.ToString());
                                    }
                                    if (pJobFormView != null && pJobFormView.FormValues != null && pJobFormView.FormValues.Count > 0)
                                    {
                                        JobFormHeading JobFormHeading = GetJobFormDetails(_job);
                                        if (JobFormHeading != null)
                                        {
                                            pJobFormView.JobFormHeading = JobFormHeading;
                                        }
                                        else
                                        {
                                            pJobFormView.JobFormHeading = null;
                                        }

                                        pJobFormView.FormValues.OrderBy(x => x.ControlID);
                                        pVisit.Append(GetJobFormHTMLForStoreVisit(pJobFormView));
                                    }
                                    else
                                        pVisit.Append(GetJobFormHTMLForStoreVisit(pJobFormView));
                                }

                            }

                        }
                        else
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','No data to generate PDF');</script>";
                            return RedirectToAction("Index", "StoreVisit", new { id = "Visits" });
                        }
                    }
                    else if (!string.IsNullOrEmpty(type) && type == "StoreNonVisit")
                    {
                        Market pmarket = _IMarketRepository.GetMarketByID(new Guid(id));

                        if (pmarket != null)
                        {

                            MarketModel _marketModel = ConvertToStoreNonVisit(pmarket);
                            if (_marketModel != null)
                            {
                                //pVisit.Append("<div>");

                                //pVisit.Append("<table style='clear:both;width:100%;border-spacing: 0px;border-color:black;color: black;border-collapse: collapse;font-family: verdana;font-size: 10px;border:1px solid black;' border='1' cellpadding='8'>");
                                //pVisit.Append("<thead>");

                                Session["VisitType"] = "Store Non-Visit Report";
                                string datevalue = Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(DateTime.UtcNow, Session["TimeZoneID"].ToString()) : DateTime.UtcNow.ToString();
                                Response.AddHeader("content-disposition", "attachment;filename=" + Session["OrganizationName"].ToString() + "_StoreNonVisit_" + datevalue + ".pdf");
                                //Response.AddHeader("content-disposition", "attachment;filename=NonVisits.pdf");
                                //pVisit.Append("<tr>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Region</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Store ID</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Store Name</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Last Visit Date</td>");
                                ////pVisit.Append("<td style='font-weight:bold;font-size:10px;font-family:verdana'>Status</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Field Manager</td>");
                                //pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Details</td>");
                                //pVisit.Append("</tr>");
                                //pVisit.Append("</thead>");

                                //pVisit.Append("<tbody>");
                                //pVisit.Append("<tr>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _marketModel.RegionName + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _marketModel.MarketID + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _marketModel.MarketName + "</td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _marketModel.LastStoreVisitedDate + "</td>");
                                ////pVisit.Append("<td style='font-size:8px;font-family:verdana'></td>");
                                //pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + _marketModel.FirstName + " " + _marketModel.LastName + "</td>");
                                //Job _jobnew = _IJobRepository.GetJobByCustomerStopGUID(new Guid(_marketModel.MarketGUID));
                                //if (_jobnew != null)
                                //{
                                //    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'><a href='#" + _marketModel.MarketGUID.ToString() + "'>Detail</a></td>");
                                //}
                                //else
                                //{
                                //    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'></td>");
                                //}
                                //pVisit.Append("</tr>");
                                //pVisit.Append("</tbody>");
                                //pVisit.Append("</table>");
                                //pVisit.Append("</div>");

                                JobFormNew pJobFormView = new JobFormNew();

                                Job _job = _IJobRepository.GetJobByCustomerStopGUID(new Guid(_marketModel.MarketGUID));
                                if (_job != null)
                                {
                                    ViewBag.JobName = _job.JobName;

                                    if (!string.IsNullOrEmpty(_job.JobForm))
                                    {
                                        pJobFormView = JobFormJsonConvert(_job.JobForm, "PDFImageURL", _job.JobGUID.ToString());
                                    }
                                    if (pJobFormView != null && pJobFormView.FormValues != null && pJobFormView.FormValues.Count > 0)
                                    {
                                        JobFormHeading JobFormHeading = GetJobFormDetails(_job);
                                        if (JobFormHeading != null)
                                        {
                                            pJobFormView.JobFormHeading = JobFormHeading;
                                        }
                                        else
                                        {
                                            pJobFormView.JobFormHeading = null;
                                        }
                                        pJobFormView.FormValues.OrderBy(x => x.ControlID);
                                        pVisit.Append(GetJobFormHTMLForStoreNonVisit(pJobFormView));
                                    }

                                }
                            }
                        }
                        else
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','No data to generate PDF');</script>";
                            return RedirectToAction("Index", "StoreVisit", new { id = "Non-Visits" });
                        }
                    }

                    if (!string.IsNullOrEmpty(pVisit.ToString()))
                    {
                        Document document = new Document(PageSize.A4, 70, 55, 40, 25);
                        PdfWriter writer = PdfWriter.GetInstance(document, System.Web.HttpContext.Current.Response.OutputStream);
                        //writer.PageEvent = new PDFFooter();
                        document.Open();
                        TextReader txtReader = new StringReader(pVisit.ToString());
                        var xmlWorkerHelper = XMLWorkerHelper.GetInstance();
                        xmlWorkerHelper.ParseXHtml(writer, document, txtReader);
                        document.Close();
                        System.Web.HttpContext.Current.Response.Write(document);
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','No data to generate PDF');</script>";
                        return RedirectToAction("Index", "StoreVisit", new { id = "Non-Visits" });
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
            }
            return null;
        }

        private JobFormHeading GetJobFormDetails(Job _job)
        {
            if (_job != null)
            {
                Place _place = _job.CustomerGUID != null ? _IPlaceRepository.GetPlaceByID(new Guid(_job.CustomerGUID.ToString())) : null;
                Market _market = _job.CustomerStopGUID != null ? _IMarketRepository.GetMarketByID(new Guid(_job.CustomerStopGUID.ToString())) : null;
                if (_place != null && _market != null)
                {
                    JobFormHeading JobFormHeading = new JobFormHeading();
                    JobFormHeading.JobGUID = _job.JobGUID.ToString();
                    JobFormHeading.JobName = _job.JobName;
                    JobFormHeading.PlaceName = _place.PlaceName;
                    JobFormHeading.PlaceID = _place.PlaceID;
                    JobFormHeading.MarketName = _market.MarketName;
                    JobFormHeading.MarketID = _market.MarketID;
                    JobFormHeading.MarketAddress = (string.IsNullOrEmpty(_market.AddressLine1) ? "" : _market.AddressLine1 + ",") +
            (string.IsNullOrEmpty(_market.AddressLine2) ? "" : _market.AddressLine2 + ",") +
            (string.IsNullOrEmpty(_market.City) ? "" : _market.City + ",") +
            (string.IsNullOrEmpty(_market.State) ? "" : _market.State + ",") +
            (string.IsNullOrEmpty(_market.ZipCode) ? "" : _market.ZipCode);
                    JobFormHeading.CheckInTime = (Session["TimeZoneID"] != null && !string.IsNullOrEmpty(Session["TimeZoneID"].ToString())) ? _IUserRepository.GetLocalDateTime(_job.ActualStartTime, Session["TimeZoneID"].ToString()) : DateTime.UtcNow.ToString();
                    JobFormHeading.CheckInTime = !string.IsNullOrEmpty(JobFormHeading.CheckInTime) ? Convert.ToDateTime(JobFormHeading.CheckInTime).ToString("MM/dd/yy hh:mm tt") : "";
                    JobFormHeading.CheckOutTime = (Session["TimeZoneID"] != null && !string.IsNullOrEmpty(Session["TimeZoneID"].ToString())) ? _IUserRepository.GetLocalDateTime(_job.ActualEndTime, Session["TimeZoneID"].ToString()) : DateTime.UtcNow.ToString();
                    JobFormHeading.CheckOutTime = !string.IsNullOrEmpty(JobFormHeading.CheckOutTime) ? Convert.ToDateTime(JobFormHeading.CheckOutTime).ToString("MM/dd/yy hh:mm tt") : "";
                    JobFormHeading.Status = _IJobRepository.GetStatusName(_job.StatusCode != null ? Convert.ToInt32(_job.StatusCode) : 6);
                    JobFormHeading.PoNumber = _job.PONumber;
                    return JobFormHeading;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public ActionResult GeneratePDF(string pdfcontent)
        {
            Logger.Debug("Inside Store Visit Controller- GeneratePDF");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(pdfcontent.ToString()))
                    {
                        Response.ContentType = "application/pdf";

                        Response.Cache.SetCacheability(HttpCacheability.NoCache);

                        StringBuilder pVisit = new StringBuilder();
                        // pVisit.Append("<header>Prabhu</header>");
                        //pVisit.Append("<html>");
                        //pVisit.Append("<head>");
                        //pVisit.Append("<title></title>");
                        //pVisit.Append("</head>");
                        //pVisit.Append("<body>");
                        pVisit.Append("<div>");

                        pVisit.Append("<table style='clear:both;width:100%;border-spacing: 0px;border-color:black;color: black;border-collapse: collapse;font-family: verdana;font-size: 10px;border:1px solid black;' border='1' cellpadding='8'>");
                        pVisit.Append("<thead>");
                        if (pdfcontent == "StoreVisit" && Session["StoreVisit"] != null)
                        {
                            Session["VisitType"] = "Store Visit Report";
                            Response.AddHeader("content-disposition", "attachment;filename=Visits.pdf");
                            pVisit.Append("<tr>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Region</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Store ID</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Store Name</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Visit Date</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Status</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Field Manager</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Details</td>");
                            pVisit.Append("</tr>");
                            pVisit.Append("</thead>");
                            pVisit.Append("<tbody>");
                            List<StoreVisit> visit = new List<StoreVisit>();
                            visit = (List<StoreVisit>)Session["StoreVisit"];
                            if (visit.Count > 0)
                            {
                                foreach (StoreVisit item in visit)
                                {
                                    pVisit.Append("<tr>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.RegionName + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.MarketID + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.CustomerStopName + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.Date + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.Status + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.FMName + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'><a href='#" + item.JobGUID.ToString() + "'>Detail</a></td>");
                                    //pVisit.Append("<a href='#'>Detail</a>");
                                    pVisit.Append("</tr>");
                                }
                            }
                            else
                            {
                                TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','No data to generate PDF');</script>";
                                return RedirectToAction("Index", "StoreVisit", new { id = "Visits" });
                            }
                        }
                        else if (pdfcontent == "StoreNonVisit" && Session["StoreNonVisit"] != null)
                        {
                            Session["VisitType"] = "Store Non-Visit Report";
                            Response.AddHeader("content-disposition", "attachment;filename=NonVisits.pdf");
                            pVisit.Append("<tr>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Region</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Store ID</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Store Name</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Last Visit Date</td>");
                            //pVisit.Append("<td style='font-weight:bold;font-size:10px;font-family:verdana'>Status</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Field Manager</td>");
                            pVisit.Append("<td style='font-weight:bold;font-size:10px;border-color:black;font-family:verdana'>Details</td>");
                            pVisit.Append("</tr>");
                            pVisit.Append("</thead>");
                            pVisit.Append("<tbody>");
                            List<MarketModel> nonvisit = new List<MarketModel>();
                            nonvisit = (List<MarketModel>)Session["StoreNonVisit"];
                            if (nonvisit.Count > 0)
                            {
                                foreach (MarketModel item in nonvisit)
                                {
                                    pVisit.Append("<tr>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.RegionName + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.MarketID + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.MarketName + "</td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.LastStoreVisitedDate + "</td>");
                                    //pVisit.Append("<td style='font-size:8px;font-family:verdana'></td>");
                                    pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'>" + item.FirstName + " " + item.LastName + "</td>");
                                    Job _jobnew = _IJobRepository.GetJobByCustomerStopGUID(new Guid(item.MarketGUID));
                                    if (_jobnew != null)
                                    {
                                        pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'><a href='#" + item.MarketGUID.ToString() + "'>Detail</a></td>");
                                    }
                                    else
                                    {
                                        pVisit.Append("<td style='font-size:8px;border-color:black;font-family:verdana'></td>");
                                    }
                                    pVisit.Append("</tr>");
                                }
                            }
                            else
                            {
                                TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','No data to generate PDF');</script>";
                                return RedirectToAction("Index", "StoreVisit", new { id = "Non-Visits" });
                            }
                        }
                        else
                        {
                            pVisit.Append("<tr>");
                            pVisit.Append("<td style='font-weight:bold;border-color:black;font-size:10px;font-family:verdana'>Region</td>");
                            pVisit.Append("<td style='font-weight:bold;border-color:black;font-size:10px;font-family:verdana'>Store ID</td>");
                            pVisit.Append("<td style='font-weight:bold;border-color:black;font-size:10px;font-family:verdana'>Store Name</td>");
                            pVisit.Append("<td style='font-weight:bold;border-color:black;font-size:10px;font-family:verdana'>Visit Date</td>");
                            pVisit.Append("<td style='font-weight:bold;border-color:black;font-size:10px;font-family:verdana'>Status</td>");
                            pVisit.Append("<td style='font-weight:bold;border-color:black;font-size:10px;font-family:verdana'>Field Manager</td>");
                            pVisit.Append("<td style='font-weight:bold;border-color:black;font-size:10px;font-family:verdana'>Details</td>");
                            pVisit.Append("</tr>");
                            pVisit.Append("</thead>");
                            pVisit.Append("<tbody>");
                        }


                        pVisit.Append("</tbody>");
                        pVisit.Append("</table>");
                        pVisit.Append("</div>");
                        if (pdfcontent == "StoreVisit" && Session["StoreVisit"] != null)
                        {
                            List<StoreVisit> visit = new List<StoreVisit>();
                            visit = (List<StoreVisit>)Session["StoreVisit"];
                            if (visit.Count > 0)
                            {
                                List<string> jobNameList = new List<string>();
                                foreach (StoreVisit item in visit)
                                {
                                    JobFormNew pJobFormView = new JobFormNew();
                                    Job _job = new Job();
                                    _job = _IJobRepository.GetJobByID(item.JobGUID);
                                    if (_job != null)
                                    {
                                        ViewBag.JobName = _job.JobName;
                                        if (!jobNameList.Contains(_job.JobGUID.ToString()))
                                        {
                                            jobNameList.Add(_job.JobGUID.ToString());
                                            if (!string.IsNullOrEmpty(_job.JobForm))
                                            {
                                                pJobFormView = JobFormJsonConvert(_job.JobForm, "PDFImageURL", _job.JobGUID.ToString());
                                            }
                                            if (pJobFormView != null && pJobFormView.FormValues != null && pJobFormView.FormValues.Count > 0)
                                            {
                                                pJobFormView.FormValues.OrderBy(x => x.ControlID);
                                                //  pVisit.Append(GetJobFormHTMLForStoreVisit(pJobFormView, _job.JobName, _job.JobGUID.ToString(), _job.CustomerStopGUID.ToString(), _job.JobGUID.ToString(), _job.StatusCode != null ? Convert.ToInt32(_job.StatusCode) : 6));
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else if (pdfcontent == "StoreNonVisit" && Session["StoreNonVisit"] != null)
                        {
                            List<MarketModel> nonvisit = new List<MarketModel>();
                            nonvisit = (List<MarketModel>)Session["StoreNonVisit"];
                            if (nonvisit.Count > 0)
                            {
                                List<string> jobNameList = new List<string>();
                                foreach (MarketModel item in nonvisit)
                                {
                                    JobFormNew pJobFormView = new JobFormNew();
                                    Job _job = new Job();
                                    Market _market = _IMarketRepository.GetMarketByID(new Guid(item.MarketGUID));
                                    if (_market != null)
                                    {
                                        _job = _IJobRepository.GetJobByCustomerStopGUID(new Guid(item.MarketGUID));
                                        if (_job != null)
                                        {
                                            ViewBag.JobName = _job.JobName;
                                            if (!jobNameList.Contains(_job.CustomerStopGUID.ToString()))
                                            {
                                                jobNameList.Add(_job.CustomerStopGUID.ToString());
                                                if (!string.IsNullOrEmpty(_job.JobForm))
                                                {
                                                    pJobFormView = JobFormJsonConvert(_job.JobForm, "PDFImageURL", _job.JobGUID.ToString());
                                                }
                                                if (pJobFormView != null && pJobFormView.FormValues != null && pJobFormView.FormValues.Count > 0)
                                                {
                                                    pJobFormView.FormValues.OrderBy(x => x.ControlID);
                                                    // pVisit.Append(GetJobFormHTMLForStoreNonVisit(pJobFormView, _job.JobName, _job.CustomerStopGUID.ToString(), _job.CustomerGUID.ToString(), _job.StatusCode != null ? Convert.ToInt32(_job.StatusCode) : 6));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //pVisit.Append("</body>");
                        //pVisit.Append("</html>");



                        //byte[] byteArray = Encoding.UTF8.GetBytes(pVisit.ToString());
                        //MemoryStream ms = new MemoryStream(byteArray);

                        //Stream stream = ms;
                        //StreamReader sr = new StreamReader(stream);

                        ////Document document = new Document(new Rectangle(288f, 144f), 10f, 10f, 30f, 30f);
                        //Document document = new Document();

                        //FileStream fs = new FileStream(Request.PhysicalApplicationPath + "\\StoreVisit.pdf", FileMode.Create);
                        //PdfWriter writer = PdfWriter.GetInstance(document, fs);

                        Document document = new Document(PageSize.A4, 70, 55, 40, 25);
                        PdfWriter writer = PdfWriter.GetInstance(document, System.Web.HttpContext.Current.Response.OutputStream);

                        writer.PageEvent = new PDFFooter();
                        document.Open();

                        TextReader txtReader = new StringReader(pVisit.ToString());
                        var xmlWorkerHelper = XMLWorkerHelper.GetInstance();
                        xmlWorkerHelper.ParseXHtml(writer, document, txtReader);

                        //iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
                        //hw.Parse(sr);
                        document.Close();

                        //Response.ContentType = "application/pdf";

                        ////Set default file Name as current datetime
                        //Response.AddHeader("content-disposition", "attachment; filename=" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
                        System.Web.HttpContext.Current.Response.Write(document);

                        Response.Flush();
                        Response.End();
                        //WebClient client = new WebClient();
                        //Byte[] buffer = client.DownloadData(Request.PhysicalApplicationPath + "\\StoreVisit.pdf");
                        //return File(buffer, "application/pdf");
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
            }
            return null;
        }

        private string GetJobFormHTMLForStoreVisit(JobFormNew JobFormNew)
        {
            try
            {
                List<string> parentIDList = new List<string>();
                List<string> controlIDList = new List<string>();
                int imagecount = 0;
                List<WorkersInMotion.Model.ViewModel.JobFormValueDetails> FormValues = new List<WorkersInMotion.Model.ViewModel.JobFormValueDetails>();
                StringBuilder sbJobForm = new StringBuilder();
                //sbJobForm.Append("<div style='page-break-before:always'>&nbsp;</div>");
                sbJobForm.Append("<html>");
                sbJobForm.Append("<head><script type='text/javascript' src='http://maps.googleapis.com/maps/api/js?sensor=false'></script></head>");
                sbJobForm.Append("<body>");
                #region Generate Form
                if (JobFormNew.JobFormHeading != null)
                {

                    sbJobForm.Append("<div id='" + JobFormNew.JobFormHeading.JobGUID + "' name='" + JobFormNew.JobFormHeading.JobGUID + "'>");
                    sbJobForm.Append("<div align='center'>");
                    sbJobForm.Append("<table style='width:100%' align='center' cellpadding='0'>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td colspan='2' style='font-size:18px;font-family:verdana;font-weight:bold;text-align:center;'>" + JobFormNew.JobFormHeading.JobName + "&nbsp;Report</td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td colspan='2' style='font-size:18px;font-family:verdana;font-weight:bold;text-align:center;'>&nbsp;</td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Client Name :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.PlaceName + "</span> </td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>Client ID :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.PlaceID + "</span> </td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Store ID :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketID + "</span> </td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>Check-In :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.CheckInTime + "</span> </td>");

                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Store Name : <span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketName + "</span></td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>Check-Out : <span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.CheckOutTime + "</span></td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;' align='left'>Address :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketAddress + "</span> </td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("</table>");
                    sbJobForm.Append("</div>");
                    sbJobForm.Append("<a name='" + JobFormNew.JobFormHeading.JobGUID + "' style='text-decoration: none;'>&nbsp;</a>");
                    sbJobForm.Append("<hr style='boder:1px solid black;width:100%'/>");
                }
                else
                {
                    sbJobForm.Append("<div>");
                }

                if (JobFormNew != null && JobFormNew.FormValues != null)
                {
                    foreach (var item1 in JobFormNew.FormValues)
                    {
                        if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -100) || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Email") || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Phone"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                //if (!parentIDList.Contains(item1.parentID.ToString()))
                                //{
                                //    parentIDList.Add(item1.parentID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) || ((one.ControlLabel == "Email" || one.ControlLabel == "Phone") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        // sbJobForm.Append("<p>&nbsp;</p>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");

                            }
                            //}
                        }
                        //else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -101) || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Date"))
                        //{
                        //    if (!parentIDList.Contains(item1.parentID.ToString()))
                        //    {
                        //        parentIDList.Add(item1.parentID.ToString());
                        //        sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                        //        sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                        //        sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                        //        sbJobForm.Append("<tr>");
                        //        sbJobForm.Append("<td align='left'>");
                        //        foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && ((one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -101) || (one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && one.ControlLabel == "Date")))))
                        //        {
                        //            if (!controlIDList.Contains(items.ControlID.ToString()))
                        //            {

                        //                controlIDList.Add(items.ControlID.ToString());
                        //                if (items.ControlLabel == "Year" || items.ControlLabel == "Day")
                        //                {
                        //                    sbJobForm.Append(" <span style='padding-left:0px;line-height:20px;font-size:10px;'>-" + items.Value + "</span>");
                        //                }
                        //                else
                        //                {
                        //                    sbJobForm.Append(" <span style='padding-left:10px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                        //                }


                        //            }
                        //        }
                        //        sbJobForm.Append("</td>");
                        //        sbJobForm.Append("</tr>");
                        //        sbJobForm.Append("</table>");
                        //        sbJobForm.Append("</div>");

                        //    }
                        //}

                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) && (item1.ControlLabel == "Region"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => ((one.ControlLabel == "Region" || one.ControlLabel == "Market") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + items.ControlLabel + "</label><br></br>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        // sbJobForm.Append("<p style='padding:0px 10px 0px 10px; border-bottom: 1px solid gray; border-left: 1px solid gray; border-right: 1px solid gray; line-height: 2px;'>&nbsp;</p>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) && (item1.ControlLabel == "Store Number # *" || item1.ControlLabel == "Store Address"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => ((one.ControlLabel == "Store Number # *" || one.ControlLabel == "Store Address") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + items.ControlLabel + "</label><br></br>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        //sbJobForm.Append("<p>&nbsp;</p>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }

                        }

                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID < 0))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                //sbJobForm.Append("<td align='left'>");
                                //sbJobForm.Append("</td>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        if (!controlIDList.Contains(items.ControlID.ToString()))
                                        {

                                            controlIDList.Add(items.ControlID.ToString());
                                            sbJobForm.Append("<td align='left' style='width:25%'>");
                                            sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                            sbJobForm.Append("</td>");

                                        }

                                    }
                                }

                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");

                            }
                        }
                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_DROP_DOWN) && item1.controlParentLabel != "Email" && item1.controlParentLabel != "Phone")
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                if (!string.IsNullOrEmpty(item1.ControlLabel) && !string.IsNullOrEmpty(item1.Value))
                                {
                                    sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                    sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>&nbsp;" + item1.ControlLabel + "</label>");
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    sbJobForm.Append("<tr>");
                                    sbJobForm.Append("<td align='left' style='width:40%'>");
                                    sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + item1.Value + "</span>");
                                    // sbJobForm.Append("<p style='padding:0px 10px 0px 10px; border-bottom: 1px solid gray; border-left: 1px solid gray; border-right: 1px solid gray; line-height: 2px;'>&nbsp;</p>");
                                    sbJobForm.Append("</td>");
                                    sbJobForm.Append("</tr>");
                                    sbJobForm.Append("</table>");
                                    sbJobForm.Append("</div>");
                                }
                            }
                        }

                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_CHECKBOX)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_CHECKBOX).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {

                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left' style='width:25%'>");
                                                if (items.Value == "true")
                                                {

                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/checkbox_yes.png") + "' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<input type='checkbox' name='checked' id='mychk' value='on'/>");
                                                }
                                                else
                                                {
                                                    //sbJobForm.Append("<img src='http://localhost:4100/assets/img/1403875956_ui-check-box.png' alt='logo'  width='20px' height='20px' />");
                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/checkbox_no.png") + "' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<input type='checkbox' name='checked' id='mychk' value='on'/>");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;vertical-align:top'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");

                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_MULTITEXT)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left'>");
                                sbJobForm.Append("<tr>");
                                sbJobForm.Append("<td align='left'>");
                                sbJobForm.Append("<label style='font-size:12px;'>" + item1.ControlLabel + "</label>");

                                sbJobForm.Append("<label>:</label>");

                                sbJobForm.Append("<textarea cols='20' rows='2' style='background-color:#FFFFFD;font-size:10px;'>" + item1.Value + "</textarea>");
                                sbJobForm.Append("</td>");
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }
                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_LABEL)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left'>");
                                sbJobForm.Append("<tr>");
                                sbJobForm.Append("<td align='left'>");
                                sbJobForm.Append("<label style='font-size:12px;'>" + item1.ControlLabel + "</label>");

                                sbJobForm.Append("<label>:</label>");

                                sbJobForm.Append("<label style='font-size:10px;'>&nbsp;&nbsp;" + item1.Value + "</label>&nbsp;");
                                sbJobForm.Append("</td>");
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");


                                sbJobForm.Append("</div>");
                            }
                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_GROUP)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div id='Div' class='leftbar-heading'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;border-bottom:10px;'>" + item1.ControlLabel + "</label>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='5'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_BUTTON)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        if (items.Value == "true")
                                        {
                                            sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876293_radiobutton_yes.png") + "' alt='logo'  width='20px' height='20px' />");
                                            //sbJobForm.Append("<input type='radio' disabled='disabled' checked />");
                                        }
                                        else
                                        {
                                            // sbJobForm.Append("<input type='radio' disabled='disabled' />");
                                            sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876299_radiobutton_no.png") + "' alt='logo'  width='20px' height='20px' />");
                                        }
                                        sbJobForm.Append("<span style='color:black;border-bottom:10px;;font-size:10px;vertical-align:top;'>" + items.ControlLabel + "</span>&nbsp;");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }
                        }


                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_SWITCH)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.ControlLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_BUTTON).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {

                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];

                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());

                                                sbJobForm.Append("<td align='left' style='width:25%'>");
                                                if (items.Value == "true")
                                                {
                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876293_radiobutton_yes.png") + "' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<img src='http://localhost:4100/assets/img/1403876293_radiobutton_yes.png' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<input type='radio' disabled='disabled' checked></input>");
                                                }
                                                else
                                                {
                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876299_radiobutton_no.png") + "' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<img src='http://localhost:4100/assets/img/1403876299_radiobutton_no.png' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<input type='radio' disabled='disabled'></input>");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;vertical-align:top;'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }

                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }

                                sbJobForm.Append("<div id='Div'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    if (!parentIDList.Contains(item1.ControlID.ToString()))
                                    {
                                        parentIDList.Add(item1.ControlID.ToString());
                                        sbJobForm.Append("<table style='width:100%' align='left'>");
                                        for (int i = 0; i < FormValues.Count; i = i + 3)
                                        {
                                            sbJobForm.Append("<tr>");
                                            for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                            {
                                                WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                                if (!controlIDList.Contains(items.ControlID.ToString()))
                                                {
                                                    controlIDList.Add(items.ControlID.ToString());


                                                    sbJobForm.Append("<td align='left'>");
                                                    if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                    {
                                                        sbJobForm.Append("<div>");
                                                        sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                        Logger.Debug(items.ImagePath + '/' + items.Value);
                                                        //sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' style='width:120px;height:120px;' />");
                                                        sbJobForm.Append("</div>");
                                                    }
                                                    sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.ControlLabel + "</span>&nbsp;");
                                                    sbJobForm.Append("</td>");

                                                }
                                            }
                                            sbJobForm.Append("</tr>");
                                        }
                                        sbJobForm.Append("</table>");
                                    }
                                }
                                sbJobForm.Append("</div>");

                                sbJobForm.Append("</div>");
                            }
                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {
                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left'>");
                                                if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                {
                                                    sbJobForm.Append("<div>");
                                                    sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                    Logger.Debug(items.ImagePath + '/' + items.Value);
                                                    //sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + " style='width:120px;height:120px;'/>");
                                                    sbJobForm.Append("</div>");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE_DESC)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE_DESC).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {
                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left'>");
                                                if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                {
                                                    sbJobForm.Append("<div>");

                                                    //string base64String = string.Empty;
                                                    //using (var webClient = new WebClient())
                                                    //{
                                                    //    byte[] imageAsByteArray = webClient.DownloadData(items.ImagePath + '/' + items.Value);
                                                    //    base64String = Convert.ToBase64String(imageAsByteArray, 0, imageAsByteArray.Length);
                                                    //}

                                                    //sbJobForm.Append("<img src='data:image/jpeg;base64," + base64String + "' alt='logo'  width='120' height='120' />");


                                                    sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                    Logger.Debug(items.ImagePath + '/' + items.Value);
                                                    sbJobForm.Append("</div>");
                                                }
                                                foreach (var item in JobFormNew.FormValues.Where(one => one.parentID == items.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT))
                                                {
                                                    if (!controlIDList.Contains(item.ControlID.ToString()))
                                                    {

                                                        controlIDList.Add(item.ControlID.ToString());
                                                        sbJobForm.Append("<span style='color:black;font-size:10px;'>" + item.ControlLabel + "</span>");
                                                    }
                                                }
                                                sbJobForm.Append("&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_ISSUES_TO_REPORT || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_OPEN_CHARGEBACKS || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_UNSOLD_QUOTES)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.ControlLabel + "</label>");


                                sbJobForm.Append("<div id='div_" + item1.ControlID + "'>");
                                if (JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID).ToList().Count > 0)
                                {
                                    foreach (var items in JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID))
                                    {
                                        if (!controlIDList.Contains(items.ControlID.ToString()))
                                        {
                                            controlIDList.Add(items.ControlID.ToString());
                                            sbJobForm.Append("<table style='width:100%' align='left'>");
                                            sbJobForm.Append("<tr>");
                                            sbJobForm.Append("<td align='left' style='width:100px'>");
                                            if (items.currentValueID > 0)
                                            {
                                                sbJobForm.Append("<span style='color:black;font-weight:bold;font-size:10px;'>" + items.Value + "</span>");
                                            }
                                            else
                                            {
                                                sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.Value + "</span>");
                                            }
                                            sbJobForm.Append("&nbsp;");
                                            sbJobForm.Append("</td>");
                                            sbJobForm.Append("</tr>");
                                            sbJobForm.Append("</table>");
                                        }
                                    }
                                }
                                else
                                {
                                    //sbJobForm.Append("<table style='width:100%' align='left'>");
                                    //sbJobForm.Append("<tr>");
                                    sbJobForm.Append("<div align='left' style='width:100px;font-size:10px;'>None</div>");
                                    //sbJobForm.Append("<tr>");
                                    //sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                                sbJobForm.Append("</div>");
                            }
                        }

                    }
                }
                sbJobForm.Append("</div>");
                #endregion
                string marker = string.Empty;
                if (JobFormNew != null && JobFormNew.CoordinateList != null && JobFormNew.CoordinateList.Count > 0)
                {
                    int i = 0;
                    string address = string.Empty;
                    foreach (CoOrdinates items in JobFormNew.CoordinateList)
                    {
                        i++;
                        if (i == JobFormNew.CoordinateList.Count)
                        {
                            if (!string.IsNullOrEmpty(items.EndTime))
                                marker = marker + "markers=size:small%7Ccolor:0xcc0000%7C" + items.Latitude + "," + items.Longitude;
                            else if (!string.IsNullOrEmpty(items.StartTime))
                                marker = marker + "markers=size:mid%7Ccolor:0x33d100%7C" + items.Latitude + "," + items.Longitude;
                            else if (!string.IsNullOrEmpty(items.StoreName))
                                marker = marker + "markers=color:0xff6600%7C" + items.Latitude + "," + items.Longitude;
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(items.EndTime))
                                marker = marker + "markers=size:small%7Ccolor:0xcc0000%7C" + items.Latitude + "," + items.Longitude + "&";
                            else if (!string.IsNullOrEmpty(items.StartTime))
                                marker = marker + "markers=size:mid%7Ccolor:0x33d100%7C" + items.Latitude + "," + items.Longitude + "&";
                            else if (!string.IsNullOrEmpty(items.StoreName))
                                marker = marker + "markers=color:0xff6600%7C" + items.Latitude + "," + items.Longitude + "&";


                        }
                        if (!string.IsNullOrEmpty(items.City))
                        {
                            address = address + "+" + items.City;
                        }
                        if (!string.IsNullOrEmpty(items.State))
                        {
                            address = address + "+" + items.State;
                        }
                        if (!string.IsNullOrEmpty(items.Country))
                        {
                            address = address + "+" + items.Country;
                        }
                        //markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284

                    }
                    sbJobForm.Append("<div style='page-break-before:always'>");
                    sbJobForm.Append("<img src='http://maps.googleapis.com/maps/api/staticmap?center=" + address + "&zoom=10&size=1000x1000&maptype=roadmap&" + marker + "'></img>");

                    // Logger.Debug("<img src='http://maps.googleapis.com/maps/api/staticmap?center=" + address + "&zoom=10&size=1000x1000&maptype=roadmap&" + marker + "'></img>");
                    sbJobForm.Append("</div>");
                }

                sbJobForm.Append("</body>");
                sbJobForm.Append("</html>");

                return sbJobForm.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return "";
            }
        }

        private string GetJobFormHTMLForStoreNonVisit(JobFormNew JobFormNew)
        {
            try
            {
                List<string> parentIDList = new List<string>();
                List<string> controlIDList = new List<string>();
                int imagecount = 0;
                List<WorkersInMotion.Model.ViewModel.JobFormValueDetails> FormValues = new List<WorkersInMotion.Model.ViewModel.JobFormValueDetails>();
                StringBuilder sbJobForm = new StringBuilder();
                // sbJobForm.Append("<div style='page-break-before:always'>&nbsp;</div>");
                sbJobForm.Append("<html>");
                sbJobForm.Append("<head><script type='text/javascript' src='http://maps.googleapis.com/maps/api/js?sensor=false'></script></head>");
                sbJobForm.Append("<body>");
                #region Generate JobForm
                if (JobFormNew.JobFormHeading != null)
                {
                    sbJobForm.Append("<div id='" + JobFormNew.JobFormHeading.JobGUID + "' name='" + JobFormNew.JobFormHeading.JobGUID + "'>");
                    sbJobForm.Append("<div align='center'>");
                    sbJobForm.Append("<table style='width:100%' align='center' cellpadding='0'>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td colspan='2' style='font-size:18px;font-family:verdana;font-weight:bold;text-align:center;'>" + JobFormNew.JobFormHeading.JobName + "&nbsp;Report</td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td colspan='2' style='font-size:18px;font-family:verdana;font-weight:bold;text-align:center;'>&nbsp;</td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Client Name :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.PlaceName + "</span> </td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>Client ID :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.PlaceID + "</span> </td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Store ID :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketID + "</span> </td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>Check-In :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.CheckInTime + "</span> </td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Store Name : <span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketName + "</span></td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>Check-Out : <span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.CheckOutTime + "</span></td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;' align='left'>Address :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketAddress + "</span> </td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("</table>");
                    sbJobForm.Append("</div>");
                    sbJobForm.Append("<a name='" + JobFormNew.JobFormHeading.JobGUID + "' style='text-decoration: none;'>&nbsp;</a>");
                    sbJobForm.Append("<hr style='boder:1px solid black;width:100%'/>");
                }
                else
                {
                    sbJobForm.Append("<div>");
                }

                if (JobFormNew != null && JobFormNew.FormValues != null)
                {
                    foreach (var item1 in JobFormNew.FormValues)
                    {
                        if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -100) || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Email") || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Phone"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                //if (!parentIDList.Contains(item1.parentID.ToString()))
                                //{
                                //    parentIDList.Add(item1.parentID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) || ((one.ControlLabel == "Email" || one.ControlLabel == "Phone") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        // sbJobForm.Append("<p>&nbsp;</p>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");

                            }
                        }
                        //}
                        //else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -101) || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Date"))
                        //{
                        //    if (!parentIDList.Contains(item1.parentID.ToString()))
                        //    {
                        //        parentIDList.Add(item1.parentID.ToString());
                        //        sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                        //        sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                        //        sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                        //        sbJobForm.Append("<tr>");
                        //        sbJobForm.Append("<td align='left'>");
                        //        foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && ((one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -101) || (one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && one.ControlLabel == "Date")))))
                        //        {
                        //            if (!controlIDList.Contains(items.ControlID.ToString()))
                        //            {

                        //                controlIDList.Add(items.ControlID.ToString());
                        //                if (items.ControlLabel == "Year" || items.ControlLabel == "Day")
                        //                {
                        //                    sbJobForm.Append(" <span style='padding-left:0px;line-height:20px;font-size:10px;'>-" + items.Value + "</span>");
                        //                }
                        //                else
                        //                {
                        //                    sbJobForm.Append(" <span style='padding-left:10px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                        //                }


                        //            }
                        //        }
                        //        sbJobForm.Append("</td>");
                        //        sbJobForm.Append("</tr>");
                        //        sbJobForm.Append("</table>");
                        //        sbJobForm.Append("</div>");

                        //    }
                        //}

                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) && (item1.ControlLabel == "Region"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => ((one.ControlLabel == "Region" || one.ControlLabel == "Market") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + items.ControlLabel + "</label><br></br>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        // sbJobForm.Append("<p style='padding:0px 10px 0px 10px; border-bottom: 1px solid gray; border-left: 1px solid gray; border-right: 1px solid gray; line-height: 2px;'>&nbsp;</p>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) && (item1.ControlLabel == "Store Number # *" || item1.ControlLabel == "Store Address"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => ((one.ControlLabel == "Store Number # *" || one.ControlLabel == "Store Address") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + items.ControlLabel + "</label><br></br>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        //sbJobForm.Append("<p>&nbsp;</p>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID < 0))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                //sbJobForm.Append("<td align='left'>");
                                //sbJobForm.Append("</td>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        if (!controlIDList.Contains(items.ControlID.ToString()))
                                        {

                                            controlIDList.Add(items.ControlID.ToString());
                                            sbJobForm.Append("<td align='left' style='width:25%'>");
                                            sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                            sbJobForm.Append("</td>");

                                        }

                                    }
                                }

                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");

                            }
                        }
                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_DROP_DOWN) && item1.controlParentLabel != "Email" && item1.controlParentLabel != "Phone")
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                if (!string.IsNullOrEmpty(item1.ControlLabel) && !string.IsNullOrEmpty(item1.Value))
                                {
                                    sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                    sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>&nbsp;" + item1.ControlLabel + "</label>");
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    sbJobForm.Append("<tr>");
                                    sbJobForm.Append("<td align='left' style='width:40%'>");
                                    sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + item1.Value + "</span>");
                                    // sbJobForm.Append("<p style='padding:0px 10px 0px 10px; border-bottom: 1px solid gray; border-left: 1px solid gray; border-right: 1px solid gray; line-height: 2px;'>&nbsp;</p>");
                                    sbJobForm.Append("</td>");
                                    sbJobForm.Append("</tr>");
                                    sbJobForm.Append("</table>");
                                    sbJobForm.Append("</div>");
                                }
                            }
                        }

                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_CHECKBOX)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_CHECKBOX).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {

                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left' style='width:25%'>");
                                                if (items.Value == "true")
                                                {

                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/checkbox_yes.png") + "' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<input type='checkbox' name='checked' id='mychk' value='on'/>");
                                                }
                                                else
                                                {
                                                    //sbJobForm.Append("<img src='http://localhost:4100/assets/img/1403875956_ui-check-box.png' alt='logo'  width='20px' height='20px' />");
                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/checkbox_no.png") + "' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<input type='checkbox' name='checked' id='mychk' value='on'/>");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;vertical-align:top'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");

                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_MULTITEXT)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left'>");
                                sbJobForm.Append("<tr>");
                                sbJobForm.Append("<td align='left'>");
                                sbJobForm.Append("<label style='font-size:12px;'>" + item1.ControlLabel + "</label>");

                                sbJobForm.Append("<label>:</label>");

                                sbJobForm.Append("<textarea cols='20' rows='2' style='background-color:#FFFFFD;font-size:10px;'>" + item1.Value + "</textarea>");
                                sbJobForm.Append("</td>");
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }
                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_LABEL)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left'>");
                                sbJobForm.Append("<tr>");
                                sbJobForm.Append("<td align='left'>");
                                sbJobForm.Append("<label style='font-size:12px;'>" + item1.ControlLabel + "</label>");

                                sbJobForm.Append("<label>:</label>");

                                sbJobForm.Append("<label style='font-size:10px;'>&nbsp;&nbsp;" + item1.Value + "</label>&nbsp;");
                                sbJobForm.Append("</td>");
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");


                                sbJobForm.Append("</div>");
                            }
                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_GROUP)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div id='Div' class='leftbar-heading'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;border-bottom:10px;'>" + item1.ControlLabel + "</label>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='5'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_BUTTON)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        if (items.Value == "true")
                                        {
                                            sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876293_radiobutton_yes.png") + "' alt='logo'  width='20px' height='20px' />");
                                            //sbJobForm.Append("<input type='radio' disabled='disabled' checked />");
                                        }
                                        else
                                        {
                                            // sbJobForm.Append("<input type='radio' disabled='disabled' />");
                                            sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876299_radiobutton_no.png") + "' alt='logo'  width='20px' height='20px' />");
                                        }
                                        sbJobForm.Append("<span style='color:black;border-bottom:10px;;font-size:10px;vertical-align:top;'>" + items.ControlLabel + "</span>&nbsp;");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }
                        }


                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_SWITCH)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.ControlLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_BUTTON).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {

                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];

                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());

                                                sbJobForm.Append("<td align='left' style='width:25%'>");
                                                if (items.Value == "true")
                                                {
                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876293_radiobutton_yes.png") + "' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<img src='http://localhost:4100/assets/img/1403876293_radiobutton_yes.png' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<input type='radio' disabled='disabled' checked></input>");
                                                }
                                                else
                                                {
                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876299_radiobutton_no.png") + "' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<img src='http://localhost:4100/assets/img/1403876299_radiobutton_no.png' alt='logo'  width='20px' height='20px' />");
                                                    //sbJobForm.Append("<input type='radio' disabled='disabled'></input>");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;vertical-align:top;'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }

                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }

                                sbJobForm.Append("<div id='Div'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    if (!parentIDList.Contains(item1.ControlID.ToString()))
                                    {
                                        parentIDList.Add(item1.ControlID.ToString());
                                        sbJobForm.Append("<table style='width:100%' align='left'>");
                                        for (int i = 0; i < FormValues.Count; i = i + 3)
                                        {
                                            sbJobForm.Append("<tr>");
                                            for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                            {
                                                WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                                if (!controlIDList.Contains(items.ControlID.ToString()))
                                                {
                                                    controlIDList.Add(items.ControlID.ToString());


                                                    sbJobForm.Append("<td align='left'>");
                                                    if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                    {
                                                        sbJobForm.Append("<div>");
                                                        sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                        Logger.Debug(items.ImagePath + '/' + items.Value);
                                                        //sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' style='width:120px;height:120px;' />");
                                                        sbJobForm.Append("</div>");
                                                    }
                                                    sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.ControlLabel + "</span>&nbsp;");
                                                    sbJobForm.Append("</td>");

                                                }
                                            }
                                            sbJobForm.Append("</tr>");
                                        }
                                        sbJobForm.Append("</table>");
                                    }
                                }
                                sbJobForm.Append("</div>");

                                sbJobForm.Append("</div>");
                            }
                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {
                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left'>");
                                                if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                {
                                                    sbJobForm.Append("<div>");
                                                    sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                    Logger.Debug(items.ImagePath + '/' + items.Value);
                                                    //sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + " style='width:120px;height:120px;'/>");
                                                    sbJobForm.Append("</div>");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE_DESC)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE_DESC).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {
                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left'>");
                                                if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                {
                                                    sbJobForm.Append("<div>");

                                                    //string base64String = string.Empty;
                                                    //using (var webClient = new WebClient())
                                                    //{
                                                    //    byte[] imageAsByteArray = webClient.DownloadData(items.ImagePath + '/' + items.Value);
                                                    //    base64String = Convert.ToBase64String(imageAsByteArray, 0, imageAsByteArray.Length);
                                                    //}

                                                    //sbJobForm.Append("<img src='data:image/jpeg;base64," + base64String + "' alt='logo'  width='120' height='120' />");


                                                    sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                    Logger.Debug(items.ImagePath + '/' + items.Value);
                                                    sbJobForm.Append("</div>");
                                                }
                                                foreach (var item in JobFormNew.FormValues.Where(one => one.parentID == items.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT))
                                                {
                                                    if (!controlIDList.Contains(item.ControlID.ToString()))
                                                    {

                                                        controlIDList.Add(item.ControlID.ToString());
                                                        sbJobForm.Append("<span style='color:black;font-size:10px;'>" + item.ControlLabel + "</span>");
                                                    }
                                                }
                                                sbJobForm.Append("&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_ISSUES_TO_REPORT || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_OPEN_CHARGEBACKS || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_UNSOLD_QUOTES)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.ControlLabel + "</label>");


                                sbJobForm.Append("<div id='div_" + item1.ControlID + "'>");
                                if (JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID).ToList().Count > 0)
                                {
                                    foreach (var items in JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID))
                                    {
                                        if (!controlIDList.Contains(items.ControlID.ToString()))
                                        {
                                            controlIDList.Add(items.ControlID.ToString());
                                            sbJobForm.Append("<table style='width:100%' align='left'>");
                                            sbJobForm.Append("<tr>");
                                            sbJobForm.Append("<td align='left' style='width:100px'>");
                                            if (items.currentValueID > 0)
                                            {
                                                sbJobForm.Append("<span style='color:black;font-weight:bold;font-size:10px;'>" + items.Value + "</span>");
                                            }
                                            else
                                            {
                                                sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.Value + "</span>");
                                            }
                                            sbJobForm.Append("&nbsp;");
                                            sbJobForm.Append("</td>");
                                            sbJobForm.Append("</tr>");
                                            sbJobForm.Append("</table>");
                                        }
                                    }
                                }
                                else
                                {
                                    //sbJobForm.Append("<table style='width:100%' align='left'>");
                                    //sbJobForm.Append("<tr>");
                                    sbJobForm.Append("<div align='left' style='width:100px;font-size:10px;'>None</div>");
                                    //sbJobForm.Append("<tr>");
                                    //sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                                sbJobForm.Append("</div>");
                            }
                        }

                    }
                }
                sbJobForm.Append("</div>");

                #endregion
                string marker = string.Empty;
                if (JobFormNew != null && JobFormNew.CoordinateList != null && JobFormNew.CoordinateList.Count > 0)
                {

                    int i = 0;
                    string address = string.Empty;
                    foreach (CoOrdinates items in JobFormNew.CoordinateList)
                    {
                        i++;
                        if (i == JobFormNew.CoordinateList.Count)
                        {
                            if (!string.IsNullOrEmpty(items.EndTime))
                                marker = marker + "markers=size:small%7Ccolor:0xcc0000%7C" + items.Latitude + "," + items.Longitude;
                            else if (!string.IsNullOrEmpty(items.StartTime))
                                marker = marker + "markers=size:mid%7Ccolor:0x33d100%7C" + items.Latitude + "," + items.Longitude;
                            else if (!string.IsNullOrEmpty(items.StoreName))
                                marker = marker + "markers=color:0xff6600%7C" + items.Latitude + "," + items.Longitude;
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(items.EndTime))
                                marker = marker + "markers=size:small%7Ccolor:0xcc0000%7C" + items.Latitude + "," + items.Longitude + "&";
                            else if (!string.IsNullOrEmpty(items.StartTime))
                                marker = marker + "markers=size:mid%7Ccolor:0x33d100%7C" + items.Latitude + "," + items.Longitude + "&";
                            else if (!string.IsNullOrEmpty(items.StoreName))
                                marker = marker + "markers=color:0xff6600%7C" + items.Latitude + "," + items.Longitude + "&";


                        }
                        if (!string.IsNullOrEmpty(items.City))
                        {
                            address = address + "+" + items.City;
                        }
                        if (!string.IsNullOrEmpty(items.State))
                        {
                            address = address + "+" + items.State;
                        }
                        if (!string.IsNullOrEmpty(items.Country))
                        {
                            address = address + "+" + items.Country;
                        }
                        //markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284

                    }
                    sbJobForm.Append("<div style='page-break-before:always'>");
                    sbJobForm.Append("<img src='http://maps.googleapis.com/maps/api/staticmap?center=" + address + "&zoom=10&size=1000x1000&maptype=roadmap&" + marker + "'></img>");
                    sbJobForm.Append("</div>");
                }

                sbJobForm.Append("</body>");
                sbJobForm.Append("</html>");
                return sbJobForm.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return "";
            }
        }

        public ActionResult Delete(string id = "")
        {
            Logger.Debug("Inside Store Visit Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    _IJobRepository.SetDeleteFlag(new Guid(id));
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

        private bool ParseExact(string dateString, string format, out DateTime dateTime)
        {
            bool result = DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
            Logger.Debug(result.ToString());
            return result;
        }
        //public JsonResult CoOrdinates(string JobGUID)
        //{
        //    Logger.Debug("Inside Store Controller- CoOrdinates");
        //    JsonResult result = new JsonResult();
        //    try
        //    {
        //        var TerritoryDetails = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(regionguid)).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
        //        {
        //            Value = r.TerritoryGUID.ToString(),
        //            Text = r.Name
        //        });

        //        result.Data = new SelectList(TerritoryDetails, "Value", "Text");
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex.Message);
        //        return result;
        //    }
        //}
    }
}