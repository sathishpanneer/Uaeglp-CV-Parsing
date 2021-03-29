using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts.Communication
{
	public interface IProfileAssessmentResponse : IBaseResponse
	{
        ProfileEducationView ProfileEducationView { get; set; }
        BioView BioView { get; set; }
        ProfileWorkExperienceView ProfileWorkExperienceView { get; set; }
        SkillAndInterestView SkillAndInterestView { get; set; }
        ProfileAchievementView ProfileAchievementView { get; set; }
        ProfileMembershipView ProfileMembershipView { get; set; }
        ProfileTrainingView ProfileTrainingView { get; set; }
	}
}
