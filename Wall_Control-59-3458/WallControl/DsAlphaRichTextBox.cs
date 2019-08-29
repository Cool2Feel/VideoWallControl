using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class DsAlphaRichTextBox : RichTextBox
    {
        public DsAlphaRichTextBox()
        {
            InitializeComponent();
            this.Cursor = Cursors.Arrow;
            this.ReadOnly = true;
            this.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this, true, null); 
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x7 || m.Msg == 0x201 || m.Msg == 0x202 || m.Msg == 0x203 || m.Msg == 0x204 || m.Msg == 0x205 || m.Msg == 0x206 || m.Msg == 0x0100 || m.Msg == 0x0101)
            {
                return;
            }
            base.WndProc(ref m);
        }  

    }
}
