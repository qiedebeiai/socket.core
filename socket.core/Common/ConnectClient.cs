using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace socket.core.Common
{
    /// <summary>
    /// 已经连接的客户端
    /// </summary>
    class ConnectClient
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public Guid connectId { get; set; }
        /// <summary>
        /// 套接字
        /// </summary>
        public Socket socket{get;set;}
        /// <summary>
        /// 接受端SocketAsyncEventArgs对象
        /// </summary>
        public SocketAsyncEventArgs saea_receive { get; set; }
        /// <summary>
        /// 发送端SocketAsyncEventArgs对象
        /// </summary>
        public SocketAsyncEventArgs saea_send { get; set; }
        /// <summary>
        /// 每隔10秒扫描次数,用于检查客户端是否存活
        /// </summary>
        public int keep_alive { get; set; }
    }
}
