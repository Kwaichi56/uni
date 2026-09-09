using System.Windows;
using System.Windows.Controls;
using SportNutritionShop.ViewModels;

namespace SportNutritionShop.Views;

public partial class DatabaseWindow : Window
{
    private readonly DatabaseAdminViewModel _viewModel = new();

    public DatabaseWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += async (_, _) => await _viewModel.InitializeAsync();
    }

    private void DatabaseGrid_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName is "ImageData" or "Password" or "SqlText")
            e.Cancel = true;
        else if (e.PropertyName == "Id")
            e.Column.IsReadOnly = true;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
