using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    public class GetHistoricalStatsForAccount
    {
        [JsonProperty("Response")]
        public DestinyHistoricalStatsAccountResult Response { get; set; }
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
    public class DestinyHistoricalStatsAccountResult
    {
        [JsonProperty("mergedDeletedCharacters")]
        public DestinyHistoricalStatsWithMerged MergedDeletedCharacters { get; set; }
        [JsonProperty("mergedAllCharacters")]
        public DestinyHistoricalStatsWithMerged MergedAllCharacters { get; set; }
        [JsonProperty("characters")]
        public DestinyHistoricalStatsPerCharacter[] Characters { get; set; }
    }
    public class DestinyHistoricalStatsWithMerged
    {
        [JsonProperty("results")]
        public Dictionary<string, DestinyHistoricalStatsByPeriod> Results { get; set; }
        [JsonProperty("merged")]
        public DestinyHistoricalStatsByPeriod Merged { get; set; }
    }
    public class DestinyHistoricalStatsByPeriod
    {
        [JsonProperty("allTime")]
        public Dictionary<string, DestinyHistoricalStatsValue> AllTime { get; set; }
        [JsonProperty("allTimeTier1")]
        public Dictionary<string, DestinyHistoricalStatsValue> AllTimeTier1 { get; set; }
        [JsonProperty("allTimeTier2")]
        public Dictionary<string, DestinyHistoricalStatsValue> AllTimeTier2 { get; set; }
        [JsonProperty("allTimeTier3")]
        public Dictionary<string, DestinyHistoricalStatsValue> AllTimeTier3 { get; set; }
        [JsonProperty("daily")]
        public DestinyHistoricalStatsPeriodGroup[] Daily { get; set; }
        [JsonProperty("monthly")]
        public DestinyHistoricalStatsPeriodGroup[] Monthly { get; set; }
    }
    public class DestinyHistoricalStatsPerCharacter
    {
        [JsonProperty("characterId")]
        public Int64 CharacterId { get; set; }
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }
        [JsonProperty("results")]
        public Dictionary<string, DestinyHistoricalStatsByPeriod> Results { get; set; }
        [JsonProperty("merged")]
        public DestinyHistoricalStatsByPeriod Merged { get; set; }
    }
}
