using BugTrackingSystem.Services;
using System.Collections.Generic;

namespace BugTrackingSystem.Interfaces
{
    public class UploadFileInfo
    {
        public string OriginalFileName { get; set; }
        public string TempFilePath { get; set; }
    }

    public class AttachmentFileResult
    {
        public string PhysicalPath { get; set; }
        public string FileName { get; set; }
    }

    public interface IAttachmentsService
    {
        ServiceResult Upload(int bugId, IList<UploadFileInfo> files, string uploadsPath, int currentUserId);
        ServiceResult GetByBug(int bugId);
        ServiceResult GetForDownload(int id);
        ServiceResult Delete(int id, int currentUserId);
    }
}
