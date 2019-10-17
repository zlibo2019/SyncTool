using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class SelectTable : Form
    {
        private string _Task_id;

        public SelectTable(string as_task_id)
        {
            InitializeComponent();
            _Task_id = as_task_id;
        }

        private void SelectUser_Load(object sender, EventArgs e)
        {
            DataGridStyle.GridDisplayStyle(dgv_tables,true);
            RetrieveUser();
        }
        private void RetrieveUser()
        {
            string ls_sql = "SELECT 0 as selected, Table_Name, Table_desc FROM IF_Table_infor " +
                "Where Table_Name not in " +
                "(SELECT OurTableName FROM IF_Task " +
                "Where Task_id = '" + _Task_id + "')";
            DataTable ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                dgv_tables.DataSource = ldt;
            }
        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgvr in dgv_tables.Rows)
            {
                string ls_OurTableName = dgvr.Cells[1].Value.ToString();
                string ls_OurTableDesc = dgvr.Cells[2].Value.ToString();
                string ls_selected = dgvr.Cells[0].Value.ToString();
                if (ls_selected == "1")
                {
                    string ls_sql = "Insert into IF_Task(Task_id,Task_Name,TheirTableName,TheirSql,OurTableName,OurTable_desc,OurSql,interfaceDesc,DeleteNotDrop,DropTable,direction,IncrementInsert) values ('" +
                        _Task_id + "','','" + ls_OurTableName + "','', '" + ls_OurTableName + "', '" + ls_OurTableDesc + "','','','N','Y','0','N')";
                    AccessDBop.SQLExecute(ls_sql);
                }
            }
            DialogResult = DialogResult.OK;
        }

        private void b_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgvr in dgv_tables.Rows)
            {
                dgvr.Cells[0].Value = checkBox2.Checked ? 1 : 0;
            }
        }
    }
}