using System;
using System.Collections.Generic;

namespace QFD.Models
{
    public partial class Attachment
    {
        public Attachment()
        {
            AttachmentEntityMap = new HashSet<AttachmentEntityMap>();
        }
        public int AttachmentId { get; set; }
        public Guid FileGuid { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public string Comment { get; set; }
        public int? AttachmentTypeId { get; set; }
        public virtual ICollection<AttachmentEntityMap> AttachmentEntityMap { get; set; }
    }
}
