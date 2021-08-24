using ClearsBot.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public static class Raids
    {
        public static Dictionary<ulong, List<Raid>> raids = new Dictionary<ulong, List<Raid>>();
        private const string configFolder = "Resources";
        private const string configFile = "raids.json";
        public static async Task Initialize()
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
        public static void SaveRaids()
        {
            File.WriteAllText(configFolder + "/" + configFile, JsonConvert.SerializeObject(raids, Formatting.Indented));
        }
    }
}
