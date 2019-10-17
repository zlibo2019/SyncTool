using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace ScmDataInterFace
{
    public partial class FrmSetPhoto : Form
    {
        DataTable ldt;
        public FrmSetPhoto()
        {
            InitializeComponent();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void b_ok_Click(object sender, EventArgs e)
        {
            ldt.Rows[0]["StoreType"] = (r_database2file.Checked ? "1" : (r_file2file.Checked ? "2" : "3"));
            ldt.Rows[0]["GetType"] = (r_fromtable.Checked ? "1" : "2");

            ldt.Rows[0]["TableName"] = TableName.Text;
            ldt.Rows[0]["XH"] = XH.Text;
            ldt.Rows[0]["ZP"] = ZP.Text;
            ldt.Rows[0]["XHSQL"] = XHSQL.Text;
            ldt.Rows[0]["ZPSQL"] = ZPSQL.Text;
            ldt.Rows[0]["FROMSQL"] = FROMSQL.Text;
            ldt.Rows[0]["WHERESQL"] = WHERESQL.Text;
            //ldt.Rows[0]["db_name"] = (rbBs.Checked ? "bs" : "scm");
            //ldt.Rows[0]["db_position"] = (cbPosition.Checked ? "0" : "1");
            if (r_database2file.Checked)
            {
                ldt.Rows[0]["Compress"] = r_enable.Checked ? "1" : "0";
                ldt.Rows[0]["CompressRate"] = CompressRate.Value;
                ldt.Rows[0]["BsPath"] = BsPath.Text;
                ldt.Rows[0]["OUR_XH"] = OurXh.Text;
                ldt.Rows[0]["ZPIsPath"] = cbZpIsPath.Checked ? "1" : "0";
            }
            else if(r_file2file.Checked)
            {
                ldt.Rows[0]["Compress"] = r_enable1.Checked ? "1" : "0";
                ldt.Rows[0]["CompressRate"] = CompressRate1.Value;
                ldt.Rows[0]["BsPath"] = BsPath1.Text;
                ldt.Rows[0]["OUR_XH"] = OurXh2.Text;
            }
            else if (r_file2database.Checked)
            {
                ldt.Rows[0]["TableName"] = tbTableName.Text;
                ldt.Rows[0]["XH"] = tbXH.Text;
                ldt.Rows[0]["ZP"] = tbZP.Text;
                ldt.Rows[0]["BsPath"] = tbFilePath.Text;
            }
            ldt.Rows[0]["width"] = nudWidth.Value;
            ldt.Rows[0]["height"] = nudHeight.Value;
            ldt.Rows[0]["FileToXHType"] = r_XH.Checked ? "0" : "1";
            ldt.Rows[0]["PhotoNumbers"] = PhotoNumbers.Text;
            ldt.Rows[0]["TheirPhotoPath"] = TheirPhotoPath.Text;
            ldt.Rows[0]["StartPos"] = StartPos.Text;
            ldt.Rows[0]["EndPos"] = EndPos.Text;

            string ls_sql = "SELECT *" +
                  " FROM IF_PHOTO where serial = 1";

            AccessDBop.SQLExecute("delete from IF_Task where Task_id = '10000000000000'");
            AccessDBop.SQLExecute("insert into IF_Task(Task_id,Task_Name,direction,IncrementInsert) values('10000000000000','" + t_taskname.Text + "','0','N')");
            if (AccessDBop.SQLUpdate(ls_sql, ref ldt))
            {
                MessageBox.Show("保存成功");
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("保存失败");
            }
        }

        private void b_cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void FrmSetPhoto_Load(object sender, EventArgs e)
        {
            RetrieveData();
            //string a = Encrypt("123456","nitcnitc");
        }

        //public static string Encrypt(string Atext, string Key)
        //{
        //    DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
        //    byte[] bytes = Encoding.Default.GetBytes(Atext);
        //    provider.Key = Encoding.ASCII.GetBytes(Key);
        //    provider.IV = Encoding.ASCII.GetBytes(Key);
        //    MemoryStream stream = new MemoryStream();
        //    CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
        //    stream2.Write(bytes, 0, bytes.Length);
        //    stream2.FlushFinalBlock();
        //    StringBuilder builder = new StringBuilder();
        //    foreach (byte num in stream.ToArray())
        //    {
        //        builder.AppendFormat("{0:X2}", num);
        //    }
        //    return builder.ToString();
        //}

        void SetLabelShow()
        {
            label1.Text = Project.theirDB_Alias + "照片表表名：";
            label2.Text = Project.theirDB_Alias + "照片表中人员字段名：";
            label3.Text = Project.theirDB_Alias + "照片表中照片字段名：";
            label11.Text = Project.DB_Alias + "人员表中对应其人员的字段名：";
            label4.Text = Project.DB_Alias + "存储路径：";
            label20.Text = Project.theirDB_Alias + "原始照片文件路径：";
            label21.Text = Project.theirDB_Alias + "文件名称与工号关系：";
            label22.Text = Project.DB_Alias + "存储路径：";
            label43.Text = Project.theirDB_Alias + "照片字段名：";
            label44.Text = Project.theirDB_Alias + "主键字段名：";
            label45.Text = Project.theirDB_Alias + "照片表表名：";
            label48.Text = Project.DB_Alias + "照片路径：";
        }

        private void RetrieveData()
        {
            SetLabelShow();
            string ls_sql = "";

            //初始化datagird
            ls_sql = "SELECT id,Task_id, Task_Name" +
                " FROM IF_Task Where Task_id ='10000000000000'";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                if (ldt.Rows.Count > 0)
                {
                    t_taskname.Text = ldt.Rows[0]["Task_Name"].ToString();
                }
            }
            ls_sql = "SELECT *" +
                    " FROM IF_PHOTO where serial = 1";
            ldt = new DataTable();
            if (AccessDBop.SQLSelect(ls_sql, ref ldt))
            {
                if (ldt.Rows.Count > 0)
                {
                    if (ldt.Rows[0]["StoreType"].ToString() == "1")
                    {
                        r_database2file.Checked = true;
                        OurXh.Text = ldt.Rows[0]["OUR_XH"].ToString();
                        cbZpIsPath.Checked = ldt.Rows[0]["ZPIsPath"].ToString() == "1" ? true : false;
                        if (ldt.Rows[0]["Compress"].ToString() == "0")
                        {
                            r_disable.Checked = true;
                        }
                        else
                        {
                            r_enable.Checked = true;
                        }
                    }
                    else if (ldt.Rows[0]["StoreType"].ToString() == "2")
                    {
                        r_file2file.Checked = true;
                        OurXh2.Text = ldt.Rows[0]["OUR_XH"].ToString();
                        if (ldt.Rows[0]["Compress"].ToString() == "0")
                        {
                            r_disable1.Checked = true;
                        }
                        else
                        {
                            r_enable1.Checked = true;
                        }
                    }
                    else if (ldt.Rows[0]["StoreType"].ToString() == "3")
                    {
                        r_file2database.Checked = true;
                        tbTableName.Text = ldt.Rows[0]["TableName"].ToString();
                        tbXH.Text = ldt.Rows[0]["XH"].ToString();
                        tbZP.Text = ldt.Rows[0]["ZP"].ToString();
                        tbFilePath.Text = ldt.Rows[0]["BsPath"].ToString();
                    }
                    if (ldt.Rows[0]["GetType"].ToString() == "1")
                    {
                        r_fromtable.Checked = true;
                    }
                    else
                    {
                        r_fromsql.Checked = true;
                    }
                    //if (ldt.Rows[0]["db_name"].ToString() == "bs")
                    //{
                    //    rbBs.Checked = true;
                    //}
                    //else
                    //{
                    //    rbScm.Checked = true;
                    //}
                    //if (ldt.Rows[0]["db_position"].ToString() == "0")
                    //{
                    //    cbPosition.Checked = true;
                    //}
                    //else
                    //{
                    //    cbPosition.Checked = false;
                    //}
                    if (ldt.Rows[0]["FileToXHType"].ToString() == "0")
                    {
                        r_XH.Checked = true;
                    }
                    else
                    {
                        r_Get_XH.Checked = true;
                    }
                    TableName.Text = ldt.Rows[0]["TableName"].ToString();
                    XH.Text = ldt.Rows[0]["XH"].ToString();
                    ZP.Text = ldt.Rows[0]["ZP"].ToString();
                    XHSQL.Text = ldt.Rows[0]["XHSQL"].ToString();
                    ZPSQL.Text = ldt.Rows[0]["ZPSQL"].ToString();
                    FROMSQL.Text = ldt.Rows[0]["FROMSQL"].ToString();
                    WHERESQL.Text = ldt.Rows[0]["WHERESQL"].ToString();
                    ZPSQL.Text = ldt.Rows[0]["ZPSQL"].ToString();
                    CompressRate.Value = Convert.ToDecimal(ldt.Rows[0]["CompressRate"].ToString());
                    CompressRate1.Value = Convert.ToDecimal(ldt.Rows[0]["CompressRate"].ToString());
                    string sValue = ldt.Rows[0]["width"].ToString().Trim();
                    sValue = sValue == "" ? "140" : sValue;
                    nudWidth.Value = Convert.ToDecimal(sValue);
                    sValue = ldt.Rows[0]["height"].ToString().Trim();
                    sValue = sValue == "" ? "194" : sValue;
                    nudHeight.Value = Convert.ToDecimal(sValue);
                    PhotoNumbers.Text = ldt.Rows[0]["PhotoNumbers"].ToString();
                    BsPath.Text = ldt.Rows[0]["BsPath"].ToString();
                    BsPath1.Text = ldt.Rows[0]["BsPath"].ToString();
                    TheirPhotoPath.Text = ldt.Rows[0]["TheirPhotoPath"].ToString();
                    StartPos.Text = ldt.Rows[0]["StartPos"].ToString();
                    EndPos.Text = ldt.Rows[0]["EndPos"].ToString();
                }
            }
        }

        private void r_database_CheckedChanged(object sender, EventArgs e)
        {
            if (r_database2file.Checked)
            {
                tabControl1.SelectedIndex = 0;
            }
            if (r_file2file.Checked)
            {
                tabControl1.SelectedIndex = 1;
            }
            if (r_file2database.Checked)
            {
                tabControl1.SelectedIndex = 2;
            }
        }

        private void r_fromtable_CheckedChanged(object sender, EventArgs e)
        {
            if (r_fromtable.Checked)
            {
                panel3.Enabled = true;
                panel4.Enabled = false;
            }
            if (r_fromsql.Checked)
            {
                panel3.Enabled = false;
                panel4.Enabled = true;
            }
        }

        private void r_enable_CheckedChanged(object sender, EventArgs e)
        {
            if (r_disable.Checked)
            {
                panel2.Enabled = false;
            }
            if (r_enable.Checked)
            {
                panel2.Enabled = true;
            }
        }

        private void r_disable1_CheckedChanged(object sender, EventArgs e)
        {
            if (r_disable1.Checked)
            {
                panel6.Enabled = false;
            }
            if (r_enable1.Checked)
            {
                panel6.Enabled = true;
            }
        }

        private void r_XH_CheckedChanged(object sender, EventArgs e)
        {
            if (r_XH.Checked)
            {
                StartPos.Enabled = false;
                EndPos.Enabled = false;
            }
            else
            {
                StartPos.Enabled = true;
                EndPos.Enabled = true;
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
    }
}