using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class FrmTableTranse : Form
    {
        private string _Task_id = "";
        private bool Isloading = true;
        private DataTable gd_dt = new DataTable();
        private DataTable dtOurTableName = new DataTable();
        private DataTable dtTheirTableName = new DataTable();
        private ComboBox comb = null;

        public FrmTableTranse()
        {
            InitializeComponent();
        }

        public FrmTableTranse(string as_Task_id)
        {
            InitializeComponent();
            _Task_id = as_Task_id;
        }
        /// <summary>
        /// 设置dgv_task中方向的下拉菜单内容
        /// </summary>
        /// <param name="EditState">0表示加载状态1表示保存状态</param>
        void SetDGVColumn_Direction(int EditState = 0)
        {

            DataTable dtDirection = new DataTable();
            DataColumn dcDirection = new DataColumn("direction");
            dtDirection.Columns.Add(dcDirection);
            DataRow drDirection = dtDirection.NewRow();
            if (EditState == 0)
            {
                drDirection[0] = "导入" + Project.DB_Alias;
                dtDirection.Rows.Add(drDirection);
                drDirection = dtDirection.NewRow();
                drDirection[0] = "导入" + Project.theirDB_Alias;
            }
            else
            {
                drDirection[0] = "0";
                dtDirection.Rows.Add(drDirection);
                drDirection = dtDirection.NewRow();
                drDirection[0] = "1";
            }

            dtDirection.Rows.Add(drDirection);
            DataGridViewComboBoxColumn dgvComboBoxColumn = dgv_task.Columns["direction"] as DataGridViewComboBoxColumn;
            dgvComboBoxColumn.DataPropertyName = "direction";// "direction_alias";
            dgvComboBoxColumn.DataSource = dtDirection;
            dgvComboBoxColumn.DisplayMember = "direction";
            dgvComboBoxColumn.ValueMember = "direction";
        }

        void LoadInterfaceTrans()
        {
            dgv_task.Columns["TheirTableName"].HeaderText = Project.theirDB_Alias;
            dgv_task.Columns["OurTableName"].HeaderText = Project.DB_Alias;
        }

        private void FrmTableTranse_Load(object sender, EventArgs e)
        {
            RetrieveData();
            LoadInterfaceTrans();
            DataGridStyle.GridDisplayStyle(dgv_task, true);
            Isloading = false;
            string sql = "select Table_Name from IF_Table_Infor where table_position = '0'";
            AccessDBop.SQLSelect(sql, ref dtOurTableName);
            sql = "select Table_Name from IF_Table_Infor where table_position = '1'";
            AccessDBop.SQLSelect(sql, ref dtTheirTableName);
        }

        void DirectionTrans(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["direction"].Equals("0"))
                {
                    dt.Rows[i]["direction"] = "导入" + Project.DB_Alias;
                }
                else
                {
                    dt.Rows[i]["direction"] = "导入" + Project.theirDB_Alias;
                }
            }
        }

        private void RetrieveData()
        {

            string ls_sql = "";
            DataTable ldt;
            SetDGVColumn_Direction();
            //初始化datagird
            if (_Task_id != "" && _Task_id != null)
            {
                //ls_sql = "SELECT id,Task_id, Task_Name, TheirTableName,TheirSql, OurTableName, OurTable_desc, OurSql,interfaceDesc,SWITCH(direction = '0', '{0}',True,'{1}') AS direction_alias,'删除' as del ,IncrementInsert,DeleteNotDrop,DropTable,GroupSql ,GroupField" +
                //    " FROM IF_Task Where Task_id ='" + _Task_id + "' order by id";
                ls_sql = "SELECT id,Task_id, Task_Name, TheirTableName,TheirSql, OurTableName, OurTable_desc, OurSql,interfaceDesc,direction,'删除' as del ,IncrementInsert,DeleteNotDrop,DropTable,GroupSql ,GroupField" +
                   " FROM IF_Task Where Task_id ='" + _Task_id + "' order by id";
                //ls_sql = string.Format(ls_sql, "导入" + Project.DB_Alias, "导入" + Project.theirDB_Alias);
                ldt = new DataTable();
                if (AccessDBop.SQLSelect(ls_sql, ref ldt))
                {
                    if (ldt.Rows.Count > 0)
                    {
                        DirectionTrans(ldt);
                        t_task.Text = ldt.Rows[0]["Task_id"].ToString();
                        t_taskname.Text = ldt.Rows[0]["Task_Name"].ToString();
                    }
                    dgv_task.DataSource = ldt;
                    if (Isloading)
                        gd_dt = ldt.Copy();
                }
            }
            else
            {
                t_task.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
                _Task_id = t_task.Text;
                ls_sql = "SELECT id,Task_id, Task_Name, TheirTableName,TheirSql, OurTableName, OurTable_desc, OurSql,interfaceDesc,direction,'删除' as del ,IncrementInsert ,DeleteNotDrop,DropTable,GroupSql ,GroupField" +
                    " FROM IF_Task Where 1 < 0";
                //ls_sql = string.Format(ls_sql, "导入" + Project.DB_Alias, "导入" + Project.theirDB_Alias);
                ldt = new DataTable();
                if (AccessDBop.SQLSelect(ls_sql, ref ldt))
                {
                    dgv_task.DataSource = ldt;
                    if (Isloading)
                        gd_dt = ldt.Copy();
                }
            }
        }

        private void b_addtable_Click(object sender, EventArgs e)
        {
            SelectTable st = new SelectTable(t_task.Text);
            if (st.ShowDialog() == DialogResult.OK)
            {
                RetrieveData();
            }
        }

        private void b_save_Click(object sender, EventArgs e)
        {
            bool NeedTable = false;
            bool NeedField = false;
            bool NeedChecked = false;
            string NeedTableName = "";
            //SetDGVColumn_Direction(1);
            DataTable ldt1 = (DataTable)dgv_task.DataSource;
            foreach (DataRow ldr in ldt1.Rows)
                if (ldr.RowState != DataRowState.Deleted)
                {
                    if (ldr["TheirTableName"].ToString() == "")
                        NeedTable = true;
                    if (ldr["interfaceDesc"].ToString() == "")
                    {
                        //如果是exe方式取数据，则不需要配置规则
                        if (!ldr["TheirTableName"].ToString().Contains(".exe"))
                        {
                            NeedField = true;
                            NeedTableName = ldr["TheirTableName"].ToString();
                            break;
                        }
                    }
                    if (
                        (ldr["IncrementInsert"].ToString().ToLower() == "n")
                        && (ldr["DeleteNotDrop"].ToString().ToLower() == "n")
                        && (ldr["DropTable"].ToString().ToLower() == "n")
                        )
                    {
                        NeedChecked = true;
                        break;
                    }
                    ldr["Task_id"] = t_task.Text;
                    ldr["Task_Name"] = t_taskname.Text;
                    string sTemp = "导入" + Project.DB_Alias;
                    ldr["direction"] = ldr["direction"].ToString() == sTemp ? "0" : "1";
                }
            if (NeedTable)
            {
                MessageBox.Show("请完全输入对方表名");
                return;
            }
            if (NeedField)
            {
                MessageBox.Show("对方表[" + NeedTableName + "]未配置导入规则");
                return;
            }
            if (NeedChecked)
            {
                MessageBox.Show("未进行选择数据导入方式!");
                return;
            }
            string ls_sql = "SELECT id,Task_id, Task_Name, TheirTableName,TheirSql, OurTableName, OurTable_desc, OurSql,interfaceDesc,direction,IncrementInsert ,DeleteNotDrop,DropTable,GroupSql ,GroupField " +
                    " FROM IF_Task Where Task_id ='" + t_task.Text + "'"; ;
            if (!AccessDBop.SQLUpdate(ls_sql, ref ldt1))
            {
                MessageBox.Show("更新列时出错！");
                return;
            }
            //else
            //{ MessageBox.Show("更新成功！"); }
            DialogResult = DialogResult.OK;
        }

        private void b_close_Click(object sender, EventArgs e)
        {
            string ls_sql = "SELECT id,Task_id, Task_Name, TheirTableName,TheirSql, OurTableName, OurTable_desc, OurSql,interfaceDesc,direction,IncrementInsert ,DeleteNotDrop,DropTable,GroupSql ,GroupField " +
                    " FROM IF_Task Where Task_id ='" + t_task.Text + "'";
            foreach (DataRow dr in gd_dt.Rows)
            {
                dr.SetAdded();
            }
            string ls_sql1 = "delete FROM IF_Task Where Task_id ='" + t_task.Text + "'";
            if (AccessDBop.SQLExecute(ls_sql1))
            {
                if (!AccessDBop.SQLUpdate(ls_sql, ref gd_dt))
                {
                    MessageBox.Show("取消更新时出错！");
                    return;
                }
            }
            DialogResult = DialogResult.Cancel;
        }

        private void dgv_task_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 10)
            {
                if (MessageBox.Show("确定要删除此关系吗？", "删除提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                   == DialogResult.OK)
                {
                    //获取连接信息
                    string ls_sql = "delete from IF_Task where Task_id = '"
                        + t_task.Text + "' and OurTableName = '" + dgv_task["OurTableName", e.RowIndex].Value.ToString() + "'";
                    AccessDBop.SQLExecute(ls_sql);
                    RetrieveData();
                }
            }
            else if (e.ColumnIndex == 11)
            {
                if ((bool)dgv_task[e.ColumnIndex, e.RowIndex].EditedFormattedValue == false)
                {
                    dgv_task["DeleteNotDrop", e.RowIndex].Value = "N";
                    dgv_task["DropTable", e.RowIndex].Value = "N";
                }
            }
            else if (e.ColumnIndex == 12)
            {
                if ((bool)dgv_task[e.ColumnIndex, e.RowIndex].EditedFormattedValue == false)
                {
                    dgv_task["IncrementInsert", e.RowIndex].Value = "N";
                    dgv_task["DropTable", e.RowIndex].Value = "N";
                }
            }
            else if (e.ColumnIndex == 13)
            {
                if ((bool)dgv_task[e.ColumnIndex, e.RowIndex].EditedFormattedValue == false)
                {
                    dgv_task["DeleteNotDrop", e.RowIndex].Value = "N";
                    dgv_task["IncrementInsert", e.RowIndex].Value = "N";
                }
            }
        }

        private void dgv_task_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            if ((Project.DB_Type == "exe") || (Project.TheirDB_Type == "exe"))
            {
                return;
            }
            string ls_TheirTableName = dgv_task["TheirTableName", e.RowIndex].Value.ToString();
            string ls_OurTableName = dgv_task["OurTableName", e.RowIndex].Value.ToString();
            string ls_interfaceDesc = dgv_task["interfaceDesc", e.RowIndex].Value.ToString();
            string ls_direction = (dgv_task["direction", e.RowIndex].Value.ToString() == "导入" + Project.DB_Alias ? "0" : "1");

            //if (ls_TheirTableName == "")
            //{
            //    MessageBox.Show("请输入对方表名");
            //    return;
            //}
            FrmFieldTranse fft = new FrmFieldTranse(ls_TheirTableName, ls_OurTableName, ls_interfaceDesc, ls_direction);
            if (fft.ShowDialog() == DialogResult.OK)
            {
                dgv_task["interfaceDesc", e.RowIndex].Value = fft.gs_interfaceDesc;
                dgv_task["TheirSql", e.RowIndex].Value = fft.gs_TheirSql;
                dgv_task["OurSql", e.RowIndex].Value = fft.gs_OurSql;
                dgv_task["GroupSql", e.RowIndex].Value = fft.gs_GroupSql;
                dgv_task["GroupField", e.RowIndex].Value = fft.gs_GroupField;
                dgv_task["TheirTableName", e.RowIndex].Value = fft.gs_TheirTableName;
            }
        }

        private void dgv_task_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 9)
                {
                    string ls_TheirTableName = dgv_task["TheirTableName", e.RowIndex].Value.ToString();
                    string ls_OurTableName = dgv_task["OurTableName", e.RowIndex].Value.ToString();
                    string ls_interfaceDesc = dgv_task["interfaceDesc", e.RowIndex].Value.ToString();
                    string ls_direction = (dgv_task["direction", e.RowIndex].Value.ToString() == "导入" + Project.DB_Alias ? "0" : "1");

                    if (ls_TheirTableName == "")
                    {
                        return;
                    }
                    FrmFieldTranse fft = new FrmFieldTranse(ls_TheirTableName, ls_OurTableName, ls_interfaceDesc, ls_direction);
                    fft.LoadData();
                    fft.GetRuleSQL(ref fft.gs_TheirSql, ref fft.gs_OurSql, ref fft.gs_GroupSql);
                    dgv_task["TheirSql", e.RowIndex].Value = fft.gs_TheirSql;
                    dgv_task["OurSql", e.RowIndex].Value = fft.gs_OurSql;
                    dgv_task["GroupSql", e.RowIndex].Value = fft.gs_GroupSql;
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgv_task.CurrentRow == null)
                return;
            DataTable aFolderDocsTable = (DataTable)dgv_task.DataSource;
            //aFolderDoscTable是帮定在DataGrid里面的DataTable
            if (dgv_task.CurrentRow.Index == -1)
            {
                return;
            }
            if (dgv_task.CurrentRow.Index == 0)
            {
                return;
            }
            if (dgv_task.CurrentRow.Index > 0)
            {
                DataRow tempRow = aFolderDocsTable.NewRow();
                DataRow tempRow1 = aFolderDocsTable.NewRow();
                //新增一条row，来保存所选定的DataRow
                for (int i = 0; i < aFolderDocsTable.Columns.Count; i++)
                {
                    tempRow[i] = aFolderDocsTable.Rows[dgv_task.CurrentRow.Index][i];
                    tempRow1[i] = aFolderDocsTable.Rows[dgv_task.CurrentRow.Index - 1][i];
                }

                for (int i = 0; i < aFolderDocsTable.Columns.Count; i++)
                {
                    aFolderDocsTable.Rows[dgv_task.CurrentRow.Index][i] = tempRow1[i];
                    aFolderDocsTable.Rows[dgv_task.CurrentRow.Index - 1][i] = tempRow[i];
                }
                dgv_task.CurrentCell = dgv_task[3, dgv_task.CurrentRow.Index - 1];
                dgv_task.DataSource = aFolderDocsTable;
                //重新绑定
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgv_task.CurrentRow == null)
                return;
            DataTable aFolderDocsTable = (DataTable)dgv_task.DataSource;
            //aFolderDoscTable是帮定在DataGrid里面的DataTable
            if (dgv_task.CurrentRow.Index == aFolderDocsTable.Rows.Count - 1)
            {
                return;
            }
            else if (dgv_task.CurrentRow.Index == -1)
            {
                return;
            }

            else
            {
                DataRow tempRow = aFolderDocsTable.NewRow();
                DataRow tempRow1 = aFolderDocsTable.NewRow();
                //新增一条row，来保存所选定的DataRow
                for (int i = 0; i < aFolderDocsTable.Columns.Count; i++)
                {
                    tempRow[i] = aFolderDocsTable.Rows[dgv_task.CurrentRow.Index][i];
                    tempRow1[i] = aFolderDocsTable.Rows[dgv_task.CurrentRow.Index + 1][i];
                }

                for (int i = 0; i < aFolderDocsTable.Columns.Count; i++)
                {
                    aFolderDocsTable.Rows[dgv_task.CurrentRow.Index][i] = tempRow1[i];
                    aFolderDocsTable.Rows[dgv_task.CurrentRow.Index + 1][i] = tempRow[i];
                }
                dgv_task.CurrentCell = dgv_task[3, dgv_task.CurrentRow.Index + 1];

                dgv_task.DataSource = aFolderDocsTable;
                //重新绑定
            }
        }


        private void dgv_task_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if ((e.ColumnIndex == 3 || e.ColumnIndex == 5) && e.RowIndex >= 0)
            {
                DataGridViewCell dgvc = dgv_task.Rows[e.RowIndex].Cells[e.ColumnIndex];
                Rectangle rect = dgv_task.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                comb = new ComboBox();
                comb.Location = new Point(rect.X, rect.Y);
                //comb.DropDownStyle = ComboBoxStyle.DropDownList;
                comb.Width = rect.Width;
                comb.Height = rect.Height;
                comb.Font = new Font(new FontFamily("宋体"), (float)((rect.Height * 0.6) - 2));
                comb.DisplayMember = "Table_Name";
                dgv_task.Controls.Add(comb);
                if (e.ColumnIndex == 3)
                {
                    comb.DataSource = dtTheirTableName;
                    for (int i = 0; i < dtTheirTableName.Rows.Count; i++)
                    {
                        if (dtTheirTableName.Rows[i]["Table_Name"].ToString() == dgvc.Value.ToString())
                        {
                            comb.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else if (e.ColumnIndex == 5)
                {
                    comb.DataSource = dtOurTableName;
                    for (int i = 0; i < dtOurTableName.Rows.Count; i++)
                    {
                        if (dtOurTableName.Rows[i]["Table_Name"].ToString() == dgvc.Value.ToString())
                        {
                            comb.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void dgv_task_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 3 || e.ColumnIndex == 5) && e.RowIndex >= 0)
            {
                DataGridViewCell dgvc = dgv_task.Rows[e.RowIndex].Cells[e.ColumnIndex];
                //DataRowView dgv = (DataRowView)comb.SelectedItem;
                //if (dgv != null)
                //{
                //    dgvc.Value = dgv[0].ToString();
                //}
                //else
                //{
                //    dgvc.Value = comb.Text;
                //}
                dgvc.Value = comb.Text;
                dgv_task.Controls.Remove(comb);
                comb.Dispose();
                comb = null;
            }
        }
    }
}