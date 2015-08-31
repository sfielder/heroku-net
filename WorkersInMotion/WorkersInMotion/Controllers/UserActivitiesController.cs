using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class UserActivitiesController : BaseController
    {
        #region Constructor
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IJobRepository _IJobRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IPORepository _IPORepository;
        public UserActivitiesController()
        {
            this._IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
            this._IPORepository = new PORepository(new WorkersInMotionDB());
        }

        public UserActivitiesController(WorkersInMotionDB context)
        {
            this._IOrganizationRepository = new OrganizationRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
            this._IPlaceRepository = new PlaceRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._IUserRepository = new UserRepository(context);
            this._IJobRepository = new JobRepository(context);
            this._IPORepository = new PORepository(context);
        }

        #endregion
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

        //
        // GET: /JobStatus/
        public ActionResult Index(string FromDate = "", string ToDate = "", string assigneduserguid = "", string jobindexguid = "", string Date = "", string selection = "", string ponumber = "", string RowCount = "", int page = 1, string search = "")
        {
            Logger.Debug("Inside AssignJob Controller- Index");
            try
            {
                ViewBag.AssignedUserID = assigneduserguid;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.Date = Date;
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

                    if (!string.IsNullOrEmpty(Date))
                    {
                        ViewBag.DateValue = HttpUtility.HtmlDecode(Date);
                    }
                    var jobStatus = new JobStatusViewModel();
                    jobStatus.JobStatusModel = new List<JobStatusModel>();
                    var jobGroup = new List<Job>();
                    //Job ljob = new Job();
                    DateTime pFrom = new DateTime(), pTo = new DateTime();
                    OrganizationUsersMap pOrganizationUsersMap = _IUserRepository.GetUserByID(new Guid(Session["UserGUID"].ToString()));
                    Logger.Debug("UserGUID:" + Session["UserGUID"].ToString());
                    if (pOrganizationUsersMap != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<div class='actions'>");
                        sb.Append("<div class='btn-group'>");
                        if (!string.IsNullOrEmpty(assigneduserguid))
                        {
                            ViewBag.AssignedUserID = assigneduserguid;
                            Logger.Debug("Inside AssignedUSerGUID" + assigneduserguid.ToString());
                            UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(new Guid(assigneduserguid), pOrganizationUsersMap.OrganizationGUID);
                            if (_userprofile != null)
                            {
                                sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> " + _userprofile.FirstName + " " + _userprofile.LastName + " <i class='icon-angle-down'></i></a>");
                            }
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(selection) && selection == "All")
                            {
                                sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> All <i class='icon-angle-down'></i></a>");
                            }
                            else
                            {
                                sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Select User <i class='icon-angle-down'></i></a>");
                            }
                        }
                        sb.Append("<ul id='ulworkgroup' style='height:200px;overflow-y:scroll' class='dropdown-menu pull-right'>");

                        if (Session["UserType"] != null && Session["UserType"].ToString() != "ENT_U" && pOrganizationUsersMap != null)
                        {
                            if (string.IsNullOrEmpty(selection) || selection != "All")
                            {
                                //sb.Append("<li><a href=" + Url.Action("Index", "UserActivities", new { selection = "All" }) + ">All</a></li>");
                                sb.Append("<li><a onclick=\"RedirectAction('');\">All</a></li>");
                            }
                            List<UserProfile> pUserProfile = _IUserProfileRepository.GetUserProfilesbyOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                            if (pUserProfile != null && pUserProfile.Count > 0)
                            {
                                pUserProfile = pUserProfile.OrderBy(x => x.FirstName).ToList();
                                foreach (UserProfile item in pUserProfile)
                                {
                                    //sb.Append("<li><a href=" + Url.Action("Index", "UserActivities", new { assigneduserguid = item.UserGUID.ToString() }) + " data-groupguid=" + item.UserGUID + ">" + item.FirstName + " " + item.LastName + "</a></li>");
                                    sb.Append("<li><a onclick=\"RedirectAction('" + item.UserGUID.ToString() + "');\" data-groupguid=" + item.UserGUID + ">" + item.FirstName + " " + item.LastName + "</a></li>");
                                    Logger.Debug("Inside User foreach");
                                }
                            }
                        }
                        sb.Append("</ul>");
                        sb.Append("</div>");
                        sb.Append("</div>");

                        ViewBag.UserList = sb.ToString();
                        Job mjob = new Job();
                        if (!string.IsNullOrEmpty(ponumber))
                        {
                            mjob.PONumber = ponumber;
                            TempData["PoNumber"] = ponumber;
                        }
                        //FOr Regional Manager
                        if (Session["UserType"] != null && !string.IsNullOrEmpty(Session["UserType"].ToString()) && Session["UserType"].ToString() == "ENT_U_RM" && Session["UserGUID"] != null)
                        {
                            OrganizationUsersMap OrgUserMap = _IOrganizationRepository.GetOrganizationUserMapByUserGUID(new Guid(Session["UserGUID"].ToString()), new Guid(Session["OrganizationGUID"].ToString()));
                            if (OrgUserMap != null)
                            {
                                mjob.RegionGUID = OrgUserMap.RegionGUID;
                            }
                        }
                        if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate) && !string.IsNullOrEmpty(assigneduserguid))
                        {
                            //ViewBag.FromDate = FromDate;
                            //ViewBag.ToDate = ToDate;
                            Logger.Debug("Inside 1");
                            if (Session["UserType"] != null && Session["UserType"].ToString() == "ENT_U")
                            {
                                mjob.AssignedUserGUID = new Guid(assigneduserguid);
                                if (ParseExact(FromDate, "dd-MM-yyyy", out pFrom))
                                {
                                    mjob.ActualStartTime = pFrom;
                                }
                                else
                                {
                                    Logger.Debug("From else");
                                    mjob.ActualStartTime = DateTime.Now.AddDays(-29);
                                }
                                if (ParseExact(ToDate, "dd-MM-yyyy", out pTo))
                                {
                                    mjob.ActualEndTime = pTo;
                                }
                                else
                                {
                                    Logger.Debug("To else");
                                    mjob.ActualEndTime = DateTime.Now;
                                }
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                            else
                            {
                                Logger.Debug("Inside 1 else");
                                mjob.AssignedUserGUID = new Guid(assigneduserguid);
                                if (ParseExact(FromDate, "dd-MM-yyyy", out pFrom))
                                {
                                    mjob.ActualStartTime = pFrom;
                                }
                                else
                                {
                                    Logger.Debug("From else 1");
                                    mjob.ActualStartTime = DateTime.Now.AddDays(-29);
                                }
                                if (ParseExact(ToDate, "dd-MM-yyyy", out pTo))
                                {
                                    mjob.ActualEndTime = pTo;
                                }
                                else
                                {
                                    Logger.Debug("To Else 1");
                                    mjob.ActualEndTime = DateTime.Now;
                                }
                                //    mjob.PreferedEndTime = _IUserRepository.GetLocalDateTime(mjob.PreferedEndTime, Session["TimeZoneID"].ToString());
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                        }
                        else if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                        {
                            Logger.Debug("Inside 2");
                            if (Session["UserType"] != null && Session["UserType"].ToString() == "ENT_U")
                            {
                                if (ParseExact(FromDate, "dd-MM-yyyy", out pFrom))
                                {
                                    mjob.ActualStartTime = pFrom;
                                }
                                else
                                {
                                    Logger.Debug("From else");
                                    mjob.ActualStartTime = DateTime.Now.AddDays(-29);
                                }
                                if (ParseExact(ToDate, "dd-MM-yyyy", out pTo))
                                {
                                    mjob.ActualEndTime = pTo;
                                }
                                else
                                {
                                    Logger.Debug("To else");
                                    mjob.ActualEndTime = DateTime.Now;
                                }
                                mjob.AssignedUserGUID = new Guid(Session["UserGUID"].ToString());
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                            else
                            {
                                Logger.Debug("Inside 3");
                                if (ParseExact(FromDate, "dd-MM-yyyy", out pFrom))
                                {
                                    mjob.ActualStartTime = pFrom;
                                }
                                else
                                {
                                    Logger.Debug("From Else");
                                    mjob.ActualStartTime = DateTime.Now.AddDays(-29);
                                }
                                if (ParseExact(ToDate, "dd-MM-yyyy", out pTo))
                                {
                                    mjob.ActualEndTime = pTo;
                                }
                                else
                                {
                                    Logger.Debug("To Else");
                                    mjob.ActualEndTime = DateTime.Now;
                                }
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                        }
                        else if (!string.IsNullOrEmpty(assigneduserguid))
                        {
                            mjob.ActualStartTime = DateTime.Now.AddDays(-29);
                            mjob.ActualEndTime = DateTime.Now;
                            mjob.AssignedUserGUID = new Guid(assigneduserguid);
                            mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                            jobGroup = _IJobRepository.GetJobs(mjob);
                        }
                        else
                        {
                            mjob.ActualStartTime = DateTime.Now.AddDays(-29);
                            mjob.ActualEndTime = DateTime.Now;
                            if (Session["UserType"] != null && Session["UserType"].ToString() == "ENT_U")
                            {
                                mjob.AssignedUserGUID = new Guid(Session["UserGUID"].ToString());
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                            else
                            {
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                        }

                        if (jobGroup != null && jobGroup.Count > 0)
                        {
                            ViewBag.Search = search;
                            if (!string.IsNullOrEmpty(search))
                            {
                                search = search.ToLower();
                                jobGroup = jobGroup.Where(x => (x.CustomerGUID != null ? GetCompanyName(x.CustomerGUID).ToLower() : GetCompanyNameByPO(x.PONumber).ToLower()).Contains(search)
                                    || (!String.IsNullOrEmpty(x.JobName) && x.JobName.ToLower().Contains(search))
                                    || (!String.IsNullOrEmpty(x.PONumber) && x.PONumber.ToLower().Contains(search))
                                    || (!string.IsNullOrEmpty(x.CustomerGUID.ToString()) ? getStoreID(x.CustomerStopGUID.ToString()) : "").Contains(search)
                                    || (_IJobRepository.GetStatusName(Convert.ToInt32(x.StatusCode)).ToLower().Contains(search))
                                    ).ToList();
                            }

                            totalRecord = jobGroup.ToList().Count;
                            totalPage = (totalRecord / (int)ViewBag.pageCountValue) + ((totalRecord % (int)ViewBag.pageCountValue) > 0 ? 1 : 0);
                            
                            ViewBag.TotalRows = totalRecord;

                            jobGroup = jobGroup.OrderBy(a => a.OrganizationGUID).Skip(((page - 1) * (int)ViewBag.pageCountValue)).Take((int)ViewBag.pageCountValue).ToList();

                            foreach (var job in jobGroup.ToList())
                            {
                                JobStatusModel js = new JobStatusModel();
                                js.JobName = job.JobName;
                                js.JobIndexGUID = job.JobGUID.ToString();
                                //  js.JobLogicalID = job.JobFormGUID.ToString();
                                js.UserGUID = job.AssignedUserGUID.ToString();

                                js.PreferredStartTime = job.PreferedStartTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(job.PreferedStartTime, Session["TimeZoneID"].ToString()) : job.PreferedStartTime.ToString() : "";
                                js.PreferredStartTime = !string.IsNullOrEmpty(js.PreferredStartTime) ? _IUserRepository.GetLocalDateTime(job.PreferedStartTime, Session["TimeZoneID"].ToString()) : "";
                                js.PreferredStartTime = !string.IsNullOrEmpty(js.PreferredStartTime) ? Convert.ToDateTime(js.PreferredStartTime).ToString("MM/dd/yy HH:mm") : "";

                                js.PreferredEndTime = job.PreferedEndTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(job.PreferedEndTime, Session["TimeZoneID"].ToString()) : job.PreferedEndTime.ToString() : "";
                                js.PreferredEndTime = !string.IsNullOrEmpty(js.PreferredEndTime) ? _IUserRepository.GetLocalDateTime(job.PreferedEndTime, Session["TimeZoneID"].ToString()) : "";
                                js.PreferredEndTime = !string.IsNullOrEmpty(js.PreferredEndTime) ? Convert.ToDateTime(js.PreferredEndTime).ToString("MM/dd/yy HH:mm") : "";

                                js.ActualStartTime = job.ActualStartTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(job.ActualStartTime, Session["TimeZoneID"].ToString()) : job.ActualStartTime.ToString() : "";
                                js.ActualStartTime = !string.IsNullOrEmpty(js.ActualStartTime) ? _IUserRepository.GetLocalDateTime(job.ActualStartTime, Session["TimeZoneID"].ToString()) : "";
                                js.ActualStartTime = !string.IsNullOrEmpty(js.ActualStartTime) ? Convert.ToDateTime(js.ActualStartTime).ToString("MM/dd/yy HH:mm") : "";

                                js.ActualEndTime = job.PreferedEndTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(job.ActualEndTime, Session["TimeZoneID"].ToString()) : job.ActualEndTime.ToString() : "";
                                js.ActualEndTime = !string.IsNullOrEmpty(js.ActualEndTime) ? _IUserRepository.GetLocalDateTime(job.ActualEndTime, Session["TimeZoneID"].ToString()) : "";
                                js.ActualEndTime = !string.IsNullOrEmpty(js.ActualEndTime) ? Convert.ToDateTime(js.ActualEndTime).ToString("MM/dd/yy HH:mm") : "";


                                js.LastModifiedDate = job.PreferedEndTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(job.LastModifiedDate, Session["TimeZoneID"].ToString()) : job.LastModifiedDate.ToString() : "";
                                js.LastModifiedDate = !string.IsNullOrEmpty(js.LastModifiedDate) ? _IUserRepository.GetLocalDateTime(job.LastModifiedDate, Session["TimeZoneID"].ToString()) : "";
                                js.LastModifiedDate = !string.IsNullOrEmpty(js.LastModifiedDate) ? Convert.ToDateTime(js.LastModifiedDate).ToString("MM/dd/yy HH:mm") : "";



                                //  ActualStartTime = job.ActualStartTime.ToString() != "" ? Convert.ToDateTime(job.ActualStartTime).ToString("yyyy/MM/dd HH:mm") : "",//, Session["TimeZoneID"].ToString()
                                js.EstimatedStartTime = job.ActualStartTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(job.ActualStartTime, Session["TimeZoneID"].ToString()) : job.ActualStartTime.ToString() : "";

                                js.EstimatedStartTime = !string.IsNullOrEmpty(js.EstimatedStartTime) ? _IUserRepository.GetLocalDateTime(job.ActualStartTime, Session["TimeZoneID"].ToString()) : "";
                                js.EstimatedStartTime = !string.IsNullOrEmpty(js.EstimatedStartTime) ? Convert.ToDateTime(js.EstimatedStartTime).ToString("MM/dd/yy HH:mm") : "";
                                //js.AssignedTo = _IGlobalUserRepository.GetGlobalUserByID(new Guid(job.AssignedUserGUID.ToString())) != null ? _IGlobalUserRepository.GetGlobalUserByID(new Guid(job.AssignedUserGUID.ToString())).UserName : "";
                                js.EstimatedDuration = Convert.ToDouble(job.EstimatedDuration);
                                js.Status = _IJobRepository.GetStatusName(Convert.ToInt32(job.StatusCode));
                                // js.CustomerName = !string.IsNullOrEmpty(job.CustomerGUID.ToString()) ? GetCompanyName(new Guid(job.CustomerGUID.ToString())) : "";
                                js.CustomerName = job.CustomerGUID != null ? GetCompanyName(job.CustomerGUID) : GetCompanyNameByPO(job.PONumber);
                                js.StoreID = !string.IsNullOrEmpty(job.CustomerGUID.ToString()) ? getStoreID(job.CustomerStopGUID.ToString()) : "";
                                js.PONumber = job.PONumber;
                                js.statuscode = job.StatusCode != null ? Convert.ToInt32(job.StatusCode) : 0;
                                js.RegionGUID = job.RegionGUID != null ? job.RegionGUID.ToString() : "";
                                js.TerritoryGUID = job.TerritoryGUID != null ? job.TerritoryGUID.ToString() : "";
                                js.SiteAddress = job.ServiceAddress;
                                JobProgress pJobProgress = _IJobRepository.GetJobProgressMismatch(job.JobGUID, js.statuscode);
                                bool mismatch;
                                if (pJobProgress != null && bool.TryParse(pJobProgress.LocationMismatch.ToString(), out mismatch))
                                {
                                    js.locationmismatch = mismatch;
                                }
                                if (job.StatusCode >= 2 && job.AssignedUserGUID != null)
                                {

                                    js.Email = GetEmails(new Guid(job.AssignedUserGUID.ToString()), pOrganizationUsersMap.OrganizationGUID);
                                    js.AssociateContactNumber = GetContactNumber(new Guid(job.AssignedUserGUID.ToString()), pOrganizationUsersMap.OrganizationGUID);
                                    GlobalUser _globaluser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(job.AssignedUserGUID.ToString()));
                                    if (_globaluser != null)
                                    {
                                        js.AssociateName = _globaluser.UserName;
                                        js.AssignedTo = _globaluser.UserName;
                                    }
                                    else
                                    {
                                        js.AssignedTo = "";
                                        js.AssociateName = "";
                                    }
                                }
                                else
                                {
                                    js.Email = "";
                                    js.AssignedTo = "";
                                    js.AssociateName = "";
                                    js.AssociateContactNumber = "";
                                }

                                if (job.CustomerStopGUID != null && job.CustomerStopGUID != Guid.Empty)
                                {
                                    Market _Market = _IMarketRepository.GetMarketByID(new Guid(job.CustomerStopGUID.ToString()));
                                    if (_Market != null)
                                    {
                                        if (!string.IsNullOrEmpty(_Market.RMUserID))
                                        {
                                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.RMUserID, Session["OrganizationGUID"].ToString());
                                            if (_globalUser != null)
                                            {
                                                UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                                if (_userprofile != null)
                                                {
                                                    js.RegionalManager = _userprofile.FirstName + " " + _userprofile.LastName;
                                                }
                                                else
                                                {
                                                    js.RegionalManager = "";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            js.RegionalManager = "";
                                        }
                                    }
                                    else
                                    {
                                        js.RegionalManager = "";
                                        // js.FieldManager = "";
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
                                            if (!string.IsNullOrEmpty(_Market.RMUserID))
                                            {
                                                GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.RMUserID, Session["OrganizationGUID"].ToString());
                                                if (_globalUser != null)
                                                {
                                                    UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, pOrganizationUsersMap.OrganizationGUID);
                                                    if (_userprofile != null)
                                                    {
                                                        js.RegionalManager = _userprofile.FirstName + " " + _userprofile.LastName;
                                                    }
                                                    else
                                                    {
                                                        js.RegionalManager = "";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                js.RegionalManager = "";
                                            }
                                        }
                                        else
                                        {
                                            js.RegionalManager = "";
                                            //js.FieldManager = "";
                                        }
                                    }
                                    else
                                    {
                                        js.RegionalManager = "";
                                        //js.FieldManager = "";
                                    }
                                }
                                else
                                {
                                    js.RegionalManager = "";
                                    // js.FieldManager = "";
                                }

                                if (job.ManagerUserGUID != null && job.ManagerUserGUID != Guid.Empty)
                                {
                                    GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(job.ManagerUserGUID.ToString()));
                                    if (_globalUser != null)
                                    {
                                        UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, pOrganizationUsersMap.OrganizationGUID);
                                        if (_userprofile != null)
                                        {
                                            js.FieldManager = _userprofile.FirstName + " " + _userprofile.LastName;
                                        }
                                        else
                                        {
                                            js.FieldManager = "";
                                        }
                                    }
                                }
                                else
                                {
                                    js.FieldManager = "";
                                }

                                jobStatus.JobStatusModel.Add(js);
                            }
                        }
                        else
                        {
                            ViewBag.TotalRows = 0;
                        }
                        if (!string.IsNullOrEmpty(assigneduserguid) && !string.IsNullOrEmpty(jobindexguid))
                        {
                            GlobalUser _GlobalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(assigneduserguid));
                            jobStatus.GlobalUsers = new List<GlobalUserModel>();
                            if (_GlobalUser != null)
                            {
                                jobStatus.GlobalUsers.Add(new GlobalUserModel
                                {
                                    UserGUID = _GlobalUser.UserGUID,
                                    UserName = _GlobalUser.UserName
                                });
                            }
                            if (!string.IsNullOrEmpty(jobindexguid))
                            {
                                jobStatus.JobModel = new JobModel();
                                jobStatus.JobModel.JobName = _IJobRepository.GetJobByID(new Guid(jobindexguid)).JobName;
                                jobStatus.JobModel.JobIndexGUID = new Guid(jobindexguid);
                            }
                        }
                    }

                    if (jobStatus.JobStatusModel != null && jobStatus.JobStatusModel.Count > 0)
                    {
                        if (Session["JobStatusModel"] != null)
                        {
                            Session["JobStatusModel"] = null;
                            Session["JobStatusModel"] = jobStatus.JobStatusModel.ToList();
                        }
                        else
                        {
                            Session["JobStatusModel"] = jobStatus.JobStatusModel.ToList();
                        }
                    }
                    else
                    {
                        Session["JobStatusModel"] = null;
                    }
                    if (!string.IsNullOrEmpty(RowCount))
                        ViewBag.pageCountValue = int.Parse(RowCount);
                    else
                        ViewBag.pageCountValue = 5;
                    return View(jobStatus);
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

        public ActionResult Delete(string id = "")
        {
            Logger.Debug("Inside Job Status Controller- Delete");
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

        private string GetCustomerName(Guid placeid)
        {
            string customername = "";
            if (placeid != null && placeid != Guid.Empty)
            {
                Place lplace = _IPlaceRepository.GetPlaceByID(placeid);
                if (lplace != null)
                    customername = lplace.FirstName;
                else
                    customername = "";
            }
            return customername;
        }
        private string GetCompanyName(Nullable<System.Guid> placeid)
        {
            string companyname = "";
            if (placeid != null && placeid != Guid.Empty)
            {
                Place lplace = _IPlaceRepository.GetPlaceByID(new Guid(placeid.ToString()));
                if (lplace != null)
                    companyname = lplace.PlaceName;
                else
                    companyname = "";
            }
            return companyname;
        }

        private string GetCompanyName(string placeid, Guid OrganizationGUID)
        {
            string companyname = "";
            if (placeid != null && !string.IsNullOrEmpty(placeid) && OrganizationGUID != Guid.Empty)
            {
                Place lplace = _IPlaceRepository.GetPlaceByID(placeid, OrganizationGUID);
                if (lplace != null)
                    companyname = lplace.PlaceName;
                else
                    companyname = "";
            }
            return companyname;
        }


        private string GetCompanyNameByPO(string PONumber)
        {
            string companyname = "";
            if (!string.IsNullOrEmpty(PONumber))
            {
                POs lponumber = _IPORepository.GetPObyPoNumber(PONumber);
                if (lponumber != null)
                {
                    companyname = GetCompanyName(lponumber.PlaceID, lponumber.OrganizationGUID != null ? new Guid(lponumber.OrganizationGUID.ToString()) : Guid.Empty);
                }
                else
                    companyname = "";
            }
            return companyname;
        }
        private string getStoreID(string CustomerStopGUID)
        {
            string StoreID = "";
            if (!string.IsNullOrEmpty(CustomerStopGUID) && new Guid(CustomerStopGUID) != Guid.Empty)
            {

                Market _market = _IMarketRepository.GetMarketByID(new Guid(CustomerStopGUID));
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
        private string GetEmails(Guid userguid, Guid OrganizationGUID)
        {
            string email = "";
            if (userguid != null && userguid != Guid.Empty && OrganizationGUID != null && OrganizationGUID != Guid.Empty)
            {
                UserProfile luser = _IUserProfileRepository.GetUserProfileByUserID(userguid, OrganizationGUID);
                if (luser != null)
                    email = luser.EmailID;
                else
                    email = "";
            }
            return email;
        }

        private string GetContactNumber(Guid userguid, Guid OrganizationGUID)
        {
            string cnumber = "";
            if (userguid != null && userguid != Guid.Empty && OrganizationGUID != null && OrganizationGUID != Guid.Empty)
            {
                UserProfile luser = _IUserProfileRepository.GetUserProfileByUserID(userguid, OrganizationGUID);
                if (luser != null)
                    cnumber = luser.BusinessPhone;
                else
                    cnumber = "";
            }
            return cnumber;
        }


        public ActionResult JobForm(string id = "")
        {
            Logger.Debug("Inside Job Controller- View Job");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    Job _job = new Job();
                    JobFormNew pJobFormView = new JobFormNew();
                    _job = _IJobRepository.GetJobByID(new Guid(id));

                    // pJobFormView.JobGUID = id;
                    if (_job != null)
                    {
                        ViewBag.JobName = _job.JobName;


                        //JobFormNew pJobFormView = (_job != null && !string.IsNullOrEmpty(_job.JobForm)) ? new JavaScriptSerializer().Deserialize<JobFormNew>(_job.JobForm) : null;


                        //if (pJobFormView != null)
                        //{
                        //    if (pJobFormView.Values != null)
                        //    {
                        //        pJobFormView.FormValues = new List<JobFormValueDetails>();
                        //        //for (int i = 0; i < pJobFormView.Values.Count; i++)
                        //        foreach (JobFormValues pFormValues in pJobFormView.Values)
                        //        {
                        //            //JobFormValues pFormValues = pJobFormView.Values[i];
                        //            JobFormValueDetails pFormDetails = new JobFormValueDetails();
                        //            string[] Controls = pFormValues.ControlID.Split('_');
                        //            if (Controls.Length > 2)
                        //            {
                        //                int controlid, controltype;
                        //                pFormDetails.FormID = Controls[0];
                        //                if (int.TryParse(Controls[1], out controlid))
                        //                {
                        //                    pFormDetails.ControlID = controlid;
                        //                }
                        //                else
                        //                {
                        //                    pFormDetails.ControlID = 0;
                        //                }
                        //                if (int.TryParse(Controls[2], out controltype))
                        //                {
                        //                    pFormDetails.ControlType = (ControlType)controltype;
                        //                }
                        //                else
                        //                {
                        //                    pFormDetails.ControlType = 0;
                        //                }

                        //            }
                        //            int parentid;
                        //            pFormDetails.Value = pFormValues.Value;
                        //            pFormDetails.ControlLabel = pFormValues.ControlLabel;
                        //            if (int.TryParse(pFormValues.parentID, out parentid))
                        //            {
                        //                pFormDetails.parentID = parentid;
                        //            }
                        //            else
                        //            {
                        //                pFormDetails.parentID = 0;
                        //            }
                        //            pFormDetails.controlParentLabel = pFormValues.controlParentLabel;
                        //            pFormDetails.ValueID = pFormValues.ValueID;
                        //            pFormDetails.currentValueID = pFormValues.currentValueID;

                        //            pFormDetails.ImagePath = System.Configuration.ConfigurationManager.AppSettings.Get("ImageURL").ToString() + Session["OrganizationGUID"].ToString() + "/Jobs/" + pJobFormView.JobGUID;

                        //            pJobFormView.FormValues.Add(pFormDetails);
                        //        }
                        //    }
                        //}


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
                                pJobFormView.JobFormHeading = null;
                            }
                            pJobFormView.FormValues.OrderBy(x => x.ControlID);
                        }

                        return View(pJobFormView);
                    }
                    else
                    {
                        pJobFormView = null;
                        return View(pJobFormView);
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

                        Market pMarket = _IMarketRepository.GetMarketByID(new Guid(pjob.CustomerStopGUID.ToString()));
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
            Logger.Debug("Inside Site Visit Controller- GeneratePDF");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        StringBuilder pVisit = new StringBuilder();
                        Job _job = _IJobRepository.GetJobByID(new Guid(id));
                        if (_job != null)
                        {
                            Response.ContentType = "application/pdf";

                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Session["VisitType"] = "Visit Report";
                            string datevalue = Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(DateTime.UtcNow, Session["TimeZoneID"].ToString()) : DateTime.UtcNow.ToString();
                            Response.AddHeader("content-disposition", "attachment;filename=" + Session["OrganizationName"].ToString() + "_UserActivities_" + datevalue + ".pdf");
                            // Response.AddHeader("content-disposition", "attachment;filename=Visits.pdf");

                            JobFormNew pJobFormView = new JobFormNew();

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
                                    pVisit.Append(GetJobFormHTML(pJobFormView));
                                }

                            }
                        }
                        else
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','No data to generate PDF');</script>";
                            return RedirectToAction("Index", "UserActivities");
                        }

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
                        return RedirectToAction("Index", "UserActivities");
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


        private string GetJobFormHTML(JobFormNew JobFormNew)
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
                #region Generate Job Form
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
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>PO Number :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.PoNumber + "</span> </td>");
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
                }

                sbJobForm.Append("<hr style='boder:1px solid black;width:100%'/>");
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
                        //                    sbJobForm.Append(" <span style='padding-left:0px;line-height:0px;font-size:10px;'>-" + items.Value + "</span>");
                        //                }
                        //                else
                        //                {
                        //                    sbJobForm.Append(" <span style='padding-left:10px;line-height:0px;font-size:10px;'>" + items.Value + "</span>");
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
                                                }
                                                else
                                                {
                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/checkbox_no.png") + "' alt='logo'  width='20px' height='20px' />");
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
                                sbJobForm.Append("<label style='color:black;font-weight:bold;border-bottom:10px;;font-size:12px;'>" + item1.ControlLabel + "</label>");
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
                                                }
                                                else
                                                {
                                                    sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/1403876299_radiobutton_no.png") + "' alt='logo'  width='20px' height='20px' />");
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
                            Logger.Debug("Start with control type as Image");
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
                                                    //string base64String = string.Empty;
                                                    //using (var webClient = new WebClient())
                                                    //{
                                                    //    byte[] imageAsByteArray = getResizedImage(items.OrganizationGUID, items.JobGUID, items.Value, 120, 120);
                                                    //    base64String = Convert.ToBase64String(imageAsByteArray, 0, imageAsByteArray.Length);
                                                    //    Logger.Debug(base64String);
                                                    //}
                                                    //sbJobForm.Append("<img src='" + string.Format("data:" + getContentType(items.ImagePath + '/' + items.Value) + "/jpeg;base64,{0}", base64String) + "' alt='logo'  width='120px' height='120px' />");

                                                    //sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/sidebar-toggler-purple.jpg") + "' alt='logo'  width='20px' height='20px' />");
                                                    sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                    Logger.Debug(items.ImagePath + '/' + items.Value);
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

                                Logger.Debug("End with control type as Image");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE_DESC)
                        {
                            Logger.Debug("Start with control type as Image Description");
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
                                                    //    byte[] imageAsByteArray = getResizedImage(items.OrganizationGUID, items.JobGUID, items.Value, 120, 120);
                                                    //    base64String = Convert.ToBase64String(imageAsByteArray, 0, imageAsByteArray.Length);
                                                    //}
                                                    //sbJobForm.Append("<img src='" + string.Format("data:" + getContentType(items.ImagePath + '/' + items.Value) + "/jpeg;base64,{0}", base64String) + "' alt='logo'  width='120px' height='120px' />");

                                                    // sbJobForm.Append("<img src='" + Server.MapPath("~/" + items.OrganizationGUID + "/Jobs/" + items.JobGUID + "/" + items.Value + "") + "' alt='logo'  width='120px' height='120px' />");
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

                                Logger.Debug("End with control type as Image Description");
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
                                    sbJobForm.Append("<div align='left' style='width:100px;font-size:10px;'>None</div>");
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
                    sbJobForm.Append("<img src='http://maps.googleapis.com/maps/api/staticmap?center=" + address + "&zoom=13&size=1000x1000&maptype=roadmap&" + marker + "'></img>");
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


        public ActionResult GenerateExcel()
        {
            Logger.Debug("Inside User Activity Controller- Generate Excel");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (Session["JobStatusModel"] != null)
                    {
                        StringBuilder pUser = new StringBuilder();
                        List<JobStatusModel> JobStatusModel = new List<JobStatusModel>();
                        JobStatusModel = (List<JobStatusModel>)(Session["JobStatusModel"]);
                        if (JobStatusModel != null && JobStatusModel.Count > 0)
                        {
                            Response.Clear();
                            Response.ClearContent();
                            Response.ClearHeaders();
                            Response.ContentType = "application/vnd.ms-excel";
                            Response.Charset = "";
                            Response.Buffer = true;
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Session["VisitType"] = "Visit Report";
                            string datevalue = Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(DateTime.UtcNow, Session["TimeZoneID"].ToString()) : DateTime.UtcNow.ToString();
                            Response.AddHeader("content-disposition", "attachment;filename=" + Session["OrganizationName"].ToString() + "_UserActivities_" + datevalue + ".xls");

                            // Response.AddHeader("content-disposition", "attachment;filename=Visits.pdf");

                            pUser.Append("<html><body><div><table cellpadding='0' cellspacing='0'>");
                            pUser.Append("<tr>");
                            pUser.Append("<td colspan='8' align='center' style='color:#254117;font:bold 13px arial;background-color:#FFD2D2;border:1px solid #B0B0B0 ;' >User Activities</td>");
                            pUser.Append("<tr>");
                            pUser.Append("<td style='color:#ffffff;font:bold 13px arial;background-color:#004566;border:1px solid #B0B0B0;vertical-align:middle;'>Visit Type</td>");
                            pUser.Append("<td style='color:#ffffff;font:bold 13px arial;background-color:#004566;border:1px solid #B0B0B0;vertical-align:middle;'>Client Name</td>");
                            pUser.Append("<td style='color:#ffffff;font:bold 13px arial;background-color:#004566;border:1px solid #B0B0B0;vertical-align:middle;'>Store ID</td>");
                            pUser.Append("<td style='color:#ffffff;font:bold 13px arial;background-color:#004566;border:1px solid #B0B0B0;vertical-align:middle;'>PO Number</td>");
                            pUser.Append("<td style='color:#ffffff;font:bold 13px arial;background-color:#004566;border:1px solid #B0B0B0;vertical-align:middle;'>Status</td>");
                            pUser.Append("<td style='color:#ffffff;font:bold 13px arial;background-color:#004566;border:1px solid #B0B0B0;vertical-align:middle;'>Check-In</td>");
                            pUser.Append("<td style='color:#ffffff;font:bold 13px arial;background-color:#004566;border:1px solid #B0B0B0;vertical-align:middle;'>Check-Out</td>");
                            pUser.Append("<td style='color:#ffffff;font:bold 13px arial;background-color:#004566;border:1px solid #B0B0B0;vertical-align:middle;'>Field Manager</td>");
                            pUser.Append("</tr>");

                            foreach (JobStatusModel pJobStatusModel in JobStatusModel)
                            {
                                pUser.Append("<tr>");
                                if (!String.IsNullOrEmpty(pJobStatusModel.JobName))
                                    pUser.Append("<td align='left' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>" + pJobStatusModel.JobName + "</td>");
                                else
                                    pUser.Append("<td align='right' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>&nbsp;</td>");
                                if (!String.IsNullOrEmpty(pJobStatusModel.CustomerName))
                                    pUser.Append("<td align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>" + pJobStatusModel.CustomerName + "</td>");
                                else
                                    pUser.Append("<td align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>&nbsp;</td>");

                                if (!String.IsNullOrEmpty(pJobStatusModel.StoreID))
                                    pUser.Append("<td  align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>" + pJobStatusModel.StoreID + "</td>");
                                else
                                    pUser.Append("<td align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>&nbsp;</td>");
                                if (!String.IsNullOrEmpty(pJobStatusModel.PONumber))
                                    pUser.Append("<td  align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>" + pJobStatusModel.PONumber + "</td>");
                                else
                                    pUser.Append("<td align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>&nbsp;</td>");
                                if (!String.IsNullOrEmpty(pJobStatusModel.Status))
                                    pUser.Append("<td  align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>" + pJobStatusModel.Status + "</td>");
                                else
                                    pUser.Append("<td align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>&nbsp;</td>");
                                if (!String.IsNullOrEmpty(pJobStatusModel.ActualStartTime))
                                    pUser.Append("<td  align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>" + pJobStatusModel.ActualStartTime + "</td>");
                                else
                                    pUser.Append("<td align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>&nbsp;</td>");
                                if (!String.IsNullOrEmpty(pJobStatusModel.ActualEndTime))
                                    pUser.Append("<td  align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>" + pJobStatusModel.ActualEndTime + "</td>");
                                else
                                    pUser.Append("<td align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>&nbsp;</td>");
                                if (!String.IsNullOrEmpty(pJobStatusModel.FieldManager))
                                    pUser.Append("<td  align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>" + pJobStatusModel.FieldManager + "</td>");
                                else
                                    pUser.Append("<td align='center' style='color:#151B8D;font:bold 11px arial;border:1px solid #B0B0B0 ;'>&nbsp;</td>");

                                pUser.Append("</tr>");
                            }
                            pUser.Append("</table></div></body></html>");
                        }
                        else
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','No data to generate Excel');</script>";
                            return RedirectToAction("Index", "UserActivities");
                        }

                        //Document document = new Document();
                        // writer = PdfWriter.GetInstance(document, System.Web.HttpContext.Current.Response.OutputStream);
                        ////writer.PageEvent = new PDFFooter();
                        //document.Open();
                        //TextReader txtReader = new StringReader(pUser.ToString());
                        //var xmlWorkerHelper = XMLWorkerHelper.GetInstance();
                        //xmlWorkerHelper.ParseXHtml(writer, document, txtReader);
                        //document.Close();
                        //System.Web.HttpContext.Current.Response.Write(document);
                        Response.Output.Write(pUser.ToString());
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','No data to generate Excel');</script>";
                        return RedirectToAction("Index", "UserActivities");
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
        private bool ParseExact(string dateString, string format, out DateTime dateTime)
        {
            bool result = DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
            Logger.Debug(result.ToString());
            return result;
        }
    }
}