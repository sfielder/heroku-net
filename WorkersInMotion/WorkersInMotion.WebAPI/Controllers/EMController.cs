using AttributeRouting;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using WorkersInMotion.WebAPI.Models.MobileModel;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;
using WorkersInMotion.WebAPI.Models.MobileModel.Service;

namespace WorkersInMotion.WebAPI.Controllers
{
    [RoutePrefix("EM")]
    public class EMController : BaseAPIController
    {
        private IEMServer _IEMServer;
        public EMController()
        {
            _IEMServer = new EMServer();
        }
        public EMController(IEMServer EMServer)
        {
            _IEMServer = EMServer;
        }
        #region GetCustomers
        [HttpPost]
        [ActionName("GetCustomers")]
        public HttpResponseMessage GetCustomers()
        {
            Logger.Debug("Inside EM Controller- GetCustomers");
            Logger.Debug("No Request Body");
            EMCustomers lResponse = new EMCustomers();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IEMServer.ValidateUser(SessionID))
                    {
                        System.Guid OrganizationGUID = _IEMServer.GetOrganizationGUID(SessionID);
                        if (OrganizationGUID != Guid.Empty)
                        {
                            lResponse = _IEMServer.GetCustomers(new Guid(OrganizationGUID.ToString()));
                            if (lResponse != null)
                            {
                                Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                                return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                            }
                            else
                            {
                                ErrorResponse error = new ErrorResponse();
                                error.ErrorCode = HttpStatusCode.InternalServerError;
                                error.ErrorMessage = "Failure";
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
                            }
                        }
                        else
                        {
                            ErrorResponse error = new ErrorResponse();
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Failure";
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, lResponse);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                ErrorResponse error = new ErrorResponse();
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        #endregion

        #region GetContacts
        [HttpPost]
        [ActionName("GetContacts")]
        public HttpResponseMessage GetContacts()
        {
            Logger.Debug("Inside EM Controller- GetContacts");
            Logger.Debug("No Request Body");
            EMContacts lResponse = new EMContacts();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IEMServer.ValidateUser(SessionID))
                    {
                        System.Guid OrganizationGUID = _IEMServer.GetOrganizationGUID(SessionID);
                        if (OrganizationGUID != Guid.Empty)
                        {
                            lResponse = _IEMServer.GetContacts(new Guid(OrganizationGUID.ToString()));
                            if (lResponse != null)
                            {
                                Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                                return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                            }
                            else
                            {
                                ErrorResponse error = new ErrorResponse();
                                error.ErrorCode = HttpStatusCode.InternalServerError;
                                error.ErrorMessage = "Failure";
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
                            }
                        }
                        else
                        {
                            ErrorResponse error = new ErrorResponse();
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Failure";
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, lResponse);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                ErrorResponse error = new ErrorResponse();
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        #endregion

        #region GetCustomerStops
        [HttpPost]
        [ActionName("GetCustomerStops")]
        public HttpResponseMessage GetCustomerStops()
        {
            Logger.Debug("Inside EM Controller- GetCustomerStops");
            Logger.Debug("No Request Body");
            EMMarkets lResponse = new EMMarkets();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IEMServer.ValidateUser(SessionID))
                    {
                        System.Guid OrganizationGUID = _IEMServer.GetOrganizationGUID(SessionID);
                        if (OrganizationGUID != Guid.Empty)
                        {
                            lResponse = _IEMServer.GetCustomerStops(new Guid(OrganizationGUID.ToString()));
                            if (lResponse != null)
                            {
                                Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                                return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                            }
                            else
                            {
                                ErrorResponse error = new ErrorResponse();
                                error.ErrorCode = HttpStatusCode.InternalServerError;
                                error.ErrorMessage = "Failure";
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
                            }
                        }
                        else
                        {
                            ErrorResponse error = new ErrorResponse();
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Failure";
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, lResponse);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                ErrorResponse error = new ErrorResponse();
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        #endregion

        #region GetServicePoints
        [HttpPost]
        [ActionName("GetServicePoints")]
        public HttpResponseMessage GetServicePoints()
        {
            Logger.Debug("Inside EM Controller- GetServicePoints");
            Logger.Debug("No Request Body");
            EMMarkets lResponse = new EMMarkets();
            try
            {
                string SessionID = Guid.Empty.ToString();
                if (Request.Headers != null)
                {
                    SessionID = Request.Headers.LastOrDefault().Value.First();
                    Logger.Debug("SessionID: " + SessionID.ToString());
                    if (_IEMServer.ValidateUser(SessionID))
                    {
                        System.Guid OrganizationGUID = _IEMServer.GetOrganizationGUID(SessionID);
                        if (OrganizationGUID != Guid.Empty)
                        {
                            lResponse = _IEMServer.GetServicePoints(new Guid(OrganizationGUID.ToString()));
                            if (lResponse != null)
                            {
                                //Logger.Debug("Response: " + new JavaScriptSerializer().Serialize(lResponse));
                                return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                            }
                            else
                            {
                                ErrorResponse error = new ErrorResponse();
                                error.ErrorCode = HttpStatusCode.InternalServerError;
                                error.ErrorMessage = "Failure";
                                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
                            }
                        }
                        else
                        {
                            ErrorResponse error = new ErrorResponse();
                            error.ErrorCode = HttpStatusCode.InternalServerError;
                            error.ErrorMessage = "Failure";
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, lResponse);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                ErrorResponse error = new ErrorResponse();
                error.ErrorCode = HttpStatusCode.InternalServerError;
                error.ErrorMessage = "Failure";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, error);
            }
        }

        #endregion
    }
}
