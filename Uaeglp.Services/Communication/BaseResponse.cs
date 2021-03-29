using NLog;
using System;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.Services.Nlog;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
	public abstract class BaseResponse : IBaseResponse
	{
		private static ILogger logger = LogManager.GetCurrentClassLogger();

		public bool Success { get; protected set; }
		public string Message { get; protected set; }
		public HttpStatusCode Status { get; protected set; }
		
		protected BaseResponse(bool success, string message): this (success, message, HttpStatusCode.OK)
		{
			Success = success;
			Message = ClientMessageConstant.Success;
			logger.Info(message);
		}

		protected BaseResponse(bool success, string message, HttpStatusCode status)
		{
			Success = success;
			Message = message;
            Status = status;
			logger.Warn(message);
		}

        protected BaseResponse(bool success, string message, HttpStatusCode status, Exception e)
        {
            Success = false;
            Message = message;
            Status = status;
            logger.Error(e);
        }

		protected BaseResponse(bool success, string message, Exception e)
        {
            Success = success;
            Message = message;
			logger.Error(e);
		}

		protected BaseResponse()
        {
            Message = ClientMessageConstant.Success;
			Success = true;
		}

		protected BaseResponse(Exception e)
		{
			Success = false;
			Message = ClientMessageConstant.WeAreUnableToProcessYourRequest;
			Status = HttpStatusCode.InternalServerError;
			logger.Error(e);
		}
	}
}
