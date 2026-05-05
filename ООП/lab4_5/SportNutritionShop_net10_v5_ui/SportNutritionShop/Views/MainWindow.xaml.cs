using System.Windows;
using SportNutritionShop.Models;
using SportNutritionShop.ViewModels;

namespace SportNutritionShop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(UserRole role, string userName)
        {
            InitializeComponent();
            DataContext = new MainViewModel(role, userName);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}
