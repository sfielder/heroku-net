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
    public class POController : BaseController
    {

        #region Constructor
        private readonly IJobRepository _IJobRepository;
        private readonly IPORepository _IPORepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IRegionRepository _IRegionRepository;
        public POController()
        {
            this._IPORepository = new PORepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
        }
        public POController(WorkersInMotionDB context)
        {
            this._IJobRepository = new JobRepository(context);
            this._IPORepository = new PORepository(context);
            this._IPlaceRepository = new PlaceRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IRegionRepository = new RegionRepository(context);

        }

        #endregion
        //
        // GET: /PO/
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

        public ActionResult Index(string customerid = "", string selection = "", string RowCount = "", int page = 1, string search = "")
        {
            Logger.Debug("Inside Job Controller- Index");
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


                    var poList = new POViewLists();
                    poList.POViewList = new List<POView>();
                    var poGroup = new List<POs>();
                    POs lpo = new POs();
                    if (!string.IsNullOrEmpty(customerid))
                    {
                        Place _place = _IPlaceRepository.GetPlaceByID(new Guid(customerid));
                        if (_place != null)
                        {
                            poGroup = _IPORepository.GetPOListByPlaceID(new Guid(Session["OrganizationGUID"].ToString()), _place.PlaceID).ToList();
                        }

                    }
                    else
                    {
                        poGroup = _IPORepository.GetPOList(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class='actions'>");
                    sb.Append("<div class='btn-group'>");
                    if (!string.IsNullOrEmpty(customerid))
                    {
                        sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> " + _IPlaceRepository.GetPlaceByID(new Guid(customerid)).PlaceName + " <i class='icon-angle-down'></i></a>");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(selection) && selection == "All")
                        {
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> All <i class='icon-angle-down'></i></a>");
                        }
                        else
                        {
                            sb.Append("<a href='#' id='ulaworkergroup' class='btn green' data-toggle='dropdown'><i class='icon-map-marker'></i> Select Client <i class='icon-angle-down'></i></a>");
                        }
                    }
                    sb.Append("<ul id='ulworkgroup' style='height:100px;overflow-y:scroll' class='dropdown-menu pull-right'>");
                    if (string.IsNullOrEmpty(selection) || selection != "All")
                    {
                        sb.Append("<li><a href=" + Url.Action("Index", "PO", new { selection = "All" }) + ">All</a></li>");
                    }
                    List<Place> placeList = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();
                    if (placeList != null)
                    {
                        foreach (Place item in placeList)
                        {
                            sb.Append("<li><a href=" + Url.Action("Index", "PO", new { customerid = item.PlaceGUID.ToString() }) + " data-groupguid=" + item.PlaceGUID + ">" + item.PlaceName + "</a></li>");
                        }
                    }
                    sb.Append("</ul>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                    ViewBag.CustomerList = sb.ToString();
                    if (poGroup != null && poGroup.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(search))
                        {
                            search = search.ToLower();
                            poGroup = poGroup.Where(x => (x.PlaceID != null ? _IPlaceRepository.GetPlaceByID(x.PlaceID, new Guid(x.OrganizationGUID.ToString())).PlaceName.ToLower() : "").Contains(search)
                                    || (!String.IsNullOrEmpty(x.PONumber) && GetJobName(x.PONumber).ToLower().Contains(search))
                                    || (!String.IsNullOrEmpty(x.PONumber) && x.PONumber.ToLower().Contains(search))
                                    || (!String.IsNullOrEmpty(x.MarketID) && x.MarketID.ToLower().Contains(search))
                                    || (!String.IsNullOrEmpty(x.InstallerName) && x.InstallerName.ToLower().Contains(search))
                                    || (!String.IsNullOrEmpty(x.POCustomerName) && x.POCustomerName.ToLower().Contains(search))
                                    || (!String.IsNullOrEmpty(x.POCustomerPhone) && x.POCustomerPhone.ToLower().Contains(search))
                                    ).ToList();


                            poList.POViewList = poList.POViewList.Where(
                                p => (p.PlaceName.ToLower().StartsWith(search))
                                || (p.PONumber.ToLower().StartsWith(search)) 
                                || (p.MarketID.ToLower().StartsWith(search)) 
                                || (p.InstallerName.ToLower().StartsWith(search)) 
                                || (p.POCustomerName.ToLower().StartsWith(search)) 
                                || (p.POCustomerPhone.ToLower().StartsWith(search))).ToList();
                        }

                        totalRecord = poGroup.ToList().Count;
                        totalPage = (totalRecord / (int)ViewBag.pageCountValue) + ((totalRecord % (int)ViewBag.pageCountValue) > 0 ? 1 : 0);

                        ViewBag.TotalRows = totalRecord;

                        poGroup = poGroup.OrderBy(a => a.OrganizationGUID).Skip(((page - 1) * (int)ViewBag.pageCountValue)).Take((int)ViewBag.pageCountValue).ToList();


                        foreach (var pos in poGroup.ToList())
                        {
                            POView Pos = new POView();
                            Pos.POGUID = pos.POGUID;
                            Pos.OrganizationGUID = pos.OrganizationGUID;
                            Pos.RegionGUID = pos.RegionGUID;
                            Pos.TerritoryGUID = pos.TerritoryGUID;
                            Pos.PONumber = pos.PONumber;
                            Pos.RegionName = pos.RegionGUID != null && pos.RegionGUID != Guid.Empty ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(pos.RegionGUID.ToString())) : "";
                            Pos.TerritoryName = pos.TerritoryGUID != null && pos.TerritoryGUID != Guid.Empty ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(pos.TerritoryGUID.ToString())) : "";
                            Pos.InstallerName = pos.InstallerName;
                            Pos.POCustomerName = pos.POCustomerName;
                            Pos.POCustomerPhone = pos.POCustomerPhone;
                            Pos.PoCustomerMobile = pos.POCustomerMobile;
                            if (!string.IsNullOrEmpty(pos.MarketID) && !string.IsNullOrEmpty(pos.PlaceID))
                            {
                                Market _market = _IMarketRepository.GetMarketByCustomerID(new Guid(pos.OrganizationGUID.ToString()), pos.PlaceID, pos.MarketID);
                                if (_market != null)
                                {
                                    Pos.StoreName = _market.MarketName;
                                }
                                else
                                {
                                    Pos.StoreName = "";
                                }
                            }
                            else
                            {
                                Pos.StoreName = "";
                            }
                            if (!string.IsNullOrEmpty(pos.PlaceID))
                            {
                                Place _place = _IPlaceRepository.GetPlaceByID(pos.PlaceID, new Guid(pos.OrganizationGUID.ToString()));
                                if (_place != null)
                                {
                                    Pos.PlaceName = _place.PlaceName;
                                }
                                else
                                {
                                    Pos.PlaceName = "";
                                }
                            }
                            else
                            {
                                Pos.PlaceName = "";
                            }
                            Pos.EmailID = !string.IsNullOrEmpty(pos.MarketID) ? GetEmails(pos.OrganizationGUID, pos.MarketID, pos.PlaceID) : "";
                            Pos.JobName = GetJobName(pos.PONumber);
                            Pos.Status = pos.Status;
                            Pos.Description = pos.Description;
                            Pos.PlaceID = pos.PlaceID;
                            Pos.LocationType = pos.LocationType;
                            Pos.MarketID = pos.MarketID;
                            Pos.EndCustomerAddress = pos.EndCustomerAddress;
                            Pos.EndCustomerName = pos.EndCustomerName;
                            Pos.EndCustomerPhone = pos.EndCustomerPhone;
                            Pos.WorkerName = pos.WorkerName;
                            Pos.CustomBooleanValue = pos.CustomBooleanValue;
                            Pos.JobClass = pos.JobClass;
                            Pos.OrderDate = pos.OrderDate.ToString();
                            Pos.PreferredDateTime = pos.PreferredDateTime.ToString();
                            Pos.EstimatedCost = pos.EstimatedCost;
                            Pos.CreateDate = pos.CreateDate.ToString();
                            Pos.CreateBy = pos.CreateBy;
                            Pos.LastModifiedDate = pos.LastModifiedDate.ToString();
                            Pos.LastModifiedBy = pos.LastModifiedBy;
                            poList.POViewList.Add(Pos);

                        }
                    }                    

                    return View(poList);
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
        private string GetEmails(Nullable<System.Guid> OrganizationGUID, string markerID, string PlaceID)
        {
            string email = "";

            if (OrganizationGUID != null && !string.IsNullOrEmpty(markerID))
            {
                Market _market = _IMarketRepository.GetMarketByCustomerID(new Guid(OrganizationGUID.ToString()), PlaceID, markerID);
                if (_market != null)
                    email = _market.Emails;
                else
                    email = "";
            }
            return email;
        }
        private string GetJobName(string PoNumber)
        {
            string jobname = "";
            if (!string.IsNullOrEmpty(PoNumber))
            {
                Job job = _IJobRepository.GetJobByPONumber(PoNumber);
                if (job != null)
                    jobname = job.JobName;
                else
                    jobname = "";
            }
            return jobname;
        }
    }
}