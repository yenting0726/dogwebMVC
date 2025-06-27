
using dogwebMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


// using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);

// 加入 Session 記憶體快取
builder.Services.AddDistributedMemoryCache();

// 加入 Session 支援
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); // 設定 Session 的過期時間
}); // <<== ← 這個分號你原本漏掉了
builder.Services.AddControllersWithViews();       // 將 MVC 控制器與視圖功能加入至服務容器中

builder.Services.AddRazorPages();             // <-- Razor Pages

// 加入資料庫 先註解 因為要發布
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 加入 MVC 控制器與視圖
builder.Services.AddControllersWithViews();

var app = builder.Build();
// 設定應用程式基底路徑，必須放在最前面
app.UsePathBase("/08");

// 錯誤處理 & HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ 啟用 Session（這行順序不能錯）
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
