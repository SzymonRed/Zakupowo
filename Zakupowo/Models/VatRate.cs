using System.ComponentModel.DataAnnotations;

namespace Zakupowo.Models
{
    public class VatRate
    {
        [Key]
        public int VatRateId { get; set; }
        
        public decimal? Rate { get; set; }
    }
}