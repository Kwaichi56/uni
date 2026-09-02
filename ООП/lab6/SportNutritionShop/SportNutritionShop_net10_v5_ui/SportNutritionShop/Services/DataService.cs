using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using SportNutritionShop.Models;

namespace SportNutritionShop.Services
{
    public static class DataService
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "products.json");

        public static ObservableCollection<Product> LoadProducts()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                    var seed = GetSeedProducts();
                    SaveProducts(seed);
                    return seed;
                }
                var json = File.ReadAllText(FilePath);
                var data = JsonSerializer.Deserialize<ObservableCollection<Product>>(json);
                return data ?? new ObservableCollection<Product>();
            }
            catch
            {
                return GetSeedProducts();
            }
        }

        public static void SaveProducts(IEnumerable<Product> products)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(products, options);
            File.WriteAllText(FilePath, json);
        }

        private static ObservableCollection<Product> GetSeedProducts()
        {
            return new ObservableCollection<Product>
            {
                new Product { Id = 1, ShortName = "Creatine Monohydrate", FullName = "OstroVit Creatine Monohydrate 300 g", Description = "Классический креатин для роста силы и восстановления.", Category = "Креатин", Brand = "OstroVit", Country = "Польша", Flavor = "Без вкуса", Weight = "300 г", Price = 48, DiscountPercent = 0, Quantity = 14, Rating = 4.8, SoldCount = 121, ImagePath = "Assets/Products/creatine.jpg" },
                new Product { Id = 2, ShortName = "Whey Protein", FullName = "BioTech USA 100% Pure Whey 1000 g", Description = "Сывороточный белок для набора и сохранения мышечной массы.", Category = "Протеин", Brand = "BioTech USA", Country = "США", Flavor = "Chocolate", Weight = "1000 г", Price = 99, DiscountPercent = 10, Quantity = 8, Rating = 4.9, SoldCount = 250, ImagePath = "Assets/Products/whey.jpg" },
                new Product { Id = 3, ShortName = "BCAA 2:1:1", FullName = "Mutant BCAA 2:1:1 400 g", Description = "Аминокислоты для поддержки мышц и снижения катаболизма.", Category = "Аминокислоты", Brand = "Mutant", Country = "Канада", Flavor = "Orange", Weight = "400 г", Price = 65, DiscountPercent = 5, Quantity = 12, Rating = 4.6, SoldCount = 89, ImagePath = "Assets/Products/bcaa.jpg" },
                new Product { Id = 4, ShortName = "Mass Gainer", FullName = "Optimum Nutrition Serious Mass 2700 g", Description = "Высококалорийный гейнер для набора массы.", Category = "Гейнер", Brand = "Optimum Nutrition", Country = "США", Flavor = "Vanilla", Weight = "2700 г", Price = 170, DiscountPercent = 0, Quantity = 6, Rating = 4.7, SoldCount = 73, ImagePath = "Assets/Products/gainer.jpg" },
                new Product { Id = 5, ShortName = "Multivitamin", FullName = "NOW Foods ADAM Superior Men’s Multi", Description = "Комплекс витаминов и минералов на каждый день.", Category = "Витамины", Brand = "NOW Foods", Country = "США", Flavor = "Neutral", Weight = "90 капсул", Price = 54, DiscountPercent = 0, Quantity = 20, Rating = 4.5, SoldCount = 61, ImagePath = "Assets/Products/vitamins.jpg" },
                new Product { Id = 6, ShortName = "Pre-Workout", FullName = "Bombbar Pre Workout 300 g", Description = "Предтренировочный комплекс для энергии и концентрации.", Category = "Предтрен", Brand = "Bombbar", Country = "Россия", Flavor = "Berry", Weight = "300 г", Price = 59, DiscountPercent = 0, Quantity = 10, Rating = 4.4, SoldCount = 97, ImagePath = "Assets/Products/preworkout.jpg" }
            };
        }
    }
}
