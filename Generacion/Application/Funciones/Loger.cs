using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace Generacion.Application.Funciones
{
    public class Logger
    {
        private static readonly string _logFileName = "Log.txt";

        private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logFileName);

        public static void LogInformation(string message)
        {
            LogMessage("INFO", message);
        }

        public static void LogError(string message, Exception exception = null)
        {
            LogMessage("ERROR", message, exception);
        }

        private static void LogMessage(string logLevel, string message, Exception exception = null)
        {
            string logEntry = $"{DateTime.Now} [{logLevel}] - {message}";

            // EventLog.WriteEntry("OficinaVirtualWeb", logEntry, GetEventLogEntryType(logLevel));

            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            if (exception != null)
            {
                File.AppendAllText(_logFilePath, $"Exception Details: {exception.ToString()}" + Environment.NewLine);
            }
        }

        private static EventLogEntryType GetEventLogEntryType(string logLevel)
        {
            return logLevel.ToUpper() == "ERROR" ? EventLogEntryType.Error : EventLogEntryType.Information;
        }
    }

}