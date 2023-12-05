using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QFD.Models
{
    public class AttachmentSummary
    {

     
            public string FileName { get; set; }
            public int EntityId { get; set; }
            public int EntityTypeId { get; set; }

            public int FileId { get; set; }
            public Guid FileGuid { get; set; }
            public long FileSize { get; set; }
            public string Comment { get; set; }
            public DateTime DateCreated { get; set; }
            public String CreatedBy { get; set; }
            public String DocumentType { get; set; }
            public String ViewType { get; set; }



        
    }
}
