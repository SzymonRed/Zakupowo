using System.Linq;
using System.Web.Mvc;
using Zakupowo.Models;

namespace Zakupowo.Controllers
{
    public class ProductController : Controller
    {
        // GET
        private ZakupowoDbContext _context = new ZakupowoDbContext();
        public ActionResult ProductList()
        {
            var products = _context.Products.ToList();
            return View(products);
        }
    }
}