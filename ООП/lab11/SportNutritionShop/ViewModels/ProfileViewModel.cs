using System.Windows;
using System.Windows.Input;
using SportNutritionShop.Helpers;
using SportNutritionShop.Models;
using SportNutritionShop.Services;

namespace SportNutritionShop.ViewModels;

public sealed class ProfileViewModel : BaseViewModel
{
    public ProfileViewModel(string login)
    {
        Profile = ProfileService.LoadProfile(login);
        SaveCommand = new RelayCommand(_ => Save());
        CloseCommand = new RelayCommand(_ => CloseRequested?.Invoke(this, EventArgs.Empty));
    }

    public UserProfile Profile { get; }
    public ICommand SaveCommand { get; }
    public ICommand CloseCommand { get; }
    public event EventHandler? CloseRequested;

    public bool LanguageRu
    {
        get => LocalizationService.CurrentCulture == "ru-RU";
        set { if (value) ChangeLanguage("ru-RU"); }
    }

    public bool LanguageEn
    {
        get => LocalizationService.CurrentCulture == "en-US";
        set { if (value) ChangeLanguage("en-US"); }
    }

    public bool ThemeClassic
    {
        get => ThemeService.CurrentTheme == "Classic";
        set { if (value) ChangeTheme("Classic"); }
    }

    public bool ThemeOptimistic
    {
        get => ThemeService.CurrentTheme == "Optimistic";
        set { if (value) ChangeTheme("Optimistic"); }
    }

    public bool ThemePink
    {
        get => ThemeService.CurrentTheme == "Pink";
        set { if (value) ChangeTheme("Pink"); }
    }

    public bool ThemeGrayscale
    {
        get => ThemeService.CurrentTheme == "Grayscale";
        set { if (value) ChangeTheme("Grayscale"); }
    }

    private void ChangeLanguage(string culture)
    {
        if (LocalizationService.CurrentCulture == culture) return;
        LocalizationService.ChangeLanguage(culture);
        OnPropertyChanged(nameof(LanguageRu));
        OnPropertyChanged(nameof(LanguageEn));
    }

    private void ChangeTheme(string theme)
    {
        if (ThemeService.CurrentTheme == theme) return;
        ThemeService.ChangeTheme(theme);
        OnPropertyChanged(nameof(ThemeClassic));
        OnPropertyChanged(nameof(ThemeOptimistic));
        OnPropertyChanged(nameof(ThemePink));
        OnPropertyChanged(nameof(ThemeGrayscale));
    }

    private void Save()
    {
        Profile.FirstName = Profile.FirstName.Trim();
        Profile.LastName = Profile.LastName.Trim();
        Profile.Email = Profile.Email.Trim();
        Profile.Phone = Profile.Phone.Trim();
        ProfileService.SaveProfile(Profile);
        MessageBox.Show(
            Application.Current.TryFindResource("SaveSuccess")?.ToString(),
            Application.Current.TryFindResource("InfoCaption")?.ToString(),
            MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
