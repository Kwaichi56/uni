namespace SportNutritionShop.Models
{
    /// <summary>
    /// Сохранённый снимок корзины.
    /// </summary>
    public class SavedCart
    {
        public string Name { get; set; } = "";
        public List<CartItem> Items { get; set; } = new();

        public override string ToString() => Name;
    }
}
