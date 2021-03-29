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
	public class EventResponse<T> : BaseResponse, IEventResponse<T>
    {
        public T Data { get; set; }

        public EventResponse(T data) : base(true, string.Empty)
        {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public EventResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }

        public EventResponse(Exception e) : base(e)
        { }

        public EventResponse() : base()
        { }

    }
}
