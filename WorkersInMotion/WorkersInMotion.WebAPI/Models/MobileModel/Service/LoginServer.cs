using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkersInMotion.WebAPI.Models.MobileModel;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;
using WorkersInMotion.DataAccess.Repository;
using WorkersInMotion.DataAccess.Model;


namespace WorkersInMotion.WebAPI.Models.MobileModel.Service
{
    public class LoginServer : ILoginServer
    {
        public LoginResponse Login(LoginRequest pLoginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();
            IUserRepository _IUserRepository;
            _IUserRepository = new UserRepository(new WorkersInMotionDB());

            //UserRepository lUserRepository = new UserRepository();


            // MasterLogin lMasterLogin = _IUserRepository.UserLogin(pLoginRequest.UserName, pLoginRequest.Password, pLoginRequest.IpAddress, pLoginRequest.Phone, pLoginRequest.DeviceID);
            //if (lMasterLogin != null && lMasterLogin.UserGUID != null && lMasterLogin.UserGUID != Guid.Empty)
            //{
            //    loginResponse.MasterLogin = lMasterLogin;
            //}
            return loginResponse;
        }
    }
}
