using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_Protocol : Form
    {
        private WallSetForm f;
        private IniFiles languageFile;
        //private IniFiles settingFile;
        public Form_Protocol(WallSetForm f)
        {
            InitializeComponent();
            this.f = f;
            //settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.mainForm.package);
            
            Init_FormString();
            
            if (f.mainForm.Chinese_English == 1)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
            }
            else if (f.mainForm.Chinese_English == 0)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Protocol));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            resources.ApplyResources(button_Del, button_Del.Name);
            resources.ApplyResources(button_Add, button_Add.Name);
            resources.ApplyResources(button_AddHex, button_AddHex.Name);
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
            this.Text = languageFile.ReadString("PROTOCOLFORM", "TITLE", "协议添加");
            this.label_type.Text = languageFile.ReadString("PROTOCOLFORM", "MATRIX_TYPE", "矩阵类型");
            this.label_company.Text = languageFile.ReadString("PROTOCOLFORM", "MATRIX_NUMBER", "矩阵编号");
            this.label2.Text = languageFile.ReadString("PROTOCOLFORM", "NUMBER_TYPE", "数字0~9形式");
            this.label3.Text = languageFile.ReadString("PROTOCOLFORM", "NUMBER_TYPE", "数字0~9形式");
            this.groupBox1.Text = languageFile.ReadString("PROTOCOLFORM", "STRING", "字符串协议");
            this.groupBox1.Text = languageFile.ReadString("PROTOCOLFORM", "HEX", "HEX协议");
            this.radioButton1.Text = languageFile.ReadString("PROTOCOLFORM", "MODE1", "00~09模式");
            this.radioButton4.Text = languageFile.ReadString("PROTOCOLFORM", "MODE1", "00~09模式");
            this.radioButton2.Text = languageFile.ReadString("PROTOCOLFORM", "MODE2", "0~9模式");
            this.radioButton3.Text = languageFile.ReadString("PROTOCOLFORM", "MODE2", "0~9模式");

            this.button_Del.Text = languageFile.ReadString("PROTOCOLFORM", "DELETE", "删除");
            this.button_Add.Text = languageFile.ReadString("PROTOCOLFORM", "ADD", "添加");
            this.button_AddHex.Text = languageFile.ReadString("PROTOCOLFORM", "ADD", "添加");

            this.label1.Text = languageFile.ReadString("PROTOCOLFORM", "TP1", "特殊字符：#id表示矩阵id,#in表示输入通道,#out表示输出通道");
            this.label4.Text = languageFile.ReadString("PROTOCOLFORM", "TP2", "例：协议格式为PH（头码），ID（#id）,SW(操作码)，in（#in）.out(#out),NT(结束码)；文本输入：PH,#id,SW,#in,#out,NT;多余留空");
            this.label5.Text = languageFile.ReadString("PROTOCOLFORM", "TP3", "例：BB 04 00 02 #in #out #id     提示： 请确保输入为16进制的字符，校验位可以输入对应公式如：#in+#out+#id ；多余留空");
        }

        private void Form_Protocol_Load(object sender, EventArgs e)
        {
            comboBox_Mt.SelectedIndex = 0;
        }
        /// <summary>
        /// 删除对应协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Del_Click(object sender, EventArgs e)
        {
            if (textBox_num.Text.Length > 0)
            {
                if (IsInts(textBox_num.Text) && int.Parse(textBox_num.Text) < 40)
                {
                    string ts = languageFile.ReadString("PROTOCOLFORM", "T1", "此协议禁止删除！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                    return;
                }
                DataTable clientProtocol = AccessFunction.GetClientProtocol(textBox_num.Text + "-" + comboBox_Mt.Text);
                if (clientProtocol.Rows.Count != 0)
                {
                    AccessFunction.DeleteClientProtocol(textBox_num.Text + "-" + comboBox_Mt.Text);
                    if (comboBox_Mt.Text == "hdmi")
                    {
                        for (int i = 0; i < f.comboBox_hdmi.Items.Count; i++)
                        {
                            if (f.comboBox_hdmi.Items[i].ToString() == textBox_num.Text)
                            {
                                if (f.comboBox_hdmi.Text == textBox_num.Text)
                                {
                                    f.comboBox_hdmi.SelectedIndex = 0;
                                    //settingFile.WriteInteger("Matrix", "HDMIMatrix", 0);
                                }
                                f.comboBox_hdmi.Items.Remove(textBox_num.Text);
                                break;
                            }
                        }
                    }
                    else if (comboBox_Mt.Text == "dvi")
                    {
                        for (int i = 0; i < f.comboBox_dvi.Items.Count; i++)
                        {
                            if (f.comboBox_dvi.Items[i].ToString() == textBox_num.Text)
                            {
                                if (f.comboBox_dvi.Text == textBox_num.Text)
                                {
                                    f.comboBox_dvi.SelectedIndex = 0;
                                    //settingFile.WriteInteger("Matrix", "DVIMatrix", 0);
                                }
                                f.comboBox_dvi.Items.Remove(textBox_num.Text);
                                break;
                            }
                        }
                    }
                    else if (comboBox_Mt.Text == "vga")
                    {
                        for (int i = 0; i < f.comboBox_vga.Items.Count; i++)
                        {
                            if (f.comboBox_vga.Items[i].ToString() == textBox_num.Text)
                            {
                                if (f.comboBox_vga.Text == textBox_num.Text)
                                {
                                    f.comboBox_vga.SelectedIndex = 0;
                                    //settingFile.WriteInteger("Matrix", "VGAMatrix", 0);
                                }
                                f.comboBox_vga.Items.Remove(textBox_num.Text);
                                break;
                            }
                        }
                    }
                    else if (comboBox_Mt.Text == "video")
                    {
                        for (int i = 0; i < f.comboBox_video.Items.Count; i++)
                        {
                            if (f.comboBox_video.Items[i].ToString() == textBox_num.Text)
                            {
                                if (f.comboBox_video.Text == textBox_num.Text)
                                {
                                    f.comboBox_video.SelectedIndex = 0;
                                    //settingFile.WriteInteger("Matrix", "VIDEOMatrix", 0);
                                }
                                f.comboBox_video.Items.Remove(textBox_num.Text);
                                break;
                            }
                        }
                    }
                    {
                        string ts = languageFile.ReadString("PROTOCOLFORM", "T2", "此协议已删除！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(textBox_num.Text + "-" + comboBox_Mt.Text + ts,tp);
                    }
                }
                else
                {
                    string ts = languageFile.ReadString("PROTOCOLFORM", "T3", "此协议不存在！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }

            }
        }

        private bool IsInts(string str)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            return rex.IsMatch(str);
        }
        /// <summary>
        /// 添加字符串协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Add_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if (textBox_num.Text.Length > 0)
            {
                if (textBox_c1.Text == "" && textBox_c2.Text == "" && textBox_c3.Text == "" && textBox_c4.Text == "" && textBox_c5.Text == "" && textBox_c6.Text == "" && textBox_c7.Text == "" && textBox_c8.Text == "" && textBox_c9.Text == "")
                {
                    {
                        string ts = languageFile.ReadString("PROTOCOLFORM", "T4", "添加的协议不能为空，请重新设置！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                    }
                    return;
                }
                if (IsInts(textBox_num.Text) && int.Parse(textBox_num.Text) < 40)
                {
                    {
                        string ts = languageFile.ReadString("PROTOCOLFORM", "T5", "此协议编号已存在，请重新设置！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                    }
                    return;
                }
                if (comboBox_Mt.Text == "video")
                {
                    for (int i = 0; i < f.comboBox_video.Items.Count; i++)
                    {
                        if (f.comboBox_video.Items[i].ToString() == textBox_num.Text)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                else if (comboBox_Mt.Text == "vga")
                {
                    for (int i = 0; i < f.comboBox_vga.Items.Count; i++)
                    {
                        if (f.comboBox_vga.Items[i].ToString() == textBox_num.Text)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                else if (comboBox_Mt.Text == "dvi")
                {
                    for (int i = 0; i < f.comboBox_dvi.Items.Count; i++)
                    {
                        if (f.comboBox_dvi.Items[i].ToString() == textBox_num.Text)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                else if (comboBox_Mt.Text == "hdmi")
                {
                    for (int i = 0; i < f.comboBox_hdmi.Items.Count; i++)
                    {
                        if (f.comboBox_hdmi.Items[i].ToString() == textBox_num.Text)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    string[] array = new string[14];
                    array[0] = textBox_num.Text + "-" + comboBox_Mt.Text;
                    array[1] = comboBox_Mt.Text;
                    array[2] = textBox_num.Text;
                    array[3] = textBox_c1.Text;
                    array[4] = textBox_c2.Text;
                    array[5] = textBox_c3.Text;
                    array[6] = textBox_c4.Text;
                    array[7] = textBox_c5.Text;
                    array[8] = textBox_c6.Text;
                    array[9] = textBox_c7.Text;
                    array[10] = textBox_c8.Text;
                    array[11] = textBox_c9.Text;
                    array[12] = (radioButton1.Checked ? "2" : "1");
                    array[12] = array[12] + "0";
                    array[13] = "1";
                    AccessFunction.InsertClientProtocol(array);
                    
                    if (comboBox_Mt.Text == "video")
                    {
                        f.comboBox_video.Items.Add(textBox_num.Text);
                    }
                    if (comboBox_Mt.Text == "vga")
                    {
                        f.comboBox_vga.Items.Add(textBox_num.Text);
                    }
                    if (comboBox_Mt.Text == "hdmi")
                    {
                        f.comboBox_hdmi.Items.Add(textBox_num.Text);
                    }
                    if (comboBox_Mt.Text == "dvi")
                    {
                        f.comboBox_dvi.Items.Add(textBox_num.Text);
                    }
                    else
                    {
                        string ts = languageFile.ReadString("PROTOCOLFORM", "T6", "添加协议完成！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                    }
                    LogHelper.WriteLog("======完成添加矩阵协议======");
                }
                else
                {
                    string ts = languageFile.ReadString("PROTOCOLFORM", "T7", "协议重复！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
        }

        private void LimitNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// 添加HEX协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_AddHex_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if (textBox_num.Text.Length > 0)
            {
                if (textBox_h1.Text == "" && textBox_h2.Text == "" && textBox_h3.Text == "" && textBox_h4.Text == "" && textBox_h5.Text == "" && textBox_h6.Text == "" && textBox_h7.Text == "" && textBox_h8.Text == "" && textBox_h9.Text == "")
                {
                    {
                        string ts = languageFile.ReadString("PROTOCOLFORM", "T4", "添加的协议不能为空，请重新设置！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                    }
                    return;
                }
                if (IsInts(textBox_num.Text) && int.Parse(textBox_num.Text) < 40)
                {
                    {
                        string ts = languageFile.ReadString("PROTOCOLFORM", "T5", "此协议编号已存在，请重新设置！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                    }
                    return;
                }
                if (comboBox_Mt.Text == "video")
                {
                    for (int i = 0; i < f.comboBox_video.Items.Count; i++)
                    {
                        if (f.comboBox_video.Items[i].ToString() == textBox_num.Text)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (comboBox_Mt.Text == "vga")
                {
                    for (int i = 0; i < f.comboBox_vga.Items.Count; i++)
                    {
                        if (f.comboBox_vga.Items[i].ToString() == textBox_num.Text)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (comboBox_Mt.Text == "dvi")
                {
                    for (int i = 0; i < f.comboBox_dvi.Items.Count; i++)
                    {
                        if (f.comboBox_dvi.Items[i].ToString() == textBox_num.Text)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (comboBox_Mt.Text == "hdmi")
                {
                    for (int i = 0; i < f.comboBox_hdmi.Items.Count; i++)
                    {
                        if (f.comboBox_hdmi.Items[i].ToString() == textBox_num.Text)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    string[] array = new string[14];
                    array[0] = textBox_num.Text + "-" + comboBox_Mt.Text;
                    array[1] = comboBox_Mt.Text;
                    array[2] = textBox_num.Text;
                    array[3] = textBox_h1.Text;
                    if (array[3].Length % 2 != 0)
                    {
                       MessageBox.Show("输入错误,请重新输入！","提示");
                        return;
                    }
                    array[4] = textBox_h2.Text;
                    array[5] = textBox_h3.Text;
                    array[6] = textBox_h4.Text;
                    array[7] = textBox_h5.Text;
                    array[8] = textBox_h6.Text;
                    array[9] = textBox_h7.Text;
                    array[10] = textBox_h8.Text;
                    array[11] = textBox_h9.Text;
                    array[12] = (radioButton4.Checked ? "2" : "1");
                    array[12] = array[12] + "0";
                    array[13] = "2";
                    AccessFunction.InsertClientProtocol(array);

                    if (comboBox_Mt.Text == "video")
                    {
                        f.comboBox_video.Items.Add(textBox_num.Text);
                    }
                    if (comboBox_Mt.Text == "vga")
                    {
                        f.comboBox_vga.Items.Add(textBox_num.Text);
                    }
                    if (comboBox_Mt.Text == "hdmi")
                    {
                        f.comboBox_hdmi.Items.Add(textBox_num.Text);
                    }
                    if (comboBox_Mt.Text == "dvi")
                    {
                        f.comboBox_dvi.Items.Add(textBox_num.Text);
                    }
                    {
                        string ts = languageFile.ReadString("PROTOCOLFORM", "T6", "添加协议完成！");
                        string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                        MessageBox.Show(ts, tp);
                    }
                    LogHelper.WriteLog("======完成添加矩阵协议======");
                }
                else
                {
                    string ts = languageFile.ReadString("PROTOCOLFORM", "T7", "协议重复！");
                    string tp = languageFile.ReadString("MESSAGEBOX", "TP", "提示");
                    MessageBox.Show(ts, tp);
                }
            }
        }

        private void textBox_num_TextChanged(object sender, EventArgs e)
        {
            if (IsInts(textBox_num.Text))
            {
                if (int.Parse(textBox_num.Text) > 200)
                {
                    //MessageBox.Show("请保证协议编号小于200，请重新设置！", "提示");
                    textBox_num.Text = "";
                    return;
                }
            }
            else
            {
                if (textBox_num.Text.Length > 10)
                {
                    //MessageBox.Show("协议编号长度（<10）输入有限，请修改正确设置！", "提示");
                    return;
                }
            }
        }
    }
}
