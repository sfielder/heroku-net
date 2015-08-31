using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.DataAccess.Repository;
using PagedList;
using System.Text;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class TerritoryController : BaseController
    {
        #region Constructor
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        public TerritoryController()
        {
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
        }
        public TerritoryController(WorkersInMotionDB context)
        {
            this._IRegionRepository = new RegionRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IGlobalUserRepository = new GlobalUserRepository(context);
        }

        #endregion
        // GET: /User/
        public ActionResult Index(string Id = "", int? page = 1)
        {
            Logger.Debug("Inside Region Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    var territoryList = new TerritoryViewModel();
                    territoryList.Territory = new List<TerritoryModel>();

                    var appTerritory = !string.IsNullOrEmpty(Id) ? _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(Id)).ToList() : _ITerritoryRepository.GetTerritoryByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    foreach (var territory in appTerritory.ToList())
                    {
                        territoryList.Territory.Add(new TerritoryModel { Name = territory.Name, RegionGUID = territory.RegionGUID.ToString(), TerritoryGUID = territory.TerritoryGUID != null ? territory.TerritoryGUID.ToString() : Guid.Empty.ToString(), Description = territory.Description, OrganizationGUID = territory.OrganizationGUID != null ? territory.OrganizationGUID.ToString() : Guid.Empty.ToString() });
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class='actions'>");
                    sb.Append("<div class='btn-group'>");
                    if (!string.IsNullOrEmpty(Id))
                    {
                        sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>" + _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(Id)) + " <i class='icon-angle-down'></i></a>");
                    }
                    else
                    {
                        sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i>Select Area <i class='icon-angle-down'></i></a>");
                    }
                    sb.Append("<ul id='ulworkgroup' class='dropdown-menu pull-left'>");
                    sb.Append("<li><a href=" + Url.Action("Index", new { id = "" }) + ">All</a></li>");
                    List<Territory> TerritoryList = _ITerritoryRepository.GetTerritoryByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    foreach (Territory item in TerritoryList)
                    {
                        sb.Append("<li><a href=" + Url.Action("Index", new { id = item.TerritoryGUID.ToString() }) + " data-groupguid=" + item.TerritoryGUID + ">" + item.Name + "</a></li>");
                    }
                    sb.Append("</ul>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    ViewBag.TerritoryList = sb.ToString();

                    ViewBag.Id = Id;
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(territoryList.Territory.ToPagedList(pageNumber, pageSize).AsEnumerable());
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

        public ActionResult Create(string id = "")
        {
            if (!string.IsNullOrEmpty(id))
                ViewBag.RegionGUID = id;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TerritoryModel territory, string id = "")
        {
            Logger.Debug("Inside Territory Controller- Create HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        Territory _territory = _ITerritoryRepository.GetTerritoryByTerritoryID(territory.TerritoryID, new Guid(Session["OrganizationGUID"].ToString()));
                        if (_territory == null)
                        {
                            Territory Territory = new Territory();
                            Territory.TerritoryGUID = Guid.NewGuid();
                            if (!string.IsNullOrEmpty(id) && id != Guid.Empty.ToString())
                            {
                                Territory.RegionGUID = new Guid(id);
                            }
                            else
                            {
                                Territory.RegionGUID = null;
                            }
                            Territory.TerritoryID = territory.TerritoryID;
                            Territory.Name = territory.Name;
                            Territory.Description = territory.Description;
                            Territory.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                            Territory.IsDefault = false;
                            _ITerritoryRepository.InsertTerritory(Territory);
                            // _ITerritoryRepository.Save();
                            return RedirectToAction("Index", "MyCompany", new { id = "Markets", regionguid = Territory.RegionGUID });
                        }
                        else
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','MarketID already exists');</script>";
                            if (!string.IsNullOrEmpty(id))
                                ViewBag.RegionGUID = id;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(id))
                            ViewBag.RegionGUID = id;
                    }
                    return View(territory);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(territory);
            }
        }
        public ActionResult Edit(string id = "")
        {
            Logger.Debug("Inside User Controller- Edit");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    TerritoryModel territory = new TerritoryModel();
                    territory.TerritoryGUID = id;
                    Territory Territory = _ITerritoryRepository.GetTerritoryByID(new Guid(territory.TerritoryGUID));
                    if (Territory == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        territory.TerritoryGUID = Territory.TerritoryGUID.ToString();
                        territory.RegionGUID = Territory.RegionGUID != null ? Territory.RegionGUID.ToString() : Guid.Empty.ToString();
                        //   region.RegionGUID = Region.RegionGUID.ToString();
                        territory.Name = Territory.Name;
                        ViewBag.TerritoryName = Territory.Name;
                        territory.Description = Territory.Description;
                        territory.TerritoryID = Territory.TerritoryID;
                        territory.OrganizationGUID = Territory.OrganizationGUID != null ? Territory.OrganizationGUID.ToString() : Guid.Empty.ToString();
                    }
                    return View(territory);
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

        [HttpPost]
        public ActionResult Edit(TerritoryModel territory)
        {
            Logger.Debug("Inside Region Controller- Edit HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        //Territory _territory = _ITerritoryRepository.GetTerritoryByTerritoryID(territory.TerritoryID, new Guid(Session["OrganizationGUID"].ToString()));
                        //if (_territory != null)
                        //{
                        Territory Territory = new Territory();
                        Territory.TerritoryGUID = new Guid(territory.TerritoryGUID);
                        if (!string.IsNullOrEmpty(territory.RegionGUID) && territory.RegionGUID != Guid.Empty.ToString())
                        {
                            Territory.RegionGUID = new Guid(territory.RegionGUID);
                        }
                        else
                        {
                            Territory.RegionGUID = null;
                        }
                        Territory.Name = territory.Name;
                        Territory.Description = territory.Description;
                        if (!string.IsNullOrEmpty(territory.OrganizationGUID) && territory.OrganizationGUID != Guid.Empty.ToString())
                        {
                            Territory.OrganizationGUID = new Guid(territory.OrganizationGUID);
                        }
                        else
                        {
                            Territory.OrganizationGUID = null;
                        }
                        Territory.TerritoryID = territory.TerritoryID;
                        Territory.IsDefault = false;
                        _ITerritoryRepository.UpdateTerritory(Territory);
                        // _ITerritoryRepository.Save();
                        return RedirectToAction("Index", "MyCompany", new { id = "Markets", territoryguid = territory.TerritoryGUID });
                    }
                    //}

                    return View(territory);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(territory);
            }
        }
        public ActionResult Delete(string id = "", string id1 = "")
        {
            Logger.Debug("Inside Territory Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    int countCustomerStop = _IMarketRepository.GetMarketByTerritoryGUID(new Guid(id1), 1).Count();
                    int countgobalUser = _IGlobalUserRepository.GetGlobalUserByRegionandTerritory(new Guid(id), new Guid(id1)).Count();
                    if (countCustomerStop == 0 && countgobalUser == 0)
                    {
                        TerritoryModel territory = new TerritoryModel();
                        territory.RegionGUID = id;
                        territory.TerritoryGUID = id1;
                        _IMarketRepository.DeleteMarketByTerritoryGUID(new Guid(territory.TerritoryGUID));
                        _ITerritoryRepository.DeleteTerritory(new Guid(territory.TerritoryGUID));
                        //_ITerritoryRepository.Save();
                        return RedirectToAction("Index", "MyCompany", new { id = "Markets", regionguid = id });
                    }
                    else
                    {
                        if (countCustomerStop > 0 && countgobalUser > 0)
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Territory is associate with Users and Store.');</script>";
                        }
                        else if (countCustomerStop > 0)
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Territory is associate with Store.');</script>";
                        }
                        else if (countgobalUser > 0)
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Territory is associate with Users.');</script>";
                        }
                        return RedirectToAction("Index", "MyCompany", new { id = "Markets", territoryguid = id1 });
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
                return RedirectToAction("../MyCompany/Index/Markets");
            }
        }
    }
}
