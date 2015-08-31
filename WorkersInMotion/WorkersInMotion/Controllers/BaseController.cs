using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.Log;

namespace WorkersInMotion.Controllers
{
    public class BaseController : Controller
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
        public BaseController()
        {
            _ILogservice = new Log4NetService(GetType());
        }
        #endregion
	}
}