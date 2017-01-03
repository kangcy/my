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
    /// 文章评论
    /// </summary>
    [Serializable]
    public class Comment
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
        /// 文章作者
        /// </summary>
        public int ArticleUserID { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [SubSonicStringLength(5000), SubSonicNullString]
        public string Summary { get; set; }

        /// <summary>
        /// 评论省份
        /// </summary>
        [SubSonicNullString]
        public string Province { get; set; }

        /// <summary>
        /// 评论城市
        /// </summary>
        [SubSonicNullString]
        public string City { get; set; }

        /// <summary>
        /// 回复评论ID
        /// </summary>
        public int ParentCommentID { get; set; }

        /// <summary>
        /// 回复用戶ID
        /// </summary>
        public int ParentUserID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int Goods { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建IP
        /// </summary>
        [SubSonicNullString]
        public string CreateIP { get; set; }

        #region 扩展

        /// <summary>
        /// 创建月
        /// </summary>
        [SubSonicIgnore]
        public string Month { get; set; }

        /// <summary>
        /// 创建日
        /// </summary>
        [SubSonicIgnore]
        public string Day { get; set; }

        #endregion
    }
}