using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.DataAccess.Repository;
using PagedList;
using System.Text;

namespace WorkersInMotion.Controllers
{
    public class GroupController : Controller
    {
        #region Constructor
        private readonly IGroupRepository _IGroupRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        public GroupController()
        {
            this._IGroupRepository = new GroupRepository(new WorkersInMotionContext());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionContext());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionContext());
        }
        public GroupController(WorkersInMotionContext context)
        {
            this._IGroupRepository = new GroupRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
        }

        #endregion

        public void DropdownValues()
        {
            var GroupDetails = _IGroupRepository.GetGroupByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList().OrderBy(r => r.Name).Select(r => new SelectListItem
            {
                Value = r.GroupGUID.ToString(),
                Text = r.Name
            });
            ViewBag.GroupDetails = new SelectList(GroupDetails, "Value", "Text");
        }
        // GET: /User/
        public ActionResult Index(int? page = 1)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside Group Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    var groupList = new GroupViewModel();
                    groupList.Group = new List<GroupModel>();
                    var appGroup = _IGroupRepository.GetGroupByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    foreach (var group in appGroup.ToList())
                    {
                        groupList.Group.Add(new GroupModel { Name = group.Name, GroupGUID = group.GroupGUID.ToString(), Description = group.Description, OrganizationGUID = group.OrganizationGUID.ToString() });
                    }
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(groupList.Group.ToPagedList(pageNumber, pageSize).AsEnumerable());
                }
                else
                {
                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("../User/Login");
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                return null;

            }
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GroupModel group)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside Group Controller- Create HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        Group Group = new Group();
                        Group.GroupGUID = Guid.NewGuid();
                        Group.Name = group.Name;
                        Group.Description = group.Description;
                        Group.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        Group.IsDefault = false;
                        _IGroupRepository.InsertGroup(Group);
                        _IGroupRepository.Save();
                        return RedirectToAction("Index");
                    }

                    return View(group);
                }
                else
                {
                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("../User/Login");
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                return View(group);
            }
        }
        public ActionResult Edit(string id = "")
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside Group Controller- Edit");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    GroupModel group = new GroupModel();
                    group.GroupGUID = id;
                    Group Group = _IGroupRepository.GetGroupByID(new Guid(group.GroupGUID));
                    if (Group == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        group.GroupGUID = Group.GroupGUID.ToString();
                        group.Name = Group.Name;
                        group.Description = Group.Description;
                        group.OrganizationGUID = Group.OrganizationGUID.ToString();
                    }
                    return View(group);
                }
                else
                {
                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("../User/Login");
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                return View();
            }
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(GroupModel group)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside Group Controller- Edit HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        Group Group = new Group();
                        Group.GroupGUID = new Guid(group.GroupGUID);
                        Group.Name = group.Name;
                        Group.Description = group.Description;
                        Group.OrganizationGUID = new Guid(group.OrganizationGUID);
                        Group.IsDefault = false;
                        _IGroupRepository.UpdateGroup(Group);
                        _IGroupRepository.Save();
                        return RedirectToAction("Index");
                    }

                    return View(group);
                }
                else
                {
                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("../User/Login");
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                return View(group);
            }
        }
        public ActionResult Delete(string id = "")
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside Group Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    GroupModel group = new GroupModel();
                    group.GroupGUID = id;
                    _IGroupRepository.DeleteGroup(new Guid(group.GroupGUID));
                    _IGroupRepository.Save();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("../User/Login");
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                return RedirectToAction("Index");
            }
        }
        public JsonResult GroupUsers(string GroupGuid)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside Group Controller- PageItem");
            JsonResult result = new JsonResult();
            StringBuilder sb = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(GroupGuid))
                {
                    IList<UserProfile> UserList = _IUserProfileRepository.GetUserProfileByGroupGUID(new Guid(GroupGuid)).ToList();

                    sb.Append("<table class='table table-striped table-bordered table-hover '>");
                    sb.Append("<tbody><tr>");
                    sb.Append("<th>Name</th>");
                    sb.Append("<th>Mobile Phone</th>");
                    sb.Append("<th>Action</th>");
                    sb.Append("</tr>");
                    if (UserList != null && UserList.Count > 0)
                    {
                        foreach (UserProfile user in UserList)
                        {
                            sb.Append("<tr class='odd gradeX'>");
                            sb.Append("<td>" + user.FirstName + "</td>");
                            sb.Append("<td>" + user.MobilePhone + "</td>");
                            sb.Append("<td><a class='label label-sm label-info' href='#' id='" + user.UserGUID.ToString() + "' onclick='return btngroupclick(this);' style='border-radius:0px;'>Change Group</a></td>");
                            sb.Append("</tr>");
                        }

                    }
                    else
                    {
                        sb.Append("<tr><td colspan='3' align='center' style='color:red'>No Data Found</td></tr>");
                    }
                    sb.Append("</tbody></table>");
                }
                result.Data = sb.ToString();
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                return result;
            }
        }
        public ActionResult ChangeGroup(string id, string DropGroupGUID = "")
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside Group Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    int result = _IGlobalUserRepository.UpdateGroupGUID(new Guid(id), new Guid(DropGroupGUID));
                    if (result > 0)
                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','User Group Changed Successfully');</script>";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("../User/Login");
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}
