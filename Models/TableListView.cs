using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class TableListView
    {
        [Key]
        public int TableId { get; set; }

        [Display(Name = "Table Number")]
        public int TableNumber { get; set; }

        [Display(Name = "Table Location")]
        public string Location { get; set; }

        [Display(Name = "Table Description")]
        public string Description { get; set; }

        [Display(Name = "Total Person")]
        public int TotalPerson { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public string ProtectedTableId { get; set; }
    }
}
