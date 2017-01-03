using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonTools;
using EGT_OTA.Helper;
using EGT_OTA.Models;
using Newtonsoft.Json;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 文章
    /// </summary>
    public class ArticleController : BaseController
    {
        /// <summary>
        /// 复制
        /// </summary>
        public ActionResult Copy()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "当前文章不存在" }, JsonRequestBehavior.AllowGet);
                }
                var result = false;
                var model = article;
                model.Title = article.Title + "(副本)";
                model.Province = ZNRequest.GetString("Province");
                model.City = ZNRequest.GetString("City");
                model.CreateUserID = user.ID;
                model.CreateDate = DateTime.Now;
                model.CreateIP = Tools.GetClientIP;
                model.UpdateUserID = user.ID;
                model.UpdateDate = DateTime.Now;
                model.UpdateIP = Tools.GetClientIP;
                model.Status = Enum_Status.Approved;
                model.Views = 0;
                model.Goods = 0;
                model.Keeps = 0;
                model.Comments = 0;
                model.Pays = 0;
                model.Tag = Enum_ArticleTag.None;
                model.ArticlePower = Enum_ArticlePower.Myself;
                model.Number = ValidateCodeHelper.BuildCode(10);
                model.ID = Tools.SafeInt(db.Add<Article>(model));
                result = model.ID > 0;

                if (result)
                {
                    List<ArticlePart> list = new List<ArticlePart>();
                    var parts = db.Find<ArticlePart>(x => x.ArticleID == id).ToList();
                    parts.ForEach(x =>
                    {
                        ArticlePart part = new ArticlePart();
                        part.ArticleID = model.ID;
                        part.Types = x.Types;
                        part.Introduction = x.Introduction;
                        part.SortID = x.SortID;
                        part.Status = Enum_Status.Audit;
                        part.CreateDate = DateTime.Now;
                        part.CreateUserID = user.ID;
                        part.CreateIP = Tools.GetClientIP;
                        list.Add(part);
                    });
                    db.AddMany<ArticlePart>(list);
                }
                if (result)
                {
                    return Json(new { result = true, message = model.ID }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Copy:" + ex.Message);
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
                var id = ZNRequest.GetInt("ArticleID");
                Article article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "当前文章不存在" }, JsonRequestBehavior.AllowGet);
                }
                if (article.CreateUserID != user.ID)
                {
                    return Json(new { result = false, message = "没有权限" }, JsonRequestBehavior.AllowGet);
                }
                var result = db.Delete<Article>(id) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Delete:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

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
                Article model = new Article();
                model.ID = ZNRequest.GetInt("ArticleID");
                if (model.ID > 0)
                {
                    model = db.Single<Article>(x => x.ID == model.ID);
                    if (model == null)
                    {
                        model = new Article();
                    }
                }
                model.Title = SqlFilter(ZNRequest.GetString("Title"));
                model.MusicID = ZNRequest.GetInt("MusicID", 0);
                model.MusicName = ZNRequest.GetString("MusicName");
                model.MusicUrl = ZNRequest.GetString("MusicUrl");
                model.Province = ZNRequest.GetString("Province");
                model.City = ZNRequest.GetString("City");
                model.UpdateUserID = user.ID;
                model.UpdateDate = DateTime.Now;
                model.UpdateIP = Tools.GetClientIP;
                model.Status = Enum_Status.Approved;
                var result = false;
                if (model.ID == 0)
                {
                    var cover = ZNRequest.GetString("Cover");
                    var covers = cover.Split(',');

                    model.Cover = covers[0];
                    model.Views = 0;
                    model.Goods = 0;
                    model.Keeps = 0;
                    model.Comments = 0;
                    model.Pays = 0;
                    model.Tag = Enum_ArticleTag.None;
                    model.TypeID = 10000;
                    model.TypeIDList = "-10000-";
                    model.ArticlePower = Enum_ArticlePower.Myself;
                    model.CreateUserID = user.ID;
                    model.CreateDate = DateTime.Now;
                    model.CreateIP = Tools.GetClientIP;
                    model.Number = ValidateCodeHelper.BuildCode(10);
                    model.Background = 0;
                    model.Template = 0;
                    model.ID = Tools.SafeInt(db.Add<Article>(model));
                    result = model.ID > 0;

                    //初始化文章段落
                    if (result)
                    {
                        for (var i = 0; i < covers.Length; i++)
                        {
                            ArticlePart part = new ArticlePart();
                            part.ArticleID = model.ID;
                            part.Types = Enum_ArticlePart.Pic;
                            part.Introduction = covers[i];
                            part.SortID = i;
                            part.Status = Enum_Status.Audit;
                            part.CreateDate = DateTime.Now;
                            part.CreateUserID = user.ID;
                            part.CreateIP = Tools.GetClientIP;
                            part.ID = Tools.SafeInt(db.Add<ArticlePart>(part));
                            result = part.ID > 0;
                        }
                    }
                    if (result)
                    {
                        return Json(new { result = true, message = model.ID }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    result = db.Update<Article>(model) > 0;

                    var parts = ZNRequest.GetString("PartIDs");
                    if (!string.IsNullOrEmpty(parts))
                    {
                        var articlePart = new SubSonic.Query.Select(Repository.GetProvider(), "ID", "SortID").From<ArticlePart>().ExecuteTypedList<ArticlePart>();

                        var ids = parts.Split(',');
                        ids.ToList().ForEach(x =>
                        {
                            var id = x.Split('-');
                            var partid = Tools.SafeInt(id[0]);
                            var index = Tools.SafeInt(id[1]);
                            new SubSonic.Query.Update<ArticlePart>(Repository.GetProvider()).Set("SortID").EqualTo(index).Where<ArticlePart>(y => y.ID == partid).Execute();
                        });
                    }
                }
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 详情
        /// </summary>
        public ActionResult Detail()
        {
            try
            {
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                Article model = db.Single<Article>(x => x.ID == id);
                if (model == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }

                if (model.Status == Enum_Status.DELETE)
                {
                    return Json(new { result = false, message = "当前文章已删除，请刷新重试" }, JsonRequestBehavior.AllowGet);
                }

                string password = ZNRequest.GetString("ArticlePassword");

                //浏览数
                new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("Views").EqualTo(model.Views + 1).Where<Article>(x => x.ID == model.ID).Execute();

                //创建人
                User createUser = db.Single<User>(x => x.ID == model.CreateUserID);
                if (createUser != null)
                {
                    model.NickName = createUser == null ? "" : createUser.NickName;
                    model.Avatar = createUser == null ? GetFullUrl(null) : GetFullUrl(createUser.Avatar);
                    model.AutoMusic = createUser.AutoMusic;
                    model.ShareNick = createUser.ShareNick;
                }
                //类型
                ArticleType articleType = GetArticleType().Single<ArticleType>(x => x.ID == model.TypeID);
                model.TypeName = articleType == null ? "" : articleType.Name;

                //音乐
                if (model.MusicID > 0)
                {
                    Music music = db.Single<Music>(x => x.ID == model.MusicID);
                    model.MusicUrl = music == null ? "" : music.FileUrl;
                    model.MusicName = music == null ? "" : music.Name;
                }

                //文章部分
                model.ArticlePart = new SubSonic.Query.Select(Repository.GetProvider()).From<ArticlePart>().Where<ArticlePart>(x => x.ArticleID == id).OrderAsc("SortID").ExecuteTypedList<ArticlePart>();

                model.CreateDateText = DateTime.Now.ToString("yyyy-MM-dd");
                model.ShareUrl = System.Configuration.ConfigurationManager.AppSettings["share_url"] + model.Number;

                //模板配置
                if (model.Template > 0)
                {
                    model.TemplateJson = GetArticleTemp().SingleOrDefault(x => x.ID == model.Template);

                    if (model.TemplateJson == null)
                    {
                        model.TemplateJson = new Template();
                    }
                }
                else
                {
                    model.TemplateJson = new Template();
                }

                return Json(new { result = true, message = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Detail:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑模板
        /// </summary>
        public ActionResult EditArticleTemp()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var Template = ZNRequest.GetInt("Template");
                var result = new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("Template").EqualTo(Template).Where<Article>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticleTemp:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑封面
        /// </summary>
        public ActionResult EditArticleCover()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var Cover = ZNRequest.GetString("Cover");
                var result = new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("Cover").EqualTo(Cover).Where<Article>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticleCover:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑音乐
        /// </summary>
        public ActionResult EditArticleMusic()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var MusicID = ZNRequest.GetInt("MusicID");
                var MusicName = ZNRequest.GetString("MusicName");
                var MusicUrl = ZNRequest.GetString("MusicUrl");
                var result = new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("MusicID").EqualTo(MusicID).Set("MusicUrl").EqualTo(MusicUrl).Set("MusicName").EqualTo(MusicName).Where<Article>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticleMusic:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑权限
        /// </summary>
        public ActionResult EditArticlePower()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var ArticlePower = ZNRequest.GetInt("ArticlePower", Enum_ArticlePower.Myself);
                var result = new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("ArticlePower").EqualTo(ArticlePower).Where<Article>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    var parts = db.Find<ArticlePart>(x => x.ArticleID == id).ToList();
                    var status = ArticlePower == Enum_ArticlePower.Public ? Enum_Status.Approved : Enum_Status.Audit;
                    parts.ForEach(x =>
                    {
                        x.Status = status;
                    });
                    db.UpdateMany<ArticlePart>(parts);
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticlePower:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑分类
        /// </summary>
        public ActionResult EditArticleType()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var TypeID = ZNRequest.GetInt("ArticleType");
                var articleType = GetArticleType().Single<ArticleType>(x => x.ID == TypeID);
                if (articleType == null)
                {
                    return Json(new { result = false, message = "不存在当前类型" }, JsonRequestBehavior.AllowGet);
                }
                var result = new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("TypeID").EqualTo(TypeID).Set("TypeIDList").EqualTo(articleType.ParentIDList).Where<Article>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditArticleType:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验权限
        /// </summary>
        public ActionResult CheckPowerPwd()
        {
            try
            {
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var pwd = ZNRequest.GetInt("ArticlePowerPwd");
                var result = db.Exists<Article>(x => x.ID == id && x.ArticlePower == Enum_ArticlePower.Password && x.ArticlePowerPwd == pwd);
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_CheckPowerPwd:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑背景
        /// </summary>
        public ActionResult EditBackground()
        {
            try
            {
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                var background = ZNRequest.GetInt("Background");
                var result = new SubSonic.Query.Update<Article>(Repository.GetProvider()).Set("Background").EqualTo(background).Where<Article>(x => x.ID == id).Execute() > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_EditBackground:" + ex.Message);
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
                //创建人
                var pager = new Pager();
                var query = new SubSonic.Query.Select(Repository.GetProvider()).From<Article>().Where<Article>(x => x.ID > 0);

                //昵称、轻墨号
                var title = ZNRequest.GetString("Title");
                if (!string.IsNullOrWhiteSpace(title))
                {
                    query.And("Title").Like("%" + title + "%");
                }
                var CreateUserID = ZNRequest.GetInt("CreateUserID");
                if (CreateUserID > 0)
                {
                    query = query.And("CreateUserID").IsEqualTo(CreateUserID);
                }

                var CurrUserID = ZNRequest.GetInt("CurrUserID", 0);
                if (CreateUserID != CurrUserID || CreateUserID == 0)
                {
                    query = query.And("ArticlePower").IsEqualTo(Enum_ArticlePower.Public);
                }

                //文章类型
                var TypeID = ZNRequest.GetInt("TypeID");

                if (TypeID > 0)
                {
                    query = query.And("TypeIDList").Like("%-" + TypeID.ToString() + "-%");
                }

                //搜索默认显示推荐文章
                var Source = ZNRequest.GetString("Source");
                if (!string.IsNullOrWhiteSpace(Source))
                {
                    query = query.And("Tag").IsEqualTo(Enum_ArticleTag.Recommend);
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
                var list = query.Paged(pager.Index, pager.Size).OrderDesc(new string[] { "Tag", "ID" }).ExecuteTypedList<Article>();
                var articletypes = GetArticleType();
                var users = new SubSonic.Query.Select(Repository.GetProvider(), "ID", "NickName", "Avatar").From<User>().Where("ID").In(list.Select(x => x.CreateUserID).ToArray()).ExecuteTypedList<User>();

                var array = list.Select(x => x.ID).ToArray();

                var parts = new SubSonic.Query.Select(Repository.GetProvider()).From<ArticlePart>().Where<ArticlePart>(x => x.Types == Enum_ArticlePart.Pic).And("ArticleID").In(array).OrderAsc("SortID").ExecuteTypedList<ArticlePart>();

                var newlist = (from a in list
                               join u in users on a.CreateUserID equals u.ID
                               select new
                               {
                                   NickName = u.NickName,
                                   Avatar = GetFullUrl(u.Avatar),
                                   ArticleID = a.ID,
                                   Title = a.Title,
                                   Views = a.Views,
                                   Goods = a.Goods,
                                   Comments = a.Comments,
                                   Keeps = a.Keeps,
                                   Pays = a.Pays,
                                   UserID = a.CreateUserID,
                                   Cover = a.Cover,
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
                LogHelper.ErrorLoger.Error("ArticleController_All:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult Top()
        {
            try
            {
                //创建人
                var pager = new Pager();
                var query = new SubSonic.Query.Select(Repository.GetProvider()).From<Article>().Where<Article>(x => x.ID > 0);

                //昵称、轻墨号
                var title = ZNRequest.GetString("Title");
                if (!string.IsNullOrEmpty(title))
                {
                    query.And("Title").Like("%" + title + "%");
                }
                var CreateUserID = ZNRequest.GetInt("CreateUserID");
                if (CreateUserID > 0)
                {
                    query = query.And("CreateUserID").IsEqualTo(CreateUserID);
                }

                var CurrUserID = ZNRequest.GetInt("CurrUserID", 0);
                if (CreateUserID != CurrUserID || CreateUserID == 0)
                {
                    query = query.And("ArticlePower").IsEqualTo(Enum_ArticlePower.Public);
                }

                //文章类型
                var TypeID = ZNRequest.GetInt("TypeID");
                if (TypeID > 0)
                {
                    query = query.And("TypeIDList").Like("%-" + TypeID + "-%");
                }

                //搜索默认显示推荐文章
                var Source = ZNRequest.GetString("Source");
                if (!string.IsNullOrWhiteSpace(Source))
                {
                    query = query.And("Tag").IsEqualTo(Enum_ArticleTag.Recommend);
                }

                var recordCount = query.GetRecordCount();
                var list = query.Paged(pager.Index, pager.Size).OrderDesc(new string[] { "Tag", "ID" }).ExecuteTypedList<Article>();

                var newlist = (from a in list
                               select new
                               {
                                   ArticleID = a.ID,
                                   Title = a.Title,
                                   Views = a.Views,
                                   Goods = a.Goods,
                                   Comments = a.Comments,
                                   Keeps = a.Keeps,
                                   Pays = a.Pays,
                                   UserID = a.CreateUserID,
                                   Cover = a.Cover,
                                   CreateDate = FormatTime(a.CreateDate),
                                   ArticlePower = a.ArticlePower,
                                   Tag = a.Tag,
                                   City = a.City
                               }).ToList();
                var result = new
                {
                    records = recordCount,
                    list = newlist
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Top:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 模板列表
        /// </summary>
        public ActionResult Template()
        {
            try
            {
                var list = GetArticleTemp().OrderBy(x => x.ID).ToList();
                var result = new
                {
                    currpage = 1,
                    records = list.Count(),
                    totalpage = 1,
                    list = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Template:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 投稿
        /// </summary>
        public ActionResult Recommend()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var id = ZNRequest.GetInt("ArticleID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var article = db.Single<Article>(x => x.ID == id);
                if (article == null)
                {
                    return Json(new { result = false, message = "文章信息异常" }, JsonRequestBehavior.AllowGet);
                }
                var time = DateTime.Now.AddDays(-7);
                var log = db.Single<ArticleRecommend>(x => x.CreateUserID == user.ID && x.CreateDate > time);
                if (log != null)
                {
                    return Json(new { result = false, message = "每7日只有一次投稿机会，上次投稿时间为：" + log.CreateDate.ToString("yyyy-MM-dd") }, JsonRequestBehavior.AllowGet);
                }
                ArticleRecommend model = new ArticleRecommend();
                model.ArticleID = id;
                model.CreateUserID = user.ID;
                model.CreateDate = DateTime.Now;
                model.CreateIP = Tools.GetClientIP;
                var result = Tools.SafeInt(db.Add<ArticleRecommend>(model)) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleController_Recommend:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }
    }
}
