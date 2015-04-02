using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        //测试请求：telnet 127.0.0.1 2424
        static void Main(string[] args)
        {
            //1，服务器端创建一个负责监IP地址跟端口号的Socket
            Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //2，创建ip端口号的EndPoint
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ip, 2424);
            //3，监听EndPoint
            socketWatch.Bind(ipEndPoint);
            Console.WriteLine("监听127.0.0.1:2424中...");
            //4，设置监听队列长度
            socketWatch.Listen(10);

            //5-1，接受客户端请求，非线程
            //AcceptRequest(socketWatch);
            //5-2，接受客户端请求，用线程
            //因为Socket.Accept()方法会阻碍主线程的执行，因为Accept会一直监听请求直到监听到
            Thread acceptThread = new Thread(AcceptRequest);
            acceptThread.IsBackground = true;
            acceptThread.Start(socketWatch);
            Console.ReadLine();
        }

        //参数声明为object类型是因为把带参函数给线程时，要求参数必须是object的
        private static void AcceptRequest(object socket)
        {
            Socket socketWatch = socket as Socket;
            //while true是为了能一直监听多个请求，否则一个请求成功就会停止监听
            while (true)
            {
                //socketWatch.Accept返回负责跟客户端通信的Socket——socketSend
                //在while (true)里即为每一个客户端都创建一个与之通信的Socket
                Socket socketSend = socketWatch.Accept();
                //127.0.0.1：连接成功
                Console.WriteLine(socketSend.RemoteEndPoint.ToString() + ": 连接成功");

                Thread receiveThread = new Thread(ReceiveRequest);
                receiveThread.IsBackground = true;
                receiveThread.Start(socketSend);
            }
        }

        //循环接受客户端请求
        private static void ReceiveRequest(object socket)
        {
            Socket socketSend = socket as Socket;
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 2];
                int realReceived = socketSend.Receive(buffer);
                //如果实际接受为0则对方已退出本次Socket连接，不再循环接受请求
                if (realReceived == 0)
                {
                    break;
                }
                //注意GetString(buffer, 0, realReceived)
                string msg = Encoding.UTF8.GetString(buffer, 0, realReceived);
                Console.WriteLine(socketSend.RemoteEndPoint.ToString() + ": " + msg);
            }
        }
    }
}
