using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // لإضافة [NotMapped]

namespace July_Team.Models
{
    // يمكن تزيين الـ Enum بـ Display Name إذا أردت عرض أسماء مختلفة في الواجهة
    public enum TaskStatus
    {
        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Completed")]
        Done
    }

    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Task title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        [Display(Name = "Task Title")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        [DataType(DataType.MultilineText)] // يخبر الواجهة أن هذا حقل متعدد الأسطر
        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)] // يضمن أن يتم التعامل معه كتاريخ فقط (بدون وقت)
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        // إضافة خاصية لتاريخ إنشاء المهمة، مفيدة جداً للفرز والتحليل
        [Required]
        [Display(Name = "Creation Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key للمستخدم
        [Required]
        public string OwnerId { get; set; }

        // ========== خصائص محسوبة (غير مخزنة في قاعدة البيانات) ==========
        // هذه الخصائص مفيدة جداً في الواجهة الأمامية (View)

        [NotMapped] // هذا يعني أن Entity Framework لن يحاول إنشاء عمود لها في الجدول
        public bool IsOverdue => Status == TaskStatus.Pending && DueDate < DateTime.Today;

        [NotMapped]
        public string TimeLeft
        {
            get
            {
                if (Status == TaskStatus.Done) return "Completed";
                if (IsOverdue) return "Overdue";

                var timeLeft = DueDate.Date - DateTime.Today;
                if (timeLeft.Days == 0) return "Today";
                if (timeLeft.Days == 1) return "Tomorrow";
                if (timeLeft.Days < 7) return $"{timeLeft.Days} days left";

                return DueDate.ToString("MMM dd"); // إذا كان التاريخ بعيداً، اعرضه
            }
        }
    }
}
