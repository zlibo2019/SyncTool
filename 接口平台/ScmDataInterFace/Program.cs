using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace ScmDataInterFace
{
    static class Program
    {
        private static Mutex mutex = null;  //设为Static成员，是为了在整个程序生命周期内持有Mutex
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string sResult = ApplicationSetting.initLanguageBase();
            if (sResult != "")
            {
                MessageBox.Show(sResult);
            }
            bool firstInstance;
            string s = Application.StartupPath;
            s = s.Replace('\\','a');
            mutex = new Mutex(true,s, out firstInstance);
            try
            {
                if (!firstInstance)
                {
                    MessageBox.Show(ApplicationSetting.Translate("the same exe is running"), ApplicationSetting.Translate("prompt"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Application.Exit();
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());
            }
            finally
            {
                //只有第一个实例获得控制权，因此只有在这种情况下才需要ReleaseMutex，否则会引发异常。
                if (firstInstance)
                {
                    mutex.ReleaseMutex();
                }
                mutex.Close();
                mutex = null;
            }
        }
    }
}