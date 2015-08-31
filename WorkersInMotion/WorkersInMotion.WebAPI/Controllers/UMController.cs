using AttributeRouting;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using WorkersInMotion.WebAPI.Models.MobileModel;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;
using WorkersInMotion.WebAPI.Models.MobileModel.Service;


namespace WorkersInMotion.WebAPI.Controllers
{
    [RoutePrefix("UM")]
    public class UMController : BaseAPIController
    {
        private IUMServer _IUMServer;
        public UMController()
        {
            _IUMServer = new UMServer();
        }
        public UMController(IUMServer UMServer)
        {
            _IUMServer = UMServer;
        }

        #region Login
        [HttpPost]
        [ActionName("Login")]
        public HttpResponseMessage Login([FromBody]LoginRequest pLoginRequest)
        {

            Logger.Debug("Inside UMController- Login");
            //Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(pLoginRequest));
            LoginResponse LoginResponse = new LoginResponse();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();

            //error.ErrorCode = HttpStatusCode.OK;
            //// error.ErrorMessage = "Unable to Get User Details";
            //error.ErrorMessage = "Checking Prabhu.";
            //response.Add("ErrorResponse", error);
            //return Request.CreateResponse(HttpStatusCode.OK, response);
            try
            {
                //int b = 0;
                //int a = 1 / b;
                //pLoginRequest.Cred.UserName = Encoding.UTF8.GetString(Convert.FromBase64String(pLoginRequest.Cred.UserName));
                //pLoginRequest.Cred.Password = Encoding.UTF8.GetString(Convert.FromBase64String(pLoginRequest.Cred.Password));
                string[] lCred = Encoding.UTF8.GetString(Convert.FromBase64String(pLoginRequest.Cred)).Split(':');
                if (lCred.Count() > 1)
                {
                    pLoginRequest.UserName = lCred[0];
                    pLoginRequest.Password = lCred[1];
                }
                pLoginRequest.LoginType = eLoginType.DeviceLogin;
                LoginResponse = _IUMServer.Login(pLoginRequest);
                if (LoginResponse != null)
                {
                    Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(LoginResponse));
                    response.Add("LoginResponse", LoginResponse);
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.PreconditionFailed;
                    // error.ErrorMessage = "Unable to Get User Details";
                    error.ErrorMessage = "Invalid Username and Password." + pLoginRequest.UserName + ":" + pLoginRequest.Password;
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.PreconditionFailed, response);
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

        #region Web-API CreateAccount
        [HttpPost]
        [ActionName("CreateAccount")]
        public HttpResponseMessage CreateAccount([FromBody]LoginRequest pLoginRequest)
        {

            Logger.Debug("Inside UMController- Create Account");
            //Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(pLoginRequest));
            CreateAccountResponse CreateAccountResponse = new CreateAccountResponse();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string[] lCred = Encoding.UTF8.GetString(Convert.FromBase64String(pLoginRequest.Cred)).Split(':');
                if (lCred.Count() > 1)
                {
                    pLoginRequest.UserName = lCred[0];
                    pLoginRequest.Password = lCred[1];
                }
                CreateAccountResponse = _IUMServer.CreateAccount(pLoginRequest);
                if (CreateAccountResponse != null)
                {
                    //Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(CreateAccountResponse));
                    response.Add("CreateAccountResponse", CreateAccountResponse);
                    return Request.CreateResponse(HttpStatusCode.Created, response);
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.Conflict;
                    error.ErrorMessage = "Unable to create account or account already exists";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.Conflict, response);
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

        #region ForgotPWD
        [HttpPost]
        [ActionName("ForgotPWD")]
        public HttpResponseMessage ForgotPWD([FromBody]ForgotPasswordRequest pForgotPassword)
        {
            Logger.Debug("Inside UMController- Forgot Password");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(pForgotPassword));
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                int result = _IUMServer.ForgotPassword(pForgotPassword);
                if (result == 1)
                {
                    Logger.Debug("Response: No response In Body,Process is Success");
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else if (result == 401)
                {
                    error.ErrorCode = HttpStatusCode.Forbidden;
                    error.ErrorMessage = "Provided Email ID is not match with any User";
                    response.Add("ErrorResponse", error);
                    return Request.CreateResponse(HttpStatusCode.Forbidden, response);
                }
                else
                {
                    error.ErrorCode = HttpStatusCode.BadRequest;
                    error.ErrorMessage = "Unable to send mail";
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

        #region GetRouteUsers
        [HttpPost]
        [ActionName("GetRouteUsers")]
        public HttpResponseMessage GetRouteUsers([FromBody]GetRouteUserRequest GetRouteUserRequest)
        {
            Logger.Debug("Inside UM Controller- GetRouteUsers ");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(GetRouteUserRequest));
            GetRouteUserResponse lResponse = new GetRouteUserResponse();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IUMServer.ValidateUser(SessionID))
                    {
                        lResponse = _IUMServer.GetRouteUsers(GetRouteUserRequest);
                        if (lResponse != null)
                        {
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Failure";
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

        #region PostHeartBeat
        [HttpPost]
        [ActionName("PostHeartBeat")]
        public HttpResponseMessage PostHeartBeat([FromBody]HeartBeatRequest HeartBeatRequest)
        {
            Logger.Debug("Inside UM Controller- PostHeartBeat");
            Logger.Debug("Request: " + new JavaScriptSerializer().Serialize(HeartBeatRequest));
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IUMServer.ValidateUser(SessionID))
                    {
                        System.Guid UserGUID = _IUMServer.GetUserGUID(SessionID);
                        if (_IUMServer.PostHeartBeat(HeartBeatRequest, UserGUID) > 0)
                        {
                            Logger.Debug("Response: No response In Body,Process is Success");
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to insert PostHeartBeat";
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

        #region GetUserLocation
        [HttpPost]
        [ActionName("GetUserLocation")]
        public HttpResponseMessage GetUserLocation([FromBody]Guid WorkerID)
        {
            Logger.Debug("Inside UM Controller- GetUserLocation");
            if (WorkerID != null)
                Logger.Debug("WorkerID: " + WorkerID.ToString());
            LocationResponse lResponse = new LocationResponse();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IUMServer.ValidateUser(SessionID))
                    {
                        if (WorkerID == null || WorkerID == Guid.Empty)
                        {
                            WorkerID = _IUMServer.GetUserGUID(SessionID);
                        }
                        lResponse = _IUMServer.GetUserLocation(WorkerID);
                        if (lResponse != null)
                        {
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            response.Add("Worker", lResponse);
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to get User Location";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, lResponse);
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

        #region DownloadUsers
        [HttpPost]
        [ActionName("DownloadUsers")]
        public HttpResponseMessage DownloadUsers()
        {
            Logger.Debug("Inside UM Controller- GetUserLocation");
            DownloadUsers lResponse = new DownloadUsers();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IUMServer.ValidateUser(SessionID))
                    {
                        lResponse = _IUMServer.DownloadUsers(SessionID);
                        if (lResponse != null)
                        {
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to get User Details";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, lResponse);
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

        #region GetTerritoryRegion
        [HttpPost]
        [ActionName("GetTerritoryRegion")]
        public HttpResponseMessage GetTerritoryRegion()
        {
            Logger.Debug("Inside UM Controller- GetTerritoryRegion");
            TerritoryRegion lResponse = new TerritoryRegion();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IUMServer.ValidateUser(SessionID))
                    {
                        Guid UserGUID = _IUMServer.GetUserGUID(SessionID);
                        lResponse = _IUMServer.GetTerritoryRegion(UserGUID);
                        if (lResponse != null)
                        {
                            Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                            response.Add("TerritoryRegion", lResponse);
                            return Request.CreateResponse(HttpStatusCode.OK, response);
                        }
                        else
                        {
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Unable to get Region and Territory Details";
                            response.Add("ErrorResponse", error);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, lResponse);
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

        #region GetOrganization
        [HttpGet]
        [ActionName("GetOrganization")]
        public HttpResponseMessage GetOrganization()
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside UM Controller- GetOrganization");
            UMResponseOrganization umResponse = new UMResponseOrganization();

            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {

                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    umResponse.Organization = _IUMServer.GetOrganization(SessionID).Organization;

                }

                return Request.CreateResponse(HttpStatusCode.OK, umResponse);
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                ErrorResponse error = new ErrorResponse();
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        #endregion

        #region GetCustomers
        [HttpPost]
        [ActionName("GetCustomers")]
        public HttpResponseMessage GetCustomers(Customer pcustomer)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside UM Controller- GetCustomers");
            UMResponseCustomer lResponse = new UMResponseCustomer();
            ErrorResponse error = new ErrorResponse();
            var response = new Dictionary<string, object>();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {

                    // SessionID = Request.Headers.LastOrDefault().Value.First();
                    SessionID = pcustomer.SessionGUID;
                    Guid lOrganizationGuid = _IUMServer.GetOrganizationGUIDBySessionID(SessionID);
                    if (lOrganizationGuid != null && lOrganizationGuid != Guid.Empty)
                    {
                        lResponse.Customers = _IUMServer.GetCustomers(lOrganizationGuid);
                        Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                        response.Add("CustomersResponse", lResponse);
                        return Request.CreateResponse(HttpStatusCode.OK, response);
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
                log.Debug(ex.Message);
                error = new ErrorResponse();
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        #endregion
    }
}
