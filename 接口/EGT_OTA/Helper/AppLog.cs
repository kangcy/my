using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// AppLog 的摘要说明
    /// </summary>
    public class AppLog
    {
        //全局异常变量
        public StringBuilder _strLog = new StringBuilder();

        static string _logPath = System.Configuration.ConfigurationManager.AppSettings["log"] + "/" + DateTime.Now.ToString("yyyy-MM-dd");
        static string _FilePath = _logPath + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AppLog()
        {
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
            _strLog = new StringBuilder();
        }

        public AppLog(bool newfile)
        {
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
            _strLog = new StringBuilder();
            if (newfile)
            {
                //创建一个日志文件
                string FilePath = _logPath + "/" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".txt";

                if (!System.IO.File.Exists(FilePath))
                {
                    FileStream fs = File.Create(FilePath);
                    _FilePath = FilePath;
                    fs.Close();
                }
            }
        }

        public AppLog(string logname, string msg)
        {
            _logPath = System.Configuration.ConfigurationManager.AppSettings["log"] + "/mdc_log";

            string FilePath = _logPath + "/" + logname + ".txt";
            try
            {
                if (!Directory.Exists(_logPath))
                {
                    Directory.CreateDirectory(_logPath);
                }
                if (!System.IO.File.Exists(FilePath))
                {
                    //创建一个日志文件
                    if (System.IO.File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }
                    FileStream fs = File.Create(FilePath);
                    fs.Close();

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(FilePath, true, System.Text.Encoding.Default))
                    {
                        sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                        sw.WriteLine(DateTime.Now.ToString() + ":" + msg + "\r\n");
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch { }
        }

        public void AddLog(string msg)
        {
            _strLog.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]  " + msg + "\r");
        }

        /// <summary>
        /// 判断异常捕获
        /// </summary>
        /// <param name="querySQL"></param>
        public void Save()
        {
            try
            {
                if (!System.IO.File.Exists(_FilePath))
                {
                    FileStream fs = File.Create(_FilePath);
                    fs.Close();
                }
                AddFile(_FilePath, _strLog.ToString());
                _strLog = new StringBuilder();
            }
            catch (Exception e)
            {
                WriteError(e.Message + "|e3");
            }
        }

        public void Save(string thredfilename)
        {
            try
            {
                string FilePath = _logPath + "/" + DateTime.Now.ToString("yyyy-MM-dd") + thredfilename + ".txt";
                if (!System.IO.File.Exists(FilePath))
                {
                    FileStream fs = File.Create(FilePath);
                    fs.Close();
                }
                AddFile(FilePath, _strLog.ToString());
                _strLog = new StringBuilder();
            }
            catch (Exception e)
            {
                WriteError(e.Message + "|e3");
            }
        }

        public static Object txtLock = new object();


        /// <summary>
        /// 在文件末尾添加
        /// </summary>
        /// <param name="?"></param>
        public void AddFile(string filepath, string str)
        {
            try
            {
                //lock (txtLock)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath, true, System.Text.Encoding.Default))
                    {
                        sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                        sw.WriteLine(str);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    string error = _logPath + "/error.txt";
                    if (!System.IO.File.Exists(error))
                    {
                        FileStream fs = File.Create(error);
                        fs.Close();
                    }
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(error, true, System.Text.Encoding.Default))
                    {
                        sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                        sw.WriteLine(str + "\r\n" + e.Message + "\r\n");
                        sw.Flush();
                        sw.Close();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 日志记录异常捕获
        /// </summary>
        /// <param name="msg"></param>
        public void WriteError(string msg)
        {
            try
            {
                string error = _logPath + "/error" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!System.IO.File.Exists(error))
                {
                    FileStream fs = File.Create(error);
                    fs.Close();
                }
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(error, true, System.Text.Encoding.Default))
                {
                    sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                    sw.WriteLine(DateTime.Now.ToString() + ":" + msg + "\r\n");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
            }
        }
    }
}
