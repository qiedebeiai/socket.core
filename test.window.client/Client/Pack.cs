
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
        public Pack(int receiveBufferSize, string ip, int port, uint headerFlag)
        {
            client = new TcpPackClient(receiveBufferSize, headerFlag);
            client.OnConnect += Client_OnConnect;
            client.OnReceive += Client_OnReceive;
            client.OnSend += Client_OnSend;
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

        private void Client_OnConnect(bool obj)
        {
            Console.WriteLine($"pack连接{obj}");
        }

        public void Send(byte[] data, int offset, int length)
        {
            client.Send(data, offset, length);

        }

        private void Client_OnSend(int obj)
        {
            Console.WriteLine($"pack已发送长度{obj}");
        }

        public void Close()
        {
            client.Close();
        }

    }
}
