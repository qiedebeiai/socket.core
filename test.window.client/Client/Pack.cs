
using socket.core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test.window.client.Client
{
    public class Pack
    {
        TcpPackClient client;
        public Pack(int receiveBufferSize, string ip, int port,int headerFlag)
        {
            client = new TcpPackClient(receiveBufferSize, headerFlag);
            client.OnAccept += Client_OnAccept;
            client.OnReceive += Client_OnReceive;
            client.OnClose += Client_OnClose;
            client.Connect(ip, port);
        }

        private void Client_OnClose()
        {
            Console.WriteLine($"pack断开");
        }

        private void Client_OnReceive(byte[] obj)
        {
            Console.WriteLine($"pack接收byte[{obj.Length}]");
        }

        private void Client_OnAccept(bool obj)
        {
            Console.WriteLine($"pack连接{obj}");
        }

        public void Send(byte[] data, int offset, int length)
        {
            client.Send(data, offset, length);
            Console.WriteLine($"pack发送byte[{length}]");
        }

        public void Close()
        {
            client.Close();
        }

    }
}
