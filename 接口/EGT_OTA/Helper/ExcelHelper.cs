using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Data.OleDb;

namespace EGT_OTA.Helper
{
    public class ExcelHelper
    {

        /// <summary> 
        /// 用流的方式，把内容以Table的格式向Excel中放数据 好处是 可以生成格式丰富复杂的Excel，页面无刷新
        /// </summary> 
        /// <param name="content">Excel中内容(Table格式)</param> 
        /// <param name="filename">文件名</param> 
        /// <param name="cssText">样式内容</param> 
        public static void ExportToExcel(string filename, string content, string cssText)
        {
            var res = HttpContext.Current.Response;
            content = String.Format("<style type='text/css'>{0}</style>{1}", cssText, content);
            res.Clear();
            res.Buffer = true;
            res.Charset = "UTF-8";
            res.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            res.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            res.ContentType = "application/ms-excel;charset=UTF-8";
            res.Write(content);
            res.Flush();
            res.End();
        }
        #region  导出Excel示例
        //方法调用
        //内容很好理解，只需当成Table来拼字符串即可 
        //private string getExcelContent() 
        //{ 
        //StringBuilder sb = new StringBuilder(); 
        //sb.Append("<table borderColor='black' border='1' >"); 
        //sb.Append("<thead><tr><th colSpan='2' bgColor='#ccfefe'>标题</th></tr>"); 
        //sb.Append("<tr><th bgColor='#ccfefe'>号码</th><th bgColor='#ccfefe'>名字</th></tr></thead>"); 
        //sb.Append("<tbody>"); 
        //sb.Append("<tr class='firstTR'><td bgcolor='#FF99CC'></td><td></td></tr>"); 
        //sb.Append("<tr class='secondTR'><td></td><td bgcolor='lightskyblue'></td></tr>"); 
        //sb.Append("</tbody></table>"); 
        //return sb.ToString(); 
        //} 

        //private void hidExport_Click(object sender, System.EventArgs e) 
        //{ 
        //string content = getExcelContent(); 
        //string css = ".firstTR td{color:blue;width:100px;}.secondTR td{color:blue;width:100px;}"; 
        //string filename = "Test.xls"; 
        //ExcelHelper.ExportToExcel(filename, content ,css); 
        //}
        //}
        #endregion

        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <param name="savePath">excel文件路径</param>
        /// <param name="filename">excel文件名</param>
        public static DataSet ExecleDs(string savePath, string filename)
        {
            string strConn = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source=" + savePath + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataSet ds = new DataSet();
            OleDbDataAdapter odda = new OleDbDataAdapter("select * from [Sheet1$]", conn);
            odda.Fill(ds, filename);
            conn.Close();
            return ds;
        }
        #region  读取Excel示例
        public class ExcelData { public string Mobile { get; set; } }
        public static List<ExcelData> ExcelDataList(string savePath, string filename)
        {
            try
            {
                DataSet ds = ExecleDs(savePath, filename);        //调用自定义方法
                DataRow[] dr = ds.Tables[0].Select();            //定义一个DataRow数组
                int rowsnum = ds.Tables[0].Rows.Count;

                List<ExcelData> usertemplist = new List<ExcelData>();
                if (rowsnum != 0)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {
                        ExcelData tmp = new ExcelData();
                        tmp.Mobile = dr[i]["Mobile"].ToString();
                        usertemplist.Add(tmp);
                    }
                }
                return usertemplist;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error(ex.Message, ex);
            }
            return null;
        }
        #endregion
    }
}

