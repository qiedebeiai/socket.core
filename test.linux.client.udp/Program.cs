using socket.core.Client;
using System;
using System.Net;


namespace test.linux.client.udp
{
    class Program
    {
        static UdpClients udpclient;
        static void Main(string[] args)
        {
            udpclient = new UdpClients(1024);
            udpclient.Start("127.0.0.1", 6666);
            udpclient.OnReceive += UdpServer_OnReceive;
            udpclient.OnSend += UdpServer_OnSend;
            for (int i = 0; i < 10; i++)
            {
                udpclient.Send(new byte[] { 65, 96 }, 0, 2);
            }
            Console.Read();
        }

        private static void UdpServer_OnSend(int arg2)
        {
            Console.WriteLine("发送长度：" + arg2);
        }

        private static void UdpServer_OnReceive(byte[] arg2, int arg3, int arg4)
        {
            Console.WriteLine("接收长度：" + arg4);
            //udpServer.Send( arg2, arg3, arg4);
        }
    }
}
