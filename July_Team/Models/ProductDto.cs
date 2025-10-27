using System.ComponentModel.DataAnnotations;

namespace July_Team.Models
{

    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم المنتج لا يزيد عن 100 حرف")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "الوصف طويل جدًا")]
        public string? Description { get; set; }

        [Range(1, 9999, ErrorMessage = "السعر يجب أن يكون بين 1 و9999")]
        public decimal Price { get; set; }

        [Range(0, 1000, ErrorMessage = "المخزون يجب أن يكون رقمًا موجبًا")]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }
        public string? ImageUrl_Back { get; set; }
    }
}

