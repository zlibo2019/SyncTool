using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        //private string LoadUser = "";
        //public string list_returndep = "";//返回改管理员组的部门权限

        DataTable dt_gly = new DataTable();//管理员权限对应表
        //DataTable dt_group = new DataTable();//权限部门对应表
        //DataTable dt_gly_group = new DataTable();//权限部门对应表

        private void frmLogin_Load(object sender, EventArgs e)
        {
            cmb_login.SelectedIndex = 1;
            //loadGly();   
        }

        private void loadGly()
        {
            //string sql =  "Select a.[Gly_no], a.[Gly_pass], b.[Group_no], b.[Group_lname] From [WT_GLY] as a left join [WT_GLY_GROUP] as b on a.[Gly_group] = b.[Group_no]";
            string sql = "Select [Gly_no],[Gly_pass] From [WT_GLY]";
            bool bResult = SQLServerDBop.SQLSelect(Project.DB_Connection, sql, ref dt_gly, "");           
        }

  


        private void btn_Load_Click(object sender, EventArgs e)
        {
            //if (Project.DB_Type == "webservice")
            //{
            //    bool bResult = TaskManager.verify(cmb_login.Text, tb_Password.Text);
            //    if (!bResult)
            //    {
            //        MessageBox.Show("用户名或密码错误!");
            //        return;
            //    }
            //}
            //else
            //{
            //    loadGly(); 
            //    DataRow[] dr = dt_gly.Select("Gly_no = '" + cmb_login.Text + "'");
            //    if (dr.Length <= 0)
            //    {
            //        MessageBox.Show("无此管理员。");
            //        return;
            //    }
            //    else
            //    {
            //        if (tb_Password.Text != dr[0]["Gly_pass"].ToString())
            //        {
            //            MessageBox.Show("用户名或密码错误!");
            //            return;
            //        }
            //    }
            //}
            if ((cmb_login.Text == "admin" && tb_Password.Text == "1234") || (cmb_login.Text != "admin"))
            {
                Project.login = cmb_login.Text;
                Project.LoginPass = tb_Password.Text;
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                MessageBox.Show("密码错误!");
            }
        }

        private void btn_quit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void cmb_login_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_login.Text.ToLower().Equals("user"))
            {
                tb_Password.Enabled = false;
            }
            else
            {
                tb_Password.Enabled = true;
            }
        }
    }
}
