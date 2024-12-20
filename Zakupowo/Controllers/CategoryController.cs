using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
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
        // Pobieramy tylko kategorie nadrzędne i ich podkategorie
        var categories = db.Categories
            .Include(c => c.SubCategories) // Ładujemy podkategorie
            .Where(c => c.ParentCategoryId == null) // Tylko kategorie nadrzędne
            .ToList();

        return View(categories);
    }
    
    public ActionResult CategoryProducts(int id)
    {
        // Pobieramy produkty przypisane do danej kategorii
        var products = db.Products
            .Where(p => p.CategoryId == id && !p.IsDeleted && !p.IsHidden)
            .ToList();
    
        // Pobieramy szczegóły kategorii dla wyświetlenia w widoku
        var category = db.Categories.Find(id);
        if (category == null)
        {
            return HttpNotFound();
        }
    
        ViewBag.CategoryName = category.Name;
        ViewBag.CurrencyCode = Session["SelectedCurrencyCode"]?.ToString() ?? "PLN";
    
        return View(products);
    }
    

    public ActionResult AdminCategoryList()
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
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();

            TempData["CategorySuccess"] = "Kategoria została zaktualizowana!";
            return RedirectToAction("AdminCategoryList");
        }

        ViewBag.Categories = new SelectList(db.Categories.Where(c => c.CategoryId != model.CategoryId).ToList(), "CategoryId", "Name");
        return View(model);
    }

    public ActionResult DeleteCategory(int id)
    {
        var category = db.Categories.Include("SubCategories").FirstOrDefault(c => c.CategoryId == id);
        if (category == null)
        {
            return HttpNotFound();
        }

        if (category.SubCategories != null && category.SubCategories.Any())
        {
            TempData["HasSubCategories"] = true;
        }

        return View(category);
    }

    [HttpPost, ActionName("DeleteCategory")]
    [ValidateAntiForgeryToken]
    public ActionResult ConfirmDeleteCategory(int id)
    {
        var category = db.Categories.Include("SubCategories").FirstOrDefault(c => c.CategoryId == id);
        if (category == null)
        {
            return HttpNotFound();
        }

        if (category.SubCategories != null && category.SubCategories.Any())
        {
            TempData["DeleteError"] = "Nie można usunąć kategorii, która posiada podkategorie!";
            return RedirectToAction("DeleteCategory", new { id });
        }

        db.Categories.Remove(category);
        db.SaveChanges();
        TempData["CategorySuccess"] = "Kategoria została usunięta!";

        return RedirectToAction("AdminCategoryList");
    }
        public ActionResult GeneratePdf(int categoryId)
        {
            var category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return HttpNotFound("Nie znaleziono kategorii.");
            }

            var products = db.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Name).Include(product => product.VatRate)
                .ToList();

            var tempPath = Path.Combine(Path.GetTempPath(), $"Cennik_{category.Name}.pdf");
            string fontPath = HostingEnvironment.MapPath("~/Fonts/arial.ttf");

            using (var writer = new PdfWriter(tempPath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf);
                    PdfFont font = PdfFontFactory.CreateFont(fontPath);
                    
                    document.SetFont(font);

                    document.Add(new Paragraph($"Cennik dla kategorii: {category.Name}")
                        .SetFontSize(18)
                        .SimulateBold());
                    
                    var table = new Table(3); 
                    
                    table.AddHeaderCell("Nazwa produktu");
                    table.AddHeaderCell("Cena netto");
                    table.AddHeaderCell("Cena brutto");
                    var discount = Session["UserDiscount"] != null ? (decimal)Session["UserDiscount"] : 0;
                    ViewBag.CurrencyCode = Session["SelectedCurrencyCode"]?.ToString() ?? "PLN";

                    
                    foreach (var product in products)
                    {
                        table.AddCell(product.Name);
                        table.AddCell((product.Price * (1 / (decimal)Session["SelectedExchangeRate"]) / (discount + 1)).ToString("N2") + ViewBag.CurrencyCode); 
                        if (product.VatRate != null && product.VatRate.Rate.HasValue)
                        {
                            table.AddCell(((product.Price * (1 / (decimal)Session["SelectedExchangeRate"]) * (1 + product.VatRate.Rate.Value) / (discount + 1)).ToString("N2")) + ViewBag.CurrencyCode);
                            
                        }
                        else
                        {
                            table.AddCell("Zwolniony z VAT"); 
                        }
                    }

                    document.Add(table);

                    document.Add(new Paragraph("Wygenerowano automatycznie przez system Zakupowo.")
                        .SetFontSize(10)
                        .SimulateItalic());
                }
            }
            var fileBytes = System.IO.File.ReadAllBytes(tempPath);
            System.IO.File.Delete(tempPath); 
            return File(fileBytes, "application/pdf", $"Cennik_{category.Name}.pdf");
        }
    }

