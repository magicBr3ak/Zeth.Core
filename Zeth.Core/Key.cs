using Microsoft.Win32;

namespace Zeth.Core
{
    public static class Key
    {
        public static object GetRegistryValue(this string key, string value)
        {
            var registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

            return (registryKey.OpenSubKey(key).GetValue(value) ?? string.Empty).ToString();
        }
        public static string GetConfigValue(this string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}
