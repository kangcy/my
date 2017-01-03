using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 币种枚举
    /// </summary>
    public class Enum_Price : EnumBase
    {
        /// <summary>
        /// 人民币
        /// </summary>
        [EnumAttribute("人民币")]
        public const int RenMingBi = 1;
        /// <summary>
        /// 美元
        /// </summary>
        [EnumAttribute("美元")]
        public const int MeiYuan = 2;
        /// <summary>
        /// 英镑
        /// </summary>
        [EnumAttribute("英镑")]
        public const int YingBang = 3;
        /// <summary>
        /// 欧元
        /// </summary>
        [EnumAttribute("欧元")]
        public const int OuYuan = 4;
        /// <summary>
        /// 瑞郎
        /// </summary>
        [EnumAttribute("瑞郎")]
        public const int RuiLang = 5;
    }
}
