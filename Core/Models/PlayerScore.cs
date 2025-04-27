using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class PlayerScore
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string PlayerName { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}