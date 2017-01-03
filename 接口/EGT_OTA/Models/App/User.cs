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
    /// 用户信息
    /// </summary>
    [Serializable]
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        ///<summary>
        ///登陆账号
        ///</summary>
        [SubSonicStringLength(20), SubSonicNullString]
        public string UserName { get; set; }

        ///<summary>
        ///登陆密码
        ///</summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string Password { get; set; }

        ///<summary>
        ///用户头像
        ///</summary>
        [SubSonicNullString]
        public string Avatar { get; set; }

        ///<summary>
        ///用户昵称
        ///</summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string NickName { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string CityName { get; set; }

        ///<summary>
        ///个性签名
        ///</summary>
        [SubSonicStringLength(500), SubSonicNullString]
        public string Signature { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        ///<summary>
        ///电子邮件
        ///</summary>
        [SubSonicNullString]
        public string Email { get; set; }

        ///<summary>
        ///绑定手机
        ///</summary>
        [SubSonicStringLength(11), SubSonicNullString]
        public string Phone { get; set; }

        ///<summary>
        ///绑定微信
        ///</summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string WeiXin { get; set; }

        /// <summary>
        /// 音乐自动播放
        /// </summary>
        public int AutoMusic { get; set; }

        /// <summary>
        /// 分享带昵称
        /// </summary>
        public int ShareNick { get; set; }

        ///<summary>
        ///上次登录IP
        ///</summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string LastLoginIP { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// 登陆次数
        /// </summary>
        public int LoginTimes { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [SubSonicNullString]
        public string Cover { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否邮箱认证
        /// </summary>
        public int IsEmail { get; set; }

        /// <summary>
        /// 是否启用打赏
        /// </summary>
        public int IsPay { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public int Money { get; set; }

        #region  扩展字段

        /// <summary>
        /// 地址
        /// </summary>
        [SubSonicIgnore]
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [SubSonicIgnore]
        public string BirthdayText { get; set; }

        /// <summary>
        /// 关注
        /// </summary>
        [SubSonicIgnore]
        public int Follows { get; set; }

        /// <summary>
        /// 粉丝
        /// </summary>
        [SubSonicIgnore]
        public int Fans { get; set; }

        /// <summary>
        /// 文章数
        /// </summary>
        [SubSonicIgnore]
        public int Articles { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        [SubSonicIgnore]
        public int Keeps { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        [SubSonicIgnore]
        public int Comments { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        [SubSonicIgnore]
        public int Zans { get; set; }

        /// <summary>
        /// 打赏数
        /// </summary>
        [SubSonicIgnore]
        public int Pays { get; set; }

        /// <summary>
        /// 关注人
        /// </summary>
        [SubSonicIgnore]
        public string FanText { get; set; }

        /// <summary>
        /// 收藏文章
        /// </summary>
        [SubSonicIgnore]
        public string KeepText { get; set; }

        /// <summary>
        /// 登录方式
        /// </summary>
        [SubSonicIgnore]
        public List<UserLogin> UserLogin { get; set; }

        #endregion
    }
}