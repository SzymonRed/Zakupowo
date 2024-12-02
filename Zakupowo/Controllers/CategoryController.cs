using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class CategoryController : Controller
{
    private ZakupowoDbContext db = new ZakupowoDbContext();

    // Widok formularza dodawania kategorii
    public ActionResult AddCategory()
    {
        var categories = db.Categories.ToList();

        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddCategory(Category model)
    {
        if (ModelState.IsValid)
        {
            db.Categories.Add(model);
            db.SaveChanges();

            TempData["CategorySuccess"] = "Kategoria została dodana!";
            return RedirectToAction("AddCategory");
        }

        ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
        return View(model);
    }
}