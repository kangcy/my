/************************************************************************************ 
 * Copyright (c) 2016 安讯科技（南京）有限公司 版权所有 All Rights Reserved.
 * 文件名：  EGT_OTA.Models.App.User 
 * 版本号：  V1.0.0.0 
 * 创建人： 康春阳
 * 电子邮箱：kangcy@axon.com.cn 
 * 创建时间：2016/7/29 11:03:10 
 * 描述    :
 * =====================================================================
 * 修改时间：2016/7/29 11:03:10 
 * 修改人  ：  
 * 版本号  ：V1.0.0.0 
 * 描述    ：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 用户登录方式信息
    /// </summary>
    [Serializable]
    public class UserLogin
    {
        public UserLogin() { }

        public UserLogin(int userId, string openId, int source)
        {
            this.UserID = userId;
            this.OpenID = openId;
            this.Source = source;
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 第三方登录编号
        /// </summary>
        [SubSonicNullString]
        public string OpenID { get; set; }

        /// <summary>
        /// 登录方式
        /// </summary>
        public int Source { get; set; }
    }
}