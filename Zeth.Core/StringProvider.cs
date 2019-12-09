namespace Zeth.Core
{
    public abstract class StringProvider
    {
        #region Classes
        public class ConfigProvider : StringProvider
        {
            public string Key { get; private set; }

            public override string GetPath()
            {
                return Core.Key.GetConfigValue(Key);
            }

            public ConfigProvider(string key)
            {
                Key = key;
            }
        }
        public class RegistryProvider : ConfigProvider
        {
            public string Value { get; private set; }
            public override string GetPath()
            {
                return (string)Core.Key.GetRegistryValue(Key, Value);
            }

            public RegistryProvider(string key, string value) : base(key)
            {
                Value = value;
            }
        }
        public class ConstantProvider : StringProvider
        {
            public string Path { get; private set; }

            public override string GetPath()
            {
                return Path;
            }

            public ConstantProvider(string path)
            {
                Path = path;
            }
        }
        #endregion

        #region Methods
        public abstract string GetPath();
        #endregion
    }
}
