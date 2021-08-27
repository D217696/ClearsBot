using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public static class Languages
    {
        public static Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();
        private const string configFolder = "Resources";
        private const string configFile = "languages.json";
        public static async Task Initialize()
        {
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(languages, Formatting.Indented));
            }
            else
            {
                string languagesString = File.ReadAllText(configFolder + "/" + configFile);
                if (languagesString == "")
                {
                    languages = new Dictionary<string, Dictionary<string, string>>();
                }
                else
                {
                    languages = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(languagesString);
                }
            }

            SaveLanguages();
        }

        public static void SaveLanguages()
        {
            File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(languages, Formatting.Indented));
        }
    }
}
