using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

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
                var cookie = Request.Cookies[_TokenKey];
                var formatter = default(BinaryFormatter);
                var stream = default(MemoryStream);

                if (Equals(_Token, default(T)) && cookie != null)
                {
                    formatter = new BinaryFormatter();
                    stream = new MemoryStream(CryptoData.DecryptSHA256(Encoding.UTF8.GetBytes(cookie.Value)));

                    _Token = (T)formatter.Deserialize(stream);
                }

                return _Token;
            }
            set
            {
                var cookie = default(HttpCookie);
                var formatter = default(BinaryFormatter);
                var stream = default(MemoryStream);

                if (Equals(value, default(T)))
                {
                    cookie = Request.Cookies[_TokenKey];

                    if (cookie != null)
                    {
                        cookie.Expires = DateTime.Now.AddDays(-1);
                    }
                }
                else
                {
                    cookie = new HttpCookie(_TokenKey);
                    formatter = new BinaryFormatter();
                    stream = new MemoryStream();

                    formatter.Serialize(stream, value);

                    cookie.Expires = DateTime.Now.AddMonths(1);
                    cookie.Value = Encoding.UTF8.GetString(CryptoData.EncryptSHA256(stream.ToArray()));

                    stream.Dispose();
                }

                if (cookie != null) Response.Cookies.Add(cookie);
            }
        }
        #endregion
    }
}
