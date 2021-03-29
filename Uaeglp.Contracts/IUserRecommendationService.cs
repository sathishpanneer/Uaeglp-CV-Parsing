using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts
{
    public interface IUserRecommendationService
    {
        Task<IUserRecommendationResponse> SendRecommendationAsync(MultipleUserRecommendationModelView view);
        Task<IUserRecommendationResponse> AcceptDeclineRecommendationAsync(int recommendId, int recipientUserId, bool isAccepted, bool isDeclined);
        Task<IUserRecommendationResponse> SetReadRecommendationAsync(int recommendId, int recipientUserId,bool isread);
        Task<IUserRecommendationResponse> ReceiveRecommendationAsync(int recipientUser, int recommendID);
        Task<IUserRecommendationResponse> ReceiveAllRecommendationAsync(int recipientUser);
        Task<IUserRecommendationResponse> ReceiveAcceptedAllRecommendationAsync(int recipientUser);
        Task<IUserRecommendationResponse> DeleteRecommendationAsync(int recommendID);

    }
}
