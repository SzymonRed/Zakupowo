using System;
using System.ComponentModel.DataAnnotations;

namespace Zakupowo.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }

        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; }

        [Required]
        public decimal ExchangeRate { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }
    }
}