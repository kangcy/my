using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public class ReflectHelper
    {
        /// <summary>
        /// 通过数据行填充实体类型
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <param name="dRow">数据行</param>
        public static void FillInstanceValue(object model, DataRow dRow)
        {
            Type type = model.GetType();
            ///循环行
            for (int i = 0; i < dRow.Table.Columns.Count; i++)
            {
                ///搜索指定名称的公共属性
                PropertyInfo property = type.GetProperty(dRow.Table.Columns[i].ColumnName);
                if (property != null)
                {
                    ///给实体的这个属性赋值
                    property.SetValue(model, dRow[i], null);
                }
            }
        }

        /// <summary>
        /// 通过数据只读器填充实体类型
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <param name="dr">数据只读器</param>
        public static void FillInstanceValue(object model, IDataReader dr)
        {
            Type type = model.GetType();
            ///循环列
            for (int i = 0; i < dr.FieldCount; i++)
            {
                ///搜索指定名称的公共属性
                PropertyInfo property = type.GetProperty(dr.GetName(i));
                if (property != null)
                {
                    ///给实体的这个属性赋值
                    property.SetValue(model, dr[i], null);
                }
            }
        }

        /// <summary>
        /// 获取实体相关属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetInstanceValue(object obj, string propertyName)
        {
            object objRet = null;
            if (!String.IsNullOrEmpty(propertyName))
            {
                ///从实体的属性集合中获取该属性
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(obj).Find(propertyName, true);
                if (descriptor != null)
                {
                    objRet = descriptor.GetValue(obj);
                }
            }
            return objRet;
        }
    }

    /// <summary>
    /// 使用CodeDomProvider（.net 3.5 及以后）或是CSharpCodeProvider（.net 2及以后）动态编译代码
    /// 再使用反射获取类,动态创建类对象,然后获取函数对象,调用方法
    /// </summary>
    public class ReflectHelper2
    {
        const string code = @"public class MyClass
                              {
                                   public string Test()
                                   {
                                       return ""测试方法"";
                                   }
                              }";

        public string Test()
        {
            //编译参数
            var objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            using (CodeDomProvider provider = new CSharpCodeProvider())
            {
                //编译
                var result = provider.CompileAssemblyFromSource(objCompilerParameters, code);
                if (result.Errors.HasErrors)
                    throw new ApplicationException("编译错误");

                //反射获取创建类对象
                var myClass = result.CompiledAssembly.CreateInstance("MyClass");
                //获取方法对象
                var method = myClass.GetType().GetMethod("Test");
                //调用方法
                var obj = method.Invoke(myClass, new object[0]);
                return string.Format("返回结果：{0} # {1}", obj.GetType(), obj);
            }
        }
    }

    public class ReflectHelper3
    {
        public string name1 { get; set; }
        public string name2 { get; set; }

        public ReflectHelper3()
        {
            this.name1 = "康";
            this.name2 = "春阳";
        }

        public ReflectHelper3(string str1, string str2)
        {
            this.name1 = str1;
            this.name2 = str2;
        }

        public string WriteString(string name)
        {
            return "欢迎您：" + name;
        }

        public static string WriteName(string name)
        {
            return "欢迎您：" + name;
        }

        public string WriteNoPara()
        {
            return "您使用的是无参方法";
        }

        private string WritePrivate()
        {
            return "您使用的是私有方法";
        }
    }

    public class Test
    {
        delegate string TestDelegate(string value1, string value2);
        static void Main()
        {
            Type a = typeof(EGT_OTA.Helper.ReflectHelper2);
            //创建指定类型的实例
            object obj = Activator.CreateInstance(a, new string[] { "康", "春阳" });
            MethodInfo mi = a.GetMethod("WriteName");

            //带参数方法调用
            string s = (string)mi.Invoke(obj, new string[] { "康", "春阳" });

            //不带参数方法调用
            string s1 = (string)mi.Invoke(obj, null);

            //静态方法调用
            string s2 = (string)mi.Invoke(null, null);

            //不带参数方法调用
            MethodInfo mi3 = a.GetMethod("WriteNoPara");
            string s3 = (string)mi3.Invoke(obj, null);

            //私有类型方法调用
            MethodInfo mi4 = a.GetMethod("WritePrivate", BindingFlags.Instance | BindingFlags.NonPublic);
            string s4 = (string)mi4.Invoke(obj, null);
        }
    }
}
