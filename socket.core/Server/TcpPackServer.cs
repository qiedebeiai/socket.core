using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace socket.core.Server
{
    public class TcpPackServer
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
        /// 接收到的数据缓存
        /// </summary>
        private Dictionary<Guid, List<byte>> queue;
        /// <summary>
        /// 包头标记
        /// </summary>
        private int headerFlag;
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        /// <param name="headerFlag">包头标记范围0~1023(0x3FF),当包头标识等于0时，不校验包头</param>
        public TcpPackServer(int numConnections, int receiveBufferSize, int overtime,int headerFlag)
        {
            if(headerFlag<0|| headerFlag>1023)
            {
                headerFlag = 0;
            }
            this.headerFlag = headerFlag;
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
            data = AddHead(data.Skip(offset).Take(length).ToArray());
            tcpServer.Send(connectId, data, 0, data.Length);
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
                byte[] datas=Read(connectId);
                if(datas != null&& datas.Length>0)
                {
                    OnReceive(connectId, datas);
                }
            }
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
            if (queue.ContainsKey(connectId))
            {
                queue.Remove(connectId);
            }
            if (OnClose != null)
                OnClose(connectId);
        }

        /// <summary>
        /// 在数据起始位置增加4字节包头
        /// </summary>
        /// <param name="headflag"></param>
        /// <param name="length"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] AddHead(byte[] data)
        {
            int len = data.Length;
            int header = (headerFlag << 22) | len;
            byte[] head=System.BitConverter.GetBytes(header);
            return head.Concat(data).ToArray();           
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        private byte[] Read(Guid connectId)
        {
            if (!queue.ContainsKey(connectId))
            {
                return null;
            }
            List<byte> data = queue[connectId];
            int header=BitConverter.ToInt32(data.ToArray(), 0);        
            if(headerFlag!= (header >> 22))
            {
                return null;
            }
            int len = header & 0x3fffff;
            if (len > data.Count-4)
            {
                return null;
            }
            byte[] f = data.Skip(4).Take(len).ToArray();
            queue[connectId].RemoveRange(0,len+4);
            return f;
        }


    }
}
