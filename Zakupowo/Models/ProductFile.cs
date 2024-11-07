using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zakupowo.Models
{
    public class ProductFile
    {
        [Key]
        public int FileId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FilePath { get; set; }

        [MaxLength(255)]
        public string FileDescription { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Currency Currency { get; set; }
    }
}