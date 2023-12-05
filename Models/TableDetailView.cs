using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class TableDetailView
    {
        [Key]
        public int TableId { get; set; }

        [Required]
        [Display(Name = "Table Number")]
        public int TableNumber { get; set; }

        [Required]
        [Display(Name = "Table Location")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Table Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Total Person")]
        public int TotalPerson { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        public string ProtectedTableId { get; set; }
    }
}
