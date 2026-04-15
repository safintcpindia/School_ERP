using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;

namespace SchoolERP.Net.Services
{
    /// <summary>
    /// This class provides business logic and data access services for LocalizationService.
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CacheKeyPrefix = "Culture_";

        public LocalizationService(IWebHostEnvironment webHostEnvironment, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves an explicit string match from the rapid access RAM cache.
        /// If the exact translation key is missing, it dynamically falls back to the requested raw key itself natively.
        /// </summary>
        public string GetString(string key)
        {
            var translations = GetAllStrings();
            if (translations != null && translations.TryGetValue(key, out string value))
            {
                return value;
            }
            return key; // Return the key itself if no translation is found
        }

        public Dictionary<string, string> GetAllStrings()
        {
            string culture = GetCurrentLanguage();
            string cacheKey = CacheKeyPrefix + culture;

            if (!_cache.TryGetValue(cacheKey, out Dictionary<string, string> translations))
            {
                // Correct path as per user request: wwwroot/language/[Language]/app_files/system_lang.json
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "language", culture, "app_files", "system_lang.json");
                if (System.IO.File.Exists(filePath))
                {
                    string jsonContent = System.IO.File.ReadAllText(filePath);
                    translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                    _cache.Set(cacheKey, translations, TimeSpan.FromHours(1));
                }
                else
                {
                    // Fallback to English if file not found
                    if (culture.ToLower() != "english")
                    {
                        return GetTranslationsForLanguage("English");
                    }
                    translations = new Dictionary<string, string>();
                }
            }

            return translations;
        }

        public Dictionary<string, string> GetTranslations(string language)
        {
            return GetTranslationsForLanguage(language);
        }

        public void SaveTranslations(string language, Dictionary<string, string> translations)
        {
            // Update to use the new path if saving is needed
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "language", language, "app_files", "system_lang.json");
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var options = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            string jsonContent = JsonSerializer.Serialize(translations, options);
            System.IO.File.WriteAllText(filePath, jsonContent);

            // Update cache
            string cacheKey = CacheKeyPrefix + language;
            _cache.Set(cacheKey, translations, TimeSpan.FromHours(1));
        }

        private Dictionary<string, string> GetTranslationsForLanguage(string language)
        {
            string cacheKey = CacheKeyPrefix + language;
            if (!_cache.TryGetValue(cacheKey, out Dictionary<string, string> translations))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "language", language, "app_files", "system_lang.json");
                if (System.IO.File.Exists(filePath))
                {
                    string jsonContent = System.IO.File.ReadAllText(filePath);
                    translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                    _cache.Set(cacheKey, translations, TimeSpan.FromHours(1));
                }
                else
                {
                    translations = new Dictionary<string, string>();
                }
            }
            return translations;
        }

        public void SetLanguage(string languageCode)
        {
            var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("Language", languageCode, options);
        }

        public string GetCurrentLanguage()
        {
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["Language"];
            return string.IsNullOrEmpty(cookieValue) ? "English" : cookieValue;
        }

        public List<string> GetAvailableLanguages()
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "language");
            if (Directory.Exists(path))
            {
                // List all subdirectories as available languages
                return Directory.GetDirectories(path)
                                .Select(d => Path.GetFileName(d))
                                .Where(name => !name.StartsWith(".")) // Exclude hidden folders if any
                                .ToList();
            }
            return new List<string> { "English" };
        }

    }
}
