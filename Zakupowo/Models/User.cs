using System.ComponentModel.DataAnnotations;

namespace Zakupowo.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        public bool IsAdmin { get; set; }  // 0 = false, 1 = true

        [Required]
        public bool Newsletter { get; set; }  // 0 = false, 1 = true
    }
}