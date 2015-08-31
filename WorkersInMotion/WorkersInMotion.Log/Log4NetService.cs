using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkersInMotion.Log
{
    public class Log4NetService : ILogService
    {
        #region Variables Declaration
        private readonly ILog _logger;
        #endregion

        #region Constructor
        static Log4NetService()
        {
            // Gets directory path of the calling application
            // RelativeSearchPath is null if the executing assembly i.e. calling assembly is a
            // stand alone exe file (Console, WinForm, etc). 
            // RelativeSearchPath is not null if the calling assembly is a web hosted application i.e. a web site
            var log4NetConfigDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var log4NetConfigFilePath = Path.Combine(log4NetConfigDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(log4NetConfigFilePath));
        }

        public Log4NetService(Type logClass)
        {
            _logger = LogManager.GetLogger(logClass);
        }
        #endregion

        #region Methods
        public void Fatal(string message)
        {
            if (_logger.IsFatalEnabled)
                _logger.Fatal(message);
        }

        public void Error(string message)
        {
            if (_logger.IsErrorEnabled)
                _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            if (_logger.IsErrorEnabled)
                _logger.Error(message, exception);
        }

        public void Warn(string message)
        {
            if (_logger.IsWarnEnabled)
                _logger.Warn(message);
        }

        public void Info(string message)
        {
            if (_logger.IsInfoEnabled)
                _logger.Info(message);
        }

        public void Debug(string message)
        {
            if (_logger.IsDebugEnabled)
                _logger.Debug(message);
        }
        #endregion
    }
}