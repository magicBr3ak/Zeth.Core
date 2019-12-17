using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

namespace Zeth.Core.Web
{
    public static class ControllerExtension
    {
        public static T GetToken<T>(this System.Web.Mvc.Controller controller, string tokenKey = null)
        {
            var cookie = controller.Request.Cookies[tokenKey ?? Controller.TOKEN_KEY];
            var formatter = default(BinaryFormatter);
            var stream = default(MemoryStream);
            var token = default(T);

            if (cookie != null)
            {
                formatter = new BinaryFormatter();
                stream = new MemoryStream(CryptoData.DecryptSHA256(Convert.FromBase64String(cookie.Value)));

                token = (T)formatter.Deserialize(stream);
            }

            return token;
        }
        public static void SetToken<T>(this System.Web.Mvc.Controller controller, T value, string tokenKey = null)
        {
            var cookie = default(HttpCookie);
            var formatter = default(BinaryFormatter);
            var stream = default(MemoryStream);

            if (Equals(value, default(T)))
            {
                cookie = controller.Request.Cookies[tokenKey ?? Controller.TOKEN_KEY];

                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                }
            }
            else
            {
                cookie = new HttpCookie(tokenKey ?? Controller.TOKEN_KEY);
                formatter = new BinaryFormatter();
                stream = new MemoryStream();

                formatter.Serialize(stream, value);

                cookie.Expires = DateTime.Now.AddMonths(1);
                cookie.Value = Convert.ToBase64String(CryptoData.EncryptSHA256(stream.ToArray()));

                stream.Dispose();
            }

            if (cookie != null) controller.Response.Cookies.Add(cookie);
        }
    }
}
