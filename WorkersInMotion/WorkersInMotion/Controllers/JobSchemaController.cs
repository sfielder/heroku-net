using System;
using System.Collections.Generic;
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
    public class JobSchemaController : BaseController
    {

        #region Constructor
        private readonly IJobSchemaRepository _IJobSchemaRepository;
        private readonly IJobRepository _IJobRepository;
        private readonly IUserRepository _IUserRepository;
        // private readonly IGroupRepository _IGroupRepository;

        public static JobPageSchemaViewModel jobPageSchemaViewModel = new JobPageSchemaViewModel();
        public static List<JobPageSchemaModel> jobPageList = new List<JobPageSchemaModel>();

        public static JobPageItemViewModel jobPageItemViewModel = new JobPageItemViewModel();
        public static List<JobPageItemModel> jobItemList = new List<JobPageItemModel>();

        public static JobForm Jschema = new JobForm();


        public JobSchemaController()
        {
            this._IJobSchemaRepository = new JobSchemaRepository(new WorkersInMotionDB());
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
            //   this._IGroupRepository = new GroupRepository(new WorkersInMotionContext());
        }
        public JobSchemaController(WorkersInMotionDB context)
        {
            this._IJobSchemaRepository = new JobSchemaRepository(context);
            this._IJobRepository = new JobRepository(context);
            this._IUserRepository = new UserRepository(context);
        }

        #endregion
        public void DropdownValues()
        {
            //var GroupDetails = _IGroupRepository.GetGroupByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList().Select(r => new SelectListItem
            //{
            //    Value = r.GroupGUID.ToString(),
            //    Text = r.Name
            //});
            //ViewBag.GroupDetails = new SelectList(GroupDetails, "Value", "Text");

        }

        //
        // GET: /Job/
        public ActionResult Index(string id = "")
        {
            Logger.Debug("Inside Job Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    var jobSchemaList = new JobSchemaViewModel();
                    jobSchemaList.JobSchemaModel = new List<JobSchemaModel>();
                    var appJobSchema = new List<JobForm>();
                    // if (string.IsNullOrEmpty(id))
                    {
                        appJobSchema = _IJobSchemaRepository.GetJobSchema(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    }
                    //else
                    //{
                    //    appJobSchema = _IJobSchemaRepository.GetJobSchemabyGroupCode(new Guid(id)).ToList();
                    //}
                    foreach (var jobshcema in appJobSchema.ToList())
                    {
                        JobSchemaModel pJobSchemaModel = new JobSchemaModel();

                        pJobSchemaModel.JobLogicalID = jobshcema.JobFormGUID.ToString();
                        pJobSchemaModel.OrganizationGUID = jobshcema.OrganizationGUID != null ? jobshcema.OrganizationGUID.ToString() : Guid.Empty.ToString();
                        pJobSchemaModel.JobSchemaName = jobshcema.FriendlyName;
                        // GroupCode = "",//jobshcema.GroupCode.ToString(),

                        pJobSchemaModel.LastModifiedDate = Session["TimeZoneID"] != null ? Convert.ToDateTime(_IUserRepository.GetLocalDateTime(jobshcema.LastModifiedDate, Session["TimeZoneID"].ToString())) : Convert.ToDateTime(jobshcema.LastModifiedDate);
                        pJobSchemaModel.LastModifiedDateTime = pJobSchemaModel.LastModifiedDate.ToString("MM/dd/yy hh:mm tt");
                        pJobSchemaModel.LastModifiedBy = jobshcema.LastModifiedBy.ToString();
                        //  EstimatedDuration = jobshcema.EstimatedDuration


                        jobSchemaList.JobSchemaModel.Add(pJobSchemaModel);
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class='actions'>");
                    sb.Append("<div class='btn-group'>");
                    //if (!string.IsNullOrEmpty(id))
                    //{
                    //    sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>" + _IGroupRepository.GetGroupNameByGroupGUID(new Guid(id)) + " <i class='icon-angle-down'></i></a>");
                    //}
                    //else
                    {
                        sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>Worker Group <i class='icon-angle-down'></i></a>");
                    }
                    sb.Append("<ul id='ulworkgroup' class='dropdown-menu pull-left'>");
                    sb.Append("<li><a href=" + Url.Action("Index", new { id = "" }) + ">All</a></li>");
                    //List<Group> GroupList = _IGroupRepository.GetGroupByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    //foreach (Group item in GroupList)
                    //{

                    //    sb.Append("<li><a href=" + Url.Action("Index", new { id = item.GroupGUID.ToString() }) + " data-groupguid=" + item.GroupGUID + ">" + item.Name + "</a></li>");
                    //}
                    sb.Append("</ul>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    ViewBag.Group = sb.ToString();
                    return View(jobSchemaList.JobSchemaModel.AsEnumerable());
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
                jobPageList.Clear();
                jobItemList.Clear();
                Jschema = new JobForm();
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
        public ActionResult Create(JobSchemaModel jobschema)
        {
            Logger.Debug("Inside Job Controller- Create HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    if (ModelState.IsValid)
                    {
                        Jschema = new JobForm();
                        jobPageList.Clear();
                        jobItemList.Clear();
                        Jschema.JobFormGUID = Guid.NewGuid();
                        Jschema.FriendlyName = jobschema.JobSchemaName;
                        Jschema.IsActive = true;
                        Jschema.JobClass = jobschema.JobClass;
                        Jschema.Skill = "";// new Guid(jobschema.GroupCode);
                        //Jschema.EstimatedDuration = jobschema.EstimatedDuration;
                        Jschema.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        Jschema.LastModifiedBy = new Guid(Session["UserGUID"].ToString());
                        Jschema.IsDeleted = false;
                        Jschema.LastModifiedDate = Convert.ToDateTime(DateTime.UtcNow);

                        ViewBag.JobLogicalID = Jschema.JobFormGUID.ToString();
                        ViewData["JobName"] = jobschema.JobSchemaName;
                        ViewData["JobClass"] = jobschema.JobClass;
                        // ViewData["GroupCode"] = jobschema.GroupCode;
                        ViewData["GroupName"] = "";// _IGroupRepository.GetGroupByID(new Guid(jobschema.GroupCode)).Name;
                        ViewData["EstimatedDuration"] = "";// jobschema.EstimatedDuration;
                        ViewBag.BasicDetails = "true";



                    }

                    return View(jobschema);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(jobschema);
            }
        }

        public ActionResult CreateJob()
        {
            Logger.Debug("Inside Job Controller- Create Job");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    JobFormModel jobFormModel = new JobFormModel();
                    jobFormModel.JobPageList = jobPageList;
                    jobFormModel.JobItemList = jobItemList;
                    Jschema.JobForm1 = new JavaScriptSerializer().Serialize(jobFormModel);
                    _IJobSchemaRepository.InsertJobSchema(Jschema);
                    // _IJobSchemaRepository.Save();
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
                //_IJobSchemaRepository.DeleteJobPageItemByJobLogicalGUID(Jschema.JobLogicalID);
                //_IJobSchemaRepository.DeleteJobPageByJobLogicalGUID(Jschema.JobLogicalID);
                //_IJobSchemaRepository.DeleteJobSchema(Jschema.JobLogicalID);
                //_IJobSchemaRepository.Save();
                return RedirectToAction("Index");
            }
        }
        public ActionResult Edit(string id)
        {
            if (Session["OrganizationGUID"] != null)
            {
                DropdownValues();
                Jschema = new JobForm();
                jobPageList.Clear();
                jobItemList.Clear();
                Jschema = _IJobSchemaRepository.JobSchemaDetails(new Guid(id));
                JobSchemaModel jobschemamodel = new JobSchemaModel();
                jobschemamodel.JobLogicalID = Jschema.JobFormGUID.ToString();
                jobschemamodel.OrganizationGUID = Jschema.OrganizationGUID != null ? Jschema.OrganizationGUID.ToString() : Guid.Empty.ToString();
                jobschemamodel.LastModifiedBy = Jschema.LastModifiedBy != null ? Jschema.LastModifiedBy.ToString() : Guid.Empty.ToString();
                jobschemamodel.LastModifiedDate = Convert.ToDateTime(Jschema.LastModifiedDate);
                jobschemamodel.JobSchemaName = Jschema.FriendlyName;
                // jobschemamodel.EstimatedDuration = Jschema.EstimatedDuration;
                // jobschemamodel.GroupCode = Jschema.GroupCode.ToString();
                jobschemamodel.IsDeleted = Convert.ToBoolean(Jschema.IsDeleted);
                jobschemamodel.JobClass = Jschema.JobClass;

                ViewBag.JobLogicalID = Jschema.JobFormGUID.ToString();
                ViewData["JobName"] = Jschema.FriendlyName;
                ViewData["JobClass"] = Jschema.JobClass == null ? 0 : Jschema.JobClass;// Jschema.Description;
                ViewData["GroupCode"] = "";// Jschema.GroupCode;
                ViewData["GroupName"] = "";// _IGroupRepository.GetGroupByID(Jschema.GroupCode).Name;
                ViewData["EstimatedDuration"] = "";// Jschema.EstimatedDuration;
                ViewBag.BasicDetails = "true";


                JobFormModel jobFormModel = new JavaScriptSerializer().Deserialize<JobFormModel>(Jschema.JobForm1);
                jobPageList = jobFormModel.JobPageList;
                jobItemList = jobFormModel.JobItemList;

                JobSchemaEditView jobschemaedit = new JobSchemaEditView();
                jobschemaedit.JobPageSchemaModel = jobPageList;
                jobschemaedit.JobPageItemModel = jobItemList;
                jobschemaedit.JobSchemaModel = jobschemamodel;

                return View(jobschemaedit);
            }
            else
            {
                return RedirectToAction("SessionTimeOut", "User");
            }
        }

        public ActionResult Editjob()
        {
            Logger.Debug("Inside Job Controller- Create Job");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    int deleteresult = 0;
                    Int16 jobclass;
                    if (short.TryParse(Jschema.JobClass.ToString(), out jobclass) && Jschema.OrganizationGUID != null)
                    {
                        IList<Job> job = _IJobSchemaRepository.GetjobByJobFormClass(jobclass, new Guid(Jschema.OrganizationGUID.ToString())).ToList();
                        if (job != null && job.Count > 0)
                        {
                            deleteresult = _IJobSchemaRepository.SetDeleteFlag(Jschema.JobFormGUID);
                        }
                        else
                        {
                            deleteresult = _IJobSchemaRepository.DeleteJobSchema(Jschema.JobFormGUID);
                            //deleteresult = _IJobSchemaRepository.Save();
                        }
                    }

                    if (deleteresult > 0)
                    {
                        JobFormModel jobFormModel = new JobFormModel();
                        jobFormModel.JobPageList = jobPageList;
                        jobFormModel.JobItemList = jobItemList;
                        Jschema.JobForm1 = new JavaScriptSerializer().Serialize(jobFormModel);
                        Jschema.JobFormGUID = Guid.NewGuid();
                        Jschema.IsActive = true;
                        int JobResult = _IJobSchemaRepository.InsertJobSchema(Jschema);
                        //int JobResult = _IJobSchemaRepository.Save();
                    }

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
                //_IJobSchemaRepository.DeleteJobPageItemByJobLogicalGUID(Jschema.JobLogicalID);
                //_IJobSchemaRepository.DeleteJobPageByJobLogicalGUID(Jschema.JobLogicalID);
                //_IJobSchemaRepository.DeleteJobSchema(Jschema.JobLogicalID);
                //_IJobSchemaRepository.Save();
                return RedirectToAction("Index");
            }
        }

        public ActionResult ViewSchema(string id = "")
        {
            Logger.Debug("Inside Job Controller- View Job");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    Jschema = new JobForm();
                    jobPageList.Clear();
                    jobItemList.Clear();
                    Jschema = _IJobSchemaRepository.JobSchemaDetails(new Guid(id));
                    ViewBag.JobName = Jschema.FriendlyName;
                    JobFormModel jobFormModel = new JavaScriptSerializer().Deserialize<JobFormModel>(Jschema.JobForm1);
                    jobPageList = jobFormModel.JobPageList;
                    jobItemList = jobFormModel.JobItemList;

                    JobSchemaEditView jobschemaedit = new JobSchemaEditView();
                    jobschemaedit.JobPageSchemaModel = jobPageList;
                    jobschemaedit.JobPageItemModel = jobItemList;
                    jobschemaedit.JobSchemaModel = null;

                    return View(jobschemaedit);
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

        public JsonResult Page(string joblogicalID, string pagename, string pageorder, bool canrepeat)
        {
            Logger.Debug("Inside Job Controller- Page");
            JsonResult result = new JsonResult();
            try
            {
                JobPageSchemaModel jobpageSchema = new JobPageSchemaModel();
                jobpageSchema.JobLogicalID = joblogicalID;
                jobpageSchema.PageSchemaName = pagename;
                jobpageSchema.PageLogicalID = Guid.NewGuid().ToString();
                jobpageSchema.CanRepeat = canrepeat ? 1 : 0;
                jobpageSchema.CreateDate = DateTime.UtcNow;

                jobPageList.Add(jobpageSchema);

                Dictionary<string, string> page = new Dictionary<string, string>();
                page.Add("ID", jobpageSchema.PageLogicalID);
                page.Add("Name", jobpageSchema.PageSchemaName);

                result.Data = page;
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return result;
            }
        }
        public JsonResult PageItem(string joblogicalid, string pagelogicalid, string itemname, string type, bool isrequired, bool canedit, bool canview, bool canrepeat, bool itemvalue, string itemvaluetype)
        {
            Logger.Debug("Inside Job Controller- PageItem");
            JsonResult result = new JsonResult();
            try
            {
                JobPageItemModel jobitemSchema = new JobPageItemModel();
                jobitemSchema.JobLogicalID = joblogicalid;
                jobitemSchema.ItemName = itemname;
                jobitemSchema.PageLogicalID = pagelogicalid;
                jobitemSchema.ItemLogicalID = Guid.NewGuid().ToString();
                jobitemSchema.ItemControlType = type;
                jobitemSchema.ItemCaptureTime = DateTime.UtcNow;
                jobitemSchema.ItemValueType = itemvaluetype;
                jobitemSchema.ItemValue = itemvalue ? "1" : "0";
                jobitemSchema.IsRequired = isrequired;
                jobitemSchema.CanView = canview ? 1 : 0;
                jobitemSchema.CanEdit = canedit ? 1 : 0;
                jobitemSchema.CanRepeat = canrepeat ? 1 : 0;
                jobitemSchema.Createdate = DateTime.UtcNow;
                jobitemSchema.ItemOrder = 0;
                jobitemSchema.SortOrder = 0;

                jobItemList.Add(jobitemSchema);
                if (type == "text" || type == "decimal" || type == "image" || type == "audio" || type == "number" || type == "phone" || type == "date" || type == "email")
                    result.Data = "text$" + jobitemSchema.ItemLogicalID + "";
                else if (type == "check")
                    result.Data = "check$" + jobitemSchema.ItemLogicalID + "";
                else if (type == "label")
                    result.Data = "label$" + jobitemSchema.ItemLogicalID + "";
                else if (type == "multitext")
                    result.Data = "multitext$" + jobitemSchema.ItemLogicalID + "";
                else if (type == "radio")
                    result.Data = "radio$" + jobitemSchema.ItemLogicalID + "";
                else if (type == "dropdown")
                    result.Data = "dropdown$" + jobitemSchema.ItemLogicalID + "";
                else
                    result.Data = "success";
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return result;
            }
        }

        public JsonResult DeletePageItem(string itemlogicalID)
        {
            Logger.Debug("Inside Job Controller- DeletePageItem");
            JsonResult result = new JsonResult();
            try
            {
                jobItemList.Remove(jobItemList.Where(x => x.ItemLogicalID == itemlogicalID).FirstOrDefault());
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
        }

        public JsonResult DeletePage(string PageID)
        {
            Logger.Debug("Inside Job Controller- DeletePage");
            JsonResult result = new JsonResult();
            try
            {
                foreach (JobPageItemModel item in jobItemList.Where(x => x.PageLogicalID == PageID).ToList())
                {
                    jobItemList.Remove(jobItemList.Where(x => x.ItemLogicalID == item.ItemLogicalID).FirstOrDefault());
                }
                jobPageList.Remove(jobPageList.Where(x => x.PageLogicalID == PageID).FirstOrDefault());

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
        }

        public ActionResult Delete(string id)
        {
            Logger.Debug("Inside Job Controller- Delete");
            JsonResult result = new JsonResult();
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    Job lJob = new Job();
                    lJob.JobGUID = new Guid(id);
                    lJob.IsDeleted = false;
                    IList<Job> Job = _IJobRepository.GetJobStatus(lJob);
                    if (Job != null && Job.Count > 0)
                    {
                        _IJobSchemaRepository.SetDeleteFlag(new Guid(id));
                    }
                    else
                    {
                        //_IJobSchemaRepository.DeleteJobPageItemByJobLogicalGUID(new Guid(id));
                        //_IJobSchemaRepository.DeleteJobPageByJobLogicalGUID(new Guid(id));
                        //_IJobRepository.DeleteJobByJobLogicalID(new Guid(id));
                        _IJobSchemaRepository.DeleteJobSchema(new Guid(id));
                        //_IJobSchemaRepository.Save();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("../User/Login");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}