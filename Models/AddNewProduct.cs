using QFD.Business;
using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class AddNewProduct
    {
        [Key]
        [Display(Name = "Name")]
        public string ProductName { get; set; }

        [Display(Name = "Category")]
        public int ProductCategoryId { get; set; }

        [Display(Name = "Description")]
        public string? ProductDescription { get; set;}

        [Display(Name = "Available For Sale")]
        public bool IsAvailableForSale { get; set; }

        //[DataType(DataType.Upload)]
        //[MaxFileSize(4 * 640 * 480)]
        //[AllowedExtensions(new string[] { ".jpg", ".png" })]
        //public IFormFile Image { get; set; }


        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal ProductCost { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? DeletedDate { get; set; }
    }
}
