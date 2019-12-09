using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
    public abstract class ObservableModel<V, M> : ObservableObject<V> where V : ObservableModel<V, M>
    {

        #region Properties
        public M Model { get; set; }
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
        #endregion

        #region Constructors
        public ObservableModel(M item)
        {

        }
        #endregion
    }
}
