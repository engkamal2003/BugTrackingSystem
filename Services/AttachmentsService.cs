using BugTrackingSystem.Data;
using BugTrackingSystem.DTOs.Responses;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BugTrackingSystem.Services
{
    public class AttachmentsService : IAttachmentsService
    {
        private readonly BugTrackingDbContext _context;
        private const string UploadsVirtualPath = "~/Uploads/Attachments/";

        public AttachmentsService(BugTrackingDbContext context)
        {
            _context = context;
        }

        public ServiceResult Upload(int bugId, IList<UploadFileInfo> files, string uploadsPath, int currentUserId)
        {
            var bug = _context.Bugs.FirstOrDefault(b => b.Id == bugId && !b.IsDeleted);
            if (bug == null)
                return ServiceResult.Fail("Bug not found.", ServiceStatus.NotFound);

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.OriginalFileName);
                var uniqueFileName = Guid.NewGuid().ToString("N") + extension;
                var finalPath = Path.Combine(uploadsPath, uniqueFileName);

                File.Move(file.TempFilePath, finalPath);

                _context.Attachments.Add(new Attachment
                {
                    BugId = bugId,
                    FileName = file.OriginalFileName,
                    FilePath = UploadsVirtualPath + uniqueFileName,
                    UploadedBy = currentUserId,
                    UploadedAt = DateTime.Now,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.Now
                });
            }

            _context.SaveChanges();

            return ServiceResult.Ok(message: "File uploaded successfully.");
        }

        public ServiceResult GetByBug(int bugId)
        {
            if (!_context.Bugs.Any(b => b.Id == bugId && !b.IsDeleted))
                return ServiceResult.Fail("Bug not found.", ServiceStatus.NotFound);

            var attachments = _context.Attachments
                .Where(a => a.BugId == bugId && !a.IsDeleted)
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    BugId = a.BugId,
                    FileName = a.FileName,
                    UploadedByName = a.UploadedByUser.FullName,
                    UploadedAt = a.UploadedAt
                })
                .ToList();

            return ServiceResult.Ok(attachments);
        }

        public ServiceResult GetForDownload(int id)
        {
            var attachment = _context.Attachments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (attachment == null)
                return ServiceResult.Fail("Attachment not found.", ServiceStatus.NotFound);

            var physicalPath = HttpContext.Current.Server.MapPath(attachment.FilePath);
            if (!File.Exists(physicalPath))
                return ServiceResult.Fail("File not found on server.", ServiceStatus.NotFound);

            return ServiceResult.Ok(new AttachmentFileResult
            {
                PhysicalPath = physicalPath,
                FileName = attachment.FileName
            });
        }

        public ServiceResult Delete(int id, int currentUserId)
        {
            var attachment = _context.Attachments.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (attachment == null)
                return ServiceResult.Fail("Attachment not found.", ServiceStatus.NotFound);

            attachment.IsDeleted = true;
            attachment.DeletedBy = currentUserId;
            attachment.DeletedAt = DateTime.Now;

            _context.SaveChanges();

            return ServiceResult.Ok(message: "Attachment deleted successfully.");
        }
    }
}
