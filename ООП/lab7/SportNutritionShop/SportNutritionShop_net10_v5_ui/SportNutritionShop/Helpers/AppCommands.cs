using System.Windows.Input;

namespace SportNutritionShop.Helpers
{
   
    public static class AppCommands
    {
        public static readonly RoutedUICommand SaveCartCommand =
            new RoutedUICommand(
                "Сохранить корзину",
                "SaveCart",
                typeof(AppCommands),
                new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control) });
    }
}
