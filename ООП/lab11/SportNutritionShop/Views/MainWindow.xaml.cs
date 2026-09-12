using System.Diagnostics;
using System.Windows;
using SportNutritionShop.Models;
using SportNutritionShop.UserControls;
using SportNutritionShop.ViewModels;

namespace SportNutritionShop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(UserRole role, string userName)
        {
            InitializeComponent();
            var viewModel = new MainViewModel(role, userName);
            DataContext = viewModel;
            viewModel.ProfileRequested += (_, _) =>
                new ProfileWindow(userName) { Owner = this }.ShowDialog();
            viewModel.DatabaseRequested += (_, _) =>
            {
                new DatabaseWindow { Owner = this }.ShowDialog();
                viewModel.ReloadProducts();
            };
            viewModel.LogoutRequested += (_, _) =>
            {
                new LoginWindow().Show();
                Close();
            };

            // Демонстрация RoutedEvent:
            // - Direct (RatingChanged) — НЕ достигает родителя (подписка ниже не сработает)
            // - Tunnel (PreviewRangeChanged) — достигает Window первым (идёт сверху вниз)
            // - Bubble (RangeChanged) — достигает Window последним (идёт снизу вверх)
            AddHandler(PriceRangeSlider.PreviewRangeChangedEvent, (RoutedEventHandler)OnPreviewRange);
            AddHandler(PriceRangeSlider.RangeChangedEvent, (RoutedEventHandler)OnRangeChanged);
            AddHandler(RatingStars.RatingChangedEvent, (RoutedEventHandler)OnRatingChanged);
        }

        // ===== Демонстрация маршрутизации RoutedEvent =====
        private void OnPreviewRange(object sender, RoutedEventArgs e) =>
            Debug.WriteLine("[Tunnel]  PreviewRangeChanged достиг Window");

        private void OnRangeChanged(object sender, RoutedEventArgs e) =>
            Debug.WriteLine("[Bubble]  RangeChanged достиг Window");

        private void OnRatingChanged(object sender, RoutedEventArgs e) =>
            Debug.WriteLine("[Direct]  RatingChanged — это событие НЕ должно достигать Window (Direct не маршрутизируется)");

        private void PriceRange_Preview(object sender, RoutedEventArgs e) =>
            Debug.WriteLine("[Tunnel]  PreviewRangeChanged на самом UserControl");

        private void PriceRange_Changed(object sender, RoutedEventArgs e) =>
            Debug.WriteLine("[Bubble]  RangeChanged на самом UserControl");
    }
}
