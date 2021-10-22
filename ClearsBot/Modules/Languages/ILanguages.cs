namespace ClearsBot.Modules
{
    public interface ILanguages
    {
        string EditLanguageText(string language, string key, string value);
        string GetLanguageText(string language, string key);
    }
}