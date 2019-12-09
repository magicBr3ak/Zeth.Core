using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.ObjectModel
{
    public interface IObservableObject : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Methods
        void OnDataError(string propertyName);
        void OnPropertyChanged(params string[] propertyNameArray);
        void OnPropertyChanged(string propertyName);
        bool SetProperty<T>(T newValue, ref T oldValue, [CallerMemberName] string propertyName = "");
        void ValidateProperties();
        void ValidateProperty(string propertyName); 
        #endregion
    }
}