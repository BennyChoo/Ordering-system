using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class TableQrCode
    {
        [Key]
        public int QrId { get; set; }
        public Guid QrCode { get; set; }
        public int TableId { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? DeletedDate { get; set; }
    }
}
