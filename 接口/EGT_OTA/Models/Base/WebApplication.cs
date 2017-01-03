using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using EGT_OTA.Models;
using CommonTools;
using EGT_OTA.Helper;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 在Global里面执行的方法
    /// </summary>
    public class WebApplication
    {
        /// <summary>
        /// 程序运行时执行
        /// </summary>
        /// <param name="context">当前请求上下文</param>
        public void Start(HttpContext context)
        {
            //加载log4net的配置文件
            LogHelper.LoadConfig(new FileInfo(context.Server.MapPath("~/Config/log4net.config")));
        }

        /// <summary>
        /// 程序结束运行时执行
        /// </summary>
        /// <param name="context">当前请求上下文</param>
        public void End(HttpContext context)
        {
            LogHelper.InfoLoger.Info(String.Format("程序于[{0}]关闭", DateTime.Now));
        }

        /// <summary>
        /// 程序出错时执行的方法
        /// </summary>
        /// <param name="context">当前请求上下文</param>
        public void Error(HttpContext context)
        {
            LogHelper.InfoLoger.Error(String.Format("程序于[{0}]关闭。", DateTime.Now));

            ///获取错误信息
            Exception error = context.Server.GetLastError().GetBaseException();

            ///忽略搜索引擎的异常
            if (error is HttpException)
            {
                return;
            }
            StringBuilder sbError = new StringBuilder(500);
            sbError.AppendFormat("错误页面：{0}\r\n", Tools.RequestUrl);
            sbError.AppendFormat("错误信息：{0}\r\n", error.Message.ToString());
            sbError.AppendFormat("追踪信息：(Trace)\r\n{0}\r\n", error.StackTrace.ToString());
            sbError.Append("[用户信息]：\r\n");
            sbError.AppendFormat("UserAgent：{0}\r\n", context.Request.UserAgent);
            sbError.AppendFormat("ClientIP：{0}\r\n", Tools.GetClientIP);
            sbError.AppendFormat("RawUrl：{0}\r\n", Tools.RawUrl);
            sbError.AppendFormat("UrlReferrer：{0}\r\n", context.Request.UrlReferrer);
            LogHelper.ErrorLoger.Error(sbError.ToString());
        }
    }
}
