namespace System.ObjectModel
{
    public class PopupReference
    {
        #region Properties
        public IPopupController Controller { get; private set; }
        public object Content { get; private set; }
        public Action<object> OnClose { get; set; }
        public Action<object> OnHide { get; set; }
        public Action<object> OnShow { get; set; }
        #endregion

        #region Methods
        public void Show()
        {
            if (Controller.PopupContainer.Count == 0) Controller.Show(this);
            else if (Controller.PopupContainer.Peek() == this) Controller.Show();
            else if (!Controller.PopupContainer.Contains(this)) Controller.Show(this);
        }
        public void Hide()
        {
            if (Controller.PopupContainer.Count > 0 && Controller.PopupContainer.Peek() == this) Controller.Hide();
        }
        public void Close()
        {
            if (Controller.PopupContainer.Count > 0 && Controller.PopupContainer.Peek() == this) Controller.Close();
        }
        public T GetContent<T>()
        {
            return (T)Content;
        }
        #endregion

        #region Constructors
        public PopupReference(object content)
        {
            Content = content;
        } 
        #endregion
    }
}
