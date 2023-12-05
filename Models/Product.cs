using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        public string? ProductDescription { get; set;}
        public bool IsAvailableForSale { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? DeletedDate { get; set; }
    }
}
