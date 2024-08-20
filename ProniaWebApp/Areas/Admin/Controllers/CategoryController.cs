using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Models;

namespace ProniaWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.Include(c => c.Products).Where(c=>c.IsDeleted==false).ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Categories.AnyAsync(c=>c.Name.Trim()==category.Name.Trim());
            if (result) 
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View();
            
            }

            category.CreatedAt = DateTime.Now;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> Update(int? id) 
        {
            if (id == null || id <= 0) return BadRequest();    
            
            Category category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
           
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if(id == null || id <= 0)  return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();

            bool result = await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id!=category.Id);
            if (result) 
            {
                ModelState.AddModelError("Name","Name already exists");
                return View();
            
            }

            existed.Name = category.Name;
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || id <= 0) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            if(category.IsDeleted)
            {
                category.IsDeleted = false;
            }
            else
            {
                category.IsDeleted = true;
            }

            //category.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");


        }


    }
}

