using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //1，新建创建连接的Socket对象
            Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //2，创建endPoint
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ip, 2424);

            //3，connect endPoint
            socketClient.Connect(endPoint);
            Console.WriteLine("连接{0}:{1}成功", endPoint.Address, endPoint.Port);
            Console.WriteLine("请输入信息, 输入AFK退出");

            //4，发送消息
            while (true)
            {
                string msg = Console.ReadLine();
                if (string.IsNullOrEmpty(msg))
                {
                    Console.WriteLine("输入内容不能为空，请重新输入");
                    continue;
                }
                else
                {
                    byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
                    socketClient.Send(msgBytes);
                    if (msg.Equals("AFK"))
                    {
                        socketClient.Shutdown(SocketShutdown.Both);
                        break;
                    }
                }
            }
        }
    }
}
