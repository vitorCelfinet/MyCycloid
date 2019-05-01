using NLog;

namespace Cycloid.Logger
{
    public static class LogEngine
    {
        public static readonly NLog.Logger DefaultLogger = LogManager.GetLogger("DefaultLogger");
    }

    public enum LogLevels
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public static class LogHelper
    {
        public static void WriteToLog(this NLog.Logger logger, LogLevels level, string message, string loggerName)
        {
            switch (level)
            {
                case LogLevels.Debug: logger.Debug($"{loggerName}||{message}"); break;
                case LogLevels.Info: logger.Info($"{loggerName}|{message}"); break;
                case LogLevels.Warning: logger.Warn($"{loggerName}|{message}"); break;
                case LogLevels.Error: logger.Error($"{loggerName}|{message}"); break;
                default: logger.Debug($"{loggerName}|{message}"); break;
            }
        }

        public static void WriteToLog(this ILogger logger, LogLevels level, string sessionId, string deviceId, string message)
        {
            switch (level)
            {
                case LogLevels.Debug: logger.Debug($"{sessionId}|{deviceId}|{message}"); break;
                case LogLevels.Info: logger.Info($"{sessionId}|{deviceId}|{message}"); break;
                case LogLevels.Warning: logger.Warn($"{sessionId}|{deviceId}|{message}"); break;
                case LogLevels.Error: logger.Error($"{sessionId}|{deviceId}|{message}"); break;
                default: logger.Debug($"{sessionId}|{deviceId}|{message}"); break;
            }
        }
    }
}
