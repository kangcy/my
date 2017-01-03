/************************************************************************************ 
 * Copyright (c) 2016 安讯科技（南京）有限公司 版权所有 All Rights Reserved.
 * 文件名：  Redis.RedisModel 
 * 版本号：  V1.0.0.0 
 * 创建人： 康春阳
 * 电子邮箱：kangcy@axon.com.cn 
 * 创建时间：2016/12/21 13:56:03 
 * 描述    :
 * =====================================================================
 * 修改时间：2016/12/21 13:56:03 
 * 修改人  ：  
 * 版本号  ：V1.0.0.0 
 * 描述    ：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRedis
{
    public class RedisConfig
    {
        public List<RedisIps> RedisIps { get; set; }
        public bool Enabled { get; set; }
        public int KeepAlive { get; set; }
        public string ClientName { get; set; }
        public string Password { get; set; }
    }

    public class RedisIps
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }
}
