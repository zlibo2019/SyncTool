using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Collections.Specialized;
using System.Threading;


namespace ScmDataInterFace
{
    public partial class FrmMain : Form
    {
        private DataTable _DataTable = new DataTable();
        private string reg = "";
        private bool isLoad = false;
        private bool isLoad_config = false;
        private bool isQuery = true;
        private string _SqlFileName = string.Empty;
        private static List<string> lsTabPageAccess = new List<string>();
        public FrmMain()
        {
            InitializeComponent();
        }



        /// <summary>
        /// 装载调度任务
        /// </summary>
        /// <param name="value">调度</param>
        /// <returns></returns>
        public void LoadSchedule()
        {
            //string ls_accessFile = Project.ProjectFile;
            string ls_sql = "select *,'" + ApplicationSetting.Translate("run") + "' as excute,'" + ApplicationSetting.Translate("delete") + "' as del from PT_schedule";
            //string ls_sql = "select * from PT_schedule ";
            AccessDBop.SQLSelect(ls_sql, ref _DataTable);
            dgvSchedule.DataSource = _DataTable;
            for (int i = 0; i < dgvSchedule.Columns.Count; i++)
            {
                if (dgvSchedule.Columns[i].Name == "C1" || dgvSchedule.Columns[i].Name == "C2" ||
                    dgvSchedule.Columns[i].Name == "C3" || dgvSchedule.Columns[i].Name == "C4"
                    || dgvSchedule.Columns[i].Name == "C5" || dgvSchedule.Columns[i].Name == "C7"
                    || dgvSchedule.Columns[i].Name == "C8")
                    continue;
                dgvSchedule.Columns[i].Visible = false;
            }
        }

        private void test_Click(object sender, EventArgs e)
        {
            //if (ownSql_DataBase.Text.Trim() == "")
            //{
            //    ownSql_DataBase.Focus();
            //    return;
            //}
            bool IsSuccess = false;
            string strTag = ((Button)sender).Tag.ToString();
            string strConDirection = strTag.Split(',')[0];
            string strDataBase = strTag.Split(',')[1];
            //MessageBox.Show(GetConnectionString(strConDirection, strDataBase) );
            //MessageBox.Show( strConDirection + "，，，" + strDataBase);
            if (strDataBase == "sql")
            {
                IsSuccess = SQLServerDBop.IsConnected(GetConnectionString(strConDirection, strDataBase));
            }
            else if (strDataBase == "oracle")
            {
                IsSuccess = OracleDBop.IsConnected(GetConnectionString(strConDirection, strDataBase));
            }
            else if (strDataBase == "mysql")
            {
                IsSuccess = MySqlDBop.IsConnected(GetConnectionString(strConDirection, strDataBase));
            }
            if (IsSuccess)
            {
                MessageBox.Show(ApplicationSetting.Translate("connect success"), ApplicationSetting.Translate("prompt"));
            }
            else
            {
                MessageBox.Show(ApplicationSetting.Translate("connect fail"), ApplicationSetting.Translate("prompt"));
            }
        }

        /// <summary>
        /// 获得数据库连接字符串
        /// </summary>
        /// <returns>连接字符串</returns>
        private string GetConnectionString(string _strConDirection, string _database)
        {
            switch (_strConDirection.ToLower())
            {
                case "own":
                    if (_database == "sql")
                    {
                        if (r_nt.Checked)
                            return "Integrated Security=SSPI;Persist Security Info=False;pooling=false;Initial Catalog="
                                   + ownSql_DataBase.Text
                                   + ";Data Source=" + ownSql_ServerName.Text;
                        else
                            return "Data Source=" + ownSql_ServerName.Text +
                                    ";User ID=" + ownSql_SID.Text +
                                    ";Password=" + ownSql_PWD.Text +
                                    ";Initial Catalog=" + ownSql_DataBase.Text +
                                    ";Connect Timeout=5;pooling=false;";
                    }
                    else if (_database == "oracle")
                    {
                        return "Data Source=" + ownOracle_Service.Text +
                                ";User ID=" + ownOracle_UID.Text +
                                ";Password=" + ownOracle_pwd.Text + ";";
                    }
                    else
                    {
                        string[] tmpArr = ownmySql_ServerName.Text.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        return "Database = " + ownMySql_DataBase.Text +
                            (tmpArr.Length > 1 ? ";port=" + tmpArr[1] : "") +
                            (tmpArr.Length >= 1 ? ";Data Source = " + tmpArr[0] : ";Data Source = .") +
                            ";User Id = " + ownMySql_SID.Text +
                            ";Password =" + ownMySql_PWD.Text +
                            ";Charset = utf8";
                    }
                case "external":
                    if (_database == "sql")
                    {
                        if (r_exnt.Checked)
                            return "Integrated Security=SSPI;Persist Security Info=False;pooling=false;Initial Catalog="
                                  + exSql_DataBase.Text
                                  + ";Data Source=" + exSql_ServerName.Text;
                        else
                            return "Data Source=" + exSql_ServerName.Text +
                                    ";User ID=" + exSql_SID.Text +
                                    ";Password=" + exSql_PWD.Text +
                                    ";Initial Catalog=" + exSql_DataBase.Text +
                                    ";Connect Timeout=5;pooling=false;";
                    }
                    else if (_database == "oracle")
                    {
                        return "Data Source=" + exOracle_Service.Text +
                                ";User ID=" + exOracle_UID.Text +
                                ";Password=" + exOracle_pwd.Text + ";";
                    }
                    else
                    {
                        string[] tmpArr = ExMySql_ServerName.Text.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        return "Database = " + ExMySql_DataBase.Text +
                             (tmpArr.Length > 1 ? ";port = " + tmpArr[1] : "") +
                             (tmpArr.Length >= 1 ? ";Data Source = " + tmpArr[0] : ";Data Source = .") +
                            ";User Id = " + ExMySql_SID.Text +
                            ";Password=" + ExMySql_PWD.Text +
                            ";Charset = utf8";
                    }
                default:
                    break;
            }
            return "";
        }

        private void r_sql_CheckedChanged(object sender, EventArgs e)
        {
            if (r_sql.Checked)
            {
                tb_owndatabase.SelectedIndex = 0;
            }
            else if (r_oracle.Checked)
            {
                tb_owndatabase.SelectedIndex = 1;
            }
            else if (r_Mysql.Checked)
            {
                tb_owndatabase.SelectedIndex = 2;
            }
            else if (r_Exe.Checked)
            {
                tb_owndatabase.SelectedIndex = 3;
            }
            else if (r_webservice.Checked)
            {
                tb_owndatabase.SelectedIndex = 4;
            }
            else if (r_http.Checked)
            {
                tb_owndatabase.SelectedIndex = 5;
            }
            else if (r_access.Checked)
            {
                tb_owndatabase.SelectedIndex = 6;
            }
        }

        private void r_exsql_CheckedChanged(object sender, EventArgs e)
        {
            if (r_exsql.Checked)
            {
                tb_exdatabase.SelectedIndex = 0;
            }
            else if (r_exoracle.Checked)
            {
                tb_exdatabase.SelectedIndex = 1;
            }
            else if (r_exMysql.Checked)
            {
                tb_exdatabase.SelectedIndex = 2;
            }
            else if (r_exExe.Checked)
            {
                tb_exdatabase.SelectedIndex = 3;
            }
            else if (r_exWebservice.Checked)
            {
                tb_exdatabase.SelectedIndex = 4;
            }
            else if (r_exHttp.Checked)
            {
                tb_exdatabase.SelectedIndex = 5;
            }
            else if (r_exAccess.Checked)
            {
                tb_exdatabase.SelectedIndex = 6;
            }
        }

        private void b_setinterface(object sender, EventArgs e)
        {
            tb_setinterface.SelectedIndex = int.Parse(((Button)sender).Tag.ToString());
            ((Button)sender).Focus();
            ClearOtherButtonStatus();
            ((Button)sender).ForeColor = Color.White;
            ((Button)sender).BackgroundImage = Image.FromFile("images\\push.png");
        }

