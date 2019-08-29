using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_Modify : Form
    {
        private MainForm f;
        private IniFiles languageFile;
        public Form_Modify(MainForm f)
        {
            this.f = f;
            InitializeComponent();

            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            this.Text = languageFile.ReadString("MODIFYFORM", "TITLE", "权限限制");
            this.label1.Text = languageFile.ReadString("MODIFYFORM", "PASSWORD", "密码：");
            this.button1.Text = languageFile.ReadString("MODIFYFORM", "OK", "确认");
            this.button2.Text = languageFile.ReadString("MODIFYFORM", "CANCEL", "取消");
            
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
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Modify));
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
            if (textBox1.Text == "123")
                f.f_m = true;
            else
            {
                if (f.Chinese_English == 1)
                    MessageBox.Show("   Wrong password！");
                else
                    MessageBox.Show("   密码错误！");
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            f.f_m = false; ;
            this.Close();
        }
    }
}
