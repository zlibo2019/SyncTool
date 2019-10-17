using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class ShowInfo : Form
    {
        string _Info = "";
        public ShowInfo()
        {
            InitializeComponent();
        }

        public ShowInfo(string Info)
        {
            InitializeComponent();
            _Info = Info;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btCopy_Click(object sender, EventArgs e)
        {
            if (rtbInfo.Text.Trim() != "")
            {
                Clipboard.SetDataObject(rtbInfo.Text);
                MessageBox.Show("文本已复制到剪切板");         
            }
        }

        private void ShowInfo_Load(object sender, EventArgs e)
        {
            rtbInfo.Text = _Info;
        }
    }
}
