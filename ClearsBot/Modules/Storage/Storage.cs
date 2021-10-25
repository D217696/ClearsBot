using ClearsBot.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClearsBot.Modules
{
    public class Storage : IStorage
    {
        private const string configFolder = "Resources";
        private const string usersFile = "users.json";
        private const string raidsFile = "raids.json";
        private const string raidTemplateFile = "raidsTemplate.json";
        private const string guildsFile = "guilds.json";
        private const string languagesFile = "languages.json";
        public List<User> GetUsersFromStorage()
        {
            string stringFromFile = ReadFromFile(usersFile);
            if (stringFromFile != "")
            {
                return JsonConvert.DeserializeObject<List<User>>(stringFromFile);
            }
            return new List<User>();
        }

        public Dictionary<ulong, List<Raid>> GetRaidsFromStorage()
        {
            string stringFromFile = ReadFromFile(raidsFile);
            if (stringFromFile != "")
            {
                return JsonConvert.DeserializeObject<Dictionary<ulong, List<Raid>>>(stringFromFile);
            }
            return new Dictionary<ulong, List<Raid>>();
        }
        public Dictionary<ulong, InternalGuild> GetGuildsFromStorage()
        {
            string stringFromFile = ReadFromFile(guildsFile);
            if (stringFromFile != "")
            {
                return JsonConvert.DeserializeObject<Dictionary<ulong, InternalGuild>>(stringFromFile);
            }
            return new Dictionary<ulong, InternalGuild>();
        }
        public Dictionary<string, Dictionary<string, string>> GetLanguagesFromStorage()
        {
            string stringFromFile = ReadFromFile(languagesFile);
            if (stringFromFile != "")
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(stringFromFile);
            }
            return new Dictionary<string, Dictionary<string, string>>();
        }
        public List<Raid> GetRaidTemplateFromStorage()
        {
            string stringFromFile = ReadFromFile(raidTemplateFile);
            if (stringFromFile != "")
            {
                return JsonConvert.DeserializeObject<List<Raid>>(stringFromFile);
            }
            return new List<Raid>();
        }

        public void SaveUsers(List<User> users)
        {
            File.WriteAllText(Path.Combine(configFolder, usersFile), JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented));
        }

        public void SaveRaids(Dictionary<ulong, List<Raid>> raids)
        {
            File.WriteAllText(Path.Combine(configFolder, raidsFile), JsonConvert.SerializeObject(raids, Newtonsoft.Json.Formatting.Indented));
        }

        public void SaveGuilds(Dictionary<ulong, InternalGuild> guilds)
        {
            File.WriteAllText(Path.Combine(configFolder, guildsFile), JsonConvert.SerializeObject(guilds, Newtonsoft.Json.Formatting.Indented));
        }

        public void SaveLanguages(Dictionary<string, Dictionary<string, string>> languages)
        {
            File.WriteAllText(Path.Combine(configFolder, languagesFile), JsonConvert.SerializeObject(languages, Newtonsoft.Json.Formatting.Indented));
        }

        private string ReadFromFile(string file)
        {
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

            if (!File.Exists(Path.Combine(configFolder, file)))
            {
                File.WriteAllText(Path.Combine(configFolder, file), "");
                return "";
            }
            else
            {
                return File.ReadAllText(Path.Combine(configFolder, file));
            }
        }
    }
}
