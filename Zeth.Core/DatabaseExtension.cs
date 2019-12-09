using System;
using System.Data;
using System.Data.Common;

namespace Zeth.Core
{
    public static class DatabaseExtension
    {
        public static DbConnection WithConfigString(this DbConnection Connection, string Key, bool Open = true)
        {
            Connection.ConnectionString = Key.GetConfigValue();

            if (Open) Connection.Open();

            return Connection;
        }
        public static DbConnection TryClose(this DbConnection Connection)
        {
            if (Connection == null || Connection.State == System.Data.ConnectionState.Closed) return Connection;

            Connection.Close();

            return Connection;
        }
    
        public static DbCommand CreateProcedure(this DbConnection Connection, string ProcedureName)
        {
            var Command = Connection.CreateCommand();

            Command.CommandType = System.Data.CommandType.StoredProcedure;
            Command.CommandText = ProcedureName;

            return Command;
        }
        public static DbCommand WithParameter(this DbCommand Command, string ParameterName, object value)
        {
            var Parameter = Command.CreateParameter();

            Parameter.ParameterName = ParameterName;
            Parameter.Value = value;
            Parameter.Direction = System.Data.ParameterDirection.Input;

            Command.Parameters.Add(Parameter);

            return Command;
        }
        public static DbCommand WithParameter(this DbCommand Command, string ParameterName, DbType Type)
        {
            var Parameter = Command.CreateParameter();

            Parameter.ParameterName = ParameterName;
            Parameter.DbType = Type;
            Parameter.Direction = System.Data.ParameterDirection.Output;

            Command.Parameters.Add(Parameter);

            return Command;
        }
        public static DbCommand WithParameter<T>(this DbCommand Command, string ParameterName, Action<T> TypeSetter) where T : DbParameter
        {
            var Parameter = Command.CreateParameter();

            TypeSetter((T)Parameter);

            Parameter.ParameterName = ParameterName;
            Parameter.Direction = System.Data.ParameterDirection.Output;

            Command.Parameters.Add(Parameter);

            return Command;
        }
    
        public static object NullableValue(this string value)
        {
            return string.IsNullOrEmpty(value) ? DBNull.Value : (object)value;
        }
    }
}
