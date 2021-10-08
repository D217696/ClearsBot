using ClearsBot.Modules;
using ClearsBot.Objects;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClearsBot
{
    public class BungieDestiny2RequestHandler : IBungieDestiny2RequestHandler
    {
        private string ApiKey { get; set; } = Config.bot.apiKey;
        private string BaseUrl { get; set; } = "https://www.bungie.net/Platform";
        private readonly HttpClient client = new HttpClient();
        readonly ILogger _logger;

        public BungieDestiny2RequestHandler(ILogger logger)
        {
            client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);
            _logger = logger;
        }

        public async Task<GetActivityHistory> GetActivityHistoryAsync(int membershipType, long membershipId, long characterId, int page)
        {
            string json = await (await client.GetAsync($"{BaseUrl}/Destiny2/{membershipType}/Account/{membershipId}/Character/{characterId}/Stats/Activities/?mode=4&count=250&page={page}")).Content.ReadAsStringAsync();
            GetActivityHistory getActivityHistory = JsonConvert.DeserializeObject<GetActivityHistory>(json);
            if (getActivityHistory.ErrorCode != 1)
            {
                _logger.LogBungieError("GetActivityHistory", getActivityHistory.ErrorCode, getActivityHistory.Message, membershipType, membershipId, characterId, $"page: {page}");
            }
            else
            {
                _logger.LogBungieSuccess("GetActivityHistory", membershipType, membershipId, characterId);
            }

            return getActivityHistory;
        }
        public async Task<GetProfile> GetProfileAsync(int membershipType, long membershipId, DestinyComponentType[] components = null)
        {
            string componentString = "";
            if (components != null)
            {
                componentString = "?components=";
                foreach (DestinyComponentType component in components)
                {
                    componentString += $"{(int)component},";
                }
                componentString = componentString.Remove(componentString.Length - 1);
            }

            string json = await (await client.GetAsync($"{BaseUrl}/Destiny2/{membershipType}/Profile/{membershipId}/{componentString}")).Content.ReadAsStringAsync();
            GetProfile getProfile = JsonConvert.DeserializeObject<GetProfile>(json);
            if (getProfile.ErrorCode != 1)
            {
                _logger.LogBungieError("GetProfile", getProfile.ErrorCode, getProfile.Message, membershipType, membershipId, 0);
            }
            else
            {
                _logger.LogBungieSuccess("GetProfile", membershipType, membershipId, 0);
            }

            return getProfile;
        }
        public async Task<SearchDestinyPlayer> SearchDestinyPlayerAsync(string membershipId, string membershipType = "")
        {
            membershipId = Uri.EscapeDataString(membershipId);
            string json = await (await client.GetAsync($"{BaseUrl}/Destiny2/SearchDestinyPlayer/{membershipType}/{membershipId}/")).Content.ReadAsStringAsync();
            SearchDestinyPlayer searchDestinyPlayer = JsonConvert.DeserializeObject<SearchDestinyPlayer>(json);
            if (searchDestinyPlayer.ErrorCode != 1)
            {
                _logger.LogBungieError("SearchDestinyPlayer", searchDestinyPlayer.ErrorCode, searchDestinyPlayer.Message, 0, 0, 0, $"membershipType: {membershipType}", $"username: {membershipId}");
            }
            else
            {
                _logger.LogBungieSuccess("SearchDestinyPlayer", 0, 0, 0);
            }

            return searchDestinyPlayer;
        }
        public async Task<GetMembershipFromHardLinkedCredential> GetMembershipFromHardLinkedCredentialAsync(long membershipId)
        {
            string json = await (await client.GetAsync($"{BaseUrl}/User/GetMembershipFromHardLinkedCredential/SteamId/{membershipId}")).Content.ReadAsStringAsync();
            GetMembershipFromHardLinkedCredential getMembershipFromHardLinkedCredential = JsonConvert.DeserializeObject<GetMembershipFromHardLinkedCredential>(json);
            if (getMembershipFromHardLinkedCredential.ErrorCode != 1)
            {
                _logger.LogBungieError("GetMembershipFromHardLinkedCredential", getMembershipFromHardLinkedCredential.ErrorCode, getMembershipFromHardLinkedCredential.Message, 0, membershipId, 0);
            }
            else
            {
                _logger.LogBungieSuccess("GetMembershipFromHardLinkedCredential", 0, membershipId, 0);
            }
            return getMembershipFromHardLinkedCredential;
        }
        public async Task<GetHistoricalStatsForAccount> GetHistoricalStatsForAccount(int membershipType, long membershipId)
        {
            string json = await (await client.GetAsync($"{BaseUrl}/Destiny2/{membershipType}/Account/{membershipId}/Stats/")).Content.ReadAsStringAsync();
            GetHistoricalStatsForAccount getHistoricalStatsForAccount = JsonConvert.DeserializeObject<GetHistoricalStatsForAccount>(json);
            if (getHistoricalStatsForAccount.ErrorCode != 1)
            {
                _logger.LogBungieError("GetHistoricalStatsForAccount", getHistoricalStatsForAccount.ErrorCode, getHistoricalStatsForAccount.Message, membershipType, membershipId, 0);
            }
            else
            {
                _logger.LogBungieSuccess("GetHistoricalStatsForAccount", membershipType, membershipId, 0);
            }
            return getHistoricalStatsForAccount;
        }
        public async Task<GetPostGameCarnageReport> GetPostGameCarnageReportAsync(long postGameCarnageReportId)
        {
            string json = await (await client.GetAsync($"http://stats.bungie.net/Platform/Destiny2/Stats/PostGameCarnageReport/{postGameCarnageReportId}/")).Content.ReadAsStringAsync();
            GetPostGameCarnageReport getPostGameCarnageReport = JsonConvert.DeserializeObject<GetPostGameCarnageReport>(json);
            if (getPostGameCarnageReport.ErrorCode != 1)
            {
                _logger.LogBungieError("GetPostGameCarnageReport", getPostGameCarnageReport.ErrorCode, getPostGameCarnageReport.Message, 0, 0, 0, $"PostCarnageReportId: {postGameCarnageReportId}");
            }
            else
            {
                _logger.LogBungieSuccess("GetPostGameCarnageReport", 0, 0, 0, $"PostCarnageReportId: {postGameCarnageReportId}");
            }
            return getPostGameCarnageReport;
        }
    }
}
