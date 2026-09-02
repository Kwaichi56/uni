using System;

namespace SportNutritionShop.Helpers
{
    public class UndoableAction : IUndoableAction
    {
        private readonly Action _doAction;
        private readonly Action _undoAction;
        public string Description { get; }

        public UndoableAction(string description, Action doAction, Action undoAction)
        {
            Description = description;
            _doAction = doAction;
            _undoAction = undoAction;
        }

        public void Do() => _doAction();
        public void Undo() => _undoAction();
    }
}
