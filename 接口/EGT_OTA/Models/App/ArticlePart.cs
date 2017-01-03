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
    /// 文章部分
    /// </summary>
    [Serializable]
    public class ArticlePart
    {
        /// <summary>
        /// ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 文章ID
        /// </summary>
        public int ArticleID { get; set; }

        /// <summary>
        /// 类型（1：图片,2：文字,3：视频）
        /// </summary>
        public int Types { get; set; }

        /// <summary>
        /// 详细
        /// </summary>
        [SubSonicStringLength(5000), SubSonicNullString]
        public string Introduction { get; set; }

        public int SortID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        [SubSonicNullString]
        public string CreateIP { get; set; }
    }
}