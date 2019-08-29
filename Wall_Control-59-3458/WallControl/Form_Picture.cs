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
    public partial class Form_Picture : Form
    {
        public MainForm f;
        private bool port_flag;
        private string Str;
        private bool receive_flag;
        public static int h;
        public static int v;
        public static int o;
        public Form_Picture(MainForm f)
        {
            InitializeComponent();
            this.f = f;
            serialPort1.PortName = f.PortName;
            serialPort1.BaudRate = f.BaudRate;

            try
            {
                serialPort1.Open();
                port_flag = true;
            }
            catch
            {
                serialPort1.Close();
                MessageBox.Show("串口打开出错。");
                port_flag = false;
            }
            Init_data();
            numericUpDown1.Value = (decimal)h;
            numericUpDown2.Value = (decimal)v;
            numericUpDown3.Value = (decimal)o;
        }
        private void Init_data()
        {
            byte[] array = new byte[5];
            receive_flag = true; 
            int num = 0;
            do
            {
                array[0] = 0xE5;
                array[1] = f.check_address[f.address_backup];
                array[2] = 0x20;
                array[3] = 0x8A;
                array[4] = (byte)(0xFF- (0xFF & array[0] + array[1] + array[2] + array[3]));
                serialPort1.Write(array, 0, 5);
                Thread.Sleep(300);
                num++;
                try
                {
                    if (Str.IndexOf("<CAP_POS:_", 0) >= 0 && (Str.IndexOf(">\r\n", 0) > 0 || Str.IndexOf(">\r\r\n", 0) > 0))
                    {
                        receive_flag = false;
                        int num2 = Str.IndexOf("<CAP_POS:_", 0) + 10;
                        int num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        num2 = num3;
                        num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        h = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = Str.IndexOf("_", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        v = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        num2 = num3;
                        num3 = Str.IndexOf(">", num2 + 1);
                        Str.Substring(num2 + 1, num3 - num2);
                        o = int.Parse(Str.Substring(num2 + 1, num3 - num2 - 1));
                        
                    }
                }
                catch
                {
                    receive_flag = true;
                    f.Delay(1000);
                }
            } while (receive_flag && num < 6);
            if (num > 5 && receive_flag)
            {
                MessageBox.Show("显示定位单元串口通讯出错！请检测");
            }
            //f.richTextBox1.Text = Str;
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(300);
            string str = serialPort1.ReadExisting();
            if (receive_flag)
            {
                Str += str;
                /*
                this.BeginInvoke(new MethodInvoker(delegate()
                {
                    f.richTextBox1.Text = Str;
                }));*/
                //f.richTextBox1.Text = Str;
                //Console.Write(Str);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num = 0;
            do
            {
                if (port_flag)
                {
                    try
                    {
                        byte[] array = new byte[11];
                        array[0] = 0xEB;
                        array[1] = f.check_address[f.address_backup];
                        array[2] = 0x20;
                        array[3] = 0x89;
                        array[4] = 0x00;
                        array[5] = (byte)h;
                        array[6] = 0x00;
                        array[7] = (byte)v;
                        array[8] = 0x00;
                        array[9] = (byte)o;
                        array[10] = (byte)(255 - (255 & array[0] + array[1] + array[2] + array[3] + array[4] + array[5] + array[6] + array[7] + array[8] + array[9]));

                        serialPort1.Write(array, 0, 11);
                        f.Delay(200);
                    }
                    catch
                    {
                        MessageBox.Show("串口通讯出错！");
                        //port_flag = false;
                    }
                }
                num++;
            } while (num < 15);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (port_flag)
            {
                serialPort1.Close();
            }
            this.Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            h = (int)numericUpDown1.Value; 
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            v = (int)numericUpDown2.Value; 
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            o = (int)numericUpDown3.Value; 
        }

        private void Form_Picture_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                serialPort1.Close();
            }
            catch
            {
                MessageBox.Show("串口出错！");
            }
        }
    }
}
