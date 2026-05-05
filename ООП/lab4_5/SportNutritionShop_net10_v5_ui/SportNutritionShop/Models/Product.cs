using SportNutritionShop.Helpers;

namespace SportNutritionShop.Models
{
    public class Product : BaseViewModel
    {
        private string _shortName = "";
        private string _fullName = "";
        private string _description = "";
        private string _category = "";
        private string _brand = "";
        private string _country = "";
        private string _flavor = "";
        private string _imagePath = "";
        private decimal _price;
        private decimal _discountPercent;
        private int _quantity;
        private double _rating;
        private int _soldCount;
        private string _weight = "";

        public int Id { get; set; }
        public string ShortName { get => _shortName; set { _shortName = value; OnPropertyChanged(); } }
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
        public string Category { get => _category; set { _category = value; OnPropertyChanged(); } }
        public string Brand { get => _brand; set { _brand = value; OnPropertyChanged(); } }
        public string Country { get => _country; set { _country = value; OnPropertyChanged(); } }
        public string Flavor { get => _flavor; set { _flavor = value; OnPropertyChanged(); } }
        public string Weight { get => _weight; set { _weight = value; OnPropertyChanged(); } }
        public string ImagePath { get => _imagePath; set { _imagePath = value; OnPropertyChanged(); } }
        public decimal Price { get => _price; set { _price = value; OnPropertyChanged(); OnPropertyChanged(nameof(FinalPrice)); } }
        public decimal DiscountPercent { get => _discountPercent; set { _discountPercent = value; OnPropertyChanged(); OnPropertyChanged(nameof(FinalPrice)); } }
        public int Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsOutOfStock)); } }
        public double Rating { get => _rating; set { _rating = value; OnPropertyChanged(); } }
        public int SoldCount { get => _soldCount; set { _soldCount = value; OnPropertyChanged(); } }
        public bool IsOutOfStock => Quantity <= 0;
        public decimal FinalPrice => Price - (Price * DiscountPercent / 100m);
    }
}
