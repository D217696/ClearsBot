using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Languages : ILanguages
    {
        readonly IStorage _storage;
        private Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();
        public Languages(IStorage storage)
        {
            _storage = storage;
            languages = _storage.GetLanguagesFromStorage();
            //var x = new Dictionary<string, string>();
            //x.Add("user-completions-active", "**{0}) {1}: {2} completions** \n");
            //languages.Add("en", x);
            SaveLanguages();
        }

        private void SaveLanguages()
        {
            _storage.SaveLanguages(languages);
        }

        public string GetLanguageText(string language, string key)
        {
            if (!languages.ContainsKey(language)) return "";
            if (!languages[language].ContainsKey(key)) return "";

            return languages[language][key];
        }

        public string EditLanguageText(string language, string key, string value)
        {
            if (!languages.ContainsKey(language)) return $"Dictionary doesn't contain language {language}";
            if (!languages[language].ContainsKey(key)) return $"Dictionary doesn't contain key {language} {key}";
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) return "value may not be empty";

            languages[language][key] = value;
            SaveLanguages();
            return $"{language} {key} was set to {value}";
        }
    }
}
