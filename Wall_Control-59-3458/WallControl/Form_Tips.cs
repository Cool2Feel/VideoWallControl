using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_Tips : Form
    {
        MainForm f;
        private static int t = 30;
        private IniFiles languageFile;
        public Form_Tips(MainForm f,bool b)
        {
            InitializeComponent();
            this.f = f;
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            //Init_FormString();
            this.label1.Text = languageFile.ReadString("TIPSFORM", "TIPS", "定时开关机提示：");
            this.button1.Text = languageFile.ReadString("TIPSFORM", "OK", "确认");
            this.button2.Text = languageFile.ReadString("TIPSFORM", "CANCEL", "取消");
            if (b)
                label1.Text = languageFile.ReadString("TIPSFORM", "TIPS1", "定时开机提示：");
            else
                label1.Text = languageFile.ReadString("TIPSFORM", "TIPS2", "定时关机提示：");
            /*
            if (f.Chinese_English)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
                if (b)
                    label1.Text = "Regular boot tips：";
                else
                    label1.Text = "Timed shutdown tips：";
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
                if (b)
                    label1.Text = "定时开机提示：";
                else
                    label1.Text = "定时关机提示：";
                
            }
             */ 
            t = 30;//提示倒计时
            label2.Text = "30";
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Tips));
            foreach (Control ctl in panel1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = t.ToString();
            t--;
            if (t < 0)
            {
                timer1.Stop();
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            f.Time_ok = false;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            this.Close();
        }
    }
}
