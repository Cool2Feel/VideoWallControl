using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_about : Form
    {
        private IniFiles languageFile;
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public Form_about(MainForm f)
        {
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
            ApplyResource();
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            this.Text = languageFile.ReadString("ABOUTFORM", "TITLE", "关于软件");
            this.label1.Text = languageFile.ReadString("ABOUTFORM", "T1", "拼接控制系统");
            this.label2.Text = languageFile.ReadString("ABOUTFORM", "T2", "版本 ： V 3.0.5");
            this.label3.Text = languageFile.ReadString("ABOUTFORM", "T3", "LCD splicing control system");
            this.button1.Text = languageFile.ReadString("ABOUTFORM", "T_OK", "确认");
            
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
            this.label2.Text = languageFile.ReadString("ABOUTFORM", "T2", "版本 ： V 3.0.5");
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_about));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            foreach (Control ctl in groupBox1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_about_Load(object sender, EventArgs e)
        {
            //asc.controllInitializeSize(this);  
        }

        private void Form_about_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }
    }
}
