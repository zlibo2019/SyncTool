using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using System.Configuration;
using System.Collections.Specialized;

namespace ScmDataInterFace
{
    public partial class FrmFieldTranse : Form
    {
        string _TheirTableName = "";
        string _OurTableName = "";
        string _interfaceDesc = "";
        string _direction = "";
        public string gs_interfaceDesc = "";
        public string gs_TheirSql = "";
        public string gs_OurSql = "";
        public string gs_GroupSql = "";
        public string gs_GroupField = "";
        public string gs_TheirTableName = "";
        public FrmFieldTranse(string as_TheirTableName, string as_OurTableName, string as_interfaceDesc, string as_direction)
        {
            InitializeComponent();
            _TheirTableName = as_TheirTableName;
            _OurTableName = as_OurTableName;
            _interfaceDesc = as_interfaceDesc;
            _direction = as_direction;
        }

        void LoadInterfaceTrans()
        {
            dgv_Fields.Columns["TField_Name"].HeaderText = Project.theirDB_Alias + "字段名";
            dgv_Fields.Columns["Field_Name"].HeaderText = Project.DB_Alias + "字段名";
            dgv_Fields.Columns["Field_Desc"].HeaderText = Project.DB_Alias + "字段描述";
            dgv_Fields.Columns["Field_Type"].HeaderText = Project.DB_Alias + "字段类型";
            dgv_Fields.Columns["CanNull"].HeaderText = Project.DB_Alias + "字段可否空";
        }

        private void FrmFieldTranse_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadInterfaceTrans();
            DataGridStyle.GridDisplayStyle(dgv_Fields, true);
        }

        public void LoadData()
        {
            t_TheirTableName.Text = _TheirTableName;
            t_OurTableName.Text = _OurTableName;
            string ls_sql = "";
            DataTable ldt = new DataTable();
            if (_interfaceDesc == "")
            {
                ls_sql = "select Field_Name as TField_Name,Field_Name,Field_Desc, Field_Type, CanNull,'Y' as IsTrans,'' as convertCol From IF_field_infor" +
                    " Where Table_Name = '" + _OurTableName + "'";
                ldt = new DataTable();
                if (AccessDBop.SQLSelect(ls_sql, ref ldt))
                {
                    dgv_Fields.DataSource = ldt;
                }
            }
            else
            {
                string[] Rule_contents = _interfaceDesc.Split(new string[] { "{separator}" }, StringSplitOptions.None);
                string ls_Rule_content = Rule_contents[0];

                string[] ls_rows = ls_Rule_content.Split(new string[] { "{|}" }, StringSplitOptions.None);
                //初始化datagridview
                for (int i = 0; i < ls_rows.Length; i++)
                {
                    string[] ls_value = ls_rows[i].Split(new string[] { "{,}" }, StringSplitOptions.None);
                    dgv_Fields.Rows.Add(ls_value);
                }
                if (Rule_contents.Length > 1)
                {
                    t_where.Text = Rule_contents[1];
                    t_group.Text = Rule_contents[2];
                }
            }
        }

        private void b_save_Click(object sender, EventArgs e)
        {
            if (!CheckInput())
                return;
            gs_TheirTableName = t_TheirTableName.Text;
            gs_interfaceDesc = GetRulecontent();
            GetRuleSQL(ref gs_TheirSql, ref gs_OurSql, ref gs_GroupSql);
            gs_GroupField = t_group.Text;
            DialogResult = DialogResult.OK;
        }

        private bool CheckInput()
        {
            for (int i = 0; i < dgv_Fields.Rows.Count - 1; i++)
            {
                if (
                    (dgv_Fields[1, i].Value == null && dgv_Fields[5, i].Value.ToString() == "Y")
                    ||
                    (dgv_Fields[0, i].Value == null && dgv_Fields[5, i].Value.ToString() == "Y" && dgv_Fields[6, i].Value == null)
                   )
                {
                    MessageBox.Show("请确保字段输入完整");
                    return false;
                }
            }
            return true;
        }
        private string GetRulecontent()
        {
            string ls_resultStr = "";
            for (int i = 0; i < dgv_Fields.Rows.Count - 1; i++)
            {
                string ls_line = "";
                for (int j = 0; j < dgv_Fields.Columns.Count; j++)
                {
                    object ls_tmpobj = (dgv_Fields[j, i].Value == null ? (object)"" : dgv_Fields[j, i].Value);
                    ls_line += (j == 0 ? "" : "{,}") + ls_tmpobj.ToString();
                }
                ls_resultStr += (ls_resultStr == "" ? "" : "{|}") + ls_line;
            }
            ls_resultStr += "{separator}" + t_where.Text + "{separator}" + t_group.Text;
            return ls_resultStr;
        }

