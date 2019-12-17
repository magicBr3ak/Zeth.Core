using System.Windows.Input;

namespace System.ObjectModel.Data
{
    public class ObservableMenuItem : ObservableObject<ObservableMenuItem>, IObservableMenuItem
    {
        #region Variables
        private string _MenuName;
        private int _MenuType;
        private Command _MenuAction;
        #endregion

        #region Properties
        public string MenuName
        {
            get { return _MenuName; }
            set { SetProperty(value, ref _MenuName); }
        }
        public int MenuType
        {
            get { return _MenuType; }
            set { SetProperty(value, ref _MenuType); }
        }
        public Command MenuAction
        {
            get { return _MenuAction; }
        }

        ICommand IObservableMenuItem.MenuAction
        {
            get { return MenuAction; }
        }
        #endregion

        #region Constructors
        public ObservableMenuItem(string menuName, Command menuAction, int menuType = 0)
        {
            _MenuName = menuName;
            _MenuAction = menuAction;
            _MenuType = menuType;
        }
        public ObservableMenuItem(string menuName, Action<object> menuAction, int menuType = 0) : this(menuName, new Command(menuAction), menuType)
        {

        }
        #endregion
    }
}
