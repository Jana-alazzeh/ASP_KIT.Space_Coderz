using System.ComponentModel.DataAnnotations;

namespace July_Team.Models
{
    public class RegesterModel
    {
        [Required(ErrorMessage = "الاسم مطلوب.")]
        [Display(Name = "اسم المستخدم")]
        public string User_Name { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string User_Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
    
        [Compare("Password", ErrorMessage = "كلمتا المرور غير متطابقتين.")]
        public string Confirm_Password { get; set; }
    }
}
