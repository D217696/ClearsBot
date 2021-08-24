using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    public class GetActivityHistory
    {
        [JsonProperty("Response")]
        public DestinyActivityHistoryResults Response { get; set; }
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
    public class DestinyActivityHistoryResults
    {
        [JsonProperty("activities")]
        public DestinyHistoricalStatsPeriodGroup[] Activities { get; set; }
    }
    public class DestinyHistoricalStatsPeriodGroup
    {
        [JsonProperty("period")]
        public DateTime Period { get; set; }
        [JsonProperty("activityDetails")]
        public DestinyHistoricalStatsActivity ActivityDetails { get; set; }
        [JsonProperty("values")]
        public Dictionary<string, DestinyHistoricalStatsValue> Values { get; set; }
    }
    public class DestinyHistoricalStatsActivity
    {
        [JsonProperty("referenceId")]
        public UInt32 ReferenceId { get; set; }
        [JsonProperty("directorActivityHash")]
        public UInt32 DirectorActivityHash { get; set; }
        [JsonProperty("instanceId")]
        public Int64 InstanceId { get; set; }
        [JsonProperty("mode")]
        public Int32 Mode { get; set; }
        [JsonProperty("modes")]
        public Int32[] Modes { get; set; }
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }
        [JsonProperty("membershipType")]
        public Int32 MembershipType { get; set; }
    }
    public class DestinyHistoricalStatsValue
    {
        [JsonProperty("statId")]
        public string StatId { get; set; }
        [JsonProperty("basic")]
        public DestinyHistoricalStatsValuePair Basic { get; set; }
        [JsonProperty("pga")]
        public DestinyHistoricalStatsValuePair Pga { get; set; }
        [JsonProperty("weighted")]
        public DestinyHistoricalStatsValuePair Weighted { get; set; }
        [JsonProperty("activityId")]
        public Int64 ActivityId { get; set; }
    }
    public class DestinyHistoricalStatsValuePair
    {
        [JsonProperty("value")]
        public double Value { get; set; }
        [JsonProperty("displayValue")]
        public string DisplayValue { get; set; }
    }
}
