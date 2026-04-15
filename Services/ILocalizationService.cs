using System.Collections.Generic;

namespace SchoolERP.Net.Services
{
    public interface ILocalizationService
    {
        string GetString(string key);
        Dictionary<string, string> GetAllStrings();
        Dictionary<string, string> GetTranslations(string language);
        void SaveTranslations(string language, Dictionary<string, string> translations);
        void SetLanguage(string languageCode);
        string GetCurrentLanguage();
        List<string> GetAvailableLanguages();
    }
}
