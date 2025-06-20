// 已加入完整的註解，幫助理解每行的功能

using System; // 基本系統命名空間
using System.Collections.Generic; // 用於集合類型
using System.Linq; // 提供 LINQ 查詢功能
using System.Threading.Tasks; // 提供非同步支援
using System.IO; // 處理檔案與目錄
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC 核心命名空間
using Microsoft.AspNetCore.Mvc.Rendering; // 用於下拉選單等 HTML UI 元件
using Microsoft.EntityFrameworkCore; // Entity Framework Core 功能
using dogwebMVC.Models; // 專案的資料模型命名空間

namespace dogwebMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context; // 用來操作資料庫的資料上下文

        public ProductsController(AppDbContext context)
        {
            _context = context; // 建構函式注入 DbContext
        }

        // 顯示所有產品資料
        public async Task<IActionResult> Index()
        {
            return View(await _context.Productsbydogweb.ToListAsync()); // 非同步讀取產品資料並傳送到 View
        }

        // 顯示單一產品詳細資料
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound(); // 若無 id 傳入，回傳 404

            var productdogweb = await _context.Productsbydogweb.FirstOrDefaultAsync(m => m.Id == id); // 查詢資料
            if (productdogweb == null) return NotFound(); // 若找不到資料，也回傳 404

            return View(productdogweb); // 傳送資料至 View
        }

        // 顯示新增產品的表單
        public IActionResult Create()
        {
            return View(); // 回傳空白表單頁面
        }

        // 處理表單送出的新增資料
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price")] Productdogweb productdogweb, IFormFile PhotoPath)
        {
            // 移除因 IFormFile 而產生的驗證錯誤
            if (ModelState.ContainsKey("PhotoPath")) ModelState.Remove("PhotoPath");

            // 除錯輸出
            System.Diagnostics.Debug.WriteLine("=== CREATE DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"Name: {productdogweb.Name}");
            System.Diagnostics.Debug.WriteLine($"Price: {productdogweb.Price}");
            System.Diagnostics.Debug.WriteLine($"PhotoPath is null: {PhotoPath == null}");

            // 顯示檔案資訊（如果有）
            if (PhotoPath != null)
            {
                System.Diagnostics.Debug.WriteLine($"File name: {PhotoPath.FileName}");
                System.Diagnostics.Debug.WriteLine($"File size: {PhotoPath.Length} bytes");
            }

            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // 驗證失敗時顯示錯誤
            if (!ModelState.IsValid)
            {
                ViewBag.DebugMessage = "ModelState 驗證失敗: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(productdogweb);
            }

            if (PhotoPath != null && PhotoPath.Length > 0)
            {
                try
                {
                    // 限制檔案大小在 5MB 以下
                    if (PhotoPath.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("PhotoPath", "檔案大小不能超過 5MB");
                        ViewBag.DebugMessage = "檔案太大";
                        return View(productdogweb);
                    }
                    string productName = productdogweb.Name; // 從表單取得的產品名稱
                    string originalFileName = Path.GetFileNameWithoutExtension(PhotoPath.FileName); // 使用者上傳的原始檔案名
                    string cleanedProductName = string.Concat(productName.Split(Path.GetInvalidFileNameChars()));// 移除產品名稱中的非法字元
                    string cleanedFileName = string.Concat(originalFileName.Split(Path.GetInvalidFileNameChars()));// 移除產品名稱和檔案名中的非法字元
                    string fileExtension = Path.GetExtension(PhotoPath.FileName);// 取得副檔名
                    string folderName = string.Concat(productdogweb.Name.Split(Path.GetInvalidFileNameChars()));

                    // 建立產品名稱的資料夾路徑        // 擷取產品名稱並移除非法字元

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    // 限制允許的副檔名
                    // var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };


                    //.ToLowerInvariant(); // 取得副檔名並轉為小寫
                    // if (!allowedExtensions.Contains(fileExtension))
                    // {
                    //     ModelState.AddModelError("PhotoPath", "只允許上傳圖片檔案");
                    //     ViewBag.DebugMessage = $"不支援的檔案類型: {fileExtension}";
                    //     return View(productdogweb);
                    // }



                    // 建立唯一檔名並儲存
                    // var fileName = Guid.NewGuid().ToString() + fileExtension;
                    // var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    // var fileName = $"{originalFileName}_{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";// 組合檔名，包含原始名稱和時間戳
                    string finalFileName = $"{cleanedProductName}__{DateTime.Now:yyyyMMdd}{fileExtension}";

                    // if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);// 確保上傳目錄存在

                    // 組合完整儲存路徑
                    //var filePath = Path.Combine(uploadsFolder, fileName);
                    var filePath = Path.Combine(uploadsFolder, finalFileName);// 組合完整儲存路徑



                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PhotoPath.CopyToAsync(stream);  //將檔案寫入其他
                    }

                    // productdogweb.PhotoPath = $"/images/{folderName}"; // 儲存路徑到資料庫欄位
                    productdogweb.PhotoPath = $"/images/{finalFileName}"; // 儲存路徑到資料庫欄位
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("PhotoPath", "檔案上傳失敗: " + ex.Message);
                    ViewBag.DebugMessage = "檔案上傳失敗: " + ex.Message;
                    return View(productdogweb);
                }
            }

            // 儲存到資料庫
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

        // 顯示編輯表單
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var productdogweb = await _context.Productsbydogweb.FindAsync(id);
            if (productdogweb == null) return NotFound();

            return View(productdogweb);
        }

        // 處理編輯表單送出
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,PhotoPath")] Productdogweb productdogweb, IFormFile Photopath)
        {
            if (id != productdogweb.Id) return NotFound();

            var existingProduct = await _context.Productsbydogweb.FindAsync(id); // 取得現有資料
            if (existingProduct == null) return NotFound();

            // 更新欄位
            existingProduct.Name = productdogweb.Name;
            existingProduct.Price = productdogweb.Price;

            if (ModelState.ContainsKey("PhotoPath")) ModelState.Remove("PhotoPath");

            if (ModelState.IsValid)
            {
                try
                {
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

                        if (!string.IsNullOrEmpty(existingProduct.PhotoPath))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + existingProduct.PhotoPath);
                            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                        }

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

        // 確認刪除動作
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productdogweb = await _context.Productsbydogweb.FindAsync(id);
            if (productdogweb != null)
            {
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

        // 檢查產品是否存在
        private bool ProductdogwebExists(int id)
        {
            return _context.Productsbydogweb.Any(e => e.Id == id);
        }
    }
}
