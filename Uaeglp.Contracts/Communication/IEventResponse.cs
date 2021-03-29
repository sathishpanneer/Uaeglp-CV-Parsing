using Uaeglp.Contract.Communication;

namespace Uaeglp.Contracts.Communication
{
	public interface IEventResponse<T> : IBaseResponse
	{
        T Data { get; set; }
    }
}
