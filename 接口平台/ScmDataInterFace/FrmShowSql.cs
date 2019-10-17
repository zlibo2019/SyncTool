using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class FrmShowSql : Form
    {
        public FrmShowSql(string as_sql)
        {
            InitializeComponent();
            t_sql.Text = as_sql;
        }

        private void b_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(t_sql.Text);
        }
    }
}