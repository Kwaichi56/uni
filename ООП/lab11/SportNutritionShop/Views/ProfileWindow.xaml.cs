using System.Windows;
using SportNutritionShop.ViewModels;

namespace SportNutritionShop.Views;

public partial class ProfileWindow : Window
{
    public ProfileWindow(string login)
    {
        InitializeComponent();
        var viewModel = new ProfileViewModel(login);
        viewModel.CloseRequested += (_, _) => Close();
        DataContext = viewModel;
    }
}
