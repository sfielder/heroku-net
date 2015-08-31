using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkersInMotion.DataAccess.Model;
using WorkersInMotion.DataAccess.Repository;

namespace WorkersInMotion.Model.Filter
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        private readonly IUserRepository _IUserRepository;

        public SessionExpireFilterAttribute()
        {
            this._IUserRepository = new UserRepository(new WorkersInMotionDB());
        }
        public SessionExpireFilterAttribute(WorkersInMotionDB context)
        {
            this._IUserRepository = new UserRepository(context);
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            // check if session is supported
            if (ctx != null && ctx.Session["OrganizationGUID"] == null)
            {
                // check if a new session id was generated
             
                filterContext.Result = new RedirectResult("~/User/Login?redirectUrl=" + _IUserRepository.EncodeTo64(ctx.Request.Url.AbsoluteUri) + "&SessionExpire=True");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}