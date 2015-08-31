using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WorkersInMotion.Log;

namespace WorkersInMotion.WebAPI.Controllers
{
    public class BaseAPIController : ApiController
    {
        #region Variables Declaration
        readonly ILogService _ILogservice;
        protected ILogService Logger
        {
            get
            {
                return _ILogservice;
            }
        }
        #endregion

        #region Constructor
        public BaseAPIController()
        {
            _ILogservice = new Log4NetService(GetType());
        }
        #endregion

       
    }
}
