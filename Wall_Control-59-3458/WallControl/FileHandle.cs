using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace WallControl
{
    public class FileHandle
    {
        /// <summary>
        /// 创建一个新文件夹，如无则直接创建，如有则询问是否创建（覆盖询问）
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        public static  int createDirectory(String directoryName,int ch)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
            if (directoryInfo.Exists)
            {
                /*
                DialogResult dialogResult;
                //Console.WriteLine("有目录" + directoryName);
                if (ch)
                    dialogResult = MessageBox.Show("The file already exists, whether to confirm the new?", "Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                else
                    dialogResult = MessageBox.Show("该文件已存在，是否确认新建?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK || dialogResult == DialogResult.Yes)
                 */ 
                { //确认删除文件夹下的文件

                    foreach (FileSystemInfo i in directoryInfo.GetFileSystemInfos())
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else
                        {
                            File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                    return 1;//删除了
                }
            }
            else
            {
                //Console.WriteLine("没有目录" + directoryName);
                directoryInfo.Create();
                return 3;//新建的目录

            }
        }

        public static int createDirectory(String directoryName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
            if (directoryInfo.Exists)
            {
                    foreach (FileSystemInfo i in directoryInfo.GetFileSystemInfos())
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else
                        {
                            File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                    return 1;//删除了
            }
            else
            {
                //Console.WriteLine("没有目录" + directoryName);
                directoryInfo.Create();
                return 2;//新建的目录
            }
        }


        /// <summary>
        /// 选择directoryName的文件夹，如无则直接创建，如有则不创建（无覆盖）
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        public static void selectDirectory(String directoryName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
            if (!directoryInfo.Exists)
            {
                //Console.WriteLine("有目录" + directoryName);
                directoryInfo.Create();

            }

        }
    }
}
