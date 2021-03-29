using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts
{
	public interface IFileService
    {
        FileViewModel GetFile(int id, bool loadBytes = false);
        FileViewModel GetFile(Guid idGuid, bool loadBytes = false);
        Task<IFileResponse> GetProfileImageAsync(int profileId);

        Task<IFileResponse> DownloadDocumentAsync(int documentId);

        Task<IFileResponse> UploadProfileImageAsync(UploadProfileImageView file);

        Task<IFileResponse> UploadDocumentAsync(UploadDocumentView model);

        Task<IFileResponse> UploadBioVideoAsync(UploadBioVideoView model);

        Task<IFileResponse> DeleteDocumentAsync(int documentId);

        Task<IFileResponse> DownloadVideoAsync(int profileId);

        Task<IFileResponse> DeleteDocumentsAsync(List<DeleteDocumentView> models);
        Task<IFileResponse> UploadDocumentsAsync(UploadAllDocumentView model);

        Task<IFileResponse> GetPostFileAsync(string imageId);

        Task<IFileResponse> GetPostImageAsync(string postId);

        Task<IFileResponse> GetMessagingImageFileAsync(string roomId);
        Task<IFileResponse> GetAssessmentImageAsync(int assessmentId);

        Task<IFileResponse> GetFileAsync(string fileId);

        Task<File> SaveFileAsync(IFormFile file, int userId);
        Task<File> SaveMeetupFileAsync(IFormFile file, int userId);
        Task<IFileResponse> GetCorrelationFileAsync(string fileId);
        Task<FileView> UploadFileAsync(GenericFileView model);


    }
}
