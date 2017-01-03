using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// 邮件发送
    /// </summary>
    public static class MailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sub">邮件主题</param>
        /// <param name="message">邮件内容</param>
        /// <param name="fromUser">发送人实体类</param>
        public static void SendMail(string sub, string message, FromUserModel fromUser)
        {
            SmtpClient smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false,
                Host = ConfigGetter.mailHost,
                Port = 0x19,
                Credentials = new NetworkCredential(fromUser.UserID, CoderMaker.Decode(fromUser.UserPwd))
            };
            MailMessage mm = new MailMessage
            {
                Priority = MailPriority.High,
                From = new MailAddress(fromUser.UserID, fromUser.UserName, Encoding.GetEncoding(0x3a8))
            };
            for (int i = 0; i < fromUser.ToUserArray.Length; i++)
            {
                try
                {
                    mm.To.Add(new MailAddress(fromUser.ToUserArray[i].UserID, fromUser.ToUserArray[i].UserName, Encoding.GetEncoding(936)));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            mm.Subject = sub;
            mm.SubjectEncoding = Encoding.GetEncoding(0x3a8);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = Encoding.GetEncoding(0x3a8);
            mm.Body = message;
            try
            {
                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 收件人
    /// </summary>
    public class ToUserModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
    }

    /// <summary>
    /// 发件人
    /// </summary>
    public class FromUserModel
    {
        public string UserID { get; set; }
        public string UserPwd { get; set; }
        public string UserName { get; set; }
        public ToUserModel[] ToUserArray { get; set; }
    }

    public static class CoderMaker
    {
        public static string Encode(string data)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public static string Decode(string data)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }
    }

    public static class ConfigGetter
    {
        //邮箱smtp
        public static string mailHost = "smtp.qiye.163.com";
    }
}
