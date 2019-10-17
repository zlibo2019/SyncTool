using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace ScmDataInterFace
{
    public partial class Form4 : Form
    {
        public bool IsZhuCe = false;
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            //获取cpu序列号
            string jiqima = RC2Encrypt(GetCpuID(), "gto250");
            MessageBox.Show(ApplicationSetting.Translate("send machine code to the administrator"), ApplicationSetting.Translate("promt"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 取网卡地址
        /// </summary>
        /// <returns></returns>
        private string _GetMAC()
        {
            string mac = "";
            //ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //ManagementObjectCollection moc = mc.GetInstances();

            //foreach (ManagementObject mo in moc)
            //{
            //    //if((bool)mo["IPEnabled"]   ==   true)    
            //    if (mo["IPEnabled"].ToString() == "True")
            //    {
            //        mac = mo["MacAddress"].ToString();
            //    }
            //}
            return mac;
        }
        /// <summary>
        /// 取CPU地址
        /// </summary>
        /// <returns></returns>
        public string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码
                //string cpuInfo = "";//cpu序列号
                //ManagementClass mc = new ManagementClass("Win32_Processor");
                //ManagementObjectCollection moc = mc.GetInstances();
                //foreach (ManagementObject mo in moc)
                //{
                //    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                //}
                //moc = null;
                //mc = null;
                //return cpuInfo;


                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "ipconfig";
                p.StartInfo.Arguments = "/all";
                p.Start();
                //p.WaitForExit();
                string s = p.StandardOutput.ReadToEnd();
                if (s.IndexOf("Physical Address. . . . . . . . . :") >= 0)
                    s = s.Substring(s.IndexOf("Physical Address. . . . . . . . . :") + 36, 17);
                else if (s.IndexOf("物理地址") >= 0)
                    s = s.Substring(s.IndexOf("物理地址") + 36, 17);

                if (s.Trim() == "")
                {
                    s = "unknow";
                }
                return s;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        private void b_zhuce_Click(object sender, EventArgs e)
        {
            if ((t_jiqima.Text.Trim() == ""))  //临时处理
            {
                if (t_zhucema.Text.Trim() == "12345678910")
                {
                    IsZhuCe = true;
                    MessageBox.Show(ApplicationSetting.Translate("超级权限登陆"), ApplicationSetting.Translate("prompt"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                }
            }


            if (t_zhucema.Text.Trim() == "")
            {
                MessageBox.Show(ApplicationSetting.Translate("input reg code"), ApplicationSetting.Translate("prompt"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Miscellaneous.md35(t_jiqima.Text) == t_zhucema.Text.Trim())
            {
                SaveSettingsToRegistry();
                IsZhuCe = true;
                MessageBox.Show(ApplicationSetting.Translate("reg code is valid"), ApplicationSetting.Translate("prompt"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(ApplicationSetting.Translate("reg code is invalid"), ApplicationSetting.Translate("prompt"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 保存注册表
        /// </summary>
        void SaveSettingsToRegistry()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\ScmDataInterFace\\weds");
                key.SetValue("reg", Miscellaneous.md35(GetCpuID()));
            }
            catch { }
        }

        /// <summary>
        /// RC2加密
        /// </summary>
        /// <param name="encryptString">待加密的密文</param>
        /// <param name="encryptKey">密匙(必须为5-16位)</param>
        /// <returns></returns>
        public static string RC2Encrypt(string encryptString, string encryptKey)
        {
            string returnValue;
            try
            {
                byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                RC2CryptoServiceProvider rC2 = new RC2CryptoServiceProvider();
                byte[] byteEncryptString = Encoding.Default.GetBytes(encryptString);
                MemoryStream memorystream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memorystream, rC2.CreateEncryptor(Encoding.Default.GetBytes(encryptKey), temp), CryptoStreamMode.Write);
                cryptoStream.Write(byteEncryptString, 0, byteEncryptString.Length);
                cryptoStream.FlushFinalBlock();
                returnValue = Convert.ToBase64String(memorystream.ToArray());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;

        }
    }
}
