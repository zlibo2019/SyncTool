using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ScmDataInterFace
{
    /// <summary>
    /// SQL Server���ݿ�Ĳ���
    /// </summary>
    /// <remarks>
    /// Provider=SQLOLEDB;
    /// Data Source=.;
    /// Persist Security Info=True;
    /// User ID=sa;
    /// Initial Catalog=WEDS_CONFIG_DB
    /// </remarks>
    public sealed class SQLServerDBop
    {
        /// <summary>
        /// ���ݿ����Ӵ� 
        /// Provider=SQLOLEDB;
        /// Persist Security Info=True;
        /// </summary>
        static string CommonConnection
        {
            get
            {
                return "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Data Source=" +
                    (ApplicationSetting.Setting.ContainsKey("ServerName") ?
                    ApplicationSetting.Setting["ServerName"] : "Data.mdb") + ";" +
                    "User ID=" +
                    (ApplicationSetting.Setting.ContainsKey("SID") ?
                    ApplicationSetting.Setting["SID"] : "sa") + ";" +
                    "Jet OleDb:DataBase Password=" +
                    (ApplicationSetting.Setting.ContainsKey("PWD") ?
                    ApplicationSetting.Setting["PWD"] : "") + ";" +
                    "Connect Timeout=5;";
            }
        }

        struct SQLConnectionInfor
        {
            public SqlConnection sqlConn;
            public string scheduleName;
        }

        static SQLConnectionInfor[] sci = new SQLConnectionInfor[1000];


        #region ���ݿ����-IsConnected
        /// <summary>
        /// �ж����ݿ��Ƿ�����
        /// </summary>
        /// <returns>�Ƿ�����</returns>
        public static bool IsConnected()
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(CommonConnection);
            try { l_connection.Open(); }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// �ж���Ŀ���ݿ��Ƿ�����
        /// </summary>
        /// <param name="as_constring">�����ַ���</param>
        /// <returns>�Ƿ�����</returns>
        public static bool IsConnected(string as_constring)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(as_constring);
            try { l_connection.Open(); }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-SQLSelect
        /// <summary>
        /// �����ݿ�������ݵ�������DataTable��
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ����</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string sqlstring, ref DataTable Result,string scheduleName)
        {
            SqlDataAdapter l_adapter = new SqlDataAdapter(sqlstring, CommonConnection);
            bool lb_ret = true;
            try
            {
                Result.Clear();
                l_adapter.Fill(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                arrInfo[3] = scheduleName;
                Miscellaneous.Write2List(arrInfo);
            }
            finally { l_adapter.Dispose(); }
            return lb_ret;
        }
        /// <summary>
        /// �����ݿ�������ݵ�������DataTable��
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ����</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string connstring, string sqlstring, ref DataTable Result, string scheduleName)
        {
            SqlDataAdapter l_adapter = new SqlDataAdapter(sqlstring, connstring);
            bool lb_ret = true;
            try
            {
                Result.Clear();
                l_adapter.Fill(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                arrInfo[3] = scheduleName;
                Miscellaneous.Write2List(arrInfo);
            }
            finally { l_adapter.Dispose(); }
            return lb_ret;
        }
        /// <summary>
        /// �����ݿ������һ�С���һ�����ݵ�object��
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ��</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string sqlstring, ref object Result, string scheduleName)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(CommonConnection);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandType = CommandType.Text;
                    l_command.CommandText = sqlstring;
                    Result = l_command.ExecuteScalar();
                    if (Result == null) lb_ret = false;
                }
                catch { lb_ret = false; }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                arrInfo[3] = scheduleName;
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// �����ݿ������һ�С���һ�����ݵ�object��
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ��</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string connstring, string sqlstring, ref object Result,string scheduleName)
        {
            SqlConnection l_connection = new SqlConnection(connstring);
            bool lb_ret = true;
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandType = CommandType.Text;
                    l_command.CommandText = sqlstring;
                    Result = l_command.ExecuteScalar();
                    if (Result == null) lb_ret = false;
                }
                catch { lb_ret = false; }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                arrInfo[3] = scheduleName;
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-SQLExecute
        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string sqlstring)
        {
            bool lb_ret = true;

            SqlConnection l_connection = new SqlConnection(CommonConnection);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();
                    l_transaction.Commit();
                    lb_ret = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed) l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }

        private static void conn_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            string scheduleName = string.Empty;
            for (int i = 0; i < sci.Length; i++)
            {
                if (sci[i].sqlConn == (sender as SqlConnection))
                {
                    scheduleName = sci[i].scheduleName;
                    sci[i].sqlConn = null;
                    break;
                }
            }
            string[] arrInfo = new string[4];
            arrInfo[0] = "��SQLִ�н����";
            arrInfo[1] = e.Message.ToString();
            arrInfo[2] = "1";
            arrInfo[3] = scheduleName;
            Miscellaneous.Write2List(arrInfo);
        }

        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string sqlstring)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(connstring);
            //l_connection.InfoMessage += new SqlInfoMessageEventHandler(conn_InfoMessage);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                //SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    //l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();
                    //l_transaction.Commit();
                    lb_ret = true;

                }
                catch (Exception ex)
                {
                    //l_transaction.Rollback();
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }

        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute_log(string connstring, string sqlstring)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(connstring);
            //l_connection.InfoMessage += new SqlInfoMessageEventHandler(conn_InfoMessage);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                //SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    //l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();
                    //l_transaction.Commit();
                    lb_ret = true;

                }
                catch (Exception ex)
                {
                    //l_transaction.Rollback();
                    lb_ret = false;
                    //string[] arrInfo = new string[4];
                    //arrInfo[0] = ApplicationSetting.Translate("database exception"); ;
                    //arrInfo[1] = ex.Message;
                    //arrInfo[2] = "2";
                    //Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }

        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static string StrSQLExecute(string scheduleName, string connstring, string sqlstring)
        {
            string sResult = string.Empty;
            SqlConnection l_connection = new SqlConnection(connstring);
            for (int i = 0; i < sci.Length; i++)
            {
                if (sci[i].sqlConn == null)
                {
                    sci[i].sqlConn = l_connection;
                    sci[i].scheduleName = scheduleName;
                    break;
                }
            }
            l_connection.InfoMessage += new SqlInfoMessageEventHandler(conn_InfoMessage);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                //SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    //l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();
                    //l_transaction.Commit();
                    //lb_ret = true;
                }
                catch (Exception ex)
                {
                    //l_transaction.Rollback();
                    sResult = ex.Message;
                }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                sResult += ex.Message;
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return sResult;
        }

        /// <summary>
        /// ִ��һ��������䣬���ص�һ�е�һ�е�ֵ
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���ؽ��</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string sqlstring, ref object Result,string scheduleName)
        {

            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(CommonConnection);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    Result = l_command.ExecuteScalar();
                    if (Result == null) lb_ret = false;
                    l_transaction.Commit();
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    arrInfo[3] = scheduleName;
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                arrInfo[3] = scheduleName;
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// ִ��һ��������䣬���ص�һ�е�һ�е�ֵ
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���ؽ��</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string sqlstring, ref object Result,string scheduleName)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(connstring);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    Result = l_command.ExecuteScalar();
                    if (Result == null) lb_ret = false;
                    l_transaction.Commit();
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    arrInfo[3] = scheduleName;
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// ִ��һ��SQL��䣬���ɹ��ع�
        /// </summary>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string[] sqlstring, string scheduleName)
        {
            bool lb_return = false;
            SqlConnection l_connection = new SqlConnection(CommonConnection);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                SqlTransaction l_transaction = l_connection.BeginTransaction();
                l_command.Transaction = l_transaction;
                try
                {
                    string ls_sql = string.Join(";", sqlstring);
                    l_command.CommandText = ls_sql;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();

                    //for (int i = 0; i < sqlstring.Length; i++)
                    //{
                    //    if (sqlstring[i].Trim() != string.Empty)
                    //    {
                    //        l_command.CommandText = sqlstring[i];
                    //        l_command.ExecuteNonQuery();
                    //    }
                    //}
                    l_transaction.Commit();
                    lb_return = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_return = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    arrInfo[3] = scheduleName;
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_return = true; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_return;
        }
        /// <summary>
        /// ִ��һ��SQL��䣬���ɹ��ع�
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string[] sqlstring, string scheduleName)
        {
            bool lb_return = false;
            SqlConnection l_connection = new SqlConnection(connstring);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {

                    ArrayList al = new ArrayList(sqlstring);
                    l_command.Transaction = l_transaction;
                    for (int i = 0; i < al.Count; i = i + 5000)
                    {
                        string[] strsql = new string[al.Count - i > 5000 ? 5000 : al.Count - i];
                        al.CopyTo(i, strsql, 0, (al.Count - i > 5000 ? 5000 : al.Count - i));
                        string sql = string.Join("; ", strsql);
                        sql = "begin " + sql + "; end; ";
                        l_command.CommandText = sql;
                        l_command.CommandTimeout = 36000;
                        l_command.ExecuteNonQuery();
                    }



                    //l_command.Transaction = l_transaction;
                    //string ls_sql = string.Join("; ", sqlstring);
                    //l_command.CommandText = ls_sql;
                    //l_command.CommandTimeout = 36000;
                    //l_command.ExecuteNonQuery();

                    //for (int i = 0; i < sqlstring.Length; i++)
                    //{
                    //    if (sqlstring[i].Trim() != string.Empty)
                    //    {
                    //        l_command.CommandText = sqlstring[i];
                    //        l_command.ExecuteNonQuery();
                    //    }
                    //}
                    l_transaction.Commit();
                    lb_return = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_return = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    arrInfo[3] = scheduleName;
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_return = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_return;
        }
        #endregion
        #region ���ݿ����-SQLExecuteNoTrans
        /// <summary>
        /// ִ��һ��SQL��䣬������
        /// </summary>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecuteNoTrans(string[] sqlstring)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(CommonConnection);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                try
                {
                    string ls_sql = string.Join(";", sqlstring);
                    l_command.CommandText = ls_sql;
                    l_command.ExecuteNonQuery();
                    //for (int i = 0; i < sqlstring.Length; i++)
                    //{
                    //    l_command.CommandText = sqlstring[i];
                    //    l_command.ExecuteNonQuery();
                    //}
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }

            return lb_ret;
        }
        /// <summary>
        /// ִ��һ��SQL��䣬������
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecuteNoTrans(string connstring, string[] sqlstring)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(connstring);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                try
                {
                    string ls_sql = string.Join(";", sqlstring);
                    l_command.CommandText = ls_sql;
                    l_command.ExecuteNonQuery();
                    //for (int i = 0; i < sqlstring.Length; i++)
                    //{
                    //    l_command.CommandText = sqlstring[i];
                    //    l_command.ExecuteNonQuery();
                    //}
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }

        #endregion
        #region ���ݿ����-SQLUpdate
        /// <summary>
        /// ִ��ĳ������ݸ���
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">Ҫ���µ����ݼ�</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLUpdate(string sqlstring, ref DataTable Result)
        {
            bool lb_ret = true;
            SqlDataAdapter l_adapter = new SqlDataAdapter(sqlstring, CommonConnection);
            SqlCommandBuilder l_commandbuilder = new SqlCommandBuilder(l_adapter);
            DataTable ldt = new DataTable();
            try
            {
                l_adapter.Fill(ldt);
                l_adapter.Update(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                l_adapter.Dispose();
                l_commandbuilder.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// ִ��ĳ������ݸ���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">Ҫ���µ����ݼ�</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLUpdate(string connstring, string sqlstring, ref DataTable Result)
        {
            bool lb_ret = true;
            SqlDataAdapter l_adapter = new SqlDataAdapter(sqlstring, connstring);
            SqlCommandBuilder l_commandbuilder = new SqlCommandBuilder(l_adapter);
            DataTable ldt = new DataTable();
            try
            {
                l_adapter.Fill(ldt);
                l_adapter.Update(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                l_adapter.Dispose();
                l_commandbuilder.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-IsExistsDB
        /// <summary>
        /// �ж����ݿ��Ƿ����
        /// </summary>
        /// <param name="connstring">�����ַ���</param>
        /// <param name="dbname">���ݿ���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool IsExistsDB(string connstring, string dbname)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(connstring);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandText = "SELECT count(*) FROM master.dbo.sysdatabases WHERE name = N'" +
                        dbname + "'";
                    int li_count = Convert.ToInt32(l_command.ExecuteScalar());
                    if (li_count < 1) lb_ret = false;
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// �ж����ݿ��Ƿ����
        /// </summary>
        /// <param name="dbname">���ݿ���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool IsExistsDB(string dbname)
        {
            string ls_conn = "Data Source=" +
            (ApplicationSetting.Setting.ContainsKey("ServerName") ?
            ApplicationSetting.Setting["ServerName"] : ".") + ";" +
            "User ID=" +
            (ApplicationSetting.Setting.ContainsKey("SID") ?
            ApplicationSetting.Setting["SID"] : "sa") + ";" +
            "Password=" +
            (ApplicationSetting.Setting.ContainsKey("PWD") ?
            ApplicationSetting.Setting["PWD"] : "") + ";" +
            "Initial Catalog=;";
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(ls_conn);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandText = "SELECT count(*) FROM master.dbo.sysdatabases WHERE name = N'" +
                        dbname + "'";
                    int li_count = Convert.ToInt32(l_command.ExecuteScalar());
                    if (li_count < 1) lb_ret = false;
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-IsExistsTable
        /// <summary>
        /// �ж����ݱ��Ƿ����
        /// </summary>
        /// <param name="connstring">�����ַ���</param>
        /// <param name="tablename">���ݱ���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool IsExistsTable(string connstring, string tablename)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(connstring);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandText = "select count(*) from dbo.sysobjects where id = object_id(N'"
                    + tablename + "') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
                    int li_count = Convert.ToInt32(l_command.ExecuteScalar());
                    if (li_count < 1) lb_ret = false;
                }
                catch { lb_ret = false; }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }

            return lb_ret;
        }
        /// <summary>
        /// �ж����ݱ��Ƿ����
        /// </summary>
        /// <param name="tablename">���ݱ���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool IsExistsTable(string tablename)
        {
            bool lb_ret = true;
            SqlConnection l_connection = new SqlConnection(CommonConnection);
            try
            {
                l_connection.Open();
                SqlCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandText = "select count(*) from dbo.sysobjects where id = object_id(N'"
                    + tablename + "') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
                    int li_count = Convert.ToInt32(l_command.ExecuteScalar());
                    if (li_count < 1) lb_ret = false;
                }
                catch { lb_ret = false; }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-���ٲ�������SqlBulkCopy
        public static string DataTableToSQLServer(DataTable dt, string connstring, string tableName, List<string> lsField)
        {
            string sResult = string.Empty;
            tableName = "dbo." + tableName;
            using (SqlConnection destinationConnection = new SqlConnection(connstring))
            {
                destinationConnection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    try
                    {
                        bulkCopy.DestinationTableName = tableName;//Ҫ����ı�ı���
                        for (int i = 0; i < lsField.Count; i++)
                        {
                            string fieldName = lsField[i];
                            bulkCopy.ColumnMappings.Add(fieldName, fieldName);//ӳ���ֶ��� DataTable���� ,���ݿ� ��Ӧ������
                        }
                        bulkCopy.WriteToServer(dt);
                    }
                    catch (Exception ex)
                    {
                        sResult = ex.Message;
                    }
                    finally
                    {
                        bulkCopy.Close();
                    }
                }
            }
            return sResult;
        }
        #endregion ���ݿ����-���ٲ�������SqlBulkCopy
    }

    /// <summary>
    /// Oracle���ݿ�Ĳ���
    /// </summary>
    public sealed class OracleDBop
    {
        /// <summary>
        /// ���ݿ����Ӵ� 
        /// Data Source=weds;User ID=scm;Password=123456
        /// </summary>
        static string CommonConnection
        {
            get
            {
                return "Data Source=" +
                    (ApplicationSetting.Setting.ContainsKey("ServerName") ?
                    ApplicationSetting.Setting["ServerName"] : ".") + ";" +
                    "User ID=" +
                    (ApplicationSetting.Setting.ContainsKey("SID") ?
                    ApplicationSetting.Setting["SID"] : "sa") + ";" +
                    "Password=" +
                    (ApplicationSetting.Setting.ContainsKey("PWD") ?
                    ApplicationSetting.Setting["PWD"] : "") + ";";
            }
        }
        #region ���ݿ����-IsConnected
        /// <summary>
        /// �ж����ݿ��Ƿ�����
        /// </summary>
        /// <returns>�Ƿ�����</returns>
        public static bool IsConnected()
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(CommonConnection);
            try { l_connection.Open(); }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// �ж���Ŀ���ݿ��Ƿ�����
        /// </summary>
        /// <param name="as_constring">�����ַ���</param>
        /// <returns>�Ƿ�����</returns>
        public static bool IsConnected(string as_constring)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(as_constring);
            try { l_connection.Open(); }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-SQLSelect
        /// <summary>
        /// �����ݿ�������ݵ�������DataTable��
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ����</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string sqlstring, ref DataTable Result)
        {
            bool lb_ret = true;
            OracleDataAdapter l_adapter = new OracleDataAdapter(sqlstring, CommonConnection);
            try
            {
                Result.Clear();
                l_adapter.Fill(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally { l_adapter.Dispose(); }
            return lb_ret;
        }
        /// <summary>
        /// �����ݿ�������ݵ�������DataTable��
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ����</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string connstring, string sqlstring, ref DataTable Result)
        {
            bool lb_ret = true;
            OracleCommand oc = new OracleCommand(sqlstring);
            oc.CommandTimeout = 360000;
            OracleDataAdapter l_adapter = new OracleDataAdapter(sqlstring, connstring);
            try
            {
                Result.Clear();
                l_adapter.Fill(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally { l_adapter.Dispose(); }
            return lb_ret;
        }
        /// <summary>
        /// �����ݿ������һ�С���һ�����ݵ�object��
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ��</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string sqlstring, ref object Result)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(CommonConnection);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandType = CommandType.Text;
                    l_command.CommandText = sqlstring;
                    Result = l_command.ExecuteScalar();
                    if (Result == null) lb_ret = false;
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// �����ݿ������һ�С���һ�����ݵ�object��
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ��</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string connstring, string sqlstring, ref object Result)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(connstring);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandType = CommandType.Text;
                    l_command.CommandText = sqlstring;
                    Result = l_command.ExecuteScalar();
                    if (Result == null) lb_ret = false;
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-SQLExecute
        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string sqlstring)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(CommonConnection);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                OracleTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();
                    l_transaction.Commit();
                    lb_ret = true;
                }
                catch { l_transaction.Rollback(); lb_ret = false; }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string sqlstring)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(connstring);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                OracleTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();
                    l_transaction.Commit();
                    lb_ret = true;
                }
                catch { l_transaction.Rollback(); lb_ret = false; }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }


        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static string StrSQLExecute(string connstring, string sqlstring)
        {
            string sResult = string.Empty;
            OracleConnection l_connection = new OracleConnection(connstring);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                OracleTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 30;
                    l_command.ExecuteNonQuery();
                    l_transaction.Commit();
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    sResult = ex.Message;
                }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                sResult += ex.Message;
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return sResult;
        }

        /// <summary>
        /// ִ��һ��������䣬���ص�һ�е�һ�е�ֵ
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���ؽ��</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string sqlstring, ref object Result)
        {

            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(CommonConnection);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                OracleTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    Result = l_command.ExecuteScalar();
                    if (Result == null) lb_ret = false;
                    l_transaction.Commit();
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// ִ��һ��������䣬���ص�һ�е�һ�е�ֵ
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���ؽ��</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string sqlstring, ref object Result)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(connstring);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                OracleTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    Result = l_command.ExecuteScalar();
                    if (Result == null) lb_ret = false;
                    l_transaction.Commit();
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// ִ��һ��SQL��䣬���ɹ��ع�
        /// </summary>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string[] sqlstring)
        {
            bool lb_return = false;
            OracleConnection l_connection = new OracleConnection(CommonConnection);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                OracleTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    for (int i = 0; i < sqlstring.Length; i++)
                    {
                        if (sqlstring[i].Trim() != string.Empty)
                        {
                            l_command.CommandText = sqlstring[i];
                            l_command.CommandTimeout = 36000;
                            l_command.ExecuteNonQuery();
                        }
                    }
                    l_transaction.Commit();
                    lb_return = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_return = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_return = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_return;
        }
        /// <summary>
        /// ִ��һ��SQL��䣬���ɹ��ع�
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string[] sqlstring)
        {
            bool lb_return = false;
            OracleConnection l_connection = new OracleConnection(connstring);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                OracleTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    ArrayList al = new ArrayList(sqlstring);
                    l_command.Transaction = l_transaction;
                    for (int i = 0; i < al.Count; i = i + 500)
                    {
                        string[] strsql = new string[al.Count - i > 500 ? 500 : al.Count - i];
                        al.CopyTo(i, strsql, 0, (al.Count - i > 500 ? 500 : al.Count - i));
                        string sql = string.Join("; ", strsql);
                        sql = "begin " + sql + "; end; ";
                        l_command.CommandText = sql;
                        l_command.CommandTimeout = 36000;
                        l_command.ExecuteNonQuery();
                    }


                    //for (int i = 0; i < sqlstring.Length; i++)
                    //{
                    //    if (sqlstring[i].Trim() != string.Empty)
                    //    {
                    //        l_command.CommandText = sqlstring[i];
                    //        l_command.ExecuteNonQuery();
                    //    }
                    //}

                    l_transaction.Commit();
                    lb_return = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_return = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_return = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_return;
        }
        #endregion
        #region ���ݿ����-SQLExecuteNoTrans
        /// <summary>
        /// ִ��һ��SQL��䣬������
        /// </summary>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecuteNoTrans(string[] sqlstring)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(CommonConnection);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                try
                {
                    for (int i = 0; i < sqlstring.Length; i++)
                    {
                        l_command.CommandText = sqlstring[i];
                        l_command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// ִ��һ��SQL��䣬������
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecuteNoTrans(string connstring, string[] sqlstring)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(connstring);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                try
                {
                    for (int i = 0; i < sqlstring.Length; i++)
                    {
                        l_command.CommandText = sqlstring[i];
                        l_command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-SQLUpdate
        /// <summary>
        /// ִ��ĳ������ݸ���
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">Ҫ���µ����ݼ�</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLUpdate(string sqlstring, ref DataTable Result)
        {
            bool lb_ret = true;
            OracleDataAdapter l_adapter = new OracleDataAdapter(sqlstring, CommonConnection);
            OracleCommandBuilder l_commandbuilder = new OracleCommandBuilder(l_adapter);
            DataTable ldt = new DataTable();
            try
            {
                l_adapter.Fill(ldt);
                l_adapter.Update(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally { l_adapter.Dispose(); l_commandbuilder.Dispose(); }
            return lb_ret;
        }
        /// <summary>
        /// ִ��ĳ������ݸ���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">Ҫ���µ����ݼ�</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLUpdate(string connstring, string sqlstring, ref DataTable Result)
        {
            bool lb_ret = true;
            OracleDataAdapter l_adapter = new OracleDataAdapter(sqlstring, connstring);
            OracleCommandBuilder l_commandbuilder = new OracleCommandBuilder(l_adapter);
            DataTable ldt = new DataTable();
            try
            {
                l_adapter.Fill(ldt);
                l_adapter.Update(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                l_adapter.Dispose();
                l_commandbuilder.Dispose();
            }
            return lb_ret;
        }
        #endregion
        #region ���ݿ����-IsExistsTable
        /// <summary>
        /// �ж����ݱ��Ƿ����
        /// </summary>
        /// <param name="connstring">�����ַ���</param>
        /// <param name="tablename">���ݱ���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool IsExistsTable(string connstring, string tablename)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(connstring);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandText = "select count(*) from user_tables where table_name='" + tablename + "'";
                    int li_count = Convert.ToInt32(l_command.ExecuteScalar());
                    if (li_count < 1) lb_ret = false;
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// �ж����ݱ��Ƿ����
        /// </summary>
        /// <param name="tablename">���ݱ���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool IsExistsTable(string tablename)
        {
            bool lb_ret = true;
            OracleConnection l_connection = new OracleConnection(CommonConnection);
            try
            {
                l_connection.Open();
                OracleCommand l_command = l_connection.CreateCommand();
                try
                {
                    l_command.CommandText = "select count(*) from user_tables where table_name='" + tablename + "'";
                    int li_count = Convert.ToInt32(l_command.ExecuteScalar());
                    if (li_count < 1) lb_ret = false;
                }
                catch (Exception ex)
                {
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = "";
                    arrInfo[1] = "";
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion
    }
    /// <summary>
    /// Access���ݿ����
    /// </summary>
    public sealed class AccessDBop
    {
        /// <summary>
        /// ���ݿ����Ӵ� 
        /// Provider=SQLOLEDB;
        /// Persist Security Info=True;
        /// </summary>
        static string CommonConnection
        {
            get
            {
                return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\ScmDataInterFace.mdb;";
                // @"Driver= {MicrosoftAccessDriver(*.mdb)};DBQ=" + System.IO.Directory.GetCurrentDirectory() + "\\ScmDataInterFace.mdb;";
                //"PWD=;";
            }
        }

        /// <summary>
        /// ���ݿ����Ӵ� 
        /// Provider=SQLOLEDB;
        /// Persist Security Info=True;
        /// </summary>
        static string CommonConnection_configBase
        {
            get
            {
                return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\configBase.mdb;";
                // @"Driver= {MicrosoftAccessDriver(*.mdb)};DBQ=" + System.IO.Directory.GetCurrentDirectory() + "\\ScmDataInterFace.mdb;";
                //"PWD=;";
            }
        }
        /// <summary>
        /// Select ����
        /// </summary>
        /// <param name="as_Filename">Access���ݿ��ļ���</param>
        /// <param name="as_passwd">Access���ݿ����</param>
        /// <param name="as_sql">SQL���</param>
        /// <returns>DataTable���͵����ݽ����</returns>
        public static bool SQLSelect(string as_sql, ref DataTable Result)
        {
            bool b_result = true;
            try
            {
                OleDbConnection oc = new OleDbConnection();
                oc.ConnectionString = CommonConnection;
                OleDbDataAdapter oda = new OleDbDataAdapter(as_sql, oc);
                Result.Clear();
                oda.Fill(Result);
                oda.Dispose();
                oc.Close();
                oc.Dispose();
            }
            catch (Exception ex)
            {
                b_result = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            return b_result;

        }


        /// <summary>
        /// Select ����
        /// </summary>
        /// <param name="as_Filename">Access���ݿ��ļ���</param>
        /// <param name="as_passwd">Access���ݿ����</param>
        /// <param name="as_sql">SQL���</param>
        /// <returns>DataTable���͵����ݽ����</returns>
        public static bool SQLSelect(string CommonConnection,string as_sql, ref DataTable Result)
        {
            bool b_result = true;
            try
            {
                OleDbConnection oc = new OleDbConnection();
                oc.ConnectionString = CommonConnection;
                OleDbDataAdapter oda = new OleDbDataAdapter(as_sql, oc);
                Result.Clear();
                oda.Fill(Result);
                oda.Dispose();
                oc.Close();
                oc.Dispose();
            }
            catch (Exception ex)
            {
                b_result = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            return b_result;

        }

        /// <summary>
        /// Select ����
        /// </summary>
        /// <param name="as_Filename">Access���ݿ��ļ���</param>
        /// <param name="as_passwd">Access���ݿ����</param>
        /// <param name="as_sql">SQL���</param>
        /// <returns>DataTable���͵����ݽ����</returns>
        public static bool SQLSelect_configBase(string as_sql, ref DataTable Result)
        {
            bool b_result = true;
            try
            {
                OleDbConnection oc = new OleDbConnection();
                oc.ConnectionString = CommonConnection_configBase;
                OleDbDataAdapter oda = new OleDbDataAdapter(as_sql, oc);
                Result.Clear();
                oda.Fill(Result);
                oda.Dispose();
                oc.Close();
                oc.Dispose();
            }
            catch (Exception ex)
            {
                b_result = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            return b_result;

        }

        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string sqlstring)
        {
            bool lb_ret = true;
            OleDbConnection l_connection = new OleDbConnection(CommonConnection);
            try
            {
                l_connection.Open();

                OleDbCommand l_command = l_connection.CreateCommand();
                OleDbTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.ExecuteNonQuery();
                    l_transaction.Commit();
                    lb_ret = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed) l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }

        /// <summary>
        /// ִ��һ��SQL��䣬���ɹ��ع�
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string[] sqlstring)
        {
            bool lb_return = false;
            OleDbConnection l_connection = new OleDbConnection(connstring);
            try
            {
                l_connection.Open();
                OleDbCommand l_command = l_connection.CreateCommand();
                OleDbTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    for (int i = 0; i < sqlstring.Length; i++)
                    {
                        if (sqlstring[i].Trim() != string.Empty)
                        {
                            l_command.CommandText = sqlstring[i];
                            l_command.ExecuteNonQuery();
                        }
                    }
                    l_transaction.Commit();
                    lb_return = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_return = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_return = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_return;
        }

        /// <summary>
        /// ִ��һ��SQL��䣬���ɹ��ع�
        /// </summary>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string[] sqlstring)
        {
            bool lb_return = false;
            OleDbConnection l_connection = new OleDbConnection(CommonConnection);
            try
            {
                l_connection.Open();
                OleDbCommand l_command = l_connection.CreateCommand();
                OleDbTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    for (int i = 0; i < sqlstring.Length; i++)
                    {
                        if (sqlstring[i].Trim() != string.Empty)
                        {
                            l_command.CommandText = sqlstring[i];
                            l_command.ExecuteNonQuery();
                        }
                    }
                    l_transaction.Commit();
                    lb_return = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_return = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_return = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_return;
        }

        /// <summary>
        /// ִ��һ��SQL��䣬���ɹ��ع�
        /// </summary>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute_ConfigBase(string[] sqlstring)
        {
            bool lb_return = false;
            OleDbConnection l_connection = new OleDbConnection(CommonConnection_configBase);
            try
            {
                l_connection.Open();
                OleDbCommand l_command = l_connection.CreateCommand();
                OleDbTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    l_command.Transaction = l_transaction;
                    for (int i = 0; i < sqlstring.Length; i++)
                    {
                        if (sqlstring[i].Trim() != string.Empty)
                        {
                            l_command.CommandText = sqlstring[i];
                            l_command.ExecuteNonQuery();
                        }
                    }
                    l_transaction.Commit();
                    lb_return = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_return = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_return = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_return;
        }

        /// <summary>
        /// �ж����ݿ��Ƿ�����
        /// </summary>
        /// <returns>����/δ����--true/false</returns>
        public static bool isconnected(string as_Filename, string as_passwd)
        {
            bool lb_ret = false;
            OleDbConnection oc = new OleDbConnection();
            oc.ConnectionString = @"DBQ=" + as_Filename + ";" +
                "Driver={Driver do Microsoft Access (*.mdb)};" +
                "PWD=" + as_passwd + ";";
            try
            {
                oc.Open();
                lb_ret = true;
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (oc.State != System.Data.ConnectionState.Closed)
                    oc.Close();
                oc.Dispose();
            }
            return lb_ret;
        }

        /// <summary>
        /// �ж����ݿ��Ƿ�����
        /// </summary>
        /// <returns>����/δ����--true/false</returns>
        public static bool IsConnected(string ConnectionStr)
        {
            bool lb_ret = false;
            OleDbConnection oc = new OleDbConnection();
            oc.ConnectionString = ConnectionStr;
            try
            {
                oc.Open();
                lb_ret = true;
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                if (oc.State != System.Data.ConnectionState.Closed)
                    oc.Close();
                oc.Dispose();
            }
            return lb_ret;
        }
        /// <summary>
        /// ִ��ĳ������ݸ���
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">Ҫ���µ����ݼ�</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLUpdate(string sqlstring, ref DataTable Result)
        {
            bool lb_ret = true;
            OleDbDataAdapter l_adapter = new OleDbDataAdapter(sqlstring, CommonConnection);
            OleDbCommandBuilder l_commandbuilder = new OleDbCommandBuilder(l_adapter);
            DataTable ldt = new DataTable();
            try
            {
                l_adapter.Fill(ldt);
                l_adapter.Update(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");;
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                l_adapter.Dispose();
                l_commandbuilder.Dispose();
            }
            return lb_ret;
        }

        /// <summary>
        /// ִ��ĳ������ݸ���
        /// </summary>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">Ҫ���µ����ݼ�</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLUpdate_configBase(string sqlstring, ref DataTable Result)
        {
            bool lb_ret = true;
            OleDbDataAdapter l_adapter = new OleDbDataAdapter(sqlstring, CommonConnection_configBase);
            OleDbCommandBuilder l_commandbuilder = new OleDbCommandBuilder(l_adapter);
            DataTable ldt = new DataTable();
            try
            {
                l_adapter.Fill(ldt);
                l_adapter.Update(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = "";
                arrInfo[1] = "";
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally
            {
                l_adapter.Dispose();
                l_commandbuilder.Dispose();
            }
            return lb_ret;
        }
    }
    public sealed class MySqlDBop
    {
        #region ���ݿ����-IsConnected
        /// <summary>
        /// �ж���Ŀ���ݿ��Ƿ�����
        /// </summary>
        /// <param name="as_constring">�����ַ���</param>
        /// <returns>�Ƿ�����</returns>
        public static bool IsConnected(string as_constring)
        {
            bool lb_ret = true;
            MySqlConnection l_connection = new MySqlConnection(as_constring);
            try { l_connection.Open(); }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }
        #endregion

        private static void conn_InfoMessage(object sender, MySqlInfoMessageEventArgs e)
        {
            string[] arrInfo = new string[4];
            arrInfo[0] = "��SQLִ�н����" + e.ToString();
            arrInfo[1] = "��SQLִ�н����" + e.ToString();
            arrInfo[2] = "1";
            Miscellaneous.Write2List(arrInfo);
        }

        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static string StrSQLExecute(string connstring, string sqlstring)
        {
            string sResult = string.Empty;
            MySqlConnection l_connection = new MySqlConnection(connstring);
            l_connection.InfoMessage += new MySqlInfoMessageEventHandler(conn_InfoMessage);
            try
            {
                l_connection.Open();
                MySqlCommand l_command = l_connection.CreateCommand();
                //SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    //l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();
                    //l_transaction.Commit();
                    //lb_ret = true;
                }
                catch (Exception ex)
                {
                    //l_transaction.Rollback();
                    sResult = ex.Message;
                }
                finally { l_command.Dispose(); }
            }
            catch (Exception ex)
            {
                sResult += ex.Message;
            }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return sResult;
        }

        /// <summary>
        /// �����ݿ�������ݵ�������DataTable��
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <param name="Result">���صĽ����</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLSelect(string connstring, string sqlstring, ref DataTable Result)
        {
            MySqlDataAdapter l_adapter = new MySqlDataAdapter(sqlstring, connstring);
            bool lb_ret = true;
            try
            {
                Result.Clear();
                l_adapter.Fill(Result);
            }
            catch (Exception ex)
            {
                lb_ret = false;
                string[] arrInfo = new string[4];
                arrInfo[0] = ApplicationSetting.Translate("database exception");
                arrInfo[1] = ex.Message;
                arrInfo[2] = "2";
                Miscellaneous.Write2List(arrInfo);
            }
            finally { l_adapter.Dispose(); }
            return lb_ret;
        }

        /// <summary>
        /// ִ��һ��SQL���
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL���</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string sqlstring)
        {
            bool lb_ret = true;
            MySqlConnection l_connection = new MySqlConnection(connstring);
            //l_connection.InfoMessage += new SqlInfoMessageEventHandler(conn_InfoMessage);
            try
            {
                l_connection.Open();
                MySqlCommand l_command = l_connection.CreateCommand();
                //SqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {
                    //l_command.Transaction = l_transaction;
                    l_command.CommandText = sqlstring;
                    l_command.CommandTimeout = 36000;
                    l_command.ExecuteNonQuery();
                    //l_transaction.Commit();
                    lb_ret = true;

                }
                catch (Exception ex)
                {
                    //l_transaction.Rollback();
                    lb_ret = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_ret = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_ret;
        }

        /// <summary>
        /// ִ��һ��SQL��䣬���ɹ��ع�
        /// </summary>
        /// <param name="connstring">���Ӵ�</param>
        /// <param name="sqlstring">SQL��伯</param>
        /// <returns>�ɹ�/ʧ��</returns>
        public static bool SQLExecute(string connstring, string[] sqlstring)
        {
            bool lb_return = false;
            MySqlConnection l_connection = new MySqlConnection(connstring);
            try
            {
                l_connection.Open();
                MySqlCommand l_command = l_connection.CreateCommand();
                MySqlTransaction l_transaction = l_connection.BeginTransaction();
                try
                {

                    ArrayList al = new ArrayList(sqlstring);
                    l_command.Transaction = l_transaction;
                    for (int i = 0; i < al.Count; i = i + 5000)
                    {
                        string[] strsql = new string[al.Count - i > 5000 ? 5000 : al.Count - i];
                        al.CopyTo(i, strsql, 0, (al.Count - i > 5000 ? 5000 : al.Count - i));
                        string sql = string.Join("; ", strsql);
                        //sql = "begin " + sql + "; end; ";
                        l_command.CommandText = sql;
                        l_command.CommandTimeout = 36000;
                        l_command.ExecuteNonQuery();
                    }



                    //l_command.Transaction = l_transaction;
                    //string ls_sql = string.Join("; ", sqlstring);
                    //l_command.CommandText = ls_sql;
                    //l_command.CommandTimeout = 36000;
                    //l_command.ExecuteNonQuery();

                    //for (int i = 0; i < sqlstring.Length; i++)
                    //{
                    //    if (sqlstring[i].Trim() != string.Empty)
                    //    {
                    //        l_command.CommandText = sqlstring[i];
                    //        l_command.ExecuteNonQuery();
                    //    }
                    //}
                    l_transaction.Commit();
                    lb_return = true;
                }
                catch (Exception ex)
                {
                    l_transaction.Rollback();
                    lb_return = false;
                    string[] arrInfo = new string[4];
                    arrInfo[0] = ApplicationSetting.Translate("database exception");;
                    arrInfo[1] = ex.Message;
                    arrInfo[2] = "2";
                    Miscellaneous.Write2List(arrInfo);
                }
                finally { l_command.Dispose(); }
            }
            catch { lb_return = false; }
            finally
            {
                if (l_connection.State != ConnectionState.Closed)
                    l_connection.Close();
                l_connection.Dispose();
            }
            return lb_return;
        }
    }
}
