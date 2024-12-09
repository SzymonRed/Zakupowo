using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class BaseController : Controller
{
    
    private ZakupowoDbContext db = new ZakupowoDbContext();
    // Inicjalizacja wspólnych danych dla wszystkich kontrolerów
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        
        // Sprawdzamy, czy w sesji są już ustawione wartości
        if (Session["SelectedCurrencyId"] == null)
        {
            // Ustawienie domyślnych wartości waluty
            Session["SelectedCurrencyId"] = 1; // PLN
            Session["SelectedExchangeRate"] = 1.0m; // Kurs wymiany (1 PLN)
            Session["SelectedCurrencyCode"] = "PLN"; // Kod waluty (PLN)
        }

        // Pobieramy waluty i ustawiamy w ViewBag
        ViewBag.Currencies = db.Currencies.ToList();
        
        base.OnActionExecuting(filterContext);
    }
}