using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class AllLookupItemsView
    {
        public List<LookupItemView> LookupItems { get; set; }
        public List<ProfileEducationFieldOfStudyView> EducationFieldOfStudy { get; set; }
        public List<ProfileWorkExperienceJobTitleView> WorkExperienceJobTitle { get; set; }
        public List<WorkFieldView> WorkField { get; set; }
        public List<OrganizationView> Organization { get; set; }
        public List<CountryView> Country { get; set; }
        public List<IndustryView> Industry { get; set; }
        public List<ProfileSkillView> ProfileSkill { get; set; }
    }
}
