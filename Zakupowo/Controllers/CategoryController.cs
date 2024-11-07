using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

public class CategoryController : Controller
{
    private ZakupowoDbContext db = new ZakupowoDbContext();

    // Widok formularza dodawania kategorii
    public ActionResult AddCategory()
    {
        // Pobieramy listę kategorii (możemy wykluczyć kategorie, które nie mogą być rodzicem, np. kategorie, które już są nadrzędne dla innych)
        var categories = db.Categories.ToList();

        // Przekazujemy listę kategorii do widoku
        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");

        return View();
    }

    // Obsługa formularza dodawania kategorii
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddCategory(Category model)
    {
        if (ModelState.IsValid)
        {
            // Dodanie nowej kategorii do bazy
            db.Categories.Add(model);
            db.SaveChanges();

            TempData["CategorySuccess"] = "Kategoria została dodana!";
            return RedirectToAction("AddCategory");
        }

        // W przypadku błędów walidacji, ponownie ładujemy listę kategorii i przekazujemy do widoku
        ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
        return View(model);
    }
}