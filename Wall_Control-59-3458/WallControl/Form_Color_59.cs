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
    public partial class Form_Color_59 : Form
    {
        public MainForm f;
        public int B_R;
        public int B_G;
        public int B_B;
        public int O_R;
        public int O_G;
        public int O_B;
        public int Lum;
        public int Conrast;
        public int Saturation;
        public int BlackL;
        public int Clarity;
        private string Str = "";
        private bool flag_ch = true;
        private bool port_flag = false;
        private bool receive_flag = true;
        private bool flag = false;
        private IniFiles languageFile;
        //private Thread myThread = null;
        //private int num_k = 1;
        //private int receiveFlag = -1;//参数接受的标志,0表示完成，1表示接受COLOR_BALANCE:部分，2表示COLOR_DATA:，3表示CAPTURE DATA:
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public Form_Color_59(MainForm f,bool mf)
        {
            InitializeComponent();
            this.f = f;
            this.flag = mf;
            serialPort1.PortName = f.PortName;
            serialPort1.BaudRate = f.BaudRate;
            //Console.WriteLine("====="+flag);
            
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            try
            {
                serialPort1.Open();
                port_flag = true;
            }
            catch
            {
                serialPort1.Close();
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                port_flag = false;
            }
            Init_FormString();
            
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
            
            for (int i = 0; i < 128; i++)
            {
                if (f.select_address[i] != 0)
                {
                    //comboBox1.Text = f.select_address[i].ToString();
                    comboBox1.Items.Add(f.select_address[i]);
                }
            }
            comboBox1.SelectedIndex = 0;
            if(!flag)
            {
                label26.Visible = false;
                comboBox1.Visible = false;
            }
            Init_Data1();
            //Thread.Sleep(100);
            //Init_Data2();
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Color_59));
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
            foreach (Control ctl in groupBox2.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox3.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
             */
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        /// <summary>
        /// 语言设置加载
        /// </summary>
        private void Init_FormString()
        {
            this.Text = languageFile.ReadString("COLORFORM", "TITLE", "屏幕参数调整");
            this.groupBox1.Text = languageFile.ReadString("COLORFORM", "PARAMRTER", "用户色彩(1~100)");
            this.label3.Text = languageFile.ReadString("COLORFORM", "SAT", "亮度:");
            this.label1.Text = languageFile.ReadString("COLORFORM", "BRI", "对比度:");
            this.label2.Text = languageFile.ReadString("COLORFORM", "CON", "饱和度:");
            this.label21.Text = languageFile.ReadString("COLORFORM", "BAC", "背光:");
            this.label22.Text = languageFile.ReadString("COLORFORM", "CLA", "清晰度:");

            this.groupBox1.Text = languageFile.ReadString("COLORFORM", "WHITE", "白平衡");
            this.label6.Text = languageFile.ReadString("COLORFORM", "R", "红：");
            this.label7.Text = languageFile.ReadString("COLORFORM", "G", "绿：");
            this.label8.Text = languageFile.ReadString("COLORFORM", "B", "蓝：");

            this.groupBox1.Text = languageFile.ReadString("COLORFORM", "DARK", "暗平衡");
            this.label9.Text = languageFile.ReadString("COLORFORM", "B", "蓝：");
            this.label10.Text = languageFile.ReadString("COLORFORM", "G", "绿：");
            this.label11.Text = languageFile.ReadString("COLORFORM", "R", "红：");

            this.label26.Text = languageFile.ReadString("COLORFORM", "UINT", "屏幕单元");
            this.radioButton1.Text = languageFile.ReadString("COLORFORM", "SINGLE", "单个单元");
            this.radioButton2.Text = languageFile.ReadString("COLORFORM", "SPLICE", "拼接单元");
            this.button1.Text = languageFile.ReadString("COLORFORM", "CLOSE", "关闭");
            this.button2.Text = languageFile.ReadString("COLORFORM", "REST", "色彩复位");

        }


        private void InitDataThread()
        {
            Init_Data1();
        }

        public void ColorBelance_get1()
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = Byte.Parse(comboBox1.Text);
            array[2] = 0x20;
            array[3] = 0x88;
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            try
            {
                //serialPort1.Write(array, 0, 5);
                SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 2);
                f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                f.richTextBox2.ScrollToCaret();
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }
        public void ColorBelance_get2()
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = Byte.Parse(comboBox1.Text);
            array[2] = 0x20;
            array[3] = 0x86;
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            try
            {
                //serialPort1.Write(array, 0, 5);
                SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 2);
                f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                f.richTextBox2.ScrollToCaret();
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }
        public void ColorBelance_get3()
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = Byte.Parse(comboBox1.Text);
            array[2] = 0x20;
            array[3] = 0x09;
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            try
            {
                //serialPort1.Write(array, 0, 5);
                SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 2);
                f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                f.richTextBox2.ScrollToCaret();
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }
        private void Init_Data1()
        {
            receive_flag = true;
            Str = "";
            ColorBelance_get1();
            Thread.Sleep(600);
            if (Str != "")
            {
                try
                {
                    if (Str.IndexOf("COLOR_BALANCE:_", 0) >= 0 && (Str.IndexOf("_\r\n", 0) > 0 || Str.IndexOf("_\r\r\n", 0) > 0))
                    {
                        receive_flag = false;
                        //MessageBox.Show("1receive_flag = false");
                        int num2 = Str.IndexOf("COLOR_BALANCE:_", 0) + 14;
                        int num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        B_R = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        B_G = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        B_B = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        O_R = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        O_G = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        O_B = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        numericUpDown6.Value = B_R;
                        numericUpDown7.Value = B_G;
                        numericUpDown8.Value = B_B;
                        numericUpDown9.Value = O_R;
                        numericUpDown10.Value = O_G;
                        numericUpDown11.Value = O_B;
                        //Console.WriteLine("OOOOO_B=="+ O_B);
                    }
                }
                catch
                {
                    receive_flag = true;
                    MessageBox.Show("Display positioning unit screen parameter error! Please test", "Tips");
                    return;
                }
            }
            else
            {
                if (!button2.Enabled)
                    button2.Enabled = true;
                return;
            }
            Init_Data2();
        }

        private void Init_Data2()
        {
            receive_flag = true;
            Str = "";
            ColorBelance_get2();
            Thread.Sleep(600);
            string str = Str;
            //Console.WriteLine("sssss=== "+str);
            if (str != "")
            {
                try
                {
                    if (str.IndexOf("COLOR_DATA:_", 0) >= 0 && (str.IndexOf("_\r\n", 0) > 0 || str.IndexOf("_\r\r\n", 0) > 0))
                    {
                        receive_flag = false;
                        int num2 = str.IndexOf("COLOR_DATA:_", 0) + 11;
                        int num3 = str.IndexOf("_", num2 + 1);
                        str.Substring(num2 + 1, num3 - num2);
                        Lum = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = str.IndexOf("_", num2 + 1);
                        str.Substring(num2 + 1, num3 - num2);
                        Conrast = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = str.IndexOf("_", num2 + 1);
                        str.Substring(num2 + 1, num3 - num2);
                        BlackL = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = str.IndexOf("_", num2 + 1);
                        str.Substring(num2 + 1, num3 - num2);
                        Clarity = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = str.IndexOf("_", num2 + 1);
                        str.Substring(num2 + 1, num3 - num2);
                        Saturation = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        numericUpDown1.Value = Lum;
                        numericUpDown2.Value = Conrast;
                        numericUpDown3.Value = Saturation;
                        numericUpDown4.Value = BlackL;
                        numericUpDown5.Value = Clarity;
                    }
                }
                catch
                {
                    receive_flag = true;
                    if (f.Chinese_English == 1)
                    return;
                }
                receive_flag = true;
            }
            else
            {
                if (!button2.Enabled)
                    button2.Enabled = true;
                return;
            }
            if(!button2.Enabled)
                button2.Enabled = true;
            //Init_Data3();
        }

        private void Init_Data3()
        {
            int num = 0;
            receive_flag = true;
            do
            {
                Str = "";
                num++;
                ColorBelance_get3();
                
                string str = Str;
                if (str != "")
                {
                    try
                    {
                        if (str.IndexOf("CAPTURE DATA:_", 0) >= 0 && (str.IndexOf("_\r\n", 0) > 0 || str.IndexOf("_\r\r\n", 0) > 0))
                        {
                            receive_flag = false;
                            int num2 = str.IndexOf("_", 0);
                            int num3 = str.IndexOf("_", num2 + 1);
                            str.Substring(num2 + 1, num3 - num2);
                            //Hori_left = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                            num2 = num3;
                            num3 = str.IndexOf("_", num2 + 1);
                            str.Substring(num2 + 1, num3 - num2);
                            //Vert_up = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                            num2 = num3;
                            num3 = str.IndexOf("_", num2 + 1);
                            str.Substring(num2 + 1, num3 - num2);
                            //Hori = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                            num2 = num3;
                            num3 = str.IndexOf("_", num2 + 1);
                            str.Substring(num2 + 1, num3 - num2);
                            //Vert = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                            //numericUpDown1.Value = Hori;
                            //numericUpDown2.Value = Vert;

                        }
                    }
                    catch
                    {
                        receive_flag = true;
                    }
                }
            } while (num < 3 && receive_flag);
            if (num > 2 && receive_flag)
            {
                MessageBox.Show("显示定位3出错！请检测");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            //for (int i = 0; i <= count; i++)
            {
                byte[] array = new byte[6];
                array[0] = 0xE5;
                if (flag_ch)
                {
                    array[1] = Byte.Parse(comboBox1.Text);
                    //Console.WriteLine(array[1]);
                }
                else
                {
                    array[1] = 0xFD;
                }
                array[2] = 0x20;
                array[3] = 0x81;
                array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));

                try
                {
                    if (port_flag)
                    {
                        //serialPort1.Write(array, 0, 5);
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 1);
                        //f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                        //Thread.Sleep(100);
                        LogHelper.WriteLog("======操作屏幕的色彩复位设置======");
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (port_flag)
                {
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                        f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                        f.richTextBox2.ScrollToCaret();
                    }));
                    Init_Data1();

                    string ts = languageFile.ReadString("MESSAGEBOX", "C4", "色彩参数调整复位成功！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
        }

        private void Adjust(byte A_0)
        {
            //timer1.Interval = 100;
            //for (int i = 0; i <= count; i++)
            {
                byte[] array = new byte[8];
                array[0] = 0xE8;
                if (flag_ch)
                {
                    array[1] = Byte.Parse(comboBox1.Text);
                    //Console.WriteLine(array[1]);
                }
                else
                {
                    array[1] = 0xFD;
                }
                array[2] = 32;
                array[3] = A_0;
                array[4] = 0x0;
                if (A_0 == 96)
                {
                    array[5] = (byte)Lum;
                    array[6] = 0x0;
                }
                else if (A_0 == 97)
                {
                    array[5] = (byte)Conrast;
                    array[6] = 0x0;
                }
                else if (A_0 == 98)
                {
                    array[5] = (byte)BlackL;
                    array[6] = 0x0;
                }
                else if (A_0 == 99)
                {
                    array[5] = (byte)Saturation;
                    array[6] = 0x0;
                }
                else if (A_0 == 106)
                {
                    array[5] = (byte)Clarity;
                    array[6] = 0x0;
                }
                else if (A_0 == 100)
                {
                    array[5] = (byte)B_R;
                    array[6] = 0x0;
                }
                else if (A_0 == 101)
                {
                    array[5] = (byte)B_G;
                    array[6] = 0x0;
                }
                else if (A_0 == 102)
                {
                    array[5] = (byte)B_B;
                    array[6] = 0x0;
                }
                else if (A_0 == 103)
                {
                    array[6] = (byte)(O_R % 256);
                    array[5] = (byte)(O_R / 256);
                }
                else if (A_0 == 104)
                {
                    array[6] = (byte)(O_G % 256);
                    array[5] = (byte)(O_G / 256);
                }
                else if (A_0 == 105)
                {
                    array[6] = (byte)(O_B % 256);
                    array[5] = (byte)(O_B / 256);
                }
                else
                {
                    array[5] = (byte)Lum;
                    array[6] = 0x0;
                }
                //Console.WriteLine(array[5] +","+ array[6]);
                array[7] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4] + array[5] + array[6]));

                try
                {
                    if (port_flag)
                    {
                        //serialPort1.Write(array, 0, 6);
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 8, 100, 1);
                        //f.richTextBox2.AppendText(MainForm.ToHexString(array, 6));
                        //Thread.Sleep(500);
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
                this.Invoke(new MethodInvoker(delegate()
                {
                    f.richTextBox2.AppendText(MainForm.ToHexString(array, 8));
                    f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                    f.richTextBox2.ScrollToCaret();
                }));
                //if(count > 0)
                //comboBox1.SelectedIndex = i;
            } 
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value < 0)
                numericUpDown1.Value = 0;
            if (numericUpDown1.Value > 100)
                numericUpDown1.Value = 100;
            if (receive_flag)
            {
                Lum = (int)numericUpDown1.Value;
                Adjust(96);
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value < 0)
                numericUpDown2.Value = 0;
            if (numericUpDown2.Value > 100)
                numericUpDown2.Value = 100;
            if (receive_flag)
            {
                Conrast = (int)numericUpDown2.Value;
                Adjust(97);
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown3.Value < 0)
                numericUpDown3.Value = 0;
            if (numericUpDown3.Value > 100)
                numericUpDown3.Value = 100;
            if (receive_flag)
            {
                Saturation = (int)numericUpDown3.Value;
                Adjust(99);
            }
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown4.Value < 0)
                numericUpDown4.Value = 0;
            if (numericUpDown4.Value > 100)
                numericUpDown4.Value = 100;
            if (receive_flag)
            {
                BlackL = (int)numericUpDown4.Value;
                Adjust(98);
            }
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown5.Value < 0)
                numericUpDown5.Value = 0;
            if (numericUpDown5.Value > 100)
                numericUpDown5.Value = 100;
            if (receive_flag)
            {
                Clarity = (int)numericUpDown5.Value;
                Adjust(106);
            }
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown6.Value < 0)
                numericUpDown6.Value = 0;
            if (numericUpDown6.Value > 255)
                numericUpDown6.Value = 255;
            if (receive_flag)
            {
                B_R = (int)numericUpDown6.Value;
                Adjust(100);
            }
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown7.Value < 0)
                numericUpDown7.Value = 0;
            if (numericUpDown7.Value > 255)
                numericUpDown7.Value = 255;
            if (receive_flag)
            {
                B_G = (int)numericUpDown7.Value;
                Adjust(101);
            }
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown8.Value < 0)
                numericUpDown8.Value = 0;
            if (numericUpDown8.Value > 255)
                numericUpDown8.Value = 255;
            if (receive_flag)
            {
                B_B = (int)numericUpDown8.Value;
                Adjust(102);
            }
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown9.Value < 0)
                numericUpDown9.Value = 0;
            if (numericUpDown9.Value > 2047)
                numericUpDown9.Value = 2047;
            if (receive_flag)
            {
                O_R = (int)numericUpDown9.Value;
                Adjust(103);
            }
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown10.Value < 0)
                numericUpDown10.Value = 0;
            if (numericUpDown10.Value > 2047)
                numericUpDown10.Value = 2047;
            if (receive_flag)
            {
                O_G = (int)numericUpDown10.Value;
                Adjust(104);
            }
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown11.Value < 0)
                numericUpDown11.Value = 0;
            if (numericUpDown11.Value > 2047)
                numericUpDown11.Value = 2047;
            if (receive_flag)
            {
                O_B = (int)numericUpDown11.Value;
                Adjust(105);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                if (flag)
                {
                    //comboBox1.Visible = true;
                    //label26.Visible = true;
                }
                radioButton2.Checked = false;
                flag_ch = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                comboBox1.Visible = false;
                label26.Visible = false;
                radioButton1.Checked = false;
                flag_ch = false;
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(300);
            try
            {
                int byteNumber = serialPort1.BytesToRead;
                //Common.Delay(20);
                //延时等待数据接收完毕。
                while ((byteNumber < serialPort1.BytesToRead) && (serialPort1.BytesToRead < 4096))
                {
                    byteNumber = serialPort1.BytesToRead;
                    Thread.Sleep(50);
                }
                string str = serialPort1.ReadExisting();
                if (receive_flag)
                    Str += str;
                //Console.Write(str);
            }
            catch
            {

            }
        }

        private void Form_Color_59_FormClosing(object sender, FormClosingEventArgs e)
        {
            Str = "";
            try
            {
                if (port_flag)
                {
                    serialPort1.Close();
                    port_flag = false;
                }
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Init_Data1();
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }


    }
}
