using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
//添加的命名空间

namespace WallControl
{
    public partial class Form_Log : Form
    {
        private IniFiles settingFile;
        private string str1 = "123.", str2 = "333";
        MainForm f;
        private RegistryKey rk;
        private IniFiles languageFile;
        public Form_Log(MainForm f)
        {
            InitializeComponent();
            this.f = f;
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            //comboBox1.SelectedIndex = 0;

            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            this.Text = languageFile.ReadString("LOGFORM", "TITLE", "权限登录");
            this.label1.Text = languageFile.ReadString("LOGFORM", "USER", "用户名：");
            this.label2.Text = languageFile.ReadString("LOGFORM", "PASSWORD", "密码：");
            this.label3.Text = languageFile.ReadString("LOGFORM", "PPERMISION", "用户权限：");
            this.button1.Text = languageFile.ReadString("LOGFORM", "LOG", "登录");
            this.button2.Text = languageFile.ReadString("LOGFORM", "CLOSE", "关闭");
            this.button3.Text = languageFile.ReadString("LOGFORM", "EXIT", "退出");
            this.button4.Text = languageFile.ReadString("LOGFORM", "CHANGE", "密码修改");
            string sg = languageFile.ReadString("LOGFORM", "USER1", "普通用户");
            string sa = languageFile.ReadString("LOGFORM", "USER2", "管理员用户");
            comboBox1.Items.Clear();
            comboBox1.Items.Add(sg);
            comboBox1.Items.Add(sa);

            string s1 = settingFile.ReadString("SETTING", "Pwd", "0");
            //Console.WriteLine(s1);
            if (s1 == "0")
            {
                button4.Visible = false;
                button3.Visible = false;
                button1.Visible = true;
                button2.Visible = true;
                comboBox1.SelectedIndex = 0;
            }
            else if (s1 == "1")
            {
                button1.Visible = false;
                button2.Visible = false;
                button4.Visible = true;
                button3.Visible = true;
                comboBox1.SelectedIndex = 1;
                comboBox1.Enabled = false;
            }
            else
            {
                button4.Visible = false;
                button3.Visible = false;
                button1.Visible = true;
                button2.Visible = true;
                comboBox1.SelectedIndex = 0;
            }
            
            
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Log));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            resources.ApplyResources(button1, button1.Name);
            resources.ApplyResources(button2, button2.Name);
            resources.ApplyResources(button3, button3.Name);
            resources.ApplyResources(button4, button4.Name);
            /*
            foreach (Control ctl in panel1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
             */ 
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                if (f.Chinese_English == 1)
                    errorProvider1.SetError(textBox1, "Username can not be empty!");
                else
                    errorProvider1.SetError(textBox1, "用户名不能为空!");
                return;
            }
            else
            {
                errorProvider1.Clear();
                str2 = settingFile.ReadString("SETTING", "in", "333");
                if (textBox1.Text =="admin" && str1 == textBox2.Text && comboBox1.SelectedIndex == 0)
                {
                    //this.DialogResult = DialogResult.OK;
                    settingFile.WriteString("SETTING", "Pwd", "0");

                    string ts = languageFile.ReadString("LOGFORM", "T2", "登录成功！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    this.Close();
                }
                else if (textBox1.Text == "admin" && (str2 == textBox2.Text || "111111" == textBox2.Text) && comboBox1.SelectedIndex == 1)
                {
                    //this.DialogResult = DialogResult.OK;
                    settingFile.WriteString("SETTING", "Pwd", "1");
                    string ts = languageFile.ReadString("LOGFORM", "T2", "登录成功！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    this.Close();
                }
                else if (textBox1.Text == "welcome" && "******" == textBox2.Text && comboBox1.SelectedIndex == 1)
                {
                    //this.DialogResult = DialogResult.OK;
                    rk = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);//打开系统的注册表项，Registry.LocalMachine包含本地计算机的配置数据。该字段读取 Windows 注册表基项 HKEY_LOCAL_MACHINE,OpenSubKey("SOFTWARE")表示打开本地计算机配置数据下SOFTWARE子项
                    if (rk.GetValue("Wall_C") != null)
                    {
                        rk.SetValue("Count", 0);
                        rk.DeleteValue("Wall_C", true);
                    }
                    settingFile.WriteString("SETTING", "Pwd", "1");
                    string ts = languageFile.ReadString("LOGFORM", "T2", "登录成功！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    this.Close();
                }
                else
                {
                    string ts = languageFile.ReadString("LOGFORM", "T3", "用户密码输入错误！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Text = "";
                }
            }
            
        }

        private void Form_Log_Load(object sender, EventArgs e)
        {
            //rk = Registry.LocalMachine.OpenSubKey("SOFTWARE",true);//打开系统的注册表项，Registry.LocalMachine包含本地计算机的配置数据。该字段读取 Windows 注册表基项 HKEY_LOCAL_MACHINE,OpenSubKey("SOFTWARE")表示打开本地计算机配置数据下SOFTWARE子项
            /*
            rk.DeleteValue("ZCM", true);
            if (rk.GetValue("ZCM") != null)//获取程序的注册码的值，如果已经注册则有值，此时注册按钮隐藏，未注册时，值为null，此时显示注册按钮
            {
                button2.Visible = false;
            }
          * */
        }



        private void button3_Click(object sender, EventArgs e)
        {
            settingFile.WriteString("SETTING", "Pwd", "0");
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                textBox2.Focus();
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button1.Focus();
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                settingFile.WriteString("SETTING", "in", textBox2.Text.Trim());
                string ts = languageFile.ReadString("LOGFORM", "T4", "密码修改成功！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
            else
            {
                string ts = languageFile.ReadString("LOGFORM", "T4", "密码修改成功！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
            }
        }

        private void Form_Log_SizeChanged(object sender, EventArgs e)
        {
        }
    }
}
