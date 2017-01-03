using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 文章段落类型枚举
    /// </summary>
    public class Enum_ArticlePart : EnumBase
    {
        /// <summary>
        /// 无
        /// </summary>
        [EnumAttribute("无")]
        public const int None = 0;

        /// <summary>
        /// 图片
        /// </summary>
        [EnumAttribute("图片")]
        public const int Pic = 1;

        /// <summary>
        /// 文字
        /// </summary>
        [EnumAttribute("文字")]
        public const int Text = 2;

        /// <summary>
        /// 视频
        /// </summary>
        [EnumAttribute("视频")]
        public const int Video = 3;
    }
}
