using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace ScmDataInterFace
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = TransExcelToDataSet("E:\\vk\\平泉\\用户部门表\\dept.xls", new List<string>(new string[] { "Sheet1" })).Tables[0].DefaultView;
        }

        /// <summary>
        /// 将excel中指定sheet内容读入dataset
        /// </summary>
        /// <param name="fileName">excel文件路径</param>
        /// <param name="sheetNames">需从excel中读取的sheet名称</param>
        /// <returns></returns>
        public DataSet TransExcelToDataSet(string fileName, List<string> sheetNames)
        {
            OleDbConnection objConn = null;
            DataSet data = new DataSet();

            try
            {
                //创建读取excel连接
                string strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + fileName
                    + ";Extended Properties='Excel 8.0; HDR=YES; IMEX=1'";
                objConn = new OleDbConnection(strConn);
                objConn.Open();
                OleDbDataAdapter sqlada = new OleDbDataAdapter();
                //遍历从配置文件中读取的sheet名称
                foreach (string sheetName in sheetNames)
                {
                    string strSql = "select * --From [" + sheetName.Trim() + "$]";
                    OleDbCommand objCmd = new OleDbCommand(strSql, objConn);
                    sqlada.SelectCommand = objCmd;
                    //填充dataset
                    sqlada.Fill(data);
                }
            }
            catch (Exception e)
            {
                throw new Exception("将excel中指定sheet内容读入dataset出错！" + e.Message);
            }
            finally
            {
                if (objConn != null)
                {
                    objConn.Dispose();
                    objConn.Close();
                }
            }
            return data;
        }


    }
}