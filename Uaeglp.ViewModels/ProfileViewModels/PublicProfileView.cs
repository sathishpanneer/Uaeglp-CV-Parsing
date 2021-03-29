using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class PublicProfileView
    {

        public int Id { get; set; }
        public string FirstNameAr { get; set; }
        public string SecondNameAr { get; set; }
        public string ThirdNameAr { get; set; }
        public string LastNameAr { get; set; }

        public string FirstNameEn { get; set; }
        public string SecondNameEn { get; set; }
        public string ThirdNameEn { get; set; }
        public string LastNameEn { get; set; }

        public string FullNameEN => LastNameEn == "" ? FirstNameEn?.Trim() : FirstNameEn?.Trim() + " " + LastNameEn?.Trim();
        public string FullNameAR => LastNameAr == "" ? FirstNameAr?.Trim() : FirstNameAr?.Trim() + " " + LastNameAr?.Trim();

        public int PostCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public decimal CompletePercentage { get; set; }
        public int LPSPoint { get; set; }

        public int UserImageFileId { get; set; }
        public string ProfileImageUrl  => ConstantUrlPath.ProfileImagePath + UserImageFileId;

        public bool IsAmFollowing { get; set; } 

        public string About { get; set; }
        public string Designation { get; set; }
        public string DesignationAr { get; set; }

        //[JsonIgnore]
        //public List<ProfileWorkExperienceView> ProfileWorkExperience { get; set; } = new List<ProfileWorkExperienceView>();

    }
}
