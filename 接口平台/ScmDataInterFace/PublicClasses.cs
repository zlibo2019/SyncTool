using System;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Threading;
using System.Data.Common;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Net;
using System.Web.Services.Description;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.Configuration;

namespace ScmDataInterFace
{
    #region 应用设置
    /// <summary>
    /// 应用设置
    /// </summary>
    public class ApplicationSetting
    {
        /// <summary>
        /// 设置参数集合
        /// </summary>
        public static Dictionary<string, string> Setting = new Dictionary<string, string>();
        /// <summary>
        /// 设置键与数值

        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">数值</param>
        public static void SetValue(string key, string value)
        {
            if (Setting.ContainsKey(key)) Setting[key] = value;
            else Setting.Add(key, value);
        }
        /// <summary>
        /// 保存设置
        /// </summary>
        public static void SaveSetting()
        {
            IsolatedStorageFile storFile = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream storStream = new IsolatedStorageFileStream("WEDS_Config.xml",
                FileMode.Create, FileAccess.Write);
            XmlTextWriter writer = new XmlTextWriter(storStream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("Setting");
            foreach (string var in Setting.Keys)
            {
                writer.WriteStartElement(var);
                writer.WriteValue(Setting[var]);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();

            storStream.Close();
            storFile.Close();
        }
        /// <summary>
        /// 读取设置
        /// </summary>
        public static void ReadSetting()
        {
            string filename = "WEDS_Config.xml";
            IsolatedStorageFile storFile = IsolatedStorageFile.GetUserStoreForDomain();
            string[] userFiles = storFile.GetFileNames(filename);
            foreach (string userFile in userFiles)
            {
                if (userFile == filename)
                {
                    IsolatedStorageFileStream storStream = new IsolatedStorageFileStream(
                        filename, FileMode.Open, storFile);
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(storStream);
                    XmlElement rootelement = xmldoc.DocumentElement;
                    XmlNodeList NodeList = rootelement.ChildNodes;
                    foreach (XmlNode node in NodeList)
                        SetValue(node.Name, node.InnerText);
                    storStream.Close();
                }
            }
            storFile.Close();
        }

        public static string initLanguageBase()
        {
            string language = System.Globalization.CultureInfo.InstalledUICulture.Name;
            if (language.ToLower().Contains("en"))  //提示信息默认语言为英文
            {
                return "";
            }
            NameValueCollection nvcLanguage = (NameValueCollection)ConfigurationManager.GetSection("language-" + language);
            if (nvcLanguage == null)
            {
                return "can't find font:'language-" + language + "'";
            }
            foreach (String str in nvcLanguage.AllKeys)
            {
                SetValue(str, nvcLanguage[str]);
            }
            return "";
        }

        public static string Translate(string key)
        {
            if (Setting.ContainsKey(key))
            {
                return Setting[key];
            }
            else
            {
                return key;
            }
        }
    }
    #endregion

    #region Miscellaneous 杂项
    /// <summary>
    /// 杂项
    /// </summary>
    public sealed class Miscellaneous
    {
        //public static List<string[]> lsShowInfo = new List<string[]>();
        //public static List<string> lsShowInfo = new List<string>();
        public static List<Array> lsShowInfo = new List<Array>();
        public static bool IsLocked = false;
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string MD5hashString(string data)
        {
            // This is one implementation of the abstract class MD5.
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(result).Replace("-", "").ToLower();
        }
        /// <summary>
        /// 复制源文件夹中子文件夹及文件到目标文件夹下
        /// </summary>
        /// <param name="source">源文件夹</param>
        /// <param name="destination">目标文件夹</param>
        public static bool CopyFullDirectory(string source, string destination)
        {
            bool lb_ret = false;
            try
            {
                if (!Directory.Exists(source)) return false;
                if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
                DirectoryInfo rootdir = new DirectoryInfo(source);

                //遍历文件   
                FileInfo[] fileinfo = rootdir.GetFiles();
                foreach (FileInfo file in fileinfo)
                {
                    try
                    {
                        string ls_filename = destination + "\\" + file.Name;
                        if (File.Exists(ls_filename)) File.SetAttributes(ls_filename, FileAttributes.Normal);
                        file.CopyTo(ls_filename, true);
                    }
                    catch { }
                }

                //递归   
                DirectoryInfo[] childdir = rootdir.GetDirectories();
                foreach (DirectoryInfo dir in childdir)
                {
                    try { CopyFullDirectory(dir.FullName, destination + "\\" + dir.Name); }
                    catch { }
                }
                lb_ret = true;
            }
            catch { lb_ret = false; }
            return lb_ret;
        }
        /// <summary>
        /// 执行可执行程序
        /// </summary>
        /// <param name="ProccessName">可执行程序名</param>
        /// <param name="par">调用可执行程序时传的参数</param>
        /// <returns></returns>
        public static string RunExe(string ProccessName, string par)
        {
            string sResult = string.Empty;
            try
            {
                string path = Application.StartupPath + ProccessName;// @"D:\项目文件\webservice接口服务\website_client\WindowsFormsApplication3\WindowsFormsApplication3\bin\Debug\WindowsFormsApplication3.exe";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = ProccessName;
                process.StartInfo.WorkingDirectory = Application.StartupPath;
                process.StartInfo.Arguments = par;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                sResult = ex.Message;
            }
            return sResult;
        }
        /// <summary>
        /// 删除文件夹中子文件夹的文件

        /// </summary>
        /// <param name="source">源文件夹</param>
        /// <returns>成功失败</returns>
        public static bool RemoveDirectory(string source)
        {
            bool lb_ret = false;
            try
            {
                if (!Directory.Exists(source)) return false;
                DirectoryInfo rootdir = new DirectoryInfo(source);

                //遍历文件   
                FileInfo[] fileinfo = rootdir.GetFiles();
                foreach (FileInfo file in fileinfo)
                {
                    try
                    {
                        file.Attributes = FileAttributes.Normal;
                        file.Delete();
                    }
                    catch { }
                }

                //递归   
                DirectoryInfo[] childdir = rootdir.GetDirectories();
                foreach (DirectoryInfo dir in childdir)
                {
                    try { RemoveDirectory(dir.FullName); }
                    catch { }
                }
                Directory.Delete(source, true);
                lb_ret = true;
            }
            catch { lb_ret = false; }
            return lb_ret;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="as_text">文本行</param>
        public static void Write2List(string[] arr_text)
        {
            List<string> lsTmp = new List<string>(arr_text);
            lsTmp.Add(DateTime.Now.ToString());
            lsShowInfo.Add(lsTmp.ToArray());
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="as_text">文本行</param>
        public static void Write2Log(string as_text)
        {
            string ls_path = Application.StartupPath + @"\" + DateTime.Now.ToString("yyyyMM");
            ls_path = ls_path.Replace(@"\\", @"\");
            string ls_file = ls_path + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            ls_file = ls_file.Replace(@"\\", @"\");

            if (!Directory.Exists(ls_path)) Directory.CreateDirectory(ls_path);
            FileMode fm = (!File.Exists(ls_file)) ? FileMode.OpenOrCreate : FileMode.Append;
            FileStream fs = new FileStream(ls_file, fm, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
            string sShowInfo = as_text;//DateTime.Now.ToLongTimeString() + "  " + as_text;
            sw.WriteLine(sShowInfo);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public static string md35(string strPwd)
        {
            return Encrypt(Encrypt(Encrypt(strPwd)));
        }

        public static string Encrypt(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length - 1; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

    }
    #endregion

    #region Project 项目的静态变量

    /// <summary>
    /// 项目类

    /// </summary>
    public sealed class Project
    {
        /// <summary>
        /// 字段：项目编号
        /// </summary>
        static string _Project_ID = string.Empty;
        /// <summary>
        /// 属性：项目编号
        /// </summary>
        public static string Project_ID
        {
            get { return Project._Project_ID; }
            set { Project._Project_ID = value; }
        }        /// <summary>
        /// 字段：项目名称
        /// </summary>
        static string _Project_Name = string.Empty;
        /// <summary>
        /// 属性：项目名称
        /// </summary>
        public static string Project_Name
        {
            get { return Project._Project_Name; }
            set { Project._Project_Name = value; }
        }
        /// <summary>
        /// 字段：我方数据库类型
        /// </summary>
        static string _DB_Type = string.Empty;
        /// <summary>
        /// 属性：我方数据库类型
        /// </summary>
        public static string DB_Type
        {
            get { return Project._DB_Type; }
            set { Project._DB_Type = value; }
        }
        /// <summary>
        /// 字段：对方数据库类型
        /// </summary>
        static string _TheirDB_Type = string.Empty;
        /// <summary>
        /// 属性：对方数据库类型
        /// </summary>
        public static string TheirDB_Type
        {
            get { return Project._TheirDB_Type; }
            set { Project._TheirDB_Type = value; }
        }
        /// <summary>
        /// 字段：我方数据库连接串
        /// </summary>
        static string _DB_Connection = string.Empty;
        /// <summary>
        /// 属性：我方数据库连接串
        /// </summary>
        public static string DB_Connection
        {
            get { return Project._DB_Connection; }
            set { Project._DB_Connection = value; }
        }

        static string _DB_Alias = string.Empty;
        public static string DB_Alias
        {
            get { return Project._DB_Alias; }
            set { Project._DB_Alias = value; }
        }

        static string _theirDB_Alias = string.Empty;
        public static string theirDB_Alias
        {
            get { return Project._theirDB_Alias; }
            set { Project._theirDB_Alias = value; }
        }

        static string _ConfigBase = string.Empty;
        public static string ConfigBase
        {
            get { return Project._ConfigBase; }
            set { Project._ConfigBase = value; }
        }
        //配置库类型0备用库1标准库
        static int _ConfigBase_type = 0;
        public static int ConfigBase_type
        {
            get { return Project._ConfigBase_type; }
            set { Project._ConfigBase_type = value; }
        }

        static string _Uid = string.Empty;
        public static string Uid
        {
            get { return Project._Uid; }
            set { Project._Uid = value; }
        }

        static string _Pwd = string.Empty;
        public static string Pwd  /////
        {
            get { return Project._Pwd; }
            set { Project._Pwd = value; }
        }

        static string _theirUid = string.Empty;
        public static string theirUid
        {
            get { return Project._theirUid; }
            set { Project._theirUid = value; }
        }

        static string _theirPwd = string.Empty;
        public static string theirPwd
        {
            get { return Project._theirPwd; }
            set { Project._theirPwd = value; }
        }

        static string _SeverName = string.Empty;
        public static string SeverName
        {
            get { return Project._SeverName; }
            set { Project._SeverName = value; }
        }

        static string _theirSeverName = string.Empty;
        public static string theirSeverName
        {
            get { return Project._theirSeverName; }
            set { Project._theirSeverName = value; }
        }

        static string _DbName = string.Empty;
        public static string DbName
        {
            get { return Project._DbName; }
            set { Project._DbName = value; }
        }

        static string _theirDbName = string.Empty;
        public static string theirDbName
        {
            get { return Project._theirDbName; }
            set { Project._theirDbName = value; }
        }

        static int _TaskSuccessSum = 0;
        public static int TaskSuccessSum
        {
            get { return Project._TaskSuccessSum; }
            set { Project._TaskSuccessSum = value; }
        }

        static int _TaskFailSum = 0;
        public static int TaskFailSum
        {
            get { return Project._TaskFailSum; }
            set { Project._TaskFailSum = value; }
        }

        static int _TaskFinalDealSuccessSum = 0;
        public static int TaskFinalDealSuccessSum
        {
            get { return Project._TaskFinalDealSuccessSum; }
            set { Project._TaskFinalDealSuccessSum = value; }
        }

        static int _TaskFinalDealFailSum = 0;
        public static int TaskFinalDealFailSum
        {
            get { return Project._TaskFinalDealFailSum; }
            set { Project._TaskFinalDealFailSum = value; }
        }

        /// <summary>
        /// 字段：对方数据库连接串
        /// </summary>
        static string _TheirDB_Connection = string.Empty;
        /// <summary>
        /// 属性：对方数据库连接串
        /// </summary>
        public static string TheirDB_Connection
        {
            get { return Project._TheirDB_Connection; }
            set { Project._TheirDB_Connection = value; }
        }


        static string _login = string.Empty;
        public static string login
        {
            get { return Project._login; }
            set { Project._login = value; }
        }


        static string _LoginPass = string.Empty;
        public static string LoginPass
        {
            get { return Project._LoginPass; }
            set { Project._LoginPass = value; }
        }

        /// <summary>
        /// 字段：创建日期
        /// </summary>
        static string _Create_Date = string.Empty;
        /// <summary>
        /// 属性：创建日期
        /// </summary>
        public static string Create_Date
        {
            get { return Project._Create_Date; }
            set { Project._Create_Date = value; }
        }

        static int _sysType = 0;
        /// <summary>
        /// 属性：对接的系统类型 0为scm及其他 1为集约化
        /// </summary>
        public static int sysType
        {
            get { return Project._sysType; }
            set { Project._sysType = value; }
        }

        //private static int _MaxRecordBh = 0;
        ///// <summary>
        ///// 属性：最大记录编号
        ///// </summary>
        //public static int MaxRecordBh
        //{
        //    get { return Project._MaxRecordBh; }
        //    set { Project._MaxRecordBh = value; }
        //}

        //private static int _TempMaxRecordBh = 0;

        //public static int TempMaxRecordBh
        //{
        //    get { return Project._TempMaxRecordBh; }
        //    set { Project._TempMaxRecordBh = value; }
        //}

        /// <summary>
        /// 获得项目服务器名
        /// </summary>
        /// <returns></returns>
        public static string get_ServerName()
        {
            string[] ls_items = DB_Connection.Split(new string[] { ";" },
                StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in ls_items)
            {
                string[] ls_parts = item.Split(new string[] { "=" }, 2, StringSplitOptions.None);
                if (ls_parts[0] == "Data Source") return ls_parts[1];
            }
            return "";
        }
        /// <summary>
        /// 获得项目用户名
        /// </summary>
        /// <returns></returns>
        public static string get_UserName()
        {
            string[] ls_items = DB_Connection.Split(new string[] { ";" },
                StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in ls_items)
            {
                string[] ls_parts = item.Split(new string[] { "=" }, 2, StringSplitOptions.None);
                if (ls_parts[0] == "User ID") return ls_parts[1];
            }
            return "";
        }
        /// <summary>
        /// 获得项目用户密码
        /// </summary>
        /// <returns></returns>
        public static string get_Password()
        {
            string[] ls_items = DB_Connection.Split(new string[] { ";" },
                StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in ls_items)
            {
                string[] ls_parts = item.Split(new string[] { "=" }, 2, StringSplitOptions.None);
                if (ls_parts[0] == "Password") return ls_parts[1];
            }
            return "";
        }
        /// <summary>
        /// 获得项目数据库名
        /// </summary>
        /// <returns></returns>
        public static string get_Database()
        {
            string[] ls_items = DB_Connection.Split(new string[] { ";" },
                StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in ls_items)
            {
                string[] ls_parts = item.Split(new string[] { "=" }, 2, StringSplitOptions.None);
                if (ls_parts[0] == "Initial Catalog") return ls_parts[1];
            }
            return "";
        }

        /// <summary>
        /// 字段：项目选用的字体

        /// </summary>
        static string _ProjectFontName = "楷体_GB2312";
        /// <summary>
        /// 属性：项目选用的字体

        /// </summary>
        public static string ProjectFontName
        {
            get { return _ProjectFontName; }
            set { _ProjectFontName = value; }
        }
        public static bool IsInsertLogToDb = false;
    }
    #endregion

    #region CurrentForm 当前页面
    /// <summary>
    /// 当前流程
    /// </summary>
    public static class CurrentForm
    {
        /// <summary>
        /// 字段：表名

        /// </summary>
        static string _Table_Name = string.Empty;
        /// <summary>
        /// 属性：表名
        /// </summary>
        public static string Table_Name
        {
            get { return _Table_Name; }
            set { _Table_Name = value; }
        }
        /// <summary>
        /// 字段：表名

        /// </summary>
        static string _FieldName = string.Empty;
        /// <summary>
        /// 属性：表名
        /// </summary>
        public static string CurrentFieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }

    }
    #endregion

    #region CurrentFlow 当前流程
    /// <summary>
    /// 当前流程
    /// </summary>
    public sealed class CurrentFlow
    {
        /// <summary>
        /// 字段：流程编号

        /// </summary>
        static string _Flow_ID = string.Empty;
        /// <summary>
        /// 属性：流程编号
        /// </summary>
        public static string Flow_ID
        {
            get { return CurrentFlow._Flow_ID; }
            set { CurrentFlow._Flow_ID = value; }
        }        /// <summary>
        /// 字段：流程名称

        /// </summary>
        static string _Flow_Name = string.Empty;
        /// <summary>
        /// 属性：流程名称
        /// </summary>
        public static string Flow_Name
        {
            get { return CurrentFlow._Flow_Name; }
            set { CurrentFlow._Flow_Name = value; }
        }
        /// <summary>
        /// 字段：表单描述

        /// </summary>
        static string _Form_Name = string.Empty;
        /// <summary>
        /// 属性：表单描述
        /// </summary>
        public static string Form_Name
        {
            get { return CurrentFlow._Form_Name; }
            set { CurrentFlow._Form_Name = value; }
        }
        /// <summary>
        /// 字段：表单对应数据表名

        /// </summary>
        static string _Table_Name = string.Empty;
        /// <summary>
        /// 属性：表单对应数据表名
        /// </summary>
        public static string Table_Name
        {
            get { return CurrentFlow._Table_Name; }
            set { CurrentFlow._Table_Name = value; }
        }
        /// <summary>
        /// 字段：节点ID
        /// </summary>
        static string _Node_ID = string.Empty;
        /// <summary>
        /// 属性：节点ID
        /// </summary>
        public static string Node_ID
        {
            get { return CurrentFlow._Node_ID; }
            set { CurrentFlow._Node_ID = value; }
        }
        /// <summary>
        /// 字段：节点父ID
        /// </summary>
        static string _Parent_ID = string.Empty;
        /// <summary>
        /// 属性：节点父ID
        /// </summary>
        public static string Parent_ID
        {
            get { return CurrentFlow._Parent_ID; }
            set { CurrentFlow._Parent_ID = value; }
        }
        /// <summary>
        /// 字段：允许驳回

        /// </summary>
        static string _Allow_Reject = "不允许";
        /// <summary>
        /// 属性：允许驳回
        /// </summary>
        public static string Allow_Reject
        {
            get { return CurrentFlow._Allow_Reject; }
            set { CurrentFlow._Allow_Reject = value; }
        }
    }
    #endregion

    #region 调度相关

    /// <summary>
    /// 调度器类
    /// </summary>
    public sealed class ScheduleManager
    {
        public static List<string> schedule_running = new List<string>();
        public static List<string> schedule_Mutex_WaitingForRun = new List<string>();
        private static object lockHelper = new object();
        /// <summary>
        /// 调度结构定义
        /// </summary>
        private struct Schedule_Struct
        {
            public int schedule_ID;
            public string schedule_name;
            public string schedule_description;
            public int freq_type;
            public int freq_interval;
            public int freq_subday_type;
            public int freq_subday_interval;
            public int freq_relative_interval;
            public int freq_recurrence_factor;
            public int active_start_date;
            public int active_end_date;
            public int active_start_time;
            public int active_end_time;
            public string freq_description;
            public string command;
        }
        /// <summary>
        /// 调度列表
        /// </summary>
        private static List<Schedule_Struct> Schedule_List = new List<Schedule_Struct>();
        /// <summary>
        /// execute_schedule,执行调度
        /// </summary>
        public static void execute_schedule()
        {
            string AgainstReason;
            List<string> lsDelete = new List<string>();
            foreach (string s_command_wait in schedule_Mutex_WaitingForRun)
            {
                if (IsPermitExcute(s_command_wait, out AgainstReason))
                {
                    //schedule_running.Add(s_command_wait);
                    lsDelete.Add(s_command_wait);
                    Thread lth = new Thread(new ParameterizedThreadStart(execute_job));
                    lth.Start(s_command_wait);
                }
            }
            foreach (string s_delete in lsDelete)
            {
                schedule_Mutex_WaitingForRun.Remove(s_delete);
            }

            try
            {
                int li_today = DateTime.Today.Year * 10000 + DateTime.Today.Month * 100 + DateTime.Today.Day;
                int li_now = DateTime.Now.Hour * 10000 + DateTime.Now.Minute * 100 + DateTime.Now.Second;
                foreach (Schedule_Struct sch in Schedule_List)
                {
                    if (is_schedule_time(sch, li_today, li_now))
                    {
                        string ls_command = sch.command;
                        string schedule_name = sch.schedule_name;
                        if (IsPermitExcute(ls_command, out AgainstReason))
                        {
                            //schedule_running.Add(ls_command);
                            Thread lth = new Thread(new ParameterizedThreadStart(execute_job));
                            lth.Start(ls_command);
                        }
                        else
                        {
                            if (!schedule_Mutex_WaitingForRun.Contains(ls_command))
                            {
                                schedule_Mutex_WaitingForRun.Add(ls_command);
                            }
                            string[] arrInfo = new string[4];
                            arrInfo[0] = "【" + schedule_name + "】相同任务或其它互斥任务正在执行，当前任务加入等待列表!";
                            arrInfo[1] = "";
                            arrInfo[2] = "1";
                            arrInfo[3] = schedule_name;
                            Miscellaneous.Write2List(arrInfo);
                        }
                    }
                }
            }
            catch { }
        }


        private static string Execute_FinalDeal(string schedule_name, string sql, bool IsOurPosition)
        {
            if (IsOurPosition)
            {
                if (Project.DB_Type.ToLower().Equals("sql server"))
                    return SQLServerDBop.StrSQLExecute(schedule_name, Project.DB_Connection, sql);
                else if (Project.DB_Type.ToLower().Equals("oracle"))
                    return OracleDBop.StrSQLExecute(Project.DB_Connection, sql);
                else
                    return MySqlDBop.StrSQLExecute(Project.DB_Connection, sql);
            }
            else
            {
                if (Project.TheirDB_Type.ToLower().Equals("sql server"))
                    return SQLServerDBop.StrSQLExecute(schedule_name, Project.TheirDB_Connection, sql);
                else if (Project.TheirDB_Type.ToLower().Equals("oracle"))
                    return OracleDBop.StrSQLExecute(Project.TheirDB_Connection, sql);
                else
                    return MySqlDBop.StrSQLExecute(Project.TheirDB_Connection, sql);
            }
        }

        /// <summary>
        /// 两种情况不允许执行：1调度正在执行中2同名互斥调度正在执行中
        /// </summary>
        /// <param name="as_command"></param>
        /// <param name="schedule_name"></param>
        /// <returns></returns>
        public static bool IsPermitExcute(string as_command, out string AgainstReason)
        {
            try
            {
                AgainstReason = string.Empty;
                string ls_command_want = as_command.Trim();
                string MutexName_want = ls_command_want.Split(new string[] { "{,}" }, StringSplitOptions.None)[3].Trim();
                foreach (string s_command_running in schedule_running)   //任务正在执行，则添加到等待列表schedule_WaitingForRun,然后退出
                {
                    string mutexName_running = s_command_running.Split(new string[] { "{,}" }, StringSplitOptions.None)[3].Trim();
                    if (s_command_running == ls_command_want)
                    {
                        AgainstReason = "相同任务正在执行";
                        return false;
                    }
                    if ((MutexName_want == mutexName_running) && (MutexName_want != ""))     //如果调度类型为互斥，则如果有其它互斥类的调度正在执行，则添加到等待列表schedule_WaitingForRun,然后退出
                    {
                        AgainstReason = "互斥任务正在执行,调度名称为:" + GetScheduleNameFromScheduleList(s_command_running);
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                AgainstReason = "格式错误";
                return false;
            }
        }
        /// <summary>
        /// 根据调度内容获取调度名称
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static string GetScheduleNameFromScheduleList(string command)
        {
            foreach (Schedule_Struct stru in Schedule_List)
            {
                if (stru.command == command)
                {
                    return stru.schedule_name;
                }
            }
            return "";
        }

        /// <summary>
        /// 执行调度内容（任务及后期处理）
        /// </summary>
        /// <param name="ls_command"></param>
        private static void execute_job(object as_command)
        {
            lock (lockHelper)
            {
                string ls_command = as_command == null ? "" : as_command.ToString().Trim();
                if (ls_command.Equals(""))
                {
                    //schedule_running.Remove(ls_command);
                    return;
                }
                schedule_running.Add(ls_command);
                string schedule_name = string.Empty;
                string sql = "select schedule_name from PT_schedule where command = '" + ls_command + "'";
                DataTable dt = new DataTable();
                bool bResult = AccessDBop.SQLSelect(sql, ref dt);
                string sResult = string.Empty;
                if (bResult)
                {
                    if (dt.Rows.Count > 0)
                    {
                        schedule_name = dt.Rows[0][0].ToString();
                    }
                }
                try
                {
                    if (ls_command == "") return;
                    string[] ls_commands = ls_command.Split(new string[] { "{,}" }, StringSplitOptions.None);
                    if (ls_commands.Length == 4)
                    {
                        // 执行任务
                        if (ls_commands[0] != "")     //modify 2014-05-29 by zlibo
                        {
                            bResult = TaskManager.ExecTask(schedule_name, ls_commands[0]);
                            if (bResult)   //任务执行成功后再执行最后处理
                            {
                                Project.TaskSuccessSum++;
                                if (ls_commands[1] != "")
                                {
                                    sResult = Execute_FinalDeal(schedule_name, ls_commands[1], bool.Parse(ls_commands[2]));   //执行最后处理
                                    if (sResult != "")
                                    {
                                        Project.TaskFinalDealFailSum++;
                                        string[] arrInfo = new string[4];
                                        arrInfo[0] = "【" + schedule_name + "】任务后期处理失败:" + sResult;
                                        arrInfo[1] = ls_command;
                                        arrInfo[2] = "2";
                                        arrInfo[3] = schedule_name;
                                        Miscellaneous.Write2List(arrInfo);
                                    }
                                    else
                                    {
                                        Project.TaskFinalDealSuccessSum++;
                                        string[] arrInfo = new string[4];
                                        arrInfo[0] = "【" + schedule_name + "】任务后期处理成功";
                                        arrInfo[1] = ls_command;
                                        arrInfo[2] = "0";
                                        arrInfo[3] = schedule_name;
                                        Miscellaneous.Write2List(arrInfo);
                                    }
                                }
                            }
                            else
                            {
                                Project.TaskFailSum++;
                            }
                        }
                        // 执行存储过程
                        else if (ls_commands[1] != "")   //只执行最后处理
                        {
                            if (ls_commands[1].Contains(".exe"))
                            {
                                string[] arrExe = ls_commands[1].Split(new string[] { "|" }, StringSplitOptions.None);
                                string ProccessName = arrExe[0];
                                string par = "";
                                if (arrExe.Length == 2)
                                {
                                    par = arrExe[1];
                                }
                                sResult = Miscellaneous.RunExe(ProccessName, par);
                                if (sResult != "")
                                {
                                    Project.TaskFinalDealFailSum++;
                                    string[] arrInfo = new string[4];
                                    arrInfo[0] = "【" + schedule_name + "】可执行程序执行出错：" + sResult;
                                    arrInfo[1] = ls_command;
                                    arrInfo[2] = "2";
                                    arrInfo[3] = schedule_name;
                                    Miscellaneous.Write2List(arrInfo);
                                }
                                else
                                {
                                    Project.TaskFinalDealSuccessSum++;
                                    string[] arrInfo = new string[4];
                                    arrInfo[0] = "【" + schedule_name + "】可执行程序执行成功！";
                                    arrInfo[1] = ls_command;
                                    arrInfo[2] = "0";
                                    arrInfo[3] = schedule_name;
                                    Miscellaneous.Write2List(arrInfo);
                                }
                            }
                            else
                            {
                                sResult = Execute_FinalDeal(schedule_name, ls_commands[1], bool.Parse(ls_commands[2]));
                                if (!bResult)
                                {
                                    Project.TaskFinalDealFailSum++;
                                    string[] arrInfo = new string[4];
                                    arrInfo[0] = "【" + schedule_name + "】任务后期处理失败:" + sResult;
                                    arrInfo[1] = ls_command;
                                    arrInfo[2] = "2";
                                    arrInfo[3] = schedule_name;
                                    Miscellaneous.Write2List(arrInfo);
                                }
                                else
                                {
                                    Project.TaskFinalDealSuccessSum++;
                                    string[] arrInfo = new string[4];
                                    arrInfo[0] = "【" + schedule_name + "】任务后期处理成功";
                                    arrInfo[1] = ls_command;
                                    arrInfo[2] = "0";
                                    arrInfo[3] = schedule_name;
                                    Miscellaneous.Write2List(arrInfo);
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] arrInfo = new string[4];
                        arrInfo[0] = "【" + schedule_name + "】调度内容格式不符合规则";
                        arrInfo[1] = "";
                        arrInfo[2] = "2";
                        arrInfo[3] = schedule_name;
                        Miscellaneous.Write2List(arrInfo);
                    }
                }
                catch (Exception ex)
                {
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ex.Message;
                    arrInfo[1] = "";
                    arrInfo[2] = "2";
                    arrInfo[3] = schedule_name;
                    Miscellaneous.Write2List(arrInfo);
                    schedule_running.Remove(ls_command);
                    return;
                }
                if (schedule_running.Contains(ls_command))
                {
                    schedule_running.Remove(ls_command);
                }
            }
        }


        /// <summary>
        /// 手动执行调度内容（任务及后期处理）
        /// </summary>
        /// <param name="ls_command"></param>
        public static string execute_job_NoThread(string schedule_name, object as_command)
        {
            string sResult = string.Empty;
            string ls_command = as_command == null ? "" : as_command.ToString().Trim();
            if (ls_command.Equals(""))
            {
                Project.TaskFinalDealFailSum++;
                string[] arrInfo = new string[4];
                arrInfo[0] = "调度内容为空!";
                arrInfo[1] = "";
                arrInfo[2] = "2";
                arrInfo[3] = schedule_name;
                Miscellaneous.Write2List(arrInfo);
                return "指令为空!";
            }
            //schedule_running.Add(ls_command);
            try
            {
                string[] ls_commands = ls_command.Split(new string[] { "{,}" }, StringSplitOptions.None);
                if (ls_commands.Length == 4)
                {
                    // 执行任务
                    if (ls_commands[0] != "")     //modify 2014-05-29 by zlibo
                    {
                        bool bResult = TaskManager.ExecTask(schedule_name, ls_commands[0]);
                        if (bResult)   //任务执行成功后再执行最后处理
                        {
                            Project.TaskSuccessSum++;
                            if (ls_commands[1] != "")
                            {
                                sResult = Execute_FinalDeal(schedule_name, ls_commands[1], bool.Parse(ls_commands[2]));   //执行最后处理
                                if (sResult != "")
                                {
                                    Project.TaskFinalDealFailSum++;
                                    string[] arrInfo = new string[4];
                                    arrInfo[0] = "任务后期处理失败:" + sResult;
                                    arrInfo[1] = "";
                                    arrInfo[2] = "2";
                                    arrInfo[3] = schedule_name;
                                    Miscellaneous.Write2List(arrInfo);
                                }
                            }
                        }
                        else
                        {
                            sResult = "fail";
                            Project.TaskFinalDealFailSum++;
                        }
                    }
                    // 执行存储过程
                    else if (ls_commands[1] != "")   //只执行最后处理
                    {
                        if (ls_commands[1].Contains(".exe"))
                        {
                            string[] arrExe = ls_commands[1].Split(new string[] { "|" }, StringSplitOptions.None);
                            string ProccessName = arrExe[0];
                            string par = "";
                            if (arrExe.Length == 2)
                            {
                                par = arrExe[1];
                            }
                            sResult = Miscellaneous.RunExe(ProccessName, par);
                            if (sResult != "")
                            {
                                Project.TaskFinalDealFailSum++;
                                string[] arrInfo = new string[4];
                                arrInfo[0] = "可执行程序执行出错：" + sResult;
                                arrInfo[1] = ls_command;
                                arrInfo[2] = "2";
                                arrInfo[3] = schedule_name;
                                Miscellaneous.Write2List(arrInfo);
                                sResult = "fail";
                            }
                            else
                            {
                                Project.TaskFinalDealSuccessSum++;
                                string[] arrInfo = new string[4];
                                arrInfo[0] = "可执行程序执行成功！";
                                arrInfo[1] = ls_command;
                                arrInfo[2] = "0";
                                arrInfo[3] = schedule_name;
                                Miscellaneous.Write2List(arrInfo);
                            }
                        }
                        else
                        {
                            sResult = Execute_FinalDeal(schedule_name, ls_commands[1], bool.Parse(ls_commands[2]));
                            if (sResult != "")
                            {
                                Project.TaskFinalDealFailSum++;
                                string[] arrInfo = new string[4];
                                arrInfo[0] = "后期处理失败";
                                arrInfo[1] = ls_command;
                                arrInfo[2] = "2";
                                arrInfo[3] = schedule_name;
                                Miscellaneous.Write2List(arrInfo);
                            }
                        }
                    }
                }
                else
                {
                    string[] arrInfo = new string[4];
                    arrInfo[0] = "调度内容格式不符合规则";
                    arrInfo[1] = "";
                    arrInfo[2] = "2";
                    arrInfo[3] = schedule_name;
                    Miscellaneous.Write2List(arrInfo);
                    sResult = "fail";
                }
            }
            catch (Exception ex)
            {
                string[] arrInfo = new string[4];
                arrInfo[0] = ex.Message;
                arrInfo[1] = "";
                arrInfo[2] = "2";
                arrInfo[3] = schedule_name;
                Miscellaneous.Write2List(arrInfo);
                schedule_running.Remove(ls_command); //删除任务
                sResult = "fail";
                return sResult;
            }
            schedule_running.Remove(ls_command); //删除任务
            return sResult;
        }
        /// <summary>
        /// 读取所有的调度
        /// </summary>
        public static void Get_ScheduleList()
        {
            Schedule_List.Clear();
            string ls_sql = "SELECT schedule_ID, schedule_name, schedule_description, " +
                "freq_type, freq_interval, " +
                "freq_subday_type, freq_subday_interval, freq_relative_interval, " +
                "freq_recurrence_factor, active_start_date, active_end_date, " +
                "active_start_time, active_end_time, command, freq_description FROM PT_schedule";
            DataTable ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                for (int i = 0; i < ldt.Rows.Count; i++)
                {
                    Schedule_Struct sch = new Schedule_Struct();
                    sch.schedule_ID = int.Parse(ldt.Rows[i]["schedule_ID"].ToString());
                    sch.schedule_name = ldt.Rows[i]["schedule_name"].ToString();
                    sch.schedule_description = ldt.Rows[i]["schedule_description"].ToString();
                    sch.freq_type = int.Parse(ldt.Rows[i]["freq_type"].ToString());
                    sch.freq_interval = int.Parse(ldt.Rows[i]["freq_interval"].ToString());
                    sch.freq_subday_type = int.Parse(ldt.Rows[i]["freq_subday_type"].ToString());
                    sch.freq_subday_interval = int.Parse(ldt.Rows[i]["freq_subday_interval"].ToString());
                    sch.freq_relative_interval = int.Parse(ldt.Rows[i]["freq_relative_interval"].ToString());
                    sch.freq_recurrence_factor = int.Parse(ldt.Rows[i]["freq_recurrence_factor"].ToString());
                    sch.active_start_date = int.Parse(ldt.Rows[i]["active_start_date"].ToString());
                    sch.active_end_date = int.Parse(ldt.Rows[i]["active_end_date"].ToString());
                    sch.active_start_time = int.Parse(ldt.Rows[i]["active_start_time"].ToString());
                    sch.active_end_time = int.Parse(ldt.Rows[i]["active_end_time"].ToString());
                    sch.command = ldt.Rows[i]["command"].ToString();
                    sch.freq_description = ldt.Rows[i]["freq_description"].ToString();
                    ScheduleManager.Schedule_List.Add(sch);
                }
            }
        }

        /// <summary>
        /// 获取数字的星期几
        /// </summary>
        /// <param name="weekName"></param>
        /// <returns></returns>
        public static int IntWeek(string weekName)
        {
            switch (weekName)
            {
                case "Sunday":
                    return 0;
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
            }
            return 0;
        }


        /// <summary>
        /// 判断调度是否到时间
        /// </summary>
        /// <param name="schedule">调度内容</param>
        /// <param name="li_today">li_today,当天的日期YYYYmmdd格式的数值,int</param>
        /// <param name="li_now">li_now,现在的时间HHMMDD格式的数值,int</param>
        /// <returns></returns>
        private static bool is_schedule_time(Schedule_Struct schedule, int li_today, int li_now)
        {
            try
            {
                int freq_type = schedule.freq_type;
                if ((freq_type & 1) == 1)
                {
                    // 执行一次
                    if (li_today == schedule.active_start_date && li_now == schedule.active_start_time) return true;
                    else return false;
                }
                else if ((freq_type & 4) == 4)
                {
                    //每天
                    if (!(schedule.active_start_date <= li_today && li_today <= schedule.active_end_date)) return false;
                    DateTime ld_start = DateTime.Parse(Convert.ToString(schedule.active_start_date / 10000) + "-" +
                        Convert.ToString(schedule.active_start_date / 100 % 100) + "-" +
                        Convert.ToString(schedule.active_start_date % 100) + " 00:00:00");
                    DateTime ld_today = DateTime.Parse(Convert.ToString(li_today / 10000) + "-" +
                        Convert.ToString(li_today / 100 % 100) + "-" +
                        Convert.ToString(li_today % 100) + " 00:00:00");
                    TimeSpan ts1 = new TimeSpan(ld_today.Ticks);
                    TimeSpan ts2 = new TimeSpan(ld_start.Ticks);
                    TimeSpan delta = ts1.Subtract(ts2).Duration();
                    // 间隔天数
                    if (delta.Days % schedule.freq_interval == 0)
                        return is_time_subday(schedule, li_now);
                    else return false;
                }
                else if ((freq_type & 8) == 8)
                {
                    //每周
                    if (!(schedule.active_start_date <= li_today && li_today <= schedule.active_end_date)) return false;
                    DateTime ld_today = DateTime.Parse(Convert.ToString(li_today / 10000) + "-" +
                        Convert.ToString(li_today / 100 % 100) + "-" +
                        Convert.ToString(li_today % 100));
                    int li_today_year = ld_today.Year;
                    int li_today_week = Get_WeekofYear(ld_today);
                    int li_today_weekday = IntWeek(ld_today.DayOfWeek.ToString()) + 1;
                    //int li_today_weekday = int.Parse(ld_today.DayOfWeek.ToString()) + 1;
                    // 间隔周数
                    if (li_today_week % schedule.freq_recurrence_factor != 0) return false;
                    // 星期一 ~ 星期天的组合

                    if ((schedule.freq_interval & (2 ^ (li_today_weekday % 7))) == (2 ^ (li_today_weekday % 7)))
                        return is_time_subday(schedule, li_now);
                    else return false;
                }
                else if ((freq_type & 16) == 16)
                {
                    //每月
                    if (!(schedule.active_start_date <= li_today && li_today <= schedule.active_end_date)) return false;
                    // 间隔月数
                    if ((li_today / 100 % 100) % schedule.freq_recurrence_factor != 0) return false;
                    if (schedule.freq_interval == (li_today % 100))
                        return is_time_subday(schedule, li_now);
                    else return false;
                }
                else if ((freq_type & 32) == 32)
                {
                    //每月
                    if (!(schedule.active_start_date <= li_today && li_today <= schedule.active_end_date)) return false;
                    // 间隔月数
                    if ((li_today / 100 % 100) % schedule.freq_recurrence_factor != 0) return false;
                    // 计算相应的天
                    int freq_interval = schedule.freq_interval; // 1~7 --星期日~星期六 8--天 9--工作日 10--休息日
                    int freq_relative_interval = schedule.freq_relative_interval; //1--第一个 2--第二个 4--第三个 8--第四个 16--最后一个
                    int li_day = get_day(freq_relative_interval, freq_interval, li_today);
                    if (li_day == (li_today % 100)) return is_time_subday(schedule, li_now);
                    else return false;
                }
                //else if (freq_type & 64 == 64)
                //{
                //    //开机启动
                //    li_last_time = schedule.last_invoke_time;
                //    if (li_last_time == 0) return true;
                //    else return false;
                //}
                //else if (freq_type & 128 == 128)
                //{
                //    //空闲启动
                //    if (sys_variable.running_subprocess == 0 ) return true;
                //    else return false;
                //}
                else return false;
            }
            catch { return false; }
        }
        /// <summary>
        /// 获得某天是当年的第几周
        /// </summary>
        /// <param name="ld_today">欲计算的日期</param>
        /// <returns>第几周</returns>
        /// The ISO year consists of 52 or 53 full weeks, and where a week starts on a 
        /// Monday and ends on a Sunday. The first week of an ISO year is the first (Gregorian) 
        /// calendar week of a year containing a Thursday. This is called week number 1, 
        /// and the ISO year of that Thursday is the same as its Gregorian year.
        ///
        /// For example, 2004 begins on a Thursday, so the first week of ISO year 2004 
        /// begins on Monday, 29 Dec 2003 and ends on Sunday, 4 Jan 2004, so that 
        /// date(2003, 12, 29).isocalendar() == (2004, 1, 1) 
        /// and date(2004, 1, 4).isocalendar() == (2004, 1, 7).
        private static int Get_WeekofYear(DateTime ld_today)
        {
            int li_weekofyear = 0;
            // 当年元旦
            DateTime ld_NewYearDay = DateTime.Parse(ld_today.Year.ToString() + "-1-1");
            // 当年元旦是周几
            int li_dayofweek_NewYear = GetISODayofWeek(ld_NewYearDay);
            // 下年元旦
            DateTime ld_NextNewYearDay = ld_NewYearDay.AddYears(1);
            // 下年元旦是周几
            int li_dayofweek_NextNewYear = GetISODayofWeek(ld_NextNewYearDay);
            // 当天到下年元旦的天数
            int li_days2NextYear = ((TimeSpan)ld_NextNewYearDay.Subtract(ld_today)).Days;
            // 当天是周几
            int li_dayofweek_today = GetISODayofWeek(ld_today);
            // 判断年度最后一周（即下年第一周）
            if (li_days2NextYear < 7 && li_dayofweek_today < li_dayofweek_NextNewYear && li_dayofweek_NextNewYear <= 4)
                li_weekofyear = 1;
            else
            {
                if (li_dayofweek_NewYear > 4)
                {
                    int ls_days2NewYear = ((TimeSpan)ld_today.Subtract(ld_NewYearDay)).Days;
                    // 元旦后的同一周
                    if (ls_days2NewYear < 7 && li_dayofweek_today >= li_dayofweek_NewYear)
                    {
                        // 上年元旦
                        DateTime ld_LastNewYearDay = DateTime.Parse((ld_today.Year - 1).ToString() + "-1-1");
                        // 上年天数
                        int li_days = ((TimeSpan)ld_NewYearDay.Subtract(ld_LastNewYearDay)).Days;
                        // 上年元旦是周几
                        int li_dayofweek_LastNewYear = GetISODayofWeek(ld_LastNewYearDay);
                        // 元旦到第一周开始的天数
                        int li_delta = (li_dayofweek_LastNewYear > 4) ?
                            li_dayofweek_LastNewYear - 8 : li_dayofweek_LastNewYear - 1;
                        li_weekofyear = (int)Math.Ceiling((ld_today.DayOfYear + li_delta + li_days) / 7.0);
                    }
                    else li_weekofyear = (int)Math.Ceiling((ld_today.DayOfYear + li_dayofweek_NewYear - 8) / 7.0); // 计算周数
                }
                else li_weekofyear = (int)Math.Ceiling((ld_today.DayOfYear + li_dayofweek_NewYear - 1) / 7.0);
            }
            return li_weekofyear;
        }
        /// <summary>
        /// 获得ISO的周几
        /// </summary>
        /// <param name="ld_Day">日期</param>
        /// <returns>ISO周几</returns>
        private static int GetISODayofWeek(DateTime ld_Day)
        {
            int li_dayofweek = (ld_Day.DayOfWeek == DayOfWeek.Sunday) ?
                7 : Convert.ToInt32(ld_Day.DayOfWeek);
            return li_dayofweek;
        }
        /// <summary>
        /// 判断一天中时间是否满足调度条件 
        /// </summary>
        /// <param name="schedule">调度内容</param>
        /// <param name="li_now">li_now,现在的时间HHMMDD格式的数值,int</param>
        /// <returns>是否可以执行</returns>
        private static bool is_time_subday(Schedule_Struct schedule, int li_now)
        {
            try
            {
                if (schedule.freq_subday_type == 1)
                    // 一次
                    if (li_now == schedule.active_start_time) return true;
                    else return false;
                else
                {
                    if (!(schedule.active_start_time <= li_now && li_now <= schedule.active_end_time)) return false;
                    int li_start_seconds = schedule.active_start_time / 10000 * 3600 +
                        schedule.active_start_time / 100 % 100 * 60 +
                        schedule.active_start_time % 100;
                    int li_today_seconds = li_now / 10000 * 3600 + li_now / 100 % 100 * 60 + li_now % 100;
                    int delta = li_today_seconds - li_start_seconds;
                    int freq_subday_interval = 60;
                    if (schedule.freq_subday_type == 4) // 频繁执行 间隔单位：分钟
                        freq_subday_interval = schedule.freq_subday_interval * 60;
                    else if (schedule.freq_subday_type == 8) // 频繁执行 间隔单位：小时
                        freq_subday_interval = schedule.freq_subday_interval * 3600;
                    else if (schedule.freq_subday_type == 16) // 频繁执行 间隔单位：秒   功能：调度增加秒 20160127 by zlibo
                        freq_subday_interval = schedule.freq_subday_interval;
                    else return false;
                    if ((delta % freq_subday_interval) == 0) return true;
                    else return false;
                }
            }
            catch { return false; }

        }
        /// <summary>
        /// 一月内第几个星期几是哪一天
        /// </summary>
        /// <param name="freq_relative_interval">第几个</param>
        /// <param name="freq_interval">第几天，1-Sunday 2…7-Monday…Friday 8-Monday~Sunday 9-Monday~Friday 10-Saturday~Sunday</param>
        /// <param name="li_today">li_today,当天的日期YYYYmmdd格式的数值,int</param>
        /// <returns></returns>
        private static int get_day(int freq_relative_interval, int freq_interval, int li_today)
        {
            int li_day = 0;

            int li_times = (int)(Math.Log((double)freq_relative_interval, 2) + 1) % 5;
            int ll_week0 = 0;
            int ll_week1 = 7;

            if (freq_interval == 1) { ll_week0 = 7; ll_week1 = 7; }
            else if (freq_interval >= 2 && freq_interval < 8) { ll_week0 = freq_interval - 1; ll_week1 = freq_interval - 1; }
            else if (freq_interval == 8) { ll_week0 = 1; ll_week1 = 7; }
            else if (freq_interval == 9) { ll_week0 = 1; ll_week1 = 5; }
            else if (freq_interval == 10) { ll_week0 = 6; ll_week1 = 7; }
            else { ll_week0 = 0; ll_week1 = 0; }
            int li_c = 0;
            int li_year = li_today / 10000;
            int li_month = li_today / 100 % 100;
            for (int i = 0; i < 32; i++)
            {
                try
                {
                    DateTime dt = DateTime.Parse(li_year.ToString() + "-" + li_month.ToString() + "-" + i.ToString() + " 00:00:00");
                    int li_dayofweek = int.Parse(dt.DayOfWeek.ToString()) + 1;
                    if (li_dayofweek >= ll_week0 && li_dayofweek <= ll_week1)
                    {
                        li_day = i;
                        li_c++;
                        if (li_c == li_times) break;
                    }
                }
                catch { continue; }
            }
            return li_day;
        }

    }
    #endregion

    #region 数据表格的风格
    public sealed class DataGridStyle
    {
        public static void GridDisplayStyle(DataGridView adgv, bool ab_fill, int rowHeight, bool enable = true)
        {
            DataGridViewCellStyle CellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle CellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle CellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle CellStyle4 = new DataGridViewCellStyle();
            // 奇数行的背景色
            CellStyle1.BackColor = Color.FromArgb(242, 245, 249);
            adgv.AlternatingRowsDefaultCellStyle = CellStyle1;

            // 表格背景色
            adgv.BackgroundColor = Color.White;
            // 表格标题栏的样式
            CellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            CellStyle2.BackColor = Color.FromArgb(237, 237, 237);
            CellStyle2.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CellStyle2.ForeColor = SystemColors.WindowText;
            CellStyle2.SelectionBackColor = SystemColors.Highlight;
            CellStyle2.SelectionForeColor = SystemColors.HighlightText;
            CellStyle2.WrapMode = DataGridViewTriState.True;
            adgv.ColumnHeadersDefaultCellStyle = CellStyle2;
            // 标题栏高度
            adgv.ColumnHeadersHeight = 32;
            adgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            adgv.EnableHeadersVisualStyles = false;
            // 单元格样式
            CellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            CellStyle3.BackColor = Color.FromArgb(255, 255, 255);
            CellStyle3.SelectionBackColor = SystemColors.Highlight;
            CellStyle3.SelectionForeColor = SystemColors.HighlightText;

            CellStyle3.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CellStyle3.ForeColor = SystemColors.ControlText;

            CellStyle3.WrapMode = DataGridViewTriState.False;
            adgv.DefaultCellStyle = CellStyle3;
            // 行头边框样式
            adgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            // 行头样式
            CellStyle4.BackColor = Color.FromArgb(237, 237, 237);
            CellStyle4.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CellStyle4.ForeColor = SystemColors.WindowText;
            CellStyle4.SelectionBackColor = SystemColors.Highlight;
            CellStyle4.SelectionForeColor = SystemColors.HighlightText;
            CellStyle4.WrapMode = DataGridViewTriState.True;

            adgv.RowHeadersDefaultCellStyle = CellStyle4;
            adgv.RowHeadersWidth = 26;
            adgv.RowTemplate.Height = rowHeight;
            adgv.RowTemplate.Resizable = DataGridViewTriState.False;
            adgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 设置列比例和列显示模式
            if (ab_fill)
            {
                foreach (DataGridViewColumn dgvc in adgv.Columns) dgvc.FillWeight = (float)dgvc.Width;
                adgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            if (!enable)
            {
                adgv.AlternatingRowsDefaultCellStyle = null;
                CellStyle3.BackColor = Color.LightGray;
                CellStyle3.SelectionBackColor = Color.LightGray;
                CellStyle3.SelectionForeColor = Color.LightGray;
                CellStyle4.BackColor = Color.LightGray;
                CellStyle4.SelectionBackColor = Color.LightGray;
                CellStyle4.SelectionForeColor = Color.LightGray;
                adgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            }
        }

        public static void GridDisplayStyle(DataGridView adgv, bool ab_fill, bool enable = true)
        {
            DataGridViewCellStyle CellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle CellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle CellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle CellStyle4 = new DataGridViewCellStyle();
            // 奇数行的背景色
            CellStyle1.BackColor = Color.FromArgb(242, 245, 249);
            adgv.AlternatingRowsDefaultCellStyle = CellStyle1;

            // 表格背景色
            adgv.BackgroundColor = Color.White;
            // 表格标题栏的样式
            CellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            CellStyle2.BackColor = Color.FromArgb(237, 237, 237);
            CellStyle2.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CellStyle2.ForeColor = SystemColors.WindowText;
            CellStyle2.SelectionBackColor = SystemColors.Highlight;
            CellStyle2.SelectionForeColor = SystemColors.HighlightText;
            CellStyle2.WrapMode = DataGridViewTriState.True;
            adgv.ColumnHeadersDefaultCellStyle = CellStyle2;
            // 标题栏高度
            adgv.ColumnHeadersHeight = 32;
            adgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            adgv.EnableHeadersVisualStyles = false;
            // 单元格样式
            CellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            CellStyle3.BackColor = Color.FromArgb(255, 255, 255);
            CellStyle3.SelectionBackColor = SystemColors.Highlight;
            CellStyle3.SelectionForeColor = SystemColors.HighlightText;

            CellStyle3.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CellStyle3.ForeColor = SystemColors.ControlText;

            CellStyle3.WrapMode = DataGridViewTriState.False;
            adgv.DefaultCellStyle = CellStyle3;
            // 行头边框样式
            adgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            // 行头样式
            CellStyle4.BackColor = Color.FromArgb(237, 237, 237);
            CellStyle4.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            CellStyle4.ForeColor = SystemColors.WindowText;
            CellStyle4.SelectionBackColor = SystemColors.Highlight;
            CellStyle4.SelectionForeColor = SystemColors.HighlightText;
            CellStyle4.WrapMode = DataGridViewTriState.True;

            adgv.RowHeadersDefaultCellStyle = CellStyle4;
            adgv.RowHeadersWidth = 26;
            adgv.RowTemplate.Height = 32;
            adgv.RowTemplate.Resizable = DataGridViewTriState.False;
            adgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 设置列比例和列显示模式
            if (ab_fill)
            {
                foreach (DataGridViewColumn dgvc in adgv.Columns) dgvc.FillWeight = (float)dgvc.Width;
                adgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            if (!enable)
            {
                adgv.AlternatingRowsDefaultCellStyle = null;
                CellStyle3.BackColor = Color.LightGray;
                CellStyle3.SelectionBackColor = Color.LightGray;
                CellStyle3.SelectionForeColor = Color.LightGray;
                CellStyle4.BackColor = Color.LightGray;
                CellStyle4.SelectionBackColor = Color.LightGray;
                CellStyle4.SelectionForeColor = Color.LightGray;
                adgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            }
        }
    }
    #endregion

    #region 数据库类型转换
    public sealed class DataBaseManager
    {
        public static bool DataBaseConvert()
        {
            string ls_sql = "select * from IF_field_infor";
            DataTable ldt = new DataTable();
            if (!AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                return false;
            }
            if (Project.DB_Type.ToLower().Equals("sql server"))
            {

                for (int i = 0; i < ldt.Rows.Count; i++)
                {
                    ldt.Rows[i]["Field_Type"] = OracleToSql(ldt.Rows[i]["Field_Type"].ToString());
                }
                if (!AccessDBop.SQLUpdate(ls_sql, ref ldt))
                    return false;
            }
            else if (Project.DB_Type.ToLower().Equals("oracle"))
            {
                for (int i = 0; i < ldt.Rows.Count; i++)
                {
                    ldt.Rows[i]["Field_Type"] = SqlToOracle(ldt.Rows[i]["Field_Type"].ToString());
                }
                if (!AccessDBop.SQLUpdate(ls_sql, ref ldt))
                    return false;
            }

            ls_sql = "select * from IF_Table_infor where Table_Type = 'L'";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                for (int i = 0; i < ldt.Rows.Count; i++)
                {
                    string table_postion = ldt.Rows[i]["table_position"].ToString();
                    string db_type = table_postion.Equals("对方") ? Project.TheirDB_Type : Project.DB_Type;
                    string ls_SQL = GetCreateTableSQL(ldt.Rows[i]["Table_Name"].ToString(), db_type);
                    if (!AccessDBop.SQLExecute("update IF_Table_infor set CreateTable='" + ls_SQL.Replace("'", "''") + "' Where Table_Name='" + ldt.Rows[i]["Table_Name"].ToString() + "'"))
                    {
                        return false;
                    }
                }
            }


            ls_sql = "select * from IF_Table_Compare";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                for (int i = 0; i < ldt.Rows.Count; i++)
                {
                    FrmCompareTable fct = new FrmCompareTable(ldt.Rows[i]["TableName"].ToString());
                    fct.LoadData();
                    fct.Save();
                    fct.Dispose();
                }
            }
            return true;
        }

        private static string ConvertDataType_SQL_to_ORA(string SQLType)
        {
            string sResult = "";
            if (SQLType.ToLower().Contains("varchar2"))
                sResult = SQLType;
            else if (SQLType.ToLower().Contains("varchar"))
                sResult = SQLType.Replace("varchar", "varchar2");
            else if (SQLType.ToLower() == "int")
                sResult = "number(10)";
            else if (SQLType.ToLower() == "bigint")
                sResult = "number(10)";
            else if (SQLType.ToLower() == "smallint")
                sResult = "number(4)";
            else if (SQLType.ToLower() == "float")
                sResult = "number(10)";
            else if (SQLType.ToLower().Contains("datetime"))
                sResult = "date";
            else if (SQLType.ToLower() == "image")
                sResult = "blob";
            else
                sResult = SQLType;
            return sResult;
        }


        public static string GetCreateTableSQL(string Table_Name, string DBType)
        {
            string sql = "SELECT Table_Type FROM IF_Table_infor WHERE Table_Type='L' AND Table_Name = '" + Table_Name + "'";
            DataTable dt = new DataTable();
            if (!AccessDBop.SQLSelect(sql, ref dt))
            {
                return "error";
            }
            if (dt.Rows.Count == 0)
            {
                return "";
            }
            sql = "SELECT Field_Name,Field_Type,CanNull,IsPK,IsIdentity FROM IF_field_infor WHERE Table_Name = '" + Table_Name + "' ORDER BY ShowIndex";
            dt = new DataTable();
            if (!AccessDBop.SQLSelect(sql, ref dt))
            {
                return "error";
            }
            string DBName = DBType.ToLower().ToString();
            string CreateSQL = "";
            if (DBName.Equals("sql server"))
            {
                CreateSQL = "create table " + Table_Name + " (";
            }
            else if (DBName.Equals("oracle"))
            {
                //CreateSQL = "declare v_count number;begin Select count(*) into v_count from user_tables where table_name = '" + Table_Name.ToUpper() + "';if v_count = 0 then execute immediate 'create table " + Table_Name + " (";
                CreateSQL = "execute immediate 'create table " + Table_Name + " (";
            }
            else if (DBName.Equals("mysql"))
            {
                CreateSQL = "CREATE TABLE IF NOT EXISTS " + Table_Name + " (";
            }
            string PkSQL = "";
            string field_type = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                if (dr["IsPK"].ToString().ToLower().Equals("y"))
                {
                    PkSQL += (dr["Field_Name"] + ",");
                }
                CreateSQL += dr["Field_Name"].ToString();
                if (DBName.Equals("oracle"))
                    field_type = ConvertDataType_SQL_to_ORA(dr["Field_Type"].ToString());
                else
                    field_type = dr["Field_Type"].ToString();
                CreateSQL += " " + field_type;
                if (DBName.Equals("sql server"))
                {
                    if (dr["IsIdentity"].ToString().ToLower().Equals("y"))
                    {
                        CreateSQL += " IDENTITY (1, 1)";
                    }
                }
                if (dr["CanNull"].ToString().ToLower().Equals("n"))
                {
                    CreateSQL += " not null";
                }
                CreateSQL += ",";
            }
            if (!PkSQL.Equals(""))
            {
                PkSQL = PkSQL.Substring(0, PkSQL.Length - 1);
                PkSQL = string.Format("constraint PK_" + Table_Name + " primary key ({0})", PkSQL);
            }
            else
            {
                if (CreateSQL != "")
                    CreateSQL = CreateSQL.Substring(0, CreateSQL.Length - 1);
            }
            if (DBName.Equals("sql server"))
            {
                CreateSQL += PkSQL + ");";
            }
            else if (DBName.Equals("oracle"))
            {
                //CreateSQL += PkSQL + ")'; end if; end;";
                CreateSQL += PkSQL + ")';";
            }
            else if (DBName.Equals("mysql"))
            {
                CreateSQL += PkSQL + ");";
            }
            return CreateSQL;
        }

        private static string SqlToOracle(string type)
        {
            string Para = "";
            if (type.IndexOf("(") != -1)
            {
                string[] tempArr = type.Split('(');
                type = tempArr[0];
                Para = "(" + tempArr[1];
            }
            Dictionary<string, string> typeDict = new Dictionary<string, string>();
            typeDict.Add("varchar", "varchar2");
            typeDict.Add("nvarchar", "nvarchar2");
            typeDict.Add("bigint", "number(10)");
            typeDict.Add("decimal", "number");
            typeDict.Add("numeric", "number");
            typeDict.Add("money", "number");
            typeDict.Add("real", "float");
            typeDict.Add("datetime", "date");
            if (typeDict.ContainsKey(type.ToLower()))
            {
                return typeDict[type.ToLower()] + Para;
            }
            return type + Para;
        }

        private static string OracleToSql(string type)
        {
            string Para = "";
            if (type.IndexOf("(") != -1)
            {
                string[] tempArr = type.Split('(');
                type = tempArr[0];
                Para = "(" + tempArr[1];
            }
            Dictionary<string, string> typeDict = new Dictionary<string, string>();
            typeDict.Add("varchar2", "varchar");
            typeDict.Add("nvarchar2", "nvarchar");
            typeDict.Add("long", "ntext");
            typeDict.Add("number", "numeric");
            typeDict.Add("date", "datetime");
            if (typeDict.ContainsKey(type.ToLower()))
            {
                return typeDict[type.ToLower()] + Para;
            }
            return type + Para;
        }
    }
    #endregion

    #region ExcelHelper
    public class ExcelHelper
    {
        protected string templetFile = null;
        protected string outputFile = null;
        protected object missing = Missing.Value;
        public Excel.Application m_xlApp = null;

        //唯一需要解释的一点是这个连接字符串中，HDR=YES 表示此Excel表第一行用于显示字段名称（Header），如果没有字段名，则应 HDR=NO 
        //public static string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=K:\工具\教务接口学校\教师信息.Excel;Extended Properties=""Excel 8.0;HDR=NO;""";
        private static string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;"
            + @"Data Source=K:\工具\教务接口学校\教师信息.Excel;"
            + "Extended Properties='Excel 8.0;HDR=yes;IMEX=1'";

        /// <summary> 
        /// 读取 Excel 返回 DataSet 
        /// </summary> 
        /// <param name="connectionString">Excel 连接字符串</param> 
        /// <param name="commandString">查询语句, for example:"SELECT ID,userName,userAddress FROM [Sheet1$]" </param> 
        /// <returns></returns> 
        public static DataTable GetExcelDataTable()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");

            DbDataAdapter adapter = factory.CreateDataAdapter();


            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;

            connection.Open();

            DataTable ldt = connection.GetSchema("Tables");

            DbCommand selectCommand = factory.CreateCommand();
            //commandString例如:"SELECT ID,userName,userAddress FROM [Sheet1$]" 
            selectCommand.CommandText = "select * from [" + ldt.Rows[0][2].ToString() + "]";
            selectCommand.Connection = connection;
            adapter.SelectCommand = selectCommand;

            DataTable cities = new DataTable();
            adapter.Fill(cities);

            connection.Close();
            return cities;
        }
        /// <summary> 
        /// 读取 Excel 返回 DataSet 
        /// </summary> 
        /// <param name="connectionString">Excel 连接字符串</param> 
        /// <param name="commandString">查询语句, for example:"SELECT ID,userName,userAddress FROM [Sheet1$]" </param> 
        /// <returns></returns> 
        public static DataTable GetExcelDataTable(string Excel_name, string sheet_name, int ValidRowNo, int ValidColNo, string IsHaveHead)
        {
            try
            {
                string connectionString = "Provider= Microsoft.ACE.OLEDB.12.0;"
                + @"Data Source="
                + Excel_name
                + ";Extended Properties='Excel 12.0;HDR=" + IsHaveHead + ";IMEX=1'";
                DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
                DbDataAdapter adapter = factory.CreateDataAdapter();
                DbConnection connection = factory.CreateConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                DataTable ldt = connection.GetSchema("Tables");

                DbCommand selectCommand = factory.CreateCommand();
                //commandString例如:"SELECT ID,userName,userAddress FROM [Sheet1$]" 
                selectCommand.CommandText = "select * from [" + ldt.Rows[0][2].ToString() + "]";
                //selectCommand.CommandText = "select * from [" + sheet_name + "]";
                selectCommand.Connection = connection;
                adapter.SelectCommand = selectCommand;

                DataTable cities = new DataTable();
                adapter.Fill(cities);

                if (IsHaveHead == "yes")
                {
                    if (ValidRowNo > 1)                       //注：Excel中第一行有效数据之前的空行已被删除，所以有效行等于1的已经是标题了，不需要处理
                    {
                        for (int j = ValidColNo - 1; j < cities.Columns.Count; j++)           //取标题
                        {
                            cities.Columns[j].ColumnName = cities.Rows[ValidRowNo - 2][j].ToString();
                        }

                        List<DataRow> rowList = new List<DataRow>();   //删除非有效行
                        for (int j = 0; j < ValidRowNo - 1 + 4; j++)
                        {
                            rowList.Add(cities.Rows[j]);
                        }
                        foreach (DataRow item in rowList)
                        {
                            cities.Rows.Remove(item);
                        }
                    }
                }
                else
                {
                    List<DataRow> rowList = new List<DataRow>();   //删除非有效行
                    for (int j = 0; j < ValidRowNo - 1; j++)
                    {
                        rowList.Add(cities.Rows[j]);
                    }
                    foreach (DataRow item in rowList)
                    {
                        cities.Rows.Remove(item);
                    }
                }



                List<DataColumn> colList = new List<DataColumn>();   //删除非有效列
                for (int j = 1; j < ValidColNo; j++)
                {
                    colList.Add(cities.Columns[j - 1]);
                }
                foreach (DataColumn item in colList)
                {
                    cities.Columns.Remove(item);
                }

                connection.Close();
                return cities;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 导出到CSV或excel
        /// </summary>
        /// <param name="dt"></param>
        public static void DataTableToFile(DataTable dt, string FileName)
        {
            string s = "";
            if (FileName.ToLower().Contains("Excel"))    //保存成Excel用\t,CSV则用","
            {
                s = "\t";
            }
            else
            {
                s = ",";
            }

            Stream myStream = File.Open(FileName, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
            string columnTitle = "";
            try
            {
                //写入列标题
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        columnTitle += s;
                    }
                    columnTitle += dt.Columns[i].ColumnName;
                }
                sw.WriteLine(columnTitle);
                //写入列内容
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    string columnValue = "";
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            columnValue += s;
                        }
                        if (dt.Rows[j][k] == null)
                            columnValue += "";
                        else
                            columnValue += dt.Rows[j][k].ToString();
                    }
                    sw.WriteLine(columnValue);
                }
                sw.Close();
                myStream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }

        /// <summary>
        /// 导出到CSV或excel(实为txt文件)
        /// </summary>
        /// <param name="dt"></param>
        public static void DataTableToFile(System.Windows.Forms.ProgressBar pb, DataTable dt)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV files (*.csv)|*.csv|Execl files (*.Excel)|*.Excel";
            dlg.FilterIndex = 0;
            dlg.RestoreDirectory = true;
            dlg.CreatePrompt = true;
            //dlg.Title = "保存为CSV文件";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pb.Value = 0;
                pb.Visible = true;
                pb.Minimum = 0;
                pb.Maximum = dt.Rows.Count;
                pb.Step = 1;
                string s = "";
                if (dlg.FileName.Contains("Excel"))    //保存成Excel用\t,CSV则用","
                {
                    s = "\t";
                }
                else
                {
                    s = ",";
                }
                Stream myStream;
                myStream = dlg.OpenFile();
                StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
                string columnTitle = "";
                try
                {
                    //写入列标题
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i > 0)
                        {
                            columnTitle += s;
                        }
                        columnTitle += dt.Columns[i].ColumnName;
                    }
                    sw.WriteLine(columnTitle);
                    //写入列内容
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string columnValue = "";
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            if (k > 0)
                            {
                                columnValue += s;
                            }
                            if (dt.Rows[j][k] == null)
                                columnValue += "";
                            else
                                columnValue += dt.Rows[j][k].ToString();
                        }
                        sw.WriteLine(columnValue);
                        pb.PerformStep();
                    }
                    sw.Close();
                    myStream.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                finally
                {
                    sw.Close();
                    myStream.Close();
                    pb.Visible = false;
                }
            }
        }

        public static List<string> GetSheetlist(string Excel_name)
        {
            //唯一需要解释的一点是这个连接字符串中，HDR=YES 表示此Excel表第一行用于显示字段名称（Header），如果没有字段名，则应 HDR=NO 
            //public static string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=K:\工具\教务接口学校\教师信息.Excel;Extended Properties=""Excel 8.0;HDR=NO;""";
            try
            {
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;"
                                        + @"Data Source="
                                        + Excel_name
                                        + ";Extended Properties='Excel 8.0;HDR=yes;IMEX=1'";
                DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
                DbDataAdapter adapter = factory.CreateDataAdapter();
                DbConnection connection = factory.CreateConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                DataTable ldt = connection.GetSchema("Tables");

                List<string> sheet_list = new List<string>();
                for (int i = 0; i < ldt.Rows.Count; i++)
                //for (int i = 0; i < 3; i++)
                {
                    sheet_list.Add(ldt.Rows[i][2].ToString());
                }
                return sheet_list;
            }
            catch
            {
                return null;
            }
        }

        ///   <summary> 
        /// 构造函数，需指定模板文件和输出文件完整路径
        ///   </summary> 
        ///   <param name="templetFilePath"> Excel模板文件路径 </param> 
        ///   <param name="outputFilePath"> 输出Excel文件路径 </param> 
        public ExcelHelper(string templetFilePath, string outputFilePath)
        {
            if (templetFilePath == null)
                throw new Exception(" Excel模板文件路径不能为空！ ");
            //if (!File.Exists(templetFilePath))
            //    throw new Exception(" 指定路径的Excel模板文件不存在！ ");
            this.templetFile = templetFilePath;
            this.outputFile = outputFilePath;
            if (CheckPath() == false)
                throw new Exception(" 输出Excel文件路径不能为空！ ");
        }


        ///   <summary> 
        /// 获取WorkSheet数量
        ///   </summary> 
        ///   <param name="rowCount"> 记录总行数 </param> 
        ///   <param name="rows"> 每WorkSheet行数 </param> 
        private int GetSheetCount(int rowCount, int rows)
        {
            int n = rowCount % rows;         // 余数
            if (n == 0)
                return rowCount / rows;
            else
                return Convert.ToInt32(rowCount / rows) + 1;
        }



        /// <summary>
        /// 验证输出文件的路径，如果没有就新建
        /// </summary>
        /// <returns>true or false</returns>
        private bool CheckPath()
        {
            try
            {
                string tempPath = outputFile.Substring(0, outputFile.LastIndexOf("\\"));
                if (Directory.Exists(tempPath) == false)
                {
                    Directory.CreateDirectory(tempPath);
                }

                if (File.Exists(outputFile) == true)
                {
                    File.Move(outputFile, outputFile + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 列标号，Excel最大列数是256
        /// </summary>
        public static string[] SeqToXlsCol = new string[] {"",
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
            "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
            "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ",
            "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ",
            "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ",
            "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ",
            "GA", "GB", "GC", "GD", "GE", "GF", "GG", "GH", "GI", "GJ", "GK", "GL", "GM", "GN", "GO", "GP", "GQ", "GR", "GS", "GT", "GU", "GV", "GW", "GX", "GY", "GZ",
            "HA", "HB", "HC", "HD", "HE", "HF", "HG", "HH", "HI", "HJ", "HK", "HL", "HM", "HN", "HO", "HP", "HQ", "HR", "HS", "HT", "HU", "HV", "HW", "HX", "HY", "HZ",
            "IA", "IB", "IC", "ID", "IE", "IF", "IG", "IH", "II", "IJ", "IK", "IL", "IM", "IN", "IO", "IP", "IQ", "IR", "IS", "IT", "IU", "IV"
        };

        public void Export(DataSet ds)
        {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + "D:\\bb.xls" + ";" + "Extended Properties=\"Excel 8.0;HDR=NO;\"";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            System.Data.DataTable dt = ds.Tables[0];
            StringBuilder sb = new StringBuilder();
            string sql = "insert into " + "[Sheet1" + "$A1:I60000] values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')";
            int rowscount = dt.Rows.Count;
            string split = ";";
            for (int i = 0; i < rowscount; i++)
            {
                DataRow dr = dt.Rows[i];
                string sqlval = string.Format(sql,
                    dr[0].ToString(),
                    dr[1].ToString(),
                    dr[2].ToString(),
                    dr[3].ToString(),
                    dr[4].ToString(),
                    dr[5].ToString(),
                    dr[6].ToString(),
                    dr[7].ToString(),
                    dr[8].ToString());
                sb.Append(sqlval);
                sb.Append(split);
            }
            OleDbCommand ole = new OleDbCommand(sb.ToString(), conn);
            ole.ExecuteNonQuery();
            conn.Close();
        }


        private bool IsDefault(Dictionary<int, int> dicColCompare, DataTable dt)
        {
            if (dicColCompare.Count == 0)
            {
                return true;
            }
            if (dicColCompare.Count != dt.Columns.Count)
            {
                return false;
            }
            for (int i = 0; i < dicColCompare.Count; i++)
            {
                if (dicColCompare[i] != i)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsDefault(Dictionary<int, int> dicColCompare, DataGridView dgv)
        {
            if (dicColCompare.Count == 0)
            {
                return true;
            }
            if (dicColCompare.Count != dgv.Columns.Count)
            {
                return false;
            }
            for (int i = 0; i < dicColCompare.Count; i++)
            {
                if ((!dicColCompare.ContainsKey(i)) || (dicColCompare[i] != i))
                {
                    return false;
                }
            }
            return true;
        }


        #region 导出EXCEL方法二
        /// <summary>
        /// 此方法关键之处是使用Range一次存储内存中的多行多列数据到Excel
        /// 此方法效率明显高的多，推荐使用
        /// 此方法个人觉得很精彩，大家在看代码的时候可以使用设置断点来看每个变量值的变化
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="saveFileDialog"></param>
        public bool ToExcel2(DataTable dt, int RowIndex, int ColIndex, int rows, string ReportName, string depName, string sdate, string edate, Dictionary<int, int> dicColCompare)
        {
            int rowCount = dt.Rows.Count;         // 源DataTable行数 
            int sheetCount = this.GetSheetCount(rowCount, rows);     // WorkSheet个数
            bool bDefault = IsDefault(dicColCompare, dt);
            System.Reflection.Missing miss = System.Reflection.Missing.Value;
            //创建EXCEL对象appExcel,Workbook对象,Worksheet对象,Range对象
            Microsoft.Office.Interop.Excel.Application appExcel;
            appExcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbookData;
            Microsoft.Office.Interop.Excel.Worksheet worksheetData;
            Microsoft.Office.Interop.Excel.Range rangedata;
            //设置对象不可见
            appExcel.Visible = false;
            /* 在调用Excel应用程序，或创建Excel工作簿之前，记着加上下面的两行代码
             * 这是因为Excel有一个Bug，如果你的操作系统的环境不是英文的，而Excel就会在执行下面的代码时，报异常。
             */
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //workbookData = appExcel.Workbooks.Add(miss);
            try
            {
                workbookData = appExcel.Workbooks.Open(templetFile, missing, missing, missing, missing, missing,
                                       missing, missing, missing, missing, missing, missing, missing);
                //worksheetData = (Microsoft.Office.Interop.Excel.Worksheet)workbookData.Worksheets.Add(miss, miss, miss, miss);
                worksheetData = (Excel.Worksheet)workbookData.Sheets.get_Item(1);
                //给工作表赋名称
                worksheetData.Name = "Sheet1";
                // 复制sheetCount-1个WorkSheet对象 
                for (int i = 1; i < sheetCount; i++)
                {
                    ((Excel.Worksheet)workbookData.Worksheets.get_Item(i)).Copy(missing, workbookData.Worksheets[i]);
                }
                //清零计数并开始计数
                //TimeP = new System.DateTime(0);
                //timer1.Start();
                //label1.Text = TimeP.ToString("HH:mm:ss");

                //iColumnAccount为实际列数，最大列数
                int iColumnAccount = dt.Columns.Count;
                //int iColumnAccount = dicColCompare.Count == 0 ? dt.Columns.Count : dicColCompare.Count;
                //iEachSize为每次写行的数值，可以自己设置，每次写1000行和每次写2000行大家可以自己测试下效率
                int iEachSize = 1000;
                Microsoft.Office.Interop.Excel.Range xlRang = null;

                for (int si = 1; si <= sheetCount; si++)
                {
                    xlRang = null;
                    int iParstedRow = 0, iCurrSize = 0;
                    Excel.Worksheet sheet = (Excel.Worksheet)workbookData.Worksheets.get_Item(si);
                    sheet.Name = "Sheet-" + si.ToString();
                    //先给Range对象一个范围为A2开始，Range对象可以给一个CELL的范围，也可以给例如A1到H10这样的范围
                    //因为第一行已经写了表头，所以所有数据都应该从A2开始
                    rangedata = sheet.get_Range("A2", miss);

                    sheet.Cells[1, ColIndex] = ReportName;
                    sheet.Cells[2, ColIndex] = "单位:" + depName;
                    sheet.Cells[3, ColIndex] = "查询日期从:" + sdate + " 到 " + edate + "   打印时间: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm");


                    // 保存到WorkSheet的表头，你应该看到，是一个Cell一个Cell的存储，这样效率特别低，解决的办法是，使用Rang，一块一块地存储到Excel
                    if (bDefault)   //未定义规则默认导出全部字段
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sheet.Cells[RowIndex, ColIndex + i] = dt.Columns[i].ColumnName;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (dicColCompare.ContainsKey(i))
                            {
                                sheet.Cells[RowIndex, ColIndex + dicColCompare[i]] = dt.Columns[i].ColumnName;
                            }
                        }
                    }

                    //iRowCount为实际行数，最大行
                    int iLastRowCount = rowCount % rows == 0 ? rows : rowCount % rows;
                    int iRowCount = si == sheetCount ? iLastRowCount : rows;

                    //在内存中声明一个iEachSize×iColumnAccount的数组，iEachSize是每次最大存储的行数，iColumnAccount就是存储的实际列数
                    object[,] objVal = new object[iEachSize, iColumnAccount];

                    //给进度条赋最大值为实际行数最大值
                    //progressBar1.Maximum = gridView.RowCount;
                    iCurrSize = iEachSize;



                    while (iParstedRow < iRowCount)
                    {
                        if ((iRowCount - iParstedRow) < iEachSize)
                            iCurrSize = iRowCount - iParstedRow;
                        for (int i = 0; i < iCurrSize; i++)
                        {
                            if (bDefault)
                            {
                                for (int j = 0; j < iColumnAccount; j++)
                                {
                                    objVal[i, j] = dt.Rows[i + iParstedRow + (si - 1) * rows][j].ToString();
                                }
                            }
                            else
                            {
                                for (int j = 0; j < iColumnAccount; j++)
                                {
                                    if (dicColCompare.ContainsKey(j))
                                    {
                                        objVal[i, dicColCompare[j]] = dt.Rows[i + iParstedRow + (si - 1) * rows][j].ToString();
                                    }
                                }
                            }
                            //progressBar1.Value++;
                            System.Windows.Forms.Application.DoEvents();
                        }
                        /*
                         * 建议使用设置断点研究下哈
                         * 例如A1到H10的意思是从A到H，第一行到第十行
                         * 下句很关键，要保证获取orkSheet中对应的Range范围
                         * 下句实际上是得到这样的一个代码语句xlRang = worksheetData.get_Range("A2","H100");
                         * 注意看实现的过程
                         * 'A' + iColumnAccount - 1这儿是获取你的最后列，A的数字码为65，大家可以仔细看下是不是得到最后列的字母
                         * iParstedRow + iCurrSize + 1获取最后行
                         * 若WHILE第一次循环的话这应该是A2,最后列字母+最后行数字
                         * iParstedRow + 2要注意，每次循环这个值不一样，他取决于你每次循环RANGE取了多大，也就是iEachSize设置值的大小哦
                         */
                        xlRang = sheet.get_Range(SeqToXlsCol[ColIndex] + ((int)(iParstedRow + RowIndex + 1)).ToString(), SeqToXlsCol[ColIndex + iColumnAccount - 1] + ((int)(iParstedRow + iCurrSize + RowIndex)).ToString());
                        // 调用Range的Value2属性，把内存中的值赋给Excel
                        xlRang.Value2 = objVal;
                        iParstedRow = iParstedRow + iCurrSize;
                    }

                    //progressBar1.Value = 0;

                    //timer1.Stop();
                    //MessageBox.Show("数据已经成功导出到：" + saveFileDialog.FileName.ToString(), "导出完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //保存工作表
                workbookData.SaveAs(outputFile, miss, miss, miss, miss, miss, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, miss, miss, miss);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlRang);
                //调用方法关闭EXCEL进程，大家可以试下不用的话如果程序不关闭在进程里一直会有EXCEL.EXE这个进程并锁定你的EXCEL表格
                this.KillSpecialExcel(appExcel);
            }
            catch (Exception ex)
            {
                string[] arrInfo = new string[4];
                arrInfo[0] = "从数据集导出到电子表格出错";
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
                //timer1.Stop();
                return false;
            }
            // 别忘了在结束程序之前恢复你的环境！
            System.Threading.Thread.CurrentThread.CurrentCulture = CurrentCI;
            return true;
        }
        #endregion


        #region 导出EXCEL方法二
        /// <summary>
        /// 此方法关键之处是使用Range一次存储内存中的多行多列数据到Excel
        /// 此方法效率明显高的多，推荐使用
        /// 此方法个人觉得很精彩，大家在看代码的时候可以使用设置断点来看每个变量值的变化
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="saveFileDialog"></param>
        public bool ToExcel2(DataGridView dgv, int RowIndex, int ColIndex, int rows, string ReportName, string depName, string sdate, string edate, Dictionary<int, int> dicColCompare)
        {
            int rowCount = dgv.Rows.Count - 1;         // 源DataTable行数 
            int sheetCount = this.GetSheetCount(rowCount, rows);     // WorkSheet个数
            bool bDefault = IsDefault(dicColCompare, dgv);
            System.Reflection.Missing miss = System.Reflection.Missing.Value;
            //创建EXCEL对象appExcel,Workbook对象,Worksheet对象,Range对象
            Microsoft.Office.Interop.Excel.Application appExcel;
            appExcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbookData;
            Microsoft.Office.Interop.Excel.Worksheet worksheetData;
            Microsoft.Office.Interop.Excel.Range rangedata;
            //设置对象不可见
            appExcel.Visible = false;
            /* 在调用Excel应用程序，或创建Excel工作簿之前，记着加上下面的两行代码
             * 这是因为Excel有一个Bug，如果你的操作系统的环境不是英文的，而Excel就会在执行下面的代码时，报异常。
             */
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //workbookData = appExcel.Workbooks.Add(miss);
            try
            {
                workbookData = appExcel.Workbooks.Open(templetFile, missing, missing, missing, missing, missing,
                                       missing, missing, missing, missing, missing, missing, missing);
                //worksheetData = (Microsoft.Office.Interop.Excel.Worksheet)workbookData.Worksheets.Add(miss, miss, miss, miss);
                worksheetData = (Excel.Worksheet)workbookData.Sheets.get_Item(1);
                //给工作表赋名称
                worksheetData.Name = "Sheet1";
                // 复制sheetCount-1个WorkSheet对象 
                for (int i = 1; i < sheetCount; i++)
                {
                    ((Excel.Worksheet)workbookData.Worksheets.get_Item(i)).Copy(missing, workbookData.Worksheets[i]);
                }
                //清零计数并开始计数
                //TimeP = new System.DateTime(0);
                //timer1.Start();
                //label1.Text = TimeP.ToString("HH:mm:ss");

                //iColumnAccount为实际列数，最大列数
                int iColumnAccount = dgv.Columns.Count;
                //int iColumnAccount = dicColCompare.Count == 0 ? dt.Columns.Count : dicColCompare.Count;
                //iEachSize为每次写行的数值，可以自己设置，每次写1000行和每次写2000行大家可以自己测试下效率
                int iEachSize = 1000;
                Microsoft.Office.Interop.Excel.Range xlRang = null;

                for (int si = 1; si <= sheetCount; si++)
                {
                    xlRang = null;
                    int iParstedRow = 0, iCurrSize = 0;
                    Excel.Worksheet sheet = (Excel.Worksheet)workbookData.Worksheets.get_Item(si);
                    sheet.Name = "Sheet-" + si.ToString();
                    //先给Range对象一个范围为A2开始，Range对象可以给一个CELL的范围，也可以给例如A1到H10这样的范围
                    //因为第一行已经写了表头，所以所有数据都应该从A2开始
                    rangedata = sheet.get_Range("A2", miss);

                    //sheet.Cells[1, ColIndex] = ReportName;
                    //sheet.Cells[2, ColIndex] = "单位:" + depName;
                    //sheet.Cells[3, ColIndex] = "查询日期从:" + sdate + " 到 " + edate + "   打印时间: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm");


                    // 保存到WorkSheet的表头，你应该看到，是一个Cell一个Cell的存储，这样效率特别低，解决的办法是，使用Rang，一块一块地存储到Excel
                    if (bDefault)   //未定义规则默认导出全部字段
                    {
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            sheet.Cells[RowIndex, ColIndex + i] = dgv.Columns[i].HeaderText;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            if (dicColCompare.ContainsKey(i))
                            {
                                sheet.Cells[RowIndex, ColIndex + dicColCompare[i]] = dgv.Columns[i].HeaderText;
                            }
                        }
                    }

                    //iRowCount为实际行数，最大行
                    int iLastRowCount = rowCount % rows == 0 ? rows : rowCount % rows;
                    int iRowCount = si == sheetCount ? iLastRowCount : rows;

                    //在内存中声明一个iEachSize×iColumnAccount的数组，iEachSize是每次最大存储的行数，iColumnAccount就是存储的实际列数
                    object[,] objVal = new object[iEachSize, iColumnAccount];

                    //给进度条赋最大值为实际行数最大值
                    //progressBar1.Maximum = gridView.RowCount;
                    iCurrSize = iEachSize;



                    while (iParstedRow < iRowCount)
                    {
                        if ((iRowCount - iParstedRow) < iEachSize)
                            iCurrSize = iRowCount - iParstedRow;
                        for (int i = 0; i < iCurrSize; i++)
                        {
                            if (bDefault)
                            {
                                for (int j = 0; j < iColumnAccount; j++)
                                {
                                    objVal[i, j] = dgv.Rows[i + iParstedRow + (si - 1) * rows].Cells[j].Value.ToString();
                                }
                            }
                            else
                            {
                                for (int j = 0; j < iColumnAccount; j++)
                                {
                                    if (dicColCompare.ContainsKey(j))
                                    {
                                        objVal[i, dicColCompare[j]] = dgv.Rows[i + iParstedRow + (si - 1) * rows].Cells[j].Value.ToString();
                                    }
                                }
                            }
                            //progressBar1.Value++;
                            System.Windows.Forms.Application.DoEvents();
                        }
                        /*
                         * 建议使用设置断点研究下哈
                         * 例如A1到H10的意思是从A到H，第一行到第十行
                         * 下句很关键，要保证获取orkSheet中对应的Range范围
                         * 下句实际上是得到这样的一个代码语句xlRang = worksheetData.get_Range("A2","H100");
                         * 注意看实现的过程
                         * 'A' + iColumnAccount - 1这儿是获取你的最后列，A的数字码为65，大家可以仔细看下是不是得到最后列的字母
                         * iParstedRow + iCurrSize + 1获取最后行
                         * 若WHILE第一次循环的话这应该是A2,最后列字母+最后行数字
                         * iParstedRow + 2要注意，每次循环这个值不一样，他取决于你每次循环RANGE取了多大，也就是iEachSize设置值的大小哦
                         */
                        xlRang = sheet.get_Range(SeqToXlsCol[ColIndex] + ((int)(iParstedRow + RowIndex + 1)).ToString(), SeqToXlsCol[ColIndex + iColumnAccount - 1] + ((int)(iParstedRow + iCurrSize + RowIndex)).ToString());
                        // 调用Range的Value2属性，把内存中的值赋给Excel
                        xlRang.Value2 = objVal;
                        iParstedRow = iParstedRow + iCurrSize;
                    }

                    //progressBar1.Value = 0;

                    //timer1.Stop();
                    //MessageBox.Show("数据已经成功导出到：" + saveFileDialog.FileName.ToString(), "导出完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //保存工作表
                workbookData.SaveAs(outputFile, miss, miss, miss, miss, miss, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, miss, miss, miss);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlRang);
                //调用方法关闭EXCEL进程，大家可以试下不用的话如果程序不关闭在进程里一直会有EXCEL.EXE这个进程并锁定你的EXCEL表格
                this.KillSpecialExcel(appExcel);
            }
            catch (Exception ex)
            {
                string[] arrInfo = new string[4];
                arrInfo[0] = "从数据集导出到电子表格出错";
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
                //timer1.Stop();
                return false;
            }
            // 别忘了在结束程序之前恢复你的环境！
            System.Threading.Thread.CurrentThread.CurrentCulture = CurrentCI;
            return true;
        }
        #endregion


        #region 结束EXCEL.EXE进程的方法
        /// <summary>
        /// 结束EXCEL.EXE进程的方法
        /// </summary>
        /// <param name="m_objExcel">EXCEL对象</param>
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public void KillSpecialExcel(Microsoft.Office.Interop.Excel.Application m_objExcel)
        {
            try
            {
                if (m_objExcel != null)
                {
                    int lpdwProcessId;
                    GetWindowThreadProcessId(new IntPtr(m_objExcel.Hwnd), out lpdwProcessId);

                    System.Diagnostics.Process.GetProcessById(lpdwProcessId).Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
    #endregion ExcelHelper

    #region ini文件读写
    /// <summary>
    /// IniFiles的类
    /// </summary>
    public class IniFiles
    {
        public string FileName; //INI文件名
        //声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);
        //类的构造函数，传递INI文件名
        public IniFiles(string AFileName)
        {
            // 判断文件是否存在
            FileInfo fileInfo = new FileInfo(AFileName);
            //Todo:搞清枚举的用法
            if ((!fileInfo.Exists))
            { //|| (FileAttributes.Directory in fileInfo.Attributes))
                //文件不存在，建立文件
                System.IO.StreamWriter sw = new System.IO.StreamWriter(AFileName, false, System.Text.Encoding.Default);
                try
                {
                    sw.Write("#表格配置档案");
                    sw.Close();
                }
                catch
                {
                    throw (new ApplicationException("Ini文件不存在"));
                }
            }
            //必须是完全路径，不能是相对路径
            FileName = fileInfo.FullName;
        }
        //写INI文件
        public void WriteString(string Section, string Ident, string Value)
        {
            if (!WritePrivateProfileString(Section, Ident, Value, FileName))
            {
                throw (new ApplicationException("写Ini文件出错"));
            }
        }
        //读取INI文件指定
        public string ReadString(string Section, string Ident, string Default)
        {
            Byte[] Buffer = new Byte[65535];
            int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);
            //必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
            string s = Encoding.GetEncoding(0).GetString(Buffer);
            s = s.Substring(0, bufLen);
            return s.Trim();
        }

        //读整数
        public int ReadInteger(string Section, string Ident, int Default)
        {
            string intStr = ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToInt32(intStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        //写整数
        public void WriteInteger(string Section, string Ident, int Value)
        {
            WriteString(Section, Ident, Value.ToString());
        }

        //读布尔
        public bool ReadBool(string Section, string Ident, bool Default)
        {
            try
            {
                return Convert.ToBoolean(ReadString(Section, Ident, Convert.ToString(Default)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        //写Bool
        public void WriteBool(string Section, string Ident, bool Value)
        {
            WriteString(Section, Ident, Convert.ToString(Value));
        }

        //从Ini文件中，将指定的Section名称中的所有Ident添加到列表中
        public void ReadSection(string Section, StringCollection Idents)
        {
            Byte[] Buffer = new Byte[16384];
            //Idents.Clear();

            int bufLen = GetPrivateProfileString(Section, null, null, Buffer, Buffer.GetUpperBound(0),
                  FileName);
            //对Section进行解析
            GetStringsFromBuffer(Buffer, bufLen, Idents);
        }

        private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, StringCollection Strings)
        {
            Strings.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {
                        String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
                        Strings.Add(s);
                        start = i + 1;
                    }
                }
            }
        }
        //从Ini文件中，读取所有的Sections的名称
        public void ReadSections(StringCollection SectionList)
        {
            //Note:必须得用Bytes来实现，StringBuilder只能取到第一个Section
            byte[] Buffer = new byte[65535];
            int bufLen = 0;
            bufLen = GetPrivateProfileString(null, null, null, Buffer,
             Buffer.GetUpperBound(0), FileName);
            GetStringsFromBuffer(Buffer, bufLen, SectionList);
        }
        //读取指定的Section的所有Value到列表中
        public void ReadSectionValues(string Section, NameValueCollection Values)
        {
            StringCollection KeyList = new StringCollection();
            ReadSection(Section, KeyList);
            Values.Clear();
            foreach (string key in KeyList)
            {
                Values.Add(key, ReadString(Section, key, ""));
            }
        }
        ////读取指定的Section的所有Value到列表中，
        //public void ReadSectionValues(string Section, NameValueCollection Values,char splitString)
        //{　 string sectionValue;
        //　　string[] sectionValueSplit;
        //　　StringCollection KeyList = new StringCollection();
        //　　ReadSection(Section, KeyList);
        //　　Values.Clear();
        //　　foreach (string key in KeyList)
        //　　{
        //　　　　sectionValue=ReadString(Section, key, "");
        //　　　　sectionValueSplit=sectionValue.Split(splitString);
        //　　　　Values.Add(key, sectionValueSplit[0].ToString(),sectionValueSplit[1].ToString());

        //　　}
        //}
        //清除某个Section
        public void EraseSection(string Section)
        {
            if (!WritePrivateProfileString(Section, null, null, FileName))
            {
                throw (new ApplicationException("无法清除Ini文件中的Section"));
            }
        }
        //删除某个Section下的键
        public void DeleteKey(string Section, string Ident)
        {
            WritePrivateProfileString(Section, Ident, null, FileName);
        }
        //Note:对于Win9X，来说需要实现UpdateFile方法将缓冲中的数据写入文件
        //在Win NT, 2000和XP上，都是直接写文件，没有缓冲，所以，无须实现UpdateFile
        //执行完对Ini文件的修改之后，应该调用本方法更新缓冲区。
        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, FileName);
        }

        //检查某个Section下的某个键值是否存在
        public bool ValueExists(string Section, string Ident)
        {
            StringCollection Idents = new StringCollection();
            ReadSection(Section, Idents);
            return Idents.IndexOf(Ident) > -1;
        }

        //确保资源的释放
        ~IniFiles()
        {
            UpdateFile();
        }
    }
    #endregion

    #region webservice动态调用
    public class WSHelper
    {
        /// <summary>
        /// xml转datatable
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static DataTable XMLToDatatable(string xml)
        {
            DataSet ds = new DataSet();
            StringReader streamReader = new StringReader(xml);
            XmlTextReader xtReader = new XmlTextReader(streamReader);
            ds.ReadXml(xtReader);
            return ds.Tables[ds.Tables.Count - 1].Copy();
        }

        /// < summary> 
        /// 动态调用web服务 
        /// < /summary> 
        /// < param name="url">WSDL服务地址< /param> 
        /// < param name="classname">类名< /param> 
        /// < param name="methodname">方法名< /param> 
        /// < param name="args">参数< /param> 
        /// < returns>< /returns> 
        public static string InvokeWebService(string url, string classname, string methodname, object[] args, ref DataTable dt)
        {
            string sResult = string.Empty;
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = WSHelper.GetWsClassName(url);
            }
            try
            { //获取WSDL 
                WebClient wc = new WebClient();
                //Stream stream = wc.OpenRead(url + "?WSDL");
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码 
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();
                //设定编译参数 
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类 
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    return sb.ToString();
                }
                //生成代理实例，并调用方法 
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                object data = mi.Invoke(obj, args);

                DataSet ds = new DataSet();
                try
                {
                    string xml = data.ToString();
                    dt = XMLToDatatable(xml);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                return sResult;
            }
            catch (Exception ex)
            {
                return ex.Message;
                //MessageBox.Show(ex.Message);
                //throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }
        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }
    #endregion

    #region http取数据
    public static class HttpHelper
    {
        /// <summary>    
        /// 将 Json 解析成 DateTable。   
        /// Json 数据格式如:  
        ///     {table:[{column1:1,column2:2,column3:3},{column1:1,column2:2,column3:3}]} /// </summary>    
        /// <param name="strJson">要解析的 Json 字符串</param>    
        /// <returns>返回 DateTable</returns>    
        public static DataTable JsonToDataTable(string strJson)
        {
            // 取出表名    
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            // 去除表名    
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));
            // 获取数据    
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split(',');
                // 创建表    
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split(':');
                        dc.ColumnName = strCell[0].Replace("\"", "");
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }
                // 增加内容    
                DataRow dr = tb.NewRow();
                for (int j = 0; j < strRows.Length; j++)
                {
                    dr[j] = strRows[j].Split(':')[1].Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
        }

        public static string selectData(string url, string format, ref DataTable dt)
        {
            try
            {
                var uri = new Uri(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json;charset=utf-8";
                request.Method = "GET";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8);
                String temp = string.Empty;
                while (streamRead.Peek() > -1)
                {
                    String input = streamRead.ReadLine();
                    temp += input;
                }
                streamRead.Close();
                if (format.ToLower() == "xml")
                    dt = WSHelper.XMLToDatatable(temp);
                else
                    dt = JsonToDataTable(temp);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }
    #endregion

    #region 读写配置文件
    public static class ConfigHelper
    {
        //依据连接串名字connectionName返回数据连接字符串  
        public static string GetConnectionStringsConfig(string connectionName)
        {
            //指定config文件读取
            string file = System.Windows.Forms.Application.ExecutablePath;
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            string connectionString =
                config.ConnectionStrings.ConnectionStrings[connectionName].ConnectionString.ToString();
            return connectionString;
        }

        ///<summary> 
        ///更新连接字符串  
        ///</summary> 
        ///<param name="newName">连接字符串名称</param> 
        ///<param name="newConString">连接字符串内容</param> 
        ///<param name="newProviderName">数据提供程序名称</param> 
        public static void UpdateConnectionStringsConfig(string newName, string newConString, string newProviderName)
        {
            //指定config文件读取
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);

            bool exist = false; //记录该连接串是否已经存在  
            //如果要更改的连接串已经存在  
            if (config.ConnectionStrings.ConnectionStrings[newName] != null)
            {
                exist = true;
            }
            // 如果连接串已存在，首先删除它  
            if (exist)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(newName);
            }
            //新建一个连接字符串实例  
            ConnectionStringSettings mySettings =
                new ConnectionStringSettings(newName, newConString, newProviderName);
            // 将新的连接串添加到配置文件中.  
            config.ConnectionStrings.ConnectionStrings.Add(mySettings);
            // 保存对配置文件所作的更改  
            config.Save(ConfigurationSaveMode.Modified);
            // 强制重新载入配置文件的ConnectionStrings配置节  
            ConfigurationManager.RefreshSection("ConnectionStrings");
        }

        ///<summary> 
        ///返回*.exe.config文件中appSettings配置节的value项  
        ///</summary> 
        ///<param name="strKey"></param> 
        ///<returns></returns> 
        public static string GetAppConfig(string strKey)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }

        ///<summary>  
        ///在*.exe.config文件中appSettings配置节增加一对键值对  
        ///</summary>  
        ///<param name="newKey"></param>  
        ///<param name="newValue"></param>  
        public static void UpdateAppConfig(string newKey, string newValue)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            bool exist = false;
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == newKey)
                {
                    exist = true;
                }
            }
            if (exist)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            config.AppSettings.Settings.Add(newKey, newValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        // 修改applicationSettings中App.Properties.Settings中服务的IP地址
        public static void UpdateConfig(string configPath, string serverIP)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(configPath);
            ConfigurationSectionGroup sec = config.SectionGroups["applicationSettings"];
            ConfigurationSection configSection = sec.Sections["DataService.Properties.Settings"];
            ClientSettingsSection clientSettingsSection = configSection as ClientSettingsSection;
            if (clientSettingsSection != null)
            {
                SettingElement element1 = clientSettingsSection.Settings.Get("DataService_SystemManagerWS_SystemManagerWS");
                if (element1 != null)
                {
                    clientSettingsSection.Settings.Remove(element1);
                    string oldValue = element1.Value.ValueXml.InnerXml;
                    element1.Value.ValueXml.InnerXml = GetNewIP(oldValue, serverIP);
                    clientSettingsSection.Settings.Add(element1);
                }

                SettingElement element2 = clientSettingsSection.Settings.Get("DataService_EquipManagerWS_EquipManagerWS");
                if (element2 != null)
                {
                    clientSettingsSection.Settings.Remove(element2);
                    string oldValue = element2.Value.ValueXml.InnerXml;
                    element2.Value.ValueXml.InnerXml = GetNewIP(oldValue, serverIP);
                    clientSettingsSection.Settings.Add(element2);
                }
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("applicationSettings");
        }

        private static string GetNewIP(string oldValue, string serverIP)
        {
            string pattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
            string replacement = string.Format("{0}", serverIP);
            string newvalue = Regex.Replace(oldValue, pattern, replacement);
            return newvalue;
        }
    }
    #endregion
}
