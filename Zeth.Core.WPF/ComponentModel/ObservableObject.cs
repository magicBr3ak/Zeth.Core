using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Validation;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion

        #region Properties
        public abstract bool HasErrors { get; }
        #endregion

        #region Methods
        public abstract IEnumerable GetErrors(string propertyName);
        public void OnDataError(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ValidateProperty(propertyName);
        }
        public void OnPropertyChanged(params string[] propertyNameArray)
        {
            foreach (var propertyName in propertyNameArray)
            {
                OnPropertyChanged(propertyName);
            }
        }
        public bool SetProperty<T>(T newValue, ref T oldValue, [CallerMemberName] string propertyName = "")
        {
            if (Equals(newValue, oldValue)) return false;

            oldValue = newValue;
            OnPropertyChanged(propertyName);

            return true;
        }
        public abstract void ValidateProperty(string propertyName);
        public abstract void ValidateProperties();
        #endregion
    }

    public abstract class ObservableObject<V> : ObservableObject where V : ObservableObject<V>
    {
        #region Static
        public static Dictionary<string, RuleCollection<V>> Rules { get; private set; }
        #endregion

        #region Properties
        public Dictionary<string, List<object>> Errors { get; private set; }
        public override bool HasErrors => Errors.Any(x => x.Value != null && x.Value.Count > 0);
        #endregion

        #region Methods
        public override void ValidateProperty(string propertyName)
        {
            if (Rules.ContainsKey(propertyName))
            {
                Errors[propertyName] = Rules[propertyName].HasError((V)this, out var errorList) ? errorList : null;
                OnPropertyChanged("HasError");
            }
        }
        public override void ValidateProperties()
        {
            foreach(var rule in Rules)
            {
                Errors[rule.Key] = rule.Value.HasError((V)this, out var errorList) ? errorList : null;
            }
            OnPropertyChanged("HasError");
        }
        public override IEnumerable GetErrors(string propertyName)
        {
            var errors = default(IEnumerable);

            if (!string.IsNullOrEmpty(propertyName) && Errors.ContainsKey(propertyName))
            {
                errors = Errors[propertyName];
            }

            return errors ?? new object[] { };
        }
        #endregion

        #region Constructors
        public ObservableObject()
        {
            Errors = new Dictionary<string, List<object>>();
        }
        static ObservableObject()
        {
            Rules = new Dictionary<string, RuleCollection<V>>();
        }
        #endregion
    }
}
