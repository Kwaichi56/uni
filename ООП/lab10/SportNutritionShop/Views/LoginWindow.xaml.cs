using System.Windows;
using SportNutritionShop.Models;
using SportNutritionShop.Services;

namespace SportNutritionShop.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text.Trim();
            var password = PasswordBox.Password.Trim();
            if (AuthService.TryLogin(login, password, out UserRole role))
            {
                var mainWindow = new MainWindow(role, login);
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show(Application.Current.TryFindResource("InvalidCredentials")?.ToString(), Application.Current.TryFindResource("ErrorCaption")?.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
