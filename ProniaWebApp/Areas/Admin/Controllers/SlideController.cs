using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.Areas.Admin.ViewModels;
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
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!slideVM.Photo.ValidateType("image/")) 
            {
                ModelState.AddModelError("Photo","File type is not correct");
                return View();            
            }

            if(!slideVM.Photo.ValidateSize(FileSizes.MB,2))
            {
                ModelState.AddModelError("Photo", "File size must be less than 2MB");
                return View();
            }           

            string filename = await slideVM.Photo.CreateFileAsync(_env.WebRootPath,"assets","images","website-images");

            Slide slide = new Slide
            {
                Title = slideVM.Title,
                SubTitle=slideVM.SubTitle,
                Description = slideVM.Description,
                Order = slideVM.Order,
                CreatedAt=DateTime.Now,
                IsDeleted=false,
                ImageURL=filename
            };
            
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id <= 0) return BadRequest();

            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slide == null) return NotFound();

            UpdateSlideVM slideVM = new UpdateSlideVM
            {
                ImageURL = slide.ImageURL,
                Title = slide.Title,
                SubTitle = slide.SubTitle,
                Description = slide.Description,
                Order = slide.Order,
            };

            return View(slideVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateSlideVM slideVM)
        {
            if(!ModelState.IsValid)  return View(slideVM);

            Slide existed = await _context.Slides.FirstOrDefaultAsync(s=>s.Id==id);
            if (existed == null) return NotFound();

            if(slideVM.Photo is not null)
            {
                if (!slideVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type is not correct");
                    return View(slideVM);
                }

                if (!slideVM.Photo.ValidateSize(FileSizes.MB, 2))
                {
                    ModelState.AddModelError("Photo", "File size must be less than 2MB");
                    return View(slideVM);
                }

                string filename = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                existed.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ImageURL=filename;

            }

            existed.Title = slideVM.Title;
            existed.Description = slideVM.Description;
            existed.SubTitle = slideVM.SubTitle;
            existed.Order = slideVM.Order;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Hard delete (Soft delete categoryControllerde)
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || id <= 0) return BadRequest();

            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide == null) return NotFound();

            slide.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");            

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");


        }
    }
}
