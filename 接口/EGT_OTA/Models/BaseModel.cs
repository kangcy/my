using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [SubSonicNullString]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [SubSonicNullString]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建IP
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string CreateIP { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        [SubSonicNullString]
        public int UpdateUserID { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        [SubSonicNullString]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 修改IP
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string UpdateIP { get; set; }

        /// <summary>
        /// 获取实体类对象的Json格式对象
        /// </summary>
        /// <returns></returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
