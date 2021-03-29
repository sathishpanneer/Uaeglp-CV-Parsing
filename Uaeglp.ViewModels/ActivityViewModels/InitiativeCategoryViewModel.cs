using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.ActivityViewModels
{
    public class InitiativeCategoryViewModel
    {
        public int Id { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionEn { get; set; }
        public int? ImageId { get; set; }
        public int? BannerImageId { get; set; }
        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public string DescriptionAr { get; set; }

        public string TitleAr { get; set; }

        public List<InitiativeViewModel> Activities { get; set; }

        public string ImageUrl => ConstantUrlPath.DocumentDownloadPath + ImageId;
        public string BannerImageUrl => ConstantUrlPath.DocumentDownloadPath + BannerImageId;
    }
}
