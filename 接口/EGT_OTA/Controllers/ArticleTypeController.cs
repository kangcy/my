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
    /// 文章类型管理
    /// </summary>
    public class ArticleTypeController : BaseController
    {
        /// <summary>
        /// 列表
        /// </summary>
        public ActionResult All()
        {
            try
            {
                var list = GetArticleType().OrderBy(x => x.SortID).ToList();
                var newlist = (from l in list
                               select new
                               {
                                   ID = l.ID,
                                   Cover = GetFullUrl(l.Cover),
                                   Name = l.Name,
                                   Summary = l.Summary,
                                   ParentID = l.ParentID,
                                   ParentIDList = l.ParentIDList
                               }).ToList();
                var result = new
                {
                    currpage = 1,
                    records = list.Count(),
                    totalpage = 1,
                    list = newlist
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("ArticleTypeController_All:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
