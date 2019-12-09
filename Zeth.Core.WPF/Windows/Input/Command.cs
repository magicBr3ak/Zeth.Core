namespace System.Windows.Input
{
    public class Command : ICommand
    {
        #region Events
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Variables
        private static readonly Func<object, bool> _CanExecuteDefault = (x) => true;

        private Func<object, bool> _CanExecute;
        private Action<object> _Execute;
        #endregion

        #region Methods
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
        public bool CanExecute(object parameter)
        {
            return _CanExecute(parameter);
        }
        public void Execute(object parameter)
        {
            _Execute(parameter);
        }
        #endregion

        #region Constructors
        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _Execute = execute;
            _CanExecute = canExecute ?? _CanExecuteDefault;
        }
        #endregion
    }
}
