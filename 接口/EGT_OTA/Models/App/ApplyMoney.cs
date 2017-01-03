using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 申请提现记录
    /// </summary>
    [Serializable]
    public class ApplyMoney
    {
        public ApplyMoney() { }

        ///<summary>
        ///ID
        ///</summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        ///<summary>
        ///用户ID
        ///</summary>
        public int UserID { get; set; }

        ///<summary>
        ///提款账号
        ///</summary>
        [SubSonicNullString]
        public string Account { get; set; }

        ///<summary>
        ///提款账户昵称
        ///</summary>
        [SubSonicNullString]
        public string AccountName { get; set; }

        ///<summary>
        ///发送结果
        ///</summary>
        public int Status { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        [SubSonicNullString]
        public string CreateIP { get; set; }
    }
}
