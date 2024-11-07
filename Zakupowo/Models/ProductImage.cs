using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zakupowo.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        [MaxLength(255)]
        public string SmallImage { get; set; }

        [Required]
        [MaxLength(255)]
        public string LargeImage { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Currency Currency { get; set; }
    }
}