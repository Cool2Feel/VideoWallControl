using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WallControl
{
    public partial class ColorDropDownList : ComboBox
    {
        public ColorDropDownList()
        {
            InitializeComponent();
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.Items.Clear();//清空原有项
            fillList();
            this.SelectedIndex = 0;
            this.DrawItem += new DrawItemEventHandler(ColorDropDownList_DrawItem);  
        }

        private void fillList()
        {
            //string[] names = Enum.GetNames(typeof(KnownColor));
            string[] names = {
            "Black",
            "Blue",
            "Gold",
            "Green",
            "Lime",
            "Linen",
            "Maroon",
            "Navy",
            "Olive",
            "Orange",
            "Pink",
            "Plum",
            "Purple",
            "Red",
            "Salmon",
            "Silver",
            "Teal",
            "White",
            "Yellow"
            };
            this.Items.AddRange(names);
            /*
            this.Items.Add(Color.Black.ToString());
            this.Items.Add(Color.Blue.ToString());
            this.Items.Add(Color.Maroon.ToString());
            this.Items.Add(Color.Navy.ToString());
            this.Items.Add(Color.Purple.ToString());
            this.Items.Add(Color.Teal.ToString());
            this.Items.Add(Color.Gray.ToString());
            this.Items.Add(Color.Red.ToString());
            this.Items.Add(Color.Silver.ToString());
            this.Items.Add(Color.Olive.ToString());
             */ 
        }

        private void ColorDropDownList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)//判断是否需要重绘
            {
                string colorName = (string)this.Items[e.Index];
                Color color = Color.FromName(colorName);
                Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width / 4, e.Bounds.Height - 2);
                Brush brush = new SolidBrush(color);
                e.Graphics.DrawRectangle(new Pen(Color.Black), rect);
                rect.X += 1;
                rect.Y += 1;
                rect.Width -= 1;
                rect.Height -= 1;
                e.Graphics.FillRectangle(brush, rect);
                rect.Offset(rect.Width + 4, 0);
                e.Graphics.DrawString(colorName, e.Font, Brushes.Black, rect.Location);
            }
        }

        /// <summary>
        /// 选择的颜色名称
        /// </summary>
        public string SelectColorName
        {
            get { return this.Text; }
        }

        /// <summary>
        /// 选择的颜色
        /// </summary>
        public Color SelectColor
        {
            get { return Color.FromName(this.Text); }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
