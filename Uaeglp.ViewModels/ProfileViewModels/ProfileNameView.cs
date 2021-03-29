using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class ProfileNameView : BaseProfileView
    {
        public int UserId { get; set; }

        public string FirstNameEN { get; set; }

        public string SecondNameEN { get; set; }

        public string ThirdNameEN { get; set; }

        public string LastNameEN { get; set; }

        public string FirstNameAR { get; set; }

        public string SecondNameAR { get; set; }

        public string ThirdNameAR { get; set; }

        public string LastNameAR { get; set; }

        public string FullNameEN => FirstNameEN + " " + LastNameEN;
        public string FullNameAR => FirstNameAR + " " + LastNameAR;


    }


    public class UserDetailsView : Result
    {
        public int UserId { get; set; }

        public string FirstNameEN { get; set; }

        public string SecondNameEN { get; set; }

        public string ThirdNameEN { get; set; }

        public string LastNameEN { get; set; }

        public string FirstNameAR { get; set; }

        public string SecondNameAR { get; set; }

        public string ThirdNameAR { get; set; }

        public string LastNameAR { get; set; }

        public string FullNameEN => FirstNameEN + " " + LastNameEN;
        public string FullNameAR => FirstNameAR + " " + LastNameAR;

        public bool HasAdminRights { get; set; }
        public bool HasUserRights { get; set; }

        public string Designation { get; set; }
        public string DesignationAr { get; set; }
        public int UserImageFileId { get; set; }
        public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;

        public decimal ProfileCompletedPercentage { get; set; }

        public List<FilterTypeViewModel> UserGroups { get; set; } = new List<FilterTypeViewModel>();

        public List<ApplicationSettingViewModel> ApplicationSetting { get; set; } = new List<ApplicationSettingViewModel>();
    }
}
