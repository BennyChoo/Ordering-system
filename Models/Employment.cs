using System;
using System.Collections.Generic;

namespace QFD.Models
{
    public partial class Employment
    {
        public Employment()
        {
            
        }

        public int EmploymentId { get; set; }
        public int PersonId { get; set; }
        public DateTime? ProposedStartDate { get; set; }
        public int EmploymentTypeId { get; set; }
        public int EmploymentStatusTypeId { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

       
    }
}
