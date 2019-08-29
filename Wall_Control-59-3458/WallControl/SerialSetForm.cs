using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace WallControl
{
    public partial class SerialSetForm : Form
    {
        public MainForm f;
        private IniFiles languageFile;
        //public static bool do_Attime = false;
        //public static bool do_Athex = true;
        //private bool connecte = false;
        //public static byte Leng = 0;
        //public static string str = "";
        //private byte[] Buf = new byte[128];
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public SerialSetForm(MainForm f)
        {
            this.f = f;
            InitializeComponent();

            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);

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
            initSerialPort();
            if (f.Motherboard_flag == 1)
            {
                checkBox1.Checked = true;
                groupBox2.Enabled = false;
                checkBox2.Enabled = false;
            }
            else
            {
                groupBox2.Enabled = true;
                checkBox2.Enabled = true;
            }
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialSetForm));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
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

        private void bt_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string port1;
        private string com1;
        private string port2;
        private string com2;
         /// <summary>
        /// 语言设置加载
        /// </summary>
        private void Init_FormString()
        {
            this.Text = languageFile.ReadString("SERIALSETFORM", "TITLE", "串口设置");
            //this.label1.Text = languageFile.ReadString("[SERIALSETFORM]", "TITLE", "串口设置");
            this.groupBox1.Text = languageFile.ReadString("SERIALSETFORM", "SET", "设定");
            this.label20.Text = languageFile.ReadString("SERIALSETFORM", "MULTICOM", "多串口");
            this.label2.Text = languageFile.ReadString("SERIALSETFORM", "COMSELCT", "串口选择");
            this.lb_port.Text = languageFile.ReadString("SERIALSETFORM", "COM1", "串口号1");
            this.lb_baudRate.Text = languageFile.ReadString("SERIALSETFORM", "BPS1 ", "波特率1");
            this.bt_confirm.Text = languageFile.ReadString("SERIALSETFORM", "OK", "确认");
            this.bt_cancel.Text = languageFile.ReadString("SERIALSETFORM", "CANCEL ", "取消");
            this.bt_confirm.TextAlign = ContentAlignment.MiddleCenter;
            this.bt_cancel.TextAlign = ContentAlignment.MiddleCenter;
            port1 = languageFile.ReadString("SERIALSETFORM", "SELECT1", "串口1");
            port2 = languageFile.ReadString("SERIALSETFORM", "SELECT2", "串口2(矩阵)");
            com1 = languageFile.ReadString("SERIALSETFORM", "MULTI1", "单串口");
            com2 = languageFile.ReadString("SERIALSETFORM", "MULTI2", "多串口");
            cb_multiCom.Items.Clear();
            cb_multiCom.Items.Add(com1);
            cb_multiCom.Items.Add(com2);
        }

        /// <summary>
        /// 初始化串口
        /// </summary>
        public void initSerialPort()
        {
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            String readPort1 = settingFile.ReadString("Com Set", "port1", "COM1");//串口1
            String readPort2 = settingFile.ReadString("Com Set", "port2", "COM1");//串口2
            String[] serialPorts = System.IO.Ports.SerialPort.GetPortNames();
            cb_port1.Items.Clear();
            cb_port2.Items.Clear();
            for (int i = 0; i < serialPorts.Length; i++)//找出所有串口，并选择文件中的
            {
                cb_port1.Items.Add(serialPorts[i]);//初始化串口1
                if (readPort1.Equals(serialPorts[i]))
                {
                    cb_port1.SelectedIndex = i;
                }
                cb_port2.Items.Add(serialPorts[i]);//初始化串口2
                if (readPort2.Equals(serialPorts[i]))
                {
                    cb_port2.SelectedIndex = i;
                }

                //Console.WriteLine(serialPorts[i]);
            }
            if (cb_port1.Items.Count > 0 && cb_port1.SelectedIndex < 0)//如果文件读出来的串口名没有，则默认第一个。
            {
                cb_port1.SelectedIndex = 0;
            }
            if (cb_port2.Items.Count > 0 && cb_port2.SelectedIndex < 0)//如果文件读出来的串口名没有，则默认第一个。串口2
            {
                cb_port2.SelectedIndex = 0;
            }

            cb_baudRate1.Text = settingFile.ReadString("Com Set", "baudrate1", "9600");//串口1
            cb_baudRate2.Text = settingFile.ReadString("Com Set", "baudrate2", "9600");//串口1
            cb_timeout.Text = settingFile.ReadString("Com Set", "timeout", "10");//延时

            cb_multiCom.SelectedIndex = settingFile.ReadInteger("Com Set", "MultiCom", 1) - 1;//多串口选择
            if (cb_serialSelect.Items.Count > 0)
                cb_serialSelect.SelectedIndex = 0;//串口选择，默认为串口1

            combo_netpro.SelectedIndex = settingFile.ReadInteger("Com Set", "TCPP", 0);
            textBox_IP.Text = settingFile.ReadString("Com Set", "IP", "127.0.01");
            textBox_Port.Text = settingFile.ReadString("Com Set", "Port", "8234");
            numericUpDown1.Value = (decimal)settingFile.ReadInteger("Com Set", "Con", 16);
            if (f.TCPCOM)
            {
                checkBox2.Checked = true;
                groupBox1.Enabled = false;
                groupBox2.Enabled = true;
            }
            else
            {
                checkBox1.Checked = true;
                groupBox1.Enabled = true;
                groupBox2.Enabled = false;
            }
            if (f.PJLink_Pro)
            {
                checkBox3.Checked = true;
            }
            else
            {
                checkBox3.Checked = false;
            }

        }
        private IniFiles settingFile;//配置文件


        /// <summary>
        /// 多串口下拉发生了改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_multiCom_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cb_multiCom.SelectedIndex == 0)//0  单串口
            {
                showPort1();
                //让串口选择中串口2不可选
                cb_serialSelect.Items.Clear();
                /*
                if (f.Chinese_English)
                    cb_serialSelect.Items.Add("Port1");
                else
                    cb_serialSelect.Items.Add("串口1");//加串口1
                 */
                cb_serialSelect.Items.Add(port1);
                cb_serialSelect.SelectedIndex = 0;

            }
            else if (cb_multiCom.SelectedIndex == 1)//1  双串口
            {
                //让串口选择中串口2可选
                cb_serialSelect.Items.Clear();
                /*
                if (f.Chinese_English)
                {
                    cb_serialSelect.Items.Add("Port1");
                    cb_serialSelect.Items.Add("Port2(Matrix)");//加串口2
                }
                else
                {
                    cb_serialSelect.Items.Add("串口1");//加串口1
                    cb_serialSelect.Items.Add("串口2(矩阵)");//加串口2
                }
                 */
                cb_serialSelect.Items.Add(port1);
                cb_serialSelect.Items.Add(port2);
                cb_serialSelect.SelectedIndex = 0;
            }

        }

        /// <summary>
        /// 串口选择下拉发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_serialMulSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_serialSelect.SelectedIndex == 0)//0  串口1
            {
                showPort1();

            }
            else if (cb_multiCom.SelectedIndex == 1)//1  串口2
            {
                showPort2();
            }
        }

        /// <summary>
        /// 让串口1信息可视，串口2信息不可视
        /// </summary>
        private void showPort1()
        {
            /*
            if (f.Chinese_English)
            {
                lb_port.Text = "Serial 1";
                lb_baudRate.Text = "Baud rate 1";
            }
            else
            {
                lb_port.Text = "串口号1";
                lb_baudRate.Text = "波特率1";
            }
             */
            this.lb_port.Text = languageFile.ReadString("SERIALSETFORM", "COM1", "串口号1");
            this.lb_baudRate.Text = languageFile.ReadString("SERIALSETFORM", "BPS1 ", "波特率1");
            cb_port2.Visible = false;
            cb_baudRate2.Visible = false;
            cb_port1.Visible = true;
            cb_baudRate1.Visible = true;

        }

        /// <summary>
        /// 让串口2信息可视，串口1信息不可视
        /// </summary>
        private void showPort2()
        {
            /*
            if (f.Chinese_English)
            {
                lb_port.Text = "Serial 2";
                lb_baudRate.Text = "Baud rate 2";
            }
            else
            {
                lb_port.Text = "串口号2";
                lb_baudRate.Text = "波特率2";
            }
             */
            this.lb_port.Text = languageFile.ReadString("SERIALSETFORM", "COM2", "串口号1");
            this.lb_baudRate.Text = languageFile.ReadString("SERIALSETFORM", "BPS2 ", "波特率1");
            cb_port2.Visible = true;
            cb_baudRate2.Visible = true;
            cb_port1.Visible = false;
            cb_baudRate1.Visible = false;
        }

        /// <summary>
        /// 确认串口设置信息，并保存至文件中。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_confirm_Click(object sender, EventArgs e)
        {
            if (f.TCPCOM)
            {
                if (textBox_IP.Text == "" || textBox_Port.Text == "")
                {
                    string ts = languageFile.ReadString("ONOFFFORM", "T4", "设置的IP地址和端口不能为空!");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (checkBox3.Checked)
                    f.PJLink_Pro = true;
                else
                    f.PJLink_Pro = false;
                if (IsIP(textBox_IP.Text))
                {
                    if (combo_netpro.SelectedIndex == 0)
                    {
                        f.TCPServer = new IOCPServer(IPAddress.Parse(textBox_IP.Text), int.Parse(textBox_Port.Text), (int)numericUpDown1.Value);
                    }
                    else if (combo_netpro.SelectedIndex == 1)
                    {
                        f.TCPClient = new client(IPAddress.Parse(textBox_IP.Text), int.Parse(textBox_Port.Text));
                    }
                    else
                    {
                        f.UDPClient = new System.Net.Sockets.UdpClient();
                    }
                    f.IP = IPAddress.Parse(textBox_IP.Text);
                    f.PORT = int.Parse(textBox_Port.Text);
                }
                else
                {
                    string ts = languageFile.ReadString("ONOFFFORM", "T5", "IP 地址信息不正确！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else
            {
                if (cb_port1.Text.Equals("") || cb_baudRate1.Text.Equals(""))
                {
                    string ts = languageFile.ReadString("ONOFFFORM", "T1", "串口1设置不能为空！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                //串口设置和打开
                String portName;
                int baudrate;
                if (cb_multiCom.SelectedIndex == 0)//选的dan串口
                {
                    portName = cb_port1.Text;
                    baudrate = int.Parse(cb_baudRate1.Text);
                    f.PortName = portName;
                    f.BaudRate = baudrate;
                    f.uMultiComPort = 1;
                }
                else
                {//选的串口2
                    if (cb_port2.Text.Equals("") || cb_baudRate2.Text.Equals(""))
                    {
                        string ts = languageFile.ReadString("ONOFFFORM", "T2", "串口2设置不能为空！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                        return;
                    }
                    portName = cb_port1.Text;
                    baudrate = int.Parse(cb_baudRate1.Text);
                    if (cb_port1.Text == cb_port2.Text)
                    {
                        string ts = languageFile.ReadString("ONOFFFORM", "T3", "两个串口设置不能为同一个串口号！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                        return;
                    }
                    f.PortName = portName;
                    f.BaudRate = baudrate;
                    portName = cb_port2.Text;
                    baudrate = int.Parse(cb_baudRate2.Text);
                    f.PortName2 = portName;
                    f.BaudRate2 = baudrate;
                    f.uMultiComPort = 2;
                }
                f.Init_port();
            }
            //SerialPortUtil.setSerialPort(portName, baudrate);
            //SerialPortUtil.openSerialPort();

            //保存配置至ini文件
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            settingFile.WriteString("Com Set", "port1", cb_port1.Text);
            //Console.WriteLine(cb_port1.Text);
            settingFile.WriteString("Com Set", "baudrate1", cb_baudRate1.Text);
            settingFile.WriteString("Com Set", "port2", cb_port2.Text);
            settingFile.WriteString("Com Set", "baudrate2", cb_baudRate2.Text);
            settingFile.WriteString("Com Set", "timeout", cb_timeout.Text);
            settingFile.WriteInteger("Com Set", "MultiCom", cb_multiCom.SelectedIndex + 1);

            settingFile.WriteString("Com Set", "IP", textBox_IP.Text);
            settingFile.WriteString("Com Set", "Port", textBox_Port.Text);
            settingFile.WriteBool("Com Set", "TCPCOM", f.TCPCOM);
            settingFile.WriteBool("Com Set", "PJLink", checkBox3.Checked);
            settingFile.WriteInteger("Com Set", "TCPP", combo_netpro.SelectedIndex);
            settingFile.WriteInteger("Com Set", "Con", (int)numericUpDown1.Value);
            this.Close();
        }
        /*
        private void button2_Click(object sender, EventArgs e)
        {
            if (f.serialPort1.IsOpen)
            {
                return;
            }
            f.serialPort1.PortName = cb_port1.Text;
            f.serialPort1.BaudRate = int.Parse(cb_baudRate1.Text);
            f.serialPort1.ReadTimeout = int.Parse(cb_timeout.Text);
            //Console.WriteLine(serialPort1.PortName + "P" + serialPort1.BaudRate + "B" + serialPort1.ReadTimeout);
            try
            {
                f.serialPort1.Open();
                if (f.uMultiComPort == 2)
                    f.serialPort2.Open();
                f.Rs232Con = true;
                f.toolStripStatusLabel3.ForeColor = Color.Green;
                f.button7.ForeColor = Color.Green;
                f.button6.ForeColor = Color.Black;
                if (f.Chinese_English)
                    f.toolStripStatusLabel3.Text = "Port：" + cb_port1.Text + " Open " + int.Parse(cb_baudRate1.Text) + " Bps";
                else
                    f.toolStripStatusLabel3.Text = "端口：" + cb_port1.Text + " 已打开 " + int.Parse(cb_baudRate1.Text) + " Bps";
                connecte = true;
            }
            catch
            {
                f.serialPort1.Close();
                if (f.uMultiComPort == 2)
                    f.serialPort2.Close();
                f.Rs232Con = false;
                f.toolStripStatusLabel3.ForeColor = Color.Red;
                if (f.Chinese_English)
                    f.toolStripStatusLabel3.Text = "Port：" + cb_port1.Text + " Close " + int.Parse(cb_baudRate1.Text) + " Bps";
                else
                    f.toolStripStatusLabel3.Text = "端口：" + cb_port1.Text + " 已关闭 " + int.Parse(cb_baudRate1.Text) + " Bps";
                connecte = false;
                if (f.Chinese_English)
                    MessageBox.Show("Serial open error！", "Tips");
                else
                    MessageBox.Show("串口打开出错！");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!f.serialPort1.IsOpen)
            {
                return;
            }
            try
            {
                f.serialPort1.Close();
                if (f.uMultiComPort == 2)
                    f.serialPort2.Close();
                f.Rs232Con = false;
                f.toolStripStatusLabel3.ForeColor = Color.Red;
                f.button6.ForeColor = Color.Green;
                f.button7.ForeColor = Color.Black;
                if (f.Chinese_English)
                    f.toolStripStatusLabel3.Text = "Port：" + cb_port1.Text + " Close " + int.Parse(cb_baudRate1.Text) + " Bps";
                else
                    f.toolStripStatusLabel3.Text = "端口：" + cb_port1.Text + " 已关闭 " + int.Parse(cb_baudRate1.Text) + " Bps";
                connecte = false;
            }
            catch
            {
                if (f.Chinese_English)
                    MessageBox.Show("Serial shutdown error！", "Tips");
                else
                    MessageBox.Show("串口关闭出错！", "提示");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (connecte)
                {
                    if (do_Athex)
                    {
                        //Console.WriteLine(str);
                        f.serialPort1.Write(Buf, 0, Leng);
                    }
                    else
                        f.serialPort1.Write(str);
                }
            }
            catch
            {
                if (f.Chinese_English)
                    MessageBox.Show("Serial error！", "Tips");
                else
                    MessageBox.Show("串口出错！", "提示");
            }
        }

         * */

        private void SerialSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (f.serialPort1.IsOpen)
            {
                try
                {
                    f.serialPort1.Close();
                    f.label4.ForeColor = Color.Red;
                    f.label4.Text = "端口：" + cb_port1.Text + " 已关闭 " + int.Parse(cb_baudRate1.Text) + " Bps";
                    connecte = false;
                }
                catch
                {
                    MessageBox.Show("串口出错！");
                }
            }*/
        }

        private void SerialSetForm_Load(object sender, EventArgs e)
        {
            //asc.controllInitializeSize(this);  
        }

        private void SerialSetForm_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = false;
                checkBox2.Checked = false;
                f.TCPCOM = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = true;
                checkBox1.Checked = false;
                f.TCPCOM = true;
            }
        }

        private void combo_netpro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (f.TCPCOM)
            {
                if (combo_netpro.SelectedIndex == 0)
                {
                    label7.Visible = true;
                    numericUpDown1.Visible = true;
                }
                else
                {
                    label7.Visible = false;
                    numericUpDown1.Visible = false;
                }
            }
        }

        private bool IsIP(string ip)
        {
            //判断是否为IP
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }
}