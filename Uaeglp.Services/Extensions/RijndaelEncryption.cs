using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Uaeglp.Contracts;

namespace Uaeglp.Services.Extensions
{
    public class RijndaelEncryption : IEncryptionManager
    {
        private const string Key = "ATIA+MeHHmt+SfVyic+E45FFvpion+ZeC+GEeM+H3re=";
        private readonly Random _random;
        private readonly byte[] key;
        private readonly RijndaelManaged rm;
        private readonly UTF8Encoding encoder;

        public RijndaelEncryption(Random random)
        {
            this._random = random;
            this.rm = new RijndaelManaged();
            this.encoder = new UTF8Encoding();
            this.key = Convert.FromBase64String("ATIA+MeHHmt+SfVyic+E45FFvpion+ZeC+GEeM+H3re=");
        }


        public byte[] EncryptBytes(string unencrypted)
        {
            if (string.IsNullOrEmpty(unencrypted))
                throw new ArgumentNullException(nameof(unencrypted));
            byte[] numArray = new byte[16];
            this._random.NextBytes(numArray);
            return ((IEnumerable<byte>)numArray).Concat<byte>((IEnumerable<byte>)this.Encrypt(this.encoder.GetBytes(unencrypted), numArray)).ToArray<byte>();
        }

        public string Encrypt(string unencrypted)
        {
            if (string.IsNullOrEmpty(unencrypted))
                throw new ArgumentNullException(nameof(unencrypted));
            byte[] numArray = new byte[16];
            this._random.NextBytes(numArray);
            return this.Base64UrlEncode(((IEnumerable<byte>)numArray).Concat<byte>((IEnumerable<byte>)this.Encrypt(this.encoder.GetBytes(unencrypted), numArray)).ToArray<byte>());
        }

        public byte[] DecryptToBytes(byte[] encrypted)
        {
            if (encrypted == null)
                throw new ArgumentNullException(nameof(encrypted));
            if (encrypted.Length < 17)
                throw new ArgumentException("Not a valid encrypted string", nameof(encrypted));
            byte[] array = ((IEnumerable<byte>)encrypted).Take<byte>(16).ToArray<byte>();
            return this.Decrypt(((IEnumerable<byte>)encrypted).Skip<byte>(16).ToArray<byte>(), array);
        }

        public string Decrypt(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted))
                throw new ArgumentNullException(nameof(encrypted));
            byte[] encrypted1 = this.Base64UrlDecode(encrypted);
            if (encrypted1.Length < 17)
                throw new ArgumentException("Not a valid encrypted string", nameof(encrypted));
            return this.Decrypt(encrypted1);
        }

        private byte[] Encrypt(byte[] buffer, byte[] vector)
        {
            ICryptoTransform encryptor = this.rm.CreateEncryptor(this.key, vector);
            return this.Transform(buffer, encryptor);
        }

        private byte[] Decrypt(byte[] buffer, byte[] vector)
        {
            ICryptoTransform decryptor = this.rm.CreateDecryptor(this.key, vector);
            return this.Transform(buffer, decryptor);
        }

        private byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, transform, CryptoStreamMode.Write))
                cryptoStream.Write(buffer, 0, buffer.Length);
            return memoryStream.ToArray();
        }

        private string Base64UrlEncode(byte[] arg)
        {
            return Convert.ToBase64String(arg).Split('=')[0].Replace('+', '-').Replace('/', '_');
        }

        private byte[] Base64UrlDecode(string arg)
        {
            string s = arg.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 0:
                    return Convert.FromBase64String(s);
                case 2:
                    s += "==";
                    goto case 0;
                case 3:
                    s += "=";
                    goto case 0;
                default:
                    throw new Exception("Illegal base64url string!");
            }
        }

        public string Decrypt(byte[] encrypted)
        {
            if (encrypted == null)
                throw new ArgumentNullException(nameof(encrypted));
            return this.encoder.GetString(this.DecryptToBytes(encrypted));
        }

        public T Decrypt<T>(string encrypted, string onFailureErrorMessage = null)
        {
            string str = this.Decrypt(encrypted);
            try
            {
                return (T)Convert.ChangeType((object)str, typeof(T));
            }
            catch (Exception ex)
            {
                string message = "Failed to convert from System.String to " + typeof(T).FullName + ".\r\nEncryptedValue: " + encrypted + "\r\nDecryptedValue: " + str;
                if (!string.IsNullOrEmpty(onFailureErrorMessage))
                    message = onFailureErrorMessage + "\r\n" + message;
                throw new ArgumentException(message, ex);
            }
        }

        public byte[] EncryptBytesToBytes(byte[] bytes)
        {
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes("ATIA+MeHHmt+SfVyic+E45FFvpion+ZeC+GEeM+H3re=", new byte[4]
            {
        (byte) 67,
        (byte) 135,
        (byte) 35,
        (byte) 114
            });
            MemoryStream memoryStream = new MemoryStream();
            Aes aes = (Aes)new AesManaged();
            aes.Key = passwordDeriveBytes.GetBytes(aes.KeySize / 8);
            aes.IV = passwordDeriveBytes.GetBytes(aes.BlockSize / 8);
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        public byte[] DecryptBytesToBytes(byte[] bytes)
        {
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes("ATIA+MeHHmt+SfVyic+E45FFvpion+ZeC+GEeM+H3re=", new byte[4]
            {
        (byte) 67,
        (byte) 135,
        (byte) 35,
        (byte) 114
            });
            MemoryStream memoryStream = new MemoryStream();
            Aes aes = (Aes)new AesManaged();
            aes.Key = passwordDeriveBytes.GetBytes(aes.KeySize / 8);
            aes.IV = passwordDeriveBytes.GetBytes(aes.BlockSize / 8);
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }
    }
}
