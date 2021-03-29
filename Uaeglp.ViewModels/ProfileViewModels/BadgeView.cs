using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class BadgeView
    {
        public int Id { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public int IconId { get; set; }
        public int BackgroundImageId { get; set; }
        public string SummaryEn { get; set; }
        public string SummaryAr { get; set; }
        public int MinimumPoints { get; set; }

        public string IconURL => $"/api/File/get-download-document/{IconId}";

    }
}
