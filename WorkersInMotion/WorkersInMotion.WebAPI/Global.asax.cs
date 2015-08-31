using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WorkersInMotion.WebAPI.Controllers;
using WorkersInMotion.WebAPI.Models.MobileModel.Interface;
using WorkersInMotion.WebAPI.Models.MobileModel.Service;

namespace WorkersInMotion.WebAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        void ConfigureApi(HttpConfiguration config)
        {
            var unity = new UnityContainer();

            unity.RegisterType<LoginController>();
            unity.RegisterType<UMController>();
            unity.RegisterType<EMController>();
            unity.RegisterType<JMController>();

            unity.RegisterType<ILoginServer, LoginServer>();
            unity.RegisterType<IUMServer, UMServer>();
            unity.RegisterType<IEMServer, EMServer>();
            unity.RegisterType<IJMServer, JMServer>();
            config.DependencyResolver = new IoCContainer(unity);
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 302)
            {
                Context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }
    }
}