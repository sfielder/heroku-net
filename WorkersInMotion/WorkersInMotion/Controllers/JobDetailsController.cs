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
    public class JobDetailsController : BaseController
    {

        #region Constructor
        private readonly IJobRepository _IJobRepository;
        private readonly IJobSchemaRepository _IJobSchemaRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IRegionRepository _IRegionRepository;
        private readonly IUserRepository _IUserRepository;
        public JobDetailsController()
        {
            this._IJobRepository = new JobRepository(new WorkersInMotionDB());
            this._IJobSchemaRepository = new JobSchemaRepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
        }

        public JobDetailsController(WorkersInMotionDB context)
        {
            this._IPlaceRepository = new PlaceRepository(context);
            this._IMarketRepository = new MarketRepository(context);
            this._IRegionRepository = new RegionRepository(context);
            this._ITerritoryRepository = new TerritoryRepository(context);
            this._IJobRepository = new JobRepository(context);
            this._IJobSchemaRepository = new JobSchemaRepository(context);
            this._IUserRepository = new UserRepository(context);
        }

        #endregion
        //

        public void DropdownValues()
        {
            var JobSchema = _IJobSchemaRepository.GetJobSchema(new Guid(Session["OrganizationGUID"].ToString())).ToList().Select(r => new SelectListItem
            {
                Value = r.JobFormGUID.ToString(),
                Text = r.FriendlyName
            });
            ViewBag.JobSchemaName = new SelectList(JobSchema, "Value", "Text");

            var listItems = new List<ListItem> { new ListItem { Text = "One Time", Value = "true" }, new ListItem { Text = "Any Time", Value = "false" } };

            ViewBag.Schedule = new SelectList(listItems, "Value", "Text");
        }
        // GET: /JobDetails/
        public ActionResult Index(string id = "", string marketguid = "", string jobindexguid = "")
        {
            Logger.Debug("Inside People Controller- Create");
            try
            {

                if (Session["OrganizationGUID"] != null)
                {
                    if (!string.IsNullOrEmpty(jobindexguid))
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            TempData["TabName"] = id;
                        }
                        else
                        {
                            TempData["TabName"] = "Details";
                        }
                        DropdownValues();
                        Job _job = _IJobRepository.GetJobByID(new Guid(jobindexguid));

                        JobModel job = new JobModel();
                        job.JobIndexGUID = _job.JobGUID;
                        //   job.JobLogicalID = _IJobSchemaRepository.GetJobFormIDfromJobForm(_job.JobForm.ToString());
                        job.JobReferenceNo = _job.JobReferenceNo;
                        job.JobName = _job.JobName;
                        if (_job.CustomerGUID != null && !string.IsNullOrEmpty(_job.CustomerGUID.ToString()))
                        {
                            job.CustGUID = new Guid(_job.CustomerGUID.ToString());
                        }
                        else
                        {
                            job.CustGUID = Guid.Empty;
                        }
                        job.IsScheduled = _job.IsSecheduled == true ? "true" : "false";

                        double duration;
                        if (double.TryParse(_job.EstimatedDuration.ToString(), out duration))
                        {
                            TimeSpan time = new TimeSpan();
                            time = TimeSpan.FromSeconds(duration);
                            //timeformat = time.Hours + " : " + time.Minutes + " : " + time.Seconds;
                            job.Duration = time.Hours + " : " + time.Minutes;
                            job.EstimatedDuration = duration;
                            //job.EstimatedDuration = duration / 3600;
                        }
                        else
                        {
                            job.EstimatedDuration = 0;
                        }
                        job.PreferredStartTime = _job.PreferedStartTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(_job.PreferedStartTime, Session["TimeZoneID"].ToString()) : _job.PreferedStartTime.ToString() : "";
                        job.PreferredStartTime = !string.IsNullOrEmpty(job.PreferredStartTime) ? _IUserRepository.GetLocalDateTime(_job.PreferedStartTime, Session["TimeZoneID"].ToString()) : "";
                        job.PreferredStartTime = !string.IsNullOrEmpty(job.PreferredStartTime) ? Convert.ToDateTime(job.PreferredStartTime).ToString("MM/dd/yy HH:mm") : "";


                        job.PreferredEndTime = _job.PreferedEndTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(_job.PreferedEndTime, Session["TimeZoneID"].ToString()) : _job.PreferedEndTime.ToString() : "";
                        job.PreferredEndTime = !string.IsNullOrEmpty(job.PreferredEndTime) ? _IUserRepository.GetLocalDateTime(_job.PreferedEndTime, Session["TimeZoneID"].ToString()) : "";
                        job.PreferredEndTime = !string.IsNullOrEmpty(job.PreferredEndTime) ? Convert.ToDateTime(job.PreferredEndTime).ToString("MM/dd/yy HH:mm") : "";

                        job.ActualStartTime = _job.ActualStartTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(_job.ActualStartTime, Session["TimeZoneID"].ToString()) : _job.ActualStartTime.ToString() : "";
                        job.ActualStartTime = !string.IsNullOrEmpty(job.ActualStartTime) ? _IUserRepository.GetLocalDateTime(_job.ActualStartTime, Session["TimeZoneID"].ToString()) : "";
                        job.ActualStartTime = !string.IsNullOrEmpty(job.ActualStartTime) ? Convert.ToDateTime(job.ActualStartTime).ToString("MM/dd/yy HH:mm") : "";

                        job.ActualEndTime = _job.PreferedEndTime != null ? Session["TimeZoneID"] != null ? _IUserRepository.GetLocalDateTime(_job.ActualEndTime, Session["TimeZoneID"].ToString()) : _job.ActualEndTime.ToString() : "";
                        job.ActualEndTime = !string.IsNullOrEmpty(job.ActualEndTime) ? _IUserRepository.GetLocalDateTime(_job.ActualEndTime, Session["TimeZoneID"].ToString()) : "";
                        job.ActualEndTime = !string.IsNullOrEmpty(job.ActualEndTime) ? Convert.ToDateTime(job.ActualEndTime).ToString("MM/dd/yy HH:mm") : "";


                        //job.PreferredStartTime = _IUserRepository.GetLocalDateTime(_job.PreferedStartTime, Session["TimeZoneID"].ToString());
                        //job.PreferredStartTime = !string.IsNullOrEmpty(job.PreferredStartTime) ? Convert.ToDateTime(job.PreferredStartTime).ToString("MM/dd/yyyy HH:mm") : "";
                        //job.PreferredEndTime = _IUserRepository.GetLocalDateTime(_job.PreferedEndTime, Session["TimeZoneID"].ToString());
                        //job.PreferredEndTime = !string.IsNullOrEmpty(job.PreferredEndTime) ? Convert.ToDateTime(job.PreferredEndTime).ToString("MM/dd/yyyy HH:mm") : "";

                        //job.PreferredEndTime = Convert.ToDateTime(_job.PreferedEndTime).ToString("yyyy/MM/dd HH:mm");

                        if (_job.RegionGUID != null)
                            job.RegionCode = new Guid(_job.RegionGUID.ToString());
                        else
                            job.RegionCode = Guid.Empty;
                        if (_job.TerritoryGUID != null)
                            job.TerritoryCode = new Guid(_job.TerritoryGUID.ToString());
                        else
                            job.TerritoryCode = Guid.Empty;
                        if (_job.CustomerStopGUID != null && !string.IsNullOrEmpty(_job.CustomerStopGUID.ToString()))
                        {
                            job.StopsGUID = new Guid(_job.CustomerStopGUID.ToString());
                        }
                        else
                        {
                            job.StopsGUID = Guid.Empty;
                        }
                        job.CustomerName = job.CustGUID != Guid.Empty ? _IJobRepository.GetCustomerName(job.CustGUID) : "";
                        job.CreateDate = _job.CreateDate;
                        int StatusCode; ;
                        if (int.TryParse(_job.StatusCode.ToString(), out StatusCode))
                        {
                            job.Status = StatusCode;
                        }
                        else
                        {
                            job.Status = 0;
                        }

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
                                OrganizationGUID = place.OrganizationGUID.ToString(),
                            });
                        }

                        var marketList = new MarketViewModel();
                        marketList.MarketList = new List<MarketModel>();
                        var appMarket = new List<Market>();
                        if (string.IsNullOrEmpty(marketguid) || Guid.Empty == new Guid(marketguid))
                        {
                            // if (Session["UserType"].ToString() == "ENT_A")
                            {
                                appMarket = _IMarketRepository.GetMarketByOrganizationGUID(new Guid(Session["OrganizationGUID"].ToString()), 1).ToList();
                            }
                            //else
                            //{
                            //    appMarket = _IMarketRepository.GetMarketByUserGUID(new Guid(Session["UserGUID"].ToString()), 1).ToList();
                            //}
                            if (!string.IsNullOrEmpty(marketguid) && Guid.Empty == new Guid(marketguid))
                                ViewBag.PlaceGUID = Guid.Empty.ToString();
                        }
                        else
                        {
                            appMarket = _IMarketRepository.GetMarketByOwnerGUID(new Guid(marketguid)).ToList();
                            ViewBag.PlaceGUID = marketguid;
                        }
                        foreach (var market in appMarket.ToList())
                        {
                            marketList.MarketList.Add(new MarketModel
                            {
                                MarketGUID = market.MarketGUID.ToString(),
                                UserGUID = market.UserGUID.ToString(),
                                OrganizationGUID = market.OrganizationGUID.ToString(),
                                OwnerGUID = market.OwnerGUID.ToString(),
                                MarketName = market.MarketName,
                                MarketPhone = market.MarketPhone,
                                PrimaryContactGUID = market.PrimaryContactGUID.ToString(),
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
                        List<JobProgressViewModel> pJobProgressViewModel = new List<JobProgressViewModel>();
                        pJobProgressViewModel = getJobProgress(jobindexguid);
                        var viewModel = new JobViewModel();
                        viewModel.Place = placeList.PlaceList;
                        viewModel.Market = marketList.MarketList;
                        viewModel.JobModel = job;
                        viewModel.JobProgressList = pJobProgressViewModel;
                        return View(viewModel);
                    }
                    else
                    {
                        return RedirectToAction("Index", "JobDetails", new { id = "Details" });
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
        private List<JobProgressViewModel> getJobProgress(string jobindexguid)
        {

            List<JobProgress> lJobProgress = _IJobRepository.GetJobProgress(new Guid(jobindexguid));
            List<JobProgressViewModel> pJobProgressViewModel = new List<JobProgressViewModel>();
            if (lJobProgress != null && lJobProgress.Count > 0)
            {
                lJobProgress = lJobProgress.OrderBy(x => x.JobStatus).ToList();
                foreach (var jobprogress in lJobProgress.ToList())
                {
                    double pduration;
                    string timeformat = string.Empty;
                    TimeSpan time = new TimeSpan();
                    if (double.TryParse(jobprogress.Duration.ToString(), out pduration))
                    {
                        time = TimeSpan.FromSeconds(pduration);
                        //timeformat = time.Hours + " : " + time.Minutes + " : " + time.Seconds;
                        timeformat = time.Hours + " : " + time.Minutes;
                    }
                    else
                    {
                        timeformat = "0";
                    }

                    pJobProgressViewModel.Add(new JobProgressViewModel
                    {
                        JobProgressGUID = jobprogress.JobProgressGUID,
                        JobGUID = jobprogress.JobGUID,
                        JobStatus = jobprogress.JobStatus,
                        JobSubStatus = jobprogress.JobSubStatus,
                        StatusNote = jobprogress.StatusNote,
                        StartTime = _IUserRepository.GetLocalDateTime_DateReturnType(jobprogress.StartTime, Session["TimeZoneID"].ToString()),
                        LocationMismatch = jobprogress.LocationMismatch,
                        DurationInHourFormat = timeformat,
                        Duration = jobprogress.Duration,
                        Latitude = jobprogress.Latitude,
                        Longitude = jobprogress.Longitude,
                        LastModifiedDate = jobprogress.LastModifiedDate,
                        LastModifiedBy = jobprogress.LastModifiedBy,
                        Status = _IJobRepository.GetStatusName(jobprogress.JobStatus != null ? Convert.ToInt32(jobprogress.JobStatus) : 0)
                    });
                }
            }
            return pJobProgressViewModel;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditJobDetails(JobViewModel _jobdetails)
        {
            Logger.Debug("Inside People Controller- Create");
            try
            {
                if (Session["OrganizationGUID"] != null)
                {
                    List<JobProgressViewModel> pJobProgressViewModel = new List<JobProgressViewModel>();

                    TempData["TabName"] = "Details";
                    DropdownValues();
                    JobViewModel _job = _jobdetails;
                    if (ModelState.IsValid)
                    {
                        Job job = new Job();
                        job.JobGUID = new Guid(_job.JobModel.JobIndexGUID.ToString());
                        Int16 jobclass;

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
                        job.ActualStartTime = Convert.ToDateTime(_job.JobModel.ActualStartTime);
                        job.ActualEndTime = Convert.ToDateTime(_job.JobModel.ActualEndTime);


                        //  job.JobForm = _IJobSchemaRepository.GetJobSchemabyJobFormID(_job.JobModel.JobLogicalID).JobForm1;
                        job.CreateDate = DateTime.UtcNow;
                        job.CreateBy = new Guid(Session["UserGUID"].ToString());
                        job.LastModifiedDate = DateTime.UtcNow;
                        job.LastModifiedBy = new Guid(Session["UserGUID"].ToString());

                        int result = _IJobRepository.UpdateJob(job);
                        //int result = _IJobRepository.Save();
                        if (result > 0)
                        {
                            // return RedirectToAction("Index", "JobDetails", new { jobindexguid = _job.JobModel.JobIndexGUID.ToString() });
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
                                job1.ActualStartTime = Convert.ToDateTime(_job1.ActualStartTime).ToString("MM/dd/yy HH:mm");
                                job1.ActualEndTime = Convert.ToDateTime(_job1.ActualEndTime).ToString("MM/dd/yy HH:mm");
                                job1.PreferredStartTime = Convert.ToDateTime(_job1.PreferedStartTime).ToString("MM/dd/yy HH:mm");
                                job1.PreferredEndTime = Convert.ToDateTime(_job1.PreferedEndTime).ToString("MM/dd/yy HH:mm");
                                job1.RegionCode = _job1.RegionGUID != null ? new Guid(_job1.RegionGUID.ToString()) : Guid.Empty;
                                job1.TerritoryCode = _job1.TerritoryGUID != null ? new Guid(_job1.TerritoryGUID.ToString()) : Guid.Empty;
                                job1.StopsGUID = _job1.CustomerStopGUID != null ? new Guid(_job1.CustomerStopGUID.ToString()) : Guid.Empty;
                                job1.CustomerName = _job1.CustomerGUID != null ? _IJobRepository.GetCustomerName(new Guid(_job1.CustomerGUID.ToString())) : "";
                                job1.CreateDate = _job1.CreateDate;
                                int StatusCode; ;
                                if (int.TryParse(_job1.StatusCode.ToString(), out StatusCode))
                                {
                                    job1.Status = StatusCode;
                                }
                                else
                                {
                                    job1.Status = 0;
                                }

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
                                pJobProgressViewModel = getJobProgress(_job.JobModel.JobIndexGUID.ToString());
                                var viewModel = new JobViewModel();
                                viewModel.Place = placeList.PlaceList;
                                viewModel.Market = marketList.MarketList;
                                viewModel.JobProgressList = pJobProgressViewModel;
                                viewModel.JobModel = job1;
                                return View("Index", viewModel);
                                //return RedirectToAction("Index", "JobDetails", new { jobindexguid = _job.JobModel.JobIndexGUID.ToString() });
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
                            job1.ActualStartTime = Convert.ToDateTime(_job1.ActualStartTime).ToString("MM/dd/yy HH:mm");
                            job1.ActualEndTime = Convert.ToDateTime(_job1.ActualEndTime).ToString("MM/dd/yy HH:mm");
                            job1.PreferredStartTime = Convert.ToDateTime(_job1.PreferedStartTime).ToString("MM/dd/yy HH:mm");
                            job1.PreferredEndTime = Convert.ToDateTime(_job1.PreferedEndTime).ToString("MM/dd/yy HH:mm");
                            job1.RegionCode = _job1.RegionGUID != null ? new Guid(_job1.RegionGUID.ToString()) : Guid.Empty;
                            job1.TerritoryCode = _job1.TerritoryGUID != null ? new Guid(_job1.TerritoryGUID.ToString()) : Guid.Empty;
                            job1.StopsGUID = _job1.CustomerStopGUID != null ? new Guid(_job1.CustomerStopGUID.ToString()) : Guid.Empty;
                            job1.CustomerName = _job1.CustomerGUID != null ? _IJobRepository.GetCustomerName(new Guid(_job1.CustomerGUID.ToString())) : "";
                            job1.CreateDate = _job1.CreateDate;

                            int StatusCode; ;
                            if (int.TryParse(_job1.StatusCode.ToString(), out StatusCode))
                            {
                                job1.Status = StatusCode;
                            }
                            else
                            {
                                job1.Status = 0;
                            }
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
                            pJobProgressViewModel = getJobProgress(_job.JobModel.JobIndexGUID.ToString());
                            var viewModel = new JobViewModel();
                            viewModel.Place = placeList.PlaceList;
                            viewModel.Market = marketList.MarketList;
                            viewModel.JobProgressList = pJobProgressViewModel;
                            viewModel.JobModel = job1;

                            return View("Index", viewModel);
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
    }
}