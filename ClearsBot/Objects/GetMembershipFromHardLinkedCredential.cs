using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    public class GetMembershipFromHardLinkedCredential
    {
        [JsonProperty("Response")]
        public HardLinkedUserMembership Response { get; set; }
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
    public class HardLinkedUserMembership
    {
        [JsonProperty("membershipType")]
        public Int32 MembershipType { get; set; }
        [JsonProperty("membershipId")]
        public Int64 MembershipId { get; set; }
        [JsonProperty("CrossSaveOverriddenType")]
        public Int32 CrossSaveOverriddenType { get; set; }
        [JsonProperty("CrossSaveOverriddenMembershipId")]
        public Int64 CrossSaveOverriddenMembershipId { get; set; }
    }
}
