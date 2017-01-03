using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 性别枚举
    /// </summary>
    public class Enum_Sex : EnumBase
    {
        /// <summary>
        /// 男
        /// </summary>
        [EnumAttribute("男")]
        public const int Boy = 0;

        /// <summary>
        /// 女
        /// </summary>
        [EnumAttribute("女")]
        public const int Girl = 1;
    }
}
