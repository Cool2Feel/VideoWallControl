#define to_3458

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using unvell.ReoGrid;
using unvell.ReoGrid.Events;
using System.IO;
using System.IO.Ports;
using System.Threading;
using Microsoft.Win32;
using System.Reflection;
using System.Globalization;
using DevComponents.DotNetBar;
using System.Net;
using System.Net.Sockets;

namespace WallControl
{
    public partial class MainForm : Form
    {
        #region 变量
        private int oldLeftTopCol = 0;//鼠标选择的老的左上角的列
        private int oldLeftTopRow = 0;//鼠标选择的老的左上角的行
        private int oldRightBottomCol = 0;//鼠标选择的老的右下角的列
        private int oldRightBottomRow = 0;//鼠标选择的老的右下上的行
        //RangePosition oldRangePosition;
        //bool selecting = false;//是否是新选的范围，FALSE为新选
        public static String currentSceneName = "scene.rgf";//当前场景名
        public static String currentConnectionName = "myWall";//当前连接名
        public static String currentSceneButtonText = "场景一";//当前场景名按钮的值
        public static int rowsCount = 2;//行
        public static int colsCount = 2;//列
        private static int rowStar = 0;
        private static int rowEnd = 0;
        private static int colStar = 0;
        private static int colEnd = 0;
        //public static byte group = 0;//组号
        public bool changed = false;//是否对场景进行了改变
        public int Motherboard_type = 0;//方案主板的类型控制0,1,2,3
        public int Motherboard_flag = 0;//3458方案、59方案
        private IniFiles settingFile;//配置文件
        private IniFiles languageFile;//配置文件
        private string LanguagePack = Application.StartupPath + "\\LanguagePack";
        public string package;
        private unvell.ReoGrid.Worksheet sheet;
        private unvell.ReoGrid.Worksheet sheet_back;
        private Screen[] screens;
        public int[] address = new int[256];//所有屏对应的单元地址序号
        private List<bool> comboxEditList;//标志哪些下拉可以修改
        private int curEditIndex = 0;//信号编辑里面的combox的坐标
        private String[] allSceneName = new String[256];//所有的场景名
        private List<MergeGroup> mergeGroups;//该场景的所有合并的组
        private List<Screen> unMergeScreenList;//切换场景后所有的非合并的屏
        public bool Rs232Con = false;
        //private bool Rs232Con_true = false;
        //public bool Show_flag = false;
        public string PortName = "COM1";
        public int BaudRate = 9600;
        public string PortName2 = "COM1";
        public int BaudRate2 = 9600;
        public int uMultiComPort = 1;   //端口数
        public int Address = 0;// 更新时屏的序号
        public byte[] check_address = new byte[256];//对应地址
        public byte[] select_address = new byte[256];//对应地址
        //public byte[] select_usb = new byte[128];
        public byte address_backup;//选中屏地址
        public int Matrix_time = 200;
        public int Matrix_flag = 0;
        private bool Matrix_check_flag = false;
        public int[] screen_H = new int[4];
        public int[] screen_V = new int[4]; 
        private bool progressToRight = true;//进度条方向是否想右
        protected BackgroundWorker worker = null;//后台运行
        private bool systemRunning = false;//运行状态
        public string Edit_str = "";
        private static String defaultSignalName = "HDMI1";//默认的信源
        public byte[,] PanelCount = new byte[4,256];   //channel * 2 + 5 ( channel max 64 )
        public Thread myThread = null;
        private Button prepareCreateBt = null;//预建立的场景还不存在的button
        public int Chinese_English = 0;
        //public string language_string;
        private int Delay_time = 0;
        private bool PN = false;
        public bool f_m = false;
        public int Delay_time1
        {
            get { return Delay_time; }
            set { Delay_time = value; }
        }
        public bool Delay_on = false;
        public bool Delay_off = false;
        public bool Timing_on = false;
        public bool Timing_off = false;
        public DateTime on = DateTime.Now;
        public DateTime off = DateTime.Now;
        public byte uVga = 4;                       //VGA矩阵端口数
        public int uVideo = 4;                     //VIDEO矩阵端口数
        public byte uDvi = 0;                       //DVI矩阵端口数
        public byte uHdmi = 0;                      //HDMI矩阵端口数
        public byte uYPbPr = 0;                     //YPbPr矩阵端口数

        public static int dviAddress = 1;//DVI地址
        public static int videoAddress = 2;
        public static int vgaAddress = 3;
        public static int hdmiAddress = 4;
        public static int ypbprAddress = 5;

        public static int dviMatrixSelect = 0;//DVI矩阵选用
        public static int videoMatrixSelect = 0;
        public static int vgaMatrixSelect = 0;
        public static int hdmiMatrixSelect = 0;
        public static int ypbprMatrixSelect = 0;

        public string dviMatrix = "";//DVI矩阵选用
        public string videoMatrix = "";
        public string vgaMatrix = "";
        public string hdmiMatrix = "";
        public string ypbprMatrix = "";

        // 系统消息常量
        public const int WM_DEVICE_CHANGE = 0x219;             //设备改变           
        public const int DBT_DEVICEARRIVAL = 0x8000;          //设备插入
        public const int DBT_DEVICE_REMOVE_COMPLETE = 0x8004; //设备移除

        public bool PJLink_Pro = false;
        public bool TCPCOM = false;
        public IOCPServer TCPServer;
        public client TCPClient;
        public UdpClient UDPClient;

        public IPAddress IP;
        public int PORT;
        public string TCPReceiveData;

        #endregion
        //public bool reg_flag = false;//注册标志
        //1.声明自适应类实例  
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public MainForm()
        {
            //this.EnableGlass = false;
            InitializeComponent();
            AccessFunction.AccessOpen();
            //initRoGridControl(2, 2);
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            initRoGridSet();
            initRoGridFromFile("");
            initMatrixFromFile();
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < rowsCount * colsCount; i++)
                {
                    PanelCount[j,i] = (byte)(i + 1);
                }
            }
            screen_H = new int[rowsCount * colsCount];//拼缝调整记录
            screen_V = new int[rowsCount * colsCount];
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(s_DataReceived);

            serialPort1.ReceivedBytesThreshold = 1;
            //groupBox1.Enabled = false;
            //groupBox4.Enabled = false;
            
            //rightMouseContextMenuStrip.Enabled = false;
            //默认ToolStripMenuItem.CheckState = CheckState.Checked;
            button27.Enabled = false;
            button28.Enabled = false;
            button30.Enabled = false;
            button6.Enabled = false;
            button5.Enabled = false;
            button8.Enabled = false;
            //button10.Visible = false;
            //button13.Visible = false;
            //button32.Visible = false;
            //richTextBox2.Visible = false;
            //button18.Enabled = false;
            //button20.Enabled = false;
            //button21.Enabled = false;
            //button10.Visible = false;
            package = settingFile.ReadString("SETTING", "PACKAGE", "\\EN_package.ini");//"\\EN_package.ini";

            timer1.Interval = 1000;
            timer1.Start();
            //if()
            //pictureBox1.Visible = true;
            //SetBtnStyle(button1);
            //SetBtnStyle(button3);
            //SetBtnStyle(button5);
            SetBtnStyle(button6);
            SetBtnStyle(button7);
            //SetBtnStyle(button8);
            SetBtnStyle(button15);
            SetBtnStyle(button14);
            SetBtnStyle(button16);
            SetBtnStyle(button17);
            SetBtnStyle(button18);
            SetBtnStyle(button19);
            SetBtnStyle(button20);
            SetBtnStyle(button21);
            SetBtnStyle(button22);
            SetBtnStyle(button23);
            SetBtnStyle(button24);
            SetBtnStyle(button26);

            int s_M = settingFile.ReadInteger("SETTING", "Motherboard", 0);
            if (s_M == 0)
            {
                Motherboard_type = 0;
                Motherboard_flag = 4;
            }
            else if (s_M == 1)
            {
                Motherboard_type = 1;
                Motherboard_flag = 4;
            }
            else if (s_M == 2)
            {
                Motherboard_type = 2;
                Motherboard_flag = 2;
            }
            else if (s_M == 3)
            {
                Motherboard_type = 3;
                Motherboard_flag = 2;
            }
            else if (s_M == 4)
            {
                Motherboard_type = 4;
                Motherboard_flag = 4;
            }
            else
            {
                Motherboard_type = 0;
                Motherboard_flag = 4;
            }
#if  to_3458
            Init_Channel();
            Init_Scene();
            comboBox1.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
#else
            vIDEO1ToolStripMenuItem.Text = "VIDEO1";
            vIDEO2ToolStripMenuItem.Text = "VIDEO1";
            vIDEO3ToolStripMenuItem.Text = "VIDEO1";
            vIDEO4ToolStripMenuItem.Text = "VIDEO1";
            sVIDEOToolStripMenuItem.Text = "S-VIDEO";
            yPbPrToolStripMenuItem.Text = "YPbpR";
            vGAToolStripMenuItem.Text = "VGA";
            hDMIToolStripMenuItem.Text = "HDMI";
            dVIToolStripMenuItem.Text = "DVI";
#endif
            string s1 = settingFile.ReadString("SETTING", "Pwd", "0");
            //Thread.Sleep(50);
            string s2 = settingFile.ReadString("SETTING", "CH-US", "");
            //Thread.Sleep(50);
            //Console.WriteLine(s2);
            if (s2.Equals("1"))
            {
                Chinese_English = 1;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
                package = "\\EN_package.ini";
                languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                toolStripMenuItem4.CheckState = CheckState.Checked;
                //Init_FormString();
            }
            else if (s2.Equals("0"))
            {
                Chinese_English = 0;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
                package = "\\CH_package.ini";
                languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                toolStripMenuItem3.CheckState = CheckState.Checked;
                //Init_FormString();
                //comboBox1.Items.Add("矩阵HDMI");
                //comboBox1.Items.Add("矩阵DVI");
            }
            else if (s2.Equals("2"))
            {
                Chinese_English = 2;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
                ApplyResource();
                package = "\\TH_package.ini";
                languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                toolStripMenuItem5.CheckState = CheckState.Checked;
                button23.Visible = false;
                //Init_FormString();
                //comboBox1.Items.Add("矩阵HDMI");
                //comboBox1.Items.Add("矩阵DVI");
            }
            else//根据系统的语言进行显示
            {
                string language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                //Console.WriteLine(language);
                switch(language)
                {
                    case "zh-CN":
                        Chinese_English = 0;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                        ApplyResource();
                        package = "\\CH_package.ini";
                        languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                        toolStripMenuItem3.CheckState = CheckState.Checked;
                        break;
                    case "Zh-TW":
                        Chinese_English = 2;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
                        ApplyResource();
                        package = "\\TH_package.ini";
                        languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                        toolStripMenuItem5.CheckState = CheckState.Checked;
                        button23.Visible = false;
                        break;
                    case "en-US":
                        Chinese_English = 1;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        ApplyResource();
                        package = "\\EN_package.ini";
                        languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                        toolStripMenuItem4.CheckState = CheckState.Checked;
                        break;
                    default:
                        Chinese_English = 1;
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        ApplyResource();
                        package = "\\EN_package.ini";
                        languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                        toolStripMenuItem4.CheckState = CheckState.Checked;
                        break;
                }
            }

            toolStripStatusLabel4.Text = languageFile.ReadString("MAINFORM", "SYS_STATUS", "系统状态：") + languageFile.ReadString("MAINFORM", "STATUS1", "空闲");
            if (s1 == "1")
            {
                toolStripStatusLabel2.Text = languageFile.ReadString("MAINFORM", "USER_ADMIN", "用户权限：  管理员用户！"); //"User rights: Administrator user！";
            }
            else
                toolStripStatusLabel2.Text = languageFile.ReadString("MAINFORM", "USER_GENERAL", "用户权限：  普通用户！"); //"User rights: General user！";
            {
                Init_comboBoxE();
            }

            string sl = languageFile.ReadString("MAINFORM", "SYS_TIME ", "系统当前时间：");
            toolStripStatusLabel1.Text = sl + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string sp = languageFile.ReadString("MAINFORM", "STATUS_OFF", "关闭");
            toolStripStatusLabel3.Text = PortName + "  " + sp + " " + BaudRate + " Bps";
            toolStripStatusLabel4.Text = languageFile.ReadString("MAINFORM", "SYS_STATUS", "系统状态：") + languageFile.ReadString("MAINFORM", "STATUS1", "空闲");

            Delay_time1 = settingFile.ReadInteger("SETTING", "DELAY", 0);
            PJLink_Pro = settingFile.ReadBool("Com Set", "PJLink", false);
            TCPCOM = settingFile.ReadBool("Com Set", "TCPCOM", false);
            IP = IPAddress.Parse(settingFile.ReadString("Com Set", "IP", "127.0.0.1"));
            PORT = int.Parse(settingFile.ReadString("Com Set", "Port", "8234"));
            if (TCPCOM)
            {
                if (settingFile.ReadInteger("Com Set", "TCPP", 0) == 0)
                {
                    com_List.Visible = true;
                    //com_List.SelectedIndex = 0;
                    //button33.Text = "开始监听";
                    //button34.Text = "关闭监听";
                    TCPClient = new client(IP, PORT);
                    TCPServer = new IOCPServer(IP, PORT, settingFile.ReadInteger("Com Set", "Con", 16));
                }
                else
                {
                    com_List.Visible = false;
                    //button33.Text = "打开连接";
                    //button34.Text = "关闭连接";
                }
            }
            else
            {
                com_List.Visible = false;
                this.button7.Text = languageFile.ReadString("MAINFORM", "OPEN", "开连接");
                this.button6.Text = languageFile.ReadString("MAINFORM", "COLSE", "关连接");
            }

            initSignalEdit();
            string s = settingFile.ReadString("SETTING", "PicturePath", "");
            //Console.WriteLine("sss=====" + s);
            if (!s.Equals(""))
            {
                s = s.Substring(s.Length - 4, 4);
                s = Application.StartupPath + @"\pic\logo" + s;
                pictureBox1.Image = Image.FromFile(@s);
            }
            PN = settingFile.ReadBool("SETTING", "NameFlag", false);
            if (PN)
                label18.Text = settingFile.ReadString("SETTING", "NamePath", "液晶拼接控制系统");
            tabPage12.Parent = null;
            tabPage12.Hide();
            if (s1 == "1")
            {
                button32.Visible = true;
            }
            else
            {
                button13.Visible = false;
                richTextBox2.Visible = false;
                button10.Visible = false;
                button32.Visible = false;
            }
            //tabPage10.Parent = null;
            //tabPage10.Hide();
            this.Refresh();
        }
        /// <summary>
        /// 初始化主板的通道信息
        /// </summary>
        private void Init_Channel()
        {
            string s1 = settingFile.ReadString("SETTING", "Pwd", "0");
            if (Motherboard_type == 0)
            {
                yPbPrToolStripMenuItem.Visible = true;
                vIDEO2ToolStripMenuItem.Visible = false;
                vIDEO1ToolStripMenuItem.Text = "HDMI1";
                vIDEO2ToolStripMenuItem.Text = "HDMI2";
                vIDEO3ToolStripMenuItem.Text = "OPS";
                vIDEO4ToolStripMenuItem.Visible = false;
                sVIDEOToolStripMenuItem.Visible = false;
                yPbPrToolStripMenuItem.Text = "VGA";
                vIDEO3ToolStripMenuItem.Visible = true;
                vGAToolStripMenuItem.Visible = false;
                hDMIToolStripMenuItem.Visible = false;
                dVIToolStripMenuItem.Visible = false;
                comboBox1.Visible = false;
                comboBox3.Visible = true;
                comboBox4.Visible = false;
                comboBox5.Visible = false;
                comboBox6.Visible = false;
                button15.Show();
                if (s1 == "0")
                {
                    if (Motherboard_type == 0 || Motherboard_type == 1 || Motherboard_type == 4)
                    {
                        tabPage3.Parent = null;
                        tabPage3.Hide();
                        tabPage1.Parent = null;
                        tabPage1.Hide();
                        tabPage14.Parent = null;
                        tabPage14.Hide();
                    }
                }
                else
                {
                    tabPage5.Parent = null;
                    tabPage5.Hide();
                    tabPage14.Parent = null;
                    tabPage14.Hide();
                    tabPage4.Parent = null;
                    tabPage4.Hide();
                    tabPage3.Parent = null;
                    tabPage3.Hide();
                    tabPage1.Parent = null;
                    tabPage1.Hide();
                    tabPage2.Parent = null;
                    tabPage2.Hide();

                    tabPage2.Parent = tabControl1;
                    tabPage2.Show();
                    if (Motherboard_type == 0 || Motherboard_type == 1 || Motherboard_type == 4)
                    {
                        tabPage1.Parent = tabControl1;
                        tabPage1.Show();
                        tabPage3.Parent = tabControl1;
                        tabPage3.Show();
                    }
                    tabPage4.Parent = tabControl1;
                    tabPage4.Show();
                    tabPage14.Parent = tabControl1;
                    tabPage14.Show();
                    tabPage5.Parent = tabControl1;
                    tabPage5.Show();
                }
            }
            else if (Motherboard_type == 1)
            {
                vIDEO1ToolStripMenuItem.Visible = true;
                vIDEO2ToolStripMenuItem.Visible = true;
                vIDEO3ToolStripMenuItem.Visible = true;
                vIDEO4ToolStripMenuItem.Visible = true;
                sVIDEOToolStripMenuItem.Visible = true;
                yPbPrToolStripMenuItem.Visible = true;
                vIDEO1ToolStripMenuItem.Text = "HDMI1";
                vIDEO2ToolStripMenuItem.Text = "HDMI2";
                vIDEO3ToolStripMenuItem.Text = "OPS";
                vIDEO4ToolStripMenuItem.Text = "DVI";
                sVIDEOToolStripMenuItem.Text = "DP";
                yPbPrToolStripMenuItem.Text = "VGA";
                vGAToolStripMenuItem.Visible = false;
                hDMIToolStripMenuItem.Visible = false;
                dVIToolStripMenuItem.Visible = false;
                comboBox1.Visible = false;
                comboBox3.Visible = false;
                comboBox4.Visible = true;
                comboBox5.Visible = false;
                comboBox6.Visible = false;
                button15.Show();
                if (s1 == "0")
                {
                    if (Motherboard_type == 0 || Motherboard_type == 1 || Motherboard_type == 4)
                    {
                        tabPage3.Parent = null;
                        tabPage3.Hide();
                        tabPage1.Parent = null;
                        tabPage1.Hide();
                        tabPage14.Parent = null;
                        tabPage14.Hide();
                    }
                }
                else//先全部隐藏，再依次显示出来
                {
                    tabPage5.Parent = null;
                    tabPage5.Hide();
                    tabPage14.Parent = null;
                    tabPage14.Hide();
                    tabPage4.Parent = null;
                    tabPage4.Hide();
                    tabPage3.Parent = null;
                    tabPage3.Hide();
                    tabPage1.Parent = null;
                    tabPage1.Hide();
                    tabPage2.Parent = null;
                    tabPage2.Hide();

                    tabPage2.Parent = tabControl1;
                    tabPage2.Show();
                    if (Motherboard_type == 0 || Motherboard_type == 1 || Motherboard_type == 4)
                    {
                        tabPage1.Parent = tabControl1;
                        tabPage1.Show();
                        tabPage3.Parent = tabControl1;
                        tabPage3.Show();
                    }
                    tabPage4.Parent = tabControl1;
                    tabPage4.Show(); 
                    tabPage14.Parent = tabControl1;
                    tabPage14.Show();
                    tabPage5.Parent = tabControl1;
                    tabPage5.Show();    
                }
            }
            else if (Motherboard_type == 4)
            {
                vIDEO1ToolStripMenuItem.Visible = true;
                vIDEO2ToolStripMenuItem.Visible = true;
                vIDEO3ToolStripMenuItem.Visible = true;
                vIDEO4ToolStripMenuItem.Visible = true;
                sVIDEOToolStripMenuItem.Visible = true;
                yPbPrToolStripMenuItem.Visible = false;
                vIDEO1ToolStripMenuItem.Text = "HDMI1";
                vIDEO2ToolStripMenuItem.Text = "HDMI2";
                vIDEO3ToolStripMenuItem.Text = "OPS";
                vIDEO4ToolStripMenuItem.Text = "DVI";
                sVIDEOToolStripMenuItem.Text = "DP";
                yPbPrToolStripMenuItem.Text = "VGA";
                vGAToolStripMenuItem.Visible = false;
                hDMIToolStripMenuItem.Visible = false;
                dVIToolStripMenuItem.Visible = false;
                comboBox1.Visible = true;
                comboBox3.Visible = false;
                comboBox4.Visible = false;
                comboBox5.Visible = false;
                comboBox6.Visible = false;
                button15.Show();
                if (s1 == "0")
                {
                    if (Motherboard_type == 0 || Motherboard_type == 1 || Motherboard_type == 4)
                    {
                        tabPage3.Parent = null;
                        tabPage3.Hide();
                        tabPage1.Parent = null;
                        tabPage1.Hide();
                        tabPage14.Parent = null;
                        tabPage14.Hide();
                    }
                }
                else//先全部隐藏，再依次显示出来
                {
                    tabPage5.Parent = null;
                    tabPage5.Hide();
                    tabPage14.Parent = null;
                    tabPage14.Hide();
                    tabPage4.Parent = null;
                    tabPage4.Hide();
                    tabPage3.Parent = null;
                    tabPage3.Hide();
                    tabPage1.Parent = null;
                    tabPage1.Hide();
                    tabPage2.Parent = null;
                    tabPage2.Hide();

                    tabPage2.Parent = tabControl1;
                    tabPage2.Show();
                    if (Motherboard_type == 0 || Motherboard_type == 1 || Motherboard_type == 4)
                    {
                        tabPage1.Parent = tabControl1;
                        tabPage1.Show();
                        tabPage3.Parent = tabControl1;
                        tabPage3.Show();
                    }
                    tabPage4.Parent = tabControl1;
                    tabPage4.Show();
                    tabPage14.Parent = tabControl1;
                    tabPage14.Show();
                    tabPage5.Parent = tabControl1;
                    tabPage5.Show();
                }
            }
            else if (Motherboard_type == 2)
            {
                vIDEO1ToolStripMenuItem.Visible = true;
                vIDEO2ToolStripMenuItem.Visible = true;
                vIDEO3ToolStripMenuItem.Visible = true;
                vIDEO4ToolStripMenuItem.Visible = true;
                sVIDEOToolStripMenuItem.Visible = true;
                yPbPrToolStripMenuItem.Visible = true;
                vGAToolStripMenuItem.Visible = true;
                hDMIToolStripMenuItem.Visible = true;
                dVIToolStripMenuItem.Visible = true;
                vIDEO1ToolStripMenuItem.Text = "VIDEO1";
                vIDEO2ToolStripMenuItem.Text = "VIDEO2";
                vIDEO3ToolStripMenuItem.Text = "VIDEO3";
                vIDEO4ToolStripMenuItem.Text = "VIDEO4";
                sVIDEOToolStripMenuItem.Text = "S-VIDEO";
                yPbPrToolStripMenuItem.Text = "YPbPr";
                vGAToolStripMenuItem.Text = "VGA";
                hDMIToolStripMenuItem.Text = "HDMI";
                dVIToolStripMenuItem.Text = "DVI";
                comboBox1.Visible = false;
                comboBox4.Visible = false;
                comboBox3.Visible = false;
                comboBox6.Visible = true;
                comboBox5.Visible = false;
                tabPage3.Parent = null;
                tabPage3.Hide();
                tabPage1.Parent = null;
                tabPage1.Hide();
                tabPage14.Parent = null;
                tabPage14.Hide();
                button15.Hide();
            }
            else if (Motherboard_type == 3)
            {
                vIDEO1ToolStripMenuItem.Visible = true;
                vIDEO2ToolStripMenuItem.Visible = false;
                vIDEO3ToolStripMenuItem.Visible = false;
                vIDEO4ToolStripMenuItem.Visible = false;
                sVIDEOToolStripMenuItem.Visible = false;
                yPbPrToolStripMenuItem.Visible = false;
                vGAToolStripMenuItem.Visible = true;
                hDMIToolStripMenuItem.Visible = true;
                dVIToolStripMenuItem.Visible = true;
                vIDEO1ToolStripMenuItem.Text = "BNC";
                vIDEO2ToolStripMenuItem.Text = "VIDEO2";
                vIDEO3ToolStripMenuItem.Text = "VIDEO3";
                vIDEO4ToolStripMenuItem.Text = "VIDEO4";
                sVIDEOToolStripMenuItem.Text = "S-VIDEO";
                yPbPrToolStripMenuItem.Text = "YPbPr";
                vGAToolStripMenuItem.Text = "VGA";
                hDMIToolStripMenuItem.Text = "HDMI";
                dVIToolStripMenuItem.Text = "DVI";
                comboBox1.Visible = false;
                comboBox4.Visible = false;
                comboBox3.Visible = false;
                comboBox6.Visible = false;
                comboBox5.Visible = true;
                tabPage3.Parent = null;
                tabPage3.Hide();
                tabPage1.Parent = null;
                tabPage1.Hide();
                tabPage14.Parent = null;
                tabPage14.Hide();
                button15.Hide();
            }
        }

        private void Init_Scene()
        {
            string s = settingFile.ReadString("SCENE", "S1", "Scene - 1");
            //comboBoxEx1.Items[1]
            comboItem1.Text = s;
            s = settingFile.ReadString("SCENE", "S2", "Scene - 2");
            comboItem2.Text = s;
            s = settingFile.ReadString("SCENE", "S3", "Scene - 3");
            comboItem3.Text = s;
            s = settingFile.ReadString("SCENE", "S4", "Scene - 4");
            comboItem4.Text = s;
            s = settingFile.ReadString("SCENE", "S5", "Scene - 5");
            comboItem5.Text = s;
            s = settingFile.ReadString("SCENE", "S6", "Scene - 6");
            comboItem6.Text = s;

            comboBoxEx1.SelectedIndex = settingFile.ReadInteger("SCENE", "INDEX", 0);
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
        }
        /// <summary>
        /// 语言设置加载
        /// </summary>
        private void Init_FormString()
        {
            this.Text = languageFile.ReadString("MAINFORM", "TITLE", "拼接控制软件");
            this.tabPage6.Text = languageFile.ReadString("MAINFORM", "COMMUNICATION", "通讯设置");
            this.tabPage7.Text = languageFile.ReadString("MAINFORM", "FUNCTION", "功能设定");
            this.tabPage8.Text = languageFile.ReadString("MAINFORM", "ADDITIONAL", "附加功能");
            this.tabPage9.Text = languageFile.ReadString("MAINFORM", "ABOUT", "关于");
            this.tabPage10.Text = languageFile.ReadString("MAINFORM", "HELP", "帮助");
            this.tabPage13.Text = languageFile.ReadString("MAINFORM", "OPERATING1", "拼接操作");
            this.tabPage12.Text = languageFile.ReadString("MAINFORM", "OPERATING2", "场景操作");
            this.tabPage11.Text = languageFile.ReadString("MAINFORM", "HOME", "主页");
            this.tabPage1.Text = languageFile.ReadString("MAINFORM", "UNION", "单元序号");
            this.tabPage2.Text = languageFile.ReadString("MAINFORM", "PINGC", "屏参设置");
            this.tabPage3.Text = languageFile.ReadString("MAINFORM", "SYSTEM", "系统配置");
            this.tabPage4.Text = languageFile.ReadString("MAINFORM", "SCREEN", "场景模式");
            this.tabPage5.Text = languageFile.ReadString("MAINFORM", "DEBUG", "调试");
            this.button14.Text = languageFile.ReadString("MAINFORM", "COM", "串口设置");
            this.button15.Text = languageFile.ReadString("MAINFORM", "TIME", "定时设置");
            this.button7.Text = languageFile.ReadString("MAINFORM", "OPEN", "开连接");
            this.button6.Text = languageFile.ReadString("MAINFORM", "COLSE", "关连接");
            this.button18.Text = languageFile.ReadString("MAINFORM", "ADMINISTRATOR", "管理员");
            this.button19.Text = languageFile.ReadString("MAINFORM", "REGISTER", "软件注册");
            this.button16.Text = languageFile.ReadString("MAINFORM", "SPLIC_SET", "拼接设置");
            this.button20.Text = languageFile.ReadString("MAINFORM", "TXT_LOGO", "字幕Logo");
            this.button17.Text = languageFile.ReadString("MAINFORM", "REMOTE", "遥控控制");
            this.button21.Text = languageFile.ReadString("MAINFORM", "SYS_INFO", "系统信息");
            this.button22.Text = languageFile.ReadString("MAINFORM", "ABOUT_SOFT", "关于软件");
            this.button23.Text = languageFile.ReadString("MAINFORM", "MANUAL", "用户手册");

            this.expandablePanel1.TitleText = languageFile.ReadString("MAINFORM", "SINGLE", "信号选择");
            this.label19.Text = languageFile.ReadString("MAINFORM", "SINGLETYPE", "信号类型");
            this.label20.Text = languageFile.ReadString("MAINFORM", "MATRIXCHANNEL", "矩阵输入通道");
            this.button30.Text = languageFile.ReadString("MAINFORM", "RUN", "执行");
            this.checkBox2.Text = languageFile.ReadString("MAINFORM", "LINKAGE ", "矩阵联动");
            this.button28.Text = languageFile.ReadString("MAINFORM", "SPLIC_SHOW", "合并");
            this.button27.Text = languageFile.ReadString("MAINFORM", "SINGLE_SHOW", "分解");
            this.expandablePanel2.TitleText = languageFile.ReadString("MAINFORM", "POWER", "电源");

            this.button8.Text = languageFile.ReadString("MAINFORM", "ON", "开机");
            this.button5.Text = languageFile.ReadString("MAINFORM", "OFF", "关机");


            this.groupBox3.Text = languageFile.ReadString("MAINFORM", "SHOWUNION", "显示(序列号、地址)");
            this.button1.Text = languageFile.ReadString("MAINFORM", "SERIAL", "显示序列号");
            this.button3.Text = languageFile.ReadString("MAINFORM", "ADDRESS", "显示地址");
            this.button34.Text = languageFile.ReadString("MAINFORM", "SERIAL1", "隐藏序列号");
            this.button35.Text = languageFile.ReadString("MAINFORM", "ADDRESS1", "隐藏地址");
            this.label3.Text = languageFile.ReadString("MAINFORM", "UNIONSERIAL", "单元序列号:");
            this.label5.Text = languageFile.ReadString("MAINFORM", "ROW", "行数:");
            this.label6.Text = languageFile.ReadString("MAINFORM", "COLU", "列数:");
            this.button2.Text = languageFile.ReadString("MAINFORM", "BINDADDR", "绑定地址");

            this.label7.Text = languageFile.ReadString("MAINFORM", "AGEING", "老化:");
            this.label13.Text = languageFile.ReadString("MAINFORM", "BLUE", "蓝屏:");
            this.label2.Text = languageFile.ReadString("MAINFORM", "WHITE", "白场:");
            this.label17.Text = languageFile.ReadString("MAINFORM", "TI_MODE", "Ti Mode:");
            this.label11.Text = languageFile.ReadString("MAINFORM", "AB_MODE", "AB Mode:");
            this.radioButton1.Text = languageFile.ReadString("MAINFORM", "STATUS_ON", "开");
            this.radioButton9.Text = languageFile.ReadString("MAINFORM", "STATUS_ON", "开");
            this.radioButton18.Text = languageFile.ReadString("MAINFORM", "STATUS_ON", "开");
            this.radioButton2.Text = languageFile.ReadString("MAINFORM", "STATUS_OFF", "关");
            this.radioButton10.Text = languageFile.ReadString("MAINFORM", "STATUS_OFF", "关");
            this.radioButton19.Text = languageFile.ReadString("MAINFORM", "STATUS_OFF", "关");

            this.label14.Text = languageFile.ReadString("MAINFORM", "OSD_LANGUAGE", "OSD语言:");
            this.label16.Text = languageFile.ReadString("MAINFORM", "RESET_DEFAULT", "恢复默认:");
            this.radioButton11.Text = languageFile.ReadString("MAINFORM", "OSD_CH", "中文");
            this.radioButton12.Text = languageFile.ReadString("MAINFORM", "OSD_EN", "英文");
            this.button12.Text = languageFile.ReadString("MAINFORM", "RESET", "复位");
            this.button4.Text = languageFile.ReadString("MAINFORM", "UPDATE", "升级");
            this.button33.Text = languageFile.ReadString("MAINFORM", "FACTORY", "        工厂          复位");
            this.groupBox7.Text = languageFile.ReadString("MAINFORM", "SYS_UPDATE", "系统升级");
            this.label9.Text = languageFile.ReadString("MAINFORM", "PASSWORD", "密码:");

            this.groupBox5.Text = languageFile.ReadString("MAINFORM", "SCENES_COMB", "场景组合");
            string sc = languageFile.ReadString("MAINFORM", "SCENES", "场景");
            this.bt_sce1.Text = sc + "1";
            this.bt_sce2.Text = sc + "2";
            this.bt_sce3.Text = sc + "3";
            this.bt_sce4.Text = sc + "4";
            this.bt_sce5.Text = sc + "5";
            this.bt_sce6.Text = sc + "6";
            this.bt_sce7.Text = sc + "7";
            this.bt_sce8.Text = sc + "8";
            this.bt_sce9.Text = sc + "9";
            this.bt_sce10.Text = sc + "10";
            this.bt_sce11.Text = sc + "11";
            this.bt_sce12.Text = sc + "12";
            this.button29.Text = languageFile.ReadString("MAINFORM", "SAVESCENES", "保存");
            this.button25.Text = languageFile.ReadString("MAINFORM", "AUTOSCENES", "自动轮巡");

            this.groupBox8.Text = languageFile.ReadString("MAINFORM", "COMDEBUG", "串口调试");
            this.label10.Text = languageFile.ReadString("MAINFORM", "ENTER", "输入:");
            this.checkBox1.Text = languageFile.ReadString("MAINFORM", "HEXMODE", "HEX发送");
            this.button9.Text = languageFile.ReadString("MAINFORM", "CLEARENTER", "清空");
            this.button11.Text = languageFile.ReadString("MAINFORM", "SENDENTER", "发送");

            this.button10.Text = languageFile.ReadString("MAINFORM", "HIDEINST", "隐藏");
            this.button13.Text = languageFile.ReadString("MAINFORM", "CLEARENTER", "清空");
            this.button32.Text = languageFile.ReadString("MAINFORM", "SHOWINST", "显示");

            this.mergeToolStripMenuItem.Text = languageFile.ReadString("MAINFORM", "SPLIC_SHOW", "合并");
            this.unMergeToolStripMenuItem.Text = languageFile.ReadString("MAINFORM", "SINGLE_SHOW", "分解");
            this.开屏电源ToolStripMenuItem.Text = languageFile.ReadString("MAINFORM", "BACKGROUND_ON", "背光开");
            this.关屏电源ToolStripMenuItem.Text = languageFile.ReadString("MAINFORM", "BACKGROUND_OFF", "背光关");
            this.本地通道ToolStripMenuItem.Text = languageFile.ReadString("MAINFORM", "LOCAL_CHANEL", "本地通道");
            this.屏幕参数调整ToolStripMenuItem.Text = languageFile.ReadString("MAINFORM", "SCREEN_ADJUST", "屏幕参数调整");

            this.c复制ToolStripMenuItem.Text = languageFile.ReadString("MAINFORM", "COPY", "复制");
            this.v粘贴ToolStripMenuItem.Text = languageFile.ReadString("MAINFORM", "PASTE", "粘贴");

        }
        /// <summary>
        /// 设置透明按钮样式
        /// </summary>
        private void SetBtnStyle(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;//样式
            //btn.ForeColor = Color.Transparent;//前景
            btn.BackColor = Color.Transparent;//去背景
            btn.FlatAppearance.BorderSize = 0;//去边线
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;//鼠标经过
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;//鼠标按下
        }
        private void btn_MouseHover(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatAppearance.BorderSize = 2;
            btn.BackColor = Color.Yellow;
        }

        private void btn_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.Transparent;
        }

        /// <summary>
        /// 串口插拔的消息处理
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICE_CHANGE)        // 捕获USB设备的拔出消息WM_DEVICECHANGE
            {
                switch (m.WParam.ToInt32())
                {
                    case DBT_DEVICE_REMOVE_COMPLETE:    // USB拔出      
                        {
                            if (Rs232Con)
                            {
                                bool com = false;
                                String[] serialPorts = System.IO.Ports.SerialPort.GetPortNames();
                                for (int i = 0; i < serialPorts.Length; i++)//找出所有串口，并选择文件中的
                                {
                                    if (serialPorts[i].Equals(PortName))
                                        com = true;
                                    //Console.WriteLine(serialPorts[i]);
                                }
                                if (!com)
                                {
                                    if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                                    {
                                        stopProgress();
                                    }
                                    Rs232Con = false;
                                    button7.Enabled = true;
                                    button14.Enabled = true;
                                    button5.Enabled = false;
                                    button6.Enabled = false;
                                    button8.Enabled = false;
                                    button27.Enabled = false;
                                    button28.Enabled = false;
                                    button30.Enabled = false;
                                    toolStripStatusLabel3.ForeColor = Color.Red;

                                    string sp = languageFile.ReadString("MAINFORM", "STATUS_OFF", "关闭");
                                    toolStripStatusLabel3.Text = PortName + "  " + sp + " " + BaudRate + " Bps";
                                    string ts = languageFile.ReadString("MESSAGEBOX", "M8", "串口已断开，请重新设置打开");
                                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                                    MessageBox.Show(ts, tp);
                                    /*
                                    if (Chinese_English)
                                    {
                                        toolStripStatusLabel3.Text = " Port：" + PortName + "Disconnect" + BaudRate + " Bps";
                                        MessageBox.Show("Serial port is disconnected, Please reset open！", "Tips");
                                    }
                                    else
                                    {
                                        toolStripStatusLabel3.Text = " 端口：" + PortName + " 已断开  " + BaudRate + " Bps";
                                        MessageBox.Show("串口已断开，请重新设置打开", "提示");
                                    }
                                     */ 
                                }
                            }
                        }
                        break;
                    case DBT_DEVICEARRIVAL:             // USB插入获取对应串口名称
                        break;
                }
            }
            base.WndProc(ref m);
        }

        public void s_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                //Comm.BytesToRead中为要读入的字节长度
                int len = serialPort1.BytesToRead;
                Byte[] readBuffer = new Byte[len];
                serialPort1.Read(readBuffer, 0, len); //将数据读入缓存
                //处理readBuffer中的数据，自定义处理过程
                Encoding encoding = Encoding.Default;
                string msg = encoding.GetString(readBuffer, 0, len);
                //Console.WriteLine(msg + "0000000" );
                this.BeginInvoke(new MethodInvoker(delegate()
                {
                    richTextBox3.Text += msg;
                    richTextBox3.SelectionStart = richTextBox3.Text.Length;
                    //LogHelper.DebugLog(msg);
                    //richTextBox3.Focus();
                }));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("接收返回打印消息异常！", ex);
                //MessageBox.Show("接收返回消息异常！具体原因：" + ex.Message, "提示信息");
            }
        }

        #region 初始化设置
        /// <summary>
        /// 默认从文件中初始化grid,初始化串口，地址
        /// </summary>
        /// <param name="directName">文件夹路径</param>
        public void initRoGridFromFile(String directName) 
        {
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            currentConnectionName =  Application.StartupPath + "\\myWall";
            /*
            if ("".Equals(directName))//为""则从setting中取
            {
                currentConnectionName = settingFile.ReadString("SETTING", "CurrDirect", Application.StartupPath + "\\myWall");
                int k = settingFile.ReadInteger("SETTING", "Resolution", currentConnectionName.Length);
                currentConnectionName = currentConnectionName.Substring(0,k);
                //Console.WriteLine("currentConnectionName = " + currentConnectionName +"k = "+k);
            }
            else 
            {
                currentConnectionName = directName;
                settingFile.WriteString("SETTING", "CurrDirect", currentConnectionName);
            }
             */ 
           
            currentSceneName = "scene.rgf";

            FileHandle.selectDirectory(currentConnectionName);//直接打开，不覆盖
                //firstInit = false;
           
            FileInfo file = new FileInfo(currentConnectionName + "\\" + currentSceneName);
            if ((!file.Exists))
            {
                initRoGridControl(2, 2);//默认2行2列
                saveSceneFile(currentConnectionName + "\\"+ currentSceneName);
            }
           else 
            {
                readSceneFile(currentConnectionName + "\\" + currentSceneName);
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
                //initAddress(sheet.RowCount * sheet.ColumnCount);   
                initReoGridConrol2();
            }
            rowsCount = sheet.RowCount;
            colsCount = sheet.ColumnCount;

            initSceneButton();//初始化按钮
            setSce1CurrentSceneButtonText();//设置场景一按钮的文本为当前场景文本
            //setLbModeFlag(1);//设置当前模式

            for (int i = 1; i <= rowsCount * colsCount; i++)
            {
                address[i] = settingFile.ReadInteger("ADDR", (i) + "Addr:", 0);
                //Console.WriteLine(address[i]);
            }

            PortName = settingFile.ReadString("Com Set", "port1", "COM1");
            PortName2 = settingFile.ReadString("Com Set", "port2", "COM1");
            BaudRate = int.Parse(settingFile.ReadString("Com Set", "baudrate1", "9600"));
            BaudRate2 = int.Parse(settingFile.ReadString("Com Set", "baudrate2", "9600"));
            uMultiComPort = settingFile.ReadInteger("Com Set", "MultiCom", 1);
            Matrix_time = (settingFile.ReadInteger("SETTING", "Matrix_time", 1) +1) * 100;
            Matrix_flag = settingFile.ReadInteger("SETTING", "Matrix_flag", 0);
            //Console.WriteLine(PortName + BaudRate);
            Init_port();
            string s1 = settingFile.ReadString("SETTING", "Pwd", "0");
            if (!s1.Equals(""))
            {
                if (s1 == "0")
                {
                    //toolStripStatusLabel2.Text = "用户权限：  普通用户！";
                    tabPage5.Parent = null;
                    tabPage5.Hide();
                    tabPage4.Parent = null;
                    tabPage4.Hide();
                    if (Motherboard_type == 0 || Motherboard_type == 1 || Motherboard_type == 4)
                    {
                        tabPage3.Parent = null;
                        tabPage3.Hide();
                        tabPage1.Parent = null;
                        tabPage1.Hide();
                    }
                    tabPage2.Parent = null;
                    tabPage2.Hide();
                    button10.Visible = false;
                    button13.Visible = false;
                    button32.Visible = false;
                    richTextBox2.Visible = false;
                    pictureBox1.Visible = true;
                    屏幕参数调整ToolStripMenuItem.Visible = false;
                    screenNumberToolStripMenuItem.Visible = false;
                }
                if (s1 == "1")
                {
                    //toolStripStatusLabel2.Text = "用户权限：  管理员用户！";
                    tabPage2.Parent = tabControl1;
                    tabPage2.Show();
                    if (Motherboard_type == 0 || Motherboard_type == 1 || Motherboard_type == 4)
                    {
                        tabPage1.Parent = tabControl1;
                        tabPage1.Show();
                        tabPage3.Parent = tabControl1;
                        tabPage3.Show();
                    }
                    tabPage4.Parent = tabControl1;
                    tabPage4.Show();
                    tabPage5.Parent = tabControl1;
                    tabPage5.Show();
                    button10.Visible = false;
                    button13.Visible = false;
                    richTextBox2.Visible = false;
                    button32.Visible = true;
                    屏幕参数调整ToolStripMenuItem.Visible = true;
                    screenNumberToolStripMenuItem.Visible = true;
                }
            }
        }


        /// <summary>
        /// 初始化场景按钮的可选
        /// </summary>
        public void initSceneButton()
        {
            //selectSceneButtonEnable(bt_sce1, "scene1.rgf");
            //selectSceneButtonEnable(bt_sce2, "scene2.rgf");
            buttonChangeImg(bt_sce1);
            //selectSceneButtonEnable(bt_sce1, "scene1.rgf", bt_sce2);
            selectSceneButtonEnable(bt_sce2, "scene2.rgf", bt_sce3);
            selectSceneButtonEnable(bt_sce3, "scene3.rgf", bt_sce4);
            selectSceneButtonEnable(bt_sce4, "scene4.rgf", bt_sce5);
            selectSceneButtonEnable(bt_sce5, "scene5.rgf", bt_sce6);
            selectSceneButtonEnable(bt_sce6, "scene6.rgf", bt_sce7);
            selectSceneButtonEnable(bt_sce7, "scene7.rgf", bt_sce8);
            selectSceneButtonEnable(bt_sce8, "scene8.rgf", bt_sce9);
            selectSceneButtonEnable(bt_sce9, "scene9.rgf", bt_sce10);
            selectSceneButtonEnable(bt_sce10, "scene10.rgf", bt_sce11);
            selectSceneButtonEnable(bt_sce11, "scene11.rgf", bt_sce12);
            selectSceneButtonEnable(bt_sce12, "scene12.rgf", null);

            if (prepareCreateBt != null)
            {
                prepareCreateBt.Enabled = true;
                prepareCreateBt = null;
            }
            else
            {
                bt_sce2.Enabled = true;
            }
        }

        /// <summary>
        /// 从文件中读取并初始化矩阵信息，端口数，信源地址，矩阵选择
        /// </summary>
        public void initMatrixFromFile() {

            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            //Console.WriteLine("initMatrixFromFile");

            //初始化端口数
            uDvi = (byte)settingFile.ReadInteger("Matrix", "DVICount", 0);
            uVideo = (byte)settingFile.ReadInteger("Matrix", "VIDEOCount", 0);
            uVga = (byte)settingFile.ReadInteger("Matrix", "VGACount", 0);
            uHdmi = (byte)settingFile.ReadInteger("Matrix", "HDMICount", 0);
            uYPbPr = (byte)settingFile.ReadInteger("Matrix", "YPbPrCount", 0);

            //初始化信源地址
            dviAddress = settingFile.ReadInteger("Matrix", "DVIAddress", 0);
            videoAddress = settingFile.ReadInteger("Matrix", "VIDEOAddress", 0);
            vgaAddress = settingFile.ReadInteger("Matrix", "VGAAddress", 0);
            hdmiAddress = settingFile.ReadInteger("Matrix", "HDMIAddress", 0);
            ypbprAddress = settingFile.ReadInteger("Matrix", "YPbPrAddress", 0);

            //初始化矩阵选择
            dviMatrixSelect = settingFile.ReadInteger("Matrix", "DVIMatrix", 0) + 1;
            videoMatrixSelect = settingFile.ReadInteger("Matrix", "VIDEOMatrix", 0) + 1;
            vgaMatrixSelect = settingFile.ReadInteger("Matrix", "VGAMatrix", 0) + 1;
            hdmiMatrixSelect = settingFile.ReadInteger("Matrix", "HDMIMatrix", 0) + 1;
            ypbprMatrixSelect = settingFile.ReadInteger("Matrix", "YPbPrMatrix", 0);

            dviMatrix = settingFile.ReadString("Matrix", "dvi-Matrix", "");
            hdmiMatrix = settingFile.ReadString("Matrix", "hdmi-Matrix", "");
            videoMatrix = settingFile.ReadString("Matrix", "video-Matrix", "");
            vgaMatrix = settingFile.ReadString("Matrix", "vga-Matrix", "");
            comboBox2.Items.Clear();
            //Console.WriteLine(dviMatrixSelect + "," + hdmiMatrixSelect);
        }
        /// <summary>
        /// 通过文件是否存在设置按钮可用
        /// </summary>
        /// <param name="button"></param>
        /// <param name="fileName"></param>
        public void selectSceneButtonEnable(Button button, String fileName, Button preButton)
        {
            FileInfo file = new FileInfo(currentConnectionName + "\\" + fileName);
            if (!file.Exists)
            {
                button.BackgroundImage = null;
                button.Enabled = false;
            }
            else
            {
                button.BackgroundImage = global::WallControl.Properties.Resources.bg35;
                prepareCreateBt = preButton;
                button.Enabled = true;
                if (file.Name.Equals("scene12.rgf"))
                    bt_sce12_Click_flag = true;
            }
        }


        /// <summary>
        /// 要重置整个工作簿
        /// </summary>
        public void ResetGrid()
        {
            reoGridControl1.Reset();
            reoGridControl2.Reset();
        }


        /// <summary>
        /// 初始化grid设置
        /// </summary>
        public void initRoGridSet() 
        {
            sheet = reoGridControl1.CurrentWorksheet;
            sheet_back = reoGridControl2.CurrentWorksheet; 
            sheet.Reset();
            sheet_back.Reset();
            Thread.Sleep(100);
            reoGridControl1.Readonly = true;
            reoGridControl2.Readonly = true;
            sheet.SelectionRangeChanging += sheetSelectionRangeChanging;
            sheet.SelectionRangeChanged += sheetSelectionRangeChanged;

            sheet_back.SelectionRangeChanging += sheet_backSelectionRangeChanging;
            sheet_back.SelectionRangeChanged += sheet_backSelectionRangeChanged;
            // reoGridControl1.CurrentWorksheet.RowHeaderWidth = 0;
            reoGridControl1.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowScrolls, false);//关闭滚动条
            reoGridControl2.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowScrolls, false);//关闭滚动条
            reoGridControl1.CurrentWorksheet.SetSettings(unvell.ReoGrid.WorksheetSettings.View_ShowHeaders, false);//关闭行头和列头
            reoGridControl2.CurrentWorksheet.SetSettings(unvell.ReoGrid.WorksheetSettings.View_ShowHeaders, false);//关闭行头和列头
            
            //sheet.ColumnHeaders[0].Text = "1";
            //for (int i = 0; i < colsCount; i++)
            {
                //sheet.ColumnHeaders[i].Text = (i + 1).ToString();
            }
            //reoGridControl1.CurrentWorksheet.RowHeaderWidth = 15;
            //reoGridControl2.CurrentWorksheet.RowHeaderWidth = 15;
            //reoGridControl1.CurrentWorksheet.SetRowsHeight(1, reoGridControl1.CurrentWorksheet.RowCount, 50);
            //reoGridControl1.CurrentWorksheet.SetColumnsWidth(1, reoGridControl1.CurrentWorksheet.RowCount, 50);
            reoGridControl1.CurrentWorksheet.AutoFitColumnWidth(0,true);
            reoGridControl1.CurrentWorksheet.AutoFitRowHeight(0, true);
            reoGridControl2.CurrentWorksheet.AutoFitColumnWidth(0, true);
            reoGridControl2.CurrentWorksheet.AutoFitRowHeight(0, true);
        }

        private void initReoGridConrol2()
        {
            reoGridControl2.CurrentWorksheet.SetRows(rowsCount);//行数
            reoGridControl2.CurrentWorksheet.SetCols(colsCount);//列数
            reoGridControl2.CurrentWorksheet.Resize(rowsCount, colsCount);
            sheet_back.SetRowsHeight(0, sheet_back.RowCount, (ushort)((reoGridControl2.Size.Height) / rowsCount));//改行的高度，从0行开始，改ColumnCount行
            sheet_back.SetColumnsWidth(0, sheet_back.ColumnCount, (ushort)((reoGridControl2.Size.Width) / colsCount));//改列的宽度，从0列开始，改ColumnCount列
            for (int i = 0; i < colsCount; i++)//初始值
            {
                for (int j = 0; j < rowsCount; j++)
                {
                    //Console.WriteLine(j * colCount + i + "===块");
                    //screens[j * colsCount + i] = new Screen();
                    //screens[j * colsCount + i].IntputType = defaultSignalName;
                    //screens[j * colsCount + i].Name = "U" + (j * colsCount + i + 1);
                    //screens[j * colsCount + i].Number = (j * colsCount + i + 1);
                    //sheet.SetCellData(j, i, screens[j * colsCount + i]);
                    sheet_back.SetCellData(j, i, screens[j * colsCount + i]);
                }
            }
            if (rowsCount * colsCount > 64 || rowsCount > 8 || colsCount > 8)//9*9以上
            {
                sheet_back.SetRangeStyles(new RangePosition(0, 0, rowsCount, colsCount), new WorksheetRangeStyle//0,0开始，rowCount行colCount列，左上角
                {
                    Flag = PlainStyleFlag.All,
                    HAlign = ReoGridHorAlign.Left,
                    VAlign = ReoGridVerAlign.Top,
                    TextColor = Color.Purple,
                    FontSize = 8,

                });
            }
            else
            {//9*9以下
                sheet_back.SetRangeStyles(new RangePosition(0, 0, rowsCount, colsCount), new WorksheetRangeStyle//0,0开始，rowCount行colCount列，左上角
                {
                    Flag = PlainStyleFlag.All,
                    HAlign = ReoGridHorAlign.Left,
                    VAlign = ReoGridVerAlign.Top,
                    TextColor = Color.Purple,
                    FontSize = 12,
                    //BackColor = Color.Silver,
                    //FillPatternColor = Color.Green,
                });
            }
        }

        /// <summary>
        /// 初始化grid行列
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        public void initRoGridControl(int rowCount, int colCount)
        {
           // Console.WriteLine("rowCount=" + rowCount);
            //reoGridControl1.Reset();
            reoGridControl1.CurrentWorksheet.SetRows(rowCount);//行数
            reoGridControl1.CurrentWorksheet.SetCols(colCount);//列数
            reoGridControl2.CurrentWorksheet.SetRows(rowCount);//行数
            reoGridControl2.CurrentWorksheet.SetCols(colCount);//列数
            //Console.WriteLine("============");
            reoGridControl1.CurrentWorksheet.Resize(rowCount, colCount);
            reoGridControl2.CurrentWorksheet.Resize(rowCount, colCount);
            rowsCount = rowCount;
            colsCount = colCount;

            //for (int i = 0; i < colsCount; i++)
            {
                //sheet.ColumnHeaders[i].Text = (i +1).ToString();
            }

            sheet.SetRowsHeight(0, sheet.RowCount, (ushort)((reoGridControl1.Size.Height) / rowCount));//改行的高度，从0行开始，改ColumnCount行
            sheet.SetColumnsWidth(0, sheet.ColumnCount, (ushort)((reoGridControl1.Size.Width) / colCount));//改列的宽度，从0列开始，改ColumnCount列
            sheet_back.SetRowsHeight(0, sheet_back.RowCount, (ushort)((reoGridControl2.Size.Height) / rowCount));//改行的高度，从0行开始，改ColumnCount行
            sheet_back.SetColumnsWidth(0, sheet_back.ColumnCount, (ushort)((reoGridControl2.Size.Width) / colCount));//改列的宽度，从0列开始，改ColumnCount列
            //reoGridControl1.CurrentWorksheet.Resize(rowCount, colCount);
            screens = new Screen[rowsCount * colsCount];
            //Console.WriteLine(screens.Length + "===length");
            for (int i = 0; i < colCount; i++)//初始值
            {
                for (int j = 0; j < rowCount; j++)
                {
                    //Console.WriteLine(j * colCount + i + "===块");
                    screens[j * colCount + i] = new Screen();
                    screens[j * colCount + i].IntputType = defaultSignalName;
                    screens[j * colCount + i].Name = "U" + (j * colCount + i + 1);
                    //if ((j * colCount + i + 1) < 10)
                        //screens[j * colCount + i].Name = "U" + (j * colCount + i + 1).ToString() + " ";
                    screens[j * colCount + i].Number = (j * colCount + i + 1);
                    sheet.SetCellData(j, i, screens[j * colCount + i]);
                    sheet_back.SetCellData(j, i, screens[j * colCount + i]);
                }
            }
            if (rowsCount * colsCount > 64 || rowsCount > 8 || colsCount > 8)//9*9以上
            {
                sheet.SetRangeStyles(new RangePosition(0, 0, rowCount, colCount), new WorksheetRangeStyle//0,0开始，rowCount行colCount列，左上角
                {
                    Flag = PlainStyleFlag.All,
                    HAlign = ReoGridHorAlign.Left,
                    VAlign = ReoGridVerAlign.Top,
                    TextColor = Color.Purple,
                    FontSize = 8,

                });
                sheet_back.SetRangeStyles(new RangePosition(0, 0, rowCount, colCount), new WorksheetRangeStyle//0,0开始，rowCount行colCount列，左上角
                {
                    Flag = PlainStyleFlag.All,
                    HAlign = ReoGridHorAlign.Left,
                    VAlign = ReoGridVerAlign.Top,
                    TextColor = Color.Purple,
                    FontSize = 8,

                });
            }
             else 
            {//9*9以下
                 sheet.SetRangeStyles(new RangePosition(0, 0, rowCount, colCount), new WorksheetRangeStyle//0,0开始，rowCount行colCount列，左上角
                 {
                     Flag = PlainStyleFlag.All,
                     HAlign = ReoGridHorAlign.Left,
                     VAlign = ReoGridVerAlign.Top,
                     TextColor = Color.Purple,
                     FontSize = 12,
                 });
                 sheet_back.SetRangeStyles(new RangePosition(0, 0, rowCount, colCount), new WorksheetRangeStyle//0,0开始，rowCount行colCount列，左上角
                 {
                     Flag = PlainStyleFlag.All,
                     HAlign = ReoGridHorAlign.Left,
                     VAlign = ReoGridVerAlign.Top,
                     TextColor = Color.Purple,
                     FontSize = 12,
                 });
                 sheet.SelectionStyle = WorksheetSelectionStyle.Default;
             }

            //initAddress(sheet.ColumnCount * sheet.RowCount);
            //初始化合成屏组合非合成组
            mergeGroups = new List<MergeGroup>();//初始化合成组，保存所有的合成范围
            unMergeScreenList = new List<Screen>();//初始化非合成屏，保存所有的非合成的屏
            for (int i = 0; i < sheet.ColumnCount * sheet.RowCount; i++)//新建，则所有屏都未合成
            {
                unMergeScreenList.Add(screens[i]);
            }

        }

        #endregion

        /// <summary>
        /// 鼠标选择范围中的事件（不允许将合并过的单元格在和其他的单元格一起选择）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void sheetSelectionRangeChanging(object sender, RangeEventArgs args)
        {
            if (sheet.ColumnCount == 1 || sheet.RowCount == 1)
            { }
            //Console.WriteLine("sheetSelectionRangeChanging==" );
            //sheet.SelectionStyle = WorksheetSelectionStyle.FocusRect;
            //RangePosition oldRange = new RangePosition(oldLeftTopRow, oldLeftTopCol, oldRightBottomRow - oldLeftTopRow + 1, oldRightBottomCol - oldLeftTopCol + 1);
            else if (!sheet.CheckIntersectedMergingRange(args.Range).IsEmpty)
            {
                //选择上一项
                //Console.WriteLine("sheetSelectionRangeChanging==");
                //if (!(args.Range.Col == args.Range.EndCol && args.Range.EndCol == 0 || args.Range.Row == args.Range.EndRow && args.Range.EndRow == 0))
                sheet.SelectRange(oldLeftTopRow, oldLeftTopCol, oldRightBottomRow - oldLeftTopRow + 1, oldRightBottomCol - oldLeftTopCol + 1);
                //Console.WriteLine("有合成的(" + oldLeftTopCol + "," + oldLeftTopRow + ")->(" + oldRightBottomCol + "," + oldRightBottomRow + ")->(");
            }
                /*selecting = true;
                Console.WriteLine("不是从选中的位置开始==="); 
                oldLeftTopCol = args.Range.Col;
                oldLeftTopRow = args.Range.Row;
                oldRightBottomCol = args.Range.EndCol;
                oldRightBottomRow = args.Range.EndRow;
                
            }
            else
            {//连续的拖动鼠标选择
                int gap = Math.Abs(args.Range.Col - oldLeftTopCol) + Math.Abs(args.Range.Row - oldLeftTopRow) + Math.Abs(args.Range.EndCol - oldRightBottomCol) + Math.Abs(args.Range.EndRow - oldRightBottomRow);
                //Console.WriteLine("gap==" + gap);
                /*if (gap == 1)////间隔=1，说明是正常的逐步型拖动
                 {
                 */
            //sheet.SelectRange(oldLeftTopRow, oldLeftTopCol, oldRightBottomRow - oldLeftTopRow + 1, oldRightBottomCol - oldLeftTopCol + 1);
             
            if (isSelectionMergeRange(args.Range.Col, args.Range.Row, args.Range.EndCol, args.Range.EndRow))//看是不是一行或者一列的合成单元格
            {
                    //Console.WriteLine("有合成的(" + oldLeftTopCol + "," + oldLeftTopRow + ")->(" + oldRightBottomCol + "," + oldRightBottomRow + ")->(");
                    //sheet.SelectRange(oldLeftTopRow, oldLeftTopCol, oldRightBottomRow - oldLeftTopRow + 1, oldRightBottomCol - oldLeftTopCol + 1);
                    sheet.SelectRange(oldLeftTopRow, oldLeftTopCol, oldRightBottomRow - oldLeftTopRow + 1, oldRightBottomCol - oldLeftTopCol + 1);
                    //Console.WriteLine("有合成的(" + oldLeftTopRow + "," + oldLeftTopCol + ")->(" + (oldRightBottomRow - oldLeftTopRow + 1) + "," + (oldRightBottomCol - oldLeftTopCol + 1) + ")->(");
             }
            else
            {
                    oldLeftTopCol = args.Range.Col;
                    oldLeftTopRow = args.Range.Row;
                    oldRightBottomCol = args.Range.EndCol;
                    oldRightBottomRow = args.Range.EndRow;
                    
                    //Console.WriteLine("有合成的(" + args.Range.Col + "," + args.Range.Row + ")->(" + args.Range.EndCol + "," + args.Range.EndRow + ")->(");
            }
            if (isSelectionMergeRange(args.Range.Col, args.Range.Row, args.Range.EndCol + 1, args.Range.EndRow + 1))//看是不是一行或者一列的合成单元格
            {
                RangePosition One = sheet.CheckMergedRange((new RangePosition(sheet.SelectionRange.Row, sheet.SelectionRange.Col, 1, 1)));
                if (sheet.ColumnCount == 1 || sheet.RowCount == 1)
                    if (One.Cols != 1 || One.Rows != 1)
                    {
                        sheet.SelectRange(One); 
                    }
                //Console.WriteLine("有合成的(");
                //Console.WriteLine("有合成的(" + args.Range.Col + "," + args.Range.Row + ")->(" + args.Range.EndCol + "," + args.Range.EndRow + ")->(");
                //panel3.Enabled = false;

            }
            else
            {
                //panel3.Enabled = true;
            }
                // oldRangePosition = args.Range;
                //Console.WriteLine("oldRangePosition.StartPos=" + oldRangePosition.StartPos); 
                /* }
                 else
                 {//间隔>1，说明选到了合成的单元格，回退。
                     //Console.WriteLine("有合成的1");
                     sheet.SelectRange(oldLeftTopRow, oldLeftTopCol, oldRightBottomRow - oldLeftTopRow + 1, oldRightBottomCol - oldLeftTopCol + 1);

                 }*/
                      // oldRangePosition = args.Range;
                     //Console.WriteLine("oldRangePosition.StartPos=" + oldRangePosition.StartPos); 
            //}

                // Console.WriteLine("Selection changed: " + oldLeftTopCol + "," + oldLeftTopRow + ")->(" + oldRightBottomCol + "," + oldRightBottomRow + ")->(");
                // Console.WriteLine("Selection changed: " + args.Range.ToAddress());
            for (int i = 0; i < 256; i++)
                select_address[i] = 0;
            //if(Rs232Con)
                //button28.Enabled = true;
            for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
            {
                for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    select_address[num] = (byte)num;
                    //Console.WriteLine("select_address[num] = " + select_address[num] + "," + num);
                }
            }
            rowStar = sheet.SelectionRange.Row;
            rowEnd = sheet.SelectionRange.EndRow;
            colStar = sheet.SelectionRange.Col;
            colEnd = sheet.SelectionRange.EndCol;
        }

        private void sheet_backSelectionRangeChanging(object sender, RangeEventArgs args)
        {
            for (int i = 0; i < 256; i++)
                select_address[i] = 0;
            for (int j = sheet_back.SelectionRange.Row; j <= sheet_back.SelectionRange.EndRow; j++)
            {
                for (int i = sheet_back.SelectionRange.Col; i <= sheet_back.SelectionRange.EndCol; i++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    select_address[num] = (byte)num;
                    //Console.WriteLine("select_address[num] = " + select_address[num]);
                }
            }
            rowStar = sheet_back.SelectionRange.Row;
            rowEnd = sheet_back.SelectionRange.EndRow;
            colStar = sheet_back.SelectionRange.Col;
            colEnd = sheet_back.SelectionRange.EndCol;
        }

        public void promptSystemRun()
        {
            string ts = languageFile.ReadString("MESSAGEBOX", "M9", "系统正在忙碌中 !");
            string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
            MessageBox.Show(ts, tp, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            /*
            if(Chinese_English)
                MessageBox.Show(" The system is busy !  ", "Tips", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("   系统正在忙碌中 !   ", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
             */ 
        }
        /// <summary>
        /// 检查checkRange是否包含range
        /// </summary>
        /// <param name="range"></param>
        /// <param name="checkRange"></param>
        /// <returns></returns>
        private bool checkRangeContain(RangePosition range, RangePosition checkRange)
        {
            //Console.WriteLine("oldRange:" + range);
            //Console.WriteLine("args.Range:" + checkRange);

            //只要看checkRange左上角小于等于和右下角 都必须大于或等于range  就算包含了。
            if (!(checkRange.Col >= range.Col && checkRange.Row >= range.Row && (checkRange.Col + checkRange.Cols) <= (range.Col + range.Cols) && (checkRange.Row + checkRange.Rows) <= (range.Row + range.Rows)))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void sheetSelectionRangeChanged(object sender, RangeEventArgs args) 
        {
            //Console.WriteLine("选择完成");
            /*倒屏状态判断
            if (isSelectionMergeRange(args.Range.Col, args.Range.Row, args.Range.EndCol+1, args.Range.EndRow+1))//看是不是一行或者一列的合成单元格
            {
                panel3.Enabled = false;
            } 
            else
                panel3.Enabled = true;
             */ 
            //Console.WriteLine("(" + (sheet.SelectionRange.Row + 1) + "," + (sheet.SelectionRange.Col + 1) + ")——" + "(" + (sheet.SelectionRange.EndRow + 1) + "," + (sheet.SelectionRange.EndCol + 1) + ")");
            rowStar = sheet.SelectionRange.Row;
            rowEnd = sheet.SelectionRange.EndRow;
            colStar = sheet.SelectionRange.Col;
            colEnd = sheet.SelectionRange.EndCol;
            if (colStar == colEnd && rowStar == rowEnd)
            {
                numericUpDown2.Value = rowStar + 1;
                numericUpDown3.Value = colStar + 1;
            }
            //selecting = false;
            //保存上一步选择的区域,
            oldLeftTopCol = args.Range.Col;
            oldLeftTopRow = args.Range.Row;
            oldRightBottomCol = args.Range.EndCol;
            oldRightBottomRow = args.Range.EndRow;
        }

        private void sheet_backSelectionRangeChanged(object sender, RangeEventArgs args)
        {
            //Console.WriteLine("选择完成");
            rowStar = sheet_back.SelectionRange.Row;
            rowEnd = sheet_back.SelectionRange.EndRow;
            colStar = sheet_back.SelectionRange.Col;
            colEnd = sheet_back.SelectionRange.EndCol;
            //Console.WriteLine("(" + (sheet_back.SelectionRange.Row + 1) + "," + (sheet_back.SelectionRange.Col + 1) + ")——" + "(" + (sheet_back.SelectionRange.EndRow + 1) + "," + (sheet_back.SelectionRange.EndCol + 1) + ")");
            if (colStar == colEnd && rowStar == rowEnd)
            {
                numericUpDown2.Value = rowStar + 1;
                numericUpDown3.Value = colStar + 1;
            }
        }

        /// <summary>
        /// 分解、合成指令
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <param name="a4"></param>
        private void Send_merge(byte a1, byte a2, byte a3, byte a4)
        {
            byte[] array = new byte[10];
            array[0] = 0xEA;
            array[1] = 0xFD;
            array[2] = 0x20;
            array[3] = 0x01;
            array[4] = a1;
            array[5] = (byte)(a2 | 0x80);
            array[6] = (byte)(a3 | 0x80);
            array[7] = a4;
            if (Motherboard_flag == 4)
                array[8] = 0x10;
            else
                array[8] = 0x00;
            array[9] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4] + array[5] + array[6] + array[7] + array[8]));

            //startProgress(new SendDataMap(serialPort1, array, 0, 10, 2));//serialport1发送数据15次
            try
            {
                if (Rs232Con)
                {           
                    //serialPort1.Write(array, 0, 10);
                    //Thread.Sleep(100);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, 10);
                    }
                    else
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 10, 200, 3);
                }
            }
            catch
            {
                //Rs232Con = false;
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
            this.Invoke(new MethodInvoker(delegate()
            {
                richTextBox2.AppendText(ToHexString(array, 10));
            }));
        }

        //合并和分解的线程
        public void testSend_mergeThread(byte a1, byte a2, byte a3, byte a4)
        {
            Send_merge(a1, a2, a3, a4);
            stopProgress();//停止进度条
        }

        /// <summary>
        /// 从合并屏中增加某一组
        /// </summary>
        /// <param name="removeScreen"></param>
        private void addOnmerge(MergeGroup addMergeGroup)
        {
            int index = 0;
            if (mergeGroups.Count == 0)
            {
                mergeGroups.Add(addMergeGroup);
            }
            else
            {
                foreach (var item in mergeGroups)
                {
                    if (item.StartScreen.Number > addMergeGroup.StartScreen.Number)
                    {
                        mergeGroups.Insert(index, addMergeGroup);
                        break;
                    }
                    index++;
                }
            }
            if (index == mergeGroups.Count)
            {
                mergeGroups.Add(addMergeGroup);
            }

            //修改后面的组号
            for (int i = index; i < mergeGroups.Count; i++)
            {
                mergeGroups[i].GroupNumber = i + 1;
            }

        }

        /// <summary>
        /// 从合并屏中删除某一组
        /// </summary>
        /// <param name="removeScreen"></param>
        private void removeOnmerge(int mergeStartNum)
        {
            int index = 0;
            foreach (var item in mergeGroups)
            {
                if (item.StartScreen.Number == mergeStartNum)
                {
                    mergeGroups.Remove(item);
                    break;
                }
                index++;
            }
            //修改后面的组号
            for (int i = index; i < mergeGroups.Count; i++)
            {
                mergeGroups[i].GroupNumber = i + 1;
            }

        }

        /// <summary>
        /// 增加没合成的
        /// </summary>
        private void addOnUnMerge(Screen addScreen)
        {
            int index = 0;
            if (unMergeScreenList.Count == 0)
            {
                unMergeScreenList.Add(addScreen);
            }
            else
            {
                foreach (var item in unMergeScreenList)
                {
                    if (item.Number > addScreen.Number)
                    {
                        unMergeScreenList.Insert(index, addScreen);
                        break;
                    }
                    index++;
                }
            }
            if (index == unMergeScreenList.Count)
            {
                unMergeScreenList.Add(addScreen);
            }
        }

        /// <summary>
        /// 从非合并屏中删除某一块
        /// </summary>
        /// <param name="removeScreen"></param>
        private void removeOnUnmerge(Screen removeScreen)
        {

            foreach (var item in unMergeScreenList)
            {
                if (item.Number == removeScreen.Number)
                {
                    unMergeScreenList.Remove(item);
                    break;
                }
            }
        }

        //输出合成和没合成的
        private void displayMergeAndUnmerge()
        {
            foreach (var item in unMergeScreenList)
            {
                Console.WriteLine("unMergeScreenList===" + item + "===" + "第" + item.Number + "块屏没合并");
            }
            Console.WriteLine("mergeGroups===" + mergeGroups.Count);
            foreach (var item in mergeGroups)
            {
                Console.WriteLine("该场景的合并有:组号" + item.GroupNumber + "开始:" + item.StartScreen.Number + "input:" + item.StartScreen.IntputType + "结束:" + item.EndScreen.Number + "input:" + item.StartScreen.IntputType);
            }
        }

        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mergeToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    if (tabControl1.SelectedIndex == Motherboard_flag)
                        sheet.MergeRange(sheet.SelectionRange);
                    //sheet.SelectionStyle = WorksheetSelectionStyle.FocusRect;
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                if (sheet.SelectionRange.StartPos.Equals(sheet.SelectionRange.EndPos)) 
                {
                    //MessageBox.Show("yigeyigegeyige");
                    //return;
                }
                startProgress(0);//开启进度条显示
                //Console.WriteLine("Col" + sheet.SelectionRange.Col + "-" + sheet.SelectionRange.EndCol);
                //Console.WriteLine(sheet.SelectionRange);
                //Console.WriteLine("Row" + sheet.SelectionRange.Row + "-" + sheet.SelectionRange.EndRow);
                //Console.WriteLine("MouseDown");
                int mergeStartCol = sheet_back.SelectionRange.Col;
                int mergeStartRow = sheet_back.SelectionRange.Row;
                int mergeEndCol = sheet_back.SelectionRange.EndCol;
                int mergeEndRow = sheet_back.SelectionRange.EndRow;
                try
                {
                    //panel3.Enabled = false;
                    //if (Rs232Con)
                    {
                        if (tabControl1.SelectedIndex == Motherboard_flag)
                        {
                            sheet.MergeRange(sheet.SelectionRange);
                            mergeStartCol = sheet.SelectionRange.Col;
                            mergeStartRow = sheet.SelectionRange.Row;
                            mergeEndCol = sheet.SelectionRange.EndCol;
                            mergeEndRow = sheet.SelectionRange.EndRow;
                            changed = true;
                        }
                        else
                        {
                            //sheet.MergeRange(sheet_back.SelectionRange);
                            mergeStartCol = sheet_back.SelectionRange.Col;
                            mergeStartRow = sheet_back.SelectionRange.Row;
                            mergeEndCol = sheet_back.SelectionRange.EndCol;
                            mergeEndRow = sheet_back.SelectionRange.EndRow;
                        }
                        //Console.WriteLine("有合成的：" + mergeStartRow + "," + mergeStartCol + "-" + mergeEndRow + "," + mergeEndCol);
                        /*sheet.SetRangeStyles(new RangePosition(mergeStartCol, mergeStartRow, mergeEndCol + 1, mergeEndRow + 1), new WorksheetRangeStyle//0,0开始，rowCount行colCount列，左上角
                        {
                            BackColor = Color.Green,
                        });*/
                        if (tabControl1.SelectedIndex == Motherboard_flag)
                        {
                            MergeGroup mergeGroup = new MergeGroup();
                            List<Screen> mergeScreenList = new List<Screen>();
                            for (int g = mergeStartRow; g <= mergeEndRow; g++)
                            {
                                for (int h = mergeStartCol; h <= mergeEndCol; h++)
                                {
                                    if (g == mergeStartRow && h == mergeStartCol)
                                    {//左上角的
                                        mergeGroup.StartScreen = screens[g * sheet.ColumnCount + h];
                                        mergeScreenList.Add(mergeGroup.StartScreen);//保存合并的屏的左上角
                                        removeOnUnmerge(screens[g * sheet.ColumnCount + h]);//从非合并中移除
                                        continue;
                                    }
                                    mergeScreenList.Add(screens[g * sheet.ColumnCount + h]);//保存合并的屏
                                    removeOnUnmerge(screens[g * sheet.ColumnCount + h]);//从非合并中移除
                                    if (g == mergeEndRow && h == mergeEndCol)
                                    { //右下角
                                        mergeGroup.EndScreen = screens[g * sheet.ColumnCount + h];
                                        mergeGroup.GroupNumber = mergeGroups.Count + 1;
                                        mergeGroup.MergeScreenList = mergeScreenList;
                                        //mergeGroups.Add(mergeGroup);
                                        addOnmerge(mergeGroup);
                                    }
                                }
                            }
                        }
                        //displayMergeAndUnmerge();

                        //group++;
                        int A_0 = rowStar;
                        int A_1 = colStar + 1;
                        byte A = (byte)(A_1 + A_0 * colsCount);
                        int B_0 = rowEnd;
                        int B_1 = colEnd + 1;
                        byte B = (byte)(B_1 + B_0 * colsCount);
                        int num = (rowEnd - rowStar + 1) * (colEnd - colStar + 1);
                        string str = screens[A - 1].IntputType;
                        //Console.WriteLine(num + "num--------");
                        if (str.Contains("("))
                            str = str.Split('(')[0];
                        byte souce = 0x00;
                        if (Motherboard_flag == 4)//3458
                        {
                            if (str.Contains("HDMI"))
                            {
                                //if (checkBox2.Checked)
                                //UartSendSwitchMainSignalCmd(str, 0, num, "HDMI");
                                if (str.Contains("HDMI1"))
                                {
                                    souce = 0x00;
                                }
                                else if (str.Contains("HDMI2"))
                                {
                                    souce = 0x01;
                                }
                            }
                            else if (str.Contains("OPS"))
                            {
                                //UartSendSwitchMainSignalCmd(str, 0, num, "DVI");
                                souce = 0x02;
                            }
                            else if (str.Contains("DVI"))
                            {
                                //if (checkBox2.Checked)
                                //UartSendSwitchMainSignalCmd(str, 0, num, "DVI");
                                souce = 0x03;
                            }
                            else if (str.Contains("DP"))
                            {
                                //UartSendSwitchMainSignalCmd(str, 0, num, "VIDEO");
                                souce = 0x04;
                            }
                            else if (str.Contains("VGA"))
                            {
                                //UartSendSwitchMainSignalCmd(str, 0, num, "VIDEO");
                                souce = 0x06;
                            }
                        }
                        else//59
                        {
                            if (str.Contains("VGA"))
                            {
                                souce = 0x00;
                                if (checkBox2.Checked)
                                    UartSendSwitchMainSignalCmd(str, 0, num, "VGA", souce);
                            }
                            else if (str.Contains("HDMI"))
                            {
                                souce = 0x01;
                                if (checkBox2.Checked)
                                    UartSendSwitchMainSignalCmd(str, 0, num, "HDMI", souce);
                            }
                            else if (str.Contains("DVI"))
                            {
                                souce = 0x02;
                                if (checkBox2.Checked)
                                    UartSendSwitchMainSignalCmd(str, 0, num, "DVI", souce);
                            }
                            else if (str.Contains("VIDEO"))
                            {
                                if (str.Contains("VIDEO1"))
                                    souce = 0x03;
                                else if (str.Contains("VIDEO2"))
                                    souce = 0x04;
                                else if (str.Contains("VIDEO3"))
                                    souce = 0x05;
                                else if (str.Contains("VIDEO4"))
                                    souce = 0x06;
                                if (checkBox2.Checked)
                                    UartSendSwitchMainSignalCmd(str, 0, num, "VIDEO", souce);
                            }
                            else if (str.Contains("S-VIDEO"))
                            {
                                souce = 0x07;
                            }
                            else if (str.Contains("YPbPr"))
                            {
                                souce = 0x08;
                                if (checkBox2.Checked)
                                    UartSendSwitchMainSignalCmd(str, 0, num, "YPbPr", souce);
                            }
                            else
                                souce = 0x03;
                        }
                        byte C = 0;
                        if (tabControl1.SelectedIndex == Motherboard_flag)
                            C = (byte)(0x08 | ((mergeGroups.Count << 4) & 0xF0) | (0x08) | souce);
                        else
                            C = (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | souce);
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            testSend_mergeThread((byte)1, A, B, C);
                        })); //开线程         
                        myThread.Start(); //启动线程 
                    }
                    string  x = "";
                    for (int i = rowStar; i <= rowEnd; i++)
                    {
                        for (int j = colStar; j <= colEnd; j++)
                        {
                            x += ((j + 1) + i * colsCount).ToString() + " , ";//(j * colsCount + i).ToString();
                        }
                    }
                    //Console.WriteLine(x);
                    LogHelper.WriteLog("=====拼接合并画面操作,屏幕【" + x +"】合并显示=====");
                }
                catch (unvell.ReoGrid.RangeIntersectionException exception)
                {
                    //MessageBox.Show("不允许这样合成!" + exception.Message, "温馨提示");
                }
            }
        }
        
        

        /// <summary>
        /// 分解
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unMergeToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {    
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    if (tabControl1.SelectedIndex == Motherboard_flag)
                        sheet.UnmergeRange(sheet.SelectionRange);
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        for (int j = rowStar; j <= rowEnd; j++)
                        {
                            sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + screens[j * sheet.ColumnCount + i].IntputType);
                        }
                    }
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                int num = 0;
                //startProgress(0);//开启进度条显示
                //Console.WriteLine("分解======");
                //Console.WriteLine("Col" + sheet.SelectionRange.Col + "-" + sheet.SelectionRange.EndCol);
                //Console.WriteLine(sheet.SelectionRange);
                //Console.WriteLine("Row" + sheet.SelectionRange.Row + "-" + sheet.SelectionRange.EndRow);
                if (tabControl1.SelectedIndex == Motherboard_flag)
                {
                    if (sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                    {
                        startProgress(0);//开启进度条显示
                        sheet.UnmergeRange(sheet.SelectionRange);
                        //panel3.Enabled = true;
                        //if (Rs232Con)
                        {
                            removeOnmerge(screens[sheet.SelectionRange.Row * sheet.ColumnCount + sheet.SelectionRange.Col].Number);   //从合并中移除

                            for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
                            {
                                for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
                                {
                                    num++;
                                    addOnUnMerge(screens[j * sheet.ColumnCount + i]);//增加没合成的
                                    if (i == sheet.SelectionRange.Col && j == sheet.SelectionRange.Row)
                                    {
                                        continue;
                                    }
                                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + screens[j * sheet.ColumnCount + i].IntputType);
                                }
                            }
                            changed = true;
                            //displayMergeAndUnmerge();
                            //group--;
                            int A_0 = rowStar;
                            int A_1 = colStar + 1;
                            byte A = (byte)(A_1 + A_0 * colsCount);
                            int B_0 = rowEnd;
                            int B_1 = colEnd + 1;
                            byte B = (byte)(B_1 + B_0 * colsCount);
                            string str = screens[A - 1].IntputType;
                            if (str.Contains("("))
                                str = str.Split('(')[0];
                            //Console.WriteLine(str);
                            byte souce = 0x00;
                            if (Motherboard_flag == 4)
                            {
                                if (str.Contains("HDMI"))
                                {
                                    //if (checkBox2.Checked)
                                    //UartSendSwitchMainSignalCmd(str, 0, num, "HDMI");
                                    if (str.Contains("HDMI1"))
                                    {
                                        souce = 0x00;
                                    }
                                    else if (str.Contains("HDMI2"))
                                    {
                                        souce = 0x01;
                                    }
                                }
                                else if (str.Contains("OPS"))
                                {
                                    //UartSendSwitchMainSignalCmd(str, 0, num, "DVI");
                                    souce = 0x02;
                                }
                                else if (str.Contains("DVI"))
                                {
                                    //if (checkBox2.Checked)
                                    //UartSendSwitchMainSignalCmd(str, 0, num, "DVI");
                                    souce = 0x03;
                                }
                                else if (str.Contains("DP"))
                                {
                                    //UartSendSwitchMainSignalCmd(str, 0, num, "VIDEO");
                                    souce = 0x04;
                                }
                                else if (str.Contains("VGA"))
                                {
                                    //UartSendSwitchMainSignalCmd(str, 0, num, "VIDEO");
                                    souce = 0x06;
                                }
                            }
                            else
                            {
                                if (str.Contains("VGA"))
                                {
                                    souce = 0x00;
                                    if (checkBox2.Checked)
                                        UartSendSwitchMainSignalCmd(str, 0, num, "VGA", souce);
                                }
                                else if (str.Contains("HDMI"))
                                {
                                    souce = 0x01;
                                    if (checkBox2.Checked)
                                        UartSendSwitchMainSignalCmd(str, 0, num, "HDMI", souce);
                                }
                                else if (str.Contains("DVI"))
                                {
                                    souce = 0x02;
                                    if (checkBox2.Checked)
                                        UartSendSwitchMainSignalCmd(str, 0, num, "DVI", souce);
                                }
                                else if (str.Contains("VIDEO"))
                                {
                                    if (str.Contains("VIDEO1"))
                                        souce = 0x03;
                                    else if (str.Contains("VIDEO2"))
                                        souce = 0x04;
                                    else if (str.Contains("VIDEO3"))
                                        souce = 0x05;
                                    else if (str.Contains("VIDEO4"))
                                        souce = 0x06;
                                    if (checkBox2.Checked)
                                        UartSendSwitchMainSignalCmd(str, 0, num, "VIDEO", souce);
                                }
                                else if (str.Contains("S-VIDEO"))
                                    souce = 0x07;
                                else if (str.Contains("YPbPr"))
                                {
                                    souce = 0x08;
                                    if (checkBox2.Checked)
                                        UartSendSwitchMainSignalCmd(str, 0, num, "YPbPr",souce);
                                }
                                else
                                    souce = 0x03;
                            }
                            //Send_merge((byte)1, A, B, souce);
                            myThread = new Thread(new ThreadStart(delegate()
                            {
                                testSend_mergeThread((byte)1, A, B, souce);
                            })); //开线程         
                            myThread.Start(); //启动线程 
                            // startProgress(3000);//开启进度条
                            //Send_merge((byte)1, A, B, souce);
                            // stopProgress();
                        }
                    }
                }
                else
                {
                    startProgress(0);
                    int A_0 = rowStar;
                    int A_1 = colStar + 1;
                    byte A = (byte)(A_1 + A_0 * colsCount);
                    int B_0 = rowEnd;
                    int B_1 = colEnd + 1;
                    byte B = (byte)(B_1 + B_0 * colsCount);
                    string str = screens[A - 1].IntputType;
                    if (str.Contains("("))
                        str = str.Split('(')[0];
                    //Console.WriteLine(str);
                    byte souce = 0x00;
                    if (Motherboard_flag == 4)
                    {
                        if (str.Contains("HDMI"))
                        {
                            if (str.Contains("HDMI1"))
                            {
                                souce = 0x00;
                            }
                            else if (str.Contains("HDMI2"))
                            {
                                souce = 0x01;
                            }
                        }
                        else if (str.Contains("OPS"))
                        {
                            //UartSendSwitchMainSignalCmd(str, 0, num, "DVI");
                            souce = 0x02;
                        }
                        else if (str.Contains("DVI"))
                        {
                            //if (checkBox2.Checked)
                            //UartSendSwitchMainSignalCmd(str, 0, num, "DVI");
                            souce = 0x03;
                        }
                        else if (str.Contains("DP"))
                        {
                            //UartSendSwitchMainSignalCmd(str, 0, num, "VIDEO");
                            souce = 0x04;
                        }
                        else if (str.Contains("VGA"))
                        {
                            //UartSendSwitchMainSignalCmd(str, 0, num, "VGA");
                            souce = 0x06;
                        }
                    }
                    else
                    {
                        if (str.Contains("VGA"))
                        {
                            souce = 0x00;
                            if (checkBox2.Checked)
                                UartSendSwitchMainSignalCmd(str, 0, num, "VGA", souce);
                        }
                        else if (str.Contains("HDMI"))
                        {
                            souce = 0x01;
                            if (checkBox2.Checked)
                                UartSendSwitchMainSignalCmd(str, 0, num, "HDMI", souce);
                        }
                        else if (str.Contains("DVI"))
                        {
                            souce = 0x02;
                            if (checkBox2.Checked)
                                UartSendSwitchMainSignalCmd(str, 0, num, "DVI", souce);
                        }
                        else if (str.Contains("VIDEO"))
                        {
                            if (str.Contains("VIDEO1"))
                                souce = 0x03;
                            else if (str.Contains("VIDEO2"))
                                souce = 0x04;
                            else if (str.Contains("VIDEO3"))
                                souce = 0x05;
                            else if (str.Contains("VIDEO4"))
                                souce = 0x06;
                            if (checkBox2.Checked)
                                UartSendSwitchMainSignalCmd(str, 0, num, "VIDEO", souce);
                        }
                        else if (str.Contains("S-VIDEO"))
                            souce = 0x07;
                        else if (str.Contains("YPbPr"))
                        {
                            souce = 0x08;
                            if (checkBox2.Checked)
                                UartSendSwitchMainSignalCmd(str, 0, num, "YPbPr",souce);
                        }
                        else
                            souce = 0x03;
                    }
                    myThread = new Thread(new ThreadStart(delegate()
                    {
                        testSend_mergeThread((byte)1, A, B, souce);
                    })); //开线程         
                    myThread.Start(); //启动线程 
                } 
                string x = "";
                for (int i = rowStar; i <= rowEnd; i++)
                {
                    for (int j = colStar; j <= colEnd; j++)
                    {
                        x += ((j + 1) + i * colsCount).ToString() + " , ";//(j * colsCount + i).ToString();
                    }
                }
                LogHelper.WriteLog("=====拼接分解画面操作,屏幕【" + x + "】分开显示=====");
            }
        }



        /// <summary>
        /// 右击打开的菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rightMouseContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (systemRunning || (myThread != null && myThread.IsAlive))
            {
                promptSystemRun();
                e.Cancel = true;
                return;
            }
             // 不允许合成过的再合成
            if (tabControl1.SelectedIndex == Motherboard_flag)
            {
                if (sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                {
                    (sender as ContextMenuStrip).Items[0].Enabled = false;
                    (sender as ContextMenuStrip).Items[1].Enabled = true;
                }
                else
                {
                    (sender as ContextMenuStrip).Items[0].Enabled = true;
                    (sender as ContextMenuStrip).Items[1].Enabled = false;

                }
            }
            else
            {
                (sender as ContextMenuStrip).Items[0].Enabled = true;
                (sender as ContextMenuStrip).Items[1].Enabled = true;
            }
            if (colStar == colEnd && rowStar == rowEnd)
            {
                //Console.WriteLine("sheet.SelectionRange.Col=" + sheet.SelectionRange.Col + "sheet.SelectionRange.EndRow" + sheet.SelectionRange.EndRow);
                (sender as ContextMenuStrip).Items[0].Enabled = false;
                //screenNumberToolStripMenuItem.Enabled = true;
                屏幕参数调整ToolStripMenuItem.Enabled = true;
            }
            else 
            {
                //screenNumberToolStripMenuItem.Enabled = false; 
                if(Motherboard_flag == 2)
                    屏幕参数调整ToolStripMenuItem.Enabled = false;
                else
                    屏幕参数调整ToolStripMenuItem.Enabled = true;
            }
        }


        /// <summary>
        /// 判断选中的里面有没有合成的
        /// </summary>
        /// <returns></returns>
        public bool isSelectionMergeRange(int leftTopCol,int leftTopRow,int rightBottomCol,int rightBottomRow)
        {
           // Console.WriteLine("check");
            for (int i = leftTopCol; i <= rightBottomCol; i++)
            {
                for (int j = leftTopRow; j <= rightBottomRow; j++)
                {
                    if (sheet.IsMergedCell(sheet.CheckMergedRange(new RangePosition(j, i, 1, 1)).StartPos)) 
                    {
                        if (checkRangeContain(sheet.CheckMergedRange(new RangePosition(j, i, 1, 1)), new RangePosition(leftTopRow, leftTopCol, rightBottomRow - leftTopRow + 1, rightBottomCol - leftTopCol + 1)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 对合成的单元格进行信息的初始化
        /// </summary>
        public void initAllMergeMsg()
        {
            mergeGroups = new List<MergeGroup>();//初始化合成组，保存所有的合成范围
            unMergeScreenList = new List<Screen>();//初始化非合成屏，保存所有的非合成的屏
            //取得所有有合成的单元格
            bool[] screenMergeStatus = new bool[sheet.RowCount * sheet.ColumnCount];//场景切换各个屏的合并情况
            for (int i = 0; i < sheet.RowCount; i++)
            {
                for (int j = 0; j < sheet.ColumnCount; j++)
                {
                    //if (sheet.GetMergedCellOfRange(i, j).IsMergedCell)
                    //Console.WriteLine("if   getcell == " + i + j + "," + sheet.GetMergedCellOfRange(i, j).Data + sheet.GetCell(i, j).IsMergedCell);
                    if (sheet.GetCell(i, j).IsMergedCell)
                    { //如果是拿范围
                        RangePosition mergeRange = sheet.CheckMergedRange(new RangePosition(i, j, 1, 1));
                        int mergeStartCol = mergeRange.Col;
                        int mergeStartRow = mergeRange.Row;
                        int mergeEndCol = mergeRange.EndCol;
                        int mergeEndRow = mergeRange.EndRow;
                        //Console.WriteLine("有合成的：" + mergeStartRow + "," + mergeStartCol + "-" + mergeEndRow + "," + mergeEndCol);
                        MergeGroup mergeGroup = new MergeGroup();
                        List<Screen> mergeScreenList = new List<Screen>();
                        for (int g = mergeStartRow; g <= mergeEndRow; g++)//初始值,将合成单元格里的input都改成左上角的input
                        {
                            for (int h = mergeStartCol; h <= mergeEndCol; h++)
                            {
                                screenMergeStatus[g * sheet.ColumnCount + h] = true;

                                if (g == mergeStartRow && h == mergeStartCol)
                                {//左上角的不用改
                                    mergeGroup.StartScreen = screens[g * sheet.ColumnCount + h];
                                    mergeScreenList.Add(mergeGroup.StartScreen);//保存合并的屏的左上角
                                    continue;
                                }
                                //Console.WriteLine("改" + g + "," + h + "为" + (mergeStartRow * sheet.ColumnCount + mergeStartCol) + "," + screens[mergeStartRow * sheet.ColumnCount + mergeStartCol].IntputType);
                                //Console.WriteLine(h * sheet.ColumnCount + g + "===块");
                                screens[g * sheet.ColumnCount + h] = new Screen();
                                screens[g * sheet.ColumnCount + h].IntputType = screens[mergeStartRow * sheet.ColumnCount + mergeStartCol].IntputType;
                                screens[g * sheet.ColumnCount + h].Name = "U" + (g * sheet.ColumnCount + h + 1);
                                screens[g * sheet.ColumnCount + h].Number = (g * sheet.ColumnCount + h + 1);
                                sheet.SetCellData(g, h, screens[g * sheet.ColumnCount + h]);//给单元格设置值
                                //Console.WriteLine(screens[g * sheet.ColumnCount + h].IntputType);

                                mergeScreenList.Add(screens[g * sheet.ColumnCount + h]);//保存合并的屏

                                if (g == mergeEndRow && h == mergeEndCol)
                                { //右下角
                                    //保存该合并块的右下角
                                    mergeGroup.EndScreen = screens[g * sheet.ColumnCount + h];
                                    mergeGroup.GroupNumber = mergeGroups.Count + 1;
                                    mergeGroup.MergeScreenList = mergeScreenList;
                                    mergeGroups.Add(mergeGroup);
                                }
                            }
                        }
                    }
                    sheet_back.SetCellData(i, j, screens[i * sheet.ColumnCount + j]);//给单元格设置值
                }
            }
            /*
            for (int i = 0; i < sheet_back.RowCount; i++)
            {
                for (int j = 0; j < sheet_back.ColumnCount; j++)
                {
                    if (sheet_back.GetCell(i, j).IsMergedCell)
                    {
                        RangePosition mergeRange = sheet_back.CheckMergedRange(new RangePosition(i, j, 1, 1));
                        sheet_back.UnmergeRange(mergeRange);       
                        int mergeStartCol = mergeRange.Col;
                        int mergeStartRow = mergeRange.Row;
                        int mergeEndCol = mergeRange.EndCol;
                        int mergeEndRow = mergeRange.EndRow;
                        //Console.WriteLine("有合成的：" + mergeStartRow + "," + mergeStartCol + "-" + mergeEndRow + "," + mergeEndCol);
                        for (int g = mergeStartRow; g <= mergeEndRow; g++)//初始值,将合成单元格里的input都改成左上角的input
                        {
                            for (int h = mergeStartCol; h <= mergeEndCol; h++)
                            {
                                screenMergeStatus[g * sheet_back.ColumnCount + h] = true;
                                //if (g == mergeStartRow && h == mergeStartCol)
                                {//左上角的不用改
                                    //mergeGroup.StartScreen = screens[g * sheet.ColumnCount + h];
                                    //mergeScreenList.Add(mergeGroup.StartScreen);//保存合并的屏的左上角
                                   // continue;
                                }
                                //Console.WriteLine("改" + g + "," + h + "为" + (mergeStartRow * sheet.ColumnCount + mergeStartCol) + "," + screens[mergeStartRow * sheet.ColumnCount + mergeStartCol].IntputType);
                                //Console.WriteLine(h * sheet.ColumnCount + g + "===块");
                                //screens[g * sheet_back.ColumnCount + h] = new Screen();
                                //screens[g * sheet_back.ColumnCount + h].IntputType = screens[g * sheet.ColumnCount + h + 1].IntputType;
                                //screens[g * sheet_back.ColumnCount + h].Name = "U" + (g * sheet_back.ColumnCount + h + 1);
                                //screens[g * sheet_back.ColumnCount + h].Number = (g * sheet_back.ColumnCount + h + 1);
                                //sheet.SetCellData(g, h, screens[g * sheet_back.ColumnCount + h]);//给单元格设置值
                                sheet_back.SetCellData(g, h, screens[g * sheet.ColumnCount + h]);//给单元格设置值
                                //mergeScreenList.Add(screens[g * sheet.ColumnCount + h]);//保存合并的屏
                            }
                        }
                        
                    }
                }
            }
             */ 
            //找出所有没有合并过的
            for (int i = 0; i < screenMergeStatus.Length; i++)
            {
                if (!screenMergeStatus[i])
                { //不是合并的
                    unMergeScreenList.Add(screens[i]);
                }
            }
            /*
            foreach (var item in unMergeScreenList)
            {
                Console.WriteLine("第" + item.Number + "块屏没合并");
            }
            
            //按照这个取值方式 把下面这一块放到按钮里面去就可以取到mergeGroups的值
            for (int i = 0; i < mergeGroups.Count; i++)
            {
                Console.WriteLine("该场景的合并有:组号" + mergeGroups[i].GroupNumber + "开始:" + mergeGroups[i].StartScreen.Number + "input:" + mergeGroups[i].StartScreen.IntputType + "结束:" + mergeGroups[i].EndScreen.Number + "input:" + mergeGroups[i].StartScreen.IntputType);
            }
            */
        }
        /// <summary>
        /// 读场景文件显示
        /// </summary>
        /// <param name="path"></param>
        public void readSceneFile(string path)
        {
            //Console.WriteLine("path=" + path);
            sheet.LoadRGF(path);
            //sheet_back.Load(path);

            rowsCount = sheet.RowCount;
            colsCount = sheet.ColumnCount;
            sheet.SetRowsHeight(0, sheet.RowCount, (ushort)((reoGridControl1.Size.Height) / rowsCount));//改行的高度，从0行开始，改ColumnCount行
            sheet.SetColumnsWidth(0, sheet.ColumnCount, (ushort)((reoGridControl1.Size.Width) / colsCount));//改列的宽度，从0列开始，改ColumnCount列
            sheet_back.SetRowsHeight(0, sheet_back.RowCount, (ushort)((reoGridControl2.Size.Height) / rowsCount));//改行的高度，从0行开始，改ColumnCount行
            sheet_back.SetColumnsWidth(0, sheet_back.ColumnCount, (ushort)((reoGridControl2.Size.Width) / colsCount));//改列的宽度，从0列开始，改ColumnCount列
           // reoGridControl1.Load(path, unvell.ReoGrid.IO.FileFormat.Excel2007);
            //初始化screens
            screens = new Screen[rowsCount * colsCount];
            //Console.WriteLine(screens.Length + "===length");
            for (int i = 0; i < colsCount; i++)//初始值
            {
                for (int j = 0; j < rowsCount; j++)
                {
                    //获得文本值,分割，初始化screens
                    String screenText = (String)sheet.GetCellData(j, i);
                    //Console.WriteLine(screenText + "--" + i + "," + j);
                    if (screenText != null)//有则按值
                    {
                        String[] screentSplit = screenText.Split(' ');
                        screens[j * sheet.ColumnCount + i] = new Screen();
                        screens[j * sheet.ColumnCount + i].Name = screentSplit[0];
                        screens[j * sheet.ColumnCount + i].Number = j * sheet.ColumnCount + i + 1;
                        screens[j * sheet.ColumnCount + i].IntputType = screentSplit[1];
                        //Console.WriteLine(screentSplit[1]);
                    }
                    else
                    { //无则默认
                        screens[j * sheet.ColumnCount + i] = new Screen();
                        screens[j * sheet.ColumnCount + i].Name = "U" + (j * sheet.ColumnCount + i + 1);
                        screens[j * sheet.ColumnCount + i].Number = (j * sheet.ColumnCount + i + 1);
                        screens[j * sheet.ColumnCount + i].IntputType = defaultSignalName;

                    }
                    //Console.WriteLine(screens[j * sheet.ColumnCount + i].Number + "===块" + "name=" + screens[j * sheet.ColumnCount + i].Name + "type=" + screens[j * sheet.ColumnCount + i].IntputType);
                }
            }
            initAllMergeMsg();//找出合成的并修改screens单元格信息(特殊)
            // reoGridControl1.Load(path, unvell.ReoGrid.IO.FileFormat.Excel2007);
        }

        /// <summary>
        /// 保存至rgf场景文件
        /// </summary>
        /// <param name="path"></param>
        public void saveSceneFile(string path) 
        {
            FileInfo file = new FileInfo(path);
            if ((!file.Exists))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false, Encoding.Default);
                sw.Close();
            }
            //if(Rs232Con)
            sheet.SaveRGF(path);
           // reoGridControl1.Save(path, unvell.ReoGrid.IO.FileFormat.Excel2007);
        }

        /// <summary>
        /// 场景改变时，提示是否保存场景的改变
        /// </summary>
        /// 
        /// <param name="path"></param>
        public void saveChange(String path)
        {
            if(Rs232Con)
            {
                if (changed)
                { //有改变的，提示是否保存改场景
                    string ts = languageFile.ReadString("MESSAGEBOX", "MD", "是否保存对当前场景的修改!");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    DialogResult t = MessageBox.Show(ts, tp, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    
                    if (t == DialogResult.OK || t ==DialogResult.Yes)
                    {
                        saveSceneFile(path);
                    }
                }
                changed = false;
            }
        }

        /// <summary>
        /// 将场景按钮一的值设置给当前场景按钮值
        /// </summary>
        public void setSce1CurrentSceneButtonText() 
        {
            currentSceneButtonText = bt_sce1.Text;
        }
        /// <summary>
        /// 拼接设置初始化线程
        /// </summary>
        public void Send_toScreenThread()
        {
            Send_toScreen();
            stopProgress();
        }

        private void Init2_Matrixchannel()
        {
            if (Motherboard_type == 0)
            {
                int n = comboBox1.SelectedIndex;
                if (Chinese_English == 1)
                {
                    comboBox1.Items.Remove("矩阵HDMI");
                    comboBox1.Items.Remove("矩阵DVI");
                    comboBox1.Items.Add("MatrixHDMI");
                    comboBox1.Items.Add("MatrixDVI");
                }
                comboBox1.SelectedIndex = n;
            }
            else if (Motherboard_type == 1)
            {
                int n = comboBox3.SelectedIndex;
                if (Chinese_English == 1)
                {
                    comboBox3.Items.Remove("矩阵HDMI");
                    comboBox3.Items.Add("MatrixHDMI");
                }
                comboBox3.SelectedIndex = n;
            }
            else if (Motherboard_type == 2)
            {
                int n = comboBox4.SelectedIndex;
                if (Chinese_English == 1)
                {
                    comboBox4.Items.Remove("矩阵HDMI");
                    comboBox4.Items.Remove("矩阵DVI");
                    comboBox4.Items.Add("MatrixHDMI");
                    comboBox4.Items.Add("MatrixDVI");
                }
                comboBox4.SelectedIndex = n;
            }
        }

        public bool setWall_flag = false;
        /// <summary>
        /// 设置拼接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_setWallForm_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            setWall_flag = false;
            //saveChange(currentConnectionName + "\\" + currentSceneName);
            WallSetForm f = new WallSetForm(this);
            f.ShowDialog();
            PN = settingFile.ReadBool("SETTING", "NameFlag", false);
            Init_Channel();
            if (Motherboard_type == 0)
                comboBox3_SelectedIndexChanged(null, null);
            else if (Motherboard_type == 1)
                comboBox4_SelectedIndexChanged(null, null);
            else if (Motherboard_type == 3)
                comboBox5_SelectedIndexChanged(null, null);
            else if (Motherboard_type == 2)
                comboBox6_SelectedIndexChanged(null, null);
            else if (Motherboard_type == 4)
                comboBox1_SelectedIndexChanged(null, null);
            if (Rs232Con && setWall_flag)
            {
                settingFile.WriteBool("SCREEN", "S_flag", false);
                startProgress(0);
                myThread = new Thread(new ThreadStart(Send_toScreenThread)); //开线程         
                myThread.Start(); //启动线程 
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < rowsCount * colsCount; i++)
                    {
                        PanelCount[j, i] = (byte)(i + 1);
                    }
                }
            }
            screen_H = new int[rowsCount * colsCount];//拼缝调整记录
            screen_V = new int[rowsCount * colsCount];
            numericUpDown2.Value = 1;
            numericUpDown3.Value = 1;

            rowStar = sheet_back.SelectionRange.Row;
            rowEnd = sheet_back.SelectionRange.EndRow;
            colStar = sheet_back.SelectionRange.Col;
            colEnd = sheet_back.SelectionRange.EndCol;
        }


        /// <summary>
        /// 保存连接场景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_saveWallForm_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            if (Rs232Con)
            {
                saveSceneFile(currentConnectionName + "\\" + currentSceneName);
                saveSignalEdit();
                changed = false;

                string ts = languageFile.ReadString("MESSAGEBOX", "MC", "场景保存成功 ! ");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);

                LogHelper.WriteLog("=====保存场景成功=====");
            }
            else
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "MG", "场景保存失败 ! ");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                LogHelper.WriteLog("=====保存场景失败=====");
            }
        }

        /// <summary>
        /// 打开文件夹，即以该文件夹作为连接场景的保存路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_openWallForm_Click(object sender, EventArgs e)
        {
            //readSceneFile("test.rgf");
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            saveChange(currentConnectionName + "\\" + currentSceneName);
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件所在文件夹路径";
            dialog.SelectedPath = Application.StartupPath ;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                currentConnectionName = dialog.SelectedPath;
                //Console.WriteLine(currentConnectionName + "=======");
               // firstInit = true;
                initRoGridFromFile(currentConnectionName);
                settingFile.WriteInteger("SETTING","Row",rowsCount);
                settingFile.WriteInteger("SETTING", "Col", colsCount);
            }
        }

        /// <summary>
        /// 选择的某个矩阵信源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                //Console.WriteLine("选了" + item.Name);
                //给选中的屏修改信号
                int num = 0;
                byte A = 0;
                byte B = 0;
                byte souce = 0x00;
                //if (tabControl1.SelectedIndex == 4)
                {
                    for (int j = rowStar; j <= rowEnd; j++)
                    {
                        for (int i = colStar; i <= colEnd; i++)
                        {
                            //PanelCount[num] = (byte)screens[j * colsCount + i].Number;
                            //Console.WriteLine(PanelCount[num]);
                            sheet.SetCellData(j, i, screens[j * colsCount + i].Name + " " + item.Text);//修改单元格的值
                            sheet_back.SetCellData(j, i, screens[j * colsCount + i].Name + " " + item.Text);//修改单元格的值
                            screens[j * colsCount + i].IntputType = item.Text;//改screens里对应屏幕的信源     
                            num++;
                        }
                    }
                    int A_0 = rowStar;
                    int A_1 = colStar + 1;
                    A = (byte)(A_1 + A_0 * colsCount);
                    int B_0 = rowEnd;
                    int B_1 = colEnd + 1;
                    B = (byte)(B_1 + B_0 * colsCount);
                }
                /*
                if (tabControl3.SelectedIndex == 1)
                {
                    for (int j = sheet_back.SelectionRange.Row; j <= sheet_back.SelectionRange.EndRow; j++)
                    {
                        for (int i = sheet_back.SelectionRange.Col; i <= sheet_back.SelectionRange.EndCol; i++)
                        {
                            PanelCount[num] = (byte)screens[j * sheet_back.ColumnCount + i].Number;
                            sheet_back.SetCellData(j, i, screens[j * sheet_back.ColumnCount + i].Name + " " + item.Text);//修改单元格的值
                            screens[j * sheet_back.ColumnCount + i].IntputType = item.Text;//改screens里对应屏幕的信源     
                            num++;
                        }
                    }
                    int A_0 = sheet_back.SelectionRange.Row;
                    int A_1 = sheet_back.SelectionRange.Col + 1;
                    A = (byte)(A_1 + A_0 * colsCount);
                    int B_0 = sheet_back.SelectionRange.EndRow;
                    int B_1 = sheet_back.SelectionRange.EndCol + 1;
                    B = (byte)(B_1 + B_0 * colsCount);
                }
                 */
                string s = "";
                if (item.Text.Contains("("))
                    s = item.Text.Split('(')[0];
                else
                    s = item.Text;
                //Console.WriteLine("sss = " + s);
                if (Motherboard_flag == 4)
                {
                    if (item.Text.Contains("VGA"))
                    {
                        souce = 0x06;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "VGA", souce);
                        }
                        //Send_merge((byte)1,A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x00)); 
                    }
                    if (item.Text.Contains("VIDEO"))
                    {
                        souce = 0x03;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "VIDEO", souce);
                        }
                        //Send_merge((byte)1,  A,B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08 ) | 0x03)); 
                    }
                    if (item.Text.Contains("HDMI"))
                    {
                        souce = 0x00;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "HDMI", souce);
                        }
                        //Send_merge((byte)0x01,A,B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08 ) | 0x01));
                    }
                    if (item.Text.Contains("DVI"))
                    {
                        souce = 0x03;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "DVI", souce);
                        }
                        //Send_merge((byte)0x01,A,B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08 ) | 0x02));
                    }
                    if (item.Text.Contains("DP"))
                    {
                        //UartSendSwitchMainSignalCmd(item.Text, 0, num, "YPbPr");
                        souce = 0x04;
                        //Send_merge((byte)0x01,A,B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08 ) | 0x06));
                    }
                }
                else
                {
                    if (item.Text.Contains("VGA"))
                    {
                        souce = 0x00;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "VGA", souce);
                        }
                        //Send_merge((byte)1,A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x00)); 
                    }
                    else if (item.Text.Contains("VIDEO"))
                    {
                        souce = 0x03;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "VIDEO", souce);
                        }
                        //Send_merge((byte)1,  A,B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08 ) | 0x03)); 
                    }
                    else if (item.Text.Contains("HDMI"))
                    {
                        souce = 0x01;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "HDMI", souce);
                        }
                        //Send_merge((byte)0x01,A,B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08 ) | 0x01));
                    }
                    else if (item.Text.Contains("DVI"))
                    {
                        souce = 0x02;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "DVI", souce);
                        }
                        //Send_merge((byte)0x01,A,B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08 ) | 0x02));
                    }
                    else if (item.Text.Contains("YPbPr"))
                    {
                        souce = 0x08;
                        if (checkBox2.Checked)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd(s, 0, num, "YPbPr", souce);
                        }
                        //Send_merge((byte)0x01,A,B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08 ) | 0x06));
                    }
                }
                //if (sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                    //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | souce));
                //else
                //Send_merge((byte)0x01, A, A, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | souce));
                changed = true;
            }
            //displayScreen();
            else
            {
                ToolStripItem item = sender as ToolStripItem;
                string s = item.Name;
                s = s.Substring(5);
                Edit_str = "";
                if (Motherboard_flag == 4)
                {
                    if (s.Contains("Hdmi"))
                        s = s.Replace("Hdmi", "HDMI");
                    else if (s.Contains("Dvi"))
                        s = s.Replace("Dvi", "DVI");
                    else if (s.Contains("Vga"))
                        s = s.Replace("Vga", "VGA");
                }
                else
                {
                    if (s.Contains("Hdmi"))
                        s = s.Replace("Hdmi", "HDMI");
                    else if (s.Contains("Dvi"))
                        s = s.Replace("Dvi", "DVI");
                    else if (s.Contains("Vga"))
                        s = s.Replace("Vga", "VGA");
                    else if (s.Contains("Video"))
                        s = s.Replace("Video", "VIDEO");
                    else if (s.Contains("YPbPr"))
                        s = s.Replace("YPbPr", "YPbPr");
                }
                Form_Edit f = new Form_Edit(this, item.Name,item.Text);
                f.ShowDialog();
                if (!Edit_str.Equals(""))
                {
                    if (!s.Equals(Edit_str))
                        item.Text = s + "(" + Edit_str + ")";
                    else
                        item.Text = s;
                }
                else
                    item.Text = s;
            }
        }

        private void Send_Command(byte[] array,int leng)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    //serialPort1.Write(array, 0, leng);
                    //Thread.Sleep(300);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, leng);
                    }
                    else
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, leng, 300, 2);
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
                this.Invoke(new MethodInvoker(delegate()
                {
                    richTextBox2.AppendText(ToHexString(array, leng));
                }));
            }
            //startProgress(new SendDataMap(serialPort1, array, 0, leng, 2));//serialPort1发送15次
        }
        private void change_Scene()
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (mergeGroups.Count > 0)
            {
                int x = (mergeGroups.Count - 1) * 3;
                //Console.WriteLine("001111111=" + x);
                byte[] array = new byte[10 + x];
                array[0] = (byte)(0xEA + x);
                array[1] = 0xFD;
                array[2] = 0x20;
                array[3] = 0x01;
                array[4] = (byte)mergeGroups.Count;
                byte y = 0;
                byte souce = 0x00;
                string str = "";
                for (int i = 5, j = 4; i < 5 + mergeGroups.Count * 3; )
                {
                    str = mergeGroups[i - j - 1].StartScreen.IntputType;
                    //Console.WriteLine("intput=" + str);   
                    if (Motherboard_flag == 4)
                    {
                        if (str.Contains("HDMI1"))
                        {
                            souce = 0x00;
                        }
                        else if (str.Contains("HDMI2"))
                        {
                            souce = 0x01;
                        }
                        else if (str.Contains("OPS"))
                        {
                            souce = 0x02;
                        }
                        else if (str.Contains("DVI"))
                        {
                            souce = 0x03;
                        }
                        else if (str.Contains("DP"))
                        {
                            souce = 0x04;
                        }
                        else if (str.Contains("VGA"))
                        {
                            souce = 0x06;
                        }
                    }
                    else
                    {
                        if (str.Contains("VGA"))
                            souce = 0x00;
                        else if (str.Contains("HDMI"))
                            souce = 0x01;
                        else if (str.Contains("DVI"))
                            souce = 0x02;
                        else if (str.Contains("VIDEO1"))
                            souce = 0x03;
                        else if (str.Contains("VIDEO2"))
                            souce = 0x04;
                        else if (str.Contains("VIDEO3"))
                            souce = 0x05;
                        else if (str.Contains("VIDEO4"))
                            souce = 0x06;
                        else if (str.Contains("S-VIDEO"))
                            souce = 0x07;
                        else if (str.Contains("YPbPr"))
                            souce = 0x08;
                        else
                            souce = 0x03;
                    }
                    array[i] = (byte)(mergeGroups[i - j - 1].StartScreen.Number | 0x80);
                    array[i + 1] = (byte)(mergeGroups[i - j - 1].EndScreen.Number | 0x80);
                    array[i + 2] = (byte)(0x08 | ((mergeGroups[i - j - 1].GroupNumber << 4) & 0xF0) | souce);
                    y += (byte)(array[i] + array[i + 1] + array[i + 2]);
                    i += 3;
                    j += 2;
                }
                array[8 + x] = 0x00;
                array[9 + x] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4] + y + array[8 + x]));
                Send_Command(array, 10 + x);
            }
            //Thread.Sleep(200);
        }

        private void Send_Comand()
        {
            int count = rowsCount * colsCount;
            string str = "HDMI";
            for (int i = 0; i < count; i++)
            {
                //Console.WriteLine("count= " + count);
                //int k = unMergeScreenList[i].Number - 1;
                if (screens[i].IntputType.Contains("DVI"))
                    str = "DVI";
                else if (screens[i].IntputType.Contains("HDMI"))
                    str = "HDMI";
                else if (screens[i].IntputType.Contains("VGA"))
                    str = "VGA";
                if (Motherboard_flag == 2)
                {
                    if (screens[i].IntputType.Contains("VIDEO"))
                        str = "VIDEO";
                    else if (screens[i].IntputType.Contains("YPbPr"))
                        str = "YPbPr";
                }
                string s = "";
                if (screens[i].IntputType.Contains("("))
                    s = screens[i].IntputType.Split('(')[0];
                else
                    s = screens[i].IntputType;
                UartSendSwitchMainSignal(s, i, 1, str);
                //Console.WriteLine("count= " + k + "sss== " + s);
            }
            /*
            for (int i = 0; i < mergeGroups.Count; i++)
            {
                if (mergeGroups[i].MergeScreenList[0].IntputType.Contains("DVI"))
                    str = "DVI";
                else if (mergeGroups[i].MergeScreenList[0].IntputType.Contains("HDMI"))
                    str = "HDMI";
                else if (mergeGroups[i].MergeScreenList[0].IntputType.Contains("VGA"))
                    str = "VGA";
                if (Motherboard_flag == 2)
                {
                    if (mergeGroups[i].MergeScreenList[0].IntputType.Contains("VIDEO"))
                        str = "VIDEO";
                    else if (mergeGroups[i].MergeScreenList[0].IntputType.Contains("YPbPr"))
                        str = "YPbPr";
                }
                List<Screen> screenList = mergeGroups[i].MergeScreenList;
                string s = "";
                if (mergeGroups[i].MergeScreenList[0].IntputType.Contains("("))
                    s = mergeGroups[i].MergeScreenList[0].IntputType.Split('(')[0];
                else
                    s = mergeGroups[i].MergeScreenList[0].IntputType;
                //for (int j = 0; j < screenList.Count; j++)
                int k = (byte)screenList[i].Number - 1;
                UartSendSwitchMainSignal(s, k, 1, str);
                Console.WriteLine("screenList.Count= " + k);
            }
             */ 
            Thread.Sleep(Matrix_time);
        }

        private void Send_mergeAll()
        {
            if (Rs232Con)
            {
                if (mergeGroups.Count > 0)
                {
                    try
                    {
                        byte[] array = new byte[11];
                        array[0] = 0xEB;
                        for (int m = 0; m < mergeGroups.Count; m++)
                        {
                            for (int n = 0; n < mergeGroups[m].MergeScreenList.Count; n++)
                            {
                                array[1] = (byte)mergeGroups[m].MergeScreenList[n].Number;
                                array[2] = 0x20;
                                array[3] = 0x89;
                                array[4] = 0x00;
                                array[5] = 0x00;
                                array[6] = 0x00;
                                array[7] = 0x00;
                                array[8] = 0x00;
                                array[9] = 0x00;
                                array[10] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4]));
                                //for (int k = 0; k < 9; k++)
                                //serialPort1.Write(array, 0, 11);
                                if (TCPCOM)
                                {
                                    //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                                    TcpSendMessage(array, 0, 11);
                                }
                                else
                                    SerialPortUtil.serialPortSendData(serialPort1, array, 0, 11, 100, 2);
                                this.Invoke(new MethodInvoker(delegate()
                                {
                                    richTextBox2.AppendText(ToHexString(array, 11));
                                }));
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
            }
            else
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            Thread.Sleep(100);
        }

        public void Send_toScreen()
        {
            if (Rs232Con)
            {
                if (unMergeScreenList.Count > 0)
                {
                    try
                    {
                        int x = 0;
                        int w = 0;
                        if (unMergeScreenList.Count > 12)//合成的单元超过12块时，需把指令分开处理
                        {
                            x = 21;
                            w = 12 * 2;
                        }
                        else
                        {
                            x = (unMergeScreenList.Count - 1) * 2 - 1;
                            w = unMergeScreenList.Count * 2;
                        }
                        byte[] array = new byte[7 + w];
                        array[0] = (byte)(0xEA + x);
                        array[1] = 0xFD;
                        array[2] = 0x20;
                        array[3] = 0x01;
                        array[4] = (byte)((w / 2 << 4) & 0xF0);
                        byte y = 0;
                        byte souce = 0;
                        for (int i = 0; i < w / 2; i++)
                        {
                            array[5 + i * 2] = (byte)(unMergeScreenList[i].Number | 0x80);
                            if (Motherboard_flag == 4)
                            {
                                if (unMergeScreenList[i].IntputType.Contains("HDMI1"))
                                {
                                    souce = 0x00;
                                }
                                else if (unMergeScreenList[i].IntputType.Contains("HDMI2"))
                                {
                                    souce = 0x01;
                                }
                                else if (unMergeScreenList[i].IntputType.Contains("OPS"))
                                {
                                    souce = 0x02;
                                }
                                else if (unMergeScreenList[i].IntputType.Contains("DVI"))
                                {
                                    souce = 0x03;
                                }
                                else if (unMergeScreenList[i].IntputType.Contains("DP"))
                                {
                                    souce = 0x04;
                                }
                                else if (unMergeScreenList[i].IntputType.Contains("VGA"))
                                {
                                    souce = 0x06;
                                }
                            }
                            else
                            {
                                if (unMergeScreenList[i].IntputType.Contains("VGA"))
                                    souce = 0x00;
                                else if (unMergeScreenList[i].IntputType.Contains("VIDEO1") || unMergeScreenList[i].IntputType.Contains("BNC"))
                                    souce = 0x03;
                                else if (unMergeScreenList[i].IntputType.Contains("VIDEO2"))
                                    souce = 0x04;
                                else if (unMergeScreenList[i].IntputType.Contains("VIDEO3"))
                                    souce = 0x05;
                                else if (unMergeScreenList[i].IntputType.Contains("VIDEO4"))
                                    souce = 0x06;
                                else if (unMergeScreenList[i].IntputType.Contains("S-VIDEO"))
                                    souce = 0x07;
                                else if (unMergeScreenList[i].IntputType.Contains("DVI"))
                                    souce = 0x02;
                                else if (unMergeScreenList[i].IntputType.Contains("HDMI"))
                                    souce = 0x01;
                                else if (unMergeScreenList[i].IntputType.Contains("YPbPr"))
                                    souce = 0x08;
                            }
                            array[i * 2 + 6] = (byte)(0x80 | souce);
                            y += (byte)(array[5 + i * 2] + array[i * 2 + 6]);

                        }
                        array[8 + x] = 0x00;
                        array[9 + x] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4] + y));
                        //serialPort1.Write(array, 0, array.Length);
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(array, 0, array.Length);
                        }
                        else
                            SerialPortUtil.serialPortSendData(serialPort1, array, 0, array.Length, 400, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(array, array.Length));
                        }));
                        //Console.WriteLine(array.Length);
                        //Thread.Sleep(400);
                        //Console.WriteLine("dayu12");
                        if (unMergeScreenList.Count > 12)//指令分开处理：12块单元为一组，循环设置
                        {
                            for (int j = 0; j < (unMergeScreenList.Count - 1) / 12; j++)
                            {
                                //Console.WriteLine("unMergeScreenList.Count" + unMergeScreenList.Count);
                                x = (unMergeScreenList.Count - (12 * (j + 1))) * 2;
                                if (x > 24)
                                    x = 24;
                                array = new byte[7 + x];
                                array[0] = (byte)(0xE7 + x);
                                array[1] = 0xFD;
                                array[2] = 0x20;
                                array[3] = 0x01;
                                array[4] = (byte)(((x / 2) << 4) & 0xF0);
                                y = 0;
                                souce = 0;
                                for (int i = 0; i < x/2; i++)
                                {
                                    array[5 + i * 2] = (byte)(unMergeScreenList[i + 12 * (j + 1)].Number | 0x80);
                                    if (Motherboard_flag == 4)
                                    {
                                        if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("HDMI1"))
                                        {
                                            souce = 0x00;
                                        }
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("HDMI2"))
                                        {
                                            souce = 0x01;
                                        }
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("OPS"))
                                        {
                                            souce = 0x02;
                                        }
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("DVI"))
                                        {
                                            souce = 0x03;
                                        }
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("DP"))
                                        {
                                            souce = 0x04;
                                        }
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("VGA"))
                                        {
                                            souce = 0x06;
                                        }
                                    }
                                    else
                                    {
                                        if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("VGA"))
                                            souce = 0x00;
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("VIDEO1") || unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("BNC"))
                                            souce = 0x03;
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("VIDEO2"))
                                            souce = 0x04;
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("VIDEO3"))
                                            souce = 0x05;
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("VIDEO4"))
                                            souce = 0x06;
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("S-VIDEO"))
                                            souce = 0x07;
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("DVI"))
                                            souce = 0x02;
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("HDMI"))
                                            souce = 0x01;
                                        else if (unMergeScreenList[i + 12 * (j + 1)].IntputType.Contains("YPbPr"))
                                            souce = 0x08;
                                    }
                                    array[i * 2 + 6] = (byte)(0x80 | souce);
                                    y += (byte)(array[5 + i * 2] + array[i * 2 + 6]);
                                }
                                array[5 + x] = 0x00;
                                array[6 + x] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4] + y));
                                //Console.WriteLine(array.Length);
                                //for (int k = 0; k < 9; k++)
                                //serialPort1.Write(array, 0, 7 + x);
                                if (TCPCOM)
                                {
                                    //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                                    TcpSendMessage(array, 0, 7 + x);
                                }
                                else
                                    SerialPortUtil.serialPortSendData(serialPort1, array, 0, 7+x, 400, 2);
                                this.Invoke(new MethodInvoker(delegate()
                                {
                                    richTextBox2.AppendText(ToHexString(array, 7+x));
                                }));
                                //Thread.Sleep(400);
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
            }
            else
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            //Thread.Sleep(100);
         }

        /// <summary>
        /// 将changeButton按钮的背景图换成选中的背景图，其他的恢复默认图
        /// </summary>
        /// <param name="changeButton"></param>
        private void buttonChangeImg(Button changeButton) {
            changeButton.BackgroundImage = global::WallControl.Properties.Resources.bg37;
            changeButton.Tag = "selected";
            foreach (Control var in groupBox5.Controls)
            {
                if (var is System.Windows.Forms.Button)
                { //如果是button
                    //Console.WriteLine(var.Name);
                    //Console.WriteLine(changeButton.Name);//((Button)var).Tag.Equals("selected"));
                    if (((var as Button).Name != changeButton.Name) && var.Tag != null && (var as Button).Tag.Equals("selected"))
                    {
                        var.BackgroundImage = global::WallControl.Properties.Resources.bg35;
                        var.Tag = "unSelected";
                    }
                }
            }
        }
        /// <summary>
        /// 场景 按键切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="str"></param>
        private void Button_OnScreen(object sender,string str)
        {
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            Button bt = sender as Button;
            buttonChangeImg(bt);//改变选中的背景，将未选中的改为默认
            saveChange(currentConnectionName + "\\" + currentSceneName);
            FileInfo file = new FileInfo(currentConnectionName + "\\" + str);
            currentSceneName = str;
            currentSceneButtonText = bt.Text;
            if ((!file.Exists))//文件存在，通过文件初始化
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\" + str);
            }
            else
            {
                readSceneFile(currentConnectionName + "\\" + str);
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            //参数
            changed = false;
            rowStar = sheet.SelectionRange.Row;
            rowEnd = sheet.SelectionRange.EndRow;
            colStar = sheet.SelectionRange.Col;
            colEnd = sheet.SelectionRange.EndCol;
            //setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
                LogHelper.WriteLog("=====切换场景模式操作=====");
            }
            else
            {
                stopProgress();
            }
        }

        /// <summary>
        /// 场景一，初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_sce1_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene1.rgf");
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            FileInfo file = new FileInfo(currentConnectionName + "\\scene1.rgf");
            currentSceneName = "scene1.rgf";
            currentSceneButtonText = bt.Text;
            if ((!file.Exists))//文件存在，通过文件初始化
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene1.rgf");
            }
            else 
            {
                readSceneFile(currentConnectionName + "\\scene1.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            //参数
           // changed = false;
            setLbModeFlag(1);
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        /// <summary>
        /// 场景线程执行
        /// </summary>
        public void sceneThread()
        {
            if (Matrix_flag == 1)
            {
                if (Matrix_check_flag)
                    Send_Comand();//矩阵指令
                Send_toScreen();//屏幕没合并指令
                Thread.Sleep(400);
                change_Scene();//屏幕合并对应指令
                //Send_mergeAll();
            }
            else
            {
                Send_toScreen();//屏幕没合并指令
                Thread.Sleep(400);
                change_Scene();//屏幕合并对应指令
                if (Matrix_check_flag)
                    Send_Comand();//矩阵指令
                //Send_mergeAll();
            }
            stopProgress();//停止进度条
        }

        private void bt_sce2_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene2.rgf");
            bt_sce3.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene2.rgf";
            FileInfo file = new FileInfo(currentConnectionName + "\\scene2.rgf");
            currentSceneButtonText = bt.Text;
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene2.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene2.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag(2);
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce3_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene3.rgf");
            bt_sce4.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene3.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene3.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene3.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene3.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
           
            setLbModeFlag(3);
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce4_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene4.rgf");
            bt_sce5.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene4.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene4.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene4.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene4.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag(4);
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce5_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene5.rgf");
            bt_sce6.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene5.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene5.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene5.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene5.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce6_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene6.rgf");
            bt_sce7.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene6.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene6.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene6.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene6.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce7_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene7.rgf");
            bt_sce8.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene7.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene7.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene7.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene7.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce8_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene8.rgf");
            bt_sce9.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene8.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene8.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene8.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene8.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce9_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene9.rgf");
            bt_sce10.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene9.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene9.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene9.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene9.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce10_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene10.rgf");
            bt_sce11.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene10.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene10.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene10.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene10.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }

        private void bt_sce11_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene11.rgf");
            bt_sce12.Enabled = true;
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene11.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene11.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene11.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene11.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }
        private bool bt_sce12_Click_flag = false;
        private void bt_sce12_Click(object sender, EventArgs e)
        {
            Button_OnScreen(sender, "scene12.rgf");
            bt_sce12_Click_flag = true;
            settingFile.WriteBool("SCREEN", "S_flag", bt_sce12_Click_flag);
            /*
            if (systemRunning || (myThread != null && myThread.IsAlive))//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            buttonChangeImg(sender as Button);//改变选中的背景，将未选中的改为默认
            Button bt = sender as Button;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            currentSceneName = "scene12.rgf";
            currentSceneButtonText = bt.Text;
            FileInfo file = new FileInfo(currentConnectionName + "\\scene12.rgf");
            if ((!file.Exists))
            {
                initRoGridSet();
                initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\scene12.rgf");
            }
            else
            {
                readSceneFile(currentConnectionName + "\\scene12.rgf");
                reoGridControl1.Readonly = true;
                reoGridControl2.Readonly = true;
            }
            
            setLbModeFlag();
            if (Rs232Con)
            {
                myThread = new Thread(new ThreadStart(sceneThread)); //开线程         
                myThread.Start(); //启动线程 
            }
            else
            {
                stopProgress();
            }
             * */
            //Send_Comand();//矩阵指令
            //Send_toScreen();//屏幕没合并指令
            //change_Scene();
        }




        /// <summary>
        /// 生成默认address为0
        /// </summary>
        /// <param name="count"></param>
        //public void initAddress(int count)
        //{

        //    address = new uint[count];
        //    for (int i = 0; i < count; i++)
        //    {

        //        address[i] = 0;

        //    }

        //}
        

        /// <summary>
        /// 修改展示当前行列模式和当前场景
        /// </summary>
        /// <param name="sceneNum"></param>
        public void setLbModeFlag() 
        { 
             //lb_modeFlag.Text="拼接模式："+rowsCount+"x"+colsCount+ "当前场景："+sceneNum+"(Scene "+sceneNum+")";
            //if (allSceneName[sceneNum - 1] != null)
            //if (Chinese_English)
                //lb_modeFlag.Text = "Mode：" + rowsCount + "x" + colsCount + "Current scene：" + sceneNum + "(" + allSceneName[sceneNum - 1] + ")";
            //else
                //lb_modeFlag.Text = "拼接模式：" + rowsCount + "x" + colsCount + "当前场景：" + sceneNum + "(" + allSceneName[sceneNum - 1] + ")";
            /*
            for (int i = 0; i < colsCount; i++)
            {
                for (int j = 0; j < rowsCount; j++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    //Console.WriteLine(num);
                    select_usb[num] = 0;//对界面中USB的信号重新设置标记
                    if (screens[((i + 1) + j * colsCount) - 1].IntputType.Contains("USB"))
                        select_usb[num] = 1;
                    
                }
            }
             */ 
        }


        /// <summary>
        /// 通过配置文件初始化信源选择,包括下拉和右击的.
        /// </summary>
        public void initSignalEdit()
        {
            initSignalItemFromFile();
            initSignalComBoxFromFile();   
        }

        /// <summary>
        /// 通过端口数直接生成信源选择,包括下拉和右击的.
        /// </summary>
        public void initSignalEdit(int vgaCount, int videoCount, int dviCount, int hdmiCount, int ypbprCount, int sceneCount)
        {

            String[] nameArr;
            cb_signalEdit.Items.Clear();
            //VGA
            nameArr = generateInputNames("VGA", vgaCount);
            initOneSignalItem("VGA", vgaCount, nameArr);
            initSignalComBox("VGA", vgaCount, nameArr);

            //VIDEO
            nameArr = generateInputNames("VIDEO", videoCount);
            initOneSignalItem("VIDEO", videoCount, nameArr);
            initSignalComBox("VIDEO", videoCount, nameArr);

            //DVI

            nameArr = generateInputNames("DVI", dviCount);
            initOneSignalItem("DVI", dviCount, nameArr);
            initSignalComBox("DVI", dviCount, nameArr);

            //HDMI
            nameArr = generateInputNames("HDMI", hdmiCount);
            initOneSignalItem("HDMI", hdmiCount, nameArr);
            initSignalComBox("HDMI", hdmiCount, nameArr);

            //YPbPr
            nameArr = generateInputNames("YPbPr", ypbprCount);
            initOneSignalItem("YPbPr", ypbprCount, nameArr);
            initSignalComBox("YPbPr", ypbprCount, nameArr);

            //scene name
            cb_signalEdit.Items.Add("SCENE NAME:");//开头
            comboxEditList.Add(false);//开头
            allSceneName = new String[sceneCount];
            for (int i = 1; i <= sceneCount; i++)
            {
                allSceneName[i - 1] = "Scene " + i;
                cb_signalEdit.Items.Add(allSceneName[i - 1]);
                comboxEditList.Add(true);//" 允许改场景名"
            }
            saveSignalEdit();
            this.reoGridControl1.ColumnHeaderContextMenuStrip = null;
            this.reoGridControl1.ContextMenuStrip = this.rightMouseContextMenuStrip;
            this.reoGridControl1.LeadHeaderContextMenuStrip = null;
            this.reoGridControl2.ColumnHeaderContextMenuStrip = null;
            this.reoGridControl2.ContextMenuStrip = this.rightMouseContextMenuStrip;
            this.reoGridControl2.LeadHeaderContextMenuStrip = null;
        }

        /// <summary>
        /// 保存信号编辑的信息
        /// </summary>
        public void saveSignalEdit()
        {
            saveOneSignalEdit("VGA SWITCH:", "InputVga");
            saveOneSignalEdit("VIDEO SWITCH:", "InputVideo");
            saveOneSignalEdit("DVI SWITCH:", "InputDvi");
            saveOneSignalEdit("HDMI SWITCH:", "InputHdmi");
            saveOneSignalEdit("YPbPr SWITCH:", "InputYPbPr");

            saveAllSceneName();//保存场景名称
        }

         /// <summary>
        /// 保存某个信号编辑
        /// </summary>
        /// <param name="inputTypeStart"></param>
        /// <param name="secBeginName"></param>
        private void saveOneSignalEdit(String inputTypeStart, String secBeginName)
        {
            //Console.WriteLine("开始保存信号编辑");
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            int start = cb_signalEdit.FindString(inputTypeStart, 0);
            int end = cb_signalEdit.FindString(" ", start);
            //Console.WriteLine("start=" + start + "end=" + end);
            for (int i = 1; i < end - start; i++)
            {
                settingFile.WriteString("INPUTNAME", secBeginName + i + "Name", (String)cb_signalEdit.Items[start + i]);
                //Console.WriteLine("SAVE+" + (String)cb_signalEdit.Items[start + i]);
            }
        }
        /// <summary>
        /// 通过信源的标志，生成count个string名字的数组
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private String[] generateInputNames(String tag, int count) {

            List<String> nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(tag+i);
            }
            return nameList.ToArray();
        
        }

        /// <summary>
        /// 通过保存的文件来设置右击菜单的各个信源item
        /// </summary>
        private void initSignalItemFromFile() {

            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            List<string> nameList=null;
            int count=0;
            //VGA
            if (Motherboard_type == 4)
                count = 0;
            else
                count = settingFile.ReadInteger("Matrix", "VGACount", 0);
            //Console.WriteLine("vgacount=" + count);
            nameList = new List<string>();
            for (int i = 1; i <=count; i++) {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputVga"+i+"Name", "VGA"+i));
            }
            initOneSignalItem("VGA", count, nameList.ToArray());

            //VIDEO
            if (Motherboard_type == 2)
                count = settingFile.ReadInteger("Matrix", "VIDEOCount", 0);
            else
                count = 0;
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputVideo" + i + "Name", "VIDEO" + i));
            }
            initOneSignalItem("VIDEO", count, nameList.ToArray());

            //DVI
            if (Motherboard_type == 0)
                count = 0;
            else
                count = settingFile.ReadInteger("Matrix", "DVICount", 0);
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputDvi" + i + "Name", "DVI" + i));
            }
            initOneSignalItem("DVI", count, nameList.ToArray());

            //HDMI
            count = settingFile.ReadInteger("Matrix", "HDMICount", 0);
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputHdmi" + i + "Name", "HDMI" + i));
            }
            initOneSignalItem("HDMI", count, nameList.ToArray());

            //YPbPr
            if (Motherboard_type == 2 && Motherboard_flag == 2)
                count = settingFile.ReadInteger("Matrix", "YPbPrCount", 0);
            else
                count = 0;
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputYPbPr" + i + "Name", "YPbPr" + i));
            }
            initOneSignalItem("YPbPr", count, nameList.ToArray());

            //initAllSceneName(12);
        }

        /// <summary>
        /// 初始化场景下拉
        /// </summary>
        /// <param name="count"></param>
        void initAllSceneName(int count)
        {
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            cb_signalEdit.Items.Add("SCENE NAME:");//开头
            comboxEditList.Add(false);//开头
            allSceneName = new String[count];
            for (int i = 1; i <= count; i++)
            {
                allSceneName[i - 1] = settingFile.ReadString("SCENENAME", "Scene" + i + "Name", "Scene " + i);
                cb_signalEdit.Items.Add(allSceneName[i - 1]);
                comboxEditList.Add(true);//" 允许改场景名"
            }
        }

        /// <summary>
        /// 保存场景名称
        /// </summary>
        void saveAllSceneName()
        {
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            for (int i = 0; i < allSceneName.Length; i++)
            {
                settingFile.WriteString("SCENENAME", "Scene" + (i + 1) + "Name", allSceneName[i]);
            }

        }

        /// <summary>
        /// 通过保存的文件来设置编辑信源下拉菜单的各个item
        /// </summary>
        private void initSignalComBoxFromFile() {

            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            cb_signalEdit.Items.Clear();//清空
            List<string> nameList = null;
            comboxEditList = new List<bool>();//初始化下拉中哪些选项可以修改
            int count = 0;
            //VGA
            count = settingFile.ReadInteger("Matrix", "VGACount", 0);
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputVga" + i + "Name", "VGA" + i));
            }
            initSignalComBox("VGA", count, nameList.ToArray());

            //添加VIDEO选项
            count = settingFile.ReadInteger("Matrix", "VIDEOCount", 0);
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputVideo" + i + "Name", "VIDEO" + i));
            }
            initSignalComBox("VIDEO", count, nameList.ToArray());

            //添加DVI选项
            count = settingFile.ReadInteger("Matrix", "DVICount", 0);
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputDvi" + i + "Name", "DVI" + i));
            }
            initSignalComBox("DVI", count, nameList.ToArray());

            //添加HDMI选项
            count = settingFile.ReadInteger("Matrix", "HDMICount", 0);
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputHdmi" + i + "Name", "HDMI" + i));
            }
            initSignalComBox("HDMI", count, nameList.ToArray());

            //添加YPbPr选项
            count = settingFile.ReadInteger("Matrix", "YPbPrCount", 0);
            nameList = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                nameList.Add(settingFile.ReadString("INPUTNAME", "InputYPbPr" + i + "Name", "YPbPr" + i));
            }
            initSignalComBox("YPbPr", count, nameList.ToArray());

            //initAllSceneName(12);

        }


        /// <summary>
        /// 初始化一个信源下拉
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="count"></param>
        /// <param name="itemTexts"></param>
        private void initSignalComBox(String inputType, int count, String[] itemTexts)
        {
            String inputStartName="";
            if ("VGA".Equals(inputType))
            {
                inputStartName = "VGA SWITCH:";
            }
            else if ("VIDEO".Equals(inputType))
            {
                inputStartName = "VIDEO SWITCH:";
            }
            else if ("DVI".Equals(inputType))
            {
                inputStartName = "DVI SWITCH:";
            }
            else if ("HDMI".Equals(inputType))
            {
                inputStartName = "HDMI SWITCH:";
            }
            else if ("YPbPr".Equals(inputType))
            {
                inputStartName = "YPbPr SWITCH:";
            }

            cb_signalEdit.Items.Add(inputStartName);//开头
            comboxEditList.Add(false);//开头
            cb_signalEdit.Items.AddRange(itemTexts);
            for (int i = 0; i < itemTexts.Length; i++)
            {
                comboxEditList.Add(false);//结尾
            }
            cb_signalEdit.Items.Add(" ");//结尾
            comboxEditList.Add(false);//结尾
        
        }

        /// <summary>
        /// 初始化一个信源选择
        /// </summary>
        /// <param name="inputType">信源类型</param>
        /// <param name="count">二级菜单的个数</param>
        /// <param name="subItemTexts">二级菜单的各个text</param>
        private void initOneSignalItem(String inputType,int count,String[] subItemTexts) { 
            String itemName="";
            String itemText="";
            String subItemName="";
            Image image = global::WallControl.Properties.Resources.video;
            int index = 0;
            ToolStripMenuItem inputToolStripMenuItem;
            string type = languageFile.ReadString("WALLSETFORM", "MATRIX", "矩阵").Trim();
            if("VGA".Equals(inputType)){
                itemName = "矩阵VGAToolStripMenuItem";
                itemText = (type + "VGA").Replace("\0", "");
                subItemName = "InputVga";
                image = global::WallControl.Properties.Resources.设备_VGA;
            }
            if ("VIDEO".Equals(inputType))
            {
                itemName = "矩阵VIDEOToolStripMenuItem";
                itemText = (type + "VIDEO").Replace("\0", "");
                subItemName = "InputVideo";
                image = global::WallControl.Properties.Resources.video;
            }
            if ("DVI".Equals(inputType))
            {
                itemName = "矩阵DVIToolStripMenuItem";
                itemText = (type + "DVI").Replace("\0", "");
                subItemName = "InputDvi";
                image = global::WallControl.Properties.Resources.dvi;
            }
            if ("HDMI".Equals(inputType))
            {
                itemName = "矩阵HDMIToolStripMenuItem";
                itemText = (type + "HDMI").Replace("\0", "");
                subItemName = "InputHdmi";
                image = global::WallControl.Properties.Resources.hdmi;
            }
            if ("YPbPr".Equals(inputType))
            {
                itemName = "矩阵YPbPrToolStripMenuItem";
                itemText = (type + "YPbPr").Replace("\0", "");
                subItemName = "InputYPbPr";
            }
            if (count > 0)//该信源数量大于0
            {
                index = rightMouseContextMenuStrip.Items.IndexOfKey(itemName);
                if (index > -1)//找到有这个item
                {
                    inputToolStripMenuItem = rightMouseContextMenuStrip.Items[index] as ToolStripMenuItem;
                    inputToolStripMenuItem.Image = image;
                }
                else
                {//动态插入一级信源菜单
                    inputToolStripMenuItem = new ToolStripMenuItem();
                    inputToolStripMenuItem.Name = itemName;
                    inputToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
                    inputToolStripMenuItem.Text = itemText;
                    inputToolStripMenuItem.Image = image;
                    rightMouseContextMenuStrip.Items.Insert(rightMouseContextMenuStrip.Items.Count - 4, inputToolStripMenuItem);//

                }
                inputToolStripMenuItem.DropDownItems.Clear();
                for (int i = 0; i < count; i++)//加二级信源菜单
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Name = subItemName + (i + 1);
                    item.Size = new System.Drawing.Size(148, 22);
                    item.Text = subItemTexts[i];
                    item.Image = global::WallControl.Properties.Resources.channel;
                    item.MouseDown += new System.Windows.Forms.MouseEventHandler(this.inputToolStripMenuItem_MouseDown);
                    //item.Click += new System.EventHandler(this.inputToolStripMenuItem_MouseClick);
                    inputToolStripMenuItem.DropDownItems.Add(item);
                }

            }
            else {//小于0，一级菜单如有则删

                index = rightMouseContextMenuStrip.Items.IndexOfKey(itemName);
                if (index > -1)//找到有这个item
                {
                    rightMouseContextMenuStrip.Items.RemoveAt(index);

                }
            }

        }

        public void displayScreen()
        {
            for (int i = 0; i < screens.Length; i++)
            {
                //Console.WriteLine(screens[i].Name + screens[i].IntputType + "number:" + screens[i].Number);
            }
        }
         /// <summary>
        /// 获得当前选中的屏幕的数组
        /// </summary>
        /// <returns></returns>
        public Screen[] getSelectedScreen(int rowS, int rowE, int colS, int colE)//操作界面单元的数据获取
        {
            //Console.WriteLine("(" + (sheet.SelectionRange.Row + 1) + "," + (sheet.SelectionRange.Col + 1) + ")——" + "(" + (sheet.SelectionRange.EndRow + 1) + "," + (sheet.SelectionRange.EndCol + 1) + ")");
            List<Screen> screenList = new List<Screen>();
            for (int j = rowS; j <= rowE; j++)
            {
                for (int i = colS; i <= colE; i++)
                {
                    Screen screen = new Screen();
                    //screen.Name = screens[j * colsCount + i].Name;
                    screen.Number = screens[j * colsCount + i].Number;
                    //screen.IntputType = screens[j * colsCount + i].IntputType;
                    screenList.Add(screen);//把选中单元的数据保存的链表中，返回
                    //Console.WriteLine(screen.Number);
                }
            }
            return screenList.ToArray();
        }

        public Screen[] getSelectedScreen_back()//附页单显界面单元的数据获取
        {
            //Console.WriteLine("(" + (sheet.SelectionRange.Row + 1) + "," + (sheet.SelectionRange.Col + 1) + ")——" + "(" + (sheet.SelectionRange.EndRow + 1) + "," + (sheet.SelectionRange.EndCol + 1) + ")");
            List<Screen> screenList = new List<Screen>();
            for (int j = sheet_back.SelectionRange.Row; j <= sheet_back.SelectionRange.EndRow; j++)
            {
                for (int i = sheet_back.SelectionRange.Col; i <= sheet_back.SelectionRange.EndCol; i++)
                {
                    Screen screen = new Screen();
                    screen.Name = screens[j * sheet_back.ColumnCount + i].Name;
                    screen.Number = screens[j * sheet_back.ColumnCount + i].Number;
                    screen.IntputType = screens[j * sheet_back.ColumnCount + i].IntputType;
                    screenList.Add(screen);

                }
            }
            return screenList.ToArray();
        }

        public void Init_port()
        {
            serialPort1.PortName = PortName;
            serialPort1.BaudRate = BaudRate;
            serialPort2.PortName = PortName2;
            serialPort2.BaudRate = BaudRate2;
            serialPort1.ReadTimeout = 10;
        }
        private int TcpSendTP = 0;
        //开通讯
        private void button7_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            //saveChange(currentConnectionName + "\\" + currentSceneName);
            //Console.WriteLine(PortName);
            //Console.WriteLine(BaudRate);
            //timer1.Start();
            try
            {
                if (TCPCOM)
                {
                    int p = settingFile.ReadInteger("Com Set", "TCPP", 0);
                    if (p == 0)
                    {
                        //com_List.Visible = true;
                        if (TCPServer.Start(this))
                        {
                            TcpSendTP = 1;
                            string sl = languageFile.ReadString("MAINFORM", "TCPServerON", "TCPServer 开始监听模式中!");
                            toolStripStatusLabel3.Text = sl;
                        }
                        else
                        {
                            TcpSendTP = 0;
                            MessageBox.Show("TCP服务监听出错，请检测连接信息!");
                            return;
                        }
                    }
                    else if (p == 1)
                    {
                        //com_List.Visible = false;
                        TCPClient = new client(IP, PORT);
                        if (TCPClient.Start(this))
                        {
                            TcpSendTP = 2;
                            string sl = languageFile.ReadString("MAINFORM", "TCPClientON", "TCPClient 连接模式中!");
                            toolStripStatusLabel3.Text = sl;
                        }
                        else
                        {
                            TcpSendTP = 0;
                            string ts = languageFile.ReadString("MESSAGEBOX", "MN", "网络连接出错，请检测连接信息!");
                            string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                            MessageBox.Show(ts, tp);
                            return;
                        }
                    }
                    else
                    {
                        //com_List.Visible = false;
                        TcpSendTP = 3;
                        string sl = languageFile.ReadString("MAINFORM", "UDPON", "UDP 连接模式中!");
                        toolStripStatusLabel3.Text = sl;
                    }
                }
                else
                {
                    serialPort1.Open();
                    if (uMultiComPort == 2 && (PortName != PortName2))
                    {
                        serialPort2.Open();
                    }
                    string sl = languageFile.ReadString("MAINFORM", "STATUS_ON ", "打开");
                    toolStripStatusLabel3.Text = PortName + "  " + sl + " " + BaudRate + " Bps";
                }
            }
            catch
            {
                //Rs232Con = false;
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            toolStripStatusLabel3.ForeColor = Color.Green;
            //button7.ForeColor = Color.Green;
            //button6.ForeColor = Color.Black;
            /*
            if (Chinese_English)
                toolStripStatusLabel3.Text = " Port：" + PortName + " Opened  " + BaudRate + " Bps";
            else
                toolStripStatusLabel3.Text = " 端口：" + PortName + " 已打开 " + BaudRate + " Bps";
             */ 
            Rs232Con = true;
            button14.Enabled = false;
            button7.Enabled = false;
            button6.Enabled = true;
            //button15.Enabled = true;
            //button16.Enabled = true;
            //button17.Enabled = true;
            //button18.Enabled = true;
            //button20.Enabled = true;
            //button21.Enabled = true;
            button5.Enabled = true;
            button8.Enabled = true;
            button27.Enabled = true;
            button28.Enabled = true;
            button30.Enabled = true;
            LogHelper.WriteLog("=====打开通讯串口连接=====");
        }

        /*
        private void Init_all()
        {
            for (int i = 0; i < rowsCount * colsCount; i++)
            {
                if (screens[i].IntputType.Contains("VGA"))
                    UartSendSwitchMainSignalCmd(screens[i].IntputType, i, 1, "VGA");
                else if (screens[i].IntputType.Contains("VIDEO"))
                    UartSendSwitchMainSignalCmd(screens[i].IntputType, i, 1, "VIDEO");
                else if (screens[i].IntputType.Contains("DVI"))
                    UartSendSwitchMainSignalCmd(screens[i].IntputType, i, 1, "DVI");
                else if (screens[i].IntputType.Contains("HDMI"))
                    UartSendSwitchMainSignalCmd(screens[i].IntputType, i, 1, "HDMI");
                else if (screens[i].IntputType.Contains("YPbPr"))
                    UartSendSwitchMainSignalCmd(screens[i].IntputType, i, 1, "YPbPr");
                else
                    UartSendSwitchMainSignalCmd(screens[i].IntputType, i, 1, "VGA");
                Thread.Sleep(Matrix_time);
            }
            //change_Scene();
        }
         */ 
        //关闭连接
        private void button6_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            //saveChange(currentConnectionName + "\\" + currentSceneName);
            try
            {
                if (TCPCOM)
                {
                    int p = settingFile.ReadInteger("Com Set", "TCPP", 0);
                    if (p == 0)
                    {
                        TCPServer.Stop();
                        string sl = languageFile.ReadString("MAINFORM", "TCPServerOFF", "TCPServer 监听模式关闭!");
                        toolStripStatusLabel3.Text = sl;
                    }
                    else if (p == 1)
                    {
                        TCPClient.DisConnect();
                        string sl = languageFile.ReadString("MAINFORM", "TCPClientOFF", "UDP 连接模式断开!");
                        toolStripStatusLabel3.Text = sl;
                    }
                    else
                    {
                        UDPClient.Close();
                        string sl = languageFile.ReadString("MAINFORM", "UDPOFF", "UDP 连接模式断开!");
                        toolStripStatusLabel3.Text = sl;
                    }
                    TcpSendTP = 0;
                    Reflash(TCPServer.m_clients);
                }
                else
                {
                    serialPort1.Close();
                    if (uMultiComPort == 2 && (PortName != PortName2))
                    {
                        serialPort2.Close();
                    }
                    string sl = languageFile.ReadString("MAINFORM", "STATUS_OFF", "关闭");
                    toolStripStatusLabel3.Text = PortName + "  " + sl + " " + BaudRate + " Bps";
                }
            }
            catch
            {
                //Rs232Con = true;
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            Rs232Con = false;
            toolStripStatusLabel3.ForeColor = Color.Red;
            //button7.ForeColor = Color.Black;
            //button6.ForeColor = Color.Red;
            /*
            if (Chinese_English)
                toolStripStatusLabel3.Text = " Port：" + PortName + " Closed  " + BaudRate + " Bps";
            else
                toolStripStatusLabel3.Text = " 端口：" + PortName + " 已关闭  " + BaudRate + " Bps";
            */
            button7.Enabled = true;
            button14.Enabled = true;
            button5.Enabled = false;
            button8.Enabled = false;
            button27.Enabled = false;
            button28.Enabled = false;
            button30.Enabled = false;
            button6.Enabled = false;
            //button15.Enabled = false;
            //button16.Enabled = false;
            //button17.Enabled = false;
            //button18.Enabled = false;
            //button19.Enabled = false;
            //button20.Enabled = false;
            //button21.Enabled = false;

            LogHelper.WriteLog("=====关闭串口连接=====");
        }

        public void Disconnect()
        {
            Rs232Con = false;
            string ts = languageFile.ReadString("MESSAGEBOX", "MDC", "与服务端连接断开,请检测网络连接情况！");
            string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
            MessageBox.Show(ts, tp);
            //MessageBox.Show("与服务端连接断开,请检测网络连接情况！", "异常");
        }


        private void Power_on()//设置定时的电源开，从前往后依次开
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                stopProgress();
                return;
            }
            else
            {
                /*
                byte[] array = new byte[3];
                array[0] = 0x69;
                array[1] = 0xD0;
                array[2] = (byte)(0xFF - (0xFF & array[0] + array[1]));
                SerialPortUtil.serialPortSendData(serialPort1, array, 0, 3, Delay_time, 2);
                richTextBox2.AppendText(ToHexString(array, 3));  
                 * */
                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < colsCount; j++)
                    {
                        int k = i * colsCount + j + 1;
                        Power_on((byte)k);
                        if (Delay_on)
                        {
                            Thread.Sleep(Delay_time);//设置延时
                            Application.DoEvents();
                        }
                        Thread.Sleep(200);
                    }
                }
            }
            stopProgress();
        }

        /// <summary>
        /// 开机线程
        /// </summary>
        /// <param name="rowS"></param>
        /// <param name="rowE"></param>
        /// <param name="colS"></param>
        /// <param name="colE"></param>
        /// <param name="on_off"></param>
        private void power_onThread(int rowS,int rowE,int colS,int colE,bool on_off)
        {
            int num = 0;
            //bool on_to2 = false;
            //Console.WriteLine(rowE);
            //if (tabControl3.SelectedIndex == 0)   
            //if (on_off)
            string x = "";
            {
                for (int i = rowS; i <= rowE; i++)
                {
                    for (int j = colS; j <= colE; j++)
                    {
                        num = screens[((j + 1) + i * colsCount) - 1].Number;//对应的单元地址
                        Power_on((byte)num);
                        if (Delay_on)
                        {
                            Thread.Sleep(Delay_time);//设置延时
                            //Application.DoEvents();
                        }
                        x = num.ToString() + " , ";
                        Thread.Sleep(200);
                        //Console.WriteLine("select_address[num] = " + num);
                    }
                }
            }
            LogHelper.WriteLog("=====开机操作，屏幕【" + x + "】开机=====");
            stopProgress();
        }
        /// <summary>
        /// 关机线程
        /// </summary>
        /// <param name="rowS"></param>
        /// <param name="rowE"></param>
        /// <param name="colS"></param>
        /// <param name="colE"></param>
        /// <param name="on_off"></param>
        private void power_offThread(int rowS, int rowE, int colS, int colE, bool on_off)
        {
            int num = 0;
            //bool on_to2 = false;
            //Console.WriteLine(rowE);
            //if (tabControl3.SelectedIndex == 0)
            //if (on_off)
            string x = "";
            {
                for (int i = rowE; i >= rowS; i--)
                {
                    for (int j = colE; j >= colS; j--)
                    {
                        num = screens[((j + 1) + i * colsCount) - 1].Number;//对应的单元地址
                        //Console.WriteLine(rowE);
                        Power_off((byte)num);
                        if (Delay_off)
                        {
                            Thread.Sleep(Delay_time);//设置延时
                            //Application.DoEvents();
                        }
                        x = num.ToString() + " , ";
                        Thread.Sleep(100);
                    }
                }
            }
            LogHelper.WriteLog("=====关机操作，屏幕【" + x + "】关机=====");
            stopProgress();
        }

        private void Power_onV59()
        {
            byte[] array = new byte[3];
            array[0] = 0x69;
            array[1] = 0xD0;
            array[2] = (byte)(0xFF - (0xFF & array[0] + array[1]));
            if (TCPCOM)
            {
                //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                TcpSendMessage(array, 0, 3);
            }
            else
                SerialPortUtil.serialPortSendData(serialPort1, array, 0, 3, 200+Delay_time, 5);
            richTextBox2.AppendText(ToHexString(array, 3));
            LogHelper.WriteLog("=====开机操作，所以屏幕开机=====");
        }

        //开机
        private void button8_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            else
            {
                if (Motherboard_flag == 4)
                {
                    try
                    {
                        startProgress(0);
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            power_onThread(rowStar, rowEnd, colStar, colEnd, true);
                        })); //开线程         
                        myThread.Start(); //启动线程 
                        //Console.WriteLine("===" + myThread.Name);
                    }
                    catch
                    {
                        string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                        stopProgress();
                    }
                }
                else
                {
                    Power_onV59();
                }
            }
            //stopProgress();
        }
        //延时设置
        public void Delay(int t)
        { 
            int num = 0;
            do
            {
                for (int i = 0; i < 65535; i++)
                {
                    num++;
                }
                t--;
            } while (t > 0);
        }

        private void Power_off()//设置定时的电源关，从后往前依次关
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                stopProgress();
                return;
            }
            else
            {
                /*
                byte[] array = new byte[5];
                array[0] = 0xE5;
                array[1] = 0xFD;
                array[2] = 0x20;
                array[3] = 0x71;
                array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
                SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, Delay_time, 2);
                richTextBox2.AppendText(ToHexString(array, 5));
                */
                for (int i = rowsCount - 1; i >= 0; i--)
                {
                    for (int j = colsCount - 1; j >= 0; j--)
                    {
                        int k = i * colsCount + j + 1;   
                        Power_off((byte)k);
                        if (Delay_off)
                        {
                            Thread.Sleep(Delay_time);//设置延时
                            Application.DoEvents();
                        }
                        Thread.Sleep(200);
                        //Console.WriteLine("kkk===="+k);
                    }
                }
            }
            stopProgress();
        }

        private void Power_offV59()
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = 0xFD;
            array[2] = 0x20;
            array[3] = 0x71;
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            if (TCPCOM)
            {
                //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                TcpSendMessage(array, 0, 5);
            }
            else
                SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 200+Delay_time, 3);
            richTextBox2.AppendText(ToHexString(array, 5));
            LogHelper.WriteLog("=====关机操作，屏幕关机=====");
        }

        //关机
        private void button5_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            else
            {
                if (Motherboard_flag == 4)
                {
                    try
                    {
                        startProgress(0);
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            power_offThread(rowStar, rowEnd, colStar, colEnd, false);
                        })); //开线程         
                        myThread.Start(); //启动线程 
                    }
                    catch
                    {
                        stopProgress();
                        string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                    }
                }
                else
                    Power_offV59();
            }
            //richTextBox1.Text += Str;
        }
        /// <summary>
        /// 屏幕序号的显示指令
        /// </summary>
        /// <param name="A_0"></param>
        private void Send1(bool A_0)// 屏幕序号的显示指令
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = 0xFD;
            array[2] = 0x20;
            if (A_0)
                array[3] = 0x23;//显示
            else
                array[3] = 0x24;  
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            try
            {
                if (Rs232Con)
                {
                    //serialPort1.Write(array, 0, 5);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, 5);
                    }
                    else
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 150, 1);
                    //richTextBox2.AppendText(ToHexString(array, 5));
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
                richTextBox2.AppendText(ToHexString(array, 5));
            }));
        }

        private void Send2(bool A_0)//  屏幕的地址显示指令
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = 0xFD;
            array[2] = 0x20;
            if (A_0)
                array[3] = 0x21;
            else
                array[3] = 0x22;
            
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            try
            {
                if (Rs232Con)
                {
                    //serialPort1.Write(array, 0, 5);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, 5);
                    }
                    else
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 150, 1);
                    //richTextBox2.AppendText(ToHexString(array, 5));
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
                richTextBox2.AppendText(ToHexString(array, 5));
            }));
        }
        //显示序号
        private void button1_Click(object sender, EventArgs e)
        {
            //if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                Send1(true);
            }
            //Delay(100);
        }
        //显示隐藏地址
        private void button3_Click(object sender, EventArgs e)
        {
            //if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                Send2(true);
            }
            //Delay(100);
        }

        private void New_adress(uint A_0, byte A_1)//通过屏幕的序号来设置对应屏幕的地址----指令
        {
            byte[] array = new byte[11];
            array[0] = 0xEB;
            array[1] = 0xFD;
            array[2] = 0x20;
            array[3] = 0x13;
            array[4] = (byte)(A_0 >> 16);
            array[5] = (byte)(A_0 >> 8);
            array[6] = (byte)A_0;
            array[7] = A_1;
            array[8] = (byte)colsCount;
            array[9] = (byte)rowsCount;
            array[10] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4] + array[5] + array[6] + array[7] + array[8] + array[9]));
            try
            {
                //serialPort1.Write(array, 0, 11);
                if (TCPCOM)
                {
                    //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                    TcpSendMessage(array, 0, 11);
                }
                else
                    SerialPortUtil.serialPortSendData(serialPort1, array, 0, 11, 150, 6);
                //richTextBox2.AppendText(ToHexString(array, 11));  
            }
            catch
            {
                //Rs232Con = false;
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
            this.Invoke(new MethodInvoker(delegate()
            {
                richTextBox2.AppendText(ToHexString(array, 11));
            }));
            stopProgress();
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
                if (TCPCOM)
                {
                    //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                    TcpSendMessage(array, 0, 9);
                }
                else
                    SerialPortUtil.serialPortSendData(serialPort1, array, 0, 9, 100, 5);
                //richTextBox2.AppendText(ToHexString(array, 11));  
            }
            catch
            {
                //Rs232Con = false;
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
            this.Invoke(new MethodInvoker(delegate()
            {
                richTextBox2.AppendText(ToHexString(array, 9));
            }));
        }

        /// <summary>
        /// 切换本地信源指令
        /// </summary>
        /// <param name="A_0">地址</param>
        /// <param name="A_1">通道</param>
        private void Send_Signa(byte A_0, byte A_1)//本地通道的信源切换指令
        {
            byte[] array = new byte[6];
            array[0] = 0xE6;
            array[1] = A_0;
            array[2] = 0x20;
            array[3] = 0x50;
            array[4] = A_1;
            array[5] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4]));
            
            try
            {
                if (Rs232Con)
                {
                    //serialPort1.Write(array, 0, 6);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, 6);
                    }
                    else
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 6, 100, 3);
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        richTextBox2.AppendText(ToHexString(array, 6));
                    }));
                }
                //Thread.Sleep(100);
                //Delay(100);
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }
        
        private void Turn_on_off( bool A_1,int A_2)//屏幕背光开关----指令
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = (byte)A_2;//check_address[A_2];
            array[2] = 0x20;
            if (A_1)
            {
                array[3] = 0x73;//表示 --- 开
            }
            else
            {
                array[3] = 0x72;//表示 --- 关
            }
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            try
            {
                if (Rs232Con)
                {
                    //serialPort1.Write(array, 0, 5);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, 5);
                    }
                    else
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 3);
                    //richTextBox2.AppendText(ToHexString(array, 5));
                }
            }
            catch
            {
                //Rs232Con = false;
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
            this.Invoke(new MethodInvoker(delegate()
            {
                richTextBox2.AppendText(ToHexString(array, 5));
            }));
        }

        
        private void 画面定位ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int num0 = sheet.SelectionRange.Row;
            int num1 = sheet.SelectionRange.Col + 1;
            int num = screens[(num1 + num0 * colsCount) - 1].Number;
            address_backup = (byte)num;

            if (Rs232Con)
            {
                try
                {
                    serialPort1.Close();
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
            }

            Form_Picture f = new Form_Picture(this);
            f.ShowDialog();

            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Open();
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
        }
        /// <summary>
        /// cb_signalEdit当从下拉列表中选择项而下拉列表关闭时
        /// </summary>
        /// <param name="sender"></param>
        private void cb_signalEdit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex >= 0)
            {
                curEditIndex = cb.SelectedIndex;
            }
        }
        /// <summary>
        /// 当cb_signalEdit文本改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_signalEdit_TextUpdate(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (curEditIndex >= 0)
            {
                //curEditIndex = cb.SelectedIndex;
                if (comboxEditList[curEditIndex])
                {
                    //Console.WriteLine("可以改哟" + cb.Text);
                    cb.Items[curEditIndex] = cb.Text;
                    cb.SelectionStart = cb.Text.Length;
                    int start = cb_signalEdit.FindString("SCENE NAME:", 0);
                    allSceneName[curEditIndex - start - 1] = cb.Text;

                }
            }
        }

        private void 系统信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            if (Motherboard_flag == 4)
            {
                for (int i = 0; i < 256; i++)//清除记录
                    select_address[i] = 0;
                //if (tabControl3.SelectedIndex == 0)
                {
                    for (int j = rowStar; j <= rowEnd; j++)
                    {
                        for (int i = colStar; i <= colEnd; i++)
                        {
                            int num = screens[((i + 1) + j * colsCount) - 1].Number;
                            select_address[num] = (byte)num;
                            //Console.WriteLine("select_address[num] = " + select_address[num]);
                        }
                    }
                }
                new Form_Info(this).ShowDialog();
            }
            else
            {
                if (Rs232Con)
                {
                    try
                    {
                        serialPort1.Close();
                    }
                    catch
                    {
                        string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                        return;
                    }
                }
                int num0 = rowStar;
                int num1 = colStar + 1;
                int num = screens[(num1 + num0 * colsCount) - 1].Number;

                new Form_Info59(this, num).ShowDialog();
                if (!serialPort1.IsOpen)
                {
                    try
                    {
                        serialPort1.Open();
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

        private void 关于软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form_about(this).ShowDialog();
        }


        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Console.WriteLine("关闭=======");
            /*
            String[] sysStatus = toolStripStatusLabel4.Text.Split(' ');
            if(Chinese_English)
                sysStatus[1] = "idle";
            else
                sysStatus[1] = "空闲";
            toolStripStatusLabel4.Text = sysStatus[0] + ' ' + sysStatus[1];
             */
            toolStripStatusLabel4.Text = languageFile.ReadString("MAINFORM", "SYS_STATUS", "系统状态：") + languageFile.ReadString("MAINFORM", "STATUS1", "空闲");
            toolStripProgressBar1.Visible = false;

            this.toolStripProgressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripProgressBar1.RightToLeftLayout = false;
            progressToRight = true;
            systemRunning = false;
        }

        /// <summary>
        /// 改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Console.WriteLine("改变" + e.ProgressPercentage);
            this.toolStripProgressBar1.Value = e.ProgressPercentage;

            if (e.ProgressPercentage == 100)
            {//说明是100的倍数
                //Console.WriteLine(progressToRight + "---");
                if (progressToRight)
                {
                    //改方向为向左边
                    this.toolStripProgressBar1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                    this.toolStripProgressBar1.RightToLeftLayout = true;
                    progressToRight = false;
                }
                else
                {//改方向为向右边
                    this.toolStripProgressBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
                    this.toolStripProgressBar1.RightToLeftLayout = false;
                    progressToRight = true;
                }
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            systemRunning = true;
            //int delayMill = (int)e.Argument;//延时毫秒数
            int progressCount = 100;//最大
            int i = 1;
            int gap = 1;//一次加多少
            //double startData = (DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds;//获取开始的毫秒数
            //Console.WriteLine("startData="+startData);
            while (!worker.CancellationPending)
            {
                //Console.WriteLine("DateTime.Now.Millisecond=" + DateTime.Now.Millisecond);
                //if (delayMill > 0 && (DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds - startData >= delayMill)
                //{
                    //break;
                //}

                // Console.WriteLine("执行" + i);
                if (i == progressCount + gap)
                {
                    worker.ReportProgress(0);
                    //加要放入子线程中执行的代码（如 执行的发送指令代码）                   
                    i = 0;
                }
                else
                {
                    worker.ReportProgress(i);
                }
                Thread.Sleep(50);
                i++;
            }
            if (worker.CancellationPending)
                e.Cancel = true;
            //Console.WriteLine(e.Cancel);
            /*
            SendDataMap SendDataMap = (SendDataMap)e.Argument;
            
            int progressCount = 100;
            int i = 0;
            int num = 0;
            int gap = 1;//一次加多少
            while (!worker.CancellationPending )
            {
                if (num == 15) {
                    break;
                }
                Thread.Sleep(10);
               // Console.WriteLine("执行" + i);
                if (i == progressCount + gap)
                {
                    worker.ReportProgress(0);
                    //加要放入子线程中执行的代码（如 执行的发送指令代码）                   
                   i = 0;
                }
                else
                {
                    worker.ReportProgress(i);
                }
                if (num < SendDataMap.cycleCount)
                {            
                    try
                    {
                        if (Rs232Con)
                        {
                            //serialPort1.Write(SendDataMap.data, SendDataMap.offset, SendDataMap.count);
                            SendDataMap.serialPort.Write(SendDataMap.data, SendDataMap.offset, SendDataMap.count);
                            //Delay(100);
                            //Console.WriteLine(num);
                        }
                    }
                    catch
                    {
                        Rs232Con = false;
                        MessageBox.Show("串口出错！", "提示");
                    }
                }
                num++;
                i += gap;
            }*/
        }

        /// <summary>
        /// 开启进度条
        /// </summary>
        public void startProgress(int delay)
        {
            //Console.WriteLine("开启=======");
            /*
            String[] sysStatus = toolStripStatusLabel4.Text.Split(' ');
            if (Chinese_English)
                sysStatus[1] = "busy";
            else
                sysStatus[1] = "忙碌中";
            toolStripStatusLabel4.Text = sysStatus[0] + ' ' + sysStatus[1];
             */
            toolStripStatusLabel4.Text = languageFile.ReadString("MAINFORM", "SYS_STATUS", "系统状态：") + languageFile.ReadString("MAINFORM", "STATUS2", "忙碌中");
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = true;
            progressToRight = true;
            statusStrip1.Refresh();
            worker.RunWorkerAsync(delay);
            
        }

        /// <summary>
        /// 手动关闭进度条
        /// </summary>
        public void stopProgress()
        {
            //Thread.Sleep(50);
            //Console.WriteLine("关闭2=======");
            /*
            String[] sysStatus = toolStripStatusLabel4.Text.Split(' ');
            if (Chinese_English)
                sysStatus[1] = "Idle";
            else
                sysStatus[1] = "空闲";
             */
            toolStripStatusLabel4.Text = languageFile.ReadString("MAINFORM", "SYS_STATUS", "系统状态：") + languageFile.ReadString("MAINFORM", "STATUS1", "空闲");
            worker.CancelAsync();
            //progressBar1.Visible = false;
        }


        private void UartSendSwitchMainSignalCmd(string inputSingle, int start, int count, string ucDevice,byte souce)
        {
            {
                myThread = new Thread(new ThreadStart(delegate()
                {
                    do_Siganl_Clik(rowStar, rowEnd, colStar, colEnd, souce, ucDevice);
                })); //开线程          
                myThread.Start(); //启动线程 
                LogHelper.WriteLog("======执行切换矩阵操作【" + ucDevice + "】======");
            }
            while (myThread.IsAlive)
            {
                Application.DoEvents();
            }
            /*
            for (int i = rowStar; i <= rowEnd; i++)
            {
                for (int j = colStar; j <= colEnd; j++)
                {
                    //int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    //Send_Signa((byte)num, souce);
                    inputSingle = screens[((j + 1) + i * colsCount) - 1].IntputType;
                    if (inputSingle.Contains("("))
                        inputSingle = inputSingle.Split('(')[0];
                    for (int k = 0; k < 2;k++ )
                        UartSendSwitchMainSignal(inputSingle, (j + i * colsCount), 1, ucDevice);
                    //Console.WriteLine(start + "," + (j + i * colsCount));
                }
            }
             */ 
        }

        //
        //SWITCH_CUSTOMER
        //0:  SW_JSL        for 金三立      DB     
        //1:  SW_JSL1       for 金三立      淳中
        //2:  SW_JSL2       for 金三立      ST-MS750 云台                       //12
        //3:  SW_CH         for 长虹电子     
        //4:  SW_GDWC       for 广东威创    //1*2! 兼容协议                     //1
        //5:  SW_SHZF       for 上海卓飞
        //6:  SW_CREATOR    for standard       //creator2.0/2.5兼容  1*2!       //1
        //7:  SW_KTC01                      //SW_SHZF兼容   淳中
        //8:  SW_KTC02                      //中性系列矩阵  晶日盛
        //9:  SW_KTC03                      //许环敏 MATRIX2.01  陈锦波         //1   
        //10: SW_KTC04                      //王春燕 VGA矩阵    //艾得讯        //1 金三立  创凯
        //11: SW_KTC05                      //钟鸣 晶日盛矩阵
        //12: SW_KTC06                      //范维亮 华彩矩阵云台               //1
        //13: SW_KTC07                      //范维亮 卓飞混合矩阵
        //14: SW_KTC08                      //范维亮 卓同矩阵JC
        //15: SW_KTC09                      //范维亮 卓同矩阵AK                 //1
        //16: SW_KTC10                      //李峰虎 SISO 6.0协议               //1
        //17: SW_KTC11                      //许环敏 SISO V23                   //1
        //18: SW_KTC12                      //范维亮 Infinova                   //1
        //19: SW_KTC13                      //TCL 混合矩阵                      //1
        //20: SW_KTC14                      //范维亮 卓同VGA矩阵        SW_CREATOR  3V4,5,6.协议
        //21: SW_KTC15                      //陈景波 盘古矩阵
        //22: SW_KTC16                      //陈景波 宏控DVI    //NOT OK
        //23: SW_JSL3       for 金三立      ST-MS750 云台
        //24: SW_KTC17                      //陈景波     //NOT OK               //1
        //25: SW_KTC18                      //孙元强 
        //26: SW_KTC19                      //钟鸣 多媒体 蓝宝
        //27: SW_KTC20                      //钟鸣 博瑞矩阵 //not ok
        //28: SW_KTC21                      //黄淑芳 创凯矩阵                   //1 
        //29: SW_KTC22                      //钟鸣 誉彩VGA矩阵 //ok             //1
        //30: SW_KTC23                      //罗继超 天地伟业AV矩阵 //ok        //1
        //31: SW_KTC24                      //金三立 淳中混合矩阵   not ok      //1
        //31: SW_KTC25                      //星网 讯维矩阵   
        //32: SW_KTC26                      //清华同方 VIDEO                    //1 
        //33: SW_KTC27                      //清华同方 VGA                      //1   
        //34: SW_KTC28                      //北京大恒 HDMI                     //1
        //35: SW_KTC29                      //汉尊                              //1
        //36: SW_KTC30                      //深艾尔                            //1
        //37: SW_KTC31                      //漠龙                              //1
        //38: SW_KTC32                      //郑旋   1,2,Y                      //1
        //39: SW_KTC33                      //for 上海卓飞   /1X2. 兼容协议     //1
        //40: SW_KTC34                      //舟山人防                          //1
        //
        //
        //88: SW_YK         for YK SW 111012 ok
        //SWITCH SETTING
        //
        //
        //switch Main single change  矩阵控制选择
        private void UartSendSwitchMainSignal(string inputSingle, int start, int count, string ucDevice)
        {
            int i, Temp_Crc;
            string Temp_In = "", strCmd_Out = "";
            byte[] P = new byte[512];
            byte ucInput_source = 0, Address_temp = 0;
            //int ucInput_source_av = 0;
            int uSwitchTemp = 1;
            int ukey = 0;
            if (ucDevice.Contains("VGA"))
            {
                uSwitchTemp = vgaMatrixSelect;
                ukey = 2;
            }
            else if (ucDevice.Contains("VIDEO"))
            {
                uSwitchTemp = videoMatrixSelect;
                ukey = 3;
            }
            else if (ucDevice.Contains("DVI"))
            {
                uSwitchTemp = dviMatrixSelect;
                ukey = 1;
            }
            else if (ucDevice.Contains("HDMI"))
            {
                uSwitchTemp = hdmiMatrixSelect;
                ukey = 0;
            }
            else if (ucDevice.Contains("YPbPr"))
                uSwitchTemp = ypbprMatrixSelect;

            //Console.WriteLine("uSwitchTemp==" + uSwitchTemp);
            //SWITCH_CUSTOMER
            if (uSwitchTemp == 1)    //SW_JSL
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = "<";
                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + Temp_In + ",";
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString() + ",";
                }
                if (ucDevice == "VGA")
                    strCmd_Out = strCmd_Out + ((vgaAddress >> 4) * 16 + (vgaAddress & 0X0F)).ToString() + ",D,";
                else if (ucDevice == "VIDEO")
                    strCmd_Out = strCmd_Out + ((videoAddress >> 4) * 16 + (videoAddress & 0X0F)).ToString() + ",D,";
                else if (ucDevice == "DVI")
                    strCmd_Out = strCmd_Out + ((dviAddress >> 4) * 16 + (dviAddress & 0X0F)).ToString() + ",D,";
                else if (ucDevice == "HDMI")
                    strCmd_Out = strCmd_Out + ((hdmiAddress >> 4) * 16 + (hdmiAddress & 0X0F)).ToString() + ",D,";
                else if (ucDevice == "YPbPr")
                    strCmd_Out = strCmd_Out + ((ypbprAddress >> 4) * 16 + (ypbprAddress & 0X0F)).ToString() + ",D,";

                strCmd_Out = strCmd_Out + "V>";  //switch cmd

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);//serialPort1.Write(strCmd_Out);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
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
            else if (uSwitchTemp == 2)    //SW_JSL1
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = "";
                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + Temp_In + ",";
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString() + ",";
                }
                if (ucDevice == "VGA")
                    strCmd_Out = strCmd_Out + ((vgaAddress >> 4) * 16 + (vgaAddress & 0X0F)).ToString() + ",D,";
                else if (ucDevice == "VIDEO")
                    strCmd_Out = strCmd_Out + ((videoAddress >> 4) * 16 + (videoAddress & 0X0F)).ToString() + ",D,";
                else if (ucDevice == "DVI")
                    strCmd_Out = strCmd_Out + ((dviAddress >> 4) * 16 + (dviAddress & 0X0F)).ToString() + ",D,";
                else if (ucDevice == "HDMI")
                    strCmd_Out = strCmd_Out + ((hdmiAddress >> 4) * 16 + (hdmiAddress & 0X0F)).ToString() + ",D,";
                else if (ucDevice == "YPbPr")
                    strCmd_Out = strCmd_Out + ((ypbprAddress >> 4) * 16 + (ypbprAddress & 0X0F)).ToString() + ",D,";

                strCmd_Out = strCmd_Out + "V";  //switch cmd

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
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
            else if (uSwitchTemp == 3)    //SW_CH
            {
                P[0] = 0x96;
                /*
                if (ucDevice == "VGA")
                    P[1] = vgaAddress;
                else if (ucDevice == "AV")
                    P[1] = videoAddress;
                else if (ucDevice == "DVI")
                    P[1] = dviAddress;
                */
                P[1] = 0x01;
                P[2] = 0x01;
                P[3] = (byte)(2 + count * 2);
                if (ucDevice == "VGA")
                    P[4] = 0xA0;
                else if (ucDevice == "VIDEO")
                    P[4] = 0xA4;
                else if (ucDevice == "DVI")
                    P[4] = 0xAF;
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)) - 1);
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)) - 1);
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)) - 1);
                Temp_Crc = P[0] + P[1] + P[2] + P[3] + P[4];
                for (i = 0; i < count; i++)
                {
                    P[5 + i * 2] = (byte)(PanelCount[ukey, start + i] - 1);
                    P[5 + i * 2 + 1] = ucInput_source;
                    Temp_Crc = Temp_Crc + P[5 + i * 2] + P[5 + i * 2 + 1];
                }
                P[5 + count * 2] = (byte)(255 - (byte)Temp_Crc);    //CRC

                if (P[5 + count * 2] == 0x96)   //rule for crc
                    P[5 + count * 2] = 0x6A;

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 6 + count * 2);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 6 + count * 2, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 6 + count * 2, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 6 + count * 2);
                        else
                            serialPort1.Write(P, 0, 6 + count * 2);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, (6 + count * 2)));
                        }));
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
            else if (uSwitchTemp == 4)    //SW_GDWC
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = Temp_In + "*";
                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                strCmd_Out = strCmd_Out + "!";
                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
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
            else if (uSwitchTemp == 5)    //SW_SHZF  //  23 _ 00 _ _ 04 _ _ _ 56 FF
            {
                //SHZF SW1
                /*


                            P[0] = 0x96;
                            P[1] = 0x01;
                            P[2] = 0x01;
                            P[3] = (byte)(2 + count*2);
                            if (ucDevice == "VGA")
                                P[4] = 0xA0;
                            else if (ucDevice == "AV")
                                P[4] = 0xA4;
                            else if (ucDevice == "DVI")
                                P[4] = 0xAF;
                            if (inputSingle.Contains("VGA"))
                                ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                            else if (inputSingle.Contains("AV"))
                                ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 2)) - 1);
                            else if (inputSingle.Contains("DVI"))
                                ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                            else if (inputSingle.Contains("HDMI"))
                                ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)) - 1);

                            Temp_Crc = P[0] + P[1] + P[2] + P[3] + P[4];
                            for (i = 0; i < count; i++)
                            {
                                P[5 + i * 2] = (byte)(PanelCount[start + i] - 1);
                                P[5 + i * 2 + 1] = ucInput_source; 
                                Temp_Crc = Temp_Crc + P[5 + i * 2] + P[5 + i * 2 + 1];
                            }
                            P[5 + count*2] = (byte)(255 - (byte)Temp_Crc);    //CRC
                
                            if (P[5 + count * 2] == 0x96)   //rule for crc
                                P[5 + count * 2] = 0x6A;

                            try
                            {
                                if (Rs232Con)
                                    serialPort1.Write(P, 0, 6 + count*2);
                            }
                            catch
                            {
                                Rs232Con = false;
            #if MENU_ENG
                                MessageBox_Show("Unit COM Port Error！",false);
            #else
                                MessageBox.Show("串口出错！");
            #endif
                                uPortErr = true;
                                uPortErrDelay = 0;
                            }
                */
                //SHZF SW2  //CREATOR sw

                //SHZF SW3
                P[0] = 0x23;
                if (ucDevice == "VGA")
                    P[1] = (byte)vgaAddress;          //device address
                else if (ucDevice == "VIDEO")
                    P[1] = (byte)videoAddress;
                else if (ucDevice == "DVI")
                    P[1] = (byte)dviAddress;
                P[2] = 0;                       //data length high byte
                P[3] = (byte)((count * 3) & 0xFF);       //data length low byte
                P[4] = 0x04;                    //change channel cmd
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)) - 1);
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)) - 1);
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)) - 1);

                for (i = 0; i < count; i++)
                {
                    P[5 + i * 3] = (byte)(PanelCount[ukey, start + i] - 1);           //output
                    P[5 + i * 3 + 1] = ucInput_source;                          //input
                    P[5 + i * 3 + 2] = 0x56;                                    //video switch
                }
                P[5 + count * 3] = 0xFF;    //CRC

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 6 + count * 3);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 6 + count * 3, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 6 + count * 3, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 6 + count * 3);
                        else
                            serialPort1.Write(P, 0, 6 + count * 3);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, (6 + count * 3)));
                        }));
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
            else if (uSwitchTemp == 6)    //SW_KTC02
            {
                P[0] = 0xC5;
                P[1] = 0xB1;
                if (ucDevice == "VGA")
                {
                    P[2] = (byte)vgaAddress;          //device address
                    P[3] = 0xA3;
                }
                else if (ucDevice == "VIDEO")
                {
                    P[2] = (byte)videoAddress;
                    P[3] = 0xA2;
                }
                else if (ucDevice == "DVI")
                {
                    P[2] = (byte)dviAddress;
                    P[3] = 0xA5;
                }
                else if (ucDevice == "HDMI")
                {
                    P[2] = (byte)hdmiAddress;
                    P[3] = 0xAF;
                }
                else if (ucDevice == "YPbPr")
                {
                    P[2] = (byte)hdmiAddress;
                    P[3] = 0xAF;
                }
                P[4] = (byte)(3 + count);       //CMD LENGTH
                P[5] = 0x15;                    //SWITCH CMD MODE
                P[6] = 0x52;                    //SW VIDEO
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                P[7] = ucInput_source;
                Temp_Crc = P[2] + P[3] + P[4] + P[5] + P[6] + P[7];
                for (i = 0; i < count; i++)
                {
                    P[8 + i] = (byte)(PanelCount[ukey, start + i]);
                    Temp_Crc = Temp_Crc + P[8 + i];
                }
                P[8 + count] = (byte)(Temp_Crc);    //CRC

                if (P[8] == 0xC5)   //rule for crc
                    P[8] = 0x3A;

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 9 + count);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 9 + count, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 9 + count, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 9 + count);
                        else
                            serialPort1.Write(P, 0, 9 + count);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, (9 + count)));
                        }));
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
            else if (uSwitchTemp == 7)    //SW_KTC03///  快捷矩阵指令
            {
                if (ucDevice == "VGA")
                {
                    if (vgaAddress < 10)
                        strCmd_Out = "PV" + "0" + vgaAddress.ToString();
                    else
                        strCmd_Out = "PV" + vgaAddress.ToString();
                }
                else if (ucDevice == "VIDEO")
                {
                    if (videoAddress < 10)
                        strCmd_Out = "PA" + "0" + videoAddress.ToString();
                    else
                        strCmd_Out = "PA" + videoAddress.ToString();
                }
                else if (ucDevice == "DVI")
                {
                    if (dviAddress < 10)
                        strCmd_Out = "PD" + "0" + dviAddress.ToString();
                    else
                        strCmd_Out = "PD" + dviAddress.ToString();
                }
                else if (ucDevice == "HDMI")
                {
                    if (hdmiAddress < 10)
                        strCmd_Out = "PH" + "0" + hdmiAddress.ToString();
                    else
                        strCmd_Out = "PH" + hdmiAddress.ToString();
                }
                else if (ucDevice == "YPbPr")
                {
                    if (ypbprAddress < 10)
                        strCmd_Out = "PR" + "0" + ypbprAddress.ToString();
                    else
                        strCmd_Out = "PR" + ypbprAddress.ToString();
                }
                strCmd_Out = strCmd_Out + "SW";

                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                if (Temp_In.Length < 2)
                    strCmd_Out = strCmd_Out + "0" + Temp_In;
                else
                    strCmd_Out = strCmd_Out + Temp_In;
                for (i = 0; i < count; i++)
                {
                    if (PanelCount[ukey, i + start] < 10)
                        strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                    strCmd_Out = strCmd_Out + "NT";
                }
                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
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
            else if (uSwitchTemp == 8)    //SW_KTC04   FF _  03 _ _ AA
            {
                P[0] = 0xFF;    //start
                if (ucDevice == "VGA")
                {
                    P[1] = (byte)vgaAddress;          //device address
                    P[2] = 0x04;        //SWITCH CMD
                }
                else if (ucDevice == "VIDEO")
                {
                    P[1] = (byte)videoAddress;
                    P[2] = 0x03;        //SWITCH CMD
                }
                else if (ucDevice == "DVI")
                {
                    P[1] = (byte)dviAddress;
                    P[2] = 0x03;        //SWITCH CMD
                }
                else if (ucDevice == "HDMI")
                {
                    P[1] = (byte)hdmiAddress;
                    P[2] = 0x03;        //SWITCH CMD
                }
                else if (ucDevice == "YPbPr")
                {
                    P[1] = (byte)ypbprAddress;
                    P[2] = 0x03;        //SWITCH CMD
                }

                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                P[3] = ucInput_source;
                for (i = 0; i < count; i++)
                {
                    P[4 + i] = (byte)(PanelCount[ukey, start + i]);
                }
                P[5] = 0xAA;    //end

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 6);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 6, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 6, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 6);
                        else
                            serialPort1.Write(P, 0, 6);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 6));
                        }));
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
            else if (uSwitchTemp == 9)    //SW_KTC05
            {
                P[0] = 0x96;    //start
                if (ucDevice == "VGA")
                {
                    P[1] = (byte)vgaAddress;          //device address
                    P[4] = 0xA0;                //switch video
                }
                else if (ucDevice == "VIDEO")
                {
                    P[1] = (byte)videoAddress;
                    P[4] = 0xA1;                //switch video
                }
                else if (ucDevice == "DVI")
                {
                    P[1] = (byte)dviAddress;
                    P[4] = 0xA0;                //switch video
                }
                else if (ucDevice == "HDMI")
                {
                    P[1] = (byte)hdmiAddress;
                    P[4] = 0xA0;                //switch video
                }
                else if (ucDevice == "YPbPr")
                {
                    P[1] = (byte)ypbprAddress;
                    P[4] = 0xA0;                //switch video
                }
                P[2] = 0x01;                //SWITCH CMD
                P[3] = (byte)(2 * count + 2);       //data length

                Temp_Crc = P[0] + P[1] + P[2] + P[3] + P[4];

                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                for (i = 0; i < count; i++)
                {
                    P[5 + i * 2] = (byte)(PanelCount[ukey, start + i] - 1);
                    P[5 + i * 2 + 1] = (byte)(ucInput_source - 1);
                    Temp_Crc = Temp_Crc + P[5 + i * 2] + P[5 + i * 2 + 1];
                }
                //P[5 + count * 2] = (byte)Temp_Crc;    //CRC
                P[5 + count * 2] = (byte)(255 - (byte)Temp_Crc);    //CRC

                if (P[5 + count * 2] == 0x96)
                    P[5 + count * 2] = 0x6A;

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 6 + count * 2);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 6 + count * 2, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 6 + count * 2, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, (6 + count * 2));
                        else
                            serialPort1.Write(P, 0, (6 + count * 2));
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, (6 + count * 2)));
                        }));
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
            /*
            else if (uSwitchTemp == 2)    //SW_JSL2
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source_av = (int)(int.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                for (i = 0; i < count; i++)
                {
                    P[0 + i * 23] = 0x02;    //STX
                    P[1 + i * 23] = 0x00;    //"0"
                    P[2 + i * 23] = 0x00;    //"0"
                    P[3 + i * 23] = 0x43;    //"C"; 
                    P[4 + i * 23] = 0x4D;    //"M" 
                    P[5 + i * 23] = 0x3A;    //":"
                    P[6 + i * 23] = 0x4D;    //"M"
                    P[7 + i * 23] = 0x53;    //"S"
                    P[8 + i * 23] = (byte)(0x30 + PanelCount[start + i] / 10);                                 //"10"
                    P[9 + i * 23] = (byte)(0x30 + (PanelCount[start + i] - 10 * (PanelCount[start + i] / 10)));    //"1"
                    P[10 + i * 23] = 0x03;   //ETX

                    P[11 + i * 23] = 0x02;    //STX
                    P[12 + i * 23] = 0x00;    //"0"
                    P[13 + i * 23] = 0x00;    //"0"
                    P[14 + i * 23] = 0x43;    //"C"; 
                    P[15 + i * 23] = 0x4D;    //"M" 
                    P[16 + i * 23] = 0x3A;    //":"
                    P[17 + i * 23] = 0x43;    //"C"
                    P[18 + i * 23] = 0x53;    //"S"
                    if (inputSingle.Contains("VIDEO"))
                    {
                        P[19 + i * 23] = (byte)(0x30 + ucInput_source_av / 100);                                 //"100"
                        P[20 + i * 23] = (byte)(0x30 + ((ucInput_source_av - 100 * (ucInput_source_av / 100)) / 10));    //"10"
                        P[21 + i * 23] = (byte)(0x30 + (ucInput_source_av - 10 * (ucInput_source_av / 10)));             //"1"
                    }
                    else
                    {
                        P[19 + i * 23] = (byte)(0x30 + ucInput_source / 100);                                 //"100"
                        P[20 + i * 23] = (byte)(0x30 + ((ucInput_source - 100 * (ucInput_source / 100)) / 10));    //"10"
                        P[21 + i * 23] = (byte)(0x30 + (ucInput_source - 10 * (ucInput_source / 10)));             //"1"
                    }
                    P[22 + i * 23] = 0x03;   //ETX
                }

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, count * 23);
                        else
                            serialPort1.Write(P, 0, count * 23);
                    }
                }
                catch
                {
                    if (Chinese_English)
                        MessageBox.Show("Serial error！", "Tips");
                    else
                        MessageBox.Show("串口出错！", "提示");
                    return;
                }
            }
             * */
            else if (uSwitchTemp == 10)    //SW_KTC06
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                P[0] = 0xF4;                //START
                P[1] = ucInput_source;      //CAM
                P[2] = 0x21;                //SW CMD
                P[3] = PanelCount[ukey, start];   //MON 
                if (ucDevice == "VGA")
                {
                    P[4] = (byte)(vgaAddress);          //device address NET
                }
                else if (ucDevice == "VIDEO")
                {
                    P[4] = (byte)(videoAddress);
                }
                else if (ucDevice == "DVI")
                {
                    P[4] = (byte)(dviAddress);
                }
                else if (ucDevice == "HDMI")
                {
                    P[4] = (byte)(hdmiAddress);
                }
                else if (ucDevice == "YPbPr")
                {
                    P[4] = (byte)(ypbprAddress);
                }

                P[5] = 0x00;                                //
                P[6] = (byte)(P[1] + P[2] + P[3] + P[4] + P[5]);   //CRC

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 7);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 7, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 7, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 7);
                        else
                            serialPort1.Write(P, 0, 7);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 7));
                        }));
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
            else if (uSwitchTemp == 11)    //SW_KTC07
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = Temp_In + "X";

                for (i = 0; i < count; i++)
                {
                    if (i == (count - 1))
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString() + ".";
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString() + "&";
                }

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
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
            else if (uSwitchTemp == 12)    //SW_KTC08
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                if (Temp_In.Length < 2)
                    Temp_In = "0" + Temp_In;

                strCmd_Out = "*";  //*为地址通配符 I O

                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + "I" + Temp_In;
                    if (PanelCount[ukey, i + start].ToString().Length < 2)
                    {
                        strCmd_Out = strCmd_Out + "O" + "0" + PanelCount[ukey, i + start].ToString(); //第一个为大写O, 第二个为零
                    }
                    else
                    {
                        strCmd_Out = strCmd_Out + "O" + PanelCount[ukey, i + start].ToString();
                    }
                    if (i == (count - 1))
                        strCmd_Out = strCmd_Out + "!";
                }

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
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
            else if (uSwitchTemp == 13)    //SW_KTC09
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                if (Temp_In.Length < 2)
                    strCmd_Out = "00" + Temp_In + "B";
                else if (Temp_In.Length < 3)
                    strCmd_Out = "0" + Temp_In + "B";
                else
                    strCmd_Out = Temp_In + "B";

                for (i = 0; i < count; i++)
                {
                    if (PanelCount[ukey, i + start] < 10)
                        strCmd_Out = strCmd_Out + "00" + PanelCount[ukey, i + start].ToString();
                    else if (PanelCount[ukey, i + start] < 100)
                        strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();

                    strCmd_Out = strCmd_Out + ".";
                }

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
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
            else if (uSwitchTemp == 14)    //SW_KTC10
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                P[0] = 0xF2;    //STX
                P[1] = 0x00;    //
                P[2] = 0x00;    //
                P[3] = 0x21;    //SW CMD
                P[4] = PanelCount[ukey, start];    //MON ID
                P[5] = 0x00;    //MASTER ADD
                P[6] = 0x00;    //PEOPLE ID
                P[7] = (byte)(P[1] ^ P[2] ^ P[3] ^ P[4] ^ P[5] ^ P[6]);    //CRC

                P[8] = 0xF2;        //STX
                P[9] = 0x00;        //CAM ADDH
                P[10] = 0x00;       //CAM ADDL
                P[11] = 0x22;       //SW CMD 
                P[12] = ucInput_source;    //CAM ID
                P[13] = 0x00;       //MASTER ADD
                P[14] = 0x00;       //PEOPLE ID
                P[15] = (byte)(P[9] ^ P[10] ^ P[11] ^ P[12] ^ P[13] ^ P[14]);       //CRC

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 16);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 16, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 16, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 16);
                        else
                            serialPort1.Write(P, 0, 16);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 16));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 15)    //SW_KTC11
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                P[0] = 0xF2;        //STX
                P[1] = 0x00;        //CAM ADDH
                P[2] = ucInput_source;       //CAM ADDL
                P[3] = 0x22;       //SW CMD 
                P[4] = PanelCount[ukey, start];    //MON ID
                if (ucDevice == "VGA")
                {
                    P[5] = (byte)(vgaAddress);          //device address NET
                }
                else if (ucDevice == "VIDEO")
                {
                    P[5] = (byte)(videoAddress);
                }
                else if (ucDevice == "DVI")
                {
                    P[5] = (byte)(dviAddress);
                }
                else if (ucDevice == "HDMI")
                {
                    P[5] = (byte)(hdmiAddress);
                }
                else if (ucDevice == "YPbPr")
                {
                    P[5] = (byte)(ypbprAddress);
                }
                P[6] = 0x00;       //PEOPLE ID
                P[7] = (byte)(P[1] ^ P[2] ^ P[3] ^ P[4] ^ P[5] ^ P[6]);       //CRC

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 8);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 8, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 8, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 8);
                        else
                            serialPort1.Write(P, 0, 8);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 8));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 16)    //SW_KTC12
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));


                P[0] = (byte)(0x30 + PanelCount[ukey, start] / 10);                                 //"10"
                P[1] = (byte)(0x30 + (PanelCount[ukey, start] - 10 * (PanelCount[ukey, start] / 10)));    //"1"
                P[2] = 0x4D;    //"M"
                P[3] = 0x61;    //"a"; 

                P[4] = (byte)(0x30 + ((ucInput_source - 100 * (ucInput_source / 100)) / 10));    //"10"
                P[51] = (byte)(0x30 + (ucInput_source - 10 * (ucInput_source / 10)));             //"1"
                P[6] = 0x23;    //"#"
                P[7] = 0x61;    //"a"

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 8);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 8, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 8, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 8);
                        else
                            serialPort1.Write(P, 0, 8);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 8));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 17)    //SW_KTC13
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = "[SWCH," + vgaAddress.ToString() + "," + Temp_In + ",1,";

                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                strCmd_Out = strCmd_Out + "]";

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 18)    //SW_KTC14
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = Temp_In + "V";

                for (i = 0; i < count; i++)
                {
                    if (i == (count - 1))
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString() + ".";
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString() + ",";
                }

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);

                }
            }
            else if (uSwitchTemp == 19)    //SW_KTC15
            {
                P[0] = 0xBA;
                P[2] = 0x01;            //cmd channel sw
                if (ucDevice == "VGA")
                {
                    P[1] = (byte)vgaAddress;          //device address
                    P[4] = 0xA0;
                }
                else if (ucDevice == "VIDEO")
                {
                    P[1] = (byte)videoAddress;
                    P[4] = 0xA1;        //sdi 0xa7
                }
                else if (ucDevice == "DVI")
                {
                    P[1] = (byte)dviAddress;
                    P[4] = 0xA5;
                }
                else if (ucDevice == "HDMI")
                {
                    P[1] = (byte)hdmiAddress;
                    P[4] = 0xA6;
                }
                else if (ucDevice == "YPbPr")
                {
                    P[1] = (byte)ypbprAddress;
                    P[4] = 0xAF;
                }
                P[3] = (byte)(2 + count * 2);     //CMD LENGTH

                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                Temp_Crc = P[0] + P[1] + P[2] + P[3] + P[4];
                for (i = 0; i < count; i++)
                {
                    P[5 + i * 2] = (byte)(PanelCount[ukey, start + i] - 1);
                    P[6 + i * 2] = (byte)(ucInput_source - 1);
                    Temp_Crc = Temp_Crc + P[5 + i * 2] + P[6 + i * 2];
                }
                P[5 + count * 2] = (byte)(Temp_Crc);    //CRC

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 6 + count * 2);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 6 + count * 2, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 6 + count * 2, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 6 + count * 2);
                        else
                            serialPort1.Write(P, 0, 6 + count * 2);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 6 + count * 2));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 20)    //SW_KTC16
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = "";

                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + Temp_In + ",";
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString() + ",";
                }
                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }
                strCmd_Out = strCmd_Out + Address_temp.ToString() + "," + "D" + "," + "V";

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 21)    //SW_JSL3 
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }

                for (i = 0; i < count; i++)
                {
                    P[0 + i * 18] = 0xF8;    //START
                    P[1 + i * 18] = (byte)(ucInput_source & 0xFF);              //LOW
                    P[2 + i * 18] = (byte)((ucInput_source >> 8) & 0xFF);       //HIGH
                    P[3 + i * 18] = 0x81;    //CMD 
                    P[4 + i * 18] = (byte)PanelCount[ukey, start + i];                //MONITOR 
                    P[5 + i * 18] = 0x00;    //Address
                    P[6 + i * 18] = 0x00;    //"0"
                    P[7 + i * 18] = (byte)(P[1 + i * 18] + P[2 + i * 18] + P[3 + i * 18] + P[4 + i * 18] + P[5 + i * 18] + P[6 + i * 18]);    //crc
                    P[8 + i * 18] = 0xFF;    //END

                    P[9 + i * 18] = 0xF8;    //START
                    P[10 + i * 18] = (byte)(ucInput_source & 0xFF);             //LOW
                    P[11 + i * 18] = (byte)((ucInput_source >> 8) & 0xFF);        //HIGH
                    P[12 + i * 18] = 0x80;   //CMD 
                    P[13 + i * 18] = (byte)PanelCount[ukey, start + i];               //MONITOR 
                    P[14 + i * 18] = 0x00;   //Address
                    P[15 + i * 18] = 0x00;   //"0"
                    P[16 + i * 18] = (byte)(P[10 + i * 18] + P[11 + i * 18] + P[12 + i * 18] + P[13 + i * 18] + P[14 + i * 18] + P[15 + i * 18]);    //crc
                    P[17 + i * 18] = 0xFF;   //END
                }

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, count * 18);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, count * 18, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, count * 18, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, count * 18);
                        else
                            serialPort1.Write(P, 0, count * 18);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 18 * count));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 22)    //SW_KTC17
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = "";
                if (Temp_In.Length < 2)
                    strCmd_Out = "0" + Temp_In + "V";
                else
                    strCmd_Out = Temp_In + "V";

                for (i = 0; i < count; i++)
                {
                    if (PanelCount[ukey, i + start] < 10)
                        strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                strCmd_Out = strCmd_Out + ".";

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 23)    //SW_KTC18 
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }

                for (i = 0; i < count; i++)
                {
                    P[0 + i * 16] = 0xFF;       //START
                    P[1 + i * 16] = (byte)(ucInput_source & 0xFF);              //LOW
                    P[2 + i * 16] = (byte)((ucInput_source >> 8) & 0xFF);       //HIGH
                    P[3 + i * 16] = 0x81;       //CMD 
                    P[4 + i * 16] = (byte)PanelCount[ukey, start + i];                //MONITOR 
                    P[5 + i * 16] = 0x00;       //Address
                    P[6 + i * 16] = 0x00;       //"0"
                    P[7 + i * 16] = (byte)(P[1 + i * 16] + P[2 + i * 16] + P[3 + i * 16] + P[4 + i * 16] + P[5 + i * 16] + P[6 + i * 16]);    //crc

                    P[8 + i * 16] = 0xFF;       //START
                    P[9 + i * 16] = (byte)(ucInput_source & 0xFF);             //LOW
                    P[10 + i * 16] = (byte)((ucInput_source >> 8) & 0xFF);      //HIGH
                    P[11 + i * 16] = 0x80;      //CMD 
                    P[12 + i * 16] = (byte)PanelCount[ukey, start + i];               //MONITOR 
                    P[13 + i * 16] = 0x00;      //Address
                    P[14 + i * 16] = 0x00;      //"0"
                    P[15 + i * 16] = (byte)(P[9 + i * 16] + P[10 + i * 16] + P[11 + i * 16] + P[12 + i * 16] + P[13 + i * 16] + P[14 + i * 16]);    //crc
                }

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, count * 16);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, count * 16, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, count * 16, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, count * 16);
                        else
                            serialPort1.Write(P, 0, count * 16);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 16 * count));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 24)    //SW_KTC19 
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }

                for (i = 0; i < count; i++)
                {
                    P[0 + i * 16] = 0xF4;       //START
                    P[1 + i * 16] = 0x00;       //DATA
                    P[2 + i * 16] = 0x20;       //CMD
                    P[3 + i * 16] = (byte)PanelCount[ukey, start + i];                //MONITOR 
                    P[4 + i * 16] = 0x00;       //Address
                    P[5 + i * 16] = 0x00;
                    P[6 + i * 16] = (byte)((P[1 + i * 16] + P[2 + i * 16] + P[3 + i * 16] + P[4 + i * 16] + P[5 + i * 16] + P[7 + i * 16]) ^ 0x4d);    //crc
                    P[7 + i * 16] = 0x00;       //

                    P[8 + i * 16] = 0xF4;       //START
                    P[9 + i * 16] = (byte)(ucInput_source & 0xFF);             //LOW
                    P[10 + i * 16] = 0x21;       //CMD
                    P[11 + i * 16] = (byte)PanelCount[ukey, start + i];              //MONITOR 
                    P[12 + i * 16] = 0x00;      //Address
                    P[13 + i * 16] = 0x00;
                    P[14 + i * 16] = (byte)((P[9 + i * 16] + P[10 + i * 16] + P[11 + i * 16] + P[12 + i * 16] + P[13 + i * 16] + P[15 + i * 16]) ^ 0x4d);    //crc
                    P[15 + i * 16] = (byte)((ucInput_source >> 8) & 0xFF);     //HIGH

                }

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, count * 16);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, count * 16, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, count * 16, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, count * 16);
                        else
                            serialPort1.Write(P, 0, count * 16);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 16 * count));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 25)    //SW_KTC20 
            {
                P[0] = 0x96;
                if (ucDevice == "VGA")
                    P[1] = (byte)vgaAddress;
                else if (ucDevice == "VIDEO")
                    P[1] = (byte)videoAddress;
                else if (ucDevice == "DVI")
                    P[1] = (byte)dviAddress;
                else if (ucDevice == "HDMI")
                    P[1] = (byte)hdmiAddress;
                else if (ucDevice == "YPbPr")
                    P[1] = (byte)ypbprAddress;

                P[2] = 0x01;    //cmd
                P[3] = (byte)(2 + count * 2);
                P[4] = 0xA0;    //mode

                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)) - 1);
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)) - 1);
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)) - 1);

                for (i = 0; i < count; i++)
                {
                    P[5 + i * 2] = (byte)(PanelCount[ukey, start + i] - 1);
                    P[5 + i * 2 + 1] = ucInput_source;
                }
                P[5 + count * 2] = 0x00;    //CRC

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 6 + count * 2);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 6 + count * 2, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 6 + count * 2, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 6 + count * 2);
                        else
                            serialPort1.Write(P, 0, 6 + count * 2);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 6 + count * 2));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 26)    //SW_KTC21  FF＿09 _ _ AA
            {
                P[0] = 0xFF;    //start
                if (ucDevice == "VGA")
                {
                    P[1] = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    P[1] = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    P[1] = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    P[1] = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    P[1] = (byte)ypbprAddress;
                }
                P[2] = 0x09;        //SWITCH CMD

                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                P[3] = ucInput_source;
                for (i = 0; i < count; i++)
                {
                    P[4 + i] = (byte)(PanelCount[ukey, start + i]);
                }
                P[5] = 0xAA;    //end

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 6);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 6, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 6, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, 6);
                        else
                            serialPort1.Write(P, 0, 6);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 6));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 27)    //SW_KTC22
            {
                strCmd_Out = "!";
                if (ucDevice == "VGA")
                    strCmd_Out = strCmd_Out + vgaAddress.ToString() + ":G";
                else if (ucDevice == "VIDEO")
                    strCmd_Out = strCmd_Out + videoAddress.ToString() + ":V";
                else if (ucDevice == "DVI")
                    strCmd_Out = strCmd_Out + dviAddress.ToString() + ":L";
                else if (ucDevice == "HDMI")
                    strCmd_Out = strCmd_Out + hdmiAddress.ToString() + ":L";
                else if (ucDevice == "YPbPr")
                    strCmd_Out = strCmd_Out + ypbprAddress.ToString() + ":L";

                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = strCmd_Out + Temp_In.ToString() + "*";
                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                strCmd_Out = strCmd_Out + "~\r";

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 28)    //SW_KTC23
            {
                strCmd_Out = "*";

                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);
                if (inputSingle.Contains("VGA"))
                {
                    if (uVga < 100)
                    {
                        if (Temp_In.Length == 1)
                            Temp_In = "0" + Temp_In;
                    }
                    else
                    {
                        if (Temp_In.Length == 1)
                            Temp_In = "00" + Temp_In;
                        else if (Temp_In.Length == 2)
                            Temp_In = "0" + Temp_In;
                    }
                }
                else if (inputSingle.Contains("VIDEO"))
                {
                    if (uVideo < 100)
                    {
                        if (Temp_In.Length == 1)
                            Temp_In = "0" + Temp_In;
                    }
                    else
                    {
                        if (Temp_In.Length == 1)
                            Temp_In = "00" + Temp_In;
                        else if (Temp_In.Length == 2)
                            Temp_In = "0" + Temp_In;
                    }
                }

                strCmd_Out = strCmd_Out + Temp_In + "N";

                for (i = 0; i < count; i++)
                {
                    if (inputSingle.Contains("VGA"))
                    {
                        if (uVga < 100)
                        {
                            if (PanelCount[ukey, i + start] < 10)
                                strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                            else
                                strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                        }
                        else
                        {
                            if (PanelCount[ukey, i + start] < 10)
                                strCmd_Out = strCmd_Out + "00" + PanelCount[ukey, i + start].ToString();
                            else if (PanelCount[ukey, i + start] < 100)
                                strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                            else
                                strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                        }
                    }
                    else if (inputSingle.Contains("VIDEO"))
                    {
                        if (uVideo < 100)
                        {
                            if (PanelCount[ukey, i + start] < 10)
                                strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                            else
                                strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                        }
                        else
                        {
                            if (PanelCount[ukey, i + start] < 10)
                                strCmd_Out = strCmd_Out + "00" + PanelCount[ukey, i + start].ToString();
                            else if (PanelCount[ukey, i + start] < 100)
                                strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                            else
                                strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                        }
                    }

                }
                strCmd_Out = strCmd_Out + "#";

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 29)    //SW_KTC25
            {
                strCmd_Out = "";

                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                if (Temp_In.Length == 1)
                    Temp_In = "0" + Temp_In;

                for (i = 0; i < count; i++)
                {
                    if (inputSingle.Contains("VGA"))
                        strCmd_Out = strCmd_Out + uNumberToString((byte)vgaAddress);
                    else if (inputSingle.Contains("VIDEO"))
                        strCmd_Out = strCmd_Out + uNumberToString((byte)videoAddress);
                    else if (inputSingle.Contains("DVI"))
                        strCmd_Out = strCmd_Out + uNumberToString((byte)dviAddress);
                    else if (inputSingle.Contains("HDMI"))
                        strCmd_Out = strCmd_Out + uNumberToString((byte)hdmiAddress);
                    else if (inputSingle.Contains("YPbPr"))
                        strCmd_Out = strCmd_Out + uNumberToString((byte)ypbprAddress);

                    if (PanelCount[ukey, i + start] < 10)
                        strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString() + "<";
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString() + "<";

                    strCmd_Out = strCmd_Out + Temp_In + "!";

                }

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 30)    //SW_KTC26
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }

                for (i = 0; i < count; i++)
                {
                    P[0] = 0xBB;       //START
                    P[1] = 0x01;       //CMD 
                    P[2] = (byte)PanelCount[ukey, start + i];                //MONITOR 
                    P[3] = (byte)(ucInput_source & 0xFF);              //LOW
                    P[4] = (byte)((ucInput_source >> 8) & 0xFF);       //HIGH
                    P[5] = 0x00;       //"0"
                    P[6] = Address_temp;       //Address
                    P[7] = (byte)(P[0] + P[1] + P[2] + P[3] + P[4] + P[5] + P[6]);    //crc

                }

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, count * 8);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, count * 8, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, count * 8, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, count * 8);
                        else
                            serialPort1.Write(P, 0, count * 8);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 8 * count));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 31)    //SW_KTC27
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }

                for (i = 0; i < count; i++)
                {
                    P[0] = 0xBB;       //START
                    P[1] = 0x04;
                    P[2] = 0x00;
                    P[3] = 0x02;       //CMD 
                    P[4] = (byte)(ucInput_source & 0xFF);              //LOW
                    P[5] = (byte)PanelCount[ukey, start + i];                //MONITOR 
                    P[6] = Address_temp;       //Address
                }

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, count * 7);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, count * 7, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, count * 7, 100, 2);
                        }
                        /*
                        if (uMultiComPort == 2)
                            serialPort2.Write(P, 0, count * 7);
                        else
                            serialPort1.Write(P, 0, count * 7);
                         */
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 7 * count));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            /*
        else if (uSwitchTemp == 32)    //SW_KTC28
        {
            string[,] InOutTable = new string[4, 4] 
            {  
                {
                    "09","1D","1F","0D"
                },
                {
                    "17","12","59","08"
                },
                {
                    "5E","06","05","03"
                },
                {
                    "18","44","0F","51"
                }
            };
            strCmd_Out = "a";

            if (inputSingle.Contains("VGA"))
                Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
            else if (inputSingle.Contains("VIDEO"))
                Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
            else if (inputSingle.Contains("DVI"))
                Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
            else if (inputSingle.Contains("HDMI"))
                Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
            else if (inputSingle.Contains("YPbPr"))
                Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

            for (i = 0; i < count; i++)
            {
                strCmd_Out = strCmd_Out + InOutTable[PanelCount[i + start] - 1, int.Parse(Temp_In) - 1];
            }

            try
            {
                if (Rs232Con)
                {
                    if (uMultiComPort == 2)
                        serialPort2.Write(strCmd_Out);
                    else
                        serialPort1.Write(strCmd_Out);
                }
            }
            catch
            {
                if (Chinese_English)
                    MessageBox.Show("    Serial error！", "Tips");
                else
                    MessageBox.Show("     串口出错！", "提示");
            }
        }
             */
            else if (uSwitchTemp == 32)    //SW_KTC29
            {
                if (inputSingle.Contains("VGA"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("VIDEO"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)));
                else if (inputSingle.Contains("DVI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)));
                else if (inputSingle.Contains("HDMI"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)));
                else if (inputSingle.Contains("YPbPr"))
                    ucInput_source = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)));

                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }

                for (i = 0; i < count; i++)
                {
                    P[0] = 0xA5;       //START
                    P[1] = 0x00;       //SOURCE 
                    P[2] = Address_temp;        //Address
                    P[3] = 0x01;                //cmd
                    P[4] = (byte)PanelCount[ukey, start + i];       //output
                    P[5] = ucInput_source;      //input
                    P[6] = 0x01;       //mode v & a
                    P[7] = (byte)(P[0] + P[1] + P[2] + P[3] + P[4] + P[5] + P[6]);    //crc

                    try
                    {
                        if (Rs232Con)
                        {
                            if (TCPCOM)
                            {
                                //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                                TcpSendMessage(P, 0, count * 8);
                            }
                            else
                            {
                                if (uMultiComPort == 2)
                                    SerialPortUtil.serialPortSendData(serialPort2, P, 0, count * 8, 100, 2);
                                else
                                    SerialPortUtil.serialPortSendData(serialPort1, P, 0, count * 8, 100, 2);
                            }
                            /*
                            if (uMultiComPort == 2)
                                serialPort2.Write(P, 0, count * 8);
                            else
                                serialPort1.Write(P, 0, count * 8);
                             */
                            this.Invoke(new MethodInvoker(delegate()
                            {
                                richTextBox2.AppendText(ToHexString(P, 8 * count));
                            }));
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
            else if (uSwitchTemp == 33)    //SW_KTC30
            {
                //ID 00
                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }

                if (Address_temp < 10)
                    strCmd_Out = "0" + Address_temp.ToString() + "V";
                else
                    strCmd_Out = Address_temp.ToString() + "V";

                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                if (Temp_In.Length == 1)
                    strCmd_Out = strCmd_Out + "0" + Temp_In + "M";
                else
                    strCmd_Out = strCmd_Out + Temp_In + "M";

                for (i = 0; i < count; i++)
                {
                    if (PanelCount[ukey, i + start] < 10)
                        strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 34)    //SW_KTC31
            {
                //ID 00
                strCmd_Out = "^SWI";

                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                if (Temp_In.Length == 1)
                    strCmd_Out = strCmd_Out + "00" + Temp_In;
                else if (Temp_In.Length == 2)
                    strCmd_Out = strCmd_Out + "0" + Temp_In;
                else
                    strCmd_Out = strCmd_Out + Temp_In;

                for (i = 0; i < count; i++)
                {
                    if (PanelCount[ukey, i + start] < 10)
                        strCmd_Out = strCmd_Out + "00" + PanelCount[ukey, i + start].ToString();
                    else if (PanelCount[ukey, i + start] < 100)
                        strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                strCmd_Out = strCmd_Out + "$";

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 35)    //SW_KTC32
            {
                strCmd_Out = "";

                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = strCmd_Out + Temp_In + ",";

                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();

                    strCmd_Out = strCmd_Out + ",Y";
                }
                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 36)    //SW_KTC33   1 x ?,?,?.
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = Temp_In + "X";
                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                strCmd_Out = strCmd_Out + ".";

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 37)    //SW_KTC34
            {
                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                strCmd_Out = "*" + Temp_In + "D";
                for (i = 0; i < count; i++)
                {
                    strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                strCmd_Out = strCmd_Out + "#";

                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else if (uSwitchTemp == 38)    //SW_KTC30
            {
                //ID 00
                if (ucDevice == "VGA")
                {
                    Address_temp = (byte)vgaAddress;          //device address
                }
                else if (ucDevice == "VIDEO")
                {
                    Address_temp = (byte)videoAddress;
                }
                else if (ucDevice == "DVI")
                {
                    Address_temp = (byte)dviAddress;
                }
                else if (ucDevice == "HDMI")
                {
                    Address_temp = (byte)hdmiAddress;
                }
                else if (ucDevice == "YPbPr")
                {
                    Address_temp = (byte)ypbprAddress;
                }

                if (Address_temp < 10)
                    strCmd_Out = "0" + Address_temp.ToString() + "V";
                else
                    strCmd_Out = Address_temp.ToString() + "V";

                if (inputSingle.Contains("VGA"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("VIDEO"))
                    Temp_In = inputSingle.Substring(2, inputSingle.Length - 4);
                else if (inputSingle.Contains("DVI"))
                    Temp_In = inputSingle.Substring(3, inputSingle.Length - 3);
                else if (inputSingle.Contains("HDMI"))
                    Temp_In = inputSingle.Substring(4, inputSingle.Length - 4);
                else if (inputSingle.Contains("YPbPr"))
                    Temp_In = inputSingle.Substring(5, inputSingle.Length - 5);

                if (Temp_In.Length == 1)
                    strCmd_Out = strCmd_Out + "0" + Temp_In + "V";
                else
                    strCmd_Out = strCmd_Out + Temp_In + "V";

                for (i = 0; i < count; i++)
                {
                    if (PanelCount[ukey, i + start] < 10)
                        strCmd_Out = strCmd_Out + "0" + PanelCount[ukey, i + start].ToString();
                    else
                        strCmd_Out = strCmd_Out + PanelCount[ukey, i + start].ToString();
                }
                try
                {
                    if (Rs232Con)
                    {
                        if (uMultiComPort == 2)
                            SerialPortUtil.serialPortSendStr(serialPort2, strCmd_Out, 100, 2);
                        else
                            SerialPortUtil.serialPortSendStr(serialPort1, strCmd_Out, 100, 2);
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(strCmd_Out + "\n");
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }

            }
            else if (uSwitchTemp == 39)    //SW_YK
            {
                P[0] = (byte)(0xE0 + 6 + count);
                if (ucDevice == "VGA")
                    P[1] = (byte)vgaAddress;
                else if (ucDevice == "VIDEO")
                    P[1] = (byte)videoAddress;
                else if (ucDevice == "DVI")
                    P[1] = (byte)dviAddress;
                P[2] = 0x20;
                P[3] = 0x00;    //uart cmd
                if (inputSingle.Contains("VGA"))
                    P[4] = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                else if (inputSingle.Contains("VIDEO"))
                    P[4] = (byte)(byte.Parse(inputSingle.Substring(2, inputSingle.Length - 4)) - 1);
                else if (inputSingle.Contains("DVI"))
                    P[4] = (byte)(byte.Parse(inputSingle.Substring(3, inputSingle.Length - 3)) - 1);
                else if (inputSingle.Contains("HDMI"))
                    P[4] = (byte)(byte.Parse(inputSingle.Substring(4, inputSingle.Length - 4)) - 1);
                else if (inputSingle.Contains("YPbPr"))
                    P[4] = (byte)(byte.Parse(inputSingle.Substring(5, inputSingle.Length - 5)) - 1);

                Temp_Crc = P[0] + P[1] + P[2] + P[3] + P[4];
                for (i = 0; i < count; i++)
                {
                    P[5 + i] = (byte)(PanelCount[ukey, start + i] - 1);
                    Temp_Crc = Temp_Crc + P[5 + i];
                }
                P[5 + count] = (byte)(255 - (byte)Temp_Crc);    //CRC

                try
                {
                    if (Rs232Con)
                    {
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(P, 0, 6 + count);
                        }
                        else
                        {
                            if (uMultiComPort == 2)
                                SerialPortUtil.serialPortSendData(serialPort2, P, 0, 6 + count, 100, 2);
                            else
                                SerialPortUtil.serialPortSendData(serialPort1, P, 0, 6 + count, 100, 2);
                        }
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            richTextBox2.AppendText(ToHexString(P, 6 + count));
                        }));
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            else
            {
                string str = "";
                if (ucDevice == "DVI")
                    str = dviMatrix + "-dvi";
                else if (ucDevice == "HDMI")
                    str = hdmiMatrix + "-hdmi";
                else if (ucDevice == "VGA")
                    str = vgaMatrix + "-vga";
                else if (ucDevice == "VIDEO")
                    str = videoMatrix + "-video";
                if(str.Contains("\0"))
                    str = str.Replace("\0","");
                //Console.WriteLine(str);
                try
                {
                    DataTable tables = AccessFunction.GetClientProtocol(str);
                    if (tables != null && tables.Rows.Count > 0)
                    {
                        string s_Cmd = "";
                        string id = "";
                        string input = "";
                        string output = "";
                        bool CRC = false;
                        string[] arr = new string[9];
                        if (tables != null)
                        {
                            if (tables.Rows[0][13].ToString() == "1")
                            {
                                int Count = tables.Columns.Count;
                                for (int t = 3; t < 12; t++)
                                {
                                    string a = tables.Rows[0][t].ToString();
                                    if (a == "#id")
                                    {
                                        if (ucDevice == "VGA")
                                            a = vgaAddress.ToString();
                                        else if (ucDevice == "VIDEO")
                                            a = videoAddress.ToString();
                                        else if (ucDevice == "DVI")
                                            a = dviAddress.ToString();
                                        else if (ucDevice == "HDMI")
                                            a = hdmiAddress.ToString();
                                        id = a;
                                        if (tables.Rows[0][12].ToString() == "20")
                                            if (a.Length == 1)
                                                a = "0" + a;
                                    }
                                    if (a == "#in")
                                    {
                                        if (inputSingle.Contains("VGA"))
                                            a = inputSingle.Substring(3, inputSingle.Length - 3);
                                        else if (inputSingle.Contains("VIDEO"))
                                            a = inputSingle.Substring(2, inputSingle.Length - 4);
                                        else if (inputSingle.Contains("DVI"))
                                            a = inputSingle.Substring(3, inputSingle.Length - 3);
                                        else if (inputSingle.Contains("HDMI"))
                                            a = inputSingle.Substring(4, inputSingle.Length - 4);
                                        input = a;
                                        if (tables.Rows[0][12].ToString() == "20")
                                            if (a.Length == 1)
                                                a = "0" + a;
                                    }
                                    if (a != "")
                                        s_Cmd += a;
                                }
                                //Console.WriteLine(s_Cmd);
                                for (i = 0; i < count; i++)
                                {
                                    string s2_Cmd = s_Cmd;
                                    if (s2_Cmd.Contains("#out"))
                                    {
                                        if (tables.Rows[0][12].ToString() == "20")
                                        {
                                            if (PanelCount[ukey, i + start] < 10)
                                                s2_Cmd = s2_Cmd.Replace("#out", "0" + PanelCount[ukey, i + start].ToString());
                                            else
                                                s2_Cmd = s2_Cmd.Replace("#out", PanelCount[ukey, i + start].ToString());
                                        }
                                        else
                                            s2_Cmd = s_Cmd.Replace("#out", PanelCount[ukey, i + start].ToString());
                                        if (CRC && output != "")
                                        {
                                            if (output.Contains("T"))
                                            {
                                                output = output.Replace("T", PanelCount[ukey, i + start].ToString());

                                                MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControlClass();
                                                sc.Language = "JavaScript";
                                                string formulate1 = output;//string.Format("if({0}==2)((({0}*{1})-{3}+({1}*{2}))+{4}/{0});else(1+2*3)", 2, 3, 4, 5, 6);
                                                object objResult = sc.Eval(formulate1);
                                                if (objResult != null)
                                                {
                                                    //Response.Write(objResult.ToString());//1+12+3 
                                                    output = objResult.ToString();
                                                    s2_Cmd += output;
                                                }
                                            }
                                            //output = EvalExpress(output).ToString();
                                        }
                                    }
                                    try
                                    {
                                        if (Rs232Con)
                                        {
                                            if (uMultiComPort == 2)
                                                SerialPortUtil.serialPortSendStr(serialPort2, s2_Cmd, 100, 2);
                                            else
                                                SerialPortUtil.serialPortSendStr(serialPort1, s2_Cmd, 100, 2);
                                            this.Invoke(new MethodInvoker(delegate()
                                            {
                                                richTextBox2.AppendText(s2_Cmd + "\n");
                                            }));
                                        }
                                    }
                                    catch
                                    {
                                        string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                                        MessageBox.Show(ts, tp);
                                        return;
                                    }
                                    //Console.WriteLine(s2_Cmd);
                                }
                            }
                            else if (tables.Rows[0][13].ToString() == "2")
                            {
                                int num = 0;
                                for (int t = 3, k = 0; t < 12; t++, k++)
                                {
                                    string a = tables.Rows[0][t].ToString();
                                    if (a == "#id")
                                    {
                                        if (ucDevice == "VGA")
                                            a = vgaAddress.ToString();
                                        else if (ucDevice == "VIDEO")
                                            a = videoAddress.ToString();
                                        else if (ucDevice == "DVI")
                                            a = dviAddress.ToString();
                                        else if (ucDevice == "HDMI")
                                            a = hdmiAddress.ToString();
                                        id = a;
                                        if (tables.Rows[0][12].ToString() == "20")
                                            if (a.Length == 1)
                                                a = "0" + a;
                                    }
                                    else if (a == "#in")
                                    {
                                        if (inputSingle.Contains("VGA"))
                                            a = inputSingle.Substring(3, inputSingle.Length - 3);
                                        else if (inputSingle.Contains("VIDEO"))
                                            a = inputSingle.Substring(2, inputSingle.Length - 4);
                                        else if (inputSingle.Contains("DVI"))
                                            a = inputSingle.Substring(3, inputSingle.Length - 3);
                                        else if (inputSingle.Contains("HDMI"))
                                            a = inputSingle.Substring(4, inputSingle.Length - 4);
                                        input = a;
                                        if (tables.Rows[0][12].ToString() == "20")
                                            if (a.Length == 1)
                                                a = "0" + a;
                                    }
                                    else if (a.Contains("+") || a.Contains("-") || a.Contains("*") || a.Contains("/"))
                                    {
                                        CRC = true;
                                        if (a.Contains("#id"))
                                            a = a.Replace("#id", id);
                                        if (a.Contains("#in"))
                                            a = a.Replace("#in", input);
                                        if (a.Contains("#out"))
                                            a = a.Replace("#out", "T");
                                        output = a;
                                    }
                                    if (a != "")
                                    {
                                        arr[k] = a;
                                        num++;
                                    }
                                }
                                //byte[] bt = Convert.FromBase64String(s_Cmd);
                                //Console.WriteLine(arr.Length);
                                //string[] pro = new string[9];
                                byte[] b;
                                int m = 0;
                                for (int k = 0; k < count; k++)
                                {
                                    for (i = 0, m = 0; i < num; i++)
                                    {
                                        if (arr[i] != "")
                                        {
                                            if (arr[i].Contains("#out"))
                                            {
                                                if (tables.Rows[0][12].ToString() == "20")
                                                {
                                                    if (PanelCount[ukey, i + start] < 10)
                                                        arr[i] = "0" + PanelCount[ukey, k + start].ToString();
                                                    else
                                                        arr[i] = PanelCount[ukey, k + start].ToString();
                                                }
                                                else
                                                    arr[i] = PanelCount[ukey, k + start].ToString();
                                            }
                                            m++;
                                            //Console.WriteLine(arr[i]);
                                        }
                                        if (CRC && arr[i].Contains("T"))
                                        {
                                            if (output.Contains("T"))
                                            {
                                                output = output.Replace("T", PanelCount[ukey, start].ToString());

                                                MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControlClass();
                                                sc.Language = "JavaScript";
                                                string formulate1 = output;//string.Format("if({0}==2)((({0}*{1})-{3}+({1}*{2}))+{4}/{0});else(1+2*3)", 2, 3, 4, 5, 6);
                                                object objResult = sc.Eval(formulate1);
                                                if (objResult != null)
                                                {
                                                    //Console.WriteLine(objResult.ToString());//1+12+3 
                                                    output = objResult.ToString();
                                                    output = Convert.ToString(int.Parse(output), 16).ToUpper();
                                                    arr[i] = output;
                                                }
                                            }
                                        }
                                    }
                                    //Console.WriteLine(arr[m] + "," + tem);
                                    b = new byte[m];
                                    for (i = 0; i < m; i++)
                                    {
                                        try
                                        {
                                            if (arr[i] != "")
                                                b[i] = System.Convert.ToByte(arr[i], 16);
                                            //Console.WriteLine(b[i]);
                                        }
                                        catch (Exception e)
                                        {
                                            MessageBox.Show(e.ToString());
                                        }
                                    }
                                    try
                                    {
                                        if (Rs232Con)
                                        {
                                            if (TCPCOM)
                                            {
                                                //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                                                TcpSendMessage(b, 0, b.Length);
                                            }
                                            else
                                            {
                                                if (uMultiComPort == 2)
                                                    SerialPortUtil.serialPortSendData(serialPort2, b, 0, b.Length, 100, 2);
                                                else
                                                    SerialPortUtil.serialPortSendData(serialPort1, b, 0, b.Length, 100, 2);
                                            }
                                            this.Invoke(new MethodInvoker(delegate()
                                        {
                                            richTextBox2.AppendText(ToHexString(b, b.Length));
                                        }));
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
                            }
                        }
                    }
                }
                catch
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M11", "添加的矩阵指令不正确！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
            }
        }


        private string uNumberToString(byte uAddress)
        {
            switch (uAddress)
            {
                case 1:
                    return "A";
                case 2:
                    return "B";
                case 3:
                    return "C";
                case 4:
                    return "D";
                case 5:
                    return "E";
                case 6:
                    return "F";
                case 7:
                    return "G";
                case 8:
                    return "H";
                case 9:
                    return "I";
                case 10:
                    return "J";
                case 11:
                    return "K";
                case 12:
                    return "L";
                case 13:
                    return "M";
                case 14:
                    return "N";
                case 15:
                    return "O";
                case 16:
                    return "P";
                case 17:
                    return "Q";
                case 18:
                    return "R";
                case 19:
                    return "S";
                case 20:
                    return "T";
                case 21:
                    return "U";
                case 22:
                    return "V";
                case 23:
                    return "W";
                case 24:
                    return "X";
                case 25:
                    return "Y";
                case 26:
                    return "Z";
                default:
                    return "*";

            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Skin.AddSkinMenu(contextMenuStrip2);
            //skinEngine1.SkinFile = Application.StartupPath + @"/DeepCyan.ssk";
            /*
            if (Motherboard_flag == 4)
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                //rk.DeleteValue("Wall_C", true);
                if (rk.GetValue("Wall_C") != null)//读取系统的注册码，如果已注册则值值不为空可以使用，并标识已注册，反之则未注册
                {
                    //toolStripStatusLabel1.Text += " (已注册)";
                    //button19.Enabled = false;
                    button19.Visible = false;
                    return;
                }
                if (Chinese_English == 1)
                    this.Text += " (Trial)";
                else
                    this.Text += " (试用版)";
                //button19.Enabled = true;
                //rk.SetValue("Count", 0);
                int count = (int)rk.GetValue("Count", 0);//未注册时获取用户试用的次数，如果是首次使用则赋值为0
                if (Chinese_English == 1)
                    MessageBox.Show("Thank you for using " + count + " times, please register and use again！", "Tips", MessageBoxButtons.OK);
                else
                    MessageBox.Show("感谢你已经使用了" + count + "次，请先注册再使用！", "提示");
                //tabc.Visible = false;//因为是使用版，所以有很多 功能无法使用，这里演示将tabControl的visible属性设置为false来演示这种效果
                if (count < 30)//默认试用次数为30次，小于30次时仍可试用，反之则直接结束运行
                {
                    rk.SetValue("Count", count + 1);//需要更新注册表中用来记录登录次数的值，每试用一次，该值就增加一，等大于等于30时，程序试用结束，直接退出
                    count = 30 - count;
                    if (Chinese_English == 1)
                        MessageBox.Show("You are using a trial version! There are " + count + " times trial opportunities, please register before use！", "Tips", MessageBoxButtons.OK);
                    else
                        MessageBox.Show("您使用的是试用版！还有" + count + "次试用机会，请先注册再使用！", "提示");
                }
                else
                {
                    DialogResult t;
                    if (Chinese_English == 1)
                        t = MessageBox.Show("I'm sorry, you have exceeded the number of trials! Whether to register and use？", "Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    else
                        t = MessageBox.Show("不好意思，您已经超出试用次数！是否进行注册再使用？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    //Application.Exit();
                    if (t == DialogResult.Yes || t == DialogResult.OK)
                    {
                        Form_Reg f = new Form_Reg(this);
                        f.ShowDialog();
                    }
                    System.Environment.Exit(0);
                }
            }
            else
             */ 
                button19.Visible = false;
            //asc.controllInitializeSize(this);  
            for (double d = 0.01; d < 1; d += 0.05)
            {
                System.Threading.Thread.Sleep(10);
                Application.DoEvents();
                this.Opacity = d;
                this.Refresh();
            }
            this.Opacity = 1.0;
            LogHelper.WriteLog("=====启动正常软件=====");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult;
            string ts = languageFile.ReadString("MESSAGEBOX", "MS", "您确定要关闭程序吗?");
            string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
            dialogResult = MessageBox.Show(ts, tp, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            /*
            if(Chinese_English)
                dialogResult = MessageBox.Show("Are you sure you want to close the program?", "Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            else
                dialogResult = MessageBox.Show("   您确定要关闭程序吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
             */ 
            if (dialogResult == DialogResult.OK || dialogResult == DialogResult.Yes)
            {
                if (systemRunning)//看系统是否在忙碌中
                {
                    stopProgress();
                    myThread.Abort();
                }
                //Application.Exit();
                saveSceneFile(currentConnectionName + "\\scene.rgf");
                settingFile.WriteString("SETTING", "Pwd", "0");
                settingFile.WriteInteger("SCENE", "INDEX", comboBoxEx1.SelectedIndex);
                //Console.WriteLine(Chinese_English);
                if(Chinese_English == 0)
                    settingFile.WriteString("SETTING", "CH-US", "0");
                else if (Chinese_English == 1)
                    settingFile.WriteString("SETTING", "CH-US", "1");
                else if (Chinese_English == 2)
                    settingFile.WriteString("SETTING", "CH-US", "2");
                else
                    settingFile.WriteString("SETTING", "CH-US", "0");
                Thread.Sleep(200);
                AccessFunction.AccessCloseAll();
                LogHelper.WriteLog("=====关闭程序、退出=====\r\n============================================");
                timer_Off.Dispose();
                timer_On.Dispose();
                timer1.Dispose();
                if (Rs232Con)
                {
                    if (TCPCOM)
                    {
                        TCPServer.Stop();
                        TCPClient.DisConnect();
                    }
                    serialPort1.Close();
                    Rs232Con = false;
                }
                this.Dispose();
                System.Environment.Exit(0);
                //Application.Exit();
            }
            else if (dialogResult == DialogResult.Cancel || dialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Power_off(byte A1)//单个屏幕的关机指令
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = A1;
            array[2] = 0x20;
            array[3] = 0xAE;
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            try
            {
                if (Rs232Con)
                {
                    //serialPort1.Write(array, 0, 5);
                    //serialPort1.Write(array, 0, 5);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, 5);
                    }
                    else
                    {
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 200, 3);
                    }
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        richTextBox2.AppendText(ToHexString(array, 5));
                    }));
                }
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }

        private void Power_on(byte A1)//单个屏幕的开机指令
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[1] = A1;
            array[2] = 0x20;
            array[3] = 0xAD;
            array[4] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3]));
            try
            {
                if (Rs232Con)
                {
                    //serialPort1.Write(array, 0, 5);
                    //serialPort1.Write(array, 0, 5);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, 5);
                    }
                    else
                    {
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 200, 3);
                    }
                    //richTextBox2.AppendText(ToHexString(array, 5));
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        richTextBox2.AppendText(ToHexString(array, 5));
                    }));
                }
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }

        private void 开机ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            for (int j = rowStar; j <= rowEnd; j++)
            {
                for (int i = colStar; i <= colEnd; i++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    //Thread.Sleep(Delay_time);
                    Power_on((byte)num);
                    //Console.WriteLine("select_address[num] = " + num);
                }
            }
        }


        private void MainForm_Activated(object sender, EventArgs e)
        {
            richTextBox2.SelectionStart = richTextBox2.Text.Length;
            richTextBox2.ScrollToCaret();
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox3.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox5.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox7.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel2.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel3.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel4.Controls)
            {
                if (!ctl.Name.Equals("richTextBox2"))
                    resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel5.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel6.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel7.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel8.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel9.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            } 
            foreach (Control ctl in panel10.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel11.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel19.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            if (!PN)
            {
                foreach (Control ctl in panel20.Controls)
                {
                    resources.ApplyResources(ctl, ctl.Name);
                }
            }
            foreach (Control ctl in panel21.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in panel22.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabControl1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabControl2.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabControl3.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage2.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage3.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage4.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage5.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage6.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage7.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage8.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage9.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage10.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in tabPage14.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox8.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in expandablePanel1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in expandablePanel2.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            resources.ApplyResources(expandablePanel1, expandablePanel1.Name);
            resources.ApplyResources(expandablePanel2, expandablePanel2.Name);
            resources.ApplyResources(mergeToolStripMenuItem, mergeToolStripMenuItem.Name);
            resources.ApplyResources(unMergeToolStripMenuItem, unMergeToolStripMenuItem.Name);
            resources.ApplyResources(开屏电源ToolStripMenuItem, 开屏电源ToolStripMenuItem.Name);
            resources.ApplyResources(关屏电源ToolStripMenuItem, 关屏电源ToolStripMenuItem.Name);
            resources.ApplyResources(本地通道ToolStripMenuItem, 本地通道ToolStripMenuItem.Name);
            resources.ApplyResources(screenNumberToolStripMenuItem, screenNumberToolStripMenuItem.Name);
            resources.ApplyResources(屏幕参数调整ToolStripMenuItem, 屏幕参数调整ToolStripMenuItem.Name);
            resources.ApplyResources(toolStripMenuItem3, toolStripMenuItem3.Name);
            resources.ApplyResources(toolStripMenuItem4, toolStripMenuItem4.Name);
            resources.ApplyResources(toolStripMenuItem5, toolStripMenuItem5.Name);
            resources.ApplyResources(开机ToolStripMenuItem, 开机ToolStripMenuItem.Name);
            resources.ApplyResources(c复制ToolStripMenuItem, c复制ToolStripMenuItem.Name);
            resources.ApplyResources(v粘贴ToolStripMenuItem, v粘贴ToolStripMenuItem.Name);
            resources.ApplyResources(button13, button13.Name);
            resources.ApplyResources(button10, button10.Name);
            resources.ApplyResources(tabPage1, tabPage1.Name);
            resources.ApplyResources(tabPage3, tabPage3.Name);
            resources.ApplyResources(toolStripStatusLabel2, toolStripStatusLabel2.Name);
            resources.ApplyResources(toolStripStatusLabel3, toolStripStatusLabel3.Name);
            resources.ApplyResources(toolStripStatusLabel4, toolStripStatusLabel4.Name);
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");

            if (checkBox2.Checked)
            {
                label20.Enabled = true;
                comboBox2.Enabled = true;
            }
            string s = settingFile.ReadString("SETTING", "PicturePath", "");
            //Console.WriteLine("sss=====" + s);
            if (!s.Equals(""))
            {
                s = s.Substring(s.Length - 4, 4);
                s = Application.StartupPath + @"\pic\logo" + s;
                pictureBox1.Image = Image.FromFile(@s);
            }
        }

        public void Swich_MatrixLag()
        {
            string s = languageFile.ReadString("WALLSETFORM", "MATRIX", "矩阵");
            if (s.Contains("\0"))
                s.Replace("\0","");
            foreach (ToolStripItem str in rightMouseContextMenuStrip.Items)
            {
                //if (Chinese_English)
                {
                    if (str is ToolStripMenuItem)
                    {
                        //Console.WriteLine(" str = " + str.Name);
                        if (str.Name.Equals("矩阵VGAToolStripMenuItem"))
                            str.Text = s + "VGA";
                        if (str.Name.Equals("矩阵VIDEOToolStripMenuItem"))
                            str.Text = s + "VIDEO";
                        if (str.Name.Equals("矩阵DVIToolStripMenuItem"))
                            str.Text = s + "DVI";
                        if (str.Name.Equals("矩阵HDMIToolStripMenuItem"))
                            str.Text = s + "HDMI";
                        //if (str.Name.Equals("矩阵DPToolStripMenuItem"))
                            //str.Text = "MatrixDP";
                    }
                }
                /*
                else
                {
                    if (str is ToolStripMenuItem)
                    {
                        //Console.WriteLine(" str = " + str.Name);
                        if (str.Name.Equals("矩阵VGAToolStripMenuItem"))
                            str.Text = "矩阵VGA";
                        if (str.Name.Equals("矩阵VIDEOToolStripMenuItem"))
                            str.Text = "矩阵VIDEO";
                        if (str.Name.Equals("矩阵DVIToolStripMenuItem"))
                            str.Text = "矩阵DVI";
                        if (str.Name.Equals("矩阵HDMIToolStripMenuItem"))
                            str.Text = "矩阵HDMI";
                        //if (str.Name.Equals("矩阵DPToolStripMenuItem"))
                            //str.Text = "矩阵DP";
                    }
                }
                 */ 
            }
            Init_Matrixchannel();
        }
        /// <summary>
        /// 英文下的comboBox显示
        /// </summary>
        private void Init_comboBoxE()
        {
            string s = languageFile.ReadString("WALLSETFORM", "MATRIX", "矩阵").Replace("\0", "");;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("HDMI1");
            comboBox1.Items.Add("HDMI2");
            comboBox1.Items.Add("OPS");
            comboBox1.Items.Add("DVI");
            comboBox1.Items.Add("DP");
            string str = (s + "HDMI");
            comboBox1.Items.Add(str);
            str = (s + "DVI");
            comboBox1.Items.Add(str);
            comboBox1.SelectedIndex = 0;

            //comboBox3.Items.Remove("矩阵HDMI");
            //comboBox3.Items.Remove("矩阵VGA");
            comboBox3.Items.Clear();
            comboBox3.Items.Add("HDMI1");
            comboBox3.Items.Add("OPS");
            comboBox3.Items.Add("VGA");
            str = (s + "HDMI");
            comboBox3.Items.Add(str);
            str = (s + "VGA");
            comboBox3.Items.Add(str);
            comboBox3.SelectedIndex = 0;

            
            comboBox4.Items.Clear();
            comboBox4.Items.Add("HDMI1");
            comboBox4.Items.Add("HDMI2");
            comboBox4.Items.Add("OPS");
            comboBox4.Items.Add("DVI");
            comboBox4.Items.Add("DP");
            comboBox4.Items.Add("VGA");
            str = (s + "HDMI");
            comboBox4.Items.Add(s + "HDMI");
            str = (s + "DVI");
            comboBox4.Items.Add(s + "DVI");
            str = (s + "VGA");
            comboBox4.Items.Add(s + "VGA");
            comboBox4.SelectedIndex = 0;

            //comboBox6.Items.Remove("矩阵VGA");
            //comboBox6.Items.Remove("矩阵VIDEO");
            //comboBox6.Items.Remove("矩阵HDMI");
            //comboBox6.Items.Remove("矩阵DVI");
            //comboBox6.Items.Remove("矩阵YPbPr");
            comboBox6.Items.Clear();
            comboBox6.Items.Add("VGA");
            comboBox6.Items.Add("HDMI");
            comboBox6.Items.Add("VIDEO1");
            comboBox6.Items.Add("VIDEO2");
            comboBox6.Items.Add("VIDEO3");
            comboBox6.Items.Add("VIDEO4");
            comboBox6.Items.Add("S-VIDEO");
            comboBox6.Items.Add("YPbPr");
            str = (s + "VGA");
            comboBox6.Items.Add(str);
            str = (s + "VIDEO");
            comboBox6.Items.Add(str);
            str = (s + "HDMI");
            comboBox6.Items.Add(str);
            str = (s + "DVI");
            comboBox6.Items.Add(str);
            comboBox6.SelectedIndex = 0;
            //comboBox6.Items.Add("MatrixYPbPr");

            comboBox5.Items.Clear();
            comboBox5.Items.Add("VGA");
            comboBox5.Items.Add("HDMI");
            comboBox5.Items.Add("DVI");
            comboBox5.Items.Add("BNC");
            //comboBox5.Items.Remove("矩阵VGA");
            //comboBox5.Items.Remove("矩阵HDMI");
            //comboBox5.Items.Remove("矩阵DVI");
            str = (s + "VGA");
            comboBox5.Items.Add(str);
            str = (s + "HDMI");
            comboBox5.Items.Add(s + "HDMI");
            str = (s + "DVI");
            comboBox5.Items.Add(str);
            comboBox5.SelectedIndex = 0;
        }
        /// <summary>
        /// 中文下的comboBox显示
        /// </summary>
        private void Init_comboBoxC()
        {
            comboBox1.Items.Remove("MatrixHDMI");
            comboBox1.Items.Remove("MatrixDVI");
            comboBox1.Items.Add("矩阵HDMI");
            comboBox1.Items.Add("矩阵DVI");

            comboBox3.Items.Remove("MatrixHDMI");
            comboBox3.Items.Remove("MatrixVGA");
            comboBox3.Items.Add("矩阵HDMI");
            comboBox3.Items.Add("矩阵VGA");

            comboBox4.Items.Remove("MatrixHDMI");
            comboBox4.Items.Remove("MatrixDVI");
            comboBox4.Items.Remove("MatrixVGA");
            comboBox4.Items.Add("矩阵HDMI");
            comboBox4.Items.Add("矩阵DVI");
            comboBox4.Items.Add("矩阵VGA");

            comboBox6.Items.Remove("MatrixVGA");
            comboBox6.Items.Remove("MatrixVIDEO");
            comboBox6.Items.Remove("MatrixHDMI");
            comboBox6.Items.Remove("MatrixDVI");
            comboBox6.Items.Remove("MatrixYPbPr");
            comboBox6.Items.Add("矩阵VGA");
            comboBox6.Items.Add("矩阵VIDEO");
            comboBox6.Items.Add("矩阵HDMI");
            comboBox6.Items.Add("矩阵DVI");
            //comboBox6.Items.Add("矩阵YPbPr");

            comboBox5.Items.Remove("MatrixVGA");
            comboBox5.Items.Remove("MatrixHDMI");
            comboBox5.Items.Remove("MatrixDVI");
            comboBox5.Items.Add("矩阵VGA");
            comboBox5.Items.Add("矩阵HDMI");
            comboBox5.Items.Add("矩阵DVI");
        }
        /// <summary>
        /// 语言切换时对应改变comboBox显示
        /// </summary>
        private void Init_Matrixchannel()
        {
            if (Motherboard_type == 0)
            {
                int n = comboBox3.SelectedIndex;
                //if (Chinese_English)
                {
                    Init_comboBoxE();
                }
                //else
                //{
                    //Init_comboBoxC();
               // }
                comboBox3.SelectedIndex = n;
            }
            else if (Motherboard_type == 1)
            {
                int n = comboBox4.SelectedIndex;
                //if (Chinese_English)
                {
                    Init_comboBoxE();
                }
                //else
                //{
                //    Init_comboBoxC();
                //}
                comboBox4.SelectedIndex = n;
            }
            else if (Motherboard_type == 4)
            {
                int n = comboBox1.SelectedIndex;
                //if (Chinese_English)
                {
                    Init_comboBoxE();
                }
                //else
                //{
                 //   Init_comboBoxC();
                //}
                comboBox1.SelectedIndex = n;
            }
            else if (Motherboard_type == 2)
            {
                int n = comboBox6.SelectedIndex;
                //if (Chinese_English)
                {
                    Init_comboBoxE();
                }
                //else
                //{
                //    Init_comboBoxC();
                //}
                comboBox6.SelectedIndex = n;
            }
            else
            {
                int n = comboBox5.SelectedIndex;
                //if (Chinese_English)
                {
                    Init_comboBoxE();
                }
                //else
                //{
                //    Init_comboBoxC();
               // }
                comboBox5.SelectedIndex = n;
            }
        }
        /// <summary>
        /// 中英文切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 中文英文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LanguageMenuStrip.Show(this.button24, this.button24.Width, this.button24.Height / 2);

            /*
            string s = settingFile.ReadString("SETTING", "Pwd", "0");
            int k = comboBox2.SelectedIndex;
            if (Chinese_English == 0)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
                package = "\\CH_package.ini";
                languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                Chinese_English = 0;
            }
            else if (Chinese_English == 1)
            {
                Chinese_English = 1;
                package = "\\EN_package.ini";
                languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
                //Init_FormString();
            }


            if (s == "1")
            {
                toolStripStatusLabel2.Text = languageFile.ReadString("MAINFORM", "USER_ADMIN", "用户权限：  管理员用户！"); //"User rights: Administrator user！";
            }
            else
                toolStripStatusLabel2.Text = languageFile.ReadString("MAINFORM", "USER_GENERAL", "用户权限：  普通用户！"); //"User rights: General user！";

            if (Rs232Con)
                toolStripStatusLabel3.Text = PortName + " Opened  " + BaudRate + " Bps";
            else
                toolStripStatusLabel3.Text = PortName + " Closed  " + BaudRate + " Bps";

            Swich_MatrixLag();
            comboBox2.SelectedIndex = k;
            this.Refresh();
             */ 
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            /*
            for (int i = 0; i < 128; i++)//清除记录
                select_address[i] = 0;
            if (tabControl3.SelectedIndex == 0)
            {
                for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
                {
                    for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
                    {
                        int num = screens[((i + 1) + j * colsCount) - 1].Number;
                        select_address[num] = (byte)num;
                        //Console.WriteLine("select_address[num] = " + select_address[num]);
                    }
                }
            }
            if (tabControl3.SelectedIndex == 1)
            {
                for (int j = sheet_back.SelectionRange.Row; j <= sheet_back.SelectionRange.EndRow; j++)
                {
                    for (int i = sheet_back.SelectionRange.Col; i <= sheet_back.SelectionRange.EndCol; i++)
                    {
                        int num = screens[((i + 1) + j * colsCount) - 1].Number;
                        select_address[num] = (byte)num;
                        //Console.WriteLine("select_address[num] = " + select_address[num]);
                    }
                }
            }
             * */
        }
        //public bool Form_UartIrCmd_Opend = false;
        /// <summary>
        /// 进入遥控功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 遥控控制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            int count = 0;
            for (int i = 0; i < 256; i++)
                select_address[i] = 0;
            int num = 0;
            //if (tabControl3.SelectedIndex == 0)
            {
                for (int j = rowStar; j <= rowEnd; j++)
                {
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        num = screens[((i + 1) + j * colsCount) - 1].Number;
                        select_address[count] = (byte)num;
                        count++;
                        //Console.WriteLine("select_address[num] = " + select_address[num]);
                    }
                }
            }
            /*
            if (tabControl3.SelectedIndex == 1)
            {
                for (int j = sheet_back.SelectionRange.Row; j <= sheet_back.SelectionRange.EndRow; j++)
                {
                    for (int i = sheet_back.SelectionRange.Col; i <= sheet_back.SelectionRange.EndCol; i++)
                    {
                        num = screens[((i + 1) + j * colsCount) - 1].Number;
                        select_address[num] = (byte)num;
                        count++;
                        //Console.WriteLine("select_address[num] = " + select_address[num]);
                    }
                }
            }
             */ 
            //Form_UartIrCmd_Opend = true;
            if (Motherboard_flag == 4)
            {
                Form_UartIrCmd_3458 f = new Form_UartIrCmd_3458(this, count);
                f.ShowDialog();
            }
            else
            {
                Form_UartIrCmd_59 f = new Form_UartIrCmd_59(this, count);
                f.ShowDialog();
            }
            LogHelper.WriteLog("======进行遥控操作======");
        }
        /// <summary>
        /// 把字符转为Hex
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] array = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                array[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            }
            return array;
        }

        public bool IsIllegalHexadecimal(string hex)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(hex, "^[0-9A-Fa-f]+$");
        }

        //串口指令编辑输入
        private byte[] Buf = new byte[1024];
        /// <summary>
        /// 提示发送指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            try
            {
                if (Rs232Con)
                {
                    if (this.richTextBox1.Text.Length > 0)
                    {
                        if (this.checkBox1.Checked)
                        {
                            for (int i = 0; i < richTextBox1.Lines.Length; i++)
                            {
                                byte[] Buf = MainForm.HexStringToByteArray(this.richTextBox1.Lines[i].Trim());
                                if (TCPCOM)
                                {
                                    //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                                    TcpSendMessage(Buf, 0, Buf.Length);
                                }
                                else
                                {
                                    SerialPortUtil.serialPortSendData(serialPort1, Buf, 0, Buf.Length, 100, 1);
                                }
                                richTextBox2.AppendText(ToHexString(Buf, Buf.Length));
                            }
                        }
                        else
                        {
                            serialPort1.Write(this.richTextBox1.Text.Trim());
                            richTextBox2.AppendText(this.richTextBox1.Text + "\n");
                        }
                        LogHelper.WriteLog("======开始调试(" + richTextBox1.Text + ")======");
                    }
                    else
                    {
                        string ts = languageFile.ReadString("MESSAGEBOX", "M3", "请确保一次发送的字符长度在128之内！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                        richTextBox1.Text = richTextBox1.Text.Substring(0, 127);
                        return;
                    }

                }
                else
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
            catch
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M4", "输入发送的字符不符合要求，请重新输入！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
        }
        /// <summary>
        /// 复制粘贴功能菜单打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip_Opening_1(object sender, CancelEventArgs e)
        {
            //没有选择文本时，复制菜单禁用
            string selectText = (contextMenuStrip1.SourceControl as RichTextBox).SelectedText;
            if (selectText != "")
                c复制ToolStripMenuItem.Enabled = true;
            else
                c复制ToolStripMenuItem.Enabled = false;
            //剪切板没有文本内容时，粘贴菜单禁用
            if (Clipboard.ContainsText())
            {
                v粘贴ToolStripMenuItem.Enabled = true;
            }
            else
                v粘贴ToolStripMenuItem.Enabled = false;
        }
        /// <summary>
        /// 复制操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void c复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string selectText = (contextMenuStrip1.SourceControl as RichTextBox).SelectedText;
            if (selectText != "")
            {
                Clipboard.SetText(selectText);
            }
        }
        /// <summary>
        /// 粘贴操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void v粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                RichTextBox txtBox = contextMenuStrip1.SourceControl as RichTextBox;
                int index = txtBox.SelectionStart;  //记录下粘贴前的光标位置
                string text = txtBox.Text;
                //删除选中的文本
                text = text.Remove(txtBox.SelectionStart, txtBox.SelectionLength);
                //在当前光标输入点插入剪切板内容
                text = text.Insert(txtBox.SelectionStart, Clipboard.GetText());
                txtBox.Text = text;
                //重设光标位置
                txtBox.SelectionStart = index;
            }
        }
        /// <summary>
        /// 设置屏幕的序列号操作按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);
            if (numericUpDown2.Value > rowsCount)//做行列数值的限制，防止设置范围超出
                numericUpDown2.Value = rowsCount;
            if (numericUpDown3.Value > colsCount)
                numericUpDown3.Value = colsCount;
            decimal num = numericUpDown2.Value - 1;
            decimal num1 = numericUpDown3.Value;
            decimal num2 = screens[(int)((num1 + num * colsCount) - 1)].Number;

            New_adress((uint)numericUpDown1.Value, (byte)num2);//更新单元的序列号

            LogHelper.WriteLog("=====绑定地址【" + num2 + "】=====");
        }
        /// <summary>
        /// 用户手册打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 用户手册ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Motherboard_flag == 4)
                {
                    if (Chinese_English == 0)
                    {
                        if (Motherboard_type == 1)
                            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\3458_K6A_ch.pdf");
                        else if (Motherboard_type == 4)
                            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\3458_K6C_ch.pdf"); 
                    }
                    else
                    {
                        if (Motherboard_type == 1)
                            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\3458_K6A_en.pdf");
                        else if (Motherboard_type == 4)
                            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\3458_K6C_en.pdf");
                    }
                }
                else
                {
                    if (Motherboard_type == 3)
                    {
                        if (Chinese_English == 0)
                            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\59_Mini_ch.pdf");
                        else
                            System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\59_Mini_en.pdf");
                    }
                }
            }
            catch
            { 
                
            }
        }
        /// <summary>
        /// 串口设置打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                if (uMultiComPort == 2)
                {
                    serialPort2.Close();
                }
                Rs232Con = false;
                //button7.ForeColor = Color.Black;
                toolStripStatusLabel3.ForeColor = Color.Red;
                if (Chinese_English)
                    toolStripStatusLabel3.Text = "Port：" + PortName + " Closed  " + BaudRate + " Bps";
                else
                    toolStripStatusLabel3.Text = "端口：" + PortName + " 已关闭  " + BaudRate + " Bps";
            }
            */
            SerialSetForm f = new SerialSetForm(this);
            f.ShowDialog();
            LogHelper.WriteLog("=====完成串口设置=====");
            //button7.ForeColor = Color.Black;
            //label4.ForeColor = Color.Red;
            //label4.Text = "端口：" + PortName + " 已关闭  " + BaudRate + " Bps";
            /*
            if (Rs232Con)
            {
                try
                {
                    serialPort1.PortName = PortName;
                    serialPort1.BaudRate = BaudRate;
                    serialPort2.PortName = PortName2;
                    serialPort2.BaudRate = BaudRate2;
                    serialPort1.Open();
                    if (uMultiComPort == 2)
                    {
                        serialPort2.Open();
                    }
                    Rs232Con = true;
                    toolStripStatusLabel3.ForeColor = Color.Green;
                    //button7.ForeColor = Color.Green;
                    if (Chinese_English)
                        toolStripStatusLabel3.Text = "Port：" + PortName + " Opened  " + BaudRate + " Bps";
                    else
                        toolStripStatusLabel3.Text = "端口：" + PortName + " 已打开  " + BaudRate + " Bps";
                }
                catch
                {
                    if (Chinese_English)
                        MessageBox.Show("Serial open error， reopen！", "Tips");
                    else
                        MessageBox.Show("串口打开出错，重新打开", "提示");
                }
            }
             * */
        }
        /// <summary>
        /// 定时开关机功能打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 开关机设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            Form_OnOff f = new Form_OnOff(this);
            f.ShowDialog();
        }
        
        //private TimeSpan ts1,ts2;
        /// <summary>
        /// 定时更新时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            string sl = languageFile.ReadString("MAINFORM", "SYS_TIME ", "系统当前时间：");
            toolStripStatusLabel1.Text = sl + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            /*
            if(Chinese_English)
                toolStripStatusLabel1.Text = "System Time：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            else
                toolStripStatusLabel1.Text = "系统当前时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            /*
            if (Rs232Con_true)
            {
                if (serialPort1.IsOpen)
                {
                    Rs232Con_true = true;
                }
                else
                {
                    if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                    {
                        stopProgress();
                        //return;
                    }
                    button6_Click(null,null);
                    toolStripStatusLabel3.ForeColor = Color.Red;
                    if (Chinese_English)
                    {
                        toolStripStatusLabel3.Text = " Port：" + PortName + "Disconnect" + BaudRate + " Bps";
                        MessageBox.Show("Serial port is disconnected, Please reset open！", "Tips");
                    }
                    else
                    {
                        toolStripStatusLabel3.Text = " 端口：" + PortName + " 已断开  " + BaudRate + " Bps";
                        MessageBox.Show("串口已断开，请重新设置打开", "提示");
                    }
                }
            }
             */ 
            //Console.WriteLine("ontime= " + DateTime.Now.CompareTo(on) + Timing_on);
            //Console.WriteLine("datetime= " +DateTime.Now);
        }
        //提示窗口显示
        /// <summary>
        /// 界面操作切换刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            if (tabControl1.SelectedIndex == Motherboard_flag)
            {
                tabPage12.Parent = tabControl3;
                tabPage12.Show();
                tabPage13.Parent = null;
                tabPage13.Hide();

                rowStar = sheet.SelectionRange.Row;
                rowEnd = sheet.SelectionRange.EndRow;
                colStar = sheet.SelectionRange.Col;
                colEnd = sheet.SelectionRange.EndCol;
                sheet.SetRowsHeight(0, sheet.RowCount, (ushort)((reoGridControl1.Size.Height) / rowsCount));//改行的高度，从0行开始，改ColumnCount行
                sheet.SetColumnsWidth(0, sheet.ColumnCount, (ushort)((reoGridControl1.Size.Width) / colsCount));//改列的宽度，从0列开始，改ColumnCount列
                reoGridControl1.CurrentWorksheet.Resize(rowsCount, colsCount);
                reoGridControl1.Refresh();

                if (Chinese_English == 1)
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    //ApplyResource();
                    //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
                    foreach (Control ctl in tabControl3.Controls)
                    {
                        resources.ApplyResources(ctl, ctl.Name);
                    }
                }
                else
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                    //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
                    foreach (Control ctl in tabControl3.Controls)
                    {
                        resources.ApplyResources(ctl, ctl.Name);
                    }
                }
                changed = false;
                Auto_Run(Select_bt());
            }
            else
            {
                tabPage13.Parent = tabControl3;
                tabPage13.Show();
                tabPage12.Parent = null;
                tabPage12.Hide();

                rowStar = sheet_back.SelectionRange.Row;
                rowEnd = sheet_back.SelectionRange.EndRow;
                colStar = sheet_back.SelectionRange.Col;
                colEnd = sheet_back.SelectionRange.EndCol;
                //checkBox2.Checked = false;
                sheet_back.SetRowsHeight(0, sheet_back.RowCount, (ushort)((reoGridControl2.Size.Height) / rowsCount));//改行的高度，从0行开始，改ColumnCount行
                sheet_back.SetColumnsWidth(0, sheet_back.ColumnCount, (ushort)((reoGridControl2.Size.Width) / colsCount));//改列的宽度，从0列开始，改ColumnCount列
                reoGridControl2.CurrentWorksheet.Resize(rowsCount, colsCount);
                reoGridControl2.Refresh();

                if (Chinese_English == 1)
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    //ApplyResource();
                    //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
                    foreach (Control ctl in tabControl3.Controls)
                    {
                        resources.ApplyResources(ctl, ctl.Name);
                    }
                }
                else
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                    //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
                    foreach (Control ctl in tabControl3.Controls)
                    {
                        resources.ApplyResources(ctl, ctl.Name);
                    }
                }
            }
            /*
            if (tabControl1.SelectedIndex == 5)
            {
                richTextBox2.Visible = true;
                button10.Visible = true;
                button13.Visible = true;
                pictureBox1.Visible = false;
                button10.Visible = true;
                button13.Visible = true;
                button32.Visible = false;
            }
            if (tabControl1.SelectedIndex == 0)
            {
                richTextBox2.Visible = false;
                button10.Visible = false;
                button13.Visible = false;
                pictureBox1.Visible = true;
                button10.Visible = false;
                button13.Visible = false;
                button32.Visible = true;
            }
             */ 
        }
        /// <summary>
        /// 用户权限登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 用户管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_Log f = new Form_Log(this);
            f.ShowDialog();

            string s = settingFile.ReadString("SETTING", "Pwd", "0");
            //Console.WriteLine("s===" + s);
            if (!s.Equals(""))
            {
                if (s == "0")
                {
                    tabPage5.Parent = null;
                    tabPage5.Hide();
                    tabPage14.Parent = null;
                    tabPage14.Hide();
                    tabPage4.Parent = null;
                    tabPage4.Hide();
                    if (Motherboard_flag == 4)
                    {
                        tabPage3.Parent = null;
                        tabPage3.Hide();
                        tabPage1.Parent = null;
                        tabPage1.Hide();
                    }
                    tabPage2.Parent = null;
                    tabPage2.Hide();
                    button10.Visible = false;
                    button13.Visible = false;
                    button32.Visible = false;
                    richTextBox2.Visible = false;
                    pictureBox1.Visible = true;

                    if (Chinese_English == 1)
                    {
                        //Init_FormString();
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        ApplyResource();
                    }
                    else if (Chinese_English == 2)
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
                        ApplyResource();
                    }
                    else
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                        ApplyResource();
                    } 
                    toolStripStatusLabel2.Text = languageFile.ReadString("MAINFORM", "USER_GENERAL", "用户权限：  普通用户！"); //"User rights: General user！";
                    屏幕参数调整ToolStripMenuItem.Visible = false;
                    screenNumberToolStripMenuItem.Visible = false;
                }
                if (s == "1")
                {
                    tabPage2.Parent = tabControl1;
                    tabPage2.Show();
                    if (Motherboard_flag == 4)
                    {
                        tabPage1.Parent = tabControl1;
                        tabPage1.Show();
                        tabPage3.Parent = tabControl1;
                        tabPage3.Show();
                    }
                    tabPage4.Parent = tabControl1;
                    tabPage4.Show(); 
                    if (Motherboard_flag == 4)
                    {
                        tabPage14.Parent = tabControl1;
                        tabPage14.Show(); 
                    }
                    tabPage5.Parent = tabControl1;
                    tabPage5.Show();
                    button32.Visible = true;

                    //Init_FormString();
                    
                    if (Chinese_English == 1)
                    {
                        //Init_FormString();
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        ApplyResource();
                        //toolStripStatusLabel2.Text = "User rights: Administrator user！";
                    }
                    else if (Chinese_English == 2)
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
                        ApplyResource();
                    }
                    else
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                        ApplyResource();
                        //toolStripStatusLabel2.Text = "用户权限：  管理员用户！";
                    }
                    toolStripStatusLabel2.Text = languageFile.ReadString("MAINFORM", "USER_ADMIN", "用户权限：  管理员用户！"); //"User rights: General user！";

                    屏幕参数调整ToolStripMenuItem.Visible = true;
                    screenNumberToolStripMenuItem.Visible = true;
                }
            }
            if (Rs232Con)
            {
                string sl = languageFile.ReadString("MAINFORM", "STATUS_ON", "打开");
                toolStripStatusLabel3.Text = PortName + "  " + sl + " " + BaudRate + " Bps";
            }
            else
            {
                string sl = languageFile.ReadString("MAINFORM", "STATUS_OFF", "关闭");
                toolStripStatusLabel3.Text = PortName + "  " + sl + " " + BaudRate + " Bps";
            }
            if (checkBox2.Checked)
            {
                comboBox2.Enabled = true;
                label20.Enabled = true;
            }
            tabControl1.SelectedIndex = 0;
            this.Refresh();
        }


        /// <summary>
        /// 参数调整指令线程
        /// </summary>
        /// <param name="rowS"></param>
        /// <param name="rowE"></param>
        /// <param name="colS"></param>
        /// <param name="colE"></param>
        /// <param name="A1"></param>
        private void Control_FuncThread(int rowS, int rowE, int colS, int colE,byte A1)
        {
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[2] = 0x20;
            array[3] = A1;
            for (int j = rowS; j <= rowE; j++)
            {
                for (int i = colS; i <= colE; i++)
                {
                    array[1] = (byte)screens[(i + j * colsCount)].Number;
                    array[4] = (byte)(255 - (255 & array[0] + array[1] + array[2] + array[3]));
                    if (A1 == 0xD0 || A1 == 0xEF)
                    {
                        screen_V[screens[(i + j * colsCount)].Number - 1] = 0;//复位拼缝的数值
                        screen_H[screens[(i + j * colsCount)].Number - 1] = 0;//复位拼缝的数值
                    }
                    //Console.WriteLine("array[1] = " + array[1]);
                    try
                    {
                        //serialPort1.Write(array, 0, 5);
                        if (TCPCOM)
                        {
                            //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                            TcpSendMessage(array, 0, 5);
                        }
                        else
                        {
                            SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 2);
                        }
                        //richTextBox2.AppendText(ToHexString(array, 5));
                        //Thread.Sleep(100); 
                    }
                    catch
                    {
                        string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                        stopProgress();
                        return;
                    }
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        richTextBox2.AppendText(ToHexString(array, 5));
                    }));
                }
            }
            stopProgress();
        }
        /// <summary>
        /// 参数调整函数
        /// </summary>
        /// <param name="A1"></param>
        private void Send_Func(byte A1)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            startProgress(0);

            myThread = new Thread(new ThreadStart(delegate()
            {
                Control_FuncThread(rowStar, rowEnd, colStar, colEnd, A1);
            })); //开线程         
            myThread.Start(); //启动线程 
            /*
            byte[] array = new byte[5];
            array[0] = 0xE5;
            array[2] = 0x20;
            array[3] = A1;
            //if (tabControl3.SelectedIndex == 0)
            {
                for (int j = rowStar; j <= rowEnd; j++)
                {
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        array[1] = (byte)screens[((i + 1) + j * colsCount) - 1].Number;
                        array[4] = (byte)(255 - (255 & array[0] + array[1] + array[2] + array[3]));
                        //Console.WriteLine("array[1] = " + array[1]);
                        try
                        {
                            //serialPort1.Write(array, 0, 5);
                            SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 2);
                            richTextBox2.AppendText(ToHexString(array, 5));
                            //Thread.Sleep(100); 
                        }
                        catch
                        {
                            if (Chinese_English)
                                MessageBox.Show("    Serial error！", "Tips");
                            else
                                MessageBox.Show("    串口出错！", "提示");
                        }
                    }
                }
            }
            /*
            if (tabControl3.SelectedIndex == 1)
            {
                for (int j = sheet_back.SelectionRange.Row; j <= sheet_back.SelectionRange.EndRow; j++)
                {
                    for (int i = sheet_back.SelectionRange.Col; i <= sheet_back.SelectionRange.EndCol; i++)
                    {
                        array[1] = (byte)screens[((i + 1) + j * colsCount) - 1].Number;
                        array[4] = (byte)(255 - (255 & array[0] + array[1] + array[2] + array[3]));
                        //Console.WriteLine("array[1] = " + array[1]);
                        try
                        {
                            //serialPort1.Write(array, 0, 5);
                            SerialPortUtil.serialPortSendData(serialPort1, array, 0, 5, 100, 2);
                            richTextBox2.AppendText(ToHexString(array, 5));
                            //Thread.Sleep(100); 
                        }
                        catch
                        {
                            if (Chinese_English)
                                MessageBox.Show("    Serial error！", "Tips");
                            else
                                MessageBox.Show("    串口出错！", "提示");
                        }
                    }
                }
            }
             */ 
        }
        /// <summary>
        /// 老化开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton1.Checked)
            {
                Send_Func(0xE1);
                radioButton2.Checked = false;
                LogHelper.WriteLog("=====打开老化设置=====");
            }
        }
        /// <summary>
        /// 老化关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton2.Checked)
            {
                Send_Func(0xE2);
                radioButton1.Checked = false;
                LogHelper.WriteLog("=====关闭老化设置=====");
            }
        }
        /// <summary>
        /// 倒屏开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton4.Checked)
            {
                Send_Func(0xE3);
                radioButton3.Checked = false;
            }
        }
        /// <summary>
        /// 倒屏关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton3.Checked)
            {
                Send_Func(0xE4);
                radioButton4.Checked = false;
            }
        }
        /// <summary>
        /// 字符转十六进制
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes, int Length) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2") + " ");
                }
                hexString = strB.ToString() + "\r\n";
            }
            return hexString;
        }
        /// <summary>
        /// 指令清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox1.Refresh();
        }
        /// <summary>
        /// 调试清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            richTextBox2.Refresh();
        }
        /// <summary>
        /// 隐藏按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            richTextBox2.Visible = false;
            button32.Visible = true;
            button10.Visible = false;
            pictureBox1.Visible = true;
            button13.Visible = false;
        }
        /// <summary>
        /// TI Mode 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton5.Checked)
            {
                Send_Func(0xE6);
                radioButton6.Checked = false;
                LogHelper.WriteLog("=====设置Ti Mode 0=====");
            }
        }
        /// <summary>
        /// TI Mode 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton6.Checked)
            {
                Send_Func(0xE7);
                radioButton5.Checked = false;
                LogHelper.WriteLog("=====设置Ti Mode 1=====");
            }
        }
        /// <summary>
        /// 10 Bit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton7.Checked)
            {
                Send_Func(0xE8);
                radioButton8.Checked = false;
                radioButton15.Checked = false;
            }
        }
        /// <summary>
        /// 8 Bit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton8.Checked)
            {
                Send_Func(0xE9);
                radioButton7.Checked = false;
                radioButton15.Checked = false;
            }
        }
        /// <summary>
        /// 蓝屏开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton9.Checked)
            {
                Send_Func(0xEA);
                radioButton10.Checked = false;
                LogHelper.WriteLog("=====打开蓝屏设置=====");
            }
        }
        /// <summary>
        /// 蓝屏关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton10.Checked)
            {
                Send_Func(0xEB);
                radioButton9.Checked = false;
                LogHelper.WriteLog("=====关闭蓝屏设置=====");
            }
        }
        /// <summary>
        /// 选择中文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton11.Checked)
            {
                Send_Func(0xEC);
                radioButton12.Checked = false;
                LogHelper.WriteLog("=====设置系统显示中文=====");
            }
        }
        /// <summary>
        /// 选择英文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton12.Checked)
            {
                Send_Func(0xED);
                radioButton11.Checked = false;
                LogHelper.WriteLog("=====设置系统显示英文=====");
            }
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton13.Checked)
            {
                Send_Func(0xF4);
                radioButton14.Checked = false;
            }
        }

        private void radioButton14_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton14.Checked)
            {
                Send_Func(0xF5);
                radioButton13.Checked = false;
            }
        }

        private void Rest_form(bool r_b,bool rest)//界面的对应复位操作
        {
            if (r_b)//所以的场景清空复位
            {
                if (FileHandle.createDirectory(currentConnectionName) == 1)
                {
                    initSceneButton();
                    ResetGrid();
                    initRoGridSet();
                    initRoGridControl(rowsCount, colsCount);
                    saveSceneFile(currentConnectionName + "\\scene.rgf");
                }
            }
            else//对当前 的场景选中的进行复位，保存
            {
                if (sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                {
                    sheet.UnmergeRange(sheet.SelectionRange);
                    removeOnmerge(screens[sheet.SelectionRange.Row * sheet.ColumnCount + sheet.SelectionRange.Col].Number);   //从合并中移除
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        for (int j = rowStar; j <= rowEnd; j++)
                        {
                            addOnUnMerge(screens[j * colsCount + i]);//增加没合成的
                            if (rest)
                            {
                                sheet.SetCellData(j, i, screens[j * colsCount + i].Name + " " + screens[j * colsCount + i].IntputType);
                                sheet_back.SetCellData(j, i, screens[j * colsCount + i].Name + " " + screens[j * colsCount + i].IntputType);
                            }
                            else
                            {
                                sheet.SetCellData(j, i, screens[j * colsCount + i].Name + " " + "HDMI1");
                                sheet_back.SetCellData(j, i, screens[j * colsCount + i].Name + " " + "HDMI1");
                            }
                        }
                    }
                    sheet.SaveRGF(currentConnectionName + "\\" + currentSceneName);
                    //Console.WriteLine("UnmergeRange");
                }
                else
                {
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        for (int j = rowStar; j <= rowEnd; j++)
                        {
                            //addOnUnMerge(screens[j * colsCount + i]);//增加没合成的
                            if (rest)
                            {
                                sheet.SetCellData(j, i, screens[j * colsCount + i].Name + " " + screens[j * colsCount + i].IntputType);
                                sheet_back.SetCellData(j, i, screens[j * colsCount + i].Name + " " + screens[j * colsCount + i].IntputType);
                            }
                            else
                            {
                                sheet.SetCellData(j, i, screens[j * colsCount + i].Name + " " + "HDMI1");
                                sheet_back.SetCellData(j, i, screens[j * colsCount + i].Name + " " + "HDMI1");
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 用户复位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }

            string tt = languageFile.ReadString("MESSAGEBOX", "M5", "确定对选中的屏幕单元进行复位操作！");
            string th = languageFile.ReadString("MESSAGEBOX", "TP", "提示");

            DialogResult t = MessageBox.Show(tt, th, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            /*
            if (Chinese_English)
                t = MessageBox.Show(" Make sure to Reset the selected screen unit !", " Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            else
                t = MessageBox.Show(" 确定对选中的屏幕单元进行复位操作！", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
             */ 
            if (t == DialogResult.Yes || t == DialogResult.OK)
            {
                Send_Func(0xEF);
                radioButton11.Checked = true;
                radioButton10.Checked = true;
                radioButton3.Checked = true;
                Rest_form(false, true);
                LogHelper.WriteLog("=====进行用户复位操作=====");
            }
        }
        /// <summary>
        /// 6  Bit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton15.Checked)
            {
                Send_Func(0xEE);
                radioButton7.Checked = false;
                radioButton8.Checked = false;
            }
        }
        /// <summary>
        /// AB　Mode 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton16_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton16.Checked)
            {
                Send_Func(0xF2);
                radioButton17.Checked = false;
                LogHelper.WriteLog("=====设置AB Mode 1=====");
            }
        }
        /// <summary>
        /// AB Mode  0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton17_CheckedChanged(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton17.Checked)
            {
                Send_Func(0xF3);
                radioButton16.Checked = false;
                LogHelper.WriteLog("=====设置AB Mode 0=====");
            }
        }
        /// <summary>
        /// USB　升级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (textBox4.Text.Equals("8202"))
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M6", "确定对选中的屏幕单元进行USB升级操作！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");

                DialogResult t = MessageBox.Show(ts, tp, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                /*
                if (Chinese_English)
                    t = MessageBox.Show(" Make sure to perform a USB upgrade on the selected screen unit!", " Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                else
                    t = MessageBox.Show(" 确定对选中的屏幕单元进行USB升级操作！", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                 */ 
                if (t == DialogResult.Yes || t == DialogResult.OK)
                {
                    Send_Func(0xE5);
                    radioButton11.Checked = true;
                    radioButton10.Checked = true;
                    radioButton3.Checked = true;
                    radioButton2.Checked = true;
                    radioButton6.Checked = true;
                    radioButton7.Checked = true;
                    radioButton17.Checked = true;
                    LogHelper.WriteLog("=====进行系统升级操作=====");
                }
            }
            else
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M7", "密码输入错误，请重新输入正确密码！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                /*
                if (Chinese_English)
                    MessageBox.Show("Password entered incorrectly, re-enter the correct password!", "Tips");
                else
                    MessageBox.Show("密码输入错误，请重新输入正确密码！", "提示");
                 */ 
                textBox4.Text = "";
            }
        }
        /// <summary>
        /// 指令显示改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.SelectionStart = richTextBox2.Text.Length;
            //richTextBox2.SelectionLength = 0;
            //richTextBox2.Focus();
            richTextBox2.ScrollToCaret();
        }

        /// <summary>
        /// 软件注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 软件注册ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_Reg f = new Form_Reg(this);
            f.ShowDialog();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
            sheet.SetRowsHeight(0, sheet.RowCount, (ushort)((reoGridControl1.Size.Height) / rowsCount));//改行的高度，从0行开始，改ColumnCount行
            sheet.SetColumnsWidth(0, sheet.ColumnCount, (ushort)((reoGridControl1.Size.Width) / colsCount));//改列的宽度，从0列开始，改ColumnCount列
            reoGridControl1.CurrentWorksheet.Resize(rowsCount, colsCount);
            sheet_back.SetRowsHeight(0, sheet_back.RowCount, (ushort)((reoGridControl2.Size.Height) / rowsCount));//改行的高度，从0行开始，改ColumnCount行
            sheet_back.SetColumnsWidth(0, sheet_back.ColumnCount, (ushort)((reoGridControl2.Size.Width) / colsCount));//改列的宽度，从0列开始，改ColumnCount列
            reoGridControl2.CurrentWorksheet.Resize(rowsCount, colsCount);

            this.Refresh();
            
        }

        public int Select_bt()//判断当前的场景，返回当前场景的场景号
        {
            int b = 0;
            if (currentSceneName.Equals("scene1.rgf"))
                b = 1;
            else if (currentSceneName.Equals("scene2.rgf"))
                b = 2;
            else if (currentSceneName.Equals("scene3.rgf"))
                b = 3;
            else if (currentSceneName.Equals("scene4.rgf"))
                b = 4;
            else if (currentSceneName.Equals("scene5.rgf"))
                b = 5;
            else if (currentSceneName.Equals("scene6.rgf"))
                b = 6;
            else if (currentSceneName.Equals("scene7.rgf"))
                b = 7;
            else if (currentSceneName.Equals("scene8.rgf"))
                b = 8;
            else if (currentSceneName.Equals("scene9.rgf"))
                b = 9;
            else if (currentSceneName.Equals("scene10.rgf"))
                b = 10;
            else if (currentSceneName.Equals("scene11.rgf"))
                b = 11;
            else if (currentSceneName.Equals("scene12.rgf"))
                b = 12;
            else
                b = 1;
            return b;
        }
        /// <summary>
        /// 场景自动轮巡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button25_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            int count = 0;
            saveChange(currentConnectionName + "\\" + currentSceneName);
            foreach (Control var in groupBox5.Controls)
            {
                if (var is System.Windows.Forms.Button)
                { //如果是button
                    //Console.WriteLine(var.Name);
                    //Console.WriteLine(changeButton.Name);//((Button)var).Tag.Equals("selected"));
                    if (var.Enabled == true)
                    {
                        count++;
                        //Console.WriteLine("====" + var.Name);
                    }
                }
            }
            bt_sce12_Click_flag = settingFile.ReadBool("SCREEN", "S_flag", bt_sce12_Click_flag);
            if (bt_sce12_Click_flag && count == 12)
                count = 13;
            //Console.WriteLine("count=" + count);
            Form_Auto f = new Form_Auto(this, count - 1);
            f.ShowDialog();
        }
        /// <summary>
        /// 轮巡对应切换
        /// </summary>
        /// <param name="t"></param>
        public void Auto_Run(int t)
        {
            switch (t)
            {
                case 1:
                    if (bt_sce1.Enabled)
                        bt_sce1_Click(bt_sce1, null);
                    break;
                case 2:
                    if (bt_sce2.Enabled)
                        bt_sce2_Click(bt_sce2, null);
                    break;
                case 3:
                    if (bt_sce3.Enabled)
                        bt_sce3_Click(bt_sce3, null);
                    break;
                case 4:
                    if (bt_sce4.Enabled)
                        bt_sce4_Click(bt_sce4, null);
                    break;
                case 5:
                    if (bt_sce5.Enabled)
                        bt_sce5_Click(bt_sce5, null);
                    break;
                case 6:
                    if (bt_sce6.Enabled)
                        bt_sce6_Click(bt_sce6, null);
                    break;
                case 7:
                    if (bt_sce7.Enabled)
                        bt_sce7_Click(bt_sce7, null);
                    break;
                case 8:
                    if (bt_sce8.Enabled)
                        bt_sce8_Click(bt_sce8, null);
                    break;
                case 9:
                    if (bt_sce9.Enabled)
                        bt_sce9_Click(bt_sce9, null);
                    break;
                case 10:
                    if (bt_sce10.Enabled)
                        bt_sce10_Click(bt_sce10, null);
                    break;
                case 11:
                    if (bt_sce11.Enabled)
                        bt_sce11_Click(bt_sce11, null);
                    break;
                case 12:
                    if (bt_sce12.Enabled)
                        bt_sce12_Click(bt_sce12, null);
                    break;
                default:
                    break;
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            Form_Logo f = new Form_Logo(this.Chinese_English);
            f.ShowDialog();
            //this.contextMenuStrip2.Show(this.button26, this.button26.Width, this.button26.Height);
        }
        //private static int Count_Text = 0;
        //字幕功能设置
        private void button20_Click(object sender, EventArgs e)
        {
            /*\\\\\\\
            if (!Rs232Con)
            {
                if (Chinese_English)
                    MessageBox.Show("Serial port is not connected！", "Tips");
                else
                    MessageBox.Show("    串口未连接！", "提示");
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            for (int i = 0; i < 128; i++)
                select_address[i] = 0;
            int num = 0;
            //if (tabControl3.SelectedIndex == 0)
            {
                for (int j = rowStar; j <= rowEnd; j++)
                {
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        num = screens[((i + 1) + j * colsCount) - 1].Number;
                        select_address[num] = (byte)num;
                        //Console.WriteLine("select_address[num] = " + select_address[num]);
                    }
                }
            }
            /*
            if (tabControl3.SelectedIndex == 1)
            {
                for (int j = sheet_back.SelectionRange.Row; j <= sheet_back.SelectionRange.EndRow; j++)
                {
                    for (int i = sheet_back.SelectionRange.Col; i <= sheet_back.SelectionRange.EndCol; i++)
                    {
                        num = screens[((i + 1) + j * colsCount) - 1].Number;
                        select_address[num] = (byte)num;
                        //Console.WriteLine("select_address[num] = " + select_address[num]);
                    }
                }
            }
             */
            Form_TipsText f = new Form_TipsText(this);
            f.Show();
            LogHelper.WriteLog("======进入字幕功能操作======");
        }


        /// <summary>
        /// 执行切换界面信源
        /// </summary>
        /// <param name="n"></param>
        /// <param name="str"></param>
        private void select_Clik(int n,string str)
        {
            //byte m = (byte)n;
            //startProgress(10);//开启进度条显示
            //if (tabControl3.SelectedIndex == 0)
            string x = "";
            {
                for (int j = rowStar; j <= rowEnd; j++)
                {
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        int num = screens[((i + 1) + j * colsCount) - 1].Number;
                        if (Motherboard_flag == 4)
                        {
                            if (n == 3 || n == 6)
                            {
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + str + "1");//修改单元格的值
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + str + "1");
                                screens[j * sheet.ColumnCount + i].IntputType = str + "1";//改screens里对应屏幕的信源
                            }
                            else
                            {
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + str);//修改单元格的值
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + str);
                                screens[j * sheet.ColumnCount + i].IntputType = str;//改screens里对应屏幕的信源
                            }
                        }
                        else if (Motherboard_flag == 2)
                        {
                            if (n == 0 || n == 1 || n == 2 || n == 8)
                            {
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + str + "1");//修改单元格的值
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + str + "1");
                                screens[j * sheet.ColumnCount + i].IntputType = str + "1";//改screens里对应屏幕的信源
                            }
                            else
                            {
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + str);//修改单元格的值
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + str);
                                screens[j * sheet.ColumnCount + i].IntputType = str;//改screens里对应屏幕的信源
                            }
                        }
                        x += num.ToString() + " , ";
                        //if (!sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                            //Send_Signa((byte)num, (byte)m);
                    }
                }
                LogHelper.WriteLog("=====切换屏幕【" + x + "】的" + str + "信源=====");
            }
            /*
            if (tabControl3.SelectedIndex == 1)
            {
                for (int j = sheet_back.SelectionRange.Row; j <= sheet_back.SelectionRange.EndRow; j++)
                {
                    for (int i = sheet_back.SelectionRange.Col; i <= sheet_back.SelectionRange.EndCol; i++)
                    {
                        int num = screens[((i + 1) + j * colsCount) - 1].Number;
                        if (n == 3)
                        {
                            sheet.SetCellData(j, i, screens[j * sheet_back.ColumnCount + i].Name + " " + str + "1");//修改单元格的值
                            sheet_back.SetCellData(j, i, screens[j * sheet_back.ColumnCount + i].Name + " " + str + "1");
                            screens[j * sheet_back.ColumnCount + i].IntputType = str + "1";//改screens里对应屏幕的信源
                        }
                        else
                        {
                            sheet.SetCellData(j, i, screens[j * sheet_back.ColumnCount + i].Name + " " + str);
                            sheet_back.SetCellData(j, i, screens[j * sheet_back.ColumnCount + i].Name + " " + str);//修改单元格的值
                            screens[j * sheet_back.ColumnCount + i].IntputType = str;//改screens里对应屏幕的信源
                        }
                        //if (!sheet_back.IsMergedCell(sheet_back.SelectionRange.Row, sheet_back.SelectionRange.Col))
                            //Send_Signa((byte)num, (byte)m);
                    }
                }
            }
             */ 
            //stopProgress();
        }
        /// <summary>
        /// 执行指令发送
        /// </summary>
        /// <param name="rowS"></param>
        /// <param name="rowE"></param>
        /// <param name="colS"></param>
        /// <param name="colE"></param>
        /// <param name="m"></param>
        private void do_select_Clik(int rowS, int rowE, int colS, int colE, int m)
        {
            for (int j = rowS; j <= rowE; j++)
            {
                for (int i = colS; i <= colE; i++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    Send_Signa((byte)num, (byte)m);
                }
            }
            stopProgress();
        }

        /// <summary>
        /// 矩阵联动
        /// </summary>
        /// <param name="rowS"></param>
        /// <param name="rowE"></param>
        /// <param name="colS"></param>
        /// <param name="colE"></param>
        /// <param name="m"></param>
        /// <param name="ucDevice"></param>
        private void do_Siganl_Clik(int rowS, int rowE, int colS, int colE, byte m, string ucDevice)
        {
            for (int j = rowS; j <= rowE; j++)
            {
                for (int i = colS; i <= colE; i++)
                {
                    if (Matrix_flag == 0)
                    {
                        int num = screens[((i + 1) + j * colsCount) - 1].Number;
                        Send_Signa((byte)num, m);
                        string inputSingle = screens[((i + 1) + j * colsCount) - 1].IntputType;
                        if (inputSingle.Contains("("))
                            inputSingle = inputSingle.Split('(')[0];
                        //for (int k = 0; k < 2; k++)
                            UartSendSwitchMainSignal(inputSingle, (i + j * colsCount), 1, ucDevice);
                    }
                    else
                    {
                        string inputSingle = screens[((i + 1) + j * colsCount) - 1].IntputType;
                        if (inputSingle.Contains("("))
                            inputSingle = inputSingle.Split('(')[0];
                        //for (int k = 0; k < 2; k++)
                            UartSendSwitchMainSignal(inputSingle, (i + j * colsCount), 1, ucDevice);
                        int num = screens[((i + 1) + j * colsCount) - 1].Number;
                        Send_Signa((byte)num, (byte)m);
                    }
                    Thread.Sleep(Matrix_time);
                    //Console.WriteLine(Matrix_time);
                }
            }
            stopProgress();
        }

        /// <summary>
        /// 执行按键响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button30_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            if (Motherboard_type == 4)
            {
                int k = comboBox1.SelectedIndex;
                if (k < 5)
                {
                    startProgress(0);//开启进度条显示
                    //lock (thisLock)
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, k);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(k, comboBox1.Text);
                    //LogHelper.WriteLog("======切换信源【" + comboBox1.Text + "】======");
                }
                else
                {
                    int num1 = 0;
                    if (checkBox2.Checked && comboBox2.Items.Count > 0)
                    {
                        for (int i = colStar; i <= colEnd; i++)
                        {
                            for (int j = rowStar; j <= rowEnd; j++)
                            {
                                //PanelCount[num1] = (byte)screens[j * sheet.ColumnCount + i].Number;
                                if (comboBox1.SelectedIndex == 5)
                                {
                                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);
                                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);//修改单元格的值
                                    screens[j * sheet.ColumnCount + i].IntputType = "HDMI" + comboBox2.Text;//改screens里对应屏幕的信源
                                }
                                else if (comboBox1.SelectedIndex == 6)
                                {
                                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "DVI" + comboBox2.Text);
                                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "DVI" + comboBox2.Text);//修改单元格的值
                                    screens[j * sheet.ColumnCount + i].IntputType = "DVI" + comboBox2.Text;//改screens里对应屏幕的信源
                                }
                                num1++;
                            }
                        }
                        if (checkBox2.Checked && comboBox2.Items.Count > 0)
                        {
                            if (comboBox1.SelectedIndex == 5)
                            {
                                startProgress(0);
                                UartSendSwitchMainSignalCmd("HDMI" + comboBox2.Text, 0, num1, "HDMI",0x00);
                                //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x01));
                            }
                            if (comboBox1.SelectedIndex == 6)
                            {
                                startProgress(0);
                                UartSendSwitchMainSignalCmd("DVI" + comboBox2.Text, 0, num1, "DVI",0x03);
                                //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x02));
                            }
                        }
                    }
                }
            }
            else if (Motherboard_type == 0)
            {
                int k = comboBox3.SelectedIndex;
                if (k < 3)
                {
                    startProgress(0);//开启进度条显示
                    if (k == 1)
                        k = 2;
                    else if (k == 2)
                        k = 6;
                    //lock (thisLock)
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, k);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(k, comboBox3.Text);
                    //LogHelper.WriteLog("======切换信源【" + comboBox3.Text + "】======");
                }
                else
                {
                    int num1 = 0;
                    if (checkBox2.Checked && comboBox2.Items.Count > 0)
                    {
                        for (int i = colStar; i <= colEnd; i++)
                        {
                            for (int j = rowStar; j <= rowEnd; j++)
                            {
                                //PanelCount[num1] = (byte)screens[j * sheet.ColumnCount + i].Number;
                                if (comboBox3.SelectedIndex == 3)
                                {
                                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);
                                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);//修改单元格的值
                                    screens[j * sheet.ColumnCount + i].IntputType = "HDMI" + comboBox2.Text;//改screens里对应屏幕的信源
                                }
                                else if (comboBox3.SelectedIndex == 4)
                                {
                                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VGA" + comboBox2.Text);
                                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VGA" + comboBox2.Text);//修改单元格的值
                                    screens[j * sheet.ColumnCount + i].IntputType = "VGA" + comboBox2.Text;//改screens里对应屏幕的信源
                                }
                                num1++;
                            }
                        }
                        if (checkBox2.Checked && comboBox2.Items.Count > 0)
                        {
                            if (comboBox3.SelectedIndex == 3)
                            {
                                startProgress(0);
                                UartSendSwitchMainSignalCmd("HDMI" + comboBox2.Text, 0, num1, "HDMI",0x00);
                                //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x01));
                            }
                            if (comboBox3.SelectedIndex == 4)
                            {
                                startProgress(0);
                                UartSendSwitchMainSignalCmd("VGA" + comboBox2.Text, 0, num1, "VGA",0x06);
                                //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x02));
                            }
                        }
                    }
                }
            }
            else if (Motherboard_type == 1)
            {
                int k = comboBox4.SelectedIndex;
                if (k < 6)
                {
                    startProgress(0);//开启进度条显示
                    if (k == 5)
                        k = 6;
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, k);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(k, comboBox4.Text);
                    //LogHelper.WriteLog("======切换信源【" + comboBox4.Text + "】======");
                }
                else
                {
                    int num1 = 0;
                    if (checkBox2.Checked && comboBox2.Items.Count > 0)
                    {
                        for (int i = colStar; i <= colEnd; i++)
                        {
                            for (int j = rowStar; j <= rowEnd; j++)
                            {
                                //PanelCount[num1] = (byte)screens[j * sheet.ColumnCount + i].Number;
                                if (comboBox4.SelectedIndex == 6)
                                {
                                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);
                                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);//修改单元格的值
                                    screens[j * sheet.ColumnCount + i].IntputType = "HDMI" + comboBox2.Text;//改screens里对应屏幕的信源
                                }
                                else if (comboBox4.SelectedIndex == 7)
                                {
                                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "DVI" + comboBox2.Text);
                                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "DVI" + comboBox2.Text);//修改单元格的值
                                    screens[j * sheet.ColumnCount + i].IntputType = "DVI" + comboBox2.Text;//改screens里对应屏幕的信源
                                }
                                else if (comboBox4.SelectedIndex == 8)
                                {
                                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VGA" + comboBox2.Text);
                                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VGA" + comboBox2.Text);//修改单元格的值
                                    screens[j * sheet.ColumnCount + i].IntputType = "VGA" + comboBox2.Text;//改screens里对应屏幕的信源
                                }
                                num1++;
                            }
                        }
                        if (checkBox2.Checked && comboBox2.Items.Count > 0)
                        {
                            if (comboBox4.SelectedIndex == 6)
                            {
                                startProgress(0);
                                UartSendSwitchMainSignalCmd("HDMI" + comboBox2.Text, 0, num1, "HDMI",0x00);
                                //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x01));
                            }
                            if (comboBox4.SelectedIndex == 7)
                            {
                                startProgress(0);
                                UartSendSwitchMainSignalCmd("DVI" + comboBox2.Text, 0, num1, "DVI",0x03);
                                //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x02));
                            }
                            if (comboBox3.SelectedIndex == 8)
                            {
                                startProgress(0);
                                UartSendSwitchMainSignalCmd("VGA" + comboBox2.Text, 0, num1, "VGA",0x06);
                                //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x02));
                            }
                        }
                    }
                }
            }
            else if (Motherboard_type == 2)
            {
                int k = comboBox6.SelectedIndex;
                if (k < 9)
                {
                    startProgress(0);//开启进度条显示
                    //lock (thisLock)
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, k);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(k, comboBox6.Text);
                    //LogHelper.WriteLog("======切换信源【" + comboBox6.Text + "】======");
                }
                else if (k >= 9 && checkBox2.Checked && comboBox2.Items.Count > 0)
                {
                    int num1 = 0;
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        for (int j = rowStar; j <= rowEnd; j++)
                        {
                            //PanelCount[num1] = (byte)screens[j * sheet.ColumnCount + i].Number;
                            if (comboBox6.SelectedIndex == 9)
                            {
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VGA" + comboBox2.Text);
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VGA" + comboBox2.Text);//修改单元格的值
                                screens[j * sheet.ColumnCount + i].IntputType = "VGA" + comboBox2.Text;//改screens里对应屏幕的信源
                            }
                            else if (comboBox6.SelectedIndex == 10)
                            {
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VIDEO" + comboBox2.Text);
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VIDEO" + comboBox2.Text);//修改单元格的值
                                screens[j * sheet.ColumnCount + i].IntputType = "VIDEO" + comboBox2.Text;//改screens里对应屏幕的信源
                            }
                            else if (comboBox6.SelectedIndex == 11)
                            {
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);//修改单元格的值
                                screens[j * sheet.ColumnCount + i].IntputType = "HDMI" + comboBox2.Text;//改screens里对应屏幕的信源
                            }
                            else if (comboBox6.SelectedIndex == 12)
                            {
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "DVI" + comboBox2.Text);
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "DVI" + comboBox2.Text);//修改单元格的值
                                screens[j * sheet.ColumnCount + i].IntputType = "DVI" + comboBox2.Text;//改screens里对应屏幕的信源
                            }
                            num1++;
                        }
                    }
                    if (checkBox2.Checked && comboBox2.Items.Count > 0)
                    {
                        int A_0 = rowStar;
                        int A_1 = colStar + 1;
                        int B_0 = rowEnd;
                        int B_1 = colEnd + 1;
                        byte A = (byte)(A_1 + A_0 * colsCount);
                        byte B = (byte)(B_1 + B_0 * colsCount);
                        if (comboBox6.SelectedIndex == 9)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd("VGA" + comboBox2.Text, 0, num1, "VGA",0x00);
                            //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x00));
                        }
                        else if (comboBox6.SelectedIndex == 10)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd("VIDEO" + comboBox2.Text, 0, num1, "VIDEO",0x03);
                            //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x03));
                        }
                        else if (comboBox6.SelectedIndex == 11)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd("HDMI" + comboBox2.Text, 0, num1, "HDMI",0x01);
                            //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x01));
                        }
                        else if (comboBox6.SelectedIndex == 12)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd("DVI" + comboBox2.Text, 0, num1, "DVI",0x02);
                            //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x02));
                        }
                    }
                }
            }
            else if (Motherboard_type == 3)
            {
                int k = comboBox5.SelectedIndex;
                if (k < 4)
                {
                    startProgress(0);//开启进度条显示
                    //lock (thisLock)
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, k);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(k, comboBox5.Text);
                    //LogHelper.WriteLog("======切换信源【" + comboBox5.Text + "】======");
                }
                else if (k >= 4 && checkBox2.Checked && comboBox2.Items.Count > 0)
                {
                    int num1 = 0;
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        for (int j = rowStar; j <= rowEnd; j++)
                        {
                            //PanelCount[num1] = (byte)screens[j * sheet.ColumnCount + i].Number;
                            if (comboBox5.SelectedIndex == 4)
                            {
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VGA" + comboBox2.Text);
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "VGA" + comboBox2.Text);//修改单元格的值
                                screens[j * sheet.ColumnCount + i].IntputType = "VGA" + comboBox2.Text;//改screens里对应屏幕的信源
                            }
                            else if (comboBox5.SelectedIndex == 5)
                            {
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "HDMI" + comboBox2.Text);//修改单元格的值
                                screens[j * sheet.ColumnCount + i].IntputType = "HDMI" + comboBox2.Text;//改screens里对应屏幕的信源
                            }
                            else if (comboBox5.SelectedIndex == 6)
                            {
                                sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "DVI" + comboBox2.Text);
                                sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + "DVI" + comboBox2.Text);//修改单元格的值
                                screens[j * sheet.ColumnCount + i].IntputType = "DVI" + comboBox2.Text;//改screens里对应屏幕的信源
                            }
                            num1++;
                        }
                    }
                    if (checkBox2.Checked && comboBox2.Items.Count > 0)
                    {
                        if (comboBox6.SelectedIndex == 4)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd("VGA" + comboBox2.Text, 0, num1, "VGA",0x00);
                            //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x00));
                        }
                        else if (comboBox5.SelectedIndex == 5)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd("HDMI" + comboBox2.Text, 0, num1, "HDMI",0x01);
                            //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x01));
                        }
                        else if (comboBox5.SelectedIndex == 6)
                        {
                            startProgress(0);
                            UartSendSwitchMainSignalCmd("DVI" + comboBox2.Text, 0, num1, "DVI",0x02);
                            //Send_merge((byte)0x01, A, B, (byte)(0x08 | ((1 << 4) & 0xF0) | (0x08) | 0x02));
                        }
                    }
                }
            }
        }

        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
       /// <summary>
       /// 选择矩阵联动
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                Matrix_check_flag = true;
                label20.Enabled = true;
                comboBox2.Enabled = true;
                LogHelper.WriteLog("======打开矩阵联动设置======");
            }
            else
            {
                Matrix_check_flag = false;
                label20.Enabled = false;
                comboBox2.Enabled = false;
                LogHelper.WriteLog("======关闭矩阵联动设置======");
            }
        }
        /// <summary>
        /// 信号切换选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //comboBox2.Enabled = true;
            comboBox2.Items.Clear();
            
            if (comboBox1.SelectedIndex == 5)
            {
                if (uHdmi > 0)
                {
                    for (int i = 1; i <= uHdmi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                if (uDvi > 0)
                {
                    for (int i = 1; i <= uDvi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else
            {
                //comboBox2.Enabled = false;
                comboBox2.Items.Clear();
                comboBox2.Items.Add("");
                comboBox2.Text = "";
            }
            comboBox2.Refresh();
        }
        public bool Time_ok = true;
        /// <summary>
        /// 定时开功能执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_On_Tick(object sender, EventArgs e)
        {
            if (Timing_on)
            {
                //ts1 = DateTime.Now - on;
                if ((on.Subtract(DateTime.Now).TotalSeconds <= 30))
                //if ((DateTime.Now.Hour == on.Hour) && (DateTime.Now.Minute == on.Minute) && (DateTime.Now.Second - on.Second < 3) && (DateTime.Now.Second - on.Second > 0))
                //if ((ts1.Seconds > 0 && ts1.Seconds < 3) && (DateTime.Now.Hour == on.Hour) && (DateTime.Now.Minute == on.Minute))
                {
                    Timing_on = false;
                    timer_On.Stop();
                    Form_Tips f = new Form_Tips(this, true);
                    f.ShowDialog();
                    Thread.Sleep(200);
                    if (Time_ok)
                    {
                        //Power_on();
                        startProgress(0);
                        myThread = new Thread(new ThreadStart(Power_on)); //开线程         
                        myThread.Start(); //启动线程 
                        Timing_on = true;
                    }
                }
                Time_ok = true;
            }
        }
        /// <summary>
        /// 定时关功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Off_Tick(object sender, EventArgs e)
        {
            if (Timing_off)
            {
                //ts2 = DateTime.Now - off;
                //if ((DateTime.Now.Hour == off.Hour) && (DateTime.Now.Minute == off.Minute) && (DateTime.Now.Second - off.Second < 3) && (DateTime.Now.Second - off.Second > 0))
                //if ((ts2.Seconds > 0 && ts2.Seconds < 3) && (DateTime.Now.Hour == on.Hour) && (DateTime.Now.Minute == on.Minute))
                if ((off.Subtract(DateTime.Now).TotalSeconds <= 30))
                {
                    Timing_off = false;
                    timer_Off.Stop();
                    Form_Tips f = new Form_Tips(this, false);
                    f.ShowDialog();
                    Thread.Sleep(200);
                    if (Time_ok)
                    {
                        //Power_off();
                        startProgress(0);
                        myThread = new Thread(new ThreadStart(Power_off)); //开线程         
                        myThread.Start(); //启动线程 
                        Timing_off = true;
                    }
                }
                Time_ok = true;
            }
        }
        /// <summary>
        /// 序列号设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button31_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult;
            if (numericUpDown1.Value >= 0 && numericUpDown1.Value <= 16777214)
            {
                if (Chinese_English == 1)
                    dialogResult = MessageBox.Show("Make sure to set the serial number of the connected device!\r\n(Please confirm that the RS232 serial cable is disconnected and then operate)", "Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                else
                    dialogResult = MessageBox.Show("确定对连接设备的序号进行设置！\r\n(请先确认RS232串口线环接断开后再操作)", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK || dialogResult == DialogResult.Yes)
                {
                    Save_number((int)numericUpDown1.Value);
                }
            }
            else
            {
                if (Chinese_English == 1)
                    MessageBox.Show("Enter the correct serial number range (0 ~ 16777214)！", "Tips");
                else
                    MessageBox.Show("输入正确的序号范围（0~16777214）！", "提示");
            }
        }

        #region 信源操作
        /// <summary>
        /// VIDEO1信源
        /// </summary>
        private void vIDEO1ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }

                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                if (Motherboard_flag == 4)
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 0);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(0, item.Text);
                }
                else
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 3);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(3, item.Text);
                }
            }
            /*
            for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
            {
                for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    //Console.WriteLine(num);
#if to_3458
                    select_usb[num] = 0;
                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);//修改单元格的值
                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);
                    screens[j * sheet.ColumnCount + i].IntputType = item.Text;//改screens里对应屏幕的信源
                    //if (!sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                        Send_Signa((byte)num, (byte)0x00);
#else
                Send_Signa((byte)num,(byte)0x03);      
#endif
                }
            }
             * */
        }

        /// <summary>
        /// VIDEO2信源
        /// </summary>
        private void vIDEO2ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }

                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                if (Motherboard_flag == 4)
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 1);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(1, item.Text);
                }
                else
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 4);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(4, item.Text);
                }
            }
            /*
            for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
            {
                for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    //Console.WriteLine(num);
#if to_3458
                    select_usb[num] = 0;
                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);//修改单元格的值
                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);
                    screens[j * sheet.ColumnCount + i].IntputType = item.Text;//改screens里对应屏幕的信源  
                    //if (!sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                        Send_Signa((byte)num, (byte)0x01);
#else
                    Send_Signa((byte)num,(byte)0x04);      
#endif
                }
            }
             * */
        }

        /// <summary>
        /// VIDEO3信源
        /// </summary>
        private void vIDEO3ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                if (Motherboard_flag == 4)
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 2);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(2, item.Text);
                }
                else
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 5);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(5, item.Text);
                }
            }
            /*
            for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
            {
                for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    //Console.WriteLine(num);
#if to_3458
                    select_usb[num] = 0;
                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);//修改单元格的值
                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);
                    screens[j * sheet.ColumnCount + i].IntputType = item.Text;//改screens里对应屏幕的信源  
                    //if (!sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                        Send_Signa((byte)num, (byte)0x02);
#else
                    Send_Signa((byte)num,(byte)0x05);      
#endif
                }
            }
             * */
        }

        /// <summary>
        /// VIDEO4信源
        /// </summary>
        private void vIDEO4ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                if (Motherboard_flag == 4)
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 3);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(3, item.Text);
                }
                else
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 6);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(6, item.Text);
                }
            }
            /*
            for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
            {
                for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    //Console.WriteLine(num);
#if to_3458
                    select_usb[num] = 0;
                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);//修改单元格的值
                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);
                    screens[j * sheet.ColumnCount + i].IntputType = item.Text;//改screens里对应屏幕的信源  
                    //if (!sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                        Send_Signa((byte)num, (byte)0x03);
#else
                    Send_Signa((byte)num,(byte)0x06);      
#endif
                }
            }
             */
        }

        /// <summary>
        /// S-VIDEO信源
        /// </summary>
        private void sVIDEOToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                if (Motherboard_flag == 4)
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 4);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(4, item.Text);
                }
                else
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 7);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(7, item.Text);
                }
            }
            /*
            for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
            {
                for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number;
                    //Console.WriteLine(num);
#if to_3458
                    select_usb[num] = 0;
                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);//修改单元格的值
                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);
                    screens[j * sheet.ColumnCount + i].IntputType = item.Text;//改screens里对应屏幕的信源  
                    //if (!sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                        Send_Signa((byte)num, (byte)0x04);
#else
                    Send_Signa((byte)num,(byte)0x07);      
#endif
                }
            }
             * */
        }

        /// <summary>
        /// YPbPr信源
        /// </summary>
        private void yPbPrToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                if (Motherboard_flag == 4)
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 6);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(6, item.Text);
                }
                else
                {
                    startProgress(0);//开启进度条显示
                    {
                        myThread = new Thread(new ThreadStart(delegate()
                        {
                            do_select_Clik(rowStar, rowEnd, colStar, colEnd, 8);
                        })); //开线程          
                        myThread.Start(); //启动线程 
                    }
                    select_Clik(8, item.Text);
                }
            }
            /*
            for (int i = sheet.SelectionRange.Col; i <= sheet.SelectionRange.EndCol; i++)
            {
                for (int j = sheet.SelectionRange.Row; j <= sheet.SelectionRange.EndRow; j++)
                {
                    int num = screens[((i + 1) + j * colsCount) - 1].Number; 
#if to_3458
                    select_usb[num] = 1;//标记USB
                    sheet.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);//修改单元格的值
                    sheet_back.SetCellData(j, i, screens[j * sheet.ColumnCount + i].Name + " " + item.Text);
                    screens[j * sheet.ColumnCount + i].IntputType = item.Text;//改screens里对应屏幕的信源  
                    //Console.WriteLine("usb" + num);
                    if (!sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                        Send_Signa((byte)num, (byte)0x05);
#else
                    Send_Signa((byte)num,(byte)0x08);      
#endif
                }
            }
             * */
        }

        /// <summary>
        /// VGA信源
        /// </summary>
        private void vGAToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                startProgress(0);//开启进度条显示
                {
                    myThread = new Thread(new ThreadStart(delegate()
                    {
                        do_select_Clik(rowStar, rowEnd, colStar, colEnd, 0);
                    })); //开线程          
                    myThread.Start(); //启动线程 
                }
                select_Clik(0, item.Text);
            }
        }

        /// <summary>
        /// HDMI信源
        /// </summary>
        private void hDMIToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                startProgress(0);//开启进度条显示
                {
                    myThread = new Thread(new ThreadStart(delegate()
                    {
                        do_select_Clik(rowStar, rowEnd, colStar, colEnd, 1);
                    })); //开线程          
                    myThread.Start(); //启动线程 
                }
                select_Clik(1, item.Text);
            }
        }

        /// <summary>
        /// DVI信源
        /// </summary>
        private void dVIToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                ToolStripItem item = sender as ToolStripItem;
                startProgress(0);//开启进度条显示
                {
                    myThread = new Thread(new ThreadStart(delegate()
                    {
                        do_select_Clik(rowStar, rowEnd, colStar, colEnd, 2);
                    })); //开线程          
                    myThread.Start(); //启动线程 
                }
                select_Clik(2, item.Text);
            }
        }

        #endregion

        private bool MergedFlag = true;
        //public string data_ColorBelance;
        /// <summary>
        /// 屏幕参数调整部分
        /// </summary>
        private void 屏幕参数调整ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (serialPort1.IsOpen)
                {
                    try
                    {
                        serialPort1.Close();
                    }
                    catch
                    {
                        string ts = languageFile.ReadString("MESSAGEBOX", "M1", "串口出错！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                        return;
                    }
                }
                for (int i = 0; i < 256; i++)
                    select_address[i] = 0;
                for (int j = rowStar; j <= rowEnd; j++)
                {
                    for (int i = colStar; i <= colEnd; i++)
                    {
                        int num = screens[((i + 1) + j * colsCount) - 1].Number;
                        select_address[num] = (byte)num;
                        //Console.WriteLine("select_address[num] = " + num);
                    }
                }
                if (tabControl1.SelectedIndex == Motherboard_flag)
                {
                    MergedFlag = false;
                    if (sheet.IsMergedCell(sheet.SelectionRange.Row, sheet.SelectionRange.Col))
                    {
                        MergedFlag = true;
                    }
                    else
                    {
                        MergedFlag = false;
                    }
                }
                if (Motherboard_flag == 4)
                {
                    Form_Color f = new Form_Color(this, MergedFlag);
                    f.ShowDialog();
                }
                else
                {
                    Form_Color_59 f = new Form_Color_59(this, MergedFlag);
                    f.ShowDialog();
                }
                LogHelper.WriteLog("=====进行屏幕色彩参数调整操作=====");
                if (!serialPort1.IsOpen)
                {
                    try
                    {
                        serialPort1.Open();
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
        /// <summary>
        /// 背光开关线程
        /// </summary>
        /// <param name="rowS"></param>
        /// <param name="rowE"></param>
        /// <param name="colS"></param>
        /// <param name="colE"></param>
        private void Back_lightThread(int rowS, int rowE, int colS, int colE, bool on_off)
        {
            Screen[] screenTest = getSelectedScreen(rowS,rowE,colS,colE);
            for (int i = 0; i < screenTest.Length; i++)
            {
                //Console.WriteLine("poweroff number=" + screenTest[i].Number);
                Turn_on_off(on_off, screenTest[i].Number);
            }
            stopProgress();
        }
        /// <summary>
        /// 开背光
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 开屏电源ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }

                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                startProgress(0);
                myThread = new Thread(new ThreadStart(delegate()
                {
                    Back_lightThread(rowStar, rowEnd, colStar, colEnd, true);
                })); //开线程         
                myThread.Start(); //启动线程 
                LogHelper.WriteLog("=====打开背光操作=====");
                /*
                if (tabControl3.SelectedIndex == 0)
                {
                    Screen[] screenTest = getSelectedScreen();
                    for (int i = 0; i < screenTest.Length; i++)
                    {
                        //Console.WriteLine( "poweron number=" + screenTest[i].Number);
                        Turn_on_off(true, screenTest[i].Number);
                    }
                }
                if (tabControl3.SelectedIndex == 1)
                {
                    Screen[] screenTest = getSelectedScreen_back();
                    for (int i = 0; i < screenTest.Length; i++)
                    {
                        //Console.WriteLine( "poweron number=" + screenTest[i].Number);
                        Turn_on_off(true, screenTest[i].Number);
                    }
                }
                 */ 
            }
        }
        /// <summary>
        /// 关背光
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 关屏电源ToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                startProgress(0);
                myThread = new Thread(new ThreadStart(delegate()
                {
                    Back_lightThread(rowStar, rowEnd, colStar, colEnd, false);
                })); //开线程         
                myThread.Start(); //启动线程 
                LogHelper.WriteLog("=====关闭背光操作=====");

                /*
                if (tabControl3.SelectedIndex == 0)
                {
                    Screen[] screenTest = getSelectedScreen();
                    for (int i = 0; i < screenTest.Length; i++)
                    {
                        //Console.WriteLine("poweroff number=" + screenTest[i].Number);
                        Turn_on_off(false, screenTest[i].Number);
                    }
                }
                if (tabControl3.SelectedIndex == 1)
                {
                    Screen[] screenTest = getSelectedScreen_back();
                    for (int i = 0; i < screenTest.Length; i++)
                    {
                        //Console.WriteLine("poweroff number=" + screenTest[i].Number);
                        Turn_on_off(false, screenTest[i].Number);
                    }
                }
                 */ 
            }
        }
        /// <summary>
        /// 显示按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button32_Click(object sender, EventArgs e)
        {
            richTextBox2.Visible = true;
            pictureBox1.Visible = false;
            button32.Visible = false;
            button10.Visible = true;
            button13.Visible = true;
        }

        private void radioButton18_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton18.Checked)
            {
                radioButton19.Checked = false;
                Send_Func(0xD1);
                LogHelper.WriteLog("=====打开白场设置=====");
            }
        }

        private void radioButton19_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (radioButton19.Checked)
            {
                radioButton18.Checked = false;
                Send_Func(0xD2);
                LogHelper.WriteLog("=====关闭蓝屏设置=====");
            }
        }
        /// <summary>
        /// 工厂复位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button33_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }

            string tt = languageFile.ReadString("MESSAGEBOX", "M10", "确定对选中的屏幕单元进行工厂复位操作！");
            string th = languageFile.ReadString("MESSAGEBOX", "TP", "提示");

            DialogResult t = MessageBox.Show(tt, th, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            /*
            if (Chinese_English)
                t = MessageBox.Show(" Make sure to factory reset the selected screen unit !", " Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            else
                t = MessageBox.Show(" 确定对选中的屏幕单元进行工厂复位操作！", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
             */ 
            if (t == DialogResult.Yes || t == DialogResult.OK)
            {
                Send_Func(0xD0);
                radioButton11.Checked = true;
                radioButton10.Checked = true;
                radioButton3.Checked = true;
                radioButton2.Checked = true;
                radioButton19.Checked = true;
                radioButton7.Checked = true;
                radioButton6.Checked = true;
                radioButton16.Checked = true;
                Rest_form(false, false);
                LogHelper.WriteLog("=====进行工厂复位操作=====");
            }
        }

        private void screenNumberToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //点击的是鼠标左键
            {
                if (!Rs232Con)
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                if (systemRunning)//看系统是否在忙碌中,忙碌则提示
                {
                    promptSystemRun();
                    return;
                }
                f_m = false;
                new Form_Modify(this).ShowDialog();
                if (f_m)
                {
                    int num = rowStar;
                    int num1 = colStar + 1;
                    int num2 = screens[(num1 + num * colsCount) - 1].Number;
                    address_backup = (byte)num2;

                    Form_Addr f = new Form_Addr(this);
                    f.ShowDialog();
                    //Console.WriteLine(num2 + "===" + address.Length);
                    check_address[num2] = (byte)num2;
                    address[num2] = Address;
                    //Save_number(address[num2]);
                    //New_adress(address[num2], (byte)num2);
                }
            }
        }
        /// <summary>
        /// 信源选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();

            if (comboBox3.SelectedIndex == 3)
            {
                if (uHdmi > 0)
                {
                    for (int i = 1; i <= uHdmi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox3.SelectedIndex == 4)
            {
                if (uVga > 0)
                {
                    for (int i = 1; i <= uVga; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else
            {
                //comboBox2.Enabled = false;
                comboBox2.Items.Clear();
                comboBox2.Items.Add("");
                comboBox2.Text = "";
            }
            comboBox2.Refresh();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();

            if (comboBox4.SelectedIndex == 6)
            {
                if (uHdmi > 0)
                {
                    for (int i = 1; i <= uHdmi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox4.SelectedIndex == 7)
            {
                if (uDvi > 0)
                {
                    for (int i = 1; i <= uDvi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox4.SelectedIndex == 8)
            {
                if (uVga > 0)
                {
                    for (int i = 1; i <= uVga; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else
            {
                //comboBox2.Enabled = false;
                comboBox2.Items.Clear();
                comboBox2.Items.Add("");
                comboBox2.Text = "";
            }
            comboBox2.Refresh();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            if (comboBox5.SelectedIndex == 4)
            {
                if (uVga > 0)
                {
                    for (int i = 1; i <= uVga; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox5.SelectedIndex == 5)
            {
                if (uHdmi > 0)
                {
                    for (int i = 1; i <= uHdmi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox5.SelectedIndex == 6)
            {
                if (uDvi > 0)
                {
                    for (int i = 1; i <= uDvi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else
            {
                //comboBox2.Enabled = false;
                comboBox2.Items.Clear();
                comboBox2.Items.Add("");
                comboBox2.Text = "";
            }
            comboBox2.Refresh();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            if (comboBox6.SelectedIndex == 9)
            {
                if (uVga > 0)
                {
                    for (int i = 1; i <= uVga; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox6.SelectedIndex == 10)
            {
                if (uVideo > 0)
                {
                    for (int i = 1; i <= uVideo; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox6.SelectedIndex == 11)
            {
                if (uHdmi > 0)
                {
                    for (int i = 1; i <= uHdmi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox6.SelectedIndex == 12)
            {
                if (uDvi > 0)
                {
                    for (int i = 1; i <= uDvi; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else if (comboBox6.SelectedIndex == 13)
            {
                if (uYPbPr > 0)
                {
                    for (int i = 1; i <= uYPbPr; i++)
                    {
                        comboBox2.Items.Add(i);
                    }
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Add("");
                    comboBox2.Text = "";
                }
            }
            else
            {
                comboBox2.Items.Clear();
                comboBox2.Items.Add("");
                comboBox2.Text = "";
            }
            comboBox2.Refresh();
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        #region 语言
        private void Satuts_Language()
        {
            string s = settingFile.ReadString("SETTING", "Pwd", "0");
            if (s == "0")
            {
                toolStripStatusLabel2.Text = languageFile.ReadString("MAINFORM", "USER_GENERAL", "用户权限：  普通用户！"); //"User rights: General user！";
            }
            else
                toolStripStatusLabel2.Text = languageFile.ReadString("MAINFORM", "USER_ADMIN", "用户权限：  管理员用户！"); //"User rights: General user！";
            if (Rs232Con)
            {
                string sl = languageFile.ReadString("MAINFORM", "STATUS_ON", "打开");
                toolStripStatusLabel3.Text = PortName + "  " + sl + " " + BaudRate + " Bps";
            }
            else
            {
                string sl = languageFile.ReadString("MAINFORM", "STATUS_OFF", "关闭");
                toolStripStatusLabel3.Text = PortName + "  " + sl + " " + BaudRate + " Bps";
            }

            toolStripStatusLabel4.Text = languageFile.ReadString("MAINFORM", "SYS_STATUS", "系统状态：") + languageFile.ReadString("MAINFORM", "STATUS1", "空闲");
            
        }

        /// <summary>
        /// 中文简体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
            ApplyResource();
            package = "\\CH_package.ini";
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);

            Satuts_Language();
            Chinese_English = 0;
            toolStripMenuItem3.CheckState = CheckState.Checked;
            toolStripMenuItem4.CheckState = CheckState.Unchecked;
            toolStripMenuItem5.CheckState = CheckState.Unchecked;
            Swich_MatrixLag();
            button23.Visible = true;
        }
        /// <summary>
        /// 英语
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Chinese_English = 1;
            package = "\\EN_package.ini";
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            ApplyResource();

            Satuts_Language();
            toolStripMenuItem4.CheckState = CheckState.Checked;
            toolStripMenuItem3.CheckState = CheckState.Unchecked;
            toolStripMenuItem5.CheckState = CheckState.Unchecked;
            Swich_MatrixLag();
            button23.Visible = true;
        }
        /// <summary>
        /// 中文繁体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Chinese_English = 2;
            package = "\\TH_package.ini";
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + package);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
            ApplyResource();
            Satuts_Language();
            toolStripMenuItem5.CheckState = CheckState.Checked;
            toolStripMenuItem3.CheckState = CheckState.Unchecked;
            toolStripMenuItem4.CheckState = CheckState.Unchecked;
            Swich_MatrixLag();
            button23.Visible = false;
        }
        #endregion

        /// <summary>
        /// 隐藏序列号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button34_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            Send1(false);
        }
        /// <summary>
        /// 隐藏地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button35_Click(object sender, EventArgs e)
        {
            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "串口未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }
            Send2(false);
        }


        public void TcpSendMessage(byte[] buffer, int offset, int size)
        {
            for (int i = rowStar; i <= rowEnd; i++)
            {
                for (int j = colStar; j <= colEnd; j++)
                {
                    int num = ((j + 1) + i * colsCount) - 1;//对应的单元地址
                    //Console.WriteLine("u" + num);
                    if (TcpSendTP == 1)
                    {
                        if (TCPServer.m_clients.Count > num)
                            TCPServer.Send(TCPServer.m_clients[num].Socket, buffer, 0, buffer.Length, 10);
                        else
                        {
                            stopProgress();
                            return;
                        }
                    }
                    else if (TcpSendTP == 2)
                    {
                        TCPClient.Send(buffer);
                    }
                    else if (TcpSendTP == 3)
                    {
                        IPEndPoint remoteIPEndPoint = new IPEndPoint(IP, PORT);
                        UDPClient.Send(buffer, buffer.Length, remoteIPEndPoint);
                    }
                    else
                    {
                        MessageBox.Show("网络连接出错，检测网络连接情况!");
                        return;
                    }
                    //Console.WriteLine("select_address[num] = " + num);
                }
            }
        }

        public void TcpSendAllMessage(byte[] buffer, int offset, int size)
        {
            //int num = ((j + 1) + i * colsCount) - 1;//对应的单元地址
            //Console.WriteLine("u" + num);
            if (TcpSendTP == 1)
            {
                if (TCPServer.m_clients.Count > 0)
                {
                    for (int j = 0; j < TCPServer.m_clients.Count; j++)
                        TCPServer.Send(TCPServer.m_clients[j].Socket, buffer, 0, buffer.Length, 10);
                }
                else
                {
                    stopProgress();
                    return;
                }
            }
            else if (TcpSendTP == 2)
            {
                TCPClient.Send(buffer);
            }
            else if (TcpSendTP == 3)
            {
                IPEndPoint remoteIPEndPoint = new IPEndPoint(IP, PORT);
                UDPClient.Send(buffer, buffer.Length, remoteIPEndPoint);
            }
            else
            {
                MessageBox.Show("网络连接出错，检测网络连接情况!");
                return;
            }
            //Console.WriteLine("select_address[num] = " + num);
        }

        public void Reflash(List<AsyncUserToken> clientList)
        {
            com_List.Items.Clear();
            com_List.Items.Add("监听设备IP列表:");
            for (int i = 0; i < clientList.Count; i++)
            {
                com_List.Items.Add((i + 1).ToString() + "号: " + clientList[i].Remote.ToString());
            }
            com_List.SelectedIndex = 0;
        }

        private void Send_to_Scene(byte A_0,byte A_1)//  屏幕的预案管理指令
        {
            byte[] array = new byte[6];
            array[0] = 0xE6;
            array[1] = 0xFD;
            array[2] = 0X20;
            array[3] = A_0;
            array[4] = A_1;
            array[5] = (byte)(0xFF - (0xFF & array[0] + array[1] + array[2] + array[3] + array[4]));
            try
            {
                if (Rs232Con)
                {
                    //serialPort1.Write(array, 0, 5);
                    if (TCPCOM)
                    {
                        //TCPServer.Send(TCPServer.m_clients[0].Socket, array, 0, 10, 10);
                        TcpSendMessage(array, 0, 6);
                    }
                    else
                        SerialPortUtil.serialPortSendData(serialPort1, array, 0, 6, 150, 1);
                    //richTextBox2.AppendText(ToHexString(array, 5));
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
                richTextBox2.AppendText(ToHexString(array, 6));
            }));
        }

        private void button_save_scen_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }

            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "通讯未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            byte index = (byte)(comboBoxEx1.SelectedIndex + 1);
            string str = "Plan" + index.ToString() + ".rgf";
            FileInfo file = new FileInfo(currentConnectionName + "\\" + str);
            //currentSceneName = str;
            //currentSceneButtonText = bt.Text;
            if (file.Exists)//文件存在，通过文件初始化
            {
                saveSceneFile(currentConnectionName + "\\" + str);
            }

            Send_to_Scene(0xCC,index);
        }

        private void button_read_scen_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }

            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "通讯未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            byte index = (byte)(comboBoxEx1.SelectedIndex + 1);

            string str = "Plan" + index.ToString() + ".rgf";
            FileInfo file = new FileInfo(currentConnectionName + "\\" + str);
            //currentSceneName = str;
            //currentSceneButtonText = bt.Text;
            if ((!file.Exists))//文件存在，通过文件初始化
            {
                //initRoGridSet();
                //initRoGridControl(rowsCount, colsCount);
                saveSceneFile(currentConnectionName + "\\" + str);
            }
            else
            {
                readSceneFile(currentConnectionName + "\\" + str);
                reoGridControl1.Readonly = true;
                //reoGridControl2.Readonly = true;
            }

            Send_to_Scene(0xCD, index);
        }
        private int Index_scene;
        private void button_Loop_Click(object sender, EventArgs e)
        {
            if (systemRunning)//看系统是否在忙碌中,忙碌则提示
            {
                promptSystemRun();
                return;
            }

            if (!Rs232Con)
            {
                string ts = languageFile.ReadString("MESSAGEBOX", "M2", "通讯未连接！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            if (comboBox7.SelectedIndex == comboBox8.SelectedIndex || comboBox7.SelectedIndex > comboBox8.SelectedIndex)
                return;
            else
            {
                decimal times = numericUpDown4.Value;
                timer2.Interval = (int)(times * 1000);
                timer2.Enabled = true;
                Index_scene = comboBox7.SelectedIndex;
                startProgress(10);
                Form_WaitRun f1 = new Form_WaitRun(this, null, (int)times, 0);
                timer2.Start();
                f1.ShowDialog();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int start = comboBox7.SelectedIndex;
            int end = comboBox8.SelectedIndex;
            if (Index_scene >= start && Index_scene <= end)
                Index_scene++;
            else
                Index_scene = start + 1;

            Send_to_Scene(0xCD, (byte)Index_scene);
        }
        public void Stop_Loop()
        {
            timer2.Enabled = false;
            timer2.Stop();
            stopProgress();
        }
        private void button_st_Click(object sender, EventArgs e)
        {
            Stop_Loop();
        }

        private void button_rename_Click(object sender, EventArgs e)
        {
            int k = comboBoxEx1.SelectedIndex;
            settingFile.WriteInteger("SCENE", "INDEX", k);
            new Form_Scename(this, k + 1).ShowDialog();
            Init_Scene();
            comboBoxEx1.Refresh();
        }
    }
}
