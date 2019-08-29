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
    public partial class Form_Color : Form
    {
        public MainForm f;
        public int B_R = 128;
        public int B_G = 128;
        public int B_B = 128;
        public int O_R = 128;
        public int O_G = 128;
        public int O_B = 128;
        public int Lum = 50;
        public int Conrast = 50;
        public int Saturation = 50;
        public int BlackL = 100;
        public int Clarity = 50;
        public int Hori;
        public int Vert;
        public int Hori_left;
        public int Vert_up;
        private string Str = "";
        private int count = 0;
        private bool flag_ch = true;
        private bool port_flag = false;
        private bool receive_flag = true;
        private bool flag = false;
        private IniFiles languageFile;
        //private Thread myThread = null;
        //private bool VGA_flag = false;
        private int receiveFlag = -1;//参数接受的标志,0表示完成，1表示接受COLOR_BALANCE:部分，2表示COLOR_DATA:，3表示CAPTURE DATA:
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public Form_Color(MainForm f,bool flag)
        {
            InitializeComponent();
            this.f = f;
            this.flag = flag;
            
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
                //serialPort1.Close();
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
            /*
            myThread = new Thread(new ThreadStart(delegate()
            {
                Init_Thread();
            })); //开线程         
            myThread.Start(); //启动线程 
            */
            for (int i = 0; i < 128; i++)
            {
                if (f.select_address[i] != 0)
                {
                    //comboBox1.Text = f.select_address[i].ToString();
                    comboBox1.Items.Add(f.select_address[i]);
                }
            }
            comboBox1.SelectedIndex = 0;
            //Selecte_VGA();
            count = comboBox1.Items.Count - 1;
            if (comboBox1.Items.Count == 1)
            {
                if (f.Chinese_English == 1)
                    radioButton1.Text = "This unit";
                else
                    radioButton1.Text = "本单元";
                radioButton2.Enabled = false;
            }
            else
            {
                if (f.Chinese_English == 1)
                    radioButton1.Text = "Single unit";
                else
                    radioButton1.Text = "单个单元";
                radioButton2.Enabled = true;
            }
            //Init_Data1();
            //Thread.Sleep(100);
            //Init_Data2();
            //Thread.Sleep(100);
            //Init_Data3();
           
            if (flag)
            {
                radioButton2.Checked = true;
            }
            else
            {
                radioButton1.Checked = true;
            }

            if (comboBox1.Items.Count == 1)
            {
                radioButton1.Checked = true;
            }
            /*
            if (radioButton1.Checked)
            {
                if (comboBox1.Items.Count > 1)
                {
                    if (flag)
                        groupBox4.Enabled = true;
                    else
                        groupBox4.Enabled = false;
                }
                else
                    groupBox4.Enabled = false;
            }
            else if (radioButton2.Checked)
            {
                groupBox4.Enabled = false;   
            }
            */
            textBox1.Text = f.screen_H[int.Parse(comboBox1.Text) - 1].ToString();
            textBox2.Text = f.screen_V[int.Parse(comboBox1.Text) - 1].ToString();

            timer1.Interval = 100 * comboBox1.Items.Count +400;
            timer1.Start();

        }

        private void Init_Thread()
        {
            Init_Data1();
            Thread.Sleep(200);
            Init_Data2();
            Thread.Sleep(200);
            Init_Data3();
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Color));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            resources.ApplyResources(button2, button2.Name);
            resources.ApplyResources(button29, button29.Name);
            resources.ApplyResources(button30, button30.Name);
            resources.ApplyResources(button1, button1.Name);
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
            foreach (Control ctl in groupBox4.Controls)
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

            this.groupBox2.Text = languageFile.ReadString("COLORFORM", "WHITE", "白平衡");
            this.label6.Text = languageFile.ReadString("COLORFORM", "R", "红：");
            this.label7.Text = languageFile.ReadString("COLORFORM", "G", "绿：");
            this.label8.Text = languageFile.ReadString("COLORFORM", "B", "蓝：");

            this.groupBox3.Text = languageFile.ReadString("COLORFORM", "DARK", "暗平衡");
            this.label9.Text = languageFile.ReadString("COLORFORM", "B", "蓝：");
            this.label10.Text = languageFile.ReadString("COLORFORM", "G", "绿：");
            this.label11.Text = languageFile.ReadString("COLORFORM", "R", "红：");

            this.label26.Text = languageFile.ReadString("COLORFORM", "UINT", "屏幕单元");
            this.radioButton1.Text = languageFile.ReadString("COLORFORM", "SINGLE", "单个单元");
            this.radioButton2.Text = languageFile.ReadString("COLORFORM", "SPLICE", "拼接单元");
            this.button1.Text = languageFile.ReadString("COLORFORM", "CLOSE", "关闭");
            this.button2.Text = languageFile.ReadString("COLORFORM", "REST", "色彩复位");

            this.groupBox4.Text = languageFile.ReadString("COLORFORM", "PACH", "拼缝调整");
            this.label23.Text = languageFile.ReadString("COLORFORM", "H", "水平方向：");
            this.label24.Text = languageFile.ReadString("COLORFORM", "V", "垂直方向：");
            this.button30.Text = languageFile.ReadString("COLORFORM", "EMPOLY", "应用");
            this.button29.Text = languageFile.ReadString("COLORFORM", "RSFRESH", "刷新");

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

                if (f.TCPCOM)
                {
                    f.TcpSendMessage(array, 0, 5);
                }
                else
                    SerialPortUtil.serialPortSendData(f.serialPort1, array, 0, 5, 100, 2);
                f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
            }
            catch
            {
                MessageBox.Show("显示参数1串口通讯出错！请检测");
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
                if (f.TCPCOM)
                {
                    f.TcpSendMessage(array, 0, 5);
                }
                else
                    SerialPortUtil.serialPortSendData(f.serialPort1, array, 0, 5, 100, 2);
                f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
            }
            catch
            {
                MessageBox.Show("显示参数2串口通讯出错！请检测");
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
                if (f.TCPCOM)
                {
                    f.TcpSendMessage(array, 0, 5);
                }
                else
                    SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 2);
                f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                f.richTextBox2.ScrollToCaret();
            }
            catch
            {
                MessageBox.Show("显示参数3串口通讯出错！请检测");
            }
        }
        private void Init_Data1()
        {
            int num = 0;
            receive_flag = true;
           // receiveFlag = 1;
            //Str = "";
            
            do
            {
                Str = "";
                receiveFlag = 1;
                num++;
                ColorBelance_get1();
                double startData = (DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds;//获取开始的毫秒数
                while (receiveFlag != 0)
                {
                    if ((DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds - startData >= 1000 && Str.Equals(""))
                    {//一秒还没值，重发
                        //Console.WriteLine("1一秒无值");
                        break;
                    }
                    if ((DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds - startData >= 3000)
                    {
                        MessageBox.Show("获取参数失败！请重试");
                        return;
                    }
                    Thread.Sleep(50);
                }
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
                            //label17.Text = B_R.ToString();
                            //label16.Text = B_G.ToString();
                            //label15.Text = B_B.ToString();
                            //label20.Text = O_R.ToString();
                            //label19.Text = O_G.ToString();
                            //label18.Text = O_B.ToString();
                        }
                    }
                    catch
                    {
                        receive_flag = true;
                    }
                }
            } while (num < 4 && receive_flag);
            if (num > 3 && receive_flag)
            {
                MessageBox.Show("显示定位1出错！请检测");
                return;
            }
            Init_Data2();
        }

        private void Init_Data2()
        {
            int num = 0;
            receive_flag = true;
            
            //receiveFlag = 2;
            //Str = "";
            do
            {
                Str = "";
                receiveFlag = 2;
                num++;
                ColorBelance_get2();    
                double startData = (DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds;//获取开始的毫秒数
                while (receiveFlag != 0)
                {
                    if ((DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds - startData >= 1000 && Str.Equals(""))
                    {//一秒还没值，重发
                        break;
                    }
                    if ((DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds - startData >= 3000)
                    {
                        MessageBox.Show("获取参数失败！请重试");
                        return;
                    }
                    Thread.Sleep(50);
                }
                string str = Str;
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
                            //label12.Text = Lum.ToString();
                            //label13.Text = Conrast.ToString();
                            //label14.Text = Saturation.ToString();
                            //label5.Text = BlackL.ToString();
                            //label4.Text = Clarity.ToString();
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
                MessageBox.Show("显示定位2出错！请检测");
                return;
            }
            //Init_Data3();
        }

        private void Init_Data3()
        {
            receive_flag = true;
            Str = "";
            ColorBelance_get3();
            Thread.Sleep(500);
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
                        Hori_left = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = str.IndexOf("_", num2 + 1);
                        str.Substring(num2 + 1, num3 - num2);
                        Vert_up = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = str.IndexOf("_", num2 + 1);
                        str.Substring(num2 + 1, num3 - num2);
                        Hori = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = str.IndexOf("_", num2 + 1);
                        str.Substring(num2 + 1, num3 - num2);
                        Vert = int.Parse(str.Substring(num2 + 1, num3 - num2 - 1));
                        //numericUpDown1.Value = Hori;
                        //numericUpDown2.Value = Vert;
                        textBox1.Text = Hori.ToString();
                        textBox2.Text = Vert.ToString();
                    }
                }
                catch
                {
                    receive_flag = true;
                    if (f.Chinese_English == 1)
                        MessageBox.Show("Display positioning unit screen parameter error! Please test", "Tips");
                    else
                        MessageBox.Show("显示定位单元画面参数出错！请检测3", "提示");
                    return;
                }
            }
            else
            {
                if (f.Chinese_English == 1)
                    MessageBox.Show("Obtain positioning unit screen parameter error! Please test", "Tips");
                else
                    MessageBox.Show("获取定位单元画面参数失败！请重试3", "提示");
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            f.screen_H[int.Parse(comboBox1.Text) - 1] = int.Parse(textBox1.Text);
            f.screen_V[int.Parse(comboBox1.Text) - 1] = int.Parse(textBox2.Text);     
            
            timer1.Dispose();
            this.Close();
        }

        private void Adjust(byte A_0, byte A_1)
        {
            //timer1.Start();
            string str = comboBox1.Text;
            byte[] array = new byte[6];
            array[0] = 0xE6;
            array[2] = 32;
            array[3] = A_0;
            array[4] = A_1;
            for (int i = 0; i <= count; i++)
            {
                if (count > 0)
                    str = comboBox1.Items[i].ToString();//comboBox1.SelectedIndex = i;
                else
                    str = comboBox1.Text;
                if (flag_ch)
                {
                    array[1] = Byte.Parse(str);
                    //Console.WriteLine(array[1]);
                }
                else
                {
                    array[1] = 0xFD;
                } 
                /*
                if (A_0 == 96)
                {
                    array[5] = (byte)Lum;
                }
                else if (A_0 == 97)
                {
                    array[5] = (byte)Conrast;
                }
                else if (A_0 == 98)
                {
                    array[5] = (byte)BlackL;
                }
                else if (A_0 == 99)
                {
                    array[5] = (byte)Saturation;
                }
                else if (A_0 == 106)
                {
                    array[5] = (byte)Clarity;
                }
                else if (A_0 == 100)
                {
                    array[5] = (byte)B_R;
                }
                else if (A_0 == 101)
                {
                    array[5] = (byte)B_G;
                }
                else if (A_0 == 102)
                {
                    array[5] = (byte)B_B;
                }
                else if (A_0 == 103)
                {
                    array[5] = (byte)O_R;
                    //array[6] = (byte)(O_R & 0xFF);
                }
                else if (A_0 == 104)
                {
                    array[5] = (byte)O_G;
                    //array[6] = (byte)(O_G & 0xFF);
                }
                else if (A_0 == 105)
                {
                    array[5] = (byte)O_B;
                    //array[6] = (byte)(O_B & 0xFF);
                }
                else
                {
                    array[5] = (byte)Lum;
                }*/
                array[5] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4]));
                
                try
                {
                    if (f.Rs232Con)
                    {
                        //serialPort1.Write(array, 0, 6);
                        if (f.TCPCOM)
                        {
                            f.TcpSendMessage(array, 0, 5);
                        }
                        else
                            SerialPortUtil.serialPortSendData(serialPort1, array, 0, 6, 200, 1);
                        f.richTextBox2.AppendText(MainForm.ToHexString(array, 6));
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
                //this.BeginInvoke(new MethodInvoker(delegate()
                //{
                    //f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                //}));
                //if(count > 0)
                    //comboBox1.SelectedIndex = i;
            }
            
        }

        private void Set_Enable(Button b, bool f)
        {
            b.Enabled = f;
            //Cursor.Hide();
            if(f)
                b.Focus();
        }
        
        private void button16_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            /*
            Set_Enable((Button)sender, false);
            Lum--;
            if (Lum < 0)
                Lum = 0;
            label12.Text = Lum.ToString();
             * */
            Adjust(96, 0);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            /*
            Set_Enable((Button)sender, false);
            Lum++;
            if (Lum > 100)
                Lum = 100;
            //label12.Text = Lum.ToString();
          */
            Adjust(96, 1);
        }

        private void button18_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            Conrast--;
            if (Conrast < 0)
                Conrast = 0;
            //label13.Text = Conrast.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(97, 0);
        }

        private void button17_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            Conrast++;
            if (Conrast > 100)
                Conrast = 100;
            //label13.Text = Conrast.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(97, 1);
        }

        private void button20_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            Saturation--;
            if (Saturation < 0)
                Saturation = 0;
            //label14.Text = Saturation.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(99, 0);
        }

        private void button19_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            Saturation++;
            if (Saturation > 100)
                Saturation = 100;
            //label14.Text = Saturation.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(99, 1);
        }

        private void button3_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            B_R--;
            if (B_R < 0)
                B_R = 0;
            //label17.Text = B_R.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(100, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            B_R++;
            if (B_R > 255)
                B_R = 255;
            //label17.Text = B_R.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(100, 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            B_G--;
            if (B_G < 0)
                B_G = 0;
            //label16.Text = B_G.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(101, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            B_G++;
            if (B_G > 255)
                B_G = 255;
            //label16.Text = B_G.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(101, 1);
        }

        private void button8_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            B_B--;
            if (B_B < 0)
                B_B = 0;
            //label15.Text = B_B.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(102, 0);
        }

        private void button7_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            B_B++;
            if (B_B > 255)
                B_B = 255;
            //label15.Text = B_B.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(102, 1);
        }

        private void button14_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            O_R--;
            if (O_R < 0)
                O_R = 0;
            //label20.Text = O_R.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(103, 0);
        }

        private void button13_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            O_R++;
            if (O_R > 255)
                O_R = 255;
            //label20.Text = O_R.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(103, 1);
        }

        private void button12_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            O_G--;
            if (O_G < 0)
                O_G = 0;
            //label19.Text = O_G.ToString();
          * */
            Set_Enable(sender as Button, false);
            Adjust(104, 0);
        }

        private void button11_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            O_G++;
            if (O_G > 255)
                O_G = 255;
            //label19.Text = O_G.ToString();
          * */
            Set_Enable(sender as Button, false);
            Adjust(104, 1);
        }

        private void button10_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            O_B--;
            if (O_B < 0)
                O_B = 0;
            //label18.Text = O_B.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(105, 0);
        }

        private void button9_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            O_B++;
            if (O_B > 255)
                O_B = 255;
            //label18.Text = O_B.ToString();
          * */
            Set_Enable(sender as Button, false);
            Adjust(105, 1);
        }
        //复位调整
        private void button2_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            button2.ForeColor = Color.Green;
            byte[] array = new byte[6];
            array[0] = 0xE5;
            array[2] = 0x20;
            array[3] = 0x81;
            for (int i = 0; i <= count; i++)
            {
                if (count > 0)
                    comboBox1.SelectedIndex = i;
                if (flag_ch)
                {
                    array[1] = Byte.Parse(comboBox1.Text);      
                    //Console.WriteLine(array[1]);
                }
                else
                {
                    array[1] = 0xFD;  
                }
                
                array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
                
                try
                {
                    if (f.Rs232Con)
                    {
                        //serialPort1.Write(array, 0, 5);
                        if (f.TCPCOM)
                        {
                            f.TcpSendMessage(array, 0, 5);
                        }
                        else
                            SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 3);
                        f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                        f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                        f.richTextBox2.ScrollToCaret();
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
            }
            string tt = languageFile.ReadString("MESSAGEBOX", "C4", "色彩参数调整复位成功！");
            string th = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
            MessageBox.Show(tt, th);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                comboBox1.Visible = true;
                label26.Visible = true;
                radioButton2.Checked = false;
                flag_ch = true;
                count = 0;
                if (flag)
                {
                    groupBox4.Enabled = true;
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                comboBox1.Visible = false;
                label26.Visible = false;
                radioButton1.Checked = false;
                groupBox4.Enabled = false;
                /*
                if (comboBox1.Items.Count == MainForm.rowsCount * MainForm.colsCount)
                {
                    //Console.WriteLine(comboBox1.Items.Count + "," + MainForm.rowsCount * MainForm.colsCount);
                    count = 0;
                    flag_ch = false;
                }
                else 
                 */ 
                {
                    count = comboBox1.Items.Count - 1;
                    flag_ch = true;
                }
            }
        }

        private void Form_Color_FormClosing(object sender, FormClosingEventArgs e)
        {
            f.screen_H[int.Parse(comboBox1.Text) - 1] = int.Parse(textBox1.Text);
            f.screen_V[int.Parse(comboBox1.Text) - 1] = int.Parse(textBox2.Text);     
            timer1.Stop();
            
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

        private void button24_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            BlackL--;
            if (BlackL < 0)
                BlackL = 0;
            //label5.Text = BlackL.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(106, 0);
        }

        private void button23_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            BlackL++;
            if (BlackL > 100)
                BlackL = 100;
            //label5.Text = BlackL.ToString();
          * */
            Set_Enable(sender as Button, false);
            Adjust(106, 1);
        }

        private void button22_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            Clarity--;
            if (Clarity < 0)
                Clarity = 0;
            //label4.Text = Clarity.ToString();
          * */
            Set_Enable(sender as Button, false);
            Adjust(98, 0);
        }

        private void button21_Click(object sender, EventArgs e)
        {/*
            Set_Enable((Button)sender, false);
            Clarity++;
            if (Clarity > 100)
                Clarity = 100;
            //label4.Text = Clarity.ToString();
          */
            Set_Enable(sender as Button, false);
            Adjust(98, 1);
        }

        private void Adjust_HV(byte A1,byte A2,byte A3,byte A4)
        {
            byte[] array = new byte[9];
            array[0] = 0xE9;
            array[1] = Byte.Parse(comboBox1.Text);
            array[2] = 0x20;
            array[3] = 0x08;
            array[4] = A1;
            array[5] = A2;
            array[6] = A3;
            array[7] = A4;
            array[8] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4] + array[5] + array[6] + array[7]));
            try
            {
                if (f.Rs232Con)
                {
                    //serialPort1.Write(array, 0, 9);
                    if (f.TCPCOM)
                    {
                        f.TcpSendMessage(array, 0, 5);
                    }
                    else
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 9, 100, 3);
                    f.richTextBox2.AppendText(MainForm.ToHexString(array, 9));
                    f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                    f.richTextBox2.ScrollToCaret();
                    //Console.WriteLine("cccc="+array[6] + array[7]);
                }
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }
        private bool flag_HV = true;
        /*
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("cccccccc");
            if (numericUpDown1.Value > 10)
            {
                if (f.Chinese_English)
                    MessageBox.Show("Please enter the correct value range (0 ~ 10)！", "Tips");
                else
                    MessageBox.Show("请输入正确的设值范围（0~10）！", "提示");
                numericUpDown1.Value = 10;
            }
            if (numericUpDown1.Value < 0)
            {
                if (f.Chinese_English)
                    MessageBox.Show("Please enter the correct value range (0 ~ 10)！", "Tips");
                else
                    MessageBox.Show("请输入正确的设值范围（0~10）！", "提示");
                numericUpDown1.Value = 0;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value > 10)
            {
                if (f.Chinese_English)
                    MessageBox.Show("Please enter the correct value range (0 ~ 10)！", "Tips");
                else
                    MessageBox.Show("请输入正确的设值范围（0~10）！", "提示");
                numericUpDown2.Value = 10;
            }
            if (numericUpDown2.Value < 0)
            {
                if (f.Chinese_English)
                    MessageBox.Show("Please enter the correct value range (0 ~ 10)！", "Tips");
                else
                    MessageBox.Show("请输入正确的设值范围（0~10）！", "提示");
                numericUpDown2.Value = 0;
            }
        }
        */
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!button2.Enabled)
            {
                button2.Enabled = true;
                button2.ForeColor = Color.Black;
                button2.Focus();
            }
            foreach (Control b in groupBox1.Controls)
            {
                if (!b.Enabled)
                {
                    /*
                    if (VGA_flag)
                        if (b == button20 || b == button21 || b == button22 || b == button19) ;
                        else
                        {
                            b.Enabled = true;
                            b.Focus();
                        }
                    else
                     */ 
                    {
                        b.Enabled = true;
                        b.Focus();
                    }
                }
            }
            foreach (Control b in groupBox2.Controls)
            {
                if (!b.Enabled)
                {
                    b.Enabled = true;
                    b.Focus();
                }
            }
            foreach (Control b in groupBox3.Controls)
            {
                if (!b.Enabled)
                {
                    b.Enabled = true;
                    b.Focus();
                }
            }
            foreach (Control b in groupBox4.Controls)
            {
                if (!b.Enabled)
                {
                    b.Enabled = true;
                    b.Focus();
                }
            }
            /*
            label12.Text = Lum.ToString();
            label13.Text = Conrast.ToString();
            label14.Text = Saturation.ToString();
            label5.Text = BlackL.ToString();
            label4.Text = Clarity.ToString();
            label17.Text = B_R.ToString();
            label16.Text = B_G.ToString();
            label15.Text = B_B.ToString();
            label20.Text = O_R.ToString();
            label19.Text = O_G.ToString();
            label18.Text = O_B.ToString();
             */ 
            //timer1.Stop();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            int t  = int.Parse(textBox1.Text);
            //Console.WriteLine("tttt===" + t);
            if (t > 0)
            {
                t = t - 1;
                Set_Enable(sender as Button, false);
                textBox1.Text = t.ToString();
                Hori = t;
                if (flag_HV)
                    Adjust_HV((byte)0, (byte)0, (byte)Hori, (byte)Vert);
            }
            else
            {
                textBox1.Text = "0";
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            int t = int.Parse(textBox1.Text);
            if (t < 10)
            {
                t = t + 1;
                Set_Enable(sender as Button, false);
                textBox1.Text = t.ToString();
                Hori = t;
                if (flag_HV)
                    Adjust_HV((byte)0, (byte)0, (byte)Hori, (byte)Vert);
            }
            else
            {
                textBox1.Text = "10";
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            int t = int.Parse(textBox2.Text);
            if (t > 0)
            {
                t = t - 1;
                Set_Enable(sender as Button, false);
                textBox2.Text = t.ToString();
                Vert = t;
                if (flag_HV)
                    Adjust_HV((byte)0, (byte)0, (byte)Hori, (byte)Vert);
            }
            else
            {
                textBox2.Text = "0";
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            int t = int.Parse(textBox2.Text);
            if (t < 10)
            {
                t = t + 1;
                textBox2.Text = t.ToString();
                Set_Enable(sender as Button, false);
                Vert = t;
                if (flag_HV)
                    Adjust_HV((byte)0, (byte)0, (byte)Hori, (byte)Vert);
            }
            else
            {
                textBox2.Text = "10";
            }
        }

        private void Selecte_VGA()
        {
            /*
            if (f.screens[comboBox1.SelectedIndex].IntputType.Contains("VGA"))
            {
                label3.Enabled = false;
                button19.Enabled = false;
                button20.Enabled = false;
                label21.Enabled = false;
                button22.Enabled = false;
                button21.Enabled = false;
                VGA_flag = true;
            }
            else
            {
                label3.Enabled = true;
                button19.Enabled = true;
                button20.Enabled = true;
                label21.Enabled = true;
                button22.Enabled = true;
                button21.Enabled = true;
                VGA_flag = false;
            }
            //Console.Write(VGA_flag);
             */ 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("comboBox1=cc" + comboBox1.SelectedIndex);
            textBox1.Text = f.screen_H[int.Parse(comboBox1.Text) - 1].ToString();
            textBox2.Text = f.screen_V[int.Parse(comboBox1.Text) - 1].ToString();
            flag_HV = true;
            //Selecte_VGA();
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            flag_HV = false;
            //Console.WriteLine("comboBox1=" + comboBox1.SelectedIndex);
            textBox1.Text = f.screen_H[int.Parse(comboBox1.Text) - 1].ToString();
            textBox2.Text = f.screen_V[int.Parse(comboBox1.Text) - 1].ToString();
        }

        private void button29_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "C1", "请输入有效数字，不能为空！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                /*
                if (f.Chinese_English)
                    MessageBox.Show("Please enter a valid number, not empty！", "Tips");
                else
                    MessageBox.Show("请输入有效数字，不能为空！", "提示");
                 */ 
                return;
            }
            f.screen_H[int.Parse(comboBox1.Text) - 1] = int.Parse(textBox1.Text);
            f.screen_V[int.Parse(comboBox1.Text) - 1] = int.Parse(textBox2.Text);   
            Hori = int.Parse(textBox1.Text);
            Vert = int.Parse(textBox2.Text);
            if (flag_HV)
                Adjust_HV((byte)0, (byte)0, (byte)Hori, (byte)Vert);
            LogHelper.WriteLog("======调整屏幕拼缝设置======");  
        }

        private bool IsInts(string str)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            return rex.IsMatch(str);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
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
                if (int.Parse(textBox1.Text) > 10)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "C3", "请输入正确的设值范围（0~10）！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    textBox1.Text = "10";
                    return;
                }
                if (int.Parse(textBox1.Text) < 0)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "C3", "请输入正确的设值范围（0~10）！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    textBox1.Text = "0";
                    return;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                if (textBox2.Text.Length > 0)
                {
                    if (!IsInts(textBox2.Text))
                    {
                        string ts = languageFile.ReadString("MESSAGEBOX", "C2", "请输入有效数字字符组合(\"0~9\")");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                        textBox2.Text = "0";
                        return;
                    }
                }
                if (int.Parse(textBox2.Text) > 10)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "C3", "请输入正确的设值范围（0~10）！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    textBox2.Text = "10";
                    return;
                }
                if (int.Parse(textBox2.Text) < 0)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "C3", "请输入正确的设值范围（0~10）！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    textBox2.Text = "0";
                    return;
                }
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            Init_Data3();
            f.screen_H[int.Parse(comboBox1.Text) - 1] = int.Parse(textBox1.Text);
            f.screen_V[int.Parse(comboBox1.Text) - 1] = int.Parse(textBox2.Text);
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
                {
                    if (str.Contains("CAPTURE DATA"))
                    {
                        Str += str;
                        //Console.Write(str);
                    }
                }
                //Console.Write(str);
            }
            catch
            {

            }
        }


    }
}
