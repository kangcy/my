using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 启用状态枚举
    /// </summary>
    public class Enum_Used : EnumBase
    {
        /// <summary>
        /// 启用
        /// </summary>
        [EnumAttribute("启用")]
        public const int Approved = 1;

        /// <summary>
        /// 不启用
        /// </summary>
        [EnumAttribute("不启用")]
        public const int Audit = 2;
    }
}
