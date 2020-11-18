using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Security.Policy;

namespace FIleControl
{
    class SQLiteHelper
    {
        //要保留的数据
        static string[] tbls = DocumentHelp.Instence.DontMoveTabels;// new string[] { "UserData", "Product", "Server" };
        //中转的字典
        static Dictionary<string, DataTable> tempDIC = new Dictionary<string, DataTable>();
        static string path = DocumentHelp.Instence.NewDataBasePath;//@"C:\Users\chen\source\repos\FIleControl\bin\Debug\DB File\sqliteData2.db";
        static string pathold = DocumentHelp.Instence.OldDataBasePath; //@"C:\Users\chen\source\repos\FIleControl\bin\Debug\DB File\sqliteData3.db";
        static SQLiteCommand cmd = new SQLiteCommand();


        public static void MergeDatabase()
        {
            //连接两个数据库
            using (SQLiteConnection cn = new SQLiteConnection("data source=" + path))
            {
                using (SQLiteConnection cn_old = new SQLiteConnection("data source=" + pathold))
                {
                    //打开数据库
                    cn.Open();
                    cn_old.Open();

                    foreach (var item in tbls)
                    {
                        DataTable dt = new DataTable();
                        List<string> columns = new List<string>();


                        //拿到旧表要保留的数据的列(除去主键)
                        cmd.Connection = cn_old;
                        cmd.CommandText = $"PRAGMA TABLE_INFO({item})";
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["pk"].ToString() == "0")
                            {
                                columns.Add(row["name"].ToString());
                            }
                        }


                        //拿到旧表数据
                        dt.Clear();
                        cmd.CommandText = $"Select * from {item}";
                        SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                        dataAdapter.Fill(dt);


                        //拼接SQL语句
                        string sqlcmd_columns = string.Join(",", columns);
                        List<string> tempList = new List<string>();
                        List<string> rowList = new List<string>();
                        foreach (DataRow row in dt.Rows)
                        {
                            tempList.Clear();
                            foreach (string col in columns)
                            {
                                tempList.Add(row[col].ToString() != "" ? $"'{row[col].ToString()}'" : "null");
                            }
                            rowList.Add($"({string.Join(",", tempList)})");
                        }
                        string sqlCmd_Rows = string.Join(",", rowList);


                        //先把表清空
                        cmd.Connection = cn;
                        cmd.CommandText = $"DELETE FROM {item}";
                        cmd.ExecuteNonQuery();

                        //执行写入操作
                        cmd.CommandText = $"INSERT INTO {item} ({sqlcmd_columns}) VALUES {sqlCmd_Rows}";
                        int affectRows = cmd.ExecuteNonQuery();

                        #region 打印执行状态
                        if (affectRows > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write($"表{item}更新成功\t");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"表{item}更新失败");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        #endregion

                    }


                }
            }
        }

    }
}
