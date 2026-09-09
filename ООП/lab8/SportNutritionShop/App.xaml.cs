using System.Windows;
using SportNutritionShop.Services;
using SportNutritionShop.Views;

namespace SportNutritionShop
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                await DatabaseService.InitializeAsync();
                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Не удалось подготовить базу данных.\n\n{exception.Message}",
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(-1);
            }
        }
    }
}
