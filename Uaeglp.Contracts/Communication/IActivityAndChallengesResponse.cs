using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels.ActivityViewModels;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface IActivityAndChallengesResponse : IBaseResponse
    {
        List<InitiativeCategoryViewModel> ActivityCategories { get; set; }

        List<InitiativeViewModel> ActivityList { get; set; }
        InitiativeViewModel Activity { get; set; }

        List<QuestionViewModel> QuestionViewModel { get; set; }
    }
}