        private string getConvertSql(string key)
        {
            var curKey = key.Substring(1);
            NameValueCollection nvcFormShow = (NameValueCollection)ConfigurationManager.GetSection(curKey);
            string sql = string.Empty;

            foreach (String str in nvcFormShow.AllKeys)
            {
                if (str == "other")
                {
                    sql += " else " + nvcFormShow[str] + " end ";
                    break;
                }
                var keyArr = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < keyArr.Length; i++)
                {
                    sql += " when " + curKey + " = " + keyArr[i] + " then " + nvcFormShow[str];
                }
            }
            sql = "case" + sql;
            return sql;
        }

        public void GetRuleSQL(ref string theirsql, ref string oursql, ref string groupsql)
        {
            string ls_insertstr = "";
            string ls_selectstr = "";

            for (int i = 0; i < dgv_Fields.Rows.Count; i++)
            {
                //string ls_line = "";
                object ls_ourobj = (dgv_Fields[1, i].Value == null ? (object)"" : dgv_Fields[1, i].Value);
                object ls_Theirobj = (dgv_Fields[0, i].Value == null ? (object)"" : dgv_Fields[0, i].Value);
                object ls_convertobj = (dgv_Fields[6, i].Value == null ? (object)"" : dgv_Fields[6, i].Value);
                string convertStr = ls_convertobj.ToString();
                if (convertStr.Length > 0 && convertStr.Substring(0, 1) == "#")
                {
                    ls_convertobj = (object)getConvertSql(convertStr);
                }

                object ls_Istran = (dgv_Fields[5, i].Value == null ? (object)"" : dgv_Fields[5, i].Value);
                //需要同步的字段
                if (ls_Istran.ToString() == "Y")
                {
                    //if (dgv_Fields[0, i].Value != null && dgv_Fields[0, i].Value.ToString() != "")
                    //{
                    if (_direction == "0")
                    {
                        if (Project.TheirDB_Type != "webservice" && Project.TheirDB_Type != "http")
                        {
                            //string sTrans = dgv_Fields[6, i].Value == null ? "" : dgv_Fields[6, i].Value.ToString();
                            string sTrans = ls_convertobj.ToString();
                            string selectField = sTrans == "" ? dgv_Fields[0, i].Value.ToString() : sTrans;
                            if (Project.TheirDB_Type == "access")
                                ls_selectstr += (ls_selectstr == "" ? "" : ",") + selectField;
                            else
                                ls_selectstr += (ls_selectstr == "" ? "" : ",") + selectField + " " + dgv_Fields[1, i].Value.ToString();

                        }
                        else
                        {
                            ls_convertobj = (ls_convertobj.ToString() == "" ? (dgv_Fields[0, i].Value == null ? (object)"" : dgv_Fields[0, i].Value) : ls_convertobj);
                            ls_convertobj += " " + dgv_Fields[1, i].Value.ToString();
                            ls_selectstr += (ls_selectstr == "" ? "" : ",") + ls_convertobj.ToString();
                        }
                        ls_insertstr += (ls_insertstr == "" ? "" : ",") + ls_ourobj.ToString();
                    }
                    else
                    {
                        if (Project.DB_Type != "webservice" && Project.DB_Type != "http")
                        {
                            string sTrans = dgv_Fields[6, i].Value == null ? "" : dgv_Fields[6, i].Value.ToString();
                            string selectField = sTrans == "" ? dgv_Fields[1, i].Value.ToString() : sTrans;
                            if (Project.DB_Type == "access")
                                ls_selectstr += (ls_selectstr == "" ? "" : ",") + selectField;
                            else
                                ls_selectstr += (ls_selectstr == "" ? "" : ",") + selectField + " " + dgv_Fields[0, i].Value.ToString();
                        }
                        else
                        {
                            ls_convertobj = (ls_convertobj.ToString() == "" ? (dgv_Fields[1, i].Value == null ? (object)"" : dgv_Fields[1, i].Value) : ls_convertobj);
                            ls_convertobj += " as " + dgv_Fields[0, i].Value.ToString();
                            ls_selectstr += (ls_selectstr == "" ? "" : ",") + ls_convertobj.ToString();
                        }
                        ls_insertstr += (ls_insertstr == "" ? "" : ",") + ls_Theirobj.ToString();
                    }
                    //}
                }
            }
            //置换方式sql
            if (_direction == "0")
            {
                if (Project.TheirDB_Type != "webservice" && Project.TheirDB_Type != "http")
                    theirsql = "select " + ls_selectstr + " from " + _TheirTableName + " " + t_where.Text;
                else
                    theirsql = ls_selectstr;
                groupsql = (t_group.Text.Trim() == "" ? "" : "select " + t_group.Text + " from " + _TheirTableName + " " + t_where.Text + " group by " + t_group.Text);
                oursql = "insert into " + _OurTableName + "(" + ls_insertstr + ") ";
            }
            else
            {
                if (Project.DB_Type != "webservice" && Project.DB_Type != "http")
                    oursql = "select " + ls_selectstr + " from " + _OurTableName + " " + t_where.Text;
                else
                    oursql = ls_selectstr;
                groupsql = (t_group.Text.Trim() == "" ? "" : "select " + t_group.Text + " from " + _OurTableName + " " + t_where.Text + " group by " + t_group.Text);
                theirsql = "insert into " + _TheirTableName + "(" + ls_insertstr + ") ";
            }
        }

        private void b_close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void dgv_Fields_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (dgv_Fields.Rows.Count <= 1) return;
            if (dgv_Fields[5, dgv_Fields.Rows.Count - 2].Value == null || dgv_Fields[5, dgv_Fields.Rows.Count - 2].Value.ToString() == "")
                dgv_Fields[5, dgv_Fields.Rows.Count - 2].Value = "Y";
            if (dgv_Fields[4, dgv_Fields.Rows.Count - 2].Value == null || dgv_Fields[4, dgv_Fields.Rows.Count - 2].Value.ToString() == "")
                dgv_Fields[4, dgv_Fields.Rows.Count - 2].Value = "Y";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "电子表格文件(*.xlsx)|*.xlsx|电子表格(*.xls)|*.xls";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    string fName = sfd.FileName;
                    string sName = Path.Combine(Application.StartupPath, @"template\template.xlsx");
                    ExcelHelper excel = new ExcelHelper(sName, fName);
                    Dictionary<int, int> dicColCompare = new Dictionary<int, int>(); //解析列号对比
                    dicColCompare.Add(0, 0);
                    dicColCompare.Add(2, 1);
                    dicColCompare.Add(3, 2);
                    dicColCompare.Add(4, 3);

                    bool bResult = excel.ToExcel2(dgv_Fields, 1, 2, 10000, "", "", "", "", dicColCompare);
                    if (bResult)
                    {
                        MessageBox.Show("导出成功!");
                    }
                    else
                    {
                        MessageBox.Show("导出失败!");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "电子表格文件(*.xlsx)|*.xlsx|电子表格(*.xls)|*.xls";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string fName = ofd.FileName;
                    try
                    {
                        DataTable dt = ExcelHelper.GetExcelDataTable(fName, "", 1, 1, "yes");
                        //获取对方表名
                        t_TheirTableName.Text = dt.Rows[0][0].ToString();
                        //获取对方字段名
                        for (int i = 0; i < dgv_Fields.Rows.Count - 1; i++)
                        {
                            string sContain = dt.Rows[i][1].ToString().Trim();
                            if (sContain == "")
                            {
                                dgv_Fields.Rows[i].Cells["IsTrans"].Value = "N";
                            }
                            dgv_Fields.Rows[i].Cells["TField_Name"].Value = sContain;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("导入失败!" + ex.Message);
                    }
                    MessageBox.Show("导入成功!");
                }
            }
        }
    }
}