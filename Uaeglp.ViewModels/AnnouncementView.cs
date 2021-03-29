using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class AnnouncementView
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ImageArId { get; set; }
        public int ImageEnId { get; set; }
        public string LinkAr { get; set; }
        public string LinkEn { get; set; }
        public bool IsActive { get; set; }
        public string MobileLink { get; set; }
        public int Order { get; set; }
        public bool? isHighlights { get; set; }
        public string MobileLinkNameEN { get; set; }
        public string MobileLinkNameAR { get; set; }
        public string MobileLinkDescriptionEN { get; set; }
        public string MobileLinkDescriptionAR { get; set; }
        public string ImageEnUrl => $"/api/File/get-download-document/{ImageEnId}";
        public string ImageArUrl => $"/api/File/get-download-document/{ImageArId}";
    }
}
