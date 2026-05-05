using System;
using System.Linq;
using System.Windows;

namespace SportNutritionShop.Services
{
    public static class LocalizationService
    {
        public static void ChangeLanguage(string cultureCode)
        {
            var app = Application.Current;
            var oldDict = app.Resources.MergedDictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Strings."));
            if (oldDict != null)
                app.Resources.MergedDictionaries.Remove(oldDict);
            var newDict = new ResourceDictionary
            {
                Source = new Uri($"Resources/Languages/Strings.{cultureCode}.xaml", UriKind.Relative)
            };
            app.Resources.MergedDictionaries.Add(newDict);
        }
    }
}
