using System;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Zakupowo.Models;
using Zakupowo.Services;

namespace Zakupowo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ExchangeRateBackgroundTask _backgroundTask;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<ZakupowoDbContext>(null);

            var dbContext = new ZakupowoDbContext();
            var exchangeRateService = new ExchangeRateService(dbContext);
            _backgroundTask = new ExchangeRateBackgroundTask(exchangeRateService);
        }

        protected void Application_End()
        {
            _backgroundTask?.Stop();
        }
    }
}