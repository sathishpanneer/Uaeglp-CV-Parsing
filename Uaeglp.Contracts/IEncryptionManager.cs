using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts
{
    public interface IEncryptionManager
    {
        string Encrypt(string unencrypted);

        string Decrypt(string encrypted);

        T Decrypt<T>(string encrypted, string onFailureErrorMessage = null);

        string Decrypt(byte[] encrypted);

        byte[] EncryptBytes(string unencrypted);

        byte[] EncryptBytesToBytes(byte[] bytes);

        byte[] DecryptBytesToBytes(byte[] bytes);
    }
}
