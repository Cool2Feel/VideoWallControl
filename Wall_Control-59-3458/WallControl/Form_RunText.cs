using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;

namespace WallControl
{
    public partial class Form_RunText : Form
    {
        private Font f = null;
        private Color c = Color.Black;
        private Color bc = Color.Silver;
        private int speed = 3;
        private int Interval = 25;
        private double Transp = 1.0;
        private string text = "";
        private string back_text = "";
        private System.Timers.Timer timer = new System.Timers.Timer();
        //private Thread t;
        private bool tans_Flag = false;
        private bool image_Flag = false;
        //private PointF p;
        private Boolean m_IsFullScreen = false;//标记是否全屏
        private IniFiles settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
        //private int xx;
        //private int yy;
        //private int _border = 3;
        //private Form_BText fm;
        private IniFiles languageFile;
        public Form_RunText(IniFiles languageFile)
        {
            InitializeComponent();
            this.languageFile = languageFile;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            timer1.Enabled = true;
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
            }
            Init_FormString();
            //int xx = label_text.Location.X;
            //int yy = label_text.Location.Y;
            //foreach (var item in WindowsEnumerator.GetWindowHandles("Form_RunText"))
            //IntPtr hDeskTop = FindWindow(null, "Form_RunText");
            //MakeWindowTransparent(hDeskTop, 128); // 0~255 128是50%透明度
            //Console.WriteLine("ok------");
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_RunText));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            resources.ApplyResources(subTextToolStripMenuItem, subTextToolStripMenuItem.Name);
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }


        /// <summary>
        /// 语言设置加载
        /// </summary>
        private void Init_FormString()
        {
            this.subTextToolStripMenuItem.Text = languageFile.ReadString("TEXTFORM", "FONTSET", "字幕字体设置");
            this.subColorToolStripMenuItem.Text = languageFile.ReadString("TEXTFORM", "BACKCOLOR", "背景颜色设置");
            this.subTanstoolStripMenuItem1.Text = languageFile.ReadString("TEXTFORM", "TRANSPARENCY", "背景透明度");
            this.RighttoolStripMenuItem1.Text = languageFile.ReadString("TEXTFORM", "TL", "向右滚动");
            this.LefttoolStripMenuItem2.Text = languageFile.ReadString("TEXTFORM", "TR", "向左滚动");
            this.subStopToolStripMenuItem.Text = languageFile.ReadString("TEXTFORM", "ST", "停止滚动");
            this.subStartToolStripMenuItem.Text = languageFile.ReadString("TEXTFORM", "SR", "开始滚动");
            this.subCloseToolStripMenuItem.Text = languageFile.ReadString("TEXTFORM", "CT", "关闭字幕");
            this.AutotoolStripMenuItem1.Text = languageFile.ReadString("TEXTFORM", "AT", "自动调整显示");
            this.CenteredToolStripMenuItem.Text = languageFile.ReadString("TEXTFORM", "MC", "居中显示");
            this.LeftToolStripMenuItem.Text = languageFile.ReadString("TEXTFORM", "ML", "靠左显示");
            this.RightToolStripMenuItem.Text = languageFile.ReadString("TEXTFORM", "MR", "靠右显示");
        }


        private const int WS_DISABLED = 0x8000000;
        private const int WS_EX_LAYERED = 0x80000;        
        private const int WS_EX_TRANSPARENT = 0x20;       
        private const int GWL_STYLE = (-16);       
        private const int GWL_EXSTYLE = (-20);     
        private const int LWA_ALPHA = 1;        
        [DllImport("user32", EntryPoint = "SetWindowLong")]     
        private static extern int SetWindowLong(IntPtr hwnd,int nIndex,int dwNewLong);         
        [DllImport("user32", EntryPoint = "GetWindowLong")]       
        private static extern int GetWindowLong(IntPtr hwnd,int nIndex);          
        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);


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


        /// <summary>
        /// 对控件进行禁用Enabled设置
        /// </summary>
        /// <param name="c"></param>
        /// <param name="enabled"></param>
        public static void SetControlEnabled(Control c, bool enabled)
        {
            if (enabled)
            { 
                SetWindowLong(c.Handle, GWL_STYLE, (~WS_DISABLED) & GetWindowLong(c.Handle, GWL_STYLE));
            }
            else
            { 
                SetWindowLong(c.Handle, GWL_STYLE, WS_DISABLED + GetWindowLong(c.Handle, GWL_STYLE)); 
            }
        }

        ///         
        /// 设置窗体具有鼠标穿透效果        
        ///          
        public void SetPenetrate(Color b)
        {
            this.TopMost = true;
            GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
            SetLayeredWindowAttributes(this.Handle, GetCustomColor(b), 0, LWA_ALPHA);
        }

        private int GetCustomColor(Color color)
        {
            int nColor = color.ToArgb();
            int blue = nColor & 255;
            int green = nColor >> 8 & 255;
            int red = nColor >> 16 & 255;
            return Convert.ToInt32(blue << 16 | green << 8 | red);

        }

        /*
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
         */ 
 

        [DllImport("user32 ")]
        private static extern IntPtr FindWindows(string lpClassName, string lpWindowName);

        [DllImport("user32 ")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 0x10;
        const int HTBOTTOMRIGHT = 17;
        protected override void WndProc(ref Message m)
        {
            /*
            if (m.Msg == (int)WM.WM_NCHITTEST)
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);

                if (pos.X < 0 || pos.Y < 0)
                {
                    //非法位置
                }
                else if (pos.X <= _border)
                {
                    //左侧
                    if (pos.Y <= _border)
                    {
                        //左上侧
                        m.Result = (IntPtr)HT.HTTOPLEFT;
                        return;
                    }
                    else if (pos.Y >= this.Height - _border)
                    {
                        //左下侧
                        m.Result = (IntPtr)HT.HTBOTTOMLEFT;
                        return;
                    }
                    else
                    {
                        //左侧
                        m.Result = (IntPtr)HT.HTLEFT;
                        return;
                    }
                }
                else if (pos.X >= this.Width - _border)
                {
                    //右侧
                    if (pos.Y <= _border)
                    {
                        //右上侧
                        m.Result = (IntPtr)HT.HTTOPRIGHT;
                        return;
                    }
                    else if (pos.Y >= this.Height - _border)
                    {
                        //右下侧
                        m.Result = (IntPtr)HT.HTBOTTOMRIGHT;
                        return;
                    }
                    else
                    {
                        //右侧
                        m.Result = (IntPtr)HT.HTRIGHT;
                        return;
                    }
                }
                else
                {
                    //中部
                    if (pos.Y <= _border)
                    {
                        //上中侧
                        m.Result = (IntPtr)HT.HTTOP;
                        return;
                    }
                    else if (pos.Y >= this.Height - _border)
                    {
                        //下中侧
                        m.Result = (IntPtr)HT.HTBOTTOM;
                        return;
                    }
                    else
                    {
                    }
                }
                return;
            }
            else
            {
                base.WndProc(ref m);
            }
             */

            switch (m.Msg)
            {
                case 0x0084:
                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMLEFT;
                        else m.Result = (IntPtr)HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)HTBOTTOMRIGHT;
                        else m.Result = (IntPtr)HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)HTBOTTOM;
                    break;

                case 0x0201://鼠标左键按下的消息 用于实现拖动窗口功能
                    m.Msg = 0x00A1;//更改消息为非客户区按下鼠标
                    m.LParam = IntPtr.Zero;//默认值
                    m.WParam = new IntPtr(2);//鼠标放在标题栏内
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;
        }

        /*
         // hWnd是句柄，factor是透明度0~255
        bool MakeWindowTransparent(IntPtr hWnd, byte factor)
        {
            const int GWL_EXSTYLE = (-20);
            const uint WS_EX_LAYERED = 0x00080000;
            int Cur_STYLE = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, (uint)(Cur_STYLE | WS_EX_LAYERED));
            const uint LWA_COLORKEY = 1;
            const uint LWA_ALPHA = 2;
            const uint WHITE = 0xffffff;
            return SetLayeredWindowAttributes(hWnd, WHITE, factor, LWA_COLORKEY | LWA_ALPHA);
        }
        */

        // 枚举窗体
        public static class WindowsEnumerator
        {
            private delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);
            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam);
            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool EnumChildWindows(IntPtr hWndStart, EnumWindowsProc callback, IntPtr lParam);
            [DllImport("user32.dll", SetLastError = true)]
            static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
            [DllImport("user32.dll", SetLastError = true)]
            static extern int GetWindowTextLength(IntPtr hWnd);
            private static List<IntPtr> handles = new List<IntPtr>();
            private static string targetName;
            public static List<IntPtr> GetWindowHandles(string target)
            {
                targetName = target;
                EnumWindows(EnumWindowsCallback, IntPtr.Zero);
                return handles;
            }
            private static bool EnumWindowsCallback(IntPtr HWND, IntPtr includeChildren)
            {
                StringBuilder name = new StringBuilder(GetWindowTextLength(HWND) + 1);
                GetWindowText(HWND, name, name.Capacity);
                if (name.ToString() == targetName)
                    handles.Add(HWND);
                EnumChildWindows(HWND, EnumWindowsCallback, IntPtr.Zero);
                return true;
            }
        }

        private Color SetCustomColor(string c)
        { 
            switch (c)
            {
                case "Black":
                    return Color.DarkSlateGray;
                case "Blue":
                    return Color.RoyalBlue;
                case "Gold":
                    return Color.Goldenrod;
                case "Green":
                    return Color.ForestGreen;
                case "Lime":
                    return Color.PaleGreen;
                case "Linen":
                    return Color.AntiqueWhite;
                case "Maroon":
                    return Color.Firebrick;
                case "Navy":
                    return Color.MediumBlue;
                case "Olive":
                    return Color.OliveDrab;
                case "Orange":
                    return Color.Tomato;
                case "Pink":
                    return Color.LightPink;
                case "Plum":
                    return Color.Violet;
                case "Purple":
                    return Color.DarkViolet;
                case "Red":
                    return Color.Crimson;
                case "Salmon":
                    return Color.LightCoral;
                case "Silver":
                    return Color.DimGray;
                case "Teal":
                    return Color.DarkCyan;
                case "White":
                    return Color.WhiteSmoke;
                case "Yellow":
                    return Color.LightYellow;
                default:
                    return Color.IndianRed;
            };
        }

        public void SetInitText(Font f, Color c, Color bc, int interval, string s,int speed,double transp, bool tans,bool image)
        {
            //timer1.Start();
            this.f = f;
            this.c = c;
            this.bc = bc;
            this.Interval = interval;
            this.text = s;
            back_text = Reversal(s);
            this.speed = speed;
            this.Transp = transp;
            this.image_Flag = image;
            if (tans)
            {
                tans_Flag = true;
                //this.BackColor = bc;
                //this.TransparencyKey = c;
                //this.Opacity = Transp;
                //label_text.Parent = null;
                //SetPenetrate();
                Color b = SetCustomColor(c.Name);
                this.BackColor = b;  
                SetPenetrate(b);                                                                                                                                                                                                                                                                                                                                                                                    
                //this.TransparencyKey = b;
                label_text.BackColor = Color.Transparent;
                //if (Transp == 0.0)
                //tans_Flag = true;
                //Console.WriteLine(GetCustomColor(bc));
                
            }
            else
            {
                this.BackColor = bc;
                if (bc != Color.DimGray)
                    this.TransparencyKey = Color.DimGray;
                else
                    this.TransparencyKey = Color.Black;
                //this.Opacity = Transp;
            }
            //Console.WriteLine("====ok===");
        }

        private string Reversal(string input)
        {
            string result = "";
            for (int i = input.Length - 1; i >= 0; i--)
            {
                result += input[i];
            }
            //Console.WriteLine(result);
            return result;
        }

        private bool direction = true;
        //private string temp;  
        private void timer1_Tick(object sender, EventArgs e)
        {
            //this.TopMost = true;
            /*
            if (tans_Flag)
            {
                Graphics g = this.label_text.CreateGraphics();
                SizeF s = new SizeF();
                s = g.MeasureString(text, f);//测量文字长度  
                Brush brush = Brushes.Black;
                g.Clear(bc);//清除背景  

                if (temp != text)//文字改变时,重新显示  
                {
                    p = new PointF(this.label_text.Size.Width, 0);
                    temp = text;
                }
                else
                    p = new PointF(p.X - 3, 0);//每次偏移10  
                if (p.X <= -s.Width)
                    p = new PointF(this.label_text.Size.Width, 0);
                g.DrawString(text, f, brush, p);
                Console.WriteLine("====run");
            }
            else
            {
             */ 
                if (direction)
                {
                    label_text.Left -= (int)speed;
                    if (label_text.Right < 0)
                        label_text.Left = this.Width;
                }
                else
                {
                    label_text.Left += (int)speed;
                    if (label_text.Left > this.Width)
                        label_text.Left = -label_text.Width;
                }
            //}
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            //label_text.Refresh();

            /*
           Graphics g = this.label_text.CreateGraphics();
           SizeF s = new SizeF();
           s = g.MeasureString(text, f);//测量文字长度 
           Brush brush = Brushes.Red;
            
           g.Clear(bc);//清除背景 
           if (label_text.Text != text)//文字改变时,重新显示 
           {
               p = new PointF(this.label_text.Size.Width, 0);
               text = label_text.Text;
           }
           else
               p = new PointF(p.X - 10, 0);//每次偏移10  
           //p = new PointF(p.X - 6, 0);//每次偏移10 
           if (p.X <= -s.Width)
               p = new PointF(this.label_text.Size.Width, 0);
           g.DrawString(text, f, brush, p);
            /*
           if (this.label_text.Location.X + this.label_text.Size.Width > 0)
           {
               this.label_text.Location = new System.Drawing.Point(this.label_text.Location.X - 3, this.label_text.Location.Y);
           }
           else
           {
               this.label_text.Location = new System.Drawing.Point(xx, yy);
           }
             */ 
        }

        //声明委托
        private delegate void RunTextDelegate();
        private void RunText()
        {
            if (label_text.InvokeRequired)
            {
                //IntPtr i = label_text.Handle;
                label_text.Invoke(new RunTextDelegate(RunText));
            }
            else
            {
                if (direction)
                {
                    label_text.Left -= (int)speed;
                    if (label_text.Right < 0)
                        label_text.Left = this.Width;
                }
                else
                {
                    label_text.Left += (int)speed;
                    if (label_text.Left > this.Width)
                        label_text.Left = -label_text.Width;
                }
                //this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
                //label_text.Refresh();
            }
        }


        private void Form_RunText_Load(object sender, EventArgs e)
        {
            {
                label_text.Font = f;
                label_text.ForeColor = c;
                //Console.WriteLine("===" + c);
                //this.BackColor = bc;
                label_text.Text = text;
                if (text.Equals(""))
                {
                    label_text.Visible = false;
                    //contextMenuStrip1.Enabled = false;
                    subTextToolStripMenuItem.Enabled = false;
                    AutotoolStripMenuItem1.Enabled = false;
                }
                timer1.Interval = Interval;
                Lable_Resize();
                label_text.BackColor = Color.Transparent;
                if (tans_Flag)
                {
                    this.TopMost = true;
                    timer2.Enabled = true;
                }
                else
                {
                    if (image_Flag)
                    {
                        string s = settingFile.ReadString("SETTING", "PictureLogo", "");
                        //Console.WriteLine("s ====" + s);
                        if (!s.Equals(""))
                        {
                            s = s.Substring(s.Length - 4, 4);
                            s = Application.StartupPath + @"\pic\logoshow" + s;
                            if (this.BackgroundImage != null)
                                this.BackgroundImage.Dispose();
                            Image im = Image.FromFile(s);
                            this.BackgroundImage = new Bitmap(im);
                            this.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                        else
                        {
                            if (this.BackgroundImage != null)
                                this.BackgroundImage.Dispose();
                            //this.BackgroundImage = global::WallControl.Properties.Resources.上位机_英文版_MTC_New_Logo;
                            this.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                        timer1.Enabled = false;
                        //AutoAdjustShow();
                        label_text.TextAlign = ContentAlignment.MiddleCenter;
                        subStopToolStripMenuItem.Checked = true;
                        AutotoolStripMenuItem1.Enabled = true;
                        subStartToolStripMenuItem.CheckState = CheckState.Unchecked;
                        subStartToolStripMenuItem.Enabled = false;
                        subColorToolStripMenuItem.Enabled = false;
                        //int h = label_text.Size.Height;
                        this.Height = (this.Size.Width * 9) / 16;
                        //label_text.Location = new Point(w, (this.Size.Height - h) / 2);
                    }
                }
                //if (tans_Flag)
                {
                    if (Interval > 0 && !text.Equals(""))
                    {
                        /*
                        Thread t = new Thread(new ThreadStart(StartRunText));
                        t.IsBackground = true;
                        t.Start();
                         */
                        if (direction)
                            label_text.Left = this.Width;
                        else
                            label_text.Left = -label_text.Width;
                        if (!image_Flag)
                        {
                            timer1.Enabled = true;
                            timer1.Start();
                        }
                    }
                    else
                    {
                        timer1.Enabled = false;
                        //AutoAdjustShow();
                        label_text.TextAlign = ContentAlignment.MiddleCenter;
                        subStartToolStripMenuItem.CheckState = CheckState.Unchecked;
                        subStopToolStripMenuItem.CheckState = CheckState.Checked;
                        AutotoolStripMenuItem1.Enabled = true;
                        subStartToolStripMenuItem.Enabled = false;
                        //int h = label_text.Size.Height;
                        //int w = (this.Size.Width - label_text.Size.Width) / 2;
                        //label_text.Location = new Point(w, (this.Size.Height - h) / 2);
                    }
                    this.Activate();
                }
            }
            Lable_Resize();
            //IntPtr hDeskTop = FindWindow(null, "Form_RunText");
            //SetParent(this.Handle, hDeskTop);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            TopMostWindow.SetTopomost(this.Handle);
        }

        private void StartRunText()
        {
            timer.Interval = Interval;
            timer.Enabled = true;
            timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；  
            timer.Start();
            timer.Elapsed += (o, a) =>
            {
                RunText();
            };
        }


        private void Lable_Resize()
        {
            if (this.Height < label_text.Size.Height)
                this.Height = label_text.Size.Height + 30;
            int h = label_text.Size.Height;
            int w = 0;
            if (image_Flag || Interval <= 0)
                w = (this.Size.Width - label_text.Size.Width) / 2;
            else
                w = label_text.Location.X;
            label_text.Location = new Point(w, (this.Size.Height - h) / 2);
        }

        private Point offset;  
        private void Form_RunText_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = this.PointToScreen(e.Location);
            offset = new Point(cur.X - this.Left, cur.Y - this.Top);  
        }

        private void Form_RunText_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left != e.Button) return;

            Point cur = MousePosition;
            this.Location = new Point(cur.X - offset.X, cur.Y - offset.Y); 
        }

        private void Form_RunText_Resize(object sender, EventArgs e)
        {
            //label_text.Size = this.Size;
            Lable_Resize();
        }

        private void subTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!tans_Flag)
            {
                this.fontDialog1.Font = f;
                this.fontDialog1.Color = c;
                DialogResult result = fontDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    label_text.Font = fontDialog1.Font;
                    label_text.ForeColor = fontDialog1.Color;
                    Lable_Resize();
                    label_text.Refresh();
                }
            }
        }

        private void subColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!tans_Flag)
            {
                this.colorDialog1.Color = bc;
                DialogResult result = colorDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.BackColor = colorDialog1.Color;
                    this.Refresh();
                }
            }
        }

        private void subStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            subStartToolStripMenuItem.Checked = false;
            AutotoolStripMenuItem1.Enabled = true;
            timer1.Stop();
        }

        private void subStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            subStopToolStripMenuItem.Checked = false;
            AutotoolStripMenuItem1.Enabled = false; 
            CenteredToolStripMenuItem.CheckState = CheckState.Unchecked;
            RightToolStripMenuItem.CheckState = CheckState.Unchecked;
            LeftToolStripMenuItem.CheckState = CheckState.Unchecked;

            if (autoShow)
            {
                label_text.TextAlign = ContentAlignment.MiddleCenter;
                label_text.Dock = DockStyle.None;
                label_text.AutoSize = true;
                label_text.AutoEllipsis = false;
                //label_text.Enabled = true;
                SetControlEnabled(label_text, true);
                autoShow = false;
                Lable_Resize();
            }
            timer1.Start();
        }

        private void subCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Process.GetCurrentProcess().Kill();
            //this.BackgroundImage.Dispose();
            this.Dispose();
            this.Close();
        }


        public class TopMostWindow
        {
            public const int HWND_TOP = 0;
            public const int HWND_BOTTOM = 1;
            public const int HWND_TOPMOST = -1;
            public const int HWND_NOTOPMOST = -2;


            [DllImport("user32.dll")]
            public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);


            [DllImport("user32.dll")]
            public static extern bool GetWindowRect(IntPtr hWnd, out WindowRect lpRect);

            /// <summary>
            /// 设置窗体为TopMost
            /// </summary>
            /// <param name="hWnd"></param>
            public static void SetTopomost(IntPtr hWnd)
            {
                WindowRect rect = new WindowRect();
                GetWindowRect(hWnd, out rect);
                SetWindowPos(hWnd, (IntPtr)HWND_TOPMOST, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 1);
            }
        }

        public struct WindowRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void subTanstoolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void subTanstoolStripMenuItem1_CheckStateChanged(object sender, EventArgs e)
        {
            if (subTanstoolStripMenuItem1.Checked)
            {
                //this.Opacity = 1.0;
                //tans_Flag = true;
                if (!c.Equals(Color.Blue))
                {
                    //this.BackColor = Color.Blue;
                    //this.TransparencyKey = Color.Blue;
                    label_text.ForeColor = Color.PowderBlue;
                }
                else
                {
                    //this.BackColor = Color.Red;
                    //this.TransparencyKey = Color.Red;
                    label_text.ForeColor = Color.Gold;
                }
            }
            else
            {
                this.BackColor = bc;
                //tans_Flag = false;
                label_text.ForeColor = c;
            }
        }

        private void Form_RunText_MouseHover(object sender, EventArgs e)
        {
            if (false)
            {
                this.BackColor = bc;
                if (!c.Equals(Color.Blue))
                {
                    this.TransparencyKey = Color.Blue;
                }
                else
                    this.TransparencyKey = Color.Red;
            }
        }

        private void Form_RunText_MouseLeave(object sender, EventArgs e)
        {
            if (false)
            {
                if (!c.Equals(Color.Blue))
                {
                    this.BackColor = Color.Blue;
                    this.TransparencyKey = Color.Blue;
                }
                else
                {
                    this.BackColor = Color.Red;
                    this.TransparencyKey = Color.Red;
                }
            }
        }

        private void label_text_MouseHover(object sender, EventArgs e)
        {
            if (false)
            {
                this.BackColor = bc;
                if (!c.Equals(Color.White))
                {
                    this.TransparencyKey = Color.White;
                }
                else
                    this.TransparencyKey = Color.Black;
            }
        }
        Point downPoint;   
        private void label_text_MouseDown(object sender, MouseEventArgs e)
        {
            if (!tans_Flag)
            {
                downPoint = new Point(e.X, e.Y);
                timer1.Stop();
            }
        }

        private void label_text_MouseMove(object sender, MouseEventArgs e)
        {
            if (!tans_Flag)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Location = new Point(this.Location.X + e.X - downPoint.X,
                        this.Location.Y + e.Y - downPoint.Y);
                }
            }
        }

        private void label_text_MouseUp(object sender, MouseEventArgs e)
        {
            if (!tans_Flag)
            {
                if (subStartToolStripMenuItem.Checked)
                    timer1.Start();
            }
        }


        public void SetFont(Font font,Color color)
        {
            label_text.Font = font;
            label_text.ForeColor = color;
            Lable_Resize();
            label_text.Refresh();
        }

        public void TimeStart()
        {
            timer1.Start();
        }

        public void timeStart()
        {
            timer2.Start();
        }


        public void TimeStop()
        {
            timer1.Stop();
        }

        public void timeStop()
        {
            timer2.Stop();
        }

        public void SetActivate()
        {
            this.Show();
        }

        public void SetBackColor(Color c)
        {
            if (tans_Flag)
            {
                this.BackColor = c;
                bc = c;
                //SetPenetrate(c);
                this.Refresh();
            }
        }

        public void Form_Close()
        {
            timer1.Stop();
            this.Dispose();
            this.Close();
        }

        public void Change_Run(bool f)
        {
            if (f)
            {
                direction = true;
                label_text.Text = text;
            }
            else
            {
                direction = false;
                label_text.Text = back_text;
            }
            label_text.Refresh();
        }

        private void LefttoolStripMenuItem2_CheckedChanged(object sender, EventArgs e)
        {
            if (LefttoolStripMenuItem2.Checked)
            {
                direction = true;
                LefttoolStripMenuItem2.CheckState = CheckState.Checked;
                RighttoolStripMenuItem1.CheckState = CheckState.Unchecked;
            }
        }

        private void RighttoolStripMenuItem1_CheckedChanged(object sender, EventArgs e)
        {
            if (RighttoolStripMenuItem1.Checked)
            {
                direction = false;
                LefttoolStripMenuItem2.CheckState = CheckState.Unchecked;
                RighttoolStripMenuItem1.CheckState = CheckState.Checked;
            }
        }
        private bool autoShow = false;
        private void AutoAdjustShow()
        {
            label_text.Dock = DockStyle.Fill;
            label_text.AutoSize = false;
            label_text.AutoEllipsis = true;
            //label_text.Enabled = false;
            SetControlEnabled(label_text, false);
            label_text.ForeColor = c;
            autoShow = true;
        }

        private void CenteredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            if (!autoShow)
                AutoAdjustShow();
            label_text.TextAlign = ContentAlignment.MiddleCenter;
            //autoShow = true;
            LeftToolStripMenuItem.CheckState = CheckState.Unchecked;
            RightToolStripMenuItem.CheckState = CheckState.Unchecked;
        }

        private void LeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            if (!autoShow)
                AutoAdjustShow();
            label_text.TextAlign = ContentAlignment.MiddleLeft;
            //autoShow = true;
            CenteredToolStripMenuItem.CheckState = CheckState.Unchecked;
            RightToolStripMenuItem.CheckState = CheckState.Unchecked;
        }

        private void RightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = true;
            if (!autoShow)
                AutoAdjustShow();
            label_text.TextAlign = ContentAlignment.MiddleRight;
            //autoShow = true;
            LeftToolStripMenuItem.CheckState = CheckState.Unchecked;
            CenteredToolStripMenuItem.CheckState = CheckState.Unchecked;
        }

        private void label_text_Paint(object sender, PaintEventArgs e)
        {
            Graphics formGraphics = e.Graphics;
            formGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            formGraphics.DrawString("Hello World", f,Brushes.Firebrick, 20.0F, 20.0F);
        }
        
        private void timer2_Tick(object sender, EventArgs e)
        {
            this.TopMost = false;
            this.BringToFront();
            this.TopMost = true;
            
        }

        private void Form_RunText_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_IsFullScreen = !m_IsFullScreen;//点一次全屏，再点还原。 
            this.SuspendLayout();
            if (m_IsFullScreen)//全屏 ,按特定的顺序执行
            {
                SetFormFullScreen(m_IsFullScreen);
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                //timer1.Enabled = true;
                this.Activate();//
            }
            else//还原，按特定的顺序执行——窗体状态，窗体边框，设置任务栏和工作区域
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                SetFormFullScreen(m_IsFullScreen);
                //this.buttonX1.Visible = true;
                //timer1.Enabled = false;
                Cursor.Show();
                this.Activate();
            }
            //pictureBox1.Dock = DockStyle.Fill;
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

    }
}
