using System;
using System.ComponentModel.DataAnnotations;

namespace Zakupowo.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }

        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        [Required]
        public decimal ExchangeRate { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }
    }
}