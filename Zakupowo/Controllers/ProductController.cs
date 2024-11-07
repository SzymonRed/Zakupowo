using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers
{
    public class ProductController : Controller
    {
        private ZakupowoDbContext db = new ZakupowoDbContext();

        // GET: Product/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Załaduj kategorie do widoku
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryId", "Name");

            // Załaduj dostępne stawki VAT
            ViewBag.VatRates = new SelectList(db.VatRates.ToList(), "VatRateId", "Rate");

            return View();
        }

        // POST: Product/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(Product model)
        {
            // Sprawdzenie, czy formularz jest poprawny
            if (ModelState.IsValid)
            {
                // Dodaj produkt do bazy danych
                db.Products.Add(model);
                db.SaveChanges();

                // Ustawienie komunikatu o sukcesie
                TempData["ProductAdded"] = "Produkt został pomyślnie dodany!";

                // Przekierowanie do strony głównej lub innej strony
                return RedirectToAction("Index", "Home");
            }

            // W przypadku błędów, ponownie przekaż kategorie oraz stawki VAT do widoku
            ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
            ViewBag.VatRates = new SelectList(db.VatRates.ToList(), "VatRateId", "Rate");

            return View(model);
        }
    }
}