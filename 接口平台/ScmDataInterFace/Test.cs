using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScmDataInterFace
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
        }

        private void Test_Load(object sender, EventArgs e)
        {
            DataTable lds = ExcelHelper.GetExcelDataTable();
            dataGridView1.DataSource = lds;
            //bindingNavigator1. = lds;
        }
    }
}