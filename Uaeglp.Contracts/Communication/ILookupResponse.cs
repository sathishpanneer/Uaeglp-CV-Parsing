using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts.Communication
{
	public interface ILookupResponse : IBaseResponse
	{
         List<LookupItemView> LookupItems { get; set; }
         List<ProfileEducationFieldOfStudyView> EducationFieldOfStudyViews { get; set; }
         List<OrganizationView> OrganizationViews { get; set; }
         List<CountryView> CountryViews { get; set; }
         List<IndustryView> IndustryViews { get; set; }
         List<ProfileSkillView> ProfileSkillViews { get; set; }

         List<ProfileWorkExperienceJobTitleView> JobTitleViews { get; set; }

         List<WorkFieldView> WorkFieldViews { get; set; }

         AllLookupItemsView AllLookupItemsView { get; set; }
    }
}
