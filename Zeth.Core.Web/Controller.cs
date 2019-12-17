namespace Zeth.Core.Web
{
    public class Controller : System.Web.Mvc.Controller
    {
        public static string TOKEN_KEY = "emV0aHRva2Vua2V5";
    }
    public class Controller<T> : Controller
    {
        #region Variables
        private T _Token = default;
        private string _TokenKey = TOKEN_KEY;
        #endregion

        #region Properties
        public T Token
        {
            get
            {
                if (Equals(_Token, default(T))) _Token = this.GetToken<T>(_TokenKey);

                return _Token;
            }
            set
            {
                this.SetToken<T>(value, _TokenKey);
            }
        }
        #endregion
    }
}
