//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Utilities;

namespace Uaeglp.Services
{
    public class UserIPAddress : IUserIPAddress
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserIPAddress(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string>  GetUserIP()
        {
            //var hostname = _httpContextAccessor.HttpContext.Request.Host.Value;

            var http_x_forwarded_for = _httpContextAccessor.HttpContext.Request.HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR");
            var remote_addr = _httpContextAccessor.HttpContext.Request.HttpContext.GetServerVariable("REMOTE_ADDR");

            //logger.Info($"{ GetType().Name} {  ExtensionUtility.GetCurrentMethod() }  HTTP_X_FORWARDED_FOR: {http_x_forwarded_for} REMOTE_ADDR: { remote_addr }");
            var ip = (http_x_forwarded_for != null && http_x_forwarded_for != "") ? http_x_forwarded_for : remote_addr;
            if (ip.Contains(","))
                ip = ip.Split(',')[0].Trim();
            return ip;
        }
    }
}
