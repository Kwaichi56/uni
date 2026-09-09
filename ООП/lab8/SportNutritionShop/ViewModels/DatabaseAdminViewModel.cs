using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using SportNutritionShop.Helpers;
using SportNutritionShop.Models;
using SportNutritionShop.Services;

namespace SportNutritionShop.ViewModels;

public sealed class DatabaseAdminViewModel : BaseViewModel
{
    private DataView? _currentView;
    private DataRowView? _selectedRow;
    private DatabaseTableInfo _selectedTable;
    private bool _isBusy;
    private string _status = "Готово";

    public DatabaseAdminViewModel()
    {
        Tables =
        [
            new("Products", "Товары", true),
            new("Categories", "Категории", true),
            new("Users", "Пользователи", true),
            new("Orders", "Заказы", false),
            new("OrderItems", "Состав заказов", false),
            new("AuditLog", "Журнал изменений", false),
            new("StoredCommands", "Именованные команды", false)
        ];
        _selectedTable = Tables[0];

        SelectTableCommand = new RelayCommand(async p => await SelectTableAsync(p?.ToString()));
        RefreshCommand = new RelayCommand(async _ => await LoadCurrentTableAsync(), _ => !IsBusy);
        AddRowCommand = new RelayCommand(_ => AddRow(), _ => CanEdit);
        DeleteRowCommand = new RelayCommand(_ => DeleteRow(), _ => CanEdit && SelectedRow is not null);
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanEdit);
        SortAscendingCommand = new RelayCommand(_ => Sort(false), _ => CurrentView is not null);
        SortDescendingCommand = new RelayCommand(_ => Sort(true), _ => CurrentView is not null);
        LowStockCommand = new RelayCommand(async _ => await ShowLowStockAsync(), _ => !IsBusy);
        StoredCommand = new RelayCommand(async _ => await ShowStoredCommandAsync(), _ => !IsBusy);
    }

    public ObservableCollection<DatabaseTableInfo> Tables { get; }
    public DataView? CurrentView { get => _currentView; private set { _currentView = value; OnPropertyChanged(); } }
    public DataRowView? SelectedRow { get => _selectedRow; set { _selectedRow = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); } }
    public DatabaseTableInfo SelectedTable { get => _selectedTable; private set { _selectedTable = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanEdit)); } }
    public bool CanEdit => SelectedTable.IsEditable && !IsBusy;
    public bool IsBusy { get => _isBusy; private set { _isBusy = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanEdit)); CommandManager.InvalidateRequerySuggested(); } }
    public string Status { get => _status; private set { _status = value; OnPropertyChanged(); } }
    public string DatabasePath => DatabaseService.DatabasePath;

    public ICommand SelectTableCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AddRowCommand { get; }
    public ICommand DeleteRowCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SortAscendingCommand { get; }
    public ICommand SortDescendingCommand { get; }
    public ICommand LowStockCommand { get; }
    public ICommand StoredCommand { get; }

    public Task InitializeAsync() => LoadCurrentTableAsync();

    private async Task SelectTableAsync(string? name)
    {
        var selected = Tables.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        if (selected is null) return;
        SelectedTable = selected;
        await LoadCurrentTableAsync();
    }

    private async Task LoadCurrentTableAsync()
    {
        await RunAsync(async () =>
        {
            var table = await DatabaseService.LoadTableAsync(SelectedTable.Name);
            CurrentView = table.DefaultView;
            SelectedRow = CurrentView.Count > 0 ? CurrentView[0] : null;
            Status = $"Таблица «{SelectedTable.DisplayName}»: {CurrentView.Count} записей";
        });
    }

    private void AddRow()
    {
        if (CurrentView?.Table is not DataTable table) return;
        var row = table.NewRow();
        if (table.Columns.Contains("Id"))
            row["Id"] = -DateTime.Now.Ticks;

        switch (SelectedTable.Name)
        {
            case "Categories":
                row["Name"] = $"Новая категория {DateTime.Now:HHmmss}";
                break;
            case "Users":
                row["Login"] = $"user_{DateTime.Now:HHmmss}";
                row["Password"] = "123";
                row["Role"] = "Client";
                row["FirstName"] = string.Empty;
                row["LastName"] = string.Empty;
                row["Email"] = string.Empty;
                row["Phone"] = string.Empty;
                break;
            case "Products":
                row["CategoryId"] = 1L;
                row["ShortName"] = "Новый товар";
                row["FullName"] = "Полное название нового товара";
                row["Description"] = "Описание";
                row["Brand"] = "Бренд";
                row["Country"] = "Беларусь";
                row["Flavor"] = "Без вкуса";
                row["Weight"] = "500 г";
                row["Price"] = 0d;
                row["DiscountPercent"] = 0d;
                row["Quantity"] = 0L;
                row["Rating"] = 0d;
                row["SoldCount"] = 0L;
                row["ImageFileName"] = DBNull.Value;
                row["ImageData"] = DBNull.Value;
                break;
        }

        table.Rows.Add(row);
        SelectedRow = table.DefaultView.Cast<DataRowView>().First(x => ReferenceEquals(x.Row, row));
        Status = "Добавлена новая строка. Заполните поля и нажмите «Сохранить».";
    }

    private void DeleteRow()
    {
        if (SelectedRow is null) return;
        SelectedRow.Delete();
        SelectedRow = null;
        Status = "Строка помечена на удаление. Нажмите «Сохранить».";
    }

    private async Task SaveAsync()
    {
        if (CurrentView?.Table is not DataTable table) return;
        await RunAsync(async () =>
        {
            await DatabaseService.SaveTableAsync(SelectedTable.Name, table);
            Status = "Изменения сохранены в транзакции.";
            await LoadCurrentTableCoreAsync();
        });
    }

    private void Sort(bool descending)
    {
        if (CurrentView?.Table is not DataTable table) return;
        var preferred = table.Columns.Cast<DataColumn>()
            .FirstOrDefault(x => x.DataType == typeof(string) && x.ColumnName != "SqlText")
            ?? table.Columns.Cast<DataColumn>().FirstOrDefault(x => x.ColumnName != "ImageData");
        if (preferred is null) return;
        CurrentView.Sort = $"[{preferred.ColumnName}] {(descending ? "DESC" : "ASC")}";
        Status = $"Сортировка по столбцу {preferred.ColumnName} {(descending ? "по убыванию" : "по возрастанию")}.";
    }

    private async Task ShowLowStockAsync()
    {
        await RunAsync(async () =>
        {
            var table = await DatabaseService.GetLowStockAsync(DatabaseService.LowStockLimit);
            CurrentView = table.DefaultView;
            SelectedRow = CurrentView.Count > 0 ? CurrentView[0] : null;
            Status = $"Выполнен асинхронный параметризованный запрос: остаток не более {DatabaseService.LowStockLimit}.";
        });
    }

    private async Task ShowStoredCommandAsync()
    {
        await RunAsync(async () =>
        {
            var table = await DatabaseService.ExecuteStoredCommandAsync("sp_ProductStatistics");
            CurrentView = table.DefaultView;
            SelectedRow = CurrentView.Count > 0 ? CurrentView[0] : null;
            Status = "Выполнена именованная команда sp_ProductStatistics (SQLite-аналог процедуры).";
        });
    }

    private async Task LoadCurrentTableCoreAsync()
    {
        var table = await DatabaseService.LoadTableAsync(SelectedTable.Name);
        CurrentView = table.DefaultView;
        SelectedRow = CurrentView.Count > 0 ? CurrentView[0] : null;
    }

    private async Task RunAsync(Func<Task> action)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await action();
        }
        catch (Exception exception)
        {
            Status = "Ошибка: " + exception.Message;
            MessageBox.Show(exception.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
