using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Collections;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// 数据转换，DataSet与泛型互换
    /// </summary>
    public class CommDataHelper
    {
        public CommDataHelper() { }

        public static DataSet ToDataSet<T>(IList<T> p_List)
        {
            return ToDataSet<T>(p_List, null);
        }

        /// <summary> 
        /// 泛型集合转换DataSet 
        /// </summary> 
        /// <typeparam name="T">泛型集合的实体类名</typeparam> 
        /// <param name="p_List">泛型集合</param> 
        /// <param name="p_PropertyName">待转换属性名数组</param> 
        public static DataSet ToDataSet<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            DataSet result = new DataSet();
            DataTable _DataTable = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        // 没有指定属性的情况下全部属性都要转换 
                        _DataTable.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            _DataTable.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    _DataTable.LoadDataRow(array, true);
                }
            }
            result.Tables.Add(_DataTable);
            return result;
        }

        #region DataSet转换为List

        // DataSet装换为泛型集合 
        public static List<T> DataSetToList<T>(DataSet p_DataSet, int p_TableIndex)
        {
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return null;
            if (p_TableIndex > p_DataSet.Tables.Count - 1)
                return null;
            if (p_TableIndex < 0)
                p_TableIndex = 0;

            DataTable p_Data = p_DataSet.Tables[p_TableIndex];

            // 返回值初始化 
            List<T> result = new List<T>();
            for (int j = 0; j < p_Data.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < p_Data.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值 
                        if (pi.Name.Equals(p_Data.Columns[i].ColumnName))
                        {
                            //  DataRowCollection dataRowCollection = new DataRowCollection();
                            //dataRowCollection.Add(
                            // p_Data.Columns[i].DataType = pi.GetType(); 

                            // 数据库NULL值单独处理
                            string mm = p_Data.Rows[j][i].ToString();
                            if (p_Data.Rows[j][i] != DBNull.Value)
                            {
                                try
                                {
                                    pi.SetValue(_t, p_Data.Rows[j][i], null);
                                }
                                catch
                                {
                                    try
                                    {
                                        pi.SetValue(_t, Convert.ToInt32(p_Data.Rows[j][i].ToString()), null);
                                    }
                                    catch { }
                                }
                            }
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }

        // DataSet装换为泛型集合 
        public static List<T> DataSetToList<T>(DataSet p_DataSet, string p_TableName)
        {
            int _TableIndex = 0;
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return null;
            if (string.IsNullOrEmpty(p_TableName))
                return null;
            for (int i = 0; i < p_DataSet.Tables.Count; i++)
            {
                // 获取Table名称在Tables集合中的索引值 
                if (p_DataSet.Tables[i].TableName.Equals(p_TableName))
                {
                    _TableIndex = i;
                    break;
                }
            }
            return DataSetToList<T>(p_DataSet, _TableIndex);
        }

        #endregion

        #region  DataSet转换为IList

        /// <summary> 
        /// DataSet转换为泛型集合 
        /// </summary> 
        /// <typeparam name="T">泛型集合的实体类名</typeparam> 
        /// <param name="p_DataSet">DataSet</param> 
        /// <param name="p_TableIndex">待转换数据表索引</param> 
        public static IList<T> DataSetToIList<T>(DataSet dataSet, int tableIndex)
        {
            if (dataSet == null || dataSet.Tables.Count < 0)
                return null;
            if (tableIndex > dataSet.Tables.Count - 1)
                return null;
            if (tableIndex < 0)
                tableIndex = 0;
            DataTable p_Data = dataSet.Tables[tableIndex];
            IList<T> result = new List<T>();///返回值初始化 
            for (int j = 0; j < p_Data.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < p_Data.Columns.Count; i++)
                    {
                        ///属性与字段名称一致的进行赋值 
                        if (pi.Name.Equals(p_Data.Columns[i].ColumnName))
                        {
                            if (p_Data.Rows[j][i] != DBNull.Value)
                                pi.SetValue(_t, ChangeType(p_Data.Rows[j][i], pi.PropertyType), null);
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }

        /// <summary> 
        /// DataSet转换为泛型集合 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="dataSet">DataSet</param> 
        /// <param name="tableName">待转换数据表名称</param> 
        public static IList<T> DataSetToIList<T>(DataSet dataSet, string tableName)
        {
            int tableIndex = 0;
            if (dataSet == null || dataSet.Tables.Count < 0)
                return null;
            if (String.IsNullOrEmpty(tableName))
                return null;
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                ///获取Table名称在Tables集合中的索引值 
                if (dataSet.Tables[i].TableName.Equals(tableName))
                {
                    tableIndex = i;
                    break;
                }
            }
            return DataSetToIList<T>(dataSet, tableIndex);
        }

        #endregion

        /// <summary>
        /// 返回指定类型的对象
        /// </summary>
        public static object ChangeType(object value, Type type)
        {
            if (value == null && type.IsGenericType)
                return Activator.CreateInstance(type);
            if (value == null)
                return null;
            if (type == value.GetType())
                return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid))
                return new Guid(value as string);
            if (value is string && type == typeof(Version))
                return new Version(value as string);
            if (!(value is IConvertible))
                return value;
            object ss = Convert.ChangeType(value, type);
            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// JSON转化为DataTable
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static DataTable JsonToDataSetNet(string Json)
        {
            try
            {
                Newtonsoft.Json.Linq.JArray rows = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(Json);
                DataTable dt = new DataTable();
                foreach (var row in rows)
                {
                    Newtonsoft.Json.Linq.JToken val = row;
                    DataRow dr = dt.NewRow();
                    foreach (Newtonsoft.Json.Linq.JProperty sss in val)
                    {
                        if (!dt.Columns.Contains(sss.Name))
                        {
                            dt.Columns.Add(sss.Name);
                            dr[sss.Name] = sss.Value;
                        }
                        else
                            dr[sss.Name] = sss.Value;
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch
            {
                return null;
            }
        }
    }
}
