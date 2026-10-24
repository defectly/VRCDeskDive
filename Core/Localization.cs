using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace VRCDeskDive.Core;

public static class Localization
{
    private static ResourceDictionary? _translation;

    public static string Language { get; private set; } = "en";
    public static CultureInfo Culture { get; private set; } = CultureInfo.GetCultureInfo("en");
    public static event Action? Changed;

    public static string ResolveLanguage(string? language) => language is "en" or "ja"
        ? language
        : CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ja" ? "ja" : "en";

    public static void Apply(string? language)
    {
        Language = ResolveLanguage(language);
        Culture = CultureInfo.GetCultureInfo(Language);
        CultureInfo.CurrentCulture = Culture;
        CultureInfo.CurrentUICulture = Culture;
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        if (_translation is not null) dictionaries.Remove(_translation);
        _translation = new ResourceDictionary
        {
            Source = new Uri($"Resources/Strings.{Language}.xaml", UriKind.Relative),
        };
        dictionaries.Add(_translation);
        foreach (Window window in Application.Current.Windows)
            window.Language = XmlLanguage.GetLanguage(Culture.IetfLanguageTag);
        Changed?.Invoke();
    }

    public static string Text(string key) => (string)Application.Current.FindResource(key);
    public static string Format(string key, object value) => string.Format(Culture, Text(key), value);
}
