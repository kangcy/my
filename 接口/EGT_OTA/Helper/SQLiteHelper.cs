using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGT_OTA.Helper
{
    /// <summary>
    /// C#数据本地存储方案之SQLite
    /// </summary>
    //public class SQLiteHelper
    //{
    //    /// <summary>
    //    /// ConnectionString样例：Datasource=Test.db3;Pooling=true;FailIfMissing=false
    //    /// </summary>
    //    public static string ConnectionString { get; set; }

    //    private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string cmdText, params object[] p)
    //    {
    //        if (conn.State != ConnectionState.Open)
    //            conn.Open();
    //        cmd.Parameters.Clear();
    //        cmd.Connection = conn;
    //        cmd.CommandText = cmdText;
    //        cmd.CommandType = CommandType.Text;
    //        cmd.CommandTimeout = 30;
    //        if (p != null)
    //        {
    //            foreach (object parm in p)
    //                cmd.Parameters.AddWithValue(string.Empty, parm);
    //        }
    //    }

    //    public static DataSet ExecuteQuery(string cmdText, params object[] p)
    //    {
    //        using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
    //        {
    //            using (SQLiteCommand command = new SQLiteCommand())
    //            {
    //                DataSet ds = new DataSet();
    //                PrepareCommand(command, conn, cmdText, p);
    //                SQLiteDataAdapter da = new SQLiteDataAdapter(command);
    //                da.Fill(ds);
    //                return ds;
    //            }
    //        }
    //    }

    //    public static int ExecuteNonQuery(string cmdText, params object[] p)
    //    {
    //        using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
    //        {
    //            using (SQLiteCommand command = new SQLiteCommand())
    //            {
    //                PrepareCommand(command, conn, cmdText, p);
    //                return command.ExecuteNonQuery();
    //            }
    //        }
    //    }

    //    public static SQLiteDataReader ExecuteReader(string cmdText, params object[] p)
    //    {
    //        using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
    //        {
    //            using (SQLiteCommand command = new SQLiteCommand())
    //            {
    //                PrepareCommand(command, conn, cmdText, p);
    //                return command.ExecuteReader(CommandBehavior.CloseConnection);
    //            }
    //        }
    //    }

    //    public static object ExecuteScalar(string cmdText, params object[] p)
    //    {
    //        using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
    //        {
    //            using (SQLiteCommand command = new SQLiteCommand())
    //            {
    //                PrepareCommand(command, conn, cmdText, p);
    //                return command.ExecuteScalar();
    //            }
    //        }
    //    }
    //}
}
