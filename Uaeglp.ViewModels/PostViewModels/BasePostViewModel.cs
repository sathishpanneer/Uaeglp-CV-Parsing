using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.PostViewModels
{
    public class BasePostViewModel
    {
        public string ID { get; set; }
        public int UserID { get; set; }
        public PostType TypeID { get; set; }
        public string Text { get; set; }
        public bool IsAdminCreated { get; set; }
        public bool IsPublic { get; set; }
        public string YoutubeUrl { get; set; }
        public string DocumentFileName { get; set; }
        public IFormFile DocumentData { get; set; }
        public IFormFile ImageData { get; set; }
        public IFormFile VideoData { get; set; }
        public  PollView Poll { get; set; }
        public int? GroupID { get; set; }

    }
    public class BasePostViewModelVisibility
    {
        public string ID { get; set; }
        public int UserID { get; set; }
        public PostType TypeID { get; set; }
        public string Text { get; set; }
        public bool IsAdminCreated { get; set; }
        public bool IsPublic { get; set; }
        public string YoutubeUrl { get; set; }
        public string DocumentFileName { get; set; }
        public IFormFile DocumentData { get; set; }
        public IFormFile ImageData { get; set; }
        public IFormFile VideoData { get; set; }
        public PollView Poll { get; set; }
        public string GroupID { get; set; }
        public bool IsFollowers { get; set; }
    }
}
