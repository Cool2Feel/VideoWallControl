using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace WallControl
{
    public partial class Form_Reg : Form
    {
        private MainForm f;
        public Form_Reg(MainForm f)
        {
            InitializeComponent();
            this.f = f;
            if (f.Chinese_English == 1)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
            }
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Reg));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void Form_Reg_Load(object sender, EventArgs e)
        {
            textBox3.Text = Register.GetMachineNumber();
            textBox1.Text = Register.GetRegisterNumber();//调用Register的获取注册码的静态方法，获取当前机器的注册码，并未文本框赋值        
            FileStream fs = new FileStream(Application.StartupPath + "\\zcm.txt", FileMode.Create, FileAccess.Write);//将注册码写入程序启动项的路径中zcm.txt文件中
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(textBox1.Text);
            sw.Close();
            fs.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))//先检测用户输入是否为空
            {
                if (textBox1.Text == textBox2.Text)//再检测用户输入是否和显示的注册码一直
                {
                    try
                    {
                        RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                        rk.SetValue("Wall_C", textBox1.Text);//将注册码写入系统的关于程序信息的注册表下SOFTWARE文件下的"Wall_C"名中
                        //rk.DeleteValue("ZCM",true);
                        if (f.Chinese_English == 1)
                            MessageBox.Show("Registration Success! Please reopen the software！", "Tips");
                        else
                            MessageBox.Show("注册成功！请重新打开软件！", "提示");
                        this.Close();
                    }
                    catch
                    { 
                    }
                }
                else
                {
                    if (f.Chinese_English == 1)
                        MessageBox.Show("The registration code you entered is incorrect. Please enter it again！","Tips");
                    else
                        MessageBox.Show("您输入的注册码不正确，请再次输入！","提示");
                }
            }
            else
            {
                if (f.Chinese_English == 1)
                    MessageBox.Show("Please enter the registration code！", "Tips");
                else
                    MessageBox.Show("    请输入注册码！", "提示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
