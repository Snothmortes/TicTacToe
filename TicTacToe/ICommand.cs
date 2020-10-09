namespace Game
{
    public interface ICommand
    {
        void ExecuteAction();
        void UndoAction();
        void RedoAction();
    }

}
