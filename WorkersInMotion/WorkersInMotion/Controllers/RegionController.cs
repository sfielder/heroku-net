using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.DataAccess.Repository;
using PagedList;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class RegionController : BaseController
    {
        #region Constructor
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly IMarketRepository _IMarketRepository;
        public RegionController()
        {
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
        }
        public RegionController(WorkersInMotionDB context)
        {
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IMarketRepository = new MarketRepository(context);
        }

        #endregion
        // GET: /User/
        public ActionResult Index(int? page = 1)
        {
            Logger.Debug("Inside Territory Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    var regionList = new RegionViewModel();
                    regionList.Region = new List<RegionModel>();
                    var appRegion = _IRegionRepository.GetRegionByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    foreach (var region in appRegion.ToList())
                    {
                        regionList.Region.Add(new RegionModel { Name = region.Name, Description = region.Description, OrganizationGUID = region.OrganizationGUID != null ? region.OrganizationGUID.ToString() : Guid.Empty.ToString() });
                    }
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(regionList.Region.ToPagedList(pageNumber, pageSize).AsEnumerable());
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
        public ActionResult Create(RegionModel region)
        {
            Logger.Debug("Inside Region Controller- Create HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        Region _region = _IRegionRepository.GetRegionByRegionID(region.RegionID, new Guid(Session["OrganizationGUID"].ToString()));
                        if (_region == null)
                        {
                            Region Region = new Region();
                            Region.RegionGUID = Guid.NewGuid();
                            Region.Name = region.Name;
                            Region.Description = region.Description;
                            Region.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                            Region.REGIONID = region.RegionID;
                            Region.IsDefault = false;
                            _IRegionRepository.InsertRegion(Region);
                            //_IRegionRepository.Save();
                            return RedirectToAction("../MyCompany/Index/Regions");
                        }
                        else
                        {
                            TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','RegionId already exists');</script>";
                            return View(region);
                        }
                    }

                    return View(region);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(region);
            }
        }
        public ActionResult Edit(string id = "")
        {
            Logger.Debug("Inside Region Controller- Edit");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    RegionModel region = new RegionModel();
                    region.RegionGUID = id;
                    Region Region = _IRegionRepository.GetRegionByID(new Guid(region.RegionGUID));
                    if (Region == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        region.RegionGUID = Region.RegionGUID.ToString();
                        region.Name = Region.Name;
                        ViewBag.RegionName = Region.Name;
                        region.RegionID = Region.REGIONID;
                        region.Description = Region.Description;
                        region.OrganizationGUID = Region.OrganizationGUID != null ? Region.OrganizationGUID.ToString() : Guid.Empty.ToString();
                        //territory.TerritoryGUID = Territory.TerritoryGUID.ToString();
                        //territory.Name = Territory.Name;
                        //ViewBag.TerritoryName = Territory.Name;
                        //territory.Description = Territory.Description;
                        //territory.OrganizationGUID = Territory.OrganizationGUID != null ? Territory.OrganizationGUID.ToString() : Guid.Empty.ToString();
                    }
                    return View(region);
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
        public ActionResult Edit(RegionModel region)
        {
            Logger.Debug("Inside Region Controller- Edit HttpPost");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    if (ModelState.IsValid)
                    {
                        //Region _region = _IRegionRepository.GetRegionByRegionID(region.RegionID, new Guid(Session["OrganizationGUID"].ToString()));
                        //if (_region == null)
                        //{
                        Region Region = new Region();
                        Region.RegionGUID = new Guid(region.RegionGUID);
                        Region.Name = region.Name;
                        Region.Description = region.Description;
                        Region.REGIONID = region.RegionID;
                        if (!string.IsNullOrEmpty(region.OrganizationGUID) && region.OrganizationGUID != Guid.Empty.ToString())
                        {
                            Region.OrganizationGUID = new Guid(region.OrganizationGUID);
                        }
                        else
                        {
                            Region.OrganizationGUID = null;
                        }
                        Region.IsDefault = false;
                        _IRegionRepository.UpdateRegion(Region);
                        // _IRegionRepository.Save();
                        return RedirectToAction("../MyCompany/Index/Regions");
                        //}
                        //else
                        //{
                        //    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','RegionId already exists');</script>";
                        //    return View(region);
                        //}
                    }

                    return View(region);
                }
                else
                {
                    return RedirectToAction("SessionTimeOut", "User");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return View(region);
            }
        }
        public ActionResult Delete(string id = "")
        {
            Logger.Debug("Inside Region Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    int count = _ITerritoryRepository.GetTerritoryByRegionGUID(new Guid(id)).Count();
                    if (count == 0)
                    {
                        RegionModel region = new RegionModel();
                        region.RegionGUID = id;
                        _IMarketRepository.DeleteMarketByRegionGUID(new Guid(region.RegionGUID));
                        _ITerritoryRepository.DeleteTerritoryByRegionGUID(new Guid(region.RegionGUID));
                        _IRegionRepository.DeleteRegion(new Guid(region.RegionGUID));
                        //_IRegionRepository.Save();
                        return RedirectToAction("../MyCompany/Index/Regions");
                    }
                    else
                    {
                        TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Region is associate with Territory.');</script>";
                        return RedirectToAction("../MyCompany/Index/Regions");
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
                return RedirectToAction("../MyCompany/Index/Regions");
            }
        }
    }
}
