using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Security.Policy;
using System.Runtime;
using System.Threading;

namespace FIleControl
{
    class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static string[] _dontCopyFile = DocumentHelp.Instence.DontMoveFiles;//new string[] { "HMSLaser.dll", "HMSLaser.exe", "HMSDrawTool.exe", "Hardware.dll" };
        private static string _targetDirectory = DocumentHelp.Instence.TargetPath;// "D:/Z1/Release/";
        static void Main(string[] args)
        {
            logger.Info("Software updates");

            //程序运行
            Console.BackgroundColor = ConsoleColor.Cyan;
            Print("上位机更新V2.0\n\n", ConsoleColor.Black);
            Console.BackgroundColor = ConsoleColor.Black;

            Print("程序开始执行\n", ConsoleColor.Yellow);


            //当前文件夹信息
            DirectoryInfo currectDeirectory = new DirectoryInfo(DocumentHelp.Instence.SourceDirectory);
            FileInfo[] fileInfos = currectDeirectory.GetFiles();

            #region 打印文件列表
            //Console.WriteLine("文件列表:");
            //foreach (var item in fileInfos)
            //{
            //    Console.WriteLine(item.Name);
            //}
            //Console.WriteLine("\n\n");
            #endregion


            //把旧文件备份一下
            Print("正在备份原文件...\n", ConsoleColor.Yellow);
            Console.ForegroundColor = ConsoleColor.Blue;
            BackUpFile2_0(DocumentHelp.Instence.TargetPath);
            Print("原文件备份成功!\n", ConsoleColor.Green);


            //合并数据库
            Print("正在合并数据库...!\n", ConsoleColor.Yellow);
            SQLiteHelper.MergeDatabase();



            Print("\n开始更新...\n", ConsoleColor.Yellow);
            //覆盖数据库
            File.Copy(DocumentHelp.Instence.NewDataBasePath, DocumentHelp.Instence.OldDataBasePath, true);
            //执行复制文件操作
            if (CopyFile(fileInfos))
            {
                Print("Copy Done!\n", ConsoleColor.Green);
            }
            else
            {
                Print("程序执行失败,发生错误!", ConsoleColor.Red);
            }

            Print("窗口两秒后关闭   ");
            Print("3..");
            Thread.Sleep(700);
            Print("2..");
            Thread.Sleep(700);
            Print("1..");
            Thread.Sleep(701);
            Print("0"); 

        }


        private static void Print(string msg, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(msg);
        }
        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="path"></param>
        private static void BackUpFile(string path)
        {

            //先复制文件
            FileInfo[] files = new DirectoryInfo(path).GetFiles();
            if (files.Length > 0)
            {
                DirectoryInfo directory = new DirectoryInfo(files[0].Directory.FullName.Replace("Release", "备份\\Release" + DateTime.Now.ToString("yy年MM月dd日")));
                if (!directory.Exists)
                {
                    directory.Create();
                }
                else
                {
                    directory.Delete(true);
                    directory.Create();
                }
                foreach (var item in files)
                {
                    string newpath = item.FullName.Replace("Release", "备份\\Release" + DateTime.Now.ToString("yy年MM月dd日"));
                    File.Copy(item.FullName, newpath, true);
                }
            }
            //再复制文件夹
            DirectoryInfo[] directoryInfos = new DirectoryInfo(path).GetDirectories();
            if (directoryInfos.Length > 0)
            {
                foreach (DirectoryInfo item in directoryInfos)
                {
                    BackUpFile(item.FullName);
                }
            }
        }


        /// <summary>
        /// 带进度条的备份文件夹
        /// </summary>
        /// <param name="path">路径</param>
        private static void BackUpFile2_0(string path)
        {
            FileInfo[] files = new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories);

            DirectoryInfo[] dirs = new DirectoryInfo(path).GetDirectories("*", SearchOption.AllDirectories);


            foreach (var item in dirs)
            {
                DirectoryInfo dir = new DirectoryInfo(item.FullName.Replace("Release", "备份\\Release" + DateTime.Now.ToString("yy年MM月dd日")));
                if (!dir.Exists)
                {
                    dir.Create();
                }
                else
                {
                    dir.Delete(true);
                    dir.Create();
                }
            }

            int top = Console.CursorTop;
            int left = Console.CursorLeft;
            //先画个边界
            Console.SetCursorPosition(0, top);
            Console.Write("[");
            Console.SetCursorPosition(52, top);
            Console.Write("]");
            for (int i = 0; i < files.Length; i++)
            {
                string newpath = files[i].FullName.Replace("Release", "备份\\Release" + DateTime.Now.ToString("yy年MM月dd日"));
                File.Copy(files[i].FullName, newpath, true);


                //进度条
                double numPercentage = ((i + 1) * 1.0 / files.Length);
                int position = (int)(numPercentage * 50);
                Console.SetCursorPosition(position + 1, top);
                Console.Write("*");


                Console.SetCursorPosition(54, top);
                Console.Write($"{numPercentage.ToString("p")}");
            }
        }


        /// <summary>
        /// 执行复制操作
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <returns></returns>
        private static bool CopyFile(FileInfo[] fileInfos)
        {
            Directory.CreateDirectory(_targetDirectory);
            foreach (var item in fileInfos)
            {
                if (!_dontCopyFile.Contains(item.Name))
                {
                    File.Copy(item.FullName, _targetDirectory + item.Name, true);
                }
                else
                {
                    if (!File.Exists(_targetDirectory + item.Name))
                        File.Copy(item.FullName, _targetDirectory + item.Name);
                }
            }
            return true;
        }
    }
}
