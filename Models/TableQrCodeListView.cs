using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class TableQrCodeListView
    {
        [Key]
        public int qrId { get; set; }
        public Guid qrCode { get; set; }
        public int tableNumber { get; set; }
        public int tableId { get; set; }
        public string expiryDate { get; set; }
        public string protectedQrId { get; set; }
    }
}
