using System;
using System.Management;

namespace WallControl
{
   public static class Register
    {
        /// <summary>
        /// 功能：用于获得Mac地址、CUP、硬盘编号、生成机器码、注册码
        /// </summary>

        #region Members
        public static int[] intCode = new int[126];
        public static int[] mNumber = new int[24];
        public static char[] mChar = new char[24];
        #endregion

        #region Methods

        /// <summary>
        /// 获取本机的Mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMac()
       {
           string mac="";
           ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
           ManagementObjectCollection moc = mc.GetInstances();
           foreach (ManagementObject mo in moc)
           {
               if ((bool)mo["IPEnabled"])
               {
                   mac=mo["MacAddress"].ToString();
                   break;
               }
           }
           return mac;
       }

       /// <summary>
       /// 获取硬盘序列号
       /// </summary>
       /// <returns></returns>
       public static string GetHD()
       {
           string hd = "";
           ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
           ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
           disk.Get();
           hd=disk.GetPropertyValue("VolumeSerialNumber").ToString();
           return hd;
       }

       /// <summary>
       /// 获取CPU号
       /// </summary>
       /// <returns></returns>
       public static string GetCPU()
       {
           string cpu = "";
           ManagementClass mc = new ManagementClass("win32_Processor");
           ManagementObjectCollection moc = mc.GetInstances();
           foreach (ManagementObject mo in moc)
           {
               cpu = mo.Properties["Processorid"].Value.ToString();
           }
           return cpu;
       }

       /// <summary>
       /// 生成机器码
       /// </summary>
       /// <returns></returns>
       public static string  GetMachineNumber()
       {
           string s="";
           string yString=GetMac()+GetHD();
           /*
           string[] rNumber=new string[24];
           Random r=new Random();
           for(int i=0;i<24;i++)
           {
               rNumber[i]=yString.Substring(r.Next(0,yString.Length),1);
               s+=rNumber[i];
           }
            */
           s = yString.Substring(0, 24);
           return s;
       }
       
       /// <summary>
       /// 初始化数组
       /// </summary>
       public static void InitCode()
       {
           //Random r = new Random();
           for (int i = 0; i < intCode.Length; i++)
           {
               intCode[i] = i % 9;// r.Next(0, 9);;
           }
       }
      
       /// <summary>
       /// 生成注册码
       /// </summary>
       /// <returns></returns>
       public static string GetRegisterNumber()
       {
           InitCode();
           string machineNumber = GetMachineNumber();
           string registerNumber = "";
           for (int i = 0; i < mChar.Length; i++)   //存储机器码
           {
               mChar[i] = Convert.ToChar(machineNumber.Substring(i, 1));
           }
           for (int j = 0; j < mNumber.Length; j++)  //改变ASCII码值
           {
               mNumber[j] = Convert.ToInt32(mChar[j]) + intCode[Convert.ToInt32(mChar[j])];
           }
           for (int k = 0; k < mNumber.Length; k++)  //生成注册码
           {
               if ((mNumber[k] >= 48 && mNumber[k] <= 57) || (mNumber[k] >= 65 && mNumber[k] <= 90) || (mNumber[k] >= 97 && mNumber[k] <= 122))  //判断如果在0-9、A-Z、a-z之间
               {
                   registerNumber += Convert.ToChar(mNumber[k]).ToString();
               }
                   else if (mNumber[k] > 122)  //判断如果大于z
               {
                   registerNumber += Convert.ToChar(mNumber[k] - 10).ToString();
               }
               else
               {
                   registerNumber += Convert.ToChar(mNumber[k] - 9).ToString();
               }
           }
           /*
               else if (mNumber[i] >= 65 && mNumber[i] <= 90)
               {
                   registerNumber += (Char)mNumber[i];
               }
               else if (mNumber[i] >= 97 && mNumber[i] <= 122)
               {
                   registerNumber += (Char)mNumber[i];
               }
               else
               {
                   if (mNumber[i] > 122)
                   {
                       registerNumber += (Char)(mNumber[i] - 10);
                   }
                   else
                   {
                       registerNumber += (Char)(mNumber[i] - 9);
                   }
               }
           }
            */ 
           return registerNumber;
       }

        #endregion
    }
}
