using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class UserController : Controller
{
    private ZakupowoDbContext _context = new ZakupowoDbContext();
    
    public ActionResult UserList()
    {
        var users = _context.User.ToList();
        return View(users);
    }

    public ActionResult Details(int id)
    {
        var user = _context.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }
        return View(user);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(User model)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "Hasło jest wymagane.");
                return View(model);
            }
            _context.User.Add(model);
            _context.SaveChanges();
            TempData["success"] = $"User {model.Username} has been created!";
            return RedirectToAction("UserList");
        }
        return View(model);
    }

    public ActionResult Edit(int id)
    {
        var user = _context.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(User model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.User.Find(model.UserId);
            var oldPassword = user.Password;
            Console.WriteLine(string.IsNullOrEmpty(model.Password));
            if (user != null)
            {
                user.Username = model.Username;
                if (string.IsNullOrEmpty(model.Password))
                {
                    user.Password = oldPassword;
                }
                else
                    user.Password = model.Password;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.IsAdmin = model.IsAdmin;
                user.Newsletter = model.Newsletter;
                user.Discount = model.Discount;
                _context.SaveChanges();
                
                if (Session["UserId"] != null && (int)Session["UserId"] == user.UserId)
                {
                    Session["UserDiscount"] = user.Discount;
                }
                
                return RedirectToAction("UserList");
            }
        }
        return View(model);
    }

    public ActionResult Delete(int id)
    {
        var user = _context.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }
        
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(User model)
    {
        var user = _context.User.Find(model.UserId);
        if (user != null && !user.IsAdmin)
        {
            _context.User.Remove(user);
            _context.SaveChanges();
            TempData["mes"] = $"User {user.Username} has been deleted";
        }
        ViewBag.admin = "Nie można usunąć administratora";
        return RedirectToAction("UserList");
    }
    public ActionResult UpdateOrderStatus(int orderId, string status)
    {
        var order = _context.Orders.Include("OrderItems.Product").FirstOrDefault(o => o.OrderId == orderId);
        if (order == null)
        {
            return HttpNotFound("Nie znaleziono zamówienia.");
        }

        order.Status = status;
            
        if (status == "Completed")
        {
            foreach (var orderItem in order.OrderItems)
            {
                var product = orderItem.Product;
                if (product.Stock < orderItem.Quantity)
                {
                    return Content($"Produkt {product.Name} nie ma wystarczającego stanu magazynowego!");
                }
                product.Stock -= orderItem.Quantity;
            }
        }

        if (status == "Cancelled")
        {
            foreach (var orderItem in order.OrderItems)
            {
                var product = orderItem.Product;
                product.Stock += orderItem.Quantity;
            }
        }

        _context.SaveChanges();

        return RedirectToAction("OrderDetails", new { orderId = order.OrderId });
    }
    
    public ActionResult OrderList()
    {
        ViewBag.CurrencyCode = Session["SelectedCurrencyCode"]?.ToString() ?? "PLN";
        decimal exchangeRate = Session["SelectedExchangeRate"] != null ? (decimal)Session["SelectedExchangeRate"] : 1;
        ViewBag.exchangeRate = exchangeRate;
        
        var userId = Session["UserId"] as int?;
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = _context.User.Find(userId);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var orders = user.IsAdmin
            ? _context.Orders.Include("User").ToList()
            : _context.Orders.Include("User").Where(o => o.UserId == user.UserId).ToList();

        return View(orders);
    }
    public ActionResult OrderDetails(int orderId)
    {
        ViewBag.CurrencyCode = Session["SelectedCurrencyCode"]?.ToString() ?? "PLN";
        decimal exchangeRate = Session["SelectedExchangeRate"] != null ? (decimal)Session["SelectedExchangeRate"] : 1;
        ViewBag.exchangeRate = exchangeRate;
        var order = _context.Orders
            .Include("OrderItems.Product")
            .Include("User")
            .FirstOrDefault(o => o.OrderId == orderId);
        

        if (order == null)
        {
            return HttpNotFound("Nie znaleziono zamówienia.");
        }

        return View(order);
    }

    public new ActionResult Profile()
    {
        ViewBag.Currencies = _context.Currencies.ToList();
        var userId = Session["UserId"];
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
    
        var user = _context.User.Find((int)userId);
        if (user == null)
        {
            return HttpNotFound();
        }
        ViewBag.CurrencyCode = Session["SelectedCurrencyCode"]?.ToString() ?? "PLN";
    
        return View(user);
    }
    
    public ActionResult EditProfile()
    {
        var userId = Session["UserId"];
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
    
        var user = _context.User.Find((int)userId);
        if (user == null)
        {
            return HttpNotFound();
        }
    
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditProfile(User model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.User.Find(model.UserId);
            if (user != null)
            {
                user.Username = model.Username;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.Newsletter = model.Newsletter;
                user.Discount = model.Discount;
                _context.SaveChanges();
                return RedirectToAction("Profile");
            }
        }
        return View(model);
    }
    
    public ActionResult ChangePassword()
    {
        var userId = Session["UserId"];
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
    
        var user = _context.User.Find((int)userId);
        if (user == null)
        {
            return HttpNotFound();
        }
    
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        var userId = Session["UserId"];
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
    
        var user = _context.User.Find((int)userId);
        if (user == null)
        {
            return HttpNotFound();
        }
    
        if (user.Password != oldPassword)
        {
            ModelState.AddModelError("oldPassword", "Stare hasło jest niepoprawne.");
        }
    
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("confirmPassword", "Nowe hasła muszą się zgadzać.");
        }
    
        if (ModelState.IsValid)
        {
            user.Password = newPassword;
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
    
        return View();
    }
    
    [HttpPost]
    public ActionResult ChangeCurrency(int currencyId)
    {
        
        var currency = _context.Currencies.FirstOrDefault(c => c.CurrencyId == currencyId);
        if (currency != null)
        {
           
            Session["SelectedCurrencyId"] = currency.CurrencyId;
            Session["SelectedExchangeRate"] = currency.ExchangeRate;
            Session["SelectedCurrencyCode"] = currency.CurrencyCode;
        }
        return RedirectToAction("Profile"); 
    }

}