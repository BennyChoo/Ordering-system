using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class ProductDetailView
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string ProductCategory { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string? ProductDescription { get; set;}

        [Required]
        [Display(Name = "Availability")]
        public bool IsAvailableForSale { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal ProductPrice { get; set; }

        public string ProtectedProductId { get; set; }
    }
}
