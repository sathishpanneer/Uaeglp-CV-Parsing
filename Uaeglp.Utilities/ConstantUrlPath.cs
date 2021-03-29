using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.Utilities
{
    public sealed class ConstantUrlPath
    {
        /// <summary>
        /// FileId Need to be append prefix
        /// </summary>
        public const string ProfileImagePath = "/api/File/get-download-document/";
        /// <summary>
        /// DocumentId Need to be append prefix
        /// </summary>
        public const string DocumentDownloadPath = "/api/File/get-download-document/";

        public const string CorrelationFilePath = "/api/File/get-correlationfile/";
        


        public const string FileDownloadPath = "/api/File/get-file/";

        /// <summary>
        /// FileId Need to be append prefix
        /// </summary>
        public const string PostFileDownloadPath = "/api/File/download-post-file/";


        /// <summary>
        /// PostId Need to be append prefix
        /// </summary>
        public const string PostImageDownloadPath = "/api/File/download-post-image/";

        public const string AudioDownloadPath = "/api/File/download-recommend-audio/";

        public const string VideoDownloadPath = "/api/File/download-recommend-video/";

        /// <summary>
        /// YouTubeId Need to be append prefix
        /// </summary>
        public const string YouTubeUrlPath = "https://www.youtube.com/watch?v=";

        /// <summary>
        /// MeetupPictureId Need to be append prefix
        /// </summary>
        public const string MeetupImagePath = "/api/File/get-download-document/";

        public const string PostVideoUrlPath = "/api/File/download-post-video/";

    }
}
