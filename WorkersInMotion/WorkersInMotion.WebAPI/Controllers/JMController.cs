using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkersInMotion.WebAPI.Models.MobileModel;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;


using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.Remoting.Contexts;
using WorkersInMotion.WebAPI.Models.MobileModel.POSModels;
using System.Text;
using System.Web.Script.Serialization;
using AttributeRouting;
using WorkersInMotion.WebAPI.Models.MobileModel.Service;

namespace WorkersInMotion.WebAPI.Controllers
{
    [RoutePrefix("JM")]
    public class JMController : BaseAPIController
    {
        private IJMServer _IJMServer;
        public JMController()
        {
            _IJMServer = new JMServer();
        }
        public JMController(IJMServer JMServer)
        {
            _IJMServer = JMServer;
        }

        #region GetJobs
        [HttpPost]
        [ActionName("GetJobs")]
        public HttpResponseMessage GetJobs([FromBody]string UserGUID)
        {
            Logger.Debug("Inside JM Controller- GetJobs");
            if (UserGUID != null)
                Logger.Debug("UserGUID: " + UserGUID.ToString());
            JMResponse jmResponse = new JMResponse();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        if (!string.IsNullOrEmpty(UserGUID))
                        {
                            jmResponse = _IJMServer.GetJobs(new Guid(UserGUID));
                        }
                        else
                        {
                            jmResponse = _IJMServer.GetJobs(_IJMServer.GetUserGUID(SessionID));
                        }
                        if (jmResponse != null)
                        {

                            //Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(jmResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, jmResponse);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to get job Information";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Unable to get SessionID in Header";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region GetOpenJobs
        [HttpPost]
        [ActionName("GetOpenJobs")]
        public HttpResponseMessage GetOpenJobs([FromBody]string UserGUID)
        {
            Logger.Debug("Inside JM Controller- GetOpenJobs ");
            if (UserGUID != null)
                Logger.Debug("UserGUID: " + UserGUID.ToString());
            JMResponse jmResponse = new JMResponse();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        if (!string.IsNullOrEmpty(UserGUID))
                        {

                            Logger.Debug("User GUID provided is : " + SessionID.ToString());
                            jmResponse = _IJMServer.GetOpenJobs(new Guid(UserGUID));
                        }
                        else
                        {
                            Logger.Debug("User GUID from Session is: " + SessionID.ToString());
                            jmResponse = _IJMServer.GetOpenJobs(_IJMServer.GetUserGUID(SessionID));
                        }
                        if (jmResponse != null)
                        {
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(jmResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, jmResponse);
                        }
                        else
                        {
                            Logger.Error("Unable to get Open Job Information");
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to get Open Job Information";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        Logger.Error("Session has expired, please login again");
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    Logger.Error("Unable to get SessionID in Header");
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.InnerException.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region UpdateJobStatus
        [HttpPost]
        [ActionName("UpdateJobStatus")]
        public HttpResponseMessage UpdateJobStatus([FromBody]UpdateJobStatusRequest JMOwnJobRequest)
        {
            Logger.Debug("Inside JM Controller- UpdateJobStatus ");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(JMOwnJobRequest));
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        int result = _IJMServer.UpdateJobStatus(JMOwnJobRequest, UserGUID);
                        if (result > 0)
                        {
                            Logger.Debug("Response: No response In Body,Process is Success");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            switch (result)
                            {
                                case 0:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable insert Job Progress";
                                    break;
                                case -1:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable to update Job Open status";
                                    break;
                                case -2:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable to update Job Assigned status";
                                    break;
                                case -3:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable to update Job Progress status";
                                    break;
                                case -4:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable to update Job Abandon status";
                                    break;
                                case -5:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable to update Job Suspended status";
                                    break;
                                case -6:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable to update Job Complete status";
                                    break;
                                default:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Failure";
                                    break;
                            }

                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region UploadJobs
        [HttpPost]
        [ActionName("UploadJobs")]
        public HttpResponseMessage UploadJobs([FromBody]UploadJobRequestNew UploadJobRequest)
        {
            Logger.Debug("Inside JM Controller- UploadJobs ");
            Logger.Debug("Inside JM Controller- " + UploadJobRequest.ActualStartTime);
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(UploadJobRequest));
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    // SessionID = Request.Headers.LastOrDefault().Value.First();
                    SessionID = UploadJobRequest.SessionGUID;
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        int result = _IJMServer.UploadJobs(UploadJobRequest, UserGUID);
                        if (result > 0)
                        {
                            Logger.Debug("Response: No response In Body,Process is Success");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else if (result == -1)
                        {
                            Logger.Debug("Response: Unable to generate PDF");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to Upload Jobs";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion


        #region GetConfigurations
        [HttpPost]
        [ActionName("GetConfigurations")]
        public HttpResponseMessage GetConfigurations()
        {
            Logger.Debug("Inside JM Controller- GetConfigurations");
            Logger.Debug("Request: No Request Structure.");
            ErrorResponse error = new ErrorResponse();
            ConfigurationResponse lresponse = new ConfigurationResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        lresponse = _IJMServer.GetConfiguration(UserGUID);
                        if (lresponse != null)
                        {
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lresponse));
                            response.Add("Configurations", lresponse);
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to get Configuration Details";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region GetList
        [HttpPost]
        [ActionName("GetList")]
        public HttpResponseMessage GetList()
        {
            Logger.Debug("Inside JM Controller- GetList ");
            Logger.Debug("Request: No Request Structure.");
            ErrorResponse error = new ErrorResponse();
            ConfigurationResponse lresponse = new ConfigurationResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "List is not available";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region GetJobStatistics
        [HttpPost]
        [ActionName("GetJobStatistics")]
        public HttpResponseMessage GetJobStatistics([FromBody]JobStatusRequest JobStatusRequest)
        {
            Logger.Debug("Inside JM Controller- GetJobStatus ");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(JobStatusRequest));
            JobStatusResponse lResponse = new JobStatusResponse();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        lResponse = _IJMServer.GetJobStatus(JobStatusRequest);
                        if (lResponse != null)
                        {
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to get Job Statistics Details";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region UploadJobFormData
        [HttpPost]
        [ActionName("UploadJobFormData")]
        public HttpResponseMessage UploadJobFormData([FromBody] JobFormDataRequest JobFormDataRequest)
        {
            Logger.Debug("Inside JM Controller- UploadJobFormData");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(JobFormDataRequest));
            ErrorResponse error = new ErrorResponse();
            ConfigurationResponse lresponse = new ConfigurationResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        int result = _IJMServer.InsertJobFormData(JobFormDataRequest);
                        if (result > 0)
                        {
                            Logger.Debug("Response: No response In Body,Process is Success");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to Upload Job Form Data";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion


        #region AssignJob
        [HttpPost]
        [ActionName("AssignJob")]
        public HttpResponseMessage AssignJob([FromBody]AssignJobRequest AssignJobRequest)
        {
            Logger.Debug("Inside JM Controller- AssignJob");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(AssignJobRequest));
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            int lRetVal = 0;
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        System.Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        lRetVal = _IJMServer.AssignJob(AssignJobRequest, UserGUID);
                        if (lRetVal > 0)
                        {
                            Logger.Debug("Response: No response In Body,Process is Success");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            switch (lRetVal)
                            {
                                case 0:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable to assigne the job to the user";
                                    break;
                                case -1:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Already assigned to another worker";
                                    break;
                                case -2:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Job already started so can't be assigned";
                                    break;
                                case -5:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Unable to update assign job record";
                                    break;
                                default:
                                    error.ErrorCode = HttpStatusCode.InternalServerError;
                                    error.ErrorMessage = "Failure";
                                    break;
                            }

                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region UpdateForumStatus
        [HttpPost]
        [ActionName("UpdateForumStatus")]
        public HttpResponseMessage UpdateForumStatus([FromBody] JobForumRequest JobForumRequest)
        {
            Logger.Debug("Inside JM Controller- UpdateForumStatus");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(JobForumRequest));
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        System.Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        int lRetVal = _IJMServer.UpdateForumStatus(JobForumRequest, UserGUID);
                        Logger.Debug("lRetVal: " + lRetVal);
                        if (lRetVal > 0)
                        {
                            Logger.Debug("Response: No response In Body,Process is Success");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to Update Job Forum Status";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region CreateForumEntries
        [HttpPost]
        [ActionName("CreateForumEntries")]
        public HttpResponseMessage CreateForumEntries([FromBody]CreateForumEntryRequest CreateForumEntryRequest)
        {
            Logger.Debug("Inside JM Controller- CreateForumEntries");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(CreateForumEntryRequest));
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        System.Guid LastModifiedUserGUID = _IJMServer.GetUserGUID(SessionID);
                        if (_IJMServer.InsertForumEntries(CreateForumEntryRequest, LastModifiedUserGUID) > 0)
                        {
                            Logger.Debug("Response: No response In Body,Process is Success");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to Insert Job Forum Entries";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region GetForumEntries
        [HttpPost]
        [ActionName("GetForumEntries")]
        public HttpResponseMessage GetForumEntries([FromBody]Guid JobGUID)
        {
            Logger.Debug("Inside JM Controller- GetForumEntries");
            if (JobGUID != null)
                Logger.Debug("JobGUID: " + JobGUID.ToString());
            Forum lResponse = new Forum();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        lResponse = _IJMServer.GetForumEntries(JobGUID);
                        if (lResponse != null)
                        {
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            response.Add("Forum", lResponse);
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.BadRequest;
                            error.ErrorMessage = "Unable to Get Forum Entries";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region GetPO
        [HttpPost]
        [ActionName("GetPO")]
        public HttpResponseMessage GetPO([FromBody]MobilePO MobilePO)
        {
            Logger.Debug("Inside JM Controller- GetPO");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(MobilePO));
            MobilePO lResponse = new MobilePO();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        lResponse = _IJMServer.GetPOs(MobilePO);
                        if (lResponse != null)
                        {
                            if (lResponse.POGUID != Guid.Empty)
                            {
                                Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                                response.Add("PO", lResponse);
                                return Request.CreateResponse(HttpStatusCode.OK, response);
                            }
                            else
                            {
                                Logger.Debug("Response: No PO Found");
                                error.ErrorCode = HttpStatusCode.BadRequest;
                                error.ErrorMessage = "No PO Found";
                                response.Add("ErrorResponse", error);
                                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                            }
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.BadRequest;
                            error.ErrorMessage = "Unable to get PO Detail";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region GetPOList
        [HttpPost]
        [ActionName("GetPOList")]
        public HttpResponseMessage GetPOList()
        {
            Logger.Debug("Inside JM Controller- GetPOList");
            Logger.Debug("No Request Structure");
            MobilePoList lResponse = new MobilePoList();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        lResponse.POList = _IJMServer.GetPOList(SessionID);
                        if (lResponse != null)
                        {
                            // response.Add("PO", lResponse);
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.BadRequest;
                            error.ErrorMessage = "Unable to get PO List";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to get SessionID in Header";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region InsertPO
        [HttpPost]
        [ActionName("InsertPO")]
        public HttpResponseMessage InsertPO([FromBody]MobilePO MobilePO)
        {
            Logger.Debug("Inside JM Controller- InsertPO");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(MobilePO));
            MobilePO lResponse = new MobilePO();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        int result = _IJMServer.InsertPO(MobilePO);
                        if (result > 0)
                        {
                            Logger.Debug("Response: No response In Body,Process is Success");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.BadRequest;
                            error.ErrorMessage = "Failure";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region CreateJobForCustomerStop
        [HttpPost]
        [ActionName("CreateJobForCustomerStop")]
        public HttpResponseMessage CreateJobForCustomerStop([FromBody]CreateJobForCustomerStopRequest jobRequest)
        {
            //StringBuilder lretString = new StringBuilder();
            Logger.Debug("Inside JM Controller- CreateJobForCustomerStop");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(jobRequest));
            MobileJob lResponse = new MobileJob();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            int errorCode = 0;
            try
            {
                string SessionID = Guid.Empty.ToString();
                // lretString.Append("[1: Sesseion Initialized]");
                if (Request.Headers != null)
                {

                    // lretString.Append(" [2: Header is not null: Actual Header is :]" + Request.Headers.ToString() + "");
                    //SessionID = Request.Headers.LastOrDefault().Value.First();
                    SessionID = jobRequest.SessionGUID;

                    //lretString.Append(" [3: Sesseion ID" + SessionID + "]");
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {

                        //lretString.Append(" [4: Valid Sesseion ID" + SessionID + "]");
                        Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        //lretString.Append(" [4.1: Valid User ID" + UserGUID + "]");
                        lResponse = _IJMServer.CreateJobForCustomerStop(jobRequest, UserGUID, ref errorCode);
                        //lretString.Append(" [4.2: Error Code" + errorCode + "]");
                        if (lResponse != null)
                        {
                            //error.ErrorMessage = "Success. retStr : " + lretString.ToString();
                            // response.Add("ErrorResponse", error);

                            response.Add("Jobs", lResponse);
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.BadRequest;
                            if (errorCode == 1)
                                error.ErrorMessage = "Failure: Managers doesn't belong to this store";
                            else
                                error.ErrorMessage = "Failure: Invalid Store ID";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {

                        //lretString.Append(" [5: Invalid Sesseion ID" + SessionID + "]");
                        //error.ErrorMessage = "Session has expired, please login again. retStr : " + lretString.ToString();
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {

                    // lretString.Append(" [6: Header is  null]");
                    //error.ErrorMessage = "Failure " + lretString.ToString();
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                // lretString.Append(" [7: Error " + ex.Message + "]");
                //error.ErrorMessage = "Failure " + lretString.ToString();
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        #endregion

        #region CreateJobForPO
        [HttpPost]
        [ActionName("CreateJobForPO")]
        public HttpResponseMessage CreateJobForPO([FromBody]CreateJobForPORequest jobRequest)
        {
            //StringBuilder lretString = new StringBuilder();
            Logger.Debug("Inside JM Controller- CreateJobForPO");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(jobRequest));
            MobileJob lResponse = new MobileJob();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    //SessionID = Request.Headers.LastOrDefault().Value.First();
                    SessionID = jobRequest.SessionGUID;
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        // lretString.Append("1.Inside SessioniD : " + SessionID + "");
                        Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        int errorCode = 0;
                        lResponse = _IJMServer.CreateJobForPO(jobRequest, UserGUID, ref errorCode);
                        Logger.Debug("ErrorCode: " + errorCode);
                        if (lResponse != null && errorCode == 0)
                        {
                            //lretString.Append("2.Success : " + new JavaScriptSerializer().Serialize(lResponse) + "");
                            //response.Add("Er", lretString.ToString());

                            response.Add("Jobs", lResponse);
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        if (lResponse != null && errorCode == 1)
                        {
                            //lretString.Append("3.Error : " + new JavaScriptSerializer().Serialize(lResponse) + "");
                            //response.Add("Er", lretString.ToString());

                            response.Add("Jobs", lResponse);
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else if (errorCode == -1)
                        {
                            //lretString.Append("4.Error : " + new JavaScriptSerializer().Serialize(lResponse) + "");
                            //response.Add("Er", lretString.ToString());

                            error.ErrorCode = HttpStatusCode.MultipleChoices;
                            error.ErrorMessage = "Unable to create job, Store information is not defined";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                        else if (errorCode == -2)
                        {
                            //lretString.Append("5.Error : " + new JavaScriptSerializer().Serialize(lResponse) + "");
                            //response.Add("Er", lretString.ToString());

                            error.ErrorCode = HttpStatusCode.MultipleChoices;
                            error.ErrorMessage = "Unable to create job, Customer information is not defined";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                        else
                        {
                            //lretString.Append("6.Error : " + new JavaScriptSerializer().Serialize(lResponse) + "");
                            //response.Add("Er", lretString.ToString());

                            error.ErrorCode = HttpStatusCode.MultipleChoices;
                            // error.ErrorMessage = "Unable to create job, as a job already in progress for the PO";
                            error.ErrorMessage = "Unable to create job, not able to get PO details";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {
                        //lretString.Append("7.Error : " + new JavaScriptSerializer().Serialize(lResponse) + "");
                        //response.Add("Er", lretString.ToString());

                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    //lretString.Append("8.Error : " + new JavaScriptSerializer().Serialize(lResponse) + "");
                    //response.Add("Er", lretString.ToString());

                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                //lretString.Append("9.Error : " + new JavaScriptSerializer().Serialize(lResponse) + "");
                //response.Add("Er", lretString.ToString());

                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure: " + ex.Message.ToString();
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region CreateVisitForPO
        [HttpPost]
        [ActionName("CreateVisitForPO")]
        public HttpResponseMessage CreateVisitForPO([FromBody]CreateVisitForPORequest jobRequest)
        {
            Logger.Debug("Inside JM Controller- CreateVisitForPO");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(jobRequest));
            MobileJob lResponse = new MobileJob();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        int errorCode = 0;
                        lResponse = _IJMServer.CreateVisitForPO(jobRequest, UserGUID, ref errorCode);
                        if (lResponse != null && errorCode == 0)
                        {
                            response.Add("Jobs", lResponse);
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else if (errorCode == -1)
                        {
                            error.ErrorCode = HttpStatusCode.MultipleChoices;
                            error.ErrorMessage = "Unable to create job, Store information is not defined";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                        else if (errorCode == -2)
                        {
                            error.ErrorCode = HttpStatusCode.MultipleChoices;
                            error.ErrorMessage = "Unable to create job, Customer information is not defined";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.MultipleChoices;
                            error.ErrorMessage = "Unable to create job, not able to get PO details";
                            // error.ErrorMessage = "Unable to create job, as a job already in progress for the PO";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure: " + ex.Message.ToString();
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion

        #region UploadJobAttachment
        [HttpPost]
        [ActionName("UploadJobAttachment")]
        public HttpResponseMessage UploadJobAttachment([FromBody]UploadJobAttachmentRequest jobAttachmentRequest)
        {
            Logger.Debug("Inside JM Controller- UploadJobAttachment");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(jobAttachmentRequest));
            UploadJobAttachmentResponse lResponse = new UploadJobAttachmentResponse();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {
                        Guid UserGUID = _IJMServer.GetUserGUID(SessionID);
                        lResponse = _IJMServer.UploadJobAttachment(jobAttachmentRequest, UserGUID);
                        if (lResponse != null)
                        {
                            response.Add("JobAttachmentUpload", lResponse);
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.BadRequest;
                            error.ErrorMessage = "Failure";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }
        #endregion

        #region UploadFileMultipart
        [HttpPost]
        [ActionName("UploadJobFile")]
        public async Task<HttpResponseMessage> UploadFileMultipart()
        {
            //StringBuilder lretstring = new StringBuilder();
            var response = new Dictionary<string, object>();
            UploadJobAttachmentResponse lResponse = new UploadJobAttachmentResponse();
            ErrorResponse error = new ErrorResponse();
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                //lretstring.Append("Not MultipartContent");
                error.ErrorCode = HttpStatusCode.UnsupportedMediaType;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                // response.Add("ResultResponse", lretstring.ToString());
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
            else if (Request.Headers == null)
            {
                // lretstring.Append("Header is null");
                error.ErrorCode = HttpStatusCode.BadRequest;
                error.ErrorMessage = "Failure";
                response.Add("ErrorResponse", error);
                //  response.Add("ResultResponse", lretstring.ToString());
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
            string SessionID = string.Empty;
            string JobGUID = string.Empty;
            if (HttpContext.Current.Request.Form != null && HttpContext.Current.Request.Form.Count > 0)
            {
                // lretstring.Append("Inside Request Form");
                SessionID = HttpContext.Current.Request.Form["SessionGUID"];
                JobGUID = HttpContext.Current.Request.Form["JobGUID"];
            }
            if (_IJMServer.ValidateUser(SessionID))
            {
                // lretstring.Append("Valid User");
                string lFilesRoot = _IJMServer.GetUploadFileAttachmetsPath(JobGUID, SessionID);
                if (null == lFilesRoot)
                {
                    // lretstring.Append("Fileroot is null");
                    error.ErrorCode = HttpStatusCode.InternalServerError;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    // response.Add("ResultResponse", lretstring.ToString());
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                }
                else
                {
                    // lretstring.Append("Inside else");
                    var lProvider = new MultipartFormDataStreamProvider(lFilesRoot);
                    try
                    {
                        // Read the form data.
                        await Request.Content.ReadAsMultipartAsync(lProvider);

                        foreach (MultipartFileData file in lProvider.FileData)
                        {
                            string lDestFilePath = lFilesRoot + "/" + file.Headers.ContentDisposition.FileName;
                            if (File.Exists(lDestFilePath))
                            {
                                File.Delete(lDestFilePath);
                            }
                            File.Move(file.LocalFileName, lDestFilePath);
                            //This is all files
                            Logger.Debug("Request File Name: " + file.Headers.ContentDisposition.FileName);
                            Logger.Debug("Server file path: " + file.LocalFileName);
                        }
                        response.Add("JobAttachmentUpload", lResponse);
                        // response.Add("ResultResponse", lretstring.ToString());
                        Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                        return Request.CreateResponse(HttpStatusCode.OK, response);
                    }
                    catch (System.Exception ex)
                    {
                        //lretstring.Append("Catch");
                        Logger.Error(ex.Message);
                        error.ErrorCode = HttpStatusCode.InternalServerError;
                        error.ErrorMessage = "Failure";
                        response.Add("ErrorResponse", error);
                        // response.Add("ResultResponse", lretstring.ToString());
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                    }
                }
            }
            else
            {
                error.ErrorCode = HttpStatusCode.Forbidden;
                error.ErrorMessage = "Session has expired, please login again";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.Forbidden, response);
            }
            //string root = AppDomain.CurrentDomain.BaseDirectory + "formdata";
            //var provider = new MultipartFormDataStreamProvider(root);

            //try
            //{
            //    // Read the form data.
            //    await Request.Content.ReadAsMultipartAsync(provider);

            //    foreach (var key in provider.FormData.AllKeys)
            //    {
            //        //This should be the JSON that contains the details about the file(s) uploaded
            //        var val = provider.FormData.GetValues(key);
            //        Trace.WriteLine(string.Format("{0}: {1}", key, val));
            //    }
            //    foreach (MultipartFileData file in provider.FileData)
            //    {
            //        //This is all files
            //        Trace.WriteLine(file.Headers.ContentDisposition.FileName);
            //        Trace.WriteLine("Server file path: " + file.LocalFileName);    
            //    }
            //    return Request.CreateResponse(HttpStatusCode.OK);
            //}
            //catch (System.Exception e)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            //}
        }
        #endregion

        #region DeleteJob
        [HttpPost]
        [ActionName("DeleteJob")]
        public HttpResponseMessage DeleteJob([FromBody]DeleteJobRequest pDeleteJobRequest)
        {
            Logger.Debug("Inside JM Controller- DeleteJob");
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    //SessionID = Request.Headers.LastOrDefault().Value.First();
                    SessionID = pDeleteJobRequest.SessionGUID;
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IJMServer.ValidateUser(SessionID))
                    {

                        int lRerVal = _IJMServer.DeleteJobs(pDeleteJobRequest, SessionID);
                        if (lRerVal >= 0)
                        {
                            Logger.Debug("Delete Job Response: " + lRerVal.ToString());
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            switch (lRerVal)
                            {
                                case -1:
                                    error.ErrorMessage = "Upable to process the delete request";
                                    break;
                                case -2:
                                    error.ErrorMessage = "Job not found in the database";
                                    break;
                                case -3:
                                    error.ErrorMessage = "Delete error";
                                    break;
                                default:
                                    error.ErrorMessage = "Error while processing the delete";
                                    break;
                            }
                            error.ErrorCode = HttpStatusCode.BadRequest;

                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
                        }
                    }
                    else
                    {
                        error.ErrorCode = HttpStatusCode.Forbidden;
                        error.ErrorMessage = "Session has expired, please login again";
                        response.Add("ErrorResponse", error);
                        return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                    }
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Failure";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Unable to get SessionID in Header";
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        #endregion


        #region TestPDF
        [HttpPost]
        [ActionName("TestPDF")]
        public HttpResponseMessage TestPDF()
        {
            byte[] s = Request.Content.ReadAsByteArrayAsync().Result;
            System.IO.File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "test.pdf", s);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion

    }
}
