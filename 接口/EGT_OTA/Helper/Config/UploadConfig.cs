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
    /// 图片缩略图配置
    /// kcy 2016-12-26
    /// </summary>
    public class UploadConfig
    {

        #region  变量 构造函数 加载配置文件

        private static string configFilePath = "";
        private static object lockObject = new object();
        private List<ConfigItem> cList = new List<ConfigItem>();

        /// <summary>
        /// 私有化构造函数
        /// </summary>
        private UploadConfig()
        {
            if (HttpContext.Current == null)
            {
                throw new Exception("当前请求不存在");
            }
            configFilePath = HttpContext.Current.Server.MapPath("~/Config/upload.config");
            LoadConfig(configFilePath);
        }

        /// <summary>
        /// 单体类访问实体类
        /// </summary>
        public static UploadConfig Instance
        {
            get
            {
                if (HttpContext.Current.Cache["UploadConfig"] == null)
                {
                    lock (lockObject)
                    {
                        if (HttpContext.Current.Cache["UploadConfig"] == null)
                        {
                            UploadConfig config = new UploadConfig();
                            ///添加到缓存
                            HttpContext.Current.Cache.Insert("UploadConfig", config, new CacheDependency(configFilePath), Cache.NoAbsoluteExpiration, TimeSpan.FromHours(24));
                            return config;
                        }
                    }
                }
                ///从缓存中获取
                return HttpContext.Current.Cache["UploadConfig"] as UploadConfig;
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
                throw new Exception("无法加载配置文件。必须把配置文件upload.config存放在根目录的/Config文件夹中。");
            }
            XmlNode root = doc.SelectSingleNode("config/items");
            if (root.ChildNodes != null)
            {
                ConfigItem item = null;
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element && node.LocalName == "item")
                    {
                        item = new ConfigItem();
                        ///循环读取属性
                        XmlAttributeCollection attrs = node.Attributes;
                        if (attrs.Count > 0)
                        {
                            foreach (XmlAttribute attr in attrs)
                            {
                                if (attr.Name.ToLower() == "code")
                                    item.Code = attr.Value;
                                if (attr.Name.ToLower() == "savepath")
                                    item.SavePath = attr.Value;
                                if (attr.Name.ToLower() == "thumb")
                                {
                                    item.Thumb = attr.Value;
                                    List<ThumbMode> modeList = new List<ThumbMode>();
                                    string[] list = item.Thumb.Split('|');
                                    for (int i = 0, len = list.Length; i < len; i++)
                                    {
                                        string[] thumb = list[i].Split(',');
                                        ThumbMode mode = new ThumbMode();
                                        mode.Mode = thumb[0];
                                        mode.Width = int.Parse(thumb[1]);
                                        mode.Height = int.Parse(thumb[2]);
                                        modeList.Add(mode);
                                    }
                                    item.ModeList = modeList;
                                }
                                if (attr.Name.ToLower() == "desc")
                                    item.Description = attr.Value;
                            }
                            cList.Add(item);
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
        public ConfigItem GetConfig(string code)
        {
            ConfigItem config = null;
            foreach (ConfigItem item in cList)
            {
                if (item.Code.ToLower() == code.ToLower())
                {
                    config = item;
                }
            }
            return config;
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
            XmlNode root = doc.SelectSingleNode("config/items");
            foreach (ConfigItem config in configList)
            {
                ///查找是否有该节点
                ConfigItem item = GetConfig(config.Code);
                ///添加新节点
                if (item == null)
                {
                    XmlElement node = doc.CreateElement("item");
                    node.SetAttribute("code", config.Code);
                    node.SetAttribute("savePath", config.SavePath);
                    node.SetAttribute("thumb", config.Thumb);
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
                                if (attrs.Name.ToLower() == "code" && attrs.Value.ToLower() == config.Code.ToLower())
                                {
                                    node.Attributes["savePath"].Value = config.SavePath;
                                    node.Attributes["thumb"].Value = config.Thumb;
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
        public class ConfigItem
        {
            public ConfigItem() { }

            /// <summary>
            /// 配置项的唯一标识
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// 缩略图保存路径
            /// </summary>
            public string SavePath { get; set; }

            /// <summary>
            /// 缩略图上传配置
            /// </summary>
            public string Thumb { get; set; }

            /// <summary>
            /// 缩略图描述
            /// </summary>
            public string Description { get; set; }

            public List<ThumbMode> ModeList { get; set; }
        }
        [Serializable]
        public class ThumbMode
        {
            public ThumbMode() { }
            /// <summary>
            /// 生成缩略图的方式
            /// </summary>
            public string Mode { get; set; }
            /// <summary>
            /// 缩略图的宽
            /// </summary>
            public int Width { get; set; }
            /// <summary>
            /// 缩略图的高
            /// </summary>
            public int Height { get; set; }
        }

        #endregion

    }
}
