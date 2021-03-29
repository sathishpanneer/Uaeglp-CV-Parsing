using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts
{
    public interface IRecommendLeaderService
    {
        Task<IRecommendLeaderResponse> AddRecommendLeader(RecommendLeaderView view);
        Task<IRecommendLeaderResponse> GetRecommendFitListAsync();
        Task<IRecommendLeaderResponse> RequestCallbackAsync(RecommendationCallbackView view);
        Task<IRecommendLeaderResponse> getRecommendLeaderDetailsAsync(int recommendId);
        Task<List<RecommendSubmissionView>> GetAllRecommendLeaderListAsync(int skip, int limit);
        Task<RecommendProfileView> GetViewMatchProfile(int recommendId);
    }
}
