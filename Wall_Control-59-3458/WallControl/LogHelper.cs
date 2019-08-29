using System;
using System.Collections.Generic;
using System.Text;

namespace WallControl
{
    public class LogHelper
    {
        private LogHelper()
        {

        }

        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

        public static readonly log4net.ILog logdebug = log4net.LogManager.GetLogger("logdebug");

        /*
        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        public static void SetConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.Configure(configFile);
        }
         */ 

        public static void WriteLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        public static void DebugLog(string info)
        {
            if (logdebug.IsDebugEnabled)
            {
                logdebug.Debug(info);
            }
        }

        public static void WriteLog(string info, Exception se)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info, se);
            }
        }
    }
}
