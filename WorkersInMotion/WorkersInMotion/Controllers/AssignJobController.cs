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
    public class AssignJobController : BaseController
    {
        #region Constructor
        private readonly IJobRepository _IJobRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        private readonly IJobSchemaRepository _IJobSchemaRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        // private readonly IGroupRepository _IGroupRepository;
        public AssignJobController()
        {
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            this._IJobSchemaRepository = new JobSchemaRepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            //this._IGroupRepository = new GroupRepository(new WorkersInMotionDB());
        }

        public AssignJobController(WorkersInMotionDB context)
        {
            this._IGlobalUserRepository = new GlobalUserRepository(context);
            this._IPlaceRepository = new PlaceRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IJobRepository = new JobRepository(context);
            this._IJobSchemaRepository = new JobSchemaRepository(context);
            // this._IGroupRepository = new GroupRepository(context);
        }

        #endregion

        //
        // GET: /AssignJob/
        public ActionResult Index(string regionguid = "", string territoryguid = "", string jobindexguid = "", string groupguid = "", string territoryid = "")
        {
            Logger.Debug("Inside Job Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    var jobList = new JobViewModel();
                    jobList.JobModelList = new List<JobModel>();
                    var jobGroup = new List<Job>();
                    Job ljob = new Job();
                    if (!string.IsNullOrEmpty(groupguid))
                    {
                        jobGroup = _IJobRepository.GetjobByGroupGUID(new Guid(groupguid)).ToList();
                    }
                    else if (!string.IsNullOrEmpty(territoryid))
                    {
                        ljob.TerritoryGUID = new Guid(territoryid);
                        ljob.IsDeleted = false;
                        jobGroup = _IJobRepository.GetOpenJobs(ljob).ToList();
                    }
                    else
                    {
                        jobGroup = _IJobRepository.GetjobByRegionandTerritory(new Guid(Session["UserGUID"].ToString())).ToList();
                    }
                    foreach (var job in jobGroup.ToList())
                    {
                        jobList.JobModelList.Add(new JobModel
                        {
                            JobName = job.JobName,
                            JobIndexGUID = job.JobGUID,
                            //  JobLogicalID = new Guid(job.JobFormGUID.ToString()),
                            PreferredEndTime = Convert.ToDateTime(job.PreferedEndTime).ToString("yy/MM/dd HH:mm"),
                            PreferredStartTime = Convert.ToDateTime(job.PreferedStartTime).ToString("yy/MM/dd HH:mm"),
                            RegionCode = job.RegionGUID != null ? new Guid(job.RegionGUID.ToString()) : Guid.Empty,
                            TerritoryCode = job.TerritoryGUID != null ? new Guid(job.TerritoryGUID.ToString()) : Guid.Empty,
                        });
                    }
                    var viewmodel = new AssignJobModel();
                    if (string.IsNullOrEmpty(regionguid) && string.IsNullOrEmpty(territoryguid) && string.IsNullOrEmpty(jobindexguid))
                    {
                        viewmodel.GlobalUsers = null;
                        viewmodel.JobModel = null;
                        viewmodel.JobModelList = jobList.JobModelList;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(regionguid) && !string.IsNullOrEmpty(territoryguid))
                        {
                            IList<GlobalUser> _GlobalUser = _IGlobalUserRepository.GetGlobalUserByRegionandTerritory(new Guid(regionguid), new Guid(territoryguid)).ToList();
                            viewmodel.GlobalUsers = new List<GlobalUserModel>();
                            foreach (var user in _GlobalUser.ToList())
                            {
                                viewmodel.GlobalUsers.Add(new GlobalUserModel
                                {
                                    UserGUID = user.UserGUID,
                                    UserName = user.UserName
                                });
                            }
                        }
                        if (!string.IsNullOrEmpty(jobindexguid))
                        {
                            viewmodel.JobModel = new JobModel();
                            viewmodel.JobModel.JobName = _IJobRepository.GetJobByID(new Guid(jobindexguid)).JobName;
                            viewmodel.JobModel.JobIndexGUID = new Guid(jobindexguid);
                        }
                        viewmodel.JobModelList = jobList.JobModelList;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class='actions'>");
                    sb.Append("<div class='btn-group'>");
                    if (!string.IsNullOrEmpty(territoryid))
                    {
                        sb.Append("<a href='#' id='ulaterritorygroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>" + _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(territoryid)) + " <i class='icon-angle-down'></i></a>");
                    }
                    else
                    {
                        sb.Append("<a href='#' id='ulaterritorygroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>Zone <i class='icon-angle-down'></i></a>");
                    }
                    sb.Append("<ul id='ulterritorygroup' class='dropdown-menu pull-left'>");
                    sb.Append("<li><a href=" + Url.Action("Index", new { id = "" }) + ">All</a></li>");
                    List<Territory> TerritoryList = _ITerritoryRepository.GetTerritoryByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    foreach (Territory item in TerritoryList)
                    {
                        sb.Append("<li><a href=" + Url.Action("Index", new { territoryid = item.TerritoryGUID.ToString() }) + " data-groupguid=" + item.TerritoryGUID + ">" + item.Name + "</a></li>");
                    }
                    sb.Append("</ul>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    ViewBag.ZoneList = sb.ToString();

                    StringBuilder sb1 = new StringBuilder();
                    sb1.Append("<div class='actions'>");
                    sb1.Append("<div class='btn-group'>");
                    //if (!string.IsNullOrEmpty(groupguid))
                    //{
                    //    sb1.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>" + _IGroupRepository.GetGroupNameByGroupGUID(new Guid(groupguid)) + " <i class='icon-angle-down'></i></a>");
                    //}
                    //else
                    {
                        sb1.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>Worker Group <i class='icon-angle-down'></i></a>");
                    }
                    sb1.Append("<ul id='ulworkergroup' class='dropdown-menu pull-left'>");
                    sb1.Append("<li><a href=" + Url.Action("Index", new { id = "" }) + ">All</a></li>");
                    //List<Group> GroupList = _IGroupRepository.GetGroupByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    //foreach (Group item in GroupList)
                    //{
                    //    sb1.Append("<li><a href=" + Url.Action("Index", new { groupguid = item.GroupGUID.ToString() }) + " data-groupguid=" + item.GroupGUID + ">" + item.Name + "</a></li>");
                    //}
                    sb1.Append("</ul>");
                    sb1.Append("</div>");
                    sb1.Append("</div>");
                    ViewBag.GroupList = sb1.ToString();

                    return View(viewmodel);
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
        public ActionResult Assign(string id = "", string jobindexguid = "")
        {
            Logger.Debug("Inside Job Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    ViewBag.UserGUID = id;
                    ViewBag.JobIndexGUID = jobindexguid;
                    var jobList = new AssignJobViewModel();
                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(jobindexguid))
                    {
                        jobList.AssignJobModelList = new List<AssignModel>();
                        var jobGroup = _IJobRepository.GetjobByUserandJobID(new Guid(id), new Guid(jobindexguid)).ToList();
                        foreach (var job in jobGroup.ToList())
                        {
                            jobList.AssignJobModelList.Add(new AssignModel
                            {
                                JobName = job.JobName,
                                ServiceStartTime = job.ScheduledStartTime.ToString() != "" ? Convert.ToDateTime(job.ScheduledStartTime).ToString("yy/MM/dd HH:mm") : "",
                                JobIndexGUID = job.JobGUID,
                                UserGUID = job.AssignedUserGUID.ToString() != "" ? new Guid(job.AssignedUserGUID.ToString()) : Guid.Empty
                            });
                        }
                    }
                    return View(jobList.AssignJobModelList.AsEnumerable());
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

        public ActionResult AssignUser(string id = "", string jobindexguid = "")
        {
            Logger.Debug("Inside Job Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(jobindexguid))
                    {
                        Job _job = new Job();
                        _job.JobGUID = new Guid(jobindexguid);
                        _job.AssignedUserGUID = new Guid(id);
                        _job.LastModifiedDate = DateTime.UtcNow;
                        _job.LastModifiedBy = new Guid(Session["UserGUID"].ToString());
                        int result = _IJobRepository.AssignJob(_job);
                        if (result > 0)
                        {
                            Job job = _IJobRepository.GetJobByID(_job.JobGUID);
                            if (job != null)
                            {
                                JobAssigned _jobAssigned = new JobAssigned();
                                _jobAssigned.JobAssignGUID = Guid.NewGuid();
                                _jobAssigned.JobGUID = job.JobGUID;
                                _jobAssigned.UserGUID = job.AssignedUserGUID;
                                _jobAssigned.StartTime = job.ScheduledEndTime;
                                _jobAssigned.EndTime = job.ScheduledEndTime;
                                _jobAssigned.Latitude = job.Latitude;
                                _jobAssigned.Longitude = job.Longitude;
                                _jobAssigned.LastModifiedDate = DateTime.UtcNow;
                                _jobAssigned.LastModifiedBy = new Guid(Session["UserGUID"].ToString());
                                _IJobRepository.InsertJobAssigned(_jobAssigned);
                                //_IJobRepository.Save();
                            }

                        }
                        // _IJobRepository.AssignUser(new Guid(id), new Guid(jobindexguid));
                    }
                    return RedirectToAction("Index", "JobStatus");
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("../User/Login");

            }
        }
    }
}