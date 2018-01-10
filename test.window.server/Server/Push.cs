
using socket.core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test.window.server.Server
{

    public class Push
    {
        TcpPushServer server;
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        /// <param name="port">端口</param>
        public Push(int numConnections, int receiveBufferSize, int overtime,int port)
        {
            server = new TcpPushServer(numConnections, receiveBufferSize, overtime);
            server.OnAccept += Server_OnAccept;
            server.OnReceive += Server_OnReceive;
            server.OnSend += Server_OnSend;
            server.OnClose += Server_OnClose;          
            server.Start(port);
        }

        private void Server_OnAccept(Guid obj)
        {
            server.SetAttached(obj, 555);
            Console.WriteLine($"Push已连接{obj}");
        }
        
        private void Server_OnSend(Guid arg1, int arg2)
        {
            Console.WriteLine($"Push已发送:{arg1} 长度:{arg2}");
        }

        private void Server_OnReceive(Guid arg1, byte[] arg2)
        {
            int aaa=server.GetAttached<int>(arg1);
            Console.WriteLine($"Push已接收:{arg1} 长度:{arg2.Length}");
            server.Send(arg1, arg2, 0, arg2.Length);
        }

        private void Server_OnClose(Guid obj)
        {
            int aaa = server.GetAttached<int>(obj);
            Console.WriteLine($"Push断开{obj}");
        }

    }
}
