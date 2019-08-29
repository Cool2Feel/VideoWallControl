using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_WaitRun : Form
    {
        private MainForm f;
        private List<int> L;
        private int Time_done;
        private int b;
        private int k = 0;
        private int timeCount = 0;
        private IniFiles languageFile;
        private bool RunLoop = true;
        //AutoSizeFormClass asc = new AutoSizeFormClass(); 
        public Form_WaitRun(MainForm f, List<int> L, int t,int b)
        {
            this.f = f;
            if (L == null)
                RunLoop = false;
            else
            {
                this.L = L;
            }
            this.Time_done = t;//设置的轮巡时间
            this.b = b;
            InitializeComponent();
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.package);
            //Init_FormString();
            
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

            label4.Text = Time_done.ToString();
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
        }


        private void ApplyResource()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_WaitRun));
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
                //Console.WriteLine(ctl.Name);
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
            this.button1.Text = languageFile.ReadString("RUNFORM", "STOP", "停止轮巡");
            this.label1.Text = languageFile.ReadString("RUNFORM", "TIPS ", "预案场景轮巡期间禁止其他操作！");
            this.label3.Text = languageFile.ReadString("RUNFORM", "TIME", "轮巡时间(S)：");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!RunLoop)
                f.Stop_Loop();
            timer1.Stop();
            this.Close();
        }
        private int num = 0;
        private bool L_flag = true;
        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = (Time_done - timeCount).ToString();
            //timeCount++;//定时累加
            if (timeCount++ >= Time_done)
            {
                if (RunLoop)
                {
                    if (k < L.Count)
                    {
                        timeCount = 0;//时间归 0
                        if (L_flag && L.Count > 1)//判断是否由当前开始轮巡且不止一个场景的情况下
                        {
                            if (b == L[k])//当从当前场景开始轮巡时；时间到后自动到下一场景
                            {
                                num = L[++k];
                                b = 0;
                            }
                            else//不在当前场景，直接切到下一场景
                                num = L[k];
                        }
                        else//直接切到下一场景
                            num = L[k];
                        this.Invoke(new MethodInvoker(delegate()
                        {
                            f.Auto_Run(num);//执行轮巡操作
                        }));
                        k++;
                        L_flag = false;
                    }
                    else
                        k = 0;//从头开始轮巡
                    if (timeCount > 30)
                        timeCount = 30;
                }
                else
                {
                    if (timeCount > Time_done)
                        timeCount = 0;
                }
            }
        }

        private void Form_WaitRun_Load(object sender, EventArgs e)
        {
            //asc.controllInitializeSize(this);  
        }

        private void Form_WaitRun_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }


    }
}
