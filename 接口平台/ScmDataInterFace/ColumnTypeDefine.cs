using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class ColumnTypeDefine : Form
    {

        public string gs_columndefine;

        public ColumnTypeDefine()
        {
            InitializeComponent();
        }

        private void ColumnTypeDefine_Load(object sender, EventArgs e)
        {
            DataGridStyle.GridDisplayStyle(dgv_columntype, true);
            string ls_types = (Project.DB_Type.ToLower().Equals("sql server")) ?
               "varchar,nvarchar,char,nchar,int,bigint,decimal,numeric,money,float,real,datetime,image" :
               (Project.DB_Type.ToLower().Equals("oracle")) ?
                   "varchar2,nvarchar2,char,nchar,int,number,float,date" :
                   "text,varchar,char,int,float,datetime,date,TIMESTAMP";

            string ls_default = (Project.DB_Type.ToLower().Equals("sql server")) ?
                "50,50,10,10,,,10,10,10,,,," :
                 (Project.DB_Type.ToLower().Equals("oracle")) ?
                 "50,50,10,10,,10,," :
                 ",50,10,4,4,8,3,4";
            string[] lsa_types = ls_types.Split(',');
            string[] lsa_default = ls_default.Split(',');
            for (int i = 0; i < lsa_types.Length; i++)
            {
                dgv_columntype.Rows.Add(new object[] { lsa_types[i], lsa_default[i] });
            }
        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            if (dgv_columntype.SelectedRows.Count > 0)
            {
                string ls_type = dgv_columntype.SelectedRows[0].Cells[0].Value.ToString();
                string ls_length = dgv_columntype.SelectedRows[0].Cells[1].Value != null ?
                    dgv_columntype.SelectedRows[0].Cells[1].Value.ToString().Trim() : "";
                string ls_dec = dgv_columntype.SelectedRows[0].Cells[2].Value != null ?
                    dgv_columntype.SelectedRows[0].Cells[2].Value.ToString().Trim() : "";
                if ("varchar,nvarchar,char,nchar,varchar2,nvarchar2,".IndexOf(ls_type + ",") >= 0)
                    gs_columndefine = ls_type + (ls_length != "" ? "(" + ls_length + ")" : "");
                else if ("number,decimal,numeric,money,".IndexOf(ls_type + ",") >= 0)
                    gs_columndefine = ls_type + (ls_length != "" ?
                        "(" + ls_length + (ls_dec != "" ? "," + ls_dec : "") + ")" : "");
                else
                    gs_columndefine = ls_type;
            }
        }

        private void dgv_columntype_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            message.Text = "";
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                if (dgv_columntype.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly) return;
                if (e.ColumnIndex == 2 && e.FormattedValue.ToString().Trim() == "") return;
                int newInteger;
                if (!int.TryParse(e.FormattedValue.ToString(),
                    out newInteger) || newInteger < 0)
                {
                    e.Cancel = true;
                    message.Text = "数值必须为非负整数！";
                }
                else
                {
                    if (e.ColumnIndex == 2)
                    {
                        int li_length = Convert.ToInt32(dgv_columntype.Rows[e.RowIndex].Cells[1].Value);
                        int li_dec = int.Parse(e.FormattedValue.ToString());
                        if (li_length <= li_dec)
                        {
                            e.Cancel = true;
                            message.Text = "小数位数必须小于长度！";
                        }
                    }
                }
            }
        }

        private void dgv_columntype_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            string ls_type = dgv_columntype.Rows[e.RowIndex].Cells[0].Value.ToString().ToLower();
            if (e.ColumnIndex == 2)
            {
                string ls_types = "varchar,nvarchar,char,nchar,int,bigint,float,real,datetime," +
                    "varchar2,nvarchar2,date,";
                if (ls_types.IndexOf(ls_type + ",") >= 0)
                    dgv_columntype.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
            }
            else if (e.ColumnIndex == 1)
            {
                string ls_types = "int,bigint,float,real,datetime,date,";
                if (ls_types.IndexOf(ls_type + ",") >= 0)
                    dgv_columntype.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
            }
        }
    }
}
