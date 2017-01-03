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
    /// 文章类型
    /// </summary>
    [Serializable]
    public class ArticleType
    {
        public ArticleType()
        {

        }

        public ArticleType(int Id, int parentId, string parentIdList, string name, string summary, string cover, int status = 1, int sortId = 0)
        {
            this.ID = Id;
            this.Name = name;
            this.Summary = summary;
            this.Cover = cover;
            this.ParentID = parentId;
            this.ParentIDList = parentIdList;
            this.Status = status;
            this.SortID = sortId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 父节点ID集合
        /// </summary>
        public string ParentIDList { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortID { get; set; }
    }
}