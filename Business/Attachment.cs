using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Logging;


namespace QFD.Business
{
    public class Attachment
    {
        private readonly ILogger _logger;

        public Attachment(ILogger logger)
        {
            _logger = logger;
        }


        //public string CreateDirectoryName(string baseFilePath, int entityType)
        //{
        //    string entityTypeName = string.Empty;

        //    switch (entityType)
        //    {
        //        case (int)NoteFileAttachmentEntityTypeLookUp.Account:
        //            entityTypeName = "Account";
        //            break;
        //        case (int)NoteFileAttachmentEntityTypeLookUp.Contact:
        //            entityTypeName = "Contact";

        //            break;

        //        case (int)NoteFileAttachmentEntityTypeLookUp.Opportunity:
        //            entityTypeName = "Opportunity";

        //            break;
        //        case (int)NoteFileAttachmentEntityTypeLookUp.Quote:
        //            entityTypeName = "Quote";

        //            break;

        //        case (int)NoteFileAttachmentEntityTypeLookUp.Sale:
        //            entityTypeName = "Sale";

        //            break;

        //        default:
        //            entityTypeName = "Unknown";
        //            break;

        //    }

        //    return Path.Combine(baseFilePath,entityTypeName)
        //    return entityTypeName
        //}

        public string CreateFileName(string basePath, int entityType, int entityId)
        {
            string filePath = string.Empty;
            
            filePath = Path.Combine(basePath, "Product", entityId.ToString());

            return filePath;
        }


        public (bool OnError, bool FileSaved) SaveFile(IFormFile file, string path, string fileName)
        {
            bool fileSaved = false, onError = false;
            var fileNamePath = string.Empty;
            try
            {
                fileNamePath = Path.Combine(path, fileName);
                using (var fs = File.Create(fileNamePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                    fs.Close();
                }

                fileSaved = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Saving of file failed. File path : {0} . Error Message : {1} Stacktrace :{2}",
                    fileNamePath, ex.Message, ex.StackTrace);
                onError = true;
            }

            return (onError, fileSaved);
        }

        public (bool OnError,bool FileExists) IsFileExists(string path, string fileName)

        {
            bool onError = false, fileExists = false;
            try
            {
                var fileNamePath = Path.Combine(path, fileName);

                if (File.Exists(fileNamePath))
                {
                    fileExists = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "File check  failed. Path : {0} File : {1} . Error Message : {2} Stacktrace :{3}",
                    path,fileName, ex.Message, ex.StackTrace);
                onError = true;
            }

            return (onError, fileExists);
        }

        public (bool OnError, bool DirectoryExists) IsDirectoryExists(string path, bool create)
        {
            bool success = false, onError = false;
            try
            {
                if (!Directory.Exists(path))
                {
                    if (create)
                    {
                        Directory.CreateDirectory(path);
                        success = true;
                    }
                }
                else
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Directory check or creation failed. Directory : {0} . Error Message : {1} Stacktrace :{2}",
                    path, ex.Message, ex.StackTrace);
                onError = true;
            }

            return (onError, success);
        }


        public ( bool OnError, string FileContent) ReadTextFile(string filePath)
        {
            bool onError = false;
            string data = string.Empty ;
            try
            {
                var file = System.IO.File.OpenText(filePath);

                data = file.ReadToEnd();

            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Failed to read text file content. Message {ex.Message}. Stack {ex.StackTrace} ");
            }

            return (onError, data);
        }
    }

   
}