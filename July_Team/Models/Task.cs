using System.ComponentModel.DataAnnotations;

namespace July_Team.Models
{
    public enum TaskStatus { Pending, Done } 

    public class Task
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending; 

       
        public string OwnerId { get; set; }
       
    }
}