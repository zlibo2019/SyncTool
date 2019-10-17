using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class FrmCompareTable : Form
    {
        private string _TableName = "";
        private bool Isloading = true;
        private bool IsAutoExec = true;

        public FrmCompareTable()
        {
            InitializeComponent();
        }

        public FrmCompareTable(string as_TableName)
        {
            InitializeComponent();
            _TableName = as_TableName;
        }

        private void FrmCompareTable_Load(object sender, EventArgs e)
        {

            SetDataGridStyle();
            LoadData();

        }

        public void LoadData()
        {
            RetrieveData();
            Isloading = false;
        }

        private void SetDataGridStyle()
        {
            DataGridStyle.GridDisplayStyle(d_check, true);
            DataGridStyle.GridDisplayStyle(d_contition, true);
            DataGridStyle.GridDisplayStyle(d_replace, true);
        }

        private void RetrieveData()
        {
            string ls_sql = "";
            DataTable ldt;

            //if (_TableType != "Z")
            //{
            //绑定下拉框

            ls_sql = "select Table_Name,Table_desc From IF_Table_infor"
                + " Where Table_Type = 'Z' ";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                if (ldt.Rows.Count > 0)
                {
                    cb_Ztable.DisplayMember = "Table_desc";
                    cb_Ztable.ValueMember = "Table_Name";
                    cb_Ztable.DataSource = ldt;
                    cb_Ztable.SelectedIndex = -1;
                    cb_Ztable.SelectedIndex = 0;
                }
            }
            if (_TableName == "")
                ls_sql = "select Table_Name,Table_desc From IF_Table_infor"
                        + " Where Table_Type = 'L'"
                        + " and Table_Name not in "
                        + " (select TableName from IF_Table_Compare)";
            else
                ls_sql = "select Table_Name,Table_desc From IF_Table_infor"
                    + " Where Table_Type = 'L'";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                if (ldt.Rows.Count > 0)
                {
                    cb_Ltable.DisplayMember = "Table_desc";
                    cb_Ltable.ValueMember = "Table_Name";
                    cb_Ltable.DataSource = ldt;
                    cb_Ltable.SelectedIndex = -1;
                    cb_Ltable.SelectedIndex = 0;
                }
            }
            //}
            //初始化datagird
            if (_TableName != "" && _TableName != null)
            {
                cb_Ltable.Enabled = false;
                cb_Ztable.Enabled = false;
                ls_sql = "SELECT TableName, CompareTableName,Rule_id, Rule_content, SQLString " +
                    " FROM IF_Table_Compare Where TableName ='" + _TableName + "'";
                ldt = new DataTable();
                DataGridView dgv = new DataGridView();
                if (AccessDBop.SQLSelect(ls_sql, ref ldt))
                {
                    if (ldt.Rows.Count > 0)
                    {
                        cb_Ltable.SelectedValue = ldt.Rows[0]["TableName"].ToString();
                        cb_Ztable.SelectedValue = ldt.Rows[0]["CompareTableName"].ToString();
                        switch (ldt.Rows[0]["Rule_id"].ToString())
                        {
                            case "0":
                                dgv = d_replace;
                                r_replace.Checked = true;
                                tb_Mode.SelectedIndex = 0;
                                break;
                            case "1":
                                dgv = d_check;
                                r_check.Checked = true;
                                tb_Mode.SelectedIndex = 1;
                                break;
                            case "2":
                                dgv = d_contition;
                                r_contition.Checked = true;
                                tb_Mode.SelectedIndex = 2;
                                break;
                        }
                    }
                    string ls_Rule_content = ldt.Rows[0]["Rule_content"].ToString();
                    string[] ls_contents = ls_Rule_content.Split(new string[] { "{seprator}" }, StringSplitOptions.None);
                    string[] ls_rows = ls_contents[0].Split(new string[] { "{|}" }, StringSplitOptions.None);
                    //初始化datagridview
                    for (int i = 0; i < ls_rows.Length; i++)
                    {
                        string[] ls_value = ls_rows[i].Split(new string[] { "{,}" }, StringSplitOptions.None);
                        dgv.Rows.Add(ls_value);
                    }
                    //若条件更新初始化增删改字段
                    string[] ls_conditionfileds = ls_contents[1].Split(new string[] { "," }, StringSplitOptions.None);
                    if (r_contition.Checked)
                    {
                        t_conditionfield.Text = ls_conditionfileds[0];
                        t_addflag.Text = ls_conditionfileds[1];
                        t_updateflag.Text = ls_conditionfileds[2];
                        t_deleteflag.Text = ls_conditionfileds[3];
                        c_conAdd.Checked = (ls_conditionfileds[4] == "1" ? true : false);
                        c_conUpdate.Checked = (ls_conditionfileds[5] == "1" ? true : false);
                        c_conDel.Checked = (ls_conditionfileds[6] == "1" ? true : false);
                        if ((!c_conAdd.Checked) && (!c_conUpdate.Checked) && (!c_conDel.Checked))
                        {
                            d_contition.Enabled = false;
                            DataGridStyle.GridDisplayStyle(d_contition, true, false);
                        }
                        else
                        {
                            d_contition.Enabled = true;
                            DataGridStyle.GridDisplayStyle(d_contition, true);
                        }
                    }
                    else if (r_check.Checked)
                    {
                        c_checkAdd.Checked = (ls_conditionfileds[0] == "1" ? true : false);
                        c_checkUpdate.Checked = (ls_conditionfileds[1] == "1" ? true : false);
                        c_checkDel.Checked = (ls_conditionfileds[2] == "1" ? true : false);
                        if ((!c_checkAdd.Checked) && (!c_checkUpdate.Checked) && (!c_checkDel.Checked))
                        {
                            d_check.Enabled = false;
                            DataGridStyle.GridDisplayStyle(d_check, true, false);
                        }
                        else
                        {
                            d_check.Enabled = true;
                            DataGridStyle.GridDisplayStyle(d_check, true);
                        }
                    }
                    //初始化自定义语句
                    if (ls_contents[2] == "1")
                    {
                        r_enable.Checked = true;
                        t_sql.Text = ls_contents[3];
                    }
                    else
                    {
                        r_disable.Checked = true;
                    }
                }
            }
            else
            {
                InitDataGrid();
                tb_Mode.SelectedIndex = 1;
                c_checkAdd.Checked = false;
                c_checkDel.Checked = false;
                c_checkUpdate.Checked = false;
            }
        }

        private void InitDataGrid()
        {
            string ls_sql = "";
            DataTable ldt;

            ls_sql = "select '' as Field_Name,'' as Field_Desc,Field_Name as ZField_Name,Field_Desc as ZField_Desc,'N' as RelativeFields,'Y' as IsTrans,'' as convertCol From IF_field_infor" +
    " Where Table_Name = '" + cb_Ztable.SelectedValue + "' order by showindex";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                d_replace.DataSource = (r_replace.Checked ? ldt : d_replace.DataSource);
                d_check.DataSource = (r_check.Checked ? ldt : d_check.DataSource);
                d_contition.DataSource = (r_contition.Checked ? ldt : d_contition.DataSource);
            }


            ls_sql = "select Field_Name,Field_Desc From IF_field_infor" + " Where Table_Name = '" + cb_Ltable.SelectedValue + "' order by showindex";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                DataGridView dgv = new DataGridView();
                if (r_replace.Checked)
                    dgv = d_replace;
                if (r_check.Checked)
                    dgv = d_check;
                if (r_contition.Checked)
                    dgv = d_contition;
                int count = Math.Min(dgv.Rows.Count, ldt.Rows.Count);
                for (int i = 0; i < count; i++)
                {
                    dgv[0, i].Value = ldt.Rows[i]["Field_Name"].ToString();
                    dgv[1, i].Value = ldt.Rows[i]["Field_Desc"].ToString();
                }
            }
        }

        private void userControl11_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void userControl11_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void radionCheckedChange(object sender, EventArgs e)
        {
            if (Isloading) return;
            if (((RadioButton)sender).Checked) return;
            if (!IsAutoExec)
            {
                IsAutoExec = true;
                return;
            }
            if (_TableName != "" && _TableName != null)
            {
                if (MessageBox.Show("确定要更换配置方式吗？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                    == DialogResult.Cancel)
                {
                    IsAutoExec = false;
                    ((RadioButton)sender).Checked = true;
                    IsAutoExec = false;
                    return;
                }
            }
            if (r_replace.Checked)
            {
                tb_Mode.SelectedTab = tabPage11;
            }
            if (r_check.Checked)
            {
                tb_Mode.SelectedTab = tabPage12;
            }
            if (r_contition.Checked)
            {
                tb_Mode.SelectedTab = tabPage13;
            }
            if (!Isloading)
            {
                InitDataGrid();
                btAdd.Visible = true;
                btDel.Visible = true;
            }
        }

        private void cb_Ltable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Isloading)
            {
                InitDataGrid();
                string ls_sql = "select table_position From IF_Table_infor"
                             + " Where Table_Name = '" + cb_Ltable.SelectedValue + "'";
                DataTable ldt = new DataTable();
                if (AccessDBop.SQLSelect(ls_sql, ref ldt))
                {
                    if (ldt.Rows.Count > 0)
                    {
                        string table_position = ldt.Rows[0]["table_position"].ToString();
                        ls_sql = "select Table_Name,Table_desc From IF_Table_infor"
                            + " Where Table_Type = 'Z' and table_position = '" + table_position + "'";
                        if (AccessDBop.SQLSelect(ls_sql, ref ldt))
                        {
                            if (ldt.Rows.Count > 0)
                            {
                                cb_Ztable.DisplayMember = "Table_desc";
                                cb_Ztable.ValueMember = "Table_Name";
                                cb_Ztable.DataSource = ldt;
                                //cb_Ztable.SelectedIndex = -1;
                                //cb_Ztable.SelectedIndex = 0;
                            }
                        }
                    }
                }
            }
        }

        private void b_save_Click(object sender, EventArgs e)
        {
            //验证
            if (cb_Ltable.Text == "")
            {
                MessageBox.Show("请输入临时表信息");
                return;
            }

            DataGridView dgv = new DataGridView();
            if (r_replace.Checked)
            {
                dgv = d_replace;
            }
            if (r_check.Checked)
            {
                dgv = d_check;
            }
            if (r_contition.Checked)
            {
                dgv = d_contition;
            }
            if (dgv.Enabled)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    if ((dgv[0, i].Value == null || dgv[0, i].Value.ToString() == "" || dgv[2, i].Value == null || dgv[2, i].Value.ToString() == "") && dgv[5, i].Value.ToString() == "Y")
                    {
                        MessageBox.Show("请完全输入需要同步的字段信息");
                        return;
                    }
                }
            }

            if (r_replace.Checked)
            {
                string FieldNull = "";
                for (int i = 0; i < d_replace.Rows.Count; i++)
                {
                    if (d_replace[2, i].Value.ToString() == "" && d_replace[5, i].Value.ToString() == "Y")
                    {
                        FieldNull = d_replace[0, i].Value.ToString();
                    }
                }
                if (FieldNull != "")
                {
                    MessageBox.Show("请输入临时表字段[" + FieldNull + "]对应的正式表字段");
                    return;
                }
            }
            if (r_check.Checked)
            {
                if (!dgv.Enabled && t_sql.Text.Trim().Equals(""))
                {
                    MessageBox.Show("未设置任何内容！");
                    return;
                }
                else if (dgv.Enabled)
                {
                    bool NativeFieldCheck = false;
                    string FieldNull = "";
                    for (int i = 0; i < d_check.Rows.Count; i++)
                    {
                        if (d_check["NativeField", i].Value.ToString() == "Y")
                        {
                            NativeFieldCheck = true;
                        }
                        if (d_check["ZField_ColName", i].Value.ToString() == "" && d_check["CheckIsTrans", i].Value.ToString() == "Y")
                        {
                            FieldNull = d_check["LField_ColName", i].Value.ToString();
                        }
                    }
                    if ((cb_Ztable.Text.Trim().Equals("")) && (c_checkAdd.Checked || c_checkDel.Checked || c_checkUpdate.Checked))
                    {
                        MessageBox.Show("请输入正式表名");
                        return;
                    }
                    if ((!NativeFieldCheck) && (c_checkAdd.Checked || c_checkDel.Checked || c_checkUpdate.Checked))
                    {
                        MessageBox.Show("必须至少选择一个关联字段");
                        return;
                    }
                    if (FieldNull != "")
                    {
                        MessageBox.Show("请输入临时表字段[" + FieldNull + "]对应的正式表字段");
                        return;
                    }
                }
            }
            if (r_contition.Checked)
            {
                bool NativeFieldCheck = false;
                string FieldNull = "";
                for (int i = 0; i < d_contition.Rows.Count; i++)
                {
                    if (d_contition[4, i].Value.ToString() == "Y")
                    {
                        NativeFieldCheck = true;
                    }
                    if (d_contition[2, i].Value.ToString() == "" && d_contition[5, i].Value.ToString() == "Y")
                    {
                        FieldNull = d_contition[0, i].Value.ToString();
                    }
                }
                if (!NativeFieldCheck)
                {
                    MessageBox.Show("必须至少选择一个关联字段");
                    return;
                }
                if (FieldNull != "")
                {
                    MessageBox.Show("请输入临时表字段[" + FieldNull + "]对应的正式表字段");
                    return;
                }
                if (t_conditionfield.Text.Trim() == "" || t_addflag.Text.Trim() == ""
                    || t_updateflag.Text == "" || t_deleteflag.Text == "")
                {
                    MessageBox.Show("请完全输入条件字段信息");
                    t_conditionfield.Focus();
                    return;
                }
            }
            //保存
            if (Save())
            {
                MessageBox.Show("执行成功！");
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("执行失败！");
            }
        }

        public bool Save()
        {
            string ls_sql = "";
            DataTable ldt;
            if (_TableName != "" && _TableName != null)
            {
                ls_sql = "update IF_Table_Compare"
                        + " set Rule_id = '" + (r_replace.Checked ? "0" : (r_check.Checked ? "1" : "2")) + "',"
                        + " Rule_content = '" + GetRulecontent() + "',"
                        + " SQLString = '" + GetRuleSQL() + "'"
                        + " where  TableName= '" + _TableName + "'";
                ldt = new DataTable();
                if (AccessDBop.SQLExecute(ls_sql))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                string Ztable = cb_Ztable.SelectedValue == null ? "z_table" : cb_Ztable.SelectedValue.ToString();
                string Ltable = cb_Ltable.SelectedValue.ToString();
                ls_sql = "INSERT INTO IF_Table_Compare(TableName,CompareTableName, Rule_id, Rule_content,SQLString) " +
                   "VALUES ('" + Ltable + "','" + Ztable + "','" +
                   (r_replace.Checked ? "0" : (r_check.Checked ? "1" : "2")) + "','" + GetRulecontent() + "','" + GetRuleSQL() + "')";
                ldt = new DataTable();
                if (AccessDBop.SQLExecute(ls_sql))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //return true;
        }

        string GetPositionByTable(string TableName)
        {
            string sql = "select table_position from IF_Table_infor where Table_Name = '" + TableName + "'";
            DataTable dt = new DataTable();
            if (AccessDBop.SQLSelect(sql, ref dt))
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["table_position"].ToString();
                }
            }
            return "";
        }

        private string GetRuleSQL()
        {
            if (t_sql.Text.Trim().Contains(".exe"))
            {
                return t_sql.Text.Trim();
            }

            string ls_resultStr = "";
            //string ls_line = "";
            string ls_insertstr = "";
            string ls_updatestr = "";
            string ls_selectstr = "";
            string ls_deletestr = "";
            string ls_execstr = "";
            string ls_relationstr = "";
            bool lb_isAdd = true;
            bool lb_isUpdate = true;
            bool lb_isDel = true;
            string db_type = string.Empty;

            string Ztable = cb_Ztable.SelectedValue == null ? "z_table" : cb_Ztable.SelectedValue.ToString();
            string Ltable = cb_Ltable.SelectedValue.ToString();


            DataGridView dgv = new DataGridView();
            if (r_replace.Checked)
                dgv = d_replace;
            if (r_check.Checked)
            {
                dgv = d_check;
                lb_isAdd = (c_checkAdd.Checked ? true : false);
                lb_isUpdate = (c_checkUpdate.Checked ? true : false);
                lb_isDel = (c_checkDel.Checked ? true : false);
            }
            if (r_contition.Checked)
            {
                dgv = d_contition;
                lb_isAdd = (c_conAdd.Checked ? true : false);
                lb_isUpdate = (c_conUpdate.Checked ? true : false);
                lb_isDel = (c_conDel.Checked ? true : false);
            }

            //取库类型

            string table_position = GetPositionByTable(_TableName);
            db_type = table_position.Equals("0") ? Project.DB_Type : Project.TheirDB_Type;

            if (dgv.Enabled)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    //ls_line = "";
                    object ls_insertobj = (dgv[2, i].Value == null ? (object)"" : dgv[2, i].Value);
                    object ls_selectobj = (dgv[6, i].Value == null ? (object)"" : dgv[6, i].Value);
                    object ls_relationobj = (dgv[4, i].Value == null ? (object)"" : dgv[4, i].Value);
                    object ls_Istran = (dgv[5, i].Value == null ? (object)"" : dgv[5, i].Value);
                    ls_selectobj = (ls_selectobj.ToString() == "" ? (dgv[0, i].Value == null ? (object)"" : dgv[0, i].Value) : ls_selectobj);
                    //需要同步的字段
                    if (ls_Istran.ToString() == "Y")
                    {
                        ls_selectstr += (ls_selectstr == "" ? "" : ",") + ls_selectobj.ToString().Replace("'", "''");
                        ls_insertstr += (ls_insertstr == "" ? "" : ",") + ls_insertobj.ToString().Replace("'", "''"); ;

                        ls_updatestr += (ls_updatestr == "" ? "" : ",") + Ztable
                            + "." + ls_insertobj.ToString() + " = "
                            + ls_selectobj.ToString().Replace(dgv[0, i].Value.ToString(), cb_Ltable.SelectedValue.ToString() + "." + dgv[0, i].Value.ToString()).Replace("'", "''");
                    }
                    //关联字段
                    if (ls_relationobj.ToString() == "Y")
                    {
                        ls_relationstr += (ls_relationstr == "" ? "" : " and ") +
                            cb_Ltable.SelectedValue.ToString() + "." + dgv[0, i].Value.ToString()
                            + " = " + Ztable + "." + dgv[2, i].Value.ToString();
                    }
                }

                //置换方式sql
                if (r_replace.Checked)
                {
                    ls_deletestr = "delete from " + Ztable + "; ";
                    ls_insertstr = "insert into " + Ztable + "(" + ls_insertstr + ") ";
                    ls_selectstr = "select " + ls_selectstr + " from " + cb_Ltable.SelectedValue.ToString() + "; ";
                    ls_execstr = ls_deletestr + ls_insertstr + ls_selectstr;
                }

                //验证方式sql
                if (r_check.Checked)
                {
                    //删除冗余数据
                    ls_deletestr = "delete from  " + Ztable + " "
                            + " where not exists "
                            + " ("
                            + " select * from " + Ltable
                            + " where " + ls_relationstr
                            + " ); ";
                    //更新现有数据

                    if (db_type.ToLower() == "sql server")
                    {
                        ls_updatestr = "update " + Ztable + " set "
                            + ls_updatestr
                            + " from " + Ztable
                            + " inner join " + cb_Ltable.SelectedValue.ToString()
                            + " on " + ls_relationstr + "; ";
                    }
                    else if (db_type.ToLower() == "oracle")
                    {
                        ls_updatestr = "update " + Ztable + " set ("
                            + ls_insertstr + ") =  (SELECT "
                            + ls_selectstr
                            + " from " + cb_Ltable.SelectedValue.ToString()
                            + " where " + ls_relationstr + ")"
                            + " where exists"
                            + "("
                            + " select * from " + cb_Ltable.SelectedValue.ToString()
                            + " where " + ls_relationstr
                            + ");";
                    }
                    else if (db_type.ToLower() == "mysql")
                    {
                        ls_updatestr = "update " + Ztable + " set "
                            + ls_updatestr
                            + " from " + Ztable
                            + " inner join " + cb_Ltable.SelectedValue.ToString()
                            + " on " + ls_relationstr + "; ";
                    }
                    //插入新数据

                    ls_insertstr = "insert into " + Ztable + "(" + ls_insertstr + ") "
                            + " select " + ls_selectstr + " from  " + Ltable + " "
                            + " where not exists"
                            + "("
                            + "select * from " + Ztable
                            + " where " + ls_relationstr
                            + ");";
                    //ls_execstr = (lb_isDel ? ls_deletestr + "\r\n\r\n" : "") + (lb_isUpdate ? ls_updatestr + "\r\n\r\n" : "") + (lb_isAdd ? ls_insertstr : "");
                    ls_execstr = (lb_isDel ? ls_deletestr : "") + (lb_isUpdate ? ls_updatestr : "") + (lb_isAdd ? ls_insertstr : "");
                }
                //条件方式sql
                if (r_contition.Checked)
                {
                    //删除冗余数据
                    ls_deletestr = "delete from  " + Ztable + " "
                            + " where exists "
                            + " ("
                            + " select * from " + Ltable
                            + " where " + ls_relationstr
                            + " and " + Ltable + "." + t_conditionfield.Text + " = ''" + t_deleteflag.Text + "''"
                            + " ); ";
                    //更新现有数据
                    //更新现有数据
                    if (Project.DB_Type.ToLower() == "sql server")
                    {
                        ls_updatestr = "update " + Ztable + " set "
                                    + ls_updatestr
                                    + " from " + Ztable
                                    + " inner join " + cb_Ltable.SelectedValue.ToString()
                                    + " on " + ls_relationstr
                                    + " and " + Ltable + "." + t_conditionfield.Text + " = ''" + t_updateflag.Text + "''"
                                    + ";";
                    }
                    else if (Project.DB_Type.ToLower() == "oracle")
                    {
                        ls_updatestr = "update " + Ztable + " set ("
                            + ls_insertstr + ") =  (SELECT "
                            + ls_selectstr
                            + " from " + cb_Ltable.SelectedValue.ToString()
                            + " where " + ls_relationstr + ")"
                            + " where exists"
                            + "("
                            + "select * from " + cb_Ltable.SelectedValue.ToString()
                            + " where " + ls_relationstr
                            + " and " + Ltable + "." + t_conditionfield.Text + " = ''" + t_updateflag.Text + "''"
                            + ");";
                    }
                    else if (Project.DB_Type.ToLower() == "mysql")
                    {
                        ls_updatestr = "update " + Ztable + " set "
                                    + ls_updatestr
                                    + " from " + Ztable
                                    + " inner join " + cb_Ltable.SelectedValue.ToString()
                                    + " on " + ls_relationstr
                                    + " and " + Ltable + "." + t_conditionfield.Text + " = ''" + t_updateflag.Text + "''"
                                    + ";";
                    }

                    //插入新数据

                    ls_insertstr = "insert into " + Ztable + "(" + ls_insertstr + ") "
                            + " select " + ls_selectstr + " from  " + Ltable + " "
                            + " where not exists"
                            + "("
                            + "select * from " + Ztable
                            + " where " + ls_relationstr
                            + ")"
                            + " and " + Ltable + "." + t_conditionfield.Text + " = ''" + t_addflag.Text + "''"
                            + ";";
                    ls_execstr = (lb_isDel ? ls_deletestr : "") + (lb_isUpdate ? ls_updatestr : "") + (lb_isAdd ? ls_insertstr : "");
                }
            }
            string SqlDiy = t_sql.Text.Replace("'", "''");
            ls_execstr += (r_enable.Checked ? SqlDiy + (SqlDiy == "" ? "" : ";") : "");
            ls_resultStr = db_type == "mysql" ? ls_execstr : "begin " + ls_execstr + " end;";
            return ls_resultStr;
        }

        private string GetRulecontent()
        {
            string ls_resultStr = "";
            string ls_line = "";
            DataGridView dgv = new DataGridView();
            if (r_replace.Checked)
                dgv = d_replace;
            if (r_check.Checked)
                dgv = d_check;
            if (r_contition.Checked)
                dgv = d_contition;

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                ls_line = "";
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    object ls_tmpobj = (dgv[j, i].Value == null ? (object)"" : dgv[j, i].Value);
                    ls_line += (j == 0 ? "" : "{,}") + ls_tmpobj.ToString().Replace("'", "''");
                }
                ls_resultStr += (ls_resultStr == "" ? "" : "{|}") + ls_line;
            }
            ls_resultStr += "{seprator}";
            if (r_contition.Checked)
            {
                //ls_resultStr += "{seprator}";
                ls_resultStr += t_conditionfield.Text + ",";
                ls_resultStr += t_addflag.Text + ",";
                ls_resultStr += t_updateflag.Text + ",";
                ls_resultStr += t_deleteflag.Text + ",";

                ls_resultStr += c_conAdd.Checked ? "1," : "0,";
                ls_resultStr += c_conUpdate.Checked ? "1," : "0,";
                ls_resultStr += c_conDel.Checked ? "1" : "0";
            }
            else if (r_check.Checked)
            {
                ls_resultStr += c_checkAdd.Checked ? "1," : "0,";
                ls_resultStr += c_checkUpdate.Checked ? "1," : "0,";
                ls_resultStr += c_checkDel.Checked ? "1" : "0";
            }

            ls_resultStr += "{seprator}";
            ls_resultStr += (r_enable.Checked ? "1" : "2");
            ls_resultStr += "{seprator}";
            ls_resultStr += t_sql.Text;
            return ls_resultStr;
        }

        private void r_enable_CheckedChanged(object sender, EventArgs e)
        {
            if (r_enable.Checked)
            {
                t_sql.Enabled = true;
            }
            else
            {
                t_sql.Enabled = false;
                t_sql.Text = "";
            }
        }

        private void b_close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void r_replace_Click(object sender, EventArgs e)
        {
            //if (_TableName != "" && _TableName != null)
            //{
            //    if (MessageBox.Show("确定要更换配置方式吗？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            //        == DialogResult.Cancel)
            //    {
            //        return;
            //    }
            //}
        }

        private void b_showContitionSql_Click(object sender, EventArgs e)
        {
            FrmShowSql fss = new FrmShowSql(GetRuleSQL());
            fss.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (r_replace.Checked)
            {
                d_replace.Rows.Add(new string[] { "", "", "", "", "N", "Y", "" });
            }
            if (r_check.Checked)
            {
                d_check.Rows.Add(new string[] { "", "", "", "", "N", "Y", "" });
            }
            if (r_contition.Checked)
            {
                d_contition.Rows.Add(new string[] { "", "", "", "", "N", "Y", "" });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataGridView dgv = new DataGridView();
            if (r_replace.Checked)
            {
                dgv = d_replace;
            }
            if (r_check.Checked)
            {
                dgv = d_check;
            }
            if (r_contition.Checked)
            {
                dgv = d_contition;
            }
            if (dgv.SelectedRows == null)
                return;
            else
                dgv.Rows.Remove(dgv.SelectedRows[0]);
        }

        private void cb_Ztable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Isloading)
            {
                InitDataGrid();
            }
        }

        private void c_checkAdd_CheckedChanged(object sender, EventArgs e)
        {
            if ((!c_checkAdd.Checked) && (!c_checkUpdate.Checked) && (!c_checkDel.Checked))
            {
                d_check.Enabled = false;
                DataGridStyle.GridDisplayStyle(d_check, true, false);
            }
            else
            {
                d_check.Enabled = true;
                DataGridStyle.GridDisplayStyle(d_check, true);
            }
        }

        private void c_conAdd_CheckedChanged(object sender, EventArgs e)
        {
            if ((!c_checkAdd.Checked) && (!c_checkUpdate.Checked) && (!c_checkDel.Checked))
            {
                d_contition.Enabled = false;
                DataGridStyle.GridDisplayStyle(d_contition, true, false);
            }
            else
            {
                d_contition.Enabled = true;
                DataGridStyle.GridDisplayStyle(d_contition, true);
            }

        }
    }
}