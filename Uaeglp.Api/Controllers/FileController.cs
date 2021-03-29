using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Uaeglp.Api.Extensions;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("get-profileImage/{profileId}", Name = "GetProfileImageAsync")]
        public async Task<IActionResult> GetProfileImageAsync(int profileId)
        {

            var result = await _fileService.GetProfileImageAsync(profileId);

            if (result.FileView != null)
            {
                ContentDisposition cd = new ContentDisposition
                {
                    FileName = result.FileView.Name,
                    Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(result.FileView.FileBytes, result.FileView.MimeType);
            }

            return Ok(result);

        }

        [HttpGet("get-assessmentImage/{assessmentId}", Name = "GetAssessmentImage")]
        public async Task<IActionResult> GetAssessmentImageAsync(int assessmentId)
        {

            var result = await _fileService.GetAssessmentImageAsync(assessmentId);

            if (result.FileView != null)
            {
                ContentDisposition cd = new ContentDisposition
                {
                    FileName = result.FileView.Name,
                    Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(result.FileView.FileBytes, result.FileView.MimeType);
            }

            return Ok(result);

        }

        [HttpGet("download-post-file/{fileId}", Name = "GetPostFileAsync")]
        public async Task<IActionResult> GetPostUploadedFileAsync(string fileId)
        {

            var result = await _fileService.GetPostFileAsync(fileId);

            if (result.FileView != null)
            {
                ContentDisposition cd = new ContentDisposition
                {
                    FileName = result.FileView.Name,
                    Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(result.FileView.FileBytes, result.FileView.MimeType);
            }

            return Ok(result);

        }



        [HttpGet("download-post-image/{postId}", Name = "GetPostImageFileAsync")]
        public async Task<IActionResult> GetPostImageFileAsync(string postId)
        {

            var result = await _fileService.GetPostImageAsync(postId);

            if (result.FileView.FileBytes != null && result.FileView.MimeType != null)
            {
                ContentDisposition cd = new ContentDisposition
                {
                    FileName = result.FileView.Name,
                    Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(result.FileView.FileBytes, result.FileView.MimeType);
            }

            return Ok(result);

        }

        [HttpPost("post-upload-profile-image", Name = "UploadProfileImage")]
        public async Task<IActionResult> UploadProfileImageAsync([FromForm]UploadProfileImageView  viewModel)
        {
            var model = await _fileService.UploadProfileImageAsync(viewModel).ConfigureAwait(false);
            return Ok(model);    
        }


        [HttpPost("post-upload-document", Name = "UploadDocument")]
        public async Task<IActionResult> UploadDocumentAsync([FromForm]UploadDocumentView viewModel)
        {
            var model = await _fileService.UploadDocumentAsync(viewModel);

            return Ok(model);
        }

        [HttpPost("post-upload-documents", Name = "UploadDocuments")]
        public async Task<IActionResult> UploadDocumentsAsync([FromForm]UploadAllDocumentView viewModel)
        {
            var model = await _fileService.UploadDocumentsAsync(viewModel);
            return Ok(model);
        }

        [HttpPost("post-upload-video", Name = "UploadBioVideo")]
        public async Task<IActionResult> UploadBioVideoAsync([FromForm]UploadBioVideoView viewModel)
        {
            var domainUrl = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;
            var model = await _fileService.UploadBioVideoAsync(viewModel).ConfigureAwait(false);

            model.UploadedFileUrl = domainUrl + $"/api/File/get-download-video/{viewModel?.UserId}";
            return Ok(model);
        }

        [HttpGet("get-download-document/{documentId}", Name = "DownloadDocument")]
        public async Task<IActionResult> DownloadDocumentAsync(int documentId)
        {

            var result = await _fileService.DownloadDocumentAsync(documentId);
            if (result.FileView != null)
            {
                ContentDisposition cd = new ContentDisposition
                {
                    FileName = result.FileView.Name,
                    Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(result.FileView.FileBytes, result.FileView.MimeType);
            }

            return Ok(result);

        }

        [HttpGet("get-file/{fileId}", Name = "FileDownload")]
        public async Task<IActionResult> FileDownloadAsync(string fileId)
        {

            var result = await _fileService.GetFileAsync(fileId);
            if (result.FileView != null)
            {
                ContentDisposition cd = new ContentDisposition
                {
                    FileName = result.FileView.Name,
                    Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(result.FileView.FileBytes, result.FileView.MimeType);
            }

            return Ok(result);

        }

        [HttpGet("get-download-video/{profileId}", Name = "DownloadVideo")]
        public async Task<IActionResult> DownloadVideoAsync(int profileId)
        {

            var result = await _fileService.DownloadVideoAsync(profileId);
            if (result.FileView != null)
            {
                ContentDisposition cd = new ContentDisposition
                {
                    FileName = result.FileView.Name,
                    Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                return File(result.FileView.FileBytes, result.FileView.MimeType);
            }

            return Ok(result);

        }

        [HttpPost("delete-documents", Name = "DeleteDocuments")]
        public async Task<IActionResult> DeleteDocumentsAsync([FromBody]List<DeleteDocumentView> viewModels)
        {

            var result = await _fileService.DeleteDocumentsAsync(viewModels);
            return Ok(result);

        }

        //[HttpDelete("delete-document/{documentId}", Name = "DeleteDocument")]
        //public async Task<IActionResult> DeleteDocumentAsync(int documentId)
        //{
        //    var result = await _fileService.DeleteDocumentAsync(documentId);
        //    return Ok(result);
        //}


    }
}