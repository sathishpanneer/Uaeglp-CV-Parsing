using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.AssessmentViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services.Communication
{
	public class AssessmentResponse<T> : BaseResponse, IAssessmentResponse<T>
    {
        public T Data { get; set; }

        public AssessmentResponse(T data) : base(true, string.Empty)
        {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public AssessmentResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }

        public AssessmentResponse(Exception e) : base(e)
        { }

        public AssessmentResponse() : base()
        { }

    }
}
