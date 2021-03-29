using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts
{
    public interface ILangService
    {
        Task<IResponse<List<ViewModels.Page>>> GetLabels(ViewModels.PageLabelReq view);
        Task<IResponse<ViewModels.PageNew>> GetLabel(ViewModels.PageLabelReq view);
        Task<IResponse<List<ViewModels.PageNames>>> GetPageNamesUI();
        Task<IResponse<List<ViewModels.LabelNames>>> GetLabelsUI(ViewModels.PageLabelReq view);
        Task<IResponse<List<ViewModels.LabelNames>>> SaveLabel(ViewModels.LabelNames view);
    }
}
