
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
        TcpPushClient client;
        public Push(int receiveBufferSize,string ip,int port)
        {
            client = new TcpPushClient(receiveBufferSize);
            client.OnAccept += Client_OnAccept;
            client.OnReceive += Client_OnReceive;
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

        private void Client_OnAccept(bool obj)
        {
            Console.WriteLine($"Push连接{obj}");
        }

        public void Send(byte[] data,int offset,int length)
        {
            client.Send(data, offset, length);
            Console.WriteLine($"Push发送byte[{length}]");
        }

        public void Close()
        {
            client.Close();
        }
      
    }
}
