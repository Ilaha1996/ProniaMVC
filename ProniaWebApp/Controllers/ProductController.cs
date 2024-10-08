﻿using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Detail(int? id) 
        {
            if (id == null || id <= 0) 
            {
                return BadRequest();            
            }

            Product? product = await _context.Products.Include(p=>p.Category)
                .Include(p=>p.ProductImages.OrderByDescending(pi=>pi.IsPrimary))
                .Include(p=>p.ProductTags)
                .ThenInclude(pt=>pt.Tag)
                .Include(p=>p.ProductColors)
                .ThenInclude(pc=>pc.Color)
                .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (product == null) 
            {
                return NotFound();
            }

            DetailVM detailVM = new DetailVM 
            { 
                product = product,
                products=await _context.Products.Where(p=>p.CategoryId==product.CategoryId && p.Id!=id)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .Take(8)
                .ToListAsync()
            };

            return View(detailVM);        
        
        }
    }
}
