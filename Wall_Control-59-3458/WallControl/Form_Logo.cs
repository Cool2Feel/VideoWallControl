using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace WallControl
{
    public partial class Form_Logo : Form
    {
        private Boolean m_IsFullScreen = false;//标记是否全屏
        private IniFiles settingFile;
        private string filePath = Application.StartupPath + @"\pic";
        private int ch_en = 0;
        public Form_Logo(int mf)
        {
            InitializeComponent();
            this.pictureBox1.AllowDrop = true;
            this.ch_en = mf;
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            if (mf == 1)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
            }
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Logo));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern Int32 ShowWindow(Int32 hwnd, Int32 nCmdShow);
        public const Int32 SW_SHOW = 5; public const Int32 SW_HIDE = 0;

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        private static extern Int32 SystemParametersInfo(Int32 uAction, Int32 uParam, ref Rectangle lpvParam, Int32 fuWinIni);
        public const Int32 SPIF_UPDATEINIFILE = 0x1;
        public const Int32 SPI_SETWORKAREA = 47;
        public const Int32 SPI_GETWORKAREA = 48;

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern Int32 FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public extern static void ShowCursor(int status);


        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_IsFullScreen = !m_IsFullScreen;//点一次全屏，再点还原。 
            this.SuspendLayout();
            if (m_IsFullScreen)//全屏 ,按特定的顺序执行
            {
                SetFormFullScreen(m_IsFullScreen);
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.buttonX1.Visible = false;
                timer1.Enabled = true;
                this.Activate();//
            }
            else//还原，按特定的顺序执行——窗体状态，窗体边框，设置任务栏和工作区域
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                SetFormFullScreen(m_IsFullScreen);
                this.buttonX1.Visible = true;
                timer1.Enabled = false;
                Cursor.Show();
                this.Activate();
            }
            pictureBox1.Dock = DockStyle.Fill;
            //button1.Anchor = AnchorStyles.Bottom;
            //button1.Anchor = AnchorStyles.Right;
            this.ResumeLayout();
            this.Refresh();
        }

        /// <summary> 
        /// 设置全屏或这取消全屏 
        /// </summary> 
        /// <param name="fullscreen">true:全屏 false:恢复</param> 
        /// <param name="rectOld">设置的时候，此参数返回原始尺寸，恢复时用此参数设置恢复</param> 
        /// <returns>设置结果</returns> 
        public Boolean SetFormFullScreen(Boolean fullscreen)//, ref Rectangle rectOld
        {
            Rectangle rectOld = Rectangle.Empty;
            Int32 hwnd = 0;
            hwnd = FindWindow("Shell_TrayWnd", null);//获取任务栏的句柄

            if (hwnd == 0) return false;

            if (fullscreen)//全屏
            {
                ShowWindow(hwnd, SW_HIDE);//隐藏任务栏

                SystemParametersInfo(SPI_GETWORKAREA, 0, ref rectOld, SPIF_UPDATEINIFILE);//get 屏幕范围
                Rectangle rectFull = System.Windows.Forms.Screen.PrimaryScreen.Bounds;//全屏范围
                SystemParametersInfo(SPI_SETWORKAREA, 0, ref rectFull, SPIF_UPDATEINIFILE);//窗体全屏幕显示
            }
            else//还原 
            {
                ShowWindow(hwnd, SW_SHOW);//显示任务栏

                SystemParametersInfo(SPI_SETWORKAREA, 0, ref rectOld, SPIF_UPDATEINIFILE);//窗体还原
            }
            return true;
        }

        private void Form_Logo_KeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine(e.KeyCode.ToString());
            if (e.KeyCode == Keys.F11)
            {
                //button1.PerformClick();
                m_IsFullScreen = true;
                e.Handled = true;
                SetFormFullScreen(m_IsFullScreen);
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.buttonX1.Visible = false;
                timer1.Enabled = true;
                this.Activate();//
            }
            else if (e.KeyCode == Keys.Escape)//esc键盘退出全屏
            {
                if (m_IsFullScreen)
                {
                    m_IsFullScreen = false;
                    e.Handled = true;
                    this.WindowState = FormWindowState.Normal;//还原 
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                    SetFormFullScreen(false);
                    this.buttonX1.Visible = true;
                    timer1.Enabled = false;
                    Cursor.Show();
                    this.Activate();
                }
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "图片文件|*.bmp;*.jpg;*.gif;*.png";
            String sourcePath;
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = openfile.FileName;
                sourcePath = openfile.FileName;

                string strtype = sourcePath.Substring(sourcePath.Length - 4, 4);
                //Console.WriteLine("strtype:" + strtype);
                string targetPath = filePath + @"\logoshow" + strtype;
                bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
                try
                {
                    System.IO.File.Copy(sourcePath, targetPath, isrewrite);
                }
                catch
                {

                }
                settingFile.WriteString("SETTING", "PictureLogo", "logoshow" + strtype);
            }
            //button3.Enabled = true;
            openfile.Dispose();

            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //m_IsFullScreen = !m_IsFullScreen;//点一次全屏，再点还原。 
            this.SuspendLayout();
            if (m_IsFullScreen)//全屏 ,按特定的顺序执行
            {
                SetFormFullScreen(m_IsFullScreen);
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.buttonX1.Visible = false;
                timer1.Enabled = true;
                this.Activate();//
            }
            else//还原，按特定的顺序执行——窗体状态，窗体边框，设置任务栏和工作区域
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                SetFormFullScreen(m_IsFullScreen);
                this.buttonX1.Visible = true;
                timer1.Enabled = false;
                Cursor.Show();
                this.Activate();
            }
            pictureBox1.Dock = DockStyle.Fill;
            //button1.Anchor = AnchorStyles.Bottom;
            //button1.Anchor = AnchorStyles.Right;
            this.ResumeLayout();
            this.Refresh();
        }

        private void Form_Logo_Load(object sender, EventArgs e)
        {
            if (!System.IO.Directory.Exists(filePath))
            {
                // 目录不存在，建立目录
                System.IO.Directory.CreateDirectory(filePath);
            }

            string s = settingFile.ReadString("SETTING", "PictureLogo", "");
            //Console.WriteLine("s ====" + s);
            if (!s.Equals(""))
            {
                s = s.Substring(s.Length - 4, 4);
                s = Application.StartupPath + @"\pic\logoshow" + s;
                pictureBox1.Image.Dispose();
                pictureBox1.Load(s);
            }
            //timer1.Enabled = true;
            //timer1.Stop();
            //ShowCursor(0);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //timer1.Stop();
            Cursor.Show();
            index = 0;
            //timer1.Start();
            //Console.WriteLine("MMMM");
            //ShowCursor(1);
        }
        private int index = 0;
        private int x=0;
        private int y = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            Point p = Cursor.Position;
            //Console.WriteLine(p);
            if (p.X == x && p.Y == y)
            {
                index++;
                if (index >= 20)
                    Cursor.Hide();
            }
            else
            {
                Cursor.Show();
                index = 0;
            }

            x = Cursor.Position.X;
            y = Cursor.Position.Y;
        }


        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            //Console.WriteLine("=====mos");
            //timer1.Enabled = true;
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.ReshowDelay = 300;
            if(ch_en == 1)
                toolTip1.SetToolTip(pictureBox1, "Double click to display full screen logo\n(Support the dragging of the image file)！");
            else
                toolTip1.SetToolTip(pictureBox1, "鼠标双击进行全屏Logo显示 \n(支持图片文件的拖入加载)！");

            timer1.Interval = 50;
            if(timer1.Enabled)
                timer1.Start();
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            this.pictureBox1.Image = Image.FromFile(fileName);

            //Console.WriteLine(fileName);
            string sourcePath = fileName;

            string strtype = sourcePath.Substring(sourcePath.Length - 4, 4);
            //Console.WriteLine("strtype:" + strtype);
            string targetPath = filePath + @"\logoshow" + strtype;
            bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
            try
            {
                System.IO.File.Copy(sourcePath, targetPath, isrewrite);
            }
            catch
            {

            }
            settingFile.WriteString("SETTING", "PictureLogo", "logoshow" + strtype);

        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;   
        }

        private void buttonX1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.AutoPopDelay = 4000;
            toolTip1.InitialDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.ReshowDelay = 300;
            if(ch_en == 1)
                toolTip1.SetToolTip(buttonX1, "Select the Logo image！");
            else
                toolTip1.SetToolTip(buttonX1, "对Logo图片进行选择！");
        }


    }
}
