using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
	public abstract class Response<T> : IResponse<T>
	{
		public T Resource { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
		protected Response(bool success, string message)
		{
			Success = success;
			Message = message;
		}
	}
}
