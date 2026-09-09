using System;
using System.Linq;
using System.Windows;

namespace SportNutritionShop.Services
{
    public static class ThemeService
    {
        public static string CurrentTheme { get; private set; } = "Classic";

        public static void ChangeTheme(string themeName)
        {
            var app = Application.Current;
            var oldDict = app.Resources.MergedDictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Resources/Themes/"));
            if (oldDict != null)
                app.Resources.MergedDictionaries.Remove(oldDict);
            var newDict = new ResourceDictionary
            {
                Source = new Uri($"Resources/Themes/Theme.{themeName}.xaml", UriKind.Relative)
            };
            app.Resources.MergedDictionaries.Insert(0, newDict);
            CurrentTheme = themeName;
        }
    }
}
