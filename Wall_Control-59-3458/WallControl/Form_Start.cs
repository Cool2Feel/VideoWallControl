using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_Start : Form
    {
        private IniFiles settingFile;
        public Form_Start()
        {
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            InitializeComponent();

            int s_M = settingFile.ReadInteger("SETTING", "Motherboard", 0);
            if (s_M == 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            else if (s_M == 1)
            {
                comboBox1.SelectedIndex = 1;
            }
            else if (s_M == 4)
            {
                comboBox1.SelectedIndex = 2;
            }
            else if (s_M == 2)
            {
                comboBox1.SelectedIndex = 3;
            }
            else
            {
                comboBox1.SelectedIndex = 4;
            }
            string s2 = settingFile.ReadString("SETTING", "CH-US", "1");
            
            if (s2.Equals("0"))
            {
                comboBox2.SelectedIndex = 1;
            }
            else if (s2.Equals("1"))
            {
                comboBox2.SelectedIndex = 0;
            }
            else
                comboBox2.SelectedIndex = 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            if (pictureBox1.BackgroundImage != null)
                pictureBox1.BackgroundImage.Dispose();
            //pictureBox1.BackgroundImage = global::WallControl.Properties.Resources.506;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
            if (pictureBox1.BackgroundImage != null)
                pictureBox1.BackgroundImage.Dispose();
            //pictureBox1.BackgroundImage = global::WallControl.Properties.Resources.538;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            radioButton3.Checked = true;
            if (pictureBox1.BackgroundImage != null)
                pictureBox1.BackgroundImage.Dispose();
            //pictureBox1.BackgroundImage = global::WallControl.Properties.Resources.5538;
        }

        private void radioButton1_MouseHover(object sender, EventArgs e)
        {
            radioButton1_Click(sender,e);
        }

        private void radioButton2_MouseHover(object sender, EventArgs e)
        {
            radioButton2_Click(sender, e);
        }

        private void radioButton3_MouseHover(object sender, EventArgs e)
        {
            radioButton3_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
