using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace WallControl
{
    public class SendDataMap
    {
        public SerialPort serialPort;//发送数据的串口
        public byte[] data;
        public int offset;
        public int count;
        public int cycleCount;//循环发送次数

        public SendDataMap(SerialPort serialPort, byte[] data, int offset, int count, int cycleCount)
        {
            this.serialPort = serialPort;
            this.data = data;
            this.offset = offset;
            this.count = count;
            this.cycleCount = cycleCount;
        }
    }
}
