using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Models;
using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDBContext _context;
        public HomeController(AppDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            
            HomeVM homeVM = new HomeVM 
            { 
                Slides = _context.Slides.OrderBy(s => s.Order).Take(2).ToList(),
                Products = _context.Products
                .OrderByDescending(p=>p.CreatedAt)
                .Take(8)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .ToList()
            }; 
            return View(homeVM);
        }
    }
}
