using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Web;
using System.IO;
using System.Drawing.Imaging;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// 随机生成验证码（英文+数字）
    /// </summary>
    public class ValidateCodeHelper
    {
        #region  使用方法
        /// <summary>
        /// 生成验证码并输出
        /// </summary>
        protected void BuildValidateCode()
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                throw new Exception("当前请求不存在。");
            }
            Bitmap bmp = new Bitmap(50, 25);
            Graphics g = Graphics.FromImage(bmp);
            SolidBrush sb = new SolidBrush(getColor());
            g.DrawString(BuildCode(5), new Font("宋体", 16), sb, 0, 0);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            context.Response.Clear();
            context.Response.ContentType = "image/Png";
            context.Response.BinaryWrite(ms.GetBuffer());
            bmp.Dispose();
            ms.Dispose();
            ms.Close();
            context.Response.End();
        }
        #endregion

        /// <summary>
        /// 随机生成验证码
        /// </summary>
        /// <param name="len">生成验证码长度</param>
        public static string BuildCode(int len)
        {
            string checkcode = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            ///验证码
            string tmpStr = "";
            int iRandNum;
            Random rnd = new Random();
            for (int i = 0; i < len; i++)
            {
                iRandNum = rnd.Next(checkcode.Length);
                tmpStr += checkcode[iRandNum];
            }
            return tmpStr;
        }

        /// <summary>
        /// 随机生成背景色
        /// </summary>
        public static Color getColor()
        {
            Random r = new Random();
            return Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
        }
    }
}
