using ClearsBot.Objects;
using System;
using System.Threading.Tasks;

namespace ClearsBot.Modules
    public interface IBungie
    {
        DateTime ReleaseDate { get; set; }
        Task<GetActivityHistoryResponse> GetCharacterPagesAsync(int membershipType, long membershipId, Character character, DateTime releaseDate);
        Task<GetCompletionsResponse> GetCompletionsForUserAsync(User user);
        Task<GetFreshForCompletionResponse> GetFreshForCompletionAsync(Completion completion);
        Task<RequestData> GetRequestDataAsync(string membershipId = "", string membershipType = "");
    }
}