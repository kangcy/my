using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Web;
using System.Xml;
using System.Web.Caching;

namespace EGT_OTA.Helper.Config
{
    /// <summary>
    /// 网站常规配置
    /// kcy 2016-12-26
    /// </summary>
    public class CommonConfig
    {

        #region  变量 构造函数 加载配置文件

        private static string configFilePath = "";
        private static object lockObject = new object();
        private List<ConfigItem> cList = new List<ConfigItem>();

        /// <summary>
        /// 私有化构造函数
        /// </summary>
        private CommonConfig()
        {
            if (HttpContext.Current == null)
            {
                throw new Exception("当前请求不存在");
            }
            configFilePath = HttpContext.Current.Server.MapPath("~/Config/common.config");
            LoadConfig(configFilePath);
        }

        /// <summary>
        /// 单体类访问实体类
        /// </summary>
        public static CommonConfig Instance
        {
            get
            {
                if (HttpContext.Current.Cache["CommonConfig"] == null)
                {
                    lock (lockObject)
                    {
                        if (HttpContext.Current.Cache["CommonConfig"] == null)
                        {
                            CommonConfig config = new CommonConfig();
                            ///添加到缓存
                            HttpContext.Current.Cache.Insert("CommonConfig", config, new CacheDependency(configFilePath), Cache.NoAbsoluteExpiration, TimeSpan.FromHours(24));
                            return config;
                        }
                    }
                }
                ///从缓存中获取
                return HttpContext.Current.Cache["CommonConfig"] as CommonConfig;
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
                throw new Exception("无法加载配置文件。必须把配置文件common.config存放在根目录的/Config文件夹中。");
            }
            XmlNode root = doc.SelectSingleNode("config/common");
            if (root.ChildNodes != null)
            {
                ConfigItem item = null;
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        ///普通配置
                        if (node.LocalName == "item")
                        {
                            item = new ConfigItem();
                            StringDictionary sd = new StringDictionary();
                            ///循环读取属性
                            XmlAttributeCollection attrs = node.Attributes;
                            if (attrs.Count > 0)
                            {
                                foreach (XmlAttribute attr in attrs)
                                {
                                    if (attr.Name.ToLower() == "key")
                                        item.Key = attr.Value;
                                    if (attr.Name.ToLower() == "value")
                                        item.Value = attr.Value;
                                    if (attr.Name.ToLower() == "desc")
                                        item.Description = attr.Value;
                                    sd.Add(attr.Name, attr.Value);
                                }
                                item.Attributes = sd;
                                cList.Add(item);
                            }
                        }
                        ///如果是水印配置
                        if (node.LocalName == "water")
                        {
                            item = new ConfigItem();
                            StringDictionary sd = new StringDictionary();
                            ///循环读取属性
                            XmlAttributeCollection attrs = node.Attributes;
                            if (attrs.Count > 0)
                            {
                                foreach (XmlAttribute attr in attrs)
                                {
                                    if (attr.Name.ToLower() == "key")
                                        item.Key = attr.Value;
                                    if (attr.Name.ToLower() == "cate")
                                        item.Cate = Convert.ToInt32(attr.Value);
                                    if (attr.Name.ToLower() == "location")
                                        item.Location = Convert.ToInt32(attr.Value);
                                    if (attr.Name.ToLower() == "imageurl")
                                        item.ImageUrl = attr.Value;
                                    if (attr.Name.ToLower() == "width")
                                        item.Width = attr.Value;
                                    if (attr.Name.ToLower() == "height")
                                        item.Height = attr.Value;
                                    if (attr.Name.ToLower() == "word")
                                        item.Word = attr.Value;
                                    if (attr.Name.ToLower() == "fontsize")
                                        item.FontSize = Convert.ToInt32(attr.Value);
                                    if (attr.Name.ToLower() == "fontcolor")
                                        item.FontColor = attr.Value;
                                    if (attr.Name.ToLower() == "desc")
                                        item.Description = attr.Value;
                                }
                                cList.Add(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据配置项的唯一标识查找配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ConfigItem GetConfig(string key)
        {
            ConfigItem config = null;
            foreach (ConfigItem item in cList)
            {
                if (item.Key.ToLower() == key.ToLower())
                {
                    config = item;
                }
            }
            return config;
        }

        /// <summary>
        /// 根据配置项的唯一标识查找配置项的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            string value = "";
            foreach (ConfigItem item in cList)
            {
                if (item.Key.ToLower() == key.ToLower())
                {
                    value = item.Value;
                }
            }
            return value;
        }

        /// <summary>
        /// 保存单个配置文件的修改
        /// </summary>
        public void Save(ConfigItem config)
        {
            Save(new List<ConfigItem>() { config });
        }

        /// <summary>
        /// 批量保存配置文件的修改
        /// </summary>
        public void Save(List<ConfigItem> configList)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilePath);
            XmlNode root = doc.SelectSingleNode("config/common");
            foreach (ConfigItem config in configList)
            {
                ///查找是否有该节点
                ConfigItem item = GetConfig(config.Key);
                ///添加新节点
                if (item == null)
                {
                    XmlElement node = doc.CreateElement("item");
                    node.SetAttribute("key", config.Key);
                    node.SetAttribute("value", config.Value);
                    node.SetAttribute("desc", config.Description);
                    root.AppendChild(node);
                }
                ///修改原节点的值
                else
                {
                    foreach (XmlNode node in root.ChildNodes)
                    {
                        if (node.NodeType == XmlNodeType.Element && node.LocalName == "item")
                        {
                            foreach (XmlAttribute attrs in node.Attributes)
                            {
                                if (attrs.Name.ToLower() == "key" && attrs.Value.ToLower() == config.Key.ToLower())
                                {
                                    node.Attributes["value"].Value = config.Value;
                                    node.Attributes["desc"].Value = config.Description;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            lock (lockObject)
            {
                ///保存修改
                doc.Save(configFilePath);
            }
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
            public string Key { get; set; }

            /// <summary>
            /// 配置项的值
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// 配置项的描述
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 配置节点的所有属性及其值的键值对
            /// </summary>
            public StringDictionary Attributes { get; set; }

            #region  水印

            /// <summary>
            /// 水印类型 1：图片 0：文字
            /// </summary>
            public int Cate { get; set; }

            /// <summary>
            /// 水印位置
            /// </summary>
            public int Location { get; set; }

            /// <summary>
            /// 水印图片地址
            /// </summary>
            public string ImageUrl { get; set; }

            /// <summary>
            /// 水印文字
            /// </summary>
            public string Word { get; set; }

            /// <summary>
            /// 水印宽
            /// </summary>
            public string Width { get; set; }

            /// <summary>
            /// 水印高
            /// </summary>
            public string Height { get; set; }

            /// <summary>
            /// 水印文字大小
            /// </summary>
            public int FontSize { get; set; }

            /// <summary>
            /// 水印文字颜色
            /// </summary>
            public string FontColor { get; set; }

            #endregion

            /// <summary>
            /// 序列化与反序列化
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Value", Value);
            }

            protected ConfigItem(SerializationInfo info, StreamingContext context)
            {
                Value = (String)info.GetValue("Value", typeof(String));
            }
        }

        #endregion

    }
}
