using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Uaeglp.ViewModels
{
    public class APIResponseModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }
        [JsonProperty("result")]
        public object Result { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("errmsg")]
        public object ErrorMessage { get; set; }
    }
}
