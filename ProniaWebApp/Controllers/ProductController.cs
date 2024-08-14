using Microsoft.AspNetCore.Mvc;
using ProniaWebApp.DAL;

namespace ProniaWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDBContext _context;
        public ProductController(AppDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
