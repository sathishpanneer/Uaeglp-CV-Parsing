using Uaeglp.ViewModels;

namespace Uaeglp.Contract.Communication
{
	public interface IResponse<T>
	{
		T Resource { get; }
		bool Success { get;  }
		
	}
}
