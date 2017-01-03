using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGT_OTA.Models
{
    /// <summary>
    ///常规变量定义
    /// </summary>
    public class ConstKeys
    {
        /// <summary>
        /// 全局配置缓存键
        /// </summary>
        public const string CACHEKEY_COMMON_CONFIG = "Common_Config";

        /// <summary>
        /// 登录用户缓存键
        /// </summary>
        public const string CACHEKEY_CURRENTUSER = "CurentUser_{0}";

        /// <summary>
        /// 上传组件模块配置缓存键
        /// </summary>
        public const string CACHEKEY_UPLOADER_CONFIG = "Upload_Config";

        /// <summary>
        /// 记录用户名的cookies名称
        /// </summary>
        public const string COOKIENAME_USER_REMEMBER_LOGINNAME = ".LoginName";

        /// <summary>
        /// 记录未登录用户ID的cookies名称
        /// </summary>
        public const string COOKIENAME_USER_UNLOGIN_USERID = ".UNLoginUserID";

        /// <summary>
        /// 用户登陆时记录验证码的session标识
        /// </summary>
        public const string SESSIONKEY_VALIDATECODE = "Login_ValidateCode";

        /// <summary>
        ///  管理员登陆时SESSION标识
        /// </summary>
        public const string SESSIONKEY_ADMIN_USERID = "Session_Login_Admin";

        /// <summary>
        /// 游客UserID SESSION标识
        /// </summary>
        public const string SESSIONKEY_GUEST_USERID = "Session_Guest_UserID";

        /// <summary>
        /// 游客UserID Cookie标识
        /// </summary>
        public const string COOKIEKEY_GUEST_USERID = "Cookie_Guest_UserID";

        /// <summary>
        /// 管理员UserID Cookie标识
        /// </summary>
        public const string COOKIEKEY_ADMIN_USERID = "Cookie_Admin_UserID";

        public ConstKeys() { }
    }
}