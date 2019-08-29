namespace WallControl
{
    partial class Form_BText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BText));
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.subTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subTanstoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.RighttoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.LefttoolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.subStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label_text = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fontDialog1
            // 
            this.fontDialog1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subTextToolStripMenuItem,
            this.subColorToolStripMenuItem,
            this.subTanstoolStripMenuItem1,
            this.RighttoolStripMenuItem1,
            this.LefttoolStripMenuItem2,
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
            // 
            // RighttoolStripMenuItem1
            // 
            this.RighttoolStripMenuItem1.Name = "RighttoolStripMenuItem1";
            resources.ApplyResources(this.RighttoolStripMenuItem1, "RighttoolStripMenuItem1");
            this.RighttoolStripMenuItem1.Click += new System.EventHandler(this.RighttoolStripMenuItem1_Click);
            // 
            // LefttoolStripMenuItem2
            // 
            this.LefttoolStripMenuItem2.Checked = true;
            this.LefttoolStripMenuItem2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LefttoolStripMenuItem2.Name = "LefttoolStripMenuItem2";
            resources.ApplyResources(this.LefttoolStripMenuItem2, "LefttoolStripMenuItem2");
            this.LefttoolStripMenuItem2.Click += new System.EventHandler(this.LefttoolStripMenuItem2_Click);
            // 
            // subStopToolStripMenuItem
            // 
            this.subStopToolStripMenuItem.CheckOnClick = true;
            this.subStopToolStripMenuItem.Name = "subStopToolStripMenuItem";
            resources.ApplyResources(this.subStopToolStripMenuItem, "subStopToolStripMenuItem");
            this.subStopToolStripMenuItem.Click += new System.EventHandler(this.subStopToolStripMenuItem_Click);
            // 
            // subStartToolStripMenuItem
            // 
            this.subStartToolStripMenuItem.Checked = true;
            this.subStartToolStripMenuItem.CheckOnClick = true;
            this.subStartToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
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
            // label_text
            // 
            resources.ApplyResources(this.label_text, "label_text");
            this.label_text.BackColor = System.Drawing.Color.Transparent;
            this.label_text.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label_text.Name = "label_text";
            // 
            // Form_BText
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.label_text);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_BText";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.Form_BText_Load);
            this.MouseEnter += new System.EventHandler(this.Form_BText_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.Form_BText_MouseLeave);
            this.Resize += new System.EventHandler(this.Form_BText_Resize);
            this.LocationChanged += new System.EventHandler(this.Form_BText_LocationChanged);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem subTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subTanstoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem subStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subCloseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RighttoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem LefttoolStripMenuItem2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label_text;
    }
}