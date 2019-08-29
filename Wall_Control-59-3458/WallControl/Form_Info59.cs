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
    public partial class Form_Info59 : Form
    {
        public string Str = "";
        private MainForm f;
        private IniFiles settingFile;
        private int num;
        private IniFiles languageFile;
        public Form_Info59(MainForm f, int num)
        {
            this.f = f;
            this.num = num;
            InitializeComponent();
            serialPort1.PortName = f.PortName;
            serialPort1.BaudRate = f.BaudRate;
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);

            try
            {
                serialPort1.Open();
            }
            catch
            {
                serialPort1.Close();
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            } 
            this.Text = languageFile.ReadString("INFOFORM", "TITLE", "版本信息");
            this.groupBox1.Text = languageFile.ReadString("INFOFORM", "INFO", "信息");
            this.label2.Text = languageFile.ReadString("INFOFORM", "SV", "版本信息");
            this.button1.Text = languageFile.ReadString("INFOFORM", "UD", "更新显示");
            
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
            
            string s1 = settingFile.ReadString("SETTING", "Ver", "");
            if (s1.Length != 0)
                label1.Text = s1;
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Info59));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            resources.ApplyResources(button1, button1.Name);
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
            //for (int i = 0; i < 256; i++)
            {
                //if (f.select_address[i] != 0)
                {
                    byte[] array = new byte[5];
                    array[0] = 0xE5;
                    array[1] = (byte)num;//f.select_address[i];
                    array[2] = 0x20;
                    array[3] = A;
                    array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
                    try
                    {
                        if (serialPort1.IsOpen)
                        {
                            SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 2);
                            f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                            f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                            f.richTextBox2.ScrollToCaret();
                        }
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

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Init(0x25);
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Str = "";
            Thread.Sleep(500);
            Str = serialPort1.ReadExisting();
            //Console.WriteLine("all time ="+ Str+Str.Length);
            if (Str.Length >= 11)
            {
                this.BeginInvoke(new MethodInvoker(delegate()
                {
                    label1.Text = Str.Substring(0, 11);
                    label1.ForeColor = Color.Red;
                    button1.Enabled = true;
                }));
            }
        }

        private void Form_Info59_FormClosing(object sender, FormClosingEventArgs e)
        {
            Str = "";
            settingFile.WriteString("SETTING", "Ver", label1.Text);
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Init(0xF6);
        }
    }
}
