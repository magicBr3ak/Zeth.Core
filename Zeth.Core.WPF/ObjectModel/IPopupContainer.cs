namespace System.ObjectModel
{
    public interface IPopupContainer<T> : IObservableObject where T : IPopupContainer<T>
    {
        bool IsPopupVisible { get; set; }
        object PopupContent { get; set; }
        PopupController<T> PopupController { get; }
    }
}
