using System.ComponentModel.DataAnnotations.Schema;

namespace July_Team.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public string Status { get; set; } = "Pending"; 
    }
}