using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using EGT_OTA.Models;
using CommonTools;
using SubSonic.Repository;
using System.Text.RegularExpressions;
using EGT_OTA.Helper;

namespace EGT_OTA.Controllers
{
    public class BaseController : Controller
    {
        protected readonly SimpleRepository db = Repository.GetRepo();

        //默认管理员账号
        protected readonly string Admin_Name = System.Web.Configuration.WebConfigurationManager.AppSettings["admin_name"];
        protected readonly string Admin_Password = System.Web.Configuration.WebConfigurationManager.AppSettings["admin_password"];
        protected readonly string Base_Url = System.Web.Configuration.WebConfigurationManager.AppSettings["base_url"];

        /// <summary>
        /// 分页基础类
        /// </summary>
        public class Pager
        {
            public int Index { get; set; }
            public int Size { get; set; }

            public Pager()
            {
                this.Index = ZNRequest.GetInt("page", 1);
                this.Size = ZNRequest.GetInt("rows", 15);
            }
        }

        /// <summary>
        /// 解码
        /// </summary>
        protected string UrlDecode(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }
            return System.Web.HttpContext.Current.Server.UrlDecode(msg);
        }

        /// <summary>
        /// 防注入
        /// </summary>
        protected string SqlFilter(string inputString, bool nohtml = true)
        {
            string SqlStr = @"and|or|exec|execute|insert|select|delete|update|alter|create|drop|count|\*|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|restore|backup|net +user|net +localgroup +administrators";
            try
            {
                if (!string.IsNullOrEmpty(inputString))
                {
                    inputString = UrlDecode(inputString);
                    if (nohtml)
                    {
                        inputString = Tools.NoHTML(inputString);
                    }
                    inputString = Regex.Replace(inputString, @"\b(" + SqlStr + @")\b", string.Empty, RegexOptions.IgnoreCase);
                    if (nohtml)
                    {
                        inputString = inputString.Replace("&nbsp;", "");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SQL注入", ex);
            }
            return inputString;
        }

        /// <summary>
        /// 图片完整路径
        /// </summary>
        protected string GetFullUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Base_Url + "Images/default.png";
            }
            if (url.ToLower().StartsWith("http"))
            {
                return url;
            }
            return Base_Url + url;
        }

        /// <summary>
        /// APP访问用户信息
        /// </summary>
        protected User GetUserInfo()
        {
            var id = ZNRequest.GetInt("ID");
            return db.Single<User>(x => x.ID == id);
        }

        /// <summary>
        /// 格式化时间显示
        /// </summary>
        protected string FormatTime(DateTime date)
        {
            var totalSeconds = Convert.ToInt32((DateTime.Now - date).TotalSeconds);
            var hour = (totalSeconds / 3600);
            var year = 24 * 365;
            if (hour > year)
            {
                return Convert.ToInt32(hour / year) + "年前";
            }
            else if (hour > 24)
            {
                return Convert.ToInt32(hour / 24) + "天前";
            }
            else if (hour > 0)
            {
                return Convert.ToInt32(hour) + "小时前";
            }
            else
            {
                var minute = totalSeconds / 60;
                if (minute > 0)
                {
                    return Convert.ToInt32(minute) + "分钟前";
                }
                else
                {
                    return totalSeconds + "秒前";
                }
            }
        }

        /// <summary>
        /// 音乐
        /// </summary>
        protected List<MusicJson> GetMusic()
        {
            List<MusicJson> list = new List<MusicJson>();
            if (CacheHelper.Exists("Music"))
            {
                list = (List<MusicJson>)CacheHelper.GetCache("Music");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("/Config/music.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MusicJson>>(str);
                CacheHelper.Insert("Music", list);
            }
            return list.FindAll(x => x.Status == Enum_Status.Approved);
        }

        /// <summary>
        /// 文章类型
        /// </summary>
        protected List<ArticleType> GetArticleType()
        {
            List<ArticleType> list = new List<ArticleType>();
            if (CacheHelper.Exists("ArticleType"))
            {
                list = (List<ArticleType>)CacheHelper.GetCache("ArticleType");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("/Config/articletype.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ArticleType>>(str);
                CacheHelper.Insert("ArticleType", list);
            }
            return list.FindAll(x => x.Status == Enum_Status.Approved);
        }

        /// <summary>
        /// 文章模板
        /// </summary>
        protected List<Template> GetArticleTemp()
        {
            List<Template> list = new List<Template>();
            if (CacheHelper.Exists("Template"))
            {
                list = (List<Template>)CacheHelper.GetCache("Template");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("/Config/template.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Template>>(str);
                var baseurl = System.Configuration.ConfigurationManager.AppSettings["base_url"];
                list.ForEach(x =>
                {
                    x.ThumbUrl = baseurl + x.ThumbUrl;
                    x.Cover = baseurl + x.Cover;
                });
                CacheHelper.Insert("Template", list);
            }
            return list;
        }

        /// <summary>
        /// 敏感词
        /// </summary>
        protected List<DirtyWord> GetDirtyWord()
        {
            List<DirtyWord> list = new List<DirtyWord>();
            if (CacheHelper.Exists("DirtyWord"))
            {
                list = (List<DirtyWord>)CacheHelper.GetCache("DirtyWord");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("/Config/dirtyword.config");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DirtyWord>>(str);
                CacheHelper.Insert("DirtyWord", list);
            }
            return list;
        }

        /// <summary>
        /// 判断是否包含敏感词
        /// </summary>
        protected bool HasDirtyWord(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return false;
            }
            var list = GetDirtyWord();
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Name.Contains(content))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
