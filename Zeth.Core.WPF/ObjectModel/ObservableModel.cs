using System.Linq;
using System.Runtime.CompilerServices;

namespace System.ObjectModel
{
    public abstract class ObservableModel<V, M> : ObservableObject<V> where V : ObservableModel<V, M>
    {
        #region Variables
        private M _Model;
        #endregion

        #region Properties
        public M Model
        {
            get { return _Model; }
            set
            {
                if (SetProperty(value, ref _Model)) OnModelChanged();
            }
        }
        #endregion

        #region Methods
        public T ModelGet<T>([CallerMemberName] string propertyName = "")
        {
            var propertyInfo = typeof(M).GetProperty(propertyName);
            var propertyValue = propertyInfo.GetValue(Model);

            if (propertyInfo.PropertyType.IsValueType || propertyValue != null) return (T)propertyValue;
            else return default;
        }
        public void ModelSet<T>(T value, [CallerMemberName] string propertyName = "")
        {
            var propertyInfo = typeof(M).GetProperty(propertyName);

            propertyInfo.SetValue(Model, value);
        }
        public void OnModelChanged()
        {
            var modelPropertiesInfo = typeof(M).GetProperties().OrderBy(x => x.Name).ToArray();
            var viewType = typeof(V);

            foreach(var modelPropertyInfo in modelPropertiesInfo)
            {
                if (viewType.GetProperty(modelPropertyInfo.Name) != null)
                {
                    OnPropertyChanged(modelPropertyInfo.Name);
                }
            }
        }
        #endregion

        #region Constructors
        public ObservableModel(M item)
        {
            Model = item;
        }
        #endregion
    }
}
