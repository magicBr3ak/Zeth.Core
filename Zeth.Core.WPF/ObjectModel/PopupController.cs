using System.Collections.Generic;
using System.Timers;

namespace System.ObjectModel
{
    public class PopupController<T> : IPopupController where T : IPopupContainer<T>
    {
        #region Properties
        public PopupState State { get; private set; }
        public int DelayMillis { get; set; }
        public T View { get; private set; }
        public Stack<PopupReference> PopupContainer { get; private set; }
        protected Timer PopupTimer { get; private set; }
        #endregion

        #region Methods
        protected void Start()
        {
            PopupTimer.Interval = DelayMillis;
            PopupTimer.Start();
        }
        protected void PopupTimerTick(object sender, ElapsedEventArgs e)
        {
            var currentPopup = default(PopupReference);
            var previousPopup = default(PopupReference);

            PopupTimer.Stop();

            switch (State)
            {
                case PopupState.ON_CLOSE:
                    currentPopup = PopupContainer.Pop();
                    State = PopupState.AFTER_CLOSE;

                    currentPopup.OnClose?.Invoke(currentPopup.Content);
                    if (PopupContainer.Count > 0) Show();
                    break;
                case PopupState.ON_HIDE:
                    currentPopup = PopupContainer.Peek();
                    State = PopupState.AFTER_HIDE;

                    currentPopup.OnHide?.Invoke(currentPopup.Content);
                    break;
                case PopupState.ON_SHOW:
                    currentPopup = PopupContainer.Peek();
                    State = PopupState.AFTER_SHOW;

                    currentPopup.OnShow?.Invoke(currentPopup.Content);
                    break;
                case PopupState.ON_REPLACE:
                    currentPopup = PopupContainer.Pop();
                    previousPopup = PopupContainer.Peek();
                    State = PopupState.AFTER_HIDE;

                    previousPopup.OnHide?.Invoke(previousPopup.Content);
                    PopupContainer.Push(currentPopup);
                    State = PopupState.ON_SHOW;
                    Show(currentPopup.Content);
                    Start();
                    break;
            }
        }
        protected void Clear()
        {
            View.IsPopupVisible = false;
            View.PopupContent = null;

            View.OnPropertyChanged("IsPopupVisible");
            View.OnPropertyChanged("PopupContent");
        }
        protected void Show(object content)
        {
            View.IsPopupVisible = true;
            View.PopupContent = content;

            View.OnPropertyChanged("IsPopupVisible");
            View.OnPropertyChanged("PopupContent");
        }
        public PopupReference Create(object content)
        {
            return new PopupReference(content);
        }
        public void Show(PopupReference popup = null)
        {
            if (PopupContainer.Count > 0)
            {
                if (popup == null)
                {
                    State = PopupState.ON_SHOW;
                    Show(PopupContainer.Peek().Content);
                    Start();
                }
                else
                {
                    State = PopupState.ON_REPLACE;
                    PopupContainer.Push(popup);
                    Clear();
                    Start();
                }
            }
            else if (popup != null)
            {
                State = PopupState.ON_SHOW;
                PopupContainer.Push(popup);
                Show(popup.Content);
                Start();
            }
        }
        public void Hide()
        {
            if (PopupContainer.Count > 0)
            {
                State = PopupState.ON_HIDE;
                Clear();
                Start();
            }
        }
        public void Close()
        {
            if (PopupContainer.Count > 0)
            {
                State = PopupState.ON_CLOSE;
                Clear();
                Start();
            }
        }
        #endregion

        #region Constructors
        public PopupController(T view)
        {
            View = view;
            PopupContainer = new Stack<PopupReference>();
            PopupTimer = new Timer();
            DelayMillis = 150;

            PopupTimer.Elapsed += PopupTimerTick;
        }

        #endregion
    }
}
