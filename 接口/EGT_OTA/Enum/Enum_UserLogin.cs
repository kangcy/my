using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 登录方式枚举
    /// </summary>
    public class Enum_UserLogin : EnumBase
    {
        /// <summary>
        /// 账号登录
        /// </summary>
        [EnumAttribute("账号登录")]
        public const int Common = 0;

        /// <summary>
        /// 微信登录
        /// </summary>
        [EnumAttribute("微信登录")]
        public const int Weixin = 1;

        /// <summary>
        /// QQ登录
        /// </summary>
        [EnumAttribute("QQ登录")]
        public const int QQ = 2;

        /// <summary>
        /// 微博登录
        /// </summary>
        [EnumAttribute("微博登录")]
        public const int Weibo = 3;
    }
}
