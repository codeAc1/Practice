using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pratic.DAL;
using Pratic.Extensions;
using Pratic.Helpers;
using Pratic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pratic.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CategoryController : Controller
    {
        

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public  IActionResult Index(bool? status, bool? isMainRoute, int page = 1)
        {
            ViewBag.Status = status;
            ViewBag.isMainRoute = isMainRoute;
            
            IQueryable<Category> categories =  _context.Categories
                .Include(c => c.Products)
                .OrderByDescending(c => c.CreatedAt)
                .AsQueryable();

            if (status != null)
                categories = categories.Where(c => c.IsDeleted == status);

            if(isMainRoute != null)
                categories = categories.Where(c => c.IsMain == isMainRoute);

            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)categories.Count() / 5);
            return View(categories.Skip((page - 1) * 5).Take(5).ToList());
        }

        public async Task<IActionResult>  Create()
        {
            ViewBag.MainCategory = await _context.Categories.Where(c => c.IsMain && !c.IsDeleted).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, bool? status, bool? isMainRoute, int page = 1)
        {
            ViewBag.MainCategory = await _context.Categories.Where(c => c.IsMain && !c.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            //tag.Name = tag.Name.Trim();

            if (category.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Categories.AnyAsync(t => t.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }

            if (category.IsMain)
            {
                category.ParentId = null;
                if (category.CategoryImage==null)
                {
                    ModelState.AddModelError("CategoryImage", "Sekil mutleq olmalidir");
                    return View();
                }

                if (!category.CategoryImage.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("LogoImage", "Secilen Seklin Novu Uygun");
                    return View(category);
                }

                if (!category.CategoryImage.CheckFileSize(30))
                {
                    ModelState.AddModelError("CategoryImage", "Secilen Seklin Olcusu Maksimum 30 Kb Ola Biler");
                    return View(category);
                }


                
                category.Image = category.CategoryImage.CreateFile(_env, "assets", "images");
            }
            else
            {
                if (category.ParentId==null)
                {
                    ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir");
                    return View(category);
                }
                if (!await _context.Categories.AnyAsync(c=>c.Id==category.ParentId && !c.IsDeleted && c.IsMain))
                {
                    ModelState.AddModelError("ParentId", "ParentId yalnisdir");
                    return View(category);
                }
            }

            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

             return RedirectToAction("Index", new { status = status, page = page , isMainRoute=isMainRoute });
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();


            
            
            ViewBag.MainCategory = await _context.Categories.Where(c =>c.Id!=id && c.IsMain && !c.IsDeleted).ToListAsync();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category, bool? status, bool? isMainRoute, int page = 1)
        {
            ViewBag.MainCategory = await _context.Categories.Where(c => c.Id != id && c.IsMain && !c.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (id != category.Id) return BadRequest();
            

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            //tag.Name = tag.Name.Trim();

            if (category.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Categories.AnyAsync(t => t.Id!=id && t.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }
            if (category.ParentId!=null && id==category.ParentId)
            {
                ModelState.AddModelError("ParentId", "Duzgun Parent sec");
                return View(category);
            }

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (dbCategory == null) return NotFound();

            if (category.IsMain)
            {
                if (!dbCategory.IsMain && category.CategoryImage==null)
                {
                    ModelState.AddModelError("CategoryImage", "Sekil mutleq secilmelidir");
                    return View(category);
                }
                
                if (category.CategoryImage != null)
                {
                    if (!category.CategoryImage.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("CategoryImage", "Secilen Seklin Novu Uygun");
                        return View(category);
                    }

                    if (!category.CategoryImage.CheckFileSize(30))
                    {
                        ModelState.AddModelError("CategoryImage", "Secilen Seklin Olcusu Maksimum 30 Kb Ola Biler");
                        return View(category);
                    }
                    if (dbCategory.Image != null)
                    {
                        Helper.DeleteFile(_env, dbCategory.Image, "assets", "images");
                    }



                    dbCategory.Image = category.CategoryImage.CreateFile(_env, "assets", "images");
                }

                dbCategory.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir");
                    return View(category);
                }
                if (!await _context.Categories.AnyAsync(c => c.Id == category.ParentId && !c.IsDeleted && c.IsMain))
                {
                    ModelState.AddModelError("ParentId", "ParentId yalnisdir");
                    return View(category);
                }
                if (dbCategory.Image!=null)
                {
                    Helper.DeleteFile(_env, dbCategory.Image, "assets", "images");
                }
                
                dbCategory.ParentId = category.ParentId;
                dbCategory.Image = null;

            }
            dbCategory.IsMain = category.IsMain;
            dbCategory.Name = category.Name;
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
           
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { status = status, page = page , isMainRoute=isMainRoute });

        }

        public async Task<IActionResult> Delete(int? id, bool? status, bool? isMainRoute, int page = 1)
        {
            if (id == null) return BadRequest();

            Category dbCategory = await _context.Categories.Include(c=>c.Children).FirstOrDefaultAsync(t => t.Id == id);
            

            if (dbCategory == null) return NotFound();

            dbCategory.IsDeleted = true;
            dbCategory.DeletedAt = DateTime.UtcNow.AddHours(4);

            foreach (Category child in dbCategory.Children.Where(c=>!c.IsDeleted))
            {
                child.IsDeleted = true;
                child.DeletedAt = DateTime.UtcNow.AddHours(4);
            }
            

            await _context.SaveChangesAsync();

            ViewBag.Status = status;
            IEnumerable<Category> categories = await _context.Categories
                .Include(t => t.Products)
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)categories.Count() / 5);
            return PartialView("_CategoryIndexPartial", categories.Skip((page - 1) * 5).Take(5));

        }

        public async Task<IActionResult> Restore(int? id, bool? status, bool? isMainRoute, int page = 1)
        {
            if (id == null) return BadRequest();

            Category dbCategory = await _context.Categories.Include(c=>c.Children).FirstOrDefaultAsync(t => t.Id == id);
            

            if (dbCategory == null) return NotFound();

            foreach (Category child in dbCategory.Children)
            {
               child.IsDeleted = false;
            }
            dbCategory.IsDeleted = false;
            dbCategory.DeletedAt = null;

            await _context.SaveChangesAsync();

            ViewBag.Status = status;
            IEnumerable<Category> categories = await _context.Categories
                .Include(t => t.Products)
                .Where(t => status != null ? t.IsDeleted == status : true)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            ViewBag.PageIndex = page;
            ViewBag.PageCount = Math.Ceiling((double)categories.Count() / 5);
            return PartialView("_CategoryIndexPartial", categories.Skip((page - 1) * 5).Take(5));
        }
    }
}
