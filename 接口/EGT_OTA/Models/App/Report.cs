/************************************************************************************ 
 * Copyright (c) 2016 安讯科技（南京）有限公司 版权所有 All Rights Reserved.
 * 文件名：  EGT_OTA.Models.App.Article 
 * 版本号：  V1.0.0.0 
 * 创建人： 康春阳
 * 电子邮箱：kangcy@axon.com.cn 
 * 创建时间：2016/7/29 15:08:56 
 * 描述    :
 * =====================================================================
 * 修改时间：2016/7/29 15:08:56 
 * 修改人  ：  
 * 版本号  ：V1.0.0.0 
 * 描述    ：
*************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SubSonic.SqlGeneration.Schema;

namespace EGT_OTA.Models
{
    /// <summary>
    /// 举报
    /// </summary>
    [Serializable]
    public class Report
    {
        /// <summary>
        /// ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 文章
        /// </summary>
        public int ArticleID { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [SubSonicStringLength(1000), SubSonicNullString]
        public string Summary { get; set; }

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
    }
}