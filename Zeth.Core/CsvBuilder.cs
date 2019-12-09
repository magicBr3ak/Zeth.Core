using System;
using System.Data.Common;
using System.Text;

namespace Zeth.Core
{
    public static class CsvBuilder
    {
        public const string BLOCK_SEPARATOR = @"\^";
        public const string ROW_SEPARATOR = @"\¬";
        public const string COL_SEPARATOR = @"\|";

        public const string BLOCK_REPLACE = @"%^";
        public const string ROW_REPLACE = @"%¬";
        public const string COL_REPLACE = @"%|";

        public static DateTime FromJsTime(this string dateString)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(long.Parse(dateString)).ToLocalTime();
        }

        public static void AddCsvObject(this StringBuilder stringBuilder, object cellValue)
        {
            stringBuilder.Append(cellValue);
            stringBuilder.Append(COL_SEPARATOR);
        }
        public static void AddCsvString(this StringBuilder stringBuilder, object cellValue)
        {
            stringBuilder.AppendCsvString((string)cellValue);
            stringBuilder.Append(COL_SEPARATOR);
        }
        public static void NewRow(this StringBuilder stringBuilder)
        {
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(ROW_SEPARATOR);
        }
        public static void NewBlock(this StringBuilder stringBuilder)
        {
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(BLOCK_SEPARATOR);
        }
        public static void EndBlock(this StringBuilder stringBuilder)
        {
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
        }

        private static void AppendCsvString(this StringBuilder stringBuilder, string value)
        {
            var stringBuilderLength = stringBuilder.Length;
            var valueLength = value.Length;

            stringBuilder.Append(value);

            stringBuilder.Replace(BLOCK_SEPARATOR, BLOCK_REPLACE, stringBuilderLength, valueLength);
            stringBuilder.Replace(ROW_SEPARATOR, ROW_REPLACE, stringBuilderLength, valueLength);
            stringBuilder.Replace(COL_SEPARATOR, COL_REPLACE, stringBuilderLength, valueLength);
        }

        private static string DataReaderToDefault(DbDataReader dataReader, int position)
        {
            return dataReader.IsDBNull(position) ? string.Empty : dataReader.GetValue(position).ToString();
        }
        private static string DataReaderToString(DbDataReader dataReader, int position)
        {
            return dataReader.IsDBNull(position) ? string.Empty : dataReader.GetString(position);
        }
        private static string DataReaderToDateTime(DbDataReader dataReader, int position)
        {
            return dataReader.IsDBNull(position) ? string.Empty : dataReader.GetDateTime(position).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();
        }
        private static string DataReaderToBoolean(DbDataReader dataReader, int position)
        {
            return dataReader.IsDBNull(position) ? string.Empty : (dataReader.GetBoolean(position) ? "1" : "0");
        }

        private static Func<DbDataReader, int, string> GetDataTypeParser(string dataTypeName)
        {
            switch (dataTypeName.ToLower())
            {
                case "varchar":
                case "char":
                case "nvarchar":
                case "nchar":
                    return DataReaderToString;
                case "bit":
                    return DataReaderToBoolean;
                case "date":
                case "datetime":
                    return DataReaderToDateTime;
                default:
                    return DataReaderToDefault;
            }
        }
        private static Action<StringBuilder, object> GetCsvAppend(string dataTypeName)
        {
            switch (dataTypeName.ToLower())
            {
                case "varchar":
                case "char":
                case "nvarchar":
                case "nchar":
                    return AddCsvString;
                default:
                    return AddCsvObject;
            }
        }

        public static void AddDataReader(this StringBuilder stringBuilder, DbDataReader dataReader, string tableName, params CsvBuilderHeaderData[] headerArray)
        {
            var dataParserArray = new Func<DbDataReader, int, object>[dataReader.FieldCount];
            var csvAppendArray = new Action<StringBuilder, object>[dataReader.FieldCount];

            #region Nombre
            stringBuilder.AppendCsvString(tableName);
            stringBuilder.Append(BLOCK_SEPARATOR);
            #endregion

            #region OtrasCabecerasNombres
            if (headerArray.Length > 0)
            {
                foreach (var header in headerArray) stringBuilder.AddCsvString(header.HeaderName);

                stringBuilder.NewBlock();
            }
            else stringBuilder.Append(BLOCK_SEPARATOR);
            #endregion

            #region Cabeceras
            for (var i = 0; i < dataReader.FieldCount; i++) stringBuilder.AddCsvString(dataReader.GetName(i));

            stringBuilder.NewRow();
            for (var i = 0; i < dataReader.FieldCount; i++)
            {
                stringBuilder.AddCsvString(dataReader.GetDataTypeName(i));
                dataParserArray[i] = GetDataTypeParser(dataReader.GetDataTypeName(i));
                csvAppendArray[i] = GetCsvAppend(dataReader.GetDataTypeName(i));
            }
            #endregion

            #region OtrasCabecerasData
            if (headerArray != null && headerArray.Length > 0)
            {
                foreach(var header in headerArray)
                {
                    stringBuilder.NewRow();

                    foreach (var cell in header.HeaderData) stringBuilder.AddCsvString(cell);
                }
            }
            #endregion

            #region Detalles
            stringBuilder.NewBlock();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    for (var i = 0; i < dataReader.FieldCount; i++)
                    {
                        csvAppendArray[i](stringBuilder, dataParserArray[i](dataReader, i));
                    }

                    stringBuilder.NewRow();
                }

                stringBuilder.NewBlock();
            }
            else
            {
                stringBuilder.Append(BLOCK_SEPARATOR);
            }
            #endregion

        }
    }
}
