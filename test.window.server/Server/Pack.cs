using socket.core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.window.server.Server
{
    public class Pack
    {
        TcpPackServer server;
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        /// <param name="port">端口</param>
        /// <param name="headerFlag">包头</param>
        public Pack(int numConnections, int receiveBufferSize, int overtime,int port,int headerFlag)
        {
            server = new TcpPackServer(numConnections, receiveBufferSize, overtime, headerFlag);
            server.OnAccept += Server_OnAccept;
            server.OnReceive += Server_OnReceive;
            server.OnClose += Server_OnClose;
            server.Start(port);
        }

        private void Server_OnReceive(Guid arg1, byte[] arg2)
        {
            Console.WriteLine($"Pack接收 byte[{arg2.Length}]");
            server.Send(arg1, arg2, 0, arg2.Length);
            Console.WriteLine($"Pack发送 byte[{arg2.Length}]");
        }

        private void Server_OnClose(Guid obj)
        {
            Console.WriteLine($"Pack断开{obj}");
        }

        private void Server_OnAccept(Guid obj)
        {
            Console.WriteLine($"Pack连接{obj}");
        }
    }
}
