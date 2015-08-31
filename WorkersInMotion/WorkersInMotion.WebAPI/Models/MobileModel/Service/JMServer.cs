using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;
using System.IO;
using System.Net;
using WorkersInMotion.WebAPI.Models.MobileModel.POSModels;
using System.Configuration;
using log4net.Repository.Hierarchy;
using System.Xml;
using System.Web;
using WorkersInMotion.WebAPI.Controllers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Web.Script.Serialization;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Model.ViewModel;
using WorkersInMotion.Model.ViewModel;

namespace WorkersInMotion.WebAPI.Models.MobileModel.Service
{
    public static class DictionaryExtension
    {
        internal static TValue FindValueByKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                if (key.Equals(pair.Key)) return pair.Value;

            throw new Exception("the key is not found in the dictionary");
        }
    }
    public class JMServer : BaseAPIController, IJMServer
    {




        //private WorkersInMotionJobDB context;
        //private WorkersInMotionDB contextWIM;
        private readonly IUserProfileRepository _IUserProfileRepository;
        private readonly IGlobalUserRepository _IGlobalUserRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IMarketRepository _IMarketRepository;
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IPlaceRepository _IPlaceRepository;
        private readonly IPeopleRepository _IPeopleRepository;
        private readonly ITerritoryRepository _ITerritoryRepository;
        private readonly IRegionRepository _IRegionRepository;
        public JMServer()
        {
            //this.context = new WorkersInMotionJobDB();
            //this.contextWIM = new WorkersInMotionDB();
            this._IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
            this._IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
            this._IMarketRepository = new MarketRepository(new WorkersInMotionDB());
            this._IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
            this._IPlaceRepository = new PlaceRepository(new WorkersInMotionDB());
            this._IPeopleRepository = new PeopleRepository(new WorkersInMotionDB());
            this._ITerritoryRepository = new TerritoryRepository(new WorkersInMotionDB());
            this._IRegionRepository = new RegionRepository(new WorkersInMotionDB());
        }
        public bool ValidateUser(string SessionGUID)
        {
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            return (_IUserRepository.ValidateUser(SessionGUID));
        }
        public string convertdate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") == "0001-01-01T00:00:00Z" ? "" : date.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
        public System.Guid GetUserGUID(string SessionGUID)
        {
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            return (_IUserRepository.GetUserGUID(SessionGUID));
        }

        public JMResponse GetJobs(Guid UserGUID)
        {
            JMResponse lresponse = new JMResponse();
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            Job lJob = new Job();
            lJob.AssignedUserGUID = UserGUID;
            lJob.OrganizationGUID = _IOrganizationRepository.GetOrganizationIDByUserGUID(UserGUID);
            lJob.IsDeleted = false;
            List<Job> lJobs = _IJobRepository.GetJobs(lJob);

            lresponse.Jobs = new List<MobileJob>();
            if (lJobs != null)
            {
                foreach (Job lJobRecord in lJobs)
                {
                    MobileJob _mobilejob = new MobileJob();
                    _mobilejob.JobGUID = lJobRecord.JobGUID;
                    _mobilejob.JobName = lJobRecord.JobName;
                    _mobilejob.JobReferenceNo = lJob.JobReferenceNo;

                    _mobilejob.LocationType = lJob.LocationType;
                    _mobilejob.CustomerStopGUID = lJob.CustomerStopGUID;
                    _mobilejob.ServicePointGUID = lJob.ServicePointGUID;
                    _mobilejob.RegionGUID = lJob.RegionGUID;
                    _mobilejob.TerritoryGUID = lJob.TerritoryGUID;
                    //switch (lJob.LocationType)
                    //{
                    //    case 0:
                    //        break;
                    //    case 1:
                    //        break;
                    //    default:
                    //        break;
                    //}
                    Place Customer = _IPlaceRepository.GetPlaceByID(new Guid(lJobRecord.CustomerGUID.ToString()));
                    if (Customer != null)
                    {
                        _mobilejob.CustomerCompanyName = Customer.PlaceName;
                        _mobilejob.CustomerContactName = Customer.FirstName;
                        _mobilejob.CustomerAddress = Customer.AddressLine1 + "," + Customer.AddressLine1 + "," + Customer.City + "," + Customer.State + "," + Customer.Country + "," + Customer.ZipCode;
                        _mobilejob.CustomerPhone = Customer.PlacePhone;
                        _mobilejob.CustomerLogoURL = Customer.ImageURL;
                    }

                    _mobilejob.Cost = lJobRecord.ActualCost.ToString();
                    _mobilejob.CostType = Convert.ToInt16(lJobRecord.CostType);
                    _mobilejob.PreferedStartTime = convertdate(Convert.ToDateTime(lJobRecord.PreferedStartTime));
                    _mobilejob.PreferedEndTime = convertdate(Convert.ToDateTime(lJobRecord.PreferedEndTime));
                    _mobilejob.ScheduledStartTime = convertdate(Convert.ToDateTime(lJobRecord.ScheduledStartTime));
                    _mobilejob.ScheduledEndTime = convertdate(Convert.ToDateTime(lJobRecord.ScheduledEndTime));
                    _mobilejob.ActualStartTime = convertdate(Convert.ToDateTime(lJobRecord.ActualStartTime));
                    _mobilejob.ActualEndTime = convertdate(Convert.ToDateTime(lJobRecord.ActualEndTime));
                    _mobilejob.EstimatedDuration = Convert.ToDouble(lJobRecord.EstimatedDuration);
                    _mobilejob.ActualDuration = Convert.ToDouble(lJobRecord.ActualDuration);
                    _mobilejob.StatusCode = Convert.ToInt32(lJobRecord.StatusCode);
                    _mobilejob.SubStatusCode = Convert.ToInt32(lJobRecord.SubStatusCode);
                    _mobilejob.Latitude = lJobRecord.Latitude;
                    _mobilejob.Longitude = lJobRecord.Longitude;
                    _mobilejob.CreateDate = convertdate(Convert.ToDateTime(lJobRecord.CreateDate)); //lJobRecord.CreateDate.ToString();
                    _mobilejob.Urgent = lJobRecord.IsUrgent;
                    _mobilejob.AssignedUserGUID = lJobRecord.AssignedUserGUID;
                    _mobilejob.TermsURL = lJobRecord.TermsURL;
                    _mobilejob.PictureRequired = Convert.ToInt32(lJobRecord.PictureRequired);
                    _mobilejob.SignOffRequired = Convert.ToInt32(lJobRecord.SignOffRequired);
                    _mobilejob.JobClass = Convert.ToInt16(lJobRecord.JobClass);
                    _mobilejob.PONumber = lJobRecord.PONumber;

                    //   _mobilejob.FormID = item.JobFormGUID.ToString();
                    _mobilejob.LocationSpecific = Convert.ToBoolean(lJobRecord.LocationSpecific);


                    _mobilejob.Notes = new List<Note>();
                    List<JobNote> JobNotes = _IJobRepository.GetJobNotesfromJobGUID(lJobRecord.JobGUID);
                    if (JobNotes != null)
                    {
                        foreach (JobNote itemnote in JobNotes)
                        {
                            Note _note = new Note();
                            _note.JobGUID = lJobRecord.JobGUID;
                            _note.NoteType = Convert.ToInt16(itemnote.NoteType);
                            _note.JobNoteGUID = itemnote.JobNoteGUID.ToString();
                            _note.Deletable = Convert.ToBoolean(itemnote.Deletable);
                            _note.NoteText = itemnote.NoteText;
                            _note.CreatedByName = itemnote.FileName;
                            _note.FileURL = itemnote.FileURL;
                            _note.CreatedDate = convertdate(Convert.ToDateTime(lJobRecord.CreateDate));
                            _mobilejob.Notes.Add(_note);
                        }
                    }
                    //_njob.Customer = _IPlaceRepository.ConvertPlaceforMobile(_IPlaceRepository.GetPlaceByID(new Guid(item.CustomerGUID.ToString())));
                    //_njob.CustomerStop = _IMarketRepository.ConvertMarketforMobile(_IMarketRepository.GetMarketByID(new Guid(item.CustomerStopGUID.ToString())));
                    //_njob.ServicePoint = _IMarketRepository.ConvertMarketforMobile(_IMarketRepository.GetMarketByID(new Guid(item.ServicePointGUID.ToString())));

                    lresponse.Jobs.Add(_mobilejob);
                }

            }
            return lresponse;
        }

        public JMResponse GetOpenJobs(Guid UserGUID)
        {
            JMResponse lresponse = new JMResponse();

            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            Job lJob = new Job();
            // lJob.AssignedUserGUID = UserGUID;
            lJob.OrganizationGUID = _IOrganizationRepository.GetOrganizationIDByUserGUID(UserGUID);
            lJob.IsDeleted = false;
            List<Job> _Jobs = _IJobRepository.GetOpenJobs(lJob);
            if (_Jobs != null)
            {
                lresponse.Jobs = new List<MobileJob>();
                foreach (Job item in _Jobs)
                {
                    MobileJob _mobilejob = new MobileJob();
                    _mobilejob.JobGUID = item.JobGUID;
                    _mobilejob.JobName = item.JobName;
                    _mobilejob.JobReferenceNo = item.JobReferenceNo;
                    _mobilejob.LocationType = item.LocationType;
                    _mobilejob.CustomerStopGUID = item.CustomerStopGUID;
                    _mobilejob.ServicePointGUID = item.ServicePointGUID;
                    _mobilejob.RegionGUID = item.RegionGUID;
                    _mobilejob.TerritoryGUID = item.TerritoryGUID;
                    if (item.CustomerGUID != null)
                    {
                        Place Customer = _IPlaceRepository.GetPlaceByID(new Guid(item.CustomerGUID.ToString()));
                        if (Customer != null)
                        {
                            _mobilejob.CustomerCompanyName = Customer.PlaceName;
                            _mobilejob.CustomerContactName = Customer.FirstName;
                            _mobilejob.CustomerAddress = Customer.AddressLine1 + "," + Customer.AddressLine1 + "," + Customer.City + "," + Customer.State + "," + Customer.Country + "," + Customer.ZipCode;
                            _mobilejob.CustomerPhone = Customer.PlacePhone;
                            _mobilejob.CustomerLogoURL = Customer.ImageURL;
                        }
                    }
                    else
                    {
                        _mobilejob.CustomerCompanyName = "";
                        _mobilejob.CustomerContactName = "";
                        _mobilejob.CustomerAddress = "";
                        _mobilejob.CustomerPhone = "";
                        _mobilejob.CustomerLogoURL = "";
                    }
                    _mobilejob.Cost = item.ActualCost.ToString();
                    _mobilejob.CostType = Convert.ToInt16(item.CostType);
                    _mobilejob.PreferedStartTime = convertdate(Convert.ToDateTime(item.PreferedStartTime));// item.PreferedStartTime.ToString();
                    _mobilejob.PreferedEndTime = convertdate(Convert.ToDateTime(item.PreferedEndTime));// item.PreferedEndTime.ToString();
                    _mobilejob.ScheduledStartTime = convertdate(Convert.ToDateTime(item.ScheduledStartTime)); //item.ScheduledStartTime.ToString();
                    _mobilejob.ScheduledEndTime = convertdate(Convert.ToDateTime(item.ScheduledEndTime));// item.ScheduledEndTime.ToString();
                    _mobilejob.ActualStartTime = convertdate(Convert.ToDateTime(item.ActualStartTime));// item.ActialStartTime.ToString();
                    _mobilejob.ActualEndTime = convertdate(Convert.ToDateTime(item.ActualEndTime));// item.ActualEndTime.ToString();
                    _mobilejob.EstimatedDuration = Convert.ToDouble(item.EstimatedDuration);
                    _mobilejob.ActualDuration = Convert.ToDouble(item.ActualDuration);
                    _mobilejob.StatusCode = Convert.ToInt32(item.StatusCode);
                    _mobilejob.SubStatusCode = Convert.ToInt32(item.SubStatusCode);
                    _mobilejob.Latitude = item.Latitude;
                    _mobilejob.Longitude = item.Longitude;
                    _mobilejob.CreateDate = item.CreateDate.ToString();
                    _mobilejob.Urgent = item.IsUrgent;
                    _mobilejob.AssignedUserGUID = item.AssignedUserGUID;
                    _mobilejob.TermsURL = item.TermsURL;
                    _mobilejob.PictureRequired = Convert.ToInt32(item.PictureRequired);
                    _mobilejob.SignOffRequired = Convert.ToInt32(item.SignOffRequired);
                    // _mobilejob.FormID = item.JobFormGUID.ToString();
                    if (item.JobClass != null && item.JobClass > 0)
                    {
                        _mobilejob.JobClass = Convert.ToInt16(item.JobClass);
                    }
                    _mobilejob.PONumber = item.PONumber;
                    if (item.LocationSpecific != null)
                    {
                        _mobilejob.LocationSpecific = Convert.ToBoolean(item.LocationSpecific);
                    }



                    _mobilejob.Notes = new List<Note>();
                    List<JobNote> JobNotes = _IJobRepository.GetJobNotesfromJobGUID(item.JobGUID);
                    if (JobNotes != null)
                    {
                        foreach (JobNote itemnote in JobNotes)
                        {
                            Note _note = new Note();
                            _note.JobGUID = item.JobGUID;
                            _note.NoteType = Convert.ToInt16(itemnote.NoteType);
                            _note.JobNoteGUID = itemnote.JobNoteGUID.ToString();
                            _note.Deletable = Convert.ToBoolean(itemnote.Deletable);
                            _note.NoteText = itemnote.NoteText;
                            _note.CreatedByName = itemnote.FileName;
                            _note.FileURL = itemnote.FileURL;
                            _note.CreatedDate = convertdate(Convert.ToDateTime(itemnote.CreateDate));
                            _mobilejob.Notes.Add(_note);
                        }
                    }

                    lresponse.Jobs.Add(_mobilejob);
                }

            }
            return lresponse;
        }

        public string ServerURL = ConfigurationManager.AppSettings.Get("ServerURL");

        public int UpdateJobStatus(UpdateJobStatusRequest JMOwnJobRequest, Guid UserGUID)
        {
            int result = 0;
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            Job _job = new Job();
            _job.JobGUID = JMOwnJobRequest.JobGUID;
            _job.StatusCode = JMOwnJobRequest.Status;
            _job.SubStatusCode = JMOwnJobRequest.Substatus;
            _job.ActualStartTime = JMOwnJobRequest.StartTIme;
            _job.ActualEndTime = JMOwnJobRequest.StartTIme.AddSeconds(JMOwnJobRequest.ElapsedTime);

            _job.Latitude = JMOwnJobRequest.Latitude;
            _job.Longitude = JMOwnJobRequest.Longitude;
            _job.LastModifiedDate = DateTime.UtcNow;
            _job.LastModifiedBy = UserGUID;
            _job.AssignedUserGUID = UserGUID;

            result = _IJobRepository.UpdateJobStatus(_job);
            if (result >= 0)
            {
                JobProgress _jobProgress = new JobProgress();
                _jobProgress.JobProgressGUID = Guid.NewGuid();
                _jobProgress.JobGUID = JMOwnJobRequest.JobGUID;
                _jobProgress.JobStatus = JMOwnJobRequest.Status;
                _jobProgress.JobSubStatus = JMOwnJobRequest.Substatus;
                _jobProgress.StartTime = JMOwnJobRequest.StartTIme;
                _jobProgress.Duration = JMOwnJobRequest.ElapsedTime;
                _jobProgress.Latitude = JMOwnJobRequest.Latitude;
                _jobProgress.Longitude = JMOwnJobRequest.Longitude;
                _jobProgress.LastModifiedDate = DateTime.UtcNow;
                _jobProgress.LocationMismatch = JMOwnJobRequest.LocationMismatch;
                _jobProgress.LastModifiedBy = UserGUID;
                result = _IJobRepository.InsertJobProgress(_jobProgress);
            }
            return result;
        }



        public int UploadJobs(UploadJobRequestNew pUploadJobRequest, Guid UserGUID)
        {
            int result = 0;
            DateTime lDateTime, ActualStartTime, ActualEndTime;
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());

            try
            {
                Job _job = new Job();
                //if (pUploadJobRequest.StartTIme > DateTime.MinValue)
                //    _job.ActualEndTime = Convert.ToDateTime(pUploadJobRequest.StartTIme).AddSeconds(pUploadJobRequest.ElapsedTime);
                //_job.ActualEndTime = Convert.ToDateTime(pUploadJobRequest.ActualEndTime).ToUniversalTime();
                //_job.PreferedEndTime = Convert.ToDateTime(pUploadJobRequest.PreferedEndTime).ToUniversalTime();
                //_job.ScheduledEndTime = Convert.ToDateTime(pUploadJobRequest.ScheduledEndTime).ToUniversalTime();

                //ActualStartTime
                if (!string.IsNullOrEmpty(pUploadJobRequest.ActualStartTime) && DateTime.TryParse(pUploadJobRequest.ActualStartTime, out lDateTime))
                {
                    _job.ActualStartTime = lDateTime.ToUniversalTime();
                    ActualStartTime = lDateTime;
                }
                else
                {
                    _job.ActualStartTime = DateTime.UtcNow;
                    ActualStartTime = DateTime.UtcNow;
                }
                //ActualEndTime
                if (!string.IsNullOrEmpty(pUploadJobRequest.ActualEndTime) && DateTime.TryParse(pUploadJobRequest.ActualEndTime, out lDateTime))
                {
                    _job.ActualEndTime = lDateTime.ToUniversalTime();
                    ActualEndTime = lDateTime;
                }
                else
                {
                    _job.ActualEndTime = DateTime.UtcNow;
                    ActualEndTime = DateTime.UtcNow;
                }
                //ActualDuration
                if (pUploadJobRequest.ActualDuration > 0)
                {
                    _job.ActualDuration = pUploadJobRequest.ActualDuration;
                }
                else
                {
                    _job.ActualDuration = Convert.ToDateTime(_job.ActualEndTime).Subtract(Convert.ToDateTime(_job.ActualStartTime)).Seconds;
                }
                _job.PreferedEndTime = DateTime.UtcNow;
                _job.ScheduledEndTime = DateTime.UtcNow;
                _job.EstimatedDuration = pUploadJobRequest.EstimatedDuration;

                _job.QuotedDuration = pUploadJobRequest.EstimatedDuration;
                _job.LastModifiedDate = DateTime.UtcNow;
                _job.LastModifiedBy = UserGUID;
                _job.StatusCode = 6;
                _job.SubStatusCode = 0;
                if (pUploadJobRequest.StatusCode > 0)
                {
                    _job.StatusCode = pUploadJobRequest.StatusCode;
                }
                if (pUploadJobRequest.SubStatusCode > 0)
                {
                    _job.SubStatusCode = pUploadJobRequest.SubStatusCode;
                }
                if (pUploadJobRequest.JobGUID != null)
                    _job.JobGUID = new Guid(pUploadJobRequest.JobGUID.ToString());
                _job.JobForm = new JavaScriptSerializer().Serialize(pUploadJobRequest.Form);
                result = _IJobRepository.UploadJobs(_job);
                //return result;
                if (result > 0)
                {
                    // Insert the Job Progress for complete
                    JobProgress _jobProgress = new JobProgress();

                    _jobProgress.JobProgressGUID = Guid.NewGuid();
                    _jobProgress.JobGUID = pUploadJobRequest.JobGUID;
                    _jobProgress.JobStatus = pUploadJobRequest.StatusCode;
                    _jobProgress.JobSubStatus = pUploadJobRequest.SubStatusCode;
                    _jobProgress.StartTime = !string.IsNullOrEmpty(pUploadJobRequest.ActualEndTime) ? Convert.ToDateTime(pUploadJobRequest.ActualEndTime).ToUniversalTime() : DateTime.UtcNow;
                    _jobProgress.Duration = pUploadJobRequest.ActualDuration;
                    _jobProgress.Latitude = pUploadJobRequest.Latitude;
                    _jobProgress.Longitude = pUploadJobRequest.Longitude;
                    //_jobProgress.LocationMismatch=UploadJobRequest.LocationMismatch;
                    _jobProgress.LastModifiedDate = DateTime.UtcNow;
                    _jobProgress.LastModifiedBy = UserGUID;


                    result = _IJobRepository.InsertJobProgressWithDuration(_jobProgress);
                    if (result > 0)
                    {
                        _jobProgress.StartTime = !string.IsNullOrEmpty(pUploadJobRequest.ActualStartTime) ? Convert.ToDateTime(pUploadJobRequest.ActualStartTime).ToUniversalTime() : DateTime.UtcNow;
                        result = _IJobRepository.UpdateJobProgress(_jobProgress);
                        Logger.Debug("Upload Jobs Response start " + result);

                        //As i discuss with alok we need to command out the PDF upload in Test Server and Demo server build-03.03.2015
                        //if (result > 0 && !string.IsNullOrEmpty(pUploadJobRequest.PONumber))
                        //{
                        //    try
                        //    {
                        //        System.Threading.Thread generatePDFThresd = new System.Threading.Thread(() =>
                        //        {
                        //            GeneratePDF(_job.JobGUID.ToString(), ActualStartTime, ActualEndTime);
                        //        });
                        //        generatePDFThresd.Start();
                        //        System.Threading.Thread.Sleep(500);
                        //    }
                        //    catch (System.Threading.ThreadAbortException ex)
                        //    {
                        //        Logger.Debug("Thread Exception :" + ex.Message);
                        //    }
                        //    // result = GeneratePDF(_job.JobGUID.ToString(), ActualStartTime, ActualEndTime);
                        //}
                        Logger.Debug("Upload Jobs Response End " + result);
                    }
                }
                return result;
                //if (resultnew > 0)
                //{

                //    // Insert the Job Progress for complete
                //    JobProgress _jobProgress = new JobProgress();
                //    _jobProgress.JobProgressGUID = Guid.NewGuid();
                //    _jobProgress.JobGUID = pUploadJobRequest.JobGUID;
                //    _jobProgress.JobStatus = pUploadJobRequest.Status;
                //    _jobProgress.JobSubStatus = pUploadJobRequest.Substatus;
                //    _jobProgress.StartTime = pUploadJobRequest.StartTIme;
                //    _jobProgress.Duration = pUploadJobRequest.ElapsedTime;
                //    //_jobProgress.Latitude = UploadJobRequest.Latitude;
                //    //_jobProgress.Longitude = UploadJobRequest.Longitude;
                //    //_jobProgress.LocationMismatch=UploadJobRequest.LocationMismatch;
                //    _jobProgress.LastModifiedDate = DateTime.UtcNow;
                //    _jobProgress.LastModifiedBy = UserGUID;
                //    result = _IJobRepository.InsertJobProgress(_jobProgress);




                //    if (pUploadJobRequest.Notes != null)
                //    {
                //        foreach (UploadJobNotes _uploadJobNotes in pUploadJobRequest.Notes)
                //        {
                //            JobNote _jobNote = new JobNote();
                //            _jobNote.JobNoteGUID = Guid.NewGuid();
                //            _jobNote.JobGUID = pUploadJobRequest.JobGUID;
                //            _jobNote.NoteText = _uploadJobNotes.NoteText;
                //            _jobNote.NoteType = _uploadJobNotes.Type;
                //            _jobNote.CreateDate = DateTime.UtcNow;
                //            _jobNote.CreateBy = UserGUID;
                //            _IJobRepository.InsertJobNotes(_jobNote);
                //            _IJobRepository.Save();
                //        }
                //    }
                //    if (pUploadJobRequest.Form != null)
                //    {
                //        //foreach (JobFormDataValue _jobFormData in UploadJobRequest.Form.JobFormDataList)
                //        //{

                //        //    JobFormData _JobFormData = new JobFormData();
                //        //    _JobFormData.JobFormDataGUID = Guid.NewGuid();
                //        //    _JobFormData.JobFormGUID = new Guid(UploadJobRequest.Form.FormID);
                //        //    _JobFormData.JobGUID = new Guid(UploadJobRequest.Form.JobGUID);
                //        //    _JobFormData.ControlsID = _jobFormData.ControlID;
                //        //    _JobFormData.Val = _jobFormData.Value;
                //        //    _JobFormData.ValID = _jobFormData.ValueID;
                //        //    _IJobRepository.InsertJobFormData(_JobFormData);
                //        //    _IJobRepository.Save();

                //        //}
                //    }


                //}
                //else
                //{
                //    return 0;
                //}

            }
            catch (Exception exception)
            {
                return 0;
            }

        }

        public ConfigurationResponse GetConfiguration(Guid UserGUID)
        {
            ConfigurationResponse lresponse = new ConfigurationResponse();
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            try
            {
                // ConfigurationResponse _configurationResponse = new ConfigurationResponse();
                Guid OrganizationGUID = _IOrganizationRepository.GetOrganizationIDByUserGUID(UserGUID);

                if (OrganizationGUID != null && OrganizationGUID != Guid.Empty)
                {

                    List<JobCostType> CostTypeList = _IJobRepository.getJobCostTypes();
                    if (CostTypeList != null)
                    {
                        lresponse.CostTypes = new List<CostTypes>();
                        foreach (JobCostType item in CostTypeList)
                        {
                            CostTypes _costType = new CostTypes();
                            _costType.CostType = Convert.ToInt32(item.CostType);
                            _costType.Rate = item.Rate.ToString();
                            _costType.CostName = item.CostName;
                            _costType.Description = item.Description;
                            _costType.CurrencySymbol = item.Symbol;
                            lresponse.CostTypes.Add(_costType);
                        }
                    }
                    List<JobStatusOrganization> StatusList = _IJobRepository.getJobStatusOrganization(OrganizationGUID);
                    if (StatusList != null)
                    {
                        lresponse.Statuses = new List<Statuses>();
                        foreach (JobStatusOrganization _jsorganization in StatusList)
                        {
                            Statuses _status = new Statuses();
                            _status.StatusCode = _jsorganization.StatusCode;
                            _status.Status = _jsorganization.Status;
                            lresponse.Statuses.Add(_status);
                        }
                    }

                    List<JobSubStatusOrganization> SubStatusList = _IJobRepository.getJobSubStatusOrganization(OrganizationGUID);
                    if (SubStatusList != null)
                    {
                        lresponse.SubStatuses = new List<SubStatuses>();
                        foreach (JobSubStatusOrganization _jssuborganization in SubStatusList)
                        {
                            SubStatuses _substatus = new SubStatuses();
                            _substatus.StatusCode = Convert.ToInt32(_jssuborganization.StatusCode);
                            _substatus.SubStatusCode = _jssuborganization.SubStatusCode;
                            _substatus.Status = _jssuborganization.SubStatus;
                            lresponse.SubStatuses.Add(_substatus);
                        }

                    }


                    var OptionLists = _IJobRepository.getOptionList(OrganizationGUID);

                    if (OptionLists != null)
                    {
                        lresponse.Lists = new List<OptionLists>();
                        foreach (OptionList _optionList in OptionLists)
                        {
                            OptionLists _OpList = new OptionLists();
                            _OpList.ListGUID = _optionList.ListGUID.ToString();
                            _OpList.ListType = Convert.ToInt16(_optionList.ListType);
                            _OpList.ListURL = _optionList.ListURL;
                            _OpList.ListValues = _optionList.ListValue;
                            lresponse.Lists.Add(_OpList);

                        }
                    }
                }

                return lresponse;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        public int InsertJobFormData(JobFormDataRequest JobFormDataRequest)
        {
            int result = 0;
            try
            {
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());

                foreach (JobFormDataValue _jobFormData in JobFormDataRequest.Values)
                {

                    JobFormData _JobFormData = new JobFormData();
                    _JobFormData.JobFormDataGUID = Guid.NewGuid();
                    _JobFormData.JobFormGUID = new Guid(JobFormDataRequest.FormID);
                    _JobFormData.JobGUID = new Guid(JobFormDataRequest.JobGUID);
                    _JobFormData.ControlsID = _jobFormData.ControlID;
                    _JobFormData.Val = _jobFormData.Value;
                    _JobFormData.ValID = _jobFormData.controlParentLabel;
                    _IJobRepository.InsertJobFormData(_JobFormData);
                    //_IJobRepository.Save();

                }

                //result = _IJobRepository.InsertJobFormData(JobFormDataRequest);
                return 1; ;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public JobStatusResponse GetJobStatus(JobStatusRequest JobStatusRequest)
        {
            JobStatusResponse lresponse = new JobStatusResponse();
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[3];
                sqlParam[0] = new SqlParameter("@UserGUID", SqlDbType.UniqueIdentifier);
                sqlParam[0].Value = JobStatusRequest.WorkerID;
                sqlParam[1] = new SqlParameter("@pStatsForManager", SqlDbType.Bit);
                sqlParam[1].Value = JobStatusRequest.IsAll;
                sqlParam[2] = new SqlParameter("@pErrorCode", SqlDbType.NVarChar, 200);
                sqlParam[2].Value = "";
                sqlParam[2].Direction = ParameterDirection.Output;
                lresponse.JobStatistics = _IJobRepository.GetJobStatus(sqlParam);
                return lresponse;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        public int AssignJob(AssignJobRequest AssignJobRequest, Guid UserGUID)
        {
            int result = 0;
            try
            {
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());

                Job _job = new Job();
                _job.JobGUID = AssignJobRequest.JobGUID;
                _job.AssignedUserGUID = AssignJobRequest.WorkerID;
                _job.LastModifiedDate = DateTime.UtcNow;
                _job.LastModifiedBy = UserGUID;
                result = _IJobRepository.AssignJob(_job);

                if (result > 0)
                {
                    Job job = _IJobRepository.GetJobByID(AssignJobRequest.JobGUID);
                    if (job != null)
                    {
                        JobAssigned _jobAssigned = new JobAssigned();
                        _jobAssigned.JobAssignGUID = Guid.NewGuid();
                        _jobAssigned.JobGUID = AssignJobRequest.JobGUID;
                        _jobAssigned.UserGUID = AssignJobRequest.WorkerID;
                        _jobAssigned.StartTime = job.ScheduledEndTime;
                        _jobAssigned.EndTime = job.ScheduledEndTime;
                        _jobAssigned.Latitude = job.Latitude;
                        _jobAssigned.Longitude = job.Longitude;
                        _jobAssigned.LastModifiedDate = DateTime.UtcNow;
                        _jobAssigned.LastModifiedBy = UserGUID;
                        return _IJobRepository.InsertJobAssigned(_jobAssigned);
                        //return _IJobRepository.Save();
                    }
                    else
                    {
                        return -5;
                    }
                }
                else
                {
                    return result;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public int UpdateForumStatus(JobForumRequest JobForumRequest, Guid UserGUID)
        {
            int result = 0;
            try
            {
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());
                JobForum _jobForum = new JobForum();
                _jobForum.JobGUID = JobForumRequest.JobGUID;
                _jobForum.ForumStatus = JobForumRequest.Acceptance;
                _jobForum.LastModifiedDate = DateTime.UtcNow;
                _jobForum.LastModifiedBy = UserGUID;
                result = _IJobRepository.UpdateForumStatus(_jobForum);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
                //return 0;
            }
        }

        public int InsertForumEntries(CreateForumEntryRequest CreateForumEntryRequest, Guid UserGUID)
        {
            int result = 0;
            try
            {
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());
                JobForum _jobForum = new JobForum();
                _jobForum.JobForumGUID = Guid.NewGuid();
                _jobForum.JobGUID = CreateForumEntryRequest.JobGUID;
                _jobForum.ForumStatus = CreateForumEntryRequest.Type;
                _jobForum.UserGUID = CreateForumEntryRequest.Recipient;
                _jobForum.ForumText = CreateForumEntryRequest.Message;
                _jobForum.LastModifiedDate = DateTime.UtcNow;
                _jobForum.LastModifiedBy = UserGUID;
                result = _IJobRepository.InsertForumEntries(_jobForum);
                //result = _IJobRepository.Save();
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public Forum GetForumEntries(Guid JobGUID)
        {
            Forum lresponse = new Forum();
            try
            {
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());

                IUserRepository _IUserRepository;
                _IUserRepository = new UserRepository(new WorkersInMotionDB());
                IGlobalUserRepository _IGlobalUserRepository;
                _IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());

                lresponse.JobGUID = JobGUID;
                lresponse.Messages = new List<Message>();
                List<JobForum> LJobForum = _IJobRepository.GetForumEntries(JobGUID);
                foreach (JobForum item in LJobForum)
                {
                    int roleid = 0;
                    Message _message = new Message();
                    _message.MessgageID = item.JobForumGUID.ToString();
                    _message.PosterID = item.LastModifiedBy.ToString();
                    _message.PostDate = convertdate(Convert.ToDateTime(item.LastModifiedDate));// item.LastModifiedDate.ToString();
                    string UserType = _IGlobalUserRepository.GetUserType(new Guid(item.LastModifiedBy.ToString()));
                    if (UserType == "ENT_U_RM" || UserType == "ENT_U_TM" || UserType == "ENT_OM")
                    {
                        roleid = 1;
                    }
                    else if (UserType == "IND_C")
                    {
                        roleid = 2;
                    }
                    else if (UserType == "ENT_U")
                    {
                        roleid = 3;
                    }
                    _message.PosterRole = roleid;
                    _message.MessageType = Convert.ToInt16(item.ForumStatus);

                    lresponse.Messages.Add(_message);
                }
                return lresponse;
            }
            catch (Exception ex)
            {
                return null;
            }



        }

        public MobilePO GetPOs(MobilePO MobilePO)
        {
            MobilePO lresponse = new MobilePO();
            POs PO = new POs();
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            PO = _IJobRepository.GetPOs(ConvertMobilePOtoPO(MobilePO));
            if (PO != null)
            {
                lresponse = ConvertPOtoMobilePO(PO);
            }
            return lresponse;
        }

        public POs ConvertMobilePOtoPO(MobilePO PO)
        {
            try
            {
                POs lresponse = new POs();
                lresponse.POGUID = PO.POGUID;
                lresponse.OrganizationGUID = PO.OrganizationGUID;
                lresponse.RegionGUID = PO.RegionGUID;
                lresponse.TerritoryGUID = PO.TerritoryGUID;
                lresponse.PONumber = PO.PONumber;
                lresponse.Status = PO.Status;
                lresponse.Description = PO.Description;
                lresponse.PlaceID = PO.PlaceID;
                lresponse.LocationType = PO.LocationType;
                lresponse.MarketID = PO.MarketID;
                lresponse.EndCustomerAddress = PO.EndCustomerAddress;
                lresponse.EndCustomerName = PO.EndCustomerName;
                lresponse.EndCustomerPhone = PO.EndCustomerPhone;
                lresponse.WorkerName = PO.WorkerName;
                lresponse.CustomBooleanValue = PO.CustomBooleanValue;
                lresponse.JobClass = PO.JobClass;
                lresponse.OrderDate = Convert.ToDateTime(PO.OrderDate);
                lresponse.PreferredDateTime = Convert.ToDateTime(PO.PreferredDateTime);
                lresponse.EstimatedCost = PO.EstimatedCost;
                lresponse.CreateDate = Convert.ToDateTime(PO.CreateDate);
                lresponse.CreateBy = PO.CreateBy;
                lresponse.LastModifiedDate = Convert.ToDateTime(PO.LastModifiedDate);
                lresponse.LastModifiedBy = PO.LastModifiedBy;
                return lresponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public MobilePO ConvertPOtoMobilePO(POs PO)
        {
            try
            {
                MobilePO lresponse = new MobilePO();
                lresponse.POGUID = PO.POGUID;
                lresponse.OrganizationGUID = PO.OrganizationGUID;
                lresponse.RegionGUID = PO.RegionGUID;
                lresponse.TerritoryGUID = PO.TerritoryGUID;
                lresponse.PONumber = PO.PONumber;
                lresponse.Status = PO.Status;
                lresponse.Description = PO.Description;
                lresponse.PlaceID = PO.PlaceID;
                lresponse.LocationType = PO.LocationType;
                lresponse.MarketID = PO.MarketID;
                lresponse.EndCustomerAddress = PO.EndCustomerAddress;
                lresponse.EndCustomerName = PO.EndCustomerName;
                lresponse.EndCustomerPhone = PO.EndCustomerPhone;
                lresponse.WorkerName = PO.WorkerName;
                lresponse.CustomBooleanValue = PO.CustomBooleanValue;
                lresponse.JobClass = PO.JobClass;
                lresponse.OrderDate = convertdate(Convert.ToDateTime(PO.OrderDate));// PO.OrderDate;
                lresponse.PreferredDateTime = convertdate(Convert.ToDateTime(PO.PreferredDateTime));// PO.PreferredDateTime;
                lresponse.EstimatedCost = PO.EstimatedCost;
                lresponse.CreateDate = convertdate(Convert.ToDateTime(PO.CreateDate));// PO.CreateDate;
                lresponse.CreateBy = PO.CreateBy;
                lresponse.LastModifiedDate = convertdate(Convert.ToDateTime(PO.LastModifiedDate));// PO.LastModifiedDate;
                lresponse.LastModifiedBy = PO.LastModifiedBy;
                return lresponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<MobilePO> GetPOList(string SessionID)
        {
            List<MobilePO> listresponse = new List<MobilePO>();
            List<POs> poList = new List<POs>();
            try
            {
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());
                poList = _IJobRepository.GetPOList(SessionID);
                if (poList != null)
                {
                    foreach (POs PO in poList)
                    {
                        MobilePO lresponse = new MobilePO();
                        lresponse = ConvertPOtoMobilePO(PO);
                        listresponse.Add(lresponse);
                    }
                }
                return listresponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int InsertPO(MobilePO PO)
        {
            POs lrequest = new POs();
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            if (PO != null)
            {
                lrequest = ConvertMobilePOtoPO(PO);
                return _IJobRepository.InsertPO(lrequest);
                //return _IJobRepository.Save();
            }
            else
            {
                return 0;
            }

        }

        private int ConvertPOSStoreToMarket(Market NewMarket, POSStoreResponse pObjPOSResp, string CustomerID)
        {
            try
            {
                if (pObjPOSResp != null && pObjPOSResp.store != null)
                {

                    //TODO we need to create our objects
                    // Already exisrting propertied
                    NewMarket.MarketName = pObjPOSResp.store.name;
                    NewMarket.AddressLine1 = pObjPOSResp.store.addr1;
                    NewMarket.AddressLine2 = pObjPOSResp.store.addr2;
                    NewMarket.City = pObjPOSResp.store.city;
                    NewMarket.State = pObjPOSResp.store.state;
                    NewMarket.Country = pObjPOSResp.store.country;
                    NewMarket.Emails = pObjPOSResp.store.email;
                    NewMarket.MarketPhone = pObjPOSResp.store.phone;
                    NewMarket.MarketID = pObjPOSResp.store.storenum;
                    Place _place = _IPlaceRepository.GetPlaceByID(CustomerID, new Guid(NewMarket.OrganizationGUID.ToString()));
                    if (_place != null)
                    {
                        NewMarket.OwnerGUID = _place.PlaceGUID;
                    }
                    NewMarket.EntityType = 1;
                    NewMarket.CreateDate = DateTime.UtcNow;
                    NewMarket.UpdatedDate = DateTime.UtcNow;

                    // New Properties
                    //We can call this PlaceGUID and this will be a foreign key to the Territory Record
                    if (!string.IsNullOrEmpty(pObjPOSResp.store.parentid))
                        NewMarket.ParentID = pObjPOSResp.store.parentid;
                    //We can call this TerritoryID and this will be a foreign key to the Territory Record
                    NewMarket.TeritoryID = pObjPOSResp.store.marketid;

                    Territory _territory = _ITerritoryRepository.GetTerritoryByTerritoryID(pObjPOSResp.store.marketid, new Guid(NewMarket.OrganizationGUID.ToString()));
                    if (_territory != null)
                    {
                        NewMarket.RegionGUID = _territory.RegionGUID;
                        NewMarket.TerritoryGUID = _territory.TerritoryGUID;
                    }

                    //Region is Region Name
                    NewMarket.RegionName = pObjPOSResp.store.region;
                    // If the Regional manager is not null
                    if (pObjPOSResp.store.regionalmanager != null)
                        NewMarket.RMUserID = pObjPOSResp.store.regionalmanager.userid;
                    // If the Field manager is not null
                    if (pObjPOSResp.store.fieldmanager != null)
                        NewMarket.FMUserID = pObjPOSResp.store.fieldmanager.userid;
                    NewMarket.StoreJSON = new JavaScriptSerializer().Serialize(pObjPOSResp);
                    //We need to return the base 64 encoded string as JSON from here
                    NewMarket.StoreJSON = Convert.ToBase64String(Encoding.UTF8.GetBytes(NewMarket.StoreJSON));
                    //Create the Market using the repository
                    MarketRepository lMarketRepository = new MarketRepository(new WorkersInMotionDB());
                    return lMarketRepository.CreateMarket(NewMarket);

                    //Prabhu
                    //As per discussion with Kowsik i am doing to inset Region and Territory details
                    //bool territoryFlag = false;
                    //Place Place = new Place();
                    //Place.PlaceGUID = Guid.NewGuid();
                    //Place.UserGUID = NewMarket.UserGUID != null ? new Guid(NewMarket.UserGUID.ToString()) : Guid.Empty;
                    //Place.OrganizationGUID = NewMarket.OrganizationGUID != null ? new Guid(NewMarket.OrganizationGUID.ToString()) : Guid.Empty;
                    //Place.PlaceID = CustomerID;
                    //Place.PlaceName = "";
                    //Place.FirstName = "";
                    //Place.LastName = "";
                    //Place.MobilePhone = "";
                    //Place.PlacePhone = pObjPOSResp.store.phone;
                    //Place.HomePhone = "";
                    //Place.Emails = "";
                    //Place.AddressLine1 = pObjPOSResp.store.addr1;
                    //Place.AddressLine2 = pObjPOSResp.store.addr2;
                    //Place.City = pObjPOSResp.store.city;
                    //Place.State = pObjPOSResp.store.state;
                    //Place.Country = "";
                    //Place.ZipCode = pObjPOSResp.store.postalcode;
                    //Place.CategoryID = 0;
                    //Place.IsDeleted = false;
                    //Place.CreateDate = DateTime.UtcNow;
                    //Place.UpdatedDate = DateTime.UtcNow;
                    //LatLong latLong = new LatLong();
                    //latLong = GetLatLngCode(Place.AddressLine1, Place.AddressLine2, Place.City, Place.State, Place.Country, Place.ZipCode);
                    //Place.TimeZone = getTimeZone(latLong.Latitude, latLong.Longitude).ToString();

                    //_IPlaceRepository.InsertPlace(Place);
                    //if (_IPlaceRepository.Save() > 0)
                    //{
                    //    Person People = new Person();
                    //    People.PeopleGUID = Guid.NewGuid();
                    //    People.UserGUID = Place.UserGUID;
                    //    People.OrganizationGUID = Place.OrganizationGUID;
                    //    People.IsPrimaryContact = true;
                    //    People.PlaceGUID = Place.PlaceGUID;
                    //    People.FirstName = Place.FirstName;
                    //    People.LastName = Place.LastName;
                    //    People.MobilePhone = Place.MobilePhone;
                    //    People.CompanyName = Place.PlaceName;
                    //    People.BusinessPhone = Place.PlacePhone;
                    //    People.HomePhone = Place.HomePhone;
                    //    People.Emails = Place.Emails;
                    //    People.AddressLine1 = Place.AddressLine1;
                    //    People.AddressLine2 = Place.AddressLine2;
                    //    People.City = Place.City;
                    //    People.State = Place.State;
                    //    People.Country = Place.Country;
                    //    People.ZipCode = Place.ZipCode;
                    //    People.CategoryID = 0;
                    //    People.IsDeleted = false;
                    //    People.CreatedDate = DateTime.UtcNow;
                    //    People.UpdatedDate = DateTime.UtcNow;

                    //    _IPeopleRepository.InsertPeople(People);
                    //    if (_IPeopleRepository.Save() > 0)
                    //    {

                    //        Territory _territory = _ITerritoryRepository.GetTerritoryByTerritoryID(pObjPOSResp.store.marketid);
                    //        Territory Territory = new Territory();
                    //        Region Region = new Region();

                    //        if (_territory != null)
                    //        {
                    //            territoryFlag = true;
                    //            Territory.TerritoryGUID = _territory.TerritoryGUID;
                    //            Territory.TerritoryID = pObjPOSResp.store.marketid;
                    //            Territory.Name = pObjPOSResp.store.region;
                    //            Territory.Description = pObjPOSResp.store.region;
                    //            Territory.OrganizationGUID = NewMarket.OrganizationGUID;
                    //            Territory.IsDefault = false;


                    //            Region.RegionGUID = Guid.NewGuid();
                    //            Region.TerritoryGUID = _territory.TerritoryGUID;
                    //            Region.Name = pObjPOSResp.store.region;
                    //            Region.Description = pObjPOSResp.store.region;
                    //            Region.OrganizationGUID = NewMarket.OrganizationGUID;
                    //            Region.IsDefault = false;

                    //        }
                    //        else
                    //        {
                    //            Territory.TerritoryGUID = Guid.NewGuid();
                    //            Territory.TerritoryID = pObjPOSResp.store.marketid;
                    //            Territory.Name = pObjPOSResp.store.region;
                    //            Territory.Description = pObjPOSResp.store.region;
                    //            Territory.OrganizationGUID = NewMarket.OrganizationGUID;
                    //            Territory.IsDefault = false;

                    //            Region.RegionGUID = Guid.NewGuid();
                    //            Region.TerritoryGUID = Territory.TerritoryGUID;
                    //            Region.Name = pObjPOSResp.store.region;
                    //            Region.Description = pObjPOSResp.store.region;
                    //            Region.OrganizationGUID = NewMarket.OrganizationGUID;
                    //            Region.IsDefault = false;

                    //        }
                    //        //TODO we need to create our objects
                    //        // Already exisrting propertied
                    //        if (territoryFlag == true)
                    //        {
                    //            _ITerritoryRepository.UpdateTerritory(Territory);
                    //        }
                    //        else
                    //        {
                    //            _ITerritoryRepository.InsertTerritory(Territory);
                    //        }
                    //        if (_ITerritoryRepository.Save() > 0)
                    //        {
                    //            _IRegionRepository.InsertRegion(Region);
                    //            if (_IRegionRepository.Save() > 0)
                    //            {
                    //                NewMarket.MarketName = pObjPOSResp.store.name;
                    //                NewMarket.AddressLine1 = pObjPOSResp.store.addr1;
                    //                NewMarket.AddressLine2 = pObjPOSResp.store.addr2;
                    //                NewMarket.City = pObjPOSResp.store.city;
                    //                NewMarket.State = pObjPOSResp.store.state;
                    //                NewMarket.Country = pObjPOSResp.store.country;
                    //                NewMarket.Emails = pObjPOSResp.store.email;
                    //                NewMarket.MarketPhone = pObjPOSResp.store.phone;
                    //                NewMarket.MarketID = pObjPOSResp.store.storenum;
                    //                NewMarket.CreateDate = DateTime.UtcNow;
                    //                NewMarket.UpdatedDate = DateTime.UtcNow;
                    //                NewMarket.RegionGUID = Region.RegionGUID;
                    //                NewMarket.TerritoryGUID = Territory.TerritoryGUID;
                    //                NewMarket.OwnerGUID = Place.PlaceGUID;
                    //                // New Properties
                    //                //We can call this PlaceGUID and this will be a foreign key to the Territory Record
                    //                if (!string.IsNullOrEmpty(pObjPOSResp.store.parentid))
                    //                    NewMarket.ParentID = pObjPOSResp.store.parentid;
                    //                //We can call this TerritoryID and this will be a foreign key to the Territory Record
                    //                NewMarket.TeritoryID = pObjPOSResp.store.marketid;

                    //                //Region is Region Name
                    //                NewMarket.RegionName = pObjPOSResp.store.region;
                    //                // If the Regional manager is not null
                    //                if (pObjPOSResp.store.regionalmanager != null)
                    //                    NewMarket.RMUserID = pObjPOSResp.store.regionalmanager.userid;
                    //                // If the Field manager is not null
                    //                if (pObjPOSResp.store.fieldmanager != null)
                    //                    NewMarket.FMUserID = pObjPOSResp.store.fieldmanager.userid;
                    //                NewMarket.StoreJSON = new JavaScriptSerializer().Serialize(pObjPOSResp);
                    //                //We need to return the base 64 encoded string as JSON from here
                    //                NewMarket.StoreJSON = Convert.ToBase64String(Encoding.UTF8.GetBytes(NewMarket.StoreJSON));
                    //                //Create the Market using the repository
                    //                MarketRepository lMarketRepository = new MarketRepository(new WorkersInMotionDB());
                    //                if (lMarketRepository.CreateMarket(NewMarket) > 0)
                    //                {
                    //                    return 1;
                    //                }
                    //                else
                    //                {
                    //                    _IRegionRepository.DeleteRegion(Region.RegionGUID);
                    //                    _IRegionRepository.Save();
                    //                    _ITerritoryRepository.DeleteTerritory(Territory.TerritoryGUID);
                    //                    _ITerritoryRepository.Save();
                    //                    _IPeopleRepository.DeletePeople(People.PeopleGUID);
                    //                    _IPeopleRepository.Save();
                    //                    _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //                    _IPlaceRepository.Save();
                    //                    return 0;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                _ITerritoryRepository.DeleteTerritory(Territory.TerritoryGUID);
                    //                _ITerritoryRepository.Save();
                    //                _IPeopleRepository.DeletePeople(People.PeopleGUID);
                    //                _IPeopleRepository.Save();
                    //                _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //                _IPlaceRepository.Save();
                    //                return 0;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            _IPeopleRepository.DeletePeople(People.PeopleGUID);
                    //            _IPeopleRepository.Save();
                    //            _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //            _IPlaceRepository.Save();
                    //            return 0;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //        _IPlaceRepository.Save();
                    //        return 0;
                    //    }
                    //}
                    //else
                    //{
                    //    return 0;
                    //}
                }
                else
                {
                    // No stoer information found
                    throw new Exception("No stoer information found from client server");
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private Market GetPOSCustomerStop(Market NewMarket, Guid OrganizationGUID, string CustomerID)
        {
            Market lRetVal = NewMarket;

            var lWebClient = new WebClient();
            string lTempData = String.Format(ConfigurationManager.AppSettings.Get("ClientStoreURL"), NewMarket.MarketID);
            lTempData = lWebClient.DownloadString(lTempData);
            POSStoreResponse lObjPOSResp = new JavaScriptSerializer().Deserialize<POSStoreResponse>(lTempData);
            if (null == lObjPOSResp || !lObjPOSResp.store.apistatus.Equals("OK"))
            {
                //If this returns null, return not found error to the mobile
                lRetVal = null;
            }
            else
            {
                //Populate the Market and save it
                ConvertPOSStoreToMarket(NewMarket, lObjPOSResp, CustomerID);
            }
            return lRetVal;
        }

        public MobileJob CreateJobForCustomerStop(CreateJobForCustomerStopRequest jobRequest, Guid UserGUID, ref int errorCode)
        {
            short Tempshort = 0;
            MobileJob lresponse = new MobileJob();
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            Job _job = new Job();
            Guid OrganizationGUID = _IOrganizationRepository.GetOrganizationIDByUserGUID(UserGUID);
            //Market _market = _IMarketRepository.GetMarketByMarketID(OrganizationGUID, jobRequest.CustomerStopID, jobRequest.CustomerID);
            Market _market = _IMarketRepository.GetMarketByCustomerID(OrganizationGUID, jobRequest.CustomerID, jobRequest.CustomerStopID);
            if (null == _market)
            {
                _market = new Market();
                _market.MarketGUID = Guid.NewGuid();
                _market.MarketID = jobRequest.CustomerStopID;
                _market.OrganizationGUID = OrganizationGUID;
                _market.UserGUID = UserGUID;
                _market = GetPOSCustomerStop(_market, OrganizationGUID, jobRequest.CustomerID);
            }
            if (_market != null)
            {
                //bool IsAvailable = true;
                //As i discussed alok, Any user can create visit there is no restrictions.-23.02.2015
                //if (!string.IsNullOrEmpty(_market.StoreJSON))
                //{

                //    GlobalUser gUser = _IGlobalUserRepository.GetGlobalUserByID(UserGUID);
                //    if (gUser != null)
                //    {
                //        byte[] byteJson = Convert.FromBase64String(_market.StoreJSON);
                //        string jsString = System.Text.Encoding.UTF8.GetString(byteJson);
                //        S_POSStoreResponse lObjPOSResp = new JavaScriptSerializer().Deserialize<S_POSStoreResponse>(jsString);
                //        if (lObjPOSResp.store.fieldmanager != null && !IsAvailable)
                //        {
                //            //Logger.Debug("Inside Filed Manager");
                //            if (lObjPOSResp.store.fieldmanager.userid.ToUpper().Trim() == gUser.USERID.ToUpper().Trim())
                //                IsAvailable = true;
                //        }
                //        if (lObjPOSResp.store.regionalmanager != null && !IsAvailable)
                //        {
                //            // Logger.Debug("Inside Region Manager");
                //            if (lObjPOSResp.store.regionalmanager.userid.ToUpper().Trim() == gUser.USERID.ToUpper().Trim())
                //                IsAvailable = true;
                //        }
                //        //Logger.Debug("is available:" + IsAvailable);
                //        //Logger.Debug("Inside Manager count is:" + lObjPOSResp.store.managers.Count);
                //        if (lObjPOSResp.store.managers != null && lObjPOSResp.store.managers.Count > 0 && !IsAvailable)
                //        {
                //            //Logger.Debug("Inside Manager count is:" + lObjPOSResp.store.managers.Count);
                //            foreach (S_POSResponseManagers manager in lObjPOSResp.store.managers)
                //            {
                //                //Logger.Debug("mangarid : " + manager.userid.ToUpper().Trim() + "User ID : " + gUser.USERID.ToUpper().Trim());
                //                if (manager.userid.ToUpper().Trim() == gUser.USERID.ToUpper().Trim())
                //                {
                //                    IsAvailable = true;
                //                    break;
                //                }

                //            }
                //        }
                //    }
                //}
                //if (IsAvailable)
                {
                    _job.OrganizationGUID = OrganizationGUID;
                    _job.IsSecheduled = false;
                    _job.ManagerUserGUID = UserGUID;
                    _job.AssignedUserGUID = UserGUID;
                    _job.PreferedStartTime = DateTime.UtcNow;
                    _job.PreferedEndTime = DateTime.UtcNow;
                    _job.ActualStartTime = DateTime.UtcNow;

                    _job.LocationType = 1;
                    _job.StatusCode = 1;
                    _job.SubStatusCode = 1;
                    _job.IsActive = true;
                    _job.IsDeleted = false;
                    _job.JobName = jobRequest.JobName;

                    // Need to be discussed before using - Alok 26th May 2014
                    //_job.POSJson = _market.StoreJSON;
                    if (short.TryParse(jobRequest.JobClass, out Tempshort))
                    {
                        _job.JobClass = Tempshort;
                    }
                    _job.CustomerStopGUID = _market.MarketGUID;
                    _job.ServiceAddress = (string.IsNullOrEmpty(_market.AddressLine1) ? "" : _market.AddressLine1 + ",") +
                        (string.IsNullOrEmpty(_market.AddressLine2) ? "" : _market.AddressLine2 + ",") +
                        (string.IsNullOrEmpty(_market.City) ? "" : _market.City + ",") +
                        (string.IsNullOrEmpty(_market.State) ? "" : _market.State + ",") +
                        (string.IsNullOrEmpty(_market.ZipCode) ? "" : _market.ZipCode);
                    _job.RegionGUID = _market.RegionGUID;
                    _job.TerritoryGUID = _market.TerritoryGUID;
                    _job.CustomerGUID = _market.OwnerGUID;
                    if (!string.IsNullOrEmpty(jobRequest.Latitude) && !string.IsNullOrEmpty(jobRequest.Longitude))
                    {
                        _job.Latitude = Convert.ToDouble(jobRequest.Latitude);
                        _job.Longitude = Convert.ToDouble(jobRequest.Longitude);
                    }
                    else
                    {
                        _job.Latitude = _market.Latitude;
                        _job.Longitude = _market.Longitude;
                    }
                    _job.CreateBy = UserGUID;
                    _job.LastModifiedBy = UserGUID;
                    // lretString.Append(" [4.1.1: Job" + new JavaScriptSerializer().Serialize(_job) + "]");
                    Job job = _IJobRepository.CreateJobbyStoreID(_job);
                    int result = 0;
                    if (job != null)
                    {
                        //Insert the Job Progress for complete
                        JobProgress _jobProgress = new JobProgress();
                        _jobProgress.JobProgressGUID = Guid.NewGuid();
                        _jobProgress.JobGUID = job.JobGUID;
                        _jobProgress.JobStatus = job.StatusCode;
                        _jobProgress.JobSubStatus = job.SubStatusCode;
                        _jobProgress.StartTime = job.ActualStartTime != null ? job.ActualStartTime : DateTime.UtcNow;
                        _jobProgress.Duration = job.ActualDuration;
                        _jobProgress.Latitude = job.Latitude;
                        _jobProgress.Longitude = job.Longitude;
                        //_jobProgress.LocationMismatch=UploadJobRequest.LocationMismatch;
                        _jobProgress.LastModifiedDate = DateTime.UtcNow;
                        _jobProgress.LastModifiedBy = UserGUID;
                        result = _IJobRepository.InsertJobProgress(_jobProgress);

                    }
                    if (result > 0)
                    {
                        lresponse = ConvertJobForMobile(job, _market.StoreJSON, UserGUID.ToString(), OrganizationGUID.ToString());
                        if (lresponse != null)
                        {
                            _IMarketRepository.UpdateStoreVisitedDate(_market.MarketGUID);
                        }

                    }
                    else
                    {
                        lresponse = null;
                    }

                }
                //else
                //{
                //    //Managers doesn't belong to this store
                //    errorCode = 1;
                //    lresponse = null;
                //}
            }
            return lresponse;
        }

        private int ConvertPOSPOToPO(POs NewPO, poClass pObjPOSResp)
        {
            try
            {
                int result = 0;
                if (pObjPOSResp != null)
                {
                    //TODO we need to create our objects
                    // Already existing properties
                    NewPO.Status = 1;
                    NewPO.MarketID = pObjPOSResp.store.storenum;
                    if (!string.IsNullOrEmpty(NewPO.MarketID))
                    {
                        Market _market = _IMarketRepository.GetMarketByCustomerID(new Guid(NewPO.OrganizationGUID.ToString()), pObjPOSResp.store.parentid, NewPO.MarketID);
                        if (_market != null)
                        {
                            NewPO.RegionGUID = _market.RegionGUID;
                            NewPO.TerritoryGUID = _market.TerritoryGUID;
                        }
                        else if (!string.IsNullOrEmpty(pObjPOSResp.store.marketid))
                        {
                            Territory _territory = _ITerritoryRepository.GetTerritoryByTerritoryID(pObjPOSResp.store.marketid, new Guid(NewPO.OrganizationGUID.ToString()));
                            if (_territory != null)
                            {
                                NewPO.RegionGUID = _territory.RegionGUID;
                                NewPO.TerritoryGUID = _territory.TerritoryGUID;
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(pObjPOSResp.store.marketid))
                    {
                        Territory _territory = _ITerritoryRepository.GetTerritoryByTerritoryID(pObjPOSResp.store.marketid, new Guid(NewPO.OrganizationGUID.ToString()));
                        if (_territory != null)
                        {
                            NewPO.RegionGUID = _territory.RegionGUID;
                            NewPO.TerritoryGUID = _territory.TerritoryGUID;
                        }
                    }
                    else
                    {
                        return -2;
                    }
                    NewPO.EndCustomerName = pObjPOSResp.customer.firstname + " " + pObjPOSResp.customer.lastname;
                    NewPO.EndCustomerAddress = (string.IsNullOrEmpty(pObjPOSResp.customer.address1) ? "" : pObjPOSResp.customer.address1 + ",") +
                    (string.IsNullOrEmpty(pObjPOSResp.customer.address2) ? "" : pObjPOSResp.customer.address2 + ",") +
                    (string.IsNullOrEmpty(pObjPOSResp.customer.city) ? "" : pObjPOSResp.customer.city + ",") +
                    (string.IsNullOrEmpty(pObjPOSResp.customer.state) ? "" : pObjPOSResp.customer.state + ",") +
                    (string.IsNullOrEmpty(pObjPOSResp.customer.postalcode) ? "" : pObjPOSResp.customer.postalcode);

                    // string.IsNullOrEmpty(pObjPOSResp.po.customer.address1) ? "" : pObjPOSResp.po.customer.address1 + "," + string.IsNullOrEmpty(pObjPOSResp.po.customer.address2) ? "" : pObjPOSResp.po.customer.address2 + "," + "" + pObjPOSResp.po.customer.city + "" + pObjPOSResp.po.customer.state + "" + pObjPOSResp.po.customer.postalcode;
                    NewPO.EndCustomerPhone = pObjPOSResp.customer.mobilephone;

                    NewPO.InstallerName = pObjPOSResp.installer.name;
                    NewPO.POCustomerName = string.IsNullOrEmpty(pObjPOSResp.customer.firstname) ?
                        (string.IsNullOrEmpty(pObjPOSResp.customer.lastname) ? "" : pObjPOSResp.customer.lastname) : string.IsNullOrEmpty(pObjPOSResp.customer.lastname) ? pObjPOSResp.customer.firstname : (pObjPOSResp.customer.firstname + " " + pObjPOSResp.customer.lastname);
                    NewPO.POCustomerPhone = pObjPOSResp.customer.homephone;
                    NewPO.POCustomerMobile = pObjPOSResp.customer.mobilephone;


                    NewPO.CreateDate = DateTime.UtcNow;
                    NewPO.LastModifiedDate = DateTime.UtcNow;

                    // New Properties
                    NewPO.PONumber = pObjPOSResp.ticketnumber;
                    NewPO.PlaceID = pObjPOSResp.store.parentid;
                    if (!string.IsNullOrEmpty(NewPO.PlaceID))
                    {
                        Place _place = _IPlaceRepository.GetPlaceByID(NewPO.PlaceID, new Guid(NewPO.OrganizationGUID.ToString()));
                        if (_place == null)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return -1;
                    }

                    NewPO.TerritoryID = pObjPOSResp.store.parentid;
                    //  Region not found in the json Alok 26-May 
                    //NewPO.RegionName = pObjPOSResp.po.;
                    NewPO.RMUserID = pObjPOSResp.regionalmanager.userid;
                    NewPO.FMUserID = pObjPOSResp.fieldmanager.userid;
                    NewPO.POJobCode = pObjPOSResp.job.code;
                    NewPO.POJobType = pObjPOSResp.job.type;
                    NewPO.POJson = new JavaScriptSerializer().Serialize(pObjPOSResp);
                    NewPO.POJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(NewPO.POJson));
                    //Create the Market using the repository

                    PORepository lPORepository = new PORepository(new WorkersInMotionDB());
                    return lPORepository.CreatePO(NewPO);

                    ////TODO we need to create our objects
                    //// Already existing properties
                    //NewPO.Status = 1;
                    //NewPO.MarketID = pObjPOSResp.po.store.storenum;
                    //NewPO.CreateDate = DateTime.UtcNow;
                    //NewPO.LastModifiedDate = DateTime.UtcNow;

                    //// New Properties
                    //NewPO.PONumber = pObjPOSResp.po.ticketnumber;
                    //NewPO.PlaceID = pObjPOSResp.po.store.parentid;
                    //NewPO.TerritoryID = pObjPOSResp.po.store.marketid;
                    ////  Region not found in the json Alok 26-May 
                    ////NewPO.RegionName = pObjPOSResp.po.;
                    //NewPO.RMUserID = pObjPOSResp.po.regionalmanager.userid;
                    //NewPO.FMUserID = pObjPOSResp.po.fieldmanager.userid;
                    //NewPO.POJobCode = pObjPOSResp.po.job.code;
                    //NewPO.POJobType = pObjPOSResp.po.job.type;
                    //NewPO.POJson = new JavaScriptSerializer().Serialize(pObjPOSResp);
                    //NewPO.POJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(NewPO.POJson));
                    ////Create the Market using the repository


                    ////Prabhu
                    ////As per discussion with Kowsik i am doing to inset Customer,Contact,Customer Stop,Region and Territory details
                    //#region Basic Details
                    //Place _place = _IPlaceRepository.GetPlaceByID(NewPO.PlaceID);
                    //if (_place == null)
                    //{
                    //    Place Place = new Place();
                    //    Place.PlaceGUID = Guid.NewGuid();
                    //    Place.UserGUID = NewPO.LastModifiedBy != null ? new Guid(NewPO.LastModifiedBy.ToString()) : Guid.Empty;
                    //    Place.OrganizationGUID = NewPO.OrganizationGUID != null ? new Guid(NewPO.OrganizationGUID.ToString()) : Guid.Empty;
                    //    Place.PlaceID = pObjPOSResp.po.store.parentid;
                    //    Place.PlaceName = "";
                    //    Place.FirstName = pObjPOSResp.po.customer.firstname;
                    //    Place.LastName = pObjPOSResp.po.customer.lastname;
                    //    Place.MobilePhone = pObjPOSResp.po.customer.mobilephone;
                    //    Place.PlacePhone = "";
                    //    Place.HomePhone = pObjPOSResp.po.customer.homephone;
                    //    Place.Emails = "";
                    //    Place.AddressLine1 = pObjPOSResp.po.customer.address1;
                    //    Place.AddressLine2 = pObjPOSResp.po.customer.address2;
                    //    Place.City = pObjPOSResp.po.customer.city;
                    //    Place.State = pObjPOSResp.po.customer.state;
                    //    Place.Country = "";
                    //    Place.ZipCode = pObjPOSResp.po.customer.postalcode;
                    //    Place.CategoryID = 0;
                    //    Place.IsDeleted = false;
                    //    Place.CreateDate = DateTime.UtcNow;
                    //    Place.UpdatedDate = DateTime.UtcNow;
                    //    LatLong latLong = new LatLong();
                    //    latLong = GetLatLngCode(Place.AddressLine1, Place.AddressLine2, Place.City, Place.State, Place.Country, Place.ZipCode);
                    //    Place.TimeZone = getTimeZone(latLong.Latitude, latLong.Longitude).ToString();

                    //    Person People = new Person();
                    //    People.PeopleGUID = Guid.NewGuid();
                    //    People.UserGUID = Place.UserGUID;
                    //    People.OrganizationGUID = Place.OrganizationGUID;
                    //    People.IsPrimaryContact = true;
                    //    People.PlaceGUID = Place.PlaceGUID;
                    //    People.FirstName = Place.FirstName;
                    //    People.LastName = Place.LastName;
                    //    People.MobilePhone = Place.MobilePhone;
                    //    People.CompanyName = Place.PlaceName;
                    //    People.BusinessPhone = Place.PlacePhone;
                    //    People.HomePhone = Place.HomePhone;
                    //    People.Emails = Place.Emails;
                    //    People.AddressLine1 = Place.AddressLine1;
                    //    People.AddressLine2 = Place.AddressLine2;
                    //    People.City = Place.City;
                    //    People.State = Place.State;
                    //    People.Country = Place.Country;
                    //    People.ZipCode = Place.ZipCode;
                    //    People.CategoryID = 0;
                    //    People.IsDeleted = false;
                    //    People.CreatedDate = DateTime.UtcNow;
                    //    People.UpdatedDate = DateTime.UtcNow;

                    //    bool territoryFlag = false;
                    //    Territory _territory = _ITerritoryRepository.GetTerritoryByTerritoryID(NewPO.TerritoryID);
                    //    Territory Territory = new Territory();
                    //    Region Region = new Region();
                    //    if (_territory != null)
                    //    {
                    //        territoryFlag = true;

                    //        Territory.TerritoryGUID = _territory.TerritoryGUID;
                    //        Territory.TerritoryID = NewPO.TerritoryID;
                    //        Territory.Name = pObjPOSResp.po.store.region;
                    //        Territory.Description = pObjPOSResp.po.store.region;
                    //        Territory.OrganizationGUID = Place.OrganizationGUID;
                    //        Territory.IsDefault = false;


                    //        Region.RegionGUID = Guid.NewGuid();
                    //        Region.TerritoryGUID = _territory.TerritoryGUID;
                    //        Region.Name = pObjPOSResp.po.store.region;
                    //        Region.Description = pObjPOSResp.po.store.region;
                    //        Region.OrganizationGUID = Place.OrganizationGUID;
                    //        Region.IsDefault = false;
                    //    }
                    //    else
                    //    {
                    //        Territory.TerritoryGUID = Guid.NewGuid();
                    //        Territory.TerritoryID = NewPO.TerritoryID;
                    //        Territory.Name = pObjPOSResp.po.store.region;
                    //        Territory.Description = pObjPOSResp.po.store.region;
                    //        Territory.OrganizationGUID = Place.OrganizationGUID;
                    //        Territory.IsDefault = false;


                    //        Region.RegionGUID = Guid.NewGuid();
                    //        Region.TerritoryGUID = Territory.TerritoryGUID;
                    //        Region.Name = pObjPOSResp.po.store.region;
                    //        Region.Description = pObjPOSResp.po.store.region;
                    //        Region.OrganizationGUID = Place.OrganizationGUID;
                    //        Region.IsDefault = false;
                    //    }

                    //    Market Market = new Market();
                    //    Market.MarketGUID = Guid.NewGuid();
                    //    Market.MarketID = NewPO.MarketID;
                    //    Market.IsDefault = true;
                    //    Market.UserGUID = Place.UserGUID;
                    //    Market.EntityType = 1;
                    //    Market.OrganizationGUID = Place.OrganizationGUID;
                    //    Market.OwnerGUID = Place.PlaceGUID;
                    //    Market.MarketName = Place.PlaceName;
                    //    Market.PrimaryContactGUID = People.PeopleGUID;
                    //    Market.FirstName = Place.FirstName;
                    //    Market.LastName = Place.LastName;
                    //    Market.MobilePhone = Place.MobilePhone;
                    //    Market.MarketPhone = Place.PlacePhone;
                    //    Market.HomePhone = Place.HomePhone;
                    //    Market.Emails = Place.Emails;
                    //    Market.AddressLine1 = Place.AddressLine1;
                    //    Market.AddressLine2 = Place.AddressLine2;
                    //    Market.City = Place.City;
                    //    Market.State = Place.State;
                    //    Market.Country = Place.Country;
                    //    Market.ZipCode = Place.ZipCode;
                    //    Market.IsDeleted = false;
                    //    Market.CreateDate = DateTime.UtcNow;
                    //    Market.UpdatedDate = DateTime.UtcNow;
                    //    Market.RegionGUID = Region.RegionGUID;
                    //    Market.TerritoryGUID = Territory.TerritoryGUID;
                    //    Market.TeritoryID = NewPO.TerritoryID;
                    //    Market.ParentID = pObjPOSResp.po.store.parentid;
                    //    //Region is Region Name
                    //    Market.RegionName = pObjPOSResp.po.store.region;
                    //    // If the Regional manager is not null
                    //    if (pObjPOSResp.po.regionalmanager != null)
                    //        Market.RMUserID = pObjPOSResp.po.regionalmanager.userid;
                    //    // If the Field manager is not null
                    //    if (pObjPOSResp.po.fieldmanager != null)
                    //        Market.FMUserID = pObjPOSResp.po.fieldmanager.userid;
                    //    Market.StoreJSON = new JavaScriptSerializer().Serialize(pObjPOSResp);
                    //    //We need to return the base 64 encoded string as JSON from here
                    //    Market.StoreJSON = Convert.ToBase64String(Encoding.UTF8.GetBytes(Market.StoreJSON));

                    //    _IPlaceRepository.InsertPlace(Place);
                    //    int placeInsertResult = _IPlaceRepository.Save();
                    //    if (placeInsertResult > 0)
                    //    {
                    //        _IPeopleRepository.InsertPeople(People);
                    //        int peopleInsertResult = _IPeopleRepository.Save();
                    //        if (peopleInsertResult > 0)
                    //        {
                    //            if (territoryFlag == true)
                    //            {
                    //                _ITerritoryRepository.UpdateTerritory(Territory);
                    //            }
                    //            else
                    //            {
                    //                _ITerritoryRepository.InsertTerritory(Territory);
                    //            }
                    //            int _terresult = _ITerritoryRepository.Save();
                    //            if (_terresult > 0)
                    //            {
                    //                _IRegionRepository.InsertRegion(Region);
                    //                int regionresult = _IRegionRepository.Save();
                    //                if (regionresult > 0)
                    //                {
                    //                    _IMarketRepository.InsertMarket(Market);
                    //                    int marketInsertResult = _IMarketRepository.Save();
                    //                    if (marketInsertResult > 0)
                    //                    {
                    //                        NewPO.RegionGUID = Region.RegionGUID;
                    //                        NewPO.TerritoryGUID = Territory.TerritoryGUID;
                    //                        NewPO.PlaceID = Place.PlaceID;
                    //                        NewPO.MarketID = Market.MarketID;
                    //                        NewPO.EndCustomerAddress = Place.AddressLine1 + " " + Place.AddressLine2;
                    //                        NewPO.EndCustomerName = Place.FirstName + " " + Place.LastName;
                    //                        NewPO.EndCustomerPhone = Place.MobilePhone;
                    //                        PORepository lPORepository = new PORepository(new WorkersInMotionJobDB());
                    //                        result = lPORepository.CreatePO(NewPO);
                    //                    }
                    //                    else
                    //                    {
                    //                        _IRegionRepository.DeleteRegion(Region.RegionGUID);
                    //                        _IRegionRepository.Save();
                    //                        _ITerritoryRepository.DeleteTerritory(Territory.TerritoryGUID);
                    //                        _ITerritoryRepository.Save();
                    //                        _IPeopleRepository.DeletePeople(People.PeopleGUID);
                    //                        _IPeopleRepository.Save();
                    //                        _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //                        _IPlaceRepository.Save();
                    //                        _IMarketRepository.DeleteMarket(Market.MarketGUID);
                    //                        _IMarketRepository.Save();
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    _ITerritoryRepository.DeleteTerritory(Territory.TerritoryGUID);
                    //                    _ITerritoryRepository.Save();
                    //                    _IPeopleRepository.DeletePeople(People.PeopleGUID);
                    //                    _IPeopleRepository.Save();
                    //                    _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //                    _IPlaceRepository.Save();
                    //                }
                    //            }
                    //            else
                    //            {
                    //                _IPeopleRepository.DeletePeople(People.PeopleGUID);
                    //                _IPeopleRepository.Save();
                    //                _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //                _IPlaceRepository.Save();
                    //            }
                    //        }
                    //        else
                    //        {
                    //            _IPeopleRepository.DeletePeople(People.PeopleGUID);
                    //            _IPeopleRepository.Save();
                    //            _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //            _IPlaceRepository.Save();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        _IPlaceRepository.DeletePlace(Place.PlaceGUID);
                    //        _IPlaceRepository.Save();
                    //    }
                    //}
                    //else
                    //{
                    //    Market _Market = _IMarketRepository.GetMarketByOwnerGUID(_place.PlaceGUID).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                    //    if (_Market != null)
                    //    {
                    //        NewPO.RegionGUID = _Market.RegionGUID;
                    //        NewPO.TerritoryGUID = _Market.TerritoryGUID;
                    //        NewPO.MarketID = _Market.MarketID;
                    //    }
                    //    NewPO.PlaceID = _place.PlaceID;

                    //    NewPO.EndCustomerAddress = pObjPOSResp.po.customer.address1 + " " + pObjPOSResp.po.customer.address1;
                    //    NewPO.EndCustomerName = pObjPOSResp.po.customer.firstname + " " + pObjPOSResp.po.customer.lastname;
                    //    NewPO.EndCustomerPhone = pObjPOSResp.po.customer.mobilephone;
                    //    PORepository lPORepository = new PORepository(new WorkersInMotionJobDB());
                    //    result = lPORepository.CreatePO(NewPO);
                    //}
                    //#endregion

                }
                else
                {
                    // PO information not found in client URL
                    throw new Exception("PO information not found in client URL");
                }
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private POs GetPOSPO(POs NewPO, ref int errorCode)
        {
            POs lRetVal = NewPO;

            poClass lObjPOSResp = new JavaScriptSerializer().Deserialize<poClass>(NewPO.POJson);
            if (null == lObjPOSResp)
            {
                //If this returns null, return not found error to the mobile
                lRetVal = null;
            }
            else
            {
                //Populate the PO and save it
                int result = ConvertPOSPOToPO(NewPO, lObjPOSResp);
                if (result <= 0)
                {
                    lRetVal = null;
                }
                else
                {
                    errorCode = result;
                }
            }
            return lRetVal;
        }

        public MobileJob CreateJobForPO(CreateJobForPORequest jobRequest, Guid UserGUID, ref int errorCode)
        {
            short TempShort;
            // Still the flow need to be decided : Alok 1-April-2014
            MobileJob lresponse = new MobileJob();
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            Job _job = new Job();
            Guid OrganizationGUID = _IOrganizationRepository.GetOrganizationIDByUserGUID(UserGUID);
            Market _market = new Market();
            MobilePO lMobilePO = new MobilePO();

            if (!string.IsNullOrEmpty(jobRequest.PONumber))
            {
                //lretString.Append("1.1.PONumber : " + jobRequest.PONumber + "");
                //// Get Job for the PO NUmber
                //Job lJob = _IJobRepository.GetJobByPONumber(jobRequest.PONumber);
                //if (lJob != null)
                //{
                //    if (lJob.StatusCode >= 3)
                //    {
                //        // throw new Exception("Job with the PO Number already in progress");
                //        lresponse = null;
                //    }
                //    else
                //    {
                //        lresponse = ConvertJobForMobile(lJob);
                //    }
                //}
                //else
                {
                    lMobilePO.PONumber = jobRequest.PONumber;
                    //lretString.Append("1.2.Mobile PO : " + new JavaScriptSerializer().Serialize(lMobilePO) + "");
                    POs PO = _IJobRepository.GetPOs(ConvertMobilePOtoPO(lMobilePO));
                    //lretString.Append("1.2.PO : " + new JavaScriptSerializer().Serialize(PO) + "");
                    if (null == PO)
                    {
                        //lretString.Append("1.2 PO is null");
                        if (null == PO)
                        PO = new POs();
                        PO.POGUID = Guid.NewGuid();
                        PO.PONumber = jobRequest.PONumber;
                        PO.POJson = new JavaScriptSerializer().Serialize(jobRequest.POSJson);
                        PO.CreateBy = UserGUID;
                        PO.LastModifiedBy = UserGUID;
                        PO.OrganizationGUID = OrganizationGUID;
                        PO = GetPOSPO(PO, ref errorCode);
                    }
                    if (PO != null)
                    {
                        //lretString.Append("1.2.PONumber is not null : " + jobRequest.PONumber + "");
                        _job.PONumber = PO.PONumber;
                        _job.ServiceAddress = PO.EndCustomerAddress;
                        _job.PreferedStartTime = PO.PreferredDateTime;
                        _job.QuotedCost = PO.EstimatedCost;
                        _job.RegionGUID = PO.RegionGUID;
                        _job.TerritoryGUID = PO.TerritoryGUID;


                        _job.OrganizationGUID = OrganizationGUID;
                        _job.IsSecheduled = false;
                        _job.ManagerUserGUID = UserGUID;
                        _job.AssignedUserGUID = UserGUID;

                        _job.PreferedStartTime = DateTime.UtcNow;
                        _job.PreferedEndTime = DateTime.UtcNow;
                        _job.ActualStartTime = DateTime.UtcNow;
                        _job.CreateBy = UserGUID;
                        _job.LastModifiedBy = UserGUID;
                        _job.StatusCode = 1;
                        _job.SubStatusCode = 1;
                        _job.LocationType = 1;
                        // Need to be discussed before using - Alok 26th May 2014
                        //_job.POSJson = PO.POJson;
                        if (short.TryParse(jobRequest.JobClass, out TempShort))
                        {
                            _job.JobClass = TempShort;
                        }
                        _job.JobName = jobRequest.JobName;
                        if (!string.IsNullOrEmpty(PO.MarketID))
                        {
                            _market = _IMarketRepository.GetMarketByCustomerID(OrganizationGUID, PO.PlaceID, PO.MarketID);
                            if (_market != null)
                            {
                                _job.CustomerStopGUID = _market.MarketGUID;
                                _job.LocationType = 1;
                                _job.CustomerGUID = _market.OwnerGUID;
                                if (!string.IsNullOrEmpty(jobRequest.Latitude) && !string.IsNullOrEmpty(jobRequest.Longitude))
                                {
                                    _job.Latitude = Convert.ToDouble(jobRequest.Latitude);
                                    _job.Longitude = Convert.ToDouble(jobRequest.Longitude);
                                }
                                else
                                {
                                    _job.Latitude = _market.Latitude;
                                    _job.Longitude = _market.Longitude;
                                }
                            }
                            else if (!string.IsNullOrEmpty(jobRequest.Latitude) && !string.IsNullOrEmpty(jobRequest.Longitude))
                            {
                                _job.Latitude = Convert.ToDouble(jobRequest.Latitude);
                                _job.Longitude = Convert.ToDouble(jobRequest.Longitude);
                            }
                        }
                        else if (!string.IsNullOrEmpty(jobRequest.Latitude) && !string.IsNullOrEmpty(jobRequest.Longitude))
                        {
                            _job.Latitude = Convert.ToDouble(jobRequest.Latitude);
                            _job.Longitude = Convert.ToDouble(jobRequest.Longitude);
                        }



                        Job job = _IJobRepository.CreateJobbyPO(_job);
                        int result = 0;
                        if (job != null)
                        {
                           // lretString.Append("Insert Job Progress : " + new JavaScriptSerializer().Serialize(job) + "");
                            // Insert the Job Progress for complete
                            JobProgress _jobProgress = new JobProgress();
                            _jobProgress.JobProgressGUID = Guid.NewGuid();
                            _jobProgress.JobGUID = job.JobGUID;
                            _jobProgress.JobStatus = job.StatusCode;
                            _jobProgress.JobSubStatus = job.SubStatusCode;
                            _jobProgress.StartTime = job.ActualStartTime != null ? job.ActualStartTime : DateTime.UtcNow;
                            _jobProgress.Duration = job.ActualDuration;
                            _jobProgress.Latitude = job.Latitude;
                            _jobProgress.Longitude = job.Longitude;
                            // _jobProgress.LocationMismatch = UploadJobRequest.LocationMismatch;
                            _jobProgress.LastModifiedDate = DateTime.UtcNow;
                            _jobProgress.LastModifiedBy = UserGUID;
                            result = _IJobRepository.InsertJobProgress(_jobProgress);

                        }
                        if (result > 0)
                        {

                            lresponse = ConvertJobForMobile(job, PO.POJson, UserGUID.ToString(), OrganizationGUID.ToString());
                        }
                        else
                        {
                            lresponse = null;
                        }

                    }
                    else
                    {
                        lresponse = null;
                        throw new Exception("Invalid PONumber");
                    }

                }
            }
            return lresponse;
        }
        public MobileJob ConvertJobForMobile(Job item, string POJson)
        {
            try
            {
                MobileJob Job = new MobileJob();
                Job.JobGUID = item.JobGUID;
                Job.JobReferenceNo = item.JobReferenceNo;
                // Job.OrganizationGUID = item.OrganizationGUID;
                Job.RegionGUID = item.RegionGUID;
                Job.TerritoryGUID = item.TerritoryGUID;
                Job.LocationType = item.LocationType;
                // Job.CustomerGUID = item.CustomerGUID;
                Job.CustomerStopGUID = item.CustomerStopGUID;
                Job.ServicePointGUID = item.ServicePointGUID;
                //  Job.ServiceAddress = item.ServiceAddress;
                Job.Cost = item.ActualCost.ToString();
                Job.Latitude = item.Latitude;
                Job.Longitude = item.Longitude;
                Job.AssignedUserGUID = item.AssignedUserGUID;
                //  Job.ManagerUserGUID = item.ManagerUserGUID;
                //   Job.IsActive = item.IsActive;
                //   Job.IsDeleted = item.IsDeleted;
                //    Job.IsUrgent = item.IsUrgent;
                Job.StatusCode = Convert.ToInt32(item.StatusCode);
                Job.SubStatusCode = Convert.ToInt32(item.SubStatusCode);
                // Job.Is = item.IsSecheduled;
                Job.JobName = item.JobName;
                Job.PreferedStartTime = convertdate(Convert.ToDateTime(item.PreferedStartTime));// item.PreferedStartTime.ToString();
                Job.PreferedEndTime = convertdate(Convert.ToDateTime(item.PreferedEndTime)); //item.PreferedEndTime.ToString();
                Job.ScheduledStartTime = convertdate(Convert.ToDateTime(item.ScheduledStartTime)); //item.ScheduledStartTime.ToString();
                Job.ScheduledEndTime = convertdate(Convert.ToDateTime(item.ScheduledEndTime)); //item.ScheduledEndTime.ToString();
                Job.ActualStartTime = convertdate(Convert.ToDateTime(item.ActualStartTime));// item.ActialStartTime.ToString();
                Job.ActualEndTime = convertdate(Convert.ToDateTime(item.ActualEndTime));// item.ActualEndTime.ToString();
                Job.EstimatedDuration = Convert.ToDouble(item.EstimatedDuration);
                //   Job.QuotedDuration = item.QuotedDuration;
                Job.ActualDuration = Convert.ToDouble(item.ActualDuration);
                Job.CostType = Convert.ToInt16(item.CostType);
                Job.PONumber = item.PONumber;
                Job.PONumber = item.PONumber;

                byte[] byteJson = Convert.FromBase64String(POJson);
                string jsString = System.Text.Encoding.UTF8.GetString(byteJson);
                S_POSStoreResponse lObjPOSResp = new JavaScriptSerializer().Deserialize<S_POSStoreResponse>(jsString);

                Job.POSJson = POJson;
                if (item.JobClass != null)
                {
                    Job.JobClass = (short)item.JobClass;
                }
                Job.SignOffRequired = Convert.ToInt32(item.SignOffRequired);
                //   Job.SignoffName = item.SignoffName;
                Job.PictureRequired = Convert.ToInt32(item.PictureRequired);
                Job.LocationSpecific = Convert.ToBoolean(item.LocationSpecific);
                Job.CreateDate = convertdate(Convert.ToDateTime(item.CreateDate));// item.CreateDate.ToString();

                return Job;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        //As i discuss with Alok i have changed the process of getting  field manager details
        // Now i am assigning the current User details as the field manager details
        public MobileJob ConvertJobForMobile(Job item, string POJson, string UserGUID, string OrganizationGUID)
        {
            try
            {
                MobileJob Job = new MobileJob();
                Job.JobGUID = item.JobGUID;
                Job.JobReferenceNo = item.JobReferenceNo;
                // Job.OrganizationGUID = item.OrganizationGUID;
                Job.RegionGUID = item.RegionGUID;
                Job.TerritoryGUID = item.TerritoryGUID;
                Job.LocationType = item.LocationType;
                // Job.CustomerGUID = item.CustomerGUID;
                Job.CustomerStopGUID = item.CustomerStopGUID;
                Job.ServicePointGUID = item.ServicePointGUID;
                //  Job.ServiceAddress = item.ServiceAddress;
                Job.Cost = item.ActualCost.ToString();
                Job.Latitude = item.Latitude;
                Job.Longitude = item.Longitude;
                Job.AssignedUserGUID = item.AssignedUserGUID;
                //  Job.ManagerUserGUID = item.ManagerUserGUID;
                //   Job.IsActive = item.IsActive;
                //   Job.IsDeleted = item.IsDeleted;
                //    Job.IsUrgent = item.IsUrgent;
                Job.StatusCode = Convert.ToInt32(item.StatusCode);
                Job.SubStatusCode = Convert.ToInt32(item.SubStatusCode);
                // Job.Is = item.IsSecheduled;
                Job.JobName = item.JobName;
                Job.PreferedStartTime = convertdate(Convert.ToDateTime(item.PreferedStartTime));// item.PreferedStartTime.ToString();
                Job.PreferedEndTime = convertdate(Convert.ToDateTime(item.PreferedEndTime)); //item.PreferedEndTime.ToString();
                Job.ScheduledStartTime = convertdate(Convert.ToDateTime(item.ScheduledStartTime)); //item.ScheduledStartTime.ToString();
                Job.ScheduledEndTime = convertdate(Convert.ToDateTime(item.ScheduledEndTime)); //item.ScheduledEndTime.ToString();
                Job.ActualStartTime = convertdate(Convert.ToDateTime(item.ActualStartTime));// item.ActialStartTime.ToString();
                Job.ActualEndTime = convertdate(Convert.ToDateTime(item.ActualEndTime));// item.ActualEndTime.ToString();
                Job.EstimatedDuration = Convert.ToDouble(item.EstimatedDuration);
                //   Job.QuotedDuration = item.QuotedDuration;
                Job.ActualDuration = Convert.ToDouble(item.ActualDuration);
                Job.CostType = Convert.ToInt16(item.CostType);
                Job.PONumber = item.PONumber;
                Job.PONumber = item.PONumber;

                byte[] byteJson = Convert.FromBase64String(POJson);
                string jsString = System.Text.Encoding.UTF8.GetString(byteJson);

                //Job.POSJson = POJson;
                if (string.IsNullOrEmpty(Job.PONumber) && !string.IsNullOrEmpty(UserGUID) && !string.IsNullOrEmpty(OrganizationGUID))
                {
                    S_POSStoreResponse lObjPOSResp = new JavaScriptSerializer().Deserialize<S_POSStoreResponse>(jsString);
                    if (lObjPOSResp.store.fieldmanager != null)
                    {
                        GlobalUser globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(UserGUID));
                        UserProfile user = _IUserProfileRepository.GetUserProfileByUserID(new Guid(UserGUID), new Guid(OrganizationGUID));
                        if (globalUser != null && user != null)
                        {
                            S_POSResponseFieldManager fileManager = new S_POSResponseFieldManager();
                            fileManager.email = !string.IsNullOrEmpty(user.EmailID) ? user.EmailID.Trim() : "";
                            fileManager.name = (!string.IsNullOrEmpty(user.FirstName) ? user.FirstName.Trim() : "") + " " + (!string.IsNullOrEmpty(user.LastName) ? user.LastName.Trim() : "");
                            fileManager.phone = !string.IsNullOrEmpty(user.BusinessPhone) ? user.BusinessPhone.Trim() : "";
                            fileManager.userid = !string.IsNullOrEmpty(globalUser.USERID) ? globalUser.USERID.Trim() : "";
                            lObjPOSResp.store.fieldmanager = fileManager;
                            string StoreJSON = new JavaScriptSerializer().Serialize(lObjPOSResp);
                            Job.POSJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(StoreJSON));
                        }
                        else
                            Job.POSJson = POJson;
                    }

                }
                else if (!string.IsNullOrEmpty(Job.PONumber) && !string.IsNullOrEmpty(UserGUID) && !string.IsNullOrEmpty(OrganizationGUID))
                {
                    var lObjPOSResp = (IDictionary<string, object>)new JavaScriptSerializer().Deserialize<dynamic>(jsString);
                    if (lObjPOSResp.ContainsKey("fieldmanager"))
                    {
                        var obj = (IDictionary<string, object>)lObjPOSResp.FindValueByKey("fieldmanager");
                        GlobalUser globalUser = _IGlobalUserRepository.GetGlobalUserByID(new Guid(UserGUID));
                        UserProfile user = _IUserProfileRepository.GetUserProfileByUserID(new Guid(UserGUID), new Guid(OrganizationGUID));
                        if (globalUser != null && user != null)
                        {
                            if (obj.ContainsKey("userid"))
                                obj["userid"] = !string.IsNullOrEmpty(globalUser.USERID) ? globalUser.USERID.Trim() : "";
                            if (obj.ContainsKey("email"))
                                obj["email"] = !string.IsNullOrEmpty(user.EmailID) ? user.EmailID.Trim() : "";
                            if (obj.ContainsKey("name"))
                                obj["name"] = (!string.IsNullOrEmpty(user.FirstName) ? user.FirstName.Trim() : "") + " " + (!string.IsNullOrEmpty(user.LastName) ? user.LastName.Trim() : "");
                            if (obj.ContainsKey("phone"))
                                obj["phone"] = !string.IsNullOrEmpty(user.BusinessPhone) ? user.BusinessPhone.Trim() : "";
                            string StoreJSON = new JavaScriptSerializer().Serialize(lObjPOSResp);
                            Job.POSJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(StoreJSON));
                        }
                        else
                            Job.POSJson = POJson;
                    }
                    else
                    {
                        Job.POSJson = POJson;
                    }
                }
                else
                {
                    Job.POSJson = POJson;
                }

                if (item.JobClass != null)
                {
                    Job.JobClass = (short)item.JobClass;
                }
                Job.SignOffRequired = Convert.ToInt32(item.SignOffRequired);
                //   Job.SignoffName = item.SignoffName;
                Job.PictureRequired = Convert.ToInt32(item.PictureRequired);
                Job.LocationSpecific = Convert.ToBoolean(item.LocationSpecific);
                Job.CreateDate = convertdate(Convert.ToDateTime(item.CreateDate));// item.CreateDate.ToString();

                return Job;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public string GetUploadFileAttachmetsPath(string pJobGUID, string pSessionID)
        {
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());
            OrganizationUsersMap lObjMap = _IUserRepository.GetUserByID(GetUserGUID(pSessionID));

            //string lOrganizatonName = GetOrganizationName(new Guid(pJobGUID));

            string lFilesRoot = AppDomain.CurrentDomain.BaseDirectory + lObjMap.OrganizationGUID + "\\Jobs\\" + pJobGUID;
            Directory.CreateDirectory(lFilesRoot);
            return lFilesRoot;
        }

        public UploadJobAttachmentResponse UploadJobAttachment(UploadJobAttachmentRequest jobAttachmentRequest, Guid UserGUID)
        {
            UploadJobAttachmentResponse lUploadJobAttachmentResponse = new UploadJobAttachmentResponse();
            try
            {
                // Get the Organization
                string lFileName = ProcessTheFileName(jobAttachmentRequest);
                string lEncodedFileContent = jobAttachmentRequest.FileContent;
                //7c654401-ea06-41fb-b2f1-3b1449dff18c_image_1.gif 

                byte[] lDecodedFileContent = Convert.FromBase64String(lEncodedFileContent);

                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/" +
                    jobAttachmentRequest.OrganizationName + "/Jobs/" +
                    jobAttachmentRequest.JobGUID.ToString() + "/" + lFileName, FileMode.Create, FileAccess.Write);

                try
                {
                    fs.Write(lDecodedFileContent, 0, (int)lDecodedFileContent.Length);
                }
                finally
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return lUploadJobAttachmentResponse;
        }

        private string ProcessTheFileName(UploadJobAttachmentRequest jobAttachmentRequest)
        {
            string lFileName = string.Empty;
            string lJobGUID = string.Empty;
            string lOrganizationName = string.Empty;
            Guid JobGUID;
            try
            {
                if (!string.IsNullOrEmpty(jobAttachmentRequest.FileName))
                {
                    #region Comented Code
                    //string[] lFileInfo = jobAttachmentRequest.FileName.Split('_');
                    //if (lFileInfo != null && lFileInfo.Length > 0)
                    //{
                    //    if (string.IsNullOrEmpty(lFileInfo[0]) && new Guid(lFileInfo[0]) != Guid.Empty)
                    //        jobAttachmentRequest.JobGUID = new Guid(lFileInfo[0]);
                    //    lFileName=
                    //}

                    #endregion
                    if (jobAttachmentRequest.FileName.IndexOf('_') > 0)
                    {
                        lJobGUID = jobAttachmentRequest.FileName.Substring(0, jobAttachmentRequest.FileName.IndexOf('_'));
                        if (!string.IsNullOrEmpty(lJobGUID))
                        {
                            if (Guid.TryParse(lJobGUID, out JobGUID))
                            {

                                jobAttachmentRequest.JobGUID = JobGUID;
                                //Get the Organization Name
                                lOrganizationName = GetOrganizationName(JobGUID);
                                lOrganizationName = lOrganizationName.Trim();
                                //Check it the folder exists or else create it
                                int lRetval = CreateJobAttachmentFolder(lOrganizationName, lJobGUID);
                                jobAttachmentRequest.OrganizationName = lOrganizationName;
                            }
                        }
                        lFileName = jobAttachmentRequest.FileName.Substring(jobAttachmentRequest.FileName.IndexOf('_') + 1);

                    }
                    if (jobAttachmentRequest.FileName.IndexOf('.') > 0)
                    {
                        jobAttachmentRequest.FileType = jobAttachmentRequest.FileName.Substring(jobAttachmentRequest.FileName.LastIndexOf('.'));
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return lFileName;
        }

        private int CreateJobAttachmentFolder(string lOrganizationName, string pJobGUID)
        {
            int lRetval = -1;
            try
            {
                if (!string.IsNullOrEmpty(lOrganizationName))
                {
                    DirectoryInfo lOrganizationDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\" + lOrganizationName);
                    if (!lOrganizationDirectoryInfo.Exists)
                    {
                        lOrganizationDirectoryInfo.Create();
                    }
                    DirectoryInfo lOrganizationJobsDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\" + lOrganizationName + "\\Jobs");
                    if (!lOrganizationJobsDirectoryInfo.Exists)
                    {
                        lOrganizationJobsDirectoryInfo.Create();
                    }
                    if (!string.IsNullOrEmpty(pJobGUID))
                    {
                        DirectoryInfo lOrganizationJobGUIDDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\" + lOrganizationName + "\\Jobs" + "\\" + pJobGUID.ToUpper());
                        if (!lOrganizationJobGUIDDirectoryInfo.Exists)
                        {
                            lOrganizationJobGUIDDirectoryInfo.Create();
                        }
                        lRetval = 1;
                    }
                    else
                    {
                        throw new Exception("JobGuid not found in file name");
                    }
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return lRetval;
        }
        //public JMResponse GetJobSchemaByJobLogicalID(string JobLogicalID)
        //{
        //    IJobSchemaRepository _IJobSchemaRepository;
        //    _IJobSchemaRepository = new JobSchemaRepository(new WorkersInMotionJobContext());

        //    JMResponse _jmresponse = new JMResponse();
        //    _jmresponse.JobSchema = new jobschema();
        //    _jmresponse.JobSchema.MJobSchema = new MJobSchema();
        //    _jmresponse.JobSchema.MJobPageSchemaList = new List<MJobPageSchema>();
        //    _jmresponse.JobSchema.MJobItemSchemaList = new List<MJobItemSchema>();
        //    jobschema _jobschema = new jobschema();
        //    if (!string.IsNullOrEmpty(JobLogicalID))
        //    {
        //        JobSchema JobSchema = _IJobSchemaRepository.JobSchemaDetails(new Guid(JobLogicalID));
        //        IList<JobPageSchema> JobPageSchemaList = _IJobSchemaRepository.GetJobPageSchema(new Guid(JobLogicalID)).ToList();
        //        IList<JobItemSchema> JobItemSchemaList = _IJobSchemaRepository.GetJobItemSchema(new Guid(JobLogicalID)).ToList();

        //        if (JobSchema != null)
        //        {

        //            _jmresponse.JobSchema.MJobSchema.JobLogicalID = JobSchema.JobLogicalID;
        //            _jmresponse.JobSchema.MJobSchema.OrganizationGUID = JobSchema.OrganizationGUID;
        //            _jmresponse.JobSchema.MJobSchema.GroupCode = JobSchema.GroupCode;
        //            _jmresponse.JobSchema.MJobSchema.EstimatedDuration = JobSchema.EstimatedDuration;
        //            _jmresponse.JobSchema.MJobSchema.CreatedByUserGUID = JobSchema.CreatedByUserGUID;
        //            _jmresponse.JobSchema.MJobSchema.JobSchemaName = JobSchema.JobSchemaName;
        //            _jmresponse.JobSchema.MJobSchema.Description = JobSchema.Description;
        //            _jmresponse.JobSchema.MJobSchema.CreateDate = JobSchema.CreateDate;
        //            _jmresponse.JobSchema.MJobSchema.IsDeleted = JobSchema.IsDeleted;

        //            if (JobPageSchemaList != null && JobPageSchemaList.Count > 0)
        //            {
        //                foreach (JobPageSchema item in JobPageSchemaList)
        //                {

        //                    MJobPageSchema _mjobpageschema = new MJobPageSchema();
        //                    _mjobpageschema.PageLogicalID = item.PageLogicalID;
        //                    _mjobpageschema.JobLogicalID = item.JobLogicalID;
        //                    _mjobpageschema.PageSchemaName = item.PageSchemaName;
        //                    _mjobpageschema.CanRepeat = item.CanRepeat;
        //                    _mjobpageschema.CreateDate = item.CreateDate;

        //                    _jmresponse.JobSchema.MJobPageSchemaList.Add(_mjobpageschema);
        //                }
        //            }
        //            if (JobItemSchemaList != null && JobItemSchemaList.Count > 0)
        //            {
        //                foreach (JobItemSchema item in JobItemSchemaList)
        //                {
        //                    MJobItemSchema _mjobitemschema = new MJobItemSchema();
        //                    _mjobitemschema.ItemLogicalID = item.ItemLogicalID;
        //                    _mjobitemschema.JobLogicalID = item.JobLogicalID;
        //                    _mjobitemschema.PageLogicalID = item.PageLogicalID;
        //                    _mjobitemschema.SortOrder = item.SortOrder;
        //                    _mjobitemschema.ItemName = item.ItemName;
        //                    _mjobitemschema.ItemControlType = item.ItemControlType;
        //                    _mjobitemschema.ItemCaptureTime = item.ItemCaptureTime;
        //                    _mjobitemschema.ItemValueType = item.ItemValueType;
        //                    _mjobitemschema.ItemValue = item.ItemValue;
        //                    _mjobitemschema.IsRequired = item.IsRequired;
        //                    _mjobitemschema.CanView = item.CanView;
        //                    _mjobitemschema.CanEdit = item.CanEdit;
        //                    _mjobitemschema.CanRepeat = item.CanRepeat;
        //                    _mjobitemschema.Createdate = item.Createdate;
        //                    _mjobitemschema.ItemOrder = item.ItemOrder;

        //                    _jmresponse.JobSchema.MJobItemSchemaList.Add(_mjobitemschema);
        //                }
        //            }
        //        }
        //    }
        //    return _jmresponse;
        //}

        //public JMResponse GetJobSchemaByGroupID(string GroupCode)
        //{
        //    IJobSchemaRepository _IJobSchemaRepository;
        //    _IJobSchemaRepository = new JobSchemaRepository(new WorkersInMotionJobContext());

        //    JMResponse _jmresponse = new JMResponse();
        //    _jmresponse.LJobSchema = new List<jobschemaList>();
        //    jobschema _jobschema = new jobschema();
        //    if (!string.IsNullOrEmpty(GroupCode))
        //    {
        //        IList<JobSchema> JobSchemaList = _IJobSchemaRepository.GetJobSchemabyGroupCode(new Guid(GroupCode)).ToList();
        //        if (JobSchemaList != null && JobSchemaList.Count > 0)
        //        {
        //            jobschemaList _Jtemp = new jobschemaList();
        //            _Jtemp.MJobSchemaList = new List<jobschema>();
        //            foreach (JobSchema item in JobSchemaList)
        //            {
        //                jobschema jschema = GetJobSchemaByJobLogicalID(item.JobLogicalID.ToString()).JobSchema;
        //                _Jtemp.MJobSchemaList.Add(jschema);
        //            }
        //            _jmresponse.LJobSchema.Add(_Jtemp);
        //        }
        //    }
        //    return _jmresponse;
        //}

        //public JMResponse GetJobByFilter(JobRequest jobrequest)
        //{
        //    IJobRepository _IJobRepository;
        //    _IJobRepository = new JobRepository(new WorkersInMotionJobContext());
        //    JMResponse _jmresponse = new JMResponse();
        //    _jmresponse.JobLIst = new List<MJob>();
        //    if (jobrequest != null)
        //    {
        //        string lastdownloadtime = jobrequest.LastDownloadTime;
        //        string userguid = jobrequest.UserGUID;
        //        string regioncode = jobrequest.RegionGUID;
        //        string territorycode = jobrequest.TerritoryGUID;
        //        string groupcode = jobrequest.GroupGUID;

        //        if (!string.IsNullOrEmpty(lastdownloadtime))
        //        {
        //            IList<Job> _job = new List<Job>();
        //            _job = _IJobRepository.GetJobByLastDownloadtTime(lastdownloadtime).ToList();
        //            _jmresponse.JobLIst = getJobList(_job);
        //        }
        //        else if (!string.IsNullOrEmpty(regioncode) && !string.IsNullOrEmpty(territorycode))
        //        {
        //            IList<Job> _job = new List<Job>();
        //            _job = _IJobRepository.GetJobByRegionandTerritory(new Guid(regioncode), new Guid(territorycode)).ToList();
        //            _jmresponse.JobLIst = getJobList(_job);
        //        }
        //        else if (!string.IsNullOrEmpty(userguid))
        //        {
        //            IList<Job> _job = new List<Job>();
        //            _job = _IJobRepository.GetJobByUserGUID(new Guid(userguid)).ToList();
        //            _jmresponse.JobLIst = getJobList(_job);
        //        }
        //        else if (!string.IsNullOrEmpty(groupcode))
        //        {
        //            IList<Job> _job = new List<Job>();
        //            _job = _IJobRepository.GetJobByGroupGUIDforClient(new Guid(groupcode)).ToList();
        //            _jmresponse.JobLIst = getJobList(_job);
        //        }
        //    }

        //    return _jmresponse;
        //}

        //public JMResponse GetJobByOrganization(string SessionID)
        //{
        //    IOrganizationRepository _IOrganizationRepository;
        //    _IOrganizationRepository = new OrganizationRepository(new WorkersInMotionDB());
        //    IJobRepository _IJobRepository;
        //    _IJobRepository = new JobRepository(new WorkersInMotionJobContext());
        //    JMResponse _jmresponse = new JMResponse();
        //    _jmresponse.JobLIst = new List<MJob>();
        //    if (!string.IsNullOrEmpty(SessionID))
        //    {
        //        string organizationID = _IOrganizationRepository.GetOrganizationID(SessionID);
        //        if (!string.IsNullOrEmpty(organizationID))
        //        {
        //            IList<Job> _job = new List<Job>();
        //            _job = _IJobRepository.GetjobByOrganizationGUID(new Guid(organizationID)).ToList();
        //            _jmresponse.JobLIst = getJobList(_job);
        //        }
        //    }

        //    return _jmresponse;
        //}

        //public IList<MJob> getJobList(IList<Job> _job)
        //{
        //    IList<MJob> _mjoblist = new List<MJob>();
        //    if (_job != null && _job.Count > 0)
        //    {
        //        foreach (Job item in _job)
        //        {
        //            MJob _mjob = new MJob();
        //            _mjob.JobIndexGUID = item.JobIndexGUID;
        //            _mjob.JobLogicalID = item.JobLogicalID;
        //            _mjob.JobID = item.JobID;
        //            _mjob.JobReferenceNo = item.JobReferenceNo;
        //            _mjob.IsDeleted = item.IsDeleted;
        //            _mjob.UserGUID = item.UserGUID;
        //            _mjob.OrganizationGUID = item.OrganizationGUID;
        //            _mjob.JobName = item.JobName;
        //            _mjob.CustType = item.CustType;
        //            _mjob.CustGUID = item.CustGUID;
        //            _mjob.JobNote = item.JobNote;
        //            _mjob.SortOrder = item.SortOrder;
        //            _mjob.ServiceStartTime = item.ServiceStartTime;
        //            _mjob.ServiceEndTime = item.ServiceEndTime;
        //            _mjob.ActualStartTime = item.ActualStartTime;
        //            _mjob.ActualEndTime = item.ActualEndTime;
        //            _mjob.Status = item.Status;
        //            _mjob.IsScheduled = item.IsScheduled;
        //            _mjob.GPSLatitude = item.GPSLatitude;
        //            _mjob.GPSLongitude = item.GPSLongitude;
        //            _mjob.GPSAltitude = item.GPSAltitude;
        //            _mjob.CreateUserGUID = item.CreateUserGUID;
        //            _mjob.CreateDate = item.CreateDate;
        //            _mjob.EstimatedDuration = item.EstimatedDuration;
        //            _mjob.PreferredStartTime = item.PreferredStartTime;
        //            _mjob.PreferredEndTime = item.PreferredEndTime;
        //            _mjob.RegionCode = item.RegionCode;
        //            _mjob.TerritoryCode = item.TerritoryCode;
        //            _mjob.DeptCode = item.DeptCode;
        //            _mjob.GroupCode = item.GroupCode;
        //            _mjob.ServicePointGUID = item.ServicePointGUID;
        //            _mjob.StopsGUID = item.StopsGUID;
        //            _mjob.ServiceAddress = item.ServiceAddress;
        //            _mjob.Instruction = item.Instruction;
        //            _mjob.SignoffName = item.SignoffName;
        //            _mjob.SignoffSignature = item.SignoffSignature;
        //            _mjob.SignoffSignatureContent = item.SignoffSignatureContent;
        //            _mjob.WorkImage = item.WorkImage;
        //            _mjob.WorkImageContent = item.WorkImageContent;

        //            _mjoblist.Add(_mjob);
        //        }
        //    }
        //    return _mjoblist.ToList();
        //}

        //public void UploadJob(MUploadJob _uploadJob)
        //{
        //    IJobRepository _IJobRepository;
        //    _IJobRepository = new JobRepository(new WorkersInMotionJobContext());
        //    if (_uploadJob != null)
        //    {
        //        Job _job = new Job();
        //        _job.JobIndexGUID = _uploadJob.JobIndexGUID;
        //        _job.Status = _uploadJob.Status;
        //        _job.ActualStartTime = _uploadJob.ActualStartTime;
        //        int jobupdateresult = _IJobRepository.UpdateJobFromClient(_job);

        //        if (jobupdateresult > 0 && _uploadJob.JobPageList != null && _uploadJob.JobPageList.Count > 0)
        //        {
        //            foreach (MJobPage jobpage in _uploadJob.JobPageList)
        //            {
        //                JobPage _jobPage = new JobPage();
        //                _jobPage.JobPageGUID = Guid.NewGuid();
        //                _jobPage.JobIndexGUID = _job.JobIndexGUID;
        //                _jobPage.PageLogicalID = jobpage.PageLogicalID;
        //                _jobPage.IsDefault = false;
        //                _jobPage.PageName = jobpage.PageName;
        //                _jobPage.SortOrder = jobpage.SortOrder;
        //                _jobPage.PageDescription = jobpage.PageDescription;
        //                _jobPage.CreateDate = DateTime.UtcNow;

        //                _IJobRepository.InsertJobpagefromClient(_jobPage);
        //                int jobpageresult = _IJobRepository.Save();

        //                if (jobpageresult > 0 && jobpage.JobItemList != null && jobpage.JobItemList.Count > 0)
        //                {
        //                    foreach (MJobItem jobitem in jobpage.JobItemList)
        //                    {
        //                        JobItem _jobitem = new JobItem();
        //                        _jobitem.JobItemGUID = Guid.NewGuid();
        //                        _jobitem.JobIndexGUID = _job.JobIndexGUID;
        //                        _jobitem.JobPageGUID = _jobPage.JobPageGUID;
        //                        _jobitem.ItemLogicalID = jobitem.ItemLogicalID;
        //                        _jobitem.ItemValueGUID = jobitem.ItemValueGUID;
        //                        _jobitem.ContentType = jobitem.ContentType;
        //                        _jobitem.FileContent = jobitem.FileContent;
        //                        _jobitem.GPSLatitude = jobitem.GPSLatitude;
        //                        _jobitem.GPSLongitude = jobitem.GPSLongitude;
        //                        _jobitem.GPSAltitude = jobitem.GPSAltitude;
        //                        _jobitem.ItemCaptureTime = DateTime.UtcNow;
        //                        _jobitem.ItemValue = jobitem.ItemValue;
        //                        _jobitem.CreateUserGUID = jobitem.CreateUserGUID;
        //                        _jobitem.Createdate = DateTime.UtcNow;

        //                        _IJobRepository.InsertJobItemfromClient(_jobitem);
        //                        _IJobRepository.Save();
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _IJobRepository.UpdateJobFromClient(_job);
        //        }

        //    }

        //}


        public string GetOrganizationName(Guid JobGUID)
        {
            string lOrganizationName = string.Empty;
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            return (_IJobRepository.GetOrganizationName(JobGUID));
        }


        public int DeleteJobs(DeleteJobRequest pDeleteJobRequest, string pSessionID)
        {
            int lRetval = -1;
            try
            {
                UserRepository _IUserRepository = new UserRepository(new WorkersInMotionDB());
                OrganizationUsersMap lObjMap = _IUserRepository.GetUserByID(GetUserGUID(pSessionID));

                JobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());
                Job lJob = _IJobRepository.GetJobByID(pDeleteJobRequest.JobGUID);
                // Get the Organization Name by Job GUID
                pDeleteJobRequest.OrganizationName = GetOrganizationName(pDeleteJobRequest.JobGUID);
                if (lJob != null)
                {
                    _IJobRepository.DeleteJob(pDeleteJobRequest.JobGUID);
                    //Job updated delete successfully



                    // Delete the Files and Folders
                    DirectoryInfo lJobDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "/" +
                    lObjMap.OrganizationGUID + "/Jobs/" +
                    pDeleteJobRequest.JobGUID.ToString());
                    if (lJobDirectory.Exists)
                    {
                        lJobDirectory.Delete();
                        lRetval = 2;
                    }
                    else
                    {
                        lRetval = 1;
                    }


                }
                else
                {
                    // Job Not Found
                    lRetval = -2;
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return lRetval;
        }
        public LatLong GetLatLngCode(string address1, string address2, string city, string state, string country, string zipcode)
        {
            string address = address1 + " ," + address2 + " ," + city + " ," + state + " ," + country + " ," + zipcode;
            address = address.Substring(0, address.LastIndexOf(","));

            string urlAddress = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + HttpUtility.UrlEncode(address) + "&sensor=false";
            string[] returnValue = new string[2];
            string tzone = string.Empty;
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
                throw ex;
                // Process an error action here if needed 
            }
            return new LatLong(Convert.ToDouble(returnValue[0]), Convert.ToDouble(returnValue[1]));

        }
        public double getTimeZone(double lat, double lon)
        {
            double tzone = 0;
            try
            {
                string _timeZone = "https://maps.googleapis.com/maps/api/timezone/xml?location=" + lat + "," + lon + "&timestamp=" + DateTime.UtcNow.TimeOfDay.Ticks + "&sensor=true";
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(_timeZone);
                XmlNodeList objXmlNodeList = objXmlDocument.SelectNodes("/TimeZoneResponse");
                foreach (XmlNode objXmlNode in objXmlNodeList)
                {
                    if (objXmlNode.ChildNodes.Count >= 4)
                        tzone = Convert.ToDouble(objXmlNode.ChildNodes.Item(1).InnerText) / 60;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tzone;
        }


        public MobileJob CreateVisitForPO(CreateVisitForPORequest PO, Guid pUserGUID, ref int errorCode)
        {

            Guid lOrganizationGUID = GetOrganizationGUID(pUserGUID);

            // Check the PO Number for the Client and Stop and Organization exists or not
            POs lPOs = new POs();
            PO.OrganizationGUID = lOrganizationGUID;
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            MobileJob lresponse = new MobileJob();
            lPOs = _IJobRepository.GetPOsForClientStoreOrganization(PO);
            // if PO not exists
            if (PO == null)
            {
                //Create PO
                IPORepository _IPORepository;
                _IPORepository = new PORepository(new WorkersInMotionDB());
                _IPORepository.CreatePO(PO);
            }
            Job _job = new Job();
            _job.PONumber = PO.PONumber;
            _job.ServiceAddress = PO.EndCustomerAddress;
            _job.PreferedStartTime = PO.PreferredDateTime;
            _job.QuotedCost = PO.EstimatedCost;
            _job.RegionGUID = PO.RegionGUID;
            _job.TerritoryGUID = PO.TerritoryGUID;


            _job.OrganizationGUID = lOrganizationGUID;
            _job.IsSecheduled = false;
            _job.ManagerUserGUID = pUserGUID;
            _job.AssignedUserGUID = pUserGUID;

            _job.PreferedStartTime = DateTime.UtcNow;
            _job.PreferedEndTime = DateTime.UtcNow;
            _job.ActualStartTime = DateTime.UtcNow;
            _job.CreateBy = pUserGUID;
            _job.LastModifiedBy = pUserGUID;
            _job.StatusCode = 1;
            _job.SubStatusCode = 1;
            _job.LocationType = 1;
            // Need to be discussed before using - Alok 26th May 2014
            //_job.POSJson = PO.POJson;
            _job.JobClass = PO.JobClass;
            _job.JobName = PO.JobName;
            if (!string.IsNullOrEmpty(PO.MarketID))
            {
                Market _market = _IMarketRepository.GetMarketByCustomerID(lOrganizationGUID, PO.PlaceID, PO.MarketID);
                if (_market != null)
                {
                    _job.CustomerStopGUID = _market.MarketGUID;
                    _job.LocationType = 1;
                    _job.CustomerGUID = _market.OwnerGUID;
                    if (!string.IsNullOrEmpty(PO.Latitude) && !string.IsNullOrEmpty(PO.Longitude))
                    {
                        _job.Latitude = Convert.ToDouble(PO.Latitude);
                        _job.Longitude = Convert.ToDouble(PO.Longitude);
                    }
                    else
                    {
                        _job.Latitude = _market.Latitude;
                        _job.Longitude = _market.Longitude;
                    }
                }
                else if (!string.IsNullOrEmpty(PO.Latitude) && !string.IsNullOrEmpty(PO.Longitude))
                {
                    _job.Latitude = Convert.ToDouble(PO.Latitude);
                    _job.Longitude = Convert.ToDouble(PO.Longitude);
                }
            }
            else if (!string.IsNullOrEmpty(PO.Latitude) && !string.IsNullOrEmpty(PO.Longitude))
            {
                _job.Latitude = Convert.ToDouble(PO.Latitude);
                _job.Longitude = Convert.ToDouble(PO.Longitude);
            }
            Job job = _IJobRepository.CreateJob(_job);
            int result = 0;
            if (job != null)
            {
                // Insert the Job Progress for complete
                JobProgress _jobProgress = new JobProgress();
                _jobProgress.JobProgressGUID = Guid.NewGuid();
                _jobProgress.JobGUID = job.JobGUID;
                _jobProgress.JobStatus = job.StatusCode;
                _jobProgress.JobSubStatus = job.SubStatusCode;
                _jobProgress.StartTime = job.ActualStartTime != null ? job.ActualStartTime : DateTime.UtcNow;
                _jobProgress.Duration = job.ActualDuration;
                _jobProgress.Latitude = job.Latitude;
                _jobProgress.Longitude = job.Longitude;
                //_jobProgress.LocationMismatch=UploadJobRequest.LocationMismatch;
                _jobProgress.LastModifiedDate = DateTime.UtcNow;
                _jobProgress.LastModifiedBy = pUserGUID;
                result = _IJobRepository.InsertJobProgress(_jobProgress);

            }
            if (result > 0)
            {
                lresponse = ConvertJobForMobile(job, PO.POJson);
            }
            else
            {
                lresponse = null;
            }
            //lresponse = ConvertJobForMobile(_IJobRepository.CreateJob(_job), PO.POJson);
            // create the job for the PO , client and stop in the WIM System and return the Mobile Job

            return lresponse;
        }

        private SiteVisit ConvertToSiteVisit(Job job, DateTime ActualStartTime, DateTime ActualEndTime)
        {
            try
            {
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());
                IUserRepository _IUserRepository;
                _IUserRepository = new UserRepository(new WorkersInMotionDB());
                IGlobalUserRepository _IGlobalUserRepository;
                _IGlobalUserRepository = new GlobalUserRepository(new WorkersInMotionDB());
                IUserProfileRepository _IUserProfileRepository;
                _IUserProfileRepository = new UserProfileRepository(new WorkersInMotionDB());
                IPORepository _IPORepository;
                _IPORepository = new PORepository(new WorkersInMotionDB());


                DateTime Datecontent, Datecontent1, Datecontent2;
                SiteVisit _site = new SiteVisit();
                _site.JobGUID = job.JobGUID;
                _site.JobName = job.JobName;
                _site.PONumber = job.PONumber;
                if (job.RegionGUID != null && !string.IsNullOrEmpty(job.RegionGUID.ToString()))
                {
                    _site.RegionGUID = new Guid(job.RegionGUID.ToString());
                    _site.RegionName = _IRegionRepository.GetRegionNameByRegionGUID(_site.RegionGUID);
                }
                else
                {
                    _site.RegionName = "";
                }
                if (job.TerritoryGUID != null && !string.IsNullOrEmpty(job.TerritoryGUID.ToString()))
                {
                    _site.TerritoryGUID = new Guid(job.TerritoryGUID.ToString());
                    _site.TerritoryName = _ITerritoryRepository.GetTerritoryNameByTerritoryGUID(_site.TerritoryGUID);
                }
                else
                {
                    _site.TerritoryName = "";
                }
                _site.StatusCode = job.StatusCode != null ? Convert.ToInt32(job.StatusCode) : 0;

                _site.Status = _IJobRepository.GetStatusName(_site.StatusCode);

                //if (job.ActualStartTime != null && DateTime.TryParse(job.ActualStartTime.ToString(), out Datecontent1))
                //{
                //    _site.ActualStartTime = Datecontent1.ToString("MM/dd/yy hh:mm tt");
                //}
                //if (job.ActualEndTime != null && DateTime.TryParse(job.ActualEndTime.ToString(), out Datecontent2))
                //{
                //    _site.ActualEndTime = Datecontent2.ToString("MM/dd/yy hh:mm tt");
                //}

                if (ActualStartTime != null && DateTime.TryParse(ActualStartTime.ToString(), out Datecontent1))
                {
                    _site.ActualStartTime = Datecontent1.ToString("MM/dd/yy hh:mm tt");
                }
                if (ActualEndTime != null && DateTime.TryParse(ActualEndTime.ToString(), out Datecontent2))
                {
                    _site.ActualEndTime = Datecontent2.ToString("MM/dd/yy hh:mm tt");
                }
                if (DateTime.TryParse(job.LastModifiedDate.ToString(), out Datecontent))
                {

                    _site.Date = Datecontent.ToString("MM/dd/yy hh:mm tt");
                }
                //Prabhu--While creating Job using device they are not sending location mismatch flag,so this code will not work
                //JobProgress ljobprogress = _IJobRepository.GetJobProgressMismatch(job.JobGUID, _site.StatusCode);
                //if (ljobprogress != null)
                //    _site.LocationMismatch = ljobprogress.LocationMismatch != null ? Convert.ToBoolean(ljobprogress.LocationMismatch) : false;
                //else
                //    _site.LocationMismatch = false;



                _site.CustomerStopGUID = job.CustomerStopGUID != null ? new Guid(job.CustomerStopGUID.ToString()) : Guid.Empty;


                _site.LocationMismatch = LocationMismatch(job.JobGUID, _site.CustomerStopGUID);

                if (_site.CustomerStopGUID != Guid.Empty)
                {
                    Market _Market = _IMarketRepository.GetMarketByID(_site.CustomerStopGUID);
                    if (_Market != null)
                    {

                        _site.CustomerFirstName = _Market.FirstName;
                        _site.CustomerLastName = _Market.LastName;
                        _site.CustomerStopName = _Market.MarketName;
                        _site.MarketID = _Market.MarketID;
                        if (!string.IsNullOrEmpty(_Market.RMUserID))
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.RMUserID, job.OrganizationGUID.ToString());
                            if (_globalUser != null)
                            {
                                UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                if (_userprofile != null)
                                {
                                    _site.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                }
                                else
                                {
                                    _site.RMName = "";
                                }
                            }

                        }
                        else
                        {
                            _site.RMName = "";
                        }
                        if (!string.IsNullOrEmpty(_Market.FMUserID))
                        {
                            GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.FMUserID, job.OrganizationGUID.ToString());
                            if (_globalUser != null)
                            {
                                UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                if (_userprofile != null)
                                {
                                    _site.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                }
                                else
                                {
                                    _site.FMName = "";
                                }
                            }

                        }
                        else
                        {
                            _site.FMName = "";
                        }
                    }
                    else
                    {
                        _site.CustomerStopName = "";
                        _site.MarketID = "";
                        _site.RMName = "";
                        _site.FMName = "";
                        _site.CustomerFirstName = "";
                        _site.CustomerLastName = "";
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

                            _site.CustomerFirstName = _Market.FirstName;
                            _site.CustomerLastName = _Market.LastName;
                            _site.CustomerStopName = _Market.MarketName;
                            _site.MarketID = _Market.MarketID;
                            if (!string.IsNullOrEmpty(_Market.RMUserID))
                            {
                                GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.RMUserID, job.OrganizationGUID.ToString());
                                if (_globalUser != null)
                                {
                                    UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                    if (_userprofile != null)
                                    {
                                        _site.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                    }
                                    else
                                    {
                                        _site.RMName = "";
                                    }
                                }

                            }
                            else
                            {
                                _site.RMName = "";
                            }
                            if (!string.IsNullOrEmpty(_Market.FMUserID))
                            {
                                GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_Market.FMUserID, job.OrganizationGUID.ToString());
                                if (_globalUser != null)
                                {
                                    UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                    if (_userprofile != null)
                                    {
                                        _site.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                    }
                                    else
                                    {
                                        _site.FMName = "";
                                    }
                                }

                            }
                            else
                            {
                                _site.FMName = "";
                            }
                        }
                        else
                        {
                            _site.CustomerStopName = "";
                            _site.MarketID = _po.MarketID;

                            if (!string.IsNullOrEmpty(_po.RMUserID))
                            {
                                GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_po.RMUserID, job.OrganizationGUID.ToString());
                                if (_globalUser != null)
                                {
                                    UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                    if (_userprofile != null)
                                    {
                                        _site.RMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                    }
                                    else
                                    {
                                        _site.RMName = "";
                                    }
                                }

                            }
                            else
                            {
                                _site.RMName = "";
                            }
                            if (!string.IsNullOrEmpty(_po.FMUserID))
                            {
                                GlobalUser _globalUser = _IGlobalUserRepository.GetGlobalUserByUserID(_po.FMUserID, job.OrganizationGUID.ToString());
                                if (_globalUser != null)
                                {
                                    UserProfile _userprofile = _IUserProfileRepository.GetUserProfileByUserID(_globalUser.UserGUID, job.OrganizationGUID);
                                    if (_userprofile != null)
                                    {
                                        _site.FMName = _userprofile.FirstName + " " + _userprofile.LastName;
                                    }
                                    else
                                    {
                                        _site.FMName = "";
                                    }
                                }

                            }
                            else
                            {
                                _site.FMName = "";
                            }

                            //_site.RMName = "";
                            //_site.FMName = "";
                            _site.CustomerFirstName = "";
                            _site.CustomerLastName = "";
                        }

                    }
                    else
                    {
                        _site.CustomerStopName = "";
                        _site.MarketID = "";
                        _site.RMName = "";
                        _site.FMName = "";
                        _site.CustomerFirstName = "";
                        _site.CustomerLastName = "";
                    }
                }
                else
                {
                    _site.CustomerStopName = "";
                    _site.MarketID = "";
                    _site.RMName = "";
                    _site.FMName = "";
                    _site.CustomerFirstName = "";
                    _site.CustomerLastName = "";
                }
                return _site;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private bool LocationMismatch(Guid JobGUID, Guid CustomerStopGUID)
        {
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            bool result = false;
            List<JobProgress> jobProgressList = new List<JobProgress>();
            jobProgressList = _IJobRepository.GetJobProgress(JobGUID);
            Market pMarket = _IMarketRepository.GetMarketByID(CustomerStopGUID);
            if (pMarket != null && jobProgressList != null && jobProgressList.Count >= 2)
            {
                foreach (JobProgress jobProgress in jobProgressList)
                {
                    if (pMarket.Latitude != null && jobProgress.Latitude != null && pMarket.Longitude != null && jobProgress.Longitude != null)
                    {
                        if (pMarket.Latitude == jobProgress.Latitude && pMarket.Longitude == jobProgress.Longitude)
                        {
                            result = false;
                        }
                        else
                        {
                            result = true;
                            break;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        private JobFormNew JobFormJsonConvert(string jobfomJson, string urlname, string jobguid, Guid OrganizationGUID)
        {
            try
            {
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());
                IUserRepository _IUserRepository;
                _IUserRepository = new UserRepository(new WorkersInMotionDB());
                JobFormNew pJobFormView = (!string.IsNullOrEmpty(jobfomJson)) ? new JavaScriptSerializer().Deserialize<JobFormNew>(jobfomJson) : null;

                if (pJobFormView != null)
                {

                    if (pJobFormView.Values != null)
                    {
                        pJobFormView.FormValues = new List<JobFormValueDetails>();
                        //for (int i = 0; i < pJobFormView.Values.Count; i++)
                        foreach (JobFormValues pFormValues in pJobFormView.Values)
                        {
                            //JobFormValues pFormValues = pJobFormView.Values[i];
                            JobFormValueDetails pFormDetails = new JobFormValueDetails();
                            string[] Controls = pFormValues.ControlID.Split('_');
                            if (Controls.Length > 2)
                            {
                                int controlid, controltype;
                                pFormDetails.FormID = Controls[0];
                                if (int.TryParse(Controls[1], out controlid))
                                {
                                    pFormDetails.ControlID = controlid;
                                }
                                else
                                {
                                    pFormDetails.ControlID = 0;
                                }
                                if (int.TryParse(Controls[2], out controltype))
                                {
                                    pFormDetails.ControlType = (ControlType)controltype;
                                }
                                else
                                {
                                    pFormDetails.ControlType = 0;
                                }

                            }
                            int parentid;
                            pFormDetails.Value = pFormValues.Value;
                            pFormDetails.ControlLabel = pFormValues.ControlLabel;
                            if (int.TryParse(pFormValues.parentID, out parentid))
                            {
                                pFormDetails.parentID = parentid;
                            }
                            else
                            {
                                pFormDetails.parentID = 0;
                            }
                            pFormDetails.controlParentLabel = pFormValues.controlParentLabel;
                            pFormDetails.ValueID = pFormValues.ValueID;
                            pFormDetails.currentValueID = pFormValues.currentValueID;

                            pFormDetails.ImagePath = System.Configuration.ConfigurationManager.AppSettings.Get(urlname).ToString() + OrganizationGUID.ToString() + "/Jobs/" + pJobFormView.JobGUID;
                            pFormDetails.OrganizationGUID = OrganizationGUID.ToString();
                            pFormDetails.JobGUID = pJobFormView.JobGUID;
                            pJobFormView.FormValues.Add(pFormDetails);
                        }
                        pJobFormView.JobGUID = jobguid;
                    }
                }
                if (pJobFormView != null && pJobFormView.FormValues != null && pJobFormView.FormValues.Count > 0)
                {
                    pJobFormView.FormValues.OrderBy(x => x.ControlID);
                }
                if (!string.IsNullOrEmpty(jobguid))
                {
                    Job pjob = _IJobRepository.GetJobByID(new Guid(jobguid));
                    if (pjob != null)
                    {
                        List<JobProgress> jobProgressList = new List<JobProgress>();
                        jobProgressList = _IJobRepository.GetJobProgress(pjob.JobGUID);
                        List<string> coordinate = new List<string>();
                        int i = 0;

                        Market pMarket = pjob.CustomerStopGUID != null ? _IMarketRepository.GetMarketByID(new Guid(pjob.CustomerStopGUID.ToString())) : null;
                        if (pMarket != null)
                        {

                            pJobFormView.CoordinateList = new List<CoOrdinates>();

                            if (pMarket.Latitude != null && pMarket.Longitude != null)
                            {
                                CoOrdinates pCoOrdinates = new CoOrdinates();
                                pCoOrdinates.Latitude = Convert.ToDouble(pMarket.Latitude);
                                pCoOrdinates.Longitude = Convert.ToDouble(pMarket.Longitude);
                                pCoOrdinates.Address = pMarket.AddressLine1 + "<br><br/>" + pMarket.AddressLine2;
                                pCoOrdinates.City = pMarket.City;
                                pCoOrdinates.State = pMarket.State;
                                pCoOrdinates.Country = pMarket.Country;
                                pCoOrdinates.JobName = pjob.JobName;
                                pCoOrdinates.StoreName = pMarket.MarketName.ToString();
                                pCoOrdinates.Count = i;
                                i++;
                                pJobFormView.CoordinateList.Add(pCoOrdinates);

                                coordinate.Add(pMarket.Latitude.ToString() + "~" + pMarket.Longitude.ToString() + "~store~" + pCoOrdinates.StoreName.ToString());
                            }

                        }


                        if (jobProgressList != null && jobProgressList.Count > 0)
                        {
                            jobProgressList = jobProgressList.OrderBy(x => x.JobStatus).ToList();
                            if (pMarket == null)
                            {
                                pJobFormView.CoordinateList = new List<CoOrdinates>();
                                //jobProgressList = jobProgressList.OrderByDescending(x => x.JobStatus).ToList();
                            }
                            foreach (JobProgress pJobProgress in jobProgressList)
                            {
                                if (pJobProgress.Latitude != null && pJobProgress.Longitude != null)
                                {
                                    CoOrdinates pCoOrdinates = new CoOrdinates();
                                    pCoOrdinates.Latitude = Convert.ToDouble(pJobProgress.Latitude);
                                    pCoOrdinates.Longitude = Convert.ToDouble(pJobProgress.Longitude);
                                    pCoOrdinates.JobName = pjob.JobName;
                                    pCoOrdinates.Count = i;
                                    if (pJobProgress.JobStatus != null && Convert.ToInt32(pJobProgress.JobStatus) == 1)
                                    {

                                        pCoOrdinates.StartTime = pJobProgress.StartTime.ToString();
                                        pCoOrdinates.StartTime = !string.IsNullOrEmpty(pCoOrdinates.StartTime) ? Convert.ToDateTime(pCoOrdinates.StartTime).ToString("MM/dd/yy hh:mm tt") : "";

                                        coordinate.Add(pCoOrdinates.Latitude.ToString() + "~" + pCoOrdinates.Longitude.ToString() + "~start~" + pCoOrdinates.JobName.ToString() + "~" + pCoOrdinates.StartTime.ToString());
                                    }
                                    else
                                    {
                                        pCoOrdinates.EndTime = pJobProgress.StartTime.ToString();
                                        pCoOrdinates.EndTime = !string.IsNullOrEmpty(pCoOrdinates.EndTime) ? Convert.ToDateTime(pCoOrdinates.EndTime).ToString("MM/dd/yy hh:mm tt") : "";

                                        coordinate.Add(pCoOrdinates.Latitude.ToString() + "~" + pCoOrdinates.Longitude.ToString() + "~stop~" + pCoOrdinates.JobName.ToString() + "~" + pCoOrdinates.EndTime.ToString());
                                    }
                                    pJobFormView.CoordinateList.Add(pCoOrdinates);

                                }
                                i++;
                            }



                        }


                        //for displaying google static map in pdf
                        //first need to display job end coordinates
                        //second need to display job start coordinates
                        //Third need to display store coordinates
                        if (pJobFormView.CoordinateList != null && pJobFormView.CoordinateList.Count > 0)
                        {
                            pJobFormView.CoordinateList = pJobFormView.CoordinateList.OrderByDescending(x => x.Count).ToList();
                        }
                    }
                }
                return pJobFormView;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        private JobFormHeading GetJobFormDetails(Job _job, DateTime ActualStartTime, DateTime ActualEndTime)
        {
            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            if (_job != null)
            {
                Place _place = _job.CustomerGUID != null ? _IPlaceRepository.GetPlaceByID(new Guid(_job.CustomerGUID.ToString())) : null;
                Market _market = _job.CustomerStopGUID != null ? _IMarketRepository.GetMarketByID(new Guid(_job.CustomerStopGUID.ToString())) : null;
                if (_place != null && _market != null)
                {
                    JobFormHeading JobFormHeading = new JobFormHeading();
                    JobFormHeading.JobGUID = _job.JobGUID.ToString();
                    JobFormHeading.JobName = _job.JobName;
                    JobFormHeading.PlaceName = _place.PlaceName;
                    JobFormHeading.PlaceID = _place.PlaceID;
                    JobFormHeading.MarketName = _market.MarketName;
                    JobFormHeading.MarketID = _market.MarketID;
                    JobFormHeading.MarketAddress = (string.IsNullOrEmpty(_market.AddressLine1) ? "" : _market.AddressLine1 + ",") +
            (string.IsNullOrEmpty(_market.AddressLine2) ? "" : _market.AddressLine2 + ",") +
            (string.IsNullOrEmpty(_market.City) ? "" : _market.City + ",") +
            (string.IsNullOrEmpty(_market.State) ? "" : _market.State + ",") +
            (string.IsNullOrEmpty(_market.ZipCode) ? "" : _market.ZipCode);
                    //JobFormHeading.CheckInTime = _job.ActualStartTime.ToString();
                    JobFormHeading.CheckInTime = ActualStartTime.ToString();
                    JobFormHeading.CheckInTime = !string.IsNullOrEmpty(JobFormHeading.CheckInTime) ? Convert.ToDateTime(JobFormHeading.CheckInTime).ToString("MM/dd/yy hh:mm tt") : "";
                    //JobFormHeading.CheckOutTime = _job.ActualEndTime.ToString();
                    JobFormHeading.CheckOutTime = ActualEndTime.ToString();
                    JobFormHeading.CheckOutTime = !string.IsNullOrEmpty(JobFormHeading.CheckOutTime) ? Convert.ToDateTime(JobFormHeading.CheckOutTime).ToString("MM/dd/yy hh:mm tt") : "";
                    JobFormHeading.Status = _IJobRepository.GetStatusName(_job.StatusCode != null ? Convert.ToInt32(_job.StatusCode) : 6);
                    JobFormHeading.PoNumber = _job.PONumber;
                    return JobFormHeading;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        private string GetJobFormHTML(JobFormNew JobFormNew)
        {
            try
            {
                List<string> parentIDList = new List<string>();
                List<string> controlIDList = new List<string>();
                int imagecount = 0;
                List<WorkersInMotion.Model.ViewModel.JobFormValueDetails> FormValues = new List<WorkersInMotion.Model.ViewModel.JobFormValueDetails>();
                StringBuilder sbJobForm = new StringBuilder();
                //sbJobForm.Append("<div style='page-break-before:always'>&nbsp;</div>");
                sbJobForm.Append("<html>");
                sbJobForm.Append("<head><script type='text/javascript' src='http://maps.googleapis.com/maps/api/js?sensor=false'></script></head>");
                sbJobForm.Append("<body>");
                #region Generate Job Form
                if (JobFormNew.JobFormHeading != null)
                {
                    sbJobForm.Append("<div id='" + JobFormNew.JobFormHeading.JobGUID + "' name='" + JobFormNew.JobFormHeading.JobGUID + "'>");
                    sbJobForm.Append("<div align='center'>");
                    sbJobForm.Append("<table style='width:100%' align='center' cellpadding='0'>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td colspan='2' style='font-size:18px;font-family:verdana;font-weight:bold;text-align:center;'>" + JobFormNew.JobFormHeading.JobName + "&nbsp;Report</td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td colspan='2' style='font-size:18px;font-family:verdana;font-weight:bold;text-align:center;'>&nbsp;</td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Client Name :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.PlaceName + "</span> </td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>PO Number :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.PoNumber + "</span> </td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Store ID :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketID + "</span> </td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>Check-In :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.CheckInTime + "</span> </td>");

                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 65%;' align='left'>Store Name : <span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketName + "</span></td>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;width: 35%;' align='left'>Check-Out : <span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.CheckOutTime + "</span></td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("<tr>");
                    sbJobForm.Append("<td style='font-size:12px;font-family:verdana;font-weight:bold;text-align:left;' align='left'>Address :<span style='font-weight:normal;'>" + JobFormNew.JobFormHeading.MarketAddress + "</span> </td>");
                    sbJobForm.Append("</tr>");
                    sbJobForm.Append("</table>");
                    sbJobForm.Append("</div>");
                    sbJobForm.Append("<a name='" + JobFormNew.JobFormHeading.JobGUID + "' style='text-decoration: none;'>&nbsp;</a>");
                    sbJobForm.Append("<hr style='boder:1px solid black;width:100%'/>");
                }
                else
                {
                    sbJobForm.Append("<div>");
                }

                if (JobFormNew != null && JobFormNew.FormValues != null)
                {
                    foreach (var item1 in JobFormNew.FormValues)
                    {
                        if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -100) || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Email") || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Phone"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                //if (!parentIDList.Contains(item1.parentID.ToString()))
                                //{
                                //    parentIDList.Add(item1.parentID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) || ((one.ControlLabel == "Email" || one.ControlLabel == "Phone") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");

                            }
                            //}
                        }

                        //else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -101) || (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.controlParentLabel == "Date"))
                        //{
                        //    if (!parentIDList.Contains(item1.parentID.ToString()))
                        //    {
                        //        parentIDList.Add(item1.parentID.ToString());
                        //        sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                        //        sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                        //        sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                        //        sbJobForm.Append("<tr>");
                        //        sbJobForm.Append("<td align='left'>");
                        //        foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && ((one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID == -101) || (one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && one.ControlLabel == "Date")))))
                        //        {
                        //            if (!controlIDList.Contains(items.ControlID.ToString()))
                        //            {

                        //                controlIDList.Add(items.ControlID.ToString());
                        //                if (items.ControlLabel == "Year" || items.ControlLabel == "Day")
                        //                {
                        //                    sbJobForm.Append(" <span style='padding-left:0px;line-height:0px;font-size:10px;'>-" + items.Value + "</span>");
                        //                }
                        //                else
                        //                {
                        //                    sbJobForm.Append(" <span style='padding-left:10px;line-height:0px;font-size:10px;'>" + items.Value + "</span>");
                        //                }


                        //            }
                        //        }
                        //        sbJobForm.Append("</td>");
                        //        sbJobForm.Append("</tr>");
                        //        sbJobForm.Append("</table>");
                        //        sbJobForm.Append("</div>");

                        //    }
                        //}

                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) && (item1.ControlLabel == "Region"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => ((one.ControlLabel == "Region" || one.ControlLabel == "Market") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + items.ControlLabel + "</label><br></br>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT) && (item1.ControlLabel == "Store Number # *" || item1.ControlLabel == "Store Address"))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => ((one.ControlLabel == "Store Number # *" || one.ControlLabel == "Store Address") && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + items.ControlLabel + "</label><br></br>");
                                        sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }

                        }

                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT && item1.parentID < 0))
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<p style='color:black;font-weight:bold;font-size:12px;margin-left:1px;'>" + item1.controlParentLabel + "</p>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='0'>");
                                sbJobForm.Append("<tr>");
                                //sbJobForm.Append("<td align='left'>");
                                //sbJobForm.Append("</td>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        if (!controlIDList.Contains(items.ControlID.ToString()))
                                        {

                                            controlIDList.Add(items.ControlID.ToString());
                                            sbJobForm.Append("<td align='left' style='width:25%'>");
                                            sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + items.Value + "</span>");
                                            sbJobForm.Append("</td>");

                                        }

                                    }
                                }

                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");

                            }
                        }
                        else if ((item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_DROP_DOWN) && item1.controlParentLabel != "Email" && item1.controlParentLabel != "Phone")
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                if (!string.IsNullOrEmpty(item1.ControlLabel) && !string.IsNullOrEmpty(item1.Value))
                                {
                                    sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                    sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>&nbsp;" + item1.ControlLabel + "</label>");
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    sbJobForm.Append("<tr>");
                                    sbJobForm.Append("<td align='left' style='width:40%'>");
                                    sbJobForm.Append("<span style='padding-left:0px;line-height:20px;font-size:10px;'>" + item1.Value + "</span>");
                                    // sbJobForm.Append("<p style='padding:0px 10px 0px 10px; border-bottom: 1px solid gray; border-left: 1px solid gray; border-right: 1px solid gray; line-height: 2px;'>&nbsp;</p>");
                                    sbJobForm.Append("</td>");
                                    sbJobForm.Append("</tr>");
                                    sbJobForm.Append("</table>");
                                    sbJobForm.Append("</div>");
                                }
                            }
                        }

                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_CHECKBOX)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_CHECKBOX).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {

                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left' style='width:25%'>");
                                                if (items.Value == "true")
                                                {

                                                    sbJobForm.Append("<img src='" + ServerURL + "/assets/img/checkbox_yes.png' alt='logo'  width='20px' height='20px' />");
                                                }
                                                else
                                                {
                                                    sbJobForm.Append("<img src='" + ServerURL + "/assets/img/checkbox_no.png' alt='logo'  width='20px' height='20px' />");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;vertical-align:top'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");

                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_MULTITEXT)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left'>");
                                sbJobForm.Append("<tr>");
                                sbJobForm.Append("<td align='left'>");
                                sbJobForm.Append("<label style='font-size:12px;'>" + item1.ControlLabel + "</label>");

                                sbJobForm.Append("<label>:</label>");

                                sbJobForm.Append("<textarea cols='20' rows='2' style='background-color:#FFFFFD;font-size:10px;'>" + item1.Value + "</textarea>");
                                sbJobForm.Append("</td>");
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }
                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_LABEL)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<table style='width:100%' align='left'>");
                                sbJobForm.Append("<tr>");
                                sbJobForm.Append("<td align='left'>");
                                sbJobForm.Append("<label style='font-size:12px;'>" + item1.ControlLabel + "</label>");

                                sbJobForm.Append("<label>:</label>");

                                sbJobForm.Append("<label style='font-size:10px;'>&nbsp;&nbsp;" + item1.Value + "</label>&nbsp;");
                                sbJobForm.Append("</td>");
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");


                                sbJobForm.Append("</div>");
                            }
                        }

                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_GROUP)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div id='Div' class='leftbar-heading'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;border-bottom:10px;;font-size:12px;'>" + item1.ControlLabel + "</label>");
                                sbJobForm.Append("<table style='width:100%' align='left' cellpadding='5'>");
                                sbJobForm.Append("<tr>");
                                foreach (var items in JobFormNew.FormValues.Where(one => (one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_BUTTON)))
                                {
                                    if (!controlIDList.Contains(items.ControlID.ToString()))
                                    {

                                        controlIDList.Add(items.ControlID.ToString());

                                        sbJobForm.Append("<td align='left' style='width:25%'>");
                                        if (items.Value == "true")
                                        {
                                            sbJobForm.Append("<img src='" + ServerURL + "/assets/img/1403876293_radiobutton_yes.png' alt='logo'  width='20px' height='20px' />");
                                            //sbJobForm.Append("<input type='radio' disabled='disabled' checked />");
                                        }
                                        else
                                        {
                                            // sbJobForm.Append("<input type='radio' disabled='disabled' />");
                                            sbJobForm.Append("<img src='" + ServerURL + "/assets/img/1403876299_radiobutton_no.png' alt='logo'  width='20px' height='20px' />");
                                        }
                                        sbJobForm.Append("<span style='color:black;border-bottom:10px;;font-size:10px;vertical-align:top;'>" + items.ControlLabel + "</span>&nbsp;");
                                        sbJobForm.Append("</td>");

                                    }
                                }
                                sbJobForm.Append("</tr>");
                                sbJobForm.Append("</table>");
                                sbJobForm.Append("</div>");
                            }
                        }


                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_SWITCH)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                controlIDList.Add(item1.ControlID.ToString());
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.ControlLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_RADIO_BUTTON).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {

                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];

                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());

                                                sbJobForm.Append("<td align='left' style='width:25%'>");
                                                if (items.Value == "true")
                                                {
                                                    sbJobForm.Append("<img src='" + ServerURL + "/assets/img/1403876293_radiobutton_yes.png' alt='logo'  width='20px' height='20px' />");
                                                }
                                                else
                                                {
                                                    sbJobForm.Append("<img src='" + ServerURL + "/assets/img/1403876299_radiobutton_no.png' alt='logo'  width='20px' height='20px' />");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;vertical-align:top;'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }

                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }

                                sbJobForm.Append("<div id='Div'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    if (!parentIDList.Contains(item1.ControlID.ToString()))
                                    {
                                        parentIDList.Add(item1.ControlID.ToString());
                                        sbJobForm.Append("<table style='width:100%' align='left'>");
                                        for (int i = 0; i < FormValues.Count; i = i + 3)
                                        {
                                            sbJobForm.Append("<tr>");
                                            for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                            {
                                                WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                                if (!controlIDList.Contains(items.ControlID.ToString()))
                                                {
                                                    controlIDList.Add(items.ControlID.ToString());


                                                    sbJobForm.Append("<td align='left'>");
                                                    if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                    {
                                                        sbJobForm.Append("<div>");
                                                        sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                        sbJobForm.Append("</div>");
                                                    }
                                                    sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.ControlLabel + "</span>&nbsp;");
                                                    sbJobForm.Append("</td>");

                                                }
                                            }
                                            sbJobForm.Append("</tr>");
                                        }
                                        sbJobForm.Append("</table>");
                                    }
                                }
                                sbJobForm.Append("</div>");

                                sbJobForm.Append("</div>");
                            }
                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE)
                        {
                            //Logger.Debug("Start with control type as Image");
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {
                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left'>");
                                                if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                {
                                                    sbJobForm.Append("<div>");
                                                    //string base64String = string.Empty;
                                                    //using (var webClient = new WebClient())
                                                    //{
                                                    //    byte[] imageAsByteArray = getResizedImage(items.OrganizationGUID, items.JobGUID, items.Value, 120, 120);
                                                    //    base64String = Convert.ToBase64String(imageAsByteArray, 0, imageAsByteArray.Length);
                                                    //    Logger.Debug(base64String);
                                                    //}
                                                    //sbJobForm.Append("<img src='" + string.Format("data:" + getContentType(items.ImagePath + '/' + items.Value) + "/jpeg;base64,{0}", base64String) + "' alt='logo'  width='120px' height='120px' />");

                                                    //sbJobForm.Append("<img src='" + Server.MapPath("~/assets/img/sidebar-toggler-purple.jpg") + "' alt='logo'  width='20px' height='20px' />");
                                                    sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                    //Logger.Debug(items.ImagePath + '/' + items.Value);
                                                    sbJobForm.Append("</div>");
                                                }
                                                sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.ControlLabel + "</span>&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");

                                // Logger.Debug("End with control type as Image");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE_DESC)
                        {
                            // Logger.Debug("Start with control type as Image Description");
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.controlParentLabel + "</label>");

                                FormValues = JobFormNew.FormValues.Where(one => one.parentID == item1.parentID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_IMAGE_DESC).ToList();

                                if (FormValues.Count() > 0)
                                {
                                    sbJobForm.Append("<table style='width:100%' align='left'>");
                                    for (int i = 0; i < FormValues.Count; i = i + 3)
                                    {
                                        sbJobForm.Append("<tr>");
                                        for (int j = i; j < i + 3 && j < FormValues.Count; j++)
                                        {
                                            WorkersInMotion.Model.ViewModel.JobFormValueDetails items = (WorkersInMotion.Model.ViewModel.JobFormValueDetails)FormValues[j];
                                            if (!controlIDList.Contains(items.ControlID.ToString()))
                                            {
                                                controlIDList.Add(items.ControlID.ToString());


                                                sbJobForm.Append("<td align='left'>");
                                                if (!string.IsNullOrEmpty(items.ImagePath) && !string.IsNullOrEmpty(items.Value))
                                                {
                                                    sbJobForm.Append("<div>");
                                                    //string base64String = string.Empty;
                                                    //using (var webClient = new WebClient())
                                                    //{
                                                    //    byte[] imageAsByteArray = getResizedImage(items.OrganizationGUID, items.JobGUID, items.Value, 120, 120);
                                                    //    base64String = Convert.ToBase64String(imageAsByteArray, 0, imageAsByteArray.Length);
                                                    //}
                                                    //sbJobForm.Append("<img src='" + string.Format("data:" + getContentType(items.ImagePath + '/' + items.Value) + "/jpeg;base64,{0}", base64String) + "' alt='logo'  width='120px' height='120px' />");

                                                    // sbJobForm.Append("<img src='" + Server.MapPath("~/" + items.OrganizationGUID + "/Jobs/" + items.JobGUID + "/" + items.Value + "") + "' alt='logo'  width='120px' height='120px' />");
                                                    sbJobForm.Append("<img src='" + items.ImagePath + '/' + items.Value + "' alt='logo'  width='120px' height='120px' />");
                                                    // Logger.Debug(items.ImagePath + '/' + items.Value);
                                                    sbJobForm.Append("</div>");
                                                }
                                                foreach (var item in JobFormNew.FormValues.Where(one => one.parentID == items.ControlID && one.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_EDIT))
                                                {
                                                    if (!controlIDList.Contains(item.ControlID.ToString()))
                                                    {

                                                        controlIDList.Add(item.ControlID.ToString());
                                                        sbJobForm.Append("<span style='color:black;font-size:10px;'>" + item.ControlLabel + "</span>");
                                                    }
                                                }
                                                sbJobForm.Append("&nbsp;");
                                                sbJobForm.Append("</td>");
                                            }
                                        }
                                        sbJobForm.Append("</tr>");
                                    }
                                    sbJobForm.Append("</table>");
                                }
                                sbJobForm.Append("</div>");

                                //Logger.Debug("End with control type as Image Description");
                            }

                        }
                        else if (item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_ISSUES_TO_REPORT || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_OPEN_CHARGEBACKS || item1.ControlType == WorkersInMotion.Model.ViewModel.ControlType.CONTROL_TYPE_UNSOLD_QUOTES)
                        {
                            if (!controlIDList.Contains(item1.ControlID.ToString()))
                            {
                                sbJobForm.Append("<div style='padding-top:5px;padding-bottom:5px'>");
                                sbJobForm.Append("<label style='color:black;font-weight:bold;font-size:12px;'>" + item1.ControlLabel + "</label>");


                                sbJobForm.Append("<div id='div_" + item1.ControlID + "'>");
                                if (JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID).ToList().Count > 0)
                                {
                                    foreach (var items in JobFormNew.FormValues.Where(one => one.parentID == item1.ControlID))
                                    {
                                        if (!controlIDList.Contains(items.ControlID.ToString()))
                                        {
                                            controlIDList.Add(items.ControlID.ToString());
                                            sbJobForm.Append("<table style='width:100%' align='left'>");
                                            sbJobForm.Append("<tr>");
                                            sbJobForm.Append("<td align='left' style='width:100px'>");
                                            if (items.currentValueID > 0)
                                            {
                                                sbJobForm.Append("<span style='color:black;font-weight:bold;font-size:10px;'>" + items.Value + "</span>");
                                            }
                                            else
                                            {
                                                sbJobForm.Append("<span style='color:black;font-size:10px;'>" + items.Value + "</span>");
                                            }
                                            sbJobForm.Append("&nbsp;");
                                            sbJobForm.Append("</td>");
                                            sbJobForm.Append("</tr>");
                                            sbJobForm.Append("</table>");
                                        }
                                    }
                                }
                                else
                                {
                                    sbJobForm.Append("<div align='left' style='width:100px;font-size:10px;'>None</div>");
                                }
                                sbJobForm.Append("</div>");
                                sbJobForm.Append("</div>");
                            }
                        }

                    }
                }
                sbJobForm.Append("</div>");
                #endregion
                string marker = string.Empty;
                if (JobFormNew != null && JobFormNew.CoordinateList != null && JobFormNew.CoordinateList.Count > 0)
                {

                    int i = 0;
                    string address = string.Empty;
                    foreach (CoOrdinates items in JobFormNew.CoordinateList)
                    {
                        i++;
                        if (i == JobFormNew.CoordinateList.Count)
                        {
                            if (!string.IsNullOrEmpty(items.EndTime))
                                marker = marker + "markers=size:small%7Ccolor:0xcc0000%7C" + items.Latitude + "," + items.Longitude;
                            else if (!string.IsNullOrEmpty(items.StartTime))
                                marker = marker + "markers=size:mid%7Ccolor:0x33d100%7C" + items.Latitude + "," + items.Longitude;
                            else if (!string.IsNullOrEmpty(items.StoreName))
                                marker = marker + "markers=color:0xff6600%7C" + items.Latitude + "," + items.Longitude;
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(items.EndTime))
                                marker = marker + "markers=size:small%7Ccolor:0xcc0000%7C" + items.Latitude + "," + items.Longitude + "&";
                            else if (!string.IsNullOrEmpty(items.StartTime))
                                marker = marker + "markers=size:mid%7Ccolor:0x33d100%7C" + items.Latitude + "," + items.Longitude + "&";
                            else if (!string.IsNullOrEmpty(items.StoreName))
                                marker = marker + "markers=color:0xff6600%7C" + items.Latitude + "," + items.Longitude + "&";


                        }
                        if (!string.IsNullOrEmpty(items.City))
                        {
                            address = address + "+" + items.City;
                        }
                        if (!string.IsNullOrEmpty(items.State))
                        {
                            address = address + "+" + items.State;
                        }
                        if (!string.IsNullOrEmpty(items.Country))
                        {
                            address = address + "+" + items.Country;
                        }
                        //markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284

                    }
                    sbJobForm.Append("<div style='page-break-before:always'>");
                    sbJobForm.Append("<img src='http://maps.googleapis.com/maps/api/staticmap?center=" + address + "&zoom=10&size=1000x1000&maptype=roadmap&" + marker + "'></img>");
                    sbJobForm.Append("</div>");
                }

                sbJobForm.Append("</body>");
                sbJobForm.Append("</html>");
                return sbJobForm.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return "";
            }
        }

        public int GeneratePDF(string id, DateTime ActualStartTime, DateTime ActualEndTime)
        {

            try
            {
                Logger.Debug("JMServer-GeneratePDF-Start");
                IJobRepository _IJobRepository;
                _IJobRepository = new JobRepository(new WorkersInMotionDB());
                StringBuilder pVisit = new StringBuilder();
                Job _job = _IJobRepository.GetJobByID(new Guid(id));
                if (_job != null)
                {
                    SiteVisit _sitevisit = ConvertToSiteVisit(_job, ActualStartTime, ActualEndTime);
                    if (_sitevisit != null)
                    {

                        JobFormNew pJobFormView = new JobFormNew();

                        if (_job != null)
                        {
                            if (!string.IsNullOrEmpty(_job.JobForm))
                            {
                                pJobFormView = JobFormJsonConvert(_job.JobForm, "PDFImageURL", _job.JobGUID.ToString(), _job.OrganizationGUID);
                            }
                            if (pJobFormView != null && pJobFormView.FormValues != null && pJobFormView.FormValues.Count > 0)
                            {
                                JobFormHeading JobFormHeading = GetJobFormDetails(_job, ActualStartTime, ActualEndTime);
                                if (JobFormHeading != null)
                                {
                                    pJobFormView.JobFormHeading = JobFormHeading;
                                }
                                else
                                {
                                    pJobFormView.JobFormHeading = null;
                                }
                                pJobFormView.FormValues.OrderBy(x => x.ControlID);
                                pVisit.Append(GetJobFormHTML(pJobFormView));
                            }


                        }
                        if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "JobPDF"))
                        {
                            System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "JobPDF");
                        }
                        string filePath = AppDomain.CurrentDomain.BaseDirectory + "JobPDF/WIMVisit" + _job.PONumber + ".pdf";
                        string fileName = "SiteVisit" + _job.PONumber + "-" + DateTime.Now.ToString("ddMMyyyy-HHmm") + ".pdf";
                        Logger.Debug("File Name-" + fileName);
                        Document document = new Document(PageSize.A4, 70, 55, 40, 25);
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        document.Open();

                        TextReader txtReader = new StringReader(pVisit.ToString());
                        var xmlWorkerHelper = XMLWorkerHelper.GetInstance();
                        xmlWorkerHelper.ParseXHtml(writer, document, txtReader);
                        document.Close();
                        fs.Close();
                        if (System.IO.File.Exists(filePath))
                        {
                            Logger.Debug("JMServer-GeneratePDF - File Exists");

                            byte[] content = System.IO.File.ReadAllBytes(filePath);
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://rts.textomation.com/upload?dir=rts&filename=" + fileName + "&storeNum=" + _sitevisit.MarketID + "&ticketNum=" + _sitevisit.PONumber + "&desc=" + _job.JobName + "&nbsp;Report");
                            // HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(" http://localhost:57676/Default.aspx?dir=rts&filename=" + fileName + "&storeNum=" + _sitevisit.MarketID + "&ticketNum=" + _sitevisit.PONumber + "&desc=" + _job.JobName + "&nbsp;Report");

                            //HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:6100/JM/TestPDF?dir=rts&filename=" + fileName + "&storeNum=3333&ticketNum=586&desc=Site Visit Job");
                            httpWebRequest.ContentType = "multipart/form-data";
                            httpWebRequest.Method = "POST";
                            httpWebRequest.KeepAlive = true;
                            httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
                            Stream requestStream = httpWebRequest.GetRequestStream();
                            requestStream.Write(content, 0, content.Length);
                            requestStream.Close();
                            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                            Logger.Debug("JMServer-GeneratePDF" + response.StatusCode);
                            Logger.Debug("JMServer-GeneratePDF-End");

                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                                Logger.Debug("JMServer-GeneratePDF - File Deleted");
                            }


                            return (int)response.StatusCode;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return 0;
            }
            //var encoding = ASCIIEncoding.ASCII;
            //using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            //{
            //    string responseText = reader.ReadToEnd();
            //}

            //System.Web.HttpContext.Current.Response.Write(document);
        }

        public Guid GetOrganizationGUID(Guid UserGUID)
        {
            Guid lOrganizationGUID = Guid.Empty;

            IJobRepository _IJobRepository;
            _IJobRepository = new JobRepository(new WorkersInMotionDB());
            Guid? lTempOrganizationGUID = _IJobRepository.GetOrganizationGUID(UserGUID);
            if (lTempOrganizationGUID != null)
            {
                lOrganizationGUID = (Guid)lTempOrganizationGUID;
            }

            return lOrganizationGUID;
        }
    }


}
