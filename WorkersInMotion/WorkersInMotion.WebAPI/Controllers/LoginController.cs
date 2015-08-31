using AttributeRouting;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.WebAPI.Models.MobileModel;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;
using WorkersInMotion.WebAPI.Models.MobileModel.Service;


namespace WorkersInMotion.WebAPI.Controllers
{
    [RoutePrefix("Login")]
    public class LoginController : ApiController
    {
        private ILoginServer _ILoginServer;
        public LoginController()
        {
            _ILoginServer = new LoginServer();
        }
        public LoginController(ILoginServer LoginServer)
        {
            _ILoginServer = LoginServer;
        }
        //LoginServer lloginServer = new LoginServer();
        #region Web-API Methods
        [HttpGet]
        public HttpResponseMessage Login([FromBody]LoginRequest pLoginRequest)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Inside Login Controller- Login");
            LoginResponse loginResponse = new LoginResponse();
            try
            {
                loginResponse = _ILoginServer.Login(pLoginRequest);
                return Request.CreateResponse(HttpStatusCode.OK, loginResponse);
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
    }
}
