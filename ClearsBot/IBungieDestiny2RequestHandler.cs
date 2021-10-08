using ClearsBot.Modules;
using ClearsBot.Objects;
using System.Threading.Tasks;

namespace ClearsBot
{
    public interface IBungieDestiny2RequestHandler
    {
        Task<GetActivityHistory> GetActivityHistoryAsync(int membershipType, long membershipId, long characterId, int page);
        Task<GetHistoricalStatsForAccount> GetHistoricalStatsForAccount(int membershipType, long membershipId);
        Task<GetMembershipFromHardLinkedCredential> GetMembershipFromHardLinkedCredentialAsync(long membershipId);
        Task<GetProfile> GetProfileAsync(int membershipType, long membershipId, DestinyComponentType[] components = null);
        Task<SearchDestinyPlayer> SearchDestinyPlayerAsync(string membershipId, string membershipType = "");
        Task<GetPostGameCarnageReport> GetPostGameCarnageReportAsync(long postGameCarnageReportId);
    }
}