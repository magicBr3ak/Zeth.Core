using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Zeth.Core
{
    public static class Log
    {
        #region Constants
        public static readonly string NL = Environment.NewLine;
        public static readonly ReaderWriterLock LOCK = new ReaderWriterLock();
        public const string TB = "    ";
        #endregion

        #region Properties
        public static StringProvider FolderPath { get; set; }
        public static StringProvider ApplicationName { get; set; }
        #endregion

        #region Methods
        public static string Save(this object obj, string type)
        {
            if (FolderPath == null)
            {
                System.Diagnostics.Debug.Print("Interseguro.Log: the path provider is null");
                return "";
            }

            var timeStamp = DateTime.Now;
            var logPath = FolderPath.GetPath();

            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

            var logContent =
                "=================================================================================================" + NL +
                " CODE: " + timeStamp.ToString("HHmm") + NL;

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                logContent += " " + propertyInfo.Name + ": " + propertyInfo.GetValue(obj, null) + NL;
            }

            logContent += "=================================================================================================" + NL + NL;

            var path = logPath + "\\" + ApplicationName.GetPath() + "." + type + "." + timeStamp.ToString("dd.MM.yy") + ".txt";

            try
            {
                LOCK.AcquireWriterLock(TimeSpan.FromMilliseconds(100));

                using (var fileStream = File.Open(path, FileMode.Append, FileAccess.Write))
                {
                    var bytes = (new UTF8Encoding(true)).GetBytes(logContent);
                    fileStream.Write(bytes, 0, bytes.Length);
                }

                LOCK.ReleaseWriterLock();
            }
            catch { }

            return timeStamp.ToString("HHmm");
        }

        public static void SaveObject(this object obj, string message = "")
        {
            var objStr = NL;

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var propertyTypeName = propertyInfo.PropertyType.ToString();
                propertyTypeName = propertyTypeName.Substring(propertyTypeName.LastIndexOf('.') + 1);

                var propertyValue = (propertyInfo.GetValue(obj, null) ?? "NULL").ToString();

                objStr += TB + propertyInfo.Name + "(" + propertyTypeName + "): " + propertyValue + NL;
            }

            (new
            {
                MESSAGE = message,
                OBJECT = objStr.Substring(0, objStr.Length - NL.Length)
            }).
            Save("OBJECT");
        }
        public static string SaveException(this Exception ex, string user = "", string step = "")
        {
            var logObject = new
            {
                USER = user,
                STEP = step,
                EX_MESSAGE = ex.Message,
                EX_SOURCE = ex.Source,
                EX_STACKTRACE = ex.StackTrace
            };

            return logObject.Save("ERROR");
        }
        #endregion
    }
}
