using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zakupowo.Controllers;

public class HomeController : Controller
{
    public ActionResult Index()
    {
        var visitCount = VisitCounter.Increment();
        Session["visitCount"] = visitCount;
        return View();
    }

    public ActionResult Login()
    {
        return View();
    }

    public ActionResult About()
    {
        ViewBag.Message = "Informacje";

        return View();
    }

    public ActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";

        return View();
    }
}
public static class VisitCounter
{
    private static int _count = 0;

    public static int Increment()
    {
        _count++;
        return _count;
    }

    public static int GetCount()
    {
        return _count;
    }
}
