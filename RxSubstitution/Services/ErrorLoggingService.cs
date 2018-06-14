using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSubstitution.Services
{
    public static class ErrorLoggingService
    {
        private static IErrorLogger _logger;

        public static IErrorLogger Logger { get
            {
                if (_logger == null) _logger = LoggerFactory();
                return _logger;
            }}

        private static IErrorLogger LoggerFactory()
        {
            // read configuration and instantiate the appropriate object which implements IErrorLogger
            //for example : 
            return new DbErrorLogger("ConfigStringGoesHere");
        }
    }

    public interface IErrorLogger
    {

        void LogError(string errorMessage, string stackTrace, string paramList);
    }

    public class DbErrorLogger : IErrorLogger
    {
        public DbErrorLogger (string connectionString)
        {
            //todo: db connection init here
        }
        public void LogError(string errorMessage, string stackTrace, string paramList)
        {
            //todo: implement DB logging
        }
    }
}
