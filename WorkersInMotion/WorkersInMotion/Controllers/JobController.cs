using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using WorkersInMotion.Model;
using WorkersInMotion.Model.Filter;
using WorkersInMotion.Model.ViewModel;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Model.ViewModel;

namespace WorkersInMotion.Controllers
{
    [SessionExpireFilter]
    public class JobController : BaseController
    {
        //
        // GET: /Job/

        #region Constructor
        private readonly IJobRepository _IJobRepository;
        private readonly IJobSchemaRepository _IJobSchemaRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IRegionRepository _IRegionRepository;
        public JobController()
        {
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
            this._IJobSchemaRepository = new JobSchemaRepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
        }

        public JobController(WorkersInMotionDB context)
        {
            this._IPlaceRepository = new PlaceRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IJobRepository = new JobRepository(context);
            this._IJobSchemaRepository = new JobSchemaRepository(context);
        }

        #endregion

        public void DropdownValues()
        {
            var JobSchema = _IJobSchemaRepository.GetJobSchema(new Guid(Session["OrganizationGUID"].ToString())).ToList().Select(r => new SelectListItem
            {
                Value = r.JobClass.ToString() + "," + r.JobFormGUID.ToString(),
                Text = r.FriendlyName
            });
            ViewBag.JobSchemaName = new SelectList(JobSchema, "Value", "Text");

            var listItems = new List<ListItem> { new ListItem { Text = "One Time", Value = "true" }, new ListItem { Text = "Any Time", Value = "false" } };

            ViewBag.Schedule = new SelectList(listItems, "Value", "Text");
        }
        public ActionResult Index(string FromDate = "", string ToDate = "")
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
                    if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                    {
                        ViewBag.FromDate = FromDate;
                        ViewBag.ToDate = ToDate;
                        jobGroup = _IJobRepository.GetjobByOrganizationGUIDBetweenDate(new Guid(Session["OrganizationGUID"].ToString()), Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate)).ToList();
                    }
                    else
                    {
                        ljob.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        ljob.IsDeleted = false;
                        jobGroup = _IJobRepository.GetOpenJobs(ljob).ToList();
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
                            CustomerName = _IJobRepository.GetCustomerName(new Guid(job.CustomerGUID.ToString())),
                            GroupName = "",
                            CustomerPointName = _IJobRepository.GetCustomerPointName(new Guid(job.CustomerStopGUID.ToString()))
                        });
                    }

                    return View(jobList.JobModelList.AsEnumerable());
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
            Logger.Debug("Inside People Controller- Create");
            try
            {

                if (Session["OrganizationGUID"] != null)
                {
                    var placeList = new PlaceViewModel();
                    placeList.PlaceList = new List<PlaceModel>();
                    var appPlace = new List<Place>();
                    DropdownValues();
                    appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                    placeList.PlaceList.Add(new PlaceModel
                    {
                        PlaceGUID = Guid.Empty.ToString(),
                        PlaceName = "All",
                        UserGUID = "",
                        OrganizationGUID = "",
                    });

                    foreach (var place in appPlace.ToList())
                    {
                        placeList.PlaceList.Add(new PlaceModel
                        {
                            PlaceGUID = place.PlaceGUID.ToString(),
                            PlaceID = place.PlaceID,
                            PlaceName = place.PlaceName,
                            UserGUID = place.UserGUID.ToString(),
                            OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                        });
                    }

                    var marketList = new MarketViewModel();
                    marketList.MarketList = new List<MarketModel>();
                    var appMarket = new List<Market>();
                    if (string.IsNullOrEmpty(id) || Guid.Empty == new Guid(id))
                    {
                        // if (Session["UserType"].ToString() == "ENT_A")
                        {
                            appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 1).ToList();
                        }
                        //else
                        //{
                        //    appMarket = _IMarketRepository.GetMarketByUserGUID(new Guid(Session["UserGUID"].ToString()), 1).ToList();
                        //}
                        if (!string.IsNullOrEmpty(id) && Guid.Empty == new Guid(id))
                            ViewBag.PlaceGUID = Guid.Empty.ToString();
                    }
                    else
                    {
                        appMarket = _IMarketRepository.GetMarketByOwnerGUID(new Guid(id)).ToList();
                        ViewBag.PlaceGUID = id;
                    }
                    foreach (var market in appMarket.ToList())
                    {
                        marketList.MarketList.Add(new MarketModel
                        {
                            MarketGUID = market.MarketGUID.ToString(),
                            MarketID = market.MarketID,
                            UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString(),
                            OrganizationGUID = market.OrganizationGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                            OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString(),
                            MarketName = market.MarketName,
                            MarketPhone = market.MarketPhone,
                            PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString(),
                            FirstName = market.FirstName,
                            LastName = market.LastName,
                            MobilePhone = market.MobilePhone,
                            HomePhone = market.HomePhone,
                            Emails = market.Emails,
                            AddressLine1 = market.AddressLine1,
                            AddressLine2 = market.AddressLine2,
                            City = market.City,
                            State = market.State,
                            Country = market.Country,
                            ZipCode = market.ZipCode,
                            RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString(),
                            TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString(),
                            RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "",
                            TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "",
                        });
                    }

                    var viewModel = new JobViewModel();
                    viewModel.Place = placeList.PlaceList;
                    viewModel.Market = marketList.MarketList;
                    if (TempData["JobItem"] != null)
                    {
                        viewModel.JobModel = (JobModel)TempData["JobItem"];
                    }

                    return View(viewModel);
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


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobViewModel _job)
        {
            Logger.Debug("Inside People Controller- Create");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    Int16 jobclass;
                    DropdownValues();
                    if (ModelState.IsValid)
                    {
                        Job job = new Job();
                        job.JobGUID = Guid.NewGuid();
                        if (_job.JobModel.JobClass != null && !string.IsNullOrEmpty(_job.JobModel.JobClass))
                        {
                            string[] pjobclass = _job.JobModel.JobClass.Split(',');
                            if (pjobclass.Count() > 1)
                            {
                                if (short.TryParse(pjobclass[0], out jobclass))
                                {
                                    job.JobClass = jobclass;
                                }
                                job.JobForm = _IJobSchemaRepository.GetJobSchemabyJobFormID(new Guid(pjobclass[1])).JobForm1;
                            }
                        }
                        //  job.JobID = 0;
                        job.JobReferenceNo = _job.JobModel.JobReferenceNo;
                        job.IsDeleted = false;
                        job.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        if (_job.JobModel.RegionCode != Guid.Empty)
                        {
                            job.RegionGUID = _job.JobModel.RegionCode;
                        }
                        else
                        {
                            job.RegionGUID = null;
                        }
                        if (_job.JobModel.TerritoryCode != Guid.Empty)
                        {
                            job.TerritoryGUID = _job.JobModel.TerritoryCode;
                        }
                        else
                        {
                            job.TerritoryGUID = null;
                        }
                        job.LocationType = 1;
                        if (_job.JobModel.CustGUID != Guid.Empty)
                        {
                            job.CustomerGUID = _job.JobModel.CustGUID;
                        }
                        else
                        {
                            job.CustomerGUID = null;
                        }
                        if (_job.JobModel.StopsGUID != Guid.Empty)
                        {
                            job.CustomerStopGUID = _job.JobModel.StopsGUID;
                        }
                        else
                        {
                            job.CustomerStopGUID = null;
                        }
                        if (job.CustomerStopGUID != null)
                        {
                            Market Market = _IMarketRepository.GetMarketByID(new Guid(job.CustomerStopGUID.ToString()));
                            LatLong latLong = new LatLong();
                            latLong = GetLatLngCode(Market.AddressLine1, Market.AddressLine2, Market.City, Market.State, Market.Country, Market.ZipCode);
                            job.ServiceAddress = Market.AddressLine1 + "," + Market.AddressLine2 + "," + Market.City + "," + Market.State + "," + Market.Country + "," + Market.ZipCode;
                            job.Latitude = latLong.Latitude;
                            job.Longitude = latLong.Longitude;
                        }
                        else
                        {
                            job.ServiceAddress = "";
                            job.Latitude = null;
                            job.Longitude = null;
                        }
                        job.StatusCode = 1;
                        job.JobName = _job.JobModel.JobName;
                        job.IsSecheduled = _job.JobModel.IsScheduled == "true" ? true : false;
                        job.ManagerUserGUID = new Guid(Session["UserGUID"].ToString());
                        double duration;
                        if (double.TryParse(_job.JobModel.EstimatedDuration.ToString(), out duration))
                            job.EstimatedDuration = duration * 3600;
                        else
                            job.EstimatedDuration = 0;
                        job.ScheduledStartTime = Convert.ToDateTime(_job.JobModel.PreferredStartTime);
                        job.PreferedStartTime = Convert.ToDateTime(_job.JobModel.PreferredStartTime);
                        job.PreferedEndTime = Convert.ToDateTime(_job.JobModel.PreferredEndTime);

                        job.CreateDate = DateTime.UtcNow;
                        job.CreateBy = new Guid(Session["UserGUID"].ToString());
                        job.LastModifiedDate = DateTime.UtcNow;
                        job.LastModifiedBy = new Guid(Session["UserGUID"].ToString());
                        // job.GroupCode = _IJobSchemaRepository.JobSchemaDetails(job.JobLogicalID).GroupCode;



                        //job.Instruction = _job.JobModel.Instruction;

                        int result = _IJobRepository.InsertJob(job);
                        //int result = _IJobRepository.Save();
                        if (result > 0)
                        {
                            return RedirectToAction("Index", "JobStatus");
                        }
                        else
                        {
                            if (Session["OrganizationGUID"] != null)
                            {
                                var placeList = new PlaceViewModel();
                                placeList.PlaceList = new List<PlaceModel>();
                                var appPlace = new List<Place>();
                                DropdownValues();
                                appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                                placeList.PlaceList.Add(new PlaceModel
                                {
                                    PlaceGUID = Guid.Empty.ToString(),
                                    PlaceName = "All",
                                    UserGUID = "",
                                    OrganizationGUID = "",
                                });

                                foreach (var place in appPlace.ToList())
                                {
                                    placeList.PlaceList.Add(new PlaceModel
                                    {
                                        PlaceGUID = place.PlaceGUID.ToString(),
                                        PlaceID = place.PlaceID,
                                        PlaceName = place.PlaceName,
                                        UserGUID = place.UserGUID.ToString(),
                                        OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                    });
                                }

                                var marketList = new MarketViewModel();
                                marketList.MarketList = new List<MarketModel>();
                                var appMarket = new List<Market>();
                                //if (Session["UserType"].ToString() == "ENT_A")
                                {
                                    appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 1).ToList();
                                }
                                //else
                                //{
                                //    appMarket = _IMarketRepository.GetMarketByUserGUID(new Guid(Session["UserGUID"].ToString()), 1).ToList();
                                //}

                                foreach (var market in appMarket.ToList())
                                {
                                    marketList.MarketList.Add(new MarketModel
                                    {
                                        MarketGUID = market.MarketGUID.ToString(),
                                        MarketID = market.MarketID,
                                        UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString(),
                                        OrganizationGUID = market.UserGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                        OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString(),
                                        MarketName = market.MarketName,
                                        MarketPhone = market.MarketPhone,
                                        PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString(),
                                        FirstName = market.FirstName,
                                        LastName = market.LastName,
                                        MobilePhone = market.MobilePhone,
                                        HomePhone = market.HomePhone,
                                        Emails = market.Emails,
                                        AddressLine1 = market.AddressLine1,
                                        AddressLine2 = market.AddressLine2,
                                        City = market.City,
                                        State = market.State,
                                        Country = market.Country,
                                        ZipCode = market.ZipCode,
                                        RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString(),
                                        TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString(),
                                        RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "",
                                        TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "",
                                    });
                                }

                                var viewModel = new JobViewModel();
                                viewModel.Place = placeList.PlaceList;
                                viewModel.Market = marketList.MarketList;

                                return View(viewModel);
                            }
                            else
                            {
                                return RedirectToAction("../User/Login");
                            }

                        }
                    }
                    else
                    {
                        if (Session["OrganizationGUID"] != null)
                        {
                            var placeList = new PlaceViewModel();
                            placeList.PlaceList = new List<PlaceModel>();
                            var appPlace = new List<Place>();
                            DropdownValues();
                            appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                            placeList.PlaceList.Add(new PlaceModel
                            {
                                PlaceGUID = Guid.Empty.ToString(),
                                PlaceName = "All",
                                UserGUID = "",
                                OrganizationGUID = "",
                            });

                            foreach (var place in appPlace.ToList())
                            {
                                placeList.PlaceList.Add(new PlaceModel
                                {
                                    PlaceGUID = place.PlaceGUID.ToString(),
                                    PlaceID = place.PlaceID,
                                    PlaceName = place.PlaceName,
                                    UserGUID = place.UserGUID.ToString(),
                                    OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                });
                            }

                            var marketList = new MarketViewModel();
                            marketList.MarketList = new List<MarketModel>();
                            var appMarket = new List<Market>();
                            //if (Session["UserType"].ToString() == "ENT_A")
                            {
                                appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 1).ToList();
                            }
                            //else
                            //{
                            //    appMarket = _IMarketRepository.GetMarketByUserGUID(new Guid(Session["UserGUID"].ToString()), 1).ToList();
                            //}

                            foreach (var market in appMarket.ToList())
                            {
                                marketList.MarketList.Add(new MarketModel
                                {
                                    MarketGUID = market.MarketGUID.ToString(),
                                    MarketID = market.MarketID,
                                    UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString(),
                                    OrganizationGUID = market.OrganizationGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                    OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString(),
                                    MarketName = market.MarketName,
                                    MarketPhone = market.MarketPhone,
                                    PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString(),
                                    FirstName = market.FirstName,
                                    LastName = market.LastName,
                                    MobilePhone = market.MobilePhone,
                                    HomePhone = market.HomePhone,
                                    Emails = market.Emails,
                                    AddressLine1 = market.AddressLine1,
                                    AddressLine2 = market.AddressLine2,
                                    City = market.City,
                                    State = market.State,
                                    Country = market.Country,
                                    ZipCode = market.ZipCode,
                                    RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString(),
                                    TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString(),
                                    RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "",
                                    TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "",
                                });
                            }

                            var viewModel = new JobViewModel();
                            viewModel.Place = placeList.PlaceList;
                            viewModel.Market = marketList.MarketList;

                            return View(viewModel);
                        }
                        else
                        {
                            return RedirectToAction("../User/Login");
                        }

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
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult Edit(string id = "", string jobindexguid = "")
        {
            Logger.Debug("Inside People Controller- Create");
            try
            {

                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    Job _job = _IJobRepository.GetJobByID(new Guid(jobindexguid));
                    JobModel job = new JobModel();
                    job.JobIndexGUID = _job.JobGUID;
                    //   job.JobLogicalID = _IJobSchemaRepository.GetJobFormIDfromJobForm(_job.JobForm.ToString());
                    job.JobReferenceNo = _job.JobReferenceNo;
                    job.JobName = _job.JobName;
                    job.CustGUID = _job.CustomerGUID != null ? new Guid(_job.CustomerGUID.ToString()) : Guid.Empty;
                    job.IsScheduled = _job.IsSecheduled == true ? "true" : "false";

                    double duration;
                    if (double.TryParse(_job.EstimatedDuration.ToString(), out duration))
                        job.EstimatedDuration = duration / 3600;
                    else
                        job.EstimatedDuration = 0;

                    job.PreferredStartTime = Convert.ToDateTime(_job.PreferedStartTime).ToString("yy/MM/dd HH:mm");
                    job.PreferredEndTime = Convert.ToDateTime(_job.PreferedEndTime).ToString("yy/MM/dd HH:mm");
                    job.RegionCode = _job.RegionGUID != null ? new Guid(_job.RegionGUID.ToString()) : Guid.Empty;
                    job.TerritoryCode = _job.TerritoryGUID != null ? new Guid(_job.TerritoryGUID.ToString()) : Guid.Empty;
                    job.StopsGUID = _job.CustomerStopGUID != null ? new Guid(_job.CustomerStopGUID.ToString()) : Guid.Empty;
                    job.CustomerName = job.CustGUID != Guid.Empty ? _IJobRepository.GetCustomerName(job.CustGUID) : "";
                    job.CreateDate = _job.CreateDate;

                    var placeList = new PlaceViewModel();
                    placeList.PlaceList = new List<PlaceModel>();
                    var appPlace = new List<Place>();

                    appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                    placeList.PlaceList.Add(new PlaceModel
                    {
                        PlaceGUID = Guid.Empty.ToString(),
                        PlaceName = "All",
                        UserGUID = "",
                        OrganizationGUID = "",
                    });

                    foreach (var place in appPlace.ToList())
                    {
                        placeList.PlaceList.Add(new PlaceModel
                        {
                            PlaceGUID = place.PlaceGUID.ToString(),
                            PlaceID = place.PlaceID,
                            PlaceName = place.PlaceName,
                            UserGUID = place.UserGUID.ToString(),
                            OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                        });
                    }

                    var marketList = new MarketViewModel();
                    marketList.MarketList = new List<MarketModel>();
                    var appMarket = new List<Market>();
                    if (string.IsNullOrEmpty(id) || Guid.Empty == new Guid(id))
                    {
                        // if (Session["UserType"].ToString() == "ENT_A")
                        {
                            appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 1).ToList();
                        }
                        //else
                        //{
                        //    appMarket = _IMarketRepository.GetMarketByUserGUID(new Guid(Session["UserGUID"].ToString()), 1).ToList();
                        //}
                        if (!string.IsNullOrEmpty(id) && Guid.Empty == new Guid(id))
                            ViewBag.PlaceGUID = Guid.Empty.ToString();
                    }
                    else
                    {
                        appMarket = _IMarketRepository.GetMarketByOwnerGUID(new Guid(id)).ToList();
                        ViewBag.PlaceGUID = id;
                    }
                    foreach (var market in appMarket.ToList())
                    {
                        marketList.MarketList.Add(new MarketModel
                        {
                            MarketGUID = market.MarketGUID.ToString(),
                            MarketID = market.MarketID,
                            UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString(),
                            OrganizationGUID = market.OrganizationGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                            OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString(),
                            MarketName = market.MarketName,
                            MarketPhone = market.MarketPhone,
                            PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString(),
                            FirstName = market.FirstName,
                            LastName = market.LastName,
                            MobilePhone = market.MobilePhone,
                            HomePhone = market.HomePhone,
                            Emails = market.Emails,
                            AddressLine1 = market.AddressLine1,
                            AddressLine2 = market.AddressLine2,
                            City = market.City,
                            State = market.State,
                            Country = market.Country,
                            ZipCode = market.ZipCode,
                            RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString(),
                            TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString(),
                            RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "",
                            TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "",
                        });
                    }

                    var viewModel = new JobViewModel();
                    viewModel.Place = placeList.PlaceList;
                    viewModel.Market = marketList.MarketList;
                    viewModel.JobModel = job;

                    return View(viewModel);
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


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobViewModel _job)
        {
            Logger.Debug("Inside People Controller- Create");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    DropdownValues();
                    if (ModelState.IsValid)
                    {
                        Job job = new Job();
                        Int16 jobclass;
                        job.JobGUID = new Guid(_job.JobModel.JobIndexGUID.ToString());

                        if (_job.JobModel.JobClass != null && !string.IsNullOrEmpty(_job.JobModel.JobClass))
                        {
                            string[] pjobclass = _job.JobModel.JobClass.Split(',');
                            if (pjobclass.Count() > 1)
                            {
                                if (short.TryParse(pjobclass[0], out jobclass))
                                {
                                    job.JobClass = jobclass;
                                }
                                job.JobForm = _IJobSchemaRepository.GetJobSchemabyJobFormID(new Guid(pjobclass[1])).JobForm1;
                            }
                        }
                        //job.JobFormGUID = _job.JobModel.JobLogicalID;
                        //  job.JobID = 0;
                        job.JobReferenceNo = _job.JobModel.JobReferenceNo;
                        job.IsDeleted = false;
                        job.OrganizationGUID = new Guid(Session["OrganizationGUID"].ToString());
                        if (_job.JobModel.RegionCode != Guid.Empty)
                        {
                            job.RegionGUID = _job.JobModel.RegionCode;
                        }
                        else
                        {
                            job.RegionGUID = null;
                        }
                        if (_job.JobModel.TerritoryCode != Guid.Empty)
                        {
                            job.TerritoryGUID = _job.JobModel.TerritoryCode;
                        }
                        else
                        {
                            job.TerritoryGUID = null;
                        }
                        job.LocationType = 1;
                        if (_job.JobModel.CustGUID != Guid.Empty)
                        {
                            job.CustomerGUID = _job.JobModel.CustGUID;
                        }
                        else
                        {
                            job.CustomerGUID = null;
                        }
                        if (_job.JobModel.StopsGUID != Guid.Empty)
                        {
                            job.CustomerStopGUID = _job.JobModel.StopsGUID;
                        }
                        else
                        {
                            job.CustomerStopGUID = null;
                        }
                        if (job.CustomerStopGUID != null)
                        {
                            Market Market = _IMarketRepository.GetMarketByID(new Guid(job.CustomerStopGUID.ToString()));
                            LatLong latLong = new LatLong();
                            latLong = GetLatLngCode(Market.AddressLine1, Market.AddressLine2, Market.City, Market.State, Market.Country, Market.ZipCode);
                            job.ServiceAddress = Market.AddressLine1 + "," + Market.AddressLine2 + "," + Market.City + "," + Market.State + "," + Market.Country + "," + Market.ZipCode;
                            job.Latitude = latLong.Latitude;
                            job.Longitude = latLong.Longitude;
                        }
                        else
                        {
                            job.ServiceAddress = "";
                            job.Latitude = null;
                            job.Longitude = null;
                        }
                        job.StatusCode = 1;
                        job.JobName = _job.JobModel.JobName;
                        job.IsSecheduled = _job.JobModel.IsScheduled == "true" ? true : false;
                        job.ManagerUserGUID = new Guid(Session["UserGUID"].ToString());


                        double duration;
                        if (double.TryParse(_job.JobModel.EstimatedDuration.ToString(), out duration))
                            job.EstimatedDuration = duration * 3600;
                        else
                            job.EstimatedDuration = 0;

                        job.ScheduledStartTime = Convert.ToDateTime(_job.JobModel.PreferredStartTime);
                        job.PreferedStartTime = Convert.ToDateTime(_job.JobModel.PreferredStartTime);
                        job.PreferedEndTime = Convert.ToDateTime(_job.JobModel.PreferredEndTime);

                        job.CreateDate = DateTime.UtcNow;
                        job.CreateBy = new Guid(Session["UserGUID"].ToString());
                        job.LastModifiedDate = DateTime.UtcNow;
                        job.LastModifiedBy = new Guid(Session["UserGUID"].ToString());

                        int result = _IJobRepository.UpdateJob(job);
                        //int result = _IJobRepository.Save();
                        if (result > 0)
                        {
                            return RedirectToAction("Index", "JobStatus");
                        }
                        else
                        {
                            if (Session["OrganizationGUID"] != null)
                            {
                                Job _job1 = _IJobRepository.GetJobByID(new Guid(_job.JobModel.JobIndexGUID.ToString()));
                                JobModel job1 = new JobModel();
                                job1.JobIndexGUID = _job1.JobGUID;
                                //     job1.JobLogicalID = _IJobSchemaRepository.GetJobFormIDfromJobForm(_job1.JobForm);
                                job1.JobReferenceNo = _job1.JobReferenceNo;
                                job1.JobName = _job1.JobName;
                                job1.CustGUID = _job1.CustomerGUID != null ? new Guid(_job1.CustomerGUID.ToString()) : Guid.Empty;
                                job1.IsScheduled = _job1.IsSecheduled == true ? "true" : "false";
                                job1.EstimatedDuration = _job1.EstimatedDuration;
                                job1.PreferredStartTime = Convert.ToDateTime(_job1.PreferedStartTime).ToString("yy/MM/dd HH:mm");
                                job1.PreferredEndTime = Convert.ToDateTime(_job1.PreferedEndTime).ToString("yy/MM/dd HH:mm");
                                job1.RegionCode = _job1.RegionGUID != null ? new Guid(_job1.RegionGUID.ToString()) : Guid.Empty;
                                job1.TerritoryCode = _job1.TerritoryGUID != null ? new Guid(_job1.TerritoryGUID.ToString()) : Guid.Empty;
                                job1.StopsGUID = _job1.CustomerStopGUID != null ? new Guid(_job1.CustomerStopGUID.ToString()) : Guid.Empty;
                                job1.CustomerName = _job1.CustomerGUID != null ? _IJobRepository.GetCustomerName(new Guid(_job1.CustomerGUID.ToString())) : "";
                                job1.CreateDate = _job1.CreateDate;


                                var placeList = new PlaceViewModel();
                                placeList.PlaceList = new List<PlaceModel>();
                                var appPlace = new List<Place>();
                                DropdownValues();
                                appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                                placeList.PlaceList.Add(new PlaceModel
                                {
                                    PlaceGUID = Guid.Empty.ToString(),
                                    PlaceName = "All",
                                    UserGUID = "",
                                    OrganizationGUID = "",
                                });

                                foreach (var place in appPlace.ToList())
                                {
                                    placeList.PlaceList.Add(new PlaceModel
                                    {
                                        PlaceGUID = place.PlaceGUID.ToString(),
                                        PlaceID = place.PlaceID,
                                        PlaceName = place.PlaceName,
                                        UserGUID = place.UserGUID.ToString(),
                                        OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                    });
                                }

                                var marketList = new MarketViewModel();
                                marketList.MarketList = new List<MarketModel>();
                                var appMarket = new List<Market>();
                                // if (Session["UserType"].ToString() == "ENT_A")
                                {
                                    appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 1).ToList();
                                }
                                //else
                                //{
                                //    appMarket = _IMarketRepository.GetMarketByUserGUID(new Guid(Session["UserGUID"].ToString()), 1).ToList();
                                //}

                                foreach (var market in appMarket.ToList())
                                {
                                    marketList.MarketList.Add(new MarketModel
                                    {
                                        MarketGUID = market.MarketGUID.ToString(),
                                        MarketID = market.MarketID,
                                        UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString(),
                                        OrganizationGUID = market.OrganizationGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                        OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString(),
                                        MarketName = market.MarketName,
                                        MarketPhone = market.MarketPhone,
                                        PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString(),
                                        FirstName = market.FirstName,
                                        LastName = market.LastName,
                                        MobilePhone = market.MobilePhone,
                                        HomePhone = market.HomePhone,
                                        Emails = market.Emails,
                                        AddressLine1 = market.AddressLine1,
                                        AddressLine2 = market.AddressLine2,
                                        City = market.City,
                                        State = market.State,
                                        Country = market.Country,
                                        ZipCode = market.ZipCode,
                                        RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString(),
                                        TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString(),
                                        RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "",
                                        TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "",
                                    });
                                }

                                var viewModel = new JobViewModel();
                                viewModel.Place = placeList.PlaceList;
                                viewModel.Market = marketList.MarketList;

                                return View(viewModel);
                            }
                            else
                            {
                                return RedirectToAction("../User/Login");
                            }

                        }
                    }
                    else
                    {
                        if (Session["OrganizationGUID"] != null)
                        {
                            Job _job1 = _IJobRepository.GetJobByID(new Guid(_job.JobModel.JobIndexGUID.ToString()));
                            JobModel job1 = new JobModel();
                            job1.JobIndexGUID = _job1.JobGUID;
                            //  job1.JobLogicalID = _IJobSchemaRepository.GetJobFormIDfromJobForm(_job1.JobForm);

                            job1.JobReferenceNo = _job1.JobReferenceNo;
                            job1.JobName = _job1.JobName;
                            job1.CustGUID = _job1.CustomerGUID != null ? new Guid(_job1.CustomerGUID.ToString()) : Guid.Empty;
                            job1.IsScheduled = _job1.IsSecheduled == true ? "true" : "false";
                            job1.EstimatedDuration = _job1.EstimatedDuration;
                            job1.PreferredStartTime = Convert.ToDateTime(_job1.PreferedStartTime).ToString("yy/MM/dd HH:mm");
                            job1.PreferredEndTime = Convert.ToDateTime(_job1.PreferedEndTime).ToString("yy/MM/dd HH:mm");
                            job1.RegionCode = _job1.RegionGUID != null ? new Guid(_job1.RegionGUID.ToString()) : Guid.Empty;
                            job1.TerritoryCode = _job1.TerritoryGUID != null ? new Guid(_job1.TerritoryGUID.ToString()) : Guid.Empty;
                            job1.StopsGUID = _job1.CustomerStopGUID != null ? new Guid(_job1.CustomerStopGUID.ToString()) : Guid.Empty;
                            job1.CustomerName = _job1.CustomerGUID != null ? _IJobRepository.GetCustomerName(new Guid(_job1.CustomerGUID.ToString())) : "";
                            job1.CreateDate = _job1.CreateDate;


                            var placeList = new PlaceViewModel();
                            placeList.PlaceList = new List<PlaceModel>();
                            var appPlace = new List<Place>();
                            DropdownValues();
                            appPlace = _IPlaceRepository.GetPlaceByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString())).ToList();

                            placeList.PlaceList.Add(new PlaceModel
                            {
                                PlaceGUID = Guid.Empty.ToString(),
                                PlaceName = "All",
                                UserGUID = "",
                                OrganizationGUID = "",
                            });

                            foreach (var place in appPlace.ToList())
                            {
                                placeList.PlaceList.Add(new PlaceModel
                                {
                                    PlaceGUID = place.PlaceGUID.ToString(),
                                    PlaceID = place.PlaceID,
                                    PlaceName = place.PlaceName,
                                    UserGUID = place.UserGUID.ToString(),
                                    OrganizationGUID = place.OrganizationGUID != null ? place.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                });
                            }

                            var marketList = new MarketViewModel();
                            marketList.MarketList = new List<MarketModel>();
                            var appMarket = new List<Market>();
                            //if (Session["UserType"].ToString() == "ENT_A")
                            {
                                appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 1).ToList();
                            }
                            //else
                            //{
                            //    appMarket = _IMarketRepository.GetMarketByUserGUID(new Guid(Session["UserGUID"].ToString()), 1).ToList();
                            //}

                            foreach (var market in appMarket.ToList())
                            {
                                marketList.MarketList.Add(new MarketModel
                                {
                                    MarketGUID = market.MarketGUID.ToString(),
                                    MarketID = market.MarketID,
                                    UserGUID = market.UserGUID != null ? market.UserGUID.ToString() : Guid.Empty.ToString(),
                                    OrganizationGUID = market.OrganizationGUID != null ? market.OrganizationGUID.ToString() : Guid.Empty.ToString(),
                                    OwnerGUID = market.OwnerGUID != null ? market.OwnerGUID.ToString() : Guid.Empty.ToString(),
                                    MarketName = market.MarketName,
                                    MarketPhone = market.MarketPhone,
                                    PrimaryContactGUID = market.PrimaryContactGUID != null ? market.PrimaryContactGUID.ToString() : Guid.Empty.ToString(),
                                    FirstName = market.FirstName,
                                    LastName = market.LastName,
                                    MobilePhone = market.MobilePhone,
                                    HomePhone = market.HomePhone,
                                    Emails = market.Emails,
                                    AddressLine1 = market.AddressLine1,
                                    AddressLine2 = market.AddressLine2,
                                    City = market.City,
                                    State = market.State,
                                    Country = market.Country,
                                    ZipCode = market.ZipCode,
                                    RegionGUID = market.RegionGUID != null ? market.RegionGUID.ToString() : Guid.Empty.ToString(),
                                    TerritoryGUID = market.TerritoryGUID != null ? market.TerritoryGUID.ToString() : Guid.Empty.ToString(),
                                    RegionName = market.RegionGUID != null ? _IRegionRepository.GetRegionNameByRegionGUID(new Guid(market.RegionGUID.ToString())) : "",
                                    TerritoryName = market.TerritoryGUID != null ? _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(new Guid(market.TerritoryGUID.ToString())) : "",
                                });
                            }

                            var viewModel = new JobViewModel();
                            viewModel.Place = placeList.PlaceList;
                            viewModel.Market = marketList.MarketList;

                            return View(viewModel);
                        }
                        else
                        {
                            return RedirectToAction("../User/Login");
                        }

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
                return RedirectToAction("Login", "User");
            }
        }

        public JsonResult getEstimateDuration(string JobLogicalID)
        {
            Logger.Debug("Inside Job Controller- Get Estimate Duration");
            JsonResult result = new JsonResult();
            try
            {
                string estimateDuration = "";//_IJobSchemaRepository.JobSchemaDetails(new Guid(JobLogicalID)).EstimatedDuration.ToString();
                result.Data = estimateDuration;
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
        public LatLong GetLatLngCode(string address1, string address2, string city, string state, string country, string zipcode)
        {
            string address = address1 + " ," + address2 + " ," + city + " ," + state + " ," + country + " ," + zipcode;
            address = address.Substring(0, address.LastIndexOf(","));

            string urlAddress = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + HttpUtility.UrlEncode(address) + "&sensor=false";
            string[] returnValue = new string[2];
            string tzone = string.Empty;
            Logger.Debug("Inside Organization Controller- GetLatLngCode");
            try
            {
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(urlAddress);
                XmlNodeList objXmlNodeList = objXmlDocument.SelectNodes("/GeocodeResponse/result/geometry/location");
                foreach (XmlNode objXmlNode in objXmlNodeList)
                {
                    // GET LONGITUDE
                    returnValue[0] = objXmlNode.ChildNodes.Item(0).InnerText;

                    // GET LATITUDE
                    returnValue[1] = objXmlNode.ChildNodes.Item(1).InnerText;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                // Process an error action here if needed 
            }
            return new LatLong(Convert.ToDouble(returnValue[0]), Convert.ToDouble(returnValue[1]));

        }
        public ActionResult Delete(string id = "")
        {
            Logger.Debug("Inside Job Controller- Delete");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    _IJobRepository.DeleteJob(new Guid(id));
                    // _IJobRepository.Save();
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

        public JsonResult SaveItem(string _jobModel)
        {
            Logger.Debug("Inside Job Controller- SaveItem");
            JsonResult result = new JsonResult();
            try
            {

                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var job = js.Deserialize<dynamic>(_jobModel);
                JobModel jobModel = new JobModel();
                jobModel.JobName = job["JobName"];
                jobModel.JobReferenceNo = job["JobReferenceNo"];
                if (!String.IsNullOrEmpty(job["JobClass"]))
                    jobModel.JobClass = job["JobClass"];
                jobModel.IsScheduled = job["IsScheduled"];
                jobModel.PreferredStartTime = Convert.ToDateTime(job["PreferredStartTime"]).ToString("yy/MM/dd HH:mm");
                jobModel.PreferredEndTime = Convert.ToDateTime(job["PreferredEndTime"]).ToString("yy/MM/dd HH:mm");
                if (!String.IsNullOrEmpty(job["EstimatedDuration"]))
                    jobModel.EstimatedDuration = Convert.ToInt32(job["EstimatedDuration"]);

                jobModel.CustGUID = Guid.Empty;
                jobModel.RegionCode = Guid.Empty;
                jobModel.TerritoryCode = Guid.Empty;
                jobModel.StopsGUID = Guid.Empty;
                jobModel.CustomerName = "";
                jobModel.CreateDate = DateTime.UtcNow;
                TempData["JobItem"] = jobModel;

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
            // return Json(jobModel);
            //RedirectToAction("Create", new { id = job["ID"], jobitemmodel = jobModel });
            //Create(job["ID"], jobModel);
        }
    }
}