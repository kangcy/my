using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 伪静态XML配置
    /// </summary>
    public class RewriteConfig
    {

        #region  变量 构造函数 加载配置文件

        private static string configFilePath = "";
        private static object lockObject = new object();
        private List<ConfigItem> cList = new List<ConfigItem>();

        /// <summary>
        /// 私有化构造函数
        /// </summary>
        private RewriteConfig()
        {
            if (HttpContext.Current == null)
            {
                throw new Exception("当前请求不存在");
            }
            configFilePath = HttpContext.Current.Server.MapPath("~/Config/rewrite.config");
            LoadConfig(configFilePath);
        }

        /// <summary>
        /// 单体类访问实体类
        /// </summary>
        public static RewriteConfig Instance
        {
            get
            {
                if (HttpContext.Current.Cache["RewriteConfig"] == null)
                {
                    lock (lockObject)
                    {
                        RewriteConfig config = new RewriteConfig();
                        ///添加到缓存
                        HttpContext.Current.Cache.Insert("RewriteConfig", config, new CacheDependency(configFilePath), Cache.NoAbsoluteExpiration, TimeSpan.FromHours(24));
                        return config;
                    }
                }
                ///从缓存中获取
                return HttpContext.Current.Cache["RewriteConfig"] as RewriteConfig;
            }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadConfig(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch
            {
                throw new Exception("无法加载配置文件");
            }
            ///读取一级目录
            XmlNode root = doc.SelectSingleNode("items");
            if (root.ChildNodes != null)
            {
                ConfigItem childItem = null;
                foreach (XmlNode childNode in root.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element && childNode.LocalName == "item")
                    {
                        childItem = new ConfigItem();
                        StringDictionary childSD = new StringDictionary();
                        ///循环读取属性
                        XmlAttributeCollection childAttrs = childNode.Attributes;
                        if (childAttrs.Count > 0)
                        {
                            foreach (XmlAttribute childAttr in childAttrs)
                            {
                                if (childAttr.Name.ToLower() == "url")
                                    childItem.Url = childAttr.Value;
                                if (childAttr.Name.ToLower() == "to")
                                    childItem.To = childAttr.Value;
                                if (childAttr.Name.ToLower() == "desc")
                                    childItem.Description = childAttr.Value;
                                childSD.Add(childAttr.Name, childAttr.Value);
                            }
                            childItem.Attributes = childSD;
                            cList.Add(childItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        public List<ConfigItem> GetAllConfig()
        {
            return cList;
        }

        #endregion

        #region  配置对象实体类

        /// <summary>
        /// 配置对象实体类
        /// </summary>
        [Serializable]
        public class ConfigItem : ISerializable
        {
            public ConfigItem() { }

            /// <summary>
            /// 配置项的唯一标识
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 配置项的值
            /// </summary>
            public string To { get; set; }

            /// <summary>
            /// 配置项的描述
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 配置节点的所有属性及其值的键值对
            /// </summary>
            public StringDictionary Attributes { get; set; }

            /// <summary>
            /// 序列化与反序列化
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Value", To);
            }

            protected ConfigItem(SerializationInfo info, StreamingContext context)
            {
                To = (String)info.GetValue("Value", typeof(String));
            }
        }

        #endregion

    }
}
