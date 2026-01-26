using System.ComponentModel.DataAnnotations;

namespace July_Team.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string TrainerName { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImageUrl { get; set; }

        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "الرجاء إدخال سعر صحيح")]
        public decimal Price { get; set; }


        
        [Required]
        [StringLength(50)]
        public string Duration { get; set; } 
    }
}
