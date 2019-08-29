using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WallControl
{
    public partial class Form_Addr : Form
    {
        public MainForm f;
        private IniFiles settingFile;//配置文件
        private IniFiles languageFile;
        //private uint addr;
        public Form_Addr(MainForm f)
        {
            InitializeComponent();
            this.f = f;

            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            //numericUpDown1.Value = f.address[f.address_backup];

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
            ApplyResource();

            this.Text = languageFile.ReadString("SERIAL", "TITLE", "拼接设置");
            this.label2.Text = languageFile.ReadString("SERIAL", "TIPS", "提示：在进行设备的序列号进行设置时，请确保串口线环接在断开状态；序列号设置只能单台设备进行设置！");
            this.label1.Text = languageFile.ReadString("SERIAL", "T1", "设备序列号设置：");
            this.button1.Text = languageFile.ReadString("SERIAL", "OK", "确认");
            this.button2.Text = languageFile.ReadString("SERIAL", "CANCEL", "取消");
            /*
            if (f.Chinese_English == 1)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
            }
             */ 
            textBox1.Text = f.address[f.address_backup].ToString();
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Addr));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void Save_number(int A_0)//单独设置屏幕的单元序号----指令
        {
            byte[] array = new byte[9];
            array[0] = 0xE9;
            array[1] = 0xFD;
            array[2] = 0x20;
            array[3] = 0x12;
            array[4] = (byte)(A_0 >> 16);
            array[5] = 0x59;
            array[6] = (byte)(A_0 >> 8);
            array[7] = (byte)A_0;
            array[8] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4] + array[5] + array[6] + array[7]));
            try
            {
                //serialPort1.Write(array, 0, 11);
                SerialPortUtil.serialPortSendData(f.serialPort1, array, 0, 9, 100, 2);
                f.richTextBox2.AppendText(MainForm.ToHexString(array, 9));
                f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                f.richTextBox2.ScrollToCaret();
            }
            catch
            {
                //Rs232Con = false;
                if (f.Chinese_English == 1)
                    MessageBox.Show("    Serial error！", "Tips");
                else
                    MessageBox.Show("    串口出错！", "提示");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool have = false;
            for (int i = 0; i < 128; i++)
            {
                if (f.address[i] == int.Parse(textBox1.Text))
                {
                    if (i == f.address_backup)
                        break;
                    have = true;
                    string ts = languageFile.ReadString("SERIAL", "T2", "此序列号已存在，是否确认覆盖！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    DialogResult t = MessageBox.Show(ts, tp);
                    /*
                    if (f.Chinese_English == 1)
                        t = MessageBox.Show(" This serial number already exists, whether to confirm the coverage！", " Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    else
                        t = MessageBox.Show(" 此序列号已存在，是否确认覆盖！", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);   
                     */
                    if (t == DialogResult.Yes || t == DialogResult.OK)
                    {
                        f.address[i] = 0;
                        f.Address = int.Parse(textBox1.Text);
                        settingFile.WriteInteger("ADDR", f.address_backup + "Addr:", int.Parse(textBox1.Text));
                    }
                    else
                        f.Address = f.address[f.address_backup];
                    break;
                }
            }
            if (!have)
            {
                f.Address = int.Parse(textBox1.Text);
                settingFile.WriteInteger("ADDR", f.address_backup + "Addr:", int.Parse(textBox1.Text));
            }
            //Console.WriteLine(f.Address);
            Save_number(f.Address);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            f.Address = int.Parse(textBox1.Text);
            this.Close();
        }

         private bool IsInts(string str)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            return rex.IsMatch(str);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                string ts = languageFile.ReadString("SERIAL", "T3", "设置的序列号输入不能为空！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                textBox1.Text = "0";
                return;
            }
            if (textBox1.Text.Length > 0)
            {
                if (!IsInts(textBox1.Text))
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "C2", "请输入有效数字字符组合(\"0~9\")");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    textBox1.Text = "0";
                    return;
                }
            }
            int t = int.Parse(textBox1.Text);
            if (t > 16777214)
            {
                string ts = languageFile.ReadString("SERIAL", "T4", "请设置序列号的正确范围（0 ~ 16777214）!");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                textBox1.Text = "0";
                return;
            }
        }
    }
}
