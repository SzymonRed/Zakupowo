using System;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

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
                if (existingUser.Username == model.Username)
                {
                    ModelState.AddModelError("Username", "Użytkownik o takim loginie już istnieje.");
                }
                if (existingUser.Email == model.Email)
                {
                    ModelState.AddModelError("Email", "Użytkownik o takim adresie e-mail już istnieje.");
                }
                return View(model); 
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password, 
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
                return RedirectToAction("Login"); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas rejestracji: " + ex.Message);
            }
        }

        return View(model); 
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
            Session["UserId"] = user.UserId;
            Session["Username"] = user.Username;
            Session["UserDiscount"] = user.Discount ?? 0;

            TempData["LoginSuccess"] = "Zalogowano pomyślnie!";
            Session["IsAdmin"] = user.IsAdmin;
            return RedirectToAction("Index", "Home");
        }
        else
        {
            TempData["LoginError"] = "Nieprawidłowy login lub hasło. Spróbuj ponownie.";
            return RedirectToAction("Login");
        }
    }

    public ActionResult Logout()
    {
        Session.Clear();
        TempData["LoginSuccess"] = "Zostałeś wylogowany pomyślnie.";
        return RedirectToAction("Index", "Home");
    }
}