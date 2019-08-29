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
    public partial class Form_UartIrCmd_3458 : Form
    {
        public MainForm f;
        private int count;
        private IniFiles languageFile;
        //private bool port_flag = false;
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public Form_UartIrCmd_3458(MainForm f,int count)
        {
            InitializeComponent();
            this.f = f;
            this.count = count; 
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            
            Init_FormString();
            //serialPort1.PortName = f.PortName;
            //serialPort1.BaudRate = f.BaudRate;
            /*
            try
            {
                serialPort1.Open();
                port_flag = true;
            }
            catch
            {
                serialPort1.Close();
                if (f.Chinese_English)
                    MessageBox.Show("Serial open error！", "Tips");
                else
                    MessageBox.Show("串口打开出错！", "提示");
                port_flag = false;
            }
             * */
            //button1.Enabled = false;
            /*
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
            */
            timer1.Interval = 100 * count + 300;
            timer1.Start();
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_UartIrCmd_3458));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        /// <summary>
        /// 语言设置加载
        /// </summary>
        private void Init_FormString()
        {
            this.Text = languageFile.ReadString("UARTRFORM", "TITLE", "遥控");
            this.label10.Text = languageFile.ReadString("UARTRFORM", "P", "电源");
            this.label3.Text = languageFile.ReadString("UARTRFORM", "M", "静音");
            this.label4.Text = languageFile.ReadString("UARTRFORM", "MENU", "菜单");
            this.label11.Text = languageFile.ReadString("UARTRFORM", "S", "信源");
            this.label14.Text = languageFile.ReadString("UARTRFORM", "ID", "ID 设定");
            this.label15.Text = languageFile.ReadString("UARTRFORM", "E", "退出");
            this.button22.Text = languageFile.ReadString("UARTRFORM", "OK", "OK");
        }

        private void Send_Control(byte A_0)
        {
            try
            {
                byte[] array = new byte[5];
                array[0] = 0xE5;
                array[2] = 0x20;
                array[3] = A_0;
                for (int i = 0; i < count; i++)
                {
                    if (f.select_address[i] != 0)
                    {
                        array[1] = f.select_address[i];
                        array[4] = (byte)(255 - (255 & array[0] + array[1] + array[2] + array[3]));
                        //Console.WriteLine("array[1] = " + array[1]);
                        if (f.Rs232Con)
                        {
                            //serialPort1.Write(array, 0, 5);

                            if (f.TCPCOM)
                            {
                                f.TcpSendMessage(array, 0, 5);
                            }
                            else
                                SerialPortUtil.serialPortSendData(f.serialPort1, array, 0, 5, 100, 1);
                            f.richTextBox2.AppendText(MainForm.ToHexString(array, 5));
                            f.richTextBox2.SelectionStart = f.richTextBox2.Text.Length;
                            f.richTextBox2.ScrollToCaret();
                            
                        }
                    }
                }
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }

        private void Set_Enable(Button b, bool f)
        {
            b.Enabled = f;
            //Cursor.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB7);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC9);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB9);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xBA);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xBB);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xBC);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xBD);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xBE);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xBF);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC0);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC1);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC7);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC8);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB2);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB4);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB5);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB3);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB1);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC4);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC2);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC5);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC6);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xC3);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB6);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB0);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xB8);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (Control b in this.Controls)
            {
                if (!b.Enabled)
                {
                    b.Enabled = true;
                    b.Focus();
                }
            }
            //Cursor.Show();
        }

        private void Form_UartIrCmd_MouseClick(object sender, MouseEventArgs e)
        {
            /*
            //Console.WriteLine("dddddd dddd");
            if (port_flag)
                return;
            else
            {
                try
                {
                    serialPort1.Open();
                    port_flag = true;
                }
                catch
                {
                    serialPort1.Close();
                    if (f.Chinese_English)
                        MessageBox.Show("Serial open error！", "Tips");
                    else
                        MessageBox.Show("串口打开出错！", "提示");
                    port_flag = false;
                }
            }
             * */
        }

        private void Form_UartIrCmd_Activated(object sender, EventArgs e)
        {
            //Console.WriteLine("aaaaaaaaaaa");
            /*
            if (port_flag)
                return;
            else
            {
                try
                {
                    f.serialPort1.Close();
                    serialPort1.Open();
                    port_flag = true;
                }
                catch
                {
                    if (f.Chinese_English)
                        MessageBox.Show("Serial open error！", "Tips");
                    else
                        MessageBox.Show("串口打开出错！", "提示");
                }
            }
             * */
        }

        private void Form_UartIrCmd_Deactivate(object sender, EventArgs e)
        {
            //Console.WriteLine("ddddddddddddd");
            /*
            for (int i = 0; i < 256; i++)
                f.select_address[i] = 0;
            if (port_flag)
            {
                try
                {
                    serialPort1.Close();
                    f.serialPort1.Open();
                    port_flag = false;
                }
                catch
                {
                    if (f.Chinese_English)
                        MessageBox.Show("Serial open error！", "Tips");
                    else
                        MessageBox.Show("串口打开出错！", "提示");
                }
            }
             * */
        }

        private void Form_UartIrCmd_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            //for (int i = 0; i < 128; i++)
                //f.select_address[i] = 0;
            /*
            if (port_flag)
            {
                try
                {
                    serialPort1.Close();
                    port_flag = false;
                }
                catch
                {
                    if (f.Chinese_English)
                        MessageBox.Show("Serial error！", "Tips");
                    else
                        MessageBox.Show("串口出错！", "提示");
                }
            }
            if (!f.serialPort1.IsOpen)
            {
                try
                {
                    f.serialPort1.Open();
                }
                catch
                {
                    if (f.Chinese_English)
                        MessageBox.Show("Serial error！", "Tips");
                    else
                        MessageBox.Show("Serial open error, reopen!", "提示");
                }
            }
            //f.Form_UartIrCmd_Opend = false;
             */ 
        }

        private void button28_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xDD);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xDC);
        }

        private void Form_UartIrCmd_Load(object sender, EventArgs e)
        {
            //asc.controllInitializeSize(this); 
        }

        private void Form_UartIrCmd_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xE0);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xDB);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            Set_Enable(sender as Button, false);
            Send_Control((byte)0xD9);
        }
    }
}
