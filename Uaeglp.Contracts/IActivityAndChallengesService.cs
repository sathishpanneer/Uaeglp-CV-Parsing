using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels.ActivityViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Contracts
{
    public interface IActivityAndChallengesService
    {
        Task<IActivityAndChallengesResponse> GetActivityCategoryAsync(int profileId);

        Task<IActivityAndChallengesResponse> GetActivityListAsync(int profileId, int categoryId);

        Task<IActivityAndChallengesResponse> AddActivityAnswerAsync(ActivityAnswerViewModel model);
        Task<IActivityAndChallengesResponse> GetActivityQuestionsAsync(int profileId, int initiativeId);

        Task<IActivityAndChallengesResponse> GetActivityAsync(int profileId, int categoryId, int activityId);

        Task<IActivityAndChallengesResponse> GetReferenceAsync(int profileId, int activityId);
        Task<IActivityAndChallengesResponse> DeleteParticipantAsync(int profileId, int activityId);
    }
}
