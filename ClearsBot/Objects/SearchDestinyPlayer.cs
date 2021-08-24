﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    public class SearchDestinyPlayer
    {
        [JsonProperty("Response")]
        public UserInfoCard[] Response { get; set; }
        [JsonProperty("ErrorCode")]
        public Int32 ErrorCode { get; set; }
        [JsonProperty("ThrottleSeconds")]
        public Int32 ThrottleSeconds { get; set; }
        [JsonProperty("ErrorStatus")]
        public string ErrorStatus { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
        [JsonProperty("MessageData")]
        public Dictionary<string, string> MessageData { get; set; }
        [JsonProperty("DetailedErrorTrace")]
        public string DetailedErrorTrace { get; set; }
    }
}
