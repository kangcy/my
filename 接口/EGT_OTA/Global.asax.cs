using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EGT_OTA.Models;

namespace EGT_OTA
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //短链路由
            routes.MapRoute(
                name: "short",
                url: "{number}",
                defaults: new { controller = "Home", action = "Short" }
            );

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            new WebApplication().Start(System.Web.HttpContext.Current);
            EGT_OTA.Models.Repository.UpdateDB();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //在应用程序关闭时运行的代码
            new EGT_OTA.Models.WebApplication().End(System.Web.HttpContext.Current);
        }

        void Application_Error(object sender, EventArgs e)
        {
            //在出现未处理的错误时运行的代码
            new EGT_OTA.Models.WebApplication().Error(System.Web.HttpContext.Current);
        }

        protected void Application_BeginRequest()
        {
            HttpContext.Current.Items["DefaultConnection"] = EGT_OTA.Models.Repository.GetRepo("DefaultConnection");
        }

        protected void Application_EndRequest()
        {
            HttpContext.Current.Items.Remove("DefaultConnection");
        }
    }
}