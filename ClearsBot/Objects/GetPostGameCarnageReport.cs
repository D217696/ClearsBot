using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    public class GetPostGameCarnageReport
    {
        [JsonProperty("Response")]
        public DestinyPostGameCarnageReportData Response { get; set; }
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
    public class DestinyPostGameCarnageReportData
    {
        [JsonProperty("period")]
        public DateTime Period { get; set; }
        [JsonProperty("startingPhaseIndex")]
        public Int32 StartingPhaseIndex { get; set; }
        [JsonProperty("activityDetails")]
        public DestinyHistoricalStatsActivity ActivityDetails { get; set; }
        [JsonProperty("entries")]
        public DestinyPostGameCarnageReportEntry[] Entries { get; set; }
        [JsonProperty("teams")]
        public DestinyPostGameCarnageReportTeamEntry[] Teams { get; set; }
    }
    public class DestinyPostGameCarnageReportEntry
    {
        [JsonProperty("standing")]
        public Int32 Standing { get; set; }
        [JsonProperty("score")]
        public DestinyHistoricalStatsValue Score { get; set; }
        [JsonProperty("player")]
        public DestinyPlayer Player { get; set; }
        [JsonProperty("characterId")]
        public Int64 CharacterId { get; set; }
        [JsonProperty("values")]
        public Dictionary<string, DestinyHistoricalStatsValue> Values { get; set; }
        [JsonProperty("extended")]
        public DestinyPostGameCarnageReportExtendedData Extended { get; set; }
    }
    public class DestinyPostGameCarnageReportTeamEntry
    {
        [JsonProperty("teamId")]
        public Int32 TeamId { get; set; }
        [JsonProperty("standing")]
        public DestinyHistoricalStatsValue Standing { get; set; }
        [JsonProperty("score")]
        public DestinyHistoricalStatsValue Score { get; set; }
        [JsonProperty("teamName")]
        public string TeamName { get; set; }
    }
    public class DestinyPlayer
    {
        [JsonProperty("destinyUserInfo")]
        public UserInfoCard DestinyUserInfo { get; set; }
        [JsonProperty("characterClass")]
        public string CharacterClass { get; set; }
        [JsonProperty("classHash")]
        public UInt32 ClassHash { get; set; }
        [JsonProperty("raceHash")]
        public UInt32 RaceHash { get; set; }
        [JsonProperty("genderHash")]
        public UInt32 GenderHash { get; set; }
        [JsonProperty("characterLevel")]
        public Int32 CharacterLevel { get; set; }
        [JsonProperty("lightLevel")]
        public Int32 LightLevel { get; set; }
        [JsonProperty("bungieNetUserInfo")]
        public UserInfoCard BungieNetUserInfo { get; set; }
        [JsonProperty("clanName")]
        public string ClanName { get; set; }
        [JsonProperty("clanTag")]
        public string ClanTag { get; set; }
        [JsonProperty("emblemHash")]
        public UInt32 EmblemHash { get; set; }
    }
    public class DestinyPostGameCarnageReportExtendedData
    {
        [JsonProperty("weapons")]
        public DestinyHistoricalWeaponStats[] Weapons { get; set; }
        [JsonProperty("values")]
        public Dictionary<string, DestinyHistoricalStatsValue> Values { get; set; }
    }
    public class DestinyHistoricalWeaponStats
    {
        [JsonProperty("referenceId")]
        public UInt32 ReferenceId { get; set; }
        [JsonProperty("values")]
        public Dictionary<string, DestinyHistoricalStatsValue> Values { get; set; }
    }
}
