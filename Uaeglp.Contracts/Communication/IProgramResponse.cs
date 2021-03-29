using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface IProgramResponse : IBaseResponse
    {
        List<CompletedProgramView> CompletedPrograms { get; set; }
        List<ProgramView> ProgramViews { get; set; }
        List<QuestionViewModel> QuestionList { get; set; }
        ProgramCompletedDetailsViewModel ProgramCompletedDetails { get; set; }
        string ReferenceNumber { get; set; }

    }
}
