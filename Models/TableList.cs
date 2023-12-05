using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class TableList
    {
        [Key]
        public int TableId { get; set; }

        public int TableNumber { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public int TotalPerson { get; set; }
        public bool IsActive { get; set; }
    }
}
