using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class ProductController : BaseController
{
    private ZakupowoDbContext db = new ZakupowoDbContext();

    [HttpGet]
    public ActionResult ProductList(int page = 1, int pageSize = 10)
    {
        ViewBag.Currencies = db.Currencies.ToList();
        // Pobierz domyślny przelicznik z sesji lub ustaw 1 (PLN)
        decimal exchangeRate = Session["SelectedExchangeRate"] != null ? (decimal)Session["SelectedExchangeRate"] : 1;

        var products = db.Products
            .Where(p => !p.IsHidden && !p.Category.IsHidden)
            .OrderBy(p => p.ProductId);

        int totalProducts = products.Count();
        var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Przelicz ceny produktów
        foreach (var product in pagedProducts)
        {
            product.PriceAfterConversion = product.Price * (1/exchangeRate); // Nowa właściwość w modelu View
        }

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
        ViewBag.PageSize = pageSize;
        ViewBag.CurrencyCode = Session["SelectedCurrencyCode"]?.ToString() ?? "PLN";

        return View(pagedProducts);
    }
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
    public ActionResult Search(string keyword, int page = 1, int pageSize = 10)
    {
    var productsQuery = db.Products
        .Where(p => !p.IsHidden && !p.Category.IsHidden)
        .AsQueryable();

    if (!string.IsNullOrEmpty(keyword))
    {
        var filters = ParseKeyword(keyword);

        foreach (var filter in filters)
        {
            if (filter.Operator == "AND")
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(filter.Term) || 
                    p.Description.Contains(filter.Term));
            }
            else if (filter.Operator == "OR")
            {
                productsQuery = productsQuery.Union(db.Products.Where(p =>
                    !p.IsHidden && !p.Category.IsHidden &&
                    (p.Name.Contains(filter.Term) || p.Description.Contains(filter.Term))));
            }
            else if (filter.Operator == "NOT")
            {
                productsQuery = productsQuery.Where(p =>
                    !p.Name.Contains(filter.Term) &&
                    !p.Description.Contains(filter.Term));
            }
        }
    }

    var products = productsQuery.OrderBy(p => p.ProductId);

    int totalProducts = products.Count();

    var pagedProducts = products
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
    ViewBag.PageSize = pageSize;
    ViewBag.Keyword = keyword;

    return View("ProductList", pagedProducts);
}

    private List<FilterTerm> ParseKeyword(string keyword)
    {
        var terms = new List<FilterTerm>();
        var tokens = keyword.Split(' ');
    
        string currentOperator = "AND";
        foreach (var token in tokens)
        {
            if (string.Equals(token, "AND", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(token, "OR", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(token, "NOT", StringComparison.OrdinalIgnoreCase))
            {
                currentOperator = token.ToUpper();
            }
            else
            {
                terms.Add(new FilterTerm
                {
                    Operator = currentOperator,
                    Term = token
                });
            }
        }
        return terms;
    }

    private class FilterTerm
    {
        public string Operator { get; set; } 
        public string Term { get; set; }
    }

}

