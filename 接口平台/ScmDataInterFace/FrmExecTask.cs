using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class FrmExecTask : Form
    {
        public string gs_command = "";
        private string _command = "";
        public FrmExecTask(string as_command)
        {
            _command = as_command;
            InitializeComponent();
        }

        private void FrmExecTask_Load(object sender, EventArgs e)
        {
            RetrieveTask();
            if (_command != "")
            {
                cb_task.Text = _command.Split(new string[] { "{,}" }, StringSplitOptions.None)[0];
                t_sql.Text = _command.Split(new string[] { "{,}" }, StringSplitOptions.None)[1];
                rbOur.Checked = bool.Parse(_command.Split(new string[] { "{,}" }, StringSplitOptions.None)[2]);
                string mutexName = _command.Split(new string[] { "{,}" }, StringSplitOptions.None)[3].Trim();
                if (mutexName != string.Empty)
                {
                    cbIsMutex.Checked = true;
                    cmbMutexName.Text = mutexName;
                }
                rbTheir.Checked = !rbOur.Checked;
            }
            rbOur.Text = ApplicationSetting.Translate("run at") + ":" + Project.DB_Alias;
            rbTheir.Text = ApplicationSetting.Translate("run at") + ":" + Project.theirDB_Alias;
        }
        private void RetrieveTask()
        {
            string ls_sql = " SELECT distinct Task_id, Task_Name FROM IF_Task ";
            DataTable ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                DataRow dr = ldt.NewRow();
                ldt.Rows.InsertAt(dr, 0);
                cb_task.DisplayMember = "Task_id";
                cb_task.ValueMember = "Task_Name";
                cb_task.DataSource = ldt;
                cb_task.SelectedIndex = -1;
                cb_task.SelectedIndex = 0;
            }
        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            string mutexName = cbIsMutex.Checked ? cmbMutexName.Text : string.Empty;
            gs_command = cb_task.Text + "{,}" + t_sql.Text + "{,}" + rbOur.Checked + "{,}" + mutexName;
            DialogResult = DialogResult.OK;
        }

        private void b_cancel_Click(object sender, EventArgs e)
        {
            gs_command = "";
            DialogResult = DialogResult.Cancel;
        }

        private void cb_task_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void cb_task_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbIsMutex_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIsMutex.Checked)
            {
                panel4.Visible = true;
                if (cmbMutexName.SelectedIndex < 0)
                {
                    cmbMutexName.SelectedIndex = 0;
                }
            }
            else
            {
                panel4.Visible = false;
            }
        }
    }
}