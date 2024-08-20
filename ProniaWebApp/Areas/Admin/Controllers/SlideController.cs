using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Models;
using ProniaWebApp.Utilities.Enums;
using ProniaWebApp.Utilities.Extensions;

namespace ProniaWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {

            List<Slide> slides = await _context.Slides.Where(s=>s.IsDeleted==false).ToListAsync();

            return View(slides);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}

            if (!slide.Photo.ValidateType("image/")) 
            {
                ModelState.AddModelError("Photo","File type is not correct");
                return View();            
            }

            if(!slide.Photo.ValidateSize(FileSizes.MB,2))
            {
                ModelState.AddModelError("Photo", "File size must be less than 2MB");
                return View();
            }

           

            slide.ImageURL = await slide.Photo.CreateFileAsync(_env.WebRootPath,"assets","images","website-images");

            slide.CreatedAt = DateTime.Now;

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public async Task<IActionResult> Update(int? id)
        //{
        //    if (id == null || id <= 0) return BadRequest();

        //    Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        //    if (category == null) return NotFound();

        //    return View(category);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Update(int? id, Category category)
        //{
        //    if (id == null || id <= 0) return BadRequest();

        //    Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        //    if (existed == null) return NotFound();

        //    bool result = await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id != category.Id);
        //    if (result)
        //    {
        //        ModelState.AddModelError("Name", "Name already exists");
        //        return View();

        //    }

        //    existed.Name = category.Name;

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Index");

        //}

        //public async Task<IActionResult> Delete(int id)
        //{
        //    if (id == null || id <= 0) return BadRequest();

        //    Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        //    if (category == null) return NotFound();

        //    if (category.IsDeleted)
        //    {
        //        category.IsDeleted = false;
        //    }
        //    else
        //    {
        //        category.IsDeleted = true;
        //    }

        //    //category.IsDeleted = true;
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index");


        //}
    }
}
