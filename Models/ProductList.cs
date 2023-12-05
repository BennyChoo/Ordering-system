using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class ProductList
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public string? ProductDescription { get; set;}
        public decimal ProductPrice { get; set; }
        public bool IsAvailableForSale { get; set; }
    }
}
