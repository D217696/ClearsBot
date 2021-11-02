using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot
{
    public class Config
    {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";

        public BotConfig bot;

        public Config()
        {
            if (!Directory.Exists(configFolder)) Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
        public void EditPrefix(string newPrefix)
        {
            bot.cmdPrefix = newPrefix;
            File.WriteAllText(configFolder + configFile, bot.ToString());
        }
    }

    public class BotConfig
    {
        public string token;
        public string cmdPrefix;
        public string apiKey;
        public ulong Owner;
        public List<ulong> BotAdmins = new List<ulong>();
    }
}
