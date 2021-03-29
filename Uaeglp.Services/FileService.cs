using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using File = Uaeglp.Models.File;
using Profile = Uaeglp.Models.Profile;
using FileView = Uaeglp.ViewModels.ProfileViewModels.FileView;
using NLog;

namespace Uaeglp.Services
{
    public class FileService : IFileService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly FileDbContext _fileDbContext;
        private readonly AppDbContext _appDbContext;
        private readonly MongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;
        private readonly IEncryptionManager _encryption;
        private readonly IProfilePercentageCalculationService _profilePercentageCalculation;
        private readonly IUserIPAddress _userIPAddress;


        public FileService(IMapper mapper, FileDbContext fileDbContext, AppDbContext dbContext, IEncryptionManager encryption, IProfilePercentageCalculationService profilePercentageCalculation, MongoDbContext mongoDbContext, IUserIPAddress userIPAddress)
        {
            _mapper = mapper;
            _fileDbContext = fileDbContext;
            _appDbContext = dbContext;
            _encryption = encryption;
            _profilePercentageCalculation = profilePercentageCalculation;
            _mongoDbContext = mongoDbContext;
            _userIPAddress = userIPAddress;
        }


        public async Task<IFileResponse> GetProfileImageAsync(int profileId)
        {

            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == profileId);
                if (userInfo == null)
                {
                    return new FileResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);
                }
                var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == userInfo.User.OriginalImageFileId);

                if (file == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid);

                var model = new FileView()
                {
                    Id = file.Id,
                    IdGuid = file.IdGuid,
                    FileBytes = fileDb?.Bytes,
                    Name = file.Name,
                    CorrelationId = file.CorrelationId,
                    MimeType = file.MimeType,
                    SizeMb = file.SizeMb,
                    ExtraParams = file.ExtraParams
                };

                return new FileResponse(model);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new FileResponse(e);
            }

        }

        public FileViewModel GetFile(int id, bool loadBytes = false)
        {
            var file = _appDbContext.Files.FirstOrDefault(f => f.Id == id);
            if (file == null)
                throw new ArgumentException("Invalid fileID: " + id);

            FileViewModel fileDto = new FileViewModel()
            {
                ID = file.Id,
                ID_GUID = file.IdGuid,
                MimeType = file.MimeType,
                NameWithExtension = file.Name,
            };

            if (!loadBytes)
                return fileDto;
            var fileDb = _fileDbContext.FileDB.FirstOrDefault(k => k.Id == file.IdGuid);
            fileDto.Bytes = fileDb?.Bytes;
            return fileDto;
        }

        public FileViewModel GetFile(Guid idGuid, bool loadBytes = false)
        {
            var file = _appDbContext.Files.FirstOrDefault(f => f.IdGuid == idGuid);
            if (file == null)
                throw new ArgumentException("Invalid fileID_GUID: " + (object)idGuid);

            FileViewModel fileDto = new FileViewModel()
            {
                ID = file.Id,
                ID_GUID = file.IdGuid,
                MimeType = file.MimeType,
                NameWithExtension = file.Name,
            };
            if (!loadBytes)
                return fileDto;

            var fileDb = _fileDbContext.FileDB.FirstOrDefault(k => k.Id == idGuid);
            fileDto.Bytes = fileDb?.Bytes;
            return fileDto;
        }

        public async Task<IFileResponse> GetPostFileAsync(string fileId)
        {

            try
            {

                var bucket = new GridFSBucket(_mongoDbContext.Database);
                var file = new FileView();
                using (var stream = await bucket.OpenDownloadStreamAsync(new ObjectId(fileId)))
                {
                    if (stream == null)
                    {
                        return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                    }
                    file.FileBytes = await bucket.DownloadAsBytesAsync(new ObjectId(fileId));
                    file.Name = stream.FileInfo.Filename;
                    file.MimeType = stream.FileInfo.ContentType;
                }

                return new FileResponse(file);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }

        public async Task<IFileResponse> GetPostImageAsync(string postId)
        {

            try
            {
                var file = new FileView();
                var post = await _mongoDbContext.Posts.Find(k => k.ID == new ObjectId(postId)).FirstOrDefaultAsync();
                
                if (post == null) return new FileResponse(ClientMessageConstant.PostNotFound , HttpStatusCode.NotFound);

                if (post.TypeID == (int) PostType.Image)
                {
                    var fileId = post.ImageIDs.FirstOrDefault();

                    if (fileId == null)
                    {
                       return new FileResponse(file);
                    }

                    var bucket = new GridFSBucket(_mongoDbContext.Database);
                  
                    using (var stream = await bucket.OpenDownloadStreamAsync(new ObjectId(fileId)))
                    {
                        if (stream == null)
                        {
                            return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                        }
                        file.FileBytes = await bucket.DownloadAsBytesAsync(new ObjectId(fileId));
                        file.Name = stream.FileInfo.Filename;
                        file.MimeType = stream.FileInfo.ContentType;
                    }
                }else
                {
                    return new FileResponse(ClientMessageConstant.ImagePostNotExist, HttpStatusCode.NotFound);
                }
                return new FileResponse(file);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }

        public async Task<IFileResponse> GetMessagingImageFileAsync(string roomId)
        {

            try
            {
                var file = new FileView();
                var room = await _mongoDbContext.Rooms.Find(k => k.ID == new ObjectId(roomId)).FirstOrDefaultAsync();

                if (room == null) return new FileResponse(ClientMessageConstant.PostNotFound, HttpStatusCode.NotFound);

                //if (room.Messages. == (int)PostType.Image)
                //{
                var fileId = room.Messages.FirstOrDefault().ImagesIDs.ToString();

                if (fileId == null)
                {
                    return new FileResponse(file);
                }

                var bucket = new GridFSBucket(_mongoDbContext.Database);

                using (var stream = await bucket.OpenDownloadStreamAsync(new ObjectId(fileId)))
                {
                    if (stream == null)
                    {
                        return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                    }
                    file.FileBytes = await bucket.DownloadAsBytesAsync(new ObjectId(fileId));
                    file.Name = stream.FileInfo.Filename;
                    file.MimeType = stream.FileInfo.ContentType;
                }
                //}
                //else
                //{
                //  return new FileResponse(ClientMessageConstant.ImagePostNotExist, HttpStatusCode.NotFound);
                //}


                return new FileResponse(file);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }
        public async Task<IFileResponse> GetAssessmentImageAsync(int assessmentId)
        {

            try
            {
                var assessment = await _appDbContext.AssessmentTools.FirstOrDefaultAsync(x => x.Id == assessmentId);

                var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.IdGuid == assessment.ImageId);

                if (file == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid);

                var model = new FileView()
                {
                    Id = file.Id,
                    IdGuid = file.IdGuid,
                    FileBytes = fileDb.Bytes,
                    Name = file.Name,
                    CorrelationId = file.CorrelationId,
                    MimeType = file.MimeType,
                    SizeMb = file.SizeMb,
                    ExtraParams = file.ExtraParams
                };

                return new FileResponse(model);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }

        public async Task<IFileResponse> GetFileAsync(string fileId)
        {

            try
            {

                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  FileID: {fileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == new Guid(fileId));

                if (fileDb == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.IdGuid == fileDb.Id);

                var model = new FileView()
                {
                    Id = file.Id,
                    IdGuid = file.IdGuid,
                    FileBytes = fileDb.Bytes,
                    Name = file.Name,
                    CorrelationId = file.CorrelationId,
                    MimeType = file.MimeType,
                    SizeMb = file.SizeMb,
                    ExtraParams = file.ExtraParams
                };

                return new FileResponse(model);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new FileResponse(e);
            }

        }

        public async Task<IFileResponse> GetCorrelationFileAsync(string fileId)
        {

            try
            {

               var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.CorrelationId == new Guid(fileId));
                if (file == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid);

                var model = new FileView()
                {
                    Id = file.Id,
                    IdGuid = file.IdGuid,
                    FileBytes = fileDb?.Bytes,
                    Name = file.Name,
                    CorrelationId = file.CorrelationId,
                    MimeType = file.MimeType,
                    SizeMb = file.SizeMb,
                    ExtraParams = file.ExtraParams
                };

                return new FileResponse(model);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }
        public async Task<IFileResponse> DownloadDocumentAsync(int documentId)
        {
            try
            {
                var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == documentId);

                if (file == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid);

                var model = new FileView()
                {
                    Id = file.Id,
                    IdGuid = file.IdGuid,
                    FileBytes = fileDb?.Bytes,
                    Name = file.Name,
                    CorrelationId = file.CorrelationId,
                    MimeType = file.MimeType,
                    SizeMb = file.SizeMb,
                    ExtraParams = file.ExtraParams
                };

                return new FileResponse(model);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }


        public async Task<IFileResponse> DownloadVideoAsync(int profileId)
        {

            try
            {
                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == profileId);

                int.TryParse(profile.ExpressYourselfUrl, out var fileId);

                var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == fileId).ConfigureAwait(false);

                if (file == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid).ConfigureAwait(false);

                var model = new FileView()
                {
                    Id = file.Id,
                    IdGuid = file.IdGuid,
                    FileBytes = fileDb.Bytes,
                    Name = file.Name,
                    CorrelationId = file.CorrelationId,
                    MimeType = file.MimeType,
                    SizeMb = file.SizeMb,
                    ExtraParams = file.ExtraParams
                };
                return new FileResponse(model);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }

        public async Task<IFileResponse> DeleteDocumentAsync(int documentId)
        {
            try
            {
                var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == documentId);

                if (file == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }
                var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid);

                if (fileDb == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                _fileDbContext.FileDB.Remove(fileDb);
                await _fileDbContext.SaveChangesAsync();

                _appDbContext.Files.Remove(file);
                await _appDbContext.SaveChangesAsync();

                return new FileResponse(true, "Success", HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }


        public async Task<IFileResponse> DeleteDocumentsAsync(List<DeleteDocumentView> models)
        {
            var allResponse = new List<FileView>();
            try
            {
                var profileId = models.FirstOrDefault()?.UserId ?? 0;

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == profileId);

                if (profile == null)
                {
                    return new FileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                foreach (var model in models)
                {
                    var response = new FileView();
                    try
                    {
                        var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == model.Id);
                        if (file == null)
                        {
                            GetResponse(model.Id, model, ref response);
                            continue;
                        }
                        var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid);
                        if (fileDb != null)
                        {
                            _fileDbContext.FileDB.Remove(fileDb);
                            await _fileDbContext.SaveChangesAsync();
                        }

                        _appDbContext.Files.Remove(file);
                        await _appDbContext.SaveChangesAsync();

                        switch (model.DocumentType)
                        {
                            case DocumentType.Passport:
                                profile.PassportFileId = null;
                                GetResponse(model.Id, model, ref response);
                                break;
                            case DocumentType.Education:
                                profile.LastEducationCertificateFileId = null;
                                GetResponse(model.Id, model, ref response);

                                break;
                            case DocumentType.CV:
                                profile.CvfileId = null;
                                GetResponse(model.Id, model, ref response);

                                break;
                            case DocumentType.Emirates:
                                profile.UaeidfileId = null;
                                GetResponse(model.Id, model, ref response);

                                break;
                            case DocumentType.FamilyBook:
                                profile.FamilyBookFileId = null;
                                GetResponse(model.Id, model, ref response);
                                break;

                        }

                    }
                    catch (Exception e)
                    {
                        response.DocumentType = model.DocumentType;
                        response.IsSuccess = false;
                        response.ResponseMessage = ClientMessageConstant.WeAreUnableToProcessYourRequest;

                    }
                    allResponse.Add(response);
                }

                await _appDbContext.SaveChangesAsync();
                return new FileResponse(allResponse);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }



        }


        public async Task<IFileResponse> UploadProfileImageAsync(UploadProfileImageView model)
        {

            try
            {

                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == model.UserId);
                var data = new File()
                {
                    IdGuid = Guid.NewGuid(),
                    SizeMb = GetFileSize(model.File.Length),
                    Name = model.File.FileName,
                    ProviderName = "SqlProvider",
                    ExtraParams = model.ExtraParams,
                    Created = DateTime.UtcNow,
                    MimeType = model.File.ContentType,
                    Modified = DateTime.UtcNow,
                    CreatedBy = userInfo.Email,
                    ModifiedBy = userInfo.Email
                };

                var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;

                await _appDbContext.SaveChangesAsync();

                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == model.UserId);

                user.OriginalImageFileId = savedEntity.Id;
                user.SmallImageFileId = savedEntity.Id;
                user.LargeImageFileId = savedEntity.Id;

                await _appDbContext.SaveChangesAsync();
                var fileDb = new FileDB()
                {
                    Id = savedEntity.IdGuid,
                    Bytes = StreamToBytes(model.File.OpenReadStream())
                };

                await _fileDbContext.FileDB.AddAsync(fileDb);
                await _fileDbContext.SaveChangesAsync();
                return new FileResponse(true,"",HttpStatusCode.Accepted){ProfileCompletedPercentage = await _profilePercentageCalculation.UpdateProfileCompletedPercentageAsync(model.UserId), UploadedFileUrl = ConstantUrlPath.ProfileImagePath + savedEntity.Id};
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }


        public async Task<IFileResponse> UploadBioVideoAsync(UploadBioVideoView model)
        {

            try
            {

                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == model.UserId);
                var data = new File()
                {
                    IdGuid = Guid.NewGuid(),
                    SizeMb = GetFileSize(model.File.Length),
                    Name = model.File.FileName,
                    ProviderName = "SqlProvider",
                    ExtraParams = model.ExtraParams,
                    Created = DateTime.UtcNow,
                    MimeType = model.File.ContentType,
                    Modified = DateTime.UtcNow,
                    CreatedBy = userInfo.Email,
                    ModifiedBy = userInfo.Email
                };

                var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;

                await _appDbContext.SaveChangesAsync();

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == model.UserId);

                profile.ExpressYourselfUrl = _encryption.Encrypt(savedEntity.Id.ToString());

                await _appDbContext.SaveChangesAsync();
                var fileDb = new FileDB()
                {
                    Id = savedEntity.IdGuid,
                    Bytes = StreamToBytes(model.File.OpenReadStream())
                };

                await _fileDbContext.FileDB.AddAsync(fileDb);
                await _fileDbContext.SaveChangesAsync();
                return new FileResponse(true, "Success", HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }


        public async Task<IFileResponse> UploadDocumentAsync(UploadDocumentView model)
        {

            try
            {

                var savedEntity = await SaveFileAsync(model);

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == model.UserId);
                if (profile == null)
                {
                    return new FileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }
                switch (model.DocumentType)
                {
                    case DocumentType.Passport:
                        profile.PassportFileId = savedEntity.Id;
                        break;
                    case DocumentType.Education:
                        profile.LastEducationCertificateFileId = savedEntity.Id;
                        break;
                    case DocumentType.CV:
                        profile.CvfileId = savedEntity.Id;
                        break;
                    case DocumentType.Emirates:
                        profile.UaeidfileId = savedEntity.Id;
                        break;
                    case DocumentType.FamilyBook:
                        profile.FamilyBookFileId = savedEntity.Id;
                        break;

                }

                await UploadIntoFileDbAsync(savedEntity, model);

                await _appDbContext.SaveChangesAsync();
                return new FileResponse(true, "Success", HttpStatusCode.Accepted);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }

        }


        public async Task<IFileResponse> UploadDocumentsAsync(UploadAllDocumentView model)
        {
            try
            {
                var allResponse = new List<FileView>();

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == model.UserId);
                if (profile == null)
                {
                    return new FileResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                if (model.PassportFile != null)
                {
                    profile.PassportFileId = await FileUploadAsync(model.PassportFile, profile, allResponse, DocumentType.Passport);
                }
                if (model.CVFile != null)
                {
                    profile.CvfileId = await FileUploadAsync(model.CVFile, profile, allResponse, DocumentType.CV);
                }
                if (model.EducationFile != null)
                {
                    profile.LastEducationCertificateFileId = await FileUploadAsync(model.EducationFile, profile, allResponse, DocumentType.Education);
                }
                if (model.EmiratesFile != null)
                {
                    profile.UaeidfileId = await FileUploadAsync(model.EmiratesFile, profile, allResponse, DocumentType.Emirates);
                }
                if (model.FamilyBookFile != null)
                {
                    profile.FamilyBookFileId = await FileUploadAsync(model.FamilyBookFile, profile, allResponse, DocumentType.FamilyBook);
                }

                await _appDbContext.SaveChangesAsync();
                return new FileResponse(allResponse);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }


        }

        private async Task<int?> FileUploadAsync(IFormFile formFile, Profile profile, List<FileView> allResponse, DocumentType type)
        {
            try
            {
                var savedEntity = await SaveFileAsync(formFile, profile.Id);
                allResponse.Add(GetResponse(savedEntity.Id, type, formFile));
                await UploadIntoFileDbAsync(savedEntity.IdGuid, formFile);
                return savedEntity.Id;
            }
            catch (Exception e)
            {
                allResponse.Add(new FileView
                {
                    DocumentType = type,
                    IsSuccess = false,
                    ResponseMessage = ClientMessageConstant.WeAreUnableToProcessYourRequest
                });
            }

            return null;
        }

        private async Task<File> SaveFileAsync(UploadDocumentView model)
        {
            var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == model.UserId);
            var data = new File()
            {
                IdGuid = Guid.NewGuid(),
                SizeMb = GetFileSize(model.File.Length),
                Name = model.File.FileName,
                ProviderName = "SqlProvider",
                ExtraParams = model.ExtraParams,
                Created = DateTime.UtcNow,
                MimeType = model.File.ContentType,
                Modified = DateTime.UtcNow,
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email
            };

            var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;
            await _appDbContext.SaveChangesAsync();
            return savedEntity;
        }

        public async Task<File> SaveFileAsync(IFormFile file, int userId)
        {
            var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == userId);
            var data = new File()
            {
                IdGuid = Guid.NewGuid(),
                SizeMb = GetFileSize(file.Length),
                Name = file.FileName,
                ProviderName = "SqlProvider",
                ExtraParams = "",
                Created = DateTime.UtcNow,
                MimeType = file.ContentType,
                Modified = DateTime.UtcNow,
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email
            };

            var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;
            await _appDbContext.SaveChangesAsync();
            return savedEntity;
        }

        public async Task<File> SaveMeetupFileAsync(IFormFile file, int userId)
        {
            var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == userId);
            var data = new File()
            {
                IdGuid = Guid.NewGuid(),
                SizeMb = GetFileSize(file.Length),
                Name = file.FileName,
                ProviderName = "SqlProvider",
                ExtraParams = "",
                Created = DateTime.UtcNow,
                MimeType = file.ContentType,
                Modified = DateTime.UtcNow,
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email
            };

            var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;
            await _appDbContext.SaveChangesAsync();

            var fileDb = new FileDB()
            {
                Id = savedEntity.IdGuid,
                Bytes = StreamToBytes(file.OpenReadStream())
            };

            await _fileDbContext.FileDB.AddAsync(fileDb);
            await _fileDbContext.SaveChangesAsync();

            return savedEntity;
        }

        private async Task UploadIntoFileDbAsync(File savedEntity, UploadDocumentView model)
        {
            var fileDb = new FileDB()
            {
                Id = savedEntity.IdGuid,
                Bytes = StreamToBytes(model.File.OpenReadStream())
            };

            await _fileDbContext.FileDB.AddAsync(fileDb);
            await _fileDbContext.SaveChangesAsync();
        }

        public async Task UploadIntoFileDbAsync(Guid id, IFormFile formFile)
        {
            var fileDb = new FileDB()
            {
                Id = id,
                Bytes = StreamToBytes(formFile.OpenReadStream())
            };

            await _fileDbContext.FileDB.AddAsync(fileDb);
            await _fileDbContext.SaveChangesAsync();
        }

        private static void GetResponse(int id,
            DeleteDocumentView model, ref FileView response)
        {

            response.DocumentType = model.DocumentType;
            response.Id = id;
            response.IsSuccess = true;
            response.ResponseMessage = "Success";

        }

        private static FileView GetResponse(int id,
            DocumentType type, IFormFile file)
        {

            var response = new FileView
            {
                DocumentType = type,
                Name = file.FileName,
                Id = id,
                IsSuccess = true,
                ResponseMessage = "Success"
            };

            return response;

        }

        private decimal GetFileSize(long length)
        {
            if (length == 0L)
                return decimal.Zero;
            decimal num = Convert.ToDecimal(Math.Pow(1024.0, 2.0));
            return Math.Round(Convert.ToDecimal(length) / num, 3);
        }

        private static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task<IFileResponse> GetMeetupImageAsync(int MeetupId)
        {
            try
            {
                var meetupData = await _appDbContext.Meetups.FirstOrDefaultAsync(e => e.Id == MeetupId);
                
                var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.Id == meetupData.MeetupPictureID);

                if (file == null)
                {
                    return new FileResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == file.IdGuid);

                var model = new FileView()
                {
                    Id = file.Id,
                    IdGuid = file.IdGuid,
                    FileBytes = fileDb?.Bytes,
                    Name = file.Name,
                    CorrelationId = file.CorrelationId,
                    MimeType = file.MimeType,
                    SizeMb = file.SizeMb,
                    ExtraParams = file.ExtraParams
                };

                return new FileResponse(model);
            }
            catch (Exception e)
            {
                return new FileResponse(e);
            }
        }

        public async Task<FileView> UploadFileAsync(GenericFileView model)
        {

                var data = new File()
                {
                    IdGuid = Guid.NewGuid(),
                    SizeMb = GetFileSize(model.File.Length),
                    Name = model.File.FileName,
                    ProviderName = "SqlProvider",
                    //ExtraParams = model.ExtraParams,
                    Created = DateTime.UtcNow,
                    MimeType = model.File.ContentType,
                    Modified = DateTime.UtcNow,
                    CreatedBy = "",
                    ModifiedBy = ""
                };

                var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;

                await _appDbContext.SaveChangesAsync();

                await _appDbContext.SaveChangesAsync();
                var fileDb = new FileDB()
                {
                    Id = savedEntity.IdGuid,
                    Bytes = StreamToBytes(model.File.OpenReadStream())
                };

                await _fileDbContext.FileDB.AddAsync(fileDb);
                await _fileDbContext.SaveChangesAsync();

                var fileData = _mapper.Map<FileView>(data);
                fileData.IsSuccess = true;
                fileData.ResponseMessage = "Sucesss";


                return fileData;

        }
    }
}
