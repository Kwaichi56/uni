using SportNutritionShop.Helpers;

namespace SportNutritionShop.Models
{
    public class CartItem : BaseViewModel
    {
        private Product _product = new();
        private int _quantity = 1;
        public Product Product { get => _product; set { _product = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); } }
        public int Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); } }
        public decimal Total => Product.FinalPrice * Quantity;
    }
}
