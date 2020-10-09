using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class CommandManager
    {
        private Stack<ICommand> _undo;
        private Stack<ICommand> _redo;
        private ICommand _command;
        public bool undoEnabled { get; private set; }
        public bool redoEnabled { get; private set; }

        public CommandManager()
        {
            _undo = new Stack<ICommand>();
            _redo = new Stack<ICommand>();
        }

        public void SetCommand(ICommand command) => _command = command;

        public void Invoke()
        {
            _undo.Push(_command);
            _command.ExecuteAction();
            _redo.Clear();
            undoEnabled = true;
        }

        public void Reset()
        {
            foreach (var command in Enumerable.Reverse(_undo))
                command.UndoAction();
            _undo.Clear();
            _redo.Clear();
            undoEnabled = false;
            redoEnabled = false;
        }

        public void UndoCommand()
        {
            if (_undo.Count != 0)
            {
                var cmd = _undo.Pop();
                cmd.UndoAction();
                _redo.Push(cmd);
                redoEnabled = true;
            }
            if (_undo.Count == 0)
                undoEnabled = false;
        }

        public void RedoCommand()
        {
            if (_redo.Count != 0)
            {
                var cmd = _redo.Pop();
                cmd.RedoAction();
                _undo.Push(cmd);
                undoEnabled = true;
            }
            if (_redo.Count == 0)
                redoEnabled = false;
        }
    }
}