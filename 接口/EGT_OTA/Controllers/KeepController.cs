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
    /// 收藏管理
    /// </summary>
    public class KeepController : BaseController
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
                if (articleID == 0)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                Article article = new SubSonic.Query.Select(Repository.GetProvider(), "ID", "CreateUserID", "Keeps").From<Article>().Where<Article>(x => x.ID == articleID).ExecuteSingle<Article>();
                if (article == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                Keep model = db.Single<Keep>(x => x.CreateUserID == user.ID && x.ArticleID == articleID && x.Status == Enum_Status.Approved);
                if (model == null)
                {
                    model = new Keep();
                    model.CreateDate = DateTime.Now;
                    model.CreateUserID = user.ID;
                    model.CreateIP = Tools.GetClientIP;
                }
                model.ArticleID = articleID;
                model.ArticleUserID = article.CreateUserID;
                model.Status = Enum_Status.Approved;
                var result = false;
                if (model.ID == 0)
                {
                    result = Tools.SafeInt(db.Add<Keep>(model)) > 0;
                    //修改收藏数
                    if (result)
                    {
                        result = new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("Keeps").EqualTo(article.Keeps + 1).Where<Article>(x => x.ID == articleID).Execute() > 0;
                    }
                }
                else
                {
                    result = db.Update<Keep>(model) > 0;
                }
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("KeepController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public ActionResult Delete()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ID");
                var model = db.Single<Keep>(x => x.ID == id);
                if (model != null)
                {
                    var result = db.Delete<Keep>(id) > 0;

                    //修改文章收藏数
                    if (result)
                    {
                        Article article = new SubSonic.Query.Select(Repository.GetProvider(), "ID", "Keeps").From<Article>().Where<Article>(x => x.ID == model.ArticleID).ExecuteSingle<Article>();
                        if (article != null && article.Keeps > 0)
                        {
                            new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("Keeps").EqualTo(article.Keeps - 1).Where<Article>(x => x.ID == model.ArticleID).Execute();
                        }
                        if (result)
                        {
                            return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("KeepController_Delete:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult All()
        {
            try
            {
                var pager = new Pager();
                var query = new SubSonic.Query.Select(Repository.GetProvider()).From<Keep>().Where<Keep>(x => x.Status == Enum_Status.Approved);
                var CreateUserID = ZNRequest.GetInt("CreateUserID");
                if (CreateUserID > 0)
                {
                    query = query.And("CreateUserID").IsEqualTo(CreateUserID);
                }
                var recordCount = query.GetRecordCount();

                if (recordCount == 0)
                {
                    return Json(new
                    {
                        currpage = pager.Index,
                        records = recordCount,
                        totalpage = 1,
                        list = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                }

                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Keep>();
                var articles = new SubSonic.Query.Select(Repository.GetProvider(), "ID", "Title", "TypeID", "Cover", "Views", "Goods", "Keeps", "Comments", "CreateUserID", "CreateDate", "ArticlePower", "ArticlePowerPwd", "Tag", "City").From<Article>().Where("ID").In(list.Select(x => x.ArticleID).ToArray()).OrderDesc(new string[] { "Tag", "ID" }).ExecuteTypedList<Article>();
                var articletypes = GetArticleType();
                var users = new SubSonic.Query.Select(Repository.GetProvider(), "ID", "NickName", "Avatar", "Signature").From<User>().Where("ID").In(articles.Select(x => x.CreateUserID).Distinct().ToArray()).ExecuteTypedList<User>();

                var array = list.Select(x => x.ArticleID).ToArray();
                var parts = new SubSonic.Query.Select(Repository.GetProvider()).From<ArticlePart>().Where<ArticlePart>(x => x.Types == 1).And("ArticleID").In(array).OrderAsc("SortID").ExecuteTypedList<ArticlePart>();

                var newlist = (from a in articles
                               join u in users on a.CreateUserID equals u.ID
                               select new
                               {
                                   UserID = u.ID,
                                   NickName = u.NickName,
                                   Signature = u.Signature,
                                   Avatar = GetFullUrl(u.Avatar),
                                   ArticleID = a.ID,
                                   Title = a.Title,
                                   Views = a.Views,
                                   Goods = a.Goods,
                                   Comments = a.Comments,
                                   Keeps = a.Keeps,
                                   Pays = a.Pays,
                                   CreateDate = FormatTime(a.CreateDate),
                                   TypeName = articletypes.Exists(x => x.ID == a.TypeID) ? articletypes.FirstOrDefault(x => x.ID == a.TypeID).Name : "",
                                   ArticlePart = parts.Where(x => x.ArticleID == a.ID).OrderBy(x => x.ID).Take(4).ToList(),
                                   ArticlePower = a.ArticlePower,
                                   Tag = a.Tag,
                                   City = a.City
                               }).ToList();
                var result = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = newlist
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("KeepController_All:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
