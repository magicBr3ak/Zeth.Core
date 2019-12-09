using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Zeth.Core
{
    public static class CryptoData
    {
        internal static readonly byte[] SHA256_SALT_BYTES = new byte[] { 231, 123, 213, 189, 145, 64, 34, 99 };

        public static byte[] SHA256Encrypt(byte[] dataBytes, byte[] keyBytes)
        {
            var encryptedBytes = default(byte[]);

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var rfcKey = new Rfc2898DeriveBytes(keyBytes, SHA256_SALT_BYTES, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = rfcKey.GetBytes(AES.KeySize / 8);
                    AES.IV = rfcKey.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataBytes, 0, dataBytes.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public static string SHA256Encrypt(this string text, string key)
        {
            var dataBytes = Encoding.UTF8.GetBytes(text);
            var keyBytes = Encoding.UTF8.GetBytes(key);

            keyBytes = SHA256.Create().ComputeHash(keyBytes);

            return Convert.ToBase64String(SHA256Encrypt(dataBytes, keyBytes));
        }

        public static byte[] SHA256Decrypt(byte[] dataBytes, byte[] keyBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(keyBytes, SHA256_SALT_BYTES, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataBytes, 0, dataBytes.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        public static string SHA256Decrypt(this string text, string key)
        {
            var dataBytes = Convert.FromBase64String(text);
            var keyBytes = Encoding.UTF8.GetBytes(key);

            keyBytes = SHA256.Create().ComputeHash(keyBytes);

            return Encoding.UTF8.GetString(SHA256Decrypt(dataBytes, keyBytes));
        }
    }
}
