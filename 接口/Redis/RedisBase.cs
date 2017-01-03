/************************************************************************************ 
 * Copyright (c) 2016 安讯科技（南京）有限公司 版权所有 All Rights Reserved.
 * 文件名：  Redis.RedisBase 
 * 版本号：  V1.0.0.0 
 * 创建人： 康春阳
 * 电子邮箱：kangcy@axon.com.cn 
 * 创建时间：2016/12/21 13:47:13 
 * 描述    :
 * =====================================================================
 * 修改时间：2016/12/21 13:47:13 
 * 修改人  ：  
 * 版本号  ：V1.0.0.0 
 * 描述    ：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IRedis
{
    public class RedisBase : IRedisClient
    {
        public RedisBase(RedisConfig config)
        {
            redisConfig = config;
        }

        #region 私有公用方法   在其中我们序列化操作使用Newtonsoft.Json组件

        public static string SerializeContent(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static T DeserializeContent<T>(RedisValue myString)
        {
            return JsonConvert.DeserializeObject<T>(myString);
        }

        #endregion

        #region 初始化

        private static volatile ConnectionMultiplexer _connection;
        private static readonly object _lock = new object();
        private static ConfigurationOptions config = null;
        private static RedisConfig redisConfig = null;
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            if (!redisConfig.Enabled)
            {
                throw new Exception("the redis server is not opend");
            }
            EndPointCollection endPoints = new EndPointCollection();
            foreach (RedisIps ipConfig in redisConfig.RedisIps)
            {
                endPoints.Add(ipConfig.Address, ipConfig.Port);
            }
            config = new ConfigurationOptions
            {
                EndPoints = { endPoints[0] },
                KeepAlive = redisConfig.KeepAlive,
                ClientName = redisConfig.ClientName,
                Password = redisConfig.Password,
                AllowAdmin = true
            };
            return ConnectionMultiplexer.Connect(config);
        });

        public static ConnectionMultiplexer _redis
        {
            get
            {
                if (!redisConfig.Enabled)
                    throw new Exception("the redis server is not opend");
                if (_connection != null && _connection.IsConnected) return _connection;
                lock (_lock)
                {
                    if (_connection != null && _connection.IsConnected)
                        return _connection;
                    if (_connection != null)
                    {
                        _connection.Dispose();
                    }
                    _connection = lazyConnection.Value;
                }
                return _connection;
            }
        }

        #endregion

        #region 清空数据库

        public bool Cleardb()
        {
            foreach (RedisIps ip in redisConfig.RedisIps)
            {
                var _server = _redis.GetServer(ip.Address, ip.Port);
                _server.FlushAllDatabases(CommandFlags.FireAndForget);
            }
            return true;
        }

        /// <summary>
        /// redis Hash事务操作
        /// </summary>
        public void HashSetTranscation(Func<ITransaction, bool> action)
        {
            var _db = _redis.GetDatabase();
            var tran = _db.CreateTransaction();
            if (action(tran))
            {
                tran.Execute();
            }
        }

        #endregion

        #region Redis String数据类型操作

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type T</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        public bool StringSet<T>(string key, T value, TimeSpan? expiresAt = default(TimeSpan?), When when = When.Always,
            CommandFlags commandFlags = CommandFlags.None) where T : class
        {
            var _db = _redis.GetDatabase();
            var stringContent = SerializeContent(value);
            return _db.StringSet(key, stringContent, expiresAt, when, commandFlags);

        }

        /// <summary>
        /// Redis String数据类型 获取指定key中字符串长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long StringLength(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.StringLength(key, commandFlags);
        }

        /// <summary>
        ///  Redis String数据类型  返回拼接后总长度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="appendVal"></param>
        /// <returns>总长度</returns>
        public long StringAppend(string key, string appendVal, CommandFlags commandFlags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.StringAppend(key, appendVal, commandFlags);
        }

        /// <summary>
        /// 设置新值并且返回旧值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newVal"></param>
        /// <param name="commandFlags"></param>
        /// <returns>OldVal</returns>
        public string StringGetAndSet(string key, string newVal, CommandFlags commandFlags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return DeserializeContent<string>(_db.StringGetSet(key, newVal, commandFlags));
        }

        /// <summary>
        /// 更新时应使用此方法，代码更可读。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <param name="when"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public bool StringUpdate<T>(string key, T value, TimeSpan expiresAt, When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class
        {
            var _db = _redis.GetDatabase();
            var stringContent = SerializeContent(value);
            return _db.StringSet(key, stringContent, expiresAt, when, commandFlags);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <param name="commandFlags"></param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val, CommandFlags commandFlags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.StringIncrement(key, val, commandFlags);
        }

        public bool StringHasKey(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            RedisValue myString = _db.StringGet(key, commandFlags);
            return !myString.IsNullOrEmpty;
        }

        /// <summary>
        /// Redis String类型  Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>T</returns>
        public T StringGet<T>(string key, CommandFlags commandFlags = CommandFlags.None) where T : class
        {
            var _db = _redis.GetDatabase();
            try
            {
                RedisValue myString = _db.StringGet(key, commandFlags);
                if (myString.HasValue && !myString.IsNullOrEmpty)
                {
                    return DeserializeContent<T>(myString);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        ///  Redis String类型
        /// 类似于模糊查询  key* 查出所有key开头的键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="commandFlags"></param>
        /// <returns>List<T></returns>
        public List<T> StringGetList<T>(string host, int port, string key, int pageSize = 1000, CommandFlags commandFlags = CommandFlags.None) where T : class
        {
            var _db = _redis.GetDatabase();
            try
            {
                var server = _redis.GetServer(host, port);
                var keys = server.Keys(_db.Database, key, pageSize, commandFlags);
                var keyValues = _db.StringGet(keys.ToArray(), commandFlags);
                var result = new List<T>();
                foreach (var redisValue in keyValues)
                {
                    if (redisValue.HasValue && !redisValue.IsNullOrEmpty)
                    {
                        var item = DeserializeContent<T>(redisValue);
                        result.Add(item);
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Redis Hash散列数据类型操作

        /// <summary>
        /// Redis散列数据类型  批量新增
        /// </summary>
        public bool HashSet(string key, List<HashEntry> hashEntrys, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            _db.HashSet(key, hashEntrys.ToArray(), flags);
            return true;
        }



        /// <summary>
        /// Redis散列数据类型  新增一个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        public bool HashSet<T>(string key, string field, T val, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            _db.HashSet(key, field, SerializeContent(val), when, flags);
            return true;
        }

        /// <summary>
        ///  Redis散列数据类型 获取指定key的指定field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string field)
        {
            var _db = _redis.GetDatabase();
            if (_db.HashExists(key, field))
                return DeserializeContent<T>(_db.HashGet(key, field));
            else
                return default(T);
        }

        /// <summary>
        ///  Redis散列数据类型 获取所有field所有值,以 HashEntry[]形式返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public HashEntry[] HashGetAll(string key, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.HashGetAll(key, flags);
        }

        /// <summary>
        /// Redis散列数据类型 获取key中所有field的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public List<T> HashGetAllValues<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            List<T> list = new List<T>();
            var hashVals = _db.HashValues(key, flags).ToArray();
            foreach (var item in hashVals)
            {
                list.Add(DeserializeContent<T>(item));
            }
            return list;
        }

        /// <summary>
        /// Redis散列数据类型 获取所有Key名称
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public string[] HashGetAllKeys(string key, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.HashKeys(key, flags).ToStringArray();
        }

        /// <summary>
        ///  Redis散列数据类型  单个删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string hashField, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.HashDelete(key, hashField, flags);
        }

        /// <summary>
        ///  Redis散列数据类型  批量删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long HashDelete(string key, string[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            List<RedisValue> list = new List<RedisValue>();
            for (int i = 0; i < hashFields.Length; i++)
            {
                list.Add(hashFields[i]);
            }
            return _db.HashDelete(key, list.ToArray(), flags);
        }

        /// <summary>
        ///  Redis散列数据类型 判断指定键中是否存在此field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool HashExists(string key, string field, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.HashExists(key, field, flags);
        }

        /// <summary>
        /// Redis散列数据类型  获取指定key中field数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long HashLength(string key, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.HashLength(key, flags);
        }

        /// <summary>
        /// Redis散列数据类型  为key中指定field增加incrVal值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="incrVal"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public double HashIncrement(string key, string field, double incrVal, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.HashIncrement(key, field, incrVal, flags);
        }

        public void HashRemoveField(RedisKey key, RedisValue field, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            if (_db.HashExists(key, field))
            {
                _db.HashDelete(key, field, flags);
            }
        }

        public void HashRemoveFields(RedisKey key, RedisValue[] fields, CommandFlags flags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            _db.HashDelete(key, fields, flags);
        }

        #endregion

        #region Redis List列表数据类型操作

        #endregion

        #region Redis Set集合数据类型操作

        #endregion

        #region Redis Sort Set有序集合数据类型操作

        #endregion

        #region Redis发布订阅

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        public void RedisSub(string subChannel)
        {
            var _sub = _redis.GetSubscriber();
            _sub.Subscribe(subChannel, (channel, message) =>
            {
                Console.WriteLine((string)message);
            });
        }

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long RedisPub<T>(string channel, T msg)
        {
            var _sub = _redis.GetSubscriber();
            return _sub.Publish(channel, SerializeContent(msg));
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            var _sub = _redis.GetSubscriber();
            _sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            var _sub = _redis.GetSubscriber();
            _sub.UnsubscribeAll();
        }

        #endregion

        #region Redis各数据类型公用

        /// <summary>
        /// Redis中是否存在指定Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyExists(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.KeyExists(key, commandFlags);
        }

        /// <summary>
        /// Dispose DB connection 释放DB相关链接
        /// </summary>
        public void DbConnectionStop()
        {
            _redis.Dispose();
        }

        /// <summary>
        /// 从Redis中移除键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyRemove(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            return _db.KeyDelete(key, commandFlags);
        }

        /// <summary>
        /// 从Redis中移除多个键
        /// </summary>
        /// <param name="keys"></param>
        public void KeyRemove(RedisKey[] keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var _db = _redis.GetDatabase();
            _db.KeyDelete(keys, commandFlags);
        }

        #endregion

    }
}
