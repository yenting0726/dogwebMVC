using System.ComponentModel.DataAnnotations;

namespace dogwebMVC.Models
{
    public class Productdogweb
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}