using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class PersonalInfoView
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string EmiratesId { get; set; }

        public int UserImageFileId { get; set; }

        public DateTime? BirthDate { get; set; }

        public List<LanguageItemView> LanguageKnown { get; set; }

        public string FullNameEN { get; set; }
        public string FullNameAR { get; set; }

        public int GenderItemId { get; set; }

        public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;


    }

    public class BioView : BaseProfileView
    {
        public int UserId { get; set; }
        public string ExpressYourSelf { get; set; }
        public string ExpressYourSelfURL { get; set; } = $@"/api/File/get-download-video/";
    }

    public class BioVideoView
    {
        public int UserId { get; set; }
        public string ExpressYourSelf { get; set; }
        public IFormFile ExpressYourSelfVideo { get; set; }
        public bool IsDeleted { get; set; }
    }

}
