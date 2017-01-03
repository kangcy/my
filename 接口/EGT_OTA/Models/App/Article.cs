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
    /// 文章
    /// </summary>
    [Serializable]
    public class Article : BaseModel
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [SubSonicPrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [SubSonicStringLength(100), SubSonicNullString]
        public string Title { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [SubSonicStringLength(255), SubSonicNullString]
        public string Cover { get; set; }

        /// <summary>
        /// 音乐
        /// </summary>
        public int MusicID { get; set; }

        /// <summary>
        /// 音乐名称
        /// </summary>
        [SubSonicStringLength(255), SubSonicNullString]
        public string MusicName { get; set; }

        /// <summary>
        /// 音乐外链
        /// </summary>
        [SubSonicStringLength(500), SubSonicNullString]
        public string MusicUrl { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// 类型父集合
        /// </summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string TypeIDList { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        public int Views { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int Goods { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        public int Keeps { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int Comments { get; set; }

        /// <summary>
        /// 打赏数
        /// </summary>
        public int Pays { get; set; }

        /// <summary>
        /// 分享数
        /// </summary>
        public int Shares { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// 文章权限
        /// </summary>
        public int ArticlePower { get; set; }

        /// <summary>
        /// 文章权限密码
        /// </summary>
        [SubSonicStringLength(50), SubSonicNullString]
        public int ArticlePowerPwd { get; set; }

        /// <summary>
        /// 发帖省份
        /// </summary>
        [SubSonicNullString]
        public string Province { get; set; }

        /// <summary>
        /// 发帖城市
        /// </summary>
        [SubSonicNullString]
        public string City { get; set; }

        /// <summary>
        /// 文章编号
        /// </summary>
        [SubSonicStringLength(32), SubSonicNullString]
        public string Number { get; set; }

        /// <summary>
        /// 背景展示方式（0:全屏,1:居顶,2:平铺）
        /// </summary>
        public int Background { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        public int Template { get; set; }

        #region 扩展

        /// <summary>
        /// 模板配置
        /// </summary>
        [SubSonicIgnore]
        public Template TemplateJson { get; set; }

        /// <summary>
        /// 创建人头像
        /// </summary>
        [SubSonicIgnore]
        public string Avatar { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SubSonicIgnore]
        public string NickName { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        [SubSonicIgnore]
        public string TypeName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SubSonicIgnore]
        public string CreateDateText { get; set; }

        /// <summary>
        /// 音乐是否自动播放
        /// </summary>
        [SubSonicIgnore]
        public int AutoMusic { get; set; }

        /// <summary>
        /// 分享带昵称
        /// </summary>
        [SubSonicIgnore]
        public int ShareNick { get; set; }

        /// <summary>
        /// 分享链接
        /// </summary>
        [SubSonicIgnore]
        public string ShareUrl { get; set; }

        /// <summary>
        /// 文章部分
        /// </summary>
        [SubSonicIgnore]
        public List<ArticlePart> ArticlePart { get; set; }

        #endregion
    }
}