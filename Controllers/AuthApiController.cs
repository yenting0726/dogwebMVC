using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using dogwebMVC.Models;
using System.Linq;


namespace dogwebMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthApiController(AppDbContext context)
        {
            _context = context;
        }

        // 註冊
        [HttpPost("register")]
        public IActionResult Register([FromForm] Member request)
        {

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
             .SelectMany(v => v.Errors)
             .Select(e => e.ErrorMessage)
             .ToList();


                return BadRequest("註冊失敗：" + string.Join("；", errorMessages));
            }

            if (_context.Members.Any(m => m.Username == request.Username))
                return BadRequest("帳號已存在");
            //如果沒有輸入值，顯使請輸入資料
            // if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            // {
            //     return BadRequest("請輸入帳號、密碼與電子郵件");
            // }


            var member = new Member
            {
                Username = request.Username,
                Password = request.Password,
                Email = request.Email
            };
            _context.Members.Add(member);
            _context.SaveChanges();

            return Ok("註冊成功");




        }

        // 登入
        [HttpPost("login")]

        public IActionResult Login()
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("請輸入帳號與密碼");
            }

            var member = _context.Members.FirstOrDefault(m => m.Username == username);
            if (member == null)
            {
                return BadRequest("無此帳號");
            }

            if (member.Password != password)
            {
                return Unauthorized("密碼錯誤");
            }

            HttpContext.Session.SetString("user", username);
            return Ok(new { message = "登入成功", username });
        }




        //     public IActionResult Login([FromForm] string username, [FromForm] string password)
        //     {


        //         var member = _context.Members.FirstOrDefault(m => m.Username == username );
        //         if (member == null)
        //         {
        //             return BadRequest("無此帳號"); // ✅ 正確：先判斷帳號是否存在
        //         }
        //         if (member.Password != password)
        // {
        //     return Unauthorized("密碼錯誤"); // ✅ 密碼錯誤 
        // }

        //         HttpContext.Session.SetString("user", username);
        //         return Ok(new { message = "登入成功", username });
        //     }

        // 檢查登入狀態
        [HttpGet("check")]
        public IActionResult CheckLogin()
        {
            var user = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(user))
                return Ok("未登入");

            return Ok($"歡迎, {user}");
        }

        // 登出
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return Ok("已登出");
        }
    }
}
