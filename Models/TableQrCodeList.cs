using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class TableQrCodeList
    {
        [Key]
        public int QrId { get; set; }
        public Guid QrCode { get; set; }
        public int TableNumber { get; set; }
        public int TableId { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
    }
}
