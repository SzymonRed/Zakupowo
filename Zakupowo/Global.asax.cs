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

            // Wyłącz inicjalizator EF
            Database.SetInitializer<ZakupowoDbContext>(null);

            // Inicjalizacja zadania w tle
            var dbContext = new ZakupowoDbContext();
            var exchangeRateService = new ExchangeRateService(dbContext);
            _backgroundTask = new ExchangeRateBackgroundTask(exchangeRateService);
        }

        protected void Application_End()
        {
            // Zatrzymaj zadanie w tle, gdy aplikacja kończy działanie
            _backgroundTask?.Stop();
        }
    }
}