
using socket.core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test.window.client.Client
{
    public class Push
    {
        private TcpPushClient client;

        public Push(int receiveBufferSize,string ip,int port)
        {
            client = new TcpPushClient(receiveBufferSize);
            client.OnConnect += Client_OnConnect;
            client.OnReceive += Client_OnReceive;
            client.OnSend += Client_OnSend;
            client.OnClose += Client_OnClose;          
            client.Connect(ip, port);
        }

        private void Client_OnClose()
        {
            Console.WriteLine($"Push断开");
        }

        private void Client_OnReceive(byte[] obj)
        {
            Console.WriteLine($"Push接收byte[{obj.Length}]");
        }

        private void Client_OnConnect(bool obj)
        {
            Console.WriteLine($"Push连接{obj}");
        }

        private void Client_OnSend(int obj)
        {
            Console.WriteLine($"Push已发送长度{obj}");
        }

        public void Send(byte[] data,int offset,int length)
        {
            client.Send(data, offset, length);
        }

        public void Close()
        {
            client.Close();
        }
      
    }
}
