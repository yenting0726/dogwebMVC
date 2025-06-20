using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dogwebMVC.Models;

namespace dogwebMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Productsbydogweb.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productdogweb = await _context.Productsbydogweb
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productdogweb == null)
            {
                return NotFound();
            }

            return View(productdogweb);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price")] Productdogweb productdogweb, IFormFile PhotoPath)
        {
            if (ModelState.IsValid)
            {
                if (PhotoPath != null && PhotoPath.Length > 0)
                {
                    var fileName = Path.GetFileName(PhotoPath.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PhotoPath.CopyToAsync(stream);
                    }

            productdogweb.PhotoPath = "/images/" + fileName;
        }
                _context.Add(productdogweb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productdogweb);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productdogweb = await _context.Productsbydogweb.FindAsync(id);
            if (productdogweb == null)
            {
                return NotFound();
            }
            return View(productdogweb);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] Productdogweb productdogweb)
        {
            if (id != productdogweb.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productdogweb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductdogwebExists(productdogweb.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productdogweb);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productdogweb = await _context.Productsbydogweb
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productdogweb == null)
            {
                return NotFound();
            }

            return View(productdogweb);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productdogweb = await _context.Productsbydogweb.FindAsync(id);
            if (productdogweb != null)
            {
                _context.Productsbydogweb.Remove(productdogweb);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductdogwebExists(int id)
        {
            return _context.Productsbydogweb.Any(e => e.Id == id);
        }
    }
}
