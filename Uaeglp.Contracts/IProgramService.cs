using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Contracts
{
    public interface IProgramService
    {
        Task<IProgramResponse> GetCompletedProgramAsync(int userId);
        Task<IProgramResponse> GetAllProgramAsync(int profileId);
        Task<IProgramResponse> GetBatchQuestionsAsync(int profileId, int batchId);
        Task<IProgramResponse> GetProgramDetailsAsync(int profileId, int batchId);
        Task<IProgramResponse> AddOrUpdateApplicationAnswerAsync(ProgramAnswerViewModel model);
        Task<IProgramResponse> GetBatchDetailsAsync(int profileId, int batchId);

        Task<IProgramResponse> GetBatchsDetailAsync(int profileId, int batchId);

        Task<IProgramResponse> GetReferenceAsync(int profileId, int batchId);
    }
}
