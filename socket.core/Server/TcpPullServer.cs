using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace socket.core.Server
{
    /// <summary>
    /// pull 组件接收到数据时会触发监听事件对象OnReceive(pSender,dwConnID,iTotalLength)，告诉应用程序当前已经接收到了多少数据，应用程序检查数据的长度，如果满足则调用组件的Fetch(dwConnID,pData,iDataLength)方法，把需要的数据“拉”出来
    /// </summary>
    public class TcpPullServer 
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
        public event Action<Guid, int> OnReceive;
        /// <summary>
        /// 断开连接通知事件
        /// </summary>
        public event Action<Guid> OnClose;
        /// <summary>
        /// 接收到的数据缓存
        /// </summary>
        private Dictionary<Guid, List<byte>> queue;

        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        public TcpPullServer(int numConnections, int receiveBufferSize, int overtime)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                queue = new Dictionary<Guid, List<byte>>();
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
            while (tcpServer == null)
            {
                Thread.Sleep(2);
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
            {
                if (!queue.ContainsKey(connectId))
                {
                    queue.Add(connectId, new List<byte>());
                }
                queue[connectId].AddRange(data);
                OnReceive(connectId, queue.Count);
            }
        }

        /// <summary>
        /// 获取已经接收到的长度
        /// </summary>
        /// <param name="connectId"></param>
        /// <returns></returns>
        public int GetLength(Guid connectId)
        {
            if (!queue.ContainsKey(connectId))
            {
                return 0;
            }
            return queue[connectId].Count;
        }

        /// <summary>
        /// 取出指定长度数据
        /// </summary>
        /// <param name="connectId"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] Fetch(Guid connectId, int length)
        {
            if (!queue.ContainsKey(connectId))
            {
                return new byte[] { };
            }
            if (length > queue[connectId].Count)
            {
                length = queue[connectId].Count;
            }
            byte[] f = queue[connectId].Take(length).ToArray();
            queue[connectId].RemoveRange(0, length);
            return f;
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
            if(queue.ContainsKey(connectId))
            {
                queue.Remove(connectId);
            }
            if (OnClose != null)
                OnClose(connectId);
        }

       
        
    }
}
