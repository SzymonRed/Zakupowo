using System.ComponentModel.DataAnnotations;

namespace Zakupowo.Models
{
    public class VatRate
    {
        [Key]
        public int VatRateId { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Rate { get; set; }
    }
}