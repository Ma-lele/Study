using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;

namespace Util
{
    /// <summary>
    /// 日志工具类
    /// </summary>
    public class ULog
    {
        private const string STR_LOGEX = ".log";//日志文件扩展名
        private const string COLON = ":";
        private const string FORMAT_DATE = "yyyyMMddHH";//日期格式
        private const string FORMAT_DATETIME = "yyyyMMddHH HH:mm:ss ";//日时格式

        private const string PARENT_PATH = "../xjlog";//日志汇总目录
        public static string LOG_PATH = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + PARENT_PATH));//具体应用的日志目录

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="appName">应用名</param>
        /// <param name="level">日志等级</param>
        public static void Write(string message, string appName, Level level = Level.Info)
        {
            Write(message, appName, level, LogType.TextFile);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="appName">应用名</param>
        public static void WriteError(string message, string appName)
        {
            Write(message, appName, Level.Error, LogType.TextFile);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="appName">应用名</param>
        /// <param name="logType">日志种类</param>
        public static void WriteError(string message, string appName, LogType logType)
        {
            Write(message, appName, Level.Error, logType);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="appName">应用名</param>
        /// <param name="level">日志等级</param>
        /// <param name="logType">日志种类</param>
        public static void Write(string message, string appName, Level level = Level.Info, LogType logType = LogType.TextFile)
        {
            //if (logType.Equals(LogType.EventLog))
            //EventLog.WriteEntry(appName, message, (EventLogEntryType)level);
            //else
            WriteFile(message, appName, level);
        }
        /// <summary>
        /// 写文件日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="appName">应用名</param>
        private static void WriteFile(string message, string appName, Level level)
        {
            if (string.IsNullOrEmpty(appName))
                return;
            try
            {
                string logPath = Path.Combine(LOG_PATH, appName);
                UFile.CreateDirectory(logPath);

                //日志以小时为单位创建文件
                string filename = DateTime.Now.ToString(FORMAT_DATE) + STR_LOGEX;
                string filepath = Path.Combine(logPath, filename);

                FileStream fs = null;
                if (UFile.IsExistFile(filepath))
                    fs = new FileStream(filepath, FileMode.Append);
                else
                    fs = new FileStream(filepath, FileMode.OpenOrCreate);

                StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
                sw.Write(DateTime.Now.ToString(FORMAT_DATETIME) + level.ToString() + COLON + Environment.NewLine);
                sw.Write(message + Environment.NewLine);
                sw.Close();
                fs.Close();

            }
            catch (Exception ex)
            {
                //EventLog.WriteEntry(appName, ex.Message, level);
            }
        }

        /// <summary>
        /// 日志种类
        /// </summary>
        public enum LogType
        {
            TextFile,
            EventLog
        }

        /// <summary>
        /// 日志等级
        /// </summary>
        public enum Level
        {
            Info = 4,
            Error = 1
        }
    }
}
