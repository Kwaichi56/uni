using System.Windows;
using SportNutritionShop.Models;
using SportNutritionShop.Services;

namespace SportNutritionShop.Views
{
    public partial class ProfileWindow : Window
    {
        private readonly UserProfile _profile;
        private bool _isInitializing = true;

        public ProfileWindow(string login)
        {
            InitializeComponent();
            _profile = ProfileService.LoadProfile(login);
            FirstNameTextBox.Text = _profile.FirstName;
            LastNameTextBox.Text = _profile.LastName;
            EmailTextBox.Text = _profile.Email;
            PhoneTextBox.Text = _profile.Phone;

            // Отмечаем радиокнопку, соответствующую текущим языку и теме приложения
            LanguageRuRadio.IsChecked = LocalizationService.CurrentCulture == "ru-RU";
            LanguageEnRadio.IsChecked = LocalizationService.CurrentCulture == "en-US";

            ThemeClassicRadio.IsChecked = ThemeService.CurrentTheme == "Classic";
            ThemeOptimisticRadio.IsChecked = ThemeService.CurrentTheme == "Optimistic";
            ThemePinkRadio.IsChecked = ThemeService.CurrentTheme == "Pink";
            ThemeGrayscaleRadio.IsChecked = ThemeService.CurrentTheme == "Grayscale";

            _isInitializing = false;
        }

        private void LanguageRu_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            LocalizationService.ChangeLanguage("ru-RU");
        }

        private void LanguageEn_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            LocalizationService.ChangeLanguage("en-US");
        }

        private void ThemeClassic_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            ThemeService.ChangeTheme("Classic");
        }

        private void ThemeOptimistic_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            ThemeService.ChangeTheme("Optimistic");
        }

        private void ThemePink_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            ThemeService.ChangeTheme("Pink");
        }

        private void ThemeGrayscale_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitializing) return;
            ThemeService.ChangeTheme("Grayscale");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _profile.FirstName = FirstNameTextBox.Text.Trim();
            _profile.LastName = LastNameTextBox.Text.Trim();
            _profile.Email = EmailTextBox.Text.Trim();
            _profile.Phone = PhoneTextBox.Text.Trim();
            ProfileService.SaveProfile(_profile);
            MessageBox.Show(
                Application.Current.TryFindResource("SaveSuccess")?.ToString(),
                Application.Current.TryFindResource("InfoCaption")?.ToString(),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
