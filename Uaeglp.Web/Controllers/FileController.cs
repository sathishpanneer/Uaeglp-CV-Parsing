using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Uaeglp.Web.Extensions;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.Utilities;
using Microsoft.Extensions.Options;
using Minio;
using System.IO;
using System;
using Uaeglp.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog;
using ILogger = NLog.ILogger;
using MongoDB.Driver;
using MongoDB.Bson;
using Uaeglp.ViewModels;
using Minio.Exceptions;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] 
    public class FileController : Controller
    {
        private readonly IFileService _fileService;
        private readonly MinIoConfig _minIoConfig;
        private readonly AppDbContext _appDbContext;
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly MongoDbContext _mongoDbContext;
        private readonly FileDbContext _fileDbContext;
        public FileController(IFileService fileService, IOptions<MinIoConfig> minIoConfig, AppDbContext appDbContext, MongoDbContext mongoDbContext, FileDbContext fileDbContext)
        {
            _fileService = fileService;
            _minIoConfig = minIoConfig.Value;
            _appDbContext = appDbContext;
            _mongoDbContext = mongoDbContext;
            _fileDbContext = fileDbContext;
        }

        [Authorize]
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

        [HttpGet("download-messaging-image/{roomId}", Name = "GetMessagingImageFileAsync")]
        public async Task<IActionResult> GetMessagingImageFileAsync(string roomId)
        {

            var result = await _fileService.GetMessagingImageFileAsync(roomId);

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

        [Authorize]
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

        [Authorize]
        [HttpGet("get-download-document/{documentId}", Name = "DownloadDocument")]
        public async Task<IActionResult> DownloadDocumentAsync(int documentId)
        {
            var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == documentId);


            var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid);

            if (fileDb == null)
            {
                var minioResult = await DownloadProgramActivityVideoAsync(documentId);
                return minioResult;
            } else
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
        [HttpGet("get-correlationfile/{fileId}", Name = "CorrelationFileDownload")]
        public async Task<IActionResult> GetCorrelationFileAsync(string fileId)
        {

            var result = await _fileService.GetCorrelationFileAsync(fileId);
            if (result.FileView != null && result.FileView.FileBytes != null)
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
        //public async Task<IActionResult> DownloadVideoAsync(int profileId)
        //{

        //    var result = await _fileService.DownloadVideoAsync(profileId);
        //    if (result.FileView != null)
        //    {
        //        ContentDisposition cd = new ContentDisposition
        //        {
        //            FileName = result.FileView.Name,
        //            Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
        //        };
        //        Response.Headers.Add("Content-Disposition", cd.ToString());
        //        Response.Headers.Add("X-Content-Type-Options", "nosniff");

        //        return File(result.FileView.FileBytes, result.FileView.MimeType);
        //    }

        //    return Ok(result);

        //}

        public async Task<IActionResult> DownloadVideoAsync(int profileId)
        {

            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EndPoint : {_minIoConfig.EndPoint}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  AccessKey : {_minIoConfig.AccessKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SecretKey : {_minIoConfig.SecretKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  BucketName : {_minIoConfig.BucketName}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Location : {_minIoConfig.Location}");

            //var recommendLeader = await _appDbContext.RecommandLeaders.Where(x => x.ID == recommendId).FirstOrDefaultAsync();

            var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == profileId);

            int.TryParse(profile.ExpressYourselfUrl, out var fileId);

            var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == fileId).ConfigureAwait(false);

            //if (recommendLeader.RecommendingAudioID != null)
            //{
                var fileGuiId = file.IdGuid.ToString();
            //  mimeType = "audio/mp3";
            //}

            var endpoint = _minIoConfig.EndPoint;
            var accessKey = _minIoConfig.AccessKey;
            var secretKey = _minIoConfig.SecretKey;
            var minioForDev = _minIoConfig.MinIoForDev;
            var fileLocation = _minIoConfig.FilePath;
            var bucketName = _minIoConfig.BucketName;
            var location = _minIoConfig.Location;

            var objectName = fileGuiId;

            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            var filePath = fileLocation + objectName.ToString();
            try
            {

                if (minioForDev != true)
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey).WithSSL();
                    Stream st = new System.IO.MemoryStream();
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: file.MimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }
                else
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey);
                    Stream st = new System.IO.MemoryStream();
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: file.MimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }

                

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return NotFound();
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    // Do some work here
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                });
            }

        }

        [Authorize]
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

        [HttpGet("download-recommend-audio/{recommendId}", Name = "DownloadAudioFiles")]
        public async Task<IActionResult> GetAudioContent(int recommendId)
        {

            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EndPoint : {_minIoConfig.EndPoint}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  AccessKey : {_minIoConfig.AccessKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SecretKey : {_minIoConfig.SecretKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  BucketName : {_minIoConfig.BucketName}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Location : {_minIoConfig.Location}");

            var recommendLeader = await _appDbContext.RecommandLeaders.Where(x => x.ID == recommendId).FirstOrDefaultAsync();
            var fileId = "";
            var mimeType = "";
            if (recommendLeader.RecommendingAudioID != null)
            {
                fileId = recommendLeader.RecommendingAudioID.ToString();
                mimeType = "audio/mp3";
            }

            var bucketName = _minIoConfig.BucketName;
            var location = _minIoConfig.Location;
            var fileLocation = _minIoConfig.FilePath;

            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }
            var objectName = fileId;
            var filePath = fileLocation + objectName.ToString();
            try
            {
                var endpoint = _minIoConfig.EndPoint;
                var accessKey = _minIoConfig.AccessKey;
                var secretKey = _minIoConfig.SecretKey;
                var minioForDev = _minIoConfig.MinIoForDev;

                if (minioForDev != true)
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey).WithSSL();

                    Stream st = new System.IO.MemoryStream();
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: mimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }
                else
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey);

                    Stream st = new System.IO.MemoryStream();
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: mimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }


            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return NotFound();
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    // Do some work here
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                });
            }

        }


        [HttpGet("download-recommend-video/{recommendId}", Name = "DownloadVideoFile")]
        public async Task<IActionResult> GetVideoContent(int recommendId)
        {
            var recommendLeader = await _appDbContext.RecommandLeaders.Where(x => x.ID == recommendId).FirstOrDefaultAsync();
            var fileId = "";
            var mimeType = "";

            if (recommendLeader.RecommendingVideoID != null)
            {
                fileId = recommendLeader.RecommendingVideoID.ToString();
                mimeType = "video/mp4";
            }

            var bucketName = _minIoConfig.BucketName;
            var location = _minIoConfig.Location;
            var fileLocation = _minIoConfig.FilePath;

            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            var objectName = fileId;
            var filePath = fileLocation + objectName.ToString();
            try
            {
                var endpoint = _minIoConfig.EndPoint;
                var accessKey = _minIoConfig.AccessKey;
                var secretKey = _minIoConfig.SecretKey;
                var minioForDev = _minIoConfig.MinIoForDev;

                if(minioForDev != true)
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey).WithSSL();

                    Stream st = new System.IO.MemoryStream();
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: mimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }
                else
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey);

                    Stream st = new System.IO.MemoryStream();
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: mimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }

                

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return NotFound();
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    // Do some work here
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                });
            }

        }

        [HttpGet("download-post-video/{postVideoId}", Name = "DownloadPostVideoFile")]
        public async Task<IActionResult> GetPostVideoContent(string postVideoId)
        {
            var post = await _mongoDbContext.Posts.Find(k => k.YoutubeVideoID == postVideoId).FirstOrDefaultAsync();
            var fileId = "";
            var mimeType = "";

            if (post.YoutubeVideoID != null)
            {
                fileId = post.YoutubeVideoID;
                mimeType = "video/mp4";
            }

            var bucketName = _minIoConfig.BucketName;
            var location = _minIoConfig.Location;
            var fileLocation = _minIoConfig.FilePath;

            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            var objectName = fileId;
            var filePath = fileLocation + objectName.ToString();
            try
            {
                var endpoint = _minIoConfig.EndPoint;
                var accessKey = _minIoConfig.AccessKey;
                var secretKey = _minIoConfig.SecretKey;
                var minioForDev = _minIoConfig.MinIoForDev;

                if(minioForDev != true)
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey).WithSSL();

                    Stream st = new System.IO.MemoryStream();

                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: mimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                } else
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey);

                    Stream st = new System.IO.MemoryStream();
                    
                    
                    if(System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: mimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }
                

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return NotFound();
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    // Do some work here
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                });
            }

        }

        [Authorize]
        [HttpPost("generic-file-upload", Name = "genericFileUpload")]
        public async Task<IActionResult> genericFileUpload([FromForm]GenericFileView file)
        {
            var model = await _fileService.UploadFileAsync(file).ConfigureAwait(false);
            return Ok(model);
        }

        [HttpGet("get-program-activity-video/{documentId}", Name = "DownloadProgramActivityVideo")]
        public async Task<IActionResult> DownloadProgramActivityVideoAsync(int documentId)
        {

            var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == documentId);

            var fileGuiId = file.IdGuid.ToString();
            var bucketName = _minIoConfig.BucketName;
            var location = _minIoConfig.Location;
            var fileLocation = _minIoConfig.FilePath;

            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }
            var objectName = fileGuiId;
            var filePath = fileLocation + objectName.ToString();
            try
            {
                var endpoint = _minIoConfig.EndPoint;
                var accessKey = _minIoConfig.AccessKey;
                var secretKey = _minIoConfig.SecretKey;
                var minioForDev = _minIoConfig.MinIoForDev;

                if (minioForDev != true)
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey).WithSSL();
                    Stream st = new System.IO.MemoryStream();
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: file.MimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }
                else
                {
                    var minio = new MinioClient(endpoint, accessKey, secretKey);
                    Stream st = new System.IO.MemoryStream();
                    await minio.GetObjectAsync(bucketName, objectName, fileLocation + objectName.ToString());
                    var fileStreamVal = new FileStream(fileLocation + objectName.ToString(), FileMode.Open, FileAccess.Read);
                    FileStreamResult result = File(
                                               fileStream: fileStreamVal,
                                               contentType: file.MimeType,
                                               enableRangeProcessing: true //<-- enable range requests processing
                                           );
                    return result;
                }



        }
            catch (Exception ex)
            {
                return NotFound();
            }
            finally
            {
                Response.OnCompleted(async () =>
                {
                    // Do some work here
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                });
            }

        }


    }
}