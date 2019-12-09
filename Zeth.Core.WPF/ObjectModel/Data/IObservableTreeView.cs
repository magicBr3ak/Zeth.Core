using System.Collections.ObjectModel;

namespace System.ObjectModel.Data
{
    public interface IObservableTreeView
    {
        string TreeName { get; }
        string TreeImagePath { get; }
        bool TreeFocusable { get; }
        ObservableCollection<object> TreeItems { get; }
    }
}
