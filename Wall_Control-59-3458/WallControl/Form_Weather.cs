using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WallControl
{
    public partial class Form_Weather : Form
    {
        private Font f = null;
        private Color c = Color.Black;
        private Color bc = Color.Blue;
        private string city = "北京";
        private string weather = "30 °C";
        private string text = "";
        private bool tans = false;
        private string picture1 = "";
        private string picture2 = "";
        private string path = Application.StartupPath + "\\weather1";

        private cn.com.webxml.www.WeatherWebService w = new WallControl.cn.com.webxml.www.WeatherWebService();
        public Form_Weather()
        {
            InitializeComponent();
        }

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

        private void Form_Weather_Load(object sender, EventArgs e)
        {
            label_city.Text = city;
            label_weather.Text = weather;
            richTextBox_all.Text = text;
            //richTextBox_all.BackColor = System.Drawing.Color.Transparent;
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists)
            {
                foreach (FileSystemInfo i in directoryInfo.GetFileSystemInfos())
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                    }
                    else
                    {
                        if (i.Name.Equals(picture1))      //删除指定文件
                        {
                            pictureBox1.Load(i.FullName);
                            //Console.WriteLine(i.FullName);
                        }
                        if (i.Name.Equals(picture2))
                        {
                            pictureBox2.Load(i.FullName);
                        }
                        //Console.WriteLine(i.FullName);
                    }
                }
            }
            this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.Width - 20;
            this.Top = 20;
            timer1.Enabled = false;
            //timer1.Start();
        }

        public void SetInitText(Font f, Color c, Color bc, string city,string weather,string text,string pic1,string pic2, bool tans)
        {
            this.f = f;
            this.c = c;
            this.bc = bc;
            this.text = text;
            this.city = city;
            this.weather = weather;
            this.tans = tans;
            this.picture1 = pic1;
            this.picture2 = pic2;
            if (tans)
            {
                //this.BackColor = Color.White;
                //this.TransparencyKey = Color.White;
                //this.Opacity = 1.0;//Transp;
            }
            else
            {
                //this.BackColor = bc;
                //this.TransparencyKey = Color.White;
                //this.Opacity = 1.0;
            }
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private Point downPoint;

        private void form_MouseDown(object sender, MouseEventArgs e)
        {
            downPoint = new Point(e.X, e.Y);
        }

        private void form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }  
        }

        private void Form_Weather_SizeChanged(object sender, EventArgs e)
        {
            if (this.Size.Width <= 345)
            {
                this.Size = new Size(345, this.Size.Height);
            }
            if (this.Size.Height <= 260)
            {
                this.Size = new Size(this.Size.Width, 260);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string[] s = w.getWeatherbyCityName(label_city.Text.Trim());
                
                string text_new = label_city.Text + "  ";
                //for (int i = 5; i < 11; i++)
                {
                    text_new += s[6] + "\r\n" + s[7] + "\r\n" + s[10];
                }
                
                this.Invoke(new MethodInvoker(delegate()
                {
                    label_weather.Text = s[5];
                    richTextBox_all.ReadOnly = false;
                    richTextBox_all.Clear();
                    //richTextBox_all.Text = text_new;
                    richTextBox_all.Refresh();
                }));

                Console.WriteLine(text_new);
                //richTextBox_all.BackColor = System.Drawing.Color.Transparent;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists)
                {
                    foreach (FileSystemInfo i in directoryInfo.GetFileSystemInfos())
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            continue;
                        }
                        else
                        {
                            if (i.Name.Equals(s[8]))      //删除指定文件
                            {
                                pictureBox1.Image.Dispose();
                                pictureBox1.Load(i.FullName);
                                //Console.WriteLine(i.FullName);
                            }
                            if (i.Name.Equals(s[9]))
                            {
                                pictureBox2.Image.Dispose();
                                pictureBox2.Load(i.FullName);
                            }
                            //Console.WriteLine(i.FullName);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("网络连接异常！", "提示");
                return;
            }

        }   

    }
}
