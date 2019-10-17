using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class EditForm : Form
    {
        private string _TableName;
        private string _TableDesc;
        private string _TableType;
        private string _CompareTableName;
        private string _TablePosition;
        private bool Isload = true;

        //private string constr = "";
        private string ls_type = "";

        public EditForm(string as_TableType)
        {
            InitializeComponent();
            _TableName = "";
            _TableType = as_TableType;
            _CompareTableName = "";
        }

        public EditForm(string as_TableName, string as_TableDesc, string as_TableType, string as_CompareTableName, string as_TablePosition)
        {
            InitializeComponent();
            _TableName = as_TableName;
            _TableDesc = as_TableDesc;
            _TableType = as_TableType;
            _CompareTableName = as_CompareTableName;
            _TablePosition = as_TablePosition;
        }

        private void EditFormType_Load(object sender, EventArgs e)
        {
            try
            {
                label4.Left = 175;
                cb_CompareTable.Left = 259;
                DataGridStyle.GridDisplayStyle(dgv_columns, true);
                if (_TableName != "")
                {
                    cb_DatabaseTable.Visible = false;
                    label1.Visible = false;
                    tb_TableName.Text = _TableName;
                    tb_TableDesc.Text = _TableDesc;
                    ucbTablePosition.Text = _TablePosition;
                    ucbTablePosition.Enabled = false;
                }
                else
                {
                    cb_DatabaseTable.Visible = true;
                    label1.Visible = true;
                    ucbTablePosition.Enabled = true;
                    LoadPosition2ucbTablePosition();
                }
                if (_TableType == "Z")
                {
                    cb_CompareTable.Visible = false;
                    cb_CompareTable.Enabled = false;
                    label4.Visible = false;
                    label4.Enabled = false;
                }
                else
                {
                    cb_DatabaseTable.Visible = false;
                    label1.Visible = false;
                    cb_CompareTable.Visible = true;
                    cb_CompareTable.Enabled = true;
                    label4.Visible = true;
                    label4.Enabled = true;
                }
                RetrieveData();
                if (_TableType != "Z" && _TableName != "")
                {
                    cb_CompareTable.Enabled = false;
                    cb_CompareTable.Text =
                    ((DataTable)cb_CompareTable.DataSource).Select("Table_Name = '" + _CompareTableName + "'")[0]["Table_desc"].ToString();
                }
                Isload = false;
            }
            catch { }
        }

        void LoadPosition2ucbTablePosition()
        {
            ucbTablePosition.Items.Clear();
            ucbTablePosition.Items.Add(Project.DB_Alias);
            ucbTablePosition.Items.Add(Project.theirDB_Alias);
        }

        private void RetrieveData()
        {
            bool lb_result = false;
            //写dgv_columns
            string ls_sql = "SELECT Table_Name, Field_Name,Field_Desc, Field_Type, IsPK,CanNull,ShowIndex,IsIdentity " +
                " FROM IF_field_infor Where Table_Name ='" + _TableName + "'  order by ShowIndex";
            DataTable ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                dgv_columns.DataSource = ldt;
            }
            //写cb_CompareTable 下拉
            if (_TableType != "Z")
            {
                ls_sql = "select Table_Name,Table_desc From IF_Table_infor" +
                   " Where Table_Type = 'Z'";
                ldt = new DataTable();
                if (AccessDBop.SQLSelect(ls_sql, ref ldt))
                {
                    if (ldt.Rows.Count > 0)
                    {
                        cb_CompareTable.DisplayMember = "Table_desc";
                        cb_CompareTable.ValueMember = "Table_Name";
                        cb_CompareTable.DataSource = ldt;
                        cb_CompareTable.SelectedIndex = -1;
                        cb_CompareTable.SelectedIndex = 0;
                    }
                }
            }
            if (ucbTablePosition.Text == Project.DB_Alias)
                ls_sql = "select Db_type,Db_name,Db_server,Db_User,Db_Password From IF_DBConnectionInfor" +
                        " Where bz = 0";
            else if (ucbTablePosition.Text == Project.theirDB_Alias)
                ls_sql = "select Db_type,Db_name,Db_server,Db_User,Db_Password From IF_DBConnectionInfor" +
                    " Where bz = 1";
            else
                ls_sql = "";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                if (ldt.Rows.Count > 0)
                {
                    //constr = "";
                    ls_type = ldt.Rows[0]["Db_type"].ToString();
                    if (ls_type == "sql")
                    {
                        //constr = "server = " + ldt.Rows[0]["Db_server"]
                        //        + ";database = " + ldt.Rows[0]["Db_name"]
                        //+ ";Uid = " + ldt.Rows[0]["Db_User"]
                        //+ ";pwd = " + ldt.Rows[0]["Db_Password"];
                        ls_sql = "SELECT Name FROM SysObjects Where XType in('U','V') ORDER BY Name";
                        ldt = new DataTable();
                        if (ucbTablePosition.Text == Project.DB_Alias)
                            lb_result = SQLServerDBop.SQLSelect(Project.DB_Connection, ls_sql, ref ldt,"");
                        else
                            lb_result = SQLServerDBop.SQLSelect(Project.TheirDB_Connection, ls_sql, ref ldt, "");
                        if (lb_result)
                        {
                            cb_DatabaseTable.DisplayMember = "Name";
                            cb_DatabaseTable.ValueMember = "Name";
                            cb_DatabaseTable.DataSource = ldt;
                            cb_DatabaseTable.SelectedIndex = -1;
                            cb_DatabaseTable.SelectedIndex = 0;
                        }
                        dgv_columns.Columns["ISIDENTITY"].Visible = true;
                    }
                    else if (ls_type == "ora")
                    {
                        //constr = "server = " + ldt.Rows[0]["Db_server"]
                        //        + ";Uid = " + ldt.Rows[0]["Db_User"]
                        //        + ";pwd = " + ldt.Rows[0]["Db_Password"];
                        ls_sql = ("SELECT table_name as Name from user_tables where tablespace_name in"
                                + " ("
                                + " select default_tablespace from dba_users"
                                + " where username = '" + ldt.Rows[0]["Db_User"] + "'"
                                + " ) UNION SELECT VIEW_NAME AS NAME FROM User_Views").ToUpper();
                        ldt = new DataTable();
                        if (ucbTablePosition.Text == Project.DB_Alias)
                            lb_result = OracleDBop.SQLSelect(Project.DB_Connection, ls_sql, ref ldt);
                        else
                            lb_result = OracleDBop.SQLSelect(Project.TheirDB_Connection, ls_sql, ref ldt);
                        if (lb_result)
                        {
                            cb_DatabaseTable.DisplayMember = "Name";
                            cb_DatabaseTable.ValueMember = "Name";
                            cb_DatabaseTable.DataSource = ldt;
                            cb_DatabaseTable.SelectedIndex = -1;
                            cb_DatabaseTable.SelectedIndex = 0;
                        }
                        dgv_columns.Columns["ISIDENTITY"].Visible = false;
                    }
                    else if (ls_type == "mysql")
                    {
                        string db_name = ucbTablePosition.Text == Project.DB_Alias ? Project.DbName : Project.theirDbName;
                        ls_sql = "select table_name as name from INFORMATION_SCHEMA.TABLES Where TABLE_TYPE='BASE TABLE' and table_schema = '" + db_name + "'";
                        ldt = new DataTable();
                        if (ucbTablePosition.Text == Project.DB_Alias)
                            lb_result = MySqlDBop.SQLSelect(Project.DB_Connection, ls_sql, ref ldt);
                        else
                            lb_result = MySqlDBop.SQLSelect(Project.TheirDB_Connection, ls_sql, ref ldt);
                        if (lb_result)
                        {
                            cb_DatabaseTable.DisplayMember = "Name";
                            cb_DatabaseTable.ValueMember = "Name";
                            cb_DatabaseTable.DataSource = ldt;
                            cb_DatabaseTable.SelectedIndex = -1;
                            cb_DatabaseTable.SelectedIndex = 0;
                        }
                        dgv_columns.Columns["ISIDENTITY"].Visible = true;
                    }
                    else if (ls_type == "access")
                    {
                        //string db_name = ucbTablePosition.Text == Project.DB_Alias ? Project.DbName : Project.theirDbName;
                        //ls_sql = "SELECT name FROM MSYSOBJECTS WHERE TYPE=1 AND NAME NOT LIKE 'Msys*'";
                        //ldt = new DataTable();
                        //if (ucbTablePosition.Text == Project.DB_Alias)
                        //    lb_result = AccessDBop.SQLSelect(Project, ls_sql, ref ldt);
                        //else
                        //    lb_result = AccessDBop.SQLSelect(Project.TheirDB_Connection, ls_sql, ref ldt);
                        //if (lb_result)
                        //{
                        //    cb_DatabaseTable.DisplayMember = "Name";
                        //    cb_DatabaseTable.ValueMember = "Name";
                        //    cb_DatabaseTable.DataSource = ldt;
                        //    cb_DatabaseTable.SelectedIndex = -1;
                        //    cb_DatabaseTable.SelectedIndex = 0;
                        //}
                        //dgv_columns.Columns["ISIDENTITY"].Visible = true;
                        MessageBox.Show("暂不支持access");
                    }
                }
            }
        }
        private void InsertPublicRows(string as_formtype_id)
        {
            //if (_TableName != "") return;
            string ls_sql = "SELECT Table_Name, Field_Name,Field_Desc, Field_Type, IsPK,CanNull,ShowIndex,IsIdentity " +
                " FROM IF_field_infor Where Table_Name ='" + as_formtype_id + "' order by ShowIndex";
            DataTable ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                DataTable ldt_old = (DataTable)dgv_columns.DataSource;
                for (int i = ldt_old.Rows.Count - 1; i >= 0; i--)
                    if (ldt_old.Rows[i].RowState != DataRowState.Deleted)
                        ldt_old.Rows[i].Delete();

                foreach (DataRow dr in ldt.Rows)
                {
                    DataRow ldr = ldt_old.NewRow();
                    ldr[1] = dr[1];
                    ldr[2] = dr[2];
                    ldr[3] = dr[3];
                    ldr[4] = dr[4];
                    ldr[5] = dr[5];
                    ldr[6] = dr[6];
                    ldr[7] = dr[7];
                    ldt_old.Rows.Add(ldr);
                }
            }
        }
        private void InsertPublicRowsFromDataBase(string as_formtype_id)
        {
            //if (_TableName != "") return;
            string ls_sql = "";
            if (ls_type == "sql")
                ls_sql = "select "
                        + " '' as Table_Name"
                        + " ,A.column_name as Field_Name"
                        + " ,'' as Field_Desc"
                        + " , data_type + case  when character_maximum_length Is null or character_maximum_length > 8000 then '' else '(' + convert(varchar(10),character_maximum_length) + ')'  end as Field_Type"
                        + " ,case  when B.COLUMN_NAME Is null then 'N' else 'Y' end as IsPK"
                        + " ,case  when A.IS_NULLABLE = 'No' then 'N' else 'Y' end as CanNull"
                        + " ,0 as ShowIndex"
                        + " ,case  when  COLUMNPROPERTY(OBJECT_ID('" + as_formtype_id + "'),A.COLUMN_NAME,'IsIdentity') = 1 then 'Y' else 'N' end as IsIdentity"
                        + " from  information_schema.columns A"
                        + " left join"
                        + " INFORMATION_SCHEMA .KEY_COLUMN_USAGE B"
                        + " on "
                        + " A.table_name = B.table_name"
                        + " and A.column_name = B.column_name"
                        + " where A.table_name='" + as_formtype_id + "'";
            else if (ls_type == "ora")
                ls_sql = "SELECT '' AS TABLENAME, B.COLUMN_NAME AS FIELDNAME, COMMENTS AS FIELD_DESC,"
                        + " CASE  WHEN (B.DATA_TYPE = 'INTERGER') OR (B.DATA_TYPE = 'FLOAT') OR (B.DATA_TYPE = 'DATE') THEN  B.DATA_TYPE "
                        + " WHEN B.DATA_TYPE = 'NUMBER' THEN  B.DATA_TYPE  || '(' || TO_CHAR(B.DATA_LENGTH) || ',' || TO_CHAR(DATA_SCALE) || ')'"
                        + " ELSE   B.DATA_TYPE || '(' || B.DATA_LENGTH || ')' END AS FIELD_TYPE,"
                        + " CASE WHEN C.COLUMN_NAME IS NULL THEN 'N' ELSE 'Y' END AS ISPK,"
                        + " B.NULLABLE AS CANNULL,"
                        + " 0 AS SHOWINDEX,"
                        + " 'N' AS ISIDENTITY"
                        + " FROM USER_COL_COMMENTS A INNER JOIN USER_TAB_COLUMNS B"
                        + " ON A.TABLE_NAME = B.TABLE_NAME AND A.COLUMN_NAME = B.COLUMN_NAME"
                        + " AND A.TABLE_NAME = '" + as_formtype_id + "'"
                        + " LEFT JOIN"
                        + " ("
                        + " SELECT * FROM DBA_CONS_COLUMNS "
                        + " WHERE CONSTRAINT_NAME "
                        + " IN (SELECT CONSTRAINT_NAME "
                        + " FROM DBA_CONSTRAINTS"
                        + " WHERE OWNER='SCM_MAIN'"
                        + " AND CONSTRAINT_TYPE='P'"
                        + " AND TABLE_NAME = '" + as_formtype_id + "'"
                        + " )"
                        + " ORDER BY CONSTRAINT_NAME,POSITION"
                        + " ) C"
                        + " ON A.TABLE_NAME = C.TABLE_NAME"
                        + " AND B.COLUMN_NAME = C.COLUMN_NAME";
            else if (ls_type == "mysql")
                ls_sql = "select  '' as Table_Name, column_name  as Field_Name,'' as Field_Desc ,column_type  as Field_Type,"
                        + " case  when COLUMN_KEY = 'PRI' then 'Y' else 'N' end as IsPK,"
                        + " case  when IS_NULLABLE = 'No' then 'N' else 'Y' end as CanNull,"
                        + " 0 as ShowIndex,"
                        + " 'N' as IsIdentity"
                        + " from Information_schema.columns"
                        + " where table_Name ='" + as_formtype_id + "'";
            else if (ls_type == "access")
            {
                MessageBox.Show("暂不支持自动获取!");
            }
            DataTable ldt = new DataTable();
            bool lb_result = false;
            if (ls_type == "sql")
            {
                if (ucbTablePosition.Text == Project.DB_Alias)
                    lb_result = SQLServerDBop.SQLSelect(Project.DB_Connection, ls_sql, ref ldt, "");
                else
                    lb_result = SQLServerDBop.SQLSelect(Project.TheirDB_Connection, ls_sql, ref ldt, "");
            }
            else if (ls_type == "ora")
            {
                if (ucbTablePosition.Text == Project.DB_Alias)
                    lb_result = OracleDBop.SQLSelect(Project.DB_Connection, ls_sql, ref ldt);
                else
                    lb_result = OracleDBop.SQLSelect(Project.TheirDB_Connection, ls_sql, ref ldt);
            }
            else if (ls_type == "mysql")
            {
                if (ucbTablePosition.Text == Project.DB_Alias)
                    lb_result = MySqlDBop.SQLSelect(Project.DB_Connection, ls_sql, ref ldt);
                else
                    lb_result = MySqlDBop.SQLSelect(Project.TheirDB_Connection, ls_sql, ref ldt);
            }
            if (lb_result)
            {
                DataTable ldt_old = (DataTable)dgv_columns.DataSource;
                for (int i = ldt_old.Rows.Count - 1; i >= 0; i--)
                    if (ldt_old.Rows[i].RowState != DataRowState.Deleted)
                        ldt_old.Rows[i].Delete();

                foreach (DataRow dr in ldt.Rows)
                {
                    DataRow ldr = ldt_old.NewRow();
                    ldr[1] = dr[1];
                    ldr[2] = dr[2];
                    ldr[3] = dr[3];
                    ldr[4] = dr[4];
                    ldr[5] = dr[5];
                    ldr[6] = dr[6];
                    ldr[7] = dr[7];
                    ldt_old.Rows.Add(ldr);
                }
            }
        }

        private void tb_FormType_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable ldt1 = (DataTable)dgv_columns.DataSource;
                foreach (DataRow ldr in ldt1.Rows)
                    if (ldr.RowState != DataRowState.Deleted)
                    {
                        ldr[0] = tb_TableName.Text;
                        ldr[9] = Project.Project_ID;
                    }
            }
            catch { }
        }

        private void b_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            if (tb_TableName.Text == "")
            {
                MessageBox.Show("请输入表名！");
                return;
            }
            if (ucbTablePosition.Text == "")
            {
                MessageBox.Show("请选择表位置！");
                return;
            }
            string ls_sql = "";
            string ls_CompareTableName = (_TableType == "Z" ? "" : (cb_CompareTable.SelectedValue == null ? "" : cb_CompareTable.SelectedValue.ToString()));
            if (_TableName == "")
                ls_sql = "INSERT INTO IF_Table_infor(Table_Name,Table_desc, Table_Type, CompareTableName,table_position) " +
                    "VALUES ('{0}','{1}','" + _TableType + "','" + ls_CompareTableName + "','{2}')";
            else
                ls_sql = "UPDATE IF_Table_infor Set Table_Name='{0}', Table_desc = '{1}', CompareTableName = '" + ls_CompareTableName + "',table_position='{2}' " +
                    "Where Table_Name='" + _TableName + "' and Table_Type = '" + _TableType + "'";
            string sPosition = ucbTablePosition.Text == Project.DB_Alias ? "0" : "1";
            ls_sql = String.Format(ls_sql, new object[] { tb_TableName.Text, tb_TableDesc.Text, sPosition });
            _TableName = tb_TableName.Text;
            if (AccessDBop.SQLExecute(ls_sql))
            {
                ls_sql = "SELECT Table_Name, Field_Name, Field_Desc, " +
                    "Field_Type, IsPK, CanNull,ShowIndex,IsIdentity " +
                    " FROM IF_field_infor " +
                    " Where Table_Name ='" + _TableName + "'";
                DataTable ldt1 = (DataTable)dgv_columns.DataSource;
                int k = 0;
                foreach (DataRow ldr in ldt1.Rows)
                {
                    if (ldr.RowState != DataRowState.Deleted)
                    {
                        if (ldr["Field_Name"] == null || ldr["Field_Name"].ToString() == "")
                        { MessageBox.Show("请输入列名！"); return; }
                        if (ldr["Field_Type"] == null || ldr["Field_Type"].ToString() == "")
                        { MessageBox.Show("请输入列类型！"); return; }
                        ldr[0] = tb_TableName.Text;
                        if (ldr["IsPK"] == null || ldr["IsPK"].ToString().Trim() == "")
                            ldr["IsPK"] = "N";
                        if (ldr["CanNull"] == null || ldr["CanNull"].ToString().Trim() == "")
                            ldr["CanNull"] = "N";
                        if (ldr["IsIdentity"] == null || ldr["IsIdentity"].ToString().Trim() == "")
                            ldr["IsIdentity"] = "N";
                        ldr["ShowIndex"] = k++;
                    }
                }
                bool updateFlag = !AccessDBop.SQLUpdate(ls_sql, ref ldt1);
                string db_type = ucbTablePosition.Text == Project.DB_Alias ? Project.DB_Type : Project.TheirDB_Type;
                string CreaateTableSQL = DataBaseManager.GetCreateTableSQL(_TableName, db_type);
                if (updateFlag ||
                    (CreaateTableSQL.Equals("") ||
                    CreaateTableSQL.Equals("error") ? false : !AccessDBop.SQLExecute("update IF_Table_infor set CreateTable='" + CreaateTableSQL.Replace("'", "''") + "' Where Table_Name='" + _TableName + "'")))
                {
                    MessageBox.Show("更新列时出错！");
                    return;
                }
                else
                {
                    MessageBox.Show("更新列成功！");
                }
            }
            else
            {
                MessageBox.Show("更新表时出错！");
                return;
            }
            DialogResult = DialogResult.OK;
        }


        private void dgv_columns_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Delete)
            //{
            //    if (dgv_columns.SelectedRows.Count > 0)
            //    {
            //        string ls_fixed = dgv_columns.SelectedRows[0].Cells[8].Value.ToString();
            //        if (ls_fixed == "1")
            //            e.Handled = true;
            //        else
            //            e.Handled = false;
            //    }
            //}
        }

        private void tb_TableName_Validating(object sender, CancelEventArgs e)
        {
            //if (tb_TableName.Text == "" ||
            //    !((tb_TableName.Text.Substring(0, 1).CompareTo("a") >= 0
            //    && tb_TableName.Text.Substring(0, 1).CompareTo("z") <= 0)
            //    || (tb_TableName.Text.Substring(0, 1).CompareTo("A") >= 0
            //    && tb_TableName.Text.Substring(0, 1).CompareTo("Z") <= 0)))
            //{
            //    eP1.SetError(tb_TableName, "数据表名不能为空，第一个字符要求是字母！");
            //    e.Cancel = true;
            //}
            //else
            //{
            //    eP1.SetError(tb_TableName, "");
            //}

        }

        private void dgv_columns_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex >= 0)
            //{
            //    if (e.ColumnIndex == 9)
            //    {
            //        dgv_columns.Rows[e.RowIndex].ReadOnly = false;
            //        return;
            //    }
            //    if (dgv_columns.Rows[e.RowIndex].IsNewRow) return;
            //    try
            //    {
            //        string ls_fixed = dgv_columns.Rows[e.RowIndex].Cells[8].Value.ToString();
            //        if (ls_fixed == "1")
            //        {
            //            dgv_columns.Rows[e.RowIndex].ReadOnly = true;
            //        }
            //        else
            //            dgv_columns.Rows[e.RowIndex].ReadOnly = false;
            //    }
            //    catch { }
            //}
        }

        private void dgv_columns_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                ColumnTypeDefine dialog = new ColumnTypeDefine();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    dgv_columns.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dialog.gs_columndefine;
                }
            }
        }

        private void dgv_columns_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.FormattedValue.ToString() == "")
            {
                return;
            }
            DataGridViewColumn Column = dgv_columns.Columns[e.ColumnIndex];
            if (Column.Name == "Field_Name")
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^\d{1,}.*$");
                if (reg.IsMatch(e.FormattedValue.ToString()) || e.FormattedValue.ToString().Trim() == "")
                {
                    e.Cancel = true;
                }
            }
        }

        private void cb_Formtype_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!Isload)
                {
                    string ls_formtype_id = cb_CompareTable.SelectedValue.ToString();
                    tb_TableName.Text = "tmp_" + ls_formtype_id;
                    InsertPublicRows(ls_formtype_id);
                }
            }
            catch { }
        }

        private void cb_CompareTable_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void cb_CompareTable_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cb_DatabaseTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!Isload)
                {
                    string ls_formtype_id = cb_DatabaseTable.SelectedValue.ToString();
                    tb_TableName.Text = ls_formtype_id;
                    InsertPublicRowsFromDataBase(ls_formtype_id);
                }
            }
            catch { }
        }

        private void userCombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RetrieveData();
        }

        private void EditForm_Leave(object sender, EventArgs e)
        {

        }
    }
}