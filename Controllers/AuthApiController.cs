using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using dogwebMVC.Models;
using System;
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
            _context = context ?? throw new ArgumentNullException(nameof(context), "資料庫尚未設定");
        }

        // 註冊
        [HttpPost("register")]
        public IActionResult Register()
        {
            try
            {
                string username = Request.Form["username"].FirstOrDefault() ?? string.Empty;
                string password = Request.Form["password"].FirstOrDefault() ?? string.Empty;
                string confirmPassword = Request.Form["confirmPassword"].FirstOrDefault() ?? string.Empty;

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
                    Password = password // 建議改成密碼雜湊儲存
                };

                _context.Members.Add(newMember);
                _context.SaveChanges();

                return Ok(new { message = "註冊成功", username });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "伺服器錯誤: " + ex.Message);
            }
        }

        // 登入
        [HttpPost("login")]
        public IActionResult Login()
        {
            try
            {
                string username = Request.Form["username"].FirstOrDefault() ?? string.Empty;
                string password = Request.Form["password"].FirstOrDefault() ?? string.Empty;

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
            catch (Exception ex)
            {
                return StatusCode(500, "伺服器錯誤: " + ex.Message);
            }
        }

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
