using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class UploadDocumentResponseView
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentURL { get; set; } = $"/api/File/get-download-document/";
        public bool IsSuccess { get; set; }
        public string ResponseMessage { get; set; }

        public DocumentType DocumentType { get; set; }
    }
}
