using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO; // 加入這個 using
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price")] Productdogweb productdogweb, IFormFile PhotoPath)
        {
            // 清除 PhotoPath 的 ModelState 錯誤，因為我們是透過 IFormFile 處理的
    if (ModelState.ContainsKey("PhotoPath"))
    {
        ModelState.Remove("PhotoPath");
    }
            // 除錯訊息 - 檢查接收到的資料
            System.Diagnostics.Debug.WriteLine($"=== CREATE DEBUG ===");
    System.Diagnostics.Debug.WriteLine($"Name: {productdogweb.Name}");
    System.Diagnostics.Debug.WriteLine($"Price: {productdogweb.Price}");
    System.Diagnostics.Debug.WriteLine($"PhotoPath is null: {PhotoPath == null}");
    if (PhotoPath != null)
    {
        System.Diagnostics.Debug.WriteLine($"File name: {PhotoPath.FileName}");
        System.Diagnostics.Debug.WriteLine($"File size: {PhotoPath.Length} bytes");
    }
    System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
    
    // 如果 ModelState 無效，列出所有錯誤
    if (!ModelState.IsValid)
    {
        System.Diagnostics.Debug.WriteLine("=== ModelState Errors ===");
        foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
        {
            System.Diagnostics.Debug.WriteLine($"Error: {modelError.ErrorMessage}");
        }
        
        // 在頁面上也顯示錯誤（測試用）
        ViewBag.DebugMessage = "ModelState 驗證失敗: " + 
            string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        return View(productdogweb);
    }

    if (ModelState.IsValid)
    {
        if (PhotoPath != null && PhotoPath.Length > 0)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("開始處理檔案上傳...");
                
                // 檢查檔案大小 (限制 5MB)
                if (PhotoPath.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("PhotoPath", "檔案大小不能超過 5MB");
                    ViewBag.DebugMessage = "檔案太大";
                    return View(productdogweb);
                }

                // 檢查檔案類型
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };
                var fileExtension = Path.GetExtension(PhotoPath.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("PhotoPath", "只允許上傳圖片檔案");
                    ViewBag.DebugMessage = $"不支援的檔案類型: {fileExtension}";
                    return View(productdogweb);
                }

                // 產生唯一檔名避免重複
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                
                System.Diagnostics.Debug.WriteLine($"上傳資料夾: {uploadsFolder}");
                
                // 確保目錄存在
                if (!Directory.Exists(uploadsFolder))
                {
                    System.Diagnostics.Debug.WriteLine("建立上傳資料夾...");
                    Directory.CreateDirectory(uploadsFolder);
                }
                
                var filePath = Path.Combine(uploadsFolder, fileName);
                System.Diagnostics.Debug.WriteLine($"檔案路徑: {filePath}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PhotoPath.CopyToAsync(stream);
                }

                productdogweb.PhotoPath = "/images/" + fileName;
                System.Diagnostics.Debug.WriteLine($"設定 PhotoPath: {productdogweb.PhotoPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"檔案上傳錯誤: {ex.Message}");
                ModelState.AddModelError("PhotoPath", "檔案上傳失敗: " + ex.Message);
                ViewBag.DebugMessage = "檔案上傳失敗: " + ex.Message;
                return View(productdogweb);
            }
        }

        try
        {
            System.Diagnostics.Debug.WriteLine("準備儲存到資料庫...");
            _context.Add(productdogweb);
            await _context.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine("資料庫儲存成功！");
            
            ViewBag.DebugMessage = "儲存成功！即將跳轉...";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"資料庫儲存錯誤: {ex.Message}");
            ViewBag.DebugMessage = "資料庫儲存失敗: " + ex.Message;
            return View(productdogweb);
        }
    }
    
    ViewBag.DebugMessage = "未知錯誤";
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,PhotoPath")] Productdogweb productdogweb, IFormFile Photopath)
        {
           if (id != productdogweb.Id)
    {
        return NotFound();
    }

    // 先從資料庫取得現有資料
    var existingProduct = await _context.Productsbydogweb.FindAsync(id);
    if (existingProduct == null)
    {
        return NotFound();
    }

    // 更新基本欄位
    existingProduct.Name = productdogweb.Name;
    existingProduct.Price = productdogweb.Price;

    // 清除 PhotoPath 的 ModelState 錯誤，因為我們是透過 IFormFile 處理的
    if (ModelState.ContainsKey("PhotoPath"))
    {
        ModelState.Remove("PhotoPath");
    }

    if (ModelState.IsValid)
    {
        try
        {
            // 如果有新的檔案上傳
            if (Photopath != null && Photopath.Length > 0)
            {
                // 檢查檔案大小
                if (Photopath.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("Photopath", "檔案大小不能超過 5MB");
                    return View(productdogweb);
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };
                var fileExtension = Path.GetExtension(Photopath.FileName).ToLowerInvariant();

                // 檢查檔案格式
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("Photopath", "只允許上傳 jpg, jpeg, png, gif, jfif 格式的檔案");
                    return View(productdogweb);
                }

                // 刪除舊檔案
                if (!string.IsNullOrEmpty(existingProduct.PhotoPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existingProduct.PhotoPath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // 上傳新檔案
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Photopath.CopyToAsync(stream);
                }

                existingProduct.PhotoPath = "/images/" + fileName;
            }

            // 更新資料庫
            _context.Update(existingProduct);
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
        catch (Exception ex)
        {
            ModelState.AddModelError("", "更新失敗: " + ex.Message);
            return View(productdogweb);
        }
        return RedirectToAction(nameof(Index));
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
                // 刪除相關的圖片檔案
                if (!string.IsNullOrEmpty(productdogweb.PhotoPath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + productdogweb.PhotoPath);
                    if (System.IO.File.Exists(filePath))
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                        }
                        catch
                        {
                            // 檔案刪除失敗不影響資料庫刪除
                        }
                    }
                }

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