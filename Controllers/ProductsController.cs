// ProductsController.cs
// 此控制器負責處理產品的 CRUD 操作，包含圖片上傳與刪除。

using System; // 基本系統命名空間
using System.Collections.Generic; // 提供集合型別，如 List<T>
using System.Linq; // 提供 LINQ 查詢方法
using System.Threading.Tasks; // 提供非同步支援
using System.IO; // 處理檔案與目錄的功能
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC 功能
using Microsoft.AspNetCore.Mvc.Rendering; // 提供 HTML UI 元件（如 DropDownList）
using Microsoft.EntityFrameworkCore; // Entity Framework Core ORM 功能
using dogwebMVC.Models; // 引入專案中的資料模型命名空間

namespace dogwebMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context; // 資料庫上下文，用來操作資料表

        // 建構函式：透過依賴注入注入資料庫上下文
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // 產品列表頁：顯示所有產品資料
        public async Task<IActionResult> Index()
        {
            return View(await _context.Productsbydogweb.ToListAsync());
        }

        // 詳細頁：顯示單一產品資料
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var productdogweb = await _context.Productsbydogweb.FirstOrDefaultAsync(m => m.Id == id);
            if (productdogweb == null) return NotFound();

            return View(productdogweb);
        }

        // 顯示新增產品的表單
        public IActionResult Create()
        {
            return View();
        }

        // 接收新增產品表單送出（含圖片）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price")] Productdogweb productdogweb, IFormFile PhotoPath)
        {
            // 移除模型驗證中因 IFormFile 造成的錯誤
            if (ModelState.ContainsKey("PhotoPath")) ModelState.Remove("PhotoPath");

            // 除錯輸出
            System.Diagnostics.Debug.WriteLine("=== CREATE DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"Name: {productdogweb.Name}");
            System.Diagnostics.Debug.WriteLine($"Price: {productdogweb.Price}");
            System.Diagnostics.Debug.WriteLine($"PhotoPath is null: {PhotoPath == null}");

            if (PhotoPath != null)
            {
                System.Diagnostics.Debug.WriteLine($"File name: {PhotoPath.FileName}");
                System.Diagnostics.Debug.WriteLine($"File size: {PhotoPath.Length} bytes");
            }

            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // 驗證失敗則回傳錯誤訊息與畫面
            if (!ModelState.IsValid)
            {
                ViewBag.DebugMessage = "ModelState 驗證失敗: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(productdogweb);
            }

            // 檢查並儲存上傳圖片
            if (PhotoPath != null && PhotoPath.Length > 0)
            {
                try
                {
                    if (PhotoPath.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("PhotoPath", "檔案大小不能超過 5MB");
                        ViewBag.DebugMessage = "檔案太大";
                        return View(productdogweb);
                    }

                    string productName = productdogweb.Name;
                    string originalFileName = Path.GetFileNameWithoutExtension(PhotoPath.FileName);

                    // 清除檔案與資料夾非法字元
                    string cleanedProductName = string.Concat(productName.Split(Path.GetInvalidFileNameChars()));
                    string cleanedFileName = string.Concat(originalFileName.Split(Path.GetInvalidFileNameChars()));
                    string fileExtension = Path.GetExtension(PhotoPath.FileName);
                    string folderName = cleanedProductName;

                    // 建立圖片儲存資料夾
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    // 建立圖片檔名：產品名稱_日期.副檔名
                    string finalFileName = $"{cleanedProductName}__{DateTime.Now:yyyyMMdd}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolder, finalFileName);

                    // 寫入圖片檔案
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PhotoPath.CopyToAsync(stream);
                    }

                    // 將圖片相對路徑存入資料庫欄位
                    productdogweb.PhotoPath = $"/images/{finalFileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("PhotoPath", "檔案上傳失敗: " + ex.Message);
                    ViewBag.DebugMessage = "檔案上傳失敗: " + ex.Message;
                    return View(productdogweb);
                }
            }

            // 儲存產品資料至資料庫
            try
            {
                _context.Add(productdogweb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.DebugMessage = "資料庫儲存失敗: " + ex.Message;
                return View(productdogweb);
            }
        }

        // 顯示編輯產品的表單
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var productdogweb = await _context.Productsbydogweb.FindAsync(id);
            if (productdogweb == null) return NotFound();

            return View(productdogweb);
        }

        // 接收編輯表單送出（含圖片更新）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,PhotoPath")] Productdogweb productdogweb, IFormFile Photopath)
        {
            if (id != productdogweb.Id) return NotFound();

            var existingProduct = await _context.Productsbydogweb.FindAsync(id);
            if (existingProduct == null) return NotFound();

            // 更新基本欄位
            existingProduct.Name = productdogweb.Name;
            existingProduct.Price = productdogweb.Price;

            if (ModelState.ContainsKey("PhotoPath")) ModelState.Remove("PhotoPath");

            if (ModelState.IsValid)
            {
                try
                {
                    // 若有新圖片上傳則處理
                    if (Photopath != null && Photopath.Length > 0)
                    {
                        if (Photopath.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Photopath", "檔案大小不能超過 5MB");
                            return View(productdogweb);
                        }

                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };
                        var fileExtension = Path.GetExtension(Photopath.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("Photopath", "只允許上傳 jpg, jpeg, png, gif, jfif 格式的檔案");
                            return View(productdogweb);
                        }

                        // 刪除舊圖檔
                        if (!string.IsNullOrEmpty(existingProduct.PhotoPath))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existingProduct.PhotoPath);
                            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                        }

                        // 儲存新圖
                        var fileName = Guid.NewGuid().ToString() + fileExtension;
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
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
                    if (!ProductdogwebExists(productdogweb.Id)) return NotFound();
                    else throw;
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

        // 顯示刪除確認頁面
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var productdogweb = await _context.Productsbydogweb.FirstOrDefaultAsync(m => m.Id == id);
            if (productdogweb == null) return NotFound();

            return View(productdogweb);
        }

        // 接收刪除確認並執行刪除（含圖片）
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productdogweb = await _context.Productsbydogweb.FindAsync(id);
            if (productdogweb != null)
            {
                // 刪除圖片檔案
                if (!string.IsNullOrEmpty(productdogweb.PhotoPath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + productdogweb.PhotoPath);
                    if (System.IO.File.Exists(filePath))
                    {
                        try { System.IO.File.Delete(filePath); } catch { }
                    }
                }

                _context.Productsbydogweb.Remove(productdogweb);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 檢查指定 ID 的產品是否存在
        private bool ProductdogwebExists(int id)
        {
            return _context.Productsbydogweb.Any(e => e.Id == id);
        }
    }
}
