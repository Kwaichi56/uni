using System.Windows;
using System.Windows.Input;
using SportNutritionShop.Helpers;
using SportNutritionShop.Models;
using SportNutritionShop.Services;

namespace SportNutritionShop.ViewModels;

public sealed class LoginViewModel : BaseViewModel
{
    private string _login = string.Empty;
    private string _password = string.Empty;

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(_ => TryLogin(), _ => !string.IsNullOrWhiteSpace(Login) && Password.Length > 0);
        ExitCommand = new RelayCommand(_ => ExitRequested?.Invoke(this, EventArgs.Empty));
    }

    public string Login
    {
        get => _login;
        set { _login = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
    }

    // The view copies PasswordBox.Password here; the password is never saved in the model.
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
    }

    public ICommand LoginCommand { get; }
    public ICommand ExitCommand { get; }
    public event EventHandler<LoginSucceededEventArgs>? LoginSucceeded;
    public event EventHandler? ExitRequested;

    private void TryLogin()
    {
        var login = Login.Trim();
        if (AuthService.TryLogin(login, Password.Trim(), out UserRole role))
        {
            Password = string.Empty;
            LoginSucceeded?.Invoke(this, new LoginSucceededEventArgs(role, login));
        }
        else
        {
            MessageBox.Show(
                Application.Current.TryFindResource("InvalidCredentials")?.ToString(),
                Application.Current.TryFindResource("ErrorCaption")?.ToString(),
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

public sealed record LoginSucceededEventArgs(UserRole Role, string Login);
