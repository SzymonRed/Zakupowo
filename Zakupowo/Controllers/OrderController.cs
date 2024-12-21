using System;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class OrderController : Controller
{
    private readonly ZakupowoDbContext _db = new ZakupowoDbContext();
    public ActionResult OrderSummary()
    {
        
        decimal exchangeRate = Session["SelectedExchangeRate"] != null ? (decimal)Session["SelectedExchangeRate"] : 1;
        string currencyCode = Session["SelectedCurrencyCode"]?.ToString() ?? "PLN";
        var userId = (int?)Session["UserId"];
        if (userId == null)
        {
            TempData["ErrorMessage"] = "Musisz być zalogowany, aby złożyć zamówienie.";
            return RedirectToAction("Login", "Account");
        }

        var cart = _db.Carts
            .Include("CartItems.Product.VatRate")
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            TempData["ErrorMessage"] = "Twój koszyk jest pusty.";
            return RedirectToAction("Cart", "Cart");
        }

        var selectedPaymentMethod = Session["SelectedPaymentMethod"];
        var selectedShippingMethod = Session["SelectedShippingMethod"];
        var shippingCost = Session["ShippingCost"];

        if (selectedPaymentMethod == null || selectedShippingMethod == null || shippingCost == null)
        {
            TempData["ErrorMessage"] = "Nie wybrano metody płatności lub wysyłki.";
            return RedirectToAction("Cart", "Cart");
        }

        var netPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
        var vatAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price * (ci.Product.VatRate.Rate ?? 0));
        var totalPrice = netPrice + vatAmount + (decimal)shippingCost;

        ViewBag.NetPrice = netPrice;
        ViewBag.VatAmount = vatAmount;
        ViewBag.TotalPrice = totalPrice;
        ViewBag.SelectedPaymentMethod = selectedPaymentMethod;
        ViewBag.SelectedShippingMethod = selectedShippingMethod;
        ViewBag.ShippingCost = shippingCost;
        ViewBag.Cart = cart;
        ViewBag.CurrencyCode = currencyCode;

        return View(cart);
    }

   public ActionResult PlaceOrder()
    {
        var userId = (int?)Session["UserId"];
        if (userId == null)
        {
            TempData["ErrorMessage"] = "Musisz być zalogowany, aby złożyć zamówienie.";
            return RedirectToAction("Login", "Account");
        }

        var cart = _db.Carts
            .Include("CartItems.Product.VatRate")
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            TempData["ErrorMessage"] = "Twój koszyk jest pusty.";
            return RedirectToAction("Cart", "Cart");
        }

        var selectedPaymentMethod = Session["SelectedPaymentMethod"]?.ToString();
        var selectedShippingMethod = Session["SelectedShippingMethod"]?.ToString();
        var shippingCost = (decimal?)Session["ShippingCost"];

        if (selectedPaymentMethod == null || selectedShippingMethod == null || shippingCost == null)
        {
            TempData["ErrorMessage"] = "Nie wybrano metody płatności lub wysyłki.";
            return RedirectToAction("Cart", "Cart");
        }

        var netPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
        var vatAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price * (ci.Product.VatRate.Rate ?? 0));
        var totalPrice = netPrice + vatAmount + shippingCost.Value;

        var order = new Order
        {
            OrderDate = DateTime.Now,
            UserId = userId.Value,
            Status = "Pending",
            TotalPrice = totalPrice
        };

        _db.Orders.Add(order);
        _db.SaveChanges();

        foreach (var cartItem in cart.CartItems)
        {
            
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                ItemPrice = cartItem.Product.Price + (cartItem.Product.Price * (cartItem.Product.VatRate.Rate ?? 0)),
            };
            _db.OrderItems.Add(orderItem);
            
            var product = _db.Products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
            if (product != null)
            {
                product.Stock -= cartItem.Quantity;
                
                if (product.Stock < 0)
                {
                    TempData["ErrorMessage"] = $"Produkt {product.Name} nie jest dostępny w wystarczającej ilości.";
                    return RedirectToAction("Cart", "Cart");
                }
            }
        }

        _db.SaveChanges();

        _db.CartItems.RemoveRange(cart.CartItems);
        _db.Carts.Remove(cart);
        _db.SaveChanges();

        TempData["SuccessMessage"] = "Zamówienie zostało złożone pomyślnie!";
        return RedirectToAction("OrderDetails", new { id = order.OrderId });
    }

    public ActionResult OrderDetails(int id)
    {
        var order = _db.Orders
            .Include("OrderItems.Product")
            .FirstOrDefault(o => o.OrderId == id);

        if (order == null || order.UserId != (int?)Session["UserId"])
        {
            TempData["ErrorMessage"] = "Nie znaleziono zamówienia.";
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Order = order;

        return View(order);
    }
}