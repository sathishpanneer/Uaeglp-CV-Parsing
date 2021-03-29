using System.Net;

namespace Uaeglp.Contract.Communication
{
	public interface IBaseResponse
	{
		bool Success { get;  }
		string Message { get;  }
		HttpStatusCode Status { get; }
	}
}
