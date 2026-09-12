using System.Collections.ObjectModel;
using SportNutritionShop.Models;

namespace SportNutritionShop.Services;

public static class DataService
{
    public static ObservableCollection<Product> LoadProducts() =>
        new(Task.Run(DatabaseService.GetProductsAsync).GetAwaiter().GetResult());

    public static void SaveProducts(IEnumerable<Product> products)
    {
        var snapshot = products.ToList();
        Task.Run(() => DatabaseService.SaveProductsAsync(snapshot)).GetAwaiter().GetResult();
    }

    public static void DeleteProduct(int id) =>
        Task.Run(() => DatabaseService.DeleteProductAsync(id)).GetAwaiter().GetResult();

    public static void CreateOrder(string login, IEnumerable<CartItem> items)
    {
        var snapshot = items.ToList();
        Task.Run(() => DatabaseService.CreateOrderAsync(login, snapshot)).GetAwaiter().GetResult();
    }
}
