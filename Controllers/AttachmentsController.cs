using BugTrackingSystem.Filters;
using BugTrackingSystem.Helpers;
using BugTrackingSystem.Interfaces;
using BugTrackingSystem.Services;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BugTrackingSystem.Controllers
{
    [JwtAuthorize]
    public class AttachmentsController : ApiController
    {
        private readonly IAttachmentsService _attachmentsService;
        private const string UploadsFolder = "~/Uploads/Attachments";

        public AttachmentsController(IAttachmentsService attachmentsService)
        {
            _attachmentsService = attachmentsService;
        }

        [RequirePermission("UploadAttachment")]
        [HttpPost]
        [Route("api/bugs/{bugId:int}/attachments")]
        public async Task<IHttpActionResult> Upload(int bugId)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("Request must be multipart/form-data.");

            var uploadsPath = HttpContext.Current.Server.MapPath(UploadsFolder);

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var provider = new MultipartFormDataStreamProvider(uploadsPath);
            await Request.Content.ReadAsMultipartAsync(provider);

            if (provider.FileData.Count == 0)
                return BadRequest("No file was uploaded.");

            var files = provider.FileData
                .Select(f => new UploadFileInfo
                {
                    OriginalFileName = f.Headers.ContentDisposition.FileName.Trim('"'),
                    TempFilePath = f.LocalFileName
                })
                .ToList();

            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _attachmentsService.Upload(bugId, files, uploadsPath, currentUserId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }

        [RequirePermission("ViewAttachments")]
        [HttpGet]
        [Route("api/bugs/{bugId:int}/attachments")]
        public IHttpActionResult GetByBug(int bugId)
        {
            var result = _attachmentsService.GetByBug(bugId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(result.Data); // List<AttachmentDto>
        }

        [RequirePermission("ViewAttachments")]
        [HttpGet]
        [Route("api/attachments/{id:int}/download")]
        public IHttpActionResult Download(int id)
        {
            var result = _attachmentsService.GetForDownload(id);

            if (!result.Success)
                return NotFound();

            var fileResult = (AttachmentFileResult)result.Data;
            var stream = new FileStream(fileResult.PhysicalPath, FileMode.Open, FileAccess.Read);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileResult.FileName
            };

            return ResponseMessage(response);
        }

        [RequirePermission("DeleteAttachment")]
        [HttpDelete]
        [Route("api/attachments/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var currentUserId = JwtHelper.GetCurrentUserId(this);
            var result = _attachmentsService.Delete(id, currentUserId);

            if (!result.Success)
            {
                if (result.Status == ServiceStatus.NotFound) return NotFound();
                return BadRequest(result.Message);
            }

            return Ok(new { result.Message });
        }
    }
}
