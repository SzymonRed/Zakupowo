using System.Collections.Generic;
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
        public bool IsAdmin { get; set; }  

        [Required]
        public bool Newsletter { get; set; }  
        
        [Range(0,1)]
        public decimal? Discount { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
    }
}