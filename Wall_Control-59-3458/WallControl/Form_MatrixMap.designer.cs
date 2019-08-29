namespace WallControl
{
    partial class Form_MatrixMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_MatrixMap));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView_HDMI = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView_DVI = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridView_VGA = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dataGridView_VIDEO = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_HDMI)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_DVI)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_VGA)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_VIDEO)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.dataGridView_HDMI);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView_HDMI
            // 
            this.dataGridView_HDMI.AllowUserToDeleteRows = false;
            this.dataGridView_HDMI.AllowUserToResizeColumns = false;
            this.dataGridView_HDMI.AllowUserToResizeRows = false;
            this.dataGridView_HDMI.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_HDMI.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_HDMI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_HDMI.ColumnHeadersVisible = false;
            resources.ApplyResources(this.dataGridView_HDMI, "dataGridView_HDMI");
            this.dataGridView_HDMI.Name = "dataGridView_HDMI";
            this.dataGridView_HDMI.RowHeadersVisible = false;
            this.dataGridView_HDMI.RowTemplate.Height = 23;
            this.dataGridView_HDMI.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.MatrixMap_EditingControlShowing);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView_DVI);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView_DVI
            // 
            this.dataGridView_DVI.AllowUserToDeleteRows = false;
            this.dataGridView_DVI.AllowUserToResizeColumns = false;
            this.dataGridView_DVI.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_DVI.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_DVI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_DVI.ColumnHeadersVisible = false;
            resources.ApplyResources(this.dataGridView_DVI, "dataGridView_DVI");
            this.dataGridView_DVI.Name = "dataGridView_DVI";
            this.dataGridView_DVI.RowHeadersVisible = false;
            this.dataGridView_DVI.RowTemplate.Height = 23;
            this.dataGridView_DVI.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.MatrixMap_EditingControlShowing);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridView_VGA);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView_VGA
            // 
            this.dataGridView_VGA.AllowUserToDeleteRows = false;
            this.dataGridView_VGA.AllowUserToResizeColumns = false;
            this.dataGridView_VGA.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_VGA.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_VGA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_VGA.ColumnHeadersVisible = false;
            resources.ApplyResources(this.dataGridView_VGA, "dataGridView_VGA");
            this.dataGridView_VGA.Name = "dataGridView_VGA";
            this.dataGridView_VGA.RowHeadersVisible = false;
            this.dataGridView_VGA.RowTemplate.Height = 23;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dataGridView_VIDEO);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dataGridView_VIDEO
            // 
            this.dataGridView_VIDEO.AllowUserToDeleteRows = false;
            this.dataGridView_VIDEO.AllowUserToResizeColumns = false;
            this.dataGridView_VIDEO.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_VIDEO.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_VIDEO.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_VIDEO.ColumnHeadersVisible = false;
            resources.ApplyResources(this.dataGridView_VIDEO, "dataGridView_VIDEO");
            this.dataGridView_VIDEO.Name = "dataGridView_VIDEO";
            this.dataGridView_VIDEO.RowHeadersVisible = false;
            this.dataGridView_VIDEO.RowTemplate.Height = 23;
            // 
            // Form_MatrixMap
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_MatrixMap";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.Form_MatrixMap_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_HDMI)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_DVI)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_VGA)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_VIDEO)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView_HDMI;
        private System.Windows.Forms.DataGridView dataGridView_DVI;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dataGridView_VGA;
        private System.Windows.Forms.DataGridView dataGridView_VIDEO;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}