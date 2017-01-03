using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EGT_OTA.Redis;
using IRedis;

namespace EGT_OTA.Controllers
{
    public class RedisController : Controller
    {
        protected static readonly RedisBase redis = RedisHelper.Redis;

        public ActionResult Index()
        {
            redis.StringSet<string>("word", "测试");
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
