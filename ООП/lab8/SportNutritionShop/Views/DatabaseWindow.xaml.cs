using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SportNutritionShop.Models;
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
        else if (e.PropertyName == "CategoryId")
        {
            e.Column = new DataGridComboBoxColumn
            {
                Header = "Категория",
                ItemsSource = _viewModel.CategoryOptions,
                DisplayMemberPath = nameof(CategoryOption.Name),
                SelectedValuePath = nameof(CategoryOption.Id),
                SelectedValueBinding = new Binding("CategoryId")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                }
            };
        }
        else if (e.PropertyName == "Id")
            e.Column.IsReadOnly = true;
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        DatabaseGrid.CommitEdit(DataGridEditingUnit.Cell, true);
        DatabaseGrid.CommitEdit(DataGridEditingUnit.Row, true);
        await _viewModel.SaveChangesAsync();
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
