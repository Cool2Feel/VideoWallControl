using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WallControl
{
    public partial class Form_Info : Form
    {
        public string Str = "";
        private MainForm f;
        private IniFiles languageFile;
        //private IniFiles settingFile;
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public Form_Info(MainForm f)
        {
            this.f = f;
            InitializeComponent();

            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            this.Text = languageFile.ReadString("INFOFORM", "TITLE", "版本信息");
            this.groupBox1.Text = languageFile.ReadString("INFOFORM", "INFO", "信息");
            this.label2.Text = languageFile.ReadString("INFOFORM", "SV", "版本信息");
            this.label3.Text = languageFile.ReadString("INFOFORM", "AWH", "系统运行总时间:");
            this.button2.Text = languageFile.ReadString("INFOFORM", "UD", "更新显示");
            this.button1.Text = languageFile.ReadString("INFOFORM", "SD", "屏幕显示");


            if (f.Chinese_English == 1)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
            }
            else if (f.Chinese_English == 0)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
                ApplyResource();
            }
            //serialPort1.PortName = f.PortName;
            //serialPort1.BaudRate = f.BaudRate;
            //settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            //Console.WriteLine(serialPort1.PortName + "," + serialPort1.BaudRate);
            /*
            try
            {
                serialPort1.Open();
            }
            catch
            {
                serialPort1.Close();
                if (f.Chinese_English)
                {
                    MessageBox.Show("   Serial open error。");
                    return;
                }
                else
                {
                    MessageBox.Show("   串口打开出错。");
                    return;
                }
            }
            if (f.Chinese_English)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
            }
            int s1 = settingFile.ReadInteger("SETTING", "Time_hour", 0);
            int s2 = settingFile.ReadInteger("SETTING", "Time_minute", 0);
            if (f.Chinese_English)
            {
                if (s1 > 0)
                    label4.Text = s1 + "/hour " + s2 + "/minute";
                else
                    label4.Text = s2+ "/minute ";
            }
            else
            {
                if (s1 > 0)
                    label4.Text = s1 + "/小时 " + s2 + "/分钟";
                else
                    label4.Text = s2 + "/分钟 ";
            }
             * */
            //Init();
            //Thread.Sleep(200);
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Info));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            resources.ApplyResources(button1, button1.Name);
            resources.ApplyResources(button2, button2.Name);
            /*
            foreach (Control ctl in groupBox1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
             */ 
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void Init(byte A)
        {
            for (int i = 0; i < 128; i++)
            {
                if (f.select_address[i] != 0)
                {
                    byte[] array = new byte[5];
                    array[0] = 0xE5;
                    array[1] = f.select_address[i];
                    array[2] = 0x20;
                    array[3] = A;
                    array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
                    try
                    {
                        if (f.Rs232Con)
                        {
                            if (f.TCPCOM)
                            {
                                f.TcpSendMessage(array, 0, 5);
                            }
                            else
                                SerialPortUtil.serialPortSendData(f.serialPort1, array, 0, 5, 100, 2);
                            f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                            f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                            f.richTextBox2.ScrollToCaret();
                        }
                        /*
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                        }));
                         */ 
                    }
                    catch
                    {
                        string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                    }
                }
            }

        }

        private void Form_Info_FormClosing(object sender, FormClosingEventArgs e)
        {
            Str = "";
            /*
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
             * */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //button1.Enabled = false;
            Init(0x25);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Init(0xF6);
            /*
            for (int i = 0; i < 128; i++)
            {
                if (f.select_address[i] != 0)
                {
                    byte[] array = new byte[5];
                    array[0] = 0xE5;
                    array[1] = f.select_address[i]; ;
                    array[2] = 0x20;
                    array[3] = 0xF6;
                    array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
                    try
                    {
                        if (serialPort1.IsOpen)
                        {
                            SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 1);
                            //f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                        }
                        this.BeginInvoke(new MethodInvoker(delegate()
                        {
                            f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                        }));
                    }
                    catch
                    {
                        if (f.Chinese_English)
                            MessageBox.Show("   Serial open error！", "Tips");
                        else
                            MessageBox.Show("    串口出错！", "提示");
                    }
                }
            }
             * */
        }
    }
}
