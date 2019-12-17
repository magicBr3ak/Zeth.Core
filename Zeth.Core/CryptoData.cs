using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Zeth.Core
{
    public static class CryptoData
    {
        public static byte[] DES_KEY = Encoding.UTF8.GetBytes("asdasd");
        public static byte[] SHA256_KEY = Encoding.UTF8.GetBytes("asdasd");
        public static byte[] SHA256_SALT = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static byte[] EncryptTripleDES(byte[] dataValue)
        {
            var cryptoMD5 = new MD5CryptoServiceProvider();
            var dataKey = cryptoMD5.ComputeHash(DES_KEY);
            var cryptoTDES = new TripleDESCryptoServiceProvider();
            var dataResult = default(byte[]);

            cryptoMD5.Clear();

            cryptoTDES.Key = dataKey;
            cryptoTDES.Mode = CipherMode.ECB;
            cryptoTDES.Padding = PaddingMode.PKCS7;

            dataResult = cryptoTDES.CreateEncryptor().TransformFinalBlock(dataValue, 0, dataValue.Length);

            cryptoTDES.Clear();

            return dataResult;
        }
        public static byte[] DecryptTripleDES(byte[] dataValue)
        {
            var cryptoMD5 = new MD5CryptoServiceProvider();
            var dataKey = cryptoMD5.ComputeHash(DES_KEY);
            var cryptoTDES = new TripleDESCryptoServiceProvider();
            var dataResult = default(byte[]);

            cryptoMD5.Clear();

            cryptoTDES.Key = dataKey;
            cryptoTDES.Mode = CipherMode.ECB;
            cryptoTDES.Padding = PaddingMode.PKCS7;

            dataResult = cryptoTDES.CreateDecryptor().TransformFinalBlock(dataValue, 0, dataValue.Length);

            cryptoTDES.Clear();

            return dataResult;
        }    
        public static string EncryptTripleDES(string dataValue)
        {
            return Encoding.UTF8.GetString(EncryptTripleDES(Encoding.UTF8.GetBytes(dataValue)));
        }
        public static string DecryptTripleDES(string dataValue)
        {
            return Encoding.UTF8.GetString(DecryptTripleDES(Encoding.UTF8.GetBytes(dataValue)));
        }
    
        public static byte[] EncryptSHA256(byte[] dataValue)
        {
            var key = default(Rfc2898DeriveBytes);
            var dataResult = default(byte[]);

            using (var stream = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    key = new Rfc2898DeriveBytes(SHA256_KEY, SHA256_SALT, 1000);

                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;

                    using (var cryptoStream = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(dataValue, 0, dataValue.Length);
                        cryptoStream.Close();
                    }

                    dataResult = stream.ToArray();
                }
            }

            return dataResult;
        }
        public static byte[] DecryptSHA256(byte[] dataValue)
        {
            var key = default(Rfc2898DeriveBytes);
            var dataResult = default(byte[]);

            using (var stream = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    key = new Rfc2898DeriveBytes(SHA256_KEY, SHA256_SALT, 1000);

                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;

                    using (var cryptoStream = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(dataValue, 0, dataValue.Length);
                        cryptoStream.Close();
                    }

                    dataResult = stream.ToArray();
                }
            }

            return dataResult;
        }
        public static string EncryptSHA256(string dataValue)
        {
            return Encoding.UTF8.GetString(EncryptSHA256(Encoding.UTF8.GetBytes(dataValue)));
        }
        public static string DecryptSHA256(string dataValue)
        {
            return Encoding.UTF8.GetString(DecryptSHA256(Encoding.UTF8.GetBytes(dataValue)));
        }
    }
}
