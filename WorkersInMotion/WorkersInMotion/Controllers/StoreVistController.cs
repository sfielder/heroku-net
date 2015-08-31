using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.Model;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.DataAccess.Model;

namespace WorkersInMotion.Controllers
{
    public class StoreVistController : BaseController
    {

        #region Constructor
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPeopleRepository _IPeopleRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IJobRepository _IJobRepository;
        public StoreVistController()
        {
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
        }
        public StoreVistController(WorkersInMotionDB context)
        {
            this._IPlaceRepository = new PlaceRepository(context);
            this._IPeopleRepository = new PeopleRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IUserProfileRepository = new UserProfileRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IJobRepository = new JobRepository(context);
        }

        #endregion
        //
        // GET: /StoreVist/
        public ActionResult Index(string id = "", string customerid = "")
        {

            Logger.Debug("Inside CustomerView Controller- Index");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
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

                    return View(pStoreVisitReports);
                }
                else
                {
                    TempData["msg"] = "<script>ModalPopupsAlert('Workers-In-Motion','Session Expired');</script>";
                    return RedirectToAction("../User/Login");
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }
    }
}