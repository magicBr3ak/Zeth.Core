using System.Collections.Generic;

namespace System.ObjectModel
{
    public interface IPopupController
    {
        int DelayMillis { get; set; }
        Stack<PopupReference> PopupContainer { get; }
        PopupState State { get; }

        void Close();
        PopupReference Create(object content);
        void Hide();
        void Show(PopupReference popup = null);
    }
}