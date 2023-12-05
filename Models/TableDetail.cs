using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class TableDetail
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
        public bool IsActive { get; set; }
    }
}
