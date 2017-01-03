using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EGT_OTA.Models;
using System.IO;
using Newtonsoft.Json;
using CommonTools;
using EGT_OTA.Helper;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using System.Text;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 分享记录管理
    /// </summary>
    public class ShareLogController : BaseController
    {
        /// <summary>
        /// 编辑
        /// </summary>
        public ActionResult Edit()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var articleID = ZNRequest.GetInt("ArticleID");
                Article article = new SubSonic.Query.Select(Repository.GetProvider(), "ID", "CreateUserID", "Shares").From<Article>().Where<Article>(x => x.ID == articleID).ExecuteSingle<Article>();
                ShareLog model = new ShareLog();
                model.CreateDate = DateTime.Now;
                model.CreateUserID = user.ID;
                model.CreateIP = Tools.GetClientIP;
                model.ArticleID = articleID;
                model.Source = ZNRequest.GetString("Source");
                var result = false;

                result = Tools.SafeInt(db.Add<ShareLog>(model)) > 0;
                //修改分享数
                if (result)
                {
                    result = new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("Shares").EqualTo(article.Shares + 1).Where<Article>(x => x.ID == articleID).Execute() > 0;
                }
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ShareLogController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
