using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface ILeadershipPointSystemResponse : IBaseResponse
    {
        LeadershipPointSystemView LeadershipPointSystemView { get; set; }
        CriteriaClaimedPointsView CriteriaClaimedPointsView { get; set; }
        List<FileView> MoreDetailsView { get; set; }
    }
}
