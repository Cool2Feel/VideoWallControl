using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class Form_User : Form
    {
        //private string datasource = "";
        public Form_User(MainForm f)
        {
            InitializeComponent();
            //查询语句  
            /*
            string sqlCommandString = "select name from Login";
            //利用 Adapter 转换结果到 datagrid
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlCommandString, conn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            DataView dv = ds.Tables[0].DefaultView;

            dataGridView1.DataSource = dv;
             */ 
        }

        private void Form_User_Load(object sender, EventArgs e)
        {
            /*
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = datasource;
            connstr.Password = "admin";//设置密码，SQLite ADO.NET实现了数据库密码保护
            conn.ConnectionString = connstr.ToString();
            conn.Open(); //创建表
            //查询语句  
            string sqlCommandString = "select name from Login";
            //利用 Adapter 转换结果到 datagrid
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlCommandString, conn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            DataView dv = ds.Tables[0].DefaultView;

            dataGridView1.DataSource = dv;

            //conn.Close();
            //conn.Dispose(); 
             */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            string s3 = textBox3.Text;
            if (s1 != "" && s2 != "" && s2 == s3)
            {
                //string sql = "INSERT INTO Login VALUES(" + s1 + "," + s2 + ",'0')";
                //cmd.Parameters.Add("name", DbType.String).Value = s1;
                //cmd.Parameters.Add("password", DbType.String).Value = s2;
                //cmd.CommandText = sql;
                //cmd.ExecuteNonQuery();

                //cmd.CommandText = sql;
            }
            //databind();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            string s3 = textBox3.Text;
            if (s1 != "" && s2 != "" && s2 == s3)
            {
                //cmd.Parameters.Add("name", DbType.String).Value = s1;
                //cmd.Parameters.Add("password", DbType.String).Value = s2;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            if (s1 != "")
            {
                //cmd.Parameters.Add("name", DbType.String).Value = s1;
            }
        }

        private void Form_User_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
