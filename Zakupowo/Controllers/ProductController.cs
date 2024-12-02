using System;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class ProductController : Controller
{
    private ZakupowoDbContext db = new ZakupowoDbContext();

    [HttpGet]
    public ActionResult AddProduct()
    {
        ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryId", "Name");

        ViewBag.VatRates = new SelectList(db.VatRates.ToList(), "VatRateId", "Rate");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddProduct(Product model)
    {
        Console.WriteLine(ModelState.IsValid);
        if (ModelState.IsValid)
        {
            db.Products.Add(model);
            db.SaveChanges();

            TempData["ProductAdded"] = "Produkt został pomyślnie dodany!";

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryId", "Name");
        ViewBag.VatRates = new SelectList(db.VatRates.ToList(), "VatRateId", "Rate");

        return View(model);
    }
    public ActionResult ProductList()
    {
        var products = db.Products.Where(p => !p.IsDeleted).ToList();

        return View(products);
    }
    public ActionResult AdminProductList()
    {
        var products = db.Products.Where(p => !p.IsDeleted).ToList();

        return View(products);
    }
        public ActionResult ProductDetails(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult ProductEdit(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryId", "Name", product.CategoryId);
            ViewBag.VatRates = new SelectList(db.VatRates.ToList(), "VatRateId", "Rate", product.VatRateId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductEdit(Product model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["ProductUpdated"] = "Produkt został pomyślnie zaktualizowany!";
                return RedirectToAction("AdminProductList");
            }

            ViewBag.Categories = new SelectList(db.Categories.ToList(), "CategoryId", "Name", model.CategoryId);
            ViewBag.VatRates = new SelectList(db.VatRates.ToList(), "VatRateId", "Rate", model.VatRateId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductDelete(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                product.IsDeleted = true;
                db.SaveChanges();
                TempData["Message"] = "Produkt został przeniesiony do kosza.";
            }
            return RedirectToAction("AdminProductList");
            /*var product = db.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            TempData["ProductDeleted"] = "Produkt został pomyślnie usunięty!";
            return RedirectToAction("AdminProductList");*/
        }
        public ActionResult Trash()
        {
            var deletedProducts = db.Products.Where(p => p.IsDeleted).ToList();
            return View(deletedProducts);
        }
        public ActionResult RestoreProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product != null && product.IsDeleted)
            {
                product.IsDeleted = false;
                db.SaveChanges();
                TempData["Message"] = "Produkt został przywrócony.";
            }
            return RedirectToAction("Trash");
        }
        public ActionResult DeletePermanently(int id)
        {
            var product = db.Products.Find(id);
            if (product != null && product.IsDeleted)
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["ProductDeleted"] = "Produkt został usunięty na stałe.";
            }
            return RedirectToAction("Trash");
        }
    }

