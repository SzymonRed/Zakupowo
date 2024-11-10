using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

public class CartController : Controller
{
    private ZakupowoDbContext db = new ZakupowoDbContext();

    // Widok koszyka
   public ActionResult Cart()
{
    var userId = Session["UserId"] as int?;
    
    if (userId == null)
    {
        return RedirectToAction("Login", "Account");
    }

        // Pobierz koszyk użytkownika z bazy danych
        var cart = db.Carts
     .Where(c => c.UserId == userId)
     .Include(c => c.CartItems.Select(ci => ci.Product)) // ładowanie Product z CartItems
     .FirstOrDefault();

        if (cart != null)
        {
            // Dodatkowe ładowanie produktów
            foreach (var item in cart.CartItems)
            {
                db.Entry(item).Reference(ci => ci.Product).Load();
            }
        }

        // Jeśli koszyk nie istnieje, tworzymy nowy
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

    // Sprawdzamy liczbę przedmiotów w koszyku
    var itemCount = cart.CartItems?.Sum(ci => ci.Quantity) ?? 0;

    // Zapisujemy liczbę produktów w koszyku do sesji
    Session["CartItemCount"] = itemCount;

    // Zwracamy koszyk do widoku
    return View(cart);
}

    // Dodanie produktu do koszyka
    // Dodanie produktu do koszyka
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddToCart(int productId, int quantity)
    {
        var userId = Session["UserId"] as int?;
        if (userId == null)
        {
            // Jeśli użytkownik nie jest zalogowany, przekieruj go do strony logowania
            return RedirectToAction("Login", "Account");
        }

        // Szukamy koszyka użytkownika w bazie danych
        var cart = db.Carts.Include("CartItems").FirstOrDefault(c => c.UserId == userId);
        if (cart == null)
        {
            // Jeśli koszyk nie istnieje, tworzymy nowy
            cart = new Cart
            {
                UserId = userId.Value,
                CreatedAt = DateTime.Now
            };
            db.Carts.Add(cart);
            db.SaveChanges();
        }

        // Sprawdzamy, czy produkt już jest w koszyku
        var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (existingCartItem != null)
        {
            // Jeśli produkt jest w koszyku, zwiększamy jego ilość
            existingCartItem.Quantity += quantity;
        }
        else
        {
            // Jeśli produkt nie jest w koszyku, dodajemy go
            var cartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = productId,
                Quantity = quantity
            };
            db.CartItems.Add(cartItem);
        }

        // Zapisujemy zmiany w bazie danych
        db.SaveChanges();

        // Zaktualizowanie liczby produktów w koszyku w sesji
        Session["CartItemCount"] = cart.CartItems.Sum(ci => ci.Quantity);

        // Dodajemy komunikat o sukcesie do TempData
        TempData["CartSuccess"] = "Produkt został dodany do koszyka!";

        // Przekierowanie z powrotem do widoku produktów (lub innej odpowiedniej akcji)
        return RedirectToAction("ProductList", "Product");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateQuantity(int cartItemId, int quantity)
    {
        // Sprawdzamy, czy ilość jest poprawna (większa niż 0)
        if (quantity <= 0)
        {
            TempData["CartError"] = "Ilość musi być większa niż 0!";
            return RedirectToAction("Cart");
        }

        // Pobieramy CartItem na podstawie jego ID
        var cartItem = db.CartItems.Find(cartItemId);

        // Sprawdzamy, czy znaleźliśmy odpowiedni element w koszyku
        if (cartItem != null)
        {
            // Aktualizujemy ilość
            cartItem.Quantity = quantity;

            // Zapisujemy zmiany w bazie danych
            db.SaveChanges();

            // Ustawiamy komunikat o sukcesie
            TempData["CartSuccess"] = "Ilość produktu w koszyku została zaktualizowana!";
        }
        else
        {
            // Jeżeli nie znaleziono produktu, ustawiamy komunikat o błędzie
            TempData["CartError"] = "Nie znaleziono takiego produktu w koszyku!";
        }

        // Przekierowujemy z powrotem do widoku koszyka
        return RedirectToAction("Cart");
    }

    // Usuwanie produktu z koszyka
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult RemoveFromCart(int cartItemId)
    {
        var cartItem = db.CartItems.Find(cartItemId);
        if (cartItem != null)
        {
            db.CartItems.Remove(cartItem); // Usuwamy produkt z koszyka
            db.SaveChanges();

            // Po usunięciu produktu, aktualizujemy liczbę produktów w koszyku
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
}