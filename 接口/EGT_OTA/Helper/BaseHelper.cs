using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// 通用
    /// </summary>
    public class BaseHelper
    {
        /// <summary>
        /// 检测含中文字符串实际长度
        /// </summary>
        /// <param name="str">待检测的字符串</param>
        /// <returns>返回正整数</returns>
        public static int NumChar(string Input)
        {
            ASCIIEncoding n = new ASCIIEncoding();
            byte[] b = n.GetBytes(Input);
            int l = 0;
            for (int i = 0; i <= b.Length - 1; i++)
            {
                if (b[i] == 63)//判断是否为汉字或全脚符号
                {
                    l++;
                }
                l++;
            }
            return l;
        }

        /// <summary>
        /// 取得网站的根目录的URL
        /// </summary>
        public static string GetRootURI()
        {
            string AppPath = "";
            HttpContext HttpCurrent = HttpContext.Current;
            if (HttpCurrent != null)
            {
                HttpRequest req = HttpCurrent.Request;
                if (req.ApplicationPath != null && req.ApplicationPath != "/")
                    AppPath = req.ApplicationPath;
            }
            return AppPath;
        }

        public static string GetClientIp()
        {
            string ip = HttpContext.Current.Request.Headers["x-forwarded-for"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            return ip;
        }

        /// <summary>
        /// 获取文章中图片地址
        /// </summary>
        /// <param name="text">html内容</param>
        /// <returns>1.jpg,http://baidu/2.jpg</returns>
        public static string GetImgUrl(string text, int topNum)
        {
            StringBuilder sbr = new StringBuilder();
            string pat = @"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>";
            Regex r = new Regex(pat, RegexOptions.Compiled);
            Match m = r.Match(text);
            int index = 1;
            while (m.Success)
            {
                if (topNum == 0)
                {
                    Group g = m.Groups[2];
                    sbr.Append(",").Append(g);
                    m = m.NextMatch();
                }
                else
                {
                    if (index <= topNum && topNum > 0)
                    {
                        Group g = m.Groups[2];
                        sbr.Append(",").Append(g);
                        m = m.NextMatch();
                    }
                    index++;
                }
            }
            string str = sbr.Replace("\"", "").ToString();
            if (str.Length > 0)
                return str.Substring(1);
            return str;
        }


        /// <summary>
        /// 将Unicon字符串转成汉字String
        /// </summary>
        /// <param name="str">Unicon字符串</param>
        /// <returns>汉字字符串</returns>
        public string UniconToString(string str)
        {
            string outStr = "";
            if (!string.IsNullOrEmpty(str))
            {
                string[] strlist = str.Replace("//", "").Split('u');
                try
                {
                    for (int i = 1; i < strlist.Length; i++)
                    {
                        //将unicode字符转为10进制整数，然后转为char中文字符  
                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch (FormatException ex)
                {
                    outStr = ex.Message;
                }
            }
            return outStr;
        }

        /// <summary>
        /// kcy 2013-09-26
        /// 截断字符串
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <param name="num">截取长度</param>
        /// <param name="isEnd">是否需要...结尾</param>
        public static string CutString(object contents, int num, bool isEnd)
        {
            string content = NoHTML(Convert.ToString(contents));
            if (content.ToString().Length > num)
            {
                if (isEnd)
                    return content.ToString().Substring(0, num) + "...";
                else
                    return content.ToString().Substring(0, num);
            }
            else
            {
                return content.ToString();
            }
        }

        /// <summary>
        /// 去除html标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string NoHTML(string Htmlstring)
        {
            ///kangcy 新增筛选 2013-11-13
            if (string.IsNullOrEmpty(Htmlstring))
                return "";
            //删除脚本  
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = Htmlstring.Replace(" ", "").Trim();
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
    }
}
