using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_OnOff : Form
    {
        private MainForm f;
        private IniFiles settingFile;
        private IniFiles languageFile;
        public Form_OnOff(MainForm f)
        {
            InitializeComponent();
            this.f = f;
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            
            Init_FormString();
            
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
            
            if (f.Delay_on)
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;
            if (f.Delay_off)
                checkBox2.Checked = true;
            else
                checkBox2.Checked = false;
            if (f.Timing_on)
            {
                checkBox3.Checked = true;
                dateTimePicker1.Value = f.on;
            }
            else
                checkBox3.Checked = false;
            if (f.Timing_off)
            {
                checkBox4.Checked = true;
                dateTimePicker2.Value = f.off;
            }
            else
                checkBox4.Checked = false;
            //if (f.Delay_on || f.Delay_off)
            comboBox1.SelectedIndex = f.Delay_time1/100 - 1;

            string s = settingFile.ReadString("SETTING", "On_time", DateTime.Now.ToString());
            dateTimePicker1.Value = DateTime.Parse(s);
            s = settingFile.ReadString("SETTING", "Off_time", DateTime.Now.ToString());
            dateTimePicker2.Value = DateTime.Parse(s);
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_OnOff));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            resources.ApplyResources(button1, button1.Name);
            resources.ApplyResources(button2, button2.Name);
            /*
            foreach (Control ctl in groupBox1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            foreach (Control ctl in groupBox2.Controls)
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
            this.Text = languageFile.ReadString("ONOFFFORM", "TITLE", "开关机设置");
            this.label3.Text = languageFile.ReadString("ONOFFFORM", "OFF_D", "定时关机：");
            this.label1.Text = languageFile.ReadString("ONOFFFORM", "TIME1", "延时间隔(ms):");
            this.label2.Text = languageFile.ReadString("ONOFFFORM", "ON_D", "定时开机：");
            this.groupBox1.Text = languageFile.ReadString("ONOFFFORM", "ONOFFTIME", "延时开关机");
            this.groupBox2.Text = languageFile.ReadString("ONOFFFORM", "TIME2", "定时开关机");
            this.button2.Text = languageFile.ReadString("ONOFFFORM", "EMPLOY", "应用");
            this.button1.Text = languageFile.ReadString("ONOFFFORM", "EMPLOY", "应用");
            this.checkBox1.Text = languageFile.ReadString("ONOFFFORM", "ON_T", "开机");
            this.checkBox2.Text = languageFile.ReadString("ONOFFFORM", "OFF_T", "关机");
            this.checkBox3.Text = languageFile.ReadString("ONOFFFORM", "TURN_ON", "启用");
            this.checkBox4.Text = languageFile.ReadString("ONOFFFORM", "TURN_ON", "启用");
            this.numericUpDown1.Value = settingFile.ReadInteger("SETTING", "DELAY", 0);
            this.label4.Text = languageFile.ReadString("ONOFFFORM", "T1", "同时启用定时开机和关机操作时，请设定两个时间差大于一分钟！");
            this.label5.Text = languageFile.ReadString("ONOFFFORM", "T2", "定时开关机的时间设置至少大于系统当前时间30秒！");
        }


        /// <summary>
        /// 开关机延时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            f.Delay_time1 = (int)numericUpDown1.Value;//(comboBox1.SelectedIndex + 1) * 100;
            if (checkBox1.Checked)
            {
                f.Delay_on = true;
            }
            else
            {
                f.Delay_on = false;
            }
            if (checkBox2.Checked)
            {
                f.Delay_off = true;
            }
            else
            {
                f.Delay_off = false;
            }
            settingFile.WriteInteger("SETTING", "DELAY", f.Delay_time1);
            LogHelper.WriteLog("=====设置延时开关机=====");
        }
        /// <summary>
        /// 定时开关机应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked && checkBox4.Checked)
            {
                if (dateTimePicker1.Value.Subtract(dateTimePicker2.Value).TotalSeconds < 60 && dateTimePicker1.Value.Subtract(dateTimePicker2.Value).TotalSeconds > -60)
                {
                    string ts = languageFile.ReadString("ONOFFFORM", "T1", "同时启用定时开机和关机操作时，请设定两个时间差大于一分钟！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
            }
            if (checkBox3.Checked)
            {
                if (dateTimePicker1.Value.Subtract(DateTime.Now).TotalSeconds >= 30)
                {
                    f.Timing_on = true;
                    f.on = dateTimePicker1.Value;
                    f.timer_On.Start();
                    settingFile.WriteString("SETTING", "On_time", dateTimePicker1.Value.ToString());
                }
                else
                {
                    string ts = languageFile.ReadString("ONOFFFORM", "T2", "定时开关机的时间设置至少大于系统当前时间30秒！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                LogHelper.WriteLog("=====启用定时开机=====");
                //Console.WriteLine(f.on);
            }
            else
            {
                f.Timing_on = false;
                f.timer_On.Stop();
            }
            if (checkBox4.Checked)
            {
                if (dateTimePicker2.Value.Subtract(DateTime.Now).TotalSeconds >= 30)
                {
                    f.Timing_off = true;
                    f.off = dateTimePicker2.Value;
                    f.timer_Off.Start();
                    settingFile.WriteString("SETTING", "Off_time", dateTimePicker2.Value.ToString());
                }
                else
                {
                    string ts = languageFile.ReadString("ONOFFFORM", "T2", "定时开关机的时间设置至少大于系统当前时间30秒！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                LogHelper.WriteLog("=====启用定时关机=====");
            }
            else
            {
                f.Timing_off = false;
                f.timer_Off.Stop();
            }
            this.Close();
        }
    }
}
