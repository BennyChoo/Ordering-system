using System;


namespace QFD.Models
{
    public class AddNewAttachmentView
    {

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int EntityId { get; set; }
        public int EntityTypeId { get; set; }
        public int FileId { get; set; }
        public Guid FileGuid { get; set; }
        public long FileSize { get; set; }
        public string Comment { get; set; }
        public int AttachmentTypeId { get; set; }
    }
}
