namespace SportNutritionShop.Helpers
{
    public interface IUndoableAction
    {
        void Do();
        void Undo();
    }
}
