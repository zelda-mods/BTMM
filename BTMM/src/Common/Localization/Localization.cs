﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace BTMM.Common.Localization;

public class Localization : INotifyPropertyChanged
{
    public static Localization Instance { get; } = new();

    private static Dictionary<string, string>? _languages;

    private const string DefaultLanguage = "en-US";

    private static string LanguagePath => Path.Combine(PathConfig.AppDataPath, "languages");

    public event PropertyChangedEventHandler? PropertyChanged;

    private const string IndexerName = "Item";
    private const string IndexerArrayName = "Item[]";
    private Dictionary<string, string>? _strings;

    private Localization()
    {
        var languageCode = Settings.Settings.Instance.Language ?? CultureInfo.CurrentCulture.Name;
        if (!ExistsLanguage(languageCode))
            languageCode = DefaultLanguage;
        Language = languageCode;
        InitLanguage(Language);
    }

    public string Language { get; private set; }

    public string this[string key]
    {
        get
        {
            if (_strings != null && _strings.TryGetValue(key, out var res))
                return System.Text.RegularExpressions.Regex.Unescape(res);
            return $"{Language}:{key}";
        }
    }

    public static Dictionary<string, string> GetLanguages(bool forceUpdate = false)
    {
        if (_languages != null && !forceUpdate) return _languages;
        _languages = new Dictionary<string, string>();
        var files = Directory.GetFiles(LanguagePath, "*.json", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            try
            {
                var info = new CultureInfo(fileName);
                _languages[fileName] = info.NativeName;
            }
            catch
            {
                // ignored
            }
        }

        return _languages;
    }

    private static bool ExistsLanguage(string language)
    {
        var languagePath = Path.Combine(LanguagePath, language + ".json");
        return File.Exists(languagePath);
    }

    public string? InitLanguage(string language)
    {
        if (!ExistsLanguage(language))
        {
            language = DefaultLanguage;
        }

        var languagePath = Path.Combine(LanguagePath, language + ".json");
        if (!File.Exists(languagePath))
        {
            return $"language notfound: {languagePath}";
        }

        try
        {
            var languageData = File.ReadAllText(languagePath, Encoding.UTF8);
            _strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(languageData);
        }
        catch (Exception e)
        {
            return $"init language error: {e.Message}";
        }

        Language = language;
        OnChange();
        return null;
    }

    private void OnChange()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerArrayName));
    }
}
