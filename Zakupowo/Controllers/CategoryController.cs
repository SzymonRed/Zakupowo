using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class CategoryController : Controller
{
    private ZakupowoDbContext db = new ZakupowoDbContext();
    
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
    public ActionResult CategoryList()
    {
        var categories = db.Categories.ToList();
        return View(categories);
    }
    public ActionResult EditCategory(int id)
    {
        var category = db.Categories.Find(id);
        if (category == null)
        {
            return HttpNotFound();
        }

        ViewBag.Categories = new SelectList(db.Categories.Where(c => c.CategoryId != id).ToList(), "CategoryId", "Name");
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditCategory(Category model)
    {
        if (ModelState.IsValid)
        {
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["CategorySuccess"] = "Kategoria została zaktualizowana!";
            return RedirectToAction("CategoryList");
        }

        ViewBag.Categories = new SelectList(db.Categories.Where(c => c.CategoryId != model.CategoryId).ToList(), "CategoryId", "Name");
        return View(model);
    }

    public ActionResult DeleteCategory(int id)
    {
        var category = db.Categories.Find(id);
        if (category == null)
        {
            return HttpNotFound();
        }

        return View(category);
    }
    
    [HttpPost, ActionName("DeleteCategory")]
    [ValidateAntiForgeryToken]
    public ActionResult ConfirmDeleteCategory(int id)
    {
        var category = db.Categories.Find(id);
        if (category != null)
        {
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["CategorySuccess"] = "Kategoria została usunięta!";
        }

        return RedirectToAction("CategoryList");
    }
}
