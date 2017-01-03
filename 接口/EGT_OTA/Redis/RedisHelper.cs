using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using EGT_OTA.Helper;
using IRedis;

namespace EGT_OTA.Redis
{
    public static class RedisHelper
    {
        private static RedisBase redis = null;

        static RedisHelper()
        {
            redis = new RedisBase(GetRedisConfig());
        }

        public static RedisBase Redis
        {
            get { return redis; }
        }

        /// <summary>
        /// 读取Redis配置
        /// </summary>
        public static RedisConfig GetRedisConfig()
        {
            RedisConfig config = new RedisConfig();
            if (CacheHelper.Exists("Redis"))
            {
                config = (RedisConfig)CacheHelper.GetCache("Redis");
            }
            else
            {
                string str = string.Empty;
                string filePath = System.Web.HttpContext.Current.Server.MapPath("/Config/redis.json");
                if (System.IO.File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath, Encoding.Default);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                config = Newtonsoft.Json.JsonConvert.DeserializeObject<RedisConfig>(str);
                CacheHelper.Insert("Redis", config);
            }
            return config;
        }
    }
}