using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace dogwebMVC.Models
{
    public class Member
    {
        public int Id { get; set; }

        
        [Display(Name = "名稱")]
        [Required(ErrorMessage = "請輸入帳號")]
        public string Username { get; set; }

        
        [Display(Name = "密碼")]
         [Required(ErrorMessage = "請輸入密碼")]

        [DataType(DataType.Password)]
        //密碼長度要在1~10個字中間
        
        
        public string Password { get; set; }
        
        [Display(Name = "郵件")]
  [Required(ErrorMessage = "請輸入電子郵件")]

        public string Email { get; set; }
    }
}
