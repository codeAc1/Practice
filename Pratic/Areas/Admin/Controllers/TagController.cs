using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pratic.DAL;
using Pratic.Extensions;
using Pratic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pratic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult>  Index()
        {
            
            List<Tag> tags = await _context.Tags.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrWhiteSpace(tag.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            //tag.Name = tag.Name.Trim();

            if (tag.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Tags.AnyAsync(t => t.Name.ToLower() == tag.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }
            tag.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null) return NotFound();

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Tag tag)
        {
            if (!ModelState.IsValid) return View(tag);

            if (id == null) return BadRequest();

            if (id != tag.Id) return BadRequest();

            Tag dbTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (dbTag == null) return NotFound();

            if (string.IsNullOrWhiteSpace(tag.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View(tag);
            }

            if (tag.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View(tag);
            }

            if (await _context.Tags.AnyAsync(t => t.Id != tag.Id && t.Name.ToLower() == tag.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View(tag);
            }

            dbTag.Name = tag.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Tag dbTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (dbTag == null) return NotFound();

            dbTag.IsDeleted = true;
            dbTag.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();



            return PartialView("_TagIndexPartial", await _context.Tags.OrderByDescending(t => t.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null) return BadRequest();

            Tag dbTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);

            if (dbTag == null) return NotFound();

            dbTag.IsDeleted = false;

            await _context.SaveChangesAsync();

            return PartialView("_TagIndexPartial", await _context.Tags.OrderByDescending(t => t.CreatedAt).ToListAsync());
        }
    }
}
