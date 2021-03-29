using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Contracts
{
	public interface IYoutubeVideoUploadService
    {
        Task<string> UploadPostVideoAsync(string description,
            string postId, IFormFile file);
    }
}
