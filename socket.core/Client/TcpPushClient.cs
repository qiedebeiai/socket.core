using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace socket.core.Client
{
    /// <summary>
    /// push 
    /// </summary>
    public class TcpPushClient
    {
        /// <summary>
        /// 基础类
        /// </summary>
        private TcpClients tcpClients;
        /// <summary>
        /// 连接成功事件
        /// </summary>
        public event Action<bool> OnAccept;
        /// <summary>
        /// 接收通知事件
        /// </summary>
        public event Action<byte[]> OnReceive;
        /// <summary>
        /// 断开连接通知事件
        /// </summary>
        public event Action OnClose;

        /// <summary>
        /// 设置基本配置
        /// </summary>       
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>     
        public TcpPushClient(int receiveBufferSize)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                tcpClients = new TcpClients( receiveBufferSize);
                tcpClients.OnAccept += TcpServer_eventactionAccept;
                tcpClients.OnReceive += TcpServer_eventactionReceive;
                tcpClients.OnClose += TcpServer_eventClose;
            }));
            thread.Start();
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">ip地址或域名</param>
        /// <param name="port">端口</param>
        public void Connect(string ip,int port)
        {
            while (tcpClients == null)
            {
                Thread.Sleep(10);
            }
            tcpClients.Connect(ip,port);
        }

        /// <summary>
        /// 连接成功事件方法
        /// </summary>
        /// <param name="success">是否成功连接</param>
        private void TcpServer_eventactionAccept(bool success)
        {
            if (OnAccept != null)
                OnAccept(success);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(byte[] data, int offset, int length)
        {
            tcpClients.Send(data, offset, length);
        }

        /// <summary>
        /// 接收通知事件方法
        /// </summary>
        /// <param name="data"></param>
        private void TcpServer_eventactionReceive( byte[] data)
        {
            if (OnReceive != null)
                OnReceive(data);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            tcpClients.Close();
        }

        /// <summary>
        /// 断开连接通知事件方法
        /// </summary>
        private void TcpServer_eventClose()
        {
            if (OnClose != null)
                OnClose();
        }
    }
}
