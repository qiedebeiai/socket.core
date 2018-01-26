using socket.core.Server;
using System;
using System.Net;

namespace test.linux.server.udp
{
    class Program
    {
        static UdpServer udpServer;
        static void Main(string[] args)
        {
            udpServer = new UdpServer(1024);
            udpServer.Start(6666);
            udpServer.OnReceive += UdpServer_OnReceive;
            udpServer.OnSend += UdpServer_OnSend;
            Console.WriteLine("Hello World!");
            Console.Read(); 
        }

        private static void UdpServer_OnSend(EndPoint arg1, int arg2)
        {
            Console.WriteLine("发送长度："+arg2);
        }

        private static void UdpServer_OnReceive(EndPoint arg1, byte[] arg2, int arg3, int arg4)
        {
            Console.WriteLine("接收长度："+arg4);
            udpServer.Send(arg1, arg2, arg3, arg4);
        }
    }
}
