using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers;

public class UserController : Controller
{
    // GET
    private ZakupowoDbContext _context = new ZakupowoDbContext();
    
    // GET: User
    public ActionResult UserList()
    {
        var users = _context.User.ToList();
        return View(users);
    }

    // GET: User/Details/5
    public ActionResult Details(int id)
    {
        var user = _context.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }
        return View(user);
    }

    // GET: User/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: User/Create
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

    // GET: User/Edit/5
    public ActionResult Edit(int id)
    {
        var user = _context.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }
        return View(user);
    }

    // POST: User/Edit/5
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
                    user.Password = oldPassword; // Zachowaj stare hasło
                }
                else
                    user.Password = model.Password;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.IsAdmin = model.IsAdmin;
                user.Newsletter = model.Newsletter;
                _context.SaveChanges();
                return RedirectToAction("UserList");
            }
        }
        return View(model);
    }

    // GET: User/Delete/5
    public ActionResult Delete(int id)
    {
        var user = _context.User.Find(id);
        if (user == null)
        {
            return HttpNotFound();
        }
        return View(user);
    }

    // POST: User/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(User model)
    {
        var user = _context.User.Find(model.UserId);
        if (user != null)
        {
            _context.User.Remove(user);
            _context.SaveChanges();
            TempData["mes"] = $"User {user.Username} has been deleted";
        }
        return RedirectToAction("UserList");
    }
}