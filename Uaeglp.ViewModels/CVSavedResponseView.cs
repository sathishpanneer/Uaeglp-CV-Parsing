using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels
{
    public class CVSavedResponseView
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string LinkedInURL { get; set; }
        public string TwitterURL { get; set; }
        public string Bio { get; set; }
        public CountryDetails Country { get; set; }
        public List<LanguageItemView> Language { get; set; }
        public SkillAndInterestView Skill { get; set; }
        public List<ProfileEducationView> Education { get; set; }
        public List<ProfileWorkExperienceView> WorkExperience { get; set; }

    }
}
