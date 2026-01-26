using System.ComponentModel.DataAnnotations;

namespace July_Team.Models
{
    public class ContactUsViewModel
    {
        [Key]
        public int Id { get; set; }

        
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string Name { get; set; }

        
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress]
        public string Email { get; set; }

        
        [Required(ErrorMessage = "الموضوع مطلوب")]
        public string Subject { get; set; }

        
        [Required(ErrorMessage = "نص الرسالة مطلوب")]
        public string Message { get; set; }
    }
}
