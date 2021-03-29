using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Uaeglp.Contracts
{
    public interface IUserIPAddress
    {
        Task<string> GetUserIP();
    }
}
