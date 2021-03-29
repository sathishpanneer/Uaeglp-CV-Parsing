using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class UploadDocumentView
    {
        public int Id { get; set; }
        public string FilePath => $"/api/File/get-download-document/{Id}";
        public IFormFile File { get; set; }
        public int UserId { get; set; }
        public string ExtraParams { get; set; }
        public DocumentType DocumentType { get; set; }

    }


    public class DeleteDocumentView
    {
        public int Id { get; set; }
        public string FilePath => $"/api/File/get-download-document/{Id}";
        public int UserId { get; set; }
        public string ExtraParams { get; set; }
        public DocumentType DocumentType { get; set; }
    }

    public class UploadAllDocumentView
    {
        public IFormFile PassportFile { get; set; }
        public IFormFile EducationFile { get; set; }
        public IFormFile CVFile { get; set; }
        public IFormFile EmiratesFile { get; set; }
        public IFormFile FamilyBookFile { get; set; }
        public int UserId { get; set; }

    }
}
