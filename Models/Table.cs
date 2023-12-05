using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class Table
    {
        [Key]
        public int TableId { get; set; }

        [Required]
        [Display(Name = "Table no.")]
        public int TableNumber { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Table description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Number of people per table")]
        public int TotalPerson { get; set; }
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? DeletedDate { get; set; }
    }
}
