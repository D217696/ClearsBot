using ClearsBot.Objects;
using System.Collections.Generic;

namespace ClearsBot.Modules
{
    public interface IStorage
    {
        Dictionary<ulong, Guild> GetGuildsFromStorage();
        Dictionary<string, Dictionary<string, string>> GetLanguagesFromStorage();
        Dictionary<ulong, List<Raid>> GetRaidsFromStorage();
        List<User> GetUsersFromStorage();
        List<Raid> GetRaidTemplateFromStorage();
        void SaveGuilds(Dictionary<ulong, Guild> guilds);
        void SaveLanguages(Dictionary<string, Dictionary<string, string>> languages);
        void SaveRaids(Dictionary<ulong, List<Raid>> raids);
        void SaveUsers(List<User> users);
    }
}