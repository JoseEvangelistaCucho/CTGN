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
        private readonly IConfiguration _configuration;

        public Logger(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void LogInformation(string message)
        {
            LogMessage("INFO", message);
        }

        public void LogError(string message, Exception exception = null)
        {
            LogMessage("ERROR", message, exception);
        }

        private void LogMessage(string logLevel, string message, Exception exception = null)
        {
            string _logFileName = $"Log_{DateTime.Now.ToString("ddMMyyyy")}.txt";
            string _logFilePath = Path.Combine(_configuration["rutaLog"], _logFileName);
            string logEntry = $"{DateTime.Now} [{logLevel}] - {message}";

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