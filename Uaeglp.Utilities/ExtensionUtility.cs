using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace Uaeglp.Utilities
{
    public static class ExtensionUtility
    {
        public static string GetCurrentMethod([CallerMemberName] string callerName = "")
        {
            return callerName;
        }
        public static decimal ToFileMB(this long length)
        {
            if (length == 0L)
                return decimal.Zero;
            decimal num = Convert.ToDecimal(Math.Pow(1024.0, 2.0));
            return Math.Round(Convert.ToDecimal(length) / num, 3);
        }

        public static byte[] ToBytes(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static int ToUint(this int val)
        {
            return val < 0 ? 0 : val;
        }


        public static string ToJsonString(this object data)
        {
            return JsonConvert.SerializeObject(data);
        }

    }
}
