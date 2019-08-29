using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_TextSend : Form
    {
        public MainForm f;
        public Form_TextSend(MainForm f)
        {
            InitializeComponent();
            this.f = f;
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
            }
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_TextSend));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            foreach (Control ctl in groupBox1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox2.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox3.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox4.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox5.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox6.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            foreach (Control ctl in groupBox7.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Focus();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length > 0)
                label1.Visible = false;
            else
                label1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
