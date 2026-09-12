using System.Windows;
using SportNutritionShop.ViewModels;

namespace SportNutritionShop.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel = new();

    public LoginWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.LoginSucceeded += (_, e) =>
        {
            var mainWindow = new MainWindow(e.Role, e.Login);
            mainWindow.Show();
            Close();
        };
        _viewModel.ExitRequested += (_, _) => Application.Current.Shutdown();
    }

    // PasswordBox intentionally stays in the view; only its current value crosses to the VM.
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        => _viewModel.Password = PasswordBox.Password;
}
