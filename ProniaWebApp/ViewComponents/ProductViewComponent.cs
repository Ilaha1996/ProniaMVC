using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Models;
using ProniaWebApp.Utilities.Enums;

namespace ProniaWebApp.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        private readonly AppDBContext _context;

        public ProductViewComponent(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(SortType type)
        {
            IQueryable <Product> query = _context.Products.Where(p=>p.IsDeleted==false);
            switch (type)
            {                
                case SortType.Name:
                    query = query.OrderBy(p => p.Name);
                    break;
                case SortType.Price:
                    query = query.OrderByDescending(q => q.Price);
                    break;
                case SortType.Newest:
                    query = query.OrderByDescending(q => q.CreatedAt);
                    break;              
            }
            query = query.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null));

            return View(await query.ToListAsync());

            //return View(await Task.FromResult(products)); evvel bu tipde return edirdiler, .NET-in kechmis versiyalarinda
        }

    }
}
// Footer hisseni de View componentle getire bilerik
// public async Task<IViewComponentResult> InvokeAsync()
//{
//    var settings =await _context.Settings.ToDictionaryAsync(s=>s.Key, s=>s.Value);
//    return View (settings);
//} diger hissler de ProductViewComponentdeki kimi ede bilerik...