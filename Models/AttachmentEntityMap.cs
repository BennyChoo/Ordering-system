using System;
using System.Collections.Generic;

namespace QFD.Models
{
    public partial class AttachmentEntityMap
    {
        public int AttachmentEntityMapId { get; set; }
        public int AttachmentId { get; set; }
        public int EntityId { get; set; }
        public int EntityTypeId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

        public virtual Attachment Attachment { get; set; }
        public virtual EntityType EntityType { get; set; }
    }
}
