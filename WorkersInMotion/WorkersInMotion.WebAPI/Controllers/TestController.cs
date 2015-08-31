using AttributeRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WorkersInMotion.WebAPI.Controllers
{
    [RoutePrefix("Test")]
    public class TestController : ApiController
    {
        [HttpPost]
        [ActionName("Login")]
        public HttpResponseMessage Login()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Success");
        }
        [HttpPost]
        [ActionName("Logins")]
        public HttpResponseMessage Logins()
        {
            WorkersInMotion.WebAPI.Models.MobileModel.Interface.ILoginServer _ILoginServer = new WorkersInMotion.WebAPI.Models.MobileModel.Service.LoginServer();
            WorkersInMotion.WebAPI.Models.MobileModel.LoginResponse loginResponse = new WorkersInMotion.WebAPI.Models.MobileModel.LoginResponse();
            WorkersInMotion.WebAPI.Models.MobileModel.LoginRequest req = new Models.MobileModel.LoginRequest();
            req.UserName = "admin";
            req.Password = "a";
            //WorkersInMotion.WebAPI.Models.MobileModel.LoginRequest pLoginRequest = new Models.MobileModel.LoginRequest();
            WorkersInMotion.WebAPI.Models.MobileModel.LoginResponse LoginResponse = new WorkersInMotion.WebAPI.Models.MobileModel.LoginResponse();
            WorkersInMotion.WebAPI.Models.MobileModel.ErrorResponse error = new WorkersInMotion.WebAPI.Models.MobileModel.ErrorResponse();
            //pLoginRequest.LoginType = eLoginType.DeviceLogin;
            WorkersInMotion.WebAPI.Models.MobileModel.Interface.IUMServer _IUMServer = new WorkersInMotion.WebAPI.Models.MobileModel.Service.UMServer();
            LoginResponse = _IUMServer.Login(req);
            var response = new Dictionary<string, object>();
            if (LoginResponse != null)
            {
                response.Add("LoginResponse", LoginResponse);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                error.ErrorCode = HttpStatusCode.PreconditionFailed;
                // error.ErrorMessage = "Unable to Get User Details";
                error.ErrorMessage = "Invalid Username and Password." + req.UserName + ":" + req.Password;
                response.Add("ErrorResponse", error);
                return Request.CreateResponse(HttpStatusCode.PreconditionFailed, response);
            }
        }
    }
}
