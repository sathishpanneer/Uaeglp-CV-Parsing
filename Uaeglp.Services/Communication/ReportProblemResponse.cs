using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
    public class ReportProblemResponse : BaseResponse, IReportProblemResponse
    {
        public ReportProblemModelView ReportProblem { get; set; }

        private ReportProblemResponse(bool success, string message, ReportProblemModelView reportProblem) : base(success, message)
        {
            ReportProblem = reportProblem;
        }

        public ReportProblemResponse(ReportProblemModelView ReportProblem) : this(true, ClientMessageConstant.Success, ReportProblem)
        { }

        public ReportProblemResponse(Exception e) : base(e)
        { }

        public ReportProblemResponse() : base()
        { }

        public ReportProblemResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }
    }
}
