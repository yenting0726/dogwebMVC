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

        // 註冊更改寫法 不使用[FromFrom]]
        [HttpPost("register")]
        public IActionResult Regsister()
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string confirmPassword = Request.Form["confirmPassword"];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return BadRequest("請輸入帳號、密碼與確認密碼");
            }

            if (password != confirmPassword)
            {
                return BadRequest("密碼與確認密碼不一致");
            }

            if (_context.Members.Any(m => m.Username == username))
            {
                return BadRequest("帳號已存在");
            }

            var newMember = new Member
            {
                Username = username,
                Password = password
            };

            _context.Members.Add(newMember);
            _context.SaveChanges();

            return Ok(new { message = "註冊成功", username });
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
