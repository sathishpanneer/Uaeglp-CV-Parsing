using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface IReportProblemResponse : IBaseResponse
    {
        ReportProblemModelView ReportProblem { get; set; }
    }
}
