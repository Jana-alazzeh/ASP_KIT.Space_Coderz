using System.ComponentModel.DataAnnotations;

namespace July_Team.Models
{
    public class ProductDetailViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUrl_Back { get; set; } 
        public int AvailableStock { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "الكمية يجب أن تكون بين 1 و 100")]
        public int SelectedQuantity { get; set; } = 1;

        public string SelectedSize { get; set; }
    }
}
