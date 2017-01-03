using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;

namespace EGT_OTA.Models
{
    public abstract class EnumBase
    {
        //EnumBase.GetDescription(typeof(Enum_Subject_Type), Convert.ToInt32(id));

        //遍历方法
        //Dictionary<int, string> dic = EnumBase.GetDictionary(typeof(Enum_ModuleType));
        //foreach (int key in dic.Keys)
        //{
        //    str += key + dic[key] + "$";
        //}
        //Response.Write(str);

        /// <summary>
        /// 获取所有字段值和说明信息的列表
        /// </summary>
        public static Dictionary<int, string> GetDictionary(Type type)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            //返回所有公共字段
            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                dictionary.Add(int.Parse(field.GetValue(type).ToString()), GetDescription(field));
            }
            return dictionary;
        }

        public static Dictionary<string, string> GetDictionaryString(Type type)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                dictionary.Add(field.GetValue(type).ToString(), GetDescription(field));
            }
            return dictionary;
        }



        /// <summary>
        /// 获取字段说明信息
        /// </summary>
        public static string GetDescription(Type type, int value)
        {
            //返回所有公共字段
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (int.Parse(field.GetValue(type).ToString()) == value) return GetDescription(field);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取字段名称信息
        /// </summary>
        public static string GetName(Type type, int value)
        {
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (int.Parse(field.GetValue(type).ToString()) == value) return field.Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取字段说明信息
        /// </summary>
        public static string GetDescription(FieldInfo field)
        {
            //获取自定义属性 EnumAttribute
            object[] attributes = field.GetCustomAttributes(false);

            string defaultAttribute = field.Name;

            if (attributes.Length > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    EnumAttribute attr = attributes[i] as EnumAttribute;

                    if (string.IsNullOrEmpty(attr.Culture)) defaultAttribute = attr.Description;

                    if (string.Compare(attr.Culture, Thread.CurrentThread.CurrentUICulture.Name, true) == 0) return attr.Description;
                }
            }
            return defaultAttribute;
        }
    }
}
