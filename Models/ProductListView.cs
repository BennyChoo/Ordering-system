using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class ProductListView
    {
        [Key]
        public int productId { get; set; }
        public string productName { get; set; }
        public string productCategory { get; set; }
        public string? productDescription { get; set;}
        public decimal productPrice { get; set; }
        public bool isAvailableForSale { get; set; }
        public string protectedProductId { get; set; }
    }
}
