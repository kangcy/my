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
    /// 关注、粉丝管理
    /// </summary>
    public class FanController : BaseController
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
                var userID = ZNRequest.GetInt("ToUserID");
                if (userID == 0)
                {
                    return Json(new { result = false, message = "信息异常,请刷新重试" }, JsonRequestBehavior.AllowGet);
                }
                Fan model = db.Single<Fan>(x => x.FromUserID == user.ID && x.ToUserID == userID);
                if (model == null)
                {
                    model = new Fan();
                    model.FromUserID = user.ID;
                    model.ToUserID = userID;
                    model.Status = Enum_Status.Approved;
                }
                else
                {
                    return Json(new { result = true, message = "已关注" }, JsonRequestBehavior.AllowGet);
                }
                model.CreateDate = DateTime.Now;
                model.CreateIP = Tools.GetClientIP;
                var result = false;
                if (model.ID == 0)
                {
                    result = Tools.SafeInt(db.Add<Fan>(model)) > 0;
                }
                else
                {
                    result = db.Update<Fan>(model) > 0;
                }
                if (result)
                {
                    return Json(new { result = true, message = "已关注" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("FanController_Edit:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检测是否关注
        /// </summary>
        public ActionResult Check()
        {
            User user = GetUserInfo();
            if (user == null)
            {
                return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
            }
            var message = string.Empty;
            var FromUserID = ZNRequest.GetInt("FromUserID");
            var ToUserID = ZNRequest.GetInt("ToUserID");
            try
            {
                var result = db.Exists<Fan>(x => x.FromUserID == FromUserID && x.ToUserID == ToUserID);
                return Json(new { result = result, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("FanController_Check:" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public ActionResult Delete()
        {
            User user = GetUserInfo();
            if (user == null)
            {
                return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
            }

            var result = false;
            var message = string.Empty;
            var id = ZNRequest.GetInt("ids");
            var model = db.Single<Fan>(x => x.ID == id);
            try
            {
                if (model != null)
                {
                    model.Status = Enum_Status.DELETE;
                    result = db.Update<Fan>(model) > 0;
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("FanController_Delete:" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult All()
        {
            try
            {
                var pager = new Pager();
                var query = new SubSonic.Query.Select(Repository.GetProvider()).From<Fan>().Where<Fan>(x => x.Status == Enum_Status.Approved);

                //关注人
                var FromUserID = ZNRequest.GetInt("FromUserID");
                if (FromUserID > 0)
                {
                    query = query.And("FromUserID").IsEqualTo(FromUserID);
                }

                //被关注人
                var ToUserID = ZNRequest.GetInt("ToUserID");
                if (ToUserID > 0)
                {
                    query = query.And("ToUserID").IsEqualTo(ToUserID);
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
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<Fan>();
                var array = new List<int>();
                //我关注的
                if (FromUserID > 0)
                {
                    array = list.Select(x => x.ToUserID).Distinct().ToList();
                }
                //关注我的
                if (ToUserID > 0)
                {
                    array = list.Select(x => x.FromUserID).Distinct().ToList();
                }
                var users = new SubSonic.Query.Select(Repository.GetProvider(), "ID", "NickName", "Avatar", "Signature").From<User>().Where<User>(x => x.Status == Enum_Status.Approved).And("ID").In(array.ToArray()).ExecuteTypedList<User>();

                //我关注的列表
                if (FromUserID > 0)
                {
                    var newlist = (from l in list
                                   join u in users on l.ToUserID equals u.ID
                                   select new
                                   {
                                       CreateDate = l.CreateDate.ToString("yyyy-MM-dd"),
                                       UserID = u.ID,
                                       NickName = u.NickName,
                                       Signature = u.Signature,
                                       Avatar = GetFullUrl(u.Avatar)
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

                //关注我的列表
                if (ToUserID > 0)
                {
                    var newlist = (from l in list
                                   join u in users on l.FromUserID equals u.ID
                                   select new
                                   {
                                       CreateDate = l.CreateDate.ToString("yyyy-MM-dd"),
                                       UserID = u.ID,
                                       NickName = u.NickName,
                                       Signature = u.Signature,
                                       Avatar = GetFullUrl(u.Avatar)
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
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("FanController_All:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
