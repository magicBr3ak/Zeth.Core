using System.Collections.Generic;

namespace System.ObjectModel.Data
{
    public interface IObservableMenu : IObservableMenuItem
    {
        IEnumerable<IObservableMenuItem> MenuItems { get; }
    }
}