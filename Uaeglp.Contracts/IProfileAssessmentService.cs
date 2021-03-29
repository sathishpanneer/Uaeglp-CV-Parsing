using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts
{
	public interface IProfileAssessmentService
    {
        Task<IProfileAssessmentResponse> GetBioInfoAsync(int userId);

        Task<IProfileAssessmentResponse> UpdateBioInfoAsync(BioVideoView model);

        Task<IProfileAssessmentResponse> AddOrUpdateProfileEducationAsync(ProfileEducationView model);

        Task<IProfileAssessmentResponse> AddOrUpdateProfileWorkExperienceAsync(ProfileWorkExperienceView model);


        Task<IProfileAssessmentResponse> AddOrUpdateSkillsAndInterestAsync(
            SkillAndInterestView model);

        Task<IProfileAssessmentResponse> AddOrUpdateSkillsAsync(
            SkillAndInterestView model);

        Task<IProfileAssessmentResponse> AddOrUpdateInterestAsync(
            SkillAndInterestView model);
        Task<IProfileAssessmentResponse> AddOrUpdateLearningPreferenceAsync(int profileId,
            List<ProfileLearningPreferenceView> models);

        Task<IProfileAssessmentResponse> AddOrUpdateProfileAchievementAsync(ProfileAchievementView model);

        Task<IProfileAssessmentResponse> AddOrUpdateProfileMembershipAsync(ProfileMembershipView model);

        Task<IProfileAssessmentResponse> AddOrUpdateProfileTrainingAsync(ProfileTrainingView model);

        Task<IProfileAssessmentResponse> DeleteProfileWorkExperienceAsync(int profileExperienceId);

        Task<IProfileAssessmentResponse> DeleteProfileAchievementAsync(int achievementId);

        Task<IProfileAssessmentResponse> DeleteProfileMembershipAsync(int membershipId);

        Task<IProfileAssessmentResponse> DeleteProfileTrainingAsync(int trainingId);
        Task<IProfileAssessmentResponse> DeleteProfileEducationAsync(int profileEducationId);
    }
}
