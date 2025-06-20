using System.ComponentModel.DataAnnotations;

namespace dogwebMVC.Models
{
    public class Productdogweb
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "名稱")]
        public string Name { get; set; }

        [Display(Name = "價格")]
        public decimal Price { get; set; }

        [Display(Name = "照片路徑")]
        public string PhotoPath { get; set; }  // 新增照片路徑欄位
        
      
    }
}
