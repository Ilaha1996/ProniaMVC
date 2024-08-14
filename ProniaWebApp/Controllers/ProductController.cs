using Microsoft.AspNetCore.Mvc;

namespace ProniaWebApp.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
