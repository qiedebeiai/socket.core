
using socket.core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.window.server.Server
{
    public class Pull
    {
        TcpPullServer server;
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        ///<param name = "port" > 端口 </ param >
        public Pull(int numConnections, int receiveBufferSize, int overtime,int port)
        {
            server = new TcpPullServer(numConnections, receiveBufferSize, overtime);
            server.OnAccept += Server_OnAccept;
            server.OnReceive += Server_OnReceive;
            server.OnClose += Server_OnClose;
            server.Start(port);
        }

        private void Server_OnReceive(Guid arg1, int arg2)
        {
            //Console.WriteLine($"pull接收 byte[{arg2}]");
            byte[] data=server.Fetch(arg1, arg2);
            server.Send(arg1, data, 0, data.Length);
            //Console.WriteLine($"pull发送 byte[{arg2}]");
            //System.Threading.Thread.Sleep(1000*100);
        }

        private void Server_OnClose(Guid obj)
        {
            //Console.WriteLine($"pull断开{obj}");
        }      

        private void Server_OnAccept(Guid obj)
        {
            //Console.WriteLine($"pull连接{obj}");

        }
    }
}
