
using dogwebMVC.Models;
using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.FileProviders;



var builder = WebApplication.CreateBuilder(args);

// 資料庫設定
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// MVC 註冊
builder.Services.AddControllersWithViews();

var app = builder.Build();


/// 非開發模式下啟用例外處理.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(
//         Path.Combine(Directory.GetCurrentDirectory(), "ClientApp/dist")),
//     RequestPath = ""
// });


// HTTPS、靜態檔案、路由、授權
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
// Vue SPA fallback（這行是關鍵）
// app.MapFallbackToFile("index.html");

app.Run();
