using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_MatrixMap : Form
    {
        private int Row = 0;
        private int Col = 0;
        private TextBox textBox;
        private WallSetForm f;
        private IniFiles languageFile;
        public Form_MatrixMap(WallSetForm f,int row,int col)
        {
            InitializeComponent();
            this.Row = row;
            this.Col = col;
            this.f = f;
            languageFile = new IniFiles(Application.StartupPath + "\\LanguagePack" + f.mainForm.package);

            Init_FormString();
        }


                /// <summary>
        /// 语言设置加载
        /// </summary>
        private void Init_FormString()
        {
            string sl = languageFile.ReadString("WALLSETFORM", "MATRIX", "矩阵");
            if (sl.Contains("\0"))
                sl = sl.Replace("\0","");
            this.Text = sl + languageFile.ReadString("WALLSETFORM", "COMMAP", "端口映射");

            this.tabPage1.Text = sl + "HDMI";
            this.tabPage2.Text = sl + "DVI";
            this.tabPage3.Text = sl + "VIDEO";
            this.tabPage4.Text = sl + "VGA";

            this.button1.Text = languageFile.ReadString("WALLSETFORM", "OK", "确认");
            this.button1.Text = languageFile.ReadString("WALLSETFORM", "CLOSE", "关闭");
        }

        private void Form_MatrixMap_Load(object sender, EventArgs e)
        {
            dataGridView_HDMI.RowCount = Row;
            dataGridView_HDMI.ColumnCount = Col;
            dataGridView_DVI.RowCount = Row;
            dataGridView_DVI.ColumnCount = Col;
            dataGridView_VGA.RowCount = Row;
            dataGridView_VGA.ColumnCount = Col;
            dataGridView_VIDEO.RowCount = Row;
            dataGridView_VIDEO.ColumnCount = Col;
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    dataGridView_HDMI.Rows[i].Cells[j].Value = f.mainForm.PanelCount[0, i * Col + j];//i * Col + j + 1;
                    dataGridView_DVI.Rows[i].Cells[j].Value = f.mainForm.PanelCount[1, i * Col + j];//i * Col + j + 1;
                    dataGridView_VGA.Rows[i].Cells[j].Value = f.mainForm.PanelCount[2, i * Col + j];//i * Col + j + 1;
                    dataGridView_VIDEO.Rows[i].Cells[j].Value = f.mainForm.PanelCount[3, i * Col + j];//i * Col + j + 1;
                    //f.Panelcount[0,i * Col + j + 1] = (byte)(i * Col + j + 1);
                }
            }
        }

        private void MatrixMap_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.GetType().BaseType.Name == "TextBox")
            {
                textBox = (TextBox)e.Control;
                textBox.KeyPress += new KeyPressEventHandler(textBox_KeyPress);
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(dataGridView_HDMI.Rows[0].Cells[0].Value);           
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    f.mainForm.PanelCount[0, i * Col + j] = byte.Parse(dataGridView_HDMI.Rows[i].Cells[j].Value.ToString());
                }
            }
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    f.mainForm.PanelCount[1, i * Col + j] =  byte.Parse(dataGridView_DVI.Rows[i].Cells[j].Value.ToString());
                }
            }
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    f.mainForm.PanelCount[2, i * Col + j] =  byte.Parse(dataGridView_VGA.Rows[i].Cells[j].Value.ToString());
                }
            }
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    f.mainForm.PanelCount[3, i * Col + j] =  byte.Parse(dataGridView_VIDEO.Rows[i].Cells[j].Value.ToString());
                }
            }
            this.Close();
        }
    }
}
