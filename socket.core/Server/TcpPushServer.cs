using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace socket.core.Server
{
    /// <summary>
    /// push 
    /// </summary>
    public class TcpPushServer
    {
        /// <summary>
        /// 基础类
        /// </summary>
        private TcpServer tcpServer;
        /// <summary>
        /// 连接成功事件
        /// </summary>
        public event Action<Guid> OnAccept;
        /// <summary>
        /// 接收通知事件
        /// </summary>
        public event Action<Guid, byte[]> OnReceive;
        /// <summary>
        /// 断开连接通知事件
        /// </summary>
        public event Action<Guid> OnClose;


        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        public TcpPushServer(int numConnections, int receiveBufferSize, int overtime)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                tcpServer = new TcpServer(numConnections, receiveBufferSize, overtime);
                tcpServer.OnAccept += TcpServer_eventactionAccept;
                tcpServer.OnReceive += TcpServer_eventactionReceive;
                tcpServer.OnClose += TcpServer_eventClose;
            }));
            thread.Start();
        }

        /// <summary>
        /// 开启监听服务
        /// </summary>        
        /// <param name="port"></param>
        public void Start(int port)
        {
            while(tcpServer==null)
            {
                Thread.Sleep(10);
            }
            tcpServer.Start(port);
        }

        /// <summary>
        /// 连接成功事件方法
        /// </summary>
        /// <param name="connectId"></param>
        private void TcpServer_eventactionAccept(Guid connectId)
        {
            if (OnAccept != null)
                OnAccept(connectId);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connectId">连接ID</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(Guid connectId, byte[] data, int offset, int length)
        {
            tcpServer.Send(connectId, data, offset, length);
        }

        /// <summary>
        /// 接收通知事件方法
        /// </summary>
        /// <param name="connectId"></param>
        /// <param name="data"></param>
        private void TcpServer_eventactionReceive(Guid connectId, byte[] data)
        {
            if (OnReceive != null)
                OnReceive(connectId, data);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="guid">连接ID</param>
        public void Close(Guid connectId)
        {
            tcpServer.Close(connectId);
        }

        /// <summary>
        /// 断开连接通知事件方法
        /// </summary>
        /// <param name="connectId"></param>
        private void TcpServer_eventClose(Guid connectId)
        {
            if (OnClose != null)
                OnClose(connectId);
        }
    }
}
