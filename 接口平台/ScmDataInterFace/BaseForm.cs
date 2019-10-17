using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net;

namespace ScmDataInterFace
{
    public partial class BaseForm : Form
    {
        protected bool Modified = false;

        public BaseForm()
        {
            InitializeComponent();
        }

        public virtual void LoadData() { }
        public virtual void SaveData() { }

        private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Modified)
            {
                DialogResult dr = MessageBox.Show("数据已修改，是否保存数据？", "提示",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);
                switch (dr)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        SaveData();
                        break;
                    default:
                        break;
                }
            }
        }

        private void BaseForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BaseForm_Shown(object sender, EventArgs e)
        {
            Modified = false;
        }
    }


    public static class TaskManager
    {

        private static string GetGroupData(DataTable dtTask, int dtTaskRowIndex, ref DataTable dtGroupData, int iDirection)
        {
            string sResult = "";
            bool ls_result = true;
            //插入数据
            string sGroupSql = dtTask.Rows[dtTaskRowIndex]["GroupSql"].ToString();
            if (sGroupSql.Trim().Equals(""))
            {
                return sResult;
            }
            if (iDirection == 0)
            {
                if (Project.TheirDB_Type.ToLower().Equals("sql server"))
                    ls_result = SQLServerDBop.SQLSelect(Project.TheirDB_Connection, sGroupSql, ref dtGroupData, "");
                else if (Project.TheirDB_Type.ToLower().Equals("oracle"))
                    ls_result = OracleDBop.SQLSelect(Project.TheirDB_Connection, sGroupSql, ref dtGroupData);
                else if (Project.TheirDB_Type.ToLower().Equals("mysql"))
                    ls_result = MySqlDBop.SQLSelect(Project.TheirDB_Connection, sGroupSql, ref dtGroupData);
                else if (Project.TheirDB_Type.ToLower().Equals("access"))
                    ls_result = AccessDBop.SQLSelect(Project.TheirDB_Connection, sGroupSql, ref dtGroupData);
            }
            else
            {
                if (Project.DB_Type.ToLower().Equals("sql server"))
                    ls_result = SQLServerDBop.SQLSelect(Project.DB_Connection, sGroupSql, ref dtGroupData, "");
                else if (Project.DB_Type.ToLower().Equals("oracle"))
                    ls_result = OracleDBop.SQLSelect(Project.DB_Connection, sGroupSql, ref dtGroupData);
                else if (Project.DB_Type.ToLower().Equals("mysql"))
                    ls_result = MySqlDBop.SQLSelect(Project.DB_Connection, sGroupSql, ref dtGroupData);
                else if (Project.DB_Type.ToLower().Equals("access"))
                    ls_result = AccessDBop.SQLSelect(Project.DB_Connection, sGroupSql, ref dtGroupData);
            }
            if (!ls_result)
            {
                sResult = "获取分组数据失败，【执行语句】:" + sGroupSql;
            }
            return sResult;
        }




        /// <summary>
        /// 生成获取数据的SQL语句
        /// </summary>
        /// <param name="dtTask">任务表数据集</param>
        /// <param name="dtTaskRowIndex">当前任务记录</param>
        /// <param name="dtGroupData">分组数据集</param>
        /// <param name="dtGroupDataRowIndex">当前分组记录</param>
        /// <param name="iDirection">方向:0从对方取，1从我方取</param>
        /// <returns></returns>
        private static string MakeSql_SelectData(DataTable dtTask, int dtTaskRowIndex, DataTable dtGroupData, int dtGroupDataRowIndex, int iDirection)
        {

            string fieldName_WhoSql = iDirection == 0 ? "TheirSql" : "OurSql";
            string ls_Relation = (dtTask.Rows[dtTaskRowIndex][fieldName_WhoSql].ToString().ToLower().IndexOf("where") > 0 ? " and " : " where ");
            string ls_groupsql = "";
            string[] fields = dtTask.Rows[dtTaskRowIndex]["GroupField"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string ls_quot = "'";
            foreach (string field in fields)
            {
                if (dtGroupData.Rows.Count == 0)
                    break;
                if (dtGroupData.Columns[field].DataType == System.Type.GetType("System.Boolean")
                    || dtGroupData.Columns[field].DataType == System.Type.GetType("System.Int16")
                    || dtGroupData.Columns[field].DataType == System.Type.GetType("System.Int32")
                    || dtGroupData.Columns[field].DataType == System.Type.GetType("System.Decimal")
                    || dtGroupData.Columns[field].DataType == System.Type.GetType("System.Int64"))
                    ls_quot = "";
                else
                    ls_quot = "'";
                string ls_val = (dtGroupData.Rows[dtGroupDataRowIndex][field] == DBNull.Value ? "null" : dtGroupData.Rows[dtGroupDataRowIndex][field].ToString());
                if (dtGroupData.Columns[field].DataType == System.Type.GetType("System.Boolean"))
                {
                    ls_val = (ls_val.ToLower() == "true" ? "1" : ls_val);
                    ls_val = (ls_val.ToLower() == "false" ? "0" : ls_val);
                }
                ls_quot = (ls_val == "null" ? "" : ls_quot);
                string ls_logic = (ls_val == "null" ? " is " : " = "); ;

                ls_groupsql += (ls_groupsql == "" ? "" : " and ") + field + ls_logic + ls_quot + ls_val + ls_quot;
            }
            string sql = (dtGroupData.Rows.Count == 0 ? dtTask.Rows[dtTaskRowIndex][fieldName_WhoSql].ToString() : dtTask.Rows[dtTaskRowIndex][fieldName_WhoSql].ToString() + ls_Relation + ls_groupsql);
            return sql;
        }

        private static string GetFieldRecordBh(string sql)
        {
            string sResult = "";
            string sTemp = "@bh";
            int index = sql.IndexOf(sTemp);
            sTemp = sql.Substring(0, index - 1);
            string[] ArrStemp = sTemp.Split(new string[] { " ", ">" }, StringSplitOptions.RemoveEmptyEntries);
            sResult = ArrStemp[ArrStemp.Length - 1];
            return sResult;
        }

        public static string InsertLog(string[] arrInfo)
        {
            string sj = arrInfo[4];
            string operate = arrInfo[0];
            string infor = arrInfo[1];
            string schedule_name = arrInfo[3];
            int state = 0;
            int.TryParse(arrInfo[2], out state);

            string sql = "INSERT INTO [syn_log] ([sj],[schedule_name],[operate],[infor],[state]) VALUES ('{0}','{1}','{2}','{3}',{4})";
            sql = string.Format(sql, sj, schedule_name, operate, infor, state);
            return SQLServerDBop.StrSQLExecute(schedule_name, Project.DB_Connection, sql);
        }
        /// <summary>
        /// 从数据库取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dtResourceData"></param>
        /// <param name="iDirection"></param>
        /// <returns></returns>
        private static string SelectData(string sql, int iDirection, ref DataTable dtResourceData)
        {
            string sResult = "";
            bool ls_result = true;

            //插入数据
            if (iDirection == 0)
            {
                if (Project.TheirDB_Type.ToLower().Equals("sql server"))
                    ls_result = SQLServerDBop.SQLSelect(Project.TheirDB_Connection, sql, ref dtResourceData, "");
                else if (Project.TheirDB_Type.ToLower().Equals("oracle"))
                    ls_result = OracleDBop.SQLSelect(Project.TheirDB_Connection, sql, ref dtResourceData);
                else if (Project.TheirDB_Type.ToLower().Equals("mysql"))
                    ls_result = MySqlDBop.SQLSelect(Project.TheirDB_Connection, sql, ref dtResourceData);
                else if (Project.TheirDB_Type.ToLower().Equals("access"))
                    ls_result = AccessDBop.SQLSelect(Project.TheirDB_Connection, sql, ref dtResourceData);
            }
            else
            {
                if (Project.DB_Type.ToLower().Equals("sql server"))
                    ls_result = SQLServerDBop.SQLSelect(Project.DB_Connection, sql, ref dtResourceData, "");
                else if (Project.DB_Type.ToLower().Equals("oracle"))
                    ls_result = OracleDBop.SQLSelect(Project.DB_Connection, sql, ref dtResourceData);
                else if (Project.DB_Type.ToLower().Equals("mysql"))
                    ls_result = MySqlDBop.SQLSelect(Project.DB_Connection, sql, ref dtResourceData);
                else if (Project.DB_Type.ToLower().Equals("access"))
                    ls_result = AccessDBop.SQLSelect(Project.DB_Connection, sql, ref dtResourceData);
            }
            if (!ls_result)
            {
                sResult = ApplicationSetting.Translate("get data error") + ApplicationSetting.Translate("run sql") + ":" + sql;
            }


            return sResult;
        }


        /// <summary>
        /// 从webservice取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dtResourceData"></param>
        /// <param name="iDirection"></param>
        /// <returns></returns>
        private static string SelectData(string tableName, string url, ref DataTable dtResourceData)
        {
            string sResult = "";
            string methodName = string.Empty;
            string[] arrMethod = null;
            object[] args = null;
            arrMethod = tableName.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (arrMethod.Length > 1)
            {
                methodName = arrMethod[0];
                args = new object[arrMethod.Length - 1];
                for (int i = 0; i < arrMethod.Length - 1; i++)
                {
                    args[i] = arrMethod[i + 1];
                }
            }
            else if (arrMethod.Length == 1)
            {
                methodName = arrMethod[0];
            }
            else
            {
                return "内容不能为空!";
            }

            sResult = WSHelper.InvokeWebService(url, null, methodName, args, ref dtResourceData);
            return sResult;
        }

        /// <summary>
        /// http取数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dtResourceData"></param>
        /// <returns></returns>
        private static string SelectData_http(string url, int iDirection, ref DataTable dtResourceData)
        {
            string sResult = "";
            string xml = iDirection == 0 ? Project.theirSeverName : Project.SeverName;
            sResult = HttpHelper.selectData(url, xml, ref dtResourceData);
            return sResult;
        }


        private static string dealDtResourceData(DataTable dtTask, int dtTaskRowIndex, int iDirection, DataTable dtResourceData, ref DataTable dt)
        {
            string fieldName_whoTableName = iDirection == 0 ? "TheirTableName" : "OurTableName";
            string methodName = dtTask.Rows[dtTaskRowIndex][fieldName_whoTableName].ToString().Trim();

            string fieldName_WhoSql = iDirection == 0 ? "TheirSql" : "OurSql";
            string selectSql = dtTask.Rows[dtTaskRowIndex][fieldName_WhoSql].ToString().Trim();

            string[] arrFields = selectSql.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<string> listFields = new List<string>();
            List<string> listFields_aliasName = new List<string>();
            for (int i = 0; i < arrFields.Length; i++)
            {
                string[] arrTmp = arrFields[i].Split(new string[] { " " }, StringSplitOptions.None);
                if (arrTmp.Length > 1)
                {
                    listFields.Add(arrTmp[0]);
                    listFields_aliasName.Add(arrTmp[1]);
                }
            }

            if (listFields.Count < 1)
            {
                return "无字段信息!";
            }

            List<string> lsTmp = new List<string>();
            foreach (DataColumn dc in dtResourceData.Columns)
            {
                if (!listFields.Contains(dc.ColumnName))
                {
                    lsTmp.Add(dc.ColumnName);
                }
            }
            foreach (string sTmp in lsTmp)
            {
                dtResourceData.Columns.Remove(sTmp);
            }
            if (dtResourceData.Columns.Count != listFields.Count)
            {
                return "已设置的字段不在webservice返回的字段范围或者webservice的同一字段对应对方多个字段!";
            }
            for (int i = 0; i < listFields.Count; i++)
            {
                string fName = listFields[i];
                string fName_alias = listFields_aliasName[i];
                if (dtResourceData.Columns.Contains(fName))
                {
                    dtResourceData.Columns[fName].SetOrdinal(i);
                    dtResourceData.Columns[fName].ColumnName = fName_alias;
                    dt = dtResourceData;
                }
                else
                {
                    return "webservice返回的数据集中不包含字段:" + fName;
                }
            }
            return "";
        }

        private static List<string> GetFieldName_InsertData(DataTable dtTask, int dtTaskRowIndex, int iDirection)
        {
            string fieldName_WhoSql = iDirection == 0 ? "OurSql" : "TheirSql";
            string sql = dtTask.Rows[dtTaskRowIndex][fieldName_WhoSql].ToString().Trim();
            int startIndex = sql.IndexOf("(");
            string sFields = sql.Substring(startIndex);
            string[] fieldArr = sFields.Split(new string[] { ",", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
            return new List<string>(fieldArr);
        }

        private static List<string> MakeSql_InsertData(DataTable dtTask, int dtTaskRowIndex, DataTable dtResourceData, int iDirection)
        {
            string fieldName_WhoSql = iDirection == 0 ? "OurSql" : "TheirSql";
            List<string> ll_insert = new List<string>();
            for (int j = 0; j < dtResourceData.Rows.Count; j++)
            {
                string ls_values = "";
                string ls_quot = "";
                for (int k = 0; k < dtResourceData.Columns.Count; k++)
                {
                    if (dtResourceData.Columns[k].DataType == System.Type.GetType("System.Boolean")
                        || dtResourceData.Columns[k].DataType == System.Type.GetType("System.Int16")
                        || dtResourceData.Columns[k].DataType == System.Type.GetType("System.Int32")
                        || dtResourceData.Columns[k].DataType == System.Type.GetType("System.Decimal")
                        || dtResourceData.Columns[k].DataType == System.Type.GetType("System.Int64"))
                        ls_quot = "";
                    else
                        ls_quot = "'";
                    string ls_val = (dtResourceData.Rows[j][k] == DBNull.Value ? "null" : dtResourceData.Rows[j][k].ToString());
                    ls_val = ls_val.Replace("'", "''");
                    if (dtResourceData.Columns[k].DataType == System.Type.GetType("System.Boolean"))
                    {
                        ls_val = (ls_val.ToLower() == "true" ? "1" : ls_val);
                        ls_val = (ls_val.ToLower() == "false" ? "0" : ls_val);
                    }
                    ls_quot = (ls_val == "null" ? "" : ls_quot);

                    if ((iDirection == 0) && (Project.DB_Type.ToLower().Equals("oracle")) || (iDirection == 1) && (Project.TheirDB_Type.ToLower().Equals("oracle")))
                    {
                        if (dtResourceData.Columns[k].DataType == System.Type.GetType("System.DateTime"))
                        {
                            ls_quot = "";
                            ls_val = "to_date('" + dtResourceData.Rows[j][k].ToString() + "','yyyy-mm-dd hh24:mi:ss')";
                        }
                    }
                    ls_values += (k == 0 ? "" : ",") + ls_quot + ls_val + ls_quot;
                }
                ll_insert.Add(dtTask.Rows[dtTaskRowIndex][fieldName_WhoSql].ToString() + "values(" + ls_values + ")");
            }
            return ll_insert;
        }



        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dtTask"></param>
        /// <param name="dtTaskRowIndex"></param>
        /// <param name="iDirection"></param>
        /// <param name="DeleteNotDrop">0存在则删除，再创建 1不存在则创建 2不存在则创建，存在则清空</param>
        /// <returns></returns>
        private static string CreatTable(DataTable dtTask, int dtTaskRowIndex, int iDirection, int DeleteNotDrop)   //
        {
            string TableName = iDirection == 0 ? dtTask.Rows[dtTaskRowIndex]["OurTableName"].ToString() : dtTask.Rows[dtTaskRowIndex]["TheirTableName"].ToString();
            string sResult = "";
            string ls_sql = "select CreateTable from IF_Table_infor where Table_Name = '" + TableName + "'";
            DataTable ld_createtmptable = new DataTable();
            AccessDBop.SQLSelect(ls_sql, ref ld_createtmptable);
            bool bHaveTableSet = ld_createtmptable.Rows.Count > 0 ? true : false;
            //{
            //    if (ld_createtmptable.Rows.Count > 0)
            //    {
            //if (ld_createtmptable.Rows[0]["CreateTable"].ToString() != "")
            //{
            if (((iDirection == 0) && (Project.DB_Type == "sql server")) || ((iDirection == 1) && (Project.TheirDB_Type == "sql server")))
            {
                if (DeleteNotDrop == 0)  //需要删除原表
                {
                    if (bHaveTableSet)
                    {
                        ls_sql = "if exists (select 1 from sysobjects where id = object_id('" + TableName + "') and type = 'U') drop table " + TableName + " " + ld_createtmptable.Rows[0]["CreateTable"].ToString();
                    }
                    else
                    {
                        sResult = "未配置临时表'" + TableName + "'的表结构!";
                        return sResult;
                    }
                }
                else if (DeleteNotDrop == 1)  //增量
                {
                    if (bHaveTableSet)
                    {
                        ls_sql = "if not exists (select 1 from sysobjects where id = object_id('" + TableName + "') and type = 'U') " + ld_createtmptable.Rows[0]["CreateTable"].ToString();
                    }
                    else
                    {
                        sResult = "未配置临时表'" + TableName + "'的表结构!";
                        return sResult;
                    }
                }
                else if (DeleteNotDrop == 2)  //清空
                    //ls_sql = "if not exists (select 1 from sysobjects where id = object_id('" + TableName + "') and type = 'U') " + ld_createtmptable.Rows[0]["CreateTable"].ToString() + " truncate table " + TableName;
                    ls_sql = "if exists (select 1 from sysobjects where id = object_id('" + TableName + "') and type = 'U') truncate table " + TableName;
            }
            else if (((iDirection == 0) && (Project.DB_Type == "oracle")) || ((iDirection == 1) && (Project.TheirDB_Type == "oracle")))
            {
                if (DeleteNotDrop == 0)   //需要删除原表
                {
                    if (bHaveTableSet)
                    {
                        ls_sql = "declare v_count number;begin Select count(*) into v_count from user_tables where table_name = '" + TableName.ToUpper() + "';if v_count > 0 then execute immediate 'drop table " + TableName + "';end if;" + ld_createtmptable.Rows[0]["CreateTable"].ToString() + " end;";
                    }
                    else
                    {
                        sResult = "未配置临时表'" + TableName + "'的表结构!";
                        return sResult;
                    }
                }
                else if (DeleteNotDrop == 1)
                {
                    if (bHaveTableSet)
                    {
                        ls_sql = "declare v_count number;begin Select count(*) into v_count from user_tables where table_name = '" + TableName.ToUpper() + "';if v_count = 0 then  " + ld_createtmptable.Rows[0]["CreateTable"].ToString() + " end if; end;";
                    }
                    else
                    {
                        sResult = "未配置临时表'" + TableName + "'的表结构!";
                        return sResult;
                    }
                }
                else if (DeleteNotDrop == 2)
                    //ls_sql = "declare v_count number;begin Select count(*) into v_count from user_tables where table_name = '" + TableName.ToUpper() + "';if v_count = 0 then  " + ld_createtmptable.Rows[0]["CreateTable"].ToString() + "end if; execute immediate 'truncate table " + TableName + "'; end;";
                    ls_sql = "declare v_count number;begin Select count(*) into v_count from user_tables where table_name = '" + TableName.ToUpper() + "';if v_count > 0 then execute immediate 'truncate table " + TableName + "'; end if;";
            }
            else if (((iDirection == 0) && (Project.DB_Type == "mysql")) || ((iDirection == 1) && (Project.TheirDB_Type == "mysql")))
            {
                if (DeleteNotDrop == 0)  //需要删除原表
                {
                    if (bHaveTableSet)
                    {
                        ls_sql = "DROP TABLE IF EXISTS " + TableName + ";" + ld_createtmptable.Rows[0]["CreateTable"].ToString();
                    }
                    else
                    {
                        sResult = "未配置临时表'" + TableName + "'的表结构!";
                        return sResult;
                    }
                }

                else if (DeleteNotDrop == 1)
                {
                    if (bHaveTableSet)
                    {
                        ls_sql = ld_createtmptable.Rows[0]["CreateTable"].ToString();
                    }
                    else
                    {
                        sResult = "未配置临时表'" + TableName + "'的表结构!";
                        return sResult;
                    }
                }
                else if (DeleteNotDrop == 2)
                    //ls_sql = ld_createtmptable.Rows[0]["CreateTable"].ToString() + " truncate table " + TableName;
                    ls_sql = "truncate table " + TableName;
            }
            bool ls_result = true;
            if (iDirection == 0)
            {
                if (Project.DB_Type == "sql server")
                    ls_result = SQLServerDBop.SQLExecute(Project.DB_Connection, ls_sql);
                else if (Project.DB_Type == "oracle")
                    ls_result = OracleDBop.SQLExecute(Project.DB_Connection, ls_sql);
                else
                    ls_result = MySqlDBop.SQLExecute(Project.DB_Connection, ls_sql);
            }
            else
            {
                if (Project.TheirDB_Type == "sql server")
                    ls_result = SQLServerDBop.SQLExecute(Project.TheirDB_Connection, ls_sql);
                else if (Project.TheirDB_Type == "oracle")
                    ls_result = OracleDBop.SQLExecute(Project.TheirDB_Connection, ls_sql);
                else
                    ls_result = MySqlDBop.SQLExecute(Project.TheirDB_Connection, ls_sql);
            }
            if (!ls_result)
            {
                string sPosition = iDirection == 0 ? Project.DB_Alias : Project.theirDB_Alias;
                sResult = sPosition + " " + ApplicationSetting.Translate("middle table creat fail") + ",[run sql]:" + ls_sql;
            }
            //}
            //}
            //else
            //{
            //    if (DeleteNotDrop == 0)
            //    {
            //        sResult = "未配置临时表'" + TableName + "'的表结构,请到'基础表结构管理'模块进行配置";
            //    }
            //}
            //}
            return sResult;
        }


        //private static string TruncateTable(DataTable dtTask, int dtTaskRowIndex, int iDirection)
        //{
        //    string TableName = iDirection == 0 ? dtTask.Rows[dtTaskRowIndex]["OurTableName"].ToString() : dtTask.Rows[dtTaskRowIndex]["TheirTableName"].ToString();
        //    string sResult = "";
        //    bool ls_result = true;

        //    if (iDirection == 0)
        //    {
        //        if (Project.DB_Type.ToLower().Equals("sql server"))
        //            ls_result = SQLServerDBop.SQLExecute(Project.DB_Connection, "truncate table " + TableName);

        //        else if (Project.DB_Type.ToLower().Equals("oracle"))
        //            ls_result = OracleDBop.SQLExecute(Project.DB_Connection, "delete from " + TableName);
        //        else
        //            ls_result = MySqlDBop.SQLExecute(Project.DB_Connection, "truncate table " + TableName);
        //    }
        //    else
        //    {
        //        if (Project.TheirDB_Type.ToLower().Equals("sql server"))
        //            ls_result = SQLServerDBop.SQLExecute(Project.TheirDB_Connection, "truncate table " + TableName);
        //        else
        //            ls_result = OracleDBop.SQLExecute(Project.TheirDB_Connection, "delete from " + TableName);
        //    }
        //    if (!ls_result)
        //    {
        //        sResult = "删除旧数据失败，【执行语句】:" + "truncate table " + TableName;
        //    }
        //    return sResult;
        //}

        private static string InsertTable(List<string> ll_insert, DataTable dtTask, int dtTaskRowIndex, int iDirection)
        {
            string TableName = iDirection == 0 ? dtTask.Rows[dtTaskRowIndex]["OurTableName"].ToString() : dtTask.Rows[dtTaskRowIndex]["TheirTableName"].ToString();
            string sResult = "";
            bool ls_result = true;

            if (iDirection == 0)
            {
                if (Project.DB_Type.ToLower().Equals("sql server"))
                    ls_result = SQLServerDBop.SQLExecute(Project.DB_Connection, ll_insert.ToArray(), "");
                else if (Project.DB_Type.ToLower().Equals("oracle"))
                    ls_result = OracleDBop.SQLExecute(Project.DB_Connection, ll_insert.ToArray());
                else
                    ls_result = MySqlDBop.SQLExecute(Project.DB_Connection, ll_insert.ToArray());
            }
            else
            {
                if (Project.TheirDB_Type.ToLower().Equals("sql server"))
                    ls_result = SQLServerDBop.SQLExecute(Project.TheirDB_Connection, ll_insert.ToArray(), "");
                else if (Project.TheirDB_Type.ToLower().Equals("oracle"))
                    ls_result = OracleDBop.SQLExecute(Project.TheirDB_Connection, ll_insert.ToArray());
                else
                    ls_result = MySqlDBop.SQLExecute(Project.TheirDB_Connection, ll_insert.ToArray());
            }
            if (!ls_result)
            {
                string s = string.Empty;
                foreach (string stemp in ll_insert)
                {
                    s += "\r\n" + stemp;
                }
                sResult = "向临时表插入新数据失败，【执行语句】:" + s;
            }
            return sResult;
        }

        private static string SynTable(string schedule_name, DataTable dtTask, int dtTaskRowIndex, int iDirection)
        {
            string TableName = iDirection == 0 ? dtTask.Rows[dtTaskRowIndex]["OurTableName"].ToString() : dtTask.Rows[dtTaskRowIndex]["TheirTableName"].ToString();
            string ls_sql = "SELECT IF_Table_Compare.TableName, IF_Table_Compare.SQLString"
                                        + " FROM (IF_Table_Compare INNER JOIN"
                                        + " IF_Table_infor ON IF_Table_Compare.TableName = IF_Table_infor.Table_Name)"
                                        + " WHERE (IF_Table_infor.Table_Type = 'L')"
                                        + " AND (IF_Table_Compare.TableName = '" + TableName + "')";
            string sResult = string.Empty;
            string ls_execsql = string.Empty;
            //bool ls_result = true;
            DataTable ldt_SQLString = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt_SQLString))
            {
                if (ldt_SQLString.Rows.Count > 0)
                {
                    ls_execsql = ldt_SQLString.Rows[0]["SQLString"].ToString();
                    if (ls_execsql.Contains(".exe"))
                    {
                        string[] arrExe = ls_execsql.Split(new string[] { "|" }, StringSplitOptions.None);
                        string ProccessName = arrExe[0];
                        string par = "";
                        if (arrExe.Length == 2)
                        {
                            par = arrExe[1];
                        }
                        sResult = Miscellaneous.RunExe(ProccessName, par);
                        if (sResult != "")
                        {
                            sResult = "可执行程序执行失败,错误原因:" + sResult;
                        }
                    }
                    else
                    {
                        if (iDirection == 0)
                        {
                            if (Project.DB_Type.ToLower().Equals("sql server"))
                                sResult = SQLServerDBop.StrSQLExecute(schedule_name, Project.DB_Connection, ls_execsql);
                            else if (Project.DB_Type.ToLower().Equals("oracle"))
                                sResult = OracleDBop.StrSQLExecute(Project.DB_Connection, ls_execsql);
                            else
                                sResult = MySqlDBop.StrSQLExecute(Project.DB_Connection, ls_execsql);
                        }
                        else
                        {
                            if (Project.TheirDB_Type.ToLower().Equals("sql server"))
                                sResult = SQLServerDBop.StrSQLExecute(schedule_name, Project.TheirDB_Connection, ls_execsql);
                            else if (Project.TheirDB_Type.ToLower().Equals("oracle"))
                                sResult = OracleDBop.StrSQLExecute(Project.TheirDB_Connection, ls_execsql);
                            else
                                sResult = MySqlDBop.StrSQLExecute(Project.TheirDB_Connection, ls_execsql);
                        }
                    }
                }
            }
            if (sResult != "")
            {
                sResult = "临时表更新正式表失败，【执行语句】:" + ls_execsql + "\r\n错误原因:" + sResult;
            }
            return sResult;
        }

        private static string RunExe(string ProccessName, string par)
        {
            return Miscellaneous.RunExe(ProccessName, par);
        }

        static void Write2List(string operate, string info, string MessageType, string taskName)
        {
            string[] arrInfo = new string[4];
            arrInfo[0] = operate;
            arrInfo[1] = info;
            arrInfo[2] = MessageType;
            arrInfo[3] = taskName;
            Miscellaneous.Write2List(arrInfo);
        }

        public static bool ExecTask(string schedule_name, string as_Task_id)
        {

            DataTable dt = new DataTable();
            string sql = "select Task_Name from IF_Task where Task_Id = '" + as_Task_id + "'";
            AccessDBop.SQLSelect(sql, ref dt);
            string Task_Name = string.Empty;
            if (dt.Rows.Count > 0)
            {
                Task_Name = dt.Rows[0][0].ToString();
            }
            else
            {
                Write2List("不存在该任务", "", "2", schedule_name);
                return false;
            }
            Write2List(ApplicationSetting.Translate("task start"), "", "0", schedule_name);
            bool lb_result = true;
            #region 照片任务
            if (as_Task_id == "10000000000000")
            {
                string ls_sql_PHOTO = "SELECT *" +
                    " FROM IF_PHOTO where serial = 1";
                DataTable ldt_PHOTO = new DataTable();

                if (AccessDBop.SQLSelect(ls_sql_PHOTO, ref ldt_PHOTO))
                {
                    if (ldt_PHOTO.Rows.Count > 0)
                    {
                        string Compress = ldt_PHOTO.Rows[0]["Compress"].ToString();
                        int CompressRate = int.Parse(ldt_PHOTO.Rows[0]["CompressRate"].ToString());
                        int width = int.Parse(ldt_PHOTO.Rows[0]["width"].ToString());
                        int height = int.Parse(ldt_PHOTO.Rows[0]["height"].ToString());
                        string our_xh = ldt_PHOTO.Rows[0]["our_xh"].ToString();
                        bool ZPIsPath = ldt_PHOTO.Rows[0]["ZPIsPath"].ToString() == "1" ? true : false;
                        if (ldt_PHOTO.Rows[0]["StoreType"].ToString() == "1")   //数据库到文件
                        {
                            string XH_sql = "";
                            string XHZP_sql = "";
                            int li_photoNumbers = int.Parse(ldt_PHOTO.Rows[0]["PhotoNumbers"].ToString());
                            string BsPath = ldt_PHOTO.Rows[0]["BsPath"].ToString();
                            if (ldt_PHOTO.Rows[0]["GetType"].ToString() == "1")   //照片存在照片表或照片视图
                            {
                                XH_sql = "select " + ldt_PHOTO.Rows[0]["XH"].ToString() + " as XH from " + ldt_PHOTO.Rows[0]["TableName"].ToString();
                                XHZP_sql = "select " + ldt_PHOTO.Rows[0]["XH"].ToString() + " as XH," + ldt_PHOTO.Rows[0]["ZP"].ToString()
                                        + " as ZP from " + ldt_PHOTO.Rows[0]["TableName"].ToString()
                                        + " where " + ldt_PHOTO.Rows[0]["XH"].ToString() + " in";
                            }
                            else      //照片通过SQL关联获得                                          
                            {
                                XH_sql = "select " + ldt_PHOTO.Rows[0]["XHSQL"].ToString() + " as XH from " + ldt_PHOTO.Rows[0]["FROMSQL"].ToString()
                                        + " Where " + ldt_PHOTO.Rows[0]["WHERESQL"].ToString();
                                XHZP_sql = "select " + ldt_PHOTO.Rows[0]["XHSQL"].ToString() + " as XH," + ldt_PHOTO.Rows[0]["ZPSQL"].ToString()
                                    + " as ZP from " + ldt_PHOTO.Rows[0]["FROMSQL"].ToString()
                                    + " Where " + (ldt_PHOTO.Rows[0]["WHERESQL"].ToString().Trim() == "" ? (ldt_PHOTO.Rows[0]["XHSQL"].ToString() + " in") : (" and " + ldt_PHOTO.Rows[0]["XHSQL"].ToString() + " in"));
                            }

                            List<string> lt = new List<string>();
                            DataTable dt_XH = new DataTable();
                            if (Project.TheirDB_Type.ToLower().Equals("oracle"))
                            {
                                bool bResult = OracleDBop.SQLSelect(Project.TheirDB_Connection, XH_sql, ref dt_XH);
                                if (!bResult)
                                {
                                    string[] arrInfo_in = new string[4];
                                    arrInfo_in[0] = ApplicationSetting.Translate("get data error");
                                    arrInfo_in[1] = XH_sql;
                                    arrInfo_in[2] = "2";
                                    arrInfo_in[3] = schedule_name;
                                    Miscellaneous.Write2List(arrInfo_in);
                                }
                            }
                            else if (Project.TheirDB_Type.ToLower().Equals("sql server"))
                            {
                                SQLServerDBop.SQLSelect(Project.TheirDB_Connection, XH_sql, ref dt_XH, "");
                            }


                            if (dt_XH.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt_XH.Rows)
                                {
                                    lt.Add(dr["XH"].ToString());
                                }
                            }
                            else
                            {
                                string[] arrInfo_in = new string[4];
                                arrInfo_in[0] = "同步照片出错或没有照片数据！";
                                arrInfo_in[1] = "";
                                arrInfo_in[2] = "2";
                                arrInfo_in[3] = schedule_name;
                                Miscellaneous.Write2List(arrInfo_in);
                                lb_result = false;
                            }

                            ArrayList al = new ArrayList(lt.ToArray());
                            for (int i = 0; i < al.Count; i = i + li_photoNumbers)
                            {
                                string[] strXH = new string[al.Count - i > li_photoNumbers ? li_photoNumbers : al.Count - i];
                                al.CopyTo(i, strXH, 0, (al.Count - i > li_photoNumbers ? li_photoNumbers : al.Count - i));
                                sql = "('" + string.Join("','", strXH) + "')";
                                CreatePhoto(sql, XHZP_sql, BsPath, Compress, CompressRate, width, height, our_xh, ZPIsPath);
                            }
                        }
                        else if (ldt_PHOTO.Rows[0]["StoreType"].ToString() == "2")  //文件到文件
                        {
                            string TheirPhotoPath = ldt_PHOTO.Rows[0]["TheirPhotoPath"].ToString();
                            string BsPath = ldt_PHOTO.Rows[0]["BsPath"].ToString();
                            string FileToXHType = ldt_PHOTO.Rows[0]["FileToXHType"].ToString();
                            int StartPos = int.Parse(ldt_PHOTO.Rows[0]["StartPos"].ToString());
                            int EndPos = int.Parse(ldt_PHOTO.Rows[0]["EndPos"].ToString());
                            CreatePhotoFromFile(TheirPhotoPath, BsPath, FileToXHType, StartPos, EndPos, Compress, CompressRate, width, height, our_xh);
                        }
                        else if (ldt_PHOTO.Rows[0]["StoreType"].ToString() == "3")  //文件到数据库
                        {
                            string BsPath = ldt_PHOTO.Rows[0]["BsPath"].ToString();
                            string TableName = ldt_PHOTO.Rows[0]["TableName"].ToString();
                            string XH = ldt_PHOTO.Rows[0]["XH"].ToString();
                            string ZP = ldt_PHOTO.Rows[0]["ZP"].ToString();
                        }
                    }
                }
                string[] arrInfo_2 = new string[4];
                arrInfo_2[0] = lb_result ? "【成功】" : "【失败】";
                arrInfo_2[1] = "";
                arrInfo_2[2] = lb_result ? "0" : "2";
                arrInfo_2[3] = schedule_name;
                Miscellaneous.Write2List(arrInfo_2);
                return true;
            }
            #endregion

            #region 普通任务
            lb_result = true;
            string ls_sql;
            int iDirection = 0;
            string sPosition = string.Empty;
            string sResult = string.Empty;
            bool IsRunExe;
            DataTable dtTask;  //任务表
            ls_sql = ("SELECT TheirSql , OurSql , OurTableName,OurTable_desc,direction,IncrementInsert,DeleteNotDrop,DropTable,GroupSql,GroupField,TheirTableName from IF_Task where Task_id ='" +
                                          as_Task_id + "' order by id");
            dtTask = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref dtTask))
            {
                if (dtTask.Rows.Count > 0)
                {
                    //每个表的执行
                    for (int i = 0; i < dtTask.Rows.Count; i++)
                    {
                        string OurTable_desc = dtTask.Rows[i]["OurTable_desc"].ToString();

                        IsRunExe = false;
                        DataTable dtResourceData = new DataTable();
                        iDirection = dtTask.Rows[i]["direction"].ToString() == "0" ? 0 : 1;
                        sPosition = iDirection == 0 ? Project.DB_Alias : Project.theirDB_Alias; //sPosition为接收数据方
                        if (dtTask.Rows[i]["DeleteNotDrop"].ToString() == "Y")    //1构建中间表
                            sResult = CreatTable(dtTask, i, iDirection, 2);
                        else if (dtTask.Rows[i]["IncrementInsert"].ToString() == "Y")
                            sResult = CreatTable(dtTask, i, iDirection, 1);
                        else //if (dtTask.Rows[i]["DropTable"].ToString() == "Y")
                            sResult = CreatTable(dtTask, i, iDirection, 0);
                        if (sResult != "")
                        {
                            lb_result = false;
                            Write2List(sResult, "", "2", schedule_name);
                            return lb_result;
                        }
                        else
                        {
                            //Write2List("创建" + sPosition + "临时表成功!", "", "0", schedule_name);
                            Write2List(sPosition + ApplicationSetting.Translate("middle table create success"), "", "0", schedule_name);
                        }

                        string their_name = dtTask.Rows[i]["TheirTableName"].ToString();
                        string our_name = dtTask.Rows[i]["OurTableName"].ToString();
                        //if (their_name.Contains(".exe"))
                        if (Project.TheirDB_Type == "exe")
                        {
                            string[] arrExe = their_name.Split(new string[] { "|" }, StringSplitOptions.None);
                            string ProccessName = arrExe[0];
                            string par = "";
                            if (arrExe.Length == 2)
                            {
                                par = arrExe[1];
                            }
                            sResult = Miscellaneous.RunExe(ProccessName, par);
                            if (sResult != "")
                            {
                                sResult = "可执行程序执行失败,错误原因:" + sResult;
                            }
                            IsRunExe = true;
                        }
                        //else if (our_name.Contains(".exe"))
                        else if (Project.DB_Type == "exe")
                        {
                            string[] arrExe = our_name.Split(new string[] { "|" }, StringSplitOptions.None);
                            string ProccessName = arrExe[0];
                            string par = "";
                            if (arrExe.Length == 2)
                            {
                                par = arrExe[1];
                            }
                            sResult = Miscellaneous.RunExe(ProccessName, par);
                            if (sResult != "")
                            {
                                sResult = "可执行程序执行失败,错误原因:" + sResult;
                            }
                            IsRunExe = true;
                        }
                        if (IsRunExe)   //可执行程序方式取数据
                        {
                            if (sResult != "")
                            {
                                lb_result = false;
                                Write2List("可执行程序获取数据失败！", sResult, "2", schedule_name);
                                return lb_result;
                            }
                            else
                            {
                                Write2List("可执行程序获取数据成功!", "", "0", schedule_name);
                            }
                        }
                        else     //从数据库或webservice或http取数据
                        {
                            DataTable dtGroupData = new DataTable();
                            sResult = GetGroupData(dtTask, i, ref dtGroupData, iDirection);  //2获取分组数据
                            if (sResult != "")
                            {
                                lb_result = false;
                                Write2List(ApplicationSetting.Translate("get group data fail"), sResult, "2", schedule_name);
                                return lb_result;
                            }
                            else
                            {
                                if (dtGroupData.Rows.Count == 0)
                                {
                                    //Write2List("无分组", "", "0", schedule_name);
                                    Write2List(ApplicationSetting.Translate("no group data"), "", "0", schedule_name);
                                }
                                else
                                {
                                    Write2List(ApplicationSetting.Translate("get group data success"), ApplicationSetting.Translate("sql contain") + ":" + dtTask.Rows[i]["GroupSql"].ToString(), "0", schedule_name);
                                }
                            }

                            for (int m = 0; m < dtGroupData.Rows.Count || dtGroupData.Rows.Count == 0; m++)  //3取源表数据插入到中间表
                            {
                                if   //webservice方式,暂不支持数据推送给webservice
                                 (
                                        ((Project.DB_Type == "webservice") && (iDirection == 1))
                                        ||
                                        ((Project.TheirDB_Type == "webservice") && (iDirection == 0))
                                 )
                                {
                                    string tableName = iDirection == 0 ? their_name : our_name;
                                    string url = iDirection == 0 ? Project.theirSeverName : Project.SeverName;
                                    sResult = SelectData(tableName, url, ref dtResourceData);
                                    if (sResult != "")
                                    {
                                        lb_result = false;
                                        Write2List("从webservice获取原始数据失败", sResult, "2", schedule_name);
                                        return lb_result;
                                    }
                                    sResult = dealDtResourceData(dtTask, i, iDirection, dtResourceData, ref dtResourceData);
                                    if (sResult != "")
                                    {
                                        lb_result = false;
                                        Write2List("处理webservice数据集失败", sResult, "2", schedule_name);
                                        return lb_result;
                                    }
                                }
                                else if
                                    (
                                        ((Project.DB_Type == "http") && (iDirection == 1))
                                        ||
                                        ((Project.TheirDB_Type == "http") && (iDirection == 0))
                                    )  //webservice方式,暂不支持数据推送给http
                                {
                                    string url = iDirection == 0 ? their_name : our_name;
                                    sResult = SelectData_http(url, iDirection, ref dtResourceData);
                                    if (sResult != "")
                                    {
                                        lb_result = false;
                                        Write2List("从http获取原始数据失败", sResult, "2", schedule_name);
                                        return lb_result;
                                    }
                                    sResult = dealDtResourceData(dtTask, i, iDirection, dtResourceData, ref dtResourceData);
                                    if (sResult != "")
                                    {
                                        lb_result = false;
                                        Write2List("处理http数据集失败", sResult, "2", schedule_name);
                                        return lb_result;
                                    }
                                }
                                else
                                {
                                    string sql_SelectData = MakeSql_SelectData(dtTask, i, dtGroupData, m, iDirection); //生成获取数据SQL语句
                                    dtResourceData.Dispose();
                                    sResult = SelectData(sql_SelectData, iDirection, ref dtResourceData);   //从数据库获取数据
                                    if (sResult != "")
                                    {
                                        lb_result = false;
                                        Write2List(ApplicationSetting.Translate("get data fail"), sResult, "2", schedule_name);
                                        return lb_result;
                                    }
                                    else
                                    {
                                        Write2List(ApplicationSetting.Translate("get data success"), "【rowcount】:" + dtResourceData.Rows.Count.ToString() + ",【run sql】:" + sql_SelectData, "0", schedule_name);
                                    }
                                }

                                if ((dtResourceData == null) || (dtResourceData.Rows.Count == 0))
                                {
                                    //lb_result = false;
                                    Write2List("no data or get data fail", "", "1", schedule_name);
                                    //return lb_result;
                                    break;
                                }


                                bool bBulkCopyMode = false;
                                //sqlserver库用buckCopy
                                if (((iDirection == 0) && (Project.DB_Type.ToLower().Equals("sql server")))
                                    || ((iDirection == 1) && (Project.TheirDB_Type.ToLower().Equals("sql server"))))
                                {
                                    bBulkCopyMode = true;
                                    List<string> lsField = GetFieldName_InsertData(dtTask, i, iDirection);
                                    if (lsField.Count > 0)
                                    {
                                        string connection = iDirection == 0 ? Project.DB_Connection : Project.TheirDB_Connection;
                                        string tableName = iDirection == 0 ? our_name : their_name;
                                        //DateTime Dt1 = DateTime.Now;
                                        //启用不写日志  add by zlibo for 2014-06-18
                                        sql = "use master exec sp_dboption [" + Project.DbName + "],'trunc. log on chkpt.',true";
                                        bool bResult = SQLServerDBop.SQLExecute_log(connection, sql);
                                        //Write2List(bResult ? "启用不写日志成功" : "启用不写日志失败", "", bResult ? "0" : "2", schedule_name);

                                        sResult = SQLServerDBop.DataTableToSQLServer(dtResourceData, connection, tableName, lsField);
                                        //DateTime Dt2 = DateTime.Now;
                                        //TimeSpan CountTime = Dt2.Subtract(Dt1);
                                        //MessageBox.Show("用时:" + CountTime.Minutes.ToString() + "分" + CountTime.Seconds.ToString() + "秒" + CountTime.Milliseconds.ToString() + "毫秒");

                                        //关闭不写日志  add by zlibo for honey 2013-06-06
                                        sql = "use master exec sp_dboption [" + Project.DbName + "],'trunc. log on chkpt.',false";
                                        bResult = SQLServerDBop.SQLExecute_log(connection, sql);
                                        //Write2List(bResult ? "关闭不写日志成功" : "关闭不写日志失败", "", bResult ? "0" : "2", schedule_name);

                                        if (sResult == "")
                                        {
                                            Write2List("向" + sPosition + "插入新数据成功，【数据行数】:" + dtResourceData.Rows.Count.ToString(), "", "0", schedule_name);
                                        }
                                        else
                                        {
                                            bBulkCopyMode = false;
                                        }
                                        //else
                                        //{
                                        //    lb_result = false;
                                        //    Write2List("【任务失败】", sResult, "2", schedule_name);
                                        //    return lb_result;
                                        //}
                                    }
                                    else
                                    {
                                        lb_result = false;
                                        Write2List("未获取到待插入表的字段信息", "", "2", schedule_name);
                                        return lb_result;
                                    }
                                }
                                if (!bBulkCopyMode)     //如果不能使用bulkCopy功能或功能出错，就采用生成insert插入语句的方式
                                {
                                    //DateTime Dt1 = DateTime.Now;
                                    List<string> ll_insert = MakeSql_InsertData(dtTask, i, dtResourceData, iDirection);   //生成插入语句
                                    if (ll_insert.Count > 0)  //有插入语句
                                    {
                                        sResult = InsertTable(ll_insert, dtTask, i, iDirection); //向表插入数据
                                        //DateTime Dt2 = DateTime.Now;
                                        //TimeSpan CountTime = Dt2.Subtract(Dt1);
                                        //MessageBox.Show("用时:" + CountTime.Minutes.ToString() + "分" + CountTime.Seconds.ToString() + "秒" + CountTime.Milliseconds.ToString() + "毫秒");

                                        if (sResult == "")
                                        {
                                            Write2List(ApplicationSetting.Translate("to") + " " + sPosition + "insert data success,rowcount=" + ll_insert.Count.ToString(), "", "0", schedule_name);
                                            ll_insert.Clear();
                                        }
                                        else
                                        {
                                            Write2List("[" + ApplicationSetting.Translate("task fail") + "]", sResult, "2", schedule_name);
                                            lb_result = false;
                                            return lb_result;
                                        }
                                    }
                                }
                                if (dtGroupData.Rows.Count == 0)  //没有分组数据，执行一遍后退出
                                    break;
                            }
                        }

                        sResult = SynTable(schedule_name, dtTask, i, iDirection);              //4同步中间表数据到对方表
                        if (sResult != "")
                        {
                            Write2List("[" + ApplicationSetting.Translate("task fail") + "]", sResult, "2", schedule_name);
                            lb_result = false;
                            return lb_result;
                        }
                        else
                        {
                            Write2List(ApplicationSetting.Translate("syn success"), "", "0", schedule_name);
                        }
                    }
                }
            }
            #endregion
            Write2List((lb_result ? "[" + ApplicationSetting.Translate("success") + "]" : "[" + ApplicationSetting.Translate("fail") + "]") + "finish", "", lb_result ? "0" : "2", schedule_name);
            return lb_result;
        }

        private static void CreatePhotoFromFile(string TheirPhotoPath, string Bs_Path, string FileToXHType, int StartPos, int EndPos, string Compress, int CompressRate, int width, int height, string our_xh)
        {
            DirectoryInfo dit = new DirectoryInfo(TheirPhotoPath);
            FileInfo[] fi = dit.GetFiles();
            DataTable dt_user = new DataTable();
            if (Project.DB_Type == "sql server")
            {
                SQLServerDBop.SQLSelect(Project.DB_Connection, "select user_serial," + our_xh + " from dt_user", ref dt_user, "");
            }
            else if (Project.DB_Type == "oracle")
            {
                OracleDBop.SQLSelect(Project.DB_Connection, "select user_serial," + our_xh + " from dt_user", ref dt_user);
            }
            else if (Project.DB_Type == "mysql")
            {
                MySqlDBop.SQLSelect(Project.DB_Connection, "select user_serial," + our_xh + " from dt_user", ref dt_user);
            }
            foreach (FileInfo fif in fi)
            {
                try
                {
                    string ls_xh = Path.GetFileNameWithoutExtension(fif.Name);
                    if (FileToXHType == "1")
                    {
                        ls_xh = ls_xh.Substring(StartPos - 1, EndPos - StartPos + 1);
                    }
                    DataRow[] drs = dt_user.Select(our_xh + " = '" + ls_xh + "'");
                    if (drs != null && drs.Length > 0)
                    {
                        string user_serial = drs[0]["user_serial"].ToString();
                        string zpdoc = Convert.ToString(int.Parse(user_serial) / 1000);
                        if (!Directory.Exists(Bs_Path + "\\wwwroot\\photo\\" + zpdoc))
                        {
                            Directory.CreateDirectory(Bs_Path + "\\wwwroot\\photo\\" + zpdoc);
                        }
                        string filepath = Bs_Path + "\\wwwroot\\Photo\\" + zpdoc + "\\" + user_serial + ".jpg";

                        if (Compress == "1")
                        {
                            Image iSource = Image.FromFile(fif.FullName);
                            bool bResult = GetPicThumbnail(iSource, filepath, height, width, CompressRate);
                            if (!bResult)
                            {
                                string[] arrInfo = new string[4];
                                arrInfo[0] = filepath + " 压缩失败!";
                                arrInfo[1] = "";
                                arrInfo[2] = "2";
                                arrInfo[3] = "同步照片";
                                Miscellaneous.Write2List(arrInfo);
                            }
                        }
                        else
                        {
                            File.Copy(fif.FullName, filepath);
                        }

                        string sql = string.Empty;

                        if (Project.sysType == 0)  //0scm1集约化
                            sql = "update dt_user set user_photo = 1 where user_serial = " + user_serial + ";";   //scm
                        else
                            sql = "update S_ZXXS0101 set user_photo = 1 where user_serial = " + user_serial + ";"
                                   + "update S_ZXJZ0101 set user_photo = 1 where user_serial = " + user_serial + ";";

                        //if (db_position == Project.DB_Alias)
                        SQLServerDBop.SQLExecute(Project.DB_Connection, sql);
                    }
                    else
                    {
                        // MessageBox.Show("dt_user中无此学号");
                    }
                }
                catch { }
            }
        }

        private static void CreatePhoto(string strXH, string XHZP_sql, string Bs_Path, string Compress, int CompressRate, int width, int height, string our_xh, bool ZPIsPath)
        {

            DataTable dt_pic = new DataTable();
            if (Project.TheirDB_Type == "oracle")
            {
                OracleDBop.SQLSelect(Project.TheirDB_Connection, XHZP_sql + strXH, ref dt_pic);
            }
            else if (Project.TheirDB_Type == "sql server")
            {
                SQLServerDBop.SQLSelect(Project.TheirDB_Connection, XHZP_sql + strXH, ref dt_pic, "");
            }
            //MessageBox.Show("select   XH,ZP from zpb where XH in" + strXH);
            if (dt_pic.Rows.Count > 0)
            {
                DataTable dt_user = new DataTable();
                if (Project.DB_Type == "sql server")
                {
                    SQLServerDBop.SQLSelect(Project.DB_Connection, "select user_serial," + our_xh + " from dt_user", ref dt_user, "");
                }
                else if (Project.DB_Type == "oracle")
                {
                    bool bResult = OracleDBop.SQLSelect(Project.DB_Connection, "select user_serial," + our_xh + " from dt_user", ref dt_user);
                    if (!bResult)
                    {
                        string[] arrInfo = new string[4];
                        arrInfo[0] = "执行查询出错";
                        arrInfo[1] = "执行语句：select user_serial," + our_xh + " from dt_user";
                        arrInfo[2] = "2";
                        Miscellaneous.Write2List(arrInfo);
                    }
                }
                else if (Project.DB_Type == "mysql")
                {
                    MySqlDBop.SQLSelect(Project.DB_Connection, "select user_serial," + our_xh + " from dt_user", ref dt_user);
                }
                foreach (DataRow dr in dt_pic.Rows)
                {
                    try
                    {
                        if (!ZPIsPath)
                        {
                            //MessageBox.Show("学号：" + dr["XH"].ToString());
                            if (dr["ZP"] == System.DBNull.Value)
                            {
                                //MessageBox.Show("无照片");
                                continue;
                            }
                        }
                        DataRow[] drs = dt_user.Select(our_xh + " = '" + dr["XH"].ToString() + "'");
                        if (drs != null && drs.Length > 0)
                        {
                            MemoryStream buf = new MemoryStream();
                            byte[] blob = null;
                            if (ZPIsPath)
                            {
                                WebClient client = new WebClient();
                                blob = client.DownloadData(dr["ZP"].ToString());
                            }
                            else
                            {
                                blob = (byte[])dr["ZP"];//
                            }
                            buf.Write(blob, 0, blob.Length);
                            Image iSource = Image.FromStream(buf);



                            string user_serial = drs[0]["user_serial"].ToString();
                            string zpdoc = Convert.ToString(int.Parse(user_serial) / 1000);
                            if (!Directory.Exists(Bs_Path + "\\wwwroot\\photo\\" + zpdoc))
                            {
                                Directory.CreateDirectory(Bs_Path + "\\wwwroot\\photo\\" + zpdoc);
                            }
                            string filepath = Bs_Path + "\\wwwroot\\Photo\\" + zpdoc + "\\" + user_serial + ".jpg";
                            //MessageBox.Show("带路径的照片名" + filepath);
                            //image.Save(filepath);
                            if (Compress == "1")
                            {
                                bool bResult = GetPicThumbnail(iSource, filepath, height, width, CompressRate);
                                if (!bResult)
                                {
                                    string[] arrInfo = new string[4];
                                    arrInfo[0] = filepath + " 压缩失败!";
                                    arrInfo[1] = "";
                                    arrInfo[2] = "2";
                                    Miscellaneous.Write2List(arrInfo);
                                }
                            }
                            else
                            {
                                iSource.Save(filepath);
                            }
                            //int user_serialInt = 0;
                            //int i = 0;
                            //if (!int.TryParse(user_serial, out user_serialInt))
                            //{
                            //    i = user_serialInt / 100;
                            //}

                            string sql = string.Empty;
                            if (Project.sysType == 0)  //0scm1集约化
                                sql = "update dt_user set user_photo = 1 where user_serial = " + user_serial + ";";   //scm
                            else
                                sql = "update S_ZXXS0101 set user_photo = 1 where user_serial = " + user_serial + ";"
                                       + "update S_ZXJZ0101 set user_photo = 1 where user_serial = " + user_serial + ";";
                            //if (db_position == Project.DB_Alias)
                            SQLServerDBop.SQLExecute(Project.DB_Connection, sql);
                            //else
                            //SQLServerDBop.SQLExecute(Project.TheirDB_Connection, sql);
                            if (Project.DbName.ToLower().Contains("scm"))
                            //if (db_name == "scm")
                            {
                                if (Project.DB_Type.ToLower().Equals("oracle"))
                                {
                                    sql = "declare counN number;begin select COUNT(*)into counN from dt_photo  where user_serial = " + user_serial + ";"
                                     + "if (counN=0)then "
                                      + "insert into dt_photo(lx,user_serial,photo_name,photo_type,photo_path)"
                                     + "values(0," + user_serial + ",'" + user_serial + ".jpg',0,'../photo/" + zpdoc + "/');"
                                       + "end if; end;";
                                    OracleDBop.SQLExecute(Project.DB_Connection, sql);
                                }
                                else if (Project.DB_Type.ToLower().Equals("sql server"))
                                {
                                    sql = "if not exists(select * from dt_photo where user_serial = " + user_serial + ")" +
                                    "insert into dt_photo(lx,user_serial,photo_name,photo_type,photo_path,sj)" +
                                  "values(0," + user_serial + ",'" + user_serial + ".jpg',0,'../photo/" + zpdoc + "/','" + DateTime.Now.ToString() + "')";
                                    //if (db_position == Project.DB_Alias)
                                    SQLServerDBop.SQLExecute(Project.DB_Connection, sql);
                                    //else
                                    //SQLServerDBop.SQLExecute(Project.TheirDB_Connection, sql);
                                }
                                else if (Project.DB_Type.ToLower().Equals("mysql"))
                                {
                                    sql = "if not exists(select * from dt_photo where user_serial = " + user_serial + ")" +
                                    "insert into dt_photo(lx,user_serial,photo_name,photo_type,photo_path,sj)" +
                                  "values(0," + user_serial + ",'" + user_serial + ".jpg',0,'../photo/" + zpdoc + "/','" + DateTime.Now.ToString() + "')";
                                    MySqlDBop.SQLExecute(Project.DB_Connection, sql);
                                }
                            }
                        }
                        else
                        {
                            // MessageBox.Show("dt_user中无此学号");
                        }
                    }
                    catch { }
                }
            }
            //if (Project.DB_Type.ToLower().Equals("sql server"))   //生成全量档案日志来全量下发照片（同步照片不支持增量）
            //{
            //    string sql = "insert wt_user_up(type,lx,log_type,log_sj,log_ip,gly_no,dep_serial)values(1,1,1,getdate(),null,'syn','10000')";
            //    SQLServerDBop.SQLExecute(Project.DB_Connection, sql);
            //}
        }

        /// <summary>
        /// 等比例压缩图片
        /// </summary>
        /// <param name="sFile"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static System.IO.MemoryStream GetPicThumbnail(Image iSource, int flag)
        {
            //System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            System.Drawing.Imaging.ImageFormat tFormat = iSource.RawFormat;

            //以下代码为保存图片时，设置压缩质量  
            System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100  
            System.Drawing.Imaging.EncoderParameter eParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                if (jpegICIinfo != null)
                {
                    iSource.Save(ms, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    iSource.Save(ms, tFormat);
                }
                return ms;
            }
            catch
            {
                return null;
            }
            finally
            {
                iSource.Dispose();
                iSource.Dispose();
            }
        }

        /// <summary>
        /// 按比例压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="dHeight">高度</param>
        /// <param name="dWidth"></param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <returns></returns>
        public static bool GetPicThumbnail(Image iSource, string dFile, int dHeight, int dWidth, int flag)
        {
            //System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            System.Drawing.Imaging.ImageFormat tFormat = iSource.RawFormat;
            int sW = 0, sH = 0;
            //按比例缩放
            Size tem_size = new Size(iSource.Width, iSource.Height);
            if (tem_size.Width > dHeight || tem_size.Width > dWidth) //将**改成c#中的或者操作符号
            {
                if ((tem_size.Width * dHeight) > (tem_size.Height * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }
            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            System.Drawing.Imaging.EncoderParameter eParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
                System.Drawing.Imaging.ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }

            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }
    }
}