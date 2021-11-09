using ClearsBot.Objects;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Raids : IRaids
    {
        private Dictionary<ulong, List<Raid>> raids = new Dictionary<ulong, List<Raid>>();
        private List<Raid> RaidTemplate = new List<Raid>();
        readonly IStorage _storage;
        readonly DiscordSocketClient _client;
        public Raids(IStorage storage, DiscordSocketClient client)
        {
            _storage = storage;
            _client = client;
            raids = _storage.GetRaidsFromStorage();
            RaidTemplate = _storage.GetRaidTemplateFromStorage();
            SaveRaids();
        }
        public Func<Completion, bool> GetCriteriaByRaid(Raid raid)
        {
            if (raid == null) return completion => completion.StartingPhaseIndex <= 1;
            return completion => completion.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && completion.Time >= raid.CompletionTime && raid.Hashes.Contains(completion.RaidHash);
        }
        public Raid GetRaid(ulong guildId, string raidString)
        {
            if (raidString == "") return null;
            if (raids[guildId].Where(raid => raid.DisplayName.ToLower().Contains(raidString.ToLower().Trim()) || raid.Shortcuts.Contains(raidString.ToLower().Trim())) == null) return null;
            return raids[guildId].FirstOrDefault(raid => raid.DisplayName.ToLower().Contains(raidString.ToLower().Trim()) || raid.Shortcuts.Contains(raidString.ToLower().Trim()));
        }
        public IEnumerable<Raid> GetRaids(ulong guildId)
        {
            return raids[guildId];
        }
        //NEW guilds JOINED FUNCTION PLEASE
        public void SyncRaids()
        {
            foreach(var guild in _client.Guilds)
            {
                if (!raids.ContainsKey(guild.Id))
                {
                    raids.Add(guild.Id, RaidTemplate);
                }
            }

            SaveRaids();
        }

        public void SetAllTimeForRaid(Raid raid, TimeSpan time)
        {
            foreach(List<Raid> raidForServer in raids.Values)
            {
                foreach (Raid raidServer in raidForServer)
                {
                    if (raidServer.DisplayName == raid.DisplayName)
                    {
                        raidServer.CompletionTime = time;
                    }
                }
            }

            SaveRaids();
        }

        public Raid SetRaidTime(ulong guildId, string raidString, TimeSpan time)
        {
            Raid raid = GetRaid(guildId, raidString);
            if (raid == null) return null;

            raid.CompletionTime = time;
            SaveRaids();
            return raid;
        }
        public Raid AddShortcut(ulong guildId, string raidString, string shortcut)
        {
            Raid raid = GetRaid(guildId, raidString);
            if (raid == null) return null;

            if (raid.Shortcuts.Contains(shortcut)) return raid;

            raid.Shortcuts.Add(shortcut);
            SaveRaids();
            return raid;
        }
        private void SaveRaids()
        {
            _storage.SaveRaids(raids);
        }
        public void GuildJoined(ulong guildId)
        {
            if (raids.ContainsKey(guildId)) return;
            raids.Add(guildId, RaidTemplate);
        }
    }
}
