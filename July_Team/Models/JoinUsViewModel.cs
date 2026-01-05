using System.ComponentModel.DataAnnotations;

namespace July_Team.Models
{
    public class JoinUsViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Field { get; set; }

        [Required]
        public string Experience { get; set; }

        public string Portfolio { get; set; }

        [Required]
        public string Motivation { get; set; }

        [Required]
        public string Contribution { get; set; }
    }
}