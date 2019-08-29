using System;
using System.Collections.Generic;
using System.Text;

namespace WallControl
{
    /// <summary>
    /// 屏幕的属性实体类
    /// </summary>
    public class Screen
    {
        private String name;//名字，U+第几块
        private String intputType;//信源的类型
        private int number;//第几块

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }


        public String IntputType
        {
            get { return intputType; }
            set { intputType = value; }
        }


        public override string ToString()
        {
            return name + " " + intputType;
        } 
 

    }
}
