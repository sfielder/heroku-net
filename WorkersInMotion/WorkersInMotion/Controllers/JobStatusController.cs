using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class JobStatusController : BaseController
    {
        #region Constructor
        private readonly IJobRepository _IJobRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IPORepository _IPORepository;
        public JobStatusController()
        {
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

        public JobStatusController(WorkersInMotionDB context)
        {
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
        //
        // GET: /JobStatus/
        public ActionResult Index(string FromDate = "", string ToDate = "", string regionguid = "", string territoryguid = "", string jobindexguid = "", string Date = "", string selection = "", string ponumber = "")
        {
            Logger.Debug("Inside AssignJob Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(Date))
                    {
                        ViewBag.DateValue = HttpUtility.HtmlDecode(Date);
                    }
                    var jobStatus = new JobStatusViewModel();
                    jobStatus.JobStatusModel = new List<JobStatusModel>();
                    var jobGroup = new List<Job>();
                    //Job ljob = new Job();

                    OrganizationUsersMap pOrganizationUsersMap = _IUserRepository.GetUserByID(new Guid(Session["UserGUID"].ToString()));
                    Logger.Debug("USerGUID:" + Session["UserGUID"].ToString());
                    if (pOrganizationUsersMap != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<div class='actions'>");
                        sb.Append("<div class='btn-group'>");
                        if (!string.IsNullOrEmpty(territoryguid))
                        {
                            Logger.Debug("Inside TerritoryGUID" + territoryguid.ToString());
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> " + _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(territoryguid)) + " <i class='icon-angle-down'></i></a>");
                        }
                        else
                        {
                            if (Session["UserType"] != null && Session["UserType"].ToString() != "ENT_U")
                            {
                                if (!string.IsNullOrEmpty(selection) && selection == "All")
                                {
                                    sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> All <i class='icon-angle-down'></i></a>");
                                }
                                else
                                {
                                    sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Select Market <i class='icon-angle-down'></i></a>");
                                }
                            }
                            else if (pOrganizationUsersMap != null)
                            {
                                sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> " + _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(pOrganizationUsersMap.TerritoryGUID.ToString())) + " <i class='icon-angle-down'></i></a>");
                            }
                        }
                        sb.Append("<ul id='ulworkgroup' class='dropdown-menu pull-right'>");

                        if (Session["UserType"] != null && Session["UserType"].ToString() != "ENT_U" && pOrganizationUsersMap != null)
                        {
                            if (string.IsNullOrEmpty(selection) || selection != "All")
                            {
                                sb.Append("<li><a href=" + Url.Action("Index", "JobStatus", new { selection = "All" }) + ">All</a></li>");
                            }
                            List<Territory> TerritoryList = new List<Territory>();
                            if (Session["UserType"].ToString() == "ENT_A")
                            {
                                Logger.Debug("Inside OrganizationGUID" + pOrganizationUsersMap.OrganizationGUID.ToString());
                                TerritoryList = _ITerritoryRepository.GetTerritoryByOrganizationGUID(new Guid(pOrganizationUsersMap.OrganizationGUID.ToString())).ToList();
                            }
                            else
                            {
                                if (pOrganizationUsersMap.RegionGUID != null)
                                {
                                    Logger.Debug("Inside RegionGUID" + pOrganizationUsersMap.RegionGUID.ToString());
                                    TerritoryList = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(pOrganizationUsersMap.RegionGUID.ToString())).ToList();
                                }
                            }
                            if (TerritoryList != null && TerritoryList.Count > 0)
                            {
                                foreach (Territory item in TerritoryList)
                                {
                                    sb.Append("<li><a href=" + Url.Action("Index", "JobStatus", new { territoryguid = item.TerritoryGUID.ToString(), regionguid = item.RegionGUID.ToString() }) + " data-groupguid=" + item.TerritoryGUID + ">" + item.Name + "</a></li>");
                                    Logger.Debug("Inside Territory foreach");
                                }
                            }
                        }
                        sb.Append("</ul>");
                        sb.Append("</div>");
                        sb.Append("</div>");

                        ViewBag.RegionList = sb.ToString();
                        Job mjob = new Job();
                        if (!string.IsNullOrEmpty(ponumber))
                        {
                            mjob.PONumber = ponumber;
                            TempData["PoNumber"] = ponumber;
                        }
                        if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                        {
                            //ViewBag.FromDate = FromDate;
                            //ViewBag.ToDate = ToDate;
                            if (Session["UserType"] != null && Session["UserType"].ToString() == "ENT_U")
                            {
                                mjob.ActualStartTime = Convert.ToDateTime(FromDate);
                                // mjob.PreferedStartTime = _IUserRepository.GetLocalDateTime(mjob.PreferedStartTime, Session["TimeZoneID"].ToString());
                                mjob.ActualEndTime = Convert.ToDateTime(ToDate);
                                //  mjob.PreferedEndTime = _IUserRepository.GetLocalDateTime(mjob.PreferedEndTime, Session["TimeZoneID"].ToString());
                                mjob.TerritoryGUID = pOrganizationUsersMap.TerritoryGUID;
                                mjob.RegionGUID = pOrganizationUsersMap.RegionGUID;
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                            else
                            {
                                mjob.ActualStartTime = Convert.ToDateTime(FromDate);
                                //   mjob.PreferedStartTime = _IUserRepository.GetLocalDateTime(mjob.PreferedStartTime, Session["TimeZoneID"].ToString());
                                mjob.ActualEndTime = Convert.ToDateTime(ToDate);
                                //    mjob.PreferedEndTime = _IUserRepository.GetLocalDateTime(mjob.PreferedEndTime, Session["TimeZoneID"].ToString());
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);


                            }
                        }
                        else if (!string.IsNullOrEmpty(regionguid) && !string.IsNullOrEmpty(territoryguid))
                        {
                            mjob.ActualStartTime = DateTime.Now.AddDays(-29);
                            mjob.ActualEndTime = DateTime.Now;
                            mjob.RegionGUID = new Guid(regionguid);
                            mjob.TerritoryGUID = new Guid(territoryguid);
                            mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                            jobGroup = _IJobRepository.GetJobs(mjob);
                        }
                        else
                        {
                            mjob.ActualStartTime = DateTime.Now.AddDays(-29);
                            mjob.ActualEndTime = DateTime.Now;
                            if (Session["UserType"] != null && Session["UserType"].ToString() == "ENT_U")
                            {
                                mjob.RegionGUID = pOrganizationUsersMap.RegionGUID;
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                            else
                            {
                                mjob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                                jobGroup = _IJobRepository.GetJobs(mjob);
                            }
                            //if (Session["UserType"] != null && Session["UserType"].ToString() != "ENT_U")
                            //{
                            //    jobGroup = _IJobRepository.GetjobStatusByRegionAndTerritory(new Guid(Session["UserGUID"].ToString())).ToList();
                            //}
                            //else
                            //{
                            //    ljob.AssignedUserGUID = new Guid(Session["UserGUID"].ToString());
                            //    ljob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                            //    ljob.IsDeleted = false;
                            //    jobGroup = _IJobRepository.GetJobStatus(ljob).ToList();
                            //}
                        }

                        if (jobGroup != null && jobGroup.Count > 0)
                        {
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
                                //js.GroupName = _IJobRepository.GetGroupName(job.GroupCode);




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
                                        if (!string.IsNullOrEmpty(_Market.FMUserID))
                                        {
                                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.FMUserID, Session["OrganizationGUID"].ToString());
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
                                    }
                                    else
                                    {
                                        js.RegionalManager = "";
                                        js.FieldManager = "";
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
                                            if (!string.IsNullOrEmpty(_Market.FMUserID))
                                            {
                                                GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.FMUserID, Session["OrganizationGUID"].ToString());
                                                if (_globalUser != null)
                                                {
                                                    UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
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
                                        }
                                        else
                                        {
                                            js.RegionalManager = "";
                                            js.FieldManager = "";
                                        }

                                    }
                                    else
                                    {
                                        js.RegionalManager = "";
                                        js.FieldManager = "";
                                    }
                                }
                                else
                                {
                                    js.RegionalManager = "";
                                    js.FieldManager = "";
                                }

                                jobStatus.JobStatusModel.Add(js);
                            }
                        }
                        if (!string.IsNullOrEmpty(regionguid) && !string.IsNullOrEmpty(territoryguid) && !string.IsNullOrEmpty(jobindexguid))
                        {
                            IList<GlobalUser> _GlobalUser = _IGlobalUserRepository.GetGlobalUserByRegionandTerritory(new Guid(regionguid), new Guid(territoryguid)).ToList();
                            jobStatus.GlobalUsers = new List<GlobalUserModel>();
                            foreach (var user in _GlobalUser.ToList())
                            {
                                jobStatus.GlobalUsers.Add(new GlobalUserModel
                                {
                                    UserGUID = user.UserGUID,
                                    UserName = user.UserName
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



                    return View(jobStatus);
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
    }
}