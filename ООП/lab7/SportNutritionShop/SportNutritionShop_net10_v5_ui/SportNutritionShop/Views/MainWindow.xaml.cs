using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SportNutritionShop.Models;
using SportNutritionShop.UserControls;
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

            AddHandler(PriceRangeSlider.PreviewRangeChangedEvent, (RoutedEventHandler)OnPreviewRange);
            AddHandler(PriceRangeSlider.RangeChangedEvent, (RoutedEventHandler)OnRangeChanged);
            AddHandler(RatingStars.RatingChangedEvent, (RoutedEventHandler)OnRatingChanged);
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

        
        private void SaveCart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm) vm.SaveCart();
        }

        private void SaveCart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DataContext is MainViewModel vm && vm.CartItems.Any();
        }

       
        private void SavedCart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedSavedCart != null)
                vm.LoadSavedCart(vm.SelectedSavedCart);
        }

        
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
