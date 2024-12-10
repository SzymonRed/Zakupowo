using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class BaseController : Controller
{
    
    private ZakupowoDbContext db = new ZakupowoDbContext();
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        
        if (Session["SelectedCurrencyId"] == null)
        {
            Session["SelectedCurrencyId"] = 1;
            Session["SelectedExchangeRate"] = 1.0m; 
            Session["SelectedCurrencyCode"] = "PLN";
        }

        ViewBag.Currencies = db.Currencies.ToList();
        
        base.OnActionExecuting(filterContext);
    }
}