using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Languages
    {
        readonly IStorage _storage;
        public Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();
        public Languages(IStorage storage)
        {
            _storage = storage;
            languages = _storage.GetLanguagesFromStorage();
            SaveLanguages();
        }

        public void SaveLanguages()
        {
            _storage.SaveLanguages(languages);
        }
    }
}
