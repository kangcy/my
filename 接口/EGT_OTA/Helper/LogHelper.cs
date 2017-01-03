using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;

namespace EGT_OTA.Helper
{
    public class LogHelper
    {
        public LogHelper() { }

        /// <summary>
        /// 记录异常错误
        /// </summary>
        public static ILog ErrorLoger
        {
            get
            {
                return log4net.LogManager.GetLogger("ErrorLoger");
            }
        }
        /// <summary>
        /// 记录消息
        /// </summary>
        public static ILog InfoLoger
        {
            get
            {
                return log4net.LogManager.GetLogger("InfoLoger");
            }
        }
        /// <summary>
        /// 记录其他信息
        /// </summary>
        public static ILog OtherLoger
        {
            get
            {
                return log4net.LogManager.GetLogger("OtherLoger");
            }
        }
        /// <summary>
        /// 记录用户活动
        /// </summary>
        public static ILog UserLoger
        {
            get
            {
                return log4net.LogManager.GetLogger("UserLoger");
            }
        }

        /// <summary>
        /// 加载log4net配置文件
        /// </summary>
        public static void LoadConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// 加载log4net配置文件
        /// </summary>
        /// <param name="configFile">配置文件的路径</param>
        public static void LoadConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
        }

        public static void WriteLog(string info)
        {
            ILog log = LogManager.GetLogger("log4netlogger");
            log.Error(info);
        }

        public static void WriteLog(string info, Exception ex) { }
    }
}