        private void ClearOtherButtonStatus()
        {
            b_env.ForeColor = Color.Black;
            b_env.BackgroundImage = Image.FromFile("images\\normal.png");
            b_tablestruct.ForeColor = Color.Black;
            b_tablestruct.BackgroundImage = Image.FromFile("images\\normal.png");
            b_tablenative.ForeColor = Color.Black;
            b_tablenative.BackgroundImage = Image.FromFile("images\\normal.png");
            b_task.ForeColor = Color.Black;
            b_task.BackgroundImage = Image.FromFile("images\\normal.png");
            b_schedule.ForeColor = Color.Black;
            b_schedule.BackgroundImage = Image.FromFile("images\\normal.png");
            btExcuteSql.ForeColor = Color.Black;
            btExcuteSql.BackgroundImage = Image.FromFile("images\\normal.png");
            btConfigBase.ForeColor = Color.Black;
            btConfigBase.BackgroundImage = Image.FromFile("images\\normal.png");
        }


        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow dr = _DataTable.NewRow();
                CreateSchedule cs = new CreateSchedule(dr);
                if (cs.ShowDialog() == DialogResult.OK)
                {
                    if (dr != null && dr["schedule_name"].ToString() != "")
                    {
                        DataRow[] drs = _DataTable.Select("schedule_ID = max(schedule_ID)");
                        if (drs.Length == 0)
                            dr["schedule_ID"] = 0;
                        else
                            dr["schedule_ID"] = int.Parse(drs[0][2].ToString()) + 1;
                        _DataTable.Rows.Add(dr);
                        string ls_sql = "select * from PT_schedule";
                        //string ls_sql = "select *,'执行' as excute,'删除' as del from PT_schedule ";
                        AccessDBop.SQLUpdate(ls_sql, ref _DataTable);
                    }
                    LoadSchedule();
                }
            }
            catch { }
        }
        /// <summary>
        /// 保存调度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_save_sch_Click(object sender, EventArgs e)
        {
            DataTable ldt = _DataTable.GetChanges(DataRowState.Deleted);
            string ls_sql = "select * from PT_schedule";
            //string ls_sql = "select *,'执行' as excute,'删除' as del from PT_schedule ";
            if (AccessDBop.SQLUpdate(ls_sql, ref _DataTable))
            {
                MessageBox.Show(ApplicationSetting.Translate("save success"));
                if (button6.Tag.ToString() == "停   止")    //如果自动执行启动，则重启一下
                {
                    EventArgs ea = new EventArgs();
                    button6_Click(button6, ea);
                    button6_Click(button6, ea);
                }
            }
            else MessageBox.Show(ApplicationSetting.Translate("save fail"));
        }

        private void dgvSchedule_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            try
            {
                DataRow[] drs = _DataTable.Select("schedule_ID = " + dgvSchedule["C1", e.RowIndex].Value.ToString());
                if (drs.Length > 0)
                {
                    CreateSchedule cs = new CreateSchedule(drs[0]);
                    if (cs.ShowDialog() == DialogResult.OK)
                    {
                        string ls_sql = "select * from PT_schedule";
                        AccessDBop.SQLUpdate(ls_sql, ref _DataTable);
                    }
                }
            }
            catch { }
        }

        void LoadConfigItem(string theirAlias, string ourAlias)
        {
            DataTable dt = new DataTable();
            string ls_sql = "select theirAlias,ourAlias,synItem,isChecked,isValid from IF_ConfigBase where theirAlias = '{0}' and ourAlias = '{1}'";
            ls_sql = string.Format(ls_sql, theirAlias, ourAlias);
            AccessDBop.SQLSelect_configBase(ls_sql, ref dt);
            dgv_configBase.DataSource = dt;
            ////无同步内容的
            //if (Project.login == "admin")
            //{
            //    return;
            //}
            //if (((dgv_configBase.Rows.Count == 2) && (dgv_configBase.Rows[0].Cells["synItem"].Value.ToString() == "NULL")) || (dgv_configBase.Rows.Count < 2))
            //{
            //    dgv_configBase.Visible = false;
            //}
            //else
            //{
            //    dgv_configBase.Visible = true;
            //}
        }

        void LoadOurAliasFromAppcfg()
        {
            try
            {
                //从app.config加载我方项
                NameValueCollection nvcOurConfigBase = (NameValueCollection)ConfigurationManager.GetSection("ourCfgBase");
                cmbOurAlias.DataSource = null;
                cmbOurAlias.Items.Clear();
                foreach (String str in nvcOurConfigBase.AllKeys)
                {
                    cmbOurAlias.Items.Add(str);
                }
                if (cmbOurAlias.Items.Count > 0)
                {
                    cmbOurAlias.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {

            }
        }

        void LoadTheirAliasFromMdb()
        {
            //从mdb库加载对方项
            string ls_sql = "select distinct theirAlias from IF_configBase";
            DataTable dt = new DataTable();
            AccessDBop.SQLSelect_configBase(ls_sql, ref dt);
            cmbTheirAlias.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                cmbTheirAlias.Items.Add(dr["theirAlias"].ToString());
            }
            if (cmbTheirAlias.Items.Count > 0)
            {
                cmbTheirAlias.SelectedIndex = 0;
            }
        }

        void LoadCurrentConfigBase()
        {
            //初始化配置
            string ls_sql = "select distinct theirAlias,ourAlias from IF_ConfigBase where isValid = 'Y'";
            DataTable dt = new DataTable();
            AccessDBop.SQLSelect_configBase(ls_sql, ref dt);
            if (dt.Rows.Count > 0)
            {
                string theirAlias = dt.Rows[0]["theirAlias"].ToString();
                string ourAlias = dt.Rows[0]["ourAlias"].ToString();
                Project.DB_Alias = ourAlias;
                Project.theirDB_Alias = theirAlias;
                cmbTheirAlias.Text = theirAlias;
                cmbOurAlias.Text = ourAlias;
                LoadConfigItem(theirAlias, ourAlias);
            }
        }

        private void LoadConfigBase()
        {
            LoadOurAliasFromAppcfg();
            LoadTheirAliasFromMdb();
            LoadCurrentConfigBase();
            isLoad_config = true;
        }

        bool CheckMachineInfo()
        {
            //读取注册表,验证机器码（一级验证）
            LoadSettingsFromRegistry();
            if (reg == "")
            {
                Form4 f4 = new Form4();
                f4.ShowDialog();
                if (!f4.IsZhuCe)
                    return false;
            }
            else
            {
                Form4 f4 = new Form4();
                if (reg != Miscellaneous.md35(f4.GetCpuID()))
                {
                    MessageBox.Show(ApplicationSetting.Translate("Reg code is not correct"), ApplicationSetting.Translate("prompt"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Form4 f = new Form4();
                    f.ShowDialog();
                    if (!f4.IsZhuCe)
                        return false;
                }
            }
            return true;
        }
        //检查mdb库密钥
        bool CheckMdbEncry()
        {
            //MessageBox.Show("7");//cs

            DataTable dt = new DataTable();
            string sql = "select my from IF_PHOTO";
            string my = string.Empty;
            bool bResult = AccessDBop.SQLSelect(sql, ref dt);
            if (bResult)
            {
                if (dt.Rows.Count > 0)
                {
                    my = dt.Rows[0]["my"].ToString();
                }
            }
            else
            {
                ////MessageBox.Show("8");//cs

                return false;
            }

            //MessageBox.Show(Project.theirDB_Alias + Project.DB_Alias+"---"+my);//cs

            if (Miscellaneous.md35(Project.theirDB_Alias + Project.DB_Alias).Equals(my))
            {
                return true;
            }
            else
            {
                //MessageBox.Show("9");//cs

                return false;
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //验证机器码是否注册(一级验证)
            if (!CheckMachineInfo())
            {
                isQuery = false;
                Application.Exit();
            }

            string loginType = ConfigurationManager.AppSettings["LoginType"].ToString();
            //是否允许外部程序控制其启动和关闭调度
            int isExtenControl = int.Parse(ConfigurationManager.AppSettings["isExtenControl"].ToString());

            if (!loginType.Equals("0"))
            {
                if (Project.login == string.Empty)
                {
                    using (frmLogin frm = new frmLogin())
                    {
                        if (frm.ShowDialog() != DialogResult.OK)
                        {
                            Application.Exit();
                        }
                    }
                }
            }

            try
            {
                notifyIcon1.Text = ConfigurationManager.AppSettings["NoticeName"].ToString();
                Project.sysType = int.Parse(ConfigurationManager.AppSettings["sysType"].ToString());
                Project.IsInsertLogToDb = bool.Parse(ConfigurationManager.AppSettings["IsInsertLogToDb"].ToString());
                bool IsConfigBaseExists = File.Exists(Application.StartupPath + "\\configBase.mdb");
                while (!IsConfigBaseExists)
                {
                    using (frmSelectMdbFile frm = new frmSelectMdbFile())
                    {
                        frm.Text = "缺少configBase.mdb文件";
                        if (frm.ShowDialog() != DialogResult.OK)
                        {
                            isQuery = false;
                            Application.Exit();
                            return;
                        }
                    }
                    IsConfigBaseExists = File.Exists(Application.StartupPath + "\\configBase.mdb");
                }

                bool IsMdbExists = File.Exists(Application.StartupPath + "\\ScmDataInterFace.mdb");
                while (!IsMdbExists)
                {
                    using (frmSelectMdbFile frm = new frmSelectMdbFile())
                    {
                        frm.Text = "缺少ScmDataInterFace.mdb文件";
                        if (frm.ShowDialog() != DialogResult.OK)
                        {
                            isQuery = false;
                            Application.Exit();
                            return;
                        }
                    }
                    IsMdbExists = File.Exists(Application.StartupPath + "\\ScmDataInterFace.mdb");
                }

                //MessageBox.Show("1");//cs

                if (Project.login == "admin")
                {
                    //初始化配置库界面数据
                    LoadConfigBase();
                    //初始化其它界面数据
                    init();
                }
                else
                {
                    string isLock = string.Empty;
                    isLock = ConfigurationManager.AppSettings["isLock"].ToString();
                    if (isLock == "1")
                    {
                        MessageBox.Show("有严重错误未处理,请处理后用管理员身份登陆", "说明", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        isQuery = false;
                        Application.Exit();
                    }

                    //初始化配置库界面数据
                    LoadConfigBase();

                    //MessageBox.Show("5");//cs

                    //验证配置库是否注册（二级验证，admin用户不受该限制）
                    if (!CheckMdbEncry())
                    {
                        //MessageBox.Show("6");//cs

                        MessageBox.Show("配置库未被注册或没有配置库,程序将退出！", "说明", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        isQuery = false;
                        Application.Exit();
                    }
                    //初始化数据库配置
                    LoadDBConnection();
                    //初始化执行SQL配置


                    //MessageBox.Show("2");//cs

                    LoadRunSql();
                    //初始化任务
                    LoadTask();
                    //初始化调度
                    //MessageBox.Show("3");//cs

                    LoadSchedule();
                    //设置表格样式
                    SetDataGridStyle();
                    //设置界面样式
                    SetInterfaceStyle();

                    //MessageBox.Show("4");//cs

                    //初始化权限界面
                    //rbNormalModel.Checked = true;
                    SetOperaterAuthority(true);
                    button6_Click(sender, e);
                }
                b_setinterface(b_schedule, e);
                tsslOperater.Text = "operator:" + Project.login;
                //启动实时信息显示的线程
                ReadInfo();
                if (isExtenControl == 1)
                {
                    timerExtenControl.Enabled = true;
                }
                isLoad = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetInterfaceStyle()
        {
            //b_env.BackgroundImage = Image.FromFile("images\\push.png");
            b_tablestruct.BackgroundImage = Image.FromFile("images\\normal.png");
            b_tablenative.BackgroundImage = Image.FromFile("images\\normal.png");
            b_task.BackgroundImage = Image.FromFile("images\\normal.png");
            b_schedule.BackgroundImage = Image.FromFile("images\\normal.png");
            button6.BackgroundImage = Image.FromFile("images\\start.png");
            pictureBox2.Image = Image.FromFile("images\\server1.png");
            pictureBox4.Image = Image.FromFile("images\\server2.png");
            splitContainer1.Panel1.BackgroundImage = Image.FromFile("images\\top.png");
            pictureBox1.BackgroundImage = Image.FromFile("images\\LOGO.png");
            tableLayoutPanel1.BackgroundImage = Image.FromFile("images\\gray.png");
            tableLayoutPanel2.BackgroundImage = Image.FromFile("images\\gray.png");
            tableLayoutPanel3.BackgroundImage = Image.FromFile("images\\gray.png");
            tableLayoutPanel4.BackgroundImage = Image.FromFile("images\\gray.png");
            tableLayoutPanel6.BackgroundImage = Image.FromFile("images\\gray.png");
            //b_env.BackgroundImage = Image.FromFile("images\\start.png");
        }

        private void SetDataGridStyle()
        {
            DataGridStyle.GridDisplayStyle(dgv_CompareTable, true);
            DataGridStyle.GridDisplayStyle(dgv_Ltable, true);
            DataGridStyle.GridDisplayStyle(dgv_Ztable, true);
            DataGridStyle.GridDisplayStyle(dgvSchedule, true, 27);
            DataGridStyle.GridDisplayStyle(d_Task, true);
            DataGridStyle.GridDisplayStyle(dgv_configBase, true);
        }
        /// <summary>
        /// 配置表中存最大记录编号等杂项的表
        /// </summary>
        //private void LoadConfig()
        //{
        //    string ls_sql = "select * from config";
        //    DataTable dt = new DataTable();
        //    AccessDBop.SQLSelect(ls_sql, ref dt);
        //    if (dt.Rows.Count > 0)
        //    {
        //        Project.MaxRecordBh = Convert.ToInt32(dt.Rows[0]["MaxRecordBh"].ToString());
        //    }
        //}

        public static void SaveConfig()
        {
            //string ls_sql = "update config set MaxRecordBh =" + Project.MaxRecordBh.ToString();
            //DataTable dt = new DataTable();
            //AccessDBop.SQLExecute(ls_sql);
        }

        private void LoadTask()
        {
            //初始化任务
            string ls_sql = "select distinct Task_id,Task_Name,'删除' as del,'执行' as trans from IF_Task order by Task_id";
            DataTable dt = new DataTable();
            AccessDBop.SQLSelect(ls_sql, ref dt);
            d_Task.DataSource = dt;
        }

        private void LoadCompareTable()
        {
            //初始化数据表关系设置
            string ls_sql = "select TableName,CompareTableName,'删除' as del from IF_Table_Compare";
            DataTable dt = new DataTable();
            AccessDBop.SQLSelect(ls_sql, ref dt);
            dgv_CompareTable.DataSource = dt;
        }
        /// <summary>
        /// 初始化基础表配置

        /// </summary>
        private void LoadBaseTable()
        {
            //初始化基础表配置

            string ls_sql = "select Table_Name,Table_desc,'删除' as del,CompareTableName,SWITCH(table_position = '0', '{0}',True,'{1}') AS table_position_alias from IF_Table_infor where Table_Type = 'Z'";
            ls_sql = string.Format(ls_sql, Project.DB_Alias, Project.theirDB_Alias);
            DataTable dt = new DataTable();
            AccessDBop.SQLSelect(ls_sql, ref dt);
            dgv_Ztable.DataSource = dt;
            ls_sql = "select Table_Name,Table_desc,'删除' as del,CompareTableName,SWITCH(table_position = '0', '{0}',True,'{1}') AS table_position_alias from IF_Table_infor where Table_Type = 'L'";
            ls_sql = string.Format(ls_sql, Project.DB_Alias, Project.theirDB_Alias);
            dt = new DataTable();
            AccessDBop.SQLSelect(ls_sql, ref dt);
            dgv_Ltable.DataSource = dt;
        }

        /// <summary>
        /// 初始化数据库配置
        /// </summary>
        private void LoadOurDBConnectionFromMDB()
        {
            //获取连接信息
            lbTheirBase.Text = Project.theirDB_Alias + " " + ApplicationSetting.Translate("connect config");
            lbOurBase.Text = Project.DB_Alias + " " + ApplicationSetting.Translate("connect config");
            string ls_sql = "select * from IF_DBConnectionInfor ";
            DataTable dt = new DataTable();
            AccessDBop.SQLSelect(ls_sql, ref dt);
            DataRow[] dr_owns = dt.Select("bz = 0");//我方
            //DataRow[] dr_externals = dt.Select("bz = 1");//对方
            if (dr_owns.Length > 0)
            {
                DataRow dr_own = dr_owns[0];
                r_sql.Checked = (dr_own["Db_type"].ToString() == "sql" ? true : false);
                r_oracle.Checked = (dr_own["Db_type"].ToString() == "ora" ? true : false);
                r_Mysql.Checked = (dr_own["Db_type"].ToString() == "mysql" ? true : false);
                r_Exe.Checked = (dr_own["Db_type"].ToString() == "exe" ? true : false);
                r_webservice.Checked = (dr_own["Db_type"].ToString() == "webservice" ? true : false);
                r_http.Checked = (dr_own["Db_type"].ToString() == "http" ? true : false);
                r_access.Checked = (dr_own["Db_type"].ToString() == "access" ? true : false);
                if (r_sql.Checked)
                {
                    ownSql_ServerName.Text = dr_own["Db_server"].ToString();
                    ownSql_SID.Text = dr_own["Db_User"].ToString();
                    ownSql_PWD.Text = dr_own["Db_Password"].ToString();
                    ownSql_DataBase.Text = dr_own["Db_name"].ToString();
                    r_nt.Checked = (dr_own["Db_Mode"].ToString() == "1" ? true : false);
                    r_sq.Checked = (dr_own["Db_Mode"].ToString() == "0" ? true : false);
                }
                else if (r_oracle.Checked)
                {
                    ownOracle_Service.Text = dr_own["Db_server"].ToString();
                    ownOracle_UID.Text = dr_own["Db_User"].ToString();
                    ownOracle_pwd.Text = dr_own["Db_Password"].ToString();
                }
                else if (r_Mysql.Checked)
                {
                    ownmySql_ServerName.Text = dr_own["Db_server"].ToString();
                    ownMySql_DataBase.Text = dr_own["Db_name"].ToString();
                    ownMySql_SID.Text = dr_own["Db_User"].ToString();
                    ownMySql_PWD.Text = dr_own["Db_Password"].ToString();
                }
                else if (r_webservice.Checked)
                {
                    ownWebService_ServerName.Text = dr_own["Db_server"].ToString();
                }
                else if (r_http.Checked)
                {
                    own_cmbFormat.Text = dr_own["Db_server"].ToString() == "" ? "JSon" : dr_own["Db_server"].ToString();
                }
                else if (r_access.Checked)
                {
                    tbOwnAccessAddress.Text = dr_own["Db_server"].ToString();
                }
                string db_type = dr_own["Db_type"].ToString();
                if (db_type == "webservice")
                {
                    Project.DB_Type = "webservice";
                }
                else if (db_type == "http")
                {
                    Project.DB_Type = "http";
                }
                else if (db_type == "access")
                {
                    Project.DB_Type = "access";
                }
                else
                {
                    Project.DB_Type = (db_type == "sql" ? "sql server" : db_type == "ora" ? "oracle" : db_type == "mysql" ? "mysql" : "exe");
                }
                if (r_access.Checked)
                {
                    Project.DB_Connection = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dr_own["Db_server"].ToString();
                }
                else
                {
                    string[] tmpArr = dr_own["Db_server"].ToString().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    Project.DB_Connection = (r_sql.Checked ?
                               (r_nt.Checked ?
                                   ("Integrated Security=SSPI;Persist Security Info=False;pooling=false;Initial Catalog="
                                   + dr_own["Db_name"]
                                   + ";Data Source=" + dr_own["Db_server"])
                                   :
                                   ("server = " + dr_own["Db_server"]
                                   + ";database = " + dr_own["Db_name"]
                                   + ";Uid = " + dr_own["Db_User"]
                                   + ";pwd = " + dr_own["Db_Password"] + ";pooling=false;")
                               ) :
                               r_oracle.Checked ?
                               ("server = " + dr_own["Db_server"]
                               + ";Uid = " + dr_own["Db_User"]
                               + ";pwd = " + dr_own["Db_Password"])
                               :
                                ("Database = " + dr_own["Db_name"] +
                                (tmpArr.Length > 1 ? ";port=" + tmpArr[1] : "") +
                                (tmpArr.Length >= 1 ? ";Data Source = " + tmpArr[0] : ";Data Source = .") +
                                ";User Id = " + dr_own["Db_User"] +
                                ";Password=" + dr_own["Db_Password"] + ";Charset = utf8")
                               );
                }
                Project.Uid = dr_own["Db_User"].ToString();
                Project.Pwd = dr_own["Db_Password"].ToString();
                Project.SeverName = dr_own["Db_server"].ToString();
                Project.DbName = dr_own["Db_name"].ToString();
            }
        }

        /// <summary>
        /// 初始化数据库配置
        /// </summary>
        private void LoadTheirDBConnectionFromMDB()
        {
            //获取连接信息
            lbTheirBase.Text = Project.theirDB_Alias + " " + ApplicationSetting.Translate("connect config");
            lbOurBase.Text = Project.DB_Alias + " " + ApplicationSetting.Translate("connect config");
            string ls_sql = "select * from IF_DBConnectionInfor ";
            DataTable dt = new DataTable();
            AccessDBop.SQLSelect(ls_sql, ref dt);
            //DataRow[] dr_owns = dt.Select("bz = 0");//我方
            DataRow[] dr_externals = dt.Select("bz = 1");//对方
            if (dr_externals.Length > 0)
            {
                DataRow dr_external = dr_externals[0];
                r_exsql.Checked = (dr_external["Db_type"].ToString() == "sql" ? true : false);
                r_exoracle.Checked = (dr_external["Db_type"].ToString() == "ora" ? true : false);
                r_exMysql.Checked = (dr_external["Db_type"].ToString() == "mysql" ? true : false);
                r_exExe.Checked = (dr_external["Db_type"].ToString() == "exe" ? true : false);
                r_exWebservice.Checked = (dr_external["Db_type"].ToString() == "webservice" ? true : false);
                r_exHttp.Checked = (dr_external["Db_type"].ToString() == "http" ? true : false);
                r_exAccess.Checked = (dr_external["Db_type"].ToString() == "access" ? true : false);
                if (r_exsql.Checked)
                {
                    exSql_ServerName.Text = dr_external["Db_server"].ToString();
                    exSql_SID.Text = dr_external["Db_User"].ToString();
                    exSql_PWD.Text = dr_external["Db_Password"].ToString();
                    exSql_DataBase.Text = dr_external["Db_name"].ToString();
                    r_exnt.Checked = (dr_external["Db_Mode"].ToString() == "1" ? true : false);
                    r_exsq.Checked = (dr_external["Db_Mode"].ToString() == "0" ? true : false);
                }
                else if (r_exoracle.Checked)
                {
                    exOracle_Service.Text = dr_external["Db_server"].ToString();
                    exOracle_UID.Text = dr_external["Db_User"].ToString();
                    exOracle_pwd.Text = dr_external["Db_Password"].ToString();
                }
                else if (r_exMysql.Checked)
                {
                    ExMySql_ServerName.Text = dr_external["Db_server"].ToString();
                    ExMySql_DataBase.Text = dr_external["Db_name"].ToString();
                    ExMySql_SID.Text = dr_external["Db_User"].ToString();
                    ExMySql_PWD.Text = dr_external["Db_Password"].ToString();
                }
                else if (r_exWebservice.Checked)
                {
                    exWebService_ServerName.Text = dr_external["Db_server"].ToString();
                }
                else if (r_exHttp.Checked)
                {
                    ex_cmbFormat.Text = dr_external["Db_server"].ToString() == "" ? "JSon" : dr_external["Db_server"].ToString();
                }
                else if (r_exAccess.Checked)
                {
                    tbExAccessAddress.Text = dr_external["Db_server"].ToString();
                }
                string db_type = dr_external["Db_type"].ToString();
                if (db_type == "webservice")
                {
                    Project.TheirDB_Type = "webservice";
                }
                else if (db_type == "http")
                {
                    Project.TheirDB_Type = "http";
                }
                else if (db_type == "access")
                {
                    Project.TheirDB_Type = "access";
                }
                else
                {
                    Project.TheirDB_Type = (db_type == "sql" ? "sql server" : db_type == "ora" ? "oracle" : db_type == "mysql" ? "mysql" : "exe");
                }

                if (r_exAccess.Checked)
                {
                    Project.TheirDB_Connection = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dr_external["Db_server"].ToString();
                }
                else
                {
                    string[] tmpArr = dr_external["Db_server"].ToString().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    Project.TheirDB_Connection = (r_exsql.Checked ?
                       (r_exnt.Checked ?
                       ("Integrated Security=SSPI;Persist Security Info=False;pooling=false;Initial Catalog="
                       + dr_external["Db_name"]
                       + ";Data Source=" + dr_external["Db_server"])
                       :
                       ("server = " + dr_external["Db_server"]
                       + ";database = " + dr_external["Db_name"]
                       + ";Uid = " + dr_external["Db_User"]
                       + ";pwd = " + dr_external["Db_Password"] + ";pooling=false;")
                       ) :
                       r_exoracle.Checked ?
                       ("server = " + dr_external["Db_server"]
                       + ";Uid = " + dr_external["Db_User"]
                       + ";pwd = " + dr_external["Db_Password"])
                       :
                       ("Database = " + dr_external["Db_name"] +
                                (tmpArr.Length > 1 ? ";port=" + tmpArr[1] : "") +
                                (tmpArr.Length >= 1 ? ";Data Source = " + tmpArr[0] : ";Data Source = .") +
                                ";User Id = " + dr_external["Db_User"] +
                                ";Password=" + dr_external["Db_Password"] + ";Charset = utf8")
                       );
                }
                Project.theirUid = dr_external["Db_User"].ToString();
                Project.theirPwd = dr_external["Db_Password"].ToString();
                Project.theirSeverName = dr_external["Db_server"].ToString();
                Project.theirDbName = dr_external["Db_name"].ToString();
            }
        }

        private bool LoadOurDBConnectionFromFile()
        {
            string LastPath = Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf(@"\"));
            string fileName = LastPath + "\\odb\\odb.cfg";
            bool bResult = File.Exists(fileName);
            if (!bResult)
            {
                return false;
            }
            try
            {
                IniFiles inifile = new IniFiles(fileName);
                string Db_Password = inifile.ReadString("Database", "Password", "");
                string db_type = inifile.ReadString("Database", "cType", "sql");
                string port = inifile.ReadString("Database", "port", "3306");
                string verifyType = inifile.ReadString("Database", "sYanZhenfs", "0");
                string Db_server = inifile.ReadString("Database", "Server", "");
                string Db_User = inifile.ReadString("Database", "Username", "");
                string Db_name = inifile.ReadString("Database", "DBname", "");

                r_sql.Checked = (db_type == "sql" ? true : false);
                r_oracle.Checked = (db_type == "ora" ? true : false);
                r_Mysql.Checked = (db_type == "mysql" ? true : false);
                r_Exe.Checked = (db_type == "exe" ? true : false);
                if (r_sql.Checked)
                {
                    ownSql_ServerName.Text = Db_server;
                    ownSql_SID.Text = Db_User;
                    ownSql_PWD.Text = Db_Password;
                    ownSql_DataBase.Text = Db_name;
                    r_nt.Checked = (verifyType == "0" ? true : false);
                    r_sq.Checked = (verifyType == "1" ? true : false);
                }
                else if (r_oracle.Checked)
                {
                    ownOracle_Service.Text = Db_server;
                    ownOracle_UID.Text = Db_User;
                    ownOracle_pwd.Text = Db_Password;
                }
                else if (r_Mysql.Checked)
                {
                    string[] tmpArr = Db_server.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    ownmySql_ServerName.Text = Db_server + (port == "" ? "" : ":" + port);
                    ownMySql_DataBase.Text = Db_name;
                    ownMySql_SID.Text = Db_User;
                    ownMySql_PWD.Text = Db_Password;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 初始化数据库配置
        /// </summary>
        private void LoadDBConnection()
        {
            if (!LoadOurDBConnectionFromFile())
            {
                LoadOurDBConnectionFromMDB();
            }
            LoadTheirDBConnectionFromMDB();
        }


        /// <summary>
        /// 保存数据库配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_save_database_Click(object sender, EventArgs e)
        {
            //if (Project.DB_Type != (r_sql.Checked ? "sql server" : "oracle"))
            //{
            //    DialogResult drt = MessageBox.Show("我方数据库类型已改变，确定要将已配置的表结构自动转换吗？\n若选择[是]则将已配置的表转换结构保存\n若选择[否]则不转换表结构保存", "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            //    if (drt == DialogResult.Cancel)
            //    {
            //        if (r_sql.Checked)
            //            r_oracle.Checked = true;
            //        if (r_oracle.Checked)
            //            r_sql.Checked = true;
            //        return;
            //    }
            //    Project.DB_Type = (r_sql.Checked ? "sql server" : "oracle");
            //    if (drt == DialogResult.Yes)
            //    {
            //        if (!DataBaseManager.DataBaseConvert())
            //        {
            //            MessageBox.Show("转换失败，请手动转换");
            //        }
            //    }
            //}

            string ls_sql = "select * from IF_DBConnectionInfor ";
            DataTable dt = new DataTable();
            AccessDBop.SQLSelect(ls_sql, ref dt);
            if (dt.Rows.Count == 0)
            {
                DataRow dr = dt.NewRow();
                dr["bz"] = "0";
                dt.Rows.Add(dr);
                dr = dt.NewRow();
                dr["bz"] = "1";
                dt.Rows.Add(dr);
            }
            DataRow[] dr_owns = dt.Select("bz = 0");//我方
            DataRow[] dr_externals = dt.Select("bz = 1");//对方

            if (dr_owns.Length > 0)
            {
                DataRow dr_own = dr_owns[0];
                if (r_webservice.Checked)
                {
                    dr_own["Db_type"] = "webservice";
                }
                else if (r_http.Checked)
                {
                    dr_own["Db_type"] = "http";
                }
                else if (r_access.Checked)
                {
                    dr_own["Db_type"] = "access";
                }
                else
                {
                    dr_own["Db_type"] = (r_sql.Checked ? "sql" : (r_oracle.Checked ? "ora" : (r_Mysql.Checked ? "mysql" : "exe")));
                }

                if (r_sql.Checked)
                {
                    dr_own["Db_server"] = ownSql_ServerName.Text;
                    dr_own["Db_User"] = ownSql_SID.Text;
                    dr_own["Db_Password"] = ownSql_PWD.Text;
                    dr_own["Db_name"] = ownSql_DataBase.Text;
                    if (r_nt.Checked)
                        dr_own["Db_Mode"] = "1";
                    else
                        dr_own["Db_Mode"] = "0";
                }
                else if (r_oracle.Checked)
                {
                    dr_own["Db_server"] = ownOracle_Service.Text;
                    dr_own["Db_User"] = ownOracle_UID.Text;
                    dr_own["Db_Password"] = ownOracle_pwd.Text;
                }
                else if (r_Mysql.Checked)
                {
                    dr_own["Db_server"] = ownmySql_ServerName.Text;
                    dr_own["Db_User"] = ownMySql_SID.Text;
                    dr_own["Db_Password"] = ownMySql_PWD.Text;
                    dr_own["Db_name"] = ownMySql_DataBase.Text;
                }
                else if (r_webservice.Checked)
                {
                    dr_own["Db_server"] = ownWebService_ServerName.Text;
                }
                else if (r_http.Checked)
                {
                    dr_own["Db_server"] = own_cmbFormat.Text;
                }
                else if (r_access.Checked)
                {
                    dr_own["Db_server"] = tbOwnAccessAddress.Text;
                }

                if (r_webservice.Checked)
                    Project.DB_Type = "webservice";
                else if (r_http.Checked)
                    Project.DB_Type = "http";
                else if (r_access.Checked)
                    Project.DB_Type = "access";
                else
                    Project.DB_Type = (r_sql.Checked ? "sql server" : (r_oracle.Checked ? "oracle" : (r_Mysql.Checked ? "mysql" : "exe")));

                if (r_access.Checked)
                {
                    Project.DB_Connection = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + tbOwnAccessAddress.Text;
                }
                else
                {
                    string[] tmpArr = dr_own["Db_server"].ToString().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    Project.DB_Connection = (r_sql.Checked ?
                                           (r_nt.Checked ?
                                               ("Integrated Security=SSPI;Persist Security Info=False;pooling=false;Initial Catalog="
                                               + dr_own["Db_name"]
                                               + ";Data Source=" + dr_own["Db_server"])
                                               :
                                               ("server = " + dr_own["Db_server"]
                                               + ";database = " + dr_own["Db_name"]
                                               + ";Uid = " + dr_own["Db_User"]
                                               + ";pwd = " + dr_own["Db_Password"] + ";pooling=false;")
                                           ) :
                                           r_oracle.Checked ?
                                           ("server = " + dr_own["Db_server"]
                                           + ";Uid = " + dr_own["Db_User"]
                                           + ";pwd = " + dr_own["Db_Password"])
                                           :
                                           ("Database = " + dr_own["Db_name"] +
                                (tmpArr.Length > 1 ? ";port=" + tmpArr[1] : "") +
                                (tmpArr.Length >= 1 ? ";Data Source = " + tmpArr[0] : ";Data Source = .") +
                                ";User Id = " + dr_own["Db_User"] +
                                ";Password=" + dr_own["Db_Password"] + ";Charset = utf8")
                                           );
                }
                Project.Uid = dr_own["Db_User"].ToString();
                Project.Pwd = dr_own["Db_Password"].ToString();
                Project.SeverName = dr_own["Db_server"].ToString();
                Project.DbName = dr_own["Db_name"].ToString();
            }
            if (dr_externals.Length > 0)
            {
                DataRow dr_external = dr_externals[0];

                if (r_exWebservice.Checked)
                {
                    dr_external["Db_type"] = "webservice";
                }
                else if (r_exHttp.Checked)
                {
                    dr_external["Db_type"] = "http";
                }
                else if (r_exAccess.Checked)
                {
                    dr_external["Db_type"] = "access";
                }
                else
                {
                    dr_external["Db_type"] = (r_exsql.Checked ? "sql" : (r_exoracle.Checked ? "ora" : r_exMysql.Checked ? "mysql" : "exe"));
                }

                if (r_exsql.Checked)
                {
                    dr_external["Db_server"] = exSql_ServerName.Text;
                    dr_external["Db_User"] = exSql_SID.Text;
                    dr_external["Db_Password"] = exSql_PWD.Text;
                    dr_external["Db_name"] = exSql_DataBase.Text;
                    if (r_exnt.Checked)
                        dr_external["Db_Mode"] = "1";
                    else
                        dr_external["Db_Mode"] = "0";
                }
                else if (r_exoracle.Checked)
                {
                    dr_external["Db_server"] = exOracle_Service.Text;
                    dr_external["Db_User"] = exOracle_UID.Text;
                    dr_external["Db_Password"] = exOracle_pwd.Text;
                }
                else if (r_exMysql.Checked)
                {
                    dr_external["Db_server"] = ExMySql_ServerName.Text;
                    dr_external["Db_User"] = ExMySql_SID.Text;
                    dr_external["Db_Password"] = ExMySql_PWD.Text;
                    dr_external["Db_name"] = ExMySql_DataBase.Text;
                }
                else if (r_exWebservice.Checked)
                {
                    dr_external["Db_server"] = exWebService_ServerName.Text;
                }
                else if (r_exHttp.Checked)
                {
                    dr_external["Db_server"] = ex_cmbFormat.Text;
                }
                else if (r_exAccess.Checked)
                {
                    dr_external["Db_server"] = tbExAccessAddress.Text;
                }

                if (r_exWebservice.Checked)
                    Project.TheirDB_Type = "webservice";
                else if (r_exHttp.Checked)
                    Project.TheirDB_Type = "http";
                else if (r_exAccess.Checked)
                    Project.TheirDB_Type = "access";
                else
                    Project.TheirDB_Type = (r_exsql.Checked ? "sql server" : (r_exoracle.Checked ? "oracle" : (r_exMysql.Checked ? "mysql" : "exe")));

                if (r_exAccess.Checked)
                {
                    Project.TheirDB_Connection = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + tbExAccessAddress.Text;
                }
                else
                {
                    string[] tmpArr = dr_external["Db_server"].ToString().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    Project.TheirDB_Connection = (r_exsql.Checked ?
                                  (r_exnt.Checked ?
                                  ("Integrated Security=SSPI;Persist Security Info=False;pooling=false;Initial Catalog="
                                  + dr_external["Db_name"]
                                  + ";Data Source=" + dr_external["Db_server"])
                                  :
                                  ("server = " + dr_external["Db_server"]
                                  + ";database = " + dr_external["Db_name"]
                                  + ";Uid = " + dr_external["Db_User"]
                                  + ";pwd = " + dr_external["Db_Password"] + ";pooling=false;")
                                  ) :
                                  r_exoracle.Checked ?
                                  ("server = " + dr_external["Db_server"]
                                  + ";Uid = " + dr_external["Db_User"]
                                  + ";pwd = " + dr_external["Db_Password"])
                                  :
                                  ("Database = " + dr_external["Db_name"] +
                                (tmpArr.Length > 1 ? ";port=" + tmpArr[1] : "") +
                                (tmpArr.Length >= 1 ? ";Data Source = " + tmpArr[0] : ";Data Source = .") +
                                ";User Id = " + dr_external["Db_User"] +
                                ";Password=" + dr_external["Db_Password"] + ";Charset = utf8")
                                  );
                }
                Project.theirUid = dr_external["Db_User"].ToString();
                Project.theirPwd = dr_external["Db_Password"].ToString();
                Project.theirSeverName = dr_external["Db_server"].ToString();
                Project.theirDbName = dr_external["Db_name"].ToString();
            }
            ls_sql = "select * from IF_DBConnectionInfor";
            if (AccessDBop.SQLUpdate(ls_sql, ref dt))
            {
                //if (rbGuideModel.Checked)
                //{
                //    GiveAccess("tpRunSql");
                //}
                MessageBox.Show(ApplicationSetting.Translate("save success"));
            }
            else
            {
                MessageBox.Show(ApplicationSetting.Translate("save fail"));
                return;
            }
        }
        /// <summary>
        /// 增加基础表

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_addZtable_Click(object sender, EventArgs e)
        {
            EditForm f_edit = new EditForm("Z");
            if (f_edit.ShowDialog() == DialogResult.OK)
            {
                LoadBaseTable();
            }
        }

        private void dgv_Ztable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            EditForm f_edit = new EditForm(dgv_Ztable["Table_Name", e.RowIndex].Value.ToString(),
                dgv_Ztable["Table_desc", e.RowIndex].Value.ToString(), "Z", dgv_Ztable["C_ZCompareTable", e.RowIndex].Value.ToString(), dgv_Ztable["table_position", e.RowIndex].Value.ToString());
            if (f_edit.ShowDialog() == DialogResult.OK)
            {
                LoadBaseTable();
            }
        }

        private void b_addLtable_Click(object sender, EventArgs e)
        {
            EditForm f_edit = new EditForm("L");
            if (f_edit.ShowDialog() == DialogResult.OK)
            {
                LoadBaseTable();
            }
        }

        private void dgv_Ltable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            EditForm f_edit = new EditForm(dgv_Ltable["L_Table_Name", e.RowIndex].Value.ToString(),
                dgv_Ltable["L_Table_desc", e.RowIndex].Value.ToString(), "L", dgv_Ltable["C_LCompareTable", e.RowIndex].Value.ToString(), dgv_Ltable["lsTable_position", e.RowIndex].Value.ToString());
            if (f_edit.ShowDialog() == DialogResult.OK)
            {
                LoadBaseTable();
            }
        }

        private void dgv_Ztable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                if (MessageBox.Show("确定要删除此表吗？", "删除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                    == DialogResult.OK)
                {
                    //获取连接信息
                    string ls_sql = "delete from IF_Table_infor where Table_Name = '"
                        + ((DataGridView)sender)[0, e.RowIndex].Value.ToString() + "'";
                    AccessDBop.SQLExecute(ls_sql);
                    ls_sql = "delete from IF_field_infor where Table_Name = '"
                        + ((DataGridView)sender)[0, e.RowIndex].Value.ToString() + "'";
                    AccessDBop.SQLExecute(ls_sql);
                    LoadBaseTable();
                }
            }
        }

        private void b_addCompareTable_Click(object sender, EventArgs e)
        {
            FrmCompareTable fxt = new FrmCompareTable();
            if (fxt.ShowDialog() == DialogResult.OK)
            {
                LoadCompareTable();
            }
        }

        private void dgv_CompareTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                if (MessageBox.Show("确定要删除此表吗？", "删除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                 == DialogResult.OK)
                {
                    string ls_sql = "delete from IF_Table_Compare where TableName = '"
                                    + ((DataGridView)sender)[0, e.RowIndex].Value.ToString() + "'";
                    if (AccessDBop.SQLExecute(ls_sql))
                    {
                        MessageBox.Show("删除成功");
                        //初始化数据表关系设置
                        LoadCompareTable();
                    }
                    else
                    {
                        MessageBox.Show("删除失败");
                    }
                }
            }
        }

        private void dgv_CompareTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            string ls_tableName = ((DataGridView)sender)[0, e.RowIndex].Value.ToString();
            FrmCompareTable fxt = new FrmCompareTable(ls_tableName);
            if (fxt.ShowDialog() == DialogResult.OK)
            {
                LoadCompareTable();
            }
        }

        private void b_addTask_Click(object sender, EventArgs e)
        {
            FrmTableTranse ftt = new FrmTableTranse();
            if (ftt.ShowDialog() == DialogResult.OK)
            {
                //初始化任务
                LoadTask();
            }
        }

        private void d_Task_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            if (d_Task["Task_id", e.RowIndex].Value.ToString() == "10000000000000")
            {
                FrmSetPhoto fsp = new FrmSetPhoto();
                if (fsp.ShowDialog() == DialogResult.OK)
                {
                    //初始化任务
                    LoadTask();
                }
                return;
            }
            FrmTableTranse ftt = new FrmTableTranse(d_Task["Task_id", e.RowIndex].Value.ToString());
            if (ftt.ShowDialog() == DialogResult.OK)
            {
                //初始化任务
                LoadTask();
            }
        }

        private void d_Task_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            if (e.ColumnIndex == 2)
            {
                //if (d_Task["Task_id", e.RowIndex].Value.ToString() == "10000000000000")
                //{
                //    MessageBox.Show("特定任务禁止删除");
                //    return;
                //}
                if (MessageBox.Show("确定要删除此任务吗？", "删除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                  == DialogResult.OK)
                {
                    //获取连接信息
                    string ls_sql = "delete from IF_Task where Task_id = '"
                        + d_Task["Task_id", e.RowIndex].Value.ToString() + "'";
                    AccessDBop.SQLExecute(ls_sql);
                    //初始化任务
                    LoadTask();
                }
            }
            if (e.ColumnIndex == 3)
            {
                //if (MessageBox.Show("确定要执行此任务吗？", "执行提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                //  == DialogResult.OK)
                //{
                //    //获取连接信息
                //    if (TaskManager.ExecTask(d_Task["Task_id", e.RowIndex].Value.ToString()))
                //        MessageBox.Show("执行成功");
                //    else
                //        MessageBox.Show("执行失败");
                //}
            }
        }

        private void InitStatusStrip()
        {
            Project.TaskFinalDealFailSum = 0;
            Project.TaskFinalDealSuccessSum = 0;
            Project.TaskSuccessSum = 0;
            Project.TaskFailSum = 0;
            tsslFinalDealFail.Text = "0";
            tsslFinalDealSuccess.Text = "0";
            tsslTaskFail.Text = "0";
            tsslTaskSuccess.Text = "0";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string[] arrInfo = new string[4];
            button6.Tag = (button6.Tag.ToString() == "启   动") ? "停   止" : "启   动";
            tsl_schedule_status.Text = (button6.Tag.ToString() == "启   动") ? "stop" : "start";
            if (!timer1.Enabled)
            {
                ScheduleManager.Get_ScheduleList();
                tsslRunTime.Text = ApplicationSetting.Translate("start date") + " ";
                tsslRunTimeValue.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                timerRefreshSumData.Enabled = true;
                arrInfo[0] = ApplicationSetting.Translate("start schedule");
                arrInfo[1] = "";
                arrInfo[2] = "0";
                dgvSchedule.Columns["C7"].Visible = false;
                Miscellaneous.Write2List(arrInfo);
            }
            else
            {
                tsslRunTime.Text = ApplicationSetting.Translate("stop date");
                tsslRunTimeValue.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                timerRefreshSumData.Enabled = false;
                InitStatusStrip();//清状态栏
                arrInfo[0] = ApplicationSetting.Translate("stop schedule");
                arrInfo[1] = "";
                arrInfo[2] = "0";
                dgvSchedule.Columns["C7"].Visible = true;
                Miscellaneous.Write2List(arrInfo);
            }

            if (button6.Tag.ToString() == "启   动")
                (button6).BackgroundImage = Image.FromFile("images\\start_focus.png");
            else
                (button6).BackgroundImage = Image.FromFile("images\\stop_focus.png");
            timer1.Enabled = !timer1.Enabled;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ScheduleManager.execute_schedule();
        }

        private void b_paly_MouseLeave(object sender, EventArgs e)
        {
            if (!((Button)sender).Focused)
            {
                ((Button)sender).ForeColor = Color.Black;
                ((Button)sender).BackgroundImage = Image.FromFile("images\\normal.png");
            }
        }

        private void b_paly_Leave(object sender, EventArgs e)
        {
            //if (!((Button)sender).Focused)
            //{
            //    ((Button)sender).ForeColor = Color.Black;
            //    ((Button)sender).BackgroundImage = Image.FromFile("images\\normal.png");
            //}
        }

        private void b_paly_MouseEnter(object sender, EventArgs e)
        {
            ((Button)sender).ForeColor = Color.White;
            ((Button)sender).BackgroundImage = Image.FromFile("images\\push.png");
        }

        private void button6_MouseEnter(object sender, EventArgs e)
        {
            if (button6.Tag.ToString() == "启   动")
                ((Button)sender).BackgroundImage = Image.FromFile("images\\start_focus.png");
            else
                ((Button)sender).BackgroundImage = Image.FromFile("images\\stop_focus.png");
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            if (!((Button)sender).Focused)
            {
                if (button6.Tag.ToString() == "启   动")
                    ((Button)sender).BackgroundImage = Image.FromFile("images\\start.png");
                else
                    ((Button)sender).BackgroundImage = Image.FromFile("images\\stop.png");
            }
        }

        private void button6_Leave(object sender, EventArgs e)
        {
            if (!((Button)sender).Focused)
            {
                if (button6.Tag.ToString() == "启   动")
                    ((Button)sender).BackgroundImage = Image.FromFile("images\\start.png");
                else
                    ((Button)sender).BackgroundImage = Image.FromFile("images\\stop.png");
            }
        }

        private void test_MouseEnter(object sender, EventArgs e)
        {
            ((Button)sender).ForeColor = Color.White;
            ((Button)sender).Image = Image.FromFile("images\\button2.png");
        }

        private void test_MouseLeave(object sender, EventArgs e)
        {
            if (!((Button)sender).Focused)
            {
                ((Button)sender).ForeColor = Color.Black;
                ((Button)sender).Image = Image.FromFile("images\\button1.png");
            }
        }

        private void test_Leave(object sender, EventArgs e)
        {
            if (!((Button)sender).Focused)
            {
                ((Button)sender).ForeColor = Color.Black;
                ((Button)sender).Image = Image.FromFile("images\\button1.png");
            }
        }

        private void r_nt_CheckedChanged(object sender, EventArgs e)
        {
            if (r_nt.Checked)
            {
                p_selectmode.Enabled = false;
            }
            else
            {
                p_selectmode.Enabled = true;
            }
        }

        private void r_exnt_CheckedChanged(object sender, EventArgs e)
        {
            if (r_exnt.Checked)
            {
                p_exselectmode.Enabled = false;
            }
            else
            {
                p_exselectmode.Enabled = true;
            }
        }

        private void show_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isLoad) return;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void hide_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isLoad) return;
            this.Hide();
        }

        private void close_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 读取注册表
        /// </summary>
        private void LoadSettingsFromRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\ScmDataInterFace\\weds");
            try
            {
                reg = (string)key.GetValue("reg", "");
            }
            catch { }
        }

        /// <summary>
        /// 保存注册表
        /// </summary>
        private void SaveSettingsToRegistry(string value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\ScmDataInterFace\\weds");
                key.SetValue("reg", value);
            }
            catch { }
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void b_photo_Click(object sender, EventArgs e)
        {
            FrmSetPhoto fsp = new FrmSetPhoto();
            if (fsp.ShowDialog() == DialogResult.OK)
            {
                //初始化任务
                LoadTask();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            MessageBox.Show(list.ToString());
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isQuery)
            {
                e.Cancel = false;
                return;
            }
            if (MessageBox.Show(ApplicationSetting.Translate("are you sure to exit?"), ApplicationSetting.Translate("confirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }


        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\脚本"))
                fileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath + @"\脚本";
            else
                fileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            fileDialog.Filter = "txt files (*.txt)|*.txt|sql files (*.sql)|*.sql";
            fileDialog.FilterIndex = 2;
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = fileDialog.FileName;
                if (fileName.Contains("-"))
                {
                    MessageBox.Show("文件路径中包含特殊符号'-',操作失败!");
                    return;
                }
                _SqlFileName = fileName;
                FileStream fs = new FileStream(_SqlFileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GB2312"));
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                rtbSql.Text = sr.ReadToEnd();
                sr.Close();
                fs.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_SqlFileName != string.Empty)
            {
                try
                {
                    FileStream fs = new FileStream(_SqlFileName, FileMode.Open, FileAccess.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
                    fs.SetLength(0);//文件清空
                    sw.Write(rtbSql.Text);
                    sw.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("操作失败:" + ex.Message);
                    return;
                }
                MessageBox.Show("操作成功!");
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (rtbSql.Text.Trim().Equals(""))
            {
                return;
            }
            string db_Type = string.Empty;
            if (rbOur.Checked)
            {
                if (Project.DB_Type.ToLower().Equals("sql server"))
                {
                    db_Type = "sql server";
                }
                else if (Project.DB_Type.ToLower().Equals("oracle"))
                {
                    db_Type = "oracle";
                }
                else
                {
                    db_Type = "mysql";
                }
            }
            else
            {
                if (Project.TheirDB_Type.ToLower().Equals("sql server"))
                {
                    db_Type = "sql server";
                }
                else if (Project.TheirDB_Type.ToLower().Equals("oracle"))
                {
                    db_Type = "oracle";
                }
                else
                {
                    db_Type = "mysql";
                }
            }
            if (db_Type == "sql server")
            {
                string uid = rbOur.Checked ? Project.Uid : Project.theirUid;
                string password = rbOur.Checked ? Project.Pwd : Project.theirPwd;
                string severName = rbOur.Checked ? Project.SeverName : Project.theirSeverName;
                string dbName = rbOur.Checked ? Project.DbName : Project.theirDbName;
                string sPara = String.Format("-U {0} -P {1} -S {2} -d {3} -i {4}", uid, password, severName, dbName, _SqlFileName);

                // 调用sqlcmd
                ProcessStartInfo info = new ProcessStartInfo("sqlcmd", sPara);
                //禁用OS Shell
                info.UseShellExecute = false;
                //禁止弹出新窗口
                info.CreateNoWindow = true;
                //隐藏windows style
                info.WindowStyle = ProcessWindowStyle.Hidden;
                //标准输出
                info.RedirectStandardOutput = true;

                Process pro = new Process();
                pro.StartInfo = info;
                //启动进程
                pro.Start();
                StreamReader sr = pro.StandardOutput;
                string s = sr.ReadToEnd();
                sr.Close();
                s = s.Trim().Equals("") ? "执行成功" : s;
                rtbInfo.AppendText(s + "\r\n");
            }
            else if (db_Type == "oracle")
            {
                string sResult = OracleDBop.StrSQLExecute(rbOur.Checked ? Project.DB_Connection : Project.TheirDB_Connection, rtbInfo.Text);
                sResult = sResult.Equals("") ? "执行成功" : sResult;
                rtbInfo.AppendText(sResult + "\r\n");
            }
            else
            {
                string sResult = MySqlDBop.StrSQLExecute(rbOur.Checked ? Project.DB_Connection : Project.TheirDB_Connection, rtbInfo.Text);
                sResult = sResult.Equals("") ? "执行成功" : sResult;
                rtbInfo.AppendText(sResult + "\r\n");
            }
        }

        private void timerRefreshInterface_Tick(object sender, EventArgs e)
        {
            tsslTaskSuccess.Text = Project.TaskSuccessSum.ToString();
            tsslTaskFail.Text = Project.TaskFailSum.ToString();
            tsslFinalDealSuccess.Text = Project.TaskFinalDealSuccessSum.ToString();
            tsslFinalDealFail.Text = Project.TaskFinalDealFailSum.ToString();
        }

        private void tsslTaskSuccess_Click(object sender, EventArgs e)
        {
            try
            {
                string ls_path = Application.StartupPath + @"\" + DateTime.Now.ToString("yyyyMM");
                ls_path = ls_path.Replace(@"\\", @"\");
                string ls_file = ls_path + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                ls_file = ls_file.Replace(@"\\", @"\");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = ls_file;
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            if (e.ColumnIndex == 0)
            {
                if (MessageBox.Show(ApplicationSetting.Translate("Are you sure to execute the schedule?"), ApplicationSetting.Translate("prompt"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string AgainstReason;
                    string command = dgvSchedule["command", e.RowIndex].Value.ToString();
                    string schedule_name = dgvSchedule["c2", e.RowIndex].Value.ToString();
                    if (ScheduleManager.IsPermitExcute(command, out AgainstReason))
                    {
                        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                        //ScheduleManager.schedule_running.Add(command);
                        DateTime beforDT = System.DateTime.Now;
                        string sResult = ScheduleManager.execute_job_NoThread(schedule_name, command);
                        DateTime afterDT = System.DateTime.Now;
                        TimeSpan ts = afterDT.Subtract(beforDT);
                        this.Cursor = System.Windows.Forms.Cursors.Default;
                        sResult = sResult == string.Empty ? ApplicationSetting.Translate("run success") : ApplicationSetting.Translate("run fail");
                        //sResult += " 耗时:" + ts.TotalSeconds + "秒";
                        sResult += " " + ApplicationSetting.Translate("cost") + " " + ts.TotalSeconds + " " + ApplicationSetting.Translate("second");
                        MessageBox.Show(sResult);
                    }
                    else
                    {
                        string[] arrInfo = new string[4];
                        arrInfo[0] = AgainstReason;// "【" + schedule_name + "】同名调度或有互斥调度正在执行!";
                        arrInfo[1] = "";
                        arrInfo[2] = "1";
                        arrInfo[3] = schedule_name;
                        Miscellaneous.Write2List(arrInfo);
                        //MessageBox.Show("执行失败,原因请参考日志");
                        MessageBox.Show(ApplicationSetting.Translate("run fail") + "," + ApplicationSetting.Translate("Please see the log"));
                    }
                }
            }
            else if (e.ColumnIndex == 1)
            {
                if (MessageBox.Show(ApplicationSetting.Translate("Are you sure to delete the schedule"), ApplicationSetting.Translate("prompt"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    DataRowView drv = dgvSchedule.SelectedRows[0].DataBoundItem as DataRowView;
                    drv.Row.Delete();
                }
            }
        }

        TabPage NextPage(TabPage nowPage)
        {
            if (nowPage == tpConfigBase)
            {
                return tpTask;
            }
            //else if (nowPage == tpDBConnection)
            //{
            //    return tpRunSql;
            //}
            //else if (nowPage == tpRunSql)
            //{
            //    return tpTask;
            //}
            //else if (nowPage == tpBaseTable)
            //{
            //    return tpCompareTable;
            //}
            else if (nowPage == tpTask)
            {
                return tpCompareTable;
            }
            else if (nowPage == tpCompareTable)
            {
                return tpSchedule;
            }
            else
            {
                return tpSchedule;
            }
        }

        TabPage PrePage(TabPage nowPage)
        {
            if (nowPage == tpTask)
            {
                return tpConfigBase;
            }
            //else if (nowPage == tpRunSql)
            //{
            //    return tpDBConnection;
            //}
            //else if (nowPage == tpBaseTable)
            //{
            //    return tpRunSql;
            //}
            //else if (nowPage == tpTask)
            //{
            //    return tpRunSql;
            //}
            else if (nowPage == tpCompareTable)
            {
                return tpTask;
            }
            else if (nowPage == tpSchedule)
            {
                return tpCompareTable;
            }
            else
            {
                return tpConfigBase;
            }
        }



        private void button1_Click_2(object sender, EventArgs e)
        {
            //if
            //(
            //    ((Project.ConfigBase_type == 1)) ||
            //    (tb_setinterface.SelectedTab == tpSchedule)
            // )
            //{
            //    rbNormalModel.Visible = true;
            //    rbNormalModel.Checked = true;
            //}
            //if (tb_setinterface.SelectedTab == tpSchedule)
            //{
            //    rbNormalModel.Visible = true;
            //    rbNormalModel.Checked = true;
            //    return;
            //}
            //tb_setinterface.SelectedTab = NextPage(tb_setinterface.SelectedTab);
            //setButtonBackgroudImage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tb_setinterface.SelectedTab = PrePage(tb_setinterface.SelectedTab);
            setButtonBackgroudImage();
        }

        private void button8_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 设置界面类型
        /// </summary>
        /// <param name="ModelType">0正常模式1向导模式</param>
        void SetInterfaceModel(bool IsNomal)
        {
            //splitContainer2.Panel1Collapsed = !IsNomal;
            //splitContainer3.Panel2Collapsed = IsNomal;
            //statusStrip1.Visible = IsNomal;
            //if (!IsNomal)
            //{
            //    setButtonBackgroudImage();
            //}

            ////d_Task.Columns["c_del"].Visible = IsNomal;
            ////d_Task.Columns["trans"].Visible = IsNomal;
            ////dgvSchedule.Columns["c7"].Visible = IsNomal;
            ////dgvSchedule.Columns["c8"].Visible = IsNomal;
        }

        public static void SetAppConfig(string appKey, string appValue)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");
            var xNode = xDoc.SelectSingleNode("//appSettings");
            var xElem = (XmlElement)xNode.SelectSingleNode("//add[@key='" + appKey + "']");
            if (xElem != null) xElem.SetAttribute("value", appValue);
            else
            {
                var xNewElem = xDoc.CreateElement("add");
                xNewElem.SetAttribute("key", appKey);
                xNewElem.SetAttribute("value", appValue);
                xNode.AppendChild(xNewElem);
            }
            xDoc.Save(System.Windows.Forms.Application.ExecutablePath + ".config");
        }

        public static void SetAppConfig(string sectionName, string appKey, string appValue)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");
            var xNode = xDoc.SelectSingleNode("//" + sectionName);
            var xElem = (XmlElement)xNode.SelectSingleNode("//add[@key='" + appKey + "']");
            if (xElem != null)
            {
                xElem.SetAttribute("value", appValue);
            }
            else
            {
                xNode.RemoveAll();
                var xNewElem = xDoc.CreateElement("add");
                xNewElem.SetAttribute("key", appKey);
                xNewElem.SetAttribute("value", appValue);
                xNode.AppendChild(xNewElem);
            }
            xDoc.Save(System.Windows.Forms.Application.ExecutablePath + ".config");
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            //if (rbGuideModel.Checked)
            //{
            //    //设置界面
            //    SetInterfaceModel(false);
            //    //按钮可见性
            //    //rbNormalModel.Visible = false;
            //    //默认第一页显示配置库
            //    b_setinterface(btConfigBase, e);
            //    //保存配置
            //    SetAppConfig("interfaceModel", "guide");
            //}
            //else
            //{
            //    SetInterfaceModel(true);
            //    rbNormalModel.Visible = true;

            //    b_setinterface(b_schedule, e);
            //    //if (button6.Tag.ToString() == "启   动")
            //    //{
            //    //    button6_Click(sender, e);
            //    //}
            //    SetAppConfig("interfaceModel", "nomal");
            //}
        }

        private void cmbBase_SelectedValueChanged(object sender, EventArgs e)
        {

        }



        void LoadRunSql()
        {
            rbOur.Text = "在" + Project.DB_Alias + "执行";
            rbTheir.Text = "在" + Project.theirDB_Alias + "执行";
        }

        void init()
        {
            //初始化数据库配置
            LoadDBConnection();
            //初始化执行SQL配置
            LoadRunSql();
            //初始化基础表配置
            LoadBaseTable();
            //初始化数据表关系设置
            LoadCompareTable();

            //初始化任务
            LoadTask();
            //初始化调度
            LoadSchedule();
            //设置表格样式
            SetDataGridStyle();
            //设置界面样式
            SetInterfaceStyle();
            //b_env.Focus();
        }
        /// <summary>
        /// 0成功1能找到基础配置库2基础库和标准库都找不到3复制文件出错
        /// </summary>
        /// <returns></returns>
        int LoadBaseMdb()
        {
            int iResult = 0;
            try
            {
                string DirName = cmbTheirAlias.Text + "-" + cmbOurAlias.Text;
                string OrignFileName = Application.StartupPath + @"\config\" + DirName + @"\ScmDataInterFace.mdb";
                if (!File.Exists(OrignFileName))
                {
                    iResult = 1;
                    OrignFileName = Application.StartupPath + @"\config\" + cmbOurAlias.Text + @"\ScmDataInterFace.mdb";
                    if (!File.Exists(OrignFileName))
                    {
                        iResult = 2;
                    }
                }
                string destFileName = Application.StartupPath + @"\ScmDataInterFace.mdb";
                File.Copy(OrignFileName, destFileName, true);
            }
            catch (Exception)
            {
                iResult = 3;
            }
            return iResult;
        }

        string DeleteInvalidItem(string contain)
        {
            string sResult = string.Empty;
            if (contain == string.Empty)
            {
                return sResult;
            }
            string sql_task = "delete from IF_Task where Task_Name like '%{0}%'";
            string sql_schedule = "delete from PT_Schedule where schedule_Name like '%{0}%'";
            List<string> ls_task = new List<string>();
            List<string> ls_schedule = new List<string>();
            string[] containArr = contain.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in containArr)
            {
                ls_task.Add(string.Format(sql_task, s));
                ls_schedule.Add(string.Format(sql_schedule, s));
            }
            //bool bResult = AccessDBop.SQLExecute(ls_task.ToArray());
            //if (!bResult)
            //{
            //    sResult = "配置任务数据失败!";
            //    return sResult;
            //}
            bool bResult = AccessDBop.SQLExecute(ls_schedule.ToArray());
            if (!bResult)
            {
                sResult = "配置调度数据失败!";
                return sResult;
            }
            return sResult;
        }

        string GetDeleteItem()
        {
            string s = string.Empty;
            for (int i = 0; i < dgv_configBase.Rows.Count - 1; i++)
            {
                string isChecked = dgv_configBase.Rows[i].Cells["isChecked"].Value.ToString();
                string sValue = dgv_configBase.Rows[i].Cells["synItem"].Value.ToString();
                if (isChecked == "N")
                {
                    s += sValue + ",";
                }
            }
            return s;
        }


        private void button10_Click(object sender, EventArgs e)
        {

        }

        void setButtonBackgroudImage()
        {
            //if (tb_setinterface.SelectedTab == tpConfigBase)
            //{
            //    btPrior.BackgroundImage = Image.FromFile("images\\forword_noavailable.png");
            //    btPrior.Enabled = false;
            //    if (lsTabPageAccess.IndexOf("tpDBConnection") > -1)
            //    {
            //        btNext.BackgroundImage = Image.FromFile("images\\next_press.png");
            //        btNext.Enabled = true;
            //    }
            //    else
            //    {
            //        btNext.BackgroundImage = Image.FromFile("images\\next_noavailable.png");
            //        btNext.Enabled = false;
            //    }
            //}
            //else if (tb_setinterface.SelectedTab == tpSchedule)
            //{
            //    btPrior.BackgroundImage = Image.FromFile("images\\forword_press.png");
            //    btPrior.Enabled = true;
            //    btNext.BackgroundImage = Image.FromFile("images\\finish_press.png");
            //    btNext.Enabled = true;
            //}
            //else if (tb_setinterface.SelectedTab == tpDBConnection)
            //{
            //    btPrior.BackgroundImage = Image.FromFile("images\\forword_press.png");
            //    btPrior.Enabled = true;
            //    if (lsTabPageAccess.IndexOf("tpRunSql") > -1)
            //    {
            //        btNext.BackgroundImage = Image.FromFile("images\\next_press.png");
            //        btNext.Enabled = true;
            //    }
            //    else
            //    {
            //        btNext.BackgroundImage = Image.FromFile("images\\next_noavailable.png");
            //        btNext.Enabled = false;
            //    }
            //}
            //else if (tb_setinterface.SelectedTab == tpRunSql)
            //{
            //    btPrior.BackgroundImage = Image.FromFile("images\\forword_press.png");
            //    btPrior.Enabled = true;
            //    btNext.BackgroundImage = Image.FromFile("images\\next_press.png");
            //    btNext.Enabled = true;
            //}
            //else if (tb_setinterface.SelectedTab == tpTask)
            //{
            //    btPrior.BackgroundImage = Image.FromFile("images\\forword_press.png");
            //    btPrior.Enabled = true;
            //    btNext.BackgroundImage = Image.FromFile("images\\next_press.png");
            //    btNext.Enabled = true;
            //}
        }

        void GiveAccess(string accessTabPage)
        {
            //if (lsTabPageAccess.IndexOf(accessTabPage) == -1)
            //{
            //    lsTabPageAccess.Add(accessTabPage);
            //    btNext.BackgroundImage = Image.FromFile("images\\next_press.png");
            //    btNext.Enabled = true;
            //}
        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private List<string> LoadOurAliasFromMdb(string source)
        {
            List<string> ls = new List<string>();
            DataTable dt = new DataTable();
            string sql = "select distinct ourAlias from IF_ConfigBase where theirAlias = '{0}'";
            sql = string.Format(sql, source);
            bool bResult = AccessDBop.SQLSelect_configBase(sql, ref dt);
            if (bResult)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ls.Add(dt.Rows[i][0].ToString());
                }
            }
            return ls;
        }

        private List<string> LoadOurAliasFromDirectory(string source)
        {
            List<string> lsResult = new List<string>();
            try
            {
                string[] dirArr = Directory.GetDirectories(Application.StartupPath + @"\config");
                List<string> ls = new List<string>(dirArr);
                for (int i = 0; i < ls.Count; i++)
                {
                    string[] dirNameArr = ls[i].Trim().Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    string sName = dirNameArr[dirNameArr.Length - 1];
                    if (sName.Contains(source))
                    {
                        string[] nameArr = sName.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        lsResult.Add(nameArr[1].ToString());
                    }
                }
            }
            catch (Exception)
            {

            }
            return lsResult;
        }

        private void cmbTheirAlias_TextChanged(object sender, EventArgs e)
        {
            if (!isLoad_config)
            {
                return;
            }
            if (cmbTheirAlias.Text.Trim().Equals(""))
            {
                return;
            }
            //LoadOurAliasFromAppcfg();
            List<string> ls = LoadOurAliasFromDirectory(cmbTheirAlias.Text);
            if (ls.Count > 0)
            {
                cmbOurAlias.DataSource = ls;
                cmbOurAlias.SelectedIndex = 0;
                LoadConfigItem(cmbTheirAlias.Text, cmbOurAlias.Text);
            }
            else
            {
                LoadOurAliasFromAppcfg();
            }

            //lbContain.Items.Clear();
            //lbSelectContain.Items.Clear();
            //try
            //{
            //    NameValueCollection baseConfig = (NameValueCollection)ConfigurationManager.GetSection("cfgBaseContain");
            //    string contain = baseConfig[cmbTheirConfigBase.Text + "-" + cmbOurConfigBase.Text];
            //    string[] containArr = contain.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (string s in containArr)
            //    {
            //        lbContain.Items.Add(s);
            //        lbSelectContain.Items.Add(s);
            //    }
            //}
            //catch (Exception)
            //{

            //}
        }


        bool RegConifigBase(string theirAlias, string ourAlias)
        {
            string fileName = Application.StartupPath + @"\ScmDataInterFace.mdb";
            if (!File.Exists(fileName))
            {
                return false;
            }
            string sql = "update IF_PHOTO set my = '" + Miscellaneous.md35(theirAlias + ourAlias) + "'";
            return AccessDBop.SQLExecute(sql);
        }


        private bool SetValidConfigBase(string theirAlias, string ourAlias)
        {
            List<string> ls = new List<string>();
            string ls_sql = "update IF_ConfigBase set isValid = 'N'";
            ls.Add(ls_sql);
            ls_sql = "update IF_ConfigBase set isValid = 'Y' where theirAlias = '{0}' and ourAlias = '{1}'";
            ls_sql = string.Format(ls_sql, theirAlias, ourAlias);
            ls.Add(ls_sql);
            string[] sqlArr = ls.ToArray(); ;
            return AccessDBop.SQLExecute_ConfigBase(sqlArr);
        }

        private string DeleteConfigBase()
        {

            try
            {
                string ls_sql = "delete from IF_ConfigBase where theirAlias = '{0}'";
                ls_sql = string.Format(ls_sql, cmbTheirAlias.Text);
                bool bResult = AccessDBop.SQLExecute_ConfigBase(new string[] { ls_sql });
                if (!bResult)
                {
                    return "删除失败!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        private string SaveConfigBase()
        {
            try
            {
                DataTable dt = (DataTable)dgv_configBase.DataSource;
                if (dt.Rows.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["synItem"] = "NULL";
                    dt.Rows.Add(dr);
                }
                foreach (DataRow dr in dt.Rows)
                {
                    dr["theirAlias"] = cmbTheirAlias.Text;
                    dr["ourAlias"] = cmbOurAlias.Text;
                    //dr["isChecked"] = "Y";
                }
                string ls_sql = "select * from IF_ConfigBase";// where theirAlias = '{0}' and ourAlias = '{1}'";
                ls_sql = string.Format(ls_sql, cmbTheirAlias.Text, cmbOurAlias.Text);
                bool bResult = AccessDBop.SQLUpdate_configBase(ls_sql, ref dt);
                if (!bResult)
                {
                    return "数据更新失败!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        private string CheckValid()
        {
            if (cmbTheirAlias.Text.Trim().Equals(""))
            {
                return "源位置不能为空!";
            }
            else if (cmbOurAlias.Text.Trim().Equals(""))
            {
                return "目标位置不能为空!";
            }
            else
            {
                return "";
            }
        }

        string save()
        {
            string sResult = CheckValid();
            if (sResult != "")
            {
                return sResult;
            }
            sResult = SaveConfigBase();
            if (sResult != "")
            {
                return sResult;
            }
            return sResult;
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            string sResult = save();
            if (sResult != "")
            {
                MessageBox.Show(sResult);
            }
            else
            {
                MessageBox.Show("保存成功!");
            }
        }


        void SetOperaterAuthority(bool bResult)
        {
            ////配置库界面
            //btSave.Enabled = !bResult;
            //btDel.Enabled = !bResult;
            //dgv_configBase.ReadOnly = bResult;
            //cmbTheirAlias.Enabled = !bResult;
            //cmbOurAlias.Enabled = !bResult;
            ////执行脚本模块
            //btSaveSqlFile.Enabled = !bResult;
            //rtbSql.ReadOnly = bResult;
            ////基础表结构
            //b_addZtable.Enabled = !bResult;
            //b_addLtable.Enabled = !bResult;
            //dgv_Ztable.Columns["Column2"].Visible = !bResult;
            //dgv_Ltable.Columns["Column3"].Visible = !bResult;

            ////任务管理
            //b_addTask.Enabled = !bResult;
            //b_photo.Enabled = !bResult;
            //bool bo = d_Task.Columns["c_del"].Visible;
            //d_Task.Columns["c_del"].Visible = !bResult;
            //d_Task.Columns["trans"].Visible = !bResult;
            ////表关系设置
            //b_addCompareTable.Enabled = !bResult;
            //dgv_CompareTable.Columns["Column1"].Visible = !bResult;

            NameValueCollection nvcFormShow = (NameValueCollection)ConfigurationManager.GetSection("FormShow");
            string sShow = string.Empty;
            sShow = nvcFormShow["fEnv"];
            if (sShow == "0")
            {
                b_env.Visible = false;
            }
            sShow = nvcFormShow["fTask"];
            if (sShow == "0")
            {
                b_task.Visible = false;
            }
            sShow = nvcFormShow["fExcuteSql"];
            if (sShow == "0")
            {
                btExcuteSql.Visible = false;
            }

            btConfigBase.Visible = !bResult;
            b_tablestruct.Visible = !bResult;
            //b_task.Visible = !bResult;
            b_tablenative.Visible = !bResult;
            //rbNormalModel.Visible = !bResult;
            //rbGuideModel.Visible = !bResult;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 从内存列表取提示信息
        /// </summary>
        void ReadInfo()
        {
            Thread ThreadGetInfo = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if (!Miscellaneous.IsLocked)
                    {
                        if (Miscellaneous.lsShowInfo.Count > 0)
                        {
                            string[] arrInfo = (string[])Miscellaneous.lsShowInfo[0];
                            SetInfo(arrInfo);
                            if (Project.IsInsertLogToDb)
                            {
                                string sResult = TaskManager.InsertLog(arrInfo);
                                if (sResult != "")
                                {
                                    string[] arrTmp = new string[5];
                                    arrTmp[0] = "日志写入数据库失败!";
                                    arrTmp[1] = sResult;
                                    arrTmp[2] = "2";
                                    arrTmp[3] = "";
                                    arrTmp[4] = DateTime.Now.ToString();
                                    SetInfo(arrTmp);
                                }
                            }
                            Miscellaneous.lsShowInfo.RemoveAt(0);
                        }
                    }
                    Thread.Sleep(5);
                }
            }));
            ThreadGetInfo.IsBackground = true;
            ThreadGetInfo.Name = "循环读日志信息";
            ThreadGetInfo.Start();
        }
        /// <summary>
        /// 将提示信息显示到rtbShowInfo
        /// </summary>
        /// <param name="lsInfo"></param>
        void SetInfo(string[] arrInfo)
        {
            lvShowInfo.Invoke(new EventHandler(delegate
            {
                this.lvShowInfo.BeginUpdate();
                int imageIndex = int.Parse(arrInfo[2]);
                string sj = arrInfo[4];
                string taskName = arrInfo[3];
                string operate = arrInfo[0];
                string info = arrInfo[1];

                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = imageIndex;
                lvi.SubItems.Add(sj);
                lvi.SubItems.Add(taskName);
                lvi.SubItems.Add(operate);
                lvi.SubItems.Add(info);
                if (info.Contains("&lock&"))
                {
                    ConfigHelper.UpdateAppConfig("isLock", "1");
                    if (timer1.Enabled)
                    {
                        EventArgs ea = new EventArgs();
                        button6_Click(button6, ea);
                    }
                    if (Project.login != "admin")
                    {
                        tableLayoutPanel6.Enabled = false;
                        button6.Enabled = false;
                    }
                    else
                    {
                        lvi.BackColor = Color.Red;
                    }
                }
                else if (info.Contains("&unlock&"))
                {
                    ConfigHelper.UpdateAppConfig("isLock", "0");
                }
                this.lvShowInfo.Items.Add(lvi);
                this.lvShowInfo.EndUpdate();
                Miscellaneous.Write2Log(sj + " " + taskName + " " + operate + info);
            }));
        }


        private void cmbOurAlias_TextChanged(object sender, EventArgs e)
        {
            if (!isLoad_config)
            {
                return;
            }
            string theirAlias = cmbTheirAlias.Text;
            string ourAlias = cmbOurAlias.Text;
            LoadConfigItem(theirAlias, ourAlias);
        }

        private void button1_Click_4(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除吗?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string sResult = CheckValid();
            sResult = DeleteConfigBase();
            if (sResult != "")
            {
                MessageBox.Show(sResult);
            }
            else
            {
                MessageBox.Show("删除成功!");
                LoadTheirAliasFromMdb();
            }
        }

        private void lvShowInfo_DoubleClick(object sender, EventArgs e)
        {
            if (lvShowInfo.SelectedItems.Count == 0) return;
            ShowInfo frm = new ShowInfo(lvShowInfo.SelectedItems[0].SubItems[3].Text + "\r\n" + lvShowInfo.SelectedItems[0].SubItems[4].Text);
            frm.ShowDialog();
        }

        private void timerRefreshListView_Tick(object sender, EventArgs e)
        {
            if (lvShowInfo.Items.Count == 0)
            {
                return;
            }
            if (lvShowInfo.Items.Count > 100000)
            {
                this.lvShowInfo.Items.RemoveAt(0);
            }
            int LastIndex = lvShowInfo.Items.Count - 1;
            lvShowInfo.Items[LastIndex].Focused = true;
            lvShowInfo.Items[LastIndex].EnsureVisible();
        }

        private void lvShowInfo_Click(object sender, EventArgs e)
        {
            timerRefreshListView.Interval = 60000;
        }

        private void 取备份ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("重新配置库将会导致现有配置丢失,您确认要这样做吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            //先保存信息
            string sResult = save();
            if (sResult != "")
            {
                MessageBox.Show(sResult);
                return;
            }

            //取配置库
            int iResult = LoadBaseMdb();
            if (iResult > 1)
            {
                MessageBox.Show("未找到任何可用配置库,配置失败!");
                return;
            }

            string deleteItem = GetDeleteItem();
            //删除用户未选的项
            sResult = DeleteInvalidItem(deleteItem);
            if (sResult != "")
            {
                MessageBox.Show(sResult);
                return;
            }

            string theirAlias = cmbTheirAlias.Text;
            string ourAlias = cmbOurAlias.Text;
            // 保存当前所选配置库
            bool bResult = SetValidConfigBase(theirAlias, ourAlias);
            if (!bResult)
            {
                MessageBox.Show("启用配置信息失败！");
                return;
            }
            //注册配置库
            bResult = RegConifigBase(theirAlias, ourAlias);
            if (!bResult)
            {
                MessageBox.Show("注册配置库失败！");
                return;
            }

            Project.theirDB_Alias = theirAlias;
            Project.DB_Alias = ourAlias;
            LoadConfigBase();
            init();
            string sInfor = string.Empty;
            if (iResult == 0)
            {
                Project.ConfigBase_type = 1;
                sInfor = "您所使用的是标准库,在配置数据库连接后即可完成配置!";
            }
            else
            {
                sInfor = "您所使用的是基础库,请根据左边按钮从上到下顺序完成配置!";
            }
            MessageBox.Show(sInfor);
        }

        private void 从当前目录取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确认要注册该配置库吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            //先保存信息
            string sResult = save();
            if (sResult != "")
            {
                MessageBox.Show(sResult);
                return;
            }

            string deleteItem = GetDeleteItem();
            //删除用户未选的项
            sResult = DeleteInvalidItem(deleteItem);
            if (sResult != "")
            {
                MessageBox.Show(sResult);
                return;
            }

            string theirAlias = cmbTheirAlias.Text;
            string ourAlias = cmbOurAlias.Text;
            // 保存当前所选配置库
            bool bResult = SetValidConfigBase(theirAlias, ourAlias);
            if (!bResult)
            {
                MessageBox.Show("启用配置信息失败！");
                return;
            }

            //注册配置库
            bResult = RegConifigBase(cmbTheirAlias.Text, cmbOurAlias.Text);
            if (!bResult)
            {
                MessageBox.Show("注册配置库失败！");
                return;
            }
            else
            {
                MessageBox.Show("注册配置库成功！");
            }

            Project.theirDB_Alias = theirAlias;
            Project.DB_Alias = ourAlias;
            LoadConfigBase();
            init();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            lvShowInfo.BeginUpdate();
            lvShowInfo.Items.Clear();
            lvShowInfo.EndUpdate();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "mdb文件|*.mdb|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbOwnAccessAddress.Text = openFileDialog.FileName;
            }
        }

        private void button8_Click_2(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "mdb file|*.mdb|all file|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbExAccessAddress.Text = openFileDialog.FileName;
            }
        }

        private void timerExtenControl_Tick(object sender, EventArgs e)
        {
            string sql = "select cmd_content,exec_state from fin_cmd where service_id = 2";
            DataTable dt = new DataTable();
            bool bResult = SQLServerDBop.SQLSelect(Project.DB_Connection, sql, ref dt, "远程控制");
            if (bResult)
            {
                if (dt.Rows.Count > 0)
                {
                    int exec_state = int.Parse(dt.Rows[0]["exec_state"].ToString());
                    int cmd_content = int.Parse(dt.Rows[0]["cmd_content"].ToString());
                    if (exec_state == 0)
                    {
                        if ((cmd_content == 1) && (!timer1.Enabled))
                        {
                            EventArgs ea = new EventArgs();
                            button6_Click(button6, ea);
                        }
                        else if ((cmd_content == 2) && (timer1.Enabled))
                        {
                            EventArgs ea = new EventArgs();
                            button6_Click(button6, ea);
                        }
                        if ((cmd_content == 1) || ((cmd_content == 2) && (ScheduleManager.schedule_running.Count == 0)))
                        {
                            sql = "update fin_cmd set exec_state = 1";
                            string sResult = SQLServerDBop.StrSQLExecute("远程控制", Project.DB_Connection, sql);
                            if (sResult != "")
                            {
                                string[] arrInfo = new string[4];
                                arrInfo[0] = sResult;
                                arrInfo[1] = "";
                                arrInfo[2] = "2";
                                arrInfo[3] = "远程控制";
                                Miscellaneous.Write2List(arrInfo);
                            }
                        }
                    }
                }
                else
                {
                    string[] arrInfo = new string[4];
                    arrInfo[0] = "系统控制表中没有任何信息";
                    arrInfo[1] = "";
                    arrInfo[2] = "2";
                    arrInfo[3] = "远程控制";
                    Miscellaneous.Write2List(arrInfo);
                }
            }
        }
    }
}