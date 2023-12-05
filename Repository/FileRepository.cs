using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using QFD.Models;

namespace QFD.Repository
{
    public class FileRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;


        public FileRepository(ApplicationDbContext db, ILogger logger)
        {
            _dbContext = db;
            _logger = logger;
        }


        private string GetDisplayType(string type)
        {
            if (type.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                return "application/mssheet";
            if (type.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                return "application/msword";

            return type;
        }

        public (bool OnError, int ErrorCode, List<AttachmentSummary> AttachmentSummary) Get(int entityId,
            int entityTypeId)
        {
            var data = new List<AttachmentSummary>();
            var onError = false;
            var errorCode = 0;


            try
            {
                data = _dbContext.AttachmentSummary.FromSqlInterpolated($"GetAttachmentSummary {entityId}, {entityTypeId}")
                    .ToList();
                //data = (from a in _dbContext.AttachmentEntityMap
                //    join f in _dbContext.Attachment on a.AttachmentId equals f.AttachmentId
                //    where a.EntityId == entityId && a.EntityTypeId == entityTypeId
                //    select new AttachmentSummary
                //    {
                //        FileName = f.FileName,
                //        EntityId = a.EntityId,
                //        EntityTypeId = a.EntityTypeId,
                //        FileId = f.AttachmentId,
                //        FileGuid = f.FileGuid,
                //        FileSize = f.Size,
                //        Comment = f.Comment,
                //        DateCreated = a.CreatedDate,
                //        DoType = f.Type,
                //        ViewType = GetDisplayType(f.Type)
                //    }).ToList();
            }
            catch (Exception ex)
            {
                onError = true;
                errorCode = 18001;
                _logger.LogError(
                    $"Error code 18001. Failed to get attachment list for Entity Id  {entityId} Entity Type {entityTypeId}  Message {ex.Message} Stack {ex.StackTrace}");
            }

            return (onError, errorCode, data);
        }


        public (bool OnError, int ErrorCode, Attachment Attachment) GetFile(int entityId, string fileGuid,
            int entityTypeId)
        {
            var onError = false;
            var errorCode = 0;
            Attachment data = null;

            try
            {
                data = (from a in _dbContext.AttachmentEntityMap
                    join h in _dbContext.Attachment on a.AttachmentId equals h.AttachmentId
                    where a.EntityId == entityId && h.FileGuid.ToString() == fileGuid && a.EntityTypeId == entityTypeId
                    select h).SingleOrDefault();
            }
            catch (Exception ex)
            {
                onError = true;
                errorCode = 18002;
                _logger.LogError(
                    $"Error code 18002. Failed to get attachment details for Entity : {0} . File Guid {fileGuid} . Error {ex.Message} Stack {ex.StackTrace}");
            }

            return (onError, errorCode, data);
        }


        public void LinkEntityToAttachment(int entityId, int entityTypeId, int attachmentId, Guid userId)
        {
            var attachmentMap = new AttachmentEntityMap
            {
                EntityId = entityId,
                EntityTypeId = entityTypeId,
                AttachmentId = attachmentId,
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            };

            _dbContext.AttachmentEntityMap.Add(attachmentMap);
            _dbContext.SaveChanges();
        }


        public (bool OnError, int ErrorCode, int AttachmentId) Add(AddNewAttachmentView data, string filePath, string createdBy)
        {
            var onError = false;
            var errorCode = 0;
            int attachmentId = 0;

            try
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    var userId = new Guid(createdBy);
                    var guid = Guid.NewGuid();

                    //Note
                    var attachment = new Attachment
                    {
                        FileName = data.FileName,
                        Type = data.ContentType,
                        FileGuid = guid,
                        FilePath = filePath,
                        Size = data.FileSize,
                        Comment = data.Comment,
                        AttachmentTypeId = data.AttachmentTypeId
                    };

                    _dbContext.Attachment.Add(attachment);
                    _dbContext.SaveChanges();
                    //transaction.Commit();
                    attachmentId = attachment.AttachmentId;
                    // link this to account

                    var attachmentMap = new AttachmentEntityMap
                    {
                        EntityId = data.EntityId,
                        EntityTypeId = data.EntityTypeId,
                        AttachmentId = attachment.AttachmentId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.AttachmentEntityMap.Add(attachmentMap);
                    _dbContext.SaveChanges();


                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                onError = true;
                errorCode = 18003;
                _logger.LogError($"Error code 18003. Failed to save attachment for Entity : {data.EntityId} . File Name {data.FileName} . Message {ex.Message} Stack {ex.StackTrace}");
            }

            return (onError, errorCode, attachmentId);
        }


        public int GetAttachmentType(string attachmentType)
        {
          
                return _dbContext.AttachmentType.SingleOrDefault(p => p.TypeDescription != null && p.TypeDescription == attachmentType)
                    .AttachmentTypeId;
        }

    }
}