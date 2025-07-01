
using dogwebMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


// using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);
// 加入 MVC 控制器與視圖
builder.Services.AddControllersWithViews();// 將 MVC 控制器與視圖功能加入至服務容器中
// 加入 Session 支援
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); // 設定 Session 的過期時間
}); // <<== ← 這個分號你原本漏掉了

// 加入 Session 記憶體快取
builder.Services.AddDistributedMemoryCache();



   // CORS 設定 - 本地開發 + 生產環境 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddRazorPages();             // <-- Razor Pages

// 加入資料庫 先註解 因為要發布
//  builder.Services.AddDbContext<AppDbContext>(options =>
//      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 資料庫設定 - 本地 SQL Server 版本
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // 本地資料庫連線設定
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
        
        // 設定命令逾時時間
        sqlOptions.CommandTimeout(30);
    });
    
    // 根據環境設定不同的選項
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
    else
    {
        options.EnableServiceProviderCaching();
    }
});

var app = builder.Build();
// 設定應用程式基底路徑，必須放在最前面
app.UsePathBase("/08");

// 錯誤處理 & HTTPS
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // 開啟詳細錯誤頁
    //app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
 app.MapRazorPages();   

app.Run();
