using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zakupowo.Models
{
    [Table("order_items")]
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal ItemPrice { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}