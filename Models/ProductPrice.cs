namespace QFD.Models
{
    public class ProductPrice
    {
        public int ProductPriceId { get; set; }
        public int ProductId { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Discount { get; set; }
        public DateTimeOffset? ValidFromDate { get; set; }
        public DateTimeOffset? ValidToDate { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTimeOffset? DeletedDate { get; set; }
        public Guid? DeletedBy { get; set; }

    }
}
