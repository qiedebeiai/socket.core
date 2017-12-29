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
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        Pack(int numConnections, int receiveBufferSize, int overtime)
        {
            TcpPackServer server = new TcpPackServer(numConnections, receiveBufferSize, overtime,0x3ff);
            server.OnAccept += Server_OnAccept;
            server.OnReceive += Server_OnReceive;
            server.OnClose += Server_OnClose;
        }

        private void Server_OnReceive(Guid arg1, byte[] arg2)
        {
            throw new NotImplementedException();
        }

        private void Server_OnClose(Guid obj)
        {
            throw new NotImplementedException();
        }

        private void Server_OnAccept(Guid obj)
        {
            throw new NotImplementedException();
        }
    }
}
