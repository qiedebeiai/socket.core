
using socket.core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test.window.client.Client
{
    public class Pull
    {
        TcpPullClient client;
        public Pull(int receiveBufferSize, string ip, int port)
        {
            client = new TcpPullClient(receiveBufferSize);
            client.OnConnect += Client_OnConnect;
            client.OnReceive += Client_OnReceive;
            client.OnSend += Client_OnSend;
            client.OnClose += Client_OnClose;
            client.Connect(ip, port);
        }

        private void Client_OnReceive(int obj)
        {
            byte[] data=client.Fetch(obj);
            Console.WriteLine($"pull接收byte[{data.Length}]");
        }

        private void Client_OnClose()
        {
            Console.WriteLine($"pull断开");
        }

        private void Client_OnConnect(bool obj)
        {
            Console.WriteLine($"pull连接{obj}");
        }

        public void Send(byte[] data, int offset, int length)
        {
            client.Send(data, offset, length);
        }

        private void Client_OnSend(int obj)
        {
            Console.WriteLine($"Push已发送长度{obj}");
        }

        public void Close()
        {
            client.Close();
        }

    }
}
