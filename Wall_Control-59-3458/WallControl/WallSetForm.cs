using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
namespace WallControl
{
    public partial class WallSetForm : Form
    {
        private int Matrix_flag = 0;
        private UpDownBase up1;
        private UpDownBase up2;
        private UpDownBase up3;
        private UpDownBase up4;
        private UpDownBase up5;
        private UpDownBase up6;
        public MainForm mainForm;
        private IniFiles settingFile;
        private IniFiles languageFile;
        //public byte[,] Panelcount = new byte[4,256];
        private bool Row_Col = false;
        //private bool Matrix_select = false;
        //private bool Motherboard = false;
        //private bool f_m = false;
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public WallSetForm(MainForm mainForm)
        {
            settingFile = new IniFiles(Application.StartupPath + "\\setting.ini");
            InitializeComponent();
            //string s = settingFile.ReadString("SETTING", "PicturePath", "");
            this.mainForm = mainForm;
            //button3.Enabled = false;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
            //ApplyResource();
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + mainForm.package);
            up1 = nud_HDMIAddress as UpDownBase;
            up2 = nud_DVIAddress as UpDownBase;
            up3 = nud_VGAAddress as UpDownBase;
            up4 = nud_VIDEOAddress as UpDownBase;
            up5 = nud_HDMICount as UpDownBase;
            up6 = nud_DVICount as UpDownBase;
            up1.TextChanged += new EventHandler(up1_TextChanged);
            up2.TextChanged += new EventHandler(up2_TextChanged);
            up3.TextChanged += new EventHandler(up3_TextChanged);
            up4.TextChanged += new EventHandler(up4_TextChanged);
            up5.TextChanged += new EventHandler(up5_TextChanged);
            up6.TextChanged += new EventHandler(up6_TextChanged);

            Init_FormString();
            //tb_sceDirectoryName.Text = MainForm.currentConnectionName;
            
