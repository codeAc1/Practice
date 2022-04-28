using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pratic.DAL;
using Pratic.Models;
using Pratic.ViewModels;
using Microsoft.EntityFrameworkCore;
using Pratic.ViewModels.Home;

namespace Pratic.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task <IActionResult>Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Categories = await _context.Categories.Where(c => !c.IsDeleted && c.IsMain).ToListAsync(),
                NewArrival= await _context.Products.Where(p=>!p.IsDeleted && p.IsNewArrival).ToListAsync(),
                Bestseller= await _context.Products.Where(p=>!p.IsDeleted && p.IsBestseller).ToListAsync(),
                Featured = await _context.Products.Where(p=>!p.IsDeleted && p.IsFeatured).ToListAsync()
            };
            return View(homeVM);
        }
    }
}
