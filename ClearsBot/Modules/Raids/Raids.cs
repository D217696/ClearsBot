using ClearsBot.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Raids : IRaids
    {
        private Dictionary<ulong, List<Raid>> raids = new Dictionary<ulong, List<Raid>>();
        private const string configFolder = "Resources";
        private const string configFile = "raids.json";
        public Raids()
        {
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(raids, Formatting.Indented));
            }
            else
            {
                string RaidsString = File.ReadAllText(configFolder + "/" + configFile);
                if (RaidsString == "")
                {
                    raids = new Dictionary<ulong, List<Raid>>();
                }
                else
                {
                    raids = JsonConvert.DeserializeObject<Dictionary<ulong, List<Raid>>>(RaidsString);
                }
            }

            SaveRaids();
        }
        public Raid GetRaid(ulong guildId, string raidString)
        {
            if (raidString == "") return null;
            if (raids[guildId].Where(raid => raid.DisplayName.ToLower().Contains(raidString.ToLower().Trim()) || raid.Shortcuts.Contains(raidString.ToLower().Trim())) == null) return null;
            return raids[guildId].FirstOrDefault(raid => raid.DisplayName.ToLower().Contains(raidString.ToLower().Trim()) || raid.Shortcuts.Contains(raidString.ToLower().Trim()));
        }
        public Func<Completion, bool> GetCriteriaByRaid(Raid raid)
        {
            if (raid == null) return completion => completion.StartingPhaseIndex <= 1;
            return completion => completion.StartingPhaseIndex <= raid.StartingPhaseIndexToBeFresh && completion.Time <= raid.CompletionTime && raid.Hashes.Contains(completion.RaidHash);
        }

        public List<Raid> GetRaids(ulong guildId)
        {
            return raids[guildId];
        }
        public void SaveRaids()
        {
            File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(raids, Formatting.Indented));
        }
    }
}
