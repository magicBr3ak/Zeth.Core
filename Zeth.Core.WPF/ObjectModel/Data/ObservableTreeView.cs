using System.Collections.ObjectModel;

namespace System.ObjectModel.Data
{
    public class ObservableTreeView : ObservableObject<ObservableTreeView>, IObservableTreeView
    {
        #region Variables
        private string _TreeName = string.Empty;
        private string _TreemagePath = string.Empty;
        private bool _TreeFocusable = false;
        #endregion

        #region Properties
        public string TreeName
        {
            get { return _TreeName; }
            set { SetProperty(value, ref _TreeName); }
        }
        public string TreeImagePath
        {
            get { return _TreemagePath; }
            set { SetProperty(value, ref _TreemagePath); }
        }
        public bool TreeFocusable
        {
            get { return _TreeFocusable; }
            set { SetProperty(value, ref _TreeFocusable); }
        }
        public ObservableCollection<object> TreeItems { get; private set; }
        #endregion

        #region Constructors
        public ObservableTreeView()
        {
            TreeItems = new ObservableCollection<object>();
        }
        #endregion
    }
}
