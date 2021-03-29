using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts
{
    public interface IReportProblemService
    {
        Task<IReportProblemResponse> ReportProblemAsync(ReportProblemView view);
    }
}
