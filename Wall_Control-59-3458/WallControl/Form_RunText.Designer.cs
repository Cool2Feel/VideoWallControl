namespace WallControl
{
    partial class Form_RunText
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_RunText));
            this.label_text = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.subTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subTanstoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.LefttoolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.RighttoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.AutotoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.CenteredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_text
            // 
            resources.ApplyResources(this.label_text, "label_text");
            this.label_text.BackColor = System.Drawing.Color.Transparent;
            this.label_text.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label_text.Name = "label_text";
            this.label_text.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_text_MouseMove);
            this.label_text.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_text_MouseDown);
            this.label_text.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_text_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subTextToolStripMenuItem,
            this.subColorToolStripMenuItem,
            this.subTanstoolStripMenuItem1,
            this.LefttoolStripMenuItem2,
            this.RighttoolStripMenuItem1,
            this.AutotoolStripMenuItem1,
            this.subStopToolStripMenuItem,
            this.subStartToolStripMenuItem,
            this.subCloseToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // subTextToolStripMenuItem
            // 
            this.subTextToolStripMenuItem.Name = "subTextToolStripMenuItem";
            resources.ApplyResources(this.subTextToolStripMenuItem, "subTextToolStripMenuItem");
            this.subTextToolStripMenuItem.Click += new System.EventHandler(this.subTextToolStripMenuItem_Click);
            // 
            // subColorToolStripMenuItem
            // 
            this.subColorToolStripMenuItem.Name = "subColorToolStripMenuItem";
            resources.ApplyResources(this.subColorToolStripMenuItem, "subColorToolStripMenuItem");
            this.subColorToolStripMenuItem.Click += new System.EventHandler(this.subColorToolStripMenuItem_Click);
            // 
            // subTanstoolStripMenuItem1
            // 
            this.subTanstoolStripMenuItem1.CheckOnClick = true;
            this.subTanstoolStripMenuItem1.Name = "subTanstoolStripMenuItem1";
            resources.ApplyResources(this.subTanstoolStripMenuItem1, "subTanstoolStripMenuItem1");
            this.subTanstoolStripMenuItem1.CheckStateChanged += new System.EventHandler(this.subTanstoolStripMenuItem1_CheckStateChanged);
            // 
            // LefttoolStripMenuItem2
            // 
            this.LefttoolStripMenuItem2.Checked = true;
            this.LefttoolStripMenuItem2.CheckOnClick = true;
            this.LefttoolStripMenuItem2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LefttoolStripMenuItem2.Name = "LefttoolStripMenuItem2";
            resources.ApplyResources(this.LefttoolStripMenuItem2, "LefttoolStripMenuItem2");
            this.LefttoolStripMenuItem2.CheckedChanged += new System.EventHandler(this.LefttoolStripMenuItem2_CheckedChanged);
            // 
            // RighttoolStripMenuItem1
            // 
            this.RighttoolStripMenuItem1.CheckOnClick = true;
            this.RighttoolStripMenuItem1.Name = "RighttoolStripMenuItem1";
            resources.ApplyResources(this.RighttoolStripMenuItem1, "RighttoolStripMenuItem1");
            this.RighttoolStripMenuItem1.CheckedChanged += new System.EventHandler(this.RighttoolStripMenuItem1_CheckedChanged);
            // 
            // AutotoolStripMenuItem1
            // 
            this.AutotoolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CenteredToolStripMenuItem,
            this.LeftToolStripMenuItem,
            this.RightToolStripMenuItem});
            resources.ApplyResources(this.AutotoolStripMenuItem1, "AutotoolStripMenuItem1");
            this.AutotoolStripMenuItem1.Name = "AutotoolStripMenuItem1";
            // 
            // CenteredToolStripMenuItem
            // 
            this.CenteredToolStripMenuItem.CheckOnClick = true;
            this.CenteredToolStripMenuItem.Name = "CenteredToolStripMenuItem";
            resources.ApplyResources(this.CenteredToolStripMenuItem, "CenteredToolStripMenuItem");
            this.CenteredToolStripMenuItem.Click += new System.EventHandler(this.CenteredToolStripMenuItem_Click);
            // 
            // LeftToolStripMenuItem
            // 
            this.LeftToolStripMenuItem.CheckOnClick = true;
            this.LeftToolStripMenuItem.Name = "LeftToolStripMenuItem";
            resources.ApplyResources(this.LeftToolStripMenuItem, "LeftToolStripMenuItem");
            this.LeftToolStripMenuItem.Click += new System.EventHandler(this.LeftToolStripMenuItem_Click);
            // 
            // RightToolStripMenuItem
            // 
            this.RightToolStripMenuItem.CheckOnClick = true;
            this.RightToolStripMenuItem.Name = "RightToolStripMenuItem";
            resources.ApplyResources(this.RightToolStripMenuItem, "RightToolStripMenuItem");
            this.RightToolStripMenuItem.Click += new System.EventHandler(this.RightToolStripMenuItem_Click);
            // 
            // subStopToolStripMenuItem
            // 
            this.subStopToolStripMenuItem.CheckOnClick = true;
            this.subStopToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.subStopToolStripMenuItem.Name = "subStopToolStripMenuItem";
            resources.ApplyResources(this.subStopToolStripMenuItem, "subStopToolStripMenuItem");
            this.subStopToolStripMenuItem.Click += new System.EventHandler(this.subStopToolStripMenuItem_Click);
            // 
            // subStartToolStripMenuItem
            // 
            this.subStartToolStripMenuItem.Checked = true;
            this.subStartToolStripMenuItem.CheckOnClick = true;
            this.subStartToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.subStartToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.subStartToolStripMenuItem.Name = "subStartToolStripMenuItem";
            resources.ApplyResources(this.subStartToolStripMenuItem, "subStartToolStripMenuItem");
            this.subStartToolStripMenuItem.Click += new System.EventHandler(this.subStartToolStripMenuItem_Click);
            // 
            // subCloseToolStripMenuItem
            // 
            this.subCloseToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.subCloseToolStripMenuItem.Name = "subCloseToolStripMenuItem";
            resources.ApplyResources(this.subCloseToolStripMenuItem, "subCloseToolStripMenuItem");
            this.subCloseToolStripMenuItem.Click += new System.EventHandler(this.subCloseToolStripMenuItem_Click);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Form_RunText
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.label_text);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_RunText";
            this.ShowIcon = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_RunText_Load);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Form_RunText_MouseDoubleClick);
            this.Resize += new System.EventHandler(this.Form_RunText_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_text;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem subTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subCloseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subTanstoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem RighttoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem LefttoolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem AutotoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem CenteredToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RightToolStripMenuItem;
        private System.Windows.Forms.Timer timer2;
    }
}