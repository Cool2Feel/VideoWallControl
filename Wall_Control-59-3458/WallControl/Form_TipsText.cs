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
    public partial class Form_TipsText : Form
    {
        private IniFiles settingFile;
        private Font f = new Font("微软雅黑", 36);
        private Color c = Color.Black;
        private Color bc = Color.Silver;
        private string temp;
        private int spd = 25;
        private int speed = 3;
        private IniFiles languageFile;
        private cn.com.webxml.www.WeatherWebService w = new WallControl.cn.com.webxml.www.WeatherWebService();
        //private FontConverter fc = new FontConverter();
        //private ColorConverter cct = new ColorConverter();
        //private bool mf;
        private Thread myThread = null;
        //private Boolean m_IsFullScreen = false;//标记是否全屏
        private string filePath = Application.StartupPath + @"\pic";

        public Form_TipsText(MainForm mf)
        {
            InitializeComponent();
            this.pictureBox1.AllowDrop = true;
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");

            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + mf.package);
            //mf = f.Chinese_English;
            Init_FormString();
            
            if (mf.Chinese_English == 1)
            {
                button3.Enabled = false;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
            }
            else if (mf.Chinese_English == 0)
            {
                button3.Enabled = true;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
            }
            else
            {
                button3.Enabled = false;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
                ApplyResource();
            }
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_TipsText));
            resources.ApplyResources(button1, button1.Name);
            resources.ApplyResources(button2, button2.Name);
            resources.ApplyResources(button3, button3.Name);
            resources.ApplyResources(button_font, button_font.Name);
            resources.ApplyResources(buttonX1, buttonX1.Name);
            /*
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
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
            this.Text = languageFile.ReadString("TEXTFORM", "TITLE", "字幕设置");
            this.label3.Text = languageFile.ReadString("TEXTFORM", "TEXT_T", "字幕文本");
            this.label6.Text = languageFile.ReadString("TEXTFORM", "BACKGROUND", "背景/Logo图片");
            this.label5.Text = languageFile.ReadString("TEXTFORM", "COLOR", "字体显示颜色");
            this.label1.Text = languageFile.ReadString("TEXTFORM", "INTERVAL", "时间间隔");
            this.label2.Text = languageFile.ReadString("TEXTFORM", "SPEED", "滚动速度");
            this.label4.Text = languageFile.ReadString("TEXTFORM", "CITY_TEXT", "城市气象字幕");
            this.button_font.Text = languageFile.ReadString("TEXTFORM", "FONT", "字体设置");
            this.button2.Text = languageFile.ReadString("TEXTFORM", "BACKCOLOR", "背景颜色设置");
            this.button1.Text = languageFile.ReadString("TEXTFORM", "SHOW", "应用显示");
            this.button3.Text = languageFile.ReadString("TEXTFORM", "DISPLAY", "显示天气");
            this.buttonX1.Text = languageFile.ReadString("TEXTFORM", "PIC_SELECT", "图片选择");
            this.checkBox2.Text = languageFile.ReadString("TEXTFORM", "CROSS", "鼠标穿透");
            this.checkBox1.Text = languageFile.ReadString("TEXTFORM", "TRANSPARENCY", "背景透明度");
            this.checkBox3.Text = languageFile.ReadString("TEXTFORM", "SHOW_PIC", "显示背景图片");

        }

        Form_BText ft = null;
        private List<Form_BText> formList = new List<Form_BText>();
        private void button1_Click(object sender, EventArgs e)
        {
            //label1.Text = richTextBox1.Text;
            temp = richTextBox1.Text;
            //if (temp.Equals(""))
            //{
            //    if (mf)
            //        MessageBox.Show("Please enter the captions you want to display.！", "Tips");
            //    else
            //        MessageBox.Show("请输入需要显示的字幕！", "提示");
            //    return;
            //}
            spd = (int)numericUpDown1.Value;
            speed = (int)numericUpDown2.Value;
            double tan = (double)slider1.Value / 100;

            if (checkBox1.Checked)
            {
                ft = new Form_BText(languageFile);
                ft.SetInitText(f, c, bc, spd, temp, speed, tan, true);
                ft.Show();
                formList.Add(ft);
            }
            else
            {
                Form_RunText bf = new Form_RunText(languageFile);
                if (checkBox3.Checked)
                    bf.SetInitText(f, c, bc, spd, temp, speed, tan, false,true);
                else
                    bf.SetInitText(f, c, bc, spd, temp, speed, tan, false,false);
                bf.Show();
            }
            if (temp.Contains("\n"))
                temp = temp.Replace("\n","*");
            settingFile.WriteString("SUBSET", "TEXTNAME", temp);
            settingFile.WriteInteger("SUBSET", "MOVETIME", spd);
            settingFile.WriteInteger("SUBSET", "MOVESPEED", speed);
            settingFile.WriteInteger("SUBSET", "MOVETANS", slider1.Value);
            settingFile.WriteInteger("SUBSET", "FONTC", colorDropDownList1.SelectedIndex);
        }

        private void button_font_Click(object sender, EventArgs e)
        {
            this.fontDialog1.Font = f;
            this.fontDialog1.Color = c;
            DialogResult result = fontDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                f = fontDialog1.Font;
                c = fontDialog1.Color;
                string s = f.Name;
                settingFile.WriteString("SUBSET", "FONTNAME", s);
                s = f.Size.ToString();
                settingFile.WriteString("SUBSET", "FONTSIZE", s);
                //s = (c.ToArgb().ToString("X8"));
                //settingFile.WriteString("SUBSET", "FONTCOLOR", s);
            }  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.colorDialog1.Color = bc;
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                bc = colorDialog1.Color;         
                string s = (bc.ToArgb().ToString("X8"));
                settingFile.WriteString("SUBSET", "BCCOLOR", s);
                //label1.SelectionColor = colorDialog1.Color;
            }  
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                button2.Enabled = false;
                slider1.Visible = true;
                checkBox2.Visible = true;
                checkBox3.Checked = false;
                checkBox3.Enabled = false;
                //bc = Color.Transparent;
            }
            else
            {
                button2.Enabled = true;
                slider1.Visible = false;
                checkBox2.Visible = false;
                checkBox3.Enabled = true;
            }
        }

        private void Form_TipsText_Load(object sender, EventArgs e)
        {
            var cct = new ColorConverter();
            string s = settingFile.ReadString("SUBSET", "TEXTNAME", "");
            //Console.WriteLine("=======" + s);
            if (s.Contains("*"))
                s = s.Replace("*", "\n");
            richTextBox1.Text = s;
            string s_name = settingFile.ReadString("SUBSET", "FONTNAME", f.Name);
            string s_size = settingFile.ReadString("SUBSET", "FONTSIZE", f.Size.ToString());
            slider1.Value = settingFile.ReadInteger("SUBSET", "MOVETANS", 100);
            colorDropDownList1.SelectedIndex = settingFile.ReadInteger("SUBSET", "FONTC", 0);
            f = new Font(s_name, float.Parse(s_size));
            //f = (Font)fc.ConvertFromString(s);
            //Console.WriteLine("=======" + s);
            //s = settingFile.ReadString("SUBSET", "FONTCOLOR", (c.ToArgb().ToString("X8")));
            //Console.WriteLine("=======" + s);
            //c = (Color)cct.ConvertFromString("#" + s);
            c = colorDropDownList1.SelectColor;
            s = settingFile.ReadString("SUBSET", "BCCOLOR", (bc.ToArgb().ToString("X8")));
            bc = ColorTranslator.FromHtml("#" + s);
            //Console.WriteLine("=======");
            int k = settingFile.ReadInteger("SUBSET", "MOVETIME", spd);
            numericUpDown1.Value = (decimal)k;
            k = settingFile.ReadInteger("SUBSET", "MOVESPEED", speed);
            numericUpDown2.Value = (decimal)k;

            if (!System.IO.Directory.Exists(filePath))
            {
                // 目录不存在，建立目录
                System.IO.Directory.CreateDirectory(filePath);
            }
            string s1 = settingFile.ReadString("SETTING", "PictureLogo", "");
            
            if (!s1.Equals(""))
            {
                s1 = s1.Substring(s1.Length - 4, 4);
                s1 = Application.StartupPath + @"\pic\logoshow" + s1;
                pictureBox1.Image = null;
                pictureBox1.Image = Image.FromFile(s1);
            }

            myThread = new Thread(new ThreadStart(delegate()
            {
                WeatherThread();
            })); //开线程         
            myThread.IsBackground = true;
            myThread.Start();
            //timer1.Start();
            //Console.WriteLine("==1111==");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!comboBox_City.Text.Equals(""))
            {
                try
                {
                    string[] s = w.getWeatherbyCityName(comboBox_City.Text.Trim());
                    string text = comboBox_City.Text + "  ";
                    //for (int i = 5; i < 11; i++)
                    {
                        text += s[6] + "\r\n" + s[7] + "\r\n" + s[10];
                    }
                    //Console.WriteLine(s[8] + "==" + s[9]);
                    Form_Weather ft = new Form_Weather();
                    if (checkBox1.Checked)
                        ft.SetInitText(f, c, bc, s[1], s[5], text, s[8], s[9], true);
                    else
                        ft.SetInitText(f, c, bc, s[1], s[5], text, s[8], s[9], false);
                    ft.Show();
                }
                catch
                {
                    string ts = languageFile.ReadString("TEXTFORM", "T1", "网络连接异常,无法正常显示！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
            }
        }

        private void comboBox_province_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox_City.Items.Clear();
            //if (!mf)
            {
                try
                {
                    string[] sc = w.getSupportCity(comboBox_province.Text);
                    int l = sc.Length;
                    for (int i = 0; i < l; i++)
                    {
                        //Console.WriteLine(sc[i]);
                        string ct = sc[i].Split(' ')[0];
                        comboBox_City.Items.Add(ct);
                    }
                    comboBox_City.SelectedIndex = 0;
                }
                catch
                {
                    string ts = languageFile.ReadString("TEXTFORM", "T1", "网络连接异常,无法正常显示！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
            }
        }

        private void slider1_ValueChanged(object sender, EventArgs e)
        {
            if (slider1.Value < 1)
                slider1.Value = 1;
            if (slider1.Value > 60)
                slider1.Value = 60;
            slider1.Text = slider1.Value.ToString() + "%";
        }


        private void WeatherThread()
        {
            Thread.Sleep(100);
            //Console.WriteLine("==2222==");
            //if (!mf)
            {
                try
                {
                    string[] sp = w.getSupportProvince();
                    int l = sp.Length;
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        for (int i = 0; i < l; i++)
                        {
                            //Console.WriteLine(sp[i]);
                            comboBox_province.Items.Add(sp[i].ToString());
                        }
                        comboBox_province.SelectedIndex = 0;
                    }));
                }
                catch
                {
                    string ts = languageFile.ReadString("TEXTFORM", "T1", "网络连接异常,无法正常显示！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
            }
            /*
            else
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    comboBox_province.Items.Clear();
                    comboBox_City.Items.Clear();
                }));
            }
             */ 
        }

        private void colorDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Color b = colorDropDownList1.SelectColor;
            //Console.WriteLine(b.Name);
            richTextBox1.ForeColor = b;
            c = b;
            //richTextBox1.Refresh();
            //Console.WriteLine(colorDropDownList1.SelectColorName);
        }
        /// <summary>
        /// 鼠标穿透设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < formList.Count; i++)
            {
                if (!formList[i].IsDisposed)
                {
                    if (checkBox2.Checked)
                    {
                        formList[i].Set_Penetrate(true);
                    }
                    else
                        formList[i].Set_Penetrate(false);
                }
                else
                    formList.RemoveAt(i);
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            pictureBox1.Image.Dispose();
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

        private void buttonX1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "图片文件|*.bmp;*.jpg;*.gif;*.png";
            string sourcePath = "";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                sourcePath = openfile.FileName;
                pictureBox1.Image.Dispose();
                pictureBox1.Image = Image.FromFile(sourcePath);
                //pictureBox1.ImageLocation = openfile.FileName;

                string strtype = sourcePath.Substring(sourcePath.Length - 4, 4);
                //Console.WriteLine("strtype:" + strtype);
                string targetPath = filePath + @"\logoshow" + strtype;
                //bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
                try
                {
                //    if (System.IO.File.Exists(targetPath))
                //    {
                //        // 目录不存在，建立目录
                //        System.IO.File.Delete(targetPath);
                //    }
                    System.IO.File.Copy(sourcePath, targetPath, true);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                settingFile.WriteString("SETTING", "PictureLogo", "logoshow" + strtype);
            }
            //button3.Enabled = true;
            openfile.Dispose();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = false;
                button2.Enabled = false;
                //bc = Color.Transparent;
            }
            else
            {
                //checkBox1.Checked = true;
                checkBox1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void Form_TipsText_FormClosing(object sender, FormClosingEventArgs e)
        {
            myThread.Abort();
        }
    }
}
