using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 文章权限枚举
    /// </summary>
    public class Enum_ArticlePower : EnumBase
    {
        /// <summary>
        /// 公开
        /// </summary>
        [EnumAttribute("所有人都可以看到")]
        public const int Public = 3;

        /// <summary>
        /// 限制可见
        /// </summary>
        [EnumAttribute("仅通过自己分享后才可见")]
        public const int Share = 2;

        /// <summary>
        /// 密码可见
        /// </summary>
        [EnumAttribute("设置密码,输入密码才可见")]
        public const int Password = 1;

        /// <summary>
        /// 私密
        /// </summary>
        [EnumAttribute("仅自己可见")]
        public const int Myself = 0;
    }
}
