using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EGT_OTA.Controllers
{
    public class SmsController : Controller
    {
        //
        // GET: /Sms/

        public ActionResult Index()
        {
            //发送模板短信
            var result = RCSCloudRestAPI.sendTplSms(
                  "3099560a730045f299c34ae169c30e16",
                  "手机号",
                  "@1@=8888",
                  ""
                  );


            //查询系统帐户信息
            // result = RCSCloudRestAPI.queryUser();

            //查询短信帐号下的所有模板
            // result = RCSCloudRestAPI.queryTpls();


            //查询单个模板
            // result = RCSCloudRestAPI.queryTemplate("");

            //查询上行信息
            // result = RCSCloudRestAPI.queryMo();


            //黑名单校验
            // result = RCSCloudRestAPI.validBL("18013839478");

            //检测敏感词
            //result = RCSCloudRestAPI.validSW("枪");

            //	查询短信报告
            // result = RCSCloudRestAPI.queryRpt();


            return View();
        }


        public class RCSCloudRestAPI
        {
            public RCSCloudRestAPI() { }

            /// <summary>
            /// 接口服务器IP地址（从会员中心--短信中心--帐号签名中获取）
            /// </summary>
            private const String SERVER_IP = "121.41.114.153"; //或使用域名 api.rcscloud.cn

            /// <summary>
            /// 接口服务器端口（从会员中心--短信中心--帐号签名中获取）
            /// </summary>
            private const String SERVER_PORT = "8030";

            /// <summary>
            /// 短信账号（从会员中心获取，请注意不是会员的登录帐号短信帐号以“ZH”开头）
            /// </summary>
            private const string ACCOUNT_SID = "ZH000000143";


            /// <summary>
            /// APIKEY（从会员中心--短信中心--帐号签名中获取，APIKEY可以自主在会员中心更新）
            /// </summary>
            private const string ACCOUNT_APIKEY = "3600d836-dcf3-42c7-9f31-a6d43098acfe";


            /// <summary>
            /// 发送模板短信
            /// </summary>
            /// <param name="templateId"></param>
            /// <param name="mobile">手机号码</param>
            /// <param name="content">短信内容</param>
            /// <param name="extno">扩展码（从会员中心帐号列表中获取）</param>
            /// <returns></returns>
            public static String sendTplSms(String templateId, String mobile, String content, String extno)
            {
                String resultJson = "";
                try
                {
                    //请求头部设置认证信息
                    string src = string.Format("{0}{1}{2}{3}{4}", ACCOUNT_SID, ACCOUNT_APIKEY, templateId, mobile, content);
                    string sign = Tool.GetMD5(src);
                    //短信发送的相关参数
                    StringBuilder dic = new StringBuilder();
                    dic.AppendFormat("&sid={0}", ACCOUNT_SID);
                    dic.AppendFormat("&sign={0}", sign);

                    dic.AppendFormat("&tplid={0}", templateId);
                    dic.AppendFormat("&mobile={0}", mobile);
                    dic.AppendFormat("&content={0}", content);
                    dic.AppendFormat("&extno={0}", extno);

                    //构建请求URL，所有url都必须包含sign、sid参数
                    string url = string.Format("http://{0}:{1}/rcsapi/rest/sms/sendtplsms.json", SERVER_IP, SERVER_PORT);

                    resultJson = Tool.SendHttpPost(url.ToString(), dic.ToString());

                }
                catch (Exception e)
                {
                    //异常处理
                }
                return resultJson;

            }


            /// <summary>
            /// 查询账号信息  GET 请求
            /// </summary>
            /// <returns></returns>
            public static String queryUser()
            {
                String resultJson = "";
                try
                {
                    //构建请求URL，所有url都必须包含sign、sid参数
                    //请求头部设置认证信息
                    string src = string.Format("{0}{1}", ACCOUNT_SID, ACCOUNT_APIKEY);
                    string sign = Tool.GetMD5(src);
                    string url = GetBaseUrl("user/get.json", sign);
                    return Tool.HttpGet(url);
                }
                catch (Exception e)
                {
                    //异常处理
                }

                return resultJson;
            }

            /// <summary>
            /// 查询账号下所有模板信息
            /// </summary>
            /// <returns>return json字符串,详细描述请参考接口文档 </returns>
            public static String queryTpls()
            {
                String resultJson = "";
                try
                {
                    //构建请求URL，所有url都必须包含sign、sid参数
                    //请求头部设置认证信息
                    string src = string.Format("{0}{1}", ACCOUNT_SID, ACCOUNT_APIKEY);
                    string sign = Tool.GetMD5(src);
                    string url = GetBaseUrl("tpl/gets.json", sign);
                    return Tool.HttpGet(url);
                }
                catch (Exception e)
                {
                    //异常处理
                }
                return resultJson;
            }

            /// <summary>
            /// 根据模板ID查询模板信息
            /// </summary>
            /// <param name="tplId"></param>
            /// <returns></returns>
            public static String queryTemplate(string tplId)
            {
                string resultJson = "";
                try
                {
                    //构建请求URL，所有url都必须包含sign、sid参数
                    //请求头部设置认证信息
                    string src = string.Format("{0}{1}{2}", ACCOUNT_SID, ACCOUNT_APIKEY, tplId);
                    string sign = Tool.GetMD5(src);
                    string url = GetBaseUrl("tpl/get.json", sign);
                    url = string.Format("{0}&tplid={1}", url, tplId);
                    return Tool.HttpGet(url);
                }
                catch (Exception e)
                {
                    //异常处理
                }
                return resultJson;
            }



            /// <summary>
            /// 获取上行短信，采用GET方式
            /// </summary>
            /// <returns>return json字符串,详细描述请参考接口文档 </returns>
            public static String queryMo()
            {
                String resultJson = "";
                try
                {
                    //构建请求URL，所有url都必须包含sign、sid参数
                    //请求头部设置认证信息
                    string src = string.Format("{0}{1}", ACCOUNT_SID, ACCOUNT_APIKEY);
                    string sign = Tool.GetMD5(src);
                    string url = GetBaseUrl("sms/querymo.json", sign);
                    return Tool.HttpGet(url);
                }
                catch (Exception e)
                {
                    //异常处理
                }
                return resultJson;

            }


            /// <summary>
            ///  校验黑名单，采用GET方式
            /// </summary>
            /// <param name="mobile"></param>
            /// <returns></returns>
            public static String validBL(string mobile)
            {
                string resultJson = "";
                try
                {
                    //构建请求URL，所有url都必须包含sign、sid参数
                    //请求头部设置认证信息
                    string src = string.Format("{0}{1}", ACCOUNT_SID, ACCOUNT_APIKEY);
                    string sign = Tool.GetMD5(src);
                    string url = GetBaseUrl("assist/bl.json", sign);
                    url = string.Format("{0}&mobile={1}", url, mobile);
                    return Tool.HttpGet(url);
                }
                catch (Exception e)
                {
                    //异常处理
                }
                return resultJson;
            }


            /**
             * 校验敏感词，采用GET方式
             * /assist/sw.json?sid={sid}& sign={sign}&content={content}
             * @param content 内容
             * @return json字符串,详细描述请参考接口文档
             */
            public static String validSW(String content)
            {
                String resultJson = "";
                try
                {
                    //构建请求URL，所有url都必须包含sign、sid参数
                    //请求头部设置认证信息
                    string src = string.Format("{0}{1}", ACCOUNT_SID, ACCOUNT_APIKEY);
                    string sign = Tool.GetMD5(src);
                    string url = GetBaseUrl("assist/sw.json", sign);
                    url = string.Format("{0}&content={1}", url, content);
                    return Tool.HttpGet(url);
                }
                catch (Exception e)
                {
                    //异常处理
                }
                return resultJson;
            }

            /// <summary>
            /// 查询状态报告
            /// </summary>
            /// <returns></returns>
            public static String queryRpt()
            {
                String resultJson = "";
                try
                {
                    //构建请求URL，所有url都必须包含sign、sid参数
                    //请求头部设置认证信息
                    string src = string.Format("{0}{1}", ACCOUNT_SID, ACCOUNT_APIKEY);
                    string sign = Tool.GetMD5(src);
                    string url = GetBaseUrl("sms/queryrpt.json", sign);
                    return Tool.HttpGet(url);
                }
                catch (Exception e)
                {
                    //异常处理
                }
                return resultJson;
            }
            #region 辅助方法
            /// <summary>
            /// 组合接口URL
            /// </summary>
            /// <param name="action">接口动作</param>
            /// <param name="timestamp">时间标记</param>
            /// <param name="isDevelop">接口类别:标记是测试环境与正式环境的接口</param>
            /// <returns></returns>
            private static string GetBaseUrl(string action, string sign)
            {
                //按照SID/TOKEN/TS顺序进行MD5加密
                // String sign = string.Format("{0}{1}{2}", ACCOUNT_SID, ACCOUNT_APIKEY, timestamp);
                // String signature = Tool.GetMD5(sign);

                string url = string.Format("http://{0}:{1}/rcsapi/rest/{2}?sid={3}&sign={4}", SERVER_IP, SERVER_PORT, action, ACCOUNT_SID, sign);

                return url;
            }
            #endregion

        }


        /// <summary>
        /// 辅助类
        /// </summary>
        public class Tool
        {

            public Tool()
            {
                //
                // TODO: 在此处添加构造函数逻辑
                //
            }


            //form post
            public static string FormPost(string url, string paramData)
            {
                WebClient webClient = null;
                try
                {
                    byte[] postData = Encoding.UTF8.GetBytes(paramData);

                    webClient = new WebClient();
                    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
                    webClient.Headers.Add("Content-Encoding", "utf-8");
                    // webClient.Headers.Add("Authorization", Authorization);

                    byte[] responseData = webClient.UploadData(url, "POST", postData);//得到返回字符流  
                    return Encoding.UTF8.GetString(responseData);//解码  
                }
                catch (Exception e)
                {
                    return "post error";
                }
                finally
                {
                    if (webClient != null)
                    {
                        webClient.Dispose();
                    }
                }
            }

            /// <summary>
            /// POST请求
            /// </summary>
            /// <param name="url">路径</param>
            ///  <param name="paramData">参数</param>
            /// <returns>请求结果</returns>
            public static string SendHttpPost(string url, string paramData)
            {
                Stream respStream = null;
                HttpWebResponse httpResp = null;
                StreamReader respStreamReader = null;

                HttpWebRequest httpReq = null;
                string res = null;
                try
                {


                    byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化

                    httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    httpReq.Method = "POST";
                    httpReq.ContentType = "application/x-www-form-urlencoded";
                    httpReq.Headers.Add("Content-Encoding", "utf-8");

                    httpReq.ContentLength = byteArray.Length;

                    respStream = httpReq.GetRequestStream();
                    respStream.Write(byteArray, 0, byteArray.Length);//写入参数
                    respStream.Close();
                    httpResp = (HttpWebResponse)httpReq.GetResponse();
                    respStreamReader = new StreamReader(httpResp.GetResponseStream(), Encoding.UTF8);
                    res = respStreamReader.ReadToEnd();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    if (respStream != null)
                    {
                        respStream.Close();
                    }

                    if (httpResp != null)
                    {
                        httpResp.Close();
                    }

                    if (respStreamReader != null)
                    {
                        respStreamReader.Close();
                    }
                }
                return res;
            }

            public static string HttpGet(string Url)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.Headers.Add("Content-Encoding", "utf-8");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }



            /// <summary>
            /// 获取大写的MD5签名结果
            /// </summary>
            /// <param name="encypStr"></param>
            /// <param name="charset"></param>
            /// <returns></returns>
            public static string GetMD5(string encypStr)
            {
                string charset = "utf-8";

                string retStr;
                MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

                //创建md5对象
                byte[] inputBye;
                byte[] outputBye;

                //使用GB2312编码方式把字符串转化为字节数组．
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
                outputBye = m5.ComputeHash(inputBye);

                retStr = System.BitConverter.ToString(outputBye);
                retStr = retStr.Replace("-", "").ToUpper();
                return retStr;
            }

            /**
             * 字符编码转换
             * @param str
             * @param newCharset
             * @return
             * @throws UnsupportedEncodingException
             * String
             */
            public static String changeCharset(string str, string newCharset)
            {
                if (string.IsNullOrEmpty(newCharset))
                {
                    newCharset = "gb2312";
                }

                if (str != null)
                {
                    //ISO-8859-1
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
                    return System.Text.Encoding.Unicode.GetString(bytes);
                }
                return null;

            }


            #region Base64加解密
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static string Base64Encode(string encode)
            {
                byte[] bytes = Encoding.Default.GetBytes(encode);
                return Convert.ToBase64String(bytes);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="decode"></param>
            /// <returns></returns>
            public static string Base64Decode(string decode)
            {
                byte[] outputb = Convert.FromBase64String(decode);
                return Encoding.Default.GetString(outputb);
            }

            #endregion
        }
    }
}
