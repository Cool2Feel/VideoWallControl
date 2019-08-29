using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WallControl
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isRuned;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out isRuned);
            if (isRuned)
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    //Form_Start f1 = new Form_Start();
                    //if (f1.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(new MainForm());
                        mutex.ReleaseMutex();
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine("message =" + e.Message);
                    ExceptionLog.getLog().WriteLogFile(e, "LogFile.txt");
                }
            }
            else
            {
                MessageBox.Show("        程序已启动！\r\n(Program has started)", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
