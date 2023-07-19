using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XHS.Windows.Server
{
    public class LogHelper
    {
        private static Logger logger = LogManager.GetLogger("LogHelper");

        public static void Info(string message)
        {
            logger.Info(message);
        }

        public static void Error(string message)
        {
            logger.Error(message);
        }
    }
}
