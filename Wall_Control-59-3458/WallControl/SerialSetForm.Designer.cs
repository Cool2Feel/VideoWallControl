namespace WallControl
{
    partial class SerialSetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialSetForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_baudRate2 = new System.Windows.Forms.ComboBox();
            this.cb_port2 = new System.Windows.Forms.ComboBox();
            this.cb_timeout = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_baudRate1 = new System.Windows.Forms.ComboBox();
            this.lb_baudRate = new System.Windows.Forms.Label();
            this.cb_serialSelect = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_port1 = new System.Windows.Forms.ComboBox();
            this.lb_port = new System.Windows.Forms.Label();
            this.cb_multiCom = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.bt_cancel = new System.Windows.Forms.Button();
            this.bt_confirm = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_IP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.combo_netpro = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_baudRate2);
            this.groupBox1.Controls.Add(this.cb_port2);
            this.groupBox1.Controls.Add(this.cb_timeout);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cb_baudRate1);
            this.groupBox1.Controls.Add(this.lb_baudRate);
            this.groupBox1.Controls.Add(this.cb_serialSelect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cb_port1);
            this.groupBox1.Controls.Add(this.lb_port);
            this.groupBox1.Controls.Add(this.cb_multiCom);
            this.groupBox1.Controls.Add(this.label20);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cb_baudRate2
            // 
            this.cb_baudRate2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_baudRate2.FormattingEnabled = true;
            resources.ApplyResources(this.cb_baudRate2, "cb_baudRate2");
            this.cb_baudRate2.Items.AddRange(new object[] {
            resources.GetString("cb_baudRate2.Items"),
            resources.GetString("cb_baudRate2.Items1"),
            resources.GetString("cb_baudRate2.Items2"),
            resources.GetString("cb_baudRate2.Items3"),
            resources.GetString("cb_baudRate2.Items4"),
            resources.GetString("cb_baudRate2.Items5"),
            resources.GetString("cb_baudRate2.Items6")});
            this.cb_baudRate2.Name = "cb_baudRate2";
            // 
            // cb_port2
            // 
            this.cb_port2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_port2.FormattingEnabled = true;
            resources.ApplyResources(this.cb_port2, "cb_port2");
            this.cb_port2.Name = "cb_port2";
            // 
            // cb_timeout
            // 
            this.cb_timeout.FormattingEnabled = true;
            resources.ApplyResources(this.cb_timeout, "cb_timeout");
            this.cb_timeout.Items.AddRange(new object[] {
            resources.GetString("cb_timeout.Items"),
            resources.GetString("cb_timeout.Items1"),
            resources.GetString("cb_timeout.Items2"),
            resources.GetString("cb_timeout.Items3"),
            resources.GetString("cb_timeout.Items4"),
            resources.GetString("cb_timeout.Items5"),
            resources.GetString("cb_timeout.Items6"),
            resources.GetString("cb_timeout.Items7"),
            resources.GetString("cb_timeout.Items8")});
            this.cb_timeout.Name = "cb_timeout";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Name = "label1";
            // 
            // cb_baudRate1
            // 
            this.cb_baudRate1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_baudRate1.FormattingEnabled = true;
            resources.ApplyResources(this.cb_baudRate1, "cb_baudRate1");
            this.cb_baudRate1.Items.AddRange(new object[] {
            resources.GetString("cb_baudRate1.Items"),
            resources.GetString("cb_baudRate1.Items1"),
            resources.GetString("cb_baudRate1.Items2"),
            resources.GetString("cb_baudRate1.Items3"),
            resources.GetString("cb_baudRate1.Items4"),
            resources.GetString("cb_baudRate1.Items5"),
            resources.GetString("cb_baudRate1.Items6")});
            this.cb_baudRate1.Name = "cb_baudRate1";
            // 
            // lb_baudRate
            // 
            resources.ApplyResources(this.lb_baudRate, "lb_baudRate");
            this.lb_baudRate.Name = "lb_baudRate";
            // 
            // cb_serialSelect
            // 
            this.cb_serialSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_serialSelect.FormattingEnabled = true;
            resources.ApplyResources(this.cb_serialSelect, "cb_serialSelect");
            this.cb_serialSelect.Items.AddRange(new object[] {
            resources.GetString("cb_serialSelect.Items"),
            resources.GetString("cb_serialSelect.Items1")});
            this.cb_serialSelect.Name = "cb_serialSelect";
            this.cb_serialSelect.SelectedIndexChanged += new System.EventHandler(this.cb_serialMulSelect_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cb_port1
            // 
            this.cb_port1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_port1.FormattingEnabled = true;
            resources.ApplyResources(this.cb_port1, "cb_port1");
            this.cb_port1.Name = "cb_port1";
            // 
            // lb_port
            // 
            resources.ApplyResources(this.lb_port, "lb_port");
            this.lb_port.Name = "lb_port";
            // 
            // cb_multiCom
            // 
            this.cb_multiCom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_multiCom.FormattingEnabled = true;
            resources.ApplyResources(this.cb_multiCom, "cb_multiCom");
            this.cb_multiCom.Items.AddRange(new object[] {
            resources.GetString("cb_multiCom.Items"),
            resources.GetString("cb_multiCom.Items1")});
            this.cb_multiCom.Name = "cb_multiCom";
            this.cb_multiCom.SelectedIndexChanged += new System.EventHandler(this.cb_multiCom_SelectedIndexChanged);
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // bt_cancel
            // 
            resources.ApplyResources(this.bt_cancel, "bt_cancel");
            this.bt_cancel.Name = "bt_cancel";
            this.bt_cancel.UseVisualStyleBackColor = true;
            this.bt_cancel.Click += new System.EventHandler(this.bt_cancel_Click);
            // 
            // bt_confirm
            // 
            resources.ApplyResources(this.bt_confirm, "bt_confirm");
            this.bt_confirm.Name = "bt_confirm";
            this.bt_confirm.UseVisualStyleBackColor = true;
            this.bt_confirm.Click += new System.EventHandler(this.bt_confirm_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Name = "label6";
            // 
            // checkBox2
            // 
            resources.ApplyResources(this.checkBox2, "checkBox2");
            this.checkBox2.ForeColor = System.Drawing.Color.Red;
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.ForeColor = System.Drawing.Color.Red;
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox3);
            this.groupBox2.Controls.Add(this.numericUpDown1);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.textBox_Port);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox_IP);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.combo_netpro);
            this.groupBox2.Controls.Add(this.label8);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBox3
            // 
            resources.ApplyResources(this.checkBox3, "checkBox3");
            this.checkBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox_Port
            // 
            resources.ApplyResources(this.textBox_Port, "textBox_Port");
            this.textBox_Port.Name = "textBox_Port";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBox_IP
            // 
            resources.ApplyResources(this.textBox_IP, "textBox_IP");
            this.textBox_IP.Name = "textBox_IP";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // combo_netpro
            // 
            this.combo_netpro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.combo_netpro, "combo_netpro");
            this.combo_netpro.FormattingEnabled = true;
            this.combo_netpro.Items.AddRange(new object[] {
            resources.GetString("combo_netpro.Items"),
            resources.GetString("combo_netpro.Items1"),
            resources.GetString("combo_netpro.Items2")});
            this.combo_netpro.Name = "combo_netpro";
            this.combo_netpro.SelectedIndexChanged += new System.EventHandler(this.combo_netpro_SelectedIndexChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // SerialSetForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bt_confirm);
            this.Controls.Add(this.bt_cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SerialSetForm";
            this.ShowIcon = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cb_multiCom;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cb_baudRate1;
        private System.Windows.Forms.Label lb_baudRate;
        private System.Windows.Forms.ComboBox cb_serialSelect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cb_port1;
        private System.Windows.Forms.Label lb_port;
        private System.Windows.Forms.ComboBox cb_timeout;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bt_confirm;
        private System.Windows.Forms.Button bt_cancel;
        private System.Windows.Forms.ComboBox cb_port2;
        private System.Windows.Forms.ComboBox cb_baudRate2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_IP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox combo_netpro;
        private System.Windows.Forms.Label label8;
    }
}