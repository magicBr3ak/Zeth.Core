using System.Windows.Input;

namespace System.ObjectModel.Data
{
    public interface IObservableMenuItem
    {
        string MenuName { get; }
        int MenuType { get; }
        ICommand MenuAction { get; }
    }
}