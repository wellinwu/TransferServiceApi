using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TransferServiceApi.Help
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogHelper
    {
        public static ILoggerRepository Repository { get; set; }

        /// <summary>
        /// Log4net日志记录
        /// </summary>
        public class Log
        {
            //初始化log4net
            static Log()
            {
                Repository = LogManager.CreateRepository("NETCoreRepository");
                XmlConfigurator.Configure(Repository, new FileInfo("log4net.config"));
            }

            /// <summary>
            /// 调试信息
            /// </summary>
            /// <param name="message">内容</param>
            public static void Debug(object message)
            {
                ILog log = LogManager.GetLogger(Repository.Name, GetThreadInfo());
                log.Debug(message);
            }

            /// <summary>
            /// 信息类型信息
            /// </summary>
            /// <param name="message">内容</param>
            public static void Info(object message)
            {
                ILog log = LogManager.GetLogger(Repository.Name, GetThreadInfo());
                log.Info(message);
            }

            /// <summary>
            /// 错误信息
            /// </summary>
            /// <param name="message">内容</param>
            public static void Error(object message)
            {
                ILog log = LogManager.GetLogger(Repository.Name, GetThreadInfo());
                log.Error(message);
            }

            /// <summary>
            /// 错误信息，捕捉异常
            /// </summary>
            /// <param name="message">内容</param>
            public static void Error(object message, Exception ex)
            {
                ILog log = LogManager.GetLogger(Repository.Name, GetThreadInfo());
                log.Error(message, ex);
            }
        }

        /// <summary>
        /// 获取线程信息
        /// </summary>
        /// <returns></returns>
        private static string GetThreadInfo()
        {
            Thread th = Thread.CurrentThread;
            try
            {
                if (string.IsNullOrWhiteSpace(th.Name))
                {
                    string ThreadName = "zzfwlog";//线程名称前缀（自定义）
                    string Id = Guid.NewGuid().ToString("N").Substring(0, 16);
                    th.Name = $"[{ThreadName}_{Id}_{th.ManagedThreadId}]";
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("获取线程信息方法异常").Error(ex.Message, ex);
            }
            return th.Name;
        }
    }
}
