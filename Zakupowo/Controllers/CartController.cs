using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class CartController : Controller
{
    private ZakupowoDbContext db = new ZakupowoDbContext();

    public ActionResult Cart()
    {
        var userId = Session["UserId"] as int?;

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = db.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.CartItems.Select(ci => ci.Product)) 
            .FirstOrDefault();

        if (cart != null)
        {
            foreach (var item in cart.CartItems)
            {
                db.Entry(item).Reference(ci => ci.Product).Load();
            }
        }

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId.Value,
                CreatedAt = DateTime.Now
            };
            db.Carts.Add(cart);
            db.SaveChanges();
        }

        var itemCount = cart.CartItems?.Sum(ci => ci.Quantity) ?? 0;

        Session["CartItemCount"] = itemCount;

        return View(cart);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddToCart(int productId, int quantity)
    {
        var userId = Session["UserId"] as int?;
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = db.Carts.Include("CartItems").FirstOrDefault(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId.Value,
                CreatedAt = DateTime.Now
            };
            db.Carts.Add(cart);
            db.SaveChanges();
        }

        var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (existingCartItem != null)
        {
            existingCartItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = productId,
                Quantity = quantity
            };
            db.CartItems.Add(cartItem);
        }

        db.SaveChanges();
        
        Session["CartItemCount"] = cart.CartItems.Sum(ci => ci.Quantity);
        TempData["CartSuccess"] = "Produkt został dodany do koszyka!";
        
        return RedirectToAction("ProductList", "Product");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateQuantity(int cartItemId, int quantity)
    {
        if (quantity <= 0)
        {
            TempData["CartError"] = "Ilość musi być większa niż 0!";
            return RedirectToAction("Cart");
        }

        var cartItem = db.CartItems.Find(cartItemId);

        if (cartItem != null)
        {
            cartItem.Quantity = quantity;

            db.SaveChanges();

            TempData["CartSuccess"] = "Ilość produktu w koszyku została zaktualizowana!";
        }
        else
        {
            TempData["CartError"] = "Nie znaleziono takiego produktu w koszyku!";
        }

        return RedirectToAction("Cart");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult RemoveFromCart(int cartItemId)
    {
        var cartItem = db.CartItems.Find(cartItemId);
        if (cartItem != null)
        {
            db.CartItems.Remove(cartItem); 
            db.SaveChanges();

            var userId = Session["UserId"] as int?;
            if (userId != null)
            {
                var cart = db.Carts.Include("CartItems").FirstOrDefault(c => c.UserId == userId);
                if (cart != null)
                {
                    Session["CartItemCount"] = cart.CartItems.Sum(ci => ci.Quantity);
                }
            }

            TempData["CartSuccess"] = "Produkt został usunięty z koszyka!";
        }
        else
        {
            TempData["CartError"] = "Nie znaleziono produktu w koszyku!";
        }

        return RedirectToAction("Cart");
    }
    public ActionResult Checkout()
    {
        var paymentMethods = new List<PaymentMethod>
        {
            new PaymentMethod { Id = 1, Name = "Karta kredytowa" },
            new PaymentMethod { Id = 2, Name = "Przelew bankowy" },
            new PaymentMethod { Id = 3, Name = "PayPal" },
        };

        var shippingMethods = new List<ShippingMethod>
        {
            new ShippingMethod { Id = 1, Name = "Kurier (DPD)", Cost = 15.00m },
            new ShippingMethod { Id = 2, Name = "Paczkomat", Cost = 10.00m },
            new ShippingMethod { Id = 3, Name = "Odbiór osobisty", Cost = 0.00m },
        };

        ViewBag.PaymentMethods = paymentMethods;
        ViewBag.ShippingMethods = shippingMethods;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Checkout(int paymentMethodId, int shippingMethodId)
    {
        var paymentMethod = GetPaymentMethodById(paymentMethodId); 
        var shippingMethod = GetShippingMethodById(shippingMethodId); 

        if (paymentMethod == null || shippingMethod == null)
        {
            TempData["ErrorMessage"] = "Nieprawidłowy wybór metody płatności lub wysyłki.";
            return RedirectToAction("Checkout");
        }

        Session["SelectedPaymentMethod"] = paymentMethod.Name;
        Session["SelectedShippingMethod"] = shippingMethod.Name;
        Session["ShippingCost"] = shippingMethod.Cost;

        TempData["SuccessMessage"] = "Wybrane metody zostały zapisane. Możesz kontynuować płatność.";
        return RedirectToAction("OrderSummary", "Order");
    }

    private PaymentMethod GetPaymentMethodById(int id)
    {
        var paymentMethods = new List<PaymentMethod>
        {
            new PaymentMethod { Id = 1, Name = "Karta kredytowa" },
            new PaymentMethod { Id = 2, Name = "Przelew bankowy" },
            new PaymentMethod { Id = 3, Name = "PayPal" },
        };

        return paymentMethods.FirstOrDefault(p => p.Id == id);
    }

    private ShippingMethod GetShippingMethodById(int id)
    {
        var shippingMethods = new List<ShippingMethod>
        {
            new ShippingMethod { Id = 1, Name = "Kurier (DPD)", Cost = 15.00m },
            new ShippingMethod { Id = 2, Name = "Paczkomat", Cost = 10.00m },
            new ShippingMethod { Id = 3, Name = "Odbiór osobisty", Cost = 0.00m },
        };

        return shippingMethods.FirstOrDefault(s => s.Id == id);
    }
}