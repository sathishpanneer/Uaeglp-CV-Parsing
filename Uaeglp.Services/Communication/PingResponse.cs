using System.Collections.Generic;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.SurveyViewModels;

namespace Uaeglp.Services.Communication
{
	public class PingResponse : BaseResponse, IPingResponse
	{
		public PingResponse(string message, HttpStatusCode status) : base(false, message, status)
		{ }
		public PingResponse() : base()
		{ }
	}
}
