using Uaeglp.Contract.Communication;
using Uaeglp.Contracts;
using Uaeglp.Services.Communication;

namespace Uaeglp.Services
{
	public class PingService : IPingService
    {
        public PingService()
        {
        }

		public IPingResponse ping()
		{
			return new PingResponse("ping...", System.Net.HttpStatusCode.OK);
		}
	}
}
