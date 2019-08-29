using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_Edit : Form
    {
        private string str;
        private string t;
        MainForm f;
        private IniFiles settingFile;
        private IniFiles languageFile;
        public Form_Edit(MainForm f,string str,string text)
        {
            InitializeComponent();
            this.f = f;
            this.str = str;
            this.t = text;


            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            this.Text = languageFile.ReadString("EDITFORM", "TITLE", "信号通道编辑");
            this.label1.Text = languageFile.ReadString("EDITFORM", "EI", "编辑输入：");
            this.button1.Text = languageFile.ReadString("EDITFORM", "OK", "确认");
            this.button2.Text = languageFile.ReadString("EDITFORM", "CANCEL ", "取消");
            
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
            
            if (t.Contains("("))
            {
                t = t.Split('(')[1].Split(')')[0];
                textBox1.Text = t;
            }
            else
                textBox1.Text = "";
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Edit));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Console.WriteLine("str.name=" + str);
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            string s = str.Substring(5);
            f.Edit_str = textBox1.Text;
            if (textBox1.Text.Equals(""))
            {
                if (str.Contains("Hdmi"))
                {
                    s = s.Replace("Hdmi", "HDMI");
                    settingFile.WriteString("INPUTNAME", str + "Name", s);
                    //sConsole.WriteLine(s);
                }
                else if (str.Contains("Dvi"))
                {
                    s = s.Replace("Dvi", "DVI");
                    settingFile.WriteString("INPUTNAME", str + "Name", s);
                }
                else if (str.Contains("Vga"))
                {
                    s = s.Replace("Vga", "VGA");
                    settingFile.WriteString("INPUTNAME", str + "Name", s);
                }
                else if (str.Contains("Video"))
                {
                    s = s.Replace("Video", "VIDEO");
                    settingFile.WriteString("INPUTNAME", str + "Name", s);
                }
            }
            else
            {
                if (str.Contains("Hdmi"))
                {
                    s = s.Replace("Hdmi", "HDMI");
                    settingFile.WriteString("INPUTNAME", str + "Name", s + "(" + textBox1.Text + ")");
                }
                else if (str.Contains("Dvi"))
                {
                    s = s.Replace("Dvi", "DVI");
                    settingFile.WriteString("INPUTNAME", str + "Name", s + "(" + textBox1.Text + ")");
                }
                else if (str.Contains("Vga"))
                {
                    s = s.Replace("Vga", "VGA");
                    settingFile.WriteString("INPUTNAME", str + "Name", s + "(" + textBox1.Text + ")");
                }
                else if (str.Contains("Video"))
                {
                    s = s.Replace("Video", "VIDEO");
                    settingFile.WriteString("INPUTNAME", str + "Name", s + "(" + textBox1.Text + ")");
                }
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (t.Equals(""))
                f.Edit_str = "";
            else
                f.Edit_str = t;
            this.Close();
        }
    }
}
