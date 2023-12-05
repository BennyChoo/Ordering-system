using System;
using System.Collections.Generic;

namespace QFD.Models
{
    public partial class EntityType
    {
        public EntityType()
        {
            AttachmentEntityMap = new HashSet<AttachmentEntityMap>();
        }

        public int EntityTypeId { get; set; }
        public string EntityDescription { get; set; }

        public virtual ICollection<AttachmentEntityMap> AttachmentEntityMap { get; set; }
    }
}
