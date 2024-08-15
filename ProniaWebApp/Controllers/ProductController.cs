using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Models;
using ProniaWebApp.ViewModels;

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

        public IActionResult Detail(int? id) 
        {
            if (id == null || id <= 0) 
            {
                return BadRequest();            
            }

            Product product = _context.Products.Include(p=>p.Category)
                .Include(p=>p.ProductImages.OrderByDescending(pi=>pi.IsPrimary))
                .FirstOrDefault(p => p.Id == id);
            
            if (product == null) 
            {
                return NotFound();
            }

            DetailVM detailVM = new DetailVM 
            { 
                product = product,
                products=_context.Products.Where(p=>p.CategoryId==product.CategoryId && p.Id!=id)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .ToList()
            };

            return View(detailVM);        
        
        }
    }
}
