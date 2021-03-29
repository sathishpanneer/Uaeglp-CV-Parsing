using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class UploadProfileImageView
    {
        public string FilePath => $"/api/File/get-profileImage/{UserId}";
        public IFormFile File { get; set; }
        public int UserId { get; set; }
        public string ExtraParams { get; set; }

    }
}
