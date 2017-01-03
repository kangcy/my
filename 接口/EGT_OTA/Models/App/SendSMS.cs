using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 短信发送记录
    /// </summary>
    [Serializable]
    public class SendSMS
    {
        public SendSMS() { }

        ///<summary>
        ///ID
        ///</summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        ///<summary>
        ///号码
        ///</summary>
        [SubSonicStringLength(11), SubSonicNullString]
        public string Mobile { get; set; }

        ///<summary>
        ///内容
        ///</summary>
        [SubSonicNullString]
        public string Remark { get; set; }

        ///<summary>
        ///发送结果
        ///</summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public string Result { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        [SubSonicNullString]
        public string CreateIP { get; set; }
    }
}
