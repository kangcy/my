using System;
using System.Collections.Generic;
using Microsoft.Security.Application;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// XSS攻击
    /// </summary>
    public class AntiXssChineseString
    {
        private static Dictionary<string, string> dic = new Dictionary<string, string>();
        /// <summary>
        /// 微软的AntiXSS v4.0 让部分汉字乱码,这里将乱码部分汉字转换回来
        /// </summary>
        /// <param name="hc输入值"></param>
        /// <returns></returns>
        public static string ChineseStringSanitize(string chineseString)
        {
            chineseString = Sanitizer.GetSafeHtmlFragment(chineseString);
            string hbString_ReturnValue = chineseString;
            hbString_ReturnValue = hbString_ReturnValue.Replace("\r\n", "");//避免出现<br>等标签后被认为加上\r\n的换行符,这会出现在多行textbox控件中,不需要的人请注释这一行代码
            if (hbString_ReturnValue.Contains("&#"))
            {
                //Dictionary如果没有内容就初始化内容
                if (dic.Keys.Count == 0)
                {
                    lock (dic)
                    {
                        if (dic.Keys.Count == 0)
                        {
                            dic.Clear();//防止多线程情况下的不安全情况,双重检查理论很完美,但是在多处理器,多线程下,会有平台漏洞,原因是乱序写入这一cpu或系统功能的存在
                            dic.Add("&#20028;", "丼");
                            dic.Add("&#20284;", "似");
                            dic.Add("&#20540;", "值");
                            dic.Add("&#20796;", "儼");
                            dic.Add("&#21052;", "刼");
                            dic.Add("&#21308;", "匼");
                            dic.Add("&#21564;", "吼");
                            dic.Add("&#21820;", "唼");
                            dic.Add("&#22076;", "嘼");
                            dic.Add("&#22332;", "圼");
                            dic.Add("&#22588;", "堼");
                            dic.Add("&#23612;", "尼");
                            dic.Add("&#26684;", "格");
                            dic.Add("&#22844;", "夼");
                            dic.Add("&#23100;", "娼");
                            dic.Add("&#23356;", "嬼");
                            dic.Add("&#23868;", "崼");
                            dic.Add("&#24124;", "帼");
                            dic.Add("&#24380;", "弼");
                            dic.Add("&#24636;", "怼");
                            dic.Add("&#24892;", "愼");
                            dic.Add("&#25148;", "戼");
                            dic.Add("&#25404;", "挼");
                            dic.Add("&#25660;", "搼");
                            dic.Add("&#25916;", "攼");
                            dic.Add("&#26172;", "昼");
                            dic.Add("&#26428;", "朼");
                            dic.Add("&#26940;", "椼");
                            dic.Add("&#27196;", "樼");
                            dic.Add("&#27452;", "欼");
                            dic.Add("&#27708;", "氼");
                            dic.Add("&#27964;", "洼");
                            dic.Add("&#28220;", "渼");
                            dic.Add("&#28476;", "漼");
                            dic.Add("&#28732;", "瀼");
                            dic.Add("&#28988;", "焼");
                            dic.Add("&#29244;", "爼");
                            dic.Add("&#29500;", "猼");
                            dic.Add("&#29756;", "琼");
                            dic.Add("&#30012;", "甼");
                            dic.Add("&#30268;", "瘼");
                            dic.Add("&#30524;", "眼");
                            dic.Add("&#30780;", "砼");
                            dic.Add("&#31036;", "礼");
                            dic.Add("&#31292;", "稼");
                            dic.Add("&#31548;", "笼");
                            dic.Add("&#31804;", "簼");
                            dic.Add("&#32060;", "紼");
                            dic.Add("&#32316;", "縼");
                            dic.Add("&#32572;", "缼");
                            dic.Add("&#32828;", "耼");
                            dic.Add("&#33084;", "脼");
                            dic.Add("&#33340;", "舼");
                            dic.Add("&#33596;", "茼");
                            dic.Add("&#33852;", "萼");
                            dic.Add("&#34108;", "蔼");
                            dic.Add("&#36156;", "贼");
                            dic.Add("&#39740;", "鬼");
                        }
                    }
                }
                //开始替换的遍历
                foreach (string key in dic.Keys)
                {
                    if (hbString_ReturnValue.Contains(key))
                    {
                        hbString_ReturnValue = hbString_ReturnValue.Replace(key, dic[key]);
                    }
                }
            }
            return hbString_ReturnValue;
        }
    }
}
