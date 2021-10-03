using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    public class GetProfile
    {
        [JsonProperty("Response")]
        public DestinyProfileResponse Response { get; set; }
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
    public class DestinyProfileResponse
    {
        [JsonProperty("profile")]
        public SingleComponentResponseOfDestinyProfileComponent Profile { get; set; }
        [JsonProperty("profileTransitoryData")]
        public SingleComponentResponseOfDestinyProfileTransitoryComponent ProfileTransitoryData { get; set; }
        [JsonProperty("characters")]
        public DictionaryComponentResponseOfint64AndDestinyCharacterComponent Characters { get; set; } 
    }
    public class SingleComponentResponseOfDestinyProfileComponent
    {
        [JsonProperty("data")]
        public DestinyProfileComponent Data { get; set; }
        [JsonProperty("privacy")]
        public Int32 Privacy { get; set; }
    }
    public class DestinyProfileComponent
    {
        [JsonProperty("userInfo")]
        public UserInfoCard UserInfo { get; set; }
        [JsonProperty("dateLastPlayed")]
        public DateTime DateLastPlayed { get; set; }
        [JsonProperty("versionsOwned")]
        public Int32 VersionsOwned { get; set; }
        [JsonProperty("characterIds")]
        public Int64[] CharacterIds { get; set; }
        [JsonProperty("seasonHashes")]
        public UInt32[] SeasonHashes { get; set; }
    }
    public class UserInfoCard
    {
        [JsonProperty("iconPath")]
        public string IconPath { get; set; }
        [JsonProperty("crossSaveOverride")]
        public int CrossSaveOverride { get; set; }
        [JsonProperty("applicableMembershipTypes")]
        public int[] ApplicableMembershipTypes { get; set; }
        [JsonProperty("isPublic")]
        public bool IsPublic { get; set; }
        [JsonProperty("membershipType")]
        public int MembershipType { get; set; }
        [JsonProperty("membershipId")]
        public Int64 MembershipId { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("bungieGlobalDisplayName")]
        public string BungieGlobalDisplayName { get; set; }
        [JsonProperty("bungieGlobalDisplayNameCode")]
        public string BungieGlobalDisplayNameCode { get; set; }
    }

    public class SingleComponentResponseOfDestinyProfileTransitoryComponent
    {
        [JsonProperty("data")]
        public DestinyProfileTransitoryComponent Data { get; set; }
        [JsonProperty("privacy")]
        public Int32 Privacy { get; set; }
    }

    public class DestinyProfileTransitoryComponent
    {
        [JsonProperty("partyMembers")]
        public DestinyProfileTransitoryPartyMember[] PartyMembers { get; set; }
        [JsonProperty("currentActivity")]
        public DestinyProfileTransitoryCurrentActivity CurrentActivity { get; set; }
        [JsonProperty("joinability")]
        public DestinyProfileTransitoryJoinability Joinability { get; set; }
        [JsonProperty("tracking")]
        public DestinyProfileTransitoryTrackingEntry[] Tracking { get; set; }
        [JsonProperty("lastOrbitedDestinationHash")]
        public UInt32 LastOrbitedDestinationHash { get; set; }
    }
    public class DestinyProfileTransitoryPartyMember
    {
        [JsonProperty("membershipId")]
        public Int64 MembershipId { get; set; }
        [JsonProperty("emblemHash")]
        public UInt32 EmblemHash { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("status")]
        public Int32 Status { get; set; }
    }
    public class DestinyProfileTransitoryCurrentActivity
    {
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        [JsonProperty("score")]
        public float Score { get; set; }
        [JsonProperty("highestOpposingFactionScore")]
        public float HighestOpposingFactionScore { get; set; }
        [JsonProperty("numberOfOpponents")]
        public Int32 NumberOfOpponents { get; set; }
        [JsonProperty("numberOfPlayers")]
        public Int32 NumberOfPlayers { get; set; }
    }
    public class DestinyProfileTransitoryJoinability
    {
        [JsonProperty("openSlots")]
        public Int32 OpenSlot { get; set; }
        [JsonProperty("privacySetting")]
        public Int32 PrivacySetting { get; set; }
        [JsonProperty("closedReasons")]
        public Int32 ClosedReasons { get; set; }
    }
    public class DestinyProfileTransitoryTrackingEntry
    {
        [JsonProperty("locationHash")]
        public UInt32 LocationHash { get; set; }
        [JsonProperty("itemHash")]
        public UInt32 ItemHash { get; set; }
        [JsonProperty("objectiveHash")]
        public UInt32 ObjectiveHash { get; set; }
        [JsonProperty("activityHash")]
        public UInt32 ActivityHash { get; set; }
        [JsonProperty("questlineItemHash")]
        public UInt32 QuestlineItemHash { get; set; }
        [JsonProperty("trackedDate")]
        public DateTime TrackedDate { get; set; }
    }
    public class DictionaryComponentResponseOfint64AndDestinyCharacterComponent
    {
        [JsonProperty("data")]
        public Dictionary<Int64, DestinyCharacterComponent> Data { get; set; }
        [JsonProperty("privacy")]
        public Int32 Privacy { get; set; }
    }
    public class DestinyCharacterComponent
    {
        [JsonProperty("membershipId")]
        public Int64 MembershipId { get; set; }
        [JsonProperty("membershipType")]
        public Int32 MembershipType { get; set; }
        [JsonProperty("characterId")]
        public Int64 CharacterId { get; set; }
        [JsonProperty("dateLastPlayed")]
        public DateTime DateLastPlayed { get; set; }
        [JsonProperty("minutesPlayedThisSession")]
        public Int64 MinutesPlayedThisSession { get; set; }
        [JsonProperty("minutesPlayedTotal")]
        public Int64 MinutesPlayedTotal { get; set; }
        [JsonProperty("light")]
        public Int32 Light { get; set; }
        [JsonProperty("stats")]
        public Dictionary<UInt32, Int32> Stats { get; set; }
        [JsonProperty("raceHash")]
        public UInt32 RaceHash { get; set; }
        [JsonProperty("genderHash")]
        public UInt32 GenderHash { get; set; }
        [JsonProperty("classHash")]
        public UInt32 ClassHash { get; set; }
        [JsonProperty("raceType")]
        public Int32 RaceType { get; set; }
        [JsonProperty("classType")]
        public Int32 ClassType { get; set; }
        [JsonProperty("genderType")]
        public Int32 GenderType { get; set; }
        [JsonProperty("emblemPath")]
        public string EmblemPath { get; set; }
        [JsonProperty("emblemBackgroundPath")]
        public string EmblemBackgroundPath { get; set; }
        [JsonProperty("emblemHash")]
        public UInt32 EmblemHash { get; set; }
        [JsonProperty("emblemColor")]
        public DestinyColor EmblemColor { get; set; }
        [JsonProperty("levelProgression")]
        public DestinyProgression LevelProgression { get; set; }
        [JsonProperty("baseCharacterLevel")]
        public Int32 BaseCharacterLevel { get; set; }
        [JsonProperty("percentToNextLevel")]
        public float PercentToNextLevel { get; set; }
        [JsonProperty("titleRecordHash")]
        public UInt32 TitleRecordHash { get; set; }
    }
    public class DestinyColor
    {
        [JsonProperty("red")]
        public byte Red { get; set; }
        [JsonProperty("green")]
        public byte Green { get; set; }
        [JsonProperty("blue")]
        public byte Blue { get; set; }
        [JsonProperty("alpha")]
        public byte Alpha { get; set; }
    }
    public class DestinyProgression
    {
        [JsonProperty("progressionHash")]
        public UInt32 ProgressinHash { get; set; }
        [JsonProperty("dailyProgress")]
        public Int32 DailyProgress { get; set; }
        [JsonProperty("dailyLimit")]
        public Int32 DailyLimit { get; set; }
        [JsonProperty("weeklyProgress")]
        public Int32 WeeklyProgress { get; set; }
        [JsonProperty("weeklyLimit")]
        public Int32 WeeklyLimit { get; set; }
        [JsonProperty("currentProgress")]
        public Int32 CurrentProgress { get; set; }
        [JsonProperty("level")]
        public Int32 Level { get; set; }
        [JsonProperty("levelCap")]
        public Int32 LevelCap { get; set; }
        [JsonProperty("stepIndex")]
        public Int32 StepIndex { get; set; }
        [JsonProperty("progressToNextLevel")]
        public Int32 ProgressToNextLevel { get; set; }
        [JsonProperty("nextLevelAt")]
        public Int32 NextLevelAt { get; set; }
        [JsonProperty("currentResetCount")]
        public Int32 CurrentResetCount { get; set; }
        [JsonProperty("seasonResets")]
        public DestinyProgressionResetEntry SeasonResets { get; set; }
        [JsonProperty("rewardItemStates")]
        public Int32[] RewardItemStates { get; set; }
    }

    public class DestinyProgressionResetEntry
    {
        [JsonProperty("season")]
        public Int32 Season { get; set; }
        [JsonProperty("resets")]
        public Int32 Resets { get; set; }
    }
}
