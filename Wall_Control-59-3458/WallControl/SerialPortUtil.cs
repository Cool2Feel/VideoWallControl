using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace WallControl
{
    class SerialPortUtil
    {
        //private static SerialPort serialPort=new SerialPort();

        /// <summary>
        /// 通过serialPort发送sendData的从offset开始的count个数据
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="sendData"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public static bool serialPortSendData(SerialPort serialPort, byte[] sendData, int offset, int count, int delay, int sendCount)
        {
            for (int i = 0; i < sendCount; i++)
            {
                if (serialPort.IsOpen)
                {
                    try
                    {
                        serialPort.Write(sendData, offset, count);
                        //Console.WriteLine("fasong发送");
                        Thread.Sleep(delay);
                    }
                    catch
                    {
                        //MessageBox.Show("串口发送数据异常！" + e.Message, "系统提示");
                        return false;
                    }

                }
                else
                {
                    //MessageBox.Show("串口断开，请检查并打开！", "系统提示");
                    return false;
                }
            }
            return true;
        }

        public static bool serialPortSendStr(SerialPort serialPort, string strData, int delay, int sendCount)
        {
            for (int i = 0; i < sendCount; i++)
            {
                if (serialPort.IsOpen)
                {
                    try
                    {
                        serialPort.Write(strData);
                        //Console.WriteLine("fasong发送");
                        Thread.Sleep(delay);
                    }
                    catch
                    {
                        //MessageBox.Show("串口发送数据异常！" + e.Message, "系统提示");
                        return false;
                    }
                }
                else
                {
                    //MessageBox.Show("串口断开，请检查并打开！", "系统提示");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 设置串口通讯
        /// </summary>
        /// <param name="portName">串口名</param>
        /// <param name="baudRate">波特率</param>
        public static void setSerialPort(SerialPort serialPort, String portName, int baudRate)
        {
            serialPort.BaudRate = baudRate;
            serialPort.PortName = portName; 
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public static void openSerialPort(SerialPort serialPort)
        {

            try
            {
                //MessageBox.Show("串口打开！", "系统提示");
                serialPort.Open();

            }
            catch (Exception ex)
            {
                MessageBox.Show("串口打开失败，请检查！" + ex.Message, "系统提示");
                serialPort.Close();
            }

        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public static void closeSerialPort(SerialPort serialPort)
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("串口关闭异常，请检查！" + ex.Message, "系统提示");
            }

        }
    }
}
