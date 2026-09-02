using System.Windows;
using SportNutritionShop.Models;
using SportNutritionShop.ViewModels;

namespace SportNutritionShop.Views
{
    public partial class MainWindow : Window
    {
        private readonly string _userName;

        public MainWindow(UserRole role, string userName)
        {
            InitializeComponent();
            _userName = userName;
            DataContext = new MainViewModel(role, userName);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow(_userName) { Owner = this };
            profileWindow.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}
