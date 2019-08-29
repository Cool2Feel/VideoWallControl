using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_Scename : Form
    {
        private string key;
        private IniFiles settingFile;
        private IniFiles languageFile;

        public Form_Scename(MainForm f, int k)
        {
            InitializeComponent();
            this.key = "S" + k.ToString();
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);

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

            textBox1.Text = settingFile.ReadString("SCENE", key, "Scene - " + k.ToString());
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Scename));
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
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            settingFile.WriteString("SCENE", key, textBox1.Text.Trim());
            this.Close();
        }
    }
}