            if (this.mainForm.Chinese_English == 1)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                ApplyResource();
                //textBox2.MaxLength = 32;
            }
            else if (this.mainForm.Chinese_English == 0)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                ApplyResource();
                //textBox2.MaxLength = 32;
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHT");
                ApplyResource();
            }
            initFromFile();
            //tabPage3.Parent = null;//矩阵协议屏蔽
            //tabPage3.Hide();
            //if (!s.Equals(""))
                //pictureBox1.Image = Image.FromFile(@s);
            //textBox1.Text = s;
            Row_Col = false;
            //Matrix_select = false;
        }

        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WallSetForm));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
            }
            foreach (Control ctl in tabControl1.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
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
            foreach (Control ctl in groupBox7.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
        }

        /// <summary>
        /// 语言设置加载
        /// </summary>
        private void Init_FormString()
        {
            this.Text = languageFile.ReadString("WALLSETFORM", "TITLE", "拼接设置");
            this.tabPage1.Text = languageFile.ReadString("WALLSETFORM", "SET1", "拼接设置");
            this.tabPage2.Text = languageFile.ReadString("WALLSETFORM", "SET2", "矩阵设置");
            this.tabPage3.Text = languageFile.ReadString("WALLSETFORM", "SET3", "矩阵协议");

            this.label3.Text = languageFile.ReadString("WALLSETFORM", "COLU", "拼接(列)：");
            this.label20.Text = languageFile.ReadString("WALLSETFORM", "ROW", "拼接(行)：");
            this.label26.Text = languageFile.ReadString("WALLSETFORM", "SET_LOGO", "主页标志图片");
            this.label15.Text = languageFile.ReadString("WALLSETFORM", "TIPS1", "提示：用户设置主页标志Logo和选择主板类型需要管理员权限！");
            this.groupBox6.Text = languageFile.ReadString("WALLSETFORM", "SET_BORAD", "主板类型");
            this.button2.Text = languageFile.ReadString("WALLSETFORM", "SELECT", "选择");
            this.button3.Text = languageFile.ReadString("WALLSETFORM", "EMPLOY", "应用");
            this.bt_wallSetConfirm.Text = languageFile.ReadString("WALLSETFORM", "OK", "确认");
            this.button1.Text = languageFile.ReadString("WALLSETFORM", "CLOSE", "关闭");

            this.label14.Text = languageFile.ReadString("WALLSETFORM", "TIPS2", "注意： 矩阵选择 ：（默认 1 ~ 39）    地址：(0 ~ 255)    通道数： （0 ~ 128）");

            this.label9.Text = languageFile.ReadString("WALLSETFORM", "ADDRESS", "地址");
            this.label10.Text = languageFile.ReadString("WALLSETFORM", "ADDRESS", "地址");
            this.label11.Text = languageFile.ReadString("WALLSETFORM", "ADDRESS", "地址");
            this.label12.Text = languageFile.ReadString("WALLSETFORM", "ADDRESS", "地址");

            this.label1.Text = languageFile.ReadString("WALLSETFORM", "CHANNEL", "通道数");
            this.label2.Text = languageFile.ReadString("WALLSETFORM", "CHANNEL", "通道数");
            this.label4.Text = languageFile.ReadString("WALLSETFORM", "CHANNEL", "通道数");
            this.label5.Text = languageFile.ReadString("WALLSETFORM", "CHANNEL", "通道数");
            string sl = languageFile.ReadString("WALLSETFORM", "MATRIX", "矩阵");
            this.groupBox1.Text = "HDMI " + sl;
            this.groupBox2.Text = "DVI " + sl;
            this.groupBox3.Text = "VIDEO " + sl;
            this.groupBox5.Text = "VGA " + sl;
            this.groupBox7.Text = languageFile.ReadString("WALLSETFORM", "TIME", "时间设置");
            this.label19.Text = languageFile.ReadString("WALLSETFORM", "TIMES", "间隔时间(ms):");
            this.label22.Text = languageFile.ReadString("WALLSETFORM", "INSTRUCTION", "指令优先级：");
            this.radioButton1.Text = languageFile.ReadString("WALLSETFORM", "INST1", "拼接屏幕");
            this.radioButton2.Text = languageFile.ReadString("WALLSETFORM", "INST2", "矩阵");

            this.button_Map.Text = languageFile.ReadString("WALLSETFORM", "COMMAP", "端口映射");
            this.button_AddPro.Text = languageFile.ReadString("WALLSETFORM", "ADDPORT", "协议添加");
            this.button_Map.TextAlign = ContentAlignment.MiddleCenter;
            this.button_AddPro.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void up1_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrEmpty(up1.Text))
            {
                ///如果为空则执行相关操作
                nud_HDMIAddress.Value = 0;
                nud_HDMIAddress.Select(0, nud_HDMIAddress.Value.ToString().Length); 
            }
        }

        private void up2_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrEmpty(up2.Text))
            {
                ///如果为空则执行相关操作
                nud_DVIAddress.Value = 0;
                nud_DVIAddress.Select(0, nud_DVIAddress.Value.ToString().Length); 
            }
        }

        private void up3_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrEmpty(up3.Text))
            {
                ///如果为空则执行相关操作
                nud_VGAAddress.Value = 0;
                nud_VGAAddress.Select(0, nud_VGAAddress.Value.ToString().Length);
            }
        }

        private void up4_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrEmpty(up4.Text))
            {
                ///如果为空则执行相关操作
                nud_VIDEOAddress.Value = 0;
                nud_VIDEOAddress.Select(0, nud_VIDEOAddress.Value.ToString().Length);
            }
        }

        private void up5_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrEmpty(up5.Text))
            {
                ///如果为空则执行相关操作
                nud_HDMICount.Value = 0;
                nud_HDMICount.Select(0, nud_HDMICount.Value.ToString().Length);
            }
        }

        private void up6_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrEmpty(up6.Text))
            {
                ///如果为空则执行相关操作
                nud_DVICount.Value = 0;
                nud_DVICount.Select(0, nud_DVICount.Value.ToString().Length);
            }
        }

        /// <summary>
        /// 从文件中读取初始化连接界面的内容
        /// </summary>
        private void initFromFile() 
        {
            Init_Load();
            cb_spliceRows.SelectedIndex = settingFile.ReadInteger("SETTING", "Row", 2) - 1;//设置下拉，行
            cb_spliceCols.SelectedIndex = settingFile.ReadInteger("SETTING", "Col", 2) - 1;//设置下拉，列
            //tb_sceDirectoryName.Text = settingFile.ReadString("SETTING", "CurrDirect", Application.StartupPath + "\\myWall");//当前连接场景文件夹
            //int k = settingFile.ReadInteger("SETTING", "Resolution", tb_sceDirectoryName.Text.Length);//分辨率
            //tb_sceDirectoryName.Text = tb_sceDirectoryName.Text.Substring(0, k);
            //端口数
            nud_DVICount.Value = settingFile.ReadInteger("Matrix", "DVICount", 0);
            nud_VIDEOCount.Value = settingFile.ReadInteger("Matrix", "VIDEOCount", 0);
            nud_VGACount.Value = settingFile.ReadInteger("Matrix", "VGACount", 0);
            nud_HDMICount.Value = settingFile.ReadInteger("Matrix", "HDMICount", 0);
            nud_YPbPrCount.Value = settingFile.ReadInteger("Matrix", "YPbPrCount", 0);

            //信源地址
            nud_DVIAddress.Value = settingFile.ReadInteger("Matrix", "DVIAddress", 0);
            nud_VIDEOAddress.Value = settingFile.ReadInteger("Matrix", "VIDEOAddress", 0);
            nud_VGAAddress.Value = settingFile.ReadInteger("Matrix", "VGAAddress", 0);
            nud_HDMIAddress.Value = settingFile.ReadInteger("Matrix", "HDMIAddress", 0);
            nud_YPbPrAddress.Value = settingFile.ReadInteger("Matrix", "YPbPrAddress", 0);

            //信源矩阵选用
            comboBox_dvi.SelectedIndex = settingFile.ReadInteger("Matrix", "DVIMatrix", 0);
            comboBox_video.SelectedIndex = settingFile.ReadInteger("Matrix", "VIDEOMatrix", 0);
            comboBox_vga.SelectedIndex = settingFile.ReadInteger("Matrix", "VGAMatrix", 0);
            comboBox_hdmi.SelectedIndex = settingFile.ReadInteger("Matrix", "HDMIMatrix", 0);
            nud_YPbPrMatrix.Value = settingFile.ReadInteger("Matrix", "YPbPrMatrix", 0);

            //numericUpDown1.Value = settingFile.ReadInteger("SETTING", "Matrix_time", 200);
            comboBox1.SelectedIndex = settingFile.ReadInteger("SETTING", "Matrix_time", 1);
            string s1 = settingFile.ReadString("SETTING", "Pwd", "0");
            if (s1 == "0")
            {
                button2.Enabled = false;
                button3.Enabled = false;
                textBox2.Enabled = false;
                groupBox6.Enabled = false;
                button_AddPro.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
                button3.Enabled = true;
                textBox2.Enabled = true;
                groupBox6.Enabled = true;
                button_AddPro.Enabled = true;
            }
            int s_M = settingFile.ReadInteger("SETTING", "Motherboard", 0);
            if (s_M == 0)
            {
                radioButton4.Checked = true;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
            }
            else if (s_M == 1)
            {
                radioButton5.Checked = true;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
            }
            else if (s_M == 4)
            {
                radioButton7.Checked = true;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
            }
            else if (s_M == 2)
            {
                radioButton6.Checked = true;
                groupBox3.Enabled = true;
                groupBox4.Enabled = false;
            }
            else
            {
                radioButton3.Checked = true;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
            }
            int n = settingFile.ReadInteger("SETTING", "Matrix_flag", 0);
            if (n == 1)
                radioButton2.Checked = true;
            else
                radioButton1.Checked = true;
            string s = settingFile.ReadString("SETTING", "PicturePath", "");
            //Console.WriteLine("s ====" + s);
            if (!s.Equals(""))
            {
                s = s.Substring(s.Length - 4, 4);
                s = Application.StartupPath + @"\pic\logo" + s;
                pictureBox1.Image.Dispose();
                pictureBox1.Load(s);
            }
            textBox1.Text = s;
            bool PN = settingFile.ReadBool("SETTING", "NameFlag", false);
            if(PN)
                textBox2.Text = settingFile.ReadString("SETTING", "NamePath", "液晶拼接控制系统");
        }

        /// <summary>
        /// 保存连接设置到文件
        /// </summary>
        private void saveSettingFile() {

            //settingFile.WriteString("SETTING", "CurrDirect", tb_sceDirectoryName.Text.Trim());//保存当前连接场景文件夹
            //settingFile.WriteInteger("SETTING", "Row", cb_spliceRows.SelectedIndex + 1);//保存行数
            //settingFile.WriteInteger("SETTING", "Col", cb_spliceCols.SelectedIndex + 1);//保存列数
            //settingFile.WriteInteger("SETTING", "Resolution", tb_sceDirectoryName.Text.Length);//保存分辨率
            if (Row_Col)
            {
                settingFile.WriteInteger("SETTING", "Row", int.Parse(cb_spliceRows.Text));//保存行数
                settingFile.WriteInteger("SETTING", "Col", int.Parse(cb_spliceCols.Text));//保存列数
            }

            //保存端口数
            settingFile.WriteInteger("Matrix", "DVICount", Decimal.ToInt32(nud_DVICount.Value));
            settingFile.WriteInteger("Matrix", "VIDEOCount", Decimal.ToInt32(nud_VIDEOCount.Value));
            settingFile.WriteInteger("Matrix", "VGACount", Decimal.ToInt32(nud_VGACount.Value));
            settingFile.WriteInteger("Matrix", "HDMICount", Decimal.ToInt32(nud_HDMICount.Value));
            settingFile.WriteInteger("Matrix", "YPbPrCount", Decimal.ToInt32(nud_YPbPrCount.Value));

            //保存信源地址
            settingFile.WriteInteger("Matrix", "DVIAddress", Decimal.ToInt32(nud_DVIAddress.Value));
            settingFile.WriteInteger("Matrix", "VIDEOAddress", Decimal.ToInt32(nud_VIDEOAddress.Value));
            settingFile.WriteInteger("Matrix", "VGAAddress", Decimal.ToInt32(nud_VGAAddress.Value));
            settingFile.WriteInteger("Matrix", "HDMIAddress", Decimal.ToInt32(nud_HDMIAddress.Value));
            settingFile.WriteInteger("Matrix", "YPbPrAddress", Decimal.ToInt32(nud_YPbPrAddress.Value));

            //保存信源矩阵选用
            settingFile.WriteInteger("Matrix", "DVIMatrix", Decimal.ToInt32(comboBox_dvi.SelectedIndex));
            settingFile.WriteInteger("Matrix", "VIDEOMatrix", Decimal.ToInt32(comboBox_video.SelectedIndex));
            settingFile.WriteInteger("Matrix", "VGAMatrix", Decimal.ToInt32(comboBox_vga.SelectedIndex));
            settingFile.WriteInteger("Matrix", "HDMIMatrix", Decimal.ToInt32(comboBox_hdmi.SelectedIndex));
            settingFile.WriteInteger("Matrix", "YPbPrMatrix", Decimal.ToInt32(nud_YPbPrMatrix.Value));

            settingFile.WriteString("Matrix", "dvi-Matrix", comboBox_dvi.Text);
            settingFile.WriteString("Matrix", "video-Matrix", comboBox_video.Text);
            settingFile.WriteString("Matrix", "vga-Matrix", comboBox_vga.Text);
            settingFile.WriteString("Matrix", "hdmi-Matrix", comboBox_hdmi.Text);
            //settingFile.WriteInteger("SETTING", "YPbPrMatrix", Decimal.ToInt32(nud_YPbPrMatrix.Value));

            //记录主板类型
            if (radioButton4.Checked)
            {
                settingFile.WriteInteger("SETTING", "Motherboard", 0);
                mainForm.Motherboard_type = 0;
                mainForm.Motherboard_flag = 4;
            }
            else if (radioButton5.Checked)
            {
                settingFile.WriteInteger("SETTING", "Motherboard", 1);
                mainForm.Motherboard_type = 1;
                mainForm.Motherboard_flag = 4;
            }
            else if (radioButton7.Checked)
            {
                settingFile.WriteInteger("SETTING", "Motherboard", 4);
                mainForm.Motherboard_type = 4;
                mainForm.Motherboard_flag = 4;
            }
            else if (radioButton6.Checked)
            {
                settingFile.WriteInteger("SETTING", "Motherboard", 2);
                mainForm.Motherboard_type = 2;
                mainForm.Motherboard_flag = 2;
            }
            else if (radioButton3.Checked)
            {
                settingFile.WriteInteger("SETTING", "Motherboard", 3);
                mainForm.Motherboard_type = 3;
                mainForm.Motherboard_flag = 2;
            }
            else
            {
                settingFile.WriteInteger("SETTING", "Motherboard", 0);
                mainForm.Motherboard_type = 0;
                mainForm.Motherboard_flag = 4;
            }
            settingFile.WriteInteger("SETTING", "Matrix_time", comboBox1.SelectedIndex);
            settingFile.WriteInteger("SETTING", "Matrix_flag", Matrix_flag);
            if (Row_Col)
                settingFile.WriteString("SCREEN", "NUM", "");//清除场景记录
            settingFile.WriteString("SCREEN", "NUM_time", "30");
            mainForm.Matrix_time = (int)numericUpDown1.Value;
            mainForm.Matrix_flag = Matrix_flag;
        }

        private void bt_wallSetConfirm_Click(object sender, EventArgs e)
        {
            String sceDirectoryName = Application.StartupPath + "\\myWall";
            MainForm.currentConnectionName = sceDirectoryName;
            int t = 2;
            //Console.WriteLine(Row_Col);
            if (Row_Col)
            {
                t = FileHandle.createDirectory(sceDirectoryName, mainForm.Chinese_English);//文件夹创建好了，有内容的则确认覆盖掉了，开始初始化
            }
            //if(t != 2)
            {
                if (Row_Col)
                {
                    /*MainForm.rowsCount = cb_spliceRows.SelectedIndex + 1;
                    MainForm.colsCount = cb_spliceCols.SelectedIndex + 1;*/
                    MainForm.rowsCount = int.Parse(cb_spliceRows.Text);
                    MainForm.colsCount = int.Parse(cb_spliceCols.Text);
                    MainForm.currentSceneName = "scene.rgf";
                    mainForm.initSceneButton();
                    //mainForm.setSce1CurrentSceneButtonText();
                    mainForm.ResetGrid();
                    mainForm.initRoGridSet();
                    mainForm.initRoGridControl(int.Parse(cb_spliceRows.Text), int.Parse(cb_spliceCols.Text));
                    //mainForm.initRoGridControl(cb_spliceRows.SelectedIndex + 1, cb_spliceCols.SelectedIndex + 1);
                    mainForm.saveSceneFile(MainForm.currentConnectionName + "\\scene.rgf");
                    mainForm.setWall_flag = true;

                    LogHelper.WriteLog("======设置拼接【" + cb_spliceRows.Text + " X " + cb_spliceCols.Text + "】======");
                }
            }
            saveSettingFile();
            mainForm.initMatrixFromFile();
            if (radioButton4.Checked)
                mainForm.initSignalEdit(Decimal.ToInt32(nud_VGACount.Value), 0, 0, Decimal.ToInt32(nud_HDMICount.Value), 0, 12);
            else if (radioButton5.Checked)
                mainForm.initSignalEdit(Decimal.ToInt32(nud_VGACount.Value), 0, Decimal.ToInt32(nud_DVICount.Value), Decimal.ToInt32(nud_HDMICount.Value), 0, 12);
            else if (radioButton6.Checked)
                mainForm.initSignalEdit(Decimal.ToInt32(nud_VGACount.Value), Decimal.ToInt32(nud_VIDEOCount.Value), Decimal.ToInt32(nud_DVICount.Value), Decimal.ToInt32(nud_HDMICount.Value), 0, 12);
            else if (radioButton3.Checked)
                mainForm.initSignalEdit(Decimal.ToInt32(nud_VGACount.Value), 0, Decimal.ToInt32(nud_DVICount.Value), Decimal.ToInt32(nud_HDMICount.Value), 0, 12);
            else if (radioButton7.Checked)
                mainForm.initSignalEdit(0, 0, Decimal.ToInt32(nud_DVICount.Value), Decimal.ToInt32(nud_HDMICount.Value), 0, 12);

            mainForm.label18.Text = textBox2.Text.Trim();
            settingFile.WriteString("SETTING", "NamePath", textBox2.Text.Trim());
            this.Close();
        }

        /// <summary>
        /// 直接改行的文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_spliceRows_TextUpdate(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (int.Parse(cb.Text) > cb.Items.Count)
            {
                cb.Text = "" + cb.Items.Count;
            }
            //Console.WriteLine("文本改啦" + cb.Text);
        }

        /// <summary>
        /// 直接改列的文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_spliceCols_TextUpdate(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (int.Parse(cb.Text) > cb.Items.Count)
            {
                cb.Text = "" + cb.Items.Count;
            }

        }

        /// <summary>
        /// 检查地址是否与其他的重复
        /// </summary>
        /// <param name="checkAddress">待检查的地址</param>
        /// <param name="address">其他的地址</param>
        /// <returns>结果，true表示重复</returns>
        public bool checkAddressRepeat(decimal checkAddress, decimal[] arrAddress)
        {
            for (int i = 0; i < arrAddress.Length; i++)
            {
                if (checkAddress == arrAddress[i])
                {
                    return true;
                }
            }
            return false;
        }

        private void nud_DVIAddress_ValueChanged_1(object sender, EventArgs e)
        {
            //TextBox nud = sender as TextBox;
            //if (nud.Text == "")
                //nud_DVIAddress.Value = 0;
        }

        private void nud_VIDEOAddress_ValueChanged_1(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            //Console.WriteLine("DVIAddress" + nud.Value);
            decimal[] arrAddress = new decimal[4] { nud_DVIAddress.Value, nud_VGAAddress.Value, nud_HDMIAddress.Value, nud_YPbPrAddress.Value };
            if (checkAddressRepeat(nud.Value, arrAddress))
            {
                if (nud.Value < 255)
                    nud.Value = nud.Value + 1;
            }
        }

        private void nud_VGAAddress_ValueChanged_1(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            //Console.WriteLine("DVIAddress" + nud.Value);
            decimal[] arrAddress = new decimal[4] { nud_DVIAddress.Value, nud_VIDEOAddress.Value, nud_HDMIAddress.Value, nud_YPbPrAddress.Value };
            if (checkAddressRepeat(nud.Value, arrAddress))
            {
                if (nud.Value < 255)
                    nud.Value = nud.Value + 1;
            }
        }

        private void nud_HDMIAddress_ValueChanged_1(object sender, EventArgs e)
        {
            //TextBox nud = sender as TextBox;
            //if (nud.Text == "")
                //nud_HDMIAddress.Value = 0;
        }

        private void nud_YPbPrAddress_ValueChanged_1(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            //Console.WriteLine("DVIAddress" + nud.Value);
            decimal[] arrAddress = new decimal[4] { nud_DVIAddress.Value, nud_VGAAddress.Value, nud_HDMIAddress.Value, nud_VIDEOAddress.Value };
            if (checkAddressRepeat(nud.Value, arrAddress))
            {
                if (nud.Value < 255)
                    nud.Value = nud.Value + 1;
            }
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            mainForm.setWall_flag = false;
            this.Close();
        }
        /// <summary>
        /// 选择图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            mainForm.f_m = false;
            new Form_Modify(this.mainForm).ShowDialog();
            if (mainForm.f_m)
            {
                OpenFileDialog openfile = new OpenFileDialog();
                openfile.Filter = "图片文件|*.bmp;*.jpg;*.gif;*.png";

                if (openfile.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.ImageLocation = openfile.FileName;
                    textBox1.Text = openfile.FileName;
                }
                //button3.Enabled = true;
                openfile.Dispose();
            }
        }
        /// <summary>
        /// Logo图片设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            string filePath = Application.StartupPath + @"\pic";
            if (!System.IO.Directory.Exists(filePath))
            {
                // 目录不存在，建立目录
                System.IO.Directory.CreateDirectory(filePath);
            }
            if (!textBox1.Text.Equals(""))
            {
                mainForm.pictureBox1.Image.Dispose();//释放使用的资源
                mainForm.pictureBox1.Image = Image.FromFile(@textBox1.Text);  //动态添加图片  
                //mainForm.pictureBox1.Load(textBox1.Text);
                mainForm.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;  //是图片的大小适应控件PictureBox的大小 
                
                String sourcePath = @textBox1.Text.Trim();
                string strtype = sourcePath.Substring(sourcePath.Length - 4, 4);
                //Console.WriteLine("strtype:" + strtype);
                string targetPath = filePath + @"\logo" + strtype;
                bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
                try
                {
                    System.IO.File.Copy(sourcePath, targetPath, isrewrite);
                }
                catch
                {

                }
                settingFile.WriteString("SETTING", "PicturePath", "logo" + strtype);
            }
            mainForm.label18.Text = textBox2.Text.Trim();
            settingFile.WriteString("SETTING", "NamePath", textBox2.Text.Trim()); 
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
                Matrix_flag = 1;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
                Matrix_flag = 0;
            }
        }

        private bool IsInts(string str)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            return rex.IsMatch(str);
        }

        private void Init_Load()
        {
            //asc.controllInitializeSize(this);  
            DataTable tables = AccessFunction.GetTables();
            if (tables != null)
            {
                int count = tables.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    string a = tables.Rows[i][1].ToString();
                    string item = tables.Rows[i][2].ToString();
                    //Console.WriteLine("count" + item);
                    if (IsInts(item) && int.Parse(item) < 40)
                    {
                        //Console.WriteLine("return" + item);
                        continue;
                    }
                    else
                    {
                        //Console.WriteLine(item);
                        if (a == "video")
                        {
                            comboBox_video.Items.Add(item);
                        }
                        else if (a == "vga")
                        {
                            comboBox_vga.Items.Add(item);
                        }
                        else if (a == "dvi")
                        {
                            comboBox_dvi.Items.Add(item);
                        }
                        else if (a == "hdmi")
                        {
                            comboBox_hdmi.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void WallSetForm_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }
        private int str_count = 0;
        private void textBox2_TextChanged(object sender, EventArgs e)//限制对输入字符的长度的设置
        {
            str_count = 0;
            string str = textBox2.Text;
            for (int i = 0; i < textBox2.Text.Length; i++)
            {
                if ((int)str[i] > 127)//判断输入的字符是否是中文字符
                    str_count++;
            }
            textBox2.MaxLength = 54 - str_count;
            if (str.Equals("液晶拼接控制系统") || str.Equals("LCD Splicing Control System"))
                settingFile.WriteBool("SETTING", "NameFlag", false);
            else
                settingFile.WriteBool("SETTING", "NameFlag", true);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = Decimal.Parse(comboBox1.Text);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                //radioButton3.Enabled = false;
                //radioButton5.Enabled = false;
                //radioButton6.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                groupBox1.Enabled = true;
                groupBox5.Enabled = true;
                Row_Col = true;
            }
        }
        /// <summary>
        /// K6C
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                //radioButton4.Enabled = false;
                //radioButton5.Enabled = false;
                //radioButton6.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                groupBox2.Enabled = true;
                groupBox1.Enabled = true;
                groupBox5.Enabled = true;
                Row_Col = true;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                //radioButton4.Enabled = false;
                //radioButton3.Enabled = false;
                //radioButton6.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                groupBox2.Enabled = true;
                groupBox1.Enabled = true;
                groupBox5.Enabled = true;
                Row_Col = true;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                //radioButton4.Enabled = false;
                //radioButton3.Enabled = false;
                //radioButton5.Enabled = false;
                groupBox3.Enabled = true;
                groupBox4.Enabled = false;
                groupBox2.Enabled = true;
                groupBox1.Enabled = true;
                groupBox5.Enabled = true;
                Row_Col = true;
            }
        }

        private void button_Map_Click(object sender, EventArgs e)
        {
            new Form_MatrixMap(this, int.Parse(cb_spliceRows.Text), int.Parse(cb_spliceCols.Text)).ShowDialog();
        }

        private void button_AddPro_Click(object sender, EventArgs e)
        {
            new Form_Protocol(this).ShowDialog();
        }

        private void cb_spliceRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            Row_Col = true;
            if (!Row_Col)
            {
                int all = int.Parse(cb_spliceRows.Text) * int.Parse(cb_spliceCols.Text);
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < all; i++)
                    {
                        mainForm.PanelCount[j, i] = (byte)(i + 1);
                    }
                }
            }
        }

        private void cb_spliceCols_SelectedIndexChanged(object sender, EventArgs e)
        {
            Row_Col = true;
            if (!Row_Col)
            {
                int all = int.Parse(cb_spliceRows.Text) * int.Parse(cb_spliceCols.Text);
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < all; i++)
                    {
                        mainForm.PanelCount[j, i] = (byte)(i + 1);
                    }
                }
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                //radioButton4.Enabled = false;
                //radioButton3.Enabled = false;
                //radioButton6.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                groupBox2.Enabled = true;
                groupBox1.Enabled = true;
                groupBox5.Enabled = false;
                Row_Col = true;
            }
        }

        private void WallSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //保存信源矩阵选用
            settingFile.WriteInteger("Matrix", "DVIMatrix", Decimal.ToInt32(comboBox_dvi.SelectedIndex));
            settingFile.WriteInteger("Matrix", "VIDEOMatrix", Decimal.ToInt32(comboBox_video.SelectedIndex));
            settingFile.WriteInteger("Matrix", "VGAMatrix", Decimal.ToInt32(comboBox_vga.SelectedIndex));
            settingFile.WriteInteger("Matrix", "HDMIMatrix", Decimal.ToInt32(comboBox_hdmi.SelectedIndex));
            //settingFile.WriteInteger("Matrix", "YPbPrMatrix", Decimal.ToInt32(nud_YPbPrMatrix.Value));

            settingFile.WriteString("Matrix", "dvi-Matrix", comboBox_dvi.Text);
            settingFile.WriteString("Matrix", "video-Matrix", comboBox_video.Text);
            settingFile.WriteString("Matrix", "vga-Matrix", comboBox_vga.Text);
            settingFile.WriteString("Matrix", "hdmi-Matrix", comboBox_hdmi.Text);

            mainForm.initMatrixFromFile();
        }

    }
}
