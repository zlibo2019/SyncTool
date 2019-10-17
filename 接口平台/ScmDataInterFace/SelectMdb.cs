using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ScmDataInterFace
{
    public partial class frmSelectMdbFile : Form
    {
        public frmSelectMdbFile()
        {
            InitializeComponent();
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = folder.SelectedPath;
            }
        }

        private void btOk_Click_1(object sender, EventArgs e)
        {
            string SourcePath = tbPath.Text.Trim();
            if (SourcePath.Equals(""))
            {
                MessageBox.Show("请选择路径!");
                return;
            }
            string SourceFile1 = SourcePath + @"\configBase.mdb";
            string SourceFile2 = SourcePath + @"\ScmDataInterFace.mdb";
            string destFile1 = Application.StartupPath + @"\configBase.mdb";
            string destFile2 = Application.StartupPath + @"\ScmDataInterFace.mdb";
            if (File.Exists(SourceFile1))
            {
                File.Copy(SourceFile1, destFile1);
            }
            if (File.Exists(SourceFile2))
            {
                File.Copy(SourceFile2, destFile2);
            }
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
