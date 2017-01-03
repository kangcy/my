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
    /// 用户
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// 第三方登录
        /// </summary>
        public ActionResult LoginThird()
        {
            try
            {
                var NickName = SqlFilter(ZNRequest.GetString("NickName").Trim());
                var avatar = ZNRequest.GetString("Avatar").Trim();
                var openID = ZNRequest.GetString("OpenID").Trim();
                var source = ZNRequest.GetInt("Source");

                User user = null;
                if (string.IsNullOrWhiteSpace(openID))
                {
                    openID = Guid.NewGuid().ToString("N");
                }
                else
                {
                    var userLogin = db.Single<UserLogin>(x => x.OpenID == openID);
                    if (userLogin != null)
                    {
                        user = db.Single<User>(x => x.ID == userLogin.UserID);
                    }
                }
                if (user == null)
                {
                    user = new User();
                    user.UserName = string.Empty;
                    user.Password = string.Empty;
                    user.NickName = NickName;
                    user.Sex = ZNRequest.GetInt("Sex", Enum_Sex.Boy);
                    user.Cover = ZNRequest.GetString("Cover");
                    user.Email = string.Empty;
                    user.IsEmail = 0;
                    user.Signature = string.Empty;
                    user.Avatar = avatar;
                    user.Phone = string.Empty;
                    user.WeiXin = string.Empty;
                    user.LoginTimes = 1;
                    user.CreateDate = DateTime.Now;
                    user.LastLoginDate = DateTime.Now;
                    user.LastLoginIP = Tools.GetClientIP;
                    user.Keeps = 0;
                    user.Follows = 0;
                    user.Fans = 0;
                    user.Articles = 0;
                    user.Comments = 0;
                    user.Zans = 0;
                    user.FanText = "";
                    user.KeepText = "";
                    user.ID = Tools.SafeInt(db.Add<User>(user), 0);
                    if (user.ID > 0)
                    {
                        UserLogin userlogin = new UserLogin(user.ID, openID, source);
                        db.Add<UserLogin>(userlogin);

                        user.Address = user.ProvinceName + " " + user.CityName;
                        user.BirthdayText = user.Birthday.ToString("yyyy-MM-dd");
                        user.UserLogin = new List<UserLogin>() { userlogin };

                        return Json(new { result = true, message = user }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    user.LoginTimes += 1;
                    user.LastLoginDate = DateTime.Now;
                    user.LastLoginIP = Tools.GetClientIP;
                    var result = db.Update<User>(user) > 0;
                    if (result)
                    {
                        user.Address = user.ProvinceName + " " + user.CityName;
                        user.BirthdayText = user.Birthday.ToString("yyyy-MM-dd");

                        //关注
                        user.Follows = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Fan>().Where<Fan>(x => x.FromUserID == user.ID).GetRecordCount();

                        //粉丝
                        user.Fans = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Fan>().Where<Fan>(x => x.ToUserID == user.ID).GetRecordCount();

                        //我的
                        user.Articles = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Article>().Where<Article>(x => x.CreateUserID == user.ID).GetRecordCount();

                        //评论
                        user.Comments = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Comment>().Where<Comment>(x => x.CreateUserID == user.ID).GetRecordCount();

                        //点赞
                        user.Zans = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Zan>().Where<Zan>(x => x.CreateUserID == user.ID).GetRecordCount();

                        //我关注的用户
                        var fans = db.Find<Fan>(x => x.FromUserID == user.ID && x.Status == Enum_Status.Approved).Select(x => x.ToUserID).ToArray();
                        user.FanText = "," + string.Join(",", fans) + ",";

                        //我收藏的文章
                        var keeps = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Keep>().Where<Keep>(x => x.CreateUserID == user.ID).ExecuteTypedList<Keep>();
                        user.KeepText = "," + string.Join(",", keeps.Select(x => x.ArticleID).ToArray()) + ",";
                        user.Keeps = keeps.Count();
                        return Json(new { result = true, message = user }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_LoginThird" + ex.Message, ex);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 登录
        /// </summary>
        public ActionResult Login()
        {
            try
            {
                var username = ZNRequest.GetString("UserName").Trim();
                var password = ZNRequest.GetString("Password").Trim();
                if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                {
                    return Json(new { result = false, message = "用户名和密码不能为空" }, JsonRequestBehavior.AllowGet);
                }
                password = DesEncryptHelper.Encrypt(password);
                User user = db.Single<User>(x => x.UserName == username && x.Password == password);
                if (user == null)
                {
                    return Json(new { result = false, message = "用户名或密码错误" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string info = "\r\n" + username + "于" + DateTime.Now.ToString() + "登录APP\r\n" + "登录IP为:" + Tools.GetClientIP;
                    LogHelper.UserLoger.Info(info);

                    user.LoginTimes += 1;
                    user.LastLoginDate = DateTime.Now;
                    user.LastLoginIP = Tools.GetClientIP;
                    var result = db.Update<User>(user) > 0;
                    if (result)
                    {
                        user.Address = user.ProvinceName + " " + user.CityName;
                        user.BirthdayText = user.Birthday.ToString("yyyy-MM-dd");

                        //关注
                        user.Follows = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Fan>().Where<Fan>(x => x.FromUserID == user.ID).GetRecordCount();

                        //粉丝
                        user.Fans = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Fan>().Where<Fan>(x => x.ToUserID == user.ID).GetRecordCount();

                        //我的
                        user.Articles = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Article>().Where<Article>(x => x.CreateUserID == user.ID).GetRecordCount();

                        //评论
                        user.Comments = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Comment>().Where<Comment>(x => x.CreateUserID == user.ID).GetRecordCount();

                        //点赞
                        user.Zans = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Zan>().Where<Zan>(x => x.CreateUserID == user.ID).GetRecordCount();

                        //我关注的用户
                        var fans = db.Find<Fan>(x => x.FromUserID == user.ID && x.Status == Enum_Status.Approved).Select(x => x.ToUserID).ToArray();
                        user.FanText = "," + string.Join(",", fans) + ",";

                        //我收藏的文章
                        var keeps = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Keep>().Where<Keep>(x => x.CreateUserID == user.ID).ExecuteTypedList<Keep>();
                        user.KeepText = "," + string.Join(",", keeps.Select(x => x.ArticleID).ToArray()) + ",";
                        user.Keeps = keeps.Count();

                        user.UserLogin = db.Find<UserLogin>(x => x.UserID == user.ID).ToList();

                        return Json(new { result = true, message = user }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Login" + ex.Message, ex);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 注册
        /// </summary>
        public ActionResult Register()
        {
            var result = string.Empty;
            try
            {
                var username = ZNRequest.GetString("UserName").Trim();
                var password = ZNRequest.GetString("Password").Trim();
                if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                {
                    return Json(new { result = false, message = "用户名和密码不能为空" }, JsonRequestBehavior.AllowGet);
                }
                if (db.Exists<User>(x => x.UserName == username))
                {
                    return Json(new { result = false, message = "当前账号已注册" }, JsonRequestBehavior.AllowGet);
                }
                User user = new User();
                user.UserName = username;
                user.NickName = SqlFilter(ZNRequest.GetString("NickName"));
                user.Password = DesEncryptHelper.Encrypt(password);
                user.Sex = ZNRequest.GetInt("Sex", Enum_Sex.Boy);
                user.Cover = ZNRequest.GetString("Cover");
                user.Email = string.Empty;
                user.IsEmail = 0;
                user.Signature = string.Empty;
                user.Avatar = string.Empty;
                user.Phone = string.Empty;
                user.WeiXin = string.Empty;
                user.LoginTimes = 1;
                user.CreateDate = DateTime.Now;
                user.LastLoginDate = DateTime.Now;
                user.LastLoginIP = Tools.GetClientIP;
                user.Keeps = 0;
                user.Follows = 0;
                user.Fans = 0;
                user.FanText = "";
                user.KeepText = "";
                user.Status = Enum_Status.Approved;
                user.ID = Tools.SafeInt(db.Add<User>(user), 0);
                if (user.ID > 0)
                {
                    UserLogin userlogin = new UserLogin(user.ID, Guid.NewGuid().ToString("N"), Enum_UserLogin.Common);
                    db.Add<UserLogin>(userlogin);

                    user.Address = user.ProvinceName + " " + user.CityName;
                    user.BirthdayText = user.Birthday.ToString("yyyy-MM-dd");
                    user.UserLogin = new List<UserLogin>() { userlogin };

                    return Json(new { result = true, message = user }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Register" + ex.Message, ex);
                result = ex.Message;
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        public ActionResult EditAvatar()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var avatar = ZNRequest.GetString("Avatar").Trim();
                if (string.IsNullOrEmpty(avatar))
                {
                    return Json(new { result = false, message = "请上传头像" }, JsonRequestBehavior.AllowGet);
                }
                user.Avatar = avatar;
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditAvatar" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改封面
        /// </summary>
        public ActionResult EditCover()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var cover = ZNRequest.GetString("Cover").Trim();
                if (string.IsNullOrEmpty(cover))
                {
                    return Json(new { result = false, message = "请上传背景图片" }, JsonRequestBehavior.AllowGet);
                }
                user.Cover = cover;
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditCover" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改地址
        /// </summary>
        public ActionResult EditAddress()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                user.ProvinceName = ZNRequest.GetString("ProvinceName");
                user.CityName = ZNRequest.GetString("CityName");
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditAddress" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改性别
        /// </summary>
        public ActionResult EditSex()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                user.Sex = ZNRequest.GetInt("Sex");
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditSex" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改打赏
        /// </summary>
        public ActionResult EditPay()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                user.IsPay = ZNRequest.GetInt("IsPay");
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditPay" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改生日
        /// </summary>
        public ActionResult EditBirthday()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                user.Birthday = ZNRequest.GetDateTime("Birthday");
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditBirthday" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改昵称
        /// </summary>
        public ActionResult EditNickName()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var NickName = SqlFilter(ZNRequest.GetString("NickName").Trim());
                if (string.IsNullOrEmpty(NickName))
                {
                    return Json(new { result = false, message = "请填写昵称信息" }, JsonRequestBehavior.AllowGet);
                }
                user.NickName = NickName;
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditNickName" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改签名
        /// </summary>
        public ActionResult EditSignature()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var Signature = SqlFilter(ZNRequest.GetString("Signature").Trim());
                if (string.IsNullOrEmpty(Signature))
                {
                    return Json(new { result = false, message = "请填写签名信息" }, JsonRequestBehavior.AllowGet);
                }
                user.Signature = Signature;
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditSignature" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public ActionResult EditPassword()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var newpassword = ZNRequest.GetString("NewPassword").Trim();
                if (string.IsNullOrEmpty(newpassword))
                {
                    return Json(new { result = false, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                newpassword = DesEncryptHelper.Encrypt(newpassword);
                if (user.Password == newpassword)
                {
                    return Json(new { result = false, message = "新密码与原密码相同" }, JsonRequestBehavior.AllowGet);
                }
                user.Password = newpassword;
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditPassword" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改是否分享昵称
        /// </summary>
        public ActionResult EditShareNick()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                user.ShareNick = ZNRequest.GetInt("ShareNick");
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditShareNick" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改是否自动播放音乐
        /// </summary>
        public ActionResult EditAutoMusic()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                user.AutoMusic = ZNRequest.GetInt("AutoMusic");
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EditAutoMusic" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 邮箱认证
        /// </summary>
        public ActionResult EmailVerify()
        {
            User user = GetUserInfo();
            if (user == null)
            {
                return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
            }

            var result = false;
            var message = string.Empty;
            try
            {
                //判断是否已验证
                if (user.IsEmail == 1)
                {
                    return Json(new { result = result, message = "邮箱已认证" }, JsonRequestBehavior.AllowGet);
                }
                var email = ZNRequest.GetString("email");
                if (String.IsNullOrEmpty(email))
                {
                    return Json(new { result = result, message = "参数异常" }, JsonRequestBehavior.AllowGet);
                }
                //判断是否存在邮箱账号
                if (db.Exists<User>(x => x.Email == email && x.ID != user.ID))
                {
                    return Json(new { result = result, message = "该邮箱已被绑定" }, JsonRequestBehavior.AllowGet);
                }
                var code = Guid.NewGuid().ToString("N");
                CookieHelper.SetCookie("email" + user.ID, code);

                var url = "http://localhost/app/User/CheckeEmail?uid=" + user.ID + "&code=" + code;
                string body = @"<strong>这是发给您的邮箱认证的邮件，有效期24小时</strong><p>此为系统邮件，请勿直接回复此邮件。</p> <br />
                                请点击下面的链接完成邮箱验证，如果链接无法转向，请复制一下链接到浏览器的地址栏中直接访问。 <br />
                               <a href='" + url + "' target='_blank'>请点击此处链接</a> <br />如果链接无法转向，请复制此连接" + url + "到浏览器的地址栏中直接访问<br />";
                FromUserModel fromUserModel = new FromUserModel
                {
                    UserID = "kangcy@axon.com.cn",
                    UserPwd = "YXhvbjEyMzQ=",
                    UserName = "少侠网",
                    ToUserArray = new ToUserModel[] { new ToUserModel { UserID = email, UserName = user.UserName } }
                };
                MailHelper.SendMail("少侠网", body, fromUserModel);
                user.Email = email;
                result = db.Update<User>(user) > 0;
                return Json(new { result = result, message = "发送邮箱验证成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_EmailVerify:" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 校验邮箱认证
        /// </summary>
        public ActionResult CheckEmail()
        {
            User user = GetUserInfo();
            if (user == null)
            {
                return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
            }

            var result = false;
            var message = string.Empty;
            try
            {
                var uid = ZNRequest.GetInt("uid");
                var code = ZNRequest.GetString("code");
                var cookie = CookieHelper.GetCookieValue("email" + uid);
                if (code == cookie)
                {
                    if (user.IsEmail == 0)
                    {
                        user.IsEmail = 1;
                        result = db.Update<User>(user) > 0;
                        message = "邮箱验证成功！";
                    }
                    else
                    {
                        message = "邮箱已经验证,请勿重复验证";
                    }
                }
                else
                {
                    message = "邮箱验证失败！<br />可能原因如下：<br />1、验证码过期<br />2、点击连接时网络连接失败<br />请重新发送验证请求";
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_CheckEmail" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = result, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        public ActionResult Detail()
        {
            try
            {
                var id = ZNRequest.GetInt("ID");
                if (id == 0)
                {
                    return Json(new { result = false, message = "参数信息异常" }, JsonRequestBehavior.AllowGet);
                }
                User user = db.Single<User>(x => x.ID == id);
                if (user == null)
                {
                    return Json(new { result = false, message = "用戶信息异常" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    user.Address = user.ProvinceName + " " + user.CityName;
                    user.BirthdayText = user.Birthday.ToString("yyyy-MM-dd");

                    //关注
                    user.Follows = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Fan>().Where<Fan>(x => x.FromUserID == user.ID).GetRecordCount();

                    //粉丝
                    user.Fans = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Fan>().Where<Fan>(x => x.ToUserID == user.ID).GetRecordCount();

                    //我的
                    user.Articles = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Article>().Where<Article>(x => x.CreateUserID == user.ID).GetRecordCount();

                    //评论
                    user.Comments = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Comment>().Where<Comment>(x => x.CreateUserID == user.ID).GetRecordCount();

                    //点赞
                    user.Zans = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Zan>().Where<Zan>(x => x.CreateUserID == user.ID).GetRecordCount();

                    //我关注的用户
                    var fans = db.Find<Fan>(x => x.FromUserID == user.ID && x.Status == 1).Select(x => x.ToUserID).ToArray();
                    user.FanText = "," + string.Join(",", fans) + ",";

                    //我收藏的文章
                    var keeps = new SubSonic.Query.Select(Repository.GetProvider(), "ID").From<Keep>().Where<Keep>(x => x.CreateUserID == user.ID).ExecuteTypedList<Keep>();
                    user.KeepText = "," + string.Join(",", keeps.Select(x => x.ArticleID).ToArray()) + ",";
                    user.Keeps = keeps.Count();

                    user.UserLogin = db.Find<UserLogin>(x => x.UserID == user.ID).ToList();

                    return Json(new { result = true, message = user }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Detail" + ex.Message, ex);
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
                var nickname = ZNRequest.GetString("NickName");
                var query = new SubSonic.Query.Select(Repository.GetProvider()).From<User>().Where<User>(x => x.Status == Enum_Status.Approved);
                if (!string.IsNullOrWhiteSpace(nickname))
                {
                    query.And("NickName").IsNotNull().And("NickName").Like("%" + nickname + "%");
                }

                //搜索默认显示推荐文章
                var Source = ZNRequest.GetString("Source");
                if (!string.IsNullOrWhiteSpace(Source))
                {
                    query = query.And("NickName").IsNotNull().And("Signature").IsNotNull().And("Avatar").IsNotNull();
                }
                var list = new List<User>();
                if (string.IsNullOrWhiteSpace(Source))
                {
                    list = query.OrderDesc("ID").ExecuteTypedList<User>();
                }
                else
                {
                    list = query.Paged(1, 100).OrderDesc("ID").ExecuteTypedList<User>();
                }

                var recordCount = list.Count;
                var totalPage = 1;
                var newlist = (from l in list
                               select new
                               {
                                   ID = l.ID,
                                   NickName = l.NickName,
                                   Signature = l.Signature,
                                   Avatar = GetFullUrl(l.Avatar)
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
                LogHelper.ErrorLoger.Error("UserController_All" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 绑定号码
        /// </summary>
        public ActionResult BindPhone()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var phone = ZNRequest.GetString("Phone");
                if (string.IsNullOrEmpty(phone))
                {
                    return Json(new { result = false, message = "请填写手机号码" }, JsonRequestBehavior.AllowGet);
                }
                if (db.Exists<User>(x => x.Phone == phone && x.ID != user.ID))
                {
                    return Json(new { result = false, message = "该手机号码已绑定其他账号" }, JsonRequestBehavior.AllowGet);
                }
                user.Phone = phone;
                var result = db.Update<User>(user) > 0;
                if (result)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_BindPhone" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 绑定微信
        /// </summary>
        public ActionResult BindWeixin()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var key = ZNRequest.GetString("Key");
                if (db.Exists<UserLogin>(x => x.OpenID == key && x.Source == Enum_UserLogin.Weixin))
                {
                    return Json(new { result = false, message = "该微信账号已绑定其他账号" }, JsonRequestBehavior.AllowGet);
                }
                UserLogin userLogin = new UserLogin(user.ID, key, Enum_UserLogin.Weixin);
                userLogin.ID = Tools.SafeInt(db.Add<UserLogin>(userLogin), 0);
                if (userLogin.ID > 0)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_BindWeixin" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 绑定微博
        /// </summary>
        public ActionResult BindWeibo()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var key = ZNRequest.GetString("Key");
                if (db.Exists<UserLogin>(x => x.OpenID == key && x.Source == Enum_UserLogin.Weibo))
                {
                    return Json(new { result = false, message = "该微博账号已绑定其他账号" }, JsonRequestBehavior.AllowGet);
                }
                UserLogin userLogin = new UserLogin(user.ID, key, Enum_UserLogin.Weibo);
                userLogin.ID = Tools.SafeInt(db.Add<UserLogin>(userLogin), 0);
                if (userLogin.ID > 0)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_BindWeibo" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 绑定QQ
        /// </summary>
        public ActionResult BindQQ()
        {
            try
            {
                User user = GetUserInfo();
                if (user == null)
                {
                    return Json(new { result = false, message = "用户信息验证失败" }, JsonRequestBehavior.AllowGet);
                }
                var key = ZNRequest.GetString("Key");
                if (db.Exists<UserLogin>(x => x.OpenID == key && x.Source == Enum_UserLogin.QQ))
                {
                    return Json(new { result = false, message = "该QQ账号已绑定其他账号" }, JsonRequestBehavior.AllowGet);
                }
                UserLogin userLogin = new UserLogin(user.ID, key, Enum_UserLogin.QQ);
                userLogin.ID = Tools.SafeInt(db.Add<UserLogin>(userLogin), 0);
                if (userLogin.ID > 0)
                {
                    return Json(new { result = true, message = "成功" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_BindQQ" + ex.Message);
            }
            return Json(new { result = false, message = "失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 相册
        /// </summary>
        public ActionResult Pic()
        {
            try
            {
                var pager = new Pager();
                var query = new SubSonic.Query.Select(Repository.GetProvider()).From<ArticlePart>().Where<ArticlePart>(x => x.Types == Enum_ArticlePart.Pic && x.Status == Enum_Status.Approved);
                var UserID = ZNRequest.GetInt("UserID");
                if (UserID == 0)
                {
                    return Json(new
                    {
                        currpage = pager.Index,
                        records = 0,
                        totalpage = 1,
                        list = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                }

                query = query.And("CreateUserID").IsEqualTo(UserID);
                var recordCount = query.GetRecordCount();
                var totalPage = recordCount % pager.Size == 0 ? recordCount / pager.Size : recordCount / pager.Size + 1;
                var list = query.Paged(pager.Index, pager.Size).OrderDesc("ID").ExecuteTypedList<ArticlePart>();
                var result = new
                {
                    currpage = pager.Index,
                    records = recordCount,
                    totalpage = totalPage,
                    list = list
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UserController_Pic" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
