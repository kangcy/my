/************************************************************************************ 
 * Copyright (c) 2016 安讯科技（南京）有限公司 版权所有 All Rights Reserved.
 * 文件名：  Redis.IRedisClient 
 * 版本号：  V1.0.0.0 
 * 创建人： 康春阳
 * 电子邮箱：kangcy@axon.com.cn 
 * 创建时间：2016/12/21 13:44:24 
 * 描述    :
 * =====================================================================
 * 修改时间：2016/12/21 13:44:24 
 * 修改人  ：  
 * 版本号  ：V1.0.0.0 
 * 描述    ：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace IRedis
{
    public interface IRedisClient
    {
        #region Redis String类型操作

        /// <summary>
        /// Redis String类型 Key是否存在
        /// </summary>
        /// <param name="key">unique key of value</param>
        /// <param name="commandFlags"></param>
        /// <returns>存在返回true，不存在返回false</returns>
        bool StringHasKey(string key, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type object</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <param name= "when">枚举类型</param>
        /// <param name="commandFlags"></param>
        /// <returns>true or false</returns>
        bool StringSet<T>(string key, T value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class;

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
        bool StringUpdate<T>(string key, T value, TimeSpan expiresAt, When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class;

        /// <summary>
        /// Redis String类型  Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns>T</returns>
        T StringGet<T>(string key, CommandFlags commandFlags = CommandFlags.None) where T : class;

        /// <summary>
        /// Redis String数据类型 获取指定key中字符串长度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        long StringLength(string key, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        ///  Redis String数据类型  返回拼接后总长度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="appendVal"></param>
        /// <param name="commandFlags"></param>
        /// <returns>总长度</returns>
        long StringAppend(string key, string appendVal, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 设置新值并且返回旧值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newVal"></param>
        /// <param name="commandFlags"></param>
        /// <returns>OldVal</returns>
        string StringGetAndSet(string key, string newVal, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="commandFlags"></param>
        /// <returns>增长后的值</returns>
        double StringIncrement(string key, double val, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// Redis String数据类型
        /// 类似于模糊查询  key* 查出所有key开头的键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="commandFlags"></param>
        /// <returns>返回List<T></returns>
        List<T> StringGetList<T>(string host, int port, string key, int pageSize = 1000, CommandFlags commandFlags = CommandFlags.None) where T : class;

        #endregion

        #region Redis Hash散列数据类型操作

        /// <summary>
        /// Redis散列数据类型  批量新增
        /// </summary>
        bool HashSet(string key, List<HashEntry> hashEntrys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型  新增一个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        bool HashSet<T>(string key, string field, T val, When when = When.Always, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis散列数据类型 获取指定key的指定field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        T HashGet<T>(string key, string field);

        /// <summary>
        ///  Redis散列数据类型 获取所有field所有值,以 HashEntry[]形式返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        HashEntry[] HashGetAll(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型 获取key中所有field的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        List<T> HashGetAllValues<T>(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型 获取所有Key名称
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        string[] HashGetAllKeys(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis散列数据类型  单个删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        bool HashDelete(string key, string hashField, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis散列数据类型  批量删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        long HashDelete(string key, string[] hashFields, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis散列数据类型 判断指定键中是否存在此field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        bool HashExists(string key, string field, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型  获取指定key中field数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        long HashLength(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型  为key中指定field增加incrVal值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="incrVal"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        double HashIncrement(string key, string field, double incrVal, CommandFlags flags = CommandFlags.None);

        #endregion

        #region Redis发布订阅

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        void RedisSub(string subChannel);

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        long RedisPub<T>(string channel, T msg);

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        void Unsubscribe(string channel);

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        void UnsubscribeAll();

        #endregion

        #region Redis各数据类型公用

        /// <summary>
        /// Redis中是否存在指定Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        bool KeyExists(string key, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 从Redis中移除键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        bool KeyRemove(string key, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 从Redis中移除多个键
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        void KeyRemove(RedisKey[] keys, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// Dispose DB connection 释放DB相关链接
        /// </summary>
        void DbConnectionStop();

        #endregion
    }
}
