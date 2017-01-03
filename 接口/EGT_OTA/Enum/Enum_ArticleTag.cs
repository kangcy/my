using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 文章标签枚举
    /// </summary>
    public class Enum_ArticleTag : EnumBase
    {
        /// <summary>
        /// 无
        /// </summary>
        [EnumAttribute("无")]
        public const int None = 0;

        /// <summary>
        /// 精
        /// </summary>
        [EnumAttribute("精")]
        public const int Recommend = 99;

        /// <summary>
        /// 系统
        /// </summary>
        [EnumAttribute("系统")]
        public const int System = 100;
    }
}
