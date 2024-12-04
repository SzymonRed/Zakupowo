using System.Web.Mvc;

namespace Zakupowo.Controllers;

public class ThemeController : Controller
{
    public ActionResult ChangeTheme(string theme)
    {
        Session["SelectedTheme"] = theme;
        return Redirect(Request.UrlReferrer?.ToString() ?? "/");
    }
}