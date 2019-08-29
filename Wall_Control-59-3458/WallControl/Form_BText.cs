using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WallControl
{
    public partial class Form_BText : Form
    {
        public Font f = null;
        private Color c = Color.Black;
        private Color bc = Color.Silver;
        public int speed = 3;
        public int Interval = 25;
        public double Transp = 1.0;
        public string text = "";
        //private bool mf;
        public bool tans_Flag = false;
        private Form_RunText bf;//背景窗口
        private IniFiles languageFile;
        public Form_BText(IniFiles languageFile)
        {
            InitializeComponent();
            this.languageFile = languageFile;
            //Console.WriteLine("====first----");
            //bf.Show();//显示背景窗体 
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
            ApplyResource();
            Init_FormString();

        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BText));
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

        }



        //private const int WS_DISABLED = 0x8000000;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0;
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 0x10;
        private const int HTBOTTOMRIGHT = 17;
        
        protected override void WndProc(ref Message m)
        {
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
        }

        public void SetInitText(Font f, Color c, Color bc, int interval, string s, int speed, double transp, bool tans)
        {
            //timer1.Start();
            this.f = f;
            this.c = c;
            this.bc = bc;
            this.Interval = interval;
            this.text = s;
            this.speed = speed;
            this.Transp = transp;
            if (tans)
            {
                tans_Flag = true;
            }
            else
            {
                tans_Flag = false;
            }
            bf = new Form_RunText(languageFile);
            bf.Size = this.Size;//同步宽度
            this.BackColor = bc;
            if (tans_Flag)
            {
                //Console.WriteLine(Transp);
                subColorToolStripMenuItem.Enabled = false;
                this.Opacity = Transp;//背景窗体透明
                this.TopMost = true;
                //bf.TopMost = true;
                //bf.BackColor = Color.Blue;//设置主窗体透明颜色 这里我是随便设的 大家可以找个用不到的颜色来设置透明色
                //bf.TransparencyKey = Color.Blue;//将指定颜色设置为透明色
                if (bc == Color.White)
                    this.TransparencyKey = Color.Black;
                else
                    this.TransparencyKey = Color.White;
            }
            else
            {
                subColorToolStripMenuItem.Enabled = true;
                bf.BackColor = bc;//设置主窗体透明颜色 这里我是随便设的 大家可以找个用不到的颜色来设置透明色
                bf.TransparencyKey = bc;//将指定颜色设置为透明色
            }
            //Console.WriteLine("122222221");
        }

        private void Lable_Resize()
        {
            if (this.Height < label_text.Size.Height)
                this.Height = label_text.Size.Height + 30;
            int h = label_text.Size.Height;
            int w = label_text.Location.X;
            label_text.Location = new Point(w, (this.Size.Height - h) / 2);
        }

        private void Form_BText_Load(object sender, EventArgs e)
        {
            //Console.WriteLine("1223");
            //label_text.Text = text;
            //Lable_Resize();
            if (tans_Flag)
            {
                timer1.Enabled = true;
                timer1.Stop();
                bf.SetInitText(f, c, bc, Interval, text, speed, Transp, tans_Flag,false);
                bf.Location = this.Location;
                bf.Show();
            }
            //Console.WriteLine("333333");
            //timer1.Enabled = true;
            //timer1.Interval = 10;
            //timer1.Start();
        }

        private void Form_BText_LocationChanged(object sender, EventArgs e)
        {
            try { bf.Location = this.Location; }
            catch { }
        }

        private void Form_BText_Resize(object sender, EventArgs e)
        {
            try { bf.Size = this.Size; }
            catch { }
        }

        private void subTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.fontDialog1.Font = f;
            this.fontDialog1.Color = c;
            DialogResult result = fontDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                //label_text.Font = fontDialog1.Font;
                //label_text.ForeColor = fontDialog1.Color;
                bf.SetFont(fontDialog1.Font, fontDialog1.Color);
            }
        }

        private void subColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = bc;
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.BackColor = colorDialog1.Color;
                bf.SetBackColor(colorDialog1.Color);
                this.Refresh();
            }
        }

        private void subStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subStartToolStripMenuItem.Checked = false;
            bf.TimeStop();
        }

        private void subStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            subStopToolStripMenuItem.Checked = false;
            bf.TimeStart();
        }

        private void subCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bf.Form_Close();
            this.Close();
        }

        private void Form_BText_MouseDown(object sender, MouseEventArgs e)
        {
            bf.TimeStop();
        }

        private void Form_BText_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void RighttoolStripMenuItem1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void LefttoolStripMenuItem2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void LefttoolStripMenuItem2_Click(object sender, EventArgs e)
        {
            bf.Change_Run(true);
            LefttoolStripMenuItem2.CheckState = CheckState.Checked;
            RighttoolStripMenuItem1.CheckState = CheckState.Unchecked;
        }

        private void RighttoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bf.Change_Run(false);
            LefttoolStripMenuItem2.CheckState = CheckState.Unchecked;
            RighttoolStripMenuItem1.CheckState = CheckState.Checked;
        }
        /// <summary>
        /// 窗体对鼠标的穿透
        /// </summary>
        /// <param name="f"></param>
        public void Set_Penetrate(bool f)
        {
            //this.TopMost = true;
            if (tans_Flag)
            {
                GetWindowLong(this.Handle, GWL_EXSTYLE);
                if (f)
                    SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
                else
                    SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_LAYERED);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void Form_BText_MouseLeave(object sender, EventArgs e)
        {
            bf.timeStop();
            //timer1.Stop();
        }

        private void Form_BText_MouseEnter(object sender, EventArgs e)
        {
            bf.timeStart();
            //timer1.Start();
        }



    }
}
