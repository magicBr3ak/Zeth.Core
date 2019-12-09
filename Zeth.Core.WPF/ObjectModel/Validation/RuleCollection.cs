using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.ObjectModel.Validation
{
    public class RuleCollection<T>
    {
        #region Properties
        protected Collection<Func<T, bool>> HasErrorCollection { get; private set; }
        protected Collection<Func<T, object>> GetErrorCollection { get; private set; }
        #endregion

        #region Methods
        public void Add(Func<T, bool> hasErrorFunction, Func<T, object> getErrorFunction)
        {
            HasErrorCollection.Add(hasErrorFunction);
            GetErrorCollection.Add(getErrorFunction);
        }
        public bool HasError(T item, out List<object> errorList)
        {
            errorList = new List<object>();

            for (var i = 0; i < HasErrorCollection.Count; i++)
            {
                if (HasErrorCollection[i](item))
                {
                    errorList.Add(GetErrorCollection[i](item));
                }
            }

            return errorList.Count > 0;
        }
        #endregion
    }
}
