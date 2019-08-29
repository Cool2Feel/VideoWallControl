using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_Auto : Form
    {
        private MainForm f;
        private int count;
        private int b;
        private IniFiles settingFile;
        private IniFiles languageFile;
        //Stack<Control> st = new Stack<Control>();
        private List<int> L = new List<int>();
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public Form_Auto(MainForm f,int count)
        {
            this.f = f;
            this.count = count;
            InitializeComponent();
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            this.Text = languageFile.ReadString("AUTOFORM", "TITLE", "预案场景");
            this.label1.Text = languageFile.ReadString("AUTOFORM", "T_I", "间隔时间(30s~24h)");
            this.button1.Text = languageFile.ReadString("AUTOFORM", "START", "开始");
            string sl = languageFile.ReadString("AUTOFORM", "SCENES", "场景");
            
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
            
            foreach (Control var in this.Controls)
            {
                if (var is System.Windows.Forms.CheckBox)
                { //如果是CheckBox
                    //Console.WriteLine(var.Name);
                    var.Text = var.Name.Replace("checkBox", sl);
                    string s = var.Name;
                    //if (f.Chinese_English)
                    {
                        if (int.Parse(s.Substring(8, s.Length - 8)) > count)
                            var.Enabled = false;
                    }
                }
            }
            string str = settingFile.ReadString("SCREEN", "NUM", "");
            if (str != "")
            {
                string[] s;
                s = str.Split(new char[] { ',' });
                int k = s.Length;
                if (k > 0)
                {
                    for (int i = 0; i < k; i++)
                    {
                        //Console.WriteLine(s[i]);
                        switch (s[i])
                        {
                            case "1":
                                checkBox1.Checked = true;
                                continue;
                            case "2":
                                checkBox2.Checked = true;
                                continue;
                            case "3":
                                checkBox3.Checked = true;
                                continue;
                            case "4":
                                checkBox4.Checked = true;
                                continue;
                            case "5":
                                checkBox5.Checked = true;
                                continue;
                            case "6":
                                checkBox6.Checked = true;
                                continue;
                            case "7":
                                checkBox7.Checked = true;
                                continue;
                            case "8":
                                checkBox8.Checked = true;
                                continue;
                            case "9":
                                checkBox9.Checked = true;
                                continue;
                            case "10":
                                checkBox10.Checked = true;
                                continue;
                            case "11":
                                checkBox11.Checked = true;
                                continue;
                            case "12":
                                checkBox12.Checked = true;
                                continue;
                            default:
                                continue;
                        }
                    }
                }
            }
            textBox1.Text = settingFile.ReadString("SCREEN", "NUM_time", "30");
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Auto));
            /*
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
             */
            resources.ApplyResources(button1, button1.Name);
            resources.ApplyResources(label1, label1.Name);
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                L.Add(1);
            else
                L.Remove(1);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                L.Add(2);
            else
                L.Remove(2);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                L.Add(3);
            else
                L.Remove(3);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                L.Add(4);
            else
                L.Remove(4);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
                L.Add(5);
            else
                L.Remove(5);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                L.Add(6);
            else
                L.Remove(6);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
                L.Add(7);
            else
                L.Remove(7);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
                L.Add(8);
            else
                L.Remove(8);
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
                L.Add(9);
            else
                L.Remove(9);
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
                L.Add(10);
            else
                L.Remove(10);
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
                L.Add(11);
            else
                L.Remove(11);
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
                L.Add(12);
            else
                L.Remove(12);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int n = L.Count;
            if (n <= 0)
            {
                string ts = languageFile.ReadString("AUTOFORM", "T1", "没有可轮巡的场景预案！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                return;
            }
            int t = int.Parse(textBox1.Text);
            if (t < 30 || t > 86400)
            {
                string ts = languageFile.ReadString("AUTOFORM", "T2", "请填写正确的间隔时间（30/s ~ 24/h）！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                textBox1.Text = "30";
                return;
            }
            L.Sort();//对选中的场景排序
            b = f.Select_bt();//获取当前在那个场景中
            Form_WaitRun f1 = new Form_WaitRun(f, L, t,b);
            f1.ShowDialog();
            LogHelper.WriteLog("======开始自动进行场景轮巡======");
        }

        private bool IsInts(string str)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            return rex.IsMatch(str);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                string ts = languageFile.ReadString("AUTOFORM", "T3", "场景轮巡的间隔时间输入不能为空！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                textBox1.Text = "30";
                return;
            }
            if (textBox1.Text.Length > 0)
            {
                if (!IsInts(textBox1.Text))
                {
                    string ts = languageFile.ReadString("MESSAGEBOX", "C2", "请输入有效数字字符组合(\"0~9\")");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    textBox1.Text = "30";
                    return;
                }
            }
            int t = int.Parse(textBox1.Text);
            if (t < 0 || t > 86400)
            {
                string ts = languageFile.ReadString("AUTOFORM", "T2", "请填写正确的间隔时间（30/s ~ 24/h）！");
                string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                MessageBox.Show(ts, tp);
                textBox1.Text = "30";
                return;
            }
        }

        private void Form_Auto_Load(object sender, EventArgs e)
        {
            //asc.controllInitializeSize(this);  
        }

        private void Form_Auto_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }

        private void Form_Auto_FormClosing(object sender, FormClosingEventArgs e)
        {
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            string s = "";
            for (int i = 0; i < L.Count; i++)
            {
                s += L[i].ToString() + ",";
            }
            settingFile.WriteString("SCREEN", "NUM", s);//保存场景记录
            settingFile.WriteString("SCREEN", "NUM_time", textBox1.Text);
            L.Clear();
        }


    }
}
