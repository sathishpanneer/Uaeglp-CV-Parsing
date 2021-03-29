using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contracts.Communication;
using FileView = Uaeglp.ViewModels.ProfileViewModels.FileView;

namespace Uaeglp.Services.Communication
{
    public class FileResponse : BaseResponse, IFileResponse
	{

        public List<FileView> DocumentsInfoList { get; set; }

        
        public FileView FileView { get; set; }
        public string UploadedFileUrl { get; set; }

        public decimal ProfileCompletedPercentage { get; set; }


        private FileResponse(bool success, string message, string fileUrl) : base(success, message)
        {
            UploadedFileUrl = fileUrl;
        }

        private FileResponse(bool success, string message, FileView fileView) : base(success, message)
        {
            FileView = fileView;
        }



        private FileResponse(bool success, string message, List<FileView> documentsInfoList) : base(success, message)
        {
            DocumentsInfoList = documentsInfoList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public FileResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public FileResponse(bool state, string message, HttpStatusCode status) : base(state, message ,status)
        { }

        public FileResponse(string uploadedImageUrl) : this(true, string.Empty, uploadedImageUrl)
        { }

        public FileResponse(List<FileView> documentResponseViews) : this(true, string.Empty, documentResponseViews)
        { }

        public FileResponse(FileView fileResponse) : this(true, string.Empty, fileResponse)
        { }

        public FileResponse():base()
        {

        }

        public FileResponse(Exception e) : base(e)
        {

        }
    }
}
