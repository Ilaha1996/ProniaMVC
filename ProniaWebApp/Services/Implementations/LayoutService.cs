using Microsoft.EntityFrameworkCore;
using ProniaWebApp.DAL;
using ProniaWebApp.Services.Interfaces;

namespace ProniaWebApp.Services.Implementations
{
    public class LayoutService:ILayoutService
    {
        private readonly AppDBContext _context;

        public LayoutService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string,string>> GetSettings()
        {
         return await _context.Settings.ToDictionaryAsync(s=>s.Key, s => s.Value);
        }

    }
}
