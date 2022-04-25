using Microsoft.AspNetCore.Mvc;
using Pratic.DAL;
using Pratic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pratic.ViewComponents
{
    public class HeaderViewComponent: ViewComponent
    {
        private readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Setting model = _context.Settings.FirstOrDefault();
            return View(await Task.FromResult(model));
        }
    }
}
