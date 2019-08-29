using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace WallControl
{
    class SkinClass
    {
        public static Sunisoft.IrisSkin.SkinEngine se = null;



        /// <summary>
        /// 增加换肤菜单
        /// </summary>
        /// <param name="toolMenu"></param>
        public static void AddSkinMenu(ContextMenuStrip toolMenu)
        {
            DataSet skin = new DataSet();
            try
            {

                skin.ReadXml("skin.xml", XmlReadMode.Auto);
            }
            catch
            {

            }
            if (skin == null || skin.Tables.Count < 1)
            {
                skin = new DataSet();
                skin.Tables.Add("skin");
                skin.Tables["skin"].Columns.Add("style");
                System.Data.DataRow dr = skin.Tables["skin"].NewRow();
                dr[0] = "系统默认";
                skin.Tables[0].Rows.Add(dr);
                skin.WriteXml("skin.xml", XmlWriteMode.IgnoreSchema);
            }
            foreach (SkinType st in (SkinType[])System.Enum.GetValues(typeof(SkinType)))
            {
                toolMenu.Items.Add(new ToolStripMenuItem(st.ToString()));

                toolMenu.Items[toolMenu.Items.Count - 1].Click += new EventHandler(frm_Main_Click);
                if (st.ToString() == skin.Tables[0].Rows[0][0].ToString())
                {
                    ((ToolStripMenuItem)toolMenu.Items[toolMenu.Items.Count - 1]).Checked = true;
                    frm_Main_Click(toolMenu.Items[toolMenu.Items.Count - 1], null);
                }

            }

            toolMenu.Items.Add(new ToolStripMenuItem("系统默认"));
            toolMenu.Items[toolMenu.Items.Count - 1].Click += new EventHandler(frm_Main_Click);
            if (skin.Tables[0].Rows[0][0].ToString() == "系统默认")
            {
                ((ToolStripMenuItem)toolMenu.Items[toolMenu.Items.Count - 1]).Checked = true;
            }
        }

        static void frm_Main_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < (((ToolStripMenuItem)sender).Owner).Items.Count; i++)
            {

                if (((ToolStripMenuItem)sender).Text == (((ToolStripMenuItem)sender).Owner).Items[i].Text)
                {
                    ((ToolStripMenuItem)sender).CheckState = CheckState.Checked;
                    DataSet skin = new DataSet();
                    skin.Tables.Add("skin");
                    skin.Tables["skin"].Columns.Add("style");
                    System.Data.DataRow dr = skin.Tables["skin"].NewRow();
                    dr[0] = (((ToolStripMenuItem)sender).Owner).Items[i].Text;
                    skin.Tables[0].Rows.Add(dr);
                    skin.WriteXml("skin.xml", XmlWriteMode.IgnoreSchema);

                }
                else
                {
                    ((ToolStripMenuItem)(((ToolStripMenuItem)sender).Owner).Items[i]).CheckState = CheckState.Unchecked;
                }
            }
            if (((ToolStripMenuItem)sender).Text == "系统默认")
            {
                RemoveSkin();
                DataSet skin = new DataSet();
                skin.Tables.Add("skin");
                skin.Tables["skin"].Columns.Add("style");
                System.Data.DataRow dr = skin.Tables["skin"].NewRow();
                dr[0] = "系统默认";
                skin.Tables[0].Rows.Add(dr);
                skin.WriteXml("skin.xml", XmlWriteMode.IgnoreSchema);
                return;
            }
            foreach (SkinType st in (SkinType[])System.Enum.GetValues(typeof(SkinType)))
            {
                if (st.ToString() == ((ToolStripMenuItem)sender).Text)
                {
                    ChangeSkin(st);
                    return;
                }
            }
        }
        /// <summary>
        /// 改变皮肤
        /// </summary>
        /// <param name="st"></param>

        public static void ChangeSkin(SkinType st)
        {



            System.Reflection.Assembly thisDll = System.Reflection.Assembly.GetExecutingAssembly();


            if (se == null)
            {
                //WallControl是指命名空间，这里你可以换成你自己的。
                se = new Sunisoft.IrisSkin.SkinEngine(Application.OpenForms[0], thisDll.GetManifestResourceStream("WallControl.skin." + st.ToString() + ".ssk"));
                se.Active = true;
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    se.AddForm(Application.OpenForms[i]);
                }

            }
            else
            {
                //WallControl是指命名空间，这里你可以换成你自己的。
               se.SkinAllForm = true;     // 这句话是用来设置整个系统下所有窗体都采用这个皮肤
               se.SkinStream = thisDll.GetManifestResourceStream("WallControl.skin." + st.ToString() + ".ssk");
               se.Active = true;
            }
        }
        /// <summary>
        /// 移除皮肤
        /// </summary>
        public static void RemoveSkin()
        {
            if (se == null)
            {
                return;
            }
            else
            {
                se.Active = false;
            }
        }
    }

    /// <summary>
    /// 换肤类型
    /// </summary>
    public enum SkinType
    {
        Calmness,
        DeepCyan,
        Eighteen,
        Emerald,
        GlassBrown,
        Longhorn,
        MacOS,
        Midsummer,
        MP10,
        MSN,
        OneBlue,
        Page,
        RealOne,
        Silver,
        SportsBlack,
        SteelBlack,
        vista1,
        SportsCyan,
        Warm,
        //Wave,
        XPSilver
    }
}
