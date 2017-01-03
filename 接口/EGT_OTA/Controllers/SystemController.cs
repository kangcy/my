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
using System.Net;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 系统管理
    /// </summary>
    public class SystemController : BaseController
    {
        /// <summary>
        /// 检查更新
        /// </summary>
        public ActionResult CheckUpdate()
        {
            try
            {
                var client = ZNRequest.GetString("client");
                var version = ZNRequest.GetString("version");

                if (string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(version))
                {
                    return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
                }

                var currVersion = "";
                var currUrl = "";
                if (client == "android")
                {
                    currVersion = System.Configuration.ConfigurationManager.AppSettings["curr_android_version"];
                    currUrl = System.Configuration.ConfigurationManager.AppSettings["curr_android_url"];
                }
                else
                {
                    currVersion = System.Configuration.ConfigurationManager.AppSettings["curr_ios_version"];
                    currUrl = System.Configuration.ConfigurationManager.AppSettings["curr_ios_url"];
                }

                if (version == currVersion)
                {
                    return Json(new { result = false, message = "当前已是最新版本" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = true, version = currVersion, url = currUrl, remark = System.Configuration.ConfigurationManager.AppSettings["curr_version_remark"] }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_CheckUpdate:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 检查提现申请
        /// </summary>
        public ActionResult CheckApplyMoney()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                if (user.Money < 100)
                {
                    return Json(new { result = false, message = "账户余额不足100元,暂时无法提现" }, JsonRequestBehavior.AllowGet);
                }
                if (db.Exists<ApplyMoney>(x => x.UserID == user.ID && x.Status == Enum_Status.Audit))
                {
                    return Json(new { result = false, message = "已申请，申请后5个工作日内发放完毕" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error(ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 申请提现
        /// </summary>
        public ActionResult ApplyMoney()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                if (user.Money < 100)
                {
                    return Json(new { result = false, message = "账户余额不足100元,暂时无法提现" }, JsonRequestBehavior.AllowGet);
                }
                if (db.Exists<ApplyMoney>(x => x.UserID == user.ID && x.Status == Enum_Status.Audit))
                {
                    return Json(new { result = false, message = "已申请，申请后5个工作日内发放完毕" }, JsonRequestBehavior.AllowGet);
                }

                var name = ZNRequest.GetString("Name");
                var account = ZNRequest.GetString("Account");
                ApplyMoney model = new ApplyMoney();
                model.Account = account;
                model.AccountName = name;
                model.UserID = user.ID;
                model.Status = Enum_Status.Audit;
                model.CreateDate = DateTime.Now;
                model.CreateIP = Tools.GetClientIP;
                var result = Tools.SafeInt(db.Add<ApplyMoney>(model)) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_ApplyMoney:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 申请提现处理
        /// </summary>
        public ActionResult ApplyMoneyDeal()
        {
            try
            {
                var id = ZNRequest.GetInt("ID");
                var model = db.Single<ApplyMoney>(x => x.ID == id);
                if (model != null)
                {
                    model.Status = Enum_Status.Approved;
                    var result = db.Update<ApplyMoney>(model);
                    if (result > 0)
                    {
                        var user = db.Single<User>(x => x.ID == model.UserID);
                        if (user != null)
                        {
                            if (user.Money >= 100)
                            {
                                user.Money -= 100;
                            }
                            else
                            {
                                user.Money = 0;
                            }
                            result = db.Update<User>(user);
                            if (result > 0)
                            {
                                return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                else
                {
                    return Json(new { result = false, message = "当前记录不存在" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("SystemController_ApplyMoneyDeal:" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        public ActionResult SendSMS()
        {
            string mobile = ZNRequest.GetString("Mobile");
            string sms = ZNRequest.GetString("SMS");
            string num = new Random().Next(100000, 999999).ToString();
            if (string.IsNullOrWhiteSpace(sms))
            {
                var defaultsms = System.Web.Configuration.WebConfigurationManager.AppSettings["sms"];
                sms = string.Format(defaultsms, num);
            }

            string baseurl = System.Web.Configuration.WebConfigurationManager.AppSettings["messageurl"].ToString();
            string user = System.Web.Configuration.WebConfigurationManager.AppSettings["messageuser"].ToString();
            string pwd = System.Web.Configuration.WebConfigurationManager.AppSettings["messagepwd"].ToString();


            Encoding myEcoding = Encoding.GetEncoding("GBK");
            string param = HttpUtility.UrlEncode("usr", myEcoding) + "=" + HttpUtility.UrlEncode(user, myEcoding) + "&" +
                HttpUtility.UrlEncode("pwd", myEcoding) + "=" + HttpUtility.UrlEncode(pwd, myEcoding) + "&" +
                HttpUtility.UrlEncode("mobile", myEcoding) + "=" + HttpUtility.UrlEncode(mobile, myEcoding) + "&" +
                HttpUtility.UrlEncode("sms", myEcoding) + "=" + HttpUtility.UrlEncode(sms, myEcoding) + "&" +
                HttpUtility.UrlEncode("extdsrcid", myEcoding) + "=" + HttpUtility.UrlEncode("", myEcoding);
            byte[] bs = Encoding.ASCII.GetBytes(param);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(baseurl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded;charset=GBK";
            req.ContentLength = bs.Length;
            using (Stream reqstream = req.GetRequestStream())
            {
                reqstream.Write(bs, 0, bs.Length);
                reqstream.Close();
            }
            using (WebResponse wr = req.GetResponse())
            {
                StreamReader sr = new StreamReader(wr.GetResponseStream(), myEcoding);
                string srReturn = sr.ReadToEnd().Trim();
                wr.Close();
                sr.Close();

                //发送记录
                SendSMS log = new SendSMS();
                log.Mobile = mobile;
                log.Remark = sms;
                log.Result = srReturn;
                log.CreateDate = DateTime.Now;
                log.CreateIP = Tools.GetClientIP;
                db.Add<SendSMS>(log);

                if (srReturn.Substring(srReturn.IndexOf(",") - 1, 1) == "0")
                {
                    CookieHelper.SetCookie("SMS", num, DateTime.Now.AddMinutes(15));
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
