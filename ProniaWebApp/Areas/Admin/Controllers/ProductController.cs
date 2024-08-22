﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.Areas.Admin.ViewModels;
using ProniaWebApp.DAL;
using ProniaWebApp.Models;
using ProniaWebApp.Utilities.Enums;
using ProniaWebApp.Utilities.Extensions;

namespace ProniaWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetAdminProductVM> productVMs = await _context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .Select(p => new GetAdminProductVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    Image = p.ProductImages.FirstOrDefault().ImageURL
                })
                .ToListAsync();
            return View(productVMs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync(),
                Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync()
            };
            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            productVM.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(productVM);


            if(!productVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("MainPhoto", "File type is not correct");
                return View(productVM);

            }
            if (!productVM.MainPhoto.ValidateSize(FileSizes.MB,1))
            {
                ModelState.AddModelError("MainPhoto", "File size must be less than 1MB");
                return View(productVM);

            }

            if (!productVM.HoverPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("HoverPhoto", "File type is not correct");
                return View(productVM);

            }
            if (!productVM.HoverPhoto.ValidateSize(FileSizes.MB, 1))
            {
                ModelState.AddModelError("HoverPhoto", "File size must be less than 1MB");
                return View(productVM);

            }


            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId && c.IsDeleted == false);
            if (!result)
            {
                ModelState.AddModelError("CategoryId", "Category does not exist");
                return View(productVM);

            }

            bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
            if (tagResult) 
            {
                ModelState.AddModelError("TagIds", "Tag does not exist");
                return View(productVM);            
            
            }

            ProductImage mainImage = new ProductImage
            {
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                IsPrimary = true,
                ImageURL = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")

            };

            ProductImage hoverImage = new ProductImage
            {
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                IsPrimary = false,
                ImageURL = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")

            };

            Product product = new Product
            {
                CategoryId = productVM.CategoryId.Value,
                SKU = productVM.SKU,
                Description = productVM.Description,
                Name = productVM.Name,
                Price = productVM.Price.Value,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                ProductImages= new List<ProductImage> {mainImage,hoverImage },
                ProductTags = productVM.TagIds.Select(tId => new ProductTag
                {
                    TagId = tId

                }).ToList()

            };

            // yuxarida bir setirlik kodun daha uzun versiyasidir asagiki
            //foreach (var tId in productVM.TagIds)
            //{
            //    ProductTag pTag = new ProductTag { TagId = tId, Product = product};
            //    _context.ProductTags.Add(pTag);
            //}

            if(productVM is not null)
            {
                string text = string.Empty;
                foreach (IFormFile file in productVM.Photos)
                {
                    if (!file.ValidateType("image/"))
                    {
                        text += $"{file.FileName} named file's type is not correct";
                        continue;
                    }
                    if (!file.ValidateSize(FileSizes.MB, 1))
                    {
                        text += $"{file.FileName} named file's size must be less than 1 Mb";
                        continue;
                    }
                    ProductImage image = new ProductImage
                    {
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        IsPrimary = null,
                        ImageURL = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
                    };
                    product.ProductImages.Add(image);
                }
                TempData["ErrorMessage"] = text;


            }
            
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            if (product == null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync()
            };
            return View(productVM);

        }

        [HttpPost]

        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            if (id == null || id <= 0) return BadRequest();
            Product existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            if (existed == null) return NotFound();

            productVM.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            if (!ModelState.IsValid) return View(productVM);

            if (existed.CategoryId != productVM.CategoryId)
            {
                bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId && c.IsDeleted == false);
                if (!result)
                {
                    ModelState.AddModelError("CategoryId", "Category does not exist");
                    return View(productVM);
                }

            }

            return View(productVM);

        }



    }
}
