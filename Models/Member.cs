using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace dogwebMVC.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "名稱")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "密碼")]

        [DataType(DataType.Password)]
        //密碼長度要在1~10個字中間
        
        
        public string Password { get; set; }
        [Required]
        [Display(Name = "郵件")]
  

        public string Email { get; set; }
    }
}
