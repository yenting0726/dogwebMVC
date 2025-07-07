using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dogwebMVC.Models;

namespace dogwebMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public void savelog(string data)
    {
        //取地目前時間
        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //構建日誌內容

        //將日誌寫入csv 檔案

        //使用File.AppendAllText 方法將日誌內容追加到檔案中
         
         //這樣九可以將日誌 寫入到logs.csv 檔案

}




    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Googlemap()
    {
        return View();
    }

public IActionResult shopping()
    {
        return View();
    }
public IActionResult member()
    {
        return View();
    }
public IActionResult product()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
