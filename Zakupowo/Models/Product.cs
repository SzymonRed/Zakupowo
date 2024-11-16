﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zakupowo.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Zmieniona nazwa na VatRateValue, aby uniknąć konfliktu
       // public decimal VatRateValue { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public bool IsHidden { get; set; }
        
        [Required]
        public string Image { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        // [ForeignKey("Currency")]

        //public int CurrencyId { get; set; }


       [ForeignKey("VatRate")]
        public int VatRateId { get; set; }

        public virtual Category Category { get; set; }
        //public virtual Currency Currency { get; set; }
        public virtual VatRate VatRate { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}