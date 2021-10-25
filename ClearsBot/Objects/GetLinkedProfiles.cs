using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ClearsBot.Objects
{
    public class GetLinkedProfiles
    {
        [JsonProperty("Response")]
        public DestinyLinkedProfilesResponse Response { get; set; }
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
    public class DestinyLinkedProfilesResponse
    {
        [JsonProperty("profiles")]
        public DestinyProfileUserInfoCard[] Profiles { get; set; }
        [JsonProperty("bnetMembership")]
        public UserInfoCard BnetMembership { get; set; }
        [JsonProperty("profilesWithErrors")]
        public DestinyErrorProfile[] ProfilesWithErros { get; set; }
    }
    public class DestinyProfileUserInfoCard
    {
        [JsonProperty("dateLastPlayed")]
        public DateTime DateLastPlayed { get; set; }
        [JsonProperty("isOverridden")]
        public bool IsOverridden { get; set; }
        [JsonProperty("isCrossSavePrimary")]
        public bool IsCrossSavePrimary { get; set; }
        [JsonProperty("platformSilver")]
        public DestinyPlatformSilverComponent PlatformSilver { get; set; }
        [JsonProperty("unpairedGameVersions")]
        public Int32 UnpairedGameVersion { get; set; }
        [JsonProperty("supplementalDisplayName")]
        public string SupplementalDisplayName { get; set; }
        [JsonProperty("iconPath")]
        public string IconPath { get; set; }
        [JsonProperty("crossSaveOverride")]
        public Int32 CrossSaveOverride { get; set; }
        [JsonProperty("applicableMembershipTypes")]
        public Int32[] ApplicableMembershipTypes { get; set; }
        [JsonProperty("isPublic")]
        public bool IsPublic { get; set; }
        [JsonProperty("membershipType")]
        public Int32 MembershipType { get; set; }
        [JsonProperty("membershipId")]
        public Int64 MembershipId { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("bungieGlobalDisplayName")]
        public string BungieGlobalDisplayName { get; set; }
        [JsonProperty("bungieGlobalDisplayNameCode")]
        public Int16 BungieGlobalDisplayNameCode { get; set; }
    }

    public class DestinyPlatformSilverComponent
    {
        [JsonProperty("platformSilver")]
        public Dictionary<Int32, DestinyItemComponent> PlatformSilver { get; set; }
    }
    public class DestinyItemComponent
    {
        [JsonProperty("itemHash")]
        public UInt32 ItemHash { get; set; }
        [JsonProperty("itemInstanceId")]
        public Int64 ItemInstanceId { get; set; }
        [JsonProperty("quantity")]
        public Int32 Quantity { get; set; }
        [JsonProperty("bindStatus")]
        public Int32 BindStatus { get; set; }
        [JsonProperty("bucketHash")]
        public UInt32 BucketHash { get; set; }
        [JsonProperty("transferStatus")]
        public Int32 TransferStatus { get; set; }
        [JsonProperty("lockable")]
        public bool Lockable { get; set; }
        [JsonProperty("state")]
        public Int32 State { get; set; }
        [JsonProperty("overrideStyleItemHash")]
        public UInt32 OverriderStyleItemHash { get; set; }
        [JsonProperty("expirationDate")]
        public DateTime ExpirationDate { get; set; }
        [JsonProperty("isWrapper")]
        public bool IsWrapper { get; set; }
        [JsonProperty("tooltipNotificationIndexes")]
        public Int32[] TooltipNotificationIndexes { get; set; }
    }
    public class DestinyErrorProfile
    {
        [JsonProperty("errorCode")]
        public Int32 ErrorCode { get; set; }
        [JsonProperty("infoCard")]
        public UserInfoCard InfoCard { get; set; }
    }
}
