using System;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers
{
    public class AccountController : Controller
    {
        private ZakupowoDbContext db = new ZakupowoDbContext();

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                User existingUser = db.User.FirstOrDefault(u => u.Username == model.Username || u.Email == model.Email);
                if (existingUser != null)
                {
                    // Dodaj błąd do ModelState dla odpowiedniego komunikatu
                    if (existingUser.Username == model.Username)
                    {
                        ModelState.AddModelError("Username", "Użytkownik o takim loginie już istnieje.");
                    }
                    if (existingUser.Email == model.Email)
                    {
                        ModelState.AddModelError("Email", "Użytkownik o takim adresie e-mail już istnieje.");
                    }
                    return View(model); // Zwróć model, aby wyświetlić błędy w widoku
                }

                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password, // Hasło powinno być zahaszowane
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address
                };

                try
                {
                    db.User.Add(user);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Użytkownik został zarejestrowany pomyślnie. Możesz się teraz zalogować.";
                    return RedirectToAction("Login"); // Przekierowanie do logowania z komunikatem
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Wystąpił błąd podczas rejestracji: " + ex.Message);
                }
            }

            return View(model); // Jeśli model jest nieprawidłowy, wróć do widoku
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.User.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Zalogowano pomyślnie
                Session["UserId"] = user.UserId;
                Session["Username"] = user.Username;

                // Ustawiamy komunikat o sukcesie
                TempData["LoginSuccess"] = "Zalogowano pomyślnie!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Nieudane logowanie
                TempData["LoginError"] = "Nieprawidłowy login lub hasło. Spróbuj ponownie.";
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();  // Usuwamy dane z sesji
            TempData["LoginSuccess"] = "Zostałeś wylogowany pomyślnie.";
            return RedirectToAction("Index", "Home");
        }
    }
}