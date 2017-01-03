using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using System.Net.Security;

namespace EGT_OTA.Helper
{
    public class HttpHelper
    {
        static CookieContainer m_cookCont = new CookieContainer();

        /// <summary>
        /// httpUrl:GET请求网址
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public static string Get(string httpUrl)
        {
            WebClient wc = new WebClient();
            byte[] bData = null;
            string strlocation = "";
            try
            {
                bData = wc.DownloadData(httpUrl);
                strlocation = System.Text.Encoding.UTF8.GetString(bData);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return strlocation;
        }

        /// <summary>
        /// httpUrl:GET请求网址
        /// autoRedirect:是否自动跳转.
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="autoRedirect"></param>
        /// <returns></returns>
        public static string Get(string httpUrl, bool autoRedirect, string encoding)
        {
            string data = "";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(httpUrl);
                httpWReq.CookieContainer = m_cookCont;
                httpWReq.Method = "GET";
                httpWReq.Timeout = 40000;
                httpWReq.AllowAutoRedirect = autoRedirect;
                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
                m_cookCont = httpWReq.CookieContainer;
                using (Stream respStream = httpWResp.GetResponseStream())
                {
                    using (StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.GetEncoding(encoding)))
                    {
                        data = respStreamReader.ReadToEnd();
                    }
                }
            }
            catch { }
            return data;
        }

        public static string GetByEncode(string httpUrl, string encoding)
        {
            string data = "";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(httpUrl);
                httpWReq.CookieContainer = m_cookCont;
                httpWReq.Method = "GET";
                httpWReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1) ; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                httpWReq.AllowAutoRedirect = false;
                httpWReq.Timeout = 60000;
                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
                m_cookCont = httpWReq.CookieContainer;
                using (Stream respStream = httpWResp.GetResponseStream())
                {
                    using (StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.GetEncoding(encoding)))
                    {
                        data = respStreamReader.ReadToEnd();
                    }
                }
            }
            catch { }
            return data;
        }

        public static string GetPage(string httpUrl)
        {
            string data = "";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(httpUrl);
                httpWReq.CookieContainer = m_cookCont;
                httpWReq.Method = "GET";
                httpWReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1) ; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                httpWReq.AllowAutoRedirect = false;
                httpWReq.Timeout = 60000;
                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
                m_cookCont = httpWReq.CookieContainer;
                using (Stream respStream = httpWResp.GetResponseStream())
                {
                    using (StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.UTF8))
                    {
                        data = respStreamReader.ReadToEnd();
                    }
                }
            }
            catch { }
            return data;
        }

        public static string GetByUA(string httpUrl, string ua)
        {
            string data = "";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(httpUrl);
                httpWReq.CookieContainer = m_cookCont;
                httpWReq.Method = "GET";
                httpWReq.UserAgent = ua;
                httpWReq.AllowAutoRedirect = false;
                httpWReq.Timeout = 60000;
                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
                m_cookCont = httpWReq.CookieContainer;
                using (Stream respStream = httpWResp.GetResponseStream())
                {
                    using (StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.UTF8))
                    {
                        data = respStreamReader.ReadToEnd();
                    }
                }
            }
            catch { }
            return data;
        }

        public static string GetByRefer(string httpUrl, string ua, string refer)
        {
            string data = "";
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(httpUrl);
                httpWReq.CookieContainer = m_cookCont;
                httpWReq.Method = "GET";
                httpWReq.UserAgent = ua;
                httpWReq.AllowAutoRedirect = false;
                httpWReq.Referer = refer;
                httpWReq.Timeout = 60000;
                HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
                httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
                m_cookCont = httpWReq.CookieContainer;
                using (Stream respStream = httpWResp.GetResponseStream())
                {
                    using (StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.UTF8))
                    {
                        data = respStreamReader.ReadToEnd();
                    }
                }
            }
            catch { }
            return data;
        }

        /// <summary>
        /// postData:要发送的数据
        /// xhttpUrl:发送网址
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="xhttpUrl"></param>
        /// <returns></returns>
        public static string Post(string postData, string xhttpUrl, bool autoRedirect, string encoding)
        {
            string cookieHeader = "";
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(xhttpUrl);
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            httpWReq.Method = "POST";
            httpWReq.AllowAutoRedirect = autoRedirect;
            httpWReq.CookieContainer = m_cookCont;
            httpWReq.CookieContainer.SetCookies(new Uri(xhttpUrl), cookieHeader);
            Stream reqStream = httpWReq.GetRequestStream();
            StreamWriter reqStreamWrite = new StreamWriter(reqStream);
            reqStreamWrite.Write(postData);
            reqStreamWrite.Close();
            reqStream.Close();
            HttpWebResponse httpWResp = (HttpWebResponse)httpWReq.GetResponse();
            httpWResp.Cookies = m_cookCont.GetCookies(httpWReq.RequestUri);
            m_cookCont = httpWReq.CookieContainer;
            Stream respStream = httpWResp.GetResponseStream();
            StreamReader respStreamReader = new StreamReader(respStream, System.Text.Encoding.GetEncoding(encoding));
            string data = respStreamReader.ReadToEnd();
            respStreamReader.Close();
            respStream.Close();
            return data;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="allStr"></param>
        /// <param name="ruleStr"></param>
        /// <returns></returns>
        public static ArrayList getData(string allStr, string ruleStr)
        {
            ArrayList data = new ArrayList();
            Regex reg = new Regex(ruleStr.Replace("[DATA]", "(?<data>.*?)"), RegexOptions.Singleline | RegexOptions.IgnoreCase);
            MatchCollection mcoll = reg.Matches(allStr);
            for (int i = 0; i < mcoll.Count; i++)
            {
                data.Add(mcoll[i].Result("${data}"));
            }
            return data;
        }

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }
    }
}
