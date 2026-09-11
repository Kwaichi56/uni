using System.Collections.Generic;

namespace SportNutritionShop.Helpers
{
    public class UndoRedoManager
    {
        private readonly Stack<IUndoableAction> _undoStack = new();
        private readonly Stack<IUndoableAction> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
        public event EventHandler? HistoryChanged;

        public void ExecuteAction(IUndoableAction action)
        {
            action.Do();
            _undoStack.Push(action);
            _redoStack.Clear();
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            if (!CanUndo) return;
            var action = _undoStack.Pop();
            action.Undo();
            _redoStack.Push(action);
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Redo()
        {
            if (!CanRedo) return;
            var action = _redoStack.Pop();
            action.Do();
            _undoStack.Push(action);
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
