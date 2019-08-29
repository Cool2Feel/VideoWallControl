using System;
using System.Collections.Generic;
using System.Text;

namespace WallControl
{
    class MergeGroup
    {
        private Screen startScreen;
        private Screen endScreen;

        private int groupNumber;
        private List<Screen> mergeScreenList;//所有的合并的屏

        public List<Screen> MergeScreenList
        {
            get { return mergeScreenList; }
            set { mergeScreenList = value; }
        }
        public Screen StartScreen
        {
            get { return startScreen; }
            set { startScreen = value; }
        }


        public Screen EndScreen
        {
            get { return endScreen; }
            set { endScreen = value; }
        }


        public int GroupNumber
        {
            get { return groupNumber; }
            set { groupNumber = value; }
        }



    }
}
